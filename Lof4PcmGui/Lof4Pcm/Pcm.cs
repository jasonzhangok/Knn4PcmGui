using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using Lof4PcmGui.Pso;

namespace Lof4PcmGui.Lof4Pcm
{
    public class Pcm
    {
        private double[][] _outlier;

        public double[][] Outlier
        {
            get { return _outlier; }
        }

        private int _uniqueItemNumber;
        public int UniqueItemNumber
        {
            get { return _uniqueItemNumber; }
        }

        private double[][] _matrix = null;
        private double[][] _ori_matrix4calc = null;

        private double[] _weightinessScales;
        private double _lambdaMax = -1;
        private double _ci = -1;
        private double _ri = -1;
        private double _cr = -1;

        public double[] Weights
        {
            get { return _weightinessScales; }
        }

        public Pcm(double[][] matrix)
        {
            _matrix = matrix;
            int dimense = _matrix.Length;

            // init the result weightiness array
            _weightinessScales = new double[dimense];
            // 将排序权重初始化为-1，只有计算过后，才不是小于0的数值。
            for (int i = 0; i < dimense; i++)
                _weightinessScales[i] = -1;

            _ori_matrix4calc = new double[dimense][];
            for (int i = 0; i < dimense; i++)
            {
                _ori_matrix4calc[i] = new double[dimense];
            }
            for (int i = 0; i < dimense; i++)
            {
                for (int j = 0; j < dimense; j++)
                {
                    _ori_matrix4calc[i][j] = _matrix[i][j];
                }
            }
        }

        public void Knn(double emPrecise)
        {
            #region 孤立点分析判断不一致原因
            //////////////////////////////////////////////////////////////////////////
            // modified by Zhang Jianhua, 20240619
            // 孤立点分析，初步判断不一致原因，如果孤立点分析无法确定原因，再根据最小改动修正是否成功判断是否用最大方向修正
            int len = _matrix.GetLength(0);

            double[][] influenceMatrix = new double[len][];
            double[][] changeMatrix = new double[len][];


            double consistence = CalWeightinessPower(emPrecise);
            //init temp matrix4calc
            for (int i = 0; i < len; i++)
            {
                influenceMatrix[i] = new double[len];
                changeMatrix[i] = new double[len];
            }

            for (int i = 0; i < len; i++)
                for (int j = 0; j < len; j++)
                {
                    influenceMatrix[i][j] = 0;
                    changeMatrix[i][j] = 0;
                }

            //get the change value matrix4calc
            GetInfluenceDegreeMatrix(influenceMatrix, changeMatrix, consistence, emPrecise);

            // 计算离群系数
            _outlier = OutlierCalculateKnn(consistence, influenceMatrix, changeMatrix);

            #region 判断离群点是否为对一致性影响小的点
            var outlierThreshold = Consts.GetOutlierThresholdOfKnn(len);

            // 找出outlier以及influenceMatrix中的最大值
            double maxOutlier = 0;
            int maxOutlierRow = 0;
            int maxOutlierColumn = 1;
            double avgInfluence = 0;

            for (int i = 0; i < len - 1; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    if (maxOutlier < _outlier[i][j])
                    {
                        maxOutlier = _outlier[i][j];
                        maxOutlierRow = i;
                        maxOutlierColumn = j;
                    }
                }
            }

            if (maxOutlier > outlierThreshold)
            {
                var isOutlierBySmall = IsOutlierBySmall(maxOutlierRow, maxOutlierColumn, influenceMatrix, _outlier,
                    outlierThreshold);

                if (isOutlierBySmall)
                {
                    // 离群点是否为对一致性影响小的点
                    for (int i = 0; i < len - 1; i++)
                    {
                        for (int j = i + 1; j < len; j++)
                        {
                            _outlier[i][j] = 0.0;
                        }
                    }
                }
            }

            #endregion 判断离群点是否为对一致性影响小的点

            #endregion 孤立点分析判断不一致原因
        }

