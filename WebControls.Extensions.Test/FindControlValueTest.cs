using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebControls.Extensions;

namespace MontfoortIT.WebControls.Test
{
    /// <summary>
    /// Summary description for FindControlValueTest
    /// </summary>
    [TestClass]
    public class FindControlValueTest
    {
        public TestContext TestContext { get; set; }

        public Control control;

        [TestInitialize()]
        public void TestInit()
        {
            control = CreateControlTree();
        }

        private Control CreateControlTree()
        {
            var page = new Page();
            var tb1 = new TextBox { ID = "TextBox1", Text = "Hallo" };
            var tb2 = new TextBox { ID = "TextBox2", Text = "" };
            var tb3 = new TextBox { ID = "TextBox3", Text = "1,89" };
            var tb4 = new TextBox { ID = "TextBox4", Text = "" };
            var tb5 = new TextBox { ID = "TextBox5" };
            var tb6 = new TextBox { ID = "TextBox6", Text = "123211,89" };
            var tb7 = new TextBox { ID = "TextBox7", Text = "juist" };
            var ddl1 = new DropDownList { ID = "Ddl1", };
            var ddl2 = new DropDownList { ID = "Ddl2", };
            var ddl3 = new DropDownList { ID = "Ddl3", };
            var ddl4 = new DropDownList { ID = "Ddl4", };
            var ddl5 = new DropDownList { ID = "Ddl5", };
            var rbl1 = new RadioButtonList { ID = "Rbl1" };
            //var dp1 = new JQueryDatePicker { ID = "dpc1", SelectedDate = new DateTime(2011, 02, 24) };
            var panel1 = new Panel { ID = "FirstLevelPanel" };
            var panel2 = new Panel { ID = "SecondLevelPanel" };
            var cb1 = new CheckBox { ID = "cb1", Checked = true };
            var btn1 = new Button { ID = "Button1", Text = "NotSupportedControl" };
            var hf1 = new HiddenField { ID = "HiddenField1", Value = "300" };

            ddl1.Items.Add(new ListItem("jawel", "1"));
            ddl1.Items.Add(new ListItem("nee", "3"));
            ddl1.SelectedValue = "3";

            ddl2.Items.Add(new ListItem("Links naar recht", ContentDirection.LeftToRight.ToString()));
            ddl2.Items.Add(new ListItem("Niet aangegevn", ContentDirection.NotSet.ToString()));
            ddl2.Items.Add(new ListItem("Rechts naar links", ContentDirection.RightToLeft.ToString()));
            ddl2.SelectedValue = ContentDirection.RightToLeft.ToString();

            ddl3.Items.Add(new ListItem("selecteer", string.Empty));
            ddl3.Items.Add(new ListItem("Links naar recht", ContentDirection.LeftToRight.ToString()));
            ddl3.Items.Add(new ListItem("Niet aangegevn", ContentDirection.NotSet.ToString()));
            ddl3.Items.Add(new ListItem("Rechts naar links", ContentDirection.RightToLeft.ToString()));
            ddl3.SelectedValue = string.Empty;

            ddl4.Items.Add(new ListItem("Links naar recht", "1"));
            ddl4.Items.Add(new ListItem("Niet aangegevn", "0"));
            ddl4.Items.Add(new ListItem("Rechts naar links", "2"));
            ddl4.SelectedValue = "2";

            ddl5.Items.Add(new ListItem("Links naar recht", ContentDirection.LeftToRight.ToString()));
            ddl5.Items.Add(new ListItem("Niet aangegevn", ContentDirection.NotSet.ToString()));
            ddl5.Items.Add(new ListItem("Rechts naar links", "foo"));
            ddl5.SelectedValue = "foo";

            rbl1.Items.Add(new ListItem("jawel", "1"));
            rbl1.Items.Add(new ListItem("nee", "2"));
            rbl1.SelectedValue = "2";

            page.Controls.Add(tb1);
            page.Controls.Add(tb2);
            page.Controls.Add(tb3);
            page.Controls.Add(tb4);
            page.Controls.Add(tb5);
            page.Controls.Add(tb6);
            page.Controls.Add(btn1);
            page.Controls.Add(ddl1);
            page.Controls.Add(ddl2);
            page.Controls.Add(ddl3);
            page.Controls.Add(ddl4);
            page.Controls.Add(ddl5);
            page.Controls.Add(rbl1);
            page.Controls.Add(panel1);
            panel1.Controls.Add(cb1);
            panel1.Controls.Add(panel2);
            panel2.Controls.Add(tb7);
            panel2.Controls.Add(hf1);

            return page;
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Get_For_Empty_Control_ID_Fail()
        {
            bool value = control.Gval<bool>("");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Get_For_Not_Excisting_Control_ID_Fail()
        {
            bool value = control.Gval<bool>("NotAControl");
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void Get_For_Not_Supported_Control_Type_Fail()
        {
            string value = control.Gval<string>("Button1");
        }

        [TestMethod]
        public void Get_Text_Value_From_TextBox()
        {
            string value = control.Gval<String>("TextBox1");

            Assert.AreEqual("Hallo", value);
        }

        [TestMethod]
        public void Get_Text_Value_From_Empty_TextBox()
        {
            string value = control.Gval<String>("TextBox2");

            Assert.IsNull(value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Get_Int_Value_From_Empty_TextBox_Fail()
        {
            int value = control.Gval<int>("TextBox5");
        }

        [TestMethod]
        public void Get_Decimal_Value_From_TextBox()
        {
            decimal value = control.Gval<decimal>("TextBox3");

            Assert.AreEqual(1.89m, value);
        }

        [TestMethod]
        public void Get_Nullable_Decimal_Value_From_TextBox()
        {
            decimal? value = control.Gval<decimal?>("TextBox3");

            Assert.AreEqual(1.89m, value);
        }

        [TestMethod]
        public void Get_Null_Decimal_Value_From_TextBox()
        {
            decimal? value = control.Gval<decimal?>("TextBox4");

            Assert.IsNull(value);
        }

        [TestMethod]
        public void Get_Selected_Int_Value_From_RadioButtonList()
        {
            int value = control.Gval<int>("Rbl1");

            Assert.AreEqual(2, value);
        }
        /*
        [TestMethod]
        public void Get_DateTime_Value_From_JQueryDatePicker()
        {
            DateTime value = control.Gval<DateTime>("dpc1");

            Assert.AreEqual(new DateTime(2011, 02, 24), value);
        }

        [TestMethod]
        public void Get_Null_DateTime_Value_From_JQueryDatePicker()
        {
            DateTime? value = control.Gval<DateTime?>("dpc2");

            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void Get_Nullable_DateTime_Value_From_JQueryDatePicker()
        {
            DateTime? value = control.Gval<DateTime?>("dpc3");

            Assert.AreEqual(new DateTime(2011, 2, 25), value);
        }
        */
        [TestMethod]
        public void Get_Recursive_Bool_Value_From_CheckBox()
        {
            bool value = control.Gval<bool>("cb1");

            Assert.IsTrue(value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Get_Recursive_Bool_Max_Depth_1_Fail()
        {
            bool value = control.Gval<bool>("cb1", 1);
        }

        [TestMethod]
        public void Get_Recursive_Bool_Max_Depth_2()
        {
            bool value = control.Gval<bool>("cb1", 2);

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void Get_Recursive_Text_With_Depth_Of_3()
        {
            string value = control.Gval<string>("TextBox7", 3);

            Assert.AreEqual("juist", value);
        }

        [TestMethod]
        public void Get_Double_Value_From_TextBox()
        {
            var tb = new TextBox { ID = "DoubleTextBox", Text = "1,89" };
            double value = tb.Gval<double>("DoubleTextBox");

            Assert.AreEqual(1.89d, value);
        }

        [TestMethod]
        public void Get_Direct_Double_Value_From_TextBox()
        {
            var tb = new TextBox { ID = "DoubleTextBox", Text = "1,89" };
            decimal value = tb.Gval<decimal>();
            Assert.AreEqual(1.89m, value);
        }

        [TestMethod]
        public void Get_Decimal_Value_With_Seperator_From_TextBox()
        {
            decimal value = control.Gval<decimal>("TextBox6");

            Assert.AreEqual(123211.89m, value);
        }

        [TestMethod]
        public void Get_Decimal_Value_With_English_Culture_From_TextBox()
        {
            var page = new Page();
            var tb1 = new TextBox { ID = "TextBox1", Text = "1.34" };
            page.Controls.Add(tb1);

            decimal value = page.Gval<decimal>("TextBox1", new CultureInfo("en-US"));

            Assert.AreEqual(1.34m, value);
        }

        [TestMethod]
        public void Get_Decimal_Value_With_Dutch_Culture_From_TextBox()
        {
            var page = new Page();
            var tb1 = new TextBox { ID = "TextBox1", Text = "1,34" };
            page.Controls.Add(tb1);

            decimal value = page.Gval<decimal>("TextBox1", new CultureInfo("nl-NL"));

            Assert.AreEqual(1.34m, value);
        }

        [TestMethod]
        public void Get_DateTime_Value_With_English_Culture_From_TextBox()
        {
            var page = new Page();
            var tb1 = new TextBox { ID = "TextBox1", Text = "2-13-1983" };
            page.Controls.Add(tb1);

            DateTime value = page.Gval<DateTime>("TextBox1", new CultureInfo("en-US"));

            Assert.AreEqual(new DateTime(1983, 2, 13), value);
        }

        [TestMethod]
        public void Get_DateTime_Value_With_Dutch_Culture_From_TextBox()
        {
            var page = new Page();
            var tb1 = new TextBox { ID = "TextBox1", Text = "13-2-1983" };
            page.Controls.Add(tb1);

            DateTime value = page.Gval<DateTime>("TextBox1", new CultureInfo("nl-NL"));

            Assert.AreEqual(new DateTime(1983, 2, 13), value);
        }

        [TestMethod]
        public void Get_Enum_Value_From_DropDownList()
        {
            ContentDirection direction = control.Gval<ContentDirection>("Ddl2");

            Assert.AreEqual(ContentDirection.RightToLeft, direction);
        }

        [TestMethod]
        public void Get_Nullable_Enum_Value_From_DropDownList()
        {
            ContentDirection? direction = control.Gval<ContentDirection?>("Ddl2");

            Assert.AreEqual(ContentDirection.RightToLeft, direction);
        }

        [TestMethod]
        public void Get_Nullable_Enum_Null_Value_From_DropDownList()
        {
            ContentDirection? direction = control.Gval<ContentDirection?>("Ddl3");

            Assert.IsNull(direction);
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void Get_Enum_Value_From_DropDownList_By_Undefined_Enum_Fail()
        {
            ContentDirection direction = control.Gval<ContentDirection>("Ddl5");
        }

        [TestMethod]
        public void Get_Int_Value_From_HiddenField()
        {
            int value = control.Gval<int>("HiddenField1");

            Assert.AreEqual(300, value);
        }
    }
}
