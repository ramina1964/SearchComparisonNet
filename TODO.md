# TODO / Follow-up Tasks

Tracked follow-up work deferred from the `refactor/di-and-structure` branch to keep
each branch focused on a single theme.

## 1. Performance & concurrency investigation (memory, CPU utilization, async/await)
**Suggested branch:** `perf/concurrency`

> **Profiling-first:** start by measuring, not by changing code. Low observed CPU usage, memory growth, and async behavior are likely facets of the same execution path (`MainViewModel.SimulateAsync` runs the linear and binary simulations sequentially on single `Task.Run` threads), so confirm the real bottleneck before optimizing.

**CPU utilization**
- Profile a representative run; CPU sitting at "a few percent" suggests the work is effectively single-threaded.
- Determine whether the two searches (and/or the per-search loops) are parallelizable, or whether the workload is memory-bandwidth/allocation bound (in which case more threads won't help).
- If parallelizing, ensure thread-safety and that the shared-dataset comparison invariant is preserved.

**Memory**
- Profile under repeated large simulations (datasets up to ~500k) to confirm and locate growth.
- Inspect lifetimes of event subscriptions: `ErrorsChanged`, `PropertyChanged`, and any WPF binding/converter retention.
- Review whether large `ObservableCollection<int>` datasets are released between runs.
- Confirm singletons (`MainViewModel`, the search factory in DI) do not retain per-run state unboundedly.

**async/await**
- Audit `MainViewModel` async flow: command handlers, `Task.Run` usage, and `CancellationToken` propagation.
- Check for `async void`, blocking calls (`.Result` / `.Wait()` / `.GetAwaiter().GetResult()`), and unobserved/fire-and-forget tasks.
- Verify cancellation unwinds the simulation cleanly.

**Validation**
- Re-measure after each change to prove improvement (don't rely on intuition).
- Build + full test run; add concurrency/cancellation tests if behavior changes.

## 2. Extend test coverage and leverage xUnit v3 features
**Suggested branch:** `test/expand-coverage`

> Note: the test project already targets xUnit v3 (`xunit.v3` 3.2.2 with `xunit.runner.visualstudio` 3.1.5), so this is about broadening coverage and adopting v3 idioms, not upgrading the framework.

- Add tests beyond `LinearSearch`/`BinarySearch`: `InputValidation` rules, `ViewModelBase` error tracking (`HasErrors`/`GetErrors`), and `DataGenerator` output.
- Adopt xUnit v3 features where they improve clarity: strongly-typed `TheoryData<T>`, `Assert.Multiple`, and `[ClassData]`/`[MemberData]` for larger datasets.
- Consider edge cases: empty/single-element datasets, duplicate values, and cancellation paths in the simulation.
- Validate with a full test run.

## 3. Review SearchComparisonNet.GUI.csproj (performance / readability / maintainability)
**Suggested branch:** `refactor/gui-csproj-review`

- Remove the unused `NuGet.Configuration` package reference (no usage found in GUI source; only present in the csproj and generated `obj/` artifacts).
- Verify every remaining `PackageReference` is actually used (`CommunityToolkit.Mvvm`, `FluentValidation`, `Microsoft.Extensions.DependencyInjection`).
- Confirm WPF/build settings are minimal and correct for net10.0-windows.
- Validate with a build + full test run after any reference changes.

## Suggested ordering & effort

- **Tasks 2 and 3 are independent and low-risk** - quick wins that can be done together or in either order. Task 3 is the smallest (mostly removing the unused `NuGet.Configuration` reference plus a quick package audit); task 2 is incremental (add tests without changing production code).
- **Task 1 is the investigative one** - highest effort and strictly measurement-driven. It bundles the memory, CPU-utilization, and async/await concerns because they share the same execution path; it needs profiling to confirm real issues before any change, and may introduce behavioral/threading changes.
- **Recommended sequence:** start with the low-risk cleanups (3, then 2) to keep momentum, then take on the diagnostic work (1) when there's time for profiling and careful before/after measurement.
