namespace Lands.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Lands.Models;
    using Lands.Services;
    using Xamarin.Forms;

    public class LandsViewModel : BaseViewModel
    {
        #region Attributes

        private ObservableCollection<Land> lands;
        private ApiService apiService;
        private DialogService dialogService;

        #endregion Attributes

        #region Properties

        public ObservableCollection<Land> Lands
        {
            get { return this.lands; }
            set { SetValue(ref this.lands, value); }
        }

        #endregion Properties

        #region Constructor

        public LandsViewModel()
        {
            //  Instancia los services
            apiService = new ApiService();
            dialogService = new DialogService();

            //  Invoca el metodo que hace la carga de los Lands
            LoadLands();
        }

        #endregion Constructor

        #region Methods

        private async void LoadLands()
        {
            //  Valida la conexion en el dispositivo
            var connection = await apiService.CheckConnection();
            if(!connection.IsSuccess)
            {
                await dialogService.ShowMessage(
                    "Error", 
                    connection.Message, 
                    "Accept");
                return;
            }

            //  Invoca el metodo que optiene la lista de las Lands
            var response = await apiService.GetList<Land>(
                Application.Current.Resources["urlGetLands"].ToString(),
                "rest/",
                "v2/all");

            if(!response.IsSuccess)
            {
                await dialogService.ShowMessage(
                    "Error", 
                    response.Message, 
                    "Accept");
                return;
            }

            //  Hace un Cast del objeto Result
            var list = (List<Land>)response.Result;
            this.Lands = new ObservableCollection<Land>(list);
        }

        #endregion Methods
    }
}
