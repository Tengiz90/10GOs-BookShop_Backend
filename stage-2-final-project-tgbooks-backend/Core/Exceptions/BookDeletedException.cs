namespace stage_2_final_project_tgbooks_backend.Core.Exceptions
{
    public class BookDeletedException : Exception
    {
        public BookDeletedException(string bookTitle, string bookLanguage)
            : base($"Book '{bookTitle}' with {bookLanguage} language is no longer for sale.")
        {
        }
    }
}