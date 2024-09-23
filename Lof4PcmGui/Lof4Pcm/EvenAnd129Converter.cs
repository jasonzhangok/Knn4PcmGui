using System;

namespace Lof4PcmGui.Lof4Pcm
{
    public class EvenAnd129Converter
    {
        public static double GetEvenFrom129(double double129)
        {
            //1.2 1-9值转换为均匀值
            //        1-9标度double值记为double1To9，均匀double值记为doubleEven
            //        如果double1To9 >= 9， doubleEven = 8；
            //        如果1 <= double1To9 < 9，doubleEven = double1To9 - 1；
            //        如果1/9 =< double1To9 < 1，分两种情况：
            //        概念：小于1的double1To9整数评价等级指1/2、1/3、......、1/9。小于1的double1To9上下取整指对整数评价等级取整，例如0.22的上取整为1/4(0.25)下取整为1/5(0.20)。
            //            a. double1To9小数评价等级为0(例如0.5的小数评价等级为0.5 - 1/2 = 0)，doubleEven = double1To9对应的值；
            //            b. double1To9小数评价等级为0(例如0.5的小数评价等级不为0.6 - 1/2 = 0.1)。		以upper和lower标记double1To0的等级上下取整，
            //                1-9评价等级上下取整对应的doubleEven整数等级标记为upperEven和lowerEven。
            //                1 / (upper - lower)表示将[lower, upper]范围映射到1上的范围，double1To9的小数部分 = double1To9 - lower
            //                doubleEven = lowerEven + (double1To9 - lower) / (upper - lower)，注意doubleEven、lowerEven和upperEven为负数。
            //        如果double1To9 < 1/9，doubleEven = -8。

            //        doubleEven下取整对应的1-9double值通过判断范围获得：
            //        1 -> 0; 1/2 -> -1; 1/3 -> -2; 1/4 -> -3; 1/5 -> -4; 1/6 -> -5; 1/7 ->  -6; 1/8-> -7; 1/9 -> -8
            //        1/2 =< double1To9 < 1 ：lower = 1/2，upper = 1, lowerEven = -1, upperEven = 0；
            //        1/3 =< double1To9 < 1/2 ：lower = 1/3，upper = 1/2；lowerEven = -2, upperEven = -1；
            //        1/4 =< double1To9 < 1/3 ：lower = 1/4，upper = 1/3；lowerEven = -3, upperEven = -2；
            //        1/5 =< double1To9 < 1/4 ：lower = 1/5，upper = 1/4；lowerEven = -4, upperEven = -3；
            //        1/6 =< double1To9 < 1/5 ：lower = 1/6，upper = 1/5；lowerEven = -5, upperEven = -4；
            //        1/7 =< double1To9 < 1/6 ：lower = 1/7，upper = 1/6；lowerEven = -6, upperEven = -5；
            //        1/8 =< double1To9 < 1/7 ：lower = 1/8，upper = 1/7；lowerEven = -7, upperEven = -6；
            //        1/9 =< double1To9 < 1/8 ：lower = 1/9，upper = 1/8；lowerEven = -8, upperEven = -7；
            //    算例：
            //        double1To9 = 4.6, doubleEven = 3.6
            //        double1To9 = 1.6，那么doubleEven = 0.6；
            //        double1T09 = 0.22, 那么lower = 1/5, upper = 1/4, lowerEven = -4, upperEven = -3, doubleEven = -4 + (0.22 - 1/5) / (1/4 - 1/5) = -4 + 0.4 = -3.6
            //        double1To9 = 0.7, 那么lower = 1/2, upper = 1, lowerEven = -1, upperEven = 0, doubleEven = -1 + (0.7 - 1/2) / (1 - 1/2) = -0.6
            //        double1To9 = 0.5, 那么lower = 1/2, upper = 1, lowerEven = -1, upperEven = 0, doubleEven = -1 + (0.5 - 1/2) / (1 -1/2) = -1

            double doubleEven = 0;
            double lower = 0, upper = 0;
            double lowerEven = 0, upperEven = 0;
            double doubleTmp = 0;
            if (double129 >= 9)
                doubleEven = 8.0;
            if (double129 >= 1 && double129 < 9)
                doubleEven = double129 - 1;
            if (double129 >= (double)1 / (double)9 && double129 < 1)
            {
                if (Is129Integer(double129, out doubleTmp))
                {
                    doubleEven = doubleTmp;
                }
                else
                {
                    lower = Get129LowerValue(double129);
                    upper = Get129UpperValue(double129);
                    lowerEven = GetEvenFromInt129(lower);
                    upperEven = GetEvenFromInt129(upper);
                    doubleEven = lowerEven + (double129 - lower) / (upper - lower);
                }
            }
            if (double129 < (double)1 / (double)9)
                doubleEven = -8.0;

            return doubleEven;
        }

