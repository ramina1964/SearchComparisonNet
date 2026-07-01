# Test coverage

Coverage is collected with Coverlet's cross-platform data collector (`coverlet.collector`,
referenced by both test projects and pinned in `Directory.Packages.props`).

## How to regenerate

```powershell
dotnet test SearchComparisonNet.slnx -c Debug --collect:"XPlat Code Coverage" --results-directory TestResults
```

Each test project emits a `coverage.cobertura.xml` under `TestResults\<guid>\`. To turn the raw
Cobertura files into a browsable HTML report, install `dotnet-reportgenerator-globaltool` and run it
over `TestResults\**\coverage.cobertura.xml`.

## Latest snapshot

Measured over the full suite (185 tests: 115 Kernel + 70 ViewModel), all passing.

| Scope | Line coverage |
|-------|---------------|
| Core logic (Kernel + ViewModels, excluding the WPF shell) | **57.7%** (153/265) |
| Whole solution (including WPF views, converters, `App`, DI wiring) | **42.9%** (153/357) |

The behavioral units are essentially fully covered; the headline percentages are pulled down by
declaration-heavy or UI-shell types that are not unit-tested by design.

### Per-class line coverage

| Class | Line % | Notes |
|-------|-------:|-------|
| `Kernel.Models.DataGenerator` | 100% | data-generation invariants |
| `Kernel.Models.LinearSearch` | 100% | |
| `Kernel.Models.BinarySearch` | 100% | |
| `Kernel.Models.SearchComparisonFactory` (+ nested) | 100% | |
| `Kernel.Models.SearchItem` | 100% | |
| `Kernel.Models.DataParameters` | 100% | |
| `Kernel.Models.ProgressReportPolicy` | 100% | |
| `GUI.ViewModels.InputValidation` | 100% | FluentValidation rules |
| `GUI.ViewModels.ViewModelBase` | 100% | `INotifyDataErrorInfo` no-op contract |
| `GUI.ViewModels.MainViewModel` | 99% | simulation / cancel / validation flow |
| `Kernel.Models.SearchBase` | 40% | mostly the indexer setter / error paths |
| `Kernel.Models.ProblemConstants` | 13% | static constant/range declarations |
| `Kernel.Models.SimulationResults` | 0% | plain results DTO |
| WPF shell: `Views`, `UserControls`, `Converters`, `App`, `ServiceCollectionExtensions` | 0% | UI plumbing, not unit-tested by design |

### Untested-by-design

The WPF views/user controls (`.xaml.cs` + generated `.g.cs`), value converters, `App`, and the DI
registration extension are exercised only at runtime, not by unit tests. Bringing these under test
would require UI-level testing (see the deferred "Option C" in [`../../TODO.md`](../../TODO.md)).
