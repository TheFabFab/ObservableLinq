using Catel.MVVM;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObservableLinq.Demo.Wpf
{
    public class MainViewModel : ViewModelBase, IDragSource, IDropTarget
    {
        private readonly IList<DataOption> _dataOptions;
        private readonly ObservableCollection<int> _collection;
        private readonly ObserverCollection<int> _observer;

        private int _newItemValue;
        private DataOption _selectedOption;

        public MainViewModel()
        {
            _collection = new FixMoveObservableCollection<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            _newItemValue = _collection.Count + 1;

            _dataOptions = new List<DataOption>(new[] 
                { 
                    new DataOption("Original", _collection),
                    new DataOption("Distinct", _collection.ToQueryable().Distinct()),
                    new DataOption("Ordered Ascending", _collection.ToQueryable().OrderBy(x => x)),
                    new DataOption("Ordered Descending", _collection.ToQueryable().OrderByDescending(x => x)),
                    new DataOption("Even Numbers", _collection.ToQueryable().Where(x => x % 2 == 0)),
                    new DataOption("Ordered Even Numbers", _collection.ToQueryable().Where(x => x % 2 == 0).OrderBy(x => x)),
                    new DataOption("Squares", _collection.ToQueryable().Select(x => x * x)),
                });

            _observer = new ObserverCollection<int>();
            SelectedOption = _dataOptions.First();
        }

        public ObservableCollection<int> Collection
        {
            get { return _collection; }
        }

        public ObserverCollection<int> Observer
        {
            get { return _observer; }
        }

        public IList<DataOption> DataOptions
        {
            get { return _dataOptions; }
        } 

        public int NewItemValue
        {
            get { return _newItemValue; }
            set
            {
                if (_newItemValue != value)
                {
                    _newItemValue = value;
                    RaisePropertyChanged(() => NewItemValue);
                }
            }
        }

        public DataOption SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                if (!EqualityComparer<DataOption>.Default.Equals(_selectedOption, value))
                {
                    _selectedOption = value;
                    RaisePropertyChanged(() => SelectedOption);
                    Observer.Observe(SelectedOption.Expression);
                }
            }
        }

        public void DragCancelled()
        {
        }

        public void Dropped(IDropInfo dropInfo)
        {
            NewItemValue = _collection.Count + 1;
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            dragInfo.Data = _newItemValue;
            dragInfo.Effects = System.Windows.DragDropEffects.Copy;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is int)
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
        }

        private class FixMoveObservableCollection<T> : ObservableCollection<T>
        {
            public FixMoveObservableCollection(IEnumerable<T> collection)
                : base(collection)
            {

            }

            protected override void InsertItem(int index, T item)
            {
                bool moved = false;
                if (_removeIndex != -1)
                {
                    if (EqualityComparer<T>.Default.Equals(this[_removeIndex], item))
                    {
                        base.MoveItem(_removeIndex, index);
                        moved = true;
                        _removeIndex = -1;
                    }
                }

                if (!moved)
                {
                    base.InsertItem(index, item);
                }
            }

            protected override void RemoveItem(int index)
            {
                _removeIndex = index;
                Task.Factory.StartNew(() => RemoveCore(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            }

            private void RemoveCore()
            {
                if (_removeIndex != -1)
                {
                    base.RemoveItem(_removeIndex);
                    _removeIndex = -1;
                }
            }

            private int _removeIndex = -1;
        }
    }

    public struct DataOption : IEquatable<DataOption>
    {
        private readonly string _displayText;
        private readonly IList<int> _expression;

        public DataOption(string displayText, IList<int> expression)
        {
            _displayText = displayText;
            _expression = expression;
        }

        public string DisplayText
        {
            get { return _displayText; }
        }

        public IList<int> Expression
        {
            get { return _expression; }
        }

        public bool Equals(DataOption other)
        {
            return DisplayText == other.DisplayText;
        }
    }
}
