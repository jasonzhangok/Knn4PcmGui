namespace Lof4PcmGui.Lof4Pcm
{
    public class PosAndCr
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public double Change { get; set; }
        public double Cr { get; set; }

        public PosAndCr(int row, int column, double change, double cr)
        {
            Row = row;
            Column = column;
            Change = change;
            Cr = cr;
        }
    }
}
