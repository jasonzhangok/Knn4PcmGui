using System;
using Lof4PcmGui.Lof4Pcm;

namespace Lof4PcmGui.Pso
{
    public class PsoAhp
    {
        private const int MAX_PRECISION = 5;
        private const int MIN_PRECISION = 1;

        private const int PRECISION1 = 1;
        private const int PRECISION2 = 2;
        private const int PRECISION3 = 3;
        private const int PRECISION4 = 4;
        private const int PRECISION5 = 5;


        private const int FILL_THRESHOLD = 9999;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dimense">判断矩阵阶数</param>
        /// <param name="matrix_in">二维dimense * dimense的double数组，调整前不一致的判断矩阵数据，inout参数</param>
        /// <param name="val_max">取值范围最大值（调整时的改变量，与标度有关，例如1~9标度该值为1）</param>
        /// <param name="val_min">取值范围最小值（调整时的改变量，与标度有关，例如1~9标度该值为-1）</param>
        /// <param name="weights">输出参数，计算得到的排序权重</param>
        /// <param name="precision">计算精度，根据判断矩阵阶数及计算精度获得pso的参数，分5级，从1-5</param>
        /// <param name="varianceCoefficient">适应函数中的方差系数。此值越大，适应值函数中方差所占重要性越大，一般小于0.5</param>
        /// <param name="pso_c1">修正过程中PSO的c1参数</param>
        /// <param name="pso_c2">修正过程中PSO的c2参数</param>
        /// <param name="break_condition">调整时的PSO结束条件，也就是当判断矩阵的一致性比例小于此值，结束PSO迭代</param>
        /// <returns>调整后的判断矩阵CI</returns>
        public static double MatrixPsoAdjust(int dimense, double[][] matrix_in, double val_max, double val_min,
            double[] weights, int precision, double varianceCoefficient, double pso_c1, double pso_c2, double break_condition)
        {
            #region main
            InParaPso in_para = new InParaPso();    // pso入口参数
            OutParaPso result = null;
            int generation = 0;     // pso计算完成后这个变量保存进化代数
            double final_fitness = 0;   // 最终的适应值，即返回值CI
            double sum = 0;
            double[] matrix_temp = null;
            int adjust_positon_num = 0; // 需调整的位置计数
            int i = 0;
            int j = 0;

            PsoAhpAdjustFillData ahp_dt = null;

            in_para.C1 = pso_c1;
            in_para.C2 = pso_c2;
            in_para.MaxNoImproveStop = true;    // 达到最到无进化代数后停止，不变异
            in_para.ValMax = val_max;
            in_para.ValMin = val_min;

            in_para.SpdMax = (val_max - val_min) / (double)10; // 1-9标度，pso中的最大速度设置为0.2
            in_para.FitnessFun = FitnessAhpAdjustMinChange;    // 计算最小改变的适应值函数为FitnessAhpAdjustMinChange

            in_para.UseBreakCondition = false;    // 使用进化结束条件
            in_para.BreakCondition = break_condition;  // 进化结束条件为一致性比例小于0.1

            // 定义适应值函数数据参数，此处应该传入判断矩阵
            ahp_dt = new PsoAhpAdjustFillData();
            ahp_dt.MatrixData = matrix_in;

            // PSO计算判断矩阵ci时的精度
            ahp_dt.InnerPrecision = precision;

            // 适应值函数计算时的方差系数
            ahp_dt.VarianceCoefficient = varianceCoefficient;

            // PSO计算判断矩阵ci时的c1
            ahp_dt.InnerPsoC1 = pso_c1;
            // PSO计算判断矩阵ci时的c2
            ahp_dt.InnerPsoC2 = pso_c2;

            // 获得判断矩阵中非对角线大于等于1的所有位置，对这些位置进行调整
            // 统计需要调整的位置
            adjust_positon_num = 0;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 不对判断矩阵非对角线元素为1的情况进行调整
            // 首先统计需要进行调整的元素，与in_para.dimense = ( dimense * (dimense-1) ) / 2可能不等，如果在非对角线位置上有1的元素
            // Modified 15/11/2011, Zhang Jianhua
            // 互反判断矩阵中，因为同等重要（只考虑为1的情况）的概念判断一般不会出错，而且对1的调整左右不对称
            // 互补判断矩阵中，只处理上三角阵，需要处理的变量个数为 ( dimense * (dimense-1) ) / 2
            // 如果相同重要程度对应值不为1，也就是scale_type为EQUALS_NOT_ONE
            // 那么与互补矩阵类似，只处理上三角阵，需要处理的变量个数为 ( dimense * (dimense-1) ) / 2
            for (i = 0; i < dimense; i++)
                for (j = 0; j < dimense; j++)
                {
                    if (matrix_in[i][j] > 1.01)   // 生成测试数据时，为了不在非对角线上生成=1的项，将=1改成了1.01，所以用>1.01这里忽略掉
                        adjust_positon_num++;
                }


            // 分配需调整位置数组空间
            ahp_dt.Position = new PositionOfMatrix[adjust_positon_num];
            for (i = 0; i < adjust_positon_num; i++)
                ahp_dt.Position[i] = new PositionOfMatrix();

            //只处理大于1的情况，所以adjust_positon_num并不等于计算得到的上三角阵元素个数，即 in_para.dimense = ( dimense * (dimense-1) ) / 2;
            in_para.Dimense = adjust_positon_num; // 此优化问题维数

            // 根据判断矩阵维数确定pso参数
            in_para.PopNumber = AdjustGetPopNumber(in_para.Dimense, precision); // 种群规模
            in_para.MaxGen = AdjustGetMaxGen(in_para.Dimense, precision);       // 最大进化代数
            in_para.MaxNoImprove = in_para.MaxGen;   // 最大无进化代数

            ahp_dt.DimenseMatrix = dimense;
            ahp_dt.DimenseOptimize = in_para.Dimense;

            //判断调整位置以此矩阵为依据
            // 根据判断矩阵的上三角矩阵进行统计
            adjust_positon_num = 0;
            for (i = 0; i < dimense - 1; i++)
                for (j = i + 1; j < dimense; j++)
                {
                    if (matrix_in[i][j] > 1.01) // 生成测试数据时，为了不在非对角线上生成=1的项，将=1改成了1.01，所以用>1.01这里忽略掉
                    {   // 标记其为需要调整的位置，只处理大于1的情况
                        ahp_dt.Position[adjust_positon_num].Row = i;
                        ahp_dt.Position[adjust_positon_num].Column = j;
                        adjust_positon_num++;
                    }
                    else if (matrix_in[i][j] < 1)
                    {   // 标记其需要调整的位置为其对角线镜像位置
                        ahp_dt.Position[adjust_positon_num].Row = j;
                        ahp_dt.Position[adjust_positon_num].Column = i;
                        adjust_positon_num++;
                    }
                    // 非对角线元素，等于1的也不处理
                }
            // 调整判断矩阵非对角线元素为1的情况，这种情况调整起来可能不合理！！！
            ////////////////////////////////////////////////////////////////////////////////////////////////////////


            in_para.FitnessData = ahp_dt;  // 适应值函数data
                                           // Modified 15/11/2011, Zhang Jianhua. Need not
                                           // begin_pso(pso_c1,pso_c2);	// pso开始

            Pso pso = new Pso(in_para);
            generation = pso.Start();
            result = pso.Result;
            //generation = pso(in_para, out result, ris); // pso迭代进化

            // 用matrix_in
            // 根据pso历史最优位置结果，对matrix_out附新值
            for (i = 0; i < in_para.Dimense; i++)
            {
                //对每一个调整的位置
                var tmp = matrix_in[ahp_dt.Position[i].Row][ahp_dt.Position[i].Column] + result.BestParticle[i];
                if (tmp < 1)    // 原始值一定大于1，所以不应该修正到了1以下
                    tmp = 1;
                matrix_in[ahp_dt.Position[i].Row][ahp_dt.Position[i].Column] = tmp;

                //matrix_in[ahp_dt.Position[i].Row][ahp_dt.Position[i].Column] = 
                //    matrix_in[ahp_dt.Position[i].Row][ahp_dt.Position[i].Column] + result.BestParticle[i];
                    
                // 相应地修改对角线镜像位置
                matrix_in[ahp_dt.Position[i].Column][ahp_dt.Position[i].Row] =
                    1.0 / matrix_in[ahp_dt.Position[i].Row][ahp_dt.Position[i].Column];
            }

            // 计算最终fitness
            // 用EM方法计算判断矩阵一致性比例
            final_fitness = AhpCalcInPso(ahp_dt.DimenseMatrix, matrix_in);

            ahp_dt.MatrixData = null;
            return final_fitness;

            #endregion main
        }

