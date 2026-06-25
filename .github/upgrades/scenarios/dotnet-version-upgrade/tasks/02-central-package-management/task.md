# 02-central-package-management: Introduce Central Package Management

Adopt CPM across the solution while still on .NET 7 so the change is isolated from the framework bump. Create `Directory.Packages.props` at the repository root with `ManagePackageVersionsCentrally` enabled and a `PackageVersion` entry for every package currently referenced (7 total). Remove the `Version` attributes from the `PackageReference` elements in the three project files so versions are sourced centrally. `FluentValidation` is referenced by both Kernel and GUI (both at 11.8.0) — consolidate it into a single central version.

**Done when**: `Directory.Packages.props` exists with a `PackageVersion` entry for every referenced package; no `Version=` attributes remain on `PackageReference` items in any project; `dotnet restore` and a full build succeed on the current target framework.
