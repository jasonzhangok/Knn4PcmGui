using System.Collections.Generic;
using System.ComponentModel;
using Lof4PcmGui.Lof4Pcm;

namespace Lof4PcmGui.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private List<Data4UfoPcm> _eaDataLoaded4Lof;
        public List<Data4UfoPcm> EaDataLoaded4Lof
        {
            get { return _eaDataLoaded4Lof; }
            set
            {
                _eaDataLoaded4Lof = value;
            }
        }

    }
}
