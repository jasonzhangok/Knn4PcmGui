using System;
using System.Collections.Generic;

namespace Lof4PcmGui.Lof4Pcm
{
    public class PcmGenerator
    {
        private static double[] _scales = new[]
        {
            1.0/9.0, 1.0/8.0,
            1.0/7.0, 1.0/6.0,
            1.0/5.0, 1.0/4.0,
            1.0/3.0, 1.0/2.0,
            1.0, 2.0, 3.0,
            4.0, 5.0, 6.0,
            7.0, 8.0, 9.0
        };

        private static double[] _scalesWithoutOne = new[]
        {
            1.0/9.0, 1.0/8.0,
            1.0/7.0, 1.0/6.0,
            1.0/5.0, 1.0/4.0,
            1.0/3.0, 1.0/2.0,
            2.0, 3.0,
            4.0, 5.0, 6.0,
            7.0, 8.0, 9.0
        };

        /// <summary>
        /// 生成完全一致判断矩阵
        /// </summary>
        /// <param name="dimense">矩阵阶数。</param>
        /// <param name="withTuning">是否加入[-0.5, 0.5]的微调。</param>
        /// <param name="canOffDiagonalEqualsOne">生成的不一致判断矩阵的非对角线元素是否可以为1.0。
        /// <returns>完全一致的判断矩阵。</returns>
        public static double[][] PerfectlyConsistent(int dimense,
            bool withTuning, bool canOffDiagonalEqualsOne = true)
        {
            double[][] matrix = new double[dimense][];
            for (int i = 0; i < dimense; i++)
            {
                matrix[i] = new double[dimense];
                matrix[i][i] = 1.0;
            }

            bool got = false;
            while (!got)
            {
                // 随机第一行数据
                List<int> rands;
                if (canOffDiagonalEqualsOne)
                    rands = ZRandomRNG.Int32RandMulti(0, _scales.Length - 1, dimense - 1);
                else
                    rands = ZRandomRNG.Int32RandMulti(0, _scalesWithoutOne.Length - 1, dimense - 1);

                for (int i = 1; i < dimense; i++)
                {
                    double tuning = 0.0;
                    if (withTuning)
                    {
                        tuning = ZRandomRNG.DoubleRandUniform(-0.5, 0.5, 1);

                        double evenValue;
                        if (canOffDiagonalEqualsOne)
                            evenValue = EvenAnd129Converter.GetEvenFrom129(_scales[rands[i - 1]]) + tuning;
                        else
                            evenValue = EvenAnd129Converter.GetEvenFrom129(_scalesWithoutOne[rands[i - 1]]) + tuning;

                        matrix[0][i] = EvenAnd129Converter.Get129FromEven(evenValue);
                        matrix[i][0] = 1.0 / matrix[0][i];
                    }
                    else
                    {
                        if (canOffDiagonalEqualsOne)
                            matrix[0][i] = _scales[rands[i - 1]];
                        else
                            matrix[0][i] = _scalesWithoutOne[rands[i - 1]];
                        matrix[i][0] = 1.0 / matrix[0][i];
                    }
                }

                bool falseData = false;
                for (int i = 1; i < dimense - 1; i++)
                {
                    for (int j = i + 1; j < dimense; j++)
                    {
                        matrix[i][j] = matrix[0][j] / matrix[0][i];
                        matrix[j][i] = 1.0 / matrix[i][j];

                        if (matrix[i][j] < 1.0 / 9.0 || matrix[i][j] > 9.0)
                        {
                            falseData = true;
                            break;
                        }
                    }
                    if (falseData)
                        break;
                }

                got = !falseData;
            }

            return matrix;
        }

