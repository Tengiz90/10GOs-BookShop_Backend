using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Services.Implementations
{
    public class BookAnalyticsService : IBookAnalyticsService
    {

        private readonly IDatabaseManager _dbManager;
        private readonly ILogger<BookAnalyticsService> _logger;

        public BookAnalyticsService(IDatabaseManager dbManager, ILogger<BookAnalyticsService> logger)
        {
            _dbManager = dbManager;
            _logger = logger;
        }

        public async Task<byte[]> GetMlReadyBooksDatasetAsync()
        {
            try
            {
                _logger.LogInformation("Initiating comprehensive machine learning CSV dataset compilation pipeline at {Time}", DateTime.UtcNow);

                // Call down to the database manager repository to build out the CSV string bytes
                byte[] csvBytes = await _dbManager.GenerateBooksMlDataCsvAsync();

                _logger.LogInformation("Successfully compiled {ByteCount} bytes of structured book metrics for ML export.", csvBytes.Length);
                return csvBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An extraction anomaly occurred during the compiling of the book analytical matrix.");
                throw; // Rethrow exception up to the controller's catch blocks to generate a clean HTTP 500 error response
            }
        }

    }
}
