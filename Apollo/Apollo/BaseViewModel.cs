using System.ComponentModel;
using System.Runtime.CompilerServices;
using Apollo.Properties;

namespace Apollo
{
    public class BaseViewModel<T> : INotifyPropertyChanged where T : BaseModel
    {
        #region Constructors

        public BaseViewModel(T model)
        {
            Model = model;
            Model.PropertyChanged += ModelOnPropertyChanged;
        }

        #endregion

        #region Properties

        public T Model { get; protected set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var modelProperty = args.PropertyName;
            var thisType = GetType();
            if (ViewModelInfoRegistry.Instance.ViewModelToInfo.ContainsKey(thisType))
            {
                var info = ViewModelInfoRegistry.Instance.ViewModelToInfo[thisType];
                if (info.PropertyMapper.ContainsKey(modelProperty))
                {
                    var viewModelProperties = info.PropertyMapper[modelProperty];
                    foreach (var viewModelProperty in viewModelProperties)
                    {
                        OnPropertyChanged(viewModelProperty);
                    }
                    if (info.AllowTrivialMapping && !viewModelProperties.Contains(modelProperty))
                    {
                        OnPropertyChanged(modelProperty);
                    }
                }
                else if (info.AllowTrivialMapping)
                {
                    OnPropertyChanged(modelProperty);
                }
            }
        }

        #endregion
    }
}