        /// <summary>
        /// 生成一个特定阶数的、具有一定数量ufo并且不一致的判断矩阵。
        /// </summary>
        /// <param name="dimense">阶数</param>
        /// <param name="ufoCount">生成UFO项个数</param>
        /// <param name="minAdjust">最小调整等级。一般认为超过2个判断等级的调整脱离了决策者原有的决策，是UFO的</param>
        /// <param name="maxAdjust">最大调整等级。</param>
        /// <param name="perfectlyMatrix">输出参数，生成的矩阵对应的完全一致PCM</param>
        /// <param name="posAndCrs">输入输出参数，如果不为null，将输出各UFO项位置和CR</param>
        /// <returns>生成后的不一致判断矩阵</returns>
        public static double[][] UnconsistentUfo(int dimense, int ufoCount,
            int minAdjust, int maxAdjust,
            out double[][] perfectlyMatrix,
            ref List<PosAndCr> posAndCrs)
        {
            int times = 0;
            var matrix = PerfectlyConsistent(dimense, false);
            double[][] result;

            do
            {
                result = UnconsistentUfo(matrix, ufoCount, null, minAdjust, maxAdjust, ref posAndCrs);
                if (result != null && posAndCrs != null)
                {
                    if (posAndCrs[posAndCrs.Count - 1].Cr > 0.1)
                        break;
                }

                times++;
                if (times >= 100)
                {
                    // 100次如果都没有找到，返回null
                    posAndCrs = null;
                    perfectlyMatrix = null;
                    return null;
                }
            } while (true);

            perfectlyMatrix = matrix;
            return result;
        }

