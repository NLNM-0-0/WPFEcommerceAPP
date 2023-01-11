﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for EditInforDialog.xaml
    /// </summary>
    public partial class EditInforDialog : UserControl {
        public EditInforDialog() {
            InitializeComponent();
        }

        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { 
                SetValue(HeaderProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(EditInforDialog), new PropertyMetadata("Add address"));


    }
}
