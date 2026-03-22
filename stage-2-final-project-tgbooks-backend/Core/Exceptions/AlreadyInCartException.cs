namespace stage_2_final_project_tgbooks_backend.Core.Exceptions
{
    public class AlreadyInCartException : Exception
    {
        public AlreadyInCartException(string message) : base(message) { }

    }
}
