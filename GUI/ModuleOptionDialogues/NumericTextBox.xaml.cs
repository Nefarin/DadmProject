using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EKG_Project.GUI.ModuleOptionDialogues
{
    public partial class NumericTextBox : TextBox
    {
        #region Dependency properties

        public double Number
        {
            get { return (double)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Number.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(double), typeof(NumericTextBox), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Get and set number in correct format
        /// </summary>
        public NumberStyles NumberType
        {
            get { return (NumberStyles)GetValue(NumberTypeProperty); }
            set { SetValue(NumberTypeProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for NumberType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NumberTypeProperty =
            DependencyProperty.Register("NumberType", typeof(NumberStyles), typeof(NumericTextBox), new PropertyMetadata(default(NumberStyles)));

        #endregion

        /// <summary>
        /// Set property and pasting handle of NumericTextBox
        /// </summary>
        public NumericTextBox() : base()
        {
            DataObject.AddPastingHandler(this, OnTextBoxPasting);
            this.PreviewTextInput += this.OnPreviewTextInput;
            this.GotFocus += this.TextBox_GotFocus;
            this.GotKeyboardFocus += this.TextBox_GotKeyboardFocus;
            this.GotMouseCapture += this.TextBox_GotMouseCapture;
            this.IsEnabledChanged += this.TextBox_IsEnabledChanged;
            this.TextChanged += this.TextBox_TextChanged;
        }

        /// <summary>
        /// Check if user typed an allowed number in current culture (dot or comma)
        /// </summary>
        /// <param name="text">Wrote by user in NumericTextBox field</param>
        /// <returns></returns>
        private bool IsTextAllowed(string text)
        {
            double number;
            bool result = double.TryParse(text, this.NumberType, CultureInfo.CurrentCulture, out number);
            if (result)
                this.Number = number;
            return result;
        }

        /// <summary>
        /// Can not allow user to type not a number char
        /// </summary>
        /// <param name="sender">Supports class in .NET, default param</param>
        /// <param name="e">Contains state information and event data associated with a routed event</param>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var builder = new StringBuilder(this.Text);

            if (string.IsNullOrEmpty(this.Text))
                builder.Append(e.Text);
            else if (this.SelectionLength == 0 && this.SelectionStart > 0)
                builder.Insert(this.SelectionStart, e.Text, 1);
            else if (this.SelectionLength > 0)
                builder.Replace(this.SelectedText, e.Text, this.SelectionStart, this.SelectionLength);

            e.Handled = !this.IsTextAllowed(builder.ToString());
        }

        /// <summary>
        /// Use the DataObject.Pasting Handler 
        /// </summary>
        private void OnTextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.SelectAllText();
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.SelectAllText();
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            this.SelectAllText();
        }

        private void SelectAllText()
        {
            Keyboard.Focus(this);
            this.SelectAll();
        }

        private void TextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled)
            {
                this.Text = this.Number.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                this.Text = string.Empty;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsTextAllowed(this.Text);
        }
    }
}
