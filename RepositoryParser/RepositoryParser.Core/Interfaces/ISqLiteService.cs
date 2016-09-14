using System;
using System.Collections.Generic;

namespace RepositoryParser.Core.Interfaces
{
     [Obsolete]
     public interface ISqLiteService
     {
         void OpenConnection(string query);
         void OpenConnection(List<string> transactions);
         void ExecuteQuery(string query);
         void ExecuteTransaction(List<string> transactionsList);
         void CloseConnection();
     }
}
