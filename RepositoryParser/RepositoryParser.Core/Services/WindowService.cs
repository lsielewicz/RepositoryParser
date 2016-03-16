using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.ViewModel;

namespace RepositoryParser.Core.Services
{
    public class WindowService : IWindowService
    {
        public void showWindow(object dataContext)
        {
            AnalisysWindowView window = new AnalisysWindowView();
            window.DataContext = dataContext;
            window.Show();
        }
    }
}
