namespace Lands.ViewModels
{
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Lands.Models;
    using Lands.Views;
    using Xamarin.Forms;

    public class LandItemViewModel : Land
    {
        #region Commands

        public ICommand SelectLandCommand
        {
            get { return new RelayCommand(SelectLand); }
        }

        #endregion Commands

        #region Methods

        private async void SelectLand()
        {
            //  Invoca una instancia de la LandViewModel
            MainViewModel.GetInstance().Land = new LandViewModel(this);

            //  Navaga a la pagina LandPage()
            //    await Application.Current.MainPage.Navigation.PushAsync(
            //    new LandPage());
            await Application.Current.MainPage.Navigation.PushAsync(
                new LandTabbedPage());
        }

        #endregion Methods
    }
}
