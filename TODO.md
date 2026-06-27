# TODO / Follow-up Tasks

Tracked follow-up work deferred from the `refactor/di-and-structure` branch to keep
each branch focused on a single theme.

## 1. async/await audit
**Suggested branch:** `fix/async-correctness`

- Audit `MainViewModel` async flow: command handlers, `Task.Run` usage, and `CancellationToken` propagation.
- Check for `async void`, blocking calls (`.Result` / `.Wait()` / `.GetAwaiter().GetResult()`), and unobserved/fire-and-forget tasks.
- Verify cancellation unwinds the simulation cleanly; consider `IAsyncRelayCommand` (CommunityToolkit.Mvvm) if appropriate.
- Validate with a build + full test run; add async-specific tests if behavior changes.

## 2. Memory-leak investigation
**Suggested branch:** `fix/memory-leak`

- Profile under repeated large simulations (datasets up to ~500k) to confirm and locate growth.
- Inspect lifetimes of event subscriptions: `ErrorsChanged`, `PropertyChanged`, and any WPF binding/converter retention.
- Review whether large `ObservableCollection<int>` datasets are released between runs.
- Confirm singletons (`MainViewModel`, search services in DI) do not retain per-run state unboundedly.

## 3. Extend test coverage and leverage xUnit v3 features
**Suggested branch:** `test/expand-coverage`

> Note: the test project already targets xUnit v3 (`xunit.v3` 3.2.2 with `xunit.runner.visualstudio` 3.1.5), so this is about broadening coverage and adopting v3 idioms, not upgrading the framework.

- Add tests beyond `LinearSearch`/`BinarySearch`: `InputValidation` rules, `ViewModelBase` error tracking (`HasErrors`/`GetErrors`), and `DataGenerator` output.
- Adopt xUnit v3 features where they improve clarity: strongly-typed `TheoryData<T>`, `Assert.Multiple`, and `[ClassData]`/`[MemberData]` for larger datasets.
- Consider edge cases: empty/single-element datasets, duplicate values, and cancellation paths in the simulation.
- Validate with a full test run.

## 4. Review SearchComparisonNet.GUI.csproj (performance / readability / maintainability)
**Suggested branch:** `refactor/gui-csproj-review`

- Remove the unused `NuGet.Configuration` package reference (no usage found in GUI source; only present in the csproj and generated `obj/` artifacts).
- Verify every remaining `PackageReference` is actually used (`CommunityToolkit.Mvvm`, `FluentValidation`, `Microsoft.Extensions.DependencyInjection`).
- Confirm WPF/build settings are minimal and correct for net10.0-windows.
- Validate with a build + full test run after any reference changes.
