using BeautySalon_20._101_11.Entity;
using BeautySalon_20._101_11.Windows;
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

namespace BeautySalon_20._101_11.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        beautifulSalonEntities db = new beautifulSalonEntities();
        int page = 0;
        int count;
        string fnd = "";

        public ClientPage()
        {
            InitializeComponent();
            cmbFilter.ItemsSource = FilterList;
            cmbCount.ItemsSource = CountList;
            cmbSorting.ItemsSource = SortingList;
            Load();
        }

        public string[] FilterList { get; set; } =
        {
            "Все",
            "Мужчины",
            "Женщины"
        };

        public string[] CountList { get; set; } =
        {
            "Все",
            "10",
            "50",
            "200"
        };

        public string[] SortingList { get; set; } =
        {
            "Без сортировки",
            "По возрастанию фамилии",
            "По убыванию фамилии",
            "По возрастанию даты последнего посещения",
            "По убыванию даты последнего посещения",
            "По возрастанию посещений",
            "По убыванию посещений"
        };

        public bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        private void turnButton()
        {
            try
            {
                if (page == 0) { back.IsEnabled = false; }
                else { back.IsEnabled = true; };
                if (cmbCount.SelectedIndex != 0 && cmbCount.SelectedItem != null)
                {
                    if ((page + 1) * Int32.Parse(cmbCount.SelectedValue.ToString()) >= count) { forward.IsEnabled = false; }
                    else { forward.IsEnabled = true; }
                }
                else { forward.IsEnabled = false; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Load()
        {
            try
            {
                var result = db.Client.ToList();
                txtFullDataCount.Text = result.Count().ToString();
                if (cmbSorting.SelectedIndex == 1) result = result.OrderBy(c => c.FirstName).ToList();
                if (cmbSorting.SelectedIndex == 2) result = result.OrderByDescending(c => c.FirstName).ToList();
                if (cmbSorting.SelectedIndex == 3) result = result.OrderBy(c => c.DateService).ToList();
                if (cmbSorting.SelectedIndex == 4) result = result.OrderByDescending(c => c.DateService).ToList();
                if (cmbSorting.SelectedIndex == 5) result = result.OrderBy(c => c.CountService).ToList();
                if (cmbSorting.SelectedIndex == 6) result = result.OrderByDescending(c => c.CountService).ToList();

                if (cmbFilter.SelectedIndex != 0) result = result.Where(c => c.GenderCode == cmbFilter.SelectedIndex.ToString()).ToList();
                if (CbBirthDateInMonth.IsChecked == true) result = result.Where(c => c.Birthday.HasValue && c.Birthday.Value.Month == DateTime.Now.Month).ToList();
                result = result.Where(c => c.FirstName.ToLower().Contains(fnd.ToLower())
                              || c.LastName.ToLower().Contains(fnd.ToLower())
                              || c.Patronymic.ToLower().Contains(fnd.ToLower())
                              || c.Email.ToLower().Contains(fnd.ToLower())
                              || c.Phone.ToLower().Contains(fnd.ToLower())).ToList();

                count = result.Count();
                if (cmbCount.SelectedIndex != 0 && cmbCount.SelectedItem != null) result = result.Skip(page * Int32.Parse(cmbCount.SelectedValue.ToString()))
                                                                                                .Take(Int32.Parse(cmbCount.SelectedValue.ToString())).ToList();
                LViewAgents.ItemsSource = result;
                txtDataCount.Text = count.ToString();

                int ost = 0;
                int pag = 1;
                if (cmbCount.SelectedIndex != 0 && cmbCount.SelectedItem != null)
                {
                    ost = count % Int32.Parse(cmbCount.SelectedValue.ToString());
                    pag = (count - ost) / Int32.Parse(cmbCount.SelectedValue.ToString());
                }
                if (ost > 0) pag++;
                pagin.Children.Clear();
                for (int i = 0; i < pag; i++)
                {
                    Button myButton = new Button();
                    myButton.Height = 30;
                    myButton.Content = i + 1;
                    myButton.Width = 20;
                    myButton.Background = new SolidColorBrush(Colors.White);
                    myButton.BorderBrush = new SolidColorBrush(Colors.White);
                    myButton.Cursor = Cursors.Hand;
                    myButton.HorizontalAlignment = HorizontalAlignment.Center;
                    myButton.Tag = i;
                    myButton.Click += new RoutedEventHandler(paginButton_Click);
                    pagin.Children.Add(myButton);
                }
                turnButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void paginButton_Click(object sender, RoutedEventArgs e)
        {
            page = Convert.ToInt32(((Button)sender).Tag.ToString());
            Load();
        }

        private void cmbCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            page = 0;
            fnd = ((TextBox)sender).Text;
            Load();
        }

        private void cmbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Load();
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void CbBirthDateInMonth_Unchecked(object sender, RoutedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void ClientService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClientServiceWindow dlg = new ClientServiceWindow(LViewAgents.SelectedItem as Client, db, this);
                dlg.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (LViewAgents.SelectedItems.Count > 0)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить {LViewAgents.SelectedItems.Count} агента?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        StringBuilder errors = new StringBuilder();
                        var selected = LViewAgents.SelectedItems.Cast<Client>().ToArray();
                        int clientCount = 0;
                        foreach (var item in selected)
                        {
                            if (item.ClientService.Count > 0)
                            {
                                errors.AppendLine($"Клиент {item.ID} не может быть удален, т.к. есть информация о посещениях");
                            }
                            else
                            {
                                foreach (Tag tag in item.Tag)
                                {
                                    db.Tag.Remove(tag);
                                }
                                db.Client.Remove(item);
                                db.SaveChanges();
                                clientCount++;
                            }
                        }
                        if (errors.Length > 0)
                        {
                            MessageBox.Show(errors.ToString());
                        }
                        if (clientCount != 0)
                        {
                            MessageBox.Show($"Удалено агентов: {clientCount}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Выберите агента для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditClientWindow dlg = new AddEditClientWindow(null, db, this);
            dlg.Show();
        }

        private void rebButton_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void LViewAgents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsWindowOpen<AddEditClientWindow>())
            {
                MessageBox.Show("Окно Редактирования уже открыто.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                AddEditClientWindow dlg = new AddEditClientWindow(LViewAgents.SelectedItem as Client, db, this);
                dlg.Show();
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (page >= 0)
            {
                page--;
                Load();
            }
        }

        private void CbBirthDateInMonth_Checked(object sender, RoutedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbCount.SelectedIndex != 0 && cmbCount.SelectedItem != null)
                {
                    if (page < db.Client.Count() / Int32.Parse(cmbCount.SelectedValue.ToString()) - 1)
                    {
                        page++;
                        Load();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
