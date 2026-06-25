# Upgrade Options — SearchComparisonNet

Assessment: 3 SDK-style projects on .NET 7 (Kernel class library, GUI WPF app, Tests) upgrading to .NET 10; no Central Package Management yet; 1 deprecated package (xunit), 1 recommended bump (Microsoft.Extensions.DependencyInjection); WPF API references resolve on retarget.

## Strategy

### Upgrade Strategy
Small modern-to-modern solution (3 projects, shallow 2-tier graph: Kernel → GUI/Tests, mechanical TFM bump) — all projects can be upgraded together in one pass.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all 3 projects to .NET 10 in a single atomic pass. Fastest, no multi-targeting overhead; the solution may be briefly broken mid-upgrade until every project is updated. |
| Top-Down | Upgrade apps first, temporarily multi-target the shared Kernel library so the solution stays buildable throughout, then consolidate. Adds overhead not needed at this scale. |

## Project Structure

### Package Management
You requested Central Package Management; all 3 projects are already SDK-style and moving within the modern .NET ecosystem (.NET 7 → .NET 10), so CPM applies cleanly without `VersionOverride` friction.

| Value | Description |
|-------|-------------|
| **Central Package Management (CPM)** (selected) | Create `Directory.Packages.props`, move all package versions out of the project files into one central location. Consistent versions and easier maintenance. |
| Per-Project | Each project keeps its own package versions inline. Simpler short-term, but no central control. |

## Compatibility

### Unsupported API Handling
The assessment flagged 88 binary-incompatible and 14 behavioral API references in the WPF GUI project — these are predominantly `System.Windows.*` (WPF) and `System.Uri` references that resolve automatically when retargeting to `net10.0-windows`. No complex or removed BCL APIs were detected.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task. For this solution, retargeting resolves the WPF references; no deferred work or stubs to clean up. |
| Defer Complex Changes | Stub complex API changes and create resolution subtasks. Not needed here — no complex API changes detected. |
