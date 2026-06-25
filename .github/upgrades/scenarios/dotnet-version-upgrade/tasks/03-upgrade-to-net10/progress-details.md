# Progress Details — 03-upgrade-to-net10

## Files Modified
- `Directory.Packages.props` — bumped all 7 package versions to latest .NET 10-compatible stable releases.
- `SearchComparisonNet.Kernel/SearchComparisonNet.Kernel.csproj` — `TargetFramework` net7.0 → **net10.0**.
- `SearchComparisonNet.GUI/SearchComparisonNet.GUI.csproj` — `TargetFramework` net7.0-windows → **net10.0-windows** (UseWPF retained).
- `SearchComparisonNet.Tests/SearchComparisonNet.Tests.csproj` — `TargetFramework` net7.0 → **net10.0**.

## Package Bumps
| Package | From | To |
|---------|------|-----|
| Microsoft.Extensions.DependencyInjection | 7.0.0 | 10.0.9 |
| CommunityToolkit.Mvvm | 8.2.2 | 8.4.2 |
| FluentValidation | 11.8.0 | 12.1.1 |
| NuGet.Configuration | 6.7.0 | 7.6.0 |
| Microsoft.NET.Test.Sdk | 17.7.2 | 18.7.0 |
| xunit | 2.6.1 | 2.9.3 |
| xunit.runner.visualstudio | 2.5.3 | 3.1.5 |

## Build Result
- Errors: 0
- Warnings: 0
- Projects built: SearchComparisonNet.Kernel, SearchComparisonNet.Tests, SearchComparisonNet.GUI (all on .NET 10)
- The previous `NETSDK1138` EOL warnings are gone, confirming the retarget.

## Test Result
- Tests run: 50
- Passed: 50
- Failed: 0
- Runner: xunit.runner.visualstudio 3.1.5 executing xunit 2.9.3 tests on net10.0.

## Changes Summary
Performed the atomic All-at-Once upgrade:

1. **Retargeted** all three projects to .NET 10 (`net10.0` / `net10.0-windows`) in a single pass. WPF settings (`UseWPF`, `StartupObject`, `OutputType=WinExe`) preserved on the GUI.
2. **Bumped all 7 packages** centrally in `Directory.Packages.props` to their latest stable .NET 10-compatible versions.
3. **Resolved deprecated xunit** by moving to xunit 2.9.3 (latest stable v2) + xunit.runner.visualstudio 3.1.5, rather than the larger xunit v3 migration — minimal risk, full test compatibility.
4. **No source code changes were required** — the 88 binary-incompatible + 14 behavioral API references the assessment flagged in the GUI (predominantly `System.Windows.*` WPF and `System.Uri`) all resolved automatically on retarget. FluentValidation 11→12 needed no code changes (verified the API surface used in `InputValidation.cs`/`MainViewModel.cs` is unchanged in v12).

## Done-When Criteria Verification
- ✅ All three projects target `net10.0` / `net10.0-windows` — confirmed in build output DLL paths.
- ✅ All package versions in `Directory.Packages.props` updated to latest compatible — 7/7 bumped.
- ✅ Deprecated `xunit` replaced — 2.6.1 → 2.9.3 (+ runner 3.1.5).
- ✅ Full solution builds with 0 errors and 0 warnings.
- ✅ (Bonus) Full test suite passes (50/50), de-risking final validation.

## Issues Encountered
- `get_supported_package_version` returned **preview** versions for two packages (Microsoft.Extensions.DependencyInjection → 11.0.0-preview.5; xunit.runner.visualstudio → 4.0.0-pre.4). Rejected both per the LTS/stable policy and queried the NuGet flat-container feed for the latest **stable** releases instead (10.0.9 and 3.1.5 respectively).
