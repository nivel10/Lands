//  CHEJ - Patron de diseño, denominado InstanceLocator
//  Se emplea para relacionar los Page con la MainViewModel
namespace Lands.Infrastructure
{
    using Lands.ViewModels;

    public class InstanceLocator
    {
        #region Properties

        public MainViewModel Main
        {
            get;
            set;
        }

        #endregion Properties

        #region Constructor

        public InstanceLocator()
        {
            this.Main = new MainViewModel();
        } 

        #endregion Constructor
    }
}