        /// <summary>
        /// 根据一个完全一致判断矩阵，生成具有一定数量ufo并且不一致的判断矩阵。
        /// </summary>
        /// <param name="perfectlyMatrix">完全一致的判断矩阵</param>
        /// <param name="ufoCount">生成UFO项个数</param>
        /// <param name="posOfAdjust">如果不为null，表示固定修改这个列表中确定的几个元素</param>
        /// <param name="minAdjust">最小修改判断等级。一般认为超过2个判断等级的调整脱离了决策者原有的决策，是UFO的</param>
        /// <param name="maxAdjust">最大修改判断等级。</param>
        /// <param name="posAndCrs">输入输出参数，如果不为null，将输出各UFO项位置和CR</param>
        /// <returns>生成后的不一致判断矩阵；null：失败</returns>
        public static double[][] UnconsistentUfo(double[][] perfectlyMatrix, int ufoCount,
            List<PosAndCr> posOfAdjust, int minAdjust, int maxAdjust,
            ref List<PosAndCr> posAndCrs)
        {
            List<PosAndCr> pcs = new List<PosAndCr>();

            int ufoAdjustCount = ufoCount;
            if (posOfAdjust != null)
                ufoAdjustCount = posOfAdjust.Count;

            int dimense = perfectlyMatrix.Length;
            double[][] matrix = new double[dimense][];
            for (int i = 0; i < dimense; i++)
                matrix[i] = (double[])perfectlyMatrix[i].Clone();

            int ufoGeneratedCount = 0;
            while (ufoGeneratedCount < ufoAdjustCount)
            {
                int times = 0;
                while (true)
                {
                    times++;
                    if (times > 100)
                        return null;

                    int row;
                    int column;

                    if (posOfAdjust != null)
                    {
                        row = posOfAdjust[ufoGeneratedCount].Row;
                        column = posOfAdjust[ufoGeneratedCount].Column;
                    }
                    else
                    {
                        row = ZRandomRNG.Int32Rand(0, dimense - 1);
                        column = ZRandomRNG.Int32Rand(row + 1, dimense);
                    }

                    int timerInner = 0;
                    bool checkOk = false;
                    while (!checkOk)
                    {
                        timerInner++;
                        if (timerInner > 100)
                            return null;

                        bool hasSameRowAndColumn = false;
                        foreach (var pc in pcs)
                        {
                            if (row == pc.Row && column == pc.Column)
                            {
                                hasSameRowAndColumn = true;
                                break;
                            }
                        }

                        if (hasSameRowAndColumn)
                        {
                            checkOk = false;

                            // 重新随机row和column
                            row = ZRandomRNG.Int32Rand(0, dimense - 1);
                            column = ZRandomRNG.Int32Rand(row + 1, dimense);
                        }
                        else
                            checkOk = true;
                    }

                    int adjustLevel = ZRandomRNG.Int32Rand(minAdjust, maxAdjust);
                    double oriVal = matrix[row][column];

                    double evenLevelOri = EvenAnd129Converter.GetEvenFrom129(oriVal);

                    double leftAdjustedLevel = Math.Floor(evenLevelOri - adjustLevel);
                    double rightAdjustedLevel = Math.Ceiling(evenLevelOri + adjustLevel);

                    if (leftAdjustedLevel > -9 && rightAdjustedLevel < 9)
                    {
                        double leftVal19 = EvenAnd129Converter.Get129FromEven(leftAdjustedLevel);
                        matrix[row][column] = leftVal19;
                        Pcm pcmLeft = new Pcm(matrix);
                        double crLeft = pcmLeft.CalWeightinessPower(Consts.DoubleDelta);

                        double rightVal19 = EvenAnd129Converter.Get129FromEven(rightAdjustedLevel);
                        matrix[row][column] = rightVal19;
                        Pcm pcmRight = new Pcm(matrix);
                        double crRight = pcmRight.CalWeightinessPower(Consts.DoubleDelta);

                        if (ufoGeneratedCount + 1 >= ufoAdjustCount)  // 生成最后一个要素后，判断是否满足一致性
                        {
                            if (crLeft > 0.1 && crRight > 0.1)
                            {
                                // 随机取一个即可
                                int selector = ZRandomRNG.Int32Rand(0, 1);
                                if (selector == 0)
                                {
                                    matrix[row][column] = leftVal19;
                                    matrix[column][row] = 1.0 / leftVal19;

                                    Pcm pcm = new Pcm(matrix);
                                    double cr = -1.0;
                                    if (posAndCrs != null)
                                        cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                                    double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                                    PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                                    pcs.Add(pc);
                                }
                                else
                                {
                                    matrix[row][column] = rightVal19;
                                    matrix[column][row] = 1.0 / rightVal19;

                                    if (posAndCrs != null)
                                    {
                                        Pcm pcm = new Pcm(matrix);
                                        double cr = pcm.CalWeightinessPower(Consts.DoubleDelta);
                                        double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                                        PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                                        pcs.Add(pc);
                                    }
                                }
                            }
                            else if (crLeft > 0.1)
                            {
                                matrix[row][column] = leftVal19;
                                matrix[column][row] = 1.0 / leftVal19;

                                Pcm pcm = new Pcm(matrix);
                                double cr = -1.0;
                                if (posAndCrs != null)
                                    cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                                double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                                PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                                pcs.Add(pc);

                            }
                            else if (crRight > 0.1)
                            {
                                matrix[row][column] = rightVal19;
                                matrix[column][row] = 1.0 / rightVal19;
                                Pcm pcm = new Pcm(matrix);
                                double cr = -1.0;
                                if (posAndCrs != null)
                                    cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                                double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                                PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                                pcs.Add(pc);

                            }
                            else
                            {
                                matrix[row][column] = oriVal;
                                continue;
                            }
                        }
                        else
                        {
                            // 随机取一个即可，不需要考虑cr
                            int selector = ZRandomRNG.Int32Rand(0, 1);
                            if (selector == 0)
                            {
                                matrix[row][column] = leftVal19;
                                matrix[column][row] = 1.0 / leftVal19;
                                Pcm pcm = new Pcm(matrix);
                                double cr = -1.0;
                                if (posAndCrs != null)
                                    cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                                double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                                PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                                pcs.Add(pc);

                            }
                            else
                            {
                                matrix[row][column] = rightVal19;
                                matrix[column][row] = 1.0 / rightVal19;

                                Pcm pcm = new Pcm(matrix);
                                double cr = -1.0;
                                if (posAndCrs != null)
                                    cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                                double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                                PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                                pcs.Add(pc);

                            }
                        }
                    }
                    else if (leftAdjustedLevel > -9)
                    {
                        double leftVal19 = EvenAnd129Converter.Get129FromEven(leftAdjustedLevel);
                        matrix[row][column] = leftVal19;
                        Pcm pcmLeft = new Pcm(matrix);
                        double crLeft = pcmLeft.CalWeightinessPower(Consts.DoubleDelta);

                        if (ufoGeneratedCount + 1 >= ufoAdjustCount) // 生成最后一个要素后，判断是否满足一致性
                        {
                            if (crLeft > 0.1)
                            {
                                matrix[row][column] = leftVal19;
                                matrix[column][row] = 1.0 / leftVal19;

                                Pcm pcm = new Pcm(matrix);
                                double cr = -1.0;
                                if (posAndCrs != null)
                                    cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                                double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                                PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                                pcs.Add(pc);

                            }
                            else
                            {
                                matrix[row][column] = oriVal;
                                continue;
                            }
                        }
                        else
                        {
                            matrix[row][column] = leftVal19;
                            matrix[column][row] = 1.0 / leftVal19;
                            Pcm pcm = new Pcm(matrix);
                            double cr = -1.0;
                            if (posAndCrs != null)
                                cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                            double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                            PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                            pcs.Add(pc);

                        }

                    }
                    else if (rightAdjustedLevel < 9)
                    {
                        double rightVal19 = EvenAnd129Converter.Get129FromEven(rightAdjustedLevel);
                        matrix[row][column] = rightVal19;
                        Pcm pcmRight = new Pcm(matrix);
                        double crRight = pcmRight.CalWeightinessPower(Consts.DoubleDelta);

                        if (ufoGeneratedCount + 1 >= ufoAdjustCount) // 生成最后一个要素后，判断是否满足一致性
                        {
                            if (crRight > 0.1)
                            {
                                matrix[row][column] = rightVal19;
                                matrix[column][row] = 1.0 / rightVal19;
                                Pcm pcm = new Pcm(matrix);
                                double cr = -1.0;
                                if (posAndCrs != null)
                                    cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                                double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                                PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                                pcs.Add(pc);

                            }
                            else
                            {
                                matrix[row][column] = oriVal;
                                continue;
                            }
                        }
                        else
                        {
                            matrix[row][column] = rightVal19;
                            matrix[column][row] = 1.0 / rightVal19;
                            Pcm pcm = new Pcm(matrix);
                            double cr = -1.0;
                            if (posAndCrs != null)
                                cr = pcm.CalWeightinessPower(Consts.DoubleDelta);

                            double evenNewValue = EvenAnd129Converter.GetEvenFrom129(matrix[row][column]);
                            PosAndCr pc = new PosAndCr(row, column, Math.Abs(evenLevelOri - evenNewValue), cr);
                            pcs.Add(pc);

                        }
                    }
                    else
                    {
                        matrix[row][column] = oriVal;
                        continue;
                    }

                    break;
                }

                ufoGeneratedCount++;
            }

            posAndCrs = pcs;
            return matrix;
        }

