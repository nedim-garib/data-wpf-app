using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataWpf_Model
{
    public class PersonCollection : ObservableCollection<Person>
    {
        public static PersonCollection GetAllPersons()
        {

            PersonCollection persons = new PersonCollection();
            Person person = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT id, first_name, last_name, date_of_birth FROM Person WHERE is_deleted = 0", conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        person = Person.GetPersonFromResultSet(reader);
                        persons.Add(person);
                    }
                }
            }
            return persons;
        }
    }
}
