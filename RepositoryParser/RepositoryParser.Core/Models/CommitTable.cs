using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class CommitTable
    {
        #region Variabels
        public int ID { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string Email { get; set; }
        #endregion

       
        #region Constructors

        public CommitTable()
        {

        }
        public CommitTable(int id)
        {
            this.ID = id;
        }

        public CommitTable(int id, string message)
        {
            this.ID = id;
            this.Message = message;
        }

        public CommitTable(int id, string message, string author)
        {
            this.ID = id;
            this.Message = message;
            this.Author = author;
        }

        public CommitTable(int id, string message, string author, string date)
        {
            this.ID = id;
            this.Message = message;
            this.Author = author;
            this.Date = date;
        }
        public CommitTable(int id, string message, string author, string date, string email)
        {
            this.ID = id;
            this.Message = message;
            this.Author = author;
            this.Date = date;
            this.Email = email;
        }
        #endregion

        #region querys
        public static string SqliteQuery = "create table GitCommits(ID INTEGER PRIMARY KEY AUTOINCREMENT, Message varchar(200), Author varchar(30), Date DATETIME, Email varchar(40))";

        public static string InsertSqliteQuery(int id, string message, string author, string date, string email)
        {
            if (message.Contains("'"))
                message = SqLiteService.StripSlashes(message);
            if (author.Contains("'"))
                author = SqLiteService.StripSlashes(author);

            string query = "Insert into GitCommits (Message,Author,Date,Email) values (" +
                           "'" + message + "'" +
                           ", '" + author + "'" +
                           ", '" + date + "'" +
                           ", '" + email + "')";
            return query;
        }
        public int GetLastIndex(SQLiteConnection Connection)
        {
            int id = 0;
            string query = "select ID from GitCommits";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = Convert.ToInt32(reader["ID"]);
            }
            return id;
        }


        public static string InsertSqliteQuery(CommitTable obj)
        {
            if (obj.Message.Contains("'"))
                obj.Message = SqLiteService.StripSlashes(obj.Message);
            if (obj.Author.Contains("'"))
                obj.Author = SqLiteService.StripSlashes(obj.Author);

            string query = "Insert into GitCommits (Message,Author,Date,Email) values (" +
                           "'" + obj.Message + "'" +
                           ", '" + @obj.Author + "'" +
                           ", '" + @obj.Date + "'" +
                           ", '" + @obj.Email + "')";
            return query;
        }
        public List<CommitTable> GetDataFromBase(SQLiteConnection Connection)
        {
            List<CommitTable> tempList = new List<CommitTable>();

            string query = "select * from GitCommits";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                string message = Convert.ToString(reader["Message"]);
                string author = Convert.ToString(reader["Author"]);
                string date = Convert.ToString(reader["Date"]);
                date = SqLiteService.getDateTimeFormat(date);
                // date = date.Remove(19);
                string email = Convert.ToString(reader["Email"]);
                CommitTable tempInstance = new CommitTable(id, message, author, date, email);
                tempList.Add(tempInstance);
            }
            return tempList;
        }

        public static string deleteAllQuery = "DELETE FROM GitCommits";
        #endregion
    }
}
