using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Apollo.Properties;

namespace Apollo
{
    public class BaseViewModel<T> : INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        ///  Instantiates the base class with given model
        /// </summary>
        /// <param name="model">The model the instance of the class represents</param>
        public BaseViewModel(T model)
        {
            Model = model;

            var notifyChanged = Model as INotifyPropertyChanged;
            if (notifyChanged != null)
            {
                notifyChanged.PropertyChanged += ModelOnPropertyChanged;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///  The data model this instance of view-model represents
        /// </summary>
        public T Model { get; protected set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        ///  Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///  Maps a model property change to a view bound view model property passively reading it
        /// </summary>
        /// <param name="sender">The sender of the change event</param>
        /// <param name="args">The arguments that detail the change</param>
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
// ReSharper disable once ExplicitCallerInfoArgument
                        OnPropertyChanged(viewModelProperty);
                    }
                    if (info.AllowTrivialMapping && !viewModelProperties.Contains(modelProperty))
                    {
// ReSharper disable once ExplicitCallerInfoArgument
                        OnPropertyChanged(modelProperty);
                    }
                }
                else if (info.AllowTrivialMapping)
                {
// ReSharper disable once ExplicitCallerInfoArgument
                    OnPropertyChanged(modelProperty);
                }
            }
        }

        /// <summary>
        ///  Warns the developer if this object does not have
        ///  a public property with the specified name. This 
        ///  method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            var t = GetType();

            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (t.GetRuntimeProperty(propertyName) != null) return;

            var msg = "Invalid property name: " + propertyName;

            Debug.Assert(false, msg);
        }

        #endregion
    }
}
