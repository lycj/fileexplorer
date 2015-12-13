using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Cinch;

namespace FileExplorer.WPF.ViewModels
{
    public class MessageDialogViewModel : Screen
    {
        public enum DialogButtons { None, OK, Cancel }

        public class DialogButtonViewModel : PropertyChangedBase
        {
            internal DialogButtonViewModel(DialogButtons button, MessageDialogViewModel dlvm)
            {
                Caption = button.ToString();
                Command = new SimpleCommand() { ExecuteDelegate = 
                    (obj) => 
                    {
                        dlvm.SelectedButton = button;
                    }  
                };
            }

            public string Caption { get; private set; }
            public ICommand  Command { get; private set; }
        }

        #region Constructor

        public MessageDialogViewModel(string caption, string message, DialogButtons buttons = DialogButtons.OK)
        {            
            base.DisplayName = caption;
            _message = message;


            foreach (var b in Enum.GetValues(typeof(DialogButtons)).Cast<DialogButtons>())
                if (b != DialogButtons.None && buttons.HasFlag(b))
                    _dialogButtons.Add(new DialogButtonViewModel(b, this));
        }
        
        #endregion

        #region Methods

        private void setSelectedButton(DialogButtons value)
        {
            _selectedButton = value;
            TryClose(_selectedButton == DialogButtons.OK);
        }

        #endregion

        #region Data
        
        private string _message;
        private DialogButtons _selectedButton = DialogButtons.None;
        private ObservableCollection<DialogButtonViewModel> _dialogButtons = new ObservableCollection<DialogButtonViewModel>();

        #endregion

        #region Public Properties

        public ObservableCollection<DialogButtonViewModel> Buttons { get { return _dialogButtons; } }
        public string Caption { get { return base.DisplayName; } set { base.DisplayName = value; NotifyOfPropertyChange(() => Caption); NotifyOfPropertyChange(() => DisplayName); } }
        public string Message { get { return _message; } set { _message = value; NotifyOfPropertyChange(() => Message); } }
        public DialogButtons SelectedButton { get { return _selectedButton; }
            set { setSelectedButton(value); }
        }

      

        #endregion
    }
}
