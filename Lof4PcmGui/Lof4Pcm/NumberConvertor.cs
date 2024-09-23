namespace Lof4PcmGui.Lof4Pcm
{
    public class NumberConvertor
    {
        public static double ToDouble(string numString)
        {
            double valueDouble = 0.0;

            double num = 0;
            bool converted = double.TryParse(numString, out num);

            if (converted)
                valueDouble = num;
            else
            {
                string[] numbers = numString.Split('/');
                if (numbers.Length != 2)
                    throw new NumberConvertException("Invalidate number string!");

                double num1;    // = Convert.ToDouble(numbers[0]);
                double num2;    // = Convert.ToDouble(numbers[1]);
                bool cd1 = double.TryParse(numbers[0], out num1);
                bool cd2 = double.TryParse(numbers[1], out num2);
                if (cd1 && cd2)
                    valueDouble = num1 / num2;
                else
                    throw new NumberConvertException("Invalidate fractional number!");
            }

            return valueDouble;
        }
    }
}
