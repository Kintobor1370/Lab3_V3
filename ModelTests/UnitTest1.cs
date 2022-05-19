using System;
using Xunit;
using FluentAssertions;
using Model;

namespace ModelTests
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

            for (int i = 1; i < n; i++)
                md.NodeArray[i].Should().BeGreaterThan(md.NodeArray[i - 1]);
        }

        //....................................Тест кубического многочлена y = x^3 + 3x^2 - 6x - 18...................................
        [Theory]
        [InlineData(20, 10, 125)]
        [InlineData(4, -10, 10)]
        [InlineData(10, 0, 2)]
        public void Test_CubPol(int n, double min, double max)
        {
            var md = new MeasuredData(n, min, max, SPf.CubPol);
            
            for (int i = 0; i < n; i++)
            {
                double x = md.NodeArray[i];
                md.ValueArray[i].Should().Be(Math.Pow(x, 3) + 3 * Math.Pow(x, 2) - 6 * x - 18);
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

    public class SplinesDataTest
    {
        //..........................................Тест генерации узлов равномерной сетки..........................................
        [Theory]
        [InlineData(8, 4, -10, 10, 5, 5, 6, 6)]
        [InlineData(5, 10, 0, 2, 1, 2, 3, 4)]
        public void Test_Splines_Data(int non_uniform_num, int uniform_num, double min, double max, double der_left_1, double der_right_1, double der_left_2, double der_right_2)
        {
            var md = new MeasuredData(non_uniform_num, min, max);
            var sp = new SplineParameters(uniform_num, min, max, der_left_1, der_right_1, der_left_2, der_right_2);
            var sd = new SplinesData(md, sp);

            for (int i = 0; i < uniform_num; i++)
                sd.NodeArray[i].Should().Be(sp.NodeArray[i]);
        }
    }
}