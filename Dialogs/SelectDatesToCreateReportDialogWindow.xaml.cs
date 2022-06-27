using System;
using System.Windows;

namespace AlcantaraClient.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для SelectDatesToCreateReportDialogWindow.xaml.
    /// </summary>
    public partial class SelectDatesToCreateReportDialogWindow : Window
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public SelectDatesToCreateReportDialogWindow()
        {
            StartDate = DateTime.Now.AddDays(-7);
            EndDate = DateTime.Now.AddDays(7);

            InitializeComponent();
            DataContext = this;
        }

        private void SaveDates_Click(Object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
