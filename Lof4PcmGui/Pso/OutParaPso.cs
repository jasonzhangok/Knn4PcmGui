namespace Lof4PcmGui.Pso
{
    public class OutParaPso
    {
        /// <summary>
        /// 问题维数，主要用于确定best_position数组的大小
        /// </summary>
        public int Dimense { get; set; }

        /// <summary>
        /// 最优适应值
        /// </summary>
        public double BestFitness { get; set; }

        /// <summary>
        /// 最优适应值对应的粒子
        /// </summary>
        public double[] BestParticle { get; set; }

    }
}
