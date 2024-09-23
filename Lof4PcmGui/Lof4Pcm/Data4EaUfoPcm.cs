using System.Collections.Generic;

namespace Lof4PcmGui.Lof4Pcm
{
    public class Data4EaUfoPcm
    {
        public double[][] Matrix { get; set; }
        public double Cr { get; set; }
        public List<PosAndCr> ChangedInfo { get; set; }

        public Data4EaUfoPcm(double[][] matrix, double cr, 
            List<PosAndCr> changedInfo)
        {
            Matrix = matrix;
            Cr = cr;
            ChangedInfo = changedInfo;
        }
    }
}
