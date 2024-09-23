using System;
using System.Collections.Generic;
using System.ComponentModel;
using Lof4PcmGui.Lof4Pcm;
using Lof4PcmGui.Pso;

namespace Lof4PcmGui.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isReasonCheck = false;
        private bool _isAdjustCheck = false;
        private bool _isMinChangeCheck = false;

        private bool _reasonCheckCompletedOk;
        private bool _reasonCheckStopFlag;
        private int _reasonCheckTotal;
        private int _reasonCheckCurrent;
        private string _reasonCheckErrorMsg;

        private int _resultTotal;
        private int _resultUfoCount;
        private int _resultEaCount;

        private BackgroundWorker _workerReasonCheck = null;
        public BackgroundWorker WorkerReasonCheck
        {
            get { return _workerReasonCheck; }
        }

        private bool _adjustCheckCompletedOk;
        private bool _adjustCheckStopFlag;
        private int _adjustCheckTotal;
        private int _adjustCheckCurrent;
        private string _adjustCheckErrorMsg;

        private bool _minChangeCheckCompletedOk;
        private bool _minChangeCheckStopFlag;
        private int _minChangeCheckTotal;
        private int _minChangeCheckCurrent;
        private string _minChangeCheckErrorMsg;

        private Dictionary<int, int> _resultAdjustCheck;

        private BackgroundWorker _workerAdjustCheck = null;
        public BackgroundWorker WorkerAdjustCheck
        {
            get { return _workerAdjustCheck; }
        }

        private List<AdjustCheckResult> _adjustCheckResults = null;

        public List<AdjustCheckResult> AdjustCheckResults
        {
            get { return _adjustCheckResults; }
            set
            {
                _adjustCheckResults = value;
                OnPropertyChanged("AdjustCheckResults");
            }
        }

        private bool _optimalDirectionUsingOriMatrix = true;
        /// <summary>
        /// Adjust by original matrix.
        /// </summary>
        public bool OptimalDirectionUsingOriMatrix
        {
            get { return _optimalDirectionUsingOriMatrix; }
            set
            {
                _optimalDirectionUsingOriMatrix = value;
                OnPropertyChanged("OptimalDirectionUsingOriMatrix");
            }
        }

        private string _checkAdjustRate = "";
        public string CheckAdjustRate
        {
            get { return _checkAdjustRate; }
            set
            {
                _checkAdjustRate = value;
                OnPropertyChanged("CheckAdjustRate");
            }
        }

        private BackgroundWorker _workerMinChangeCheck = null;
        public BackgroundWorker WorkerMinChangeCheck
        {
            get { return _workerMinChangeCheck; }
        }


        private List<MinimumChangeCheckResult> _resultsMinimumChangeCheck = null;

        private List<MinimumChangeCheckResult> _minimumChangeCheckResults = null;
        public List<MinimumChangeCheckResult> MinimumChangeCheckResults
        {
            get { return _minimumChangeCheckResults; }
            set
            {
                _minimumChangeCheckResults = value;
                OnPropertyChanged("MinimumChangeCheckResults");
            }
        }

        private string _checkMinChangeRate = "";
        public string CheckMinChangeRate
        {
            get { return _checkMinChangeRate; }
            set
            {
                _checkMinChangeRate = value;
                OnPropertyChanged("CheckMinChangeRate");
            }
        }

        private int _progressValueMinChangeCheck = 0;
        /// <summary>
        /// Process value of min change check.
        /// </summary>
        public int ProgressValueMinChangeCheck
        {
            get { return _progressValueMinChangeCheck; }
            set
            {
                _progressValueMinChangeCheck = value;
                OnPropertyChanged("ProgressValueMinChangeCheck");
            }
        }

        private string _progressCountMinChangeCheck = "0/0";
        /// <summary>
        /// Process value of min change check.
        /// </summary>
        public string ProgressCountMinChangeCheck
        {
            get { return _progressCountMinChangeCheck; }
            set
            {
                _progressCountMinChangeCheck = value;
                OnPropertyChanged("ProgressCountMinChangeCheck");
            }
        }


        private bool _is3Sigma = true;
        /// <summary>
        /// 3 times sigma as threshold.
        /// </summary>
        public bool Is3Sigma
        {
            get { return _is3Sigma; }
            set
            {
                _is3Sigma = value;
                OnPropertyChanged("Is3Sigma");
            }
        }

        private int _progressValueReasonCheck = 0;
        /// <summary>
        /// Process value of reason check.
        /// </summary>
        public int ProgressValueReasonCheck
        {
            get { return _progressValueReasonCheck; }
            set
            {
                _progressValueReasonCheck = value;
                OnPropertyChanged("ProgressValueReasonCheck");
            }
        }

        private int _progressValueAdjustCheck = 0;
        /// <summary>
        /// Process value of adjust check.
        /// </summary>
        public int ProgressValueAdjustCheck
        {
            get { return _progressValueAdjustCheck; }
            set
            {
                _progressValueAdjustCheck = value;
                OnPropertyChanged("ProgressValueAdjustCheck");
            }
        }

        private string _checkResultTotal = "";
        public string CheckResultTotal
        {
            get { return _checkResultTotal; }
            set
            {
                _checkResultTotal = value;
                OnPropertyChanged("CheckResultTotal");
            }
        }

        private string _checkResultUfoCount = "";
        public string CheckResultUfoCount
        {
            get { return _checkResultUfoCount; }
            set
            {
                _checkResultUfoCount = value;
                OnPropertyChanged("CheckResultUfoCount");
            }
        }

        private string _checkResultEaCount = "";
        public string CheckResultEaCount
        {
            get { return _checkResultEaCount; }
            set
            {
                _checkResultEaCount = value;
                OnPropertyChanged("CheckResultEaCount");
            }
        }

        private string _reasonCheckMsgText = "";
        public string ReasonCheckMsgText
        {
            get { return _reasonCheckMsgText; }
            set
            {
                _reasonCheckMsgText = value;
                OnPropertyChanged("ReasonCheckMsgText");
            }
        }

        private string _adjustCheckMsgText = "";
        public string AdjustCheckMsgText
        {
            get { return _adjustCheckMsgText; }
            set
            {
                _adjustCheckMsgText = value;
                OnPropertyChanged("AdjustCheckMsgText");
            }
        }

        private string _minChangeCheckMsgText = "";
        public string MinChangeCheckMsgText
        {
            get { return _minChangeCheckMsgText; }
            set
            {
                _minChangeCheckMsgText = value;
                OnPropertyChanged("MinChangeCheckMsgText");
            }
        }

        private string _checkResultUfoRate = "";
        public string CheckResultUfoRate
        {
            get { return _checkResultUfoRate; }
            set
            {
                _checkResultUfoRate = value;
                OnPropertyChanged("CheckResultUfoRate");
            }
        }

        private string _checkResultEaRate = "";
        public string CheckResultEaRate
        {
            get { return _checkResultEaRate; }
            set
            {
                _checkResultEaRate = value;
                OnPropertyChanged("CheckResultEaRate");
            }
        }

        #region Reason Check

        bool IsInconsistentByUfo(Data4UfoPcm matrix, OutlierAlgorithm algorithm)
        {
            int dim = matrix.Matrix.Length;

            double maxOutlier;
            int uniqueItemNumber;
            bool mismatch;
            if (_isKnn)
                maxOutlier = OutlierCalc4Ufo(matrix, algorithm, out uniqueItemNumber, out mismatch);
            else
                maxOutlier = OutlierCalc4Ufo(matrix, algorithm, out uniqueItemNumber, out mismatch);

            var threshold = Consts.GetOutlierThresholdOfKnn(dim, Is3Sigma);
            if (maxOutlier > threshold)
                return true;
            else
                return false;
        }

        public void StartReasonCheck()
        {
            _workerReasonCheck = new BackgroundWorker();
            _workerReasonCheck.WorkerSupportsCancellation = true;
            _workerReasonCheck.DoWork += WorkerReasonReasonCheckDoWork;
            _workerReasonCheck.RunWorkerCompleted += WorkerReasonReasonCheckRunWorkerReasonCompleted;

            _isReasonCheck = true;

            _workerReasonCheck.RunWorkerAsync();
        }

        private void WorkerReasonReasonCheckDoWork(object sender, DoWorkEventArgs e)
        {
            _reasonCheckCompletedOk = true;
            _reasonCheckStopFlag = false;

            _reasonCheckTotal = UfoDataLoaded4Lof.Count;

            _reasonCheckCurrent = 0;

            int progress = 0;

            #region Reason Check 

            int ufoCount = 0;
            int eaCount = 0;

            foreach (var matrix in UfoDataLoaded4Lof)
            {
                if (_reasonCheckStopFlag)
                {
                    _reasonCheckCompletedOk = false;
                    _reasonCheckErrorMsg = "Stopped!";

                    return;
                }

                int p = (int)((double)_reasonCheckCurrent / (double)_reasonCheckTotal * 100);
                if (p > progress)
                {
                    progress = p;
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int>(UpdateReasonCheckProcessingInfo),
                            new object[] { progress });
                }

                // check 
                bool isUfo;
                if (_isKnn)
                    isUfo = IsInconsistentByUfo(matrix, OutlierAlgorithm.KNN);
                else
                    isUfo = IsInconsistentByUfo(matrix, OutlierAlgorithm.LOF);

                if (isUfo)
                    ufoCount++;
                else
                    eaCount++;

                _reasonCheckCurrent++;
            }

            _resultUfoCount = ufoCount;
            _resultEaCount = eaCount;
            _resultTotal = ufoCount + eaCount;

            #endregion Reason Check
        }

        private void WorkerReasonReasonCheckRunWorkerReasonCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_reasonCheckCompletedOk) // 成功计算了所有判断矩阵
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<int>(UpdateReasonCheckProcessingInfo),
                        new object[] { 100 });

                #region result

                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int, int, int>(UpdateReasonCheckMessage),
                            new object[] { _resultTotal, _resultUfoCount, _resultEaCount });

                #endregion result
            }
            else
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateReasonCheckMessageText),
                        new object[] { _eaPcmErrorMsg });
            }

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(UpdateReasonCheckCalculatStatus),
                    new object[] { false });
        }

        public void UpdateReasonCheckProcessingInfo(int progress)
        {
            ProgressValueReasonCheck = progress;
        }

        public void UpdateReasonCheckMessage(int total, int ufoCount, int eaCount)
        {
            if (total < 0 && ufoCount < 0 && eaCount < 0)
            {
                CheckResultTotal = "...";
                CheckResultUfoCount = "...";
                CheckResultEaCount = "...";
                CheckResultUfoRate = "...";
                CheckResultEaRate = "...";
            }
            else
            {
                CheckResultTotal = total.ToString();
                CheckResultUfoCount = ufoCount.ToString();
                CheckResultEaCount = eaCount.ToString();

                var ufoRate = (double)ufoCount / (double)total * 100;
                var eaRate = (double)eaCount / (double)total * 100;
                CheckResultUfoRate = ufoRate.ToString("F2") + "%";
                CheckResultEaRate = eaRate.ToString("F2") + "%";
            }
        }

        public void UpdateReasonCheckMessageText(string msg)
        {
            ReasonCheckMsgText = msg;
        }

        public void UpdateReasonCheckCalculatStatus(bool calculating)
        {
            Calculating = calculating;
        }
        #endregion Reason Check

        #region Adjust Check

        public void StartAdjustCheck()
        {
            _workerAdjustCheck = new BackgroundWorker();
            _workerAdjustCheck.WorkerSupportsCancellation = true;
            _workerAdjustCheck.DoWork += WorkerAdjustCheckDoWork;
            _workerAdjustCheck.RunWorkerCompleted += WorkerAdjustCheckRunWorkerCompleted;

            _isAdjustCheck = true;

            _workerAdjustCheck.RunWorkerAsync();
        }

        private void WorkerAdjustCheckDoWork(object sender, DoWorkEventArgs e)
        {
            if (UfoDataLoaded4Lof == null || UfoDataLoaded4Lof.Count <= 0)
                return;

            int dim = UfoDataLoaded4Lof[0].Matrix.Length;

            _adjustCheckCompletedOk = true;
            _adjustCheckStopFlag = false;

            _adjustCheckTotal = UfoDataLoaded4Lof.Count;

            _adjustCheckCurrent = 0;

            int progress = 0;

            #region Adjust Check 

            // adjustInfo中统计各修正次数出现的次数
            Dictionary<int, int> adjustInfo = new Dictionary<int, int>();

            double[][] matrix4calc = new double[dim][];
            for (int i = 0; i < dim; i++)
                matrix4calc[i] = new double[dim];

            foreach (var matrix in UfoDataLoaded4Lof)
            {
                if (_adjustCheckStopFlag)
                {
                    _adjustCheckCompletedOk = false;
                    _adjustCheckErrorMsg = "Stopped!";

                    return;
                }

                int p = (int)((double)_adjustCheckCurrent / (double)_adjustCheckTotal * 100);
                if (p > progress)
                {
                    progress = p;
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int>(UpdateAdjustCheckProcessingInfo),
                            new object[] { progress });
                }

                // clone matrix
                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        matrix4calc[i][j] = matrix.Matrix[i][j];
                    }
                }

                Pcm pcm = new Pcm(matrix4calc);
                var times = pcm.AdjustOptimalDirection(Consts.DoubleDelta, _optimalDirectionUsingOriMatrix, out var cr);
                if (adjustInfo.ContainsKey(times))
                    adjustInfo[times]++;
                else
                    adjustInfo.Add(times, 1);

                _adjustCheckCurrent++;
            }

            _resultAdjustCheck = adjustInfo;

            #endregion Adjust Check
        }

        private void WorkerAdjustCheckRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_adjustCheckCompletedOk) // 成功计算了所有判断矩阵
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<int>(UpdateAdjustCheckProcessingInfo),
                        new object[] { 100 });

                #region result

                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<Dictionary<int, int>>(UpdateAdjustCheckMessage),
                            new object[] { _resultAdjustCheck });

                #endregion result
            }
            else
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateAdjustCheckMessageText),
                        new object[] { _adjustCheckErrorMsg });
            }

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(UpdateAdjustCheckCalculatStatus),
                    new object[] { false });
        }

        public void UpdateAdjustCheckProcessingInfo(int progress)
        {
            ProgressValueAdjustCheck = progress;
        }

        public void UpdateAdjustCheckMessage(Dictionary<int, int> results)
        {
            if (results == null)
            {
                AdjustCheckResults = null;
                return;
            }

            int total = 0;
            List<AdjustCheckResult> result = new List<AdjustCheckResult>();
            foreach (var item in results)
            {
                var r = new AdjustCheckResult(item.Key, item.Value);
                if (item.Key > 0)
                    total += item.Value;
                result.Add(r);
            }

            CheckAdjustRate = ((double)total / (double)_amount * 100).ToString("F4") + "%";
            AdjustCheckResults = result;
        }

        public void UpdateAdjustCheckMessageText(string msg)
        {
            AdjustCheckMsgText = msg;
        }

        public void UpdateAdjustCheckCalculatStatus(bool calculating)
        {
            Calculating = calculating;
        }
        #endregion Adjust Check

        #region Minimum Change Check

        public void StartMinChangeCheck()
        {
            _workerMinChangeCheck = new BackgroundWorker();
            _workerMinChangeCheck.WorkerSupportsCancellation = true;
            _workerMinChangeCheck.DoWork += WorkerMinimumChangeCheckDoWork;
            _workerMinChangeCheck.RunWorkerCompleted += WorkerMinChangeCheckRunWorkerCompleted;

            _isMinChangeCheck = true;

            _workerMinChangeCheck.RunWorkerAsync();
        }

        private void WorkerMinimumChangeCheckDoWork(object sender, DoWorkEventArgs e)
        {
            if (UfoDataLoaded4Lof == null || UfoDataLoaded4Lof.Count <= 0)
                return;

            #region PSO paras

            var psoParas = new PsoArgs();
            psoParas.C1 = 1.5;
            psoParas.C2 = 1.5;
            psoParas.MaxConsistenceScale = 0.1;
            psoParas.AdjustPrecise = 3;
            psoParas.VarianceCoefficient = 0.5;
            psoParas.Range = 1.0;

            #endregion PSO paras

            int dim = UfoDataLoaded4Lof[0].Matrix.Length;

            _minChangeCheckCompletedOk = true;
            _minChangeCheckStopFlag = false;

            _minChangeCheckTotal = UfoDataLoaded4Lof.Count;

            _minChangeCheckCurrent = 0;

            int progress = 0;


            #region Check

            List<MinimumChangeCheckResult> checkInfo = new List<MinimumChangeCheckResult>();
            MinimumChangeCheckResult checkItem = new MinimumChangeCheckResult("[0.10, 0.15)",
                0, 0);
            checkInfo.Add(checkItem);
            checkItem = new MinimumChangeCheckResult("[0.15, 0.20)",
               0, 0);
            checkInfo.Add(checkItem);
            checkItem = new MinimumChangeCheckResult("[0.20, 0.30)",
               0, 0);
            checkInfo.Add(checkItem);
            checkItem = new MinimumChangeCheckResult("[0.30, +\u221e]",
               0, 0);
            checkInfo.Add(checkItem);

            double[][] matrix4calc = new double[dim][];
            for (int i = 0; i < dim; i++)
                matrix4calc[i] = new double[dim];

            foreach (var matrix in UfoDataLoaded4Lof)
            {
                if (_minChangeCheckStopFlag)
                {
                    _minChangeCheckCompletedOk = false;
                    _minChangeCheckErrorMsg = "Stopped!";

                    return;
                }

                int p = (int)((double)_minChangeCheckCurrent / (double)_minChangeCheckTotal * 100);
                if (p > progress)
                {
                    progress = p;
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<int>(UpdateMinChangeCheckProcessingInfo),
                            new object[] { progress });
                }

                // clone matrix
                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        matrix4calc[i][j] = matrix.Matrix[i][j];
                    }
                }

                Pcm pcm = new Pcm(matrix4calc);
                var crOri = pcm.CalWeightinessPower(Consts.DoubleDelta);
                var crAdjusted = pcm.AdjustMinimumChange(psoParas, out var avgChange);

                if (crOri >= 0.10 && crOri < 0.15)
                {
                    checkInfo[0].TotalOfRange++;
                    if (crAdjusted < 0.1001) // 考虑误差，小数点后四位认为修正成功
                    {
                        checkInfo[0].AdjustOkCounterOfRange++;
                        checkInfo[0].AvgChangeOfRange.Add(avgChange);
                    }
                }
                else if (crOri < 0.20)
                {
                    checkInfo[1].TotalOfRange++;
                    if (crAdjusted < 0.1001) // 考虑误差，小数点后四位认为修正成功
                    {
                        checkInfo[1].AdjustOkCounterOfRange++;
                        checkInfo[1].AvgChangeOfRange.Add(avgChange);
                    }
                }
                else if (crOri < 0.30)
                {
                    checkInfo[2].TotalOfRange++;
                    if (crAdjusted < 0.1001) // 考虑误差，小数点后四位认为修正成功
                    {
                        checkInfo[2].AdjustOkCounterOfRange++;
                        checkInfo[2].AvgChangeOfRange.Add(avgChange);
                    }
                }
                else
                {
                    checkInfo[3].TotalOfRange++;
                    if (crAdjusted < 0.1001) // 考虑误差，小数点后四位认为修正成功
                    {
                        checkInfo[3].AdjustOkCounterOfRange++;
                        checkInfo[3].AvgChangeOfRange.Add(avgChange);
                    }
                }
                _minChangeCheckCurrent++;

                if (_minChangeCheckCurrent % 2 == 0)
                {
                    int total = 0;
                    int ok = 0;
                    double avgTotal = 0;
                    foreach (MinimumChangeCheckResult result in checkInfo)
                    {
                        total += result.TotalOfRange;
                        ok += result.AdjustOkCounterOfRange;

                        foreach (var val in result.AvgChangeOfRange)
                            avgTotal += val;
                    }
                    double avgFinal = avgTotal / (double)ok;

                    double stdSum = 0;
                    foreach (MinimumChangeCheckResult result in checkInfo)
                        foreach (var val in result.AvgChangeOfRange)
                            stdSum += (val - avgFinal) * (val - avgFinal);
                    stdSum = stdSum / (double)ok;
                    double stdFinale = Math.Sqrt(stdSum);

                    string processingText = _minChangeCheckCurrent + "/" + _minChangeCheckTotal + "; Adjusted Rate: "
                                            + ((double)ok / (double)total * 100).ToString("F4") + "%; AVG: "
                                            + avgFinal.ToString("F4") + "; STD: " + stdFinale.ToString("F4");
                    if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                        AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<string>(UpdateMinChangeCheckProcessingText),
                            new object[] { processingText });
                }
            }

            // avg
            foreach (var result in checkInfo)
            {
                if (result.AdjustOkCounterOfRange > 0)
                {
                    double sum = 0;
                    foreach (var val in result.AvgChangeOfRange)
                        sum += val;
                    result.AvgAvgChangeOfRange = sum / (double)result.AdjustOkCounterOfRange;
                }
                else
                    result.AvgAvgChangeOfRange = 0;
            }
            // std
            foreach (var result in checkInfo)
            {
                if (result.AdjustOkCounterOfRange > 0)
                {
                double sum = 0;
                foreach (var val in result.AvgChangeOfRange)
                    sum += (val - result.AvgAvgChangeOfRange) * (val - result.AvgAvgChangeOfRange);
                sum = sum / (double)result.AdjustOkCounterOfRange;
                result.StdAvgChangeOfRange = Math.Sqrt(sum);
                }
                else
                    result.AvgAvgChangeOfRange = 0;
            }

            _resultsMinimumChangeCheck = checkInfo;

            #endregion  Check
        }

        private void WorkerMinChangeCheckRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_minChangeCheckCompletedOk) // 成功计算了所有判断矩阵
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<int>(UpdateMinChangeCheckProcessingInfo),
                        new object[] { 100 });

                #region result

                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                            new Action<List<MinimumChangeCheckResult>>(UpdateMinChangeCheckMessage),
                            new object[] { _resultsMinimumChangeCheck });

                #endregion result
            }
            else
            {
                if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                    AppWindowMain.Dispatcher.BeginInvoke(
                        new Action<string>(UpdateMinChangeCheckMessageText),
                        new object[] { _minChangeCheckErrorMsg });
            }

            if (AppWindowMain != null && AppWindowMain.Dispatcher != null)
                AppWindowMain.Dispatcher.BeginInvoke(
                    new Action<bool>(UpdateMinChangeCheckCalculatStatus),
                    new object[] { false });
        }

        public void UpdateMinChangeCheckProcessingInfo(int progress)
        {
            ProgressValueMinChangeCheck = progress;
        }

        public void UpdateMinChangeCheckProcessingText(string progress)
        {
            ProgressCountMinChangeCheck = progress;
        }


        public void UpdateMinChangeCheckMessage(List<MinimumChangeCheckResult> results)
        {
            if (results == null)
            {
                MinimumChangeCheckResults = null;
                return;
            }

            foreach (MinimumChangeCheckResult result in results)
            {
                if (result.TotalOfRange > 0)
                {
                    result.OkRateOfRange =
                        ((double)result.AdjustOkCounterOfRange / (double)result.TotalOfRange * 100).ToString("F4") + "%";
                }
                else
                {
                    result.OkRateOfRange = "100.0000%";
                }

            }

            MinimumChangeCheckResults = results;
        }

        public void UpdateMinChangeCheckMessageText(string msg)
        {
            MinChangeCheckMsgText = msg;
        }

        public void UpdateMinChangeCheckCalculatStatus(bool calculating)
        {
            Calculating = calculating;
        }
        #endregion Minimum Change Check

        public void SetReasonCheckStopFlag(bool stopFlag)
        {
            if (_isReasonCheck)
                _reasonCheckStopFlag = stopFlag;
        }

    }
}
