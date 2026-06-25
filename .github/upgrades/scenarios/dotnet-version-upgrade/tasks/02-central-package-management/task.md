# 02-central-package-management: Introduce Central Package Management

Adopt CPM across the solution while still on .NET 7 so the change is isolated from the framework bump. Create `Directory.Packages.props` at the repository root with `ManagePackageVersionsCentrally` enabled and a `PackageVersion` entry for every package currently referenced (7 total). Remove the `Version` attributes from the `PackageReference` elements in the three project files so versions are sourced centrally. `FluentValidation` is referenced by both Kernel and GUI (both at 11.8.0) — consolidate it into a single central version.

**Done when**: `Directory.Packages.props` exists with a `PackageVersion` entry for every referenced package; no `Version=` attributes remain on `PackageReference` items in any project; `dotnet restore` and a full build succeed on the current target framework.

## Research Findings

### Scope Inventory
- **Projects affected** (3): `SearchComparisonNet.Kernel`, `SearchComparisonNet.GUI`, `SearchComparisonNet.Tests` — all SDK-style, all on net7.0 / net7.0-windows.
- **Distinct concerns**: single concern — centralize package versions. No version bumps (those are task 03).
- **Decomposition**: Atomic. One uniform mechanical change across 3 files; no decision points, no cross-project ordering. No skill mandates decomposition.

### Package Audit (current versions — preserved as-is in this task)
| Package | Kernel | GUI | Tests | Central Version |
|---------|:------:|:---:|:-----:|-----------------|
| FluentValidation | ✓ | ✓ | | 11.8.0 |
| CommunityToolkit.Mvvm | | ✓ | | 8.2.2 |
| Microsoft.Extensions.DependencyInjection | | ✓ | | 7.0.0 |
| NuGet.Configuration | | ✓ | | 6.7.0 |
| Microsoft.NET.Test.Sdk | | | ✓ | 17.7.2 |
| xunit | | | ✓ | 2.6.1 |
| xunit.runner.visualstudio | | | ✓ | 2.5.3 |

**7 unique packages.** FluentValidation is referenced by both Kernel and GUI at the **same** version (11.8.0) — no conflict to resolve, consolidates cleanly into one `PackageVersion`.

### Files to Modify
- `Directory.Packages.props` (NEW, repo root) — `ManagePackageVersionsCentrally=true` + 7 `PackageVersion` entries.
- `SearchComparisonNet.Kernel/SearchComparisonNet.Kernel.csproj` — strip `Version` from FluentValidation.
- `SearchComparisonNet.GUI/SearchComparisonNet.GUI.csproj` — strip `Version` from 4 PackageReferences.
- `SearchComparisonNet.Tests/SearchComparisonNet.Tests.csproj` — strip `Version` from 3 PackageReferences (preserve `PrivateAssets`/`IncludeAssets` metadata on xunit.runner.visualstudio).

### Decisions Made
- Keep all current versions in this task; defer all bumps (incl. `Microsoft.Extensions.DependencyInjection` 7→10 and `xunit` replacement) to task 03, so CPM adoption stays isolated and low-risk.
- No `VersionOverride` needed — no per-project version differences exist.

### Risks
- Low. The only behavioral change is where versions are declared. The xunit.runner.visualstudio item has `PrivateAssets`/`IncludeAssets` metadata that must be preserved when stripping only the `Version` attribute.
