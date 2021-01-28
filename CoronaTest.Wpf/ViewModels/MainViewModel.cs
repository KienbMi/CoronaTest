using CoronaTest.Wpf.Common;
using CoronaTest.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(IWindowController controller) : base(controller)
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
            //throw new NotImplementedException();
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        public static async Task<MainViewModel> CreateAsync(IWindowController windowController)
        {
            var viewModel = new MainViewModel(windowController);
            //await viewModel.LoadDataAsync();
            return viewModel;
        }
    }
}
