using PdfSharpCore.Pdf;
using System.IO;
using BookNest.Data.Entities;
using HtmlRendererCore.PdfSharp;
using PdfSharpCore;
//using HtmlRendererCore.PdfSharpCore;


public class InvoiceService
{
    // This method generates the invoice PDF from the order
    public static byte[] GenerateInvoice(Order order)
    {
        // Define the HTML content for the invoice
        string invoiceHtml = $@"
            <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                        }}
                        .invoice-header {{
                            text-align: center;
                            font-size: 24px;
                            font-weight: bold;
                            margin-bottom: 20px;
                        }}
                        .invoice-body {{
                            font-size: 14px;
                            margin-bottom: 20px;
                        }}
                        .invoice-table {{
                            width: 100%;
                            border-collapse: collapse;
                            margin-top: 20px;
                        }}
                        .invoice-table th, .invoice-table td {{
                            border: 1px solid #ddd;
                            padding: 8px;
                            text-align: left;
                        }}
                    </style>
                </head>
                <body>
                    <div class='invoice-header'>
                        <p>Invoice</p>
                    </div>
                    <div class='invoice-body'>
                        <p>Order ID: {order.Id}</p>
                        <p>Date: {order.CreatedAt:yyyy-MM-dd}</p>
                        <p>User: {order.User.UserName} ({order.User.Email})</p>
                        <p>Claim Code: {order.ClaimCode}
                    </div>
                    <table class='invoice-table'>
                        <tr>
                            <th>Book ID</th>
                            <th>Quantity</th>
                            <th>Price</th>
                        </tr>";

        foreach (var item in order.OrderItems)
        {
            invoiceHtml += $@"
                <tr>
                    <td>{item.BookId}</td>
                    <td>{item.Quantity}</td>
                    <td>{item.PriceAtPurchase:C}</td>
                </tr>";
        }

        invoiceHtml += $@"
            </table>
            <div class='invoice-body'>
                <p><strong>Total:</strong> {order.TotalAmount:C}</p>
            </div>
        </body>
    </html>";

        using (var stream = new MemoryStream())
        {
            var pdf = PdfGenerator.GeneratePdf(invoiceHtml, PageSize.A4);

            pdf.Save(stream, false);
            return stream.ToArray();
        }
    }
}
