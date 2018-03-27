namespace Lands.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Lands.Models;

    public class LandViewModel : BaseViewModel
	{
        #region Attributes

        private ObservableCollection<Border> borders;
        private ObservableCollection<Currency> currencies;
        private ObservableCollection<Language> languages;

        #endregion Attributes

		#region Properties

        public Land Land
        {
            get;
            set;
        }

        public ObservableCollection<Border> Borders
        {
            get { return this.borders; }
            set { this.SetValue(ref this.borders, value); }
        }

        public ObservableCollection<Currency> Currencies
        {
            get { return this.currencies; }
            set { this.SetValue(ref this.currencies, value); }
        }

        public ObservableCollection<Language> Languages
        {
            get { return this.languages; }
            set { this.SetValue(ref this.languages, value); }
        }

        #endregion Properties

		#region Constructor

        public LandViewModel(Land land)
        {
            this.Land = land;

            //  Carga la lista de los bordes
            LoadBorders();

            //  Carga el objeto de las Currency
            this.Currencies = 
                new ObservableCollection<Currency>(this.Land.Currencies);

            //  Carga el objeto Language
            this.Languages = 
                new ObservableCollection<Language>(this.Land.Languages);
        }

        #endregion Constructor

        #region Methods

        private void LoadBorders()
        {
            this.Borders = new ObservableCollection<Border>();

            foreach (var border in this.Land.Borders)
            {
                var land = MainViewModel.GetInstance().LandsList
                                        .Where(l => l.Alpha3Code == border)
                                        .FirstOrDefault();
                if (land != null)
                {
                    this.Borders.Add(new Border 
                    { 
                        Code =land.Alpha3Code,
                        Name = land.Name,
                    });
                }
            }
        }

        #endregion Methods

	}
}