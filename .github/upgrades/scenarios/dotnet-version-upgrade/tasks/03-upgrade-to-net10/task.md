# 03-upgrade-to-net10: Retarget all projects to .NET 10 and update packages

Retarget all three projects in a single pass: `net7.0` → `net10.0` for Kernel and Tests, and `net7.0-windows` → `net10.0-windows` for the WPF GUI (keep WPF enabled). Update every package in `Directory.Packages.props` to its latest version compatible with .NET 10 — this includes the recommended `Microsoft.Extensions.DependencyInjection` 7.0.0 → 10.x bump and replacing the deprecated `xunit` 2.6.1 with a current supported version. Then build the solution and fix all compilation issues in one bounded pass. The assessment flagged 88 binary-incompatible and 14 behavioral API references in the GUI, which are predominantly `System.Windows.*` (WPF) and `System.Uri` references that resolve automatically on retarget — verify by building.

Research starting points: resolve exact target versions with `get_supported_package_version` per package for `net10.0`; decide xunit deprecation handling (latest v2 vs. v3); confirm `UseWPF`/Windows desktop settings on the GUI project.

**Done when**: all three projects target `net10.0` / `net10.0-windows`; all package versions in `Directory.Packages.props` are updated to the latest compatible; the deprecated `xunit` package is replaced; the full solution builds with 0 errors and 0 warnings.

## Research Findings

### Scope Inventory
- **Projects affected** (3): Kernel (`net7.0`→`net10.0`), Tests (`net7.0`→`net10.0`), GUI (`net7.0-windows`→`net10.0-windows`, WPF kept).
- **Concern**: single uniform retarget + central package bump. **Atomic** — no decomposition; one pattern applied across all projects, validated by one solution build.

### Package Version Decisions (resolved against NuGet stable feed)
| Package | From | To (stable) | Notes |
|---------|------|-------------|-------|
| Microsoft.Extensions.DependencyInjection | 7.0.0 | **10.0.9** | Aligned to .NET 10. `get_supported_package_version` initially returned `11.0.0-preview.5` — rejected (preview); used stable 10.x per LTS policy. |
| CommunityToolkit.Mvvm | 8.2.2 | **8.4.2** | Latest stable. |
| FluentValidation | 11.8.0 | **12.1.1** | Major bump — verified compatible (see below). |
| NuGet.Configuration | 6.7.0 | **7.6.0** | Latest stable. |
| Microsoft.NET.Test.Sdk | 17.7.2 | **18.7.0** | Latest stable. |
| xunit | 2.6.1 | **2.9.3** | Latest stable v2 (see xunit decision). |
| xunit.runner.visualstudio | 2.5.3 | **3.1.5** | Latest stable; v3 runner runs v2 tests. (tool returned `4.0.0-pre.4` — rejected, preview.) |

### xunit Decision (v2 vs v3)
The task labels xunit 2.6.1 "deprecated." xunit 2.x is **not** a deprecated product line — 2.6.1 is simply outdated. Chose **xunit 2.9.3** (latest stable v2, full .NET 10 support) + **xunit.runner.visualstudio 3.1.5** rather than migrating to xunit v3 (`xunit.v3` 3.2.2). Rationale: v3 is a different package with namespace/API/runner changes — a larger, riskier change unwarranted by a TFM bump. The v2.9.3 + runner 3.x combination resolves the "old/unsupported version" concern with minimal risk and keeps all existing test code intact.

### FluentValidation 11→12 Compatibility (verified)
Usage is confined to `SearchComparisonNet.GUI/ViewModels/InputValidation.cs` (one `AbstractValidator<MainViewModel>`) and `MainViewModel.cs`. APIs used: `RuleFor`, `NotNull`, `NotEmpty`, `WithMessage`, `Must`, `InclusiveBetween`, `ClassLevelCascadeMode = CascadeMode.Stop`, `Validate(instance, ctx => ctx.IncludeProperties(...))`, `.IsValid`. All remain supported in v12 — **no code changes expected**.

### Assessment API Flags (GUI)
88 binary-incompatible + 14 behavioral references flagged are predominantly `System.Windows.*` (WPF) and `System.Uri` — these resolve automatically on retarget to `net10.0-windows`. To be verified by the build; only investigate if the build surfaces real errors.

### Files Modified
- `Directory.Packages.props` — 7 version bumps.
- `SearchComparisonNet.Kernel.csproj` — TFM `net7.0`→`net10.0`.
- `SearchComparisonNet.GUI.csproj` — TFM `net7.0-windows`→`net10.0-windows` (UseWPF retained).
- `SearchComparisonNet.Tests.csproj` — TFM `net7.0`→`net10.0`.