        public static double GetEvenFrom129(String str129)
        {
            double double129 = NumberConvertor.ToDouble(str129);
            return GetEvenFrom129(double129);
        }

        /// <summary>
        /// 将一个字符串表示double均匀值转换为对应的1-9标度值.
        /// </summary>
        /// <param name="strEven">均匀数字符串.</param>
        /// <returns>对应的1-9值</returns>
        public static double Get129FromEven(String strEven)
        {
            double doubleEven = NumberConvertor.ToDouble(strEven);
            return Get129FromEven(doubleEven);
        }

        /// <summary>
        /// 将一个double均匀值转换为对应的1-9标度值.
        /// </summary>
        /// <param name="doubleEven">均匀值.</param>
        /// <returns>对应的1-9值</returns>
        public static double Get129FromEven(double doubleEven)
        {
            //1-9与均匀值的对应关系：
            //9->8; 8->7; 7->6; 6->5; 5->4; 4->3; 3->2; 2->1; 1->0; 1/2->-1; 1/3->-2; 1/4->-3; 1/5->-4; 1/6->-5; 1/7-> -6; 1/8-> -7; 1/9->-8
            //1.1 均匀值转换为1-9值
            //    均匀double值记为doubleEven，1-9标度double值记为double1To9
            //        如果doubleEven >= 8：double1To9 = 9；
            //        如果0 <= doubleEven < 8：double1To9 = doubleEven + 1；
            //        如果-8 <= doubleEven < 0：分两种情况：
            //             a. 小数部分不为0：double1To9 = doubleEven上取整对应的1-9double值 + doubleEven小数部分值 * (doubleEven上取整对应的1-9double值 - doubleEven下取整对应的1-9double值)，注意小数部分一定为负值；
            //             b. 小数部分为0：double1To9 = doubleEven对应的值，因为上下取整是基于0的原因，所以需要这样处理，例如-8.0的上取整为-7.0，下取整为-8.0，而-8.001的上取整为-8.0，下取整为-9.0。
            //        如果doubleEven < -8：double1To9 = 1/9。
            //算例：
            //    doubleEven = 3.6，那么double1To9 = 3.6；
            //    doubleEven = 0.6，那么double1To9 = 1.6；
            //    doubleEven = -3.6，那么doubleEven上取整(-3)对应1-9double值为1/4，doubleEven下取整(-4)对应1-9double值为1/5，doubleEven小数部分值为0.6；
            //    double1To9 = 1/4 + (-0.6) * (1/4 - 1/5) = 0.22
            //    doubleEven = -0.6，那么doubleEven上取整(0)对应1-9double值为1，doubleEven下取整(-1)对应1-9double值为1/2，doubleEven小数部分值为0.6；
            //    double1To9 = 1 + (- 0.6) * (1 - 1/2) = 0.7
            //    doubleEven = -8.0，double1To9 = 1/9
            double double129 = 0;

            double lower = 0, upper = 0;
            double decimalPart = doubleEven - (int)doubleEven;
            int intValue = 0;
            if (doubleEven >= 8)
                double129 = 9;
            if (doubleEven >= 0 && doubleEven < 8)
                double129 = doubleEven + 1;
            if (doubleEven >= -8 && doubleEven < 0)
            {
                if (IsEvenInteger(doubleEven, out intValue))
                {
                    double129 = Get129ValueByEvenIntValue(intValue);
                }
                else
                {
                    lower = Get129ValueByEvenIntValue((int)(Math.Floor(doubleEven)));
                    upper = Get129ValueByEvenIntValue((int)(Math.Floor(doubleEven)) + 1);
                    double129 = upper + (decimalPart * (upper - lower));
                }

            }
            if (doubleEven < -8)
                double129 = (double)1 / (double)9;
            return double129;
        }

