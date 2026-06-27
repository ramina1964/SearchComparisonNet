namespace SearchComparisonNet.GUI;

public partial class App
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddSingleton<DataParameters>();
        _ = services.AddSingleton<IDataGenerator, DataGenerator>();
        _ = services.AddSingleton<LinearSearch>();
        _ = services.AddSingleton<BinarySearch>();
        _ = services.AddSingleton<MainViewModel>();
        _ = services.AddSingleton<MainView>();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainView = _serviceProvider.GetRequiredService<MainView>();
        mainView.Show();
    }
}
