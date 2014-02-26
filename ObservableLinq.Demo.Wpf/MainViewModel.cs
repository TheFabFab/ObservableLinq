using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservableLinq.Demo.Wpf
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ObservableCollection<int> _collection;

        public MainViewModel()
        {
            _collection = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        }

        public ObservableCollection<int> Collection
        {
            get { return _collection; }
        }
    }
}