        private static double FitnessAhpAdjustMinChange(int dimense, double[] vals, PsoAhpAdjustFillData data)
        {
            double avg = 0;
            double std_var = 0;
            double sum = 0;
            double tmp = 0;
            int i = 0;
            double ci_of_em = 0;
            double punish = 0;
            PsoAhpAdjustFillData dt = data;

            // 各优化量绝对值累和
            for (; i < dimense; i++)
            {
                if (vals[i] >= 0)
                    tmp = vals[i];
                else
                    tmp = -1 * vals[i];
                sum += tmp;
            }
            // 各优化量绝对值均值
            avg = sum / dimense;

            // 各优化量绝对值标准差
            for (i = 0, sum = 0; i < dimense; i++)
            {
                if (vals[i] >= 0)
                    tmp = vals[i];
                else
                    tmp = -1 * vals[i];
                sum += (tmp - avg) * (tmp - avg);
            }
            std_var = Math.Sqrt(sum / dimense);

            // 计算修正后矩阵的适应值
            ci_of_em = FitnessAhpAdjustMinCi(dimense, vals, data);
            if (ci_of_em > 0.1)
                punish = 100 * (ci_of_em - 0.1); ;    // 10 * ci_of_em;
            return avg + data.VarianceCoefficient * std_var + punish;
        }

