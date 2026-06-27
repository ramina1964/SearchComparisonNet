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
