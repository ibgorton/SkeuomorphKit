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
- Correct the 9-segment mapping and preview to the real A..I segment family rather than a generic 16-segment subset.
- Tighten the 9-segment glyph rules so only explicit template-backed characters are supported; unsupported glyphs stay blank instead of rendering as an all-on generic fallback.
- Fix the live disabled-character toggle regression by keeping `DisabledCharacters` mutable at runtime so the editor can update the current profile without triggering `FieldAccessException` during reflection-based synchronization.

## Planned display-family roadmap

### Phase 1 — Add the next common LED family: 9-segment
- Target: real-world indicator displays that are a 7-segment base plus extra center/diagonal segments.
- Implementation: create a `NineSegmentDisplayProfile` and `NineMap` built on the same compact bitmask pattern used for 7-, 14-, and 16-segment layouts.
- Validation: add profile support tests and a geometry-focused editor preview that uses a 3x3 anchor lattice as the source of truth.
- Goal: cover the next common industrial/clock/instrument style without conflating it with the 14-seg hardware model.

### Phase 2 — Generalize matrix support beyond 5x7 and 8x8
- Target: both 5x7 and 8x8 are matrix layouts; this should become a first-class display family rather than a one-off.
- Implementation: define a reusable `MatrixDisplayProfile` abstraction that accepts width/height and a glyph source, then derive 5x7 and 8x8 profiles from it.
- Validation: add tests for resizing, supported glyph coverage, and editor point toggling.
- Goal: make arbitrary matrix displays easy to add without writing custom logic each time.

### Phase 3 — Add a dedicated bitmap/large-matrix path
- Target: 16x16 and larger graphics displays used for custom signage or informational panels.
- Implementation: allow a `BitmapDisplayProfile` or `LargeMatrixDisplayProfile` to map arbitrary glyphs or pixel sprites from a shared matrix library.
- Validation: exercise non-ASCII glyphs, scaling, and editor-hit handling on larger matrices.
- Goal: move from character displays to higher-resolution bitmap displays without changing the core architecture.

### Phase 4 — Evaluate niche segment families only when a target part exists
- Target: 12-segment, 15-segment, and specialty industrial layouts.
- Implementation: only add these when a real datasheet or part number is being modeled; they are not generic enough to include blindly.
- Validation: compare to the actual hardware pinout and physical segment placement before adopting the layout.
- Goal: avoid speculative display types that are not tied to a real product family.

### Phase 5 — Add LCD-style segmented profiles if the project expands into non-LED hardware
- Target: calculators, appliances, and instrumentation with non-LED segmented glass.
- Implementation: treat these as separate display families because segment geometry and drive logic differ from LED layouts.
- Validation: confirm against a real LCD reference sheet and handle shared state semantics separately from LED host logic.
- Goal: keep LED and LCD designs correctly separated while reusing the same profile architecture.

### Architecture rules for all future layouts
- Keep the shared glyph library and bitmask conventions independent from the host UI.
- Treat the editor as a geometry/verification tool, not as the source of truth for the hardware model.
- For segment displays, require a datasheet-backed segment ordering before finalizing the profile.
- For matrix displays, prefer a generalized profile abstraction over one-off code paths.
- Add targeted validation for each new layout: registry support, bit ordering, and editor rendering.
