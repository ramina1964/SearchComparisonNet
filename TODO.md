# TODO / Follow-up Tasks

Tracked follow-up work deferred from the `refactor/di-and-structure` branch to keep
each branch focused on a single theme.

## Completed

### Performance & concurrency investigation (`perf/concurrency`, merged)
Measurement-driven pass over the search/data-generation path. All conclusions were
validated with BenchmarkDotNet (CPU + `MemoryDiagnoser`) and the full test suite.

- **CPU:** profiling showed `LinearSearch.FindItem` dominating and `ObservableCollection<int>`
  indexer overhead in the hot path. Replaced `ObservableCollection<int>` with raw `int[]`
  in `SearchBase`/`LinearSearch`/`BinarySearch`.
- **Memory:** `DataGenerator.GenerateData()` was rewritten from `HashSet<int>` rejection
  sampling to a compact `BitArray` membership bitmap (no `HashSet`, no `Array.Sort`),
  measured 3.7x-4.8x faster with sharply reduced LOH churn.
- **async/await & disposal/lifetime:** answered by measurement rather than assumption.
  `SimulationSessionBenchmarks` (repeated full sessions) reported zero retained live objects;
  `CancellationLifetimeBenchmarks` (CTS create/cancel/dispose cycle) showed only collectible
  transients. Conclusion: no eager-disposal or async restructuring is warranted; the hot path
  is pure compute and the existing `Task.Run` plumbing only serves UI responsiveness.
- Benchmarks live in `BenchmarkSuite1` (opted out of central package management).

### Solution format migration + project cleanup (`chore/slnx-migration`)
- Migrated `SearchComparisonNet.sln` to `SearchComparisonNet.slnx` via `dotnet sln migrate`;
  removed the legacy `.sln` and updated the CI workflow's restore/build/test steps.
- Removed an empty `Properties\` folder include from `Kernel.csproj`.
- Fixed `<TargetFrameworks>` -> `<TargetFramework>` in `BenchmarkSuite1.csproj`.
- Removed the now-dead `global using System.Collections.ObjectModel;` (`ObservableCollection`
  was eliminated from the Kernel during the `int[]` refactor).
- Confirmed modern C# defaults are already centralized in `Directory.Build.props`
  (`LangVersion=latest`, `Nullable=enable`, `ImplicitUsings=enable`, `EnforceCodeStyleInBuild=true`)
  and the solution builds warning-free with all 56 tests passing.

### GUI project review (`refctor/gui`, merged via PR #14)
Focused pass over `SearchComparisonNet.GUI` for readability, DRY, and modernization.

- Removed the unused `NuGet.Configuration` package reference from `SearchComparisonNet.GUI.csproj`
  (confirmed no usage anywhere in the GUI source).
- Audited the remaining package references (`CommunityToolkit.Mvvm`, `FluentValidation`,
  `Microsoft.Extensions.DependencyInjection`) - all are in active use.
- Confirmed the WPF/build settings are minimal and correct for `net10.0-windows`.
- Solution builds warning-free with the full test suite (132 tests) passing.

### Extend test coverage and xUnit v3 idioms (`test/expand-coverage`, merged via PR #15)
Broadened automated coverage beyond the search algorithms and adopted xUnit v3 features.

- Added `DataGeneratorTests` (output invariants: exact count, in-range values, strictly
  ascending, unique; plus `NextRandomNo` range and single-element datasets).
- Added `InputValidationTests` (FluentValidation rules for entries/searches: required,
  non-integer, out-of-range, in-range).
- Added `ViewModelBaseTests` locking in the current `INotifyDataErrorInfo` no-op contract.
- Added `MainViewModelCancellationTests` for cancel/retry edges around `SimulateCommand`.
- Fixed a latent break: PR #14 removed the dead `LinearSearchResults`/`BinarySearchResults`
  from `MainViewModel` but left `MainViewModelSimulationTests` referencing them, so the test
  project could not compile from a clean build; updated it to the average-iteration +
  `IsSearchEnabled` contract.
- Adopted `TheoryData<T>`, `Assert.Multiple`, and `[ClassData]`/`[MemberData]`.
- Validated with a clean full run: 185 tests passing, 0 warnings.

## Remaining backlog

> Single source of truth for outstanding work. These items originate from the code review in
> [`docs/review/solution-review.md`](docs/review/solution-review.md); see that document for the
> full findings and rationale behind each one. Every change ships on its own branch as a PR into
> `main` (including docs-only changes).

### Approval items (code-level findings)

1. **G-4** - use (or remove) the DI container. *(Pairs well with test-infra Option C below.)*
2. **G-6** - enforce the declared product-range validation rule.
3. **G-7** - fix `HasErrors` swallowing exceptions and returning `true`.
   *(Note: `ViewModelBaseTests` currently asserts the no-op `HasErrors => false` contract; those
   tests must be updated in lockstep when this behavior changes.)*
4. **K-2** - make `NoOfEntries` consistent with `Data` (read-only / private setter).
5. **K-3 (values)** - only if `100_00` / grouping were genuine typos (reformatting already
   applied; values intentionally preserved per the PR #2 decision).
6. **K-5** - move `NextRandomNo` off the search type onto the generator.

### Test-infrastructure options (deferred)

- **Option B** - extract the cancellation-aware iteration logic into a Kernel-side (or plain,
  `net10.0`-referenceable) helper and unit-test the G-5 cancellation contract (token honored,
  `OperationCanceledException` thrown) without WPF. Moderate effort.
- **Option C** - full VM testability: re-target the test project to `net10.0-windows`, add a GUI
  `ProjectReference`, refactor `MainViewModel` for constructor injection (pairs with G-4), and add
  a UI-`SynchronizationContext` fixture to exercise dispatcher marshaling. Largest effort.

## Suggested ordering & effort

- **G-7** is small, well-scoped, and closely tied to recently added tests - a natural next step.
- **K-2** and **K-5** are small Kernel encapsulation/placement fixes and are low-risk.
- **G-6** is a focused validation change that pairs with the existing `InputValidation` tests.
- **G-4** + **Option C** form the largest architectural effort and are best tackled together.
- **K-3 (values)** is likely a no-op unless the grouping is confirmed to be a genuine typo.
