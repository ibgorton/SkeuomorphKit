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
- Ensure each nine-segment variant maps to its matching SVG geometry instead of duplicating a stale layout.
- Expose the canonical map catalog as a neutral JSON contract so it can be consumed by web frameworks (React, Angular, browser demos) without depending on .NET UI code.
- Use the term "glyph map" (or "display map") for the canonical device definition, and reserve "display profile" for the runtime/profile view; avoid calling a map a generic "segment device" in user-facing docs.
- Support custom map creation and bit-order remapping directly in the editor, including editing individual symbols such as `.` / `:` / `-` as first-class glyph entries that can be composed into longer strings.

## Immediate next steps
- Add or revise any remaining map definitions that still need explicit JSON geometry metadata.
- Continue validating the canonical segment ordering for each shipped layout.
- Keep the public API free of old names; pre-release code should not carry compatibility shims.
- Publish a lightweight browser demo that loads the same canonical map JSON and renders segment/bitmap glyphs in a framework-neutral way.
- Expand the editor’s custom schema tooling to include profile templates and save-as-new-map workflows for custom and remapped display families.

## Validation
- `dotnet test SkeuomorphCore.Tests/SkeuomorphCore.Tests.csproj --nologo`
