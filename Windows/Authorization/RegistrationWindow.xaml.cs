using System;
using System.Windows;
using AlcantaraClient.Entities;

namespace AlcantaraClient.Windows.Authorization
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml.
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public User NewUser { get; set; }

        public RegistrationWindow()
        {
            NewUser = new User();

            InitializeComponent();
            DataContext = NewUser;
        }

        private void RegisterNewUser_Click(object sender, RoutedEventArgs e)
        {
            NewUser.Phone = +7777777;

            if (Validation())
            {
                try
                {
                    CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Users.Add(NewUser);
                    CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.SaveChanges();

                    MessageBox.Show("Успешная регистрация!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"В процессе добавления пользователя произошла ошибка:\n{ex.Message}.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Boolean Validation()
        {
            String errors = GetErrorsList();

            if (String.IsNullOrEmpty(errors))
            {
                return true;
            }   
            
            else
            {
                MessageBox.Show($"Обнаружены ошибки:\n{errors}.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }

        private String GetErrorsList()
        {
            String errors = String.Empty;

            if (String.IsNullOrEmpty(NewUser.Name))
                errors += "У пользователя должно быть имя.";

            if (String.IsNullOrEmpty(NewUser.Lastname))
                errors += "У пользователя должна быть фамилия.";

            if (String.IsNullOrEmpty(NewUser.MiddleName))
                errors += "У пользователя должно быть отчество (в случае отсутствия прочерк).";

            if (String.IsNullOrEmpty(NewUser.Login))
                errors += "У пользователя должен быть логин.";

            if (String.IsNullOrEmpty(NewUser.Password))
                errors += "У пользователя должен быть пароль.";

            if (String.IsNullOrEmpty(UserPhone.Text) || !Int64.TryParse(UserPhone.Text, out Int64 val))
                errors += "У пользователя должен быть корректный номер телефона.";

            if (String.IsNullOrEmpty(NewUser.Email) || !NewUser.Email.Contains("@"))
                errors += "У пользователя должна быть электронная почта.";

            return errors;
        }
    }
}
