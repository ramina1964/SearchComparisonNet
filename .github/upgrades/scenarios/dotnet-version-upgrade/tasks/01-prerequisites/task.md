# 01-prerequisites: Verify .NET 10 SDK and toolchain

Confirm the environment is ready to build .NET 10 before any retargeting. Verify the .NET 10 SDK is installed and usable, and check the repo for a `global.json` that may pin an older SDK — if present, update its `version`/`rollForward` so .NET 10 is permitted. This task makes no source code changes; it is a gate that ensures the rest of the upgrade can build.

**Done when**: .NET 10 SDK is confirmed installed; any `global.json` permits the .NET 10 SDK; the solution still restores and builds on its current target framework.
