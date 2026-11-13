namespace TaskListApp;

public partial class App : Application
{
    private readonly MainPage mainPage;

    public App(MainPage page)
    {
        InitializeComponent();
        mainPage = page;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new NavigationPage(mainPage));
}