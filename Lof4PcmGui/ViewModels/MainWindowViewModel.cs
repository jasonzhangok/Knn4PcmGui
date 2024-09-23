using System.ComponentModel;
using System.Text;
using System.Windows;
using ScottPlot.WPF;

namespace Lof4PcmGui.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        public Window AppWindowMain { get; set; }

        private bool _calculating = false;
        /// <summary>
        /// Calulating......
        /// </summary>
        public bool Calculating
        {
            get { return _calculating; }
            set
            {
                _calculating = value;
                OnPropertyChanged("Calculating");
            }
        }

        public MainWindowViewModel()
        {
        }

        private string Matrix2String(double[][] matrix, int precise = 4)
        {
            string formatStr = "F" + precise;

            int len = matrix.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    sb.Append(matrix[i][j].ToString(formatStr));
                    sb.Append(", ");
                }
                sb.Append("\n");
            }

            sb.Append("----------------------------------------------------------------------\n");

            return sb.ToString();
        }

        private string Weights2String(double[] weights, int precise = 4)
        {
            string formatStr = "F" + precise;

            int len = weights.Length;
            StringBuilder sb = new StringBuilder();
            int i = 0;
            for (; i < len - 1; i++)
            {
                sb.Append(weights[i].ToString(formatStr));
                sb.Append(", ");
            }
            sb.Append(weights[i].ToString(formatStr));
            sb.Append("\n");

            return sb.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
