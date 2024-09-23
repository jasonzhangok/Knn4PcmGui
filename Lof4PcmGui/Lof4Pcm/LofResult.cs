using System.Collections.Generic;
using System.Windows;

namespace Lof4PcmGui.Lof4Pcm
{
    public class LofResult
    {
        public int Dimense { get; set; }
        public int UniqueItemNumber { get; set; }
        public double MinOuterlier { get; set; }
        public double MaxOuterlier { get; set; }
        public int Mismatch { get; set; }

        public int Amount { get; set; }

        public List<double> Outliers { get; set; }

        public double Avg { get; set; }
        public double Std { get; set; }

        public LofResult(int dimense, int uniqueItemNumber, double maxOuterlier, double minOuterlier,
            int mismatch, int amount)
        {
            Dimense = dimense;
            UniqueItemNumber = uniqueItemNumber;
            MaxOuterlier = maxOuterlier;
            MinOuterlier = minOuterlier;
            Mismatch = mismatch;
            Amount = amount;
            Outliers = new List<double>();
        }
    }
}
