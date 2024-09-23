using System.Collections.Generic;
using System.ComponentModel;
using Lof4PcmGui.Lof4Pcm;

namespace Lof4PcmGui.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {


        private List<Data4UfoPcm> _eaUfoDataLoaded4Lof;
        public List<Data4UfoPcm> EaUfoDataLoaded4Lof
        {
            get { return _eaUfoDataLoaded4Lof; }
            set
            {
                _eaUfoDataLoaded4Lof = value;
            }
        }
    }
}
