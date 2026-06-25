# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade SearchComparisonNet to .NET 10 (LTS), update all NuGet packages to their latest compatible versions, and adopt Central Package Management.
**Scope**: 3 SDK-style projects, ~1,067 LOC — `SearchComparisonNet.Kernel` (class library), `SearchComparisonNet.GUI` (WPF app), `SearchComparisonNet.Tests` (xunit tests).

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 3 projects, all on .NET 7 with a shallow 2-tier dependency graph (Kernel → GUI/Tests). This is a mechanical TFM bump with no .NET Framework boundary, so phased ordering adds overhead without benefit.

## Tasks

### 01-prerequisites: Verify .NET 10 SDK and toolchain

Confirm the environment is ready to build .NET 10 before any retargeting. Verify the .NET 10 SDK is installed and usable, and check the repo for a `global.json` that may pin an older SDK — if present, update its `version`/`rollForward` so .NET 10 is permitted. This task makes no source code changes; it is a gate that ensures the rest of the upgrade can build.

**Done when**: .NET 10 SDK is confirmed installed; any `global.json` permits the .NET 10 SDK; the solution still restores and builds on its current target framework.

---

### 02-central-package-management: Introduce Central Package Management

Adopt CPM across the solution while still on .NET 7 so the change is isolated from the framework bump. Create `Directory.Packages.props` at the repository root with `ManagePackageVersionsCentrally` enabled and a `PackageVersion` entry for every package currently referenced (7 total). Remove the `Version` attributes from the `PackageReference` elements in the three project files so versions are sourced centrally. `FluentValidation` is referenced by both Kernel and GUI (both at 11.8.0) — consolidate it into a single central version.

**Done when**: `Directory.Packages.props` exists with a `PackageVersion` entry for every referenced package; no `Version=` attributes remain on `PackageReference` items in any project; `dotnet restore` and a full build succeed on the current target framework.

---

### 03-upgrade-to-net10: Retarget all projects to .NET 10 and update packages

Retarget all three projects in a single pass: `net7.0` → `net10.0` for Kernel and Tests, and `net7.0-windows` → `net10.0-windows` for the WPF GUI (keep WPF enabled). Update every package in `Directory.Packages.props` to its latest version compatible with .NET 10 — this includes the recommended `Microsoft.Extensions.DependencyInjection` 7.0.0 → 10.x bump and replacing the deprecated `xunit` 2.6.1 with a current supported version. Then build the solution and fix all compilation issues in one bounded pass. The assessment flagged 88 binary-incompatible and 14 behavioral API references in the GUI, which are predominantly `System.Windows.*` (WPF) and `System.Uri` references that resolve automatically on retarget — verify by building.

Research starting points: resolve exact target versions with `get_supported_package_version` per package for `net10.0`; decide xunit deprecation handling (latest v2 vs. v3); confirm `UseWPF`/Windows desktop settings on the GUI project.

**Done when**: all three projects target `net10.0` / `net10.0-windows`; all package versions in `Directory.Packages.props` are updated to the latest compatible; the deprecated `xunit` package is replaced; the full solution builds with 0 errors and 0 warnings.

---

### 04-final-validation: Build solution and run test suite

Validate the upgraded solution end-to-end. Run a clean full-solution build targeting .NET 10 and execute the complete xunit test suite in `SearchComparisonNet.Tests`, confirming all tests pass. Confirm no deprecated or vulnerable packages remain and capture any follow-up recommendations.

**Done when**: the full solution builds cleanly with 0 errors and 0 warnings; the entire test suite passes; no deprecated or vulnerable package references remain.
