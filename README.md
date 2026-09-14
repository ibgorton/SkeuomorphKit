# DigitalDisplay
Before I became a software engineer, I was an Electronics Technician.

## Web-ready canonical data model

The runtime no longer depends on any particular host toolkit. The canonical map definitions live as JSON under `SkeuomorphCore/Glyphs/Maps`, and the same catalog can be consumed by browser-based tools such as React, Angular, or plain JavaScript.

The JSON schema is intentionally neutral:

- `Name`, `Kind`, `Version`, `SegmentCount`
- `BitOrder` for hardware remapping
- `Layout` metadata for editor and rendering geometry
- `Characters` for the actual glyph masks/bitmap data

This means a web demo can import the exact same map definitions without depending on the Avalonia editor or any .NET-only runtime types.

## Terminology

Use these terms consistently:

- `Glyph map` or `display map`: the canonical hardware/device definition for a family (7-segment, 9-segment, bitmap, etc.).
- `Display profile`: the runtime view over a glyph map that exposes supported symbols and sizing information to application code.
- `Layout`: the visual geometry metadata used by the editor or a toolkit to draw the segments.
- `Glyph` or `symbol`: one character-level entry, such as `A`, `.`, `:`, or `-`, which can be combined into larger strings by the consuming display system.

A "segment device" is too vague and host-specific; the project uses the more precise canonical language above.

![Image of real seven segment display](/docs/leds.jpg)

Stock Windows UI elements are boring, and poorly suited for machinery HMIs. So I implemented a digital numeric display rendered in WPF. It behaves exactly like a real seven-segment module. Then I added some Up/Down goodness, with Touch applications in mind. Nothing fancy here, no MVVM or multithreading, they are just UserControls for now.

![Gif of action](/docs/display.gif)

![Gif of colors](/docs/Luminescent.gif)

## Reference glyph data

The segment masks and ordering used by the library follow the reference implementation from dmadison/led-segment-ascii:
https://github.com/dmadison/led-segment-ascii

This project intentionally matches the upstream 7-segment and 16-segment bit order so the same ASCII glyph conventions can be shared across hardware-oriented display implementations. The upstream project is licensed under the MIT License (Copyright © 2017 David Madison), and its source is used here as a reference model.

The 8x8 dot-matrix bitmap glyphs are also informed by the Fontino project:
https://github.com/rene-d/fontino

Fontino provided a useful reference for 8x8 bitmap layouts and extended character coverage, and the embedded glyph data in this library was adapted into the project’s own C# source model rather than used as a runtime dependency.

## TODO:

* Add negative value support
* Add other numeric input support besides double
* Color changing
* Dependency properties and other goodness so the usercontrols play nicely
* Calculator demo
* Error Handling
* Test cases
