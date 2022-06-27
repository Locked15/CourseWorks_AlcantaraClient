using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AlcantaraClient.Classes;
using AlcantaraClient.Entities;

namespace AlcantaraClient.UserControls
{
    /// <summary>
    /// Логика взаимодействия для FurnitureListItem.xaml.
    /// </summary>
    public partial class FurnitureListItem : UserControl
    {
        public Furniture Model { get; set; }

        public FurnitureListItem(Furniture model)
        {
            Model = model;

            InitializeComponent();
            DataContext = Model;
        }

        private void UserControl_Loaded(Object sender, System.Windows.RoutedEventArgs e) =>
                     FurnitureImage.Source = new BitmapImage(new Uri("../Resources/!NothingHere.png", UriKind.Relative));

        private void ContextMenu_AddToOrder_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            SessionData.CurrentOrder.AddFurnitureToOrder(Model);

            MessageBox.Show("Товар добавлен в корзину.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ContextMenu_RemoveFromOrder_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить мебель из корзины?", "Внимание",
                                                      MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SessionData.CurrentOrder.RemoveFurnitureFromOrder(Model);

                MessageBox.Show("Товар удален из корзины.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
