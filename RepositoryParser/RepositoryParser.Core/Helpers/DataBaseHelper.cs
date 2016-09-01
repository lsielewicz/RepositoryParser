using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Helpers
{
    public class DataBaseHelper
    {
        public bool IsDataBaseEmpty(SQLiteConnection connection)
        {
            List<int> ids = new List<int>();
            string query = "Select ID from Repository";

            SQLiteCommand command = new SQLiteCommand(query,connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader != null && reader.Read())
            {
                ids.Add(Convert.ToInt32(reader["ID"]));
            }

            return !ids.Any();
        }
    }
}
