# HedgeFundYCInterview
Just Practice.

class PageRange
    {
        public int Start { get; set; }
        public int End { get; set; }

        public PageRange(int start, int end) { if (end >= start || end == 0) { Start = start; End = end; } }
    }

    class StartPDFGeneration
    {
        static void Main(string[] args)
        {
            string filepath = @"C:\ONLY FOR ME\Trials\Other Better Trials\Project Lists\GeneratePDF\My Files";
            string folderPath = @"C:\Temp\willis"; // "C:\Users\tmpskp\Downloads\GC";

            //ViaPDFSharp(filepath);
            //ViaiTextSharp(filepath);
            //ViaIceblue(filepath);
            //MergePdfViaPDFSharp(folderPath);


            //RemovePages(@"C:\Users\tmpskp\Downloads\Worksheets\Grade3_English.pdf",
            //            new PageRange[]
            //            {
            //                new PageRange(26,0)                            
            //            });

            //WriteFileNamesToLog(folderPath, false);
            
            RemovePages(@"C:\Users\tmpskp\Downloads\Satnaam International Construction Corp _Tax2015_3.pdf",
                    new PageRange[]
                    {
                        new PageRange(1,20)                        
                    });
            

            //Compress(@"C:\Users\tmpskp\Downloads\Satnaam International Construction Corp. - Tax Return - 2015.pdf");

        }

        public static void ViaPDFSharp(string path)
        {

            //Help: http://csharp.net-informations.com/file/txttopdf.htm

            var filepath = string.Format(@"{0}\pdfSharpExample1.pdf", path);

            SharpDoc pdf = new SharpDoc();
            var page = pdf.AddPage();

            XGraphics graph = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
            graph.DrawString("This is my first PDF document", font, XBrushes.Black, new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            pdf.Save(filepath);
        }

        public static void ViaiTextSharp(string path)
        {

            //Help: http://csharp.net-informations.com/file/txttopdf.htm

            var filepath = string.Format(@"{0}\iTextSharpExample1.pdf", path);

            TextDoc pdf = new TextDoc(PageSize.A4, 25, 10, 25, 10);
            //PdfWriter pdfWriter = PdfWriter.GetInstance(pdf, Response.OutputStream); //For ASP.NET
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdf, new FileStream(filepath, FileMode.CreateNew)); //For ASP.NET use Response.OutputStream
            pdf.Open();
            Paragraph Text = new Paragraph("This is test file");
            pdf.Add(Text);
            pdfWriter.CloseStream = false;           
            pdf.Close();

            //Below is for ASP.NET
            //Response.Buffer = true;
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=Example.pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Write(pdfDoc);
            //Response.End();
        }

        public static void ViaIceblue(string path)
        {
            //Help: https://www.e-iceblue.com/Tutorials/Spire.Doc/Demos/Convertors.html

            var filepath = string.Format(@"{0}\eIceblueExample1.pdf", path);

            using (SpireDoc doc = new SpireDoc(@"C:\ONLY FOR ME\Trials\Other Better Trials\Project Lists\GeneratePDF\My Files\HRTest.doc", Spire.Doc.FileFormat.Dotx2013))
            {
                doc.Sections[0].Paragraphs[0].AppendBreak(Spire.Doc.Documents.BreakType.PageBreak); //To Elimiate Evalaution note
                
                doc.SaveToFile(filepath, Spire.Doc.FileFormat.PDF);                
            }

            //using (SpirePdf pdf = new SpirePdf(filepath))
            //{
            //    pdf.Pages.RemoveAt(0);
            //    pdf.SaveToFile(filepath);
            //}                

            //using (PdfReader reader = new PdfReader(filepath))
            //{
            //    using (var fileStream = new FileStream(filepath + ".t", FileMode.Create, FileAccess.Write))
            //    {
            //        var document = new TextDoc(reader.GetPageSizeWithRotation(1));
            //        var writer = PdfWriter.GetInstance(document, fileStream);

            //        document.Open();

            //        for (var i = 2; i <= reader.NumberOfPages; i++)
            //        {
            //            var page = reader.GetPageN(i);
            //        }
       
            //        //document.
            //    }
            //}
        }

        public static void WriteFileNamesToLog(string folderPath, bool isDelete = false)
        {
            List<string> filestostay = new List<string>() { "52070_C000011000_cert_20180115130349.pdf",
                                                            "52075_C000011001_cert_20180115130349.pdf",
                                                            "52678_C000011057_cert_20180115130349.pdf",
                                                            "52733_C000011062_cert_20180115135621.pdf",
                                                            "55115_C000011302_cert_20180115143923.pdf",
                                                            "55778_C000011421_cert_20180115160313.pdf",
                                                            "56021_C000011476_cert_20180115160313.pdf",
                                                            "56037_C000011479_cert_20180115160313.pdf",
                                                            "56500_C000011590_cert_20180115163308.pdf",
                                                            "56800_C000011565_cert_20180115160313.pdf",
                                                            "58031_C000011184_cert_20180115141710.pdf" };

            StringBuilder _names = new StringBuilder();
            foreach (FileInfo file in new System.IO.DirectoryInfo(folderPath).GetFiles("*.pdf"))
            {
                if (!isDelete)
                    _names.Append(string.Format(" INSERT INTO #upload VALUES('{0}'){1}", file.Name , Environment.NewLine));
                else
                {
                    if (!filestostay.Contains(file.Name)) file.Delete();
                }
            }

            if (!_names.ToString().Empty())
            {
                using (var file = File.CreateText(string.Format(@"{0}\names.txt", folderPath)))
                {
                    file.WriteLine(_names.ToString());
                    file.Close();
                }
            }
        }

        public static void MergePdfViaPDFSharp(string folderPath)
        {
            using (SharpDoc outPdf = new SharpDoc())
            {
                foreach (FileInfo file in new System.IO.DirectoryInfo(folderPath).GetFiles("*.pdf"))
                {
                    using (SharpDoc one = SharpReader.Open(file.FullName , PdfDocumentOpenMode.Import))
                    {
                        CopyPages(one, outPdf);
                    }
                }

                outPdf.Save(string.Format("{0}\\Merged_On_{1}.pdf", folderPath, DateTime.Now.ToString("ddddMMMddyyyy_hhmmss")));
            }
            
        }

        private static void CopyPages(SharpDoc from, SharpDoc to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }


        public static void RemovePages(string filepath, params PageRange[] pages)
        {
            using (SharpDoc one = SharpReader.Open(filepath, PdfDocumentOpenMode.Modify))
            {
                List<PdfSharp.Pdf.PdfPage> pagesToRemove = new List<PdfSharp.Pdf.PdfPage>();

                foreach (var range in pages.Where(p => p.Start > 0 ))
                {
                    if (range.End > 0)
                    {
                        for (int i = range.Start; i <= range.End; i++)
                        {
                            pagesToRemove.Add(one.Pages[i - 1]);                            
                        }
                    }
                    else pagesToRemove.Add(one.Pages[range.Start - 1]);
                }

                pagesToRemove.ForEach(p => { one.Pages.Remove(p); });

                one.Save(filepath);
            }            
        }


        public static void Compress(string filepath)
        {
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 25, 25);
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(filepath);

            var _path = $@"{Path.GetDirectoryName(filepath)}/{Path.GetFileNameWithoutExtension(filepath)}_Compressed.pdf";

            PdfStamper stamper = new PdfStamper(reader, new FileStream(_path, FileMode.Create), PdfWriter.VERSION_1_5);

            stamper.FormFlattening = true;
            stamper.SetFullCompression();

            stamper.Close();
        }
    }    
    
    



