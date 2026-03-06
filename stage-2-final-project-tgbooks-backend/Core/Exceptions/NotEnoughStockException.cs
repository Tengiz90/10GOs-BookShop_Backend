namespace stage_2_final_project_tgbooks_backend.Core.Exceptions
{
    public class NotEnoughStockException : Exception
    {
        public NotEnoughStockException(string bookTitle, string bookLanguage, int availableQuantity, bool isOutOfStock = false)
            : base(isOutOfStock
                  ? $"Book '{bookTitle}' with {bookLanguage} language is out of stock."
                  : $"Not enough stock for book '{bookTitle}' with {bookLanguage} language. Only {availableQuantity} left.")
        {
        }
    }
}
