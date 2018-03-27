namespace Lands.Services
{
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class DialogService
    {
        public async Task ShowMessage(string _tittle, string _message, string _buttom)
        {
            await Application.Current.MainPage.DisplayAlert(
                _tittle,
                _message,
                _buttom);
        }
    }
}
