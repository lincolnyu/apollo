using System;
using System.Collections.Generic;

namespace Apollo
{
    public class ViewModelInfoRegistry
    {
        #region Constructors

        static ViewModelInfoRegistry()
        {
            Instance = new ViewModelInfoRegistry();
        }

        public ViewModelInfoRegistry()
        {
            ViewModelToInfo = new Dictionary<Type, ViewModelInfo>();
        }

        #endregion

        #region Properties

        public static ViewModelInfoRegistry Instance { get; private set; }

        public Dictionary<Type, ViewModelInfo> ViewModelToInfo { get; private set; }

        #endregion
    }
}
