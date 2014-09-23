using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MaskedTextBox.Interactivity;

namespace MaskedTextBox.Behaviors
{
    public class TextBoxInputMaskBehavior : Behavior<TextBox>
    {
        private string _lastValidText;

        public static readonly DependencyProperty ExpressionProperty = DependencyProperty.Register("Mask",
            typeof(string),
            typeof(TextBoxInputMaskBehavior),
            new PropertyMetadata(null));

        public string Mask
        {
            get { return (string)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.TextChanged += OnTextChanged;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= OnTextChanged;

            base.OnDetaching();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (AssociatedObject != null && sender != null && !string.IsNullOrEmpty(AssociatedObject.Text))
            {
                //The user may not enter more text then dictated by the MASK
                if (AssociatedObject.Text.Length > this.Mask.Length)
                    ResetText();
                else
                {
                    if (!string.IsNullOrEmpty(_lastValidText) && AssociatedObject.Text.Length == (_lastValidText.Length - 1))
                    {
                        //The user deleted chars, so we need to check if the current last char is a MASK fixed char and delete that as well!
                        int currentPosition = AssociatedObject.Text.Length - 1;
                        var currentChar = AssociatedObject.Text[currentPosition];
                        if (ValidateChar(currentChar, currentPosition, true))
                        {
                            var caretPosition = AssociatedObject.SelectionStart;
                            AssociatedObject.Text = AssociatedObject.Text.Remove(currentPosition);
                            AssociatedObject.SelectionStart = caretPosition - 1;
                        }

                        _lastValidText = AssociatedObject.Text;
                    }
                    else
                    {
                        //Check if the current char is correct in reference to the position in the MASK
                        int currentPosition = AssociatedObject.Text.Length - 1;
                        var currentChar = AssociatedObject.Text[currentPosition];

                        if (ValidateChar(currentChar, currentPosition))
                        {
                            //The current entry is correct - check MASK for fixed char and add it
                            string fixedMaskEntry = GetFixedMaskEntry(currentPosition);
                            if (!string.IsNullOrEmpty(fixedMaskEntry))
                            {
                                var caretPosition = AssociatedObject.SelectionStart;
                                AssociatedObject.Text = string.Concat(AssociatedObject.Text, fixedMaskEntry);
                                AssociatedObject.SelectionStart = caretPosition + 1;
                            }

                            _lastValidText = AssociatedObject.Text;
                        }
                        else
                            ResetText();
                    }
                }
            }
        }

        private void ResetText()
        {
            //Restore the last valid value.
            var caretPosition = AssociatedObject.SelectionStart;
            AssociatedObject.Text = _lastValidText ?? string.Empty;
            AssociatedObject.SelectionStart = caretPosition - 1;
        }

        private bool ValidateChar(char charToValidate, int position, bool isFixedMaskChar = false)
        {
            if (isFixedMaskChar)
            {
                if (!this.Mask[position].Equals('9') && !this.Mask[position].Equals('#') &&
                    charToValidate.ToString().Equals(this.Mask[position].ToString(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            else
            {
                //Validate if the given char is a numeric value
                if (this.Mask[position].Equals('9'))
                {
                    int result;
                    return int.TryParse(charToValidate.ToString(), out result);
                }

                //Validate if the given char is just any char
                if (this.Mask[position].Equals('#'))
                    return true;

                //Validate other chars
                if (!this.Mask[position].Equals('9') && !this.Mask[position].Equals('#'))
                    return charToValidate.ToString()
                        .Equals(this.Mask[position].ToString(), StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private string GetFixedMaskEntry(int position)
        {
            int nextPosition = position + 1;
            if ((nextPosition < this.Mask.Length) && !this.Mask[nextPosition].Equals('9') && !this.Mask[nextPosition].Equals('#'))
                return this.Mask[nextPosition].ToString();

            return null;
        }
    }
}
