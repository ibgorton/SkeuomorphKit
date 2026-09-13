using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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

        LayoutPicker.ItemsSource = DisplayCharacterProfiles.AllNames;
        LayoutPicker.SelectedIndex = 1;
        CharacterInput.Text = "A";
        CharacterInput.TextChanged += CharacterInput_TextChanged;
        CharacterEnabledToggle!.IsCheckedChanged += CharacterEnabledToggle_IsCheckedChanged;
        LayoutPicker.SelectionChanged += (_, _) => RefreshLayout();

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

    private void CharacterInput_TextChanged(object? sender, EventArgs e)
    {
        UpdateCharacterEnabledToggle();
    }

    private void UpdateCharacterEnabledToggle()
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var inputText = CharacterInput.Text ?? string.Empty;
        var character = inputText.Length > 0 ? inputText[0] : 'A';
        CharacterEnabledToggle!.IsChecked = DisplayCharacterProfiles.IsCharacterEnabled(selected, character);
    }

    private void LoadButton_Click(object? sender, RoutedEventArgs e)
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var inputText = CharacterInput.Text ?? string.Empty;
        var character = inputText.Length > 0 ? inputText[0] : 'A';
        var enabled = CharacterEnabledToggle!.IsChecked == true;
        DisplayCharacterProfiles.SetCharacterEnabled(selected, character, enabled);
        PersistCharacterAvailability(selected, character, enabled);
        RefreshLayout();
    }

    private void CharacterEnabledToggle_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var inputText = CharacterInput.Text ?? string.Empty;
        var character = inputText.Length > 0 ? inputText[0] : 'A';
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
        LoadCharacterIntoGrid(selected, nextCharacter.ToString());
        UpdateMaskText();
    }

    private void RefreshLayout()
    {
        var selected = LayoutPicker.SelectedItem as string ?? DisplayCharacterProfiles.AllNames.First();
        var profile = DisplayCharacterProfiles.Get(selected);
        var inputText = CharacterInput.Text ?? string.Empty;
        var character = inputText.Length > 0 ? inputText[0] : 'A';
        _segmentCount = profile.SegmentCount;

        BuildSegmentGrid(profile);
        BuildCharacterMap(profile.Name);
        UpdateCharacterEnabledToggle();
        LoadCharacterIntoGrid(profile.Name, character.ToString());
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
                LoadCharacterIntoGrid(layout, character.ToString());
                UpdateMaskText();
            };

            CharacterMapPanel.Children.Add(button);
        }
    }

    private static object CreateCharacterMapContent(char character, bool isDisabled)
    {
        var label = character == ' ' ? "space" : character.ToString();
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
        var candidates = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?.,:;+-/=\\_[](){}<>|#@%&*'\"$^~".ToCharArray();
        var characters = new List<char>();

        foreach (var character in candidates)
        {
            var supported = DisplayCharacterProfiles.Get(layout).IsSupported(character);

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
            case NineSegmentDisplayProfile:
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
                    new[] { new Point(1, 9), new Point(2, 8), new Point(8, 8), new Point(9, 9), new Point(8, 10), new Point(2, 10) },
                    new[] { new Point(2, 2), new Point(3.4, 2), new Point(8, 6.6), new Point(8, 8), new Point(6.6, 8), new Point(2, 3.4) },
                    new[] { new Point(8, 16), new Point(8, 14.6), new Point(3.4, 10), new Point(2, 10), new Point(2, 11.4), new Point(6.6, 16) }
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
            case TenSegmentDisplayProfile:
            {
                var canvas = new Canvas
                {
                    Width = 220,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Match the canonical geometry in SkeuomorphCore/Glyphs/10-segment_labeled.svg.
                // The 10-seg reference is a 7-segment core plus the common center/diagonal additions H and I.
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
            case FourteenSegmentDisplayProfile:
            {
                var canvas = new Canvas
                {
                    Width = 220,
                    Height = 260,
                    Background = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Match the canonical 14-segment geometry in SkeuomorphCore/Glyphs/14-segment_labeled_clockwise.svg.
                // This layout is a 7-segment core plus the common diagonal/center segments used by real LCD/LED parts.
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
            case DotMatrix8x8DisplayProfile:
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
                        Width = 16,
                        Height = 16,
                        IsChecked = false,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(1),
                        Tag = index,
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Content = ""
                    };

                    button.Click += SegmentButton_Click;
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, column);
                    SegmentGrid.Children.Add(button);
                    _segmentButtons.Add(button);
                }

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

                // Physical center order as described by the hardware layout:
                // a1, a2, b, c, d1, d2, e, f, j, h, k, g2, l, i, m, g1.
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
                    var button = CreatePolygonSegmentButton(segment.Index, segment.Points, new Point(18, 14), 18.0);
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
        var maskExpression = BuildMaskExpression(selected, mask);

        MaskValueText.Text = $"{text} :: mask = {mask} (0x{mask:X})";
        CodePreview.Text = $"namespace SkeuomorphCore;\n\npublic static class GeneratedGlyphMaps\n{{\n    public static readonly CharacterMap {selected}Map = new({width}, {height});\n\n    static GeneratedGlyphMaps()\n    {{\n        {selected}Map.Set('{text}', {maskExpression});\n    }}\n}}\n\n// bits: [{bits}]";
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

       var character = characterText[0];
       var mask = 0UL;
       for (var index = 0; index < _segmentButtons.Count; index++)
       {
           if (_segmentButtons[index].IsChecked == true)
           {
               mask |= 1UL << index;
           }
       }

       var filePath = ResolveMapFileForLayout(selected);
       if (filePath is null)
       {
           CodePreview.Text = $"No map file is defined for {selected}. Save is only supported for the segment-based glyph maps.";
           MaskValueText.Text = "Save skipped";
           return;
       }

       try
       {
           var source = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
           var updated = UpdateMapFileCharacterEntry(source, character, mask, selected);
           File.WriteAllText(filePath, updated);
           ApplyRuntimeMapUpdate(selected, character, mask);
           RefreshLayout();
           ClearDirty();
           CodePreview.Text = updated;
           MaskValueText.Text = $"Saved {character} :: mask = {mask} (0x{mask:X}) to {System.IO.Path.GetFileName(filePath)}";
       }
       catch (Exception ex)
       {
           CodePreview.Text = ex.Message;
           MaskValueText.Text = $"Save failed: {ex.Message}";
       }
   }

   private static void ApplyRuntimeMapUpdate(string layout, char character, ulong mask)
   {
       var mapTypeName = layout switch
       {
           "SevenSegment" => "SevenMap",
           "NineSegment" => "NineMap",
           "TenSegment" => "TenMap",
           "FourteenSegment" => "FourteenMap",
           "Rectangle5x7" => "GlyphLibrary",
           "SixteenSegment" => "SixteenMap",
           _ => null
       };

       if (mapTypeName is null)
       {
           return;
       }

       var mapType = typeof(DisplayCharacterProfiles).Assembly.GetType($"SkeuomorphCore.{mapTypeName}");
       if (mapType is null)
       {
           return;
       }

       var fieldName = mapTypeName switch
       {
           "SevenMap" => "SevenMasks",
           "NineMap" => "NineMasks",
           "TenMap" => "TenMasks",
           "FourteenMap" => "FourteenMasks",
           "GlyphLibrary" => "Patterns",
           "SixteenMap" => "SixteenMasks",
           _ => null
       };

       if (fieldName is null)
       {
           return;
       }

       var field = mapType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
       if (field is null)
       {
           return;
       }

       var dictionary = field.GetValue(null) as IDictionary;
       if (dictionary is null)
       {
           return;
       }

       var value = Convert.ChangeType(mask, field.FieldType.GetGenericArguments()[1]);
       dictionary[character] = value;
   }

   private static void PersistCharacterAvailability(string layout, char character, bool enabled)
   {
       if (layout == "Rectangle5x7")
       {
           return;
       }

       var filePath = ResolveMapFileForLayout(layout);
       if (filePath is null)
       {
           return;
       }

       var source = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
       var updated = UpdateMapDisabledCharacters(source, layout, character, enabled);
       File.WriteAllText(filePath, updated);
   }

   private static string? ResolveMapFileForLayout(string layout)
   {
       var repoRoot = ResolveRepositoryRoot();
       var coreDirectory = System.IO.Path.Combine(repoRoot, "SkeuomorphCore", "Glyphs");
       var fileName = layout switch
       {
           "SevenSegment" => "SevenMap.cs",
           "NineSegment" => "NineMap.cs",
           "TenSegment" => "TenMap.cs",
           "FourteenSegment" => "FourteenMap.cs",
           "Rectangle5x7" => "GlyphLibrary.cs",
           "SixteenSegment" => "SixteenMap.cs",
           _ => null
       };

       return fileName is null ? null : System.IO.Path.Combine(coreDirectory, fileName);
   }

   private static string BuildMaskExpression(string layout, ulong mask)
   {
       var segments = layout switch
       {
           "SevenSegment" => new[] { "SegmentA", "SegmentB", "SegmentC", "SegmentD", "SegmentE", "SegmentF", "SegmentG", "SegmentDP" },
           "NineSegment" => new[] { "SegmentA", "SegmentB", "SegmentC", "SegmentD", "SegmentE", "SegmentF", "SegmentG", "SegmentH", "SegmentI" },
           "TenSegment" => new[] { "SegmentA", "SegmentB", "SegmentC", "SegmentD", "SegmentE", "SegmentF", "SegmentG", "SegmentH", "SegmentI", "SegmentJ" },
           "FourteenSegment" => new[] { "SegmentA", "SegmentB", "SegmentC", "SegmentD", "SegmentE", "SegmentF", "SegmentG", "SegmentH", "SegmentJ", "SegmentK", "SegmentL", "SegmentM", "SegmentN", "SegmentP" },
           "Rectangle5x7" => null,
           "SixteenSegment" => new[] { "SegmentA", "SegmentB", "SegmentC", "SegmentD", "SegmentE", "SegmentF", "SegmentG", "SegmentH", "SegmentI", "SegmentJ", "SegmentK", "SegmentL", "SegmentM", "SegmentN", "SegmentO", "SegmentP" },
           _ => null
       };

       if (segments is null)
       {
           return layout == "Rectangle5x7" ? $"0x{mask:X}UL" : mask.ToString();
       }

       var active = new List<string>();
       for (var index = 0; index < segments.Length; index++)
       {
           if ((mask & (1UL << index)) != 0)
           {
               active.Add(segments[index]);
           }
       }

       return active.Count == 0 ? "Mask()" : $"Mask({string.Join(", ", active)})";
   }

   private static string UpdateMapDisabledCharacters(string source, string layout, char character, bool enabled)
   {
       if (layout == "Rectangle5x7")
       {
           return source;
       }

       var mapName = layout switch
       {
           "SevenSegment" => "SevenMap",
           "NineSegment" => "NineMap",
           "TenSegment" => "TenMap",
           "FourteenSegment" => "FourteenMap",
           "SixteenSegment" => "SixteenMap",
           _ => throw new InvalidOperationException($"Unsupported layout: {layout}")
       };

       var fieldPattern = @"(?ms)^\s*public\s+static\s+(?:readonly\s+)?HashSet<char>\s+DisabledCharacters\s*=\s*(?<value>\[[^\]]*\]|new\s*\(\)|new\s*HashSet<char>\s*\{[^}]*\})\s*;";
       var matches = Regex.Matches(source, fieldPattern);

       var currentValue = matches.Count > 0 ? matches[0].Groups["value"].Value : "[]";
       var chars = ParseHashSetCharacters(currentValue);
       var normalized = NormalizeCharacterLiteral(character);
       if (enabled)
       {
           chars.Remove(normalized);
       }
       else
       {
           chars.Add(normalized);
       }

       var newline = DetectNewline(source);
       var cleanedSource = Regex.Replace(source, fieldPattern, string.Empty);
       var insertionIndex = cleanedSource.IndexOf("public const int SegmentCount", StringComparison.Ordinal);
       if (insertionIndex < 0)
       {
           throw new InvalidOperationException($"Could not locate the segment count in {mapName}.");
       }

       var segmentIndex = cleanedSource.IndexOf(";", insertionIndex);
       if (segmentIndex < 0)
       {
           throw new InvalidOperationException($"Could not insert disabled-character tracking into {mapName}.");
       }

       var before = cleanedSource.Substring(0, segmentIndex + 1);
       var after = cleanedSource.Substring(segmentIndex + 1);
       var serialized = chars.Count == 0 ? "[]" : $"[{string.Join(", ", chars.OrderBy(c => c).Select(c => $"'{EscapeCharacterLiteral(c)}'"))}]";
       return before + newline + "    public static readonly HashSet<char> DisabledCharacters = " + serialized + ";" + newline + after;
   }

   private static HashSet<char> ParseHashSetCharacters(string literal)
   {
       var result = new HashSet<char>();
       if (string.IsNullOrWhiteSpace(literal) || literal == "[]" || literal == "new()" || literal == "new HashSet<char>()")
       {
           return result;
       }

       var match = Regex.Match(literal, @"\[(?<items>.*)\]|new\s*HashSet<char>\s*\{(?<items>.*)\}", RegexOptions.Singleline);
       if (!match.Success)
       {
           return result;
       }

       var body = match.Groups["items"].Value;
       var entries = body.Split(',');
       foreach (var entry in entries)
       {
           var text = entry.Trim();
           if (text.Length == 0)
           {
               continue;
           }

           var literalMatch = Regex.Match(text, @"^'(?<value>(?:\\'|\\\\|\\n|\\r|\\t|[^'])*)'$", RegexOptions.Singleline);
           if (!literalMatch.Success)
           {
               continue;
           }

           result.Add(ParseCharacterLiteral(literalMatch.Groups["value"].Value));
       }

       return result;
   }

   private static char NormalizeCharacterLiteral(char character)
   {
       return char.ToUpperInvariant(character);
   }

   private static string UpdateMapFileCharacterEntry(string source, char character, ulong mask, string layout)
   {
       var mapName = layout switch
       {
           "SevenSegment" => "SevenMap",
           "NineSegment" => "NineMap",
           "TenSegment" => "TenMap",
           "FourteenSegment" => "FourteenMap",
           "Rectangle5x7" => "GlyphLibrary",
           "SixteenSegment" => "SixteenMap",
           _ => throw new InvalidOperationException($"Unsupported layout: {layout}")
       };

       var dictionaryPattern = @"(?<prefix>private\s+static\s+readonly\s+Dictionary<char,\s*[^>]+>\s*\w+\s*=\s*new\(\)\s*\{\s*)" +
                               @"(?<body>.*?)" +
                               @"(?<suffix>\s*\};)";

       var match = Regex.Match(source, dictionaryPattern, RegexOptions.Singleline);
       if (!match.Success)
       {
           throw new InvalidOperationException($"Could not locate the {mapName} dictionary in the map file.");
       }

       var body = match.Groups["body"].Value;
       var lines = body.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
       var updatedLines = new List<string>();
       var replaced = false;
       var maskExpression = BuildMaskExpression(layout, mask);
       var newline = DetectNewline(source);

       foreach (var line in lines)
       {
           var trimmed = line.Trim();
           if (trimmed.Length == 0)
           {
               updatedLines.Add(line);
               continue;
           }

           var entryMatch = Regex.Match(trimmed, @"^\[\s*'(?<literal>(?:\\'|[^'])*)'\s*\]\s*=\s*(?<value>.+?)\s*,?\s*$");
           if (!entryMatch.Success)
           {
               updatedLines.Add(line);
               continue;
           }

           var key = ParseCharacterLiteral(entryMatch.Groups["literal"].Value);
           if (key == character)
           {
               updatedLines.Add($"        ['{EscapeCharacterLiteral(character)}'] = {maskExpression},");
               replaced = true;
               continue;
           }

           updatedLines.Add(line);
       }

       if (!replaced)
       {
           updatedLines.Insert(updatedLines.Count, $"        ['{EscapeCharacterLiteral(character)}'] = {maskExpression},");
       }

       var prefix = match.Groups["prefix"].Value;
       var suffix = match.Groups["suffix"].Value;
       return source.Substring(0, match.Index) + prefix + string.Join(newline, updatedLines) + newline + suffix + source.Substring(match.Index + match.Length);
   }

   private static char ParseCharacterLiteral(string literal)
   {
       if (string.IsNullOrEmpty(literal))
       {
           return ' ';
       }

       var result = new List<char>();
       for (var index = 0; index < literal.Length; index++)
       {
           var ch = literal[index];
           if (ch != '\\' || index + 1 >= literal.Length)
           {
               result.Add(ch);
               continue;
           }

           var next = literal[++index];
           result.Add(next switch
           {
               '\'' => '\'',
               '\\' => '\\',
               'n' => '\n',
               'r' => '\r',
               't' => '\t',
               _ => next
           });
       }

       return result.Count > 0 ? result[0] : ' ';
   }

   private static string EscapeCharacterLiteral(char character)
   {
       return character switch
       {
           '\\' => "\\\\",
           '\'' => "\\'",
           '\n' => "\\n",
           '\r' => "\\r",
           '\t' => "\\t",
           _ => character.ToString()
       };
   }

   private static string DetectNewline(string source)
   {
       if (source.Contains("\r\n", StringComparison.Ordinal))
       {
           return "\r\n";
       }

       if (source.Contains('\n'))
       {
           return "\n";
       }

       return Environment.NewLine;
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