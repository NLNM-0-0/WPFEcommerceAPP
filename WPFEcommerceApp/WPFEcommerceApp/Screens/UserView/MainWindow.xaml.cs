using System;
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

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Student> Students { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Students = new List<Student>();
            Students.Add(new Student() { Name = "a", Id = 1, Note = "ko"});
            Students.Add(new Student() { Name = "b", Id = 2, Note = "ko" });
            Students.Add(new Student() { Name = "c", Id = 3, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            Students.Add(new Student() { Name = "d", Id = 4, Note = "ko" });
            this.DataContext = new UserViewModel();
        }
        public class Student
        {
            public string Name { get; set; }
            public int Id { get; set; }

            public string Note { get; set; }
        }
    }
}
