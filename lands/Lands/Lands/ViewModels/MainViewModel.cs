//  CHEJ - this = regla de legibilidad, hace referencia a la propiedad interna de la case
namespace Lands.ViewModels
{
    public class MainViewModel
    {
        #region Attributes

        private static MainViewModel instance;

        #endregion Attributes

        #region Properties

        public LoginViewModel Login
        {
            get;
            set;
        }
        public LandsViewModel Lands
        {
            get;
            set;
        }

        #endregion Properties

        #region Constructor

        public MainViewModel()
        {
            //  Genera una instancia del Sigleton
            instance = this;

            //  Genera una instancia del LoginViewModel
            this.Login = new LoginViewModel();

        } 

        #endregion Constructor

        #region Methods

        //  Patron Singleton (Optiene una unica instancia desde el proyecto)
        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new MainViewModel();
            }

            return instance;
        }

        #endregion Methods
    }
}
