using DataWpf_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DataWpf_ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private Person currentPerson;
        private PersonCollection personList;
        private IEnumerable<Person> personListFilter;
        private string filterText;
        private ICommand deleteCommand;
        private Mediator mediator;

        #endregion

        #region Properties

        public Person CurrentPerson
        {
            get { return currentPerson; }
            set
            {
                if (currentPerson == value)
                {
                    return;
                }
                currentPerson = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentPerson"));
            }
        }

        public PersonCollection PersonList
        {
            get { return personList; }
            set
            {
                if (personList == value)
                {
                    return;
                }
                personList = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PersonList"));
            }
        }
        
        public IEnumerable<Person> PersonListFilter
        {
            get { return personListFilter; }
            set
            {
                if (personListFilter == value)
                {
                    return;
                }
                personListFilter = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PersonListFilter"));
            }
        }

        public string FilterText
        {
            get { return filterText; }
            set
            {
                if (filterText == value)
                    return;
                filterText = value;
                PersonListFilter = PersonList.Where(PersonFilter);
                OnPropertyChanged(new PropertyChangedEventArgs("FilterText"));
            }
        }
        public ICommand DeleteCommand
        {
            get { return deleteCommand; }
            set
            {
                if (deleteCommand == value)
                    return;
                deleteCommand = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DeleteCommand"));
            }
        }
        #endregion

        #region Constructors

        public MainWindowViewModel(Mediator mediator)
        {
            PersonList = PersonCollection.GetAllPersons();
            personListFilter = PersonList;
            CurrentPerson = new Person();
            DeleteCommand = new RelayCommand(DeleteExecute, CanDelete);
            this.mediator = mediator;
            mediator.Register("Person changed", PersonChanged);
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        //logika metode za filtriranje

        private bool PersonFilter(object obj)
        {
            if (FilterText == null) return true;
            if (FilterText.Equals("")) return true;

            Person person = obj as Person;
            return (person.FirstName.ToLower().StartsWith(FilterText.ToLower())) || (person.LastName.ToLower().StartsWith(FilterText.ToLower()));
        }

        //Execute i CanExecute metode komande za brisanje
        void DeleteExecute(object obj)
        {
            CurrentPerson.DeletePerson();
            PersonList.Remove(CurrentPerson);
        }

        bool CanDelete(object obj)
        {
            if (CurrentPerson == null)
            {
                return false;
            }
            return true;
        }

        //logika koja ce se aktivirati kada bude unesena nova ili izmijenjena postojeca osoba
        public void PersonChanged(object obj)
        {
            Person person = (Person)obj;
            int index = PersonList.IndexOf(person);
            if (index != -1)
            {
                PersonList.RemoveAt(index);
                PersonList.Insert(index, person);
            }
            else
            {
                PersonList.Add(person);
            }
        }
    }
}
