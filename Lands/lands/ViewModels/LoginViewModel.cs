namespace Lands.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Lands.Services;
    using Lands.Views;
    using System;
    using System.Windows.Input;
    using Xamarin.Forms;

    //  public class LoginViewModel : INotifyPropertyChanged
    public class LoginViewModel : BaseViewModel
    {
        #region Attributes

        private string email;
        private string password;
        private bool isEnabled;
        private bool isRunning;
        private bool isRemembered;
        private DialogService dialogService;
        private NavigationService navigationService;

        #region Event

        //  public event PropertyChangedEventHandler PropertyChanged;

        #endregion Event

        #endregion Attributes

        #region Properties

        //public string Email
        //{
        //    get
        //    {
        //        return this.email;
        //    }
        //    set
        //    {
        //        if (value != this.email)
        //        {
        //            this.email = value;
        //            PropertyChanged?.Invoke(
        //                this, 
        //                new PropertyChangedEventArgs(nameof(this.Email)));
        //        }
        //    }
        //}

        //public string Password
        //{
        //    get
        //    {
        //        return this.password;
        //    }
        //    set
        //    {
        //        if (value != this.password)
        //        {
        //            this.password = value;
        //            PropertyChanged?.Invoke(
        //                this, 
        //                new PropertyChangedEventArgs(nameof(this.Password)));
        //        }
        //    }
        //}

        //public bool IsEnabled
        //{
        //    get
        //    {
        //        return this.isEnabled;
        //    }
        //    set
        //    {
        //        if (value != this.isEnabled)
        //        {
        //            this.isEnabled = value;
        //            PropertyChanged?.Invoke(
        //                this, 
        //                new PropertyChangedEventArgs(nameof(this.IsEnabled)));
        //        }
        //    }
        //}

        //public bool IsRunning
        //{
        //    get
        //    {
        //        return this.isRunning;
        //    }
        //    set
        //    {
        //        if (value != this.isRunning)
        //        {
        //            this.isRunning = value;
        //            PropertyChanged?.Invoke(
        //                this, 
        //                new PropertyChangedEventArgs(nameof(this.IsRunning)));
        //        }
        //    }
        //}

        //public bool IsRemembered
        //{
        //    get
        //    {
        //        return this.isRemembered;
        //    }
        //    set
        //    {
        //        if (value != this.isRemembered)
        //        {
        //            this.isRemembered = value;
        //            PropertyChanged?.Invoke(
        //                this, 
        //                new PropertyChangedEventArgs(nameof(this.IsRemembered)));
        //        }
        //    }
        //}

        //  Optimizacion del codigo

        public string Email
        {
            get { return this.email; }
            set { SetValue(ref this.email, value); }
        }

        public string Password
        {
            get { return this.password; }
            set { SetValue(ref this.password, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref this.isRunning, value); }
        }

        public bool IsRemembered
        {
            get { return this.isRemembered; }
            set { SetValue(ref this.isRemembered, value); }
        }

        #endregion Properties

        #region Commands

        public ICommand LoginCommand
        {
            get { return new RelayCommand(Login); }
        }

        public ICommand RegisterCommand
        {
            get { return new RelayCommand(Register); }
        }

        public ICommand AboutCommand
        {
            get { return new RelayCommand(About); }
        }

        #endregion Commands

        #region Costructor

        public LoginViewModel()
        {
            //  Instancia los objetos necesarios
            dialogService = new DialogService();
            navigationService = new NavigationService();

            //  Inicializa los controles
            SetInitialize();
            SetStatusControls(true, false, true);

            //  Propiedades quemadas (Borrar)
            this.Email = "carlos.e.herrera.j@gmail.com";
            this.Password = this.Email;
        }
        
        #endregion Costructor

        #region Methods

        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await dialogService.ShowMessage(
                    "Error", 
                    "You must enter an email...!!!", 
                    "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "You must enter an password...!!!",
                    "Accept");
                return;
            }

            if (this.Email != "carlos.e.herrera.j@gmail.com"
                || this.Password != "carlos.e.herrera.j@gmail.com")
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Email or password incorrect...!!!",
                    "Accept"
                    );
                SetInitialize();
                return;
            }

            // Inicializa los controles
            SetInitialize();

            //  Genera una instancia del LandsViewModel
            MainViewModel.GetInstance().Lands = new LandsViewModel();

            //  await Application.Current.MainPage.Navigation.PushAsync(
            //  new LandsPage());
            await navigationService.Navigate("LandsPage");
                             
            //await dialogService.ShowMessage(
                //"Information",
                //"Access is ok...!!!",
                //"Accept");
        }

        private void Register()
        {
            throw new NotImplementedException();
        }

        private void About()
        {
            throw new NotImplementedException();
        }

        private void SetStatusControls(bool _isEnabled, bool _isRunning, bool _isRemenbered)
        {
            this.IsEnabled = _isEnabled;
            this.IsRunning = _isRunning;
            this.IsRemembered = _isRemenbered;
        }

        private void SetInitialize()
        {
            this.Email = "";
            this.Password = "";
        }

        #endregion Methods
    }
}