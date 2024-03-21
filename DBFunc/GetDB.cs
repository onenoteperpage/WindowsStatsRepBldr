using Microsoft.Data.Sqlite;
using WindowsStatsRepBldr.Models;

namespace WindowsStatsRepBldr.DBFunc
{
    internal class GetDB
    {
        public static string[] GetTableNames(string sqliteFilePath)
        {
            List<string> tableNames = new List<string>();

            using (SqliteConnection connection = new SqliteConnection($"Data Source={sqliteFilePath};"))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }
                }

                return tableNames.ToArray();
            }
        }

        public static List<WinboxDataObj> GetWinboxDataObjs(string sqliteFilePath, string tableName)
        {
            List<WinboxDataObj> winboxDataObjs = new List<WinboxDataObj>();

            using (SqliteConnection connection = new SqliteConnection($"Data Source={sqliteFilePath};"))
            {
                connection.Open();

                string query = $"SELECT * FROM {tableName}";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WinboxDataObj dataObj = new WinboxDataObj
                            {

                                Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp")),
                                Value = reader.GetDouble(reader.GetOrdinal("Value"))
                            };

                            winboxDataObjs.Add(dataObj);
                        }
                    }
                }
            }

            // Sort the list by Timestamp in ascending order (oldest to latest)
            //List<WinboxDataObj> sortedWinboxDataObjs = winboxDataObjs.OrderBy(obj => obj.Timestamp).ToList();
            //return sortedWinboxDataObjs;

            return winboxDataObjs;
        }
    }
}
