using DAL;
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
using System.Windows.Shapes;
using System.Data.Entity;

namespace Dealers
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {


       // private DAL.Dealers dbContext = new DAL.Dealers();

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            using (var context = new DealersEntities())
            {     //work with context here } 
                System.Windows.Data.CollectionViewSource dealerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("dealerViewSource")));
                // Load data by setting the CollectionViewSource.Source property:
                dealerViewSource.Source = context.Dealers.ToList();

            }


            //1-dbContext.Dealers.Load();

            //categoryViewSource.Source = _context.Categories.Local;

            //dealerViewSource.Source = dbContext.Dealers.Local;

            //this.nameTextBox.DataContext = dbContext.Dealers.Local;

            //this.DataGrid1.ItemsSource = dbContext.Dealers.Local;
            //this.estateDataGrid.ItemsSource = dbContext.Dealers.Local;

            /* 2-
            foreach (var dealers in dbContext.Dealers.Local.ToList())
            {
                MessageBox.Show(dealers.Name.ToUpper());
            }
            */

        }
    }
}
