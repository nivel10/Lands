namespace Lands.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Lands.Models;

    public class LandViewModel : BaseViewModel
	{
        #region Attributes

        private ObservableCollection<Border> borders;

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

        #endregion Properties

		#region Constructor

        public LandViewModel(Land land)
        {
            this.Land = land;

            //  Carga la lista de los bordes
            LoadBorders();
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
