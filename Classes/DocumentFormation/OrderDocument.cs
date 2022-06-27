using System;
using System.Linq;
using System.Collections.Generic;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using AlcantaraClient.Entities;

namespace AlcantaraClient.Classes.DocumentFormation
{
    public class OrderDocument : BaseDocument
    {
        #region Названия закладок в документе.

        /// <summary>
        /// Константа с названием закладки, содержащей ID заказа.
        /// </summary>
        public const String OrderID = "OrderId";

        /// <summary>
        /// Константа с названием закладки, содержащей дату заказа.
        /// </summary>
        public const String OrderDate = "OrderDate";

        /// <summary>
        /// Константа с названием закладки, содержащей ФИО клиента.
        /// </summary>
        public  const String ClientInfo = "ClientInfo";

        /// <summary>
        /// Константа с названием закладки, содержащей ID мебели.
        /// </summary>
        public const String FurnitureID = "FurId";

        /// <summary>
        /// Константа с названием закладки, содержащей название мебели.
        /// </summary>
        public const String FurnitureName = "FurName";

        /// <summary>
        /// Константа с названием закладки, содержащей название компании мебели.
        /// </summary>
        public const String FurnitureFirmName = "FurFirmName";

        /// <summary>
        /// Константа с названием закладки, содержащей название категории мебели.
        /// </summary>
        public const String FurnitureCategoryName = "FurCategoryName";

        /// <summary>
        /// Константа с названием закладки, содержащей стоимость одной единицы мебели.
        /// </summary>
        public const String OneFurnitureCost = "FurCostForOne";

        /// <summary>
        /// Константа с названием закладки, содержащей количество данной мебели в заказе.
        /// </summary>
        public const String FurnitureCountInOrder = "FurCount";

        /// <summary>
        /// Константа с названием закладки, содержащей общую стоимость заказа.
        /// </summary>
        public const String TotalOrderCost = "TotalOrderCost";
        #endregion

        public OrderDocument(String path) : base(path, 0)
        { 
            
        }

        public override void BeginDocumentCreation()
        {
            Order currentOrder = SessionData.CurrentOrder;
            List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> marks = GetBookmarks();

            FillDocumentPlaceholders(marks, currentOrder);

            SaveDocument();
        }

        private void FillDocumentPlaceholders(List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> marks, Order order)
        {
            foreach (var mark in marks)
            {
                switch (mark.bookmark.name)
                {
                    case OrderID:
                        mark.paragraph.ReplaceText("____", order.ID.ToString());
                        break;

                    case OrderDate:
                        mark.paragraph.ReplaceText("_________________", order.OrderDate.ToString("dd.MM.yyyy"));
                        break;

                    case ClientInfo:
                        mark.paragraph.ReplaceText("___________________", order.User.FullName);
                        break;

                    case FurnitureID:
                        InsertInfoAboutFurnitureId(mark.paragraph, order);
                        break;

                    case FurnitureName:
                        InsertInfoAboutFurnitureName(mark.paragraph, order);
                        break;

                    case FurnitureFirmName:
                        InsertInfoAboutFurnitureFirmsName(mark.paragraph, order);
                        break;

                    case FurnitureCategoryName:
                        InsertInfoAboutFurnitureCategoriesName(mark.paragraph, order);
                        break;

                    case FurnitureCountInOrder:
                        InsertInfoAboutFurnitureCount(mark.paragraph, order);
                        break;

                    case OneFurnitureCost:
                        InsertInfoAboutOneFurnitureCost(mark.paragraph, order);
                        break;

                    case TotalOrderCost:
                        mark.paragraph.ReplaceText("____________________", $"{order.Sum:0,00} руб.");
                        break;
                }
            }
        }

        #region Функции заполнения табличных данных.

        private void InsertInfoAboutFurnitureId(XWPFParagraph paragraph, Order order)
        {
            foreach (Furniture furniture in order.Baskets.Select(b => b.Furniture))
            {
                XWPFRun run = paragraph.CreateRun();
                run.SetFontFamily("Georgia", FontCharRange.Ascii);
                run.FontSize = 16;
                run.SetText(furniture.ID.ToString());

                if (furniture.ID != order.Baskets.Select(b => b.Furniture).LastOrDefault()?.ID)
                {
                    run.AddBreak(BreakType.TEXTWRAPPING);
                }
            }
        }

        private void InsertInfoAboutFurnitureName(XWPFParagraph paragraph, Order order)
        {
            foreach (Furniture furniture in order.Baskets.Select(b => b.Furniture))
            {
                XWPFRun run = paragraph.CreateRun();
                run.SetFontFamily("Georgia", FontCharRange.HAnsi);
                run.FontSize = 16;
                run.SetText(furniture.Name);

                if (furniture.ID != order.Baskets.Select(b => b.Furniture).LastOrDefault()?.ID)
                {
                    run.AddBreak(BreakType.TEXTWRAPPING);
                }
            }
        }

        private void InsertInfoAboutFurnitureFirmsName(XWPFParagraph paragraph, Order order)
        {
            foreach (Furniture furniture in order.Baskets.Select(b => b.Furniture))
            {
                XWPFRun run = paragraph.CreateRun();
                run.SetFontFamily("Georgia", FontCharRange.HAnsi);
                run.FontSize = 16;
                run.SetText(furniture.Firm.Name);

                if (furniture.ID != order.Baskets.Select(b => b.Furniture).LastOrDefault()?.ID)
                {
                    run.AddBreak(BreakType.TEXTWRAPPING);
                }
            }
        }

        private void InsertInfoAboutFurnitureCategoriesName(XWPFParagraph paragraph, Order order)
        {
            foreach (Furniture furniture in order.Baskets.Select(b => b.Furniture))
            {
                XWPFRun run = paragraph.CreateRun();
                run.SetFontFamily("Georgia", FontCharRange.HAnsi);
                run.FontSize = 16;
                run.SetText(furniture.CategoryReference.Name);

                if (furniture.ID != order.Baskets.Select(b => b.Furniture).LastOrDefault()?.ID)
                {
                    run.AddBreak(BreakType.TEXTWRAPPING);
                }
            }
        }

        private void InsertInfoAboutFurnitureCount(XWPFParagraph paragraph, Order order)
        {
            foreach (Furniture furniture in order.Baskets.Select(b => b.Furniture))
            {
                Basket basket = order.Baskets.FirstOrDefault(b => b.IdFurniture == furniture.ID);

                XWPFRun run = paragraph.CreateRun();
                run.SetFontFamily("Georgia", FontCharRange.Ascii);
                run.FontSize = 16;
                run.SetText(basket?.Quantity.ToString() ?? "Неизвестно");
                
                if (furniture.ID != order.Baskets.Select(b => b.Furniture).LastOrDefault()?.ID)
                {
                    run.AddBreak(BreakType.TEXTWRAPPING);
                }
            }
        }

        private void InsertInfoAboutOneFurnitureCost(XWPFParagraph paragraph, Order order)
        {
            foreach (Furniture furniture in order.Baskets.Select(b => b.Furniture))
            {
                XWPFRun run = paragraph.CreateRun();
                run.SetFontFamily("Georgia", FontCharRange.Ascii);
                run.FontSize = 16;
                run.SetText(furniture.Cost.ToString("0,00"));

                if (furniture.ID != order.Baskets.Select(b => b.Furniture).LastOrDefault()?.ID)
                {
                    run.AddBreak(BreakType.TEXTWRAPPING);
                }
            }
        }
        #endregion
    }
}
