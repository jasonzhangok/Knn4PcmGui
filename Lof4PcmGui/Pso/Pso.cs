using Lof4PcmGui.Lof4Pcm;
using System;

namespace Lof4PcmGui.Pso
{
    public class Pso
    {
        private InParaPso _inPara;
        private OutParaPso _calcResult;

        public OutParaPso Result
        {
            get { return _calcResult; }
        }

        public Pso(InParaPso inpara)
        {
            _inPara = inpara;
        }

        /// <summary>
        /// 启动粒子群算法
        /// </summary>
        /// <returns>计算结果</returns>
        public int Start()
        {
            OutParaPso rst;  // 保存计算结果

            double[] popular;    // 种群粒子位置
            double[] speed;      //种群例子速度
            double[] current_fitness;    //size为in_para.pop_number（种群规模）
                                         // 保存各粒子当前适应值，对应popular中各粒子向量
            double[] pbest_pos;  //size为in_para.pop_number * in_para.dimense（种群规模*维数）
                                 // 保存各个粒子各自的历史最优位置
            double[] pbest;      // size为in_para.pop_number（种群规模）
                                 // 保存各粒子历史最优适应值，对应pbest_pos中各粒子向量
            double[] gbest_pos;  // size为in_para.dimense（维数），保存种群的（全局）历史最优位置
            double gbest;       // 种群（全局）历史最优适应值

            double[] array4fitness;  // 计算适应值时用到的向量，即该向量起始地址
                                     // 与维数配合使用即可获得该项量的值

            int current_gen = 0;
            int no_improve = 0;
            double mutation_range = _inPara.ValMax - _inPara.ValMin;
            double current_weight = 0;
            double r1 = 0;
            double r2 = 0;

            int i = 0;
            int j = 0;

            int best_index = 0;
            double temp = 0;

            // 初始化 pbest_pos
            current_fitness = new double[_inPara.PopNumber];
            // 初始化 pbest_pos
            pbest_pos = new double[_inPara.Dimense * _inPara.PopNumber];
            // 初始化 pbest
            pbest = new double[_inPara.PopNumber];
            // 初始化 gbest_pos
            gbest_pos = new double[_inPara.Dimense];

            // 初始化种群各粒子位置
            popular = InitPopular(_inPara.Dimense, _inPara.PopNumber, _inPara.ValMax, _inPara.ValMin);
            // 初始化种群各粒子速度为0
            speed = InitSpeed(true, _inPara.Dimense, _inPara.PopNumber, _inPara.SpdMax);

            // DEBUG
            // print_popular(in_para.dimense, in_para.pop_number, popular,speed);

            // 因为是第一次迭代，将初始种群粒子位置附给pbest_pos
            for (i = 0; i < _inPara.PopNumber; i++)
                for (j = 0; j < _inPara.Dimense; j++)
                    pbest_pos[i * _inPara.Dimense + j] = popular[i * _inPara.Dimense + j];

            // 计算初始种群各粒子适应值，同时将该值赋给pbest
            for (i = 0; i < _inPara.PopNumber; i++)
            {
                //array4fitness = popular + i * in_para.Dimense;
                array4fitness = new double[_inPara.Dimense];
                for (j = 0; j < _inPara.Dimense; j++)
                {
                    array4fitness[j] = popular[i * _inPara.Dimense + j];
                }
                current_fitness[i] = _inPara.FitnessFun(_inPara.Dimense, array4fitness, _inPara.FitnessData);
                pbest[i] = current_fitness[i];
            }

            // DEBUG
            // print_fitness(in_para.pop_number, current_fitness);

            // 第一次迭代，首先置gbest为一个很大的值
            gbest = Double.MaxValue;

            // 保留历史最优值，并记录下这个历史最优位置在current_fitness中的位置
            for (i = 0; i < _inPara.Dimense; i++)
            {
                if (pbest[i] < gbest)
                {
                    gbest = pbest[i];
                    best_index = i;
                }
            }
            // 保留历史最优位置
            for (i = 0; i < _inPara.Dimense; i++)
                gbest_pos[i] = pbest_pos[best_index * _inPara.Dimense + i];

            // 进化迭代
            for (current_gen = 1; current_gen < _inPara.MaxGen; current_gen++)
            {
                // DEBUG
                // printf("GBEST:%f\t",gbest);

                // 计算当前代惯性权重
                current_weight = GetWight(current_gen, _inPara.MaxGen);
                // 两个U(0,1)随机数
                r1 = ZRandomRNG.DoubleRandUniform(0.0, 1.0, 8);
                r2 = ZRandomRNG.DoubleRandUniform(0.0, 1.0, 8);
                for (i = 0; i < _inPara.PopNumber; i++)     // 对每一个粒子
                    for (j = 0; j < _inPara.Dimense; j++)
                    {  //对每一个粒子的每一维
                       // 计算下一时刻速度v(t+1)
                        temp = current_weight * speed[i * _inPara.Dimense + j]
                            + _inPara.C1 * r1 * (pbest_pos[i * _inPara.Dimense + j] - popular[i * _inPara.Dimense + j])
                            + _inPara.C2 * r2 * (gbest_pos[j] - popular[i * _inPara.Dimense + j]);

                        // 计算下一时刻位置x(t+1)
                        if (temp > _inPara.SpdMax)
                            temp = _inPara.SpdMax;
                        if (temp < (-1 * _inPara.SpdMax))
                            temp = -1 * _inPara.SpdMax;
                        // 记录v(t+1)
                        speed[i * _inPara.Dimense + j] = temp;
                        popular[i * _inPara.Dimense + j] = popular[i * _inPara.Dimense + j] + temp;
                        if (popular[i * _inPara.Dimense + j] > _inPara.ValMax)
                            popular[i * _inPara.Dimense + j] = _inPara.ValMax;
                        if (popular[i * _inPara.Dimense + j] < _inPara.ValMin)
                            popular[i * _inPara.Dimense + j] = _inPara.ValMin;

                        // DEBUG
                        // printf("Speed:%f\t",temp);
                        // printf("Position:%f\n",*(popular + i * in_para.dimense + j));
                    }
                // 计算各粒子适应值
                for (i = 0; i < _inPara.PopNumber; i++)
                {
                    array4fitness = new double[_inPara.Dimense];
                    for (j = 0; j < _inPara.Dimense; j++)
                    {
                        array4fitness[j] = popular[i * _inPara.Dimense + j];
                    }

                    current_fitness[i] = _inPara.FitnessFun(_inPara.Dimense, array4fitness, _inPara.FitnessData);
                }
                // 设置pbest_pos及pbest
                for (i = 0; i < _inPara.PopNumber; i++)
                {
                    // 为每个粒子，计算其历史最优值
                    if (current_fitness[i] < pbest[i])
                    {
                        // 赋值粒子的历史最优值
                        pbest[i] = current_fitness[i];
                        // 复制该粒子的历史最优位置
                        for (j = 0; j < _inPara.Dimense; j++)
                            pbest_pos[i * _inPara.Dimense + j] = popular[i * _inPara.Dimense + j];
                    }
                }

                // 设置gbest_pos及gbest
                // 保留历史最优值，并记录下这个历史最优位置在current_fitness中的位置
                for (i = 0, best_index = -1; i < _inPara.PopNumber; i++)
                {
                    if (pbest[i] < gbest)
                    {
                        gbest = pbest[i];
                        best_index = i;
                    }
                }
                // 保留历史最优位置
                if (best_index != -1)
                {      // gbest改变了
                    for (i = 0; i < _inPara.Dimense; i++)
                        gbest_pos[i] = pbest_pos[best_index * _inPara.Dimense + i];
                    no_improve = 0; // 重置最大无进化代数
                }
                else
                    no_improve++;   // 最大无进化代数加一

                if (_inPara.UseBreakCondition)
                { // 使用结束条件
                    if (gbest < _inPara.BreakCondition)
                        break;
                }

                // 如果最大无进化代数达到，变异/退出
                if (no_improve > _inPara.MaxNoImprove)
                {
                    if (_inPara.MaxNoImproveStop)
                        break;
                    else
                    {
                        // DEBUG
                        // printf("MUTATE!\n");
                        // 计算变异范围，其值为取值范围的1/10
                        for (i = 0; i < _inPara.PopNumber; i++)
                            for (j = 0; j < _inPara.Dimense; j++)
                            {
                                temp = ZRandomRNG.DoubleRandUniform(0.0, 1.0, 8);
                                temp = temp * (mutation_range + mutation_range) - mutation_range;
                                popular[i * _inPara.Dimense + j] = popular[i * _inPara.Dimense + j] + temp;
                            }
                    }
                }
            }

            // 为计算结果分配内存
            rst = new OutParaPso();
            rst.Dimense = _inPara.Dimense;
            rst.BestFitness = gbest;
            // 为最优值位置分配内存
            rst.BestParticle = new double[_inPara.Dimense];     // (double*)malloc(in_para.dimense * sizeof(double));
            // 赋值最优值位置
            for (i = 0; i < _inPara.Dimense; i++)
                rst.BestParticle[i] = gbest_pos[i];
            // 赋值out参数以返回计算结果
            _calcResult = rst;

            return current_gen;

        }

