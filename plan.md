# Current project plan

Status: Active. The project is now canonicalized around JSON-backed map definitions and the core display profiles, with no compatibility layer for legacy class names.

## Completed
- Removed the legacy concrete glyph map classes (`SevenMap`, `NineMap`, `TenMap`, `FourteenMap`, `SixteenMap`, `RectangleMap`) from the runtime surface.
- Kept the canonical built-ins as JSON-first definitions under `SkeuomorphCore/Glyphs/Maps`.
- Moved editor/runtime lookup to canonical names only (`SevenSegment`, `NineSegmentSlash`, `NineSegmentBackslash`, `NineSegmentSlashAlt`, `NineSegmentBackslashAlt`, `TenSegment`, `FourteenSegment`, `Rectangle5x7`, `DotMatrix8x8`, `SixteenSegment`).
- Updated the display/profile registry and editor logic to treat canonical names as the only public contract.
- Added LED glow styling to the segment cells with a more physical lit/unlit treatment in the editor panel.
- Verified the test suite still passes under .NET 10.

## Current direction
- Keep all display definitions JSON-driven and registry-backed.
- Treat the editor as a tool for editing canonical map state, not as a compatibility layer.
- Use layout metadata from the JSON files for all segment-grid geometry.
- Keep `SkeuomorphDisplay` as a thin adapter layer that consumes the canonical registry instead of defining a second display model.
- Ensure each nine-segment variant maps to its matching SVG geometry instead of duplicating a stale layout.
- Expose the canonical map catalog as a neutral JSON contract so it can be consumed by web frameworks (React, Angular, browser demos) without depending on .NET UI code.
- Use the term "glyph map" (or "display map") for the canonical device definition, and reserve "display profile" for the runtime/profile view; avoid calling a map a generic "segment device" in user-facing docs.
- Support custom map creation and bit-order remapping directly in the editor, including editing individual symbols such as `.` / `:` / `-` as first-class glyph entries that can be composed into longer strings.
- Remove stale host-specific color/brightness metadata from the display abstraction; it does not belong in the canonical contract.

## Architecture decision

Adopt a shared-core + host-specific-ui model, not a full migration into a single toolkit.

- `SkeuomorphCore` remains the runtime-neutral backend: canonical JSON catalog, display-profile registry, validation, and bitmap/segmented glyph logic.
- The editor stays host-specific and is intentionally implemented as an Avalonia app because it already matches the project’s custom drawing needs and works without tying the runtime to WPF.
- Browser/web demos remain a separate, framework-neutral consumer of the canonical JSON contract.
- Hardware or signage integrations can later plug into the same core contract via adapter layers, without requiring the core library to know about WPF, React, Angular, Avalonia, or any specific OS UI.

Why Avalonia over MAUI:
- The project’s custom segment geometry, glow styling, and direct control-level drawing are closer to a custom-rendering desktop toolkit than a typical app shell.
- Avalonia gives us a portable desktop host with a lower-friction path for custom controls and layouts while keeping the core backend platform-neutral.
- MAUI is useful when the goal is app-first native integration, but it is a worse fit for a custom display-editor surface that depends on fine-grained rendering and a toolkit-neutral backend.

This is a deliberate split:
- `Core` = neutral data + logic
- `Editor` = Avalonia-hosted authoring tool
- `Web` = browser/demo consumer of the same JSON contract
- `Hardware/Signage` = future adapters built on the same canonical models

## Immediate next steps
- Add or revise any remaining map definitions that still need explicit JSON geometry metadata.
- Continue validating the canonical segment ordering for each shipped layout.
- Keep the public API free of old names; pre-release code should not carry compatibility shims.
- Publish a lightweight browser demo that loads the same canonical map JSON and renders segment/bitmap glyphs in a framework-neutral way.
- Expand the editor’s custom schema tooling to include profile templates and save-as-new-map workflows for custom and remapped display families.

## Validation
- `dotnet test SkeuomorphCore.Tests/SkeuomorphCore.Tests.csproj --nologo`
