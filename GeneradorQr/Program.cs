using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
namespace GeneradorQr
{
    class Program
    {
        static void Main(string[] args)
        {
            String Folio_val = args[0];
            string path = Directory.GetCurrentDirectory();
           // String Folio_val = "https://www.google.com";
            //  String Direccion = "http://rppccolimaevidencias.col.gob.mx:8182/evidencia/Certi2?Secuence=" + Folio_val;
            String Direccion = Folio_val;
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(Direccion, out qrCode);
            GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(400, QuietZoneModules.Zero), Brushes.Black, Brushes.White);
            MemoryStream ms = new MemoryStream();
            render.WriteToStream(qrCode.Matrix, ImageFormat.Jpeg, ms);
            var imagenTemporal = new Bitmap(ms);
            var imagen = new Bitmap(imagenTemporal, new Size(new Point(200, 200)));
            imagen.Save(@path+"\\codigo.jpeg", ImageFormat.Jpeg);
            using (Document doc = new Document(PageSize.LETTER,30f,20f,50f,40f))
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(@path + "\\prueba.pdf", FileMode.Create));
                writer.PageEvent = new HeaderFooter();
                doc.Open();
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@path + "\\codigo.jpeg");
                iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                img.SetAbsolutePosition(10, 500);
                img.ScaleToFit(50f, 50F);
                doc.Add(img);
                doc.Close();
                writer.Close();
            }
            Console.WriteLine(Folio_val);
        }
    }
    class HeaderFooter : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            //base.OnEndPage(writer, document);
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            iTextSharp.text .Font times = new iTextSharp.text.Font(bfTimes, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            PdfPTable tbHeader = new PdfPTable(3);
            tbHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tbHeader.DefaultCell.Border = 0;
            string path = Directory.GetCurrentDirectory();
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@path + "\\logo2.PNG");
            img.SetAbsolutePosition(0, 650);
            img.ScaleToFit(30f, 50F);                    
            tbHeader.AddCell(img);
           // tbHeader.AddCell(new Paragraph());
            PdfPCell _cell = new PdfPCell(new Paragraph("INSTITUTO PARA EL REGISTRO DEL TERRITORIO"));
            _cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            //_cell.Border = 0;
            tbHeader.AddCell(_cell);
            _cell = new PdfPCell(new Paragraph("DIRECCIÓN DEL REGISTRO PÚBLICO DE LA PROPIEDAD Y DEL COMERCIO", times));
            _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _cell.Border = 0;
            tbHeader.AddCell(_cell);
            tbHeader.AddCell(new Paragraph());
            tbHeader.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetTop(document.TopMargin)+40, writer.DirectContent);



            PdfPTable tbFooter = new PdfPTable(3);
            tbFooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tbFooter.DefaultCell.Border = 0;
            tbFooter.AddCell(new Paragraph());
             _cell = new PdfPCell(new Paragraph("Página de Validación: http://www.colima-estado.gob.mx"));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            tbFooter.AddCell(_cell);

            _cell = new PdfPCell(new Paragraph("Hoja " + writer.PageNumber));
            _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            _cell.Border = 0;
            tbFooter.AddCell(_cell);
            
            tbFooter.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetBottom(document.BottomMargin) - 5, writer.DirectContent);
        }

    }
}
