using System;
using System.Linq;
using System.Collections.Generic;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using AlcantaraClient.Dialogs;
using AlcantaraClient.Entities;

namespace AlcantaraClient.Classes.DocumentFormation
{
    public class ReportDocument : BaseDocument
    {
        #region Названия закладок в документе.

        /// <summary>
        /// Константа с названием закладки, содержащей ID отчета.
        /// </summary>
        public const String ReportID = "ReportId";

        /// <summary>
        /// Константа с названием закладки, содержащей количество заказов в отчете.
        /// </summary>
        public const String OrderCount = "OrderCount";

        /// <summary>
        /// Константа с названием закладки, содержащей стартовую дату для заказов.
        /// </summary>
        public const String ReportBeginDate = "ReportBeginDate";

        /// <summary>
        /// Константа с названием закладки, содержащей конечную дату для заказов.
        /// </summary>
        public const String ReportEndDate = "ReportEndDate";

        /// <summary>
        /// Константа с названием закладки, содержащей дату и время составления отчёта.
        /// </summary>
        public const String ReportDateTime = "ReportFormationDateTime";

        /// <summary>
        /// Константа с названием закладки, содержащей ID заказов.
        /// </summary>
        public const String OrderID_List = "OrderId";

        /// <summary>
        /// Константа с названием закладки, содержащей имена клиентов.
        /// </summary>
        public const String OrderClientName_List = "ClientName";

        /// <summary>
        /// Константа с названием закладки, содержащей названия мебели.
        /// </summary>
        public const String OrderFurnituriesName_List = "FurNames";

        /// <summary>
        /// Константа с названием закладки, содержащей стоимость мебели.
        /// </summary>
        public const String OrderFullCost_List = "OrderFullCost";

        /// <summary>
        /// Константа с названием закладки, содержащей дату заказа.
        /// </summary>
        public const String OrderDate_List = "OrderDate";

        /// <summary>
        /// Константа с названием закладки, содержащей успех выполнения заказа.
        /// </summary>
        public const String OrderIsComplete_List = "OrderComplition";
        #endregion

        public ReportDocument(String path) : base(path, 1)
        {

        }

        public override void BeginDocumentCreation()
        {
            var dialog = new SelectDatesToCreateReportDialogWindow();
            dialog.ShowDialog();

            var marks = GetBookmarks();
            FillBaseBookmarks(marks, dialog.StartDate, dialog.EndDate);

            BeginToCreateRows(dialog.StartDate, dialog.EndDate);

            SaveDocument();
        }

        private void FillBaseBookmarks(List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> marks, DateTime start, DateTime end)
        {
            List<Order> ordersInCriteria = CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Orders.Where(o =>
                                           o.OrderDate >= start && o.OrderDate <= end).ToList();

            foreach (var mark in marks)
            {
                switch (mark.bookmark.name)
                {
                    case ReportID:
                        mark.paragraph.ReplaceText("____", new Random().Next(100, 1001).ToString());
                        break;

                    case OrderCount:
                        mark.paragraph.ReplaceText("_____", ordersInCriteria.Count.ToString());
                        break;

                    case ReportBeginDate:
                        mark.paragraph.ReplaceText("_______", start.ToString("dd.MM.yyyy"));
                        break;

                    case ReportEndDate:
                        mark.paragraph.ReplaceText("_______", end.ToString("dd.MM.yyyy"));
                        break;

                    case ReportDateTime:
                        mark.paragraph.ReplaceText("__________", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                        break;
                }
            }
        }

        private void BeginToCreateRows(DateTime start, DateTime end)
        {
            List<Order> ordersInCriteria = CourseWorks_ThirdCourse_AlcantaraDbEntities.Instance.Orders.Where(o =>
                                  o.OrderDate >= start && o.OrderDate <= end).ToList();
            XWPFTable table = Document.Tables.FirstOrDefault();

            if (table != null)
            {
                for (int i = 0; i < ordersInCriteria.Count; i++)
                {
                    Order order = ordersInCriteria[i];
                    XWPFTableRow row = table.CreateRow();

                    for (int j = 0; j < 6; j++)
                    {
                        XWPFTableCell cell = row.GetCell(j);
                        XWPFParagraph paragraph = cell.Paragraphs[0];
                        
                        switch (j)
                        {
                            case 0:
                                InsertInfoAbountOrderId(paragraph, order);
                                break;

                            case 1:
                                InsertInfoAboutClient(paragraph, order);
                                break;

                            case 2:
                                InsertInfoAboutFurniture(paragraph, order);
                                break;

                            case 3:
                                InsertInfoAboutCosts(paragraph, order);
                                break;

                            case 4:
                                InsertInfoAboutOrderDate(paragraph, order);
                                break;

                            default:
                                InsertInfoAboutOrderCompilation(paragraph, order);
                                break;
                        }
                    }
                }
            }
        }

        private void InsertInfoAbountOrderId(XWPFParagraph paragraph, Order order)
        {
            XWPFRun run = paragraph.CreateRun();
            run.SetFontFamily("Georgia", FontCharRange.Ascii);
            run.FontSize = 16;

            run.SetText(order.ID.ToString());
        }

        private void InsertInfoAboutClient(XWPFParagraph paragraph, Order order)
        {
            XWPFRun run = paragraph.CreateRun();
            run.SetFontFamily("Georgia", FontCharRange.Ascii);
            run.FontSize = 16;

            run.SetText(order.User.FullName);
        }

        private void InsertInfoAboutFurniture(XWPFParagraph paragraph, Order order)
        {
            foreach (Furniture furniture in order.Baskets.Select(b => b.Furniture))
            {
                Basket basket = order.Baskets.FirstOrDefault(b => b.IdFurniture == furniture.ID);

                XWPFRun run = paragraph.CreateRun();
                run.SetFontFamily("Georgia", FontCharRange.Ascii);
                run.FontSize = 16;

                run.SetText($"{furniture.Name}, {basket.Quantity}шт;");

                run.AddBreak(BreakType.TEXTWRAPPING);
            }
        }

        private void InsertInfoAboutCosts(XWPFParagraph paragraph, Order order)
        {
            foreach (Furniture furniture in order.Baskets.Select(b => b.Furniture))
            {
                XWPFRun run = paragraph.CreateRun();
                run.SetFontFamily("Georgia", FontCharRange.Ascii);
                run.FontSize = 16;

                run.SetText(furniture.Cost.ToString("0,00"));

                run.AddBreak(BreakType.TEXTWRAPPING);
            }
        }

        private void InsertInfoAboutOrderDate(XWPFParagraph paragraph, Order order)
        {
            XWPFRun run = paragraph.CreateRun();
            run.SetFontFamily("Georgia", FontCharRange.Ascii);
            run.FontSize = 16;

            run.SetText(order.OrderDate.ToString("dd.MM.yyyy"));
        }

        private void InsertInfoAboutOrderCompilation(XWPFParagraph paragraph, Order order)
        {
            XWPFRun run = paragraph.CreateRun();
            run.SetFontFamily("Georgia", FontCharRange.Ascii);
            run.FontSize = 16;

            run.SetText(order.OrderDate > DateTime.Now ? "В процессе" : "Выполнен");
        }
    }
}
