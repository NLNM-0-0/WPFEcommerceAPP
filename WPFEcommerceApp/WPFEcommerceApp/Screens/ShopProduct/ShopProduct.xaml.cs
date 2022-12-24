﻿using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ShopProduct.xaml
    /// </summary>
    public partial class ShopProduct : UserControl
    {
        public ShopProduct()
        {
            InitializeComponent();
            Task task = Task.Run(async () => await Load());
            while (!task.IsCompleted) { }
            this.DataContext = new ShopProductViewModel();
        }
        public async Task Load()
        {
            var repo = new GenericDataRepository<MUser>();
            var t = await repo.GetSingleAsync(x => x.Id == "user02",
                                        x => x.Products,
                                        x => x.Products.Select(p => p.ImageProducts),
                                        x => x.Products.Select(p => p.Brand),
                                        x => x.Products.Select(p => p.Category));
            AccountStore.instance.CurrentAccount = t;
        }
    }
}
