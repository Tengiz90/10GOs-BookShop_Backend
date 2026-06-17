using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Services.Implementations
{
    public class ClickAnalyticsService : IClickAnalyticsService
    {
        private readonly IDatabaseManager _dbManager;

        public ClickAnalyticsService(IDatabaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<byte[]> GenerateClickMlDatasetAsync()
        {
            return await _dbManager.GenerateClickMlDatasetAsync();
        }
    }
}