        /// <summary>
        /// 从-8到8的等级值转换为从0到16的等级值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double TransFromEvenTo0To17(double val)
        {
            // 8, 7, 6, 5, 4, 3, 2, 1, 0,-1,-2，-3，-4，-5，-6，-7，-8
            //16,15,14,13,12,11,10,9, 8, 7, 6,  5,  4,  3, 2,  1,  0
            return val + 8;
        }

        /// <summary>
        /// 从0到16的等级值转换为从-8到8的等级值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double TransFrom0To17ToEven(double val)
        {
            // 8, 7, 6, 5, 4, 3, 2, 1, 0,-1,-2，-3，-4，-5，-6，-7，-8
            //16,15,14,13,12,11,10,9, 8, 7, 6,  5,  4,  3, 2,  1,  0
            return val - 8;
        }

        /// <summary>
        /// 判断给定的1-9是否为整数评级等级（例如1/2,1/6等）
        /// </summary>
        /// <param name="double129">1-9值.</param>
        /// <param name="doubleEven">输出参数，如果是整数评价等级，返回对应的平均值doubleEven，否则返回Consts.DoubleDelta.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private static bool Is129Integer(double double129, out double doubleEven)
        {
            if (Math.Abs(double129 - 9) < Consts.DoubleDelta ||
               Math.Abs(double129 - 8) < Consts.DoubleDelta ||
               Math.Abs(double129 - 7) < Consts.DoubleDelta ||
               Math.Abs(double129 - 6) < Consts.DoubleDelta ||
               Math.Abs(double129 - 5) < Consts.DoubleDelta ||
               Math.Abs(double129 - 4) < Consts.DoubleDelta ||
               Math.Abs(double129 - 3) < Consts.DoubleDelta ||
               Math.Abs(double129 - 2) < Consts.DoubleDelta ||
               Math.Abs(double129 - 1) < Consts.DoubleDelta ||
               Math.Abs(double129 - (double)1 / (double)2) < Consts.DoubleDelta ||
               Math.Abs(double129 - (double)1 / (double)3) < Consts.DoubleDelta ||
               Math.Abs(double129 - (double)1 / (double)4) < Consts.DoubleDelta ||
               Math.Abs(double129 - (double)1 / (double)5) < Consts.DoubleDelta ||
               Math.Abs(double129 - (double)1 / (double)6) < Consts.DoubleDelta ||
               Math.Abs(double129 - (double)1 / (double)7) < Consts.DoubleDelta ||
               Math.Abs(double129 - (double)1 / (double)8) < Consts.DoubleDelta ||
               Math.Abs(double129 - (double)1 / (double)9) < Consts.DoubleDelta)
            {
                doubleEven = GetEvenFromInt129(double129);
                return true;
            }
            else
            {
                doubleEven = Consts.DoubleDelta;
                return false;
            }
        }

