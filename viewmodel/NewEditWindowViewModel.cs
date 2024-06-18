using DataWpf_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataWpf_ViewModel
{
    public class NewEditWindowViewModel : INotifyPropertyChanged
    {
        private Person? currentPerson;
        private string windowTitle;
        private ICommand saveCommand;
        private Mediator mediator;

        public Person? CurrentPerson
        {
            get { return currentPerson; }
            set
            {
                if (currentPerson == value)
                    return;
                currentPerson = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentPerson"));
            }
        }

        public string WindowTitle
        {
            get
            {
                return windowTitle;
            }
            set
            {
                if (windowTitle == value)
                    return;
                windowTitle = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WindowTitle"));
            }
        }

        public ICommand SaveCommand
        {
            get { return saveCommand; }
            set
            {
                if (saveCommand == value)
                {
                    return;
                }
                saveCommand = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SaveCommand"));
            }
        }
        public NewEditWindowViewModel(Person person, Mediator mediator)
        {
            SaveCommand = new RelayCommand(SaveExecute, CanSave); //kreiranje komande
            CurrentPerson = person;
            this.mediator = mediator;
            WindowTitle = "Edit Person";
        }

        public NewEditWindowViewModel(Mediator mediator)
        {
            SaveCommand = new RelayCommand(SaveExecute, CanSave); //kreiranje komande
            CurrentPerson = new Person();
            this.mediator = mediator;
            WindowTitle = "New Person";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        //dogadjaj za zatvaranje prozora nakon uspjesnog cuvanja podataka
        public delegate void DoneEventHandler(object sender, DoneEventArgs e);

        public class DoneEventArgs : EventArgs
        {
            private string message;
            public string Message
            {
                get { return message; }
                set
                {
                    if (message == value)
                        return;
                    message = value;
                }
            }
            private bool success;
            public bool Success
            {
                get { return success; }
                set { success = value; }
            }
            public DoneEventArgs(string message, bool success)
            {
                this.message = message;
                this.success = success;
            }
        }
        public event DoneEventHandler Done;
        public void OnDone(DoneEventArgs e)
        {
            if (Done != null)
            {
                Done(this, e);
            }
        }

        //Execute i CanExecute metode komande Save
        void SaveExecute(object obj)
        {

            if (CurrentPerson != null && !CurrentPerson.HasErrors)
            {
                CurrentPerson.Save();
                OnDone(new DoneEventArgs("Person saved.", true));
                mediator.Notify("Person changed", CurrentPerson);
            }
            else
            {
                OnDone(new DoneEventArgs("Invalid input.", false));
            }
        }

        bool CanSave(object obj)
        {
            return true;
        }
    }
}