        /// <summary>
        /// 生成一个各项调整不超过一定等级(maxBias)、一致性大于0.1的PCM。
        /// </summary>
        /// <param name="dimense">生成的PCM阶数</param>
        /// <param name="minCr">要求满足的最小CR。一般是0.1，但有时为了生成特例可以设定0.2等其他值</param>
        /// <param name="adjustBias">生成非UFO的不一致判断矩阵时，各项最大的调整范围</param>
        /// <param name="canOffDiagonalEqualsOne">生成的不一致判断矩阵的非对角线元素是否可以为1.0。
        /// 因为基于PSO的修正算法，认为1是不会出错的，所以不生成非对角线包含为1的矩阵</param>
        /// <param name="perfectlyMatrix">输出参数，生成的矩阵对应的完全一致PCM</param>
        /// <param name="timesTry">输出参数，生成时尝试的次数</param>
        /// <param name="cr">输出参数，生成的判断矩阵一致性比例</param>
        /// <param name="adjustAvg">输入输出参数，如果不小于0，则计算上三角中调整过的Item调整值的均值，保存在这个参数中</param>
        /// <returns>各项调整不超过一定等级(maxBias)、一致性大于0.1的PCM</returns>
        public static double[][] UnconsistentNoUfo(int dimense, double minCr, double adjustBias,
            bool canOffDiagonalEqualsOne,
            out double[][] perfectlyMatrix, out int timesTry, out double cr, ref double adjustAvg)
        {
            int times = 0;
            double adjAvg = 0.0;
            double crFinal;

            double[][] result = null;
            double[][] pMatrix = null;
            do
            {
                if (times > 10000)
                {
                    perfectlyMatrix = null;
                    timesTry = 0;
                    cr = 0;
                    adjAvg = 0.0;
                    return null;
                }

                var matrix = PerfectlyConsistent(dimense, true);
                result = UnconsistentNoUfo(matrix, minCr, adjustBias,
                    true, canOffDiagonalEqualsOne, out crFinal, ref adjAvg);
                if (result != null)
                    pMatrix = matrix;
                times++;
            } while (result == null);

            perfectlyMatrix = pMatrix;
            timesTry = times;
            if (!(adjustAvg < 0))
                adjustAvg = adjAvg;

            cr = crFinal;
            return result;
        }

