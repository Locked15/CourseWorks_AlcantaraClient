using System;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using AlcantaraClient.Classes;
using AlcantaraClient.Entities;
using AlcantaraClient.UserControls;
using AlcantaraClient.Windows.Authorization;
using AlcantaraClient.Windows.SupplementOrder;

namespace AlcantaraClient.Windows.ProductsList
{
    /// <summary>
    /// Логика взаимодействия для FurnitureList.xaml.
    /// </summary>
    public partial class FurnitureList : Window
    {
        public List<Furniture> SelectedFurniture { get; set; } = CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Furnitures.ToList();

        public FurnitureList()
        {
            InitializeComponent();
            InitializeListenersAndContent();
        }

        private void Window_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            foreach (FurnitureListItem item in AllFurnitureList.Items)
            {
                item.Width = GetOptimalWidth();
            }
        }

        private void InitializeListenersAndContent()
        {
            List<CategoryReference> categories = new List<CategoryReference>(1)
            {
                new CategoryReference() 
                { 
                    Name = "Всё" 
                }
            };
            categories.AddRange(CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.CategoryReferences.ToList());

            List<Firm> companies = new List<Firm>(1)
            {
                new Firm()
                {
                    Name = "Всё"
                }
            };
            companies.AddRange(CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Firms.ToList());

            SearchBox.TextChanged += SelectionParametersChanged;

            SortBox.SelectedIndex = 0;
            SortBox.SelectionChanged += SelectionParametersChanged;

            CategoryBox.SelectedIndex = 0;
            CategoryBox.ItemsSource = categories;
            CategoryBox.SelectionChanged += SelectionParametersChanged;

            CompanyBox.SelectionChanged += SelectionParametersChanged;
            CompanyBox.ItemsSource = companies;
            CompanyBox.SelectedIndex = 0;
        }

        private void SelectionParametersChanged(Object sender, dynamic args)
        {
            SelectedFurniture = CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Furnitures.ToList();

            // Поиск.
            if (SearchBox.Text is var search && !String.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                SelectedFurniture = SelectedFurniture.Where(f => f.Name.ToLower().Contains(search) ||
                                    f.Firm.Name.Contains(search) ||
                                    f.CategoryReference.Name.Contains(search)).ToList();
            }

            // Сортировка.
            if (SortBox.SelectedIndex != -1)
            {
                switch (SortBox.SelectedIndex)
                {
                    case 2:
                        SelectedFurniture = SelectedFurniture.OrderBy(f => f.Cost).ToList();
                        break;

                    case 3:
                        SelectedFurniture = SelectedFurniture.OrderBy(f => f.SummaryReviewsGrade).ToList();
                        break;

                    case 4:
                        SelectedFurniture = SelectedFurniture.OrderBy(f => f.Firm.Name).ToList();
                        break;

                    case 6:
                        SelectedFurniture = SelectedFurniture.OrderByDescending(f => f.Cost).ToList();
                        break;

                    case 7:
                        SelectedFurniture = SelectedFurniture.OrderByDescending(f => f.SummaryReviewsGrade).ToList();
                        break;

                    case 8:
                        SelectedFurniture = SelectedFurniture.OrderByDescending(f => f.Firm.Name).ToList();
                        break;
                }
            }

            // Категория.
            if (CategoryBox.SelectedItem is CategoryReference category && category.Name != "Всё")
            {
                SelectedFurniture = SelectedFurniture.Where(f => f.IdCategories == category.ID).ToList();
            }

            // Компания.
            if (CompanyBox.SelectedItem is Firm firm && firm.Name != "Всё")
            {
                SelectedFurniture = SelectedFurniture.Where(f => f.IdFirms == firm.ID).ToList();
            }

            UpdateFurnitureList();
        }

        private void UpdateFurnitureList()
        {
            AllFurnitureList.Items.Clear();

            foreach (Furniture furniture in SelectedFurniture)
            {
                FurnitureListItem item = new FurnitureListItem(furniture)
                {
                    Width = GetOptimalWidth()
                };

                AllFurnitureList.Items.Add(item);
            }

            if (SelectedFurniture.Count < 1)
            {
                MessageBox.Show("По заданным критериям мебель не найдена.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AllFurnitureList_MouseDoubleClick(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (AllFurnitureList.SelectedItem is FurnitureListItem item)
            {
                if (OAuthToEmployee())
                { 
                    IntializeFurnitureWindow window = new IntializeFurnitureWindow(item.Model.CreateCopy());
                    Boolean? result = window.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        if (window.ToDelete)
                        {
                            try
                            {
                                CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Furnitures.Remove(item.Model);
                                CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.SaveChanges();

                                Thread.Sleep(250);
                                SelectionParametersChanged(default, default);

                                MessageBox.Show("Мебель удалена из списка.\nСводный список обновлен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            }

                            catch (Exception ex)
                            {
                                MessageBox.Show($"При удалении мебели произошла ошибка.\n\n{ex.Message}.",
                                                "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                        else
                        {
                            item.Model.MergeWithCopy(window.IntializingModel);

                            CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.SaveChanges();
                            UpdateFurnitureList();
                        }
                    }
                }
            }
        }

        private void CreateNewFurniture_Click(Object sender, RoutedEventArgs e)
        {
            IntializeFurnitureWindow window = new IntializeFurnitureWindow(null);
            Boolean? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Furnitures.Add(window.IntializingModel);
                CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.SaveChanges();

                SelectionParametersChanged(default, default);
            }
        }

        private void AddFurnitureToBusket_Click(Object sender, RoutedEventArgs e)
        {
            if (AllFurnitureList.SelectedItem is FurnitureListItem item)
            {
                SessionData.CurrentOrder.AddFurnitureToOrder(item.Model);
                AllFurnitureList.SelectedItem = item;

                MessageBox.Show("Мебель добавлена в корзину.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Busket_Click(Object sender, RoutedEventArgs e)
        {
            BusketWindow next = new BusketWindow();
            next.ShowDialog();
        }

        private Boolean OAuthToEmployee()
        {
            MessageBox.Show("Чтобы выполнить это действие, необходимы права сотрудника.\n\nВведите его данные.",
                            "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow auth = new MainWindow(true);
            auth.ShowDialog();

            if (auth.ExitCode == 1)
            {
                return true;
            }

            else
            {
                MessageBox.Show("Авторизация не пройдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }

        private Double GetOptimalWidth()
        {
            if (WindowState == WindowState.Maximized)
                return RenderSize.Width - 55;

            else
                return Width - 55;
        }
    }
}
