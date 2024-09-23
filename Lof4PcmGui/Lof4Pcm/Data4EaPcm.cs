namespace Lof4PcmGui.Lof4Pcm
{
    public class Data4EaPcm
    {
        public double[][] Matrix { get; set; }
        public double AvgBias { get; set; }
        public double Cr { get; set; }

        public Data4EaPcm(double[][] matrix, double cr, double avgBias)
        {
            Matrix = matrix;
            AvgBias = avgBias;
            Cr = cr;
        }
    }
}
