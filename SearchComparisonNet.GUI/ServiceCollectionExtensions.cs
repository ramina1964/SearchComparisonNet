namespace SearchComparisonNet.GUI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSearchComparisonServices(this IServiceCollection services)
    {
        // The factory is stateless; the per-run data and search objects it creates are not
        // registered in the container because they depend on runtime input (NoOfEntries).
        _ = services.AddSingleton<ISearchComparisonFactory, SearchComparisonFactory>();
        _ = services.AddSingleton<MainViewModel>();
        _ = services.AddSingleton<MainView>();

        return services;
    }
}
