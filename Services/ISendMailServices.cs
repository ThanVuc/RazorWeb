namespace RazorWeb.Services
{
    public interface ISendMailServices
    {
        public Task SendMailAsync(string email, string subject, string htmlMessage);
    }
}
