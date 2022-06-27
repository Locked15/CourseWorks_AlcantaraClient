using System;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Win32;
using AlcantaraClient.Classes;
using AlcantaraClient.Entities;
using AlcantaraClient.UserControls;
using AlcantaraClient.Windows.Authorization;
using AlcantaraClient.Classes.DocumentFormation;

namespace AlcantaraClient.Windows.SupplementOrder
{
    /// <summary>
    /// Логика взаимодействия для BusketWindow.xaml.
    /// </summary>
    public partial class BusketWindow : Window
    {
        #region Свойства.

        public List<Basket> Baskets { get; set; }
        #endregion

        #region Функции инициализации.

        public BusketWindow()
        {
            InitializeComponent();
            FillFurnituresList();
            UpdateOrderInfo();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Grid_ThirdLevel.Visibility = Visibility.Hidden;

            EmployeeSelector.ItemsSource = CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Employees.ToList();
            DateSelector.DisplayDateStart = DateTime.Now.AddDays(-1);

            EmployeeSelector.SelectedItem = SessionData.CurrentOrder.Employee;
            DateSelector.SelectedDate = SessionData.CurrentOrder.OrderDate;
        }

        private void FillFurnituresList()
        {
            Baskets = SessionData.CurrentOrder.Baskets.ToList();
            AllFurnituresInOrder.Items.Clear();

            foreach (Furniture furniture in Baskets.Select(b => b.Furniture))
            {
                FurnitureListItem item = new FurnitureListItem(furniture)
                {
                    Width = GetOptimalWidth()
                };
                item.ContextMenu_AddToOrder.Click += (sender, e) =>
                {
                    UpdateOrderInfo();

                    AllFurnituresInOrder.SelectedItem = null;
                };
                item.ContextMenu_RemoveFromOrder.Click += (sender, e) =>
                {
                    FillFurnituresList();
                    UpdateOrderInfo();
                };

                AllFurnituresInOrder.Items.Add(item);
            }
        }

        private void UpdateOrderInfo()
        {
            SumDisplay.Text = SessionData.CurrentOrder.Sum.ToString("0,00");
        }
        #endregion

        #region Функции изменения заказа.

        private void AllFurnituresInOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllFurnituresInOrder.SelectedItem is FurnitureListItem item)
            {
                Grid_ThirdLevel.Visibility = Visibility.Visible;
                CurrentFurnitureQuantity.Text = Baskets.FirstOrDefault(b => b.IdFurniture == item.Model.ID)?.Quantity.ToString();
            }

            else
            {
                Grid_ThirdLevel.Visibility = Visibility.Hidden;
            }
        }

        private void CurrentFurnitureQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            Furniture selected = (AllFurnituresInOrder.SelectedItem as FurnitureListItem)?.Model;

            if (Int32.TryParse(CurrentFurnitureQuantity.Text, out Int32 quantity) && selected != null)
            {
                if (quantity <= 0)
                {
                    MessageBoxResult result = MessageBox.Show($"Такое количество ({quantity}) приведет к удалению товара.\n\nВы уверены?",
                                                              "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SessionData.CurrentOrder.RemoveFurnitureFromOrder(selected);

                        FillFurnituresList();
                    }

                    else
                    {
                        CurrentFurnitureQuantity.Text = 1.ToString();
                    }
                }

                else
                {
                    Baskets.FirstOrDefault(b => b.IdFurniture == selected.ID).Quantity = quantity;
                }

                SessionData.CurrentOrder.UpdateSum();
                UpdateOrderInfo();
            }

            else
            {
                MessageBox.Show("Некорректное количество мебели.\nСброс...", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                CurrentFurnitureQuantity.Text = 1.ToString();
            }
        }

        private void EmployeeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeSelector.SelectedItem is Employee employee)
            {
                SessionData.CurrentOrder.Employee = employee;
                SessionData.CurrentOrder.IdEmployee = employee.ID;
            }
        }

        private void DateSelector_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SessionData.CurrentOrder.OrderDate = DateSelector.SelectedDate.Value;
        }
        #endregion

        #region Функции кнопок управления.

        private void SaveOrderInDb_Click(Object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                if (SessionData.CurrentOrder.ID == 0)
                {
                    try
                    {
                        CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Baskets.AddRange(SessionData.CurrentOrder.Baskets);
                        CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Orders.Add(SessionData.CurrentOrder);

                        CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.SaveChanges();

                        MessageBox.Show("Заказ добавлен в БД.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show($"В процессе сохранения данных произошла ошибка:\n{ex.Message}.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                else
                {
                    CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.SaveChanges();

                    MessageBox.Show("Изменения сохранены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                JoyOfDocumentCreation(0);
            }
        }

        private void CreateReportAboutOrders_Click(Object sender, RoutedEventArgs e)
        {
            if (ConfirmAccessLevel())
            {
                JoyOfDocumentCreation(1);
            }
        }
        #endregion

        #region Функции валидации.

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

            if (!DateSelector.SelectedDate.HasValue)
                errors += "Не выбрана дата заказа.\n";

            if (EmployeeSelector.SelectedItem == null)
                errors += "Не выбран сотрудник.";

            return errors;
        }

        private Boolean ConfirmAccessLevel()
        {
            MessageBox.Show("Для выполнения этого действия требуются права сотрудника.\n\nВведите соответствующие данные.",
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            MainWindow access = new MainWindow(true);
            access.ShowDialog();

            if (access.ExitCode == 1)
            {
                return true;
            }

            else
            {
                MessageBox.Show("Доступ отклонен.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }
        #endregion

        #region Функции формирования документа.

        private void JoyOfDocumentCreation(Int32 docType)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите сформировать документ?", "Вопрос", 
                                                      MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Формирование документа",
                    Filter = "Документы Microsoft Word (*.docx)|*.docx|Все файлы (*.*)|*.*",
                    OverwritePrompt = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };
                Boolean? resultToPath = saveFileDialog.ShowDialog();

                if (resultToPath.HasValue && resultToPath.Value)
                {
                    BaseDocument documentBuilder;

                    if (docType == 0)
                        documentBuilder = new OrderDocument(saveFileDialog.FileName);

                    else
                        documentBuilder = new ReportDocument(saveFileDialog.FileName);

                    InitializeDocumentCreation(documentBuilder);
                }
            }
        }

        private void InitializeDocumentCreation(BaseDocument builder)
        {
            try
            {
                Task.Run(() =>
                {
                    MessageBox.Show("Начато формирование документа...", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                builder.BeginDocumentCreation();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"В процессе формирования документа произошла ошибка: \n{ex.Message}.", "Ошибка!", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Функции смены размера окна.

        private void Window_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            foreach (FurnitureListItem item in AllFurnituresInOrder.Items)
            {
                item.Width = GetOptimalWidth();
            }
        }

        private Double GetOptimalWidth()
        {
            if (WindowState == WindowState.Maximized)
                return RenderSize.Width * 0.6575;

            else
                return Width * 0.6575;
        }
        #endregion
    }
}
