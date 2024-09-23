namespace Lof4PcmGui.Pso
{
    /// <summary>
    /// 适应值函数delegate
    /// </summary>
    /// <param name="dimense">问题维数，对于计算判断矩阵排序权重和一致性比例，即为判断矩阵阶数</param>
    /// <param name="values">向量各个分量，对于计算判断矩阵排序权重和一致性比例，为权重</param>
    /// <param name="data">判断矩阵数据，PsoAhpAdjustFillData类型数据，包括判断矩阵维数及数据</param>
    /// <returns>计算判断矩阵的CI</returns>
    public delegate double FitnessFuncDelegate(int dimense, double[] values, PsoAhpAdjustFillData data);


    public class InParaPso
    {
        /// <summary>
        /// para c1
        /// </summary>
        public double C1 { get; set; }

        /// <summary>
        /// para c2
        /// </summary>
        public double C2 { get; set; }

        /// <summary>
        /// 问题维数
        /// </summary>
        public int Dimense { get; set; }

        /// <summary>
        /// 种群规模
        /// </summary>
        public int PopNumber { get; set; }

        /// <summary>
        /// 最大进化代数
        /// </summary>
        public int MaxGen { get; set; }

        /// <summary>
        /// 最大无进化代数
        /// </summary>
        public int MaxNoImprove { get; set; }

        /// <summary>
        /// 最大无进化代数达到后是否进化停止
        /// 1: 达到后停止；0: 达到后变异种群
        /// </summary>
        public bool MaxNoImproveStop { get; set; }

        /// <summary>
        /// 各维取值的最大值
        /// </summary>
        public double ValMax { get; set; }

        /// <summary>
        /// 各维取值的最小值 
        /// </summary>
        public double ValMin { get; set; }

        //	spd_max: 最大速度
        /// <summary>
        /// 最大速度
        /// </summary>
        public double SpdMax { get; set; }

        /// <summary>
        /// 是否使用结束条件
        /// </summary>
        public bool UseBreakCondition { get; set; }

        /// <summary>
        /// 结束条件，fitness小于这个值阶数进化迭代
        /// </summary>
        public double BreakCondition { get; set; }

        /// <summary>
        /// 适应值函数
        /// </summary>
        public FitnessFuncDelegate FitnessFun { get; set; }

        /// <summary>
        /// 适应值函数所需用到的数据
        /// </summary>
        public PsoAhpAdjustFillData FitnessData { get; set; }
    }
}
