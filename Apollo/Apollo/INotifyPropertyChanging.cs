using System.ComponentModel;

namespace Apollo
{
    /// <summary>
    ///  claiming to be able to notify before a property is changed
    /// </summary>
    public interface INotifyPropertyChanging
    {
        #region Events

        event PropertyChangedEventHandler PropertyChanging;

        #endregion
    }
}
