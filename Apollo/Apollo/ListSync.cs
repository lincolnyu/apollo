using System;
using System.Collections;
using System.Collections.Specialized;

namespace Apollo
{
    public sealed class ListSync : IDisposable
    {
        #region Delegates

        public delegate object ConvertDelegate(object source);

        #endregion

        #region Fields

        private bool _disposed;

        private readonly IList _source;

        private readonly IList _target;

        private readonly ConvertDelegate _convert;

        #endregion

        #region Constructors

        public ListSync(IList target, IList source, ConvertDelegate convert)
        {
            _target = target;
            _source = source;
            _convert = convert;

            Resync();

            var sourceNotify = (INotifyCollectionChanged) source;
            sourceNotify.CollectionChanged += OnListChanged;
        }

        ~ListSync()
        {
            MyDispose();
        }

        #endregion

        #region Methods

        #region IDisposable members

        public void Dispose()
        {
            MyDispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        private void MyDispose()
        {
            if (_disposed)
            {
                return;
            }

            var sourceNotify = (INotifyCollectionChanged)_source;
            sourceNotify.CollectionChanged -= OnListChanged;

            _disposed = true;
        }

        private void OnListChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    var startIndex = args.NewStartingIndex;
                    if (startIndex >= 0)
                    {
                        var index = startIndex;
                        foreach (var item in args.NewItems)
                        {
                            _target.Insert(index++, _convert(item));
                        }
                    }
                    else
                    {
                        foreach (var item in args.NewItems)
                        {
                            _target.Add(_convert(item));
                        }
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    var startIndex = args.OldStartingIndex;
                    if (startIndex >= 0)
                    {
                        var count = args.OldItems.Count;
                        for (var i = startIndex + count - 1; i >= startIndex; i--)
                        {
                            _target.RemoveAt(i);
                        }
                    }
                    else
                    {
                        Resync();
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    var ti = args.NewStartingIndex;
                    if (ti >= 0)
                    {
                        var count = args.NewItems.Count;
                        for (var i = 0; i < count; i++)
                        {
                            _target[ti] = _convert(args.NewItems[i]);
                        }
                    }
                    else
                    {
                        Resync();
                    }
                    break;
                }
                default:
                Resync();
                    break;
            }
        }

        private void Resync()
        {
            _target.Clear();
            foreach (var item in _source)
            {
                _target.Add(_convert(item));
            }
        }

        #endregion
    }
}
