using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using RepositoryParser.Controls.Common;

namespace RepositoryParser.Controls.MahAppsDialogOverloadings.InformationDialog
{
    public class InformationDialogViewModel : NotifyProperyChangeBase
    {
        private InformationType _informationType;
        private string _dialogTitle;
        private string _dialogMessage;
        private string _buttonText;
        private ICommand _closeWindowCommand;
        private ICommand _okButtonCommand;
        private MetroWindow _metroWindow;

        public InformationDialogViewModel(CustomDialogEntryData data)
        {
            if (data != null)
            {
                this.DialogTitle = data.DialogTitle;
                this.DialogMessage = data.DialogMessage;
                this.ButtonText = data.OkButtonMessage;
                this._metroWindow = data.MetroWindow;
                this.InformationType = data.InformationType;
                if (data.OkCommand != null)
                {
                    this.OkButtonCommand = data.OkCommand;
                }
            }
        }

        public InformationType InformationType
        {
            get
            {
                return _informationType;
                
            }
            set
            {
                if (_informationType == value)
                    return;
                _informationType = value;
                this.OnPropertyChanged("InformationType");
            }
        }

        public ICommand CloseWindowCommand
        {
            get
            {
                return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand((param) =>
                {
                    if(this.OkButtonCommand!= null)
                        this.OkButtonCommand.Execute(this);
                    if (param != null && param is InformationDialog)
                    {
                        if (this._metroWindow != null)
                        {
                            _metroWindow.HideMetroDialogAsync((InformationDialog) param);
                        }

                    }
                }));
            }
        }

        public ICommand OkButtonCommand
        {
            get
            {
                return _okButtonCommand ?? (_okButtonCommand = new RelayCommand((param) =>
                {
                    
                }));
            }
            set { _okButtonCommand = value; }
        }

        public string ButtonText
        {
            get
            {
                return _buttonText;
            }
            set
            {
                if (_buttonText == value)
                    return;
                _buttonText = value;
                this.OnPropertyChanged("ButtonText");
            }
        }

        public string DialogTitle
        {
            get
            {
                return _dialogTitle;
            }
            set
            {
                if (_dialogTitle == value)
                    return;
                _dialogTitle = value;
                this.OnPropertyChanged("DialogTitle");
            }
        }

        public string DialogMessage
        {
            get
            {
                return _dialogMessage;
            }
            set
            {
                if (_dialogMessage == value)
                    return;
                _dialogMessage = value;
                this.OnPropertyChanged("DialogMessage");
            }
        }
    }
}