        private static double GetEvenFromInt129(double val)
        {
            //9->8; 8->7; 7->6; 6->5; 5->4; 4->3; 3->2; 2->1; 1->0; 1/2->-1; 1/3->-2; 1/4->-3; 1/5->-4; 1/6->-5; 1/7-> -6; 1/8-> -7; 1/9->-8
            if (Math.Abs(val - 9) < Consts.DoubleDelta)
                return 8;
            if (Math.Abs(val - 8) < Consts.DoubleDelta)
                return 7;
            if (Math.Abs(val - 7) < Consts.DoubleDelta)
                return 6;
            if (Math.Abs(val - 6) < Consts.DoubleDelta)
                return 5;
            if (Math.Abs(val - 5) < Consts.DoubleDelta)
                return 4;
            if (Math.Abs(val - 4) < Consts.DoubleDelta)
                return 3;
            if (Math.Abs(val - 3) < Consts.DoubleDelta)
                return 2;
            if (Math.Abs(val - 2) < Consts.DoubleDelta)
                return 1;
            if (Math.Abs(val - 1) < Consts.DoubleDelta)
                return 0;
            if (Math.Abs(val - (double)1 / (double)2) < Consts.DoubleDelta)
                return -1;
            if (Math.Abs(val - (double)1 / (double)3) < Consts.DoubleDelta)
                return -2;
            if (Math.Abs(val - (double)1 / (double)4) < Consts.DoubleDelta)
                return -3;
            if (Math.Abs(val - (double)1 / (double)5) < Consts.DoubleDelta)
                return -4;
            if (Math.Abs(val - (double)1 / (double)6) < Consts.DoubleDelta)
                return -5;
            if (Math.Abs(val - (double)1 / (double)7) < Consts.DoubleDelta)
                return -6;
            if (Math.Abs(val - (double)1 / (double)8) < Consts.DoubleDelta)
                return -7;
            if (Math.Abs(val - (double)1 / (double)9) < Consts.DoubleDelta)
                return -8;

            return 9999;
        }

        /// <summary>
        /// 获得1-9值的下取整值.
        /// </summary>
        /// <param name="double129">1-9值.</param>
        /// <returns>1-9下取整值</returns>
        private static double Get129LowerValue(double double129)
        {
            //    doubleEven下取整对应的1-9double值通过判断范围获得：
            //    1 -> 0; 1/2 -> -1; 1/3 -> -2; 1/4 -> -3; 1/5 -> -4; 1/6 -> -5; 1/7 ->  -6; 1/8-> -7; 1/9 -> -8
            //    1/2 =< double1To9 < 1 ：doubleEven下取整对应的1-9double值为1/2，上取整为1；
            //    1/3 =< double1To9 < 1/2 ：doubleEven下取整对应的1-9double值为1/3，上取整为1/2；
            //    1/4 =< double1To9 < 1/3 ：doubleEven下取整对应的1-9double值为1/4，上取整为1/3；
            //    1/5 =< double1To9 < 1/4 ：doubleEven下取整对应的1-9double值为1/5，上取整为1/4；
            //    1/6 =< double1To9 < 1/5 ：doubleEven下取整对应的1-9double值为1/6，上取整为1/5；
            //    1/7 =< double1To9 < 1/6 ：doubleEven下取整对应的1-9double值为1/7，上取整为1/6；
            //    1/8 =< double1To9 < 1/7 ：doubleEven下取整对应的1-9double值为1/8，上取整为1/7；
            //    1/9 =< double1To9 < 1/8 ：doubleEven下取整对应的1-9double值为1/9，上取整为1/8；
            if (double129 >= (double)1 / (double)2 && double129 < 1)
                return (double)1 / (double)2;
            if (double129 >= (double)1 / (double)3 && double129 < (double)1 / (double)2)
                return (double)1 / (double)3;
            if (double129 >= (double)1 / (double)4 && double129 < (double)1 / (double)3)
                return (double)1 / (double)4;
            if (double129 >= (double)1 / (double)5 && double129 < (double)1 / (double)4)
                return (double)1 / (double)5;
            if (double129 >= (double)1 / (double)6 && double129 < (double)1 / (double)5)
                return (double)1 / (double)6;
            if (double129 >= (double)1 / (double)7 && double129 < (double)1 / (double)6)
                return (double)1 / (double)7;
            if (double129 >= (double)1 / (double)8 && double129 < (double)1 / (double)7)
                return (double)1 / (double)8;
            if (double129 >= (double)1 / (double)9 && double129 < (double)1 / (double)8)
                return (double)1 / (double)9;
            return -1;
        }

