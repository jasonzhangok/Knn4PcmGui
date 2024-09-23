using System.Collections.Generic;

namespace Lof4PcmGui.Lof4Pcm
{
    public class Data4UfoPcm
    {
        public double[][] Matrix { get; set; }
        public double AverageBias { get; set; }
        public List<PosAndCr> ChangedInfo { get; set; }

        public Data4UfoPcm(double[][] matrix, double avgBias, List<PosAndCr> changedInfo)
        {
            Matrix = matrix;
            AverageBias = avgBias;
            ChangedInfo = changedInfo;
        }
    }
}
