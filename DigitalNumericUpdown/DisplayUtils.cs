using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DigitalNumericUpdown
{
	static class DisplayUtils
	{
		private static readonly Dictionary<char, ushort> RectangleMap = new Dictionary<char, ushort>
		{

		};

		private static readonly Dictionary<char, byte> SevenMap = new Dictionary<char, byte>
		{
			{'-', 64},
			{'0', 63},
			{'1', 6},
			{'2', 91},
			{'3', 79},
			{'4', 102},
			{'5', 109},
			{'6', 125},
			{'7', 7},
			{'8', 127},
			{'9', 111},
			{'=', 72},
			{'A', 119},
			{'B', 124},
			{'C', 57},
			{'D', 94},
			{'E', 121},
			{'F', 113},
			{'G', 61},
			{'H', 118},
			{'_', 8},
			{'r', 80},
			{'o', 92}
		};

		private static readonly Dictionary<char, ushort> SixteenMap = new Dictionary<char, ushort>
		{
			{' ', 0},
			{'!', 12},
			{'"', 516},
			{'(', 5120},
			{')', 16640},
			{'*', 65280},
			{'+', 43520},
			{',', 16384},
			{'-', 34816},
			{'/', 17408},
			{'0', 17663},
			{'1', 1036},
			{'2', 34935},
			{'3', 2111},
			{'4', 34956},
			{'5', 37043},
			{'6', 35067},
			{'7', 15},
			{'8', 35071},
			{'9', 35007},
			{':', 8704},
			{';', 16896},
			{'<', 37888},
			{'=', 34864},
			{'>', 18688},
			{'?', 10247},
			{'@', 2807},
			{'A', 35023},
			{'B', 10815},
			{'C', 243},
			{'D', 8767},
			{'E', 33011},
			{'F', 32963},
			{'G', 2299},
			{'H', 35020},
			{'I', 8755},
			{'J', 124},
			{'K', 38080},
			{'L', 240},
			{'M', 1484},
			{'N', 4556},
			{'O', 255},
			{'P', 35015},
			{'Q', 4351},
			{'R', 39111},
			{'S', 35003},
			{'T', 8707},
			{'U', 252},
			{'V', 17600},
			{'W', 20684},
			{'X', 21760},
			{'Y', 35004},
			{'Z', 17459},
			{'[', 8722},
			{'\\', 4352},
			{']', 8737},
			{'^', 20480},
			{'_', 48},
			{'`', 256},
			{'a', 41072},
			{'b', 41184},
			{'c', 32864},
			{'d', 10268},
			{'e', 49248},
			{'f', 43522},
			{'g', 41633},
			{'h', 41152},
			{'i', 8192},
			{'j', 8800},
			{'k', 13824},
			{'l', 192},
			{'m', 43080},
			{'n', 41024},
			{'o', 41056},
			{'p', 33473},
			{'q', 41601},
			{'r', 32832},
			{'s', 41121},
			{'t', 32992},
			{'u', 8288},
			{'v', 16448},
			{'w', 20552},
			{'x', 21760},
			{'y', 2588},
			{'z', 49184},
			{'{', 41490},
			{'|', 8704},
			{'}', 10785}
		};

		public static BitArray Rectangle5x7Bits(this char c) => RectangleMap.ContainsKey(c) ? ParseChar(c, RectangleMap) : new BitArray(16);

		public static BitArray SixteenSegmentBits(this char c) => SixteenMap.ContainsKey(c) ? ParseChar(c, SixteenMap) : new BitArray(16);

		public static BitArray SevenSegmentBits(this char c) => SevenMap.ContainsKey(c) ? ParseChar(c, SevenMap) : new BitArray(7);

		private static BitArray ParseChar(char c, Dictionary<char, byte> d)
		{
			return new BitArray(bytes: new byte[] { d[c] })
			{
				Length = 7
			};
		}

		private static BitArray ParseChar(char c, Dictionary<char, ushort> d)
		{
			return new BitArray(values: new int[] { d[c] })
			{
				Length = 16
			};
		}

		public static Brush CreateLEDBrush(this Color color, int brightness)
		{
			RadialGradientBrush gradient = new RadialGradientBrush
			{
				GradientOrigin = new Point(0.5, 0.5),
				Center = new Point(0.5, 0.5)
			};

			GradientStop highlight = new GradientStop();
			GradientStop primary = new GradientStop();
			
			if (brightness > 0)
			{
				highlight.Color = ChangeColorBrightness(color, brightness / 10d);
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
				highlight.Color = primary.Color = ChangeColorBrightness(color, brightness / 10d);
			}
			gradient.GradientStops.Add(highlight);
			gradient.GradientStops.Add(primary);
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
			return Color.FromArgb(color.A, Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
		}
	}
}
