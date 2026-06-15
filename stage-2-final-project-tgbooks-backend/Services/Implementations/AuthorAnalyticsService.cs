using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Services.Implementations
{
    public class AuthorAnalyticsService : IAuthorAnalyticsService
    {
        private readonly IDatabaseManager _databaseManager;

        public AuthorAnalyticsService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<byte[]> GetMlReadyAuthorsDatasetAsync()
        {
            // Direct delegation to the database manager to stream the built file bytes
            return await _databaseManager.GenerateAuthorMlDatasetAsync();
        }
    }
}
