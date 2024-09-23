using System.Collections.Generic;

namespace Lof4PcmGui.Lof4Pcm
{
    public class MinimumChangeCheckResult
    {
        /// <summary>
        /// 此项数据的CR区间
        /// </summary>
        public string RangeString { get; set; }

        /// <summary>
        /// 不一致判断矩阵总数量
        /// </summary>
        public int TotalOfRange { get; set; }
        /// <summary>
        /// 修正成功的数量
        /// </summary>
        public int AdjustOkCounterOfRange { get; set; }
        /// <summary>
        /// 修正成功比例
        /// </summary>
        public string OkRateOfRange { get; set; }
        /// <summary>
        /// 各判断矩阵、各项的改变均值
        /// </summary>
        public List<double> AvgChangeOfRange { get; set; }

        /// <summary>
        /// 用于显示的变化均值
        /// </summary>
        public double AvgAvgChangeOfRange { get; set; }
        /// <summary>
        /// 用于显示的变化标准差
        /// </summary>
        public double StdAvgChangeOfRange { get; set; }

        public MinimumChangeCheckResult(string rangeString,
            int totalOfRange, int adjustOkCounterOfRange, 
            string okRateOfRange = "")
        {
            RangeString = rangeString;
            TotalOfRange = totalOfRange;
            AdjustOkCounterOfRange = adjustOkCounterOfRange;
            OkRateOfRange = okRateOfRange;
            AvgChangeOfRange = new List<double>();

        }
    }
}
