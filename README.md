# DigitalDisplay
Before I became a software engineer, I was an Electronics Technician.

![Image of real seven segment display](/docs/leds.jpg)

Stock Windows UI elements are boring, and poorly suited for machinery HMIs. So I implemented a digital numeric display rendered in WPF. It behaves exactly like a real seven-segment module. Then I added some Up/Down goodness, with Touch applications in mind. Nothing fancy here, no MVVM or multithreading, they are just UserControls for now.

![Gif of action](/docs/display.gif)

![Gif of different colors](/docs/Luminescent.gif)

## TODO:

* Add negative value support
* Add other numeric input support besides double
* Color changing
* Dependency properties and other goodness so the usercontrols play nicely
* Calculator demo
* Error Handling
* Test cases
