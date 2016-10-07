using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RepositoryParser.Controls.Annotations;

namespace RepositoryParser.Controls.Common
{
    public abstract class NotifyProperyChangeBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache =
            new Dictionary<string, PropertyChangedEventArgs>();


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (_argsCache != null && propertyName != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache[propertyName] = new PropertyChangedEventArgs(propertyName);

                OnPropertyChanged(_argsCache[propertyName]);
            }
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

    }
}
