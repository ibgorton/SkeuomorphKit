# Performance fix plan

Status: Completed.

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

## Validation
- `dotnet test SkeuomorphCommon.Tests/SkeuomorphCommon.Tests.csproj --nologo`
- `dotnet build SkeuomorphKit.sln --nologo`

Both succeeded with 0 failing tests and 0 build errors.

## Follow-up
If we continue this effort later, the next likely performance work is to isolate more display logic from the WPF-specific control layer so the reusable display engine can be moved to a cross-platform host without extra UI churn.
