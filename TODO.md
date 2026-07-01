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

## Remaining backlog

## 1. Extend test coverage and leverage xUnit v3 features
**Suggested branch:** `test/expand-coverage`

> Note: the test project already targets xUnit v3 (`xunit.v3` 3.2.2 with `xunit.runner.visualstudio` 3.1.5), so this is about broadening coverage and adopting v3 idioms, not upgrading the framework.

- Add tests beyond `LinearSearch`/`BinarySearch`: `InputValidation` rules, `ViewModelBase` error tracking (`HasErrors`/`GetErrors`), and `DataGenerator` output.
- Adopt xUnit v3 features where they improve clarity: strongly-typed `TheoryData<T>`, `Assert.Multiple`, and `[ClassData]`/`[MemberData]` for larger datasets.
- Consider edge cases: empty/single-element datasets, duplicate values, and cancellation paths in the simulation.
- Validate with a full test run.

## Suggested ordering & effort

- **One task remains.** Expanding test coverage (`test/expand-coverage`) is incremental and low-risk - it adds tests without changing production code.
