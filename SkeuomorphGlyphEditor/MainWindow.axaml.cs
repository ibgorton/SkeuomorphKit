using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using SkeuomorphCore;

namespace SkeuomorphGlyphEditor;

public partial class MainWindow : Window
{
    private readonly List<CheckBox> _segmentButtons = new();
    private int _segmentCount;

    public MainWindow()
    {
        InitializeComponent();

        LayoutPicker.ItemsSource = new[]
        {
            "SevenSegment",
            "Rectangle5x7",
            "SixteenSegment"
        };
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
        var selected = LayoutPicker.SelectedItem as string ?? "Rectangle5x7";
        _segmentCount = selected switch
        {
            "SevenSegment" => 7,
            "Rectangle5x7" => 35,
            "SixteenSegment" => 16,
            _ => 35
        };

        BuildSegmentGrid(selected);
        var inputText = CharacterInput.Text ?? string.Empty;
        LoadCharacterIntoGrid(selected, inputText.Trim());
        UpdateMaskText();
    }

    private void BuildSegmentGrid(string layout)
    {
        SegmentGrid.Children.Clear();
        SegmentGrid.RowDefinitions.Clear();
        SegmentGrid.ColumnDefinitions.Clear();

        var rows = layout switch
        {
            "SevenSegment" => 3,
            "Rectangle5x7" => 7,
            "SixteenSegment" => 4,
            _ => 7
        };
        var columns = layout switch
        {
            "SevenSegment" => 3,
            "Rectangle5x7" => 5,
            "SixteenSegment" => 4,
            _ => 5
        };

        for (var x = 0; x < columns; x++)
        {
            SegmentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        for (var y = 0; y < rows; y++)
        {
            SegmentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        }

        _segmentButtons.Clear();

        for (var index = 0; index < _segmentCount; index++)
        {
            var row = index / columns;
            var column = index % columns;
            var button = new CheckBox
            {
                Width = 28,
                Height = 28,
                IsChecked = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4),
                Tag = index,
                Content = index.ToString()
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

    private void LoadCharacterIntoGrid(string layout, string rawText)
    {
        var character = rawText.Length > 0 ? rawText[0] : 'A';
        var bits = layout switch
        {
            "SevenSegment" => SevenMap.GetBitsSeven(character),
            "Rectangle5x7" => GlyphLibrary.GetMatrixBits(character),
            "SixteenSegment" => SixteenMap.GetBitsSixteen(character),
            _ => GlyphLibrary.GetMatrixBits(character)
        };

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

        var characterText = CharacterInput.Text ?? string.Empty;
        var character = characterText.Trim();
        var text = character.Length > 0 ? character[0] : 'A';
        var width = selected switch
        {
            "SevenSegment" => 7,
            "Rectangle5x7" => 5,
            "SixteenSegment" => 4,
            _ => 5
        };
        var height = selected switch
        {
            "SevenSegment" => 1,
            "Rectangle5x7" => 7,
            "SixteenSegment" => 4,
            _ => 7
        };
        var bits = string.Join(", ", GetCurrentBits().Select(v => v ? "true" : "false"));

        MaskValueText.Text = $"{text} :: mask = {mask} (0x{mask:X})";
        CodePreview.Text = $"namespace SkeuomorphCore;\n\npublic static class GeneratedGlyphMaps\n{{\n    public static readonly CharacterMap {selected}Map = new({width}, {height});\n\n    static GeneratedGlyphMaps()\n    {{\n        {selected}Map.Set('{text}', {mask}UL);\n    }}\n}}\n\n// bits: [{bits}]";
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
        var selected = LayoutPicker.SelectedItem as string ?? "Rectangle5x7";
        var character = (CharacterInput.Text ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(character))
        {
            return;
        }

        var mapName = selected switch
        {
            "SevenSegment" => "SevenSegmentMap",
            "Rectangle5x7" => "Rectangle5x7Map",
            "SixteenSegment" => "SixteenSegmentMap",
            _ => "Rectangle5x7Map"
        };

        var width = selected switch
        {
            "SevenSegment" => 7,
            "Rectangle5x7" => 5,
            "SixteenSegment" => 4,
            _ => 5
        };

        var height = selected switch
        {
            "SevenSegment" => 1,
            "Rectangle5x7" => 7,
            "SixteenSegment" => 4,
            _ => 7
        };

        var mask = 0UL;
        for (var index = 0; index < _segmentButtons.Count; index++)
        {
            if (_segmentButtons[index].IsChecked == true)
            {
                mask |= 1UL << index;
            }
        }

        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "SkeuomorphCore", "GeneratedGlyphMaps.cs");

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

        File.WriteAllText(filePath, classBody);
        CodePreview.Text = classBody;
        MaskValueText.Text = $"Saved {character[0]} :: mask = {mask} (0x{mask:X})";
    }
}