namespace Lands.Services
{
    using System.Threading.Tasks;
    using Lands.Views;
    using Xamarin.Forms;

    public class NavigationService
    {
       public async Task Navigate(string pageName)
        {
            switch(pageName)
            {
                case "Back":
                    await Application.Current.MainPage.Navigation.PopAsync();
                    break;

                case "LoginPage":
                    await Application.Current.MainPage.Navigation.PushAsync(
                    new LoginPage());
                    break;

                case "LandsPage":
                    await Application.Current.MainPage.Navigation.PushAsync(
                    new LandsPage());
                    break;
            }
        }
    }
}
