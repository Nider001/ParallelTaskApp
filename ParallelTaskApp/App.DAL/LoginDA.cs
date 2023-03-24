using ParallelTaskApp.App.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelTaskApp.App.DAL
{
    public class LoginDA : ILoginDA
    {
        private readonly string connectionstring;

        public LoginDA()
        {
            connectionstring = @"Data Source=.\SQLEXPRESS; Initial Catalog=ParallelTaskDatabase; Integrated Security=True";
        }

        public bool CheckLogin(string login)
        {
            bool result = false;

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select count(*) from DatabaseUsers where UserName like @login", connection))
                {
                    connection.Open();
                    sqlCommand.Parameters.AddWithValue("@login", login);
                    result = (int)sqlCommand.ExecuteScalar() == 0 ? false : true;
                }
            }

            return result;
        }

        public bool CheckPassword(string login, string password)
        {
            bool result = false;

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select count(*) from DatabaseUsers where UserName like @login and Password like @password", connection))
                {
                    connection.Open();
                    sqlCommand.Parameters.AddWithValue("@login", login);
                    sqlCommand.Parameters.AddWithValue("@password", password);
                    result = (int)sqlCommand.ExecuteScalar() == 0 ? false : true;
                }
            }

            return result;
        }
    }
}
