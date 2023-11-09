using BeautySalon_20._101_11.Entity;
using BeautySalon_20._101_11.Pages;
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

namespace BeautySalon_20._101_11.Windows
{
    /// <summary>
    /// Логика взаимодействия для ClientServiceWindow.xaml
    /// </summary>
    public partial class ClientServiceWindow : Window
    {
        Client client = new Client();
        private beautifulSalonEntities db;
        private ClientPage clientPage;
        public ClientServiceWindow(Client client, beautifulSalonEntities db, ClientPage clientPage)
        {
            InitializeComponent();
            this.client = client;
            this.db = db;
            this.clientPage = clientPage;
            DataContext = this.client;
            Load();
        }

        public void Load()
        {
            TbClientInfo.Text = $"{client.FirstName} {client.LastName} {client.Patronymic}({client.ID})";
            if (client.ServiceList.Count > 0)
            {
                LViewService.ItemsSource = client.ServiceList;
            }
            else
            {
                LViewService.Visibility = Visibility.Hidden;
                spServiceInfo.Children.Clear();
                TextBlock tb = new TextBlock();
                tb.Text = "У данного клиента нет посещений";
                tb.FontSize = 22;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                spServiceInfo.Children.Add(tb);
            }
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (clientPage != null)
                {
                    clientPage.Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefrService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LViewService.SelectedItem != null)
                {
                    ClientService clientService = LViewService.SelectedItem as ClientService;
                    var service = db.Service.Where(s => s.ID == clientService.ServiceID).First();
                    if (service != null)
                    {
                        EditService dlg = new EditService(service, db, this);
                        dlg.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите сервис для изменения", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelService_Click(object sender, RoutedEventArgs e)
        {
            if (LViewService.SelectedItems.Count > 0)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить {LViewService.SelectedItems.Count} посещение?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        StringBuilder errors = new StringBuilder();
                        var selected = LViewService.SelectedItems.Cast<ClientService>().ToArray();
                        int serviceCount = 0;
                        foreach (var item in selected)
                        {
                            db.ClientService.Remove(db.ClientService.Where(cs => cs.ServiceID == item.ID || cs.ClientID == client.ID).First());
                            db.SaveChanges();
                            serviceCount++;

                        }
                        if (errors.Length > 0)
                        {
                            MessageBox.Show(errors.ToString());
                        }
                        if (serviceCount != 0)
                        {
                            MessageBox.Show($"Удалено сервисов: {serviceCount}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        Load();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
            else
            {
                MessageBox.Show("Выберите сервис для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
