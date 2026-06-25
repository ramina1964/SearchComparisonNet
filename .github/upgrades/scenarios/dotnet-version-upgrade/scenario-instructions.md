# .NET Version Upgrade

## Preferences

### Technical Preferences
- **Target Framework**: `net10.0` (.NET 10 — latest LTS, support ends Nov 2028)
- **NuGet Packages**: Update all packages to latest compatible versions
- **Central Package Management (CPM)**: Enabled — consolidate package versions into `Directory.Packages.props`

### Execution Style
- **Flow Mode**: Automatic — run end-to-end, pause only when blocked or needing input that cannot be inferred

## Source Control
- **Source Branch**: `main`
- **Working Branch**: `upgrade-dotnet-10`
- **Commit Strategy**: Single Commit at End
- **Branch Sync**: Auto (Merge)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Project Structure
- Package Management: Central Package Management (CPM)

### Compatibility
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: 3 projects, all on .NET 7 (modern-to-modern), shallow 2-tier dependency graph (Kernel → GUI/Tests), mechanical TFM bump with no .NET Framework boundary and ≤2 high-risk migrations.

### Execution Constraints
- Single atomic upgrade — all 3 projects retargeted together; do not introduce tier/phase ordering.
- Establish CPM (`Directory.Packages.props`) before the TFM bump so package version changes happen in one central place.
- After the upgrade pass, confirm the full solution builds with 0 errors and 0 warnings before running tests.
- Run the full test suite only after the atomic upgrade compiles cleanly.

## Key Decisions Log
- **2025-11-12**: User requested upgrade to latest .NET (.NET 10 LTS chosen over .NET 11 Preview), update all NuGet packages, and adopt Central Package Management. New branch `upgrade-dotnet-10` created from `main`.
- **2025-11-12**: Confirmed upgrade options — Strategy: All-at-Once; Package Management: CPM; Unsupported API Handling: Fix Inline. Commit strategy switched to Single Commit at End to match the atomic All-at-Once approach.
- **2025-11-12**: During final validation, `dotnet list package --deprecated` flagged xunit 2.9.3 as "Legacy" (xunit v2 line superseded by xunit v3). User chose **Option A: keep xunit v2 (2.9.3) for now** — it is fully supported on .NET 10, builds clean, all 50 tests pass. The "Legacy" tag is product-direction only (not security/compatibility). **Follow-up**: migrate the test project to xunit v3 in a later, dedicated session.

## Reminders & Deferred Items
- **2025-11-12** — Migrate `SearchComparisonNet.Tests` from xunit v2 (2.9.3) to xunit v3 (`xunit.v3`) in a future dedicated session. Involves swapping the `xunit` package for `xunit.v3`, updating `Usings.cs`/`using` directives, adjusting any API/namespace differences, and updating the runner configuration. Deferred per user (Option A) — current v2 setup is fully supported on .NET 10.
