using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using Lof4PcmGui.Lof4Pcm;

namespace Lof4PcmGui.ViewModels
{
    public enum OutlierAlgorithm
    {
        KNN, LOF
    }

    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _filePathname;
        public string FilePathname
        {
            set { _filePathname = value; }
        }

        private int _dimense;
        private int _ufoCount;
        private int _amount;

        private bool _isLofCalc = false;

        private bool _lofCalcCompletedOk;
        private bool _lofCalcStopFlag;
        private int _lofCalcTotal;
        private int _lofCalcCurrent;
        private string _lofCalcErrorMsg;

        private bool _loadCompletedOk;
        private bool _loadStopFlag;
        private int _loadTotal;
        private int _loadCurrent;
        private string _loadErrorMsg;

        private List<LofResult> _lofResults;

        private BackgroundWorker _workerCalcLof = null;
        public BackgroundWorker WorkerCalcLof
        {
            get { return _workerCalcLof; }
        }

        private BackgroundWorker _workerLoadData = null;
        public BackgroundWorker WorkerLoadData
        {
            get { return _workerLoadData; }
        }

        private bool _isLof = false;
        /// <summary>
        /// LOF
        /// </summary>
        public bool IsLof
        {
            get { return _isLof; }
            set
            {
                _isLof = value;
                OnPropertyChanged("IsLof");
            }
        }

        private bool _isKnn = true;
        /// <summary>
        /// KNN
        /// </summary>
        public bool IsKnn
        {
            get { return _isKnn; }
            set
            {
                _isKnn = value;
                OnPropertyChanged("IsKnn");
            }
        }

        private int _progressValueLofCalc = 0;
        /// <summary>
        /// Process value of LOF calc.
        /// </summary>
        public int ProgressValueLofCalc
        {
            get { return _progressValueLofCalc; }
            set
            {
                _progressValueLofCalc = value;
                OnPropertyChanged("ProgressValueLofCalc");
            }
        }

        private int _progressValueDataLoad = 0;
        /// <summary>
        /// Process value of data load.
        /// </summary>
        public int ProgressValueDataLoad
        {
            get { return _progressValueDataLoad; }
            set
            {
                _progressValueDataLoad = value;
                OnPropertyChanged("ProgressValueDataLoad");
            }
        }

        private string _processTextValueLofOfUfo = "";
        /// <summary>
        /// Process text.
        /// </summary>
        public string ProcessTextValueLofOfUfo
        {
            get { return _processTextValueLofOfUfo; }
            set
            {
                _processTextValueLofOfUfo = value;
                OnPropertyChanged("ProcessTextValueLofOfUfo");
            }
        }

        private bool _isReadDataFromFile = false;
        /// <summary>
        /// Is load data progress bar is indeterminate.
        /// </summary>
        public bool IsReadDataFromFile
        {
            get { return _isReadDataFromFile; }
            set
            {
                _isReadDataFromFile = value;
                OnPropertyChanged("IsReadDataFromFile");
            }
        }

        private string _filepathnameCalcLof = "";

        public string FilepathnameCalcLof
        {
            get { return _filepathnameCalcLof; }
            set
            {
                _filepathnameCalcLof = value;
                OnPropertyChanged("FilepathnameCalcLof");
            }
        }

        private string _parametersOfLoadedData = "";

        public string ParametersOfLoadedData
        {
            get { return _parametersOfLoadedData; }
            set
            {
                _parametersOfLoadedData = value;
                OnPropertyChanged("ParametersOfLoadedData");
            }
        }

        private List<Data4UfoPcm> _ufoDataLoaded4Lof;
        public List<Data4UfoPcm> UfoDataLoaded4Lof
        {
            get { return _ufoDataLoaded4Lof; }
            set
            {
                _ufoDataLoaded4Lof = value;
            }
        }

        private double _minLofOfLofUfo = 0;

        public double MinLofOfLofUfo
        {
            get { return _minLofOfLofUfo; }
            set
            {
                _minLofOfLofUfo = value;
                OnPropertyChanged("MinLofOfLofUfo");
            }
        }

        private string _lofStatisticText = "";

        public string LofStatisticText
        {
            get { return _lofStatisticText; }
            set
            {
                _lofStatisticText = value;
                OnPropertyChanged("LofStatisticText");
            }
        }