        public void Lof(double emPrecise)
        {
            #region 孤立点分析判断不一致原因
            //////////////////////////////////////////////////////////////////////////
            // modified by Zhang Jianhua, 20240619
            // 孤立点分析，初步判断不一致原因，如果孤立点分析无法确定原因，再根据最小改动修正是否成功判断是否用最大方向修正
            int len = _matrix.GetLength(0);

            double[][] influenceMatrix = new double[len][];
            double[][] changeMatrix = new double[len][];


            double consistence = CalWeightinessPower(emPrecise);
            //init temp matrix4calc
            for (int i = 0; i < len; i++)
            {
                influenceMatrix[i] = new double[len];
                changeMatrix[i] = new double[len];
            }

            for (int i = 0; i < len; i++)
                for (int j = 0; j < len; j++)
                {
                    influenceMatrix[i][j] = 0;
                    changeMatrix[i][j] = 0;
                }

            //get the change value matrix4calc
            GetInfluenceDegreeMatrix(influenceMatrix, changeMatrix, consistence, emPrecise);

            // 计算离群系数
            _outlier = OutlierCalculateLof(consistence, influenceMatrix, changeMatrix, out var uniqueNumber);
            _uniqueItemNumber = uniqueNumber;

            #region 判断离群点是否为对一致性影响小的点
            var lofThreshold = 1.9;

            // 找出outlier以及influenceMatrix中的最大值
            double maxOutlier = 0;
            int maxOutlierRow = 0;
            int maxOutlierColumn = 1;
            double avgInfluence = 0;

            for (int i = 0; i < len - 1; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    if (maxOutlier < _outlier[i][j])
                    {
                        maxOutlier = _outlier[i][j];
                        maxOutlierRow = i;
                        maxOutlierColumn = j;
                    }
                }
            }

            if (maxOutlier > lofThreshold)
            {
                var isOutlierBySmall = IsOutlierBySmall(maxOutlierRow, maxOutlierColumn, influenceMatrix, _outlier,
                    lofThreshold);

                if (isOutlierBySmall)
                {
                    // 离群点是否为对一致性影响小的点
                    for (int i = 0; i < len - 1; i++)
                    {
                        for (int j = i + 1; j < len; j++)
                        {
                            _outlier[i][j] = 0.0;
                        }
                    }
                }
            }

            #endregion 判断离群点是否为对一致性影响小的点

