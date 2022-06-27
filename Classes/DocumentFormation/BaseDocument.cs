using System;
using System.IO;
using System.Collections.Generic;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace AlcantaraClient.Classes.DocumentFormation
{
    /// <summary>
    /// Базовый абстрактный класс для формирования документа. <br />
    /// Использование ООП позволит уменьшить количество кода за счет наследования и динамического полиморфизма.
    /// </summary>
    public abstract class BaseDocument
    {
        public String Path { get; set; }

        public XWPFDocument Document { get; set; }

        public BaseDocument(String path, Int32 type)
        {
            Path = path;

            using (Stream stream = new FileStream(GetPath(type), FileMode.Open))
            {
                Document = new XWPFDocument(stream);
            }
        }

        private String GetPath(Int32 type)
        {
            String currentDir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            currentDir = System.IO.Path.Combine(currentDir, "Resources", type == 0 ? "!Order.docx" : "!Report.docx");

            return currentDir;
        }

        public abstract void BeginDocumentCreation();

        protected List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> GetBookmarks()
        {
            List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> marks = new List<(XWPFParagraph paragraph, CT_Bookmark bookmark)>(1);

            marks.AddRange(GetBookmarksInText());
            marks.AddRange(GetBookmarksInTables());

            return marks;
        }

        protected List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> GetBookmarksInText()
        {
            List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> marks = new List<(XWPFParagraph paragraph, CT_Bookmark bookmark)>(1);

            foreach (XWPFParagraph paragraph in Document.Paragraphs)
            {
                CT_P info = paragraph.GetCTP();

                foreach (CT_Bookmark mark in info.GetBookmarkStartList())
                {
                    marks.Add((paragraph, mark));
                }
            }

            return marks;
        }

        protected List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> GetBookmarksInTables()
        {
            List<(XWPFParagraph paragraph, CT_Bookmark bookmark)> marks = new List<(XWPFParagraph paragraph, CT_Bookmark bookmark)>(1);

            foreach (XWPFTable table in Document.Tables)
            {
                foreach (XWPFTableRow row in table.Rows)
                {
                    foreach (XWPFTableCell cell in row.GetTableCells())
                    {
                        foreach (XWPFParagraph paragraph in cell.Paragraphs)
                        {
                            CT_P info = paragraph.GetCTP();

                            foreach (CT_Bookmark bookmark in info.GetBookmarkStartList())
                            {
                                marks.Add((paragraph, bookmark));
                            }
                        }
                    }
                }
            }

            return marks;
        }

        protected void SaveDocument()
        {
            using (Stream stream = new FileStream(Path, FileMode.Create))
            {
                Document.Write(stream);
            }
        }
    }
}
