using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Model;

namespace ModelTest
{
    //__________________________________________ТЕСТЫ ДЛЯ КЛАССА MeasuredData________________________________________________________
    public class MeasuredDataTest
    {
        //.................................Тест псевдослучайной генерации узлов неравномерной сетки.................................
        [Theory]
        [InlineData(20, 10, 125)]
        [InlineData(4, -10, 10)]
        [InlineData(10, 0, 2)]
        public void Test_NonUniform_Nodes(int n, double min, double max)
        {
            var md = new MeasuredData(n, min, max);

            md.Num.Should().Be(n);
            md.Scope[0].Should().Be(min);
            md.Scope[1].Should().Be(max);

            for (int i = 0; i < n; i++)
                md.NodeArray[i].Should().BeGreaterThan(md.NodeArray[i - 1]);
        }

        //....................................Тест кубического многочлена y = x^3 + 3x^2 - 6x - 18...................................
        [Theory]
        [InlineData(20, 10, 125)]
        [InlineData(4, -10, 10)]
        [InlineData(10, 0, 2)]
        public void Test_CubPol(int n, double max, double min)
        {
            var md = new MeasuredData(n, min, max, SPf.CubPol);

            for (int i = 0; i < n; i++)
            {
                double y = Math.Pow(md.NodeArray[i], 3) + 3 * Math.Pow(md.NodeArray[i], 2) - 6 * md.NodeArray[i] - 18;
                md.ValueArray[i].Should().Be(y);
            }
        }

        //......................................................Тест экспоненты.....................................................
        [Theory]
        [InlineData(20, 10, 125)]
        [InlineData(4, -10, 10)]
        [InlineData(10, 0, 2)]
        public void Test_Exp(int n, double min, double max)
        {
            var md = new MeasuredData(n, min, max, SPf.Exp);

            for (int i = 0; i < n; i++)
                md.ValueArray[i].Should().Be(Math.Exp(md.NodeArray[i]));
        }
    }

    //________________________________________ТЕСТЫ ДЛЯ КЛАССА SplineParameters______________________________________________________
    public class SplineParametersTest
    {
        //..........................................Тест генерации узлов равномерной сетки..........................................
        [Theory]
        [InlineData(4, -10, 10, 5, 5, 6, 6)]
        [InlineData(10, 0, 2, 1, 2, 3, 4)]
        public void Test_Uniform_Nodes(int n, double min, double max, double der_left_1, double der_right_1, double der_left_2, double der_right_2)
        {
            var sp = new SplineParameters(n, min, max, der_left_1, der_right_1, der_left_2, der_right_2);

            sp.Num.Should().Be(n);
            sp.Scope[0].Should().Be(min);
            sp.Scope[1].Should().Be(max);
            sp.Derivative1[0].Should().Be(der_left_1);
            sp.Derivative1[1].Should().Be(der_right_1);
            sp.Derivative2[0].Should().Be(der_left_2);
            sp.Derivative2[1].Should().Be(der_right_2);

            double step = (max - min) / (n - 1);
            for (int i = 0; i < n; i++)
                sp.NodeArray[i].Should().Be(min + step * i);
        }
    }
}