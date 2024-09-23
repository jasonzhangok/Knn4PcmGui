namespace Lof4PcmGui.Pso
{
    public class PsoAhpAdjustFillData
    {
        /// <summary>
        /// PSO计算判断矩阵ci时的精度
        /// </summary>
        public int InnerPrecision { get; set; }

        /// <summary>
        /// 适应函数中的方差系数。此值越大，适应值函数中方差所占重要性越大，一般小于0.5。
        /// </summary>
        public double VarianceCoefficient { get; set; }

        /// <summary>
        ///PSO计算判断矩阵ci时的c1 
        /// </summary>
        public double InnerPsoC1 { get; set; }

        /// <summary>
        /// PSO计算判断矩阵ci时的c2
        /// </summary>
        public double InnerPsoC2 { get; set; }
        // End of 计算判断矩阵ci时使用的内部数据

        /// <summary>
        /// 判断矩阵阶数
        /// </summary>
        public int DimenseMatrix { get; set; }

        /// <summary>
        /// 优化问题维数，等于dimense_matrix * (dimense_matrix-1) / 2
        /// </summary>
        public int DimenseOptimize { get; set; }

        /// <summary>
        /// 数组，存放判断矩阵中的位置值(判读矩阵中这些位置对应的值大于1)及该位置对应的值
        /// size为dimense_optimize
        /// </summary>
        public PositionOfMatrix[] Position { get; set; }

        /// <summary>
        /// 未调整的判断矩阵数据，完整保存
        /// </summary>
        public double[][] MatrixData { get; set; }

    }
}
