namespace SearchComparisonNet.GUI.Views;

public partial class MainView : Window
{
    public MainView(MainViewModel mainViewModel)
    {
        InitializeComponent();
        Loaded += MainView_Loaded;
        MainViewModel = mainViewModel;
    }

    public MainViewModel MainViewModel { get; set; }

    private void MainView_Loaded(object sender, RoutedEventArgs e)
    {
        DataContext = MainViewModel;
    }
}
