using BeautySalon_20._101_11.Classes;
using BeautySalon_20._101_11.Entity;
using Microsoft.Win32;
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
    /// Логика взаимодействия для EditService.xaml
    /// </summary>
    public partial class EditService : Window
    {
        Service service = new Service();
        beautifulSalonEntities db;
        ClientServiceWindow clientServiceWindow;
        public EditService(Service service, beautifulSalonEntities db, ClientServiceWindow clientServiceWindow)
        {
            InitializeComponent();
            this.service = service;
            this.db = db;
            this.clientServiceWindow = clientServiceWindow;
            DataContext = service;
        }

        private void Errors(bool expression, StringBuilder errors, string message)
        {
            if (expression)
            {
                errors.AppendLine(message);
            }
        }

        private void SaveInDB(string text)
        {
            try
            {
                db.SaveChanges();
                this.Close();
                MessageBox.Show(text, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEnterImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog GetImageDialog = new OpenFileDialog();

                GetImageDialog.Filter = "Файлы изображений: (*.png, *.jpg, *jpeg)| *.png; *.jpg; *jpeg";
                GetImageDialog.InitialDirectory = "BeautySalon-20.101-11\\Images\\picture";
                if (GetImageDialog.ShowDialog() == true) service.MainImagePath = GetImageDialog.SafeFileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string defaultImagePath = (string)FindResource("defaultImage");
                if (!string.IsNullOrEmpty(defaultImagePath))
                {
                    BitmapImage defaultImage = new BitmapImage(new Uri(defaultImagePath));
                    imgPhoto.Source = defaultImage;
                }
                service.MainImagePath = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelSer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.Service.Remove(service);
                SaveInDB("Удаление информации о сервесе завешено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefrSer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (int.TryParse(TbDiscountServ.Text, out int discount))
                {
                    Errors(discount > 100, errors, "Скидка не может быть больше 100%");
                }
                Errors(TbTitleServ.Text == ""
                    || TbDurationInSecondsServ.Text == ""
                    || TbCostServ.Text == "", errors, "Не заполнена важная ифнормация!");
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
                if (TbDiscountServ.Text == "") service.Discount = null;
                SaveInDB("Обновление информации о сервисе завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TbDescriptionServ_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (TbDescriptionServ.Text.Length >= 200 && !e.Text.Equals("\b"))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = !Validatoin.ValidTitle(e.Text);
            }
        }

        private void TbDiscountServ_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (TbDiscountServ.Text.Length >= 3 && !e.Text.Equals("\b"))
            {
                e.Handled = true;
            }
            else
            {
                string newText = TbDiscountServ.Text.Insert(TbDiscountServ.CaretIndex, e.Text);
                if (!Validatoin.ValidDiscount(newText) || (int.TryParse(newText, out int discount) && discount > 100))
                {
                    e.Handled = true;
                }
            }
        }

        private void TbCostServ_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (TbCostServ.Text.Contains(".") && e.Text == ".")
            {
                e.Handled = true;
            }
            else if (TbCostServ.Text.Length >= 11 && !e.Text.Equals("\b"))
            {
                e.Handled = true;
            }
            else
            {
                string newText = TbCostServ.Text.Insert(TbCostServ.CaretIndex, e.Text);
                e.Handled = !Validatoin.ValidDiscount(newText);
            }
        }

        private void TbDurationInSecondsServ_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (TbDurationInSecondsServ.Text.Length >= 9 && !e.Text.Equals("\b"))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = !Validatoin.ValidCost(e.Text);
            }
        }

        private void TbTitleServ_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (TbTitleServ.Text.Length >= 50 && !e.Text.Equals("\b"))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = !Validatoin.ValidTitle(e.Text);
            }
        }
    }
}
