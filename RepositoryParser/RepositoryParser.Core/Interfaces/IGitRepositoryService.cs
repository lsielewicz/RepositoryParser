
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Interfaces
{
    public interface IGitRepositoryService
    {
        void InitializeConnection();
        void FillDataBase();
        void ConnectRepositoryToDataBase(bool isNewFile);
        void ConnectRepositoryToDataBase();
    }
}
