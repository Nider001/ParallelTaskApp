using ParallelTaskApp.App.Common;
using ParallelTaskApp.App.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelTaskApp.App.DAL
{
    public class RainfallDA : IRainfallDA
    {
        private readonly string connectionstring;

        public RainfallDA()
        {
            connectionstring = @"Data Source=.\SQLEXPRESS; Initial Catalog=ParallelTaskDatabase; Integrated Security=True";
        }

        public List<string> GetAllSubdivisions()
        {
            List<string> res = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select Subdivision from Subdivisions", connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(reader["Subdivision"].ToString());
                        }
                    }
                }
            }

            return res;
        }

        public bool CheckSubdivision(string subdivision)
        {
            bool result = false;

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select count(*) from Subdivisions where Subdivision like @subdivision", connection))
                {
                    connection.Open();
                    sqlCommand.Parameters.AddWithValue("@subdivision", subdivision);
                    result = (int)sqlCommand.ExecuteScalar() == 0 ? false : true;
                }
            }

            return result;
        }

        public List<RainfallDataRow> ExtractData()
        {
            List<RainfallDataRow> res = new List<RainfallDataRow>();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select Subdivision, YEAR, JUN, JUL, AUG, SEP, JUN_SEP from Rainfalls inner join Subdivisions on Rainfalls.subdivisionID = Subdivisions.ID", connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new RainfallDataRow(
                                reader["Subdivision"].ToString(),
                                (int)reader["YEAR"],
                                Math.Max((double)reader["JUN"], 0.0),
                                Math.Max((double)reader["JUL"], 0.0),
                                Math.Max((double)reader["AUG"], 0.0),
                                Math.Max((double)reader["SEP"], 0.0),
                                Math.Max((double)reader["JUN_SEP"], 0.0)
                                ));
                        }
                    }
                }
            }

            return res;
        }

        private void PutDataIntoSubdivisionEntry(KeyValuePair<string, List<RainfallDataRow>> entry)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select Subdivision, YEAR, JUN, JUL, AUG, SEP, JUN_SEP from Rainfalls inner join Subdivisions on Rainfalls.subdivisionID = Subdivisions.ID WHERE Subdivision like @subdivision", connection))
                {
                    connection.Open();
                    sqlCommand.Parameters.AddWithValue("@subdivision", entry.Key);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entry.Value.Add(new RainfallDataRow(
                                reader["Subdivision"].ToString(),
                                (int)reader["YEAR"],
                                Math.Max((double)reader["JUN"], 0.0),
                                Math.Max((double)reader["JUL"], 0.0),
                                Math.Max((double)reader["AUG"], 0.0),
                                Math.Max((double)reader["SEP"], 0.0),
                                Math.Max((double)reader["JUN_SEP"], 0.0)
                                ));
                        }
                    }
                }
            }
        }

        public Dictionary<string, List<RainfallDataRow>> ExtractDataBySubdivision()
        {
            var subdivisions = GetAllSubdivisions();

            Dictionary<string, List<RainfallDataRow>> res = new Dictionary<string, List<RainfallDataRow>>(subdivisions.Count);

            subdivisions.ForEach(x => res.Add(x, new List<RainfallDataRow>()));

            foreach (var entry in res)
            {
                PutDataIntoSubdivisionEntry(entry);
            }

            return res;
        }

        private void PutDataIntoYearEntry(KeyValuePair<int, List<RainfallDataRow>> entry)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select Subdivision, YEAR, JUN, JUL, AUG, SEP, JUN_SEP from Rainfalls inner join Subdivisions on Rainfalls.subdivisionID = Subdivisions.ID WHERE YEAR = @year", connection))
                {
                    connection.Open();
                    sqlCommand.Parameters.AddWithValue("@year", entry.Key);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entry.Value.Add(new RainfallDataRow(
                                reader["Subdivision"].ToString(),
                                (int)reader["YEAR"],
                                Math.Max((double)reader["JUN"], 0.0),
                                Math.Max((double)reader["JUL"], 0.0),
                                Math.Max((double)reader["AUG"], 0.0),
                                Math.Max((double)reader["SEP"], 0.0),
                                Math.Max((double)reader["JUN_SEP"], 0.0)
                                ));
                        }
                    }
                }
            }
        }

        public Dictionary<int, List<RainfallDataRow>> ExtractDataByYear(int startYear = 1901, int endYear = 2021)
        {
            Dictionary<int, List<RainfallDataRow>> res = new Dictionary<int, List<RainfallDataRow>>(endYear - startYear);

            for (int i = startYear; i <= endYear; i++)
            {
                res.Add(i, new List<RainfallDataRow>());
            }

            foreach (var entry in res)
            {
                PutDataIntoYearEntry(entry);
            }

            return res;
        }

        public Dictionary<string, double> GetAveragesBySubdivision()
        {
            Dictionary<string, double> res = new Dictionary<string, double>();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select Subdivision, avg(JUN_SEP) as JUN_SEP from Rainfalls inner join Subdivisions on Rainfalls.subdivisionID = Subdivisions.ID group by Subdivision", connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(reader["Subdivision"].ToString(), Math.Max((double)reader["JUN_SEP"], 0.0));
                        }
                    }
                }
            }

            return res;
        }

        public Dictionary<int, double> GetAveragesByYear()
        {
            Dictionary<int, double> res = new Dictionary<int, double>();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select YEAR, avg(JUN_SEP) as JUN_SEP from Rainfalls inner join Subdivisions on Rainfalls.subdivisionID = Subdivisions.ID group by YEAR", connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add((int)reader["YEAR"], Math.Max((double)reader["JUN_SEP"], 0.0));
                        }
                    }
                }
            }

            return res;
        }
    }
}
