using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.ComponentModel;

namespace WebControls.Extensions
{
    /// <summary>
    /// Asp webforms extension methods for simple finding and converting of values in server controls.
    /// </summary>
    /// <remarks>Asp webforms does not support proper 2-way binding or modelbinders. These methods can be used as an alternative for updating your model</remarks>
    public static class FindControlExtenstions
    {
        private const string NOT_FOUND = "Control with ID \"{0}\" not found in control tree below control with ID \"{1}\" of Type \"{2}\" ";
        private const string NOT_SUPPORTED = "Converting of values for Control of Type \"{0}\" for ID \"{1}\" not supported";
        private const string NO_VALUE = "Value of type \"{0}\" not set for Control with ID \"{1}\" of type \"{2}\"";

        /// <summary>
        /// Get the converted value for this control. 
        /// </summary>
        public static T Gval<T>(this Control control)
        {
            return control.Gval<T>(control.ID, int.MaxValue, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Searches the control's controls tree for a control with the specified ID, and gets it's converted value. 
        /// </summary>
        public static T Gval<T>(this Control control, string controlId)
        {
            return control.Gval<T>(controlId, int.MaxValue, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Searches the control's controls tree with a maximum recursion depth, for a control with the specified ID, and gets it's converted value.
        /// </summary>
        public static T Gval<T>(this Control control, string controlId, int maxRecursiveDepth)
        {
            return control.Gval<T>(controlId, maxRecursiveDepth, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Searches the control's controls tree for a control with the specified ID, and gets it's converted value, using the specified culture.
        /// </summary>
        public static T Gval<T>(this Control control, string controlId, CultureInfo culture)
        {
            return control.Gval<T>(controlId, int.MaxValue, culture);
        }

        /// <summary>
        /// Searches the control's controls tree with a maximum recursion depth for a control with the specified ID, 
        /// and gets it's converted value, using the specified culture.
        /// </summary>
        public static T Gval<T>(this Control control, string controlId, int maxRecursiveDepth, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(controlId))
                throw new ArgumentException(string.Format(NOT_FOUND, controlId, control.ID, control.GetType().Name));

            Control ctrl;
            if (control.ID == controlId)
                ctrl = control;
            else
            {
                IEnumerable<Control> controlList = new[] { control };
                int depth = 1;
                do
                {
                    controlList = controlList.SelectMany(c => c.Controls.Cast<Control>());
                    ctrl = controlList.SingleOrDefault(c => c.ID == controlId);
                    depth++;
                } while (ctrl == null && controlList.Any() && depth <= maxRecursiveDepth);
            }

            if (ctrl == null)
                throw new ArgumentException(string.Format(NOT_FOUND, controlId, control.ID, control.GetType().Name));

            var textControl = ctrl as IEditableTextControl;  // ListControls and TextBox
            if (textControl != null)
            {
                if (!string.IsNullOrEmpty(textControl.Text))
                    return ConvertFromString<T>(textControl.Text, culture);
                T defaultValue = default(T);
                if (defaultValue == null)
                    return defaultValue;
                throw new ArgumentException(string.Format(NO_VALUE, typeof(T).Name, controlId, ctrl.GetType().Name));
            }
            /*
            var jQueryPicker = ctrl as JQueryDatePicker;
            if (jQueryPicker != null)
            {
                if (jQueryPicker.SelectedDate.HasValue)
                    return ConvertFromString<T>(jQueryPicker.SelectedDate.ToString(), culture);
                return default(T);
            }
            */
            var checkBox = ctrl as CheckBox;
            if (checkBox != null)
                return ConvertFromString<T>(checkBox.Checked.ToString(), culture);

            var hiddenField = ctrl as HiddenField;
            if (hiddenField != null)
                return ConvertFromString<T>(hiddenField.Value, culture);

            if (ctrl != null)
                throw new NotSupportedException(string.Format(NOT_SUPPORTED, ctrl.GetType().Name, controlId));
            throw new NotImplementedException();
        }

        private static T ConvertFromString<T>(string value, CultureInfo culture)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
            if (tc == null)
                throw new NullReferenceException("TypeConverter");

            return (T)tc.ConvertFromString(null, culture, value);
        }
    }
}
