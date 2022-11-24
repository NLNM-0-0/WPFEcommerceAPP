using System;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for Charts.xaml
    /// </summary>
    public partial class Charts : UserControl
    {
        public Charts()
        {
            InitializeComponent();
            this.Cartesian();
        }

        public void Cartesian()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title="Title1",
                    Values=new ChartValues<double>{400, 600, 500, 500 }
                },
                new LineSeries
                {
                    Title="Title2",
                    Values=new ChartValues<double>{600, 700, 300, 800 },
                    PointGeometry=null,
                },
                new LineSeries
                {
                    Title="Title3",
                    Values=new ChartValues<double>{400, 200, 300, 600},
                    PointGeometry=DefaultGeometries.Square,
                    PointGeometrySize=15,
                },
            };

            Labels = new[] { "label1", "label2", "label3" };
            yFormatter=(value)=>value.ToString("C");

            SeriesCollection.Add(new LineSeries
            {
                Title="title4", Values=new ChartValues<double> { 5, 700, 2 },
                LineSmoothness=0,
                PointGeometry=Geometry.Parse("m 25 70 20 -20 -20 20 -10 -10 z"),
                PointGeometrySize=15,
                PointForeground=Brushes.Green,
            });
            SeriesCollection[3].Values.Add(5d);
            DataContext = this;
        }

        public Func<double, string>yFormatter { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
    }
}
