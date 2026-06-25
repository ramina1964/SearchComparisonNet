# 03-upgrade-to-net10: Retarget all projects to .NET 10 and update packages

Retarget all three projects in a single pass: `net7.0` → `net10.0` for Kernel and Tests, and `net7.0-windows` → `net10.0-windows` for the WPF GUI (keep WPF enabled). Update every package in `Directory.Packages.props` to its latest version compatible with .NET 10 — this includes the recommended `Microsoft.Extensions.DependencyInjection` 7.0.0 → 10.x bump and replacing the deprecated `xunit` 2.6.1 with a current supported version. Then build the solution and fix all compilation issues in one bounded pass. The assessment flagged 88 binary-incompatible and 14 behavioral API references in the GUI, which are predominantly `System.Windows.*` (WPF) and `System.Uri` references that resolve automatically on retarget — verify by building.

Research starting points: resolve exact target versions with `get_supported_package_version` per package for `net10.0`; decide xunit deprecation handling (latest v2 vs. v3); confirm `UseWPF`/Windows desktop settings on the GUI project.

**Done when**: all three projects target `net10.0` / `net10.0-windows`; all package versions in `Directory.Packages.props` are updated to the latest compatible; the deprecated `xunit` package is replaced; the full solution builds with 0 errors and 0 warnings.
