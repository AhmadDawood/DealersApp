using System;
using System.Windows;
using System.Windows.Controls;

namespace Dealers
{
    class HelperRoutines
    {
        private static string title = "Data Entry Error";

        public static string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }
        //---------------------------------------- My RegEx based Validation Routines ---------------------------------------------
        public static bool IsNumber(TextBox textBox)
        {
            //RegEx based Input Validation routine to check Numbers.
            string sPattern = "^\\d+$";
            try
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, sPattern))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(textBox.Tag.ToString() + " must be Non-Empty or in a Number format: ", Title,
                        MessageBoxButton.OK, MessageBoxImage.Stop);
                    textBox.Focus();
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString(), "Dealers App::IsNumber Method",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
        }
        public static bool IsLetter(TextBox textBox)
        {
            //The Following Procedure used to Check that Given input in a textbox contains Letters only.
            string sPattern = @"^[a-zA-Z]+$";
            try
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, sPattern))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(textBox.Tag.ToString() + " must be Non Empty or in Letter format: ", Title,
                        MessageBoxButton.OK, MessageBoxImage.Stop);
                    textBox.Focus();
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString(), "Dealers App::IsLetter Method",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
        }
        public static bool IsCellNumber(TextBox textBox)
        {
            //This is Regex based Phone Number tester.
            string sPattern = "^\\d{10}$";
            try
            {
                //Todo: I think i must check that textbox is non-empty
                if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, sPattern))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(textBox.Tag.ToString() + " must be in this format: " +
                    "03XX-XXX-XXXX", Title, MessageBoxButton.OK, MessageBoxImage.Stop);
                    textBox.Focus();
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString(), "Dealers App::IsCellNumber Method",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;   
            }
        }
        //-------------------------------------------- Not used but are useful routines. --------------------
        public static  bool IsPresent(Control control)
        {
            
            if (control.GetType().ToString() == "System.Windows.Controls.TextBox")
            {
                TextBox textBox = (TextBox)control;
                int number;
                if (textBox.Text == null
                    ||textBox.Text.Length <= 0
                    || textBox.Text.Length <= 5
                    || textBox.Text.Length >= 25
                    || int.TryParse(textBox.Text, out number)
                    )
                {
                    MessageBox.Show(textBox.Tag.ToString() + " is a required field.", Title);
                    textBox.Focus();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (control.GetType().ToString() == "System.Windows.Controls.ComboBox")
            {
                ComboBox comboBox = (ComboBox)control;
                if (comboBox.SelectedIndex == -1)
                {
                    MessageBox.Show(comboBox.Tag.ToString() + " is a required field.", Title);
                    comboBox.Focus();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
        public static bool IsDigit(TextBox textBox)
        {   
            //    TextBox textBox = (TextBox)control;
            int number=0;
            if (textBox.Text == null
                || textBox.Text.Length <= 0
                || textBox.Text.Length >= 10
                || int.TryParse(textBox.Text, out number)
                )
            {
                MessageBox.Show(textBox.Tag.ToString() + " is a required field.", Title);
                textBox.Focus();
                return false;
            }
            else
            {
                try
                {
                    Convert.ToInt32(textBox.Text);
                    return true;
                }
                catch (FormatException)
                {
                    MessageBox.Show(textBox.Tag.ToString() + " must be an integer value.", Title);
                    textBox.Focus();
                    return false;
                }
            }        
        }
        public static bool IsInt32(TextBox textBox)
        {
            try
            {
                Convert.ToInt32(textBox.Text);
                return true;
            }
            catch (FormatException)
            {
                MessageBox.Show(textBox.Tag.ToString() + " must be an integer value.", Title);
                textBox.Focus();
                return false;
            }
        }
        public static bool IsPhoneNumber(TextBox textBox)
        {
            string phoneChars = textBox.Text.Replace(".", "");
            try
            {
                Convert.ToInt64(phoneChars);
                return true;
            }
            catch (FormatException)
            {
                MessageBox.Show(textBox.Tag.ToString() + " must be in this format: " +
                    "999.999.9999.", Title);
                textBox.Focus();
                return false;
            }
        }        
    }
}    