        /// <summary>
        /// 初始化种群
        /// </summary>
        /// <param name="dimense">问题维数</param>
        /// <param name="pop_number">种群规模</param>
        /// <param name="val_max">各维取值的最大值</param>
        /// <param name="val_min">各维取值的最小值</param>
        /// <returns>一个二维double数组，pop_number行，dimense列，每一行对应一个粒子</returns>
        private double[] InitPopular(int dimense, int pop_number, double val_max, double val_min)
        {
            double[] p = null;
            int i = 0;
            int j = 0;

            p = new double[dimense * pop_number];   //为二维数组分配内存空间
            double[] rands = ZRandomRNG.DoubleRandMulti(dimense * pop_number, 8);
            for (i = 0; i < pop_number; i++)
                for (j = 0; j < dimense; j++)
                    p[i * dimense + j] = rands[i * dimense + j] * (val_max - val_min) + val_min;

            return p;
        }

        /// <summary>
        /// 初始化速度数组
        /// </summary>
        /// <param name="is_zero">是否设为全0速度</param>
        /// <param name="dimense">问题维数</param>
        /// <param name="pop_number">种群规模</param>
        /// <param name="spd_max">最大速度，最小速度为 -1.0 * 最大速度</param>
        /// <returns>一个二维double数组，pop_number行，dimense列，每一行对应一个粒子速度</returns>
        private double[] InitSpeed(bool is_zero, int dimense, int pop_number, double spd_max)
        {
            double spd_min = -1 * spd_max;
            double[] p = null;
            int i = 0;
            int j = 0;

            p = new double[dimense * pop_number];   //为二维数组分配内存空间

            if (is_zero)
            {
                for (i = 0; i < pop_number; i++)
                    for (j = 0; j < dimense; j++)
                    {
                        p[i * dimense + j] = 0;
                    }
            }
            else
            {
                double[] rands = ZRandomRNG.DoubleRandMulti(dimense * pop_number, 8);

                for (i = 0; i < pop_number; i++)
                    for (j = 0; j < dimense; j++)
                    {
                        p[i * dimense + j] = rands[i * dimense + j] * (spd_max - spd_min) + spd_min;
                    }
            }

            return p;
        }

        /// <summary>
        /// 输入数组，返回数组中最小的值
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns>数组中的最小值</returns>
        private double GetMin(double[] arrays)
        {
            double min = arrays[0];
            int i = 0;
            for (; i < arrays.Length; i++)
            {
                if (arrays[i] < min)
                    min = arrays[i];
            }
            return min;
        }

        /// <summary>
        /// 计算惯性权重
        /// </summary>
        /// <param name="current_gen">当前代数</param>
        /// <param name="max_gen">最大进化代数</param>
        /// <returns>根据当前代数和最大进化代数计算出的惯性权重。</returns>
        private double GetWight(int current_gen, int max_gen)
        {
            double temp = 0.9 - ((double)current_gen / (double)max_gen) * 0.5;
            return temp;
        }

    }
}
