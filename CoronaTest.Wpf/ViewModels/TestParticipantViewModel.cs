using CoronaTest.Wpf.Common;
using CoronaTest.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CoronaTest.Wpf.ViewModels
{
    public class TestParticipantViewModel : BaseViewModel
    {
        public TestParticipantViewModel(IWindowController controller) : base(controller)
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
            //throw new NotImplementedException();
        }

        public static async Task<TestParticipantViewModel> CreateAsync(IWindowController controller)
        {
            var viewModel = new TestParticipantViewModel(controller);
            await viewModel.LoadDataAsync();
            return viewModel;
        }

        private async Task LoadDataAsync()
        {
            //throw new NotImplementedException();
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
