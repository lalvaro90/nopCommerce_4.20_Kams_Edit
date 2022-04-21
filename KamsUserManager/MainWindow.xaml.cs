using KamsUserManager.Data;
using System;
using System.Collections.Generic;
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

        Customer SelecteCustomer;

        public MainWindow()
        {
            InitializeComponent();
            entities = new kams_nopcEntities();
            prgLoading.Visibility = Visibility.Visible;
            _ = loadClientListAsync();
        }


        private async Task loadClientListAsync() {
            customerList = await entities.Customers.Where(x => x.Active == true).ToListAsync();
            prgLoading.Visibility = Visibility.Hidden;
        }

       

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            EncryptionService service = new EncryptionService();

            var cus = customerList.FirstOrDefault(x=> x.Email == txtEmailToSearch.Text);

            if (cus == null) {
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

    }
}
