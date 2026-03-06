namespace stage_2_final_project_tgbooks_backend.Data.Interfaces
{
    public interface IEmailSender
    {
        void Send(string to, string subject, string body);
    }
}
