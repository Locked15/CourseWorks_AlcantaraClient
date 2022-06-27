using System;
using System.Linq;
using System.Windows;
using AlcantaraClient.Entities;

namespace AlcantaraClient.Windows.ProductsList
{
    /// <summary>
    /// Логика взаимодействия для IntializeFurnitureWindow.xaml.
    /// </summary>
    public partial class IntializeFurnitureWindow : Window
    {
        public Boolean ToDelete { get; set; } = false;

        public Furniture IntializingModel { get; set; }

        public IntializeFurnitureWindow(Furniture model)
        {
            IntializingModel = model ?? new Furniture();

            InitializeComponent();
            DataContext = IntializingModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FurnitureCategory.ItemsSource = CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.CategoryReferences.ToList();

            FurnitureCompany.ItemsSource = CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Firms.ToList();
        }

        private void FurnitureCost_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (Decimal.TryParse(FurnitureCost.Text.Replace(".", ","), out Decimal newCost))
            {
                IntializingModel.Cost = newCost;
            }

            else
            {
                MessageBox.Show("Некорректная стоимость.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                FurnitureCost.Text = "1";
            }
        }

        private void SaveFurniture_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                DialogResult = true;

                Close();
            }
        }

        private void RemoveFurniture_Click(object sender, RoutedEventArgs e)
        {
            if (IntializingModel.ID == 0)
            {
                MessageBox.Show("Невозможно удалить ещё не созданную мебель.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                MessageBoxResult result = MessageBox.Show("Вы ТОЧНО уверены?\n\nЭто действие необратимо!", 
                                          "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    ToDelete = true;
                    DialogResult = true;

                    Close();
                }
            }
        }

        private Boolean Validate()
        {
            String errors = GetErrorsString();

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

        private String GetErrorsString()
        {
            String errors = String.Empty;

            if (String.IsNullOrEmpty(FurnitureCost.Text) || IntializingModel.Cost < 1)
                errors += "Некорректная стоимость мебели.\n";

            if (String.IsNullOrEmpty(IntializingModel.Name))
                errors += "У мебели должно быть название.\n";

            if (IntializingModel.CategoryReference == null)
                errors += "У мебели должна быть категория.\n";

            if (IntializingModel.Firm == null)
                errors += "Не указана компания.";

            return errors;
        }
    }
}