        private List<LofResult> _outlierResults = null;

        public List<LofResult> OutlierResults
        {
            get { return _outlierResults; }
            set
            {
                _outlierResults = value;
                OnPropertyChanged("OutlierResults");
            }
        }

        /// <summary>
        /// 计算UFO PCM的LOF，并统计LOF最大值。
        /// </summary>
        /// <param name="matrix">UFO PCM数据。</param>
        /// <param name="outerLierAlgorithm">使用哪种算法计算离群系数</param>
        /// <param name="uniqueItemNumber">输出参数，该矩阵上三角不重复Item的数量</param>
        /// <param name="mismatch">输出参数，最大LOF与Change项是否匹配。
        /// 如果一个UFO PCM中LOF最大的位置，与所做修改的UFO位置不同，则不匹配。</param>
        /// <returns>最大的LOF值，如果矩阵不重复数据太少，返回-1.</returns>
        private double OutlierCalc4Ufo(Data4UfoPcm matrix, OutlierAlgorithm outerLierAlgorithm, out int uniqueItemNumber, out bool mismatch)
        {
            int dimense = matrix.Matrix.Length;

            Pcm pcm = new Pcm(matrix.Matrix);
            switch (outerLierAlgorithm)
            {
                case OutlierAlgorithm.KNN:
                    pcm.Knn(Consts.DoubleDelta);
                    break;
                case OutlierAlgorithm.LOF:
                    pcm.Lof(Consts.DoubleDelta);
                    break;
                default:
                    pcm.Knn(Consts.DoubleDelta);
                    break;
            }

            if (outerLierAlgorithm == OutlierAlgorithm.LOF && pcm.UniqueItemNumber <= 2)
            {
                mismatch = false;
                uniqueItemNumber = pcm.UniqueItemNumber;
                return -1;
            }

            int rowMaxLof = 0, columnMaxLof = 1;
            double maxLof = 0;
            for (int i = 0; i < dimense - 1; i++)
            {
                for (int j = i + 1; j < dimense; j++)
                {
                    if (maxLof < pcm.Outlier[i][j])
                    {
                        maxLof = pcm.Outlier[i][j];
                        rowMaxLof = i;
                        columnMaxLof = j;
                    }
                }
            }

            // check mismatch
            bool match = false;
            if (matrix.ChangedInfo == null)
                match = true;
            else
            {
                foreach (var posAndCr in matrix.ChangedInfo)
                {
                    if (posAndCr.Row == rowMaxLof && posAndCr.Column == columnMaxLof)
                    {
                        match = true;
                        break;
                    }
                }
            }

            mismatch = !match;
            uniqueItemNumber = pcm.UniqueItemNumber;
            return maxLof;
        }

        #region Calc LOF
        public void StartCalcLof()
        {
            _workerCalcLof = new BackgroundWorker();
            _workerCalcLof.WorkerSupportsCancellation = true;
            _workerCalcLof.DoWork += WorkerCalcLofDoWork;
            _workerCalcLof.RunWorkerCompleted += WorkerCalcLofRunWorkerCalcCompleted;

            _isLofCalc = true;

            _workerCalcLof.RunWorkerAsync();
        }

        private void WorkerCalcLofDoWork(object sender, DoWorkEventArgs e)
        {
            _lofCalcCompletedOk = true;
            _lofCalcStopFlag = false;

            _lofCalcTotal = UfoDataLoaded4Lof.Count;

            _lofCalcCurrent = 0;

            int progress = 0;

            #region calc LOF

            List<LofResult> lofItems = new List<LofResult>();
            foreach (var matrix in UfoDataLoaded4Lof)
            {
                if (_lofCalcStopFlag)
                {
                    _lofCalcCompletedOk = false;
                    _lofCalcErrorMsg = "Stopped!";

                    return;
                }

                int p = (int)((double)_lofCalcCurrent / (double)_lofCalcTotal * 100);
                if (p > progress)
                {
                    progress = p;
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int>(UpdateCalcLofProcessingInfo),
                            new object[] { progress });
                }

                // calc LOF   
                double maxLof;
                int uniqueItemNumber;
                bool mismatch;
                if (_isKnn)
                    maxLof = OutlierCalc4Ufo(matrix, OutlierAlgorithm.KNN, out uniqueItemNumber, out mismatch);
                else
                    maxLof = OutlierCalc4Ufo(matrix, OutlierAlgorithm.LOF, out uniqueItemNumber, out mismatch);

                LofResult lofItem = new LofResult(matrix.Matrix.Length, uniqueItemNumber, maxLof, maxLof, mismatch ? 1 : 0, 1);
                lofItems.Add(lofItem);  // 每个矩阵都计算出lof

                _lofCalcCurrent++;
            }

