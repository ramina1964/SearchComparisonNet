namespace SearchComparisonNet.GUI
{
    public partial class App
    {
        public App()
        {
            Services = new ServiceCollection();
            ConfigureServices();
            ServiceProvider = Services.BuildServiceProvider();
        }

        public ServiceCollection Services { get; set; }
        
        public static IServiceProvider ServiceProvider;

        private void ConfigureServices()
        {
            _ = Services.AddSingleton<DataParameters>();
            _ = Services.AddSingleton<IDataGenerator, DataGenerator>();
            _ = Services.AddSingleton<LinearSearch>();
            _ = Services.AddSingleton<BinarySearch>();
            _ = Services.AddSingleton<MainViewModel>();
            _ = Services.AddSingleton<MainView>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainView = ServiceProvider.GetService<MainView>();
            mainView.Show();
        }
    }
}
