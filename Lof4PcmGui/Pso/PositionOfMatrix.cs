namespace Lof4PcmGui.Pso
{
    public class PositionOfMatrix
    {
        public PositionOfMatrix()
        {

        }

        public PositionOfMatrix(int row, int col)
        {
            Row = row;
            Column = col;
        }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
