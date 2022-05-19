using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;

namespace ViewModel
{
    //______________________________________________КЛАСС ДАННЫХ ДЛЯ ПОСТРОЕНИЯ ГРАФИКИ_______________________________________________
    public class Data
    {
        public string xTitle { get; set; }                                                          // Надпись по оси Ox
        public string yTitle { get; set; }                                                          // Надпись по оси Oy
        public double[] X { get; set; }                                                             // Значения абсцисс 
        public double[] Y { get; set; }
        public double[] Splines_X { get; set; }
        public List<double[]> Splines_Y_List { get; set; }
        public List<string> legends { get; set; }

        public Data()
        {
            legends = new List<string>();
            Splines_Y_List = new List<double[]>();
            xTitle = "x";
            yTitle = "F(x)";
        }

        public void AddMeasuredData(int nx, double[] XArray, double[] YArray)
        {
            try
            {
                X = new double[nx];
                Y = new double[nx];
                for (int i = 0; i < nx; i++)
                {
                    X[i] = XArray[i];
                    Y[i] = YArray[i];
                }
                legends.Add("Измеренные данные");
            }
            catch (Exception ex)
            { throw new Exception("Ошибка в AddMeasuredData!\n " + ex.Message); }
        }

        public void AddSplinesData(int nx, double[] Scope, double[] YArray, bool first_der_set)
        {
            try
            {
                Splines_X = new double[nx];
                double[] Splines_Y = new double[nx];
                double step = (Scope[1] - Scope[0]) / (nx - 1);
                for (int i = 0; i < nx; i++)
                {
                    Splines_X[i] = Scope[0] + step * i;
                    Splines_Y[i] = YArray[i];
                }
                Splines_Y_List.Add(Splines_Y);
                string der_set = first_der_set ? "Первый" : "Второй";
                legends.Add("Значения сплайнов: " + der_set + " набор производных");
            }
            catch (Exception ex)
            { throw new Exception("Ошибка в AddSplinesData!\n " + ex.Message); }
        }
    }

    //______________________________________КЛАСС ДЛЯ ВЗАИМОДЕЙСТВИЯ С MainViewModel (OxyPlot)________________________________________
    public class ChartData
    {
        public PlotModel plotModel { get; private set; }

        public ChartData()
        {
            this.plotModel = new PlotModel { Title = "Measured Data & Splines Charts" };
        }

        public void AddDataSeries(Data data)
        {
            Legend legend = new Legend();
            OxyColor color;

            this.plotModel.Series.Clear();
            color = OxyColors.Green;

            LineSeries lineSeries = new LineSeries();
            for (int i = 0; i < data.X.Length; i++)
                lineSeries.Points.Add(new DataPoint(data.X[i], data.Y[i]));

            lineSeries.MarkerType = MarkerType.Circle;
            lineSeries.MarkerSize = 4;
            lineSeries.Color = color;
            lineSeries.MarkerStroke = color;
            lineSeries.MarkerFill = color;
            lineSeries.Title = data.legends[0];

            plotModel.Legends.Add(legend);
            this.plotModel.Series.Add(lineSeries);
        }

        public void AddSplineSeries(Data data)
        {
            OxyColor color;
            for (int i = 0; i < data.Splines_Y_List.Count; i++)
            {
                color = i == 0 ? OxyColors.Blue : OxyColors.Orange;
                LineSeries SplineSeries = new LineSeries
                { InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline };
                for (int j = 0; j < data.Splines_X.Length; j++)
                    SplineSeries.Points.Add(new DataPoint(data.Splines_X[j], data.Splines_Y_List[i][j]));

                SplineSeries.MarkerType = MarkerType.Circle;
                SplineSeries.MarkerSize = 4;
                SplineSeries.Color = color;
                SplineSeries.MarkerStroke = color;
                SplineSeries.MarkerFill = color;
                SplineSeries.Title = data.legends[i + 1];

                this.plotModel.Series.Add(SplineSeries);
            }
        }
    }
}
