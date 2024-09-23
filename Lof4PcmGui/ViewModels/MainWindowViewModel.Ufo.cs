using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using Lof4PcmGui.Lof4Pcm;

namespace Lof4PcmGui.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private BackgroundWorker _workerMultiUfo = null;
        public BackgroundWorker WorkerMultiUfo
        {
            get { return _workerMultiUfo; }
        }


        private int _dimenseUfo = 5;
        /// <summary>
        /// Dimense of UFO matrix
        /// </summary>
        public int DimenseUfo
        {
            get { return _dimenseUfo; }
            set
            {
                _dimenseUfo = value;
                OnPropertyChanged("DimenseUfo");
            }
        }

        private int _ufoCountUfo = 1;
        /// <summary>
        /// UFO item count of matrix generated.
        /// </summary>
        public int UfoCountUfo
        {
            get { return _ufoCountUfo; }
            set
            {
                _ufoCountUfo = value;
                OnPropertyChanged("UfoCountUfo");
            }
        }

        private int _minAdjustLevelUfo = 3;
        /// <summary>
        /// The minimum levels of UFO item adjusted.
        /// </summary>
        public int MinAdjustLevelUfo
        {
            get { return _minAdjustLevelUfo; }
            set
            {
                _minAdjustLevelUfo = value;
                OnPropertyChanged("MinAdjustLevelUfo");
            }
        }

        private int _maxAdjustLevelUfo = 10;
        /// <summary>
        /// The minimum levels of UFO item adjusted.
        /// </summary>
        public int MaxAdjustLevelUfo
        {
            get { return _maxAdjustLevelUfo; }
            set
            {
                _maxAdjustLevelUfo = value;
                OnPropertyChanged("MaxAdjustLevelUfo");
            }
        }

        private int _numberOfGeneratedPcmsUfo = 100;
        /// <summary>
        /// Number of generated PCMs.
        /// </summary>
        public int NumberOfGeneratedPcmsUfo
        {
            get { return _numberOfGeneratedPcmsUfo; }
            set
            {
                _numberOfGeneratedPcmsUfo = value;
                OnPropertyChanged("NumberOfGeneratedPcmsUfo");
            }
        }

        private string _numberOfGeneratedPcmsUfoStr = "100";
        /// <summary>
        /// Number of generated PCMs.
        /// </summary>
        public string NumberOfGeneratedPcmsUfoStr
        {
            get { return _numberOfGeneratedPcmsUfoStr; }
            set
            {
                _numberOfGeneratedPcmsUfoStr = value;
                OnPropertyChanged("NumberOfGeneratedPcmsUfoStr");
            }
        }

        private string _ufoPcmText = "";
        /// <summary>
        /// Text of a single UFO PCM
        /// </summary>
        public string UfoPcmText
        {
            get { return _ufoPcmText; }
            set
            {
                _ufoPcmText = value;
                OnPropertyChanged("UfoPcmText");
            }
        }

        private string _ufo1ElapsedText = "";
        /// <summary>
        /// Elapsed generate a single UFO PCM
        /// </summary>
        public string Ufo1ElapsedText
        {
            get { return _ufo1ElapsedText; }
            set
            {
                _ufo1ElapsedText = value;
                OnPropertyChanged("Ufo1ElapsedText");
            }
        }

        private string _ufoMultiElapsedText = "";
        /// <summary>
        /// Elapsed generate multiple UFO PCMs
        /// </summary>
        public string UfoMultiElapsedText
        {
            get { return _ufoMultiElapsedText; }
            set
            {
                _ufoMultiElapsedText = value;
                OnPropertyChanged("UfoMultiElapsedText");
            }
        }

        private string _ufoMultiSaveFilePathname = "";
        /// <summary>
        /// Full filename for saving generated data.
        /// </summary>
        public string UfoMultiSaveFilePathname
        {
            get { return _ufoMultiSaveFilePathname; }
            set
            {
                _ufoMultiSaveFilePathname = value;
                OnPropertyChanged("UfoMultiSaveFilePathname");
            }
        }

        private int _progressValueUfo = 0;
        /// <summary>
        /// Process value of multiple UFO generation.
        /// </summary>
        public int ProgressValueUfo
        {
            get { return _progressValueUfo; }
            set
            {
                _progressValueUfo = value;
                OnPropertyChanged("ProgressValueUfo");
            }
        }

        private string _processTextValueUfo = "";
        /// <summary>
        /// Process text.
        /// </summary>
        public string ProcessTextValueUfo
        {
            get { return _processTextValueUfo; }
            set
            {
                _processTextValueUfo = value;
                OnPropertyChanged("ProcessTextValueUfo");
            }
        }
        private string SaveUfoPcm(string filePathname,
            int dimense, int ufoCount, int minChangeLvl, int maxChangeLvl,
            List<Data4UfoPcm> data)
        {
            if (data == null)
                return "Data is NULL!";

            string rootElementName = "UFOPCM";
            string fileHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                    "<" + rootElementName + ">" +
                                    "  <Dimense>" + dimense + "</Dimense>" +
                                    "  <UFOCount>" + ufoCount + "</UFOCount>" +
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

                List<string> rowsTxt = new List<string>();
                StringBuilder sb = new StringBuilder();

                foreach (var item in data)
                {
                    XmlElement matrixNode = xmlDoc.CreateElement("Matrix");
                    matricesNode.AppendChild(matrixNode);

                    // change infomations
                    XmlElement changesNode = xmlDoc.CreateElement("Changes");
                    matrixNode.AppendChild(changesNode);
                    foreach (var citem in item.ChangedInfo)
                    {
                        XmlElement changeNode = xmlDoc.CreateElement("Change");
                        changesNode.AppendChild(changeNode);

                        // append attribute: row/column/change/CR
                        XmlAttribute attribute = xmlDoc.CreateAttribute("Row");
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

                        XmlAttribute attribute = xmlDoc.CreateAttribute("Value");
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
        /// 生成一个由于UFO不一致的判断矩阵。
        /// </summary>
        /// <returns>用于显示的、由于UFO不一致的PCM数据文本</returns>
        public string GenOneUfoPcm()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<PosAndCr> pcs = new List<PosAndCr>();
            var matrix = PcmGenerator.UnconsistentUfo(_dimenseUfo,
                _ufoCountUfo, _minAdjustLevelUfo, _maxAdjustLevelUfo, out var perfectlyMatrix, ref pcs);
            sw.Stop();

            if (matrix == null) // 生成失败，添加一定UFO后，不能使原来完全一致的PCM变成不一致判断矩阵
            {
                MessageBox.Show("After adding a certain number of UFOs, the originally consistent PCM cannot be transformed into an inconsistent judgment matrix.\n\nPlease set a higher number of UFOs and a wider range of change level!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

            Pcm pcmPerfectly = new Pcm(perfectlyMatrix);
            var crPerfectly = pcmPerfectly.CalWeightinessPower(Consts.DoubleDelta);

            Pcm pcmUfo = new Pcm(matrix);
            var crUfo = pcmUfo.CalWeightinessPower(Consts.DoubleDelta);


            StringBuilder sb = new StringBuilder();
            sb.Append("-------------------------Perfectly PCM-------------------------\n");
            sb.Append(Matrix2String(perfectlyMatrix));
            sb.Append("CR: " + crPerfectly.ToString("F4") + "\nWeights: ");
            sb.Append(Weights2String(pcmPerfectly.Weights));

            sb.Append("\n-------------------------UFO PCM-------------------------\n");
            sb.Append(Matrix2String(matrix));
            sb.Append("CR: " + crUfo.ToString("F4") + "\nWeights: ");
            sb.Append(Weights2String(pcmUfo.Weights));
            foreach (var pc in pcs)
                sb.Append("Row: " + pc.Row + "; Column: " + pc.Column + "; CR: " + pc.Cr.ToString("F4") + "\n");

            Ufo1ElapsedText = "Elapsed time(Milliseconds): " + sw.Elapsed.TotalMilliseconds.ToString("F4");
            return sb.ToString();
        }

        private DispatcherTimer _ufoMsgTimer;
        public void StartGenMultiUfo()
        {
            _workerMultiUfo = new BackgroundWorker();
            _workerMultiUfo.WorkerSupportsCancellation = true;
            _workerMultiUfo.DoWork += WorkerMultiUfoDoWork;
            _workerMultiUfo.RunWorkerCompleted += WorkerMultiUfoRunWorkerCompleted;

            _workerMultiUfo.RunWorkerAsync();

        }

        private bool _ufoPcmCompletedOk = true;
        private string _ufoPcmErrorMsg = "";
        private List<Data4UfoPcm> _ufoPcms;
        private int _ufoTotal = 0;
        private int _ufoCurrent = 0;
        private bool _ufoStopFlag = false;

        private void WorkerMultiUfoDoWork(object sender, DoWorkEventArgs e)
        {
            _ufoPcmCompletedOk = true;
            _ufoStopFlag = false;

            _ufoTotal = int.Parse(NumberOfGeneratedPcmsUfoStr);
            _ufoPcms = new List<Data4UfoPcm>(_ufoTotal);

            int errorTimes = 0;

            _ufoCurrent = 0;

            int progress = 0;
            while (_ufoCurrent < _ufoTotal)
            {
                if (_ufoStopFlag)
                {
                    _ufoPcmCompletedOk = false;
                    _ufoPcmErrorMsg = "Stopped!";

                    return;
                }

                int p = (int)((double)_ufoCurrent / (double)_ufoTotal * 100);
                if (p > progress)
                {
                    progress = p;
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int>(UpdateUfoProcessingInfo),
                            new object[] { progress });

                }

                List<PosAndCr> pcs = new List<PosAndCr>();
                var matrix = PcmGenerator.UnconsistentUfo(_dimenseUfo,
                    _ufoCountUfo, _minAdjustLevelUfo, _maxAdjustLevelUfo, out var perfectlyMatrix, ref pcs);

                if (matrix == null) // 生成失败，添加一定UFO后，不能使原来完全一致的PCM变成不一致判断矩阵
                {
                    errorTimes++;
                    if (errorTimes > 100000000) // 超过了100000000个生成错误
                    {
                        _ufoPcmCompletedOk = false;
                        _ufoPcmErrorMsg =
                            "After adding a certain number of UFO items, the originally consistent\nPCM cannot be transformed into an inconsistent judgment matrix.\nPlease set more UFO count and a wider range of change level!";
                        return;
                    }
                }
                else
                {
                    Data4UfoPcm data4UfoPcm = new Data4UfoPcm(matrix, 0.0, pcs);
                    if (matrix != null && pcs != null)
                    {
                        _ufoPcms.Add(data4UfoPcm);
                        _ufoCurrent++;
                    }
                }
            }

        }

        private void WorkerMultiUfoRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_ufoPcmCompletedOk) // 成功生成了所有判断矩阵
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<int>(UpdateUfoProcessingInfo),
                        new object[] { 100 });


                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateUfoMessage),
                        new object[] { "Generate completed!\nSaving data ......" });

                #region Save to XML

                string errorString = SaveUfoPcm(_ufoMultiSaveFilePathname,
                     _dimenseUfo, _ufoCountUfo, _minAdjustLevelUfo, _maxAdjustLevelUfo,
                    _ufoPcms);
                if (errorString != null)
                {
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<string>(UpdateUfoMessage),
                            new object[] { errorString });
                }
                else
                {
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<string>(UpdateUfoMessage),
                            new object[] { "Generate completed!\nSaved to " + _ufoMultiSaveFilePathname });
                }

                #endregion Save to XML
            }
            else
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateUfoMessage),
                        new object[] { _ufoPcmErrorMsg });
            }

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(UpdateUfoCalculatStatus),
                    new object[] { false });
        }

        public void UpdateUfoProcessingInfo(int progress)
        {
            ProgressValueUfo = progress;
            ProcessTextValueUfo = _ufoCurrent + "/" + _ufoTotal;
        }

        public void UpdateUfoMessage(string msg)
        {
            UfoMultiElapsedText = msg;
        }

        public void UpdateUfoCalculatStatus(bool calculating)
        {
            Calculating = calculating;
        }

        public void SetUfoStopFlag(bool stopFlag)
        {
            _ufoStopFlag = stopFlag;
        }
    }
}