        private static double FitnessAhpAdjustMinCi(int dimense, double[] vals, PsoAhpAdjustFillData data)
        {
            PsoAhpAdjustFillData dt = data;
            double[][] matrix;
            double ci = 0;
            int i = 0;
            int j = 0;
            double sum1 = 0;
            double sum2 = 0;

            // 首先装配新的判断矩阵，根据data中的原始判断矩阵、需要调整的位置以及vals中的值获得
            // 分配新的判断矩阵
            matrix = new double[dt.MatrixData.Length][];
            for (i = 0; i < dt.MatrixData.Length; i++)
                matrix[i] = new double[dt.MatrixData.Length];

            // copy原始矩阵
            for (i = 0; i < dt.MatrixData.Length; i++)
            {
                for (j = 0; j < dt.MatrixData.Length; j++)
                {
                    matrix[i][j] = dt.MatrixData[i][j];
                }
            }

            for (i = 0; i < dt.DimenseOptimize; i++)
            { // 为每一个位置，修改判断矩阵
                matrix[dt.Position[i].Row][dt.Position[i].Column] = matrix[dt.Position[i].Row][dt.Position[i].Column] + vals[i];

                matrix[dt.Position[i].Column][dt.Position[i].Row] = 1.0 / matrix[dt.Position[i].Row][dt.Position[i].Column];
            }

            // 计算判断矩阵一致性比例
            // 用EM方法计算判断矩阵一致性比例
            ci = AhpCalcInPso(dt.DimenseMatrix, matrix);

            return ci;
        }


        /// <summary>
        /// 根据判断矩阵阶数和精度，确定计算ci所需的种群规模
        /// </summary>
        /// <param name="dimense">维数</param>
        /// <param name="precision">精度</param>
        /// <returns>种群规模</returns>
        private static int AdjustGetPopNumber(int dimense, int precision)
        {
            int _precision = precision;
            if (precision < MIN_PRECISION)
                _precision = MIN_PRECISION;
            if (precision > MAX_PRECISION)
                _precision = MAX_PRECISION;

            int num = 5 * dimense * _precision;
            if (num > 300)
                num = 300;
            return num;
        }

        /// <summary>
        /// 根据判断矩阵阶数和精度，确定计算ci所需的最大进化代数
        /// </summary>
        /// <param name="dimense">维数</param>
        /// <param name="precision">精度</param>
        /// <returns>最大进化代数</returns>
        private static int AdjustGetMaxGen(int dimense, int precision)
        {
            int _precision = precision;
            if (precision < MIN_PRECISION)
                _precision = MIN_PRECISION;
            if (precision > MAX_PRECISION)
                _precision = MAX_PRECISION;

            int num = 10 * dimense * _precision;
            if (num > 500)
                num = 500;
            return num;
        }

        /// <summary>
        /// 特征值法计算判断矩阵排序权重及一致性比例
        /// </summary>
        /// <param name="dimense">判断矩阵阶数</param>
        /// <param name="matrix">判断矩阵数据，dimense*dimense二维数组</param>
        /// <returns>判断矩阵的一致性比例</returns>
        private static double AhpCalcInPso(int dimense, double[][] matrix)
        {
            double[] wi = new double[dimense];
            double[] mi = new double[dimense];

            int i = 0;
            int j = 0;
            double sum = 0;
            double max = 0;
            double ri = 0;
            double ci = 0;

            // 判断矩阵按列归一化
            double[] colSums = new double[dimense];
            double[][] genMatrix = new double[dimense][];
            for (i = 0; i < dimense; i++)
                genMatrix[i] = new double[dimense];

            for (i = 0; i < dimense; i++)
                colSums[i] = 0;
            for (i = 0; i < dimense; i++)
                for (j = 0; j < dimense; j++)
                    colSums[j] += matrix[i][j];

            for (i = 0; i < dimense; i++)
                for (j = 0; j < dimense; j++)
                    genMatrix[i][j] = matrix[i][j] / colSums[j];

            for (i = 0; i < dimense; i++)
                wi[i] = 0;
            for (i = 0; i < dimense; i++)
                for (j = 0; j < dimense; j++)
                    wi[i] += genMatrix[i][j];

            //wis and its sum is ok, calculate the weightiness scale
            for (i = 0; i < dimense; i++)
                sum += wi[i];

            for (i = 0; i < dimense; i++)
            {
                for (j = 0, sum = 0; j < dimense; j++)
                    sum += matrix[i][j] * wi[j];
                mi[i] = sum / wi[i];
            }


            for (i = 0; i < dimense; i++)
                max += mi[i];
            max /= dimense;

            ci = (double)(max - dimense) / (double)(dimense - 1);
            ri = Consts.GetRiByN(dimense);

            if (ri == 0)
                return 0.0000;
            if (ri == -1)   //failed, too many items
                return -1;

            return ci / ri;
        }
    }
}
