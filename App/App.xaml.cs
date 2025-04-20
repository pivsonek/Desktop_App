namespace project.App
{     /// <summary>
      /// Main application class.
      /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Creates the main application window.
        /// </summary>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
