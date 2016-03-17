using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Interfaces
{
     public interface ISqLiteService
     {
         void OpenConnection(string query);
         void OpenConnection(List<string> transactions);
         void ExecuteQuery(string query);
         void ExecuteTransaction(List<string> transactionsList);
         void CloseConnection();
     }
}
