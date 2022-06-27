using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using AlcantaraClient.Classes;
using AlcantaraClient.Entities;
using AlcantaraClient.Windows.ProductsList;

namespace AlcantaraClient.Windows.Authorization
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Нужно ли текущее окно для проверки уровня доступа.
        /// </summary>
        private Boolean toConfirmAccess;

        /// <summary>
        /// Код завершения работы текущего окна.
        /// </summary>
        public Int32 ExitCode { get; private set; } = -1;

        /// <summary>
        /// Текущий логин.
        /// </summary>
        public String CurrentLogin { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            StartPosition.Focus();
        }

        public MainWindow(Boolean toConfirmAccess) : this()
        {
            this.toConfirmAccess = toConfirmAccess;

            CreateNewAccount.IsEnabled = false;
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TryToLogin_Click(default, default);
            }
        }

        private void TryToLogin_Click(object sender, RoutedEventArgs e)
        {
            String password = UserPassword.Password;

            if (CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Users.Where(u => 
            u.Login == CurrentLogin && u.Password == password).FirstOrDefault() is var user && user != null)
            {
                SessionData.CurrentUser = user;
                SessionData.CurrentOrder = new Order()
                {
                    OrderDate = DateTime.Now,
                    IdUsers = SessionData.CurrentUser.ID,
                    User = SessionData.CurrentUser,
                    Baskets = new List<Basket>(1)
                };

                ExitCode = 0;

                OpenNextWindow();
            }

            else if (CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Employees.Where(emp =>
            emp.Login == CurrentLogin && emp.Password == password).FirstOrDefault() is var employee && employee != null)
            {
                ExitCode = 1;

                if (toConfirmAccess)
                {
                    Close();
                }

                else
                {
                    MessageBox.Show("Идите работать.\nДанное приложение самообслуживания предназначено для клиентов.", "Вы даже не клиент!",
                                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

            else
            {
                ExitCode = 2;

                MessageBox.Show("Аккаунт не найден.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenNextWindow()
        {
            FurnitureList window = new FurnitureList();

            window.Show();
            Close();
        }

        private void CreateNewAccount_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registration = new RegistrationWindow();
            registration.ShowDialog();
        }
    }
}