            _lofResults = lofItems;

            #endregion calc LOF

        }

        private void WorkerCalcLofRunWorkerCalcCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_lofCalcCompletedOk) // 成功计算了所有判断矩阵
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<int>(UpdateCalcLofProcessingInfo),
                        new object[] { 100 });

                #region 统计结果

                // 根据lofItems中的数据，按矩阵阶数-非重复项数，统计最大LOF和无法匹配项的数量

                List<LofResult> results = new List<LofResult>();
                foreach (var lofItem in _lofResults)
                {
                    LofResult itemFounded = null;
                    // 在results中查找是否已经有矩阵阶数-非重复项数的数据
                    foreach (var item in results)
                    {
                        if (item.Dimense == lofItem.Dimense && item.UniqueItemNumber == lofItem.UniqueItemNumber)
                        {
                            itemFounded = item;
                            break;
                        }
                    }

                    if (itemFounded == null)
                    {
                        itemFounded = new LofResult(lofItem.Dimense,
                            lofItem.UniqueItemNumber, lofItem.MaxOuterlier, lofItem.MaxOuterlier, lofItem.Mismatch, 1);
                        itemFounded.Outliers.Add(lofItem.MaxOuterlier);
                        results.Add(itemFounded);
                    }
                    else
                    {
                        if (itemFounded.MinOuterlier > lofItem.MaxOuterlier)
                            itemFounded.MinOuterlier = lofItem.MaxOuterlier;
                        if (itemFounded.MaxOuterlier < lofItem.MaxOuterlier)
                            itemFounded.MaxOuterlier = lofItem.MaxOuterlier;
                        itemFounded.Mismatch += lofItem.Mismatch;
                        itemFounded.Amount += lofItem.Amount;

                        itemFounded.Outliers.Add(lofItem.MaxOuterlier);
                    }
                }

                #region 离散指标的统计值

                double avg = 0;
                double std = 0;
                foreach (var lofResult in results)
                {
                    double sum = 0;
                    foreach (var outlier in lofResult.Outliers)
                    {
                        sum += outlier;
                    }
                    avg = sum / lofResult.Outliers.Count;

                    double sumd = 0;
                    foreach (var outlier in lofResult.Outliers)
                    {
                        sumd += (outlier - avg) * (outlier - avg);
                    }

                    std = Math.Sqrt(sumd / lofResult.Outliers.Count);

                    lofResult.Avg = avg;
                    lofResult.Std = std;
                }

                #endregion 离散指标的统计值

                //StringBuilder sb = new StringBuilder();
                //foreach (var result in results)
                //{
                //    sb.Append("Dimense: " + result.Dimense
                //                          + "; UniqueItemNumber: " + result.UniqueItemNumber
                //                          + "; Amount: " + result.Amount
                //                          + "; Avg: " + result.Avg
                //                          + "; Std: " + result.Std
                //                          + "; 3*Std: " + 3.0 * result.Std
                //                          + "; MinOutlierOfMatrix: " + result.MinOuterlier.ToString("F8")
                //                          + "; MaxOutlierOfMatrix: " + result.MaxOuterlier.ToString("F8")
                //                          + "; Mismatch: " + result.Mismatch + "\n");
                //}

                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<List<LofResult>>(UpdateCalcLofMessage),
                            new object[] { results });

                #endregion 统计结果
            }
            else
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateCalcLofMessageText),
                        new object[] { _eaPcmErrorMsg });
            }

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(UpdateUfoCalculatStatus),
                    new object[] { false });
        }

        public void UpdateCalcLofProcessingInfo(int progress)
        {
            ProgressValueLofCalc = progress;
            ProcessTextValueLofOfUfo = _lofCalcCurrent + "/" + _lofCalcTotal;
        }

        public void UpdateCalcLofMessage(List<LofResult> rsts)
        {
            OutlierResults = rsts;
        }

        public void UpdateCalcLofMessageText(string msg)
        {
            LofStatisticText = msg;
        }


        public void UpdateCalcLofCalculatStatus(bool calculating)
        {
            Calculating = calculating;
        }
        #endregion Calc LOF

        #region Load data
        public void StartLoadData()
        {
            _workerLoadData = new BackgroundWorker();
            _workerLoadData.WorkerSupportsCancellation = true;
            _workerLoadData.DoWork += WorkerLoadDoWork;
            _workerLoadData.RunWorkerCompleted += WorkerLoadCompleted;

            _isLofCalc = false;

            _workerLoadData.RunWorkerAsync();
        }

        private void WorkerLoadDoWork(object sender, DoWorkEventArgs e)
        {
            _loadCompletedOk = true;
            _loadStopFlag = false;
            _loadCurrent = 0;

            int progress = 0;

            #region read file
            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(SetLoadDataProgressBarStyle),
                    new object[] { true });
            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<string>(UpdateLoadDataProcessingTextOnly),
                    new object[] { "Loading data from file ......" });

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(_filePathname);
            }
            catch (Exception ex)
            {
                _loadCompletedOk = false;
                _loadErrorMsg = "Cannot load data from file! Detail:\n" + ex.Message;
                return;
            }

            #region check the data type of this file
            string rootElementNameUfo = "UFOPCM";
            string rootElementNameEa = "EAPCM";
            string rootElementNameEaUfo = "EAUFOPCM";

            string rootElementName = null;
            var rootNode = xmlDoc[rootElementNameUfo];
            if (rootNode != null)
                rootElementName = rootElementNameUfo;
            else
            {
                rootNode = xmlDoc[rootElementNameEa];
                if (rootNode != null)
                    rootElementName = rootElementNameEa;
                else
                {
                    rootNode = xmlDoc[rootElementNameEaUfo];
                    if (rootNode != null)
                        rootElementName = rootElementNameEaUfo;
                    else
                        rootElementName = null;
                }
            }

            #endregion check the data type of this file

            if (rootElementName == null) // data file is invalidate
            {
                _loadCompletedOk = false;
                _loadErrorMsg = "Invalidate file!";
                return;
            }

            #endregion read file

            #region Load data

            _dimense = int.Parse(xmlDoc[rootElementName]["Dimense"].InnerText.Trim());

            if (rootElementName.Equals(rootElementNameEa))
                _ufoCount = 0;
            else
                _ufoCount = int.Parse(xmlDoc[rootElementName]["UFOCount"].InnerText.Trim());

            _amount = int.Parse(xmlDoc[rootElementName]["Amount"].InnerText.Trim());

            _loadTotal = _amount;

            XmlElement matricesNode = xmlDoc[rootElementName]["Matrices"];

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(SetLoadDataProgressBarStyle),
                    new object[] { false });

            _ufoDataLoaded4Lof = new List<Data4UfoPcm>();
            foreach (XmlElement matrixNode in matricesNode.ChildNodes)
            {
                if (_loadStopFlag)
                {
                    _loadCompletedOk = false;
                    _loadErrorMsg = "Stopped!";

                    return;
                }

                int p = (int)((double)_loadCurrent / (double)_loadTotal * 100);
                if (p > progress)
                {
                    progress = p;
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int>(UpdateLoadDataProcessingInfo),
                            new object[] { progress });
                }

                double avgBias = 0.0;
                if (rootElementName.Equals(rootElementNameEa))
                {
                    string biasString = matrixNode.GetAttribute("AverageBias");
                    if (double.TryParse(biasString, out var bias))
                        avgBias = bias;
                    else
                        avgBias = 0.0;
                }

                List<PosAndCr> changes = new List<PosAndCr>();
                if (!rootElementName.Equals(rootElementNameEa))
                {
                    XmlElement changesNode = matrixNode["Changes"];
                    foreach (XmlElement changeNode in changesNode.ChildNodes)
                    {
                        string rowStr = changeNode.GetAttribute("Row");
                        string columnStr = changeNode.GetAttribute("Column");
                        string changeValueStr = changeNode.GetAttribute("ChangeValue");
                        string crStr = changeNode.GetAttribute("CR");

                        PosAndCr pc = new PosAndCr(int.Parse(rowStr), int.Parse(columnStr),
                            double.Parse(changeValueStr), double.Parse(crStr));
                        changes.Add(pc);
                    }
                }

                double[][] matrix = new double[_dimense][];
                int index = 0;
                XmlElement matrixData = matrixNode["Data"];
                foreach (XmlElement rowNode in matrixData.ChildNodes)
                {
                    matrix[index] = GetDoubleFromString(rowNode.GetAttribute("Value"), _dimense);
                    index++;
                }


                Data4UfoPcm pcm;
                if (rootElementName.Equals(rootElementNameEa))
                    pcm = new Data4UfoPcm(matrix, avgBias, null);
                else
                    pcm = new Data4UfoPcm(matrix, 0.0, changes);

                _ufoDataLoaded4Lof.Add(pcm);

                _loadCurrent++;
            }
            #endregion Load data


        }

        private void WorkerLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_loadCompletedOk) // 成功计算了所有判断矩阵
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<int>(UpdateLoadDataProcessingInfo),
                        new object[] { 100 });

                #region 显示结果

                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateLoadDataMessage),
                        new object[] { _eaPcmErrorMsg });

                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateFilepathnameCalcLof),
                        new object[] { _filePathname });

                // calc the avg bias
                double sum = 0;
                foreach (var item in _ufoDataLoaded4Lof)
                    sum += item.AverageBias;
                double bias = sum / _ufoDataLoaded4Lof.Count;

                StringBuilder sb = new StringBuilder();
                sb.Append("Dimense: ");
                sb.Append(_dimense);
                sb.Append("; UFO Count: ");
                sb.Append(_ufoCount);
                sb.Append("; Amount: ");
                sb.Append(_amount);
                if (Math.Abs(bias - 0.0) > 0.00000001)
                {
                    sb.Append("; AvgBias: ");
                    sb.Append(bias.ToString("F6"));
                }

                ParametersOfLoadedData = sb.ToString();
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateParametersOfLoadedData),
                        new object[] { sb.ToString() });
                #endregion 显示结果
            }
            else
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateLoadDataMessage),
                        new object[] { _loadErrorMsg });
            }

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(UpdateLoadDataCalculatStatus),
                    new object[] { false });
        }

        public void UpdateLoadDataProcessingInfo(int progress)
        {
            ProgressValueDataLoad = progress;
            ProcessTextValueDataLoad = _loadCurrent + "/" + _loadTotal;
        }

        public void UpdateLoadDataProcessingTextOnly(string txt)
        {
            ProcessTextValueDataLoad = txt;
        }

        public void UpdateFilepathnameCalcLof(string filename)
        {
            FilepathnameCalcLof = filename;
        }

        public void UpdateParametersOfLoadedData(string txt)
        {
            ParametersOfLoadedData = txt;
        }

        public void UpdateLoadDataMessage(string msg)
        {
            LofStatisticText = msg;
        }

        public void UpdateLoadDataCalculatStatus(bool calculating)
        {
            Calculating = calculating;
        }

        private void SetLoadDataProgressBarStyle(bool isIndeterminate)
        {
            IsReadDataFromFile = isIndeterminate;
        }

        #endregion Load data
        public void SetLoadAndCalcLofStopFlag(bool stopFlag)
        {
            if (_isLofCalc)
                _lofCalcStopFlag = stopFlag;
            else 
                _loadStopFlag = stopFlag;
        }
        private double[] GetDoubleFromString(string str, int count)
        {
            double[] result = new double[count];

            int start = 0, end;
            string valStr;
            for (int i = 0; i < count; i++)
            {
                if (i < count - 1)
                {
                    end = str.IndexOf(",", start);
                    valStr = str.Substring(start, end - start);
                    start = end + 1;
                }
                else
                {
                    valStr = str.Substring(start);
                }

                result[i] = double.Parse(valStr);
            }

            return result;
        }

    }
}
