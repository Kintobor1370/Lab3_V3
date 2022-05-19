using System;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Model;

namespace ViewModel
{
    public class MainViewModel : ViewModelBase, IDataErrorInfo
    {
        public MeasuredData MeasuredData { get; set; }
        public SplineParameters SplineParameters { get; set; }
        public SplinesData SplinesData { get; set; }
        public ChartData ChartData { get; set; }
        private Data chart_data;
        private int _non_uniform_num;
        private int _uniform_num;
        private double[] _scope = new double[2];
        private double[] _der = new double[4];
        private SPf _func;
        public int NonUniformNum
        {
            get
            { return _non_uniform_num; }
            set
            {
                _non_uniform_num = value;
                OnPropertyChanged(nameof(NonUniformNum));
            }
        }
        public int UniformNum
        {
            get
            { return _uniform_num; }
            set
            {
                _uniform_num = value;
                OnPropertyChanged(nameof(UniformNum));
            }
        }
        public double Min
        {
            get
            { return _scope[0]; }
            set
            {
                _scope[0] = value;
                OnPropertyChanged(nameof(Min));
            }
        }
        public double Max
        {
            get
            { return _scope[1]; }
            set
            {
                _scope[1] = value;
                OnPropertyChanged(nameof(Max));
            }
        }
        public double Der1Left
        {
            get
            { return _der[0]; }
            set
            {
                _der[0] = value;
                OnPropertyChanged(nameof(Der1Left));
            }
        }
        public double Der1Right
        {
            get
            { return _der[1]; }
            set
            {
                _der[1] = value;
                OnPropertyChanged(nameof(Der1Right));
            }
        }
        public double Der2Left
        {
            get
            { return _der[2]; }
            set
            {
                _der[2] = value;
                OnPropertyChanged(nameof(Der2Left));
            }
        }
        public double Der2Right
        {
            get
            { return _der[3]; }
            set
            {
                _der[3] = value;
                OnPropertyChanged(nameof(Der2Right));
            }
        }
        public SPf Function
        {
            get
            { return _func; }
            set
            {
                _func = value;
                OnPropertyChanged(nameof(Chosen_Function));
            }
        }
        public ObservableCollection<SPf> ComboBox_Funcs { get; set; } = new ObservableCollection<SPf>() { SPf.CubPol, SPf.Exp, SPf.Random };
        public string Chosen_Function
        {
            get
            {
                string str = "Выбрано:\n";
                switch (Function)
                {
                    case SPf.CubPol:
                        str += "Кубический многочлен y = x^3 + 3x^2 - 6x - 18";
                        break;
                    case SPf.Exp:
                        str += "Экспонента";
                        break;
                    case SPf.Random:
                        str += "Генератор псевдослучайных чисел";
                        break;
                    default: break;
                }
                return str;
            }
        }

        public ObservableCollection<string> MeasuredDataCollection { get; set; }
        public ObservableCollection<string> SplinesDataCollection { get; set; }
        public IErrorReporter ErrorReporter { get; private set; }
        public RelayCommand MakeMD { get; private set; }
        public RelayCommand MakeSD { get; private set; }

        public MainViewModel(IErrorReporter err)
        {
            ErrorReporter = err;
            NonUniformNum = 2;
            UniformNum = 2;
            Min = 0; Max = 0;
            Der1Left = 1; Der1Right = 1;
            Der2Left = 0; Der2Right = 0;
            Function = SPf.Random;

            MakeMD = new RelayCommand(_ => { MakeMDHandler(this); OnPropertyChanged(nameof(ChartData)); }, CanMakeMDHandler);
            MakeSD = new RelayCommand(_ => { MakeSDHandler(this); OnPropertyChanged(nameof(ChartData)); }, CanMakeSDHandler);

            this.MeasuredDataCollection = new ObservableCollection<string>();
            this.SplinesDataCollection = new ObservableCollection<string>();

            MeasuredDataCollection.CollectionChanged += MDCollection_Changed;
            SplinesDataCollection.CollectionChanged += SDCollection_Changed;
        }

        //....................Коллекция узлов неравномерной сетки и значений функции в них (для вывода в ListBox)....................
        public void CreateMDCollection()
        {
            MeasuredDataCollection.Clear();
            for (int i = 0; i < NonUniformNum; i++)
                MeasuredDataCollection.Add("x = " + MeasuredData.NodeArray[i].ToString() +
                                 ";\nF(x) = " + MeasuredData.ValueArray[i].ToString());
        }
        void MDCollection_Changed(object? sender, NotifyCollectionChangedEventArgs e)
        { OnPropertyChanged("MeasuredDataCollection"); }

