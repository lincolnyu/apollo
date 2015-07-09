using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Apollo
{
    /// <summary>
    ///  Maintains sync between 
    /// </summary>
    /// <typeparam name="TModelCollection">The model collection type</typeparam>
    /// <typeparam name="TViewModelItem">The view model item type</typeparam>
    /// <typeparam name="TModelItem">The model item type</typeparam>
    public class ViewModelModelCollectionBinder<TModelCollection, TViewModelItem, TModelItem> : IDisposable
        where TModelCollection : ICollection<TModelItem>, INotifyCollectionChanged
    {
        #region Delegates

        public delegate TViewModelItem CreateViewModelDelegate(TModelItem model);

        #endregion

        #region Fields

        private readonly CreateViewModelDelegate _createViewModel;

        #endregion

        #region Constructors

        public ViewModelModelCollectionBinder(TModelCollection modelCollection, CreateViewModelDelegate createViewModel)
        {
            ViewModelDictionary = new Dictionary<TModelItem, TViewModelItem>();
            ViewModels = new ObservableCollection<TViewModelItem>();
            ModelCollection = modelCollection;

            _createViewModel = createViewModel;

            Resync();
            ModelCollection.CollectionChanged += ModelCollectionOnCollectionChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<TViewModelItem> ViewModels { get; private set; }

        public Dictionary<TModelItem, TViewModelItem> ViewModelDictionary { get; private set; }

        public TModelCollection ModelCollection { get; private set; }

        #endregion

        #region Methods

        #region IDisposable members

        public void Dispose()
        {
            if (ViewModelDictionary != null)
            {
                ModelCollection.CollectionChanged -= ModelCollectionOnCollectionChanged;
                ViewModelDictionary = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        private void ModelCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                Resync();
                return;
            }

            if (args.OldItems != null)
            {
                foreach (var oldItem in args.OldItems.OfType<TModelItem>())
                {
                    var vm = ViewModelDictionary[oldItem];
                    ViewModels.Remove(vm);
                    ViewModelDictionary.Remove(oldItem);
                }
            }
            if (args.NewItems != null)
            {
                foreach (var newItem in args.NewItems.OfType<TModelItem>())
                {
                    var vm = _createViewModel(newItem);
                    ViewModelDictionary[newItem] = vm;
                    ViewModels.Add(vm);
                }
            }
        }

        private void Resync()
        {
            ViewModelDictionary.Clear();
            ViewModels.Clear();
            foreach (var newItem in ModelCollection)
            {
                var vm = _createViewModel(newItem);
                ViewModelDictionary[newItem] = vm;
                ViewModels.Add(vm);
            }
        }

        #endregion
    }
}
