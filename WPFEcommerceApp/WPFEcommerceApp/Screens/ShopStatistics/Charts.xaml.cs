﻿using System;
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
            var store = new AccountStore();
            this.DataContext = new ShopStatisticsViewModel(store);
        }

        

        
    }
}
