using CoronaTest.Core;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.DataTransferObjects;
using CoronaTest.Persistence;
using CoronaTest.Wpf.Common;
using CoronaTest.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoronaTest.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<ExaminationDto> _examinations;
        private DateTime? _from;
        private DateTime? _to;
        private int _examinationsCount;
        private int _examinationsNegativeCount;
        private int _examinationsPositiveCount;

        public ObservableCollection<ExaminationDto> Examinations 
        {
            get => _examinations; 
            set
            {
                _examinations = value;
                OnPropertyChanged();
            }
        }

        public DateTime? From 
        { 
            get => _from; 
            set
            {
                _from = value;
                OnPropertyChanged();
                _ = LoadDataAsync();
            }
        }

        public DateTime? To 
        {
            get => _to; 
            set
            {
                _to = value;
                OnPropertyChanged();
                _ = LoadDataAsync();
            }
        }

        public int ExaminationsCount
        {
            get { return _examinationsCount; }
            set 
            { 
                _examinationsCount = value;
                OnPropertyChanged();
            }
        }

        public int ExaminationsNegativeCount
        {
            get { return _examinationsNegativeCount; }
            set 
            { 
                _examinationsNegativeCount = value;
                OnPropertyChanged();
            }
        }

        public int ExaminationsPositiveCount
        {
            get { return _examinationsPositiveCount; }
            set 
            { 
                _examinationsPositiveCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand CmdFilterReset { get; set; }
        public ICommand CmdTestParticipant { get; set; }

        public MainViewModel(IWindowController controller) : base(controller)
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
            CmdFilterReset = new RelayCommand(async _ => await FilterResetAsync(), _ => From != null || To != null);
            CmdTestParticipant = new RelayCommand(async _ => await TestParticipantAsync(), _ => true);
        }

        private async Task FilterResetAsync()
        {
            _from = null;
            OnPropertyChanged(nameof(From));
            _to = null;
            OnPropertyChanged(nameof(To));
            await LoadDataAsync();
        }

        private async Task TestParticipantAsync()
        {
            var viewModel = await TestParticipantViewModel.CreateAsync(Controller);
            Controller.ShowWindow(viewModel, true);
            _ = LoadDataAsync();
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        public static async Task<MainViewModel> CreateAsync(IWindowController windowController)
        {
            var viewModel = new MainViewModel(windowController);
            await viewModel.LoadDataAsync();
            return viewModel;
        }

        private async Task LoadDataAsync()
        {
            await using IUnitOfWork unitOfWork = new UnitOfWork();

            var examinations = await unitOfWork.Examinations.GetExaminationDtosWithFilterAsync(From, To);
            Examinations = new ObservableCollection<ExaminationDto>(examinations);

            ExaminationsCount = Examinations.Count;
            ExaminationsNegativeCount = Examinations.Count(_ => _.TestResult == TestResult.Negative);
            ExaminationsPositiveCount = Examinations.Count(_ => _.TestResult == TestResult.Positive);
        }
    }
}