        /// <summary>
        /// 获得1-9值的上取整值.
        /// </summary>
        /// <param name="double129">1-9值.</param>
        /// <returns>1-9上取整值</returns>
        private static double Get129UpperValue(double double129)
        {
            //    doubleEven下取整对应的1-9double值通过判断范围获得：
            //    1 -> 0; 1/2 -> -1; 1/3 -> -2; 1/4 -> -3; 1/5 -> -4; 1/6 -> -5; 1/7 ->  -6; 1/8-> -7; 1/9 -> -8
            //    1/2 =< double1To9 < 1 ：doubleEven下取整对应的1-9double值为1/2，上取整为1；
            //    1/3 =< double1To9 < 1/2 ：doubleEven下取整对应的1-9double值为1/3，上取整为1/2；
            //    1/4 =< double1To9 < 1/3 ：doubleEven下取整对应的1-9double值为1/4，上取整为1/3；
            //    1/5 =< double1To9 < 1/4 ：doubleEven下取整对应的1-9double值为1/5，上取整为1/4；
            //    1/6 =< double1To9 < 1/5 ：doubleEven下取整对应的1-9double值为1/6，上取整为1/5；
            //    1/7 =< double1To9 < 1/6 ：doubleEven下取整对应的1-9double值为1/7，上取整为1/6；
            //    1/8 =< double1To9 < 1/7 ：doubleEven下取整对应的1-9double值为1/8，上取整为1/7；
            //    1/9 =< double1To9 < 1/8 ：doubleEven下取整对应的1-9double值为1/9，上取整为1/8；
            //    算例：
            if (double129 >= (double)1 / (double)2 && double129 < 1)
                return 1.0;
            if (double129 >= (double)1 / (double)3 && double129 < (double)1 / (double)2)
                return (double)1 / (double)2;
            if (double129 >= (double)1 / (double)4 && double129 < (double)1 / (double)3)
                return (double)1 / (double)3;
            if (double129 >= (double)1 / (double)5 && double129 < (double)1 / (double)4)
                return (double)1 / (double)4;
            if (double129 >= (double)1 / (double)6 && double129 < (double)1 / (double)5)
                return (double)1 / (double)5;
            if (double129 >= (double)1 / (double)7 && double129 < (double)1 / (double)6)
                return (double)1 / (double)6;
            if (double129 >= (double)1 / (double)8 && double129 < (double)1 / (double)7)
                return (double)1 / (double)7;
            if (double129 >= (double)1 / (double)9 && double129 < (double)1 / (double)8)
                return (double)1 / (double)8;
            return -1;
        }


        /// <summary>
        /// 判断给定的均匀数是否小数部分为0.
        /// </summary>
        /// <param name="doubleEven">均匀数.</param>
        /// <param name="intValue">输出参数，如果小数部分为0返回对应的整数，否则返回9999.</param>
        /// <returns>
        ///   <c>true</c> 如果小数部分为0; 否则, <c>false</c>.
        /// </returns>
        private static bool IsEvenInteger(double doubleEven, out int intValue)
        {
            if (Math.Abs(doubleEven - 8) < Consts.DoubleDelta)
            {
                intValue = 8;
                return true;
            }
            if (Math.Abs(doubleEven - 7) < Consts.DoubleDelta)
            {
                intValue = 7;
                return true;
            }
            if (Math.Abs(doubleEven - 6) < Consts.DoubleDelta)
            {
                intValue = 6;
                return true;
            }
            if (Math.Abs(doubleEven - 5) < Consts.DoubleDelta)
            {
                intValue = 5;
                return true;
            }
            if (Math.Abs(doubleEven - 4) < Consts.DoubleDelta)
            {
                intValue = 4;
                return true;
            }
            if (Math.Abs(doubleEven - 3) < Consts.DoubleDelta)
            {
                intValue = 3;
                return true;
            }
            if (Math.Abs(doubleEven - 2) < Consts.DoubleDelta)
            {
                intValue = 2;
                return true;
            }
            if (Math.Abs(doubleEven - 1) < Consts.DoubleDelta)
            {
                intValue = 1;
                return true;
            }
            if (Math.Abs(doubleEven - 0) < Consts.DoubleDelta)
            {
                intValue = 0;
                return true;
            }
            if (Math.Abs(doubleEven + 1) < Consts.DoubleDelta)
            {
                intValue = -1;
                return true;
            }
            if (Math.Abs(doubleEven + 2) < Consts.DoubleDelta)
            {
                intValue = -2;
                return true;
            }
            if (Math.Abs(doubleEven + 3) < Consts.DoubleDelta)
            {
                intValue = -3;
                return true;
            }
            if (Math.Abs(doubleEven + 4) < Consts.DoubleDelta)
            {
                intValue = -4;
                return true;
            }
            if (Math.Abs(doubleEven + 5) < Consts.DoubleDelta)
            {
                intValue = -5;
                return true;
            }
            if (Math.Abs(doubleEven + 6) < Consts.DoubleDelta)
            {
                intValue = -6;
                return true;
            }
            if (Math.Abs(doubleEven + 7) < Consts.DoubleDelta)
            {
                intValue = -7;
                return true;
            }
            if (Math.Abs(doubleEven + 8) < Consts.DoubleDelta)
            {
                intValue = -8;
                return true;
            }

            intValue = 9999;
            return false;
        }

