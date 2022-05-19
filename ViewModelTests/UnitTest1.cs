using Xunit;
using System;
using System.Linq;
using System.ComponentModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;
using Model;
using ViewModel;
using FluentAssertions;

namespace ViewModelTests
{
    public class TestErrorReporter : IErrorReporter
    {
        public bool There_Was_An_Error { get; set; } = false;
        public void ReportError(string mes)
        { There_Was_An_Error = true; }
    }

    public class PropertyChangedReporter
    {
        public bool Event { get; private set; } = false;
        public void OnPropertyChange(object sender, PropertyChangedEventArgs e)
        { Event = true; }
        public void Reset()
        { Event = false; }
    }

    public class MainViewModelTest
    {
        [Fact]
        public void ConstructorTest()
        {
            var err = new TestErrorReporter();
            var main = new MainViewModel(err);
            main.NonUniformNum.Should().Be(2);
            main.UniformNum.Should().Be(2);
            main.Min.Should().Be(0);
            main.Max.Should().Be(0);
            main.Der1Left.Should().Be(1);
            main.Der1Right.Should().Be(1);
            main.Der2Left.Should().Be(0);
            main.Der2Right.Should().Be(0);
            main.Function.Should().Be(SPf.Random);
        }

        [Fact]
        public void Validation_CanExecute_Test()
        {
            var err = new TestErrorReporter();
            var main = new MainViewModel(err);

            main["NonUniformNum"].Should().Be("Число узлов должно быть больше 2");
            main["Max"].Should().Be("Левый конец отрезка должен быть меньше правого");
            main["UniformNum"].Should().Be("Число узлов должно быть больше 2");
            main.MakeMD.CanExecute(main).Should().BeFalse();
            main.MakeSD.CanExecute(main).Should().BeFalse();

            main.NonUniformNum = 20;
            main.Min = 10;
            main.Max = 125;

            main["NonUniformNum"].Should().Be(String.Empty);
            main["Max"].Should().Be(String.Empty);
            main["UniformNum"].Should().Be("Число узлов должно быть больше 2");
            main.MakeMD.CanExecute(main).Should().BeTrue();
            main.MakeSD.CanExecute(main).Should().BeFalse();

            main.UniformNum = 10;

            main["NonUniformNum"].Should().Be(String.Empty);
            main["Max"].Should().Be(String.Empty);
            main["UniformNum"].Should().Be(String.Empty);
            main.MakeMD.CanExecute(main).Should().BeTrue();
            main.MakeSD.CanExecute(main).Should().BeTrue();
        }

        [Fact]
        public void UpdateDataTest()
        {
            var err = new TestErrorReporter();
            var main = new MainViewModel(err);

            main.NonUniformNum = 20;
            main.Min = 10; main.Max = 125;
            var new_event_reporter = new PropertyChangedReporter();
            main.PropertyChanged += new_event_reporter.OnPropertyChange;
            main.MakeMD.Execute(main);
            new_event_reporter.Event.Should().BeTrue();

            new_event_reporter.Reset();
            main.UniformNum = 10;
            main.MakeSD.Execute(main);
            new_event_reporter.Event.Should().BeTrue();
        }
    }
}