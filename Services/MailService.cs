namespace HosterBackend.Services;

using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System.Security.Cryptography;
using System.Threading.Tasks;

public class MailService : IMailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser = "lehoangtuan783@gmail.com";
    private readonly string _smtpPass = "xsbl zwal wyov swcq";

    public async Task SendEmailAsync(string toEmail, string subject, Order order, string? username, string? password)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Store", _smtpUser));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var body = BuildOrderStatusEmail(order, username, password);

        var builder = new BodyBuilder { HtmlBody = body };
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpUser, _smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendExpiredDomainEmailAsync(string toEmail, string subject, RegisteredDomain registeredDomain)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Store", _smtpUser));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var body = BuildDomainExpiryEmail(registeredDomain);

        var builder = new BodyBuilder { HtmlBody = body };
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpUser, _smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendCreatedEmployee(string subject, Employee employee, string password)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Store", _smtpUser));
        message.To.Add(MailboxAddress.Parse(employee.Email));
        message.Subject = subject;

        var body = BuildAccountCreatedEmail(employee, password);

        var builder = new BodyBuilder { HtmlBody = body };
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpUser, _smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private static string BuildOrderStatusEmail(Order order, string? username, string? password)
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
                messageBody = username != "" && password != "" ? $@"
                    <p>Xin chào <strong>{order.Customer.Name}</strong>,</p>
                    <p>Đơn hàng của bạn với tên miền <strong>{domain}</strong> đã được thanh toán thành công.</p>
                    <p>Tài khoản quản trị tên miền: <strong>{username}</strong></p>
                    <p>Tài khoản quản trị tên miền: <strong>{password}</strong></p>
                    <p><strong>Ngày thanh toán:</strong> {created}</p>
                " : $@"
                    <p>Xin chào <strong>{order.Customer.Name}</strong>,</p>
                    <p>Đơn hàng của bạn với tên miền <strong>{domain}</strong> đã được thanh toán thành công.</p>
                    <p>Tên miền của bạn đã được gia hạn thành công</p>
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
    private static string BuildDomainExpiryEmail(RegisteredDomain domain)
    {
        var domainName = domain.FullDomainName;
        var expiredAt = domain.ExpiredAt.ToString("dd/MM/yyyy");
        var customerName = domain.Order.Customer.Name;
        var daysBeforeExpire = (domain.ExpiredAt.Date - DateTime.Today).Days;

        string messageHeader;
        string messageBody;

        if (daysBeforeExpire > 0)
        {
            messageHeader = $"Tên miền sắp hết hạn trong {daysBeforeExpire} ngày";
            messageBody = $@"
            <p>Xin chào <strong>{customerName}</strong>,</p>
            <p>Tên miền <strong>{domainName}</strong> của bạn sẽ hết hạn vào ngày <strong>{expiredAt}</strong>.</p>
            <p>Vui lòng gia hạn sớm để tránh gián đoạn dịch vụ.</p>
        ";
        }
        else
        {
            messageHeader = "Tên miền của bạn đã hết hạn";
            messageBody = $@"
            <p>Xin chào <strong>{customerName}</strong>,</p>
            <p>Tên miền <strong>{domainName}</strong> của bạn đã hết hạn vào ngày <strong>{expiredAt}</strong>.</p>
            <p>Vui lòng gia hạn để khôi phục hoạt động của tên miền nếu vẫn còn nhu cầu sử dụng.</p>
        ";
        }

        return $@"
    <html>
    <body>
        <h2>{messageHeader}</h2>
        {messageBody}
        <hr/>
        <p><strong>Thông tin tên miền:</strong></p>
        <ul>
            <li><strong>Tên miền:</strong> {domainName}</li>
            <li><strong>Ngày hết hạn:</strong> {expiredAt}</li>
        </ul>
        <p>Trân trọng,<br/><strong>Đội ngũ hỗ trợ</strong></p>
    </body>
    </html>";
    }
    private static string BuildAccountCreatedEmail(Employee employee, string password)
    {

        return $@"
    <html>
    <body>
        <h2>Tài khoản nhân viên đã được tạo</h2>
        <p>Xin chào <strong>{employee.Name}</strong>,</p>
        <p>Tài khoản của bạn đã được tạo thành công trên hệ thống.</p>

        <p><strong>Thông tin tài khoản:</strong></p>
        <ul>
            <li><strong>Email:</strong> {employee.Email}</li>
            <li><strong>Mật khẩu tạm thời:</strong> {password}</li>
            <li><strong>Chức vụ:</strong> Nhân viên</li>
            <li><strong>Ngày khởi tạo:</strong> {employee.CreatedAt}</li>
        </ul>

        <p>Vui lòng đăng nhập và thay đổi mật khẩu để đảm bảo bảo mật.</p>
        <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với quản trị viên.</p>

        <hr/>
        <p>Trân trọng,<br/><strong>Đội ngũ quản trị</strong></p>
    </body>
    </html>";
    }
    public async Task SendDiscountCodeEmailAsync(string toEmail, string subject, Discount discount)
    {
        try
        {
            var message = new MimeMessage();

            // Địa chỉ người gửi
            message.From.Add(new MailboxAddress("Your Store", _smtpUser));

            // Địa chỉ người nhận (email của khách hàng)
            message.To.Add(MailboxAddress.Parse(toEmail));

            // Tiêu đề email
            message.Subject = subject;

            // Tạo nội dung email (nội dung liên quan đến mã giảm giá)
            var body = BuildDiscountEmail(discount.DiscountCode);
            var builder = new BodyBuilder { HtmlBody = body };
            message.Body = builder.ToMessageBody();

            // Kết nối và gửi email
            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            // Log hoặc xử lý lỗi nếu có
            Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
        }
    }

    private static string BuildDiscountEmail(string discountCode)
    {
        return $@"
    <html>
    <body>
        <h2>Chào Quý khách,</h2>
        <p>Chúng tôi xin thông báo rằng bạn đã nhận được mã giảm giá đặc biệt từ cửa hàng của chúng tôi.</p>
        <p><strong>Mã giảm giá:</strong> {discountCode}</p>
        <p>Hãy sử dụng mã giảm giá này để nhận ưu đãi cho lần mua hàng tiếp theo của bạn!</p>
        <p>Chúc bạn có trải nghiệm mua sắm tuyệt vời tại cửa hàng của chúng tôi!</p>
        <p>Trân trọng,<br/>Đội ngũ hỗ trợ</p>
    </body>
    </html>";
    }
    public async Task SendForgotPasswordEmaiAsync(string toEmail, string subject, string token,string type)
    {
        try
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Your Store", _smtpUser));

            message.To.Add(MailboxAddress.Parse(toEmail));

            message.Subject = subject;

            var body = BuildForgotPasswordEmail(token,toEmail,type);
            var builder = new BodyBuilder { HtmlBody = body };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
        }
    }

    private static string BuildForgotPasswordEmail(string token, string email,string type)
    {
        var encodedToken = Uri.EscapeDataString(token);

        var resetLink = $"http://localhost:5173/reset-password?email={email}&token={encodedToken}";

        if(type.Equals("Customer")) resetLink = $"http://localhost:5174/reset-password?email={email}&token={encodedToken}";

        return $@"
        <h2>Xin chào</h2>
        <p>Bạn vừa yêu cầu đặt lại mật khẩu. Vui lòng nhấn vào liên kết bên dưới để thực hiện:</p>
        <p><a href='{resetLink}' style='padding:10px 20px; background-color:#4CAF50; color:white; text-decoration:none;'>Đặt lại mật khẩu</a></p>
        <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
        <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>
        ";
    }

}
