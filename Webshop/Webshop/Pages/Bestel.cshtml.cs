using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using System.Data;

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
            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "Select ID, naam, prijs from producten where ID=" + key.ToString();
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();

            var reader = dbCommand.ExecuteReader();

            if (reader.RecordsAffected == -1)
            {
                while (reader.Read())
                {
                    return reader.GetInt32(0).ToString() + "|" + reader.GetString(1) + "|" + reader.GetInt32(2).ToString();
                }
            }
            dbConnection.Close();
            return "";
        }

        public void OnPostAsync()
        {

            // Connect to the database
            string Conn = "Data Source=DESKTOP-CI1RHCI;Initial Catalog=AppleStore;Integrated Security=true;TrustServerCertificate=true; User Id=Jeffrey;Password=Jeffrey";
            IDbConnection dbConnection = new SqlConnection(Conn);

            string query = "select ID from klanten where email = '" + Email + "'";
            IDbCommand dbCommand = new SqlCommand();
            dbCommand.CommandText = query;
            dbCommand.Connection = dbConnection;
            dbConnection.Open();


            int KlantID = -1;


            using (var reader = dbCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    KlantID = reader.GetInt32(0);
                }
                reader.Close();
            }
            

            if (KlantID == -1)

                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO klanten (naam, email, postcode, plaats) VALUES (@naam, @email, @postcode, @woonplaats)";

                    SqlParameter paramA = new SqlParameter();
                    paramA.ParameterName = "@naam";
                    paramA.Value = Naam;
                    command.Parameters.Add(paramA);

                    SqlParameter paramN = new SqlParameter();
                    paramN.ParameterName = "@email";
                    paramN.Value = Email;
                    command.Parameters.Add(paramN);

                    SqlParameter paramP = new SqlParameter();
                    paramP.ParameterName = "@postcode";
                    paramP.Value = Postcode;
                    command.Parameters.Add(paramP);

                    SqlParameter paramAfb = new SqlParameter();
                    paramAfb.ParameterName = "@woonplaats";
                    paramAfb.Value = Woonplaats;
                    command.Parameters.Add(paramAfb);

                    command.ExecuteNonQuery();

                    using (var commandNewKlant = dbConnection.CreateCommand())
                    {
                        commandNewKlant.CommandText = "Select ID from klanten where email='" + Email + "'";

                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@email";
                        param.Value = Email;
                        commandNewKlant.Parameters.Add(param);


                        var reader = commandNewKlant.ExecuteReader();

                        while (reader.Read())
                        {
                            KlantID = reader.GetInt32(0);
                        }
                        reader.Close();
                    }
                }


            DateTime datum = DateTime.Now.Date;
            string betaalwijze = "PayPal";
            dbConnection.Close();
            dbConnection.Open();

            using (var commandBestellingen = dbConnection.CreateCommand())
            {
                commandBestellingen.CommandText = "INSERT INTO bestellingen (klantID, datum, betaalwijzen) VALUES (@klantID, @datum, @betaalwijze)";

                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = "@klantID";
                param1.Value = KlantID;
                commandBestellingen.Parameters.Add(param1);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@datum";
                param2.Value = datum;
                commandBestellingen.Parameters.Add(param2);

                SqlParameter param3 = new SqlParameter();
                param3.ParameterName = "@betaalwijze";
                param3.Value = betaalwijze;
                commandBestellingen.Parameters.Add(param3);

                commandBestellingen.ExecuteNonQuery();
                dbConnection.Close();
            }

            int bestelID = -1;
            dbConnection.Open();
            using (var commandLastID = dbConnection.CreateCommand())
            {
                commandLastID.CommandText = "SELECT * from bestellingen where bestelID = (SELECT MAX(bestelID) FROM bestellingen)";

                var readerLastID = commandLastID.ExecuteReader();
                while (readerLastID.Read())
                {
                    bestelID = readerLastID.GetInt32(0);
                }
               
            }
            dbConnection.Close();


            Dictionary<string, int> shoppingCart = HttpContext.Session.Get<Dictionary<string, int>>("ShoppingCart");

            foreach (var (productID, quantity) in shoppingCart)
            {
                decimal productPrice = 0;
                dbConnection.Open();
                using (var getPriceCommand = dbConnection.CreateCommand())
                {
                    getPriceCommand.CommandText = "SELECT prijs FROM producten WHERE id=@productID";

                    SqlParameter paramp = new SqlParameter();
                    paramp.ParameterName = "@productID";
                    paramp.Value = int.Parse(productID);
                    getPriceCommand.Parameters.Add(paramp);

                    var priceReader = getPriceCommand.ExecuteReader();
                    while (priceReader.Read())
                    {
                        productPrice = priceReader.GetInt32(0);
                    }
                }

                dbConnection.Close();

                decimal totalPrice = productPrice * quantity;
                dbConnection.Open();
                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO bestellingDetails (bestelID, productID, aantal, totaalPrijs) VALUES (@bestelID, @productID, @aantal, @totaalPrijs)";

                    SqlParameter paramA = new SqlParameter();
                    paramA.ParameterName = "@bestelID";
                    paramA.Value = bestelID;
                    command.Parameters.Add(paramA);

                    SqlParameter paramN = new SqlParameter();
                    paramN.ParameterName = "@productID";
                    paramN.Value = int.Parse(productID);
                    command.Parameters.Add(paramN);

                    SqlParameter paramP = new SqlParameter();
                    paramP.ParameterName = "@aantal";
                    paramP.Value = quantity;
                    command.Parameters.Add(paramP);

                    SqlParameter paramAfb = new SqlParameter();
                    paramAfb.ParameterName = "@totaalprijs";
                    paramAfb.Value = totalPrice;
                    command.Parameters.Add(paramAfb);

                    command.ExecuteNonQuery();
                    dbConnection.Close();
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
                    string productNaam = Prods[1];
                    int prijs = int.Parse(Prods[2]);
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