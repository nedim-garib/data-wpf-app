using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataWpf_Model
{
    public class Person : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Fields

        private int _id;
        private string _firstName;
        private string _lastName;
        private DateTime? _dateOfBirth;

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #region Properties

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                {
                    return;
                }
                _id = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Id"));
            }
        }
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName == value)
                {
                    return;
                }
                _firstName = value;

                List<string> errors = new List<string>();
                bool valid = true;
                if (value == null || value == "")
                {
                    errors.Add("First name can not be empty!");
                    SetErrors("FirstName", errors);
                    valid = false;
                }
                if (!Regex.Match(value, @"^[a-zA-Z]+$").Success)
                {
                    errors.Add("First name can only contain letters!");
                    SetErrors("FirstName", errors);
                    valid = false;
                }
                if (valid)
                {
                    ClearErrors("FirstName");
                }

                OnPropertyChanged(new PropertyChangedEventArgs("FirstName"));
            }
        }
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName == value)
                {
                    return;
                }
                _lastName = value;

                List<string> errors = new List<string>();
                bool valid = true;
                if (value == null || value == "")
                {
                    errors.Add("Last name can not be empty!");
                    SetErrors("LastName", errors);
                    valid = false;
                }
                if (!Regex.Match(value, @"^[a-zA-Z]+$").Success)
                {
                    errors.Add("Last name can only contain letters!");
                    SetErrors("LastName", errors);
                    valid = false;
                }
                if (valid)
                {
                    ClearErrors("LastName");
                }

                OnPropertyChanged(new PropertyChangedEventArgs("LastName"));
            }
        }
        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;

                List<string> errors = new List<string>();
                bool valid = true;

                if (value == null)
                {
                    errors.Add("Date of birth can not be empty!");
                    SetErrors("DateOfBirth", errors);
                    valid = false;
                }
                if (value < new DateTime(1880, 12, 31))
                {
                    errors.Add("Date of birth can not be before 31st December 1880!");
                    SetErrors("DateOfBirth", errors);
                    valid = false;
                }
                if (value > DateTime.Now)
                {
                    errors.Add("Date of birth can not be in future!");
                    SetErrors("DateOfBirth", errors);
                    valid = false;
                }
                if (valid)
                {
                    ClearErrors("DateOfBirth");
                }
                OnPropertyChanged(new PropertyChangedEventArgs("DateOfBirth"));
            }
        }

        //svojstvo koje govori da li odredjeno svojstvo posjeduje validacione greske ili ne
        public bool HasErrors
        {
            get
            {
                return (errors.Count > 0);
            }
        }

        #endregion

        #region Constructors

        public Person()
        {
            FirstName = "";
            LastName = "";
            DateOfBirth = null;
        }

        public Person(int id, string firstName, string lastName, DateTime? dateOfBirth)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }
        public Person(string firstName, string lastName, DateTime? dateOfBirth)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }
        #endregion

        public static Person GetPersonFromResultSet(SqlDataReader reader)
        {
            Person person = new Person((int)reader["id"], (string)reader["first_name"], (string)reader["last_name"], (DateTime)reader["date_of_birth"]);
            return person;
        }

        //brisanje podataka

        public void DeletePerson()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();

                SqlCommand command = new SqlCommand("UPDATE Person SET is_deleted=1 WHERE id=@Id", conn);
                SqlParameter param = new SqlParameter("@Id", System.Data.SqlDbType.Int, 11);
                param.Value = this.Id;

                command.Parameters.Add(param);
                int rows = command.ExecuteNonQuery();
            }
        }

        //azuriranje podataka

        public void UpdatePerson()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();

                SqlCommand command = new SqlCommand("UPDATE Person SET first_name=@FirstName, last_name=@LastName, date_of_birth=@DateOfBirth WHERE id=@Id", conn);

                SqlParameter firstNameParam = new SqlParameter("@FirstName", System.Data.SqlDbType.NVarChar);
                firstNameParam.Value = this.FirstName;

                SqlParameter lastNameParam = new SqlParameter("@LastName", System.Data.SqlDbType.NVarChar);
                lastNameParam.Value = this.LastName;

                SqlParameter dateOfBirthParam = new SqlParameter("@DateOfBirth", System.Data.SqlDbType.Date);
                dateOfBirthParam.Value = this.DateOfBirth;

                SqlParameter param = new SqlParameter("@Id", System.Data.SqlDbType.Int, 11);
                param.Value = this.Id;

                command.Parameters.Add(firstNameParam);
                command.Parameters.Add(lastNameParam);
                command.Parameters.Add(dateOfBirthParam);
                command.Parameters.Add(param);

                int rows = command.ExecuteNonQuery();
            }
        }

        //unos novih podataka

        public void InsertPerson()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Person(first_name, last_name, date_of_birth, is_deleted) VALUES(@FirstName, @LastName, @DateOfBirth, 0); SELECT IDENT_CURRENT('Person');", conn);

                SqlParameter firstNameParam = new SqlParameter("@FirstName", SqlDbType.NVarChar);
                firstNameParam.Value = this.FirstName;

                SqlParameter lastNameParam = new SqlParameter("@LastName", SqlDbType.NVarChar);
                lastNameParam.Value = this.LastName;

                SqlParameter dateOfBirthParam = new SqlParameter("@DateOfBirth", SqlDbType.Date);
                dateOfBirthParam.Value = this.DateOfBirth;

                command.Parameters.Add(firstNameParam);
                command.Parameters.Add(lastNameParam);
                command.Parameters.Add(dateOfBirthParam);

                var id = command.ExecuteScalar();

                if (id != null)
                {
                    this.Id = Convert.ToInt32(id);
                }
            }
        }

        //pomocna metoda

        public void Save()
        {
            if (Id == 0)
            {
                InsertPerson();
            }
            else
            {
                UpdatePerson();
            }
        }

        public Person Clone()
        {
            Person clonedPerson = new Person();
            clonedPerson.FirstName = this.FirstName;
            clonedPerson.LastName = this.LastName;
            clonedPerson.DateOfBirth = this.DateOfBirth;
            clonedPerson.Id = this.Id;

            return clonedPerson;
        }

        //validacija podataka
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return (errors.Values);
            }
            else
            {
                if (errors.ContainsKey(propertyName))
                {
                    return (errors[propertyName]);
                }
                else
                {
                    return null;
                }
            }
        }

        //metode za rukovanje greskama

        private void SetErrors(string propertyName,  List<string> propertyErrors)
        {
            errors.Remove(propertyName);
            errors.Add(propertyName, propertyErrors);
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
        private void ClearErrors(string propertyName)
        {
            errors.Remove(propertyName);
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        //metoda za utvrdjivanje jednakosti objekata
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Person objPerson = (Person)obj;
            if (objPerson.Id == this.Id)
            {
                return true;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
