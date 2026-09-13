# Current project plan

Status: Active. The project is now canonicalized around JSON-backed map definitions and the core display profiles, with no compatibility layer for legacy class names.

## Completed
- Removed the legacy concrete glyph map classes (`SevenMap`, `NineMap`, `TenMap`, `FourteenMap`, `SixteenMap`, `RectangleMap`) from the runtime surface.
- Kept the canonical built-ins as JSON-first definitions under `SkeuomorphCore/Glyphs/Maps`.
- Moved editor/runtime lookup to canonical names only (`SevenSegment`, `NineSegmentSlash`, `NineSegmentBackslash`, `NineSegmentSlashAlt`, `NineSegmentBackslashAlt`, `TenSegment`, `FourteenSegment`, `Rectangle5x7`, `DotMatrix8x8`, `SixteenSegment`).
- Updated the display/profile registry and editor logic to treat canonical names as the only public contract.
- Verified the test suite still passes under .NET 10.

## Current direction
- Keep all display definitions JSON-driven and registry-backed.
- Treat the editor as a tool for editing canonical map state, not as a compatibility layer.
- Use layout metadata from the JSON files for all segment-grid geometry.
- Ensure each nine-segment variant maps to its matching SVG geometry instead of duplicating a stale layout.

## Immediate next steps
- Add or revise any remaining map definitions that still need explicit JSON geometry metadata.
- Continue validating the canonical segment ordering for each shipped layout.
- Keep the public API free of old names; pre-release code should not carry compatibility shims.

## Validation
- `dotnet test SkeuomorphCore.Tests/SkeuomorphCore.Tests.csproj --nologo`
