   public interface IEmailService
    {
        void SendEmail(string mailBody,
            string subject, bool isBodyHtml, bool enableSSl,
            string from = null, string to = null);
    }
//Next the from and to addresses (along with all the other email settings) come from appsettings.json.
//But they are optional parameters and overridable.
public class EmailService : IEmailService
    {
        private IConfiguration _config;
        private string _from;
        private string _to;
        private string _smtpHost;
        private string _port;
        private string _userName;
        private string _password;
        public EmailService(IConfiguration config)
        {
            _config = config;
            _from = config["EmailSettings:From"];
            _to = config["EmailSettings:To"];
            _smtpHost = config["EmailSettings:SmtpHost"];
            _port = config["EmailSettings:Port"];
            _userName = config["EmailSettings:UserName"];
            _password = config["EmailSettings:Password"];
        }
        public void SendEmail(string mailBody,
            string subject, bool isBodyHtml, bool enableSSl,
            string fromAdd = null, string toAdd = null)
        {
            string fromAddress = (fromAdd == null) ? _from : fromAdd;
            MailAddress from = new MailAddress(fromAddress);
            string toAddress = (toAdd == null) ? _to : toAdd;
            MailAddress to = new MailAddress(toAddress);
            MailMessage mailMessage = new MailMessage(from, to);
            mailMessage.Body = mailBody;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = isBodyHtml;
            SmtpClient client = new SmtpClient(_smtpHost)
            {
                Port = int.Parse(_port),
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = enableSSl
            };
            client.Send(mailMessage);
        }
    }
//Now register in Startup.ConfigureServices()
//services.AddScoped<IEmailService, EmailService>();