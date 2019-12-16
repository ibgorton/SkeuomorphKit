using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace DigitalDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Stopwatch _sw = new Stopwatch();
        public MainWindow()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            _upDown.Digits = 10;
            _upDown.Decimals = 10;
            _toggle.Click += Toggle_Click;
            
            Loaded += (ob, ev) =>
            {
                _upDown.Maximum = 9999999;
                _numbers.Maximum = 9999999;
                _upDown.IsConnected = true;
                _upDown.IsActive = true;
                _upDown.Digits = 7;
                _upDown.Decimals = 3;
                _numbers.IsConnected = true;
                _numbers.IsActive = true;
                _numbers.Digits = 7;
                _numbers.Decimals = 3;
                //_upDown.SetValue(100.123);
                _sw.Start();
                CompositionTarget.Rendering += (ob, ev) =>
                {
                    double seconds = _sw.Elapsed.TotalSeconds;
                    if (seconds < _numbers.Maximum)
                        _numbers.Input = Math.Round(seconds, 3);
                    else
                    {
                        _sw.Reset();
                        _numbers.Input = 0d;
                    }
                };
            };
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (_sw.IsRunning)
            {
                _sw.Stop();
            }
            else
            {
                _sw.Start();
            }
        }
    }
}
