namespace Lof4PcmGui.Lof4Pcm
{
    public class DistAndPos
    {
        public double Distance { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public DistAndPos(double dist, int x, int y)
        {
            Distance = dist;
            Row = x;
            Column = y;
        }
    }
}
