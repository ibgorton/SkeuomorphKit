using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    private bool _isDirty;

    public MainWindow()
    {
        InitializeComponent();

        var firstLayout = DisplayCharacterProfiles.AllNames.FirstOrDefault() ?? "SevenSegment";
        var initialCharacter = GetDefaultCharacterForLayout(firstLayout);

        SchemaKindPicker.ItemsSource = Enum.GetValues(typeof(GlyphMapKind));
        SchemaKindPicker.SelectedItem = GlyphMapKind.Segmented;
        LayoutPicker.ItemsSource = DisplayCharacterProfiles.AllNames;
        LayoutPicker.SelectedItem = firstLayout;
        StylePicker.ItemsSource = DotMatrix8x8Map.SupportedStyles;
        StylePicker.SelectedItem = DotMatrix8x8GlyphStyles.Default;
        CustomMapNameText.Text = "CustomGlyphMap";
        CustomSegmentCountText.Text = GetCurrentMapDefinition().SegmentCount.ToString();
        CharacterInput.Text = initialCharacter.ToString();
        CharacterInput.TextChanged += CharacterInput_TextChanged;
        CharacterEnabledToggle!.IsCheckedChanged += CharacterEnabledToggle_IsCheckedChanged;
        LayoutPicker.SelectionChanged += (_, _) =>
        {
            var definition = GetCurrentMapDefinition();
            CustomSegmentCountText.Text = definition.SegmentCount.ToString();
            BitOrderText.Text = string.Join(",", definition.BitOrder);
            RefreshLayout();
        };
        StylePicker.SelectionChanged += (_, _) =>
        {
            if (LayoutPicker.SelectedItem as string == "DotMatrix8x8")
            {
                RefreshLayout();
            }
        };

        Loaded += (_, _) => RefreshLayout();
        UpdateDirtyIndicator();
    }

    private void MarkDirty()
    {
        if (_isDirty)
        {
            return;
        }

        _isDirty = true;
        UpdateDirtyIndicator();
    }

    private void ClearDirty()
    {
        if (!_isDirty)
        {
            return;
        }

        _isDirty = false;
        UpdateDirtyIndicator();
    }

    private void UpdateDirtyIndicator()
    {
        if (DirtyStateText is null || DirtyStateBorder is null)
        {
            return;
        }

        if (_isDirty)
        {
            DirtyStateText.Text = "Unsaved";
            DirtyStateText.Foreground = new SolidColorBrush(Color.FromRgb(255, 170, 170));
            DirtyStateBorder.Background = new SolidColorBrush(Color.FromRgb(54, 24, 24));
            DirtyStateBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(180, 76, 76));
            return;
        }

        DirtyStateText.Text = "Saved";
        DirtyStateText.Foreground = new SolidColorBrush(Color.FromRgb(157, 231, 180));
        DirtyStateBorder.Background = new SolidColorBrush(Color.FromRgb(18, 52, 32));
        DirtyStateBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(43, 107, 61));
    }

    private string GetSelectedStyle()
    {
        if (LayoutPicker.SelectedItem as string != "DotMatrix8x8")
        {
            return DotMatrix8x8GlyphStyles.Default;
        }

        var selected = StylePicker.SelectedItem as string;
        return DotMatrix8x8GlyphStyles.Normalize(selected ?? DotMatrix8x8GlyphStyles.Default);
    }

    private void UpdateStyleVisibility()
    {
        var isDotMatrix = LayoutPicker.SelectedItem as string == "DotMatrix8x8";
        StylePickerStack.IsVisible = isDotMatrix;
        if (isDotMatrix)
        {
            var normalized = DotMatrix8x8GlyphStyles.Normalize(GetSelectedStyle());
            if (!DotMatrix8x8Map.SupportedStyles.Contains(normalized))
            {
                StylePicker.SelectedItem = DotMatrix8x8GlyphStyles.Default;
            }
            else
            {
                StylePicker.SelectedItem = normalized;
            }
        }
    }

    private void CharacterInput_TextChanged(object? sender, EventArgs e)
    {
        UpdateCharacterEnabledToggle();
    }

    private static char GetDefaultCharacterForLayout(string layout)
    {
        foreach (var character in GetCharactersForLayout(layout))
        {
            if (!char.IsControl(character) && character != '\0')
            {
                return character;
            }
        }

        return 'A';
    }

    private void UpdateCharacterEnabledToggle()
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var inputText = CharacterInput.Text ?? string.Empty;
        var character = inputText.Length > 0 ? inputText[0] : GetDefaultCharacterForLayout(selected);
        CharacterEnabledToggle!.IsChecked = DisplayCharacterProfiles.IsCharacterEnabled(selected, character);
    }

    private void CharacterEnabledToggle_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var inputText = CharacterInput.Text ?? string.Empty;
        var character = inputText.Length > 0 ? inputText[0] : GetDefaultCharacterForLayout(selected);
        var enabled = CharacterEnabledToggle!.IsChecked == true;
        DisplayCharacterProfiles.SetCharacterEnabled(selected, character, enabled);
        PersistCharacterAvailability(selected, character, enabled);
        RefreshLayout();
    }

    private void PreviousCharacterButton_Click(object? sender, RoutedEventArgs e)
    {
        NavigateCharacter(-1);
    }

    private void NextCharacterButton_Click(object? sender, RoutedEventArgs e)
    {
        NavigateCharacter(1);
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        SaveCurrentCharacter();
    }

    private void AllOnButton_Click(object? sender, RoutedEventArgs e)
    {
        ApplyAllSegments(true);
    }

    private void AllOffButton_Click(object? sender, RoutedEventArgs e)
    {
        ApplyAllSegments(false);
    }

    private void ApplyAllSegments(bool lit)
    {
        foreach (var button in _segmentButtons)
        {
            if (button.IsChecked != lit)
            {
                button.IsChecked = lit;
            }
        }

        MarkDirty();
        UpdateMaskText();
    }

    private void CreateCustomMapButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            CreateCustomMapFromSelection();
        }
        catch (Exception ex)
        {
            MaskValueText.Text = $"Create failed: {ex.Message}";
        }
    }

    private void RemapCurrentMapButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            RemapCurrentMap();
        }
        catch (Exception ex)
        {
            MaskValueText.Text = $"Remap failed: {ex.Message}";
        }
    }

    private void NavigateCharacter(int delta)
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var characters = GetCharactersForLayout(selected).ToList();
        if (characters.Count == 0)
        {
            return;
        }

        var currentText = CharacterInput.Text ?? string.Empty;
        var current = currentText.Length > 0 ? currentText[0] : 'A';
        var currentIndex = characters.IndexOf(current);
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        var nextIndex = (currentIndex + delta + characters.Count) % characters.Count;
        var nextCharacter = characters[nextIndex];
        CharacterInput.Text = nextCharacter == ' ' ? " " : nextCharacter.ToString();
        UpdateCharacterEnabledToggle();
        LoadCharacterIntoGrid(selected, nextCharacter.ToString(), GetSelectedStyle());
        UpdateMaskText();
    }

    private void RefreshLayout()
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        UpdateStyleVisibility();
        var profile = DisplayCharacterProfiles.Get(selected);
        var inputText = CharacterInput.Text ?? string.Empty;
        var character = inputText.Length > 0 ? inputText[0] : GetDefaultCharacterForLayout(selected);
        _segmentCount = profile.SegmentCount;

        if (CustomSegmentCountText is not null && string.IsNullOrWhiteSpace(CustomSegmentCountText.Text))
        {
            CustomSegmentCountText.Text = profile.SegmentCount.ToString();
        }

        if (BitOrderText is not null && string.IsNullOrWhiteSpace(BitOrderText.Text))
        {
            BitOrderText.Text = string.Join(",", GetCurrentMapDefinition().BitOrder);
        }

        BuildSegmentGrid(profile);
        BuildCharacterMap(profile.Name);
        UpdateCharacterEnabledToggle();
        LoadCharacterIntoGrid(profile.Name, character.ToString(), GetSelectedStyle());
        UpdateMaskText();
        UpdateSegmentLegend();
        ClearDirty();
    }

    private void BuildCharacterMap(string layout)
    {
        CharacterMapPanel.Children.Clear();
        var characters = GetCharactersForLayout(layout);

        foreach (var character in characters)
        {
            var isDisabled = !DisplayCharacterProfiles.IsCharacterEnabled(layout, character);
            var button = new Button
            {
                Content = CreateCharacterMapContent(character, isDisabled),
                Width = 52,
                Height = 36,
                Margin = new Thickness(2),
                Tag = character,
                Background = isDisabled ? new SolidColorBrush(Color.FromRgb(44, 20, 20)) : new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                BorderBrush = isDisabled ? new SolidColorBrush(Color.FromRgb(200, 78, 78)) : new SolidColorBrush(Color.FromRgb(90, 90, 90)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                FontWeight = isDisabled ? FontWeight.SemiBold : FontWeight.Normal,
                Foreground = isDisabled ? new SolidColorBrush(Color.FromRgb(255, 190, 190)) : new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                Opacity = isDisabled ? 0.8 : 1.0
            };

            button.Click += (_, _) =>
            {
                CharacterInput.Text = character == ' ' ? " " : character.ToString();
                UpdateCharacterEnabledToggle();
                LoadCharacterIntoGrid(layout, character.ToString(), GetSelectedStyle());
                UpdateMaskText();
            };

            CharacterMapPanel.Children.Add(button);
        }
    }

    private static object CreateCharacterMapContent(char character, bool isDisabled)
    {
        var label = character switch
        {
            ' ' => "space",
            _ when char.IsControl(character) => $"U+{(int)character:X4}",
            _ => character.ToString()
        };
        if (!isDisabled)
        {
            return label;
        }

        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 0,
            Children =
            {
                new TextBlock { Text = label, FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center },
                new TextBlock { Text = "OFF", FontSize = 8, Foreground = new SolidColorBrush(Color.FromRgb(255, 128, 128)), FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center }
            }
        };
    }

    private static IReadOnlyList<char> GetCharactersForLayout(string layout)
    {
        if (layout == "DotMatrix8x8")
        {
            return Enumerable.Range(0, 128)
                .Select(static i => (char)i)
                .ToArray();
        }

        var candidates = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?.,:;+-/=\\_[](){}<>|#@%&*'\"$^~".ToCharArray();
        var characters = new List<char>();

        foreach (var character in candidates)
        {
            if (!characters.Contains(character))
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

        var layoutDefinition = GetLayoutDefinition(profile.Name);
        if (layoutDefinition is not null)
        {
            if (layoutDefinition.Segments.Count > 0)
            {
                var canvas = new Canvas
                {
                    Width = layoutDefinition.CanvasWidth,
                    Height = layoutDefinition.CanvasHeight,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                for (var index = 0; index < layoutDefinition.Segments.Count; index++)
                {
                    var points = layoutDefinition.Segments[index]
                        .Select(point => new Point(point[0], point[1]))
                        .ToArray();

                    var button = CreatePolygonSegmentButton(index, points, new Point(layoutDefinition.OffsetX, layoutDefinition.OffsetY), layoutDefinition.Scale);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }

            var gridColumns = layoutDefinition.Columns > 0 ? layoutDefinition.Columns : profile.Width;
            var gridRows = layoutDefinition.Rows > 0 ? layoutDefinition.Rows : profile.Height;

            for (var x = 0; x < gridColumns; x++)
            {
                SegmentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            }

            for (var y = 0; y < gridRows; y++)
            {
                SegmentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            }

            for (var index = 0; index < _segmentCount; index++)
            {
                var row = index / gridColumns;
                var column = index % gridColumns;
                var button = new ToggleButton
                {
                    Width = 20,
                    Height = 20,
                    IsChecked = false,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(2),
                    Tag = index,
                    Content = string.Empty,
                    Background = new SolidColorBrush(Color.FromArgb(28, 220, 220, 220)),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(110, 200, 200, 200)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(2),
                    Padding = new Thickness(0)
                };

                button.IsCheckedChanged += (_, _) => UpdateGridButtonVisual(button);
                button.Click += SegmentButton_Click;
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);
                UpdateGridButtonVisual(button);
                SegmentGrid.Children.Add(button);
                _segmentButtons.Add(button);
            }

            return;
        }

        switch (profile.Name)
        {
            case "SevenSegment":
            {
                var canvas = new Canvas
                {
                    Width = 220,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var polygons = new[]
                {
                    new[] { new Point(1, 1), new Point(2, 0), new Point(8, 0), new Point(9, 1), new Point(8, 2), new Point(2, 2) },
                    new[] { new Point(9, 1), new Point(10, 2), new Point(10, 8), new Point(9, 9), new Point(8, 8), new Point(8, 2) },
                    new[] { new Point(9, 9), new Point(10, 10), new Point(10, 16), new Point(9, 17), new Point(8, 16), new Point(8, 10) },
                    new[] { new Point(9, 17), new Point(8, 18), new Point(2, 18), new Point(1, 17), new Point(2, 16), new Point(8, 16) },
                    new[] { new Point(1, 17), new Point(0, 16), new Point(0, 10), new Point(1, 9), new Point(2, 10), new Point(2, 16) },
                    new[] { new Point(1, 9), new Point(0, 8), new Point(0, 2), new Point(1, 1), new Point(2, 2), new Point(2, 8) },
                    new[] { new Point(1, 9), new Point(2, 8), new Point(8, 8), new Point(9, 9), new Point(8, 10), new Point(2, 10) }
                };

                for (var index = 0; index < polygons.Length; index++)
                {
                    var points = polygons[index];
                    var button = CreatePolygonSegmentButton(index, points, new Point(22, 18), 16.0);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }
            case "NineSegmentSlash":
            case "NineSegmentSlashAlt":
            case "NineSegmentBackslash":
            case "NineSegmentBackslashAlt":
            {
                var canvas = new Canvas
                {
                    Width = 220,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var polygons = profile.Name switch
                {
                    "NineSegmentBackslash" or "NineSegmentBackslashAlt" => new[]
                    {
                        new[] { new Point(1, 1), new Point(2, 0), new Point(8, 0), new Point(9, 1), new Point(8, 2), new Point(2, 2) },
                        new[] { new Point(9, 1), new Point(10, 2), new Point(10, 8), new Point(9, 9), new Point(8, 8), new Point(8, 2) },
                        new[] { new Point(9, 9), new Point(10, 10), new Point(10, 16), new Point(9, 17), new Point(8, 16), new Point(8, 10) },
                        new[] { new Point(9, 17), new Point(8, 18), new Point(2, 18), new Point(1, 17), new Point(2, 16), new Point(8, 16) },
                        new[] { new Point(1, 17), new Point(0, 16), new Point(0, 10), new Point(1, 9), new Point(2, 10), new Point(2, 16) },
                        new[] { new Point(1, 9), new Point(0, 8), new Point(0, 2), new Point(1, 1), new Point(2, 2), new Point(2, 8) },
                        new[] { new Point(1, 9), new Point(2, 8), new Point(8, 8), new Point(9, 9), new Point(8, 10), new Point(2, 10) },
                        new[] { new Point(8, 2), new Point(8, 3.4), new Point(3.4, 8), new Point(2, 8), new Point(2, 6.6), new Point(6.6, 2) },
                        new[] { new Point(8, 10), new Point(8, 11.4), new Point(3.4, 16), new Point(2, 16), new Point(2, 14.6), new Point(6.6, 10) }
                    },
                    _ => new[]
                    {
                        new[] { new Point(1, 1), new Point(2, 0), new Point(8, 0), new Point(9, 1), new Point(8, 2), new Point(2, 2) },
                        new[] { new Point(9, 1), new Point(10, 2), new Point(10, 8), new Point(9, 9), new Point(8, 8), new Point(8, 2) },
                        new[] { new Point(9, 9), new Point(10, 10), new Point(10, 16), new Point(9, 17), new Point(8, 16), new Point(8, 10) },
                        new[] { new Point(9, 17), new Point(8, 18), new Point(2, 18), new Point(1, 17), new Point(2, 16), new Point(8, 16) },
                        new[] { new Point(1, 17), new Point(0, 16), new Point(0, 10), new Point(1, 9), new Point(2, 10), new Point(2, 16) },
                        new[] { new Point(1, 9), new Point(0, 8), new Point(0, 2), new Point(1, 1), new Point(2, 2), new Point(2, 8) },
                        new[] { new Point(1, 9), new Point(2, 8), new Point(8, 8), new Point(9, 9), new Point(8, 10), new Point(2, 10) },
                        new[] { new Point(2, 2), new Point(3.4, 2), new Point(8, 6.6), new Point(8, 8), new Point(6.6, 8), new Point(2, 3.4) },
                        new[] { new Point(8, 16), new Point(8, 14.6), new Point(3.4, 10), new Point(2, 10), new Point(2, 11.4), new Point(6.6, 16) }
                    }
                };

                for (var index = 0; index < polygons.Length; index++)
                {
                    var points = polygons[index];
                    var button = CreatePolygonSegmentButton(index, points, new Point(22, 18), 16.0);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }
            case "TenSegment":
            {
                var canvas = new Canvas
                {
                    Width = 220,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var polygons = new[]
                {
                    new[] { new Point(1, 1), new Point(2, 0), new Point(8, 0), new Point(9, 1), new Point(8, 2), new Point(2, 2) },
                    new[] { new Point(9, 1), new Point(10, 2), new Point(10, 8), new Point(9, 9), new Point(8, 8), new Point(8, 2) },
                    new[] { new Point(9, 9), new Point(10, 10), new Point(10, 16), new Point(9, 17), new Point(8, 16), new Point(8, 10) },
                    new[] { new Point(9, 17), new Point(8, 18), new Point(2, 18), new Point(1, 17), new Point(2, 16), new Point(8, 16) },
                    new[] { new Point(1, 17), new Point(0, 16), new Point(0, 10), new Point(1, 9), new Point(2, 10), new Point(2, 16) },
                    new[] { new Point(1, 9), new Point(0, 8), new Point(0, 2), new Point(1, 1), new Point(2, 2), new Point(2, 8) },
                    new[] { new Point(1, 9), new Point(2, 8), new Point(4, 8), new Point(5, 9), new Point(4, 10), new Point(2, 10) },
                    new[] { new Point(5, 9), new Point(6, 8), new Point(8, 8), new Point(9, 9), new Point(8, 10), new Point(6, 10) },
                    new[] { new Point(6, 2), new Point(6, 8), new Point(5, 9), new Point(4, 8), new Point(4, 2) },
                    new[] { new Point(5, 9), new Point(6, 10), new Point(6, 16), new Point(4, 16), new Point(4, 10) }
                };

                for (var index = 0; index < polygons.Length; index++)
                {
                    var points = polygons[index];
                    var button = CreatePolygonSegmentButton(index, points, new Point(22, 18), 16.0);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }
            case "FourteenSegment":
            {
                var canvas = new Canvas
                {
                    Width = 220,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var polygons = new[]
                {
                    new[] { new Point(1, 1), new Point(2, 0), new Point(8, 0), new Point(9, 1), new Point(8, 2), new Point(2, 2) },
                    new[] { new Point(9, 1), new Point(10, 2), new Point(10, 8), new Point(9, 9), new Point(8, 8), new Point(8, 2) },
                    new[] { new Point(9, 9), new Point(10, 10), new Point(10, 16), new Point(9, 17), new Point(8, 16), new Point(8, 10) },
                    new[] { new Point(9, 17), new Point(8, 18), new Point(2, 18), new Point(1, 17), new Point(2, 16), new Point(8, 16) },
                    new[] { new Point(1, 17), new Point(0, 16), new Point(0, 10), new Point(1, 9), new Point(2, 10), new Point(2, 16) },
                    new[] { new Point(1, 9), new Point(0, 8), new Point(0, 2), new Point(1, 1), new Point(2, 2), new Point(2, 8) },
                    new[] { new Point(1, 9), new Point(2, 8), new Point(4, 8), new Point(5, 9), new Point(4, 10), new Point(2, 10) },
                    new[] { new Point(5, 9), new Point(6, 8), new Point(8, 8), new Point(9, 9), new Point(8, 10), new Point(6, 10) },
                    new[] { new Point(6, 2), new Point(6, 8), new Point(5, 9), new Point(4, 8), new Point(4, 2) },
                    new[] { new Point(5, 9), new Point(6, 10), new Point(6, 16), new Point(4, 16), new Point(4, 10) },
                    new[] { new Point(2, 2), new Point(3, 2), new Point(4, 7), new Point(4, 8), new Point(3, 8), new Point(2, 3) },
                    new[] { new Point(8, 2), new Point(8, 3), new Point(7, 8), new Point(6, 8), new Point(6, 7), new Point(7, 2) },
                    new[] { new Point(6, 10), new Point(7, 10), new Point(8, 15), new Point(8, 16), new Point(7, 16), new Point(6, 11) },
                    new[] { new Point(4, 10), new Point(4, 11), new Point(3, 16), new Point(2, 16), new Point(2, 15), new Point(3, 10) }
                };

                for (var index = 0; index < polygons.Length; index++)
                {
                    var points = polygons[index];
                    var button = CreatePolygonSegmentButton(index, points, new Point(22, 18), 16.0);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }
            case "DotMatrix8x8":
            {
                const int matrixRows = 8;
                const int matrixColumns = 8;

                for (var x = 0; x < matrixColumns; x++)
                {
                    SegmentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                }

                for (var y = 0; y < matrixRows; y++)
                {
                    SegmentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                }

                for (var index = 0; index < _segmentCount; index++)
                {
                    var row = index / matrixColumns;
                    var column = index % matrixColumns;
                    var button = new ToggleButton
                    {
                        Width = 18,
                        Height = 18,
                        IsChecked = false,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(1),
                        Tag = index,
                        Background = new SolidColorBrush(Color.FromArgb(30, 230, 230, 230)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(120, 200, 200, 200)),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(2),
                        Padding = new Thickness(0),
                        Content = string.Empty
                    };

                    button.IsCheckedChanged += (_, _) => UpdateGridButtonVisual(button);
                    button.Click += SegmentButton_Click;
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, column);
                    UpdateGridButtonVisual(button);
                    SegmentGrid.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                return;
            }
            case "SixteenSegment":
            {
                var canvas = new Canvas
                {
                    Width = 260,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var legacyClockwiseSegmentDefinitions = new[]
                {
                    (Index: 0, Name: "a1", Points: new[] { new Point(1, 1), new Point(2, 0), new Point(4, 0), new Point(5, 1), new Point(4, 2), new Point(2, 2) }),
                    (Index: 1, Name: "a2", Points: new[] { new Point(5, 1), new Point(6, 0), new Point(8, 0), new Point(9, 1), new Point(8, 2), new Point(6, 2) }),
                    (Index: 2, Name: "b", Points: new[] { new Point(9, 1), new Point(10, 2), new Point(10, 8), new Point(9, 9), new Point(8, 8), new Point(8, 2) }),
                    (Index: 3, Name: "c", Points: new[] { new Point(9, 9), new Point(10, 10), new Point(10, 16), new Point(9, 17), new Point(8, 16), new Point(8, 10) }),
                    (Index: 4, Name: "d1", Points: new[] { new Point(9, 17), new Point(8, 18), new Point(6, 18), new Point(5, 17), new Point(6, 16), new Point(8, 16) }),
                    (Index: 5, Name: "d2", Points: new[] { new Point(5, 17), new Point(4, 18), new Point(2, 18), new Point(1, 17), new Point(2, 16), new Point(4, 16) }),
                    (Index: 6, Name: "e", Points: new[] { new Point(1, 17), new Point(0, 16), new Point(0, 10), new Point(1, 9), new Point(2, 10), new Point(2, 16) }),
                    (Index: 7, Name: "f", Points: new[] { new Point(1, 9), new Point(0, 8), new Point(0, 2), new Point(1, 1), new Point(2, 2), new Point(2, 8) }),
                    (Index: 8, Name: "j", Points: new[] { new Point(2, 2), new Point(3, 2), new Point(4, 7), new Point(4, 8), new Point(3, 8), new Point(2, 3) }),
                    (Index: 9, Name: "h", Points: new[] { new Point(5, 1), new Point(6, 2), new Point(6, 8), new Point(5, 9), new Point(4, 8), new Point(4, 2) }),
                    (Index: 10, Name: "k", Points: new[] { new Point(8, 2), new Point(8, 3), new Point(7, 8), new Point(6, 8), new Point(6, 7), new Point(7, 2) }),
                    (Index: 11, Name: "g2", Points: new[] { new Point(5, 9), new Point(6, 8), new Point(8, 8), new Point(9, 9), new Point(8, 10), new Point(6, 10) }),
                    (Index: 12, Name: "l", Points: new[] { new Point(6, 10), new Point(7, 10), new Point(8, 15), new Point(8, 16), new Point(7, 16), new Point(6, 11) }),
                    (Index: 13, Name: "i", Points: new[] { new Point(5, 9), new Point(6, 10), new Point(6, 16), new Point(5, 17), new Point(4, 16), new Point(4, 10) }),
                    (Index: 14, Name: "m", Points: new[] { new Point(4, 10), new Point(4, 11), new Point(3, 16), new Point(2, 16), new Point(2, 15), new Point(3, 10) }),
                    (Index: 15, Name: "g1", Points: new[] { new Point(1, 9), new Point(2, 8), new Point(4, 8), new Point(5, 9), new Point(4, 10), new Point(2, 10) })
                };

                foreach (var segment in legacyClockwiseSegmentDefinitions)
                {
                    var button = CreatePolygonSegmentButton(segment.Index, segment.Points, new Point(22, 18), 16.0);
                    canvas.Children.Add(button);
                    _segmentButtons.Add(button);
                }

                SegmentGrid.Children.Add(canvas);
                return;
            }
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
                Width = 20,
                Height = 20,
                IsChecked = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2),
                Tag = index,
                Content = string.Empty,
                Background = new SolidColorBrush(Color.FromArgb(28, 220, 220, 220)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(110, 200, 200, 200)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(2),
                Padding = new Thickness(0)
            };

            button.IsCheckedChanged += (_, _) => UpdateGridButtonVisual(button);
            button.Click += SegmentButton_Click;
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            UpdateGridButtonVisual(button);
            SegmentGrid.Children.Add(button);
            _segmentButtons.Add(button);
        }
    }

    private static GlyphMapLayoutDefinition? GetLayoutDefinition(string layoutName)
    {
        if (GlyphMapCatalog.BuiltInJson.TryGetValue(layoutName, out var json))
        {
            return GlyphMapDefinition.FromJson(json, layoutName).Layout;
        }

        return null;
    }

    private void SegmentButton_Click(object? sender, RoutedEventArgs e)
    {
        MarkDirty();
        UpdateMaskText();
    }

    private ToggleButton CreateSegmentButton(int index, double width, double height, double x, double y, double angle)
    {
        var segment = new SegmentSpec(index, x, y, width, height, angle);
        return CreateSegmentButton(segment);
    }

    private ToggleButton CreatePolygonSegmentButton(int index, IReadOnlyList<Point> points, Point offset, double scale)
    {
        var geometry = CreatePolygonGeometry(points, scale);
        var bounds = geometry.Bounds;
        var width = Math.Max(1, bounds.Width + 8);
        var height = Math.Max(1, bounds.Height + 8);

        var body = new Avalonia.Controls.Shapes.Path
        {
            Width = width,
            Height = height,
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
            Width = width,
            Height = height,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            Tag = index,
            Content = body,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            IsChecked = false,
            RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
        };

        button.IsCheckedChanged += (_, _) => UpdateSegmentButtonVisual(button);
        button.Click += SegmentButton_Click;

        Canvas.SetLeft(button, offset.X + bounds.X - 4);
        Canvas.SetTop(button, offset.Y + bounds.Y - 4);
        UpdateSegmentButtonVisual(button);
        return button;
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

    private static Geometry CreatePolygonGeometry(IReadOnlyList<Point> points, double scale)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure { IsClosed = true, IsFilled = true };

        var first = points[0];
        figure.StartPoint = new Point(first.X * scale, first.Y * scale);
        for (var i = 1; i < points.Count; i++)
        {
            var point = points[i];
            figure.Segments!.Add(new LineSegment
            {
                Point = new Point(point.X * scale, point.Y * scale)
            });
        }

        geometry.Figures!.Add(figure);
        return geometry;
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
            UpdateGridButtonVisual(button);
            return;
        }

        var lit = button.IsChecked == true;
        var glow = lit ? Color.FromRgb(255, 98, 98) : Color.FromRgb(88, 54, 54);
        var shadow = lit ? Color.FromRgb(255, 148, 148) : Color.FromRgb(64, 40, 40);

        path.Fill = new SolidColorBrush(glow);
        path.Stroke = lit ? new SolidColorBrush(Color.FromRgb(255, 180, 180)) : new SolidColorBrush(Color.FromRgb(70, 44, 44));
        path.StrokeThickness = lit ? 0.7 : 0.3;
        path.Opacity = lit ? 1.0 : 0.4;
        path.Effect = lit
            ? new DropShadowEffect
            {
                BlurRadius = 10,
                Color = new Color(255, 255, 90, 90),
                OffsetX = 0,
                OffsetY = 0
            }
            : null;

        button.Background = lit
            ? new SolidColorBrush(Color.FromArgb(40, 255, 100, 100))
            : new SolidColorBrush(Color.FromArgb(10, 255, 255, 255));
        button.BorderBrush = lit ? new SolidColorBrush(Color.FromArgb(120, 255, 120, 120)) : new SolidColorBrush(Color.FromArgb(80, 120, 120, 120));
        button.BorderThickness = new Thickness(lit ? 1 : 0.5);
    }

    private static void UpdateGridButtonVisual(ToggleButton button)
    {
        var lit = button.IsChecked == true;
        button.Background = lit
            ? new SolidColorBrush(Color.FromArgb(200, 255, 120, 120))
            : new SolidColorBrush(Color.FromArgb(28, 220, 220, 220));
        button.BorderBrush = lit
            ? new SolidColorBrush(Color.FromArgb(180, 255, 180, 180))
            : new SolidColorBrush(Color.FromArgb(120, 180, 180, 180));
        button.BorderThickness = new Thickness(1);
        button.Padding = new Thickness(0);
        button.Opacity = lit ? 1.0 : 0.85;
        button.Content ??= string.Empty;
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

    private void LoadCharacterIntoGrid(string layout, string rawText, string? style = null)
    {
        var profile = DisplayCharacterProfiles.Get(layout);
        var character = rawText.Length > 0 ? rawText[0] : 'A';
        var bits = layout == "DotMatrix8x8"
            ? DotMatrix8x8Map.GetBits(character, DotMatrix8x8GlyphStyles.Normalize(style ?? GetSelectedStyle()))
            : profile.GetBits(character);

        for (var index = 0; index < _segmentButtons.Count; index++)
        {
            var button = _segmentButtons[index];
            button.IsChecked = index < bits.Length && bits[index];
        }

        UpdateMaskText();
    }

    private static string BuildMaskExpression(string layout, ulong mask)
    {
        _ = layout;
        return $"0x{mask:X}";
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
        _ = profile;
        _ = text;

        MaskValueText.Text = $"{text} :: mask = {mask} (0x{mask:X})";
        UpdateSegmentLegend();
    }

    private void UpdateSegmentLegend()
    {
        var selected = LayoutPicker.SelectedItem as string;
        var names = selected switch
        {
            "NineSegmentSlash" or "NineSegmentSlashAlt" or "NineSegmentBackslash" or "NineSegmentBackslashAlt" => new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i" },
            "TenSegment" => new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" },
            "SixteenSegment" => new[]
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
            },
            _ => null
        };

        if (names is null)
        {
            SegmentLegendText.Text = string.Empty;
            return;
        }

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

       var character = characterText[0];
       ulong? mask = null;
       if (CharacterEnabledToggle!.IsChecked == true)
       {
           var bitsMask = 0UL;
           for (var index = 0; index < _segmentButtons.Count; index++)
           {
               if (_segmentButtons[index].IsChecked == true)
               {
                   bitsMask |= 1UL << index;
               }
           }

           mask = bitsMask;
       }

       var filePath = ResolveMapFileForLayout(selected);
       if (filePath is null)
       {
           MaskValueText.Text = "Save skipped";
           return;
       }

       try
       {
           ApplyRuntimeMapUpdate(selected, character, mask);
           PersistRuntimeMap(selected);
           RefreshLayout();
           ClearDirty();
           var maskText = mask.HasValue ? $"{mask.Value} (0x{mask.Value:X})" : "null";
           MaskValueText.Text = $"Saved {character} :: mask = {maskText} to {System.IO.Path.GetFileName(filePath)}";
       }
       catch (Exception ex)
       {
           MaskValueText.Text = $"Save failed: {ex.Message}";
       }
   }

   private void CreateCustomMapFromSelection()
   {
       var sourceName = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
       var sourceDefinition = GetMapDefinitionForLayout(sourceName);
       var desiredName = ValidateCustomMapName((CustomMapNameText.Text ?? string.Empty).Trim(), sourceName, allowOverwrite: false);

       var baseBitOrder = ParseBitOrderText(BitOrderText.Text, sourceDefinition.SegmentCount, sourceName, allowEmpty: true);
       var kind = SchemaKindPicker.SelectedItem is GlyphMapKind selectedKind ? selectedKind : sourceDefinition.Kind;
       var segmentCount = int.TryParse(CustomSegmentCountText.Text, out var requestedCount) && requestedCount > 0
           ? requestedCount
           : sourceDefinition.SegmentCount;

       var definition = new GlyphMapDefinition
       {
           Name = desiredName,
           Kind = kind,
           SegmentCount = segmentCount,
           BitOrder = baseBitOrder,
           Layout = CloneLayoutDefinition(sourceDefinition.Layout),
           Characters = new Dictionary<string, string>(sourceDefinition.Characters, StringComparer.Ordinal)
       };

       definition.Normalize();
       var serialized = definition.ToJson();
       DisplayCharacterProfiles.RegisterMapJson(desiredName, serialized);
       SaveMapJsonToDisk(desiredName, serialized);

       CustomMapNameText.Text = desiredName;
       LayoutPicker.ItemsSource = DisplayCharacterProfiles.AllNames;
       LayoutPicker.SelectedItem = desiredName;
       RefreshLayout();
       MaskValueText.Text = $"Created custom glyph map '{desiredName}'";
   }

   private void RemapCurrentMap()
   {
       var sourceName = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
       var sourceDefinition = GetMapDefinitionForLayout(sourceName);
       var targetName = (CustomMapNameText.Text ?? string.Empty).Trim();
       if (string.IsNullOrWhiteSpace(targetName))
       {
           targetName = sourceName + "Remapped";
       }

       if (string.Equals(targetName, sourceName, StringComparison.Ordinal))
       {
           throw new InvalidOperationException("Use a different name for the remapped map or leave the name blank to auto-generate one.");
       }

       targetName = ValidateCustomMapName(targetName, sourceName, allowOverwrite: false);
       var bitOrder = ParseBitOrderText(BitOrderText.Text, sourceDefinition.SegmentCount, sourceName, allowEmpty: false);
       var remapped = sourceDefinition.RemapBitOrder(bitOrder);
       remapped.Name = targetName;
       remapped.Version = GlyphMapDefinition.CurrentVersion;

       var serialized = remapped.ToJson();
       DisplayCharacterProfiles.RegisterMapJson(targetName, serialized);
       SaveMapJsonToDisk(targetName, serialized);

       CustomMapNameText.Text = targetName;
       LayoutPicker.ItemsSource = DisplayCharacterProfiles.AllNames;
       LayoutPicker.SelectedItem = targetName;
       RefreshLayout();
       MaskValueText.Text = $"Remapped '{sourceName}' to '{targetName}' with bit order {string.Join(",", bitOrder)}";
   }

   private GlyphMapDefinition GetCurrentMapDefinition()
   {
       var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
       return GetMapDefinitionForLayout(selected);
   }

   private static GlyphMapDefinition GetMapDefinitionForLayout(string layoutName)
   {
       var normalized = NormalizeMapName(layoutName);
       if (string.IsNullOrWhiteSpace(normalized))
       {
           throw new InvalidOperationException("A layout name is required.");
       }

       if (GlyphMapCatalog.TryGetJson(normalized, out var json))
       {
           return GlyphMapDefinition.FromJson(json, normalized);
       }

       if (DisplayCharacterProfiles.TryGet(normalized, out var profile))
       {
           var map = DisplayCharacterProfiles.GetMap(normalized);
           return GlyphMapDefinition.FromMap(normalized, map);
       }

       throw new KeyNotFoundException($"Layout '{normalized}' does not have a backing glyph map definition.");
   }

   private static string ValidateCustomMapName(string name, string sourceName, bool allowOverwrite)
   {
       var trimmed = (name ?? string.Empty).Trim();
       if (string.IsNullOrWhiteSpace(trimmed))
       {
           throw new InvalidOperationException("A custom map name is required.");
       }

       if (trimmed.Length < 2)
       {
           throw new InvalidOperationException("Custom map names must be at least 2 characters long.");
       }

       if (!char.IsLetter(trimmed[0]) && trimmed[0] != '_')
       {
           throw new InvalidOperationException("Custom map names must start with a letter or underscore.");
       }

       if (trimmed.Any(static character => !char.IsLetterOrDigit(character) && character != '_' && character != '-'))
       {
           throw new InvalidOperationException("Custom map names may only contain letters, digits, underscores, and dashes.");
       }

       var existingNames = DisplayCharacterProfiles.AllNames
           .Select(static entry => NormalizeMapName(entry))
           .Where(static entry => !string.IsNullOrWhiteSpace(entry))
           .ToHashSet(StringComparer.Ordinal);

       if (!allowOverwrite && existingNames.Contains(trimmed) && !string.Equals(trimmed, sourceName, StringComparison.Ordinal))
       {
           throw new InvalidOperationException($"A glyph map named '{trimmed}' already exists. Use a different name or leave the field blank to auto-generate one.");
       }

       if (GlyphMapCatalog.BuiltInJson.ContainsKey(trimmed))
       {
           throw new InvalidOperationException($"'{trimmed}' is already reserved as a built-in glyph map name.");
       }

       var repoPath = ResolveRepositoryRoot();
       var customMapPath = System.IO.Path.Combine(repoPath, "SkeuomorphCore", "Glyphs", "Maps", $"{trimmed}.json");
       if (File.Exists(customMapPath) && !allowOverwrite)
       {
           throw new InvalidOperationException($"A custom map file for '{trimmed}' already exists on disk.");
       }

       return trimmed;
   }

   private static GlyphMapLayoutDefinition? CloneLayoutDefinition(GlyphMapLayoutDefinition? source)
   {
       if (source is null)
       {
           return null;
       }

       return new GlyphMapLayoutDefinition
       {
           Columns = source.Columns,
           Rows = source.Rows,
           CanvasWidth = source.CanvasWidth,
           CanvasHeight = source.CanvasHeight,
           OffsetX = source.OffsetX,
           OffsetY = source.OffsetY,
           Scale = source.Scale,
           Segments = source.Segments
               .Select(static segment => segment.Select(static point => new[] { point[0], point[1] }).ToList())
               .ToList()
       };
   }

   private static List<int> ParseBitOrderText(string? text, int segmentCount, string mapName, bool allowEmpty)
   {
       if (string.IsNullOrWhiteSpace(text))
       {
           if (allowEmpty)
           {
               return Enumerable.Range(0, segmentCount).ToList();
           }

           throw new InvalidOperationException($"A bit-order remap is required for '{mapName}'.");
       }

       var values = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
           .Select(static part => int.TryParse(part, out var value) ? value : -1)
           .ToList();

       if (values.Count == 0)
       {
           return Enumerable.Range(0, segmentCount).ToList();
       }

       if (values.Any(static value => value < 0))
       {
           throw new InvalidOperationException($"Bit order values for '{mapName}' must be integers.");
       }

       if (values.Count != segmentCount)
       {
           throw new InvalidOperationException($"Bit order for '{mapName}' must contain exactly {segmentCount} values.");
       }

       var set = new HashSet<int>(values);
       if (set.Count != values.Count)
       {
           throw new InvalidOperationException($"Bit order for '{mapName}' must define a unique permutation of 0..{segmentCount - 1}.");
       }

       for (var index = 0; index < values.Count; index++)
       {
           if (values[index] < 0 || values[index] >= segmentCount)
           {
               throw new InvalidOperationException($"Bit order value '{values[index]}' is out of range for '{mapName}'.");
           }
       }

       return values;
   }

   private static void SaveMapJsonToDisk(string mapName, string json)
   {
       var directory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "SkeuomorphCore", "Glyphs", "Maps");
       Directory.CreateDirectory(directory);
       var filePath = System.IO.Path.Combine(directory, $"{mapName}.json");
       File.WriteAllText(filePath, json);
   }

   private static void ApplyRuntimeMapUpdate(string layout, char character, ulong? mask)
   {
       var map = GetRuntimeMap(layout);
       if (map is null)
       {
           return;
       }

       if (map is ISegmentedGlyphMap segmented)
       {
           if (mask.HasValue)
           {
               segmented.SetCharacterEnabled(character, true);
               if (segmented.Masks is IDictionary<char, ulong?> dictionary && dictionary.ContainsKey(character))
               {
                   dictionary[character] = mask.Value;
               }

               return;
           }

           segmented.SetCharacterEnabled(character, false);
           return;
       }

       if (map is IBitmapGlyphMap bitmap)
       {
           if (mask.HasValue)
           {
               bitmap.SetCharacterEnabled(character, true);
               return;
           }

           bitmap.SetCharacterEnabled(character, false);
       }
   }

   private static void PersistCharacterAvailability(string layout, char character, bool enabled)
   {
       var map = GetRuntimeMap(layout);
       if (map is null)
       {
           return;
       }

       map.SetCharacterEnabled(character, enabled);
       PersistRuntimeMap(layout);
   }

   private static IGlyphMap? GetRuntimeMap(string layout)
   {
       var mapName = NormalizeMapName(layout);
       if (string.IsNullOrWhiteSpace(mapName))
       {
           return null;
       }

       var mapsField = typeof(DisplayCharacterProfiles).GetField("Maps", BindingFlags.NonPublic | BindingFlags.Static);
       if (mapsField is null || mapsField.GetValue(null) is not IDictionary runtimeMaps)
       {
           return null;
       }

       var canonicalName = GetMapNameForLayout(mapName);
       if (string.IsNullOrWhiteSpace(canonicalName))
       {
           return null;
       }

       return runtimeMaps[canonicalName] as IGlyphMap;
   }

   private static void PersistRuntimeMap(string layout)
   {
       var map = GetRuntimeMap(layout);
       if (map is null)
       {
           return;
       }

       var filePath = ResolveMapFileForLayout(layout);
       if (filePath is null)
       {
           return;
       }

       var definition = GlyphMapDefinition.FromMap(GetMapNameForLayout(layout), map);
       File.WriteAllText(filePath, definition.ToJson());
   }

   private static string NormalizeMapName(string? layout)
   {
       return string.IsNullOrWhiteSpace(layout) ? string.Empty : layout.Trim();
   }

   private static string GetMapNameForLayout(string layout)
   {
       var normalized = NormalizeMapName(layout);
       return normalized switch
       {
           "SevenSegment" => "SevenSegment",
           "NineSegmentSlash" => "NineSegmentSlash",
           "NineSegmentBackslash" => "NineSegmentBackslash",
           "NineSegmentSlashAlt" => "NineSegmentSlashAlt",
           "NineSegmentBackslashAlt" => "NineSegmentBackslashAlt",
           "TenSegment" => "TenSegment",
           "FourteenSegment" => "FourteenSegment",
           "Rectangle5x7" => "Rectangle5x7",
           "DotMatrix8x8" => "DotMatrix8x8",
           "SixteenSegment" => "SixteenSegment",
           _ => string.Empty
       };
   }

   private static string? ResolveMapFileForLayout(string layout)
   {
       var repoRoot = ResolveRepositoryRoot();
       var candidateDirectories = new[]
       {
           System.IO.Path.Combine(repoRoot, "SkeuomorphCore", "Glyphs", "Maps"),
           System.IO.Path.Combine(repoRoot, "SkeuomorphCore", "Glyphs", "BitmapSources"),
           System.IO.Path.Combine(repoRoot, "SkeuomorphCore", "Glyphs")
       };

       var normalized = NormalizeMapName(layout);
       var fileName = normalized switch
       {
           "SevenSegment" => "SevenSegment.json",
           "NineSegmentSlash" => "NineSegmentSlash.json",
           "NineSegmentBackslash" => "NineSegmentBackslash.json",
           "NineSegmentSlashAlt" => "NineSegmentSlashAlt.json",
           "NineSegmentBackslashAlt" => "NineSegmentBackslashAlt.json",
           "TenSegment" => "TenSegment.json",
           "FourteenSegment" => "FourteenSegment.json",
           "Rectangle5x7" => "Rectangle5x7.json",
           "DotMatrix8x8" => "DotMatrix8x8.json",
           "SixteenSegment" => "SixteenSegment.json",
           _ => null
       };

       if (fileName is null)
       {
           return null;
       }

       foreach (var directory in candidateDirectories)
       {
           var candidatePath = System.IO.Path.Combine(directory, fileName);
           if (File.Exists(candidatePath))
           {
               return candidatePath;
           }
       }

       foreach (var directory in candidateDirectories)
       {
           var jsonCandidate = System.IO.Path.Combine(directory, fileName);
           if (File.Exists(jsonCandidate))
           {
               return jsonCandidate;
           }
       }

       return System.IO.Path.Combine(candidateDirectories[0], fileName);
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