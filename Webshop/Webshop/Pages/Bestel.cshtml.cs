using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Webshop.Pages
{
    public class BestelModel : PageModel
    {

        [BindProperty, Required]
        public string? Naam { get; set; }
        [BindProperty, Required]
        public string? Email { get; set; }
        [BindProperty, Required]
        public string? Postcode { get; set; }
        [BindProperty, Required]
        public string? Woonplaats { get; set; }




        public string GetProductNaam(int key)
        {
            using (var connection = new SqliteConnection("Data Source=producten.db"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select naam, prijs from producten where ID=" + key.ToString();

                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(0) + "|" + reader.GetString(1);
                        }
                    }
                    return "";
                }
            }
        }



        //public void SendMail(string Subject, string Body, string Email)
        //{
        //    SmtpClient mailClient = new SmtpClient();
        //    MailMessage myMail = new MailMessage("jeffrey.doornbos@navigator-eu.com", Email, Subject, Body);
        //    myMail.Priority = MailPriority.Normal;
        //    myMail.IsBodyHtml = true;
        //    myMail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

        //    // Set SMTP Server and Credentials
        //    mailClient.Host = "smtp03.hostnet.nl";
        //    mailClient.Credentials = new System.Net.NetworkCredential("jeffrey.doornbos@navigator-eu.com", "Zoetermeer1!");
        //    // set Port and SSL
        //    mailClient.Port = 587;
        //    mailClient.EnableSsl = false;

        //    try
        //    {
        //        mailClient.Send(myMail);
        //        myMail.Dispose();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}


        public void OnPostAsync()
        {

            using (var connection = new SqliteConnection("Data Source=producten.db"))
            {
                connection.Open();
                int KlantID = -1;
                using (var CheckKlant = connection.CreateCommand())
                {
                    CheckKlant.CommandText = "select ID from klanten where email = '" + Email + "'";

                    using (var reader = CheckKlant.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            KlantID = reader.GetInt32(0);
                        }
                    }
                }

                if (KlantID == -1)
                {
                    using (var command = connection.CreateCommand())
                    {


                        command.CommandText = "INSERT INTO klanten (naam, email, postcode, plaats) VALUES (@naam, @email, @postcode, @woonplaats)";
                        command.Parameters.AddWithValue("@naam", Naam);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@postcode", Postcode);
                        command.Parameters.AddWithValue("@woonplaats", Woonplaats);

                        command.ExecuteNonQuery();

                        using (var commandNewKlant = connection.CreateCommand())
                        {
                            commandNewKlant.CommandText = "Select ID from klanten where email='" + Email + "'";

                            commandNewKlant.Parameters.AddWithValue("@email", Email);

                            var reader = commandNewKlant.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    KlantID = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }

                string datum = DateTime.Now.ToShortDateString();
                string betaalwijze = "PayPal";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO bestellingen (klantID, datum, betaalwijzen) VALUES (@klantID, @datum, @betaalwijze)";
                    command.Parameters.AddWithValue("@klantID", KlantID);
                    command.Parameters.AddWithValue("@datum", datum);
                    command.Parameters.AddWithValue("@betaalwijze", betaalwijze);

                    command.ExecuteNonQuery();
                }

                int bestelID = -1;
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT last_insert_rowid()";

                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            bestelID = reader.GetInt32(0);
                        }
                    }
                }

                Dictionary<string, int> shoppingCart = HttpContext.Session.Get<Dictionary<string, int>>("ShoppingCart");

                foreach (var (productID, quantity) in shoppingCart)
                {
                    decimal productPrice = 0;
                    using (var getPriceCommand = connection.CreateCommand())
                    {
                        getPriceCommand.CommandText = "SELECT prijs FROM producten WHERE id=@productID";
                        getPriceCommand.Parameters.AddWithValue("@productID", int.Parse(productID));
                        var priceReader = getPriceCommand.ExecuteReader();
                        if (priceReader.HasRows)
                        {
                            while (priceReader.Read())
                            {
                                productPrice = priceReader.GetDecimal(0);
                            }
                        }
                    }

                    decimal totalPrice = productPrice * quantity;

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO bestellingDetails (bestelID, productID, aantal, totaalPrijs) VALUES (@bestelID, @productID, @aantal, @totaalPrijs)";
                        command.Parameters.AddWithValue("@bestelID", bestelID);
                        command.Parameters.AddWithValue("@productID", int.Parse(productID));
                        command.Parameters.AddWithValue("@aantal", quantity);
                        command.Parameters.AddWithValue("@totaalPrijs", totalPrice);

                        command.ExecuteNonQuery();
                    }
                }

                int TotaalPrijs = 0;
                string body = "Beste, " + Naam;
                body += "\r\n\r\nHartelijk dank voor uw bestelling bij ons. Wij hopen dat u tevreden bent met uw aankoop. In de bijlage vindt u de factuur die u nodig heeft voor de garantie. We raden u aan deze goed te bewaren.\r\n";
                body += "\r\n Uw bezorg adres: " + Postcode;
                body += "\r\n Uw woonplaats: " + Woonplaats;
                body += "\r\n\r\nAls u nog vragen heeft, aarzel dan niet om contact met ons op te nemen. Ons team staat voor u klaar om u te helpen.\r\n\r\n";
                body += "Met vriendelijke groet,\r\n\r\n";
                body += "Team Maccoeboeks";

                var doc = new Document();
                var pdfStream = new MemoryStream();
                var writer = PdfWriter.GetInstance(doc, pdfStream);

                doc.Open();

                // Set margins: left, right, top, bottom
                doc.SetMargins(20, 20, 120, 20);

                // Set the maximum width of the text
                float maxWidth = 400f;

                // Add the opening text using a column with a maximum width
                PdfContentByte cb = writer.DirectContent;
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.SetFontAndSize(bf, 30);

                cb.BeginText();
                cb.SetFontAndSize(bf, 18);
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Maccoeboeks.nl", doc.PageSize.Width / 2, doc.PageSize.Height - 50, 0);
                cb.EndText();
                Phrase phrase = new Phrase();
                phrase.Add(new Chunk("\n\n Factuur nummer: 2023-" + bestelID));
                phrase.Add(new Chunk("\n\n Adres: " + Postcode));
                phrase.Add(new Chunk("\n\nBeste, " + Naam));
                phrase.Add(new Chunk("\n\nBewaar dit factuur goed. Dit is uw bewijs op recht van garantie.\n\n"));


                phrase.Leading = 40f; // set the leading to 20 points
                ColumnText column = new ColumnText(cb);
                column.SetSimpleColumn(phrase, 36, doc.PageSize.Height - 100, 36 + maxWidth, doc.PageSize.Height - 150, 10, Element.ALIGN_LEFT);
                column.Go();

                // Add the image to the top right
                var imagePath = "wwwroot/maccoeboeks.png";
                var image = Image.GetInstance(imagePath);
                image.SetAbsolutePosition(doc.PageSize.Width - 150, doc.PageSize.Height - 120);
                image.ScaleAbsolute(150, 100);
                doc.Add(image);


                // Create the table
                var table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
                table.SetWidths(new float[] { 2f, 1f, 1f, 1f });

                // Add the table rows and cells
                table.AddCell("Productnaam");
                table.AddCell("Aantal");
                table.AddCell("Prijs per stuk");
                table.AddCell("Totaal prijs");

                foreach (var (key, value) in shoppingCart)
                {
                    string[] Prods = GetProductNaam(int.Parse(key)).Split('|');
                    string productNaam = Prods[0];
                    int prijs = int.Parse(Prods[1]);
                    int totaalPrijs = value * prijs;
                    table.AddCell(productNaam);
                    table.AddCell(value.ToString());
                    table.AddCell("€ " + prijs.ToString());
                    table.AddCell("€ " + totaalPrijs.ToString());

                    TotaalPrijs += totaalPrijs;
                }

                table.AddCell("");
                table.AddCell("");
                table.AddCell("Totaal prijs:");
                table.AddCell("€ " + TotaalPrijs.ToString());

                // Set the table position
                float tableYPosition = doc.PageSize.Height - 300;
                table.WriteSelectedRows(0, -1, 36, tableYPosition, cb);

                //Phrase phraseContact = new Phrase("Indien u vragen heeft over uw aankoop, kunt u altijd contact met ons opnemen via Maccoeboeks@poep.nl. Wij staan graag voor u klaar.", new iTextSharp.text.Font(bf, 10));
                //ColumnText columnContact = new ColumnText(cb);
                //columnContact.SetSimpleColumn(phraseContact, 36, 90, 36 + maxWidth, doc.PageSize.Height - 300, 10, Element.ALIGN_LEFT);
                //columnContact.Go();

                // Add the footer text
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Dit is een geautomatiseerde e-mail. Gelieve hier niet op te antwoorden. \n" + Email, doc.PageSize.Width / 2, 20, 0);
                cb.EndText();

                // Close the document and writer
                doc.Close();
                writer.Close();

                var pdfBytes = pdfStream.ToArray();

                using (var memoryStream = new MemoryStream(pdfBytes))
                {
                    var attachment = new Attachment(memoryStream, "factuur-" + Naam + ".pdf", "application/pdf");
                    var message = new MailMessage("jeffrey.doornbos@navigator-eu.com", Email, "Maccoeboeks Bestelling " + Naam, body);
                    message.Attachments.Add(attachment);

                    var mailClient = new SmtpClient("smtp03.hostnet.nl", 587);
                    mailClient.Credentials = new System.Net.NetworkCredential("jeffrey.doornbos@navigator-eu.com", "Zoetermeer1!");
                    mailClient.EnableSsl = false;

                    try
                    {
                        mailClient.Send(message);
                        attachment.Dispose();
                    }
                    catch (Exception ex)
                    {
                        // handle any errors here
                    }

                }
                HttpContext.Session.Remove("ShoppingCart");
                RedirectToPage("/Index");
            }

        }
    }
}