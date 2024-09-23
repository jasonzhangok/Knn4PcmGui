namespace Lof4PcmGui.Pso
{
    public class PsoArgs
    {
        private int adjustPrecise = 3;

        public int AdjustPrecise
        {
            get { return adjustPrecise; }
            set { adjustPrecise = value; }
        }

        private double varianceCoefficient = 0.5;

        public double VarianceCoefficient
        {
            get { return varianceCoefficient; }
            set { varianceCoefficient = value; }
        }

        private double range = 1.0;

        public double Range
        {
            get { return range; }
            set { range = value; }
        }

        private double c1;

        public double C1
        {
            get { return c1; }
            set { c1 = value; }
        }

        private double c2;

        public double C2
        {
            get { return c2; }
            set { c2 = value; }
        }

        private double maxConsistenceScale;
        public double MaxConsistenceScale
        {
            get { return this.maxConsistenceScale; }
            set { this.maxConsistenceScale = value; }
        }
    }
}
