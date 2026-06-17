namespace stage_2_final_project_tgbooks_backend.Services.Interfaces
{
    public interface IClickAnalyticsService
    {
        Task<byte[]> GenerateClickMlDatasetAsync();
    }
}
