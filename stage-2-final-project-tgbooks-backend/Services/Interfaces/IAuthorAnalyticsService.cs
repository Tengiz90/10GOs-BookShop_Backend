namespace stage_2_final_project_tgbooks_backend.Services.Interfaces
{
    public interface IAuthorAnalyticsService
    {
        /// <summary>
        /// Retrieves an aggregated, feature-engineered CSV byte array of author profiles optimized for ML training tasks.
        /// </summary>
        Task<byte[]> GetMlReadyAuthorsDatasetAsync();
    }
}
