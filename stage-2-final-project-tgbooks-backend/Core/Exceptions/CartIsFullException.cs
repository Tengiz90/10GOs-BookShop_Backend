namespace stage_2_final_project_tgbooks_backend.Core.Exceptions
{
    public class CartIsFullException : Exception
    {
        public CartIsFullException(string message) : base(message) { }

    }
}
