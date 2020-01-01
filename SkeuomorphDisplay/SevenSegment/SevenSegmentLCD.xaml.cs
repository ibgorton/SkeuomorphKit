using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DigitalNumericUpdown
{
    /// <summary>
    /// Interaction logic for SevenSegmentLCD.xaml
    /// </summary>
    public partial class SevenSegmentLCD : SevenSegmentBase
    {
        public SevenSegmentLCD() : base()
        {
            InitializeComponent();
            DecimalDisplayAngle = 0;
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
        }
    }
}
