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
        private BackgroundWorker _workerMultiEaUfo = null;
        public BackgroundWorker WorkerMultiEaUfo
        {
            get { return _workerMultiEaUfo; }
        }


        private int _dimenseEaUfo = 5;
        /// <summary>
        /// Dimense of EA and UFO matrix
        /// </summary>
        public int DimenseEaUfo
        {
            get { return _dimenseEaUfo; }
            set
            {
                _dimenseEaUfo = value;
                OnPropertyChanged("DimenseEaUfo");
            }
        }

        private double _minCrEaUfo = 0.1;
        /// <summary>
        ///  The minimum value of the CR of matrix generated.
        /// </summary>
        public double MinCrEaUfo
        {
            get { return _minCrEaUfo; }
            set
            {
                _minCrEaUfo = value;
                OnPropertyChanged("MinCrEaUfo");
            }
        }

        private double _adjustBiasEaUfo = 0.5;
        /// <summary>
        ///  The maximum range of item value change.
        /// </summary>
        public double AdjustBiasEaUfo
        {
            get { return _adjustBiasEaUfo; }
            set
            {
                _adjustBiasEaUfo = value;
                OnPropertyChanged("AdjustBiasEaUfo");
            }
        }


        private double _noUfoMaxCr = 0.05;
        /// <summary>
        ///  The maximum CR of the NoUFO change matrix.
        /// </summary>
        public double NoUfoMaxCr
        {
            get { return _noUfoMaxCr; }
            set
            {
                _noUfoMaxCr = value;
                OnPropertyChanged("NoUfoMaxCr");
            }
        }

        private bool _canOffDiagonalEqualsOneEaUfo = false;
        /// <summary>
        /// Off-diagonal items can be set to 1?
        /// </summary>
        public bool CanOffDiagonalEqualsOneEaUfo
        {
            get { return _canOffDiagonalEqualsOneEaUfo; }
            set
            {
                _canOffDiagonalEqualsOneEaUfo = value;
                OnPropertyChanged("CanOffDiagonalEqualsOneEaUfo");
            }
        }

        private int _eaUfoCountUfo = 1;
        /// <summary>
        /// EA and UFO item count of matrix generated.
        /// </summary>
        public int EaUfoCountUfo
        {
            get { return _eaUfoCountUfo; }
            set
            {
                _eaUfoCountUfo = value;
                OnPropertyChanged("EaUfoCountUfo");
            }
        }

        private int _minAdjustLevelEaUfo = 3;
        /// <summary>
        /// The minimum levels of EA and UFO item adjusted.
        /// </summary>
        public int MinAdjustLevelEaUfo
        {
            get { return _minAdjustLevelEaUfo; }
            set
            {
                _minAdjustLevelEaUfo = value;
                OnPropertyChanged("MinAdjustLevelEaUfo");
            }
        }

        private int _maxAdjustLevelEaUfo = 15;
        /// <summary>
        /// The minimum levels of EA and UFO item adjusted.
        /// </summary>
        public int MaxAdjustLevelEaUfo
        {
            get { return _maxAdjustLevelEaUfo; }
            set
            {
                _maxAdjustLevelEaUfo = value;
                OnPropertyChanged("MaxAdjustLevelEaUfo");
            }
        }

        private int _numberOfGeneratedPcmsEaUfo = 100;
        /// <summary>
        /// Number of generated PCMs.
        /// </summary>
        public int NumberOfGeneratedPcmsEaUfo
        {
            get { return _numberOfGeneratedPcmsEaUfo; }
            set
            {
                _numberOfGeneratedPcmsEaUfo = value;
                OnPropertyChanged("NumberOfGeneratedPcmsEaUfo");
            }
        }

        private string _numberOfGeneratedPcmsEaUfoStr = "100";
        /// <summary>
        /// Number of generated PCMs.
        /// </summary>
        public string NumberOfGeneratedPcmsEaUfoStr
        {
            get { return _numberOfGeneratedPcmsEaUfoStr; }
            set
            {
                _numberOfGeneratedPcmsEaUfoStr = value;
                OnPropertyChanged("NumberOfGeneratedPcmsEaUfoStr");
            }
        }

        private string _eaUfoPcmText = "";
        /// <summary>
        /// Text of a single EA and UFO PCM
        /// </summary>
        public string EaUfoPcmText
        {
            get { return _eaUfoPcmText; }
            set
            {
                _eaUfoPcmText = value;
                OnPropertyChanged("EaUfoPcmText");
            }
        }

        private string _eaUfo1ElapsedText = "";
        /// <summary>
        /// Elapsed generate a single EA and UFO PCM
        /// </summary>
        public string EaUfo1ElapsedText
        {
            get { return _eaUfo1ElapsedText; }
            set
            {
                _eaUfo1ElapsedText = value;
                OnPropertyChanged("EaUfo1ElapsedText");
            }
        }

        private string _eaUfoMultiElapsedText = "";
        /// <summary>
        /// Elapsed generate multiple UFO and UFO PCMs
        /// </summary>
        public string EaUfoMultiElapsedText
        {
            get { return _eaUfoMultiElapsedText; }
            set
            {
                _eaUfoMultiElapsedText = value;
                OnPropertyChanged("EaUfoMultiElapsedText");
            }
        }

        private string _eaUfoMultiSaveFilePathname = "";
        /// <summary>
        /// Full filename for saving generated data.
        /// </summary>
        public string EaUfoMultiSaveFilePathname
        {
            get { return _eaUfoMultiSaveFilePathname; }
            set
            {
                _eaUfoMultiSaveFilePathname = value;
                OnPropertyChanged("EaUfoMultiSaveFilePathname");
            }
        }

        private int _progressValueEaUfo = 0;
        /// <summary>
        /// Process value of multiple EA and UFO generation.
        /// </summary>
        public int ProgressValueEaUfo
        {
            get { return _progressValueEaUfo; }
            set
            {
                _progressValueEaUfo = value;
                OnPropertyChanged("ProgressValueEaUfo");
            }
        }

        private string _processTextValueEaUfo = "";
        /// <summary>
        /// Process text.
        /// </summary>
        public string ProcessTextValueEaUfo
        {
            get { return _processTextValueEaUfo; }
            set
            {
                _processTextValueEaUfo = value;
                OnPropertyChanged("ProcessTextValueEaUfo");
            }
        }

        private string _processTextValueDataLoad = "";
        /// <summary>
        /// Process text.
        /// </summary>
        public string ProcessTextValueDataLoad
        {
            get { return _processTextValueDataLoad; }
            set
            {
                _processTextValueDataLoad = value;
                OnPropertyChanged("ProcessTextValueDataLoad");
            }
        }

        private string SaveEaUfoPcm(string filePathname,
            int dimense, int ufoCount, int minChangeLvl, int maxChangeLvl,
            bool canOffDiagonalEqualsOneEa, double minCr, double maxBias,
            double noUfoMaxCrRoundedEaUfo, List<Data4EaUfoPcm> data)
        {
            if (data == null)
                return "Data is NULL!";

            string rootElementName = "EAUFOPCM";
            string fileHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                    "<" + rootElementName + ">" +
                                    "  <Dimense>" + dimense + "</Dimense>" +
                                    "  <CanOffDiagonalEqualsOne>" + canOffDiagonalEqualsOneEa + "</CanOffDiagonalEqualsOne>" +
                                    "  <MaxBias>" + maxBias + "</MaxBias>" +
                                    "  <NoUFOMatrixMaxCR>" + noUfoMaxCrRoundedEaUfo + "</NoUFOMatrixMaxCR>" +
                                    "  <UFOCount>" + ufoCount + "</UFOCount>" +
                                    "  <MinCR>" + minCr + "</MinCR>" +
                                    "  <MinChangeLvl>" + minChangeLvl + "</MinChangeLvl>" +
                                    "  <MaxChangeLvl>" + maxChangeLvl + "</MaxChangeLvl>" +
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

                    XmlAttribute attribute;

                    // change infomations
                    XmlElement changesNode = xmlDoc.CreateElement("Changes");
                    matrixNode.AppendChild(changesNode);
                    foreach (var citem in item.ChangedInfo)
                    {
                        XmlElement changeNode = xmlDoc.CreateElement("Change");
                        changesNode.AppendChild(changeNode);

                        // append attribute: row/column/change/CR
                        attribute = xmlDoc.CreateAttribute("Row");
                        attribute.Value = citem.Row.ToString();
                        changeNode.Attributes.Append(attribute);

                        attribute = xmlDoc.CreateAttribute("Column");
                        attribute.Value = citem.Column.ToString();
                        changeNode.Attributes.Append(attribute);

                        attribute = xmlDoc.CreateAttribute("ChangeValue");
                        attribute.Value = citem.Change.ToString("F8");
                        changeNode.Attributes.Append(attribute);

                        attribute = xmlDoc.CreateAttribute("CR");
                        attribute.Value = citem.Cr.ToString("F8");
                        changeNode.Attributes.Append(attribute);
                    }

                    // append attribute: CR
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
        public string GenOneEaUfoPcm()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<PosAndCr> pcs = new List<PosAndCr>();

            var minCr = (double)Math.Round((decimal)_minCrEaUfo, 2, MidpointRounding.AwayFromZero);
            var bias = (double)Math.Round((decimal)_adjustBiasEaUfo, 2, MidpointRounding.AwayFromZero);
            var noUfoMaxCr = (double)Math.Round((decimal)_noUfoMaxCr, 2, MidpointRounding.AwayFromZero);

            var matrix = PcmGenerator.UnConsistentUfoWithBias(_dimenseEaUfo, true,
                minCr, noUfoMaxCr, bias, _canOffDiagonalEqualsOneEaUfo,
                _eaUfoCountUfo, _minAdjustLevelEaUfo, _maxAdjustLevelEaUfo,
                out var perfectlyMatrix, out double cr, ref pcs);

            sw.Stop();

            if (matrix == null) // 生成失败，多次尝试后，不能使原来完全一致的PCM变成不一致判断矩阵
            {
                MessageBox.Show("After 10000 times attempts, the originally consistent PCM cannot be transformed into an inconsistent judgment matrix.\n\nPlease set a higher Max Bias or a lower Min CR!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

            Pcm pcmPerfectly = new Pcm(perfectlyMatrix);
            var crPerfectly = pcmPerfectly.CalWeightinessPower(Consts.DoubleDelta);

            Pcm pcmEaUfo = new Pcm(matrix);
            var crEaUfo = pcmEaUfo.CalWeightinessPower(Consts.DoubleDelta);


            StringBuilder sb = new StringBuilder();
            sb.Append("-------------------------Perfectly PCM-------------------------\n");
            sb.Append(Matrix2String(perfectlyMatrix));
            sb.Append("CR: " + crPerfectly.ToString("F4") + "\nWeights: ");
            sb.Append(Weights2String(pcmPerfectly.Weights));

            sb.Append("\n-------------------------EA & UFO PCM-------------------------\n");
            sb.Append(Matrix2String(matrix));
            sb.Append("CR0: " + cr.ToString("F4"));
            sb.Append("; CR1: " + crEaUfo.ToString("F4") + "\nWeights: ");
            sb.Append(Weights2String(pcmEaUfo.Weights));
            foreach (var pc in pcs)
                sb.Append("Row: " + pc.Row + "; Column: " + pc.Column + "; CR: " + pc.Cr.ToString("F4") + "\n");

            EaUfo1ElapsedText = "Elapsed time(Milliseconds): " + sw.Elapsed.TotalMilliseconds.ToString("F4");
            return sb.ToString();
        }

        public void StartGenMultiEaUfo()
        {
            _workerMultiEaUfo = new BackgroundWorker();
            _workerMultiEaUfo.WorkerSupportsCancellation = true;
            _workerMultiEaUfo.DoWork += WorkerMultiEaUfoDoWork;
            _workerMultiEaUfo.RunWorkerCompleted += WorkerMultiEaUfoRunWorkerCompleted;

            _workerMultiEaUfo.RunWorkerAsync();
        }

        private bool _eaUfoPcmCompletedOk = true;
        private string _eaUfoPcmErrorMsg = "";
        private List<Data4EaUfoPcm> _eaUfoPcms;
        private int _eaUfoTotal = 0;
        private int _eaUfoCurrent = 0;
        private bool _eaUfoStopFlag = false;

        private double _minCrRoundedEaUfo;
        private double _biasRoundedEaUfo;
        private double _noUfoMaxCrRoundedEaUfo;

        private void WorkerMultiEaUfoDoWork(object sender, DoWorkEventArgs e)
        {
            _eaUfoPcmCompletedOk = true;
            _eaUfoStopFlag = false;

            _eaUfoTotal = int.Parse(NumberOfGeneratedPcmsEaUfoStr);
            _eaUfoPcms = new List<Data4EaUfoPcm>(_eaUfoTotal);

            int errorTimes = 0;

            _eaUfoCurrent = 0;

            int progress = 0;
            while (_eaUfoCurrent < _eaUfoTotal)
            {
                if (_eaUfoStopFlag)
                {
                    _eaUfoPcmCompletedOk = false;
                    _eaUfoPcmErrorMsg = "Stopped!";

                    return;
                }

                int p = (int)((double)_eaUfoCurrent / (double)_eaUfoTotal * 100);
                if (p > progress)
                {
                    progress = p;
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int>(UpdateEaUfoProcessingInfo),
                            new object[] { progress });

                }

                List<PosAndCr> pcs = new List<PosAndCr>();

                _minCrRoundedEaUfo = (double)Math.Round((decimal)_minCrEaUfo, 2, MidpointRounding.AwayFromZero);
                _biasRoundedEaUfo = (double)Math.Round((decimal)_adjustBiasEaUfo, 2, MidpointRounding.AwayFromZero);
                _noUfoMaxCrRoundedEaUfo = (double)Math.Round((decimal)_noUfoMaxCr, 2, MidpointRounding.AwayFromZero);

                var matrix = PcmGenerator.UnConsistentUfoWithBias(_dimenseEaUfo, true,
                    _minCrRoundedEaUfo, _noUfoMaxCrRoundedEaUfo, _biasRoundedEaUfo, _canOffDiagonalEqualsOneEaUfo,
                    _eaUfoCountUfo, _minAdjustLevelEaUfo, _maxAdjustLevelEaUfo,
                    out var perfectlyMatrix, out double cr, ref pcs);


                if (matrix == null) // 生成失败，添加一定UFO后，不能使原来完全一致的PCM变成不一致判断矩阵
                {
                    errorTimes++;
                    if (errorTimes > 100000) // 超过了100000个生成错误
                    {
                        _eaUfoPcmCompletedOk = false;
                        _eaUfoPcmErrorMsg =
                            "After adding a certain number of UFO items, the originally consistent\nPCM cannot be transformed into an inconsistent judgment matrix.\nPlease set more UFO count and a wider range of change level!";
                        return;
                    }
                }
                else
                {

                    Data4EaUfoPcm data4EaUfoPcm = new Data4EaUfoPcm(matrix, cr, pcs);
                    _eaUfoPcms.Add(data4EaUfoPcm);

                    _eaUfoCurrent++;
                }
            }

        }

        private void WorkerMultiEaUfoRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_eaUfoPcmCompletedOk) // 成功生成了所有判断矩阵
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<int>(UpdateEaUfoProcessingInfo),
                        new object[] { 100 });


                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateEaUfoMessage),
                        new object[] { "Generate completed!\nSaving data ......" });

                #region Save to XML

                string errorString = SaveEaUfoPcm(_eaUfoMultiSaveFilePathname, _dimenseEaUfo, _eaUfoCountUfo,
                    _minAdjustLevelEaUfo, _maxAdjustLevelEaUfo,
                    _canOffDiagonalEqualsOneEaUfo, _minCrRoundedEaUfo, _biasRoundedEaUfo, _noUfoMaxCrRoundedEaUfo,
                    _eaUfoPcms);
                if (errorString != null)
                {
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<string>(UpdateEaUfoMessage),
                            new object[] { errorString });
                }
                else
                {
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<string>(UpdateEaUfoMessage),
                            new object[] { "Generate completed!\nSaved to " + _ufoMultiSaveFilePathname });
                }

                #endregion Save to XML
            }
            else
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateEaUfoMessage),
                        new object[] { _eaUfoPcmErrorMsg });
            }

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(UpdateUfoCalculatStatus),
                    new object[] { false });
        }

        public void UpdateEaUfoProcessingInfo(int progress)
        {
            ProgressValueEaUfo = progress;
            ProcessTextValueEaUfo = _eaUfoCurrent + "/" + _eaUfoTotal;
        }

        public void UpdateEaUfoMessage(string msg)
        {
            EaUfoMultiElapsedText = msg;
        }

        public void UpdateEaUfoCalculatStatus(bool calculating)
        {
            Calculating = calculating;
        }

        public void SetEaUfoStopFlag(bool stopFlag)
        {
            _eaUfoStopFlag = stopFlag;
        }
    }
}
