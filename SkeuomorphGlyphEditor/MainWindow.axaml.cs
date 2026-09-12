using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using SkeuomorphCore;

namespace SkeuomorphGlyphEditor;

public partial class MainWindow : Window
{
    private readonly List<ToggleButton> _segmentButtons = new();
    private int _segmentCount;

    public MainWindow()
    {
        InitializeComponent();

        LayoutPicker.ItemsSource = DisplayCharacterProfiles.AllNames;
        LayoutPicker.SelectedIndex = 1;
        CharacterInput.Text = "A";
        LayoutPicker.SelectionChanged += (_, _) => RefreshLayout();

        Loaded += (_, _) => RefreshLayout();
    }

    private void LoadButton_Click(object? sender, RoutedEventArgs e)
    {
        RefreshLayout();
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        SaveCurrentCharacter();
    }

    private void RefreshLayout()
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var profile = DisplayCharacterProfiles.Get(selected);
        _segmentCount = profile.SegmentCount;

        BuildSegmentGrid(profile);
        BuildCharacterMap(profile.Name);
        var inputText = CharacterInput.Text ?? string.Empty;
        LoadCharacterIntoGrid(profile.Name, inputText.Length > 0 ? inputText[0].ToString() : "A");
        UpdateMaskText();
        UpdateSegmentLegend();
    }

    private void BuildCharacterMap(string layout)
    {
        CharacterMapPanel.Children.Clear();
        var characters = GetCharactersForLayout(layout);

        foreach (var character in characters)
        {
            var button = new Button
            {
                Content = character == ' ' ? "space" : character.ToString(),
                Width = 56,
                Height = 34,
                Margin = new Thickness(2),
                Tag = character,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };

            button.Click += (_, _) =>
            {
                CharacterInput.Text = character == ' ' ? " " : character.ToString();
                LoadCharacterIntoGrid(layout, character.ToString());
                UpdateMaskText();
            };

            CharacterMapPanel.Children.Add(button);
        }
    }

    private static IReadOnlyList<char> GetCharactersForLayout(string layout)
    {
        var candidates = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?.,:;+-/=\\_[](){}<>|#@%&*'\"$^~".ToCharArray();
        var characters = new List<char>();

        foreach (var character in candidates)
        {
            var supported = DisplayCharacterProfiles.IsSupported(layout, character);

            if (supported && !characters.Contains(character))
            {
                characters.Add(character);
            }
        }

        return characters;
    }

    private void BuildSegmentGrid(IDisplayProfile profile)
    {
        SegmentGrid.Children.Clear();
        SegmentGrid.RowDefinitions.Clear();
        SegmentGrid.ColumnDefinitions.Clear();

        _segmentButtons.Clear();

        switch (profile)
        {
            case SevenSegmentDisplayProfile:
            {
                var canvas = new Canvas
                {
                    Width = 220,
                    Height = 240,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var segments = new[]
                {
                    new SegmentSpec(0, 60, 18, 80, 12, 0),
                    new SegmentSpec(1, 152, 52, 12, 80, 90),
                    new SegmentSpec(2, 152, 146, 12, 80, 90),
                    new SegmentSpec(3, 60, 214, 80, 12, 0),
                    new SegmentSpec(4, 24, 146, 12, 80, 90),
                    new SegmentSpec(5, 24, 52, 12, 80, 90),
                    new SegmentSpec(6, 60, 112, 80, 12, 0)
                };

                foreach (var segment in segments)
                {
                    var button = CreateSegmentButton(segment.Index, segment.Width, segment.Height, segment.X, segment.Y, segment.Angle);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }
            case FourteenSegmentDisplayProfile:
            {
                var canvas = new Canvas
                {
                    Width = 260,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var anchor = new[]
                {
                    new Point(70, 35), new Point(130, 35), new Point(190, 35),
                    new Point(70, 130), new Point(130, 130), new Point(190, 130),
                    new Point(70, 225), new Point(130, 225), new Point(190, 225)
                };

                // Real 14-seg parts omit the I and O center segments, leaving the standard
                // A/B/C/D/E/F/G/H/J/K/L/M/N/P set used by the Lite-On datasheet.
                var segments = new[]
                {
                    new SegmentSpec(0, anchor[0], anchor[1], 12),
                    new SegmentSpec(1, anchor[1], anchor[2], 12),
                    new SegmentSpec(2, anchor[2], anchor[5], 12),
                    new SegmentSpec(3, anchor[5], anchor[8], 12),
                    new SegmentSpec(4, anchor[7], anchor[8], 12),
                    new SegmentSpec(5, anchor[6], anchor[7], 12),
                    new SegmentSpec(6, anchor[3], anchor[6], 12),
                    new SegmentSpec(7, anchor[0], anchor[3], 12),
                    new SegmentSpec(8, anchor[0], anchor[4], 12),
                    new SegmentSpec(9, anchor[1], anchor[4], 12),
                    new SegmentSpec(10, anchor[4], anchor[2], 12),
                    new SegmentSpec(11, anchor[4], anchor[5], 12),
                    new SegmentSpec(12, anchor[4], anchor[8], 12),
                    new SegmentSpec(13, anchor[7], anchor[4], 12),
                    new SegmentSpec(14, anchor[6], anchor[4], 12),
                    new SegmentSpec(15, anchor[3], anchor[4], 12)
                };

                var mapping = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 15 };
                foreach (var sourceIndex in mapping)
                {
                    var button = CreateSegmentButton(segments[sourceIndex]);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }
            case SixteenSegmentDisplayProfile:
            {
                var canvas = new Canvas
                {
                    Width = 260,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // 3x3 anchor lattice: each segment must touch exactly two adjacent reference points.
                var anchor = new[]
                {
                    new Point(70, 35), new Point(130, 35), new Point(190, 35),
                    new Point(70, 130), new Point(130, 130), new Point(190, 130),
                    new Point(70, 225), new Point(130, 225), new Point(190, 225)
                };

                // Keep the segment numbering aligned to the actual 16-seg lattice:
                // 0..7 are the outer perimeter; 8..15 are the center-to-corner and center-cross segments.
                var segments = new[]
                {
                    new SegmentSpec(0, anchor[0], anchor[1], 12),
                    new SegmentSpec(1, anchor[1], anchor[2], 12),
                    new SegmentSpec(2, anchor[2], anchor[5], 12),
                    new SegmentSpec(3, anchor[5], anchor[8], 12),
                    new SegmentSpec(4, anchor[7], anchor[8], 12),
                    new SegmentSpec(5, anchor[6], anchor[7], 12),
                    new SegmentSpec(6, anchor[3], anchor[6], 12),
                    new SegmentSpec(7, anchor[0], anchor[3], 12),
                    new SegmentSpec(8, anchor[0], anchor[4], 12),
                    new SegmentSpec(9, anchor[1], anchor[4], 12),
                    new SegmentSpec(10, anchor[4], anchor[2], 12),
                    new SegmentSpec(11, anchor[4], anchor[5], 12),
                    new SegmentSpec(12, anchor[4], anchor[8], 12),
                    new SegmentSpec(13, anchor[7], anchor[4], 12),
                    new SegmentSpec(14, anchor[6], anchor[4], 12),
                    new SegmentSpec(15, anchor[3], anchor[4], 12)
                };

                foreach (var segment in segments)
                {
                    var button = CreateSegmentButton(segment);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }
            default:
                break;
        }

        var rows = profile.Height;
        var columns = profile.Width;

        for (var x = 0; x < columns; x++)
        {
            SegmentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        for (var y = 0; y < rows; y++)
        {
            SegmentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        }

        for (var index = 0; index < _segmentCount; index++)
        {
            var row = index / columns;
            var column = index % columns;
            var button = new ToggleButton
            {
                Width = 28,
                Height = 28,
                IsChecked = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4),
                Tag = index,
                Content = index.ToString(),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };

            button.Click += SegmentButton_Click;
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            SegmentGrid.Children.Add(button);
            _segmentButtons.Add(button);
        }
    }

    private void SegmentButton_Click(object? sender, RoutedEventArgs e)
    {
        UpdateMaskText();
    }

    private ToggleButton CreateSegmentButton(int index, double width, double height, double x, double y, double angle)
    {
        var segment = new SegmentSpec(index, x, y, width, height, angle);
        return CreateSegmentButton(segment);
    }

    private ToggleButton CreateSegmentButton(SegmentSpec segment)
    {
        var geometry = CreateSegmentGeometry(segment);
        var clipGeometry = geometry.Clone();

        var body = new Avalonia.Controls.Shapes.Path
        {
            Width = Math.Max(1, segment.Width + segment.Thickness),
            Height = Math.Max(1, segment.Height + segment.Thickness),
            Fill = new SolidColorBrush(Color.FromRgb(255, 82, 82)),
            Stroke = Brushes.Transparent,
            StrokeThickness = 0,
            Stretch = Stretch.Fill,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsHitTestVisible = false,
            Data = geometry,
            RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Absolute)
        };

        var button = new ToggleButton
        {
            Width = Math.Max(1, segment.Width + segment.Thickness),
            Height = Math.Max(1, segment.Height + segment.Thickness),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            Tag = segment.Index,
            Content = body,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            IsChecked = false,
            Clip = clipGeometry,
            RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
        };

        button.IsCheckedChanged += (_, _) => UpdateSegmentButtonVisual(button);
        button.Click += SegmentButton_Click;

        Canvas.SetLeft(button, segment.X);
        Canvas.SetTop(button, segment.Y);
        UpdateSegmentButtonVisual(button);
        return button;
    }

    private static Geometry CreateSegmentGeometry(SegmentSpec segment)
    {
        var halfThickness = segment.Thickness / 2.0;
        var dx = segment.End.X - segment.Start.X;
        var dy = segment.End.Y - segment.Start.Y;
        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length < 0.001)
        {
            return new RectangleGeometry
            {
                Rect = new Rect(0, 0, segment.Thickness, segment.Thickness),
                RadiusX = 2,
                RadiusY = 2
            };
        }

        var nx = -dy / length;
        var ny = dx / length;
        var originX = segment.X;
        var originY = segment.Y;

        var p1 = new Point(segment.Start.X - originX + nx * halfThickness, segment.Start.Y - originY + ny * halfThickness);
        var p2 = new Point(segment.End.X - originX + nx * halfThickness, segment.End.Y - originY + ny * halfThickness);
        var p3 = new Point(segment.End.X - originX - nx * halfThickness, segment.End.Y - originY - ny * halfThickness);
        var p4 = new Point(segment.Start.X - originX - nx * halfThickness, segment.Start.Y - originY - ny * halfThickness);

        var geometry = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = p1,
            IsClosed = true,
            IsFilled = true
        };
        figure.Segments!.Add(new LineSegment { Point = p2 });
        figure.Segments!.Add(new LineSegment { Point = p3 });
        figure.Segments!.Add(new LineSegment { Point = p4 });
        geometry.Figures!.Add(figure);
        return geometry;
    }

    private static void UpdateSegmentButtonVisual(ToggleButton button)
    {
        var path = button.Content as Avalonia.Controls.Shapes.Path;
        if (path is null)
        {
            return;
        }

        path.Fill = button.IsChecked == true
            ? new SolidColorBrush(Color.FromRgb(255, 82, 82))
            : new SolidColorBrush(Color.FromRgb(120, 76, 76));
        path.Opacity = button.IsChecked == true ? 1.0 : 0.35;
    }

    private readonly record struct SegmentSpec
    {
        public SegmentSpec(int index, Point start, Point end, double thickness)
        {
            Index = index;
            Start = start;
            End = end;
            Thickness = thickness;
            Angle = 0;
        }

        public SegmentSpec(int index, double x, double y, double width, double height, double angle)
        {
            Index = index;
            Angle = angle;

            var isVertical = Math.Abs((angle % 180.0) - 90.0) < 45.0 || Math.Abs((angle % 180.0) + 90.0) < 45.0;
            if (isVertical)
            {
                Start = new Point(x, y);
                End = new Point(x, y + height);
                Thickness = width;
            }
            else
            {
                Start = new Point(x, y);
                End = new Point(x + width, y);
                Thickness = height;
            }
        }

        public int Index { get; }
        public Point Start { get; }
        public Point End { get; }
        public double Thickness { get; }
        public double Angle { get; }

        public double Width => Math.Max(Math.Abs(End.X - Start.X), Thickness);
        public double Height => Math.Max(Math.Abs(End.Y - Start.Y), Thickness);
        public double X => Math.Min(Start.X, End.X) - (Thickness / 2.0);
        public double Y => Math.Min(Start.Y, End.Y) - (Thickness / 2.0);
    }

    private void LoadCharacterIntoGrid(string layout, string rawText)
    {
        var profile = DisplayCharacterProfiles.Get(layout);
        var character = rawText.Length > 0 ? rawText[0] : 'A';
        var bits = profile.GetBits(character);

        for (var index = 0; index < _segmentButtons.Count; index++)
        {
            var button = _segmentButtons[index];
            button.IsChecked = index < bits.Length && bits[index];
        }

        UpdateMaskText();
    }

    private void UpdateMaskText()
    {
        var selected = LayoutPicker.SelectedItem as string ?? "Rectangle5x7";
        ulong mask = 0;
        for (var index = 0; index < _segmentButtons.Count; index++)
        {
            if (_segmentButtons[index].IsChecked == true)
            {
                mask |= 1UL << index;
            }
        }

        var profile = DisplayCharacterProfiles.Get(selected);
        var characterText = CharacterInput.Text ?? string.Empty;
        var character = characterText.Length > 0 ? characterText : "A";
        var text = character[0];
        var width = profile.Width;
        var height = profile.Height;
        var bits = string.Join(", ", GetCurrentBits().Select(v => v ? "true" : "false"));

        MaskValueText.Text = $"{text} :: mask = {mask} (0x{mask:X})";
        CodePreview.Text = $"namespace SkeuomorphCore;\n\npublic static class GeneratedGlyphMaps\n{{\n    public static readonly CharacterMap {selected}Map = new({width}, {height});\n\n    static GeneratedGlyphMaps()\n    {{\n        {selected}Map.Set('{text}', {mask}UL);\n    }}\n}}\n\n// bits: [{bits}]";
        UpdateSegmentLegend();
    }

    private void UpdateSegmentLegend()
    {
        if (LayoutPicker.SelectedItem as string != "SixteenSegment")
        {
            SegmentLegendText.Text = string.Empty;
            return;
        }

        var names = new[]
        {
            "top-left",
            "top-right",
            "upper-right",
            "lower-right",
            "bottom-right",
            "bottom-left",
            "left-bottom",
            "left-top",
            "left-upper-diagonal",
            "middle-top-vertical",
            "right-upper-diagonal",
            "middle-horizontal-right",
            "right-lower-diagonal",
            "middle-bottom-vertical",
            "left-lower-diagonal",
            "middle-horizontal-left"
        };

        var lit = new List<string>();
        for (var i = 0; i < _segmentButtons.Count; i++)
        {
            if (_segmentButtons[i].IsChecked == true)
            {
                lit.Add($"{i + 1}={names[i]}");
            }
        }

        SegmentLegendText.Text = lit.Count == 0
            ? "Lit segments: none"
            : "Lit segments: " + string.Join(" | ", lit);
    }

    private IReadOnlyList<bool> GetCurrentBits()
    {
        var output = new List<bool>();
        for (var index = 0; index < _segmentButtons.Count; index++)
        {
            output.Add(_segmentButtons[index].IsChecked == true);
        }

        return output;
    }

    private void SaveCurrentCharacter()
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var profile = DisplayCharacterProfiles.Get(selected);
        var characterText = CharacterInput.Text ?? string.Empty;
        if (string.IsNullOrEmpty(characterText))
        {
            return;
        }

        var character = characterText[0].ToString();
        var mapName = profile.Name + "Map";
        var width = profile.Width;
        var height = profile.Height;

        var mask = 0UL;
        for (var index = 0; index < _segmentButtons.Count; index++)
        {
            if (_segmentButtons[index].IsChecked == true)
            {
                mask |= 1UL << index;
            }
        }

       var repoRoot = ResolveRepositoryRoot();
       var coreDirectory = System.IO.Path.Combine(repoRoot, "SkeuomorphCore");
       Directory.CreateDirectory(coreDirectory);

       var filePath = System.IO.Path.Combine(coreDirectory, "GeneratedGlyphMaps.cs");

       var classBody = $@"namespace SkeuomorphCore;
 
public static class GeneratedGlyphMaps
{{
    public static readonly CharacterMap {mapName} = new({width}, {height});
 
    static GeneratedGlyphMaps()
    {{
        {mapName}.Set('{character[0]}', {mask}UL);
    }}
}}
";

       try
       {
           File.WriteAllText(filePath, classBody);
           CodePreview.Text = classBody;
           MaskValueText.Text = $"Saved {character[0]} :: mask = {mask} (0x{mask:X})";
       }
       catch (Exception ex)
       {
           CodePreview.Text = ex.Message;
           MaskValueText.Text = $"Save failed: {ex.Message}";
       }
   }

   private static string ResolveRepositoryRoot()
   {
       var dir = new DirectoryInfo(AppContext.BaseDirectory);
       while (dir is not null)
       {
           var solutionPath = System.IO.Path.Combine(dir.FullName, "SkeuomorphKit.sln");
           var corePath = System.IO.Path.Combine(dir.FullName, "SkeuomorphCore");
           if (File.Exists(solutionPath) && Directory.Exists(corePath))
           {
               return dir.FullName;
           }

           dir = dir.Parent;
       }

       return AppContext.BaseDirectory;
   }
}