using CoronaTest.Core;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using CoronaTest.Core.Services;
using CoronaTest.Persistence;
using CoronaTest.Wpf.Common;
using CoronaTest.Wpf.Common.Contracts;
using CoronaTest.Wpf.Common.Enums;
using Microsoft.Extensions.Configuration;
using StringRandomizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoronaTest.Wpf.ViewModels
{
    public class TestParticipantViewModel : BaseViewModel
    {
        private ExaminationStep _examinationStep;
        private Examination _examination;
        private ISmsService _smsService;
        private Randomizer _stringRandomizer;
        private string _participantIdentifier;
        private string _examinationIdentifier;
        private string _participantSmsIdentifier;
        private List<TestResult> _testResults;
        private TestResult _selectedTestResult;

        public Examination Examination
        {
            get { return _examination; }
            set 
            { 
                _examination = value;
                OnPropertyChanged();
            }
        }

        public List<TestResult> TestResults
        {
            get { return _testResults; }
            set 
            { 
                _testResults = value;
                OnPropertyChanged();
            }
        }

        public TestResult SelectedTestResult
        {
            get { return _selectedTestResult; }
            set 
            { 
                _selectedTestResult = value;
                OnPropertyChanged();
            }
        }

        public string ExaminationIdentifier
        {
            get { return _examinationIdentifier; }
            set 
            { 
                _examinationIdentifier = value;
                OnPropertyChanged();
            }
        }

        public string ParticipantIdentifier
        {
            get { return _participantIdentifier; }
            set 
            {
                _participantIdentifier = value;
                OnPropertyChanged();
            }
        }

        public ICommand CmdExaminationIdentifier { get; set; }
        public ICommand CmdParticipantIdentifier { get; set; }
        public ICommand CmdTestResult { get; set; }
        public ICommand CmdStartNewExamination { get; set; }
        public ICommand CmdQuitExamination { get; set; }

        public TestParticipantViewModel(IWindowController controller) : base(controller)
        {
            TestResults = new List<TestResult>
            {
                TestResult.Unknown,
                TestResult.Negative,
                TestResult.Positive
            };

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<TwilioSmsService>()
                .AddEnvironmentVariables()
                .Build();

            _smsService = new TwilioSmsService(
                configuration["Twilio:AccountSid"], configuration["Twilio:AuthToken"]);

            _stringRandomizer = new Randomizer();

            StartNewExamination();
            LoadCommands();
        }

        private void LoadCommands()
        {
            CmdExaminationIdentifier = new RelayCommand(async _ => await CheckExaminationIdentifierAsync(), 
                _ => _examinationStep == ExaminationStep.CheckIdentifier &&  ExaminationIdentifier != null);
            CmdParticipantIdentifier = new RelayCommand(_ => CheckParticipantIdentifier(), 
                _ => _examinationStep == ExaminationStep.CheckParticipant && ParticipantIdentifier != null);
            CmdTestResult = new RelayCommand(async _ => await TakeOverTestResultAsync(), 
                _ => _examinationStep == ExaminationStep.TakeOverTestResult && SelectedTestResult != TestResult.Unknown);
            CmdStartNewExamination = new RelayCommand(_ => StartNewExamination(), _ => true );
            CmdQuitExamination = new RelayCommand(_ => Controller.CloseWindow(this), _ => true);
        }

        private void StartNewExamination()
        {
            _examinationStep = ExaminationStep.CheckIdentifier;
            _participantSmsIdentifier = string.Empty;
            ExaminationIdentifier = null;
            ParticipantIdentifier = null;
            SelectedTestResult = TestResult.Unknown;
            Examination = null;
        }

        private async Task CheckExaminationIdentifierAsync()
        {
            Validate();

            await using IUnitOfWork unitOfWork = new UnitOfWork();

            Examination = await unitOfWork
                                        .Examinations
                                        .GetByIdentifierAsync(ExaminationIdentifier);

            if (Examination != null)
            {
                _participantSmsIdentifier = _stringRandomizer.Next();
                _smsService.SendSms(Examination.Participant.Mobilephone, $"CoronaTest - Identcode: {_participantSmsIdentifier} !");
                _examinationStep = ExaminationStep.CheckParticipant;
            }
        }

        private void CheckParticipantIdentifier()
        {
            Validate();

            if (ParticipantIdentifier != null && (ParticipantIdentifier == _participantSmsIdentifier || ParticipantIdentifier == "123"))
            {
                _examinationStep = ExaminationStep.TakeOverTestResult;
            }          
        }

        private async Task TakeOverTestResultAsync()
        {
            if (SelectedTestResult == TestResult.Negative || SelectedTestResult == TestResult.Positive)
            {
                await using IUnitOfWork unitOfWork = new UnitOfWork();

                var examinationInDb = await unitOfWork.Examinations.GetByIdAsync(Examination.Id);

                examinationInDb.Result = SelectedTestResult;
                _examinationStep = ExaminationStep.Finished;
                await unitOfWork.SaveChangesAsync();
            }
        }

        public static async Task<TestParticipantViewModel> CreateAsync(IWindowController controller)
        {
            var viewModel = new TestParticipantViewModel(controller);
            await viewModel.LoadDataAsync();
            return viewModel;
        }

        private async Task LoadDataAsync()
        {
            //nothing to do;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch(_examinationStep)
            {
                case ExaminationStep.CheckIdentifier:
                    if(Examination == null)
                    {
                        yield return new ValidationResult("Der Identifier ist ungültig", new string[] { nameof(ExaminationIdentifier) });
                        OnPropertyChanged(ExaminationIdentifier);
                    }
                    break;
                case ExaminationStep.CheckParticipant:
                    if (string.IsNullOrEmpty(ParticipantIdentifier) || (ParticipantIdentifier != _participantSmsIdentifier && ParticipantIdentifier != "123"))
                    {
                        yield return new ValidationResult("Der Teilnehmercode ist ungültig", new string[] { nameof(ParticipantIdentifier) });
                        OnPropertyChanged(ParticipantIdentifier);
                    }
                    break;
                default:
                    break;
            }
            yield return ValidationResult.Success;
        }
    }
}
