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

namespace Pathfinder
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //test();
        }

        public void test()//todo:delete, this was just a test for the sorted list usage from https://stackoverflow.com/questions/5716423/c-sharp-sortable-collection-which-allows-duplicate-keys
        {
            SortedList<int, string> list = new SortedList<int, string>(new DuplicateComparer<int>());
            list.Add(3, "3");
            list.Add(3, "1");
            list.Add(3, "2");
            string f = list.First().Value;
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GridWindow newWindow = new GridWindow();
            newWindow.Show();
            this.Close();
        }
    }
}
