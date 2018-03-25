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

        //  private ObservableCollection<Land> lands;
        private ObservableCollection<LandItemViewModel> lands;
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private bool isRefreshing;
        private string filter;
        //  private List<Land> landList;

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

        //public ObservableCollection<Land> Lands
        //{
        //    get { return this.lands; }
        //    set { SetValue(ref this.lands, value); }
        //}

        public ObservableCollection<LandItemViewModel> Lands
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
            //  this.Lands = new ObservableCollection<Land>();
            this.lands = new ObservableCollection<LandItemViewModel>();
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
            //  this.landList = (List<Land>)response.Result;
            MainViewModel.GetInstance().LandsList = (List<Land>)response.Result;

            //  this.Lands = new ObservableCollection<Land>(this.landList);
            this.Lands = new ObservableCollection<LandItemViewModel>(
                this.ToLandItemViewModel());
            SetStatusControls(false);
        }

        private void Search()
        {
            if(string.IsNullOrEmpty(this.Filter))
            {
                //  this.Lands = new ObservableCollection<Land>(this.landList);
                this.Lands = 
                    new ObservableCollection<LandItemViewModel>(this.ToLandItemViewModel());
            }
            else
            {
                //this.Lands = new ObservableCollection<Land>(
                    //this.landList
                    //.Where(l => l.Name.ToLower().Contains(this.Filter.ToLower()) 
                           //|| l.Capital.ToLower().Contains(this.Filter.ToLower())));
                this.Lands = new ObservableCollection<LandItemViewModel>(
                    this.ToLandItemViewModel()
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

        private IEnumerable<LandItemViewModel> ToLandItemViewModel()
        {
            //  return this.landList.Select(l => new LandItemViewModel
            return MainViewModel.GetInstance().LandsList.Select(
                l => new LandItemViewModel
            {
                Alpha2Code = l.Alpha2Code,
                Alpha3Code = l.Alpha3Code,
                AltSpellings = l.AltSpellings,
                Area = l.Area,
                Borders = l.Borders,
                CallingCodes = l.CallingCodes,
                Capital = l.Capital,
                Cioc = l.Cioc,
                Currencies = l.Currencies,
                Demonym = l.Demonym,
                Flag = l.Flag,
                Gini = l.Gini,
                Languages = l.Languages,
                Latlng = l.Latlng,
                Name = l.Name,
                NativeName = l.NativeName,
                NumericCode = l.NumericCode,
                Population = l.Population,
                Region = l.Region,
                RegionalBlocs = l.RegionalBlocs,
                Subregion = l.Subregion,
                Timezones = l.Timezones,
                TopLevelDomain = l.TopLevelDomain,
                Translations = l.Translations,
            });
        }

        #endregion Methods
    }
}
