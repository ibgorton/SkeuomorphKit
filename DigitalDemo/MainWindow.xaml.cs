using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace DigitalDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private
        private readonly Stopwatch _sw = new Stopwatch();

        /// <summary>
        /// Class constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _numbers.Maximum = 9999999;
            _numbers.SetDigits(7);
            _numbers.SetDecimals(3);
            _upDown.Maximum = 9999999;
            _upDown.SetDigits(7);
            _upDown.SetDecimals(3);

            _toggle.Click += Toggle_Click;

            Loaded += (ob, ev) =>
            {
                
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

        /// <summary>
        /// Handles the button click event to pause and continue the running timer
        /// </summary>
        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (_sw.IsRunning)
                _sw.Stop();
            else
                _sw.Start();
        }
    }
}
