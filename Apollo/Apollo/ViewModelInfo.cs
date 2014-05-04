using System.Collections.Generic;

namespace Apollo
{
    public class ViewModelInfo
    {
        #region Constructors

        public ViewModelInfo()
        {
            PropertyMapper = new Dictionary<string, HashSet<string>>();
        }

        #endregion

        #region Properties

        public Dictionary<string, HashSet<string>> PropertyMapper { get; private set; }

        public bool AllowTrivialMapping { get; set; }

        #endregion
    }
}
