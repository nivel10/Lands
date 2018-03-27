namespace Lands
{
    using Lands.Views;
    using Xamarin.Forms;

    public partial class App : Application
	{
        #region Constructor

        public App()
        {
            InitializeComponent();

            //  MainPage = new Lands.MainPage();
            //  this.MainPage = new LoginPage();
            this.MainPage = 
                new NavigationPage(new LoginPage());

        } 

        #endregion Constructor

        #region Methods

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        } 

        #endregion Methods
    }
}
