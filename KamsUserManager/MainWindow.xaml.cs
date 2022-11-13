using KamsUserManager.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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

namespace KamsUserManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        kams_nopcEntities entities;
        List<Customer> customerList;
        public static ObservableCollection<Product> Products { get; set; }

        Customer SelecteCustomer;

        public MainWindow()
        {
            InitializeComponent();
            entities = new kams_nopcEntities();
            prgLoading.Visibility = Visibility.Visible;
            // _ = loadClientListAsync();
            Products = new ObservableCollection<Product>();
            gdr_products.DataContext = this;
        }

        #region Products 

        public async void SearchProducts()
        {
            var criteria = txt_search.Text;
            if (string.IsNullOrWhiteSpace(criteria))
            {

                var sb = new StringBuilder();
                sb.AppendLine("Error: Ingrese un criterio de busqueda.");
                MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else {
                try
                {
                    btn_search.IsEnabled = false;
                    Products.Clear();
                    Cursor = Cursors.Wait;
                    foreach (var p in await entities.Products.Where(x => x.FullDescription.Contains(criteria) || x.Name.Contains(criteria) || x.Sku == criteria).ToListAsync())
                    {
                        Products.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Error al buscar los datos.");
                    sb.AppendLine($"Detalles: {ex.Message}");
                    MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally {
                    Cursor = Cursors.Arrow;
                    btn_search.IsEnabled = true;
                }
            }
            
        }
        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            SearchProducts();
        }
        private async void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                foreach (var prod in Products)
                {
                    var p = await entities.Products.FirstAsync(x => x.Id == prod.Id);
                    p.Published = prod.Published;
                }
                await entities.SaveChangesAsync();
                Products.Clear();
                MessageBox.Show("Productos Actualizado Correctamente!", "Informacion", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Error al actualizar los datos.");
                sb.AppendLine($"Detalles: {ex.Message}");
                MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Cursor = Cursors.Arrow;
                txt_search.Text = String.Empty;
            }

        }

        #endregion

        #region User Data

        private async Task loadClientListAsync()
        {
            customerList = await entities.Customers.Where(x => x.Active == true).ToListAsync();
            prgLoading.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            EncryptionService service = new EncryptionService();

            var cus = customerList.FirstOrDefault(x => x.Email == txtEmailToSearch.Text);

            if (cus == null)
            {
                cus = customerList.Last();
                cus.Email = txtEmailToSearch.Text;
            }

            var salt = service.CreateSaltKey(5);
            cus.CustomerPasswords.Add(new CustomerPassword()
            {
                PasswordSalt = salt,
                Password = service.CreatePasswordHash(txtPassword.Text, salt, "SHA512"),
                PasswordFormatId = 1,
            });

            entities.Entry<Customer>(cus).State = EntityState.Modified;
            entities.SaveChanges();

        }

        #endregion

    }
}
