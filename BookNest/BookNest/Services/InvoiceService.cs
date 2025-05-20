//using PdfSharpCore.Pdf;
//using System.IO;
//using BookNest.Data.Entities;
//using HtmlRendererCore.PdfSharp;
//using PdfSharpCore;
////using HtmlRendererCore.PdfSharpCore;


//public class InvoiceService
//{
//    // This method generates the invoice PDF from the order
//    public static byte[] GenerateInvoice(Order order)
//    {
//        // Define the HTML content for the invoice
//        string invoiceHtml = $@"
//            <html>
//                <head>
//                    <style>
//                        body {{
//                            font-family: Arial, sans-serif;
//                        }}
//                        .invoice-header {{
//                            text-align: center;
//                            font-size: 24px;
//                            font-weight: bold;
//                            margin-bottom: 20px;
//                        }}
//                        .invoice-body {{
//                            font-size: 14px;
//                            margin-bottom: 20px;
//                        }}
//                        .invoice-table {{
//                            width: 100%;
//                            border-collapse: collapse;
//                            margin-top: 20px;
//                        }}
//                        .invoice-table th, .invoice-table td {{
//                            border: 1px solid #ddd;
//                            padding: 8px;
//                            text-align: left;
//                        }}
//                    </style>
//                </head>
//                <body>
//                    <div class='invoice-header'>
//                        <p>Invoice</p>
//                    </div>
//                    <div class='invoice-body'>
//                        <p>Order ID: {order.Id}</p>
//                        <p>Date: {order.CreatedAt:yyyy-MM-dd}</p>
//                        <p>User: {order.User.UserName} ({order.User.Email})</p>
//                        <p>Claim Code: {order.ClaimCode}
//                    </div>
//                    <table class='invoice-table'>
//                        <tr>
//                            <th>Book ID</th>
//                            <th>Quantity</th>
//                            <th>Price</th>
//                        </tr>";

//        foreach (var item in order.OrderItems)
//        {
//            invoiceHtml += $@"
//                <tr>
//                    <td>{item.BookId}</td>
//                    <td>{item.Quantity}</td>
//                    <td>{item.PriceAtPurchase:C}</td>
//                </tr>";
//        }

//        invoiceHtml += $@"
//            </table>
//            <div class='invoice-body'>
//                <p><strong>Total:</strong> {order.TotalAmount:C}</p>
//            </div>
//        </body>
//    </html>";

//        using (var stream = new MemoryStream())
//        {
//            var pdf = PdfGenerator.GeneratePdf(invoiceHtml, PageSize.A4);

//            pdf.Save(stream, false);
//            return stream.ToArray();
//        }
//    }
//}



using PdfSharpCore.Pdf;
using System.IO;
using BookNest.Data.Entities;
using HtmlRendererCore.PdfSharp;
using PdfSharpCore;

public class InvoiceService
{
    // This method generates the invoice PDF from the order
    public static byte[] GenerateInvoice(Order order)
    {
        string invoiceHtml = $@"
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            font-size: 14px;
            color: #333;
            margin: 40px;
        }}
        .invoice-container {{
            max-width: 800px;
            margin: auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 8px;
        }}
        .invoice-header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .invoice-header h1 {{
            margin: 0;
            font-size: 28px;
            color: #2c3e50;
        }}
        .invoice-details {{
            margin-bottom: 20px;
        }}
        .invoice-details p {{
            margin: 4px 0;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        th, td {{
            border: 1px solid #ddd;
            padding: 10px;
        }}
        th {{
            background-color: #f5f5f5;
            font-weight: 600;
            color: #2c3e50;
        }}
        .total-section {{
            margin-top: 20px;
            text-align: right;
        }}
        .total-section p {{
            font-size: 16px;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='invoice-container'>
        <div class='invoice-header'>
            <h1>BookNest Order Invoice</h1>
        </div>
        <div class='invoice-details'>
            <p><strong>Order ID:</strong> {order.Id}</p>
            <p><strong>Date:</strong> {order.CreatedAt:yyyy-MM-dd}</p>
            <p><strong>User:</strong> {order.User.UserName} ({order.User.Email})</p>
            <p><strong>Claim Code:</strong> {order.ClaimCode}</p>
        </div>
        <table>
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
                <td>Rs. {item.PriceAtPurchase.ToString("N2")}</td>
            </tr>";
        }

        invoiceHtml += $@"
        </table>
        <div class='total-section'>
        <p>Total Amount: Rs. {order.TotalAmount}</p>
        </div>
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
