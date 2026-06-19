namespace Paraba.DriverApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Window window = base.CreateWindow(activationState);

#if WINDOWS
		window.Width = 390;
		window.Height = 844;
		window.MinimumWidth = 360;
		window.MinimumHeight = 720;
#endif

		return window;
	}
}
