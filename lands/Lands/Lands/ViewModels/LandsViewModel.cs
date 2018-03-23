namespace Lands.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Lands.Models;
    using Lands.Services;
    using Xamarin.Forms;

    public class LandsViewModel : BaseViewModel
    {
        #region Attributes

        private ObservableCollection<Land> lands;
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private bool isRefreshing;
        private string filter;
        private List<Land> landList;

        #endregion Attributes

        #region Commands

        public ICommand RefreshCommand
        {
            get { return new RelayCommand(LoadLands); }
        }

        public ICommand SearchCommand
        {
            get { return new RelayCommand(Search); }
        }

        #endregion Commands

        #region Properties

        public ObservableCollection<Land> Lands
        {
            get { return this.lands; }
            set { SetValue(ref this.lands, value); }
        }

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }

        public string Filter
        {
            get { return this.filter; }
            set 
            { 
                SetValue(ref this.filter, value);
                this.Search();
            }
        }

        #endregion Properties

        #region Constructor

        public LandsViewModel()
        {
            //  Instancia los services
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();

            //  Invoca el metodo que hace la carga de los Lands
            LoadLands();
        }

        #endregion Constructor

        #region Methods

        private async void LoadLands()
        {
            this.Lands = new ObservableCollection<Land>();
            // this.Lands.Clear();

            SetStatusControls(true);

            //  Valida la conexion en el dispositivo
            var connection = await apiService.CheckConnection();
            if(!connection.IsSuccess)
            {
                SetStatusControls(false);
                await dialogService.ShowMessage(
                    "Error", 
                    connection.Message, 
                    "Accept");
                await navigationService.Navigate("Back");
                return;
            }

            //  Invoca el metodo que optiene la lista de las Lands
            var response = await apiService.GetList<Land>(
                Application.Current.Resources["urlGetLands"].ToString(),
                "rest/",
                "v2/all");

            if(!response.IsSuccess)
            {
                SetStatusControls(false);
                await dialogService.ShowMessage(
                    "Error", 
                    response.Message, 
                    "Accept");
                return;
            }

            //  Hace un Cast del objeto Result
            //  var list = (List<Land>)response.Result;
            //  this.Lands = new ObservableCollection<Land>(list);
            this.landList = (List<Land>)response.Result;
            this.Lands = new ObservableCollection<Land>(this.landList);

            SetStatusControls(false);
        }

        private void Search()
        {
            if(string.IsNullOrEmpty(this.Filter))
            {
                this.Lands = new ObservableCollection<Land>(this.landList);
            }
            else
            {
                this.Lands = new ObservableCollection<Land>(
                    this.landList
                    .Where(l => l.Name.ToLower().Contains(this.Filter.ToLower()) 
                           || l.Capital.ToLower().Contains(this.Filter.ToLower())));
            }
        }

        private void SetStatusControls(bool _isRefreshing)
        {
            //this.IsEnabled = _isEnabled;
            //this.IsRunning = _isRunning;
            this.IsRefreshing = _isRefreshing;
        }

        #endregion Methods
    }
}