        /// <summary>
        /// 生成一个各项调整不超过一定等级(maxBias)、一致性大于特定值的PCM。
        /// </summary>
        /// <param name="perfectlyMatrix">完全一致的判断矩阵，在此基础上在2个判断等级内修改得到不一致的PCM</param>
        /// <param name="minCr">要求满足的最小CR。一般是0.1，但有时为了生成特例可以设定0.2等其他值</param>
        /// <param name="adjustBias">生成非UFO的不一致判断矩阵时，各项最大的调整范围</param>
        /// <param name="returnNullWhenLessThanMinCr">如果没有找到小于MinCr的矩阵，是否返回null。
        /// 被另一个重载的UnconsistentNoUfo调用生成NoUfo的不一致判断矩阵时，此参数为true；
        /// 被UnConsistentUfoWithBias调用生成各要素微调，但生成UFO的不一致判断矩阵是，此参数为false</param>
        /// <param name="canOffDiagonalEqualsOne">生成的不一致判断矩阵的非对角线元素是否可以为1.0。
        /// 因为基于PSO的修正算法，认为1是不会出错的，所以不生成非对角线包含为1的矩阵</param>
        /// <param name="cr">输出参数，生成的判断矩阵一致性比例</param>
        /// <param name="adjustAvg">输入输出参数，如果大于0，则计算上三角中调整过的Item调整值的均值，保存在这个参数中</param>
        /// <returns>各项调整不超过一定等级(maxBias)、一致性大于0.1的PCM</returns>

