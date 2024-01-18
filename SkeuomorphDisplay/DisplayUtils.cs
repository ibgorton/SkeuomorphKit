using System;
using System.Windows;
using System.Windows.Media;

namespace SkeuomorphDisplay
{
    static class DisplayUtils
    {
        

        public static Brush CreateLEDBrush(this Color color, int brightness)
        {
            RadialGradientBrush gradient = new()
            {
                GradientOrigin = new Point(x: 0.5, y: 0.5),
                Center = new Point(x: 0.5, y: 0.5)
            };

            GradientStop highlight = new();
            GradientStop primary = new();
            
            if (brightness > 0)
            {
                highlight.Color = ChangeColorBrightness(color: color, correctionFactor: brightness / 10d);
                highlight.Offset = 0.0;

                primary.Color = color;
                primary.Offset = 1;
            }
            else if (brightness == 0)
            {
                highlight.Color = primary.Color = color;
            }
            else if (brightness < 0)
            {
                highlight.Color = primary.Color = ChangeColorBrightness(color: color, correctionFactor: brightness / 10d);
            }
            gradient.GradientStops.Add(value: highlight);
            gradient.GradientStops.Add(value: primary);
            return gradient;
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1.
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        private static Color ChangeColorBrightness(Color color, double correctionFactor)
        {
            double red = color.R;
            double green = color.G;
            double blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            return Color.FromArgb(a: color.A, r: Convert.ToByte(value: red), g: Convert.ToByte(value: green), b: Convert.ToByte(value: blue));
        }
    }
}