        /// <summary>
        /// 根据给定的整形均匀值，返回对应的1-9值.
        /// </summary>
        /// <param name="val">均匀值.</param>
        /// <returns>1-9值</returns>
        private static double Get129ValueByEvenIntValue(int val)
        {
            //9->8; 8->7; 7->6; 6->5; 5->4; 4->3; 3->2; 2->1; 1->0; 1/2->-1; 1/3->-2; 1/4->-3; 1/5->-4; 1/6->-5; 1/7-> -6; 1/8-> -7; 1/9->-8
            switch (val)
            {
                case 8:
                    return 9.0;
                case 7:
                    return 8.0;
                case 6:
                    return 7.0;
                case 5:
                    return 6.0;
                case 4:
                    return 5.0;
                case 3:
                    return 4.0;
                case 2:
                    return 3.0;
                case 1:
                    return 2.0;
                case 0:
                    return 1.0;

                case -1:
                    return (double)1 / (double)2;
                case -2:
                    return (double)1 / (double)3;
                case -3:
                    return (double)1 / (double)4;
                case -4:
                    return (double)1 / (double)5;
                case -5:
                    return (double)1 / (double)6;
                case -6:
                    return (double)1 / (double)7;
                case -7:
                    return (double)1 / (double)8;
                case -8:
                    return (double)1 / (double)9;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// 寻找距离小数最近的一个1-9标度
        /// </summary>
        /// <param name="val">小数的标度值</param>
        /// <param name="isBiggerFix">true：如果修正值比原始值大</param>
        /// <returns>最接近的标度值</returns>
        public static double NearestLevelOfValue(double val, bool isBiggerFix)
        {
            double result = val;
            double[] levels = new[]
            {
                        1.0 / 9.0, 1.0 / 8.0, 1.0 / 7.0, 1.0 / 6.0, 1.0 / 5.0, 1.0 / 4.0, 1.0 / 3.0,
                        1.0 / 2.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0
                    };

            int dimense = levels.Length;

            if (val < levels[0])
                result = levels[0];
            if (val > levels[dimense - 1])
                result = levels[dimense - 1];
            for (int i = 0; i < dimense - 1; i++)
            {
                if (val >= levels[i] && val <= levels[i + 1]) // 在这个区间内
                {
                    double lowDiff = val - levels[i];
                    double highDiff = levels[i + 1] - val;

                    if (Math.Abs(lowDiff - highDiff) < 0.0000001) // 正好与上下两个等级距离都相等
                    {
                        if (isBiggerFix) // 如果修正值比原始值大
                        {
                            // 向小的方向取值，以达到对原始数据改变更小的目的
                            result = levels[i];
                        }
                        else
                        {
                            result = levels[i + 1];
                        }
                    }
                    else if (lowDiff > highDiff) // 距离下等级较远，取上一个等级
                        result = levels[i + 1];
                    else
                        result = levels[i];
                }
            }

            return result;
        }

    }
}