        //...........................Коллекция значений сплайнов и первыхпроизводных (для вывода в ListBox)..........................
        public void CreateSDCollection()
        {
            SplinesDataCollection.Clear();

            SplinesDataCollection.Add("Первый набор производных:");
            SplinesDataCollection.Add($"F'({Min}) = {SplinesData.Spline1DerivativeArray[0]};    F'({Max}) = {SplinesData.Spline1DerivativeArray[UniformNum - 1]}");
            SplinesDataCollection.Add($"F({Min}) = {SplinesData.Spline1ValueArray[0]};    F'({Min}) = {SplinesData.Spline1DerivativeArray[0]}");
            SplinesDataCollection.Add($"F({Min}+h) = {SplinesData.Spline1ValueArray[1]};    F'({Min}+h) = {SplinesData.Spline1DerivativeArray[1]}");
            SplinesDataCollection.Add($"F({Max}-h) = {SplinesData.Spline1ValueArray[UniformNum - 2]};    F'({Max}-h) = {SplinesData.Spline1DerivativeArray[UniformNum - 2]}");
            SplinesDataCollection.Add($"F({Max}) = {SplinesData.Spline1ValueArray[UniformNum - 1]};    F'({Max}) = {SplinesData.Spline1DerivativeArray[UniformNum - 1]}");

            SplinesDataCollection.Add("");

            SplinesDataCollection.Add("Второй набор производных:");
            SplinesDataCollection.Add($"F'({Min}) = {SplinesData.Spline2DerivativeArray[0]};    F'({Max}) = {SplinesData.Spline2DerivativeArray[UniformNum - 1]}");
            SplinesDataCollection.Add($"F({Min}) = {SplinesData.Spline2ValueArray[0]};    F'({Min}) = {SplinesData.Spline2DerivativeArray[0]}");
            SplinesDataCollection.Add($"F({Min}+h) = {SplinesData.Spline2ValueArray[1]};    F'({Min}+h) = {SplinesData.Spline2DerivativeArray[1]}");
            SplinesDataCollection.Add($"F({Max}-h) = {SplinesData.Spline2ValueArray[UniformNum - 2]};    F'({Max}-h) = {SplinesData.Spline2DerivativeArray[UniformNum - 2]}");
            SplinesDataCollection.Add($"F({Max}) = {SplinesData.Spline2ValueArray[UniformNum - 1]};    F'({Max}) = {SplinesData.Spline2DerivativeArray[UniformNum - 1]}");

            SplinesDataCollection.Add("");

            SplinesDataCollection.Add($"где h = {(Max - Min) / (UniformNum - 1)}");
        }
        void SDCollection_Changed(object? sender, NotifyCollectionChangedEventArgs e)
        { OnPropertyChanged("SplinesDataCollection"); }

        //........................................Реализация интерфейса IDataErrorInfo...............................................
        private Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();
        public string Error { get; }
        public string this[string PropertyName]
        {
            get
            {
                string err_message = String.Empty;
                CollectErrors();
                if (Errors.ContainsKey(PropertyName))
                    err_message = Errors[PropertyName];
                return err_message;
            }
        }
        private void CollectErrors()
        {
            Errors.Clear();
            string err_message = String.Empty;
            if (NonUniformNum <= 2)
                Errors.Add("NonUniformNum", "Число узлов должно быть больше 2");

            if (Min >= Max)
                Errors.Add("Max", "Левый конец отрезка должен быть меньше правого");

            if (UniformNum <= 2)
                Errors.Add("UniformNum", "Число узлов должно быть больше 2");

            MakeMD.RaiseCanExecuteChanged();
            MakeSD.RaiseCanExecuteChanged();
        }

        //..................................................Command Handlers.........................................................
        private bool CanMakeMDHandler(object sender)
        {
            if (Errors.ContainsKey("NonUniformNum") || Errors.ContainsKey("Max"))
                return false;
            else
                return true;
        }
        private bool CanMakeSDHandler(object sender)
        {
            if (!CanMakeMDHandler(this) || Errors.ContainsKey("UniformNum"))
                return false;
            else
                return true;
        }
        private void MakeMDHandler(object sender)
        {
            this.MeasuredData = new MeasuredData(NonUniformNum, Min, Max, Function);
            
            CreateMDCollection();
            chart_data = new Data();
            chart_data.AddMeasuredData(NonUniformNum, MeasuredData.NodeArray, MeasuredData.ValueArray);

            ChartData = new ChartData();
            ChartData.AddDataSeries(chart_data);
        }
        private void MakeSDHandler(object sender)
        {
            SplineParameters = new SplineParameters(UniformNum, Min, Max, Der1Left, Der1Right, Der2Left, Der2Right);
            SplinesData = new SplinesData(MeasuredData, SplineParameters);

            SplinesData.BuildSpline();
            CreateSDCollection();

            if (chart_data.Splines_Y_List.Count == 0)
            {
                chart_data.AddSplinesData(UniformNum, _scope, SplinesData.Spline1ValueArray, true);
                chart_data.AddSplinesData(UniformNum, _scope, SplinesData.Spline2ValueArray, false);
                ChartData.AddSplineSeries(chart_data);
            }
            OnPropertyChanged(nameof(ChartData.plotModel));
        }
    }
}