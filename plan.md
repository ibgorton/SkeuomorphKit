# Performance fix plan

Status: Active — the 16-segment editor preview geometry was corrected, and the 14-segment preview now uses the same anchored segment lattice without the two unused bits so the editor matches the canonical layout model.

## Goal
Reduce repeated allocations and improve maintainability in the shared display logic while preserving the existing runtime behavior and test coverage.

## Completed work

### 1) Optimize SevenMap segment generation
- Replaced the per-call reverse/copy allocation pattern with direct writes into the destination array.
- Kept the unsupported-character behavior as a full blanking pass, without stale bits remaining in the segment buffer.
- Covered this with unit tests for supported and unsupported input.

### 2) Simplify DisplayValueFormatter parsing
- Switched the formatting path to a more deliberate `decimal`-based split of magnitude and fraction.
- Preserved invariant formatting and avoided scientific notation for tiny fractions.
- Added coverage for zero/negative-zero edge cases.

### 3) Make NumericDisplay input processing explicit and cheaper
- Replaced a set of layout magic numbers with named display-capacity constants.
- Centralized the character assignment through a smaller helper so the sign and decimal positions are easier to reason about.
- Kept the clamping and module assignment behavior stable while making the code clearer.

### 4) Simplify DisplayButtonState boundary logic
- Replaced the epsilon/Abs comparison with direct min/max boundary checks.
- Made the disable decisions explicit and easier to test.
- Added cases for values below minimum and above maximum.

### 5) Correct the 14-segment editor preview geometry
- Replaced the generic 7x2 grid with the same anchored segment-lattice approach used for the 16-seg editor.
- Kept the canonical 14-bit ordering intact while omitting the final two unused layout bits so the preview matches the device model.
- Verified the visual editor still builds cleanly and the core layout regression suite remains green.

## Validation
- `dotnet test SkeuomorphCore.Tests/SkeuomorphCore.Tests.csproj --nologo`
- `dotnet build SkeuomorphKit.sln --nologo`

Both succeeded with 0 failing tests and 0 build errors.

## Shared glyph library
- Added a canonical `GlyphLibrary` in `SkeuomorphCore` so each display layout consumes the same glyph catalog rather than maintaining independent character tables.
- `RectangleMap` now delegates to the shared matrix generator, while `SixteenMap` continues to translate the same supported glyph set into its device-specific segment pattern.
- This keeps the code open for additional layouts such as 14-segment, 7-segment, or custom font families without duplicating the character definition set.

### Compact bitmask refactor
- Replaced the large `bool[]` dictionaries with small numeric masks (`byte` for 7-seg, `ushort` for 16-seg) and convert to `bool[]` only at the boundary.
- Kept the display-order semantics intact while making the data tables much easier to scan and maintain.
- Validated that both the host-order and the character-map regressions still pass under the existing unit suite.

## Follow-up
- Confirm the 16-segment editor preview matches the physical 3x3 lattice and the host’s actual WPF segment shapes.
- Continue validating the generic character-map editor against the supported layout sets and the existing WPF display behavior.
- If a specific 14-segment hardware reference is identified, compare the compact canonical lattice against that board to confirm whether any final per-segment offsets are needed.
- If we continue this effort later, the next likely performance work is to isolate more display logic from the WPF-specific control layer so the reusable display engine can be moved to a cross-platform host without extra UI churn.
