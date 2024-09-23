namespace Lof4PcmGui.Lof4Pcm
{
    public class LrdItem
    {
        public double Influence { get; set; }
        public double Change { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public LrdItem(double influence, double change, int row, int column)
        {
            Influence = influence;
            Change = change;
            Row = row;
            Column = column;
        }

    }
}
