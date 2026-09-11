using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SkeuomorphDisplay.Wpf
{
    /// <summary>
    /// Displays the current time using segmented modules for hours, minutes, and seconds.
    /// </summary>
    public partial class Clock : UserControl
    {
        private readonly DispatcherTimer _timer = new()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        public Clock()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _timer.Tick += SetTime;
            Unloaded += (_, _) => _timer.Stop();
            SetTime(this, EventArgs.Empty);
            _timer.Start();
        }

        private void SetTime(object? sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            char[] hourDigits = now.Hour.ToString().ToCharArray();
            char[] minuteDigits = now.Minute.ToString().ToCharArray();
            char[] secondDigits = now.Second.ToString().ToCharArray();
            if (hourDigits.Length == 2)
            {
                _moduleH_.SetChar(hourDigits[0]);
                _module_H.SetChar(hourDigits[1]);
            }
            else
            {
                _moduleH_.BlankModule();
                _module_H.SetChar(hourDigits[0]);
            }
            if (minuteDigits.Length == 2)
            {
                _moduleM_.SetChar(minuteDigits[0]);
                _module_M.SetChar(minuteDigits[1]);
            }
            else
            {
                _moduleM_.SetChar('0');
                _module_M.SetChar(minuteDigits[0]);
            }
            if (secondDigits.Length == 2)
            {
                _moduleS_.SetChar(secondDigits[0]);
                _module_S.SetChar(secondDigits[1]);
            }
            else
            {
                _module_S.SetChar('0');
                _module_S.SetChar(secondDigits[0]);
            }
        }
    }
}
