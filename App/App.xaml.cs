namespace project.App
{     /// <summary>
      /// Hlavní třída aplikace.
      /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Vytvoří hlavní okno aplikace.
        /// </summary>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell()); // Hlavní okno obsahující navigaci AppShell
        }
    }
}
