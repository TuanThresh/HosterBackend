namespace HosterBackend.Services;

using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class MailService : IMailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser = "lehoangtuan783@gmail.com";
    private readonly string _smtpPass = "xsbl zwal wyov swcq";

    public async Task SendEmailAsync(string toEmail, string subject, Order order)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Store", _smtpUser));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var body = BuildOrderStatusEmail(order);

        var builder = new BodyBuilder { HtmlBody = body };
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpUser, _smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private static string BuildOrderStatusEmail(Order order)
    {
        var domain = $"{order.DomainFirstPart}.{order.DomainProduct.DomainName}";
        var created = order.CreatedAt.ToString("dd/MM/yyyy HH:mm");
        var total = order.TotalPrice.ToString("N0") + "₫";

        string subject;
        string messageHeader;
        string messageBody;

        switch (order.Status)
        {
            case OrderStatusEnum.Pending:
                subject = $"[Đơn hàng #{order.Id}] Đang chờ thanh toán";
                messageHeader = "Đơn hàng đang chờ thanh toán";
                messageBody = $@"
                    <p>Xin chào <strong>{order.Customer.Name}</strong>,</p>
                    <p>Chúng tôi đã nhận được yêu cầu đặt mua tên miền <strong>{domain}</strong>.</p>
                    <p>Vui lòng thanh toán để hoàn tất đơn hàng của bạn.</p>
                    <p><strong>Số tiền cần thanh toán:</strong> {total}</p>
                ";
                break;

            case OrderStatusEnum.Paid:
                subject = $"[Đơn hàng #{order.Id}] Đã thanh toán thành công";
                messageHeader = "Thanh toán thành công";
                messageBody = $@"
                    <p>Xin chào <strong>{order.Customer.Name}</strong>,</p>
                    <p>Đơn hàng của bạn với tên miền <strong>{domain}</strong> đã được thanh toán thành công.</p>
                    <p>Chúng tôi đang xử lý để kích hoạt dịch vụ cho bạn.</p>
                    <p><strong>Ngày thanh toán:</strong> {created}</p>
                ";
                break;

            case OrderStatusEnum.Cancelled:
                subject = $"[Đơn hàng #{order.Id}] Đã bị hủy";
                messageHeader = "Đơn hàng đã bị hủy";
                messageBody = $@"
                    <p>Xin chào <strong>{order.Customer.Name}</strong>,</p>
                    <p>Đơn hàng của bạn với tên miền <strong>{domain}</strong> đã bị hủy.</p>
                    <p>Nếu đây là sự nhầm lẫn, vui lòng đặt lại đơn hàng hoặc liên hệ bộ phận hỗ trợ.</p>
                ";
                break;

            default:
                subject = $"[Đơn hàng #{order.Id}] Trạng thái không xác định";
                messageHeader = "Thông báo đơn hàng";
                messageBody = "<p>Chúng tôi không thể xác định trạng thái đơn hàng của bạn.</p>";
                break;
        }

        return $@"
        <html>
        <body>
            <h2>{messageHeader}</h2>
            {messageBody}
            <hr/>
            <p><strong>Chi tiết đơn hàng:</strong></p>
            <ul>
                <li><strong>Mã đơn hàng:</strong> #{order.Id}</li>
                <li><strong>Tên miền:</strong> {domain}</li>
                <li><strong>Thời gian đăng ký:</strong> {order.DurationByMonth} tháng</li>
                <li><strong>Phương thức thanh toán:</strong> {order.PaymentMethod.PaymentMethodName}</li>
                <li><strong>Tổng tiền:</strong> {total}</li>
                <li><strong>Ngày tạo:</strong> {created}</li>
            </ul>
            <p>Trân trọng,<br/><strong>Đội ngũ hỗ trợ</strong></p>
        </body>
        </html>";
    }
}
