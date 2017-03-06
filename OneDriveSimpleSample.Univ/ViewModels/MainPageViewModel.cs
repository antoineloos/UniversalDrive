using Prism.Commands;
using Prism.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneDriveSimpleSample.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        INavigationService _navservice;

        public MainPageViewModel(INavigationService navservice)
        {
            _navservice = navservice;
        }

        public DelegateCommand OneDriveCommand => new DelegateCommand(OneDriveNav);

        private void OneDriveNav()
        {
            if (_navservice != null) _navservice.Navigate("OneDriveFile",null);
        }

        public DelegateCommand DropBoxCommand => new DelegateCommand(DropBoxNav);

        private void DropBoxNav()
        {
            if (_navservice != null) _navservice.Navigate("DropBox", null);
        }

        public DelegateCommand GoogleDriveCommand => new DelegateCommand(GoogleDriveNav);

        private void GoogleDriveNav()
        {
            if (_navservice != null) _navservice.Navigate("GoogleDrive", null);
        }
    }
}
