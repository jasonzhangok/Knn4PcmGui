namespace Lof4PcmGui.Lof4Pcm
{
    public class Consts
    {
        public const double DoubleDelta = 1e-8;
        public const double DuplicateDelta = 1e-5;

        public static double GetOutlierThresholdOfKnn(int len, bool is3Sigma = true)
        {
            double result;
            switch (len)
            {
                case 3:
                    result = is3Sigma ? 0.024472 : 0.019986;
                    break;
                case 4:
                    result = is3Sigma ? 0.085099 : 0.069094;
                    break;
                case 5:
                    result = is3Sigma ? 0.081641 : 0.065002;
                    break;
                case 6:
                    result = is3Sigma ? 0.040239 : 0.030882;
                    break;
                case 7:
                    result = is3Sigma ? 0.018058 : 0.013964;
                    break;
                case 8:
                    result = is3Sigma ? 0.012610 : 0.00994;
                    break;
                case 9:
                    result = is3Sigma ? 0.017696 : 0.013799;
                    break;
                default:
                    result = 0.01;
                    break;
            }

            return result;
        }

        public static double GetOutlierThresholdOfLof(int len, bool is3Sigma = true)
        {
            double result;
            switch (len)
            {
                case 3:
                    result = is3Sigma ? 0.024472 : 0.019986;
                    break;
                case 4:
                    result = is3Sigma ? 0.085099 : 0.069094;
                    break;
                case 5:
                    result = is3Sigma ? 0.081641 : 0.065002;
                    break;
                case 6:
                    result = is3Sigma ? 0.040239 : 0.030882;
                    break;
                case 7:
                    result = is3Sigma ? 0.018058 : 0.013964;
                    break;
                case 8:
                    result = is3Sigma ? 0.012610 : 0.00994;
                    break;
                case 9:
                    result = is3Sigma ? 0.017696 : 0.013799;
                    break;
                default:
                    result = 0.01;
                    break;
            }

            return result;
        }

        public static double GetRiByN(int n)
        {
            double[] arrayRi = new double[] {0,
                0,
                0.52,
                0.89,
                1.12,
                1.26,
                1.36,
                1.41,
                1.46,
                1.49,
                1.52,
                1.54,
                1.56,
                1.58,
                1.59,
                1.5943,
                1.6064,
                1.6133,
                1.6207,
                1.6292,
                1.6358,
                1.6403,
                1.6462,
                1.6497,
                1.6556,
                1.6587,
                1.6631,
                1.6670,
                1.6693,
                1.6724,
                1.682586,
                1.696853,
                1.685480,
                1.699433,
                1.693602,
                1.697872,
                1.703933,
                1.700926,
                1.702825,
                1.707529,
                1.704169,
                1.706894,
                1.706187,
                1.712653,
                1.705355,
                1.715307,
                1.714093,
                1.709338,
                1.716562,
                1.715930,
                1.714527,
                1.717436,
                1.717476,
                1.718489,
                1.721898,
                1.721203,
                1.722532,
                1.723165,
                1.719280,
                1.725096,
                1.724198,
                1.723240,
                1.723483,
                1.724026,
                1.723520,
                1.725132,
                1.723313,
                1.726184,
                1.725884,
                1.728793,
                1.726572,
                1.727011,
                1.728733,
                1.726734,
                1.728196,
                1.731163,
                1.730590,
                1.728955,
                1.729260,
                1.730539,
                1.729862,
                1.731974,
                1.730303,
                1.730412,
                1.731102,
                1.733052,
                1.732363,
                1.733024,
                1.732117,
                1.733835,
                1.733583,
                1.732952,
                1.734036,
                1.732983,
                1.735078,
                1.734759,
                1.735244,
                1.734762,
                1.734965,
                1.735734
            };
            if (n > arrayRi.GetUpperBound(0))
                return -1;
            else
            {
                return arrayRi[n - 1];
            }
        }

    }
}
