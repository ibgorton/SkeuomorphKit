using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace DigitalNumericUpdown
{
    /// <summary>
    /// Interaction logic for Clock.xaml
    /// </summary>
    public partial class Clock : UserControl
    {
        public Clock()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            _module_H.ShowColon();
            _module_M.ShowColon();
            CompositionTarget.Rendering += SetTime;
        }

        private void SetTime(object? sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            byte[] hourDigits = now.Hour.ToString().Select(c => (byte)char.GetNumericValue(c)).ToArray();
            byte[] minuteDigits = now.Minute.ToString().Select(c => (byte)char.GetNumericValue(c)).ToArray();
            byte[] secondDigits = now.Second.ToString().Select(c => (byte)char.GetNumericValue(c)).ToArray();
            if (hourDigits.Length == 2)
            {
                _moduleH_.SetDigit(hourDigits[0]);
                _module_H.SetDigit(hourDigits[1]);
            }
            else
            {
                _moduleH_.SetDigit(0);
                _module_H.SetDigit(hourDigits[0]);
            }
            if (minuteDigits.Length == 2)
            {
                _moduleM_.SetDigit(minuteDigits[0]);
                _module_M.SetDigit(minuteDigits[1]);
            }
            else
            {
                _moduleM_.SetDigit(0);
                _module_M.SetDigit(minuteDigits[0]);
            }
            if (secondDigits.Length == 2)
            {
                _moduleS_.SetDigit(secondDigits[0]);
                _module_S.SetDigit(secondDigits[1]);
            }
            else
            {
                _moduleS_.SetDigit(0);
                _module_S.SetDigit(secondDigits[0]);
            }
        }
    }
}
