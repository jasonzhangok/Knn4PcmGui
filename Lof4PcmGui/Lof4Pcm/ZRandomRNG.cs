using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Lof4PcmGui.Lof4Pcm
{
    public class ZRandomRNG
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        /// <summary>
        /// Gets the random numeric.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        public static int Int32Rand(int min, int max)
        {
            int m = max - min;
            int rnd = int.MinValue;
            decimal _base = (decimal)long.MaxValue;
            byte[] rndSeries = new byte[8];
            rngCsp.GetBytes(rndSeries);
            long l = BitConverter.ToInt64(rndSeries, 0);
            rnd = (int)(Math.Abs(l) / _base * m);
            return min + rnd;
        }

        public static List<int> Int32RandMulti(int min, int max, int count)
        {
            int num = 0;
            List<int> rstList = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                num = Int32Rand(min, max);
                rstList.Add(num);
            }
            return rstList;
        }

        public static double DoubleRandUniform(double min, double max, short precision)
        {
            int num = 0;
            int intMin = (int)(min * Math.Pow(10, (double)precision));
            int intMax = (int)(max * Math.Pow(10, (double)precision));

            num = Int32Rand(intMin, intMax);
            return (double)num / Math.Pow(10, (double)precision);
        }

        /// <summary>
        /// 产生num个[0,1]之间的double随机数
        /// </summary>
        /// <param name="num">数量</param>
        /// <param name="precision">小数点后的精度</param>
        /// <returns>随机数数组</returns>
        public static double[] DoubleRandMulti(int num, int precision)
        {
            double[] array = new double[num];
            int intMin = 0; //in * Math.Pow(10, (double)precision));
            int intMax = (int)(1.0 * Math.Pow(10, (double)precision));

            for (int i = 0; i < num; i++)
            {
                double val = Int32Rand(intMin, intMax);
                array[i] = (double)val / Math.Pow(10, (double)precision); ;
            }

            return array;
        }

    }
}
