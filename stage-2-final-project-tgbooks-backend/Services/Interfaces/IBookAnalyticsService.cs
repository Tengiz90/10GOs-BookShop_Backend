namespace stage_2_final_project_tgbooks_backend.Services.Interfaces
{
    public interface IBookAnalyticsService
    {
        /// <summary>
        /// Retrieves and compiles book inventory, transaction, and engagement metrics 
        /// into a clean, flat binary CSV payload optimized for Python data processing.
        /// </summary>
        /// <returns>A byte array representing the generated UTF-8 CSV data.</returns>
        Task<byte[]> GetMlReadyBooksDatasetAsync();
    }
}