        private static double[][] UnconsistentNoUfo(double[][] perfectlyMatrix, double minCr, double adjustBias,
            bool returnNullWhenLessThanMinCr, bool canOffDiagonalEqualsOne, out double cr, ref double adjustAvg)
        {
            int dimense = perfectlyMatrix.Length;

            int times = 0;

            double[][] matrix = new double[dimense][];
            for (int i = 0; i < dimense; i++)
                matrix[i] = (double[])perfectlyMatrix[i].Clone();

            #region 用于第二轮调整
            // 标记调整是否向左(向小的方向)
            //bool[][] adjust2LeftFlag = new bool[dimense][];
            //for (int i = 0; i < dimense; i++)
            //{
            //    adjust2LeftFlag[i] = new bool[dimense];
            //    for (int j = 0; j < dimense; j++)
            //    {
            //        adjust2LeftFlag[i][j] = false;
            //    }
            //}
            #endregion 用于第二轮调整

            // 第一轮调整
            for (int i = 0; i < dimense - 1; i++)
            {
                for (int j = i + 1; j < dimense; j++)
                {
                    double adjustLevel = adjustBias;
                    double oriVal = matrix[i][j];

                    double evenLevel = EvenAnd129Converter.GetEvenFrom129(oriVal);
                    double leftAdjustedLevel = Math.Ceiling(evenLevel - adjustLevel);
                    double rightAdjustedLevel = Math.Floor(evenLevel + adjustLevel);

                    double leftVal19 = EvenAnd129Converter.Get129FromEven(leftAdjustedLevel);
                    matrix[i][j] = leftVal19;
                    matrix[j][i] = 1.0 / leftVal19;
                    Pcm pcmLeft = new Pcm(matrix);
                    double leftCr = pcmLeft.CalWeightinessPower(Consts.DoubleDelta);

                    double rightVal19 = EvenAnd129Converter.Get129FromEven(rightAdjustedLevel);
                    matrix[i][j] = rightVal19;
                    matrix[j][i] = 1.0 / rightVal19;
                    Pcm pcmRight = new Pcm(matrix);
                    double rightCr = pcmRight.CalWeightinessPower(Consts.DoubleDelta);

                    if (leftCr > rightCr)
                    {
                        matrix[i][j] = leftVal19;
                        matrix[j][i] = 1.0 / leftVal19;

                        #region 用于第二轮调整
                        //adjust2LeftFlag[i][j] = true;
                        //adjust2LeftFlag[j][i] = true;
                        #endregion 用于第二轮调整
                    }
                    else
                    {
                        matrix[i][j] = rightVal19;
                        matrix[j][i] = 1.0 / rightVal19;
                    }

                    if (!canOffDiagonalEqualsOne) // 非对角线元素不能为1
                    {
                        // 因为基于PSO的修正算法，认为1是不会出错的，所以不生成非对角线包含为1的矩阵
                        if (Math.Abs(matrix[i][j] - 1.0) < 0.00000001)
                        {
                            //matrix[i][j] += 0.01;   // 加上一个很小的变化，使其不等于1

                            cr = 0.0;
                            return null;
                        }
                    }
                }
            }

            Pcm pcm = new Pcm(matrix);
            double cr1 = pcm.CalWeightinessPower(Consts.DoubleDelta);
            if (cr1 <= minCr)
            {
                #region 第二轮调整
                //for (int i = 0; i < dimense - 1; i++)
                //{
                //    for (int j = i + 1; j < dimense; j++)
                //    {
                //        double adjustLevel = Consts.DoubleDelta / 2.0;
                //        double oriVal = matrix[i][j];
                //        double evenLevel = EvenAnd129Converter.GetEvenFrom129(oriVal);

                //        double adjustedLevel;
                //        if (adjust2LeftFlag[i][j]) // 第一次是向左调整，现在应该向右调整一个级别
                //            adjustedLevel = evenLevel + adjustLevel;
                //        else   // 第一次是向右调整，现在应该向左调整一个级别
                //            adjustedLevel = evenLevel - adjustLevel;


                //        double val19 = EvenAnd129Converter.Get129FromEven(adjustedLevel);
                //        matrix[i][j] = val19;
                //        matrix[j][i] = 1.0 / val19;
                //        Pcm pcm2 = new Pcm(matrix);
                //        double cr = pcm2.CalWeightinessPower(Consts.DoubleDelta);


                //        if (cr > cr1)
                //        {
                //            matrix[i][j] = val19;
                //            matrix[j][i] = 1.0 / val19;

                //            cr1 = cr;
                //        }
                //        else
                //        {
                //            matrix[i][j] = oriVal;
                //            matrix[j][i] = 1.0 / oriVal;
                //        }
                //    }
                //}

                //pcm = new Pcm(matrix);
                //double cr2 = pcm.CalWeightinessPower(Consts.DoubleDelta);
                //if (cr2 <= 0.1)
                //{
                //    adjustTwice = true;
                //    return null;
                //}
                //else
                //{
                //    adjustTwice = true;
                //    return matrix;
                //}
                #endregion 第二轮调整

                if (returnNullWhenLessThanMinCr)
                {
                    cr = cr1;
                    return null;
                }
                else
                {
                    if (!(adjustAvg < 0))
                    {
                        double sum = 0;
                        // 计算上三角元素较原始完全一致矩阵的修改值
                        for (int i = 0; i < dimense - 1; i++)
                        {
                            for (int j = i + 1; j < dimense; j++)
                            {
                                var newVal = EvenAnd129Converter.GetEvenFrom129(matrix[i][j]);
                                var oldVal = EvenAnd129Converter.GetEvenFrom129(perfectlyMatrix[i][j]);
                                sum += Math.Abs(newVal - oldVal);
                            }
                        }
                        double avg = sum / (double)(dimense * (dimense - 1));
                        adjustAvg = avg;

                    }

                    cr = cr1;
                    return matrix;
                }
            }
            else
            {
                if (!(adjustAvg < 0))
                {
                    double sum = 0;
                    // 计算上三角元素较原始完全一致矩阵的修改值
                    for (int i = 0; i < dimense - 1; i++)
                    {
                        for (int j = i + 1; j < dimense; j++)
                        {
                            var newVal = EvenAnd129Converter.GetEvenFrom129(matrix[i][j]);
                            var oldVal = EvenAnd129Converter.GetEvenFrom129(perfectlyMatrix[i][j]);
                            sum += Math.Abs(newVal - oldVal);
                        }
                    }
                    double avg = sum / (double)(dimense * (dimense - 1));
                    adjustAvg = avg;

                }
                cr = cr1;
                return matrix;
            }
        }