            #region DEBUG

            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < len; i++)
            //{
            //    for (int j = 0; j < len; j++)
            //    {
            //        sb.Append(outlier[i][j].ToString("F4"));
            //        sb.Append(", ");
            //    }
            //    sb.Append("\n");
            //}

            #endregion DEBUG

            #endregion 孤立点分析判断不一致原因

        }

        private bool IsOutlierBySmall(int row, int column, double[][] influenceMatrix, double[][] lofMatrix, double smallLofThreshold)
        {
            double sum = 0;
            int count = 0;
            var dimense = influenceMatrix.Length;

            for (int i = 0; i < dimense - 1; i++)
            {
                for (int j = i + 1; j < dimense; j++)
                {
                    if (i != row && j != column)
                    {
                        if (lofMatrix[i][j] < smallLofThreshold)
                        {
                            sum += influenceMatrix[i][j];
                            count++;
                        }
                    }
                }
            }

            double avg = sum / (double)count;

            if (influenceMatrix[row][column] < avg)
                return true;
            else
                return false;
        }
        #region 离群点检测

        private double[][] OutlierCalculateKnn(double consistence, double[][] influenceMatrix, double[][] changeMatrix)
        {
            int dim = influenceMatrix.Length;

            double[][] outlier = new double[dim][];
            for (int i = 0; i < dim; i++)
            {
                outlier[i] = new double[dim];
            }

            for (int i = 0; i < dim - 1; i++)
            {
                for (int j = i + 1; j < dim; j++)
                {
                    outlier[i][j] = Knn(consistence,
                        influenceMatrix, changeMatrix, i, j);
                }
            }

            return outlier;
        }

        private double Knn(double consistence,
            double[][] influenceMatrix, double[][] changeMatrix,
            int row, int column)
        {
            int dim = influenceMatrix.Length;
            //int k = GetKOfLofByDataCount(dim);
            int k = GetKOfKnnByDataCount(dim);

            double maxInfluence = 0;
            double maxChange = 0;

            for (int i = 0; i < dim - 1; i++)
            {
                for (int j = i + 1; j < dim; j++)
                {
                    if (maxInfluence < influenceMatrix[i][j])
                        maxInfluence = influenceMatrix[i][j];

                    if (maxChange < changeMatrix[i][j])
                        maxChange = changeMatrix[i][j];
                }
            }

            List<DistAndPos> distances = new List<DistAndPos>();
            for (int i = 0; i < dim - 1; i++)
            {
                for (int j = i + 1; j < dim; j++)
                {
                    if (row == i && column == j)
                        continue;
                    else
                    {
                        double distance = Distance(dim, consistence, maxInfluence, maxChange,
                            influenceMatrix[row][column], influenceMatrix[i][j],
                            changeMatrix[row][column], changeMatrix[i][j]);
                        DistAndPos dp = new DistAndPos(distance, i, j);
                        distances.Add(dp);
                    }
                }
            }
            distances.Sort((x, y) => x.Distance.CompareTo(y.Distance));

            double kDist = distances[k - 1].Distance;   // k距离

            List<DistAndPos> distancesResult = new List<DistAndPos>();
            foreach (var d in distances)
            {
                if (d.Distance <= kDist)
                    distancesResult.Add(d);
            }

            double largest = 0;
            foreach (var dr in distancesResult)
            {
                if (dr.Distance > largest)
                    largest = dr.Distance;
            }
            double median = 0;
            if (distancesResult.Count == 1)
                median = distancesResult[0].Distance;
            else if (distancesResult.Count / 2 == 0) // Even
            {
                median = (distancesResult[distancesResult.Count / 2 - 1].Distance
                          + distancesResult[distancesResult.Count / 2].Distance) / 2.0;
            }
            else    // Odd
                median = distancesResult[(distancesResult.Count + 1) / 2 - 1].Distance;

            //return sum / (double)(distancesResult.Count);   // mean
            //return largest;   //largest
            return median;    // median

        }

        /// <summary>
        /// 计算矩阵中上三角阵各元素的局部离群因子(LOF)
        /// </summary>
        /// <param name="consistence">原始矩阵的一致性比例。</param>
        /// <param name="influenceMatrix">各要素对一致性的影响矩阵。</param>
        /// <param name="changeMatrix">对应influenceMatrix的各要素改变矩阵。</param>
        /// <param name="uniqueItemNumber">输出参数，表示矩阵里不重复的元素有几个</param>
        /// <returns>一个矩阵，其中上三角阵各元素为各元素对应的局部离群因子(LOF)</returns>
        private double[][] OutlierCalculateLof(double consistence, double[][] influenceMatrix,
            double[][] changeMatrix, out int uniqueItemNumber)
        {
            int dim = influenceMatrix.Length;

            #region 估计最大方向修正次数

            int adjustTimes = 0;
            List<double> adjusts = new List<double>();
            for (int i = 0; i < dim - 1; i++)
            {
                for (int j = i + 1; j < dim; j++)
                {
                    adjusts.Add(influenceMatrix[i][j]);
                }
            }
            adjusts.Sort((x, y) => x.CompareTo(y));

            double sum = 0;
            foreach (var adjust in adjusts)
            {
                sum += adjust;
                adjustTimes++;
                if (consistence - sum <= 0.1) // 修正成功
                    break;
            }

            #endregion 估计最大方向修正次数


            double[][] outlier = new double[dim][];
            for (int i = 0; i < dim; i++)
            {
                outlier[i] = new double[dim];
            }

            int uniqueNumber = dim * (dim - 1) / 2;
            for (int i = 0; i < dim - 1; i++)
            {
                for (int j = i + 1; j < dim; j++)
                {
                    outlier[i][j] = LocalOutlierFactor(consistence, adjustTimes,
                        influenceMatrix, changeMatrix, i, j,
                        out uniqueNumber);
                }
            }

            uniqueItemNumber = uniqueNumber;
            return outlier;
        }

        private int GetKOfLofByDataCount(int count)
        {
            //double a = 1;
            //double b = -1;
            //double c = -2 * count;

            if (count == 3) return 1;
            if (count == 4) return 2;
            double x1 = (1.0 + Math.Sqrt(1.0 + 8.0 * count)) / 2.0;
            return (int)Math.Floor(x1);
        }

        private int GetKOfKnnByDataCount(int dimense)
        {
            double itemNumber = dimense * (dimense - 1) / 2.0;
            return (int)Math.Floor(Math.Sqrt(itemNumber));
        }

        /// <summary>
        /// 计算矩阵上三角中某个位置元素的局部离群因子
        /// </summary>
        /// <param name="consistence">判断矩阵未修正前的一致性比例。</param>
        /// <param name="adjustTimes">预估的最大方向修正次数。</param>
        /// <param name="influenceMatrix">影响矩阵</param>
        /// <param name="changeMatrix">改变矩阵</param>
        /// <param name="row">上三角位置的一个行坐标，索引从0开始。</param>
        /// <param name="column">上三角位置的一个列坐标，索引从0开始。</param>
        /// <param name="uniqueItemNumber">输出参数，表示矩阵里不重复的元素有几个</param>
        /// <returns>LOF(influenceMatrix[i][j], k)</returns>
        private double LocalOutlierFactor(double consistence, int adjustTimes,
            double[][] influenceMatrix, double[][] changeMatrix,
            int row, int column, out int uniqueItemNumber)
        {
            int dim = influenceMatrix.Length;

            #region 合并重复数据

            List<LrdItem> dataWithoutDuplicate = new List<LrdItem>();
            for (int i = 0; i < dim - 1; i++)
            {
                for (int j = i + 1; j < dim; j++)
                {
                    // 检查是否已有这个数据
                    bool found = false;
                    foreach (var data in dataWithoutDuplicate)
                    {
                        if (Math.Abs(influenceMatrix[i][j] - data.Influence) < Consts.DuplicateDelta)
                        //&& Math.Abs(changeMatrix[i][j] - data.Change) < Consts.DuplicateDelta) // 完全相同
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        LrdItem lrdItem = new LrdItem(influenceMatrix[i][j], changeMatrix[i][j], i, j);
                        dataWithoutDuplicate.Add(lrdItem);
                    }
                }
            }

            int indexItem2Calc = -1;  // 根据row和column确定对应数据在dataWithoutDuplicate中的索引
            double influence = influenceMatrix[row][column];
            double change = changeMatrix[row][column];
            int index = 0;
            for (int i = 0; i < dataWithoutDuplicate.Count; i++)
            {
                if (Math.Abs(influence - dataWithoutDuplicate[i].Influence) < Consts.DuplicateDelta)
                //&& Math.Abs(change - dataWithoutDuplicate[i].Change) < Consts.DuplicateDelta) // 完全相同
                {
                    indexItem2Calc = i;
                    break;
                }
            }

            if (indexItem2Calc < 0)
            {
                uniqueItemNumber = -1;
                return 0.0;
            }

            // 根据不重复元素，改变k值
            if (dataWithoutDuplicate.Count <= 2)
            {
                uniqueItemNumber = 2;
                return 0.0;
            }

            int trueK = GetKOfLofByDataCount(dataWithoutDuplicate.Count);

            #endregion 合并重复数据


            List<double> distances = new List<double>();
            var lrdItems = LocalReachDist(dataWithoutDuplicate, indexItem2Calc, consistence, adjustTimes, trueK, out var lrd);

            //var lrdItems = LocalReachDist(influenceMatrix, changeMatrix, consistence, adjustTimes, row, column, k, out var lrd);
            double sigmaLrd = 0;

            int nk = lrdItems.Count;

            double sum = 0;
            int zeroNumber = 0;
            foreach (var item in lrdItems)
            {
                var items = LocalReachDist(dataWithoutDuplicate, item.Row, consistence, adjustTimes, trueK, out var lrdInner);
                //var items = LocalReachDist(influenceMatrix, changeMatrix, consistence, adjustTimes, item.Row, item.Column, k, out var lrdInner);
                if (Math.Abs(double.MaxValue - lrdInner) < 0.0000000001) // 距离为0
                {
                    sum = double.MaxValue;
                    zeroNumber++;
                }
                else
                    sum += lrdInner;
            }

            double lof;
            if (Math.Abs(double.MaxValue - sum) < 0.0000000001 && Math.Abs(double.MaxValue - lrd) < 0.0000000001) // 0
                //lof = (double)zeroNumber / (double)nk;
                lof = (double)1 / (double)nk;
            else
                lof = (sum / (double)nk) / lrd;

            uniqueItemNumber = dataWithoutDuplicate.Count;
            return lof;
        }

        private List<DistAndPos> LocalReachDist(List<LrdItem> data, int index,
            double consistence, int adjustTimes, int k, out double lrd)
        {
            int dim = data.Count;
            double distance = 0;
            List<DistAndPos> distances = new List<DistAndPos>();

            double maxInfluence = 0;
            double maxChange = 0;
            foreach (var lrdItem in data)
            {
                if (maxInfluence < lrdItem.Influence)
                    maxInfluence = lrdItem.Influence;

                if (maxChange < lrdItem.Change)
                    maxChange = lrdItem.Change;

            }

            for (int i = 0; i < dim; i++)
            {
                if (i == index)
                    continue;

                distance = Distance(dim, consistence, maxInfluence, maxChange,
                    data[index].Influence, data[i].Influence,
                    data[index].Change, data[i].Change);
                DistAndPos dp = new DistAndPos(distance, i, -1);    // 数据的索引index保存在x中
                distances.Add(dp);
            }

            distances.Sort((x, y) => x.Distance.CompareTo(y.Distance));

            double kDist = distances[k - 1].Distance;   // k距离

            double result = 0;
            List<DistAndPos> distancesResult = new List<DistAndPos>();
            foreach (var d in distances)
            {
                if (d.Distance <= kDist)
                {
                    if (d.Distance < kDist)
                        result += kDist;
                    else
                        result += d.Distance;
                    distancesResult.Add(d);
                }
            }

            if (result < 0.0000000000000001) // 等于0
                lrd = double.MaxValue;
            else
                lrd = (double)(distancesResult.Count) / result;
            return distancesResult;
        }

        private double Distance(int dim, double consistence,
            double maxInfluence, double maxChange,
            double influence1, double influence2,
            double change1, double change2)
        {
            double influence = Math.Abs(influence1 - influence2);
            double change = Math.Abs(change1 - change2);
            double trueChange;
            //if (change < 2.0) // 在两个两两比较等级内
            //    trueChange = 0;
            //else
            trueChange = 0.2 * change * (maxInfluence / maxChange);  // 将改变数值缩放到与影响数值同样的范围内，并且change的影响只有influence的一半

            //// 改变值也要根据对一致性的影响缩放，影响越大，改变值影响也越大，这样可以突出改变大并且一致性影响大的Item
            //trueChange *= influence / maxInfluence;

            double distance = Math.Sqrt(influence * influence + trueChange * trueChange);
            return distance;
            //return influence + trueChange;
            //return Math.Abs(influence1 - influence2);
        }

        #endregion 离群点检测

        private double CalAdjustValue(int s, int t, int n)
        {
            double p = 0;
            double q = 0;
            int i = 0;
            int j = 0;
            int k = 0;

            for (k = t + 1; k < n; k++)
                p += _matrix[s][k] / _matrix[t][k];
            for (i = 0; i < s; i++)
                p += _matrix[i][t] / _matrix[i][s];
            for (j = s + 1; j < t; j++)
                p += _matrix[s][j] * _matrix[j][t];

            for (k = t + 1; k < n; k++)
                q += _matrix[t][k] / _matrix[s][k];
            for (i = 0; i < s; i++)
                q += _matrix[i][s] / _matrix[i][t];
            for (j = s + 1; j < t; j++)
                q += 1 / (_matrix[s][j] * _matrix[j][t]);

            double result = Math.Sqrt(p / q);
            double lowerBound = 1.0 / 9.0;
            double upperBound = 9.0;

            if (result > upperBound)
                result = upperBound;
            else if (result < lowerBound)
                result = lowerBound;
            return result;
        }

        private void GetInfluenceDegreeMatrix(double[][] influenceMatrix, double[][] changeMatrix, double consistenceOfOriMatrix, double emPrecise)
        {
            int len = influenceMatrix.GetLength(0);
            int i = 0;
            int j = 0;
            double oriValue = 0;
            double oriValueReverse = 0;
            double consistence = 0;

            //for each item in matrix4calc, calculate the influence degree
            for (i = 0; i < len; i++)
                for (j = 0; j < len; j++)
                {
                    if (i < j)
                    {
                        //save the original value
                        oriValue = _matrix[i][j];
                        oriValueReverse = _matrix[j][i];
                        //set the matrix4calc value to the adjusted value
                        _matrix[i][j] = CalAdjustValue(i, j, len);
                        _matrix[j][i] = 1 / _matrix[i][j];
                        //call the CalWeightiness to get the consistence value 
                        consistence = CalWeightinessPower(emPrecise);

                        if (consistenceOfOriMatrix < consistence)
                        {
                            influenceMatrix[i][j] = 0;
                            changeMatrix[i][j] = 0;
                            changeMatrix[j][i] = 0;
                        }
                        else
                        {
                            influenceMatrix[i][j] = consistenceOfOriMatrix - consistence;

                            #region 计算等级差
                            var oriLevel = EvenAnd129Converter.GetEvenFrom129(oriValue);
                            var newLevel = EvenAnd129Converter.GetEvenFrom129(_matrix[i][j]);
                            changeMatrix[i][j] = Math.Abs(oriLevel - newLevel);
                            changeMatrix[j][i] = Math.Abs(oriLevel - newLevel);
                            #endregion 计算等级差
                        }

                        //calculte end, resume the original matrix4calc
                        _matrix[i][j] = oriValue;
                        _matrix[j][i] = oriValueReverse;
                    }
                }
            //all item is processed, resume the weightness by call CalWeightiness
            CalWeightinessPower(emPrecise);
            return;
        }

        public double CalWeightinessPower(double precise)
        {
            if (_matrix.GetLength(0) == 1)  // 一阶判断矩阵
            {
                _lambdaMax = 1;
                _weightinessScales[0] = 1.0;
                _ci = 0;
                _ri = 0;
                _cr = 0;
                return 0;
            }

            int dimense = _matrix.GetLength(0);
            double[] wi = new double[dimense];
            double mCur = 0;
            double mPre = 0;
            double[] x = new double[dimense];
            double[] y = new double[dimense];

            int i = 0;
            int j = 0;

            double[][] matrix = null;
            matrix = new double[dimense][];
            for (i = 0; i < dimense; i++)
                matrix[i] = new double[dimense];

            // 构造等价矩阵
            for (i = 0; i < dimense; i++)
                for (j = 0; j < dimense; j++)
                    matrix[i][j] = _matrix[i][j];

            double sum = 0;

            for (i = 0; i < dimense; i++)
            {
                x[i] = 1;
                y[i] = 1;
            }

            mCur = 1;
            // 迭代过程
            int times = 0;
            do
            {
                times++;
                if (times > 10000)
                {
                    // set wi
                    for (i = 0; i < dimense; i++)
                    {
                        _weightinessScales[i] = 1.0;
                    }

                    return -99999;
                }

                mPre = mCur;

                // calc x(k+1)
                for (i = 0; i < dimense; i++)
                {
                    for (j = 0, sum = 0; j < dimense; j++)
                        sum += matrix[i][j] * y[j];
                    x[i] = sum;
                }

                // m: max x(k+1)
                mCur = x[0];
                for (i = 0; i < dimense; i++)
                {
                    if (mCur < x[i])
                        mCur = x[i];
                }

                // calc y(k+1)
                for (i = 0; i < dimense; i++)
                    y[i] = x[i] / mCur;
            } while (Math.Abs(mCur - mPre) >= precise);

            // get wi
            for (i = 0, sum = 0; i < dimense; i++)
                sum += y[i];
            for (i = 0; i < dimense; i++)
            {
                wi[i] = y[i] / sum;
                _weightinessScales[i] = wi[i];
            }

            double max = mCur;
            _lambdaMax = max;

            double ci = (max - matrix.GetLength(0)) / ((matrix.GetLength(0) - 1));
            double ri = Consts.GetRiByN(matrix.GetLength(0));
            if (Math.Abs(ri - 0) < Consts.DoubleDelta)
            {
                _ci = ci;
                _ri = ri;
                _cr = 0.0000;
                return 0.0000;
            }

            if (Math.Abs(ri + 1) < Consts.DoubleDelta) //failed, too many items
            {
                _ci = ci;
                _ri = ri;
                _cr = -1;
                return -1;
            }

            _ci = ci;
            _ri = ri;
            _cr = _ci / _ri;
            return ci / ri;
        }

        private List<PosAndCr> GetSortedItemInfluence(double emPrecise = Consts.DoubleDelta)
        {
            int len = _matrix.GetLength(0);

            double[][] influenceMatrix = new double[len][];
            double[][] changeMatrix = new double[len][];

            double consistence = CalWeightinessPower(emPrecise);
            //init temp matrix4calc
            for (int i = 0; i < len; i++)
            {
                influenceMatrix[i] = new double[len];
                changeMatrix[i] = new double[len];
            }

            for (int i = 0; i < len; i++)
                for (int j = 0; j < len; j++)
                {
                    influenceMatrix[i][j] = 0;
                    changeMatrix[i][j] = 0;
                }

            //get the influence degree matrix4calc
            GetInfluenceDegreeMatrix(influenceMatrix, changeMatrix, consistence, emPrecise);

            int count = (len * (len - 1)) / 2;
            List<PosAndCr> influences = new List<PosAndCr>(count);
            for (int i = 0; i < len; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    PosAndCr pv = new PosAndCr(i, j, influenceMatrix[i][j], -1);
                    influences.Add(pv);
                }
            }

            // 影响越大排位越前
            influences.Sort((x, y) => y.Change.CompareTo(x.Change));

            return influences;
        }

        private void GetMaxInfluenceItem(out int s, out int t, out double adjustValue, double emPrecise)
        {
            int len = _matrix.GetLength(0);
            double[][] influenceMatrix = new double[len][];
            double[][] changeMatrix = new double[len][];
            int i = 0;
            int j = 0;
            int maxS = 0;
            int maxT = 0;
            double maxValue = 0;

            double consistence = CalWeightinessPower(emPrecise);
            //init temp matrix4calc
            for (; i < len; i++)
            {
                influenceMatrix[i] = new double[len];
                changeMatrix[i] = new double[len];
            }

            for (i = 0; i < len; i++)
                for (j = 0; j < len; j++)
                {
                    influenceMatrix[i][j] = 0;
                    changeMatrix[i][j] = 0;
                }

            //get the influence degree matrix4calc
            GetInfluenceDegreeMatrix(influenceMatrix, changeMatrix, consistence, emPrecise);
            //get the max influence item
            for (i = 0; i < len; i++)
                for (j = 0; j < len; j++)
                {
                    if (influenceMatrix[i][j] > maxValue)
                    {
                        maxValue = influenceMatrix[i][j];
                        maxS = i;
                        maxT = j;
                    }
                }
            s = maxS;
            t = maxT;
            adjustValue = CalAdjustValue(maxS, maxT, len);
            return;
        }

        /// <summary>
        /// Optimal Direction
        /// </summary>
        /// <param name="emPrecise">precise of power method</param>
        /// <param name="optimalDirectionUsingOriMatrix">adjust by original matrix</param>
        /// <param name="cr">CR of adjusted PCM</param>
        /// <returns>Failed: -1, otherwise: The times of adjust</returns>
        public int AdjustOptimalDirection(double emPrecise, bool optimalDirectionUsingOriMatrix, out double cr)
        {
            if (null == _matrix)
            {
                cr = 0;
                return -1;
            }

            int len = _matrix.GetLength(0);

            double consistence;
            int s, t;
            double adjustValue;

            int iterativeCount = 0;

            List<PosAndCr> itemsSortedByInfluence = null;
            if (optimalDirectionUsingOriMatrix)
                itemsSortedByInfluence = GetSortedItemInfluence();

            do
            {
                iterativeCount++;
                if (iterativeCount > Math.Ceiling(len * (len - 1) / 2 * 0.2))
                {
                    cr = 0;
                    return -1;
                }

                if (optimalDirectionUsingOriMatrix)
                {

                    s = itemsSortedByInfluence[iterativeCount - 1].Row;
                    t = itemsSortedByInfluence[iterativeCount - 1].Column;
                    adjustValue = CalAdjustValue(s, t, len);
                }
                else
                {
                    GetMaxInfluenceItem(out s, out t, out adjustValue, emPrecise);
                }

                double realAdjustValue;
                if (adjustValue - _ori_matrix4calc[s][t] > 0) // 向大的方向修正了
                    realAdjustValue = EvenAnd129Converter.NearestLevelOfValue(adjustValue, true);
                else
                    realAdjustValue = EvenAnd129Converter.NearestLevelOfValue(adjustValue, false);

                _matrix[s][t] = realAdjustValue;
                _matrix[t][s] = (double)1 / realAdjustValue;


                consistence = CalWeightinessPower(emPrecise);
                if (consistence < 0.1)
                    break;
            } while (true);

            cr = consistence;
            return iterativeCount;
        }

        /// <summary>
        /// Minimum Change
        /// </summary>
        /// <param name="paras">The paras 4 PSO.</param>
        /// <param name="avgChange">OUTPUT: The avg change of adjusted items.</param>
        /// <returns>CR of adjusted PCM</returns>
        public double AdjustMinimumChange(PsoArgs paras, out double avgChange)
        {
            int precision = paras.AdjustPrecise;
            double varianceCoefficient = paras.VarianceCoefficient;
            double range = paras.Range;

            double val_max = range;
            double val_min = -1.0 * range;

            int len = _matrix.GetLength(0);
            int dimense = len;

            int i = 0;
            int j = 0;

            double[][] matrix = null;
            matrix = new double[dimense][];
            for (i = 0; i < dimense; i++)
                matrix[i] = new double[dimense];

            double[] wi = new double[len];

            double[][] matrixIn = new double[len][];
            for (i = 0; i < len; i++)
                matrixIn[i] = new double[len];
            for (i = 0; i < len; i++)
                for (j = 0; j < len; j++)
                    matrixIn[i][j] = _matrix[i][j];

            double cr = PsoAhp.MatrixPsoAdjust(len, matrixIn, val_max, val_min, wi, precision, varianceCoefficient,
                paras.C1, paras.C2, paras.MaxConsistenceScale);

            // avg and std of changes
            int changedCount = 0;
            double changedSum = 0;
            for (i = 0; i < len; i++)
                for (j = 0; j < len; j++)
                {
                    if (_ori_matrix4calc[i][j] > 1.0)
                    {
                        changedCount++;
                        changedSum += Math.Abs(_ori_matrix4calc[i][j] - matrixIn[i][j]);
                    }
                }

            avgChange = changedSum / changedCount;

            for (i = 0; i < len; i++)
            for (j = 0; j < len; j++)
                _matrix[i][j] = matrixIn[i][j];

            #region Debug

            if (cr > 0.1001)
            {
                int stub = 0;
            }
            #endregion Debug

            return cr;
        }
    }
}
