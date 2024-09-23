using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Xml;
using Lof4PcmGui.Lof4Pcm;

namespace Lof4PcmGui.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private BackgroundWorker _workerMultiEa = null;
        public BackgroundWorker WorkerMultiEa
        {
            get { return _workerMultiEa; }
        }


        private int _dimenseEa = 5;
        /// <summary>
        /// Dimense of EA matrix
        /// </summary>
        public int DimenseEa
        {
            get { return _dimenseEa; }
            set
            {
                _dimenseEa = value;
                OnPropertyChanged("DimenseEa");
            }
        }

        private double _minCrEa = 0.1;
        /// <summary>
        ///  The minimum value of the CR of matrix generated.
        /// </summary>
        public double MinCrEa
        {
            get { return _minCrEa; }
            set
            {
                _minCrEa = value;
                OnPropertyChanged("MinCrEa");
            }
        }

        private double _adjustBias = 2.0;
        /// <summary>
        ///  The maximum range of item value change.
        /// </summary>
        public double AdjustBias
        {
            get { return _adjustBias; }
            set
            {
                _adjustBias = value;
                OnPropertyChanged("AdjustBias");
            }
        }

        private bool _canOffDiagonalEqualsOneEa = false;
        /// <summary>
        /// Off-diagonal items can be set to 1?
        /// </summary>
        public bool CanOffDiagonalEqualsOneEa
        {
            get { return _canOffDiagonalEqualsOneEa; }
            set
            {
                _canOffDiagonalEqualsOneEa = value;
                OnPropertyChanged("CanOffDiagonalEqualsOneEa");
            }
        }

        private int _numberOfGeneratedPcmsEa = 100;
        /// <summary>
        /// Number of generated PCMs.
        /// </summary>
        public int NumberOfGeneratedPcmsEa
        {
            get { return _numberOfGeneratedPcmsEa; }
            set
            {
                _numberOfGeneratedPcmsEa = value;
                OnPropertyChanged("NumberOfGeneratedPcmsEa");
            }
        }

        private string _numberOfGeneratedPcmsEaStr = "100";
        /// <summary>
        /// Number of generated PCMs.
        /// </summary>
        public string NumberOfGeneratedPcmsEaStr
        {
            get { return _numberOfGeneratedPcmsEaStr; }
            set
            {
                _numberOfGeneratedPcmsEaStr = value;
                OnPropertyChanged("NumberOfGeneratedPcmsEaStr");
            }
        }

        private string _eaPcmText = "";
        /// <summary>
        /// Text of a single EA PCM
        /// </summary>
        public string EaPcmText
        {
            get { return _eaPcmText; }
            set
            {
                _eaPcmText = value;
                OnPropertyChanged("EaPcmText");
            }
        }

        private string _ea1ElapsedText = "";
        /// <summary>
        /// Elapsed generate a single EA PCM
        /// </summary>
        public string Ea1ElapsedText
        {
            get { return _ea1ElapsedText; }
            set
            {
                _ea1ElapsedText = value;
                OnPropertyChanged("Ea1ElapsedText");
            }
        }

        private string _eaMultiElapsedText = "";
        /// <summary>
        /// Elapsed generate multiple UFO PCMs
        /// </summary>
        public string EaMultiElapsedText
        {
            get { return _eaMultiElapsedText; }
            set
            {
                _eaMultiElapsedText = value;
                OnPropertyChanged("EaMultiElapsedText");
            }
        }

        private string _eaMultiSaveFilePathname = "";
        /// <summary>
        /// Full filename for saving generated data.
        /// </summary>
        public string EaMultiSaveFilePathname
        {
            get { return _eaMultiSaveFilePathname; }
            set
            {
                _eaMultiSaveFilePathname = value;
                OnPropertyChanged("EaMultiSaveFilePathname");
            }
        }

        private int _progressValueEa = 0;
        /// <summary>
        /// Process value of multiple UFO generation.
        /// </summary>
        public int ProgressValueEa
        {
            get { return _progressValueEa; }
            set
            {
                _progressValueEa = value;
                OnPropertyChanged("ProgressValueEa");
            }
        }

        private string _processTextValueEa = "";
        /// <summary>
        /// Process text.
        /// </summary>
        public string ProcessTextValueEa
        {
            get { return _processTextValueEa; }
            set
            {
                _processTextValueEa = value;
                OnPropertyChanged("ProcessTextValueEa");
            }
        }
        private string SaveEaPcm(string filePathname,
            int dimense, bool canOffDiagonalEqualsOneEa,
            double minCr, double maxBias, List<Data4EaPcm> data)
        {
            if (data == null)
                return "Data is NULL!";

            string rootElementName = "EAPCM";
            string fileHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                    "<" + rootElementName + ">" +
                                    "  <Dimense>" + dimense + "</Dimense>" +
                                    "  <CanOffDiagonalEqualsOne>" + canOffDiagonalEqualsOneEa + "</CanOffDiagonalEqualsOne>" +
                                    "  <MinCR>" + minCr + "</MinCR>" +
                                    "  <MaxBias>" + maxBias + "</MaxBias>" +
                                    "</" + rootElementName + ">";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fileHeader);

            try
            {
                XmlNode rootNode = xmlDoc[rootElementName];

                XmlElement child = xmlDoc.CreateElement("Amount"); // create a node
                child.InnerText = data.Count.ToString();
                rootNode.AppendChild(child);

                XmlElement matricesNode = xmlDoc.CreateElement("Matrices");
                rootNode.AppendChild(matricesNode);

                StringBuilder sb = new StringBuilder();
                List<string> rowsTxt = new List<string>();

                foreach (var item in data)
                {
                    XmlElement matrixNode = xmlDoc.CreateElement("Matrix");
                    matricesNode.AppendChild(matrixNode);

                    // append attribute: Average Bias/CR
                    XmlAttribute attribute = xmlDoc.CreateAttribute("AverageBias");
                    attribute.Value = item.AvgBias.ToString("F8");
                    matrixNode.Attributes.Append(attribute);

                    attribute = xmlDoc.CreateAttribute("CR");
                    attribute.Value = item.Cr.ToString("F8");
                    matrixNode.Attributes.Append(attribute);

                    // append matrix data
                    rowsTxt.Clear();
                    int len = item.Matrix.Length;
                    for (int i = 0; i < len; i++)
                    {
                        sb.Clear();
                        for (int j = 0; j < len; j++)
                        {
                            sb.Append(item.Matrix[i][j].ToString("F8"));
                            sb.Append(",");
                        }

                        sb.Remove(sb.Length - 1, 1);    // delete the last comma
                        rowsTxt.Add(sb.ToString());
                    }

                    XmlElement dataNode = xmlDoc.CreateElement("Data");
                    matrixNode.AppendChild(dataNode);
                    foreach (var rowTex in rowsTxt)
                    {
                        XmlElement rowNode = xmlDoc.CreateElement("DataRow");
                        dataNode.AppendChild(rowNode);

                        attribute = xmlDoc.CreateAttribute("Value");
                        attribute.Value = rowTex;
                        rowNode.Attributes.Append(attribute);
                    }
                }
            }
            catch (Exception e)
            {
                return "Format XML string failed! Detial:\n" + e;
            }

            try
            {
                xmlDoc.Save(filePathname);
            }
            catch (Exception e)
            {
                return "Save data to file failed! Detial:\n" + e;
            }


            return null;    // no error!
        }

        /// <summary>
        /// 生成一个由于误差累计不一致的判断矩阵。
        /// </summary>
        /// <returns>用于显示的、由于误差累计不一致的PCM数据文本</returns>
        public string GenOneEaPcm()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            double adjustAvg = 0.0;
            var minCr = (double)Math.Round((decimal)_minCrEa, 2, MidpointRounding.AwayFromZero);
            var bias = (double)Math.Round((decimal)_adjustBias, 2, MidpointRounding.AwayFromZero);
            var matrix = PcmGenerator.UnconsistentNoUfo(_dimenseEa, minCr, bias, _canOffDiagonalEqualsOneEa,
                out var perfectlyMatrix, out int timesTry, out double cr, ref adjustAvg);

            sw.Stop();

            if (matrix == null) // 生成失败，多次尝试后，不能使原来完全一致的PCM变成不一致判断矩阵
            {
                MessageBox.Show("After 10000 times attempts, the originally consistent PCM cannot be transformed into an inconsistent judgment matrix.\n\nPlease set a higher Max Bias or a lower Min CR!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

            Pcm pcmPerfectly = new Pcm(perfectlyMatrix);
            var crPerfectly = pcmPerfectly.CalWeightinessPower(Consts.DoubleDelta);

            Pcm pcmEa = new Pcm(matrix);
            var crEa = pcmEa.CalWeightinessPower(Consts.DoubleDelta);


            StringBuilder sb = new StringBuilder();
            sb.Append("-------------------------Perfectly PCM-------------------------\n");
            sb.Append(Matrix2String(perfectlyMatrix));
            sb.Append("CR: " + crPerfectly.ToString("F4") + "\nWeights: ");
            sb.Append(Weights2String(pcmPerfectly.Weights));

            sb.Append("\n-------------------------EA PCM-------------------------\n");
            sb.Append(Matrix2String(matrix));
            sb.Append("CR0: " + cr.ToString("F4"));
            sb.Append("; CR1: " + crEa.ToString("F4") + "\nWeights: ");
            sb.Append(Weights2String(pcmEa.Weights));
            sb.Append("Attempts: " + timesTry + "; Average Bias: " + adjustAvg.ToString("F8"));

            Ea1ElapsedText = "Elapsed time(Milliseconds): " + sw.Elapsed.TotalMilliseconds.ToString("F4");
            return sb.ToString();
        }

        public void StartGenMultiEa()
        {
            _workerMultiUfo = new BackgroundWorker();
            _workerMultiUfo.WorkerSupportsCancellation = true;
            _workerMultiUfo.DoWork += WorkerMultiEaDoWork;
            _workerMultiUfo.RunWorkerCompleted += WorkerMultiEaRunWorkerCompleted;

            _workerMultiUfo.RunWorkerAsync();
        }

        private bool _eaPcmCompletedOk = true;
        private string _eaPcmErrorMsg = "";
        private List<Data4EaPcm> _eaPcms;
        private int _eaTotal = 0;
        private int _eaCurrent = 0;
        private bool _eaStopFlag = false;

        private double _minCrRounded;
        private double _biasRounded;

        private void WorkerMultiEaDoWork(object sender, DoWorkEventArgs e)
        {
            _eaPcmCompletedOk = true;
            _eaStopFlag = false;

            _eaTotal = int.Parse(NumberOfGeneratedPcmsEaStr);
            _eaPcms = new List<Data4EaPcm>(_eaTotal);

            int errorTimes = 0;

            _eaCurrent = 0;

            int progress = 0;
            while (_eaCurrent < _eaTotal)
            {
                if (_eaStopFlag)
                {
                    _eaPcmCompletedOk = false;
                    _eaPcmErrorMsg = "Stopped!";

                    return;
                }

                int p = (int)((double)_eaCurrent / (double)_eaTotal * 100);
                if (p > progress)
                {
                    progress = p;
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int>(UpdateEaProcessingInfo),
                            new object[] { progress });

                }

                double adjustAvg = 0.0;
                _minCrRounded = (double)Math.Round((decimal)_minCrEa, 2, MidpointRounding.AwayFromZero);
                _biasRounded = (double)Math.Round((decimal)_adjustBias, 2, MidpointRounding.AwayFromZero);
                var matrix = PcmGenerator.UnconsistentNoUfo(_dimenseEa, _minCrRounded, _biasRounded, _canOffDiagonalEqualsOneEa,
                    out var perfectlyMatrix, out int timesTry, out double cr, ref adjustAvg);

                if (matrix == null) // 生成失败，添加一定UFO后，不能使原来完全一致的PCM变成不一致判断矩阵
                {
                    errorTimes++;
                    if (errorTimes > 1000) // 超过了1000个生成错误
                    {
                        _ufoPcmCompletedOk = false;
                        _ufoPcmErrorMsg =
                            "After adding a certain number of UFO items, the originally consistent\nPCM cannot be transformed into an inconsistent judgment matrix.\nPlease set more UFO count and a wider range of change level!";
                        return;
                    }
                }

                Data4EaPcm data4EaPcm = new Data4EaPcm(matrix, cr, adjustAvg);
                _eaPcms.Add(data4EaPcm);

                _eaCurrent++;
            }

        }

        private void WorkerMultiEaRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_eaPcmCompletedOk) // 成功生成了所有判断矩阵
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<int>(UpdateEaProcessingInfo),
                        new object[] { 100 });


                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateEaMessage),
                        new object[] { "Generate completed!\nSaving data ......" });

                #region Save to XML

                //private string SaveEaPcm(string filePathname,
                //    int dimense, bool canOffDiagonalEqualsOneEa,
                //    double minCr, double maxBias, List<Data4EaPcm> data)

                string errorString = SaveEaPcm(_eaMultiSaveFilePathname,
                     _dimenseEa, _canOffDiagonalEqualsOneEa, _minCrRounded, _biasRounded,
                    _eaPcms);
                if (errorString != null)
                {
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<string>(UpdateEaMessage),
                            new object[] { errorString });
                }
                else
                {
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<string>(UpdateEaMessage),
                            new object[] { "Generate completed!\nSaved to " + _ufoMultiSaveFilePathname });
                }

                #endregion Save to XML
            }
            else
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateEaMessage),
                        new object[] { _eaPcmErrorMsg });
            }

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(UpdateUfoCalculatStatus),
                    new object[] { false });
        }

        public void UpdateEaProcessingInfo(int progress)
        {
            ProgressValueEa = progress;
            ProcessTextValueEa = _eaCurrent + "/" + _eaTotal;
        }

        public void UpdateEaMessage(string msg)
        {
            EaMultiElapsedText = msg;
        }

        public void UpdateEaCalculatStatus(bool calculating)
        {
            Calculating = calculating;
        }

        public void SetEaStopFlag(bool stopFlag)
        {
            _eaStopFlag = stopFlag;
        }
    }
}