        /// <summary>
        /// 生成一个各项调整不超过一定等级(maxBias)、一致性大于0.1的PCM。
        /// </summary>
        /// <param name="dimense">生成的PCM阶数</param>
        /// <param name="perfectlyMatrixWithTuning">完全一致判断矩阵生成时，是否加入[-0.5, 0.5]的微调。</param>
        /// <param name="minCr">要求满足的最小CR。一般是0.1，但有时为了生成特例可以设定0.2等其他值</param>
        /// <param name="noUfoMaxCr">微调生成的非UFO一致性比例最大值</param>
        /// <param name="adjustBias">生成非UFO的不一致判断矩阵时，各项最大的调整范围</param>
        /// <param name="canOffDiagonalEqualsOne">生成的不一致判断矩阵的非对角线元素是否可以为1.0。
        /// 因为基于PSO的修正算法，认为1是不会出错的，所以不生成非对角线包含为1的矩阵</param>
        /// <param name="ufoCount">生成UFO项个数</param>
        /// <param name="minAdjust">最小修改判断等级。一般认为超过2个判断等级的调整脱离了决策者原有的决策，是UFO的</param>
        /// <param name="maxAdjust">最大修改判断等级。</param>
        /// <param name="perfectlyMatrix">输出参数，生成的矩阵对应的完全一致PCM</param>
        /// <param name="cr">输出参数，生成的判断矩阵一致性比例</param>
        /// <param name="posAndCrs">输入输出参数，如果不为null，将输出各UFO项位置和CR</param>
        /// <returns>各项调整不超过一定等级(maxBias)、一致性大于0.1的PCM</returns>
        public static double[][] UnConsistentUfoWithBias(int dimense, bool perfectlyMatrixWithTuning,
            double minCr, double noUfoMaxCr, double adjustBias, bool canOffDiagonalEqualsOne,
            int ufoCount, int minAdjust, int maxAdjust,
            out double[][] perfectlyMatrix, out double cr,
            ref List<PosAndCr> posAndCrs)
        {
            int times = 0;
            double adjAvg = 0.0;
            double crFinal;

            double[][] noUfoMatrix = null;
            double[][] pMatrix = null;
            double[][] result = null;

            do
            {
                if (times >= 25)
                {
                    // 100次如果都没有找到，返回null
                    perfectlyMatrix = null;
                    cr = 0.0;
                    posAndCrs = null;
                    return null;
                }

                var matrix = PerfectlyConsistent(dimense, perfectlyMatrixWithTuning, false);    // 非对角线元素不能为1
                noUfoMatrix = UnconsistentNoUfo(matrix, minCr, adjustBias, false, canOffDiagonalEqualsOne, out crFinal, ref adjAvg);
                if (noUfoMatrix == null) // 只可能是非对角线存在1的元素
                    continue;

                if (crFinal < noUfoMaxCr) // 非UFO微调的矩阵满足要求
                {
                    pMatrix = matrix;
                    break;
                }

                times++;
            } while (true);

            perfectlyMatrix = pMatrix;

            times = 0;
            List<PosAndCr> pos2Adjust = new List<PosAndCr>();
            do
            {
                for (int i = 0; i < ufoCount; i++)
                {
                    var row = ZRandomRNG.Int32Rand(0, dimense - 1);
                    var column = ZRandomRNG.Int32Rand(row + 1, dimense);

                    // 这个选中的位置，值恢复完全一致判断矩阵相应的值
                    noUfoMatrix[row][column] = perfectlyMatrix[row][column];

                    pos2Adjust.Add(new PosAndCr(row, column, 0.0, 0.0));
                }

                result = UnconsistentUfo(noUfoMatrix, ufoCount, pos2Adjust,
                    minAdjust, maxAdjust, ref posAndCrs);

                if (result != null && posAndCrs != null)
                {
                    if (posAndCrs[posAndCrs.Count - 1].Cr > 0.1)
                        break;
                }

                times++;
                if (times >= 100)
                {
                    // 100次如果都没有找到，返回null
                    perfectlyMatrix = null;
                    cr = 0.0;
                    posAndCrs = null;
                    return null;
                }
            } while (true);

            cr = posAndCrs[posAndCrs.Count - 1].Cr;
            return result;
        }
    }
}
