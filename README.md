# DigitalDisplay
Before I became a software engineer, I was an Electronics Technician.

![Image of real seven segment display](/docs/leds.jpg)

Stock Windows UI elements are boring, and poorly suited for machinery HMIs. So I implemented a digital numeric display rendered in WPF. It behaves exactly like a real seven-segment module. Then I added some Up/Down goodness, with Touch applications in mind. Nothing fancy here, no MVVM or multithreading, they are just UserControls for now.

![Gif of action](/docs/display.gif)

![Gif of colors](/docs/Luminescent.gif)

## Reference glyph data

The segment masks and ordering used by the library follow the reference implementation from dmadison/led-segment-ascii:
https://github.com/dmadison/led-segment-ascii

This project intentionally matches the upstream 7-segment and 16-segment bit order so the same ASCII glyph conventions can be shared across hardware-oriented display implementations. The upstream project is licensed under the MIT License (Copyright © 2017 David Madison), and its source is used here as a reference model.

## TODO:

* Add negative value support
* Add other numeric input support besides double
* Color changing
* Dependency properties and other goodness so the usercontrols play nicely
* Calculator demo
* Error Handling
* Test cases
