using DAL;
using System;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dealers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //A Form Level Object for Establishing a DB Context.
        
        //1-private Dealer SelectedDealer;

        static DealersEntities AppDBContext = new DealersEntities();
        static CollectionViewSource dealersViewSource;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Dealers App Ver " + (Assembly.GetExecutingAssembly().GetName().Version);
            //*
            dealersViewSource = ((CollectionViewSource)(FindResource("dealerViewSource")));
            
            DataContext = this;
            DataGridMain.IsReadOnly = true;
        }
        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            //System.Windows.Data.CollectionViewSource dealerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("dealerViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // dealerViewSource.Source = [generic data source]
            try
            {
                //Loading of Data
                AppDBContext.Dealers.Load();
                dealersViewSource.Source = AppDBContext.Dealers.Local;
                //Loading of Data in ComboBoxes.
                this.LoadCombos();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::Main_Loaded Event",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Code for Disposing off DbContext Object.            
            try
            {
                if (AppDBContext.Database.Connection.State == System.Data.ConnectionState.Open)
                {
                    AppDBContext.Database.Connection.Close();
                    AppDBContext.Dispose();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::Main_Closing Event",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private void Main_Activated(object sender, EventArgs e)
        {
            this.DataContext = dealersViewSource;

           // this.DataGridMain.SelectedIndex = 0;
            //this.DataGridMain.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //1- Check Db is in New Data Add mode / Edit / Delete mode.
            //    var state = dbContext.Entry<DealersEntities>().State;
            //    MessageBox.Show(state.ToString());

            /*
            //dbContext.Configuration.AutoDetectChangesEnabled = true;
            var entries = dbContext.ChangeTracker.Entries();   
            foreach (var entry in entries)
            {
                Console.WriteLine("Entity Name: {0}", entry.Entity.GetType().Name);
                Console.WriteLine("Status: {0}", entry.State);
                dbContext.SaveChanges();
            }
            */
            this.LoadCombos();
            //2- Data should be validated against data integrity checks.
            
            MessageBoxResult result = MessageBox.Show("Do You want to Save a New Record in Database?", "Dealers App",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (IsCleanData())
                    {
                        using (var context = new DealersEntities())
                        {
                            context.Entry(PutDealerData()).State = EntityState.Added;
                            context.SaveChanges();
                            MessageBox.Show("Record has been Saved Successfully!", "Dealers App:: Add New Operation!",
                                MessageBoxButton.OK, MessageBoxImage.Question);
                            ResetControls();
                        }
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString(), "Dealers App::BtnSave_Click Event",
                        MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //This event is for updating a Record in Database.
            int PkValue = 0;
            
            //Todo: Check if there is no data in textbox then dont update Record.
            try
            {
                //Fetch the selected PrimaryKey value of a Record from Grid Control.

                var cur = dealersViewSource.View.CurrentItem as Dealer;
                PkValue = Convert.ToInt32(cur.ID);

                if (PkValue > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Do You want to Update Selected Record in Database?", "Dealers App",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (IsCleanData())
                        {
                            using (var context = new DealersEntities())
                            {
                                 var SelectedDealer = (from d in context.Dealers where d.ID == PkValue select d).First();

                                SelectedDealer.Name = TxtName.Text;
                                SelectedDealer.Cell = Convert.ToInt64(TxtCell.Text);
                                SelectedDealer.Cell2 = Convert.ToInt64(TxtCell2.Text);
                                SelectedDealer.AccountCode = Convert.ToInt32(TxtAcctCode.Text);
                                SelectedDealer.AccountBalance = Convert.ToInt64(TxtAccoutBalance.Text);
                                SelectedDealer.DealsInProgress = Convert.ToInt16(TxtDealsOn.Text);
                                SelectedDealer.DealsClosed = Convert.ToInt16(TxtDealsOff.Text);
                                
                                SelectedDealer.Estate = CboEstates.Text;
                                SelectedDealer.Area = CboArea.Text;
                                SelectedDealer.Especiality = CboEspeciality.Text;
                                
                                context.Entry(SelectedDealer).State = EntityState.Modified;
                                context.SaveChanges();
                                MessageBox.Show("Record has been Updated Successfully!", "Dealers App:: Update Operation!",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                                dealersViewSource.View.Refresh();
                                //1-dealersViewSource.View.MoveCurrentToNext();
                                //Refresh the contents of All Controls.
                                ResetControls();
                            }
                        }
                    }
                }
            }
            //catch (OptimisticConcurrencyException) { }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::BtnUpdate_Click Event",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            //This Event is used to Delete a Record from DB.
            
            try
            {
                //Fetching Current value of DealerID.
                var cur = dealersViewSource.View.CurrentItem as Dealer;
                int PkValue = Convert.ToInt32(cur.ID);
                
                MessageBoxResult result = MessageBox.Show("Do You want to Remove Selected Record From Database?", "Dealers App",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (PkValue > 0)
                    {
                        //Establishing context for Delete operation.
                        using (var context = new DealersEntities())
                        {
                            var PkFind = (from d in context.Dealers where d.ID == PkValue select d).First();
                            context.Dealers.Remove(PkFind);
                            context.SaveChanges();
                        }
                        MessageBox.Show("Record has been Removed Successfully", "Dealers App:: Delete Operation!",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                        //Refresh the contents of All Controls.
                        dealersViewSource.View.Refresh();
                        ResetControls();

                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::BtnDelete_Click Event", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            this.ResetControls();
        }
        private void DataGridMain_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            /*
            int PkValue = 0;
            try
            {
                //PkValue = Convert.ToInt32(GetDataGrid_CellValue());
                var cur = dealersViewSource.View.CurrentItem as Dealer;
                PkValue = cur.ID;
                if (PkValue > 0)
                {
                    this.FindDealer(PkValue);
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::DataGridMain_SelectionChanged Event",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            */
        }
        
        //Helping Routines
        private void LoadCombos()
        {   
            //A Procedure for Loading Combo Boxes from DB.
            using (var context = new DealersEntities())
            {
                //code for fetching data from db and assign it to Disconnected ComboBoxes.
                
                var GetEstates = (from d in context.Dealers
                                          select d.Estate);
                foreach (var i in GetEstates)
                {
                    CboEstates.Items.Add(i);
                }
                var GetAreas = (from d in context.Dealers
                               select d.Area);

                foreach (var i in GetAreas)
                {
                    CboArea.Items.Add(i);
                }
                var GetEspecialities = (from d in context.Dealers
                                        select d.Especiality);
                foreach (var i in GetEspecialities)
                {
                    CboEspeciality.Items.Add(i);
                }
                CboEstates.SelectedIndex = 0;
                CboArea.SelectedIndex = 0;
                CboEspeciality.SelectedIndex = 0;
            }
        }
        private void ResetControls()
        {
            //This will Reset all textboxes text property to null.
            this.TxtName.Text = "";
            this.TxtCell.Text = "";
            this.TxtCell2.Text = "";
            this.TxtAccoutBalance.Text = "";
            this.TxtAcctCode.Text = "";
            this.TxtDealsOn.Text = "";
            this.TxtDealsOff.Text = "";
            this.CboEstates.Text = "";
            this.CboArea.Text = "";
            this.CboEspeciality.Text = "";
            
            DataGridMain.CancelEdit();
            //DataGridMain.Columns.Clear();
            //DataGridMain.ItemsSource = null;

            DataGridMain.Items.Refresh();
            DataGridMain.IsReadOnly = true;
            this.DataGridMain.SelectedIndex = 0;
            this.DataGridMain.Focus();
        }
        private int GetDataGrid_CellValue()
        {
            //This Routine is used to get Primary key value in Selected Row of DataGrid Control.
            // it is not used anywhere.
            
            try
            {
                if (DataGridMain.SelectedItems.Count > 0)
                {
                    DataGrid dataGrid = DataGridMain as DataGrid;
                    DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DataGridCell RowColumn = dataGrid.Columns[0].GetCellContent(row).Parent as DataGridCell;
                    string CellValue = ((TextBlock)RowColumn.Content).Text;
                    return Convert.ToInt32(CellValue);
                }
                else
                { //any code
                }
            }
            catch (ArgumentNullException x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::GetDataGrid_CellValue ArgumentNullException ",
                   MessageBoxButton.OK, MessageBoxImage.Stop);
                DataGridMain.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString(), "Dealers App::GetDataGrid_CellValue Method",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            return 0;
        }
        private bool IsCleanData()
        {
            // Checks whether Data is in acceptable format.
            
            if (HelperRoutines.IsLetter(TxtName) &&
                HelperRoutines.IsCellNumber(TxtCell) &&
                HelperRoutines.IsCellNumber(TxtCell2) &&
                HelperRoutines.IsNumber(TxtAccoutBalance) &&
                HelperRoutines.IsNumber(TxtAcctCode) &&
                HelperRoutines.IsNumber(TxtDealsOn) &&
                HelperRoutines.IsNumber(TxtDealsOff)
                )
            {
                //Signal OK to start further Processing
                return true;
            }
            else
            {
                //Cancel DB Operation.
                return false;
            }
        }
        private Dealer PutDealerData()
        {
            //Todo: Need to work on it, This method may be called when Addnew operation is going to perform.
            try
            {
                Dealer dealer = new Dealer()
                {
                    Name = TxtName.Text,
                    Cell = Convert.ToInt64(TxtCell.Text),
                    Cell2 = Convert.ToInt64(TxtCell2.Text),
                    AccountCode = Convert.ToInt32(TxtAcctCode.Text),
                    AccountBalance = Convert.ToInt64(TxtAccoutBalance.Text),
                    DealsInProgress = Convert.ToInt16(TxtDealsOn.Text),
                    DealsClosed = Convert.ToInt16(TxtDealsOff.Text),
                    Estate = CboEstates.Text.ToString(),
                    Area = CboArea.Text.ToString(),
                    Especiality = CboEspeciality.Text.ToString()
                };
                return dealer;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::PutDealerData Method",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                return null;
            }
        }
        private void DisplayDealerData(Dealer SelectedDealer)
        {
            try
            {
                
                TxtName.Text = SelectedDealer.Name.ToString();
                TxtCell.Text = Convert.ToString(SelectedDealer.Cell);
                TxtCell2.Text = Convert.ToString(SelectedDealer.Cell2);
                TxtAcctCode.Text = Convert.ToString(SelectedDealer.AccountCode);
                TxtAccoutBalance.Text = Convert.ToString(SelectedDealer.AccountBalance);
                TxtDealsOn.Text = Convert.ToString(SelectedDealer.DealsInProgress);
                TxtDealsOff.Text = Convert.ToString(SelectedDealer.DealsClosed);
                //Finding Equal item in ComboBox
                CboEstates.SelectedItem =
                FindItemContaining(CboEstates.Items, SelectedDealer.Estate);
                CboArea.SelectedItem =
                FindItemContaining(CboArea.Items, SelectedDealer.Area);
                CboEspeciality.SelectedItem =
                FindItemContaining(CboEspeciality.Items, SelectedDealer.Especiality);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::DisplayDealerData Method",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        private Dealer FindDealer(int DealerID)
        {
            try
            {
                using (var context = new DealersEntities())
                {
                    Dealer DealerFound = (from d in context.Dealers where d.ID == DealerID select d).First();
                    return DealerFound;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString(), "Dealers App::FindDealer(int DealerID) Method.", 
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                return null;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //dealersViewSource.View.MoveCurrentToLast();

            //--------------------------------------------
            try
            {
                var cur = dealersViewSource.View.CurrentItem as Dealer;

                //if (cur.Name == string.Empty || cur.Name == "")
                //{
                //MessageBox.Show("No Dealer selected!");
                
                
                //    return;
                //}
            }
            catch (NullReferenceException x)
            {
                MessageBox.Show(x.Message);
                
            }

            this.LoadCombos();

            

            //--------------------------------------------
            /*
            cur.Estate = CboEstates.Text;

            AppDBContext.Entry(cur).State = EntityState.Modified;
            AppDBContext.SaveChanges();
            MessageBox.Show("Success !");
            dealersViewSource.View.Refresh();
            */
        }
        private void DataGridMain_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Single click mouse event   
            if (DataGridMain.SelectedItems.Count > 0)
            {
                DataGrid dataGrid = DataGridMain as DataGrid;
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                DataGridCell RowColumn = dataGrid.Columns[0].GetCellContent(row).Parent as DataGridCell;
                string CellValue = ((TextBlock)RowColumn.Content).Text;

                Dealer TempDealer = new Dealer();
                TempDealer = FindDealer( Convert.ToInt32(CellValue));
                DisplayDealerData(TempDealer);
                CellValue = "";
            }   
        }
        private object FindItemContaining(IEnumerable items, string target)
        {
            // Select an item containing the target string.
            //Usage : This Procedure is used in DisplayDealerData() ComboBoxes.
            foreach (object item in items)
                if (item.ToString().Contains(target))
                    return item;

            // Return null;
            return null;
        }
    }
}