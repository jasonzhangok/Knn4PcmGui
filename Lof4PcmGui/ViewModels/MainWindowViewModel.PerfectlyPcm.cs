using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Lof4PcmGui.Lof4Pcm;

namespace Lof4PcmGui.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _dimensePerfectly = 5;
        /// <summary>
        /// Dimense of perfectly matrix
        /// </summary>
        public int DimensePerfectly 
        {
            get { return _dimensePerfectly; }
            set
            {
                _dimensePerfectly = value;
                OnPropertyChanged("DimensePerfectly");
            }
        }

        private bool _withTuningPerfectly= false;
        /// <summary>
        /// Apply a fine-tuning of [-0.5, 0.5] to each item?
        /// </summary>
        public bool WithTuningPerfectly 
        {
            get { return _withTuningPerfectly; }
            set
            {
                _withTuningPerfectly = value;
                OnPropertyChanged("WithTuningPerfectly");
            }
        }

        private bool _canOffDiagonalEqualsOnePerfectly = true;
        /// <summary>
        /// Off-diagonal elements can be set to 1?
        /// </summary>
        public bool CanOffDiagonalEqualsOnePerfectly
        {
            get { return _canOffDiagonalEqualsOnePerfectly; }
            set
            {
                _canOffDiagonalEqualsOnePerfectly = value;
                OnPropertyChanged("CanOffDiagonalEqualsOnePerfectly");
            }
        }

        private string _perfectlyPcmText = "";
        /// <summary>
        /// Text of Perfectly PCM
        /// </summary>
        public string PerfectlyPcmText 
        {
            get { return _perfectlyPcmText; }
            set
            {
                _perfectlyPcmText = value;
                OnPropertyChanged("PerfectlyPcmText");
            }
        }

        private string _perfectlyElapsedText = "";
        /// <summary>
        /// Elapsed generate perfectly PCM
        /// </summary>
        public string PerfectlyElapsedText
        {
            get { return _perfectlyElapsedText; }
            set
            {
                _perfectlyElapsedText = value;
                OnPropertyChanged("PerfectlyElapsedText");
            }
        }

        /// <summary>
        /// 生成一个完全一致的判断矩阵。
        /// </summary>
        /// <returns>用于显示的、完全一致的PCM数据文本</returns>
        public string GenPerfectlyPcm()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var matrix = PcmGenerator.PerfectlyConsistent(_dimensePerfectly, 
                _withTuningPerfectly, _canOffDiagonalEqualsOnePerfectly);
            sw.Stop();

            Pcm pcm = new Pcm(matrix);
            var cr = pcm.CalWeightinessPower(Consts.DoubleDelta);


            StringBuilder sb = new StringBuilder();
            sb.Append(Matrix2String(matrix));
            sb.Append("CR: " + cr.ToString("F4") + "\nWeights: ");
            sb.Append(Weights2String(pcm.Weights));

            Ufo1ElapsedText = "Elapsed time(Milliseconds): " + sw.Elapsed.TotalMilliseconds.ToString("F4");

            return sb.ToString();
        }
    }
}
