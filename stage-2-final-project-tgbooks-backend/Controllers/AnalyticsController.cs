using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IBookAnalyticsService _bookAnalyticsService;
        private readonly IAuthorAnalyticsService _authorAnalyticsService; // Assuming a new service

        public AnalyticsController(
            IBookAnalyticsService bookAnalyticsService,
            IAuthorAnalyticsService authorAnalyticsService)
        {
            _bookAnalyticsService = bookAnalyticsService;
            _authorAnalyticsService = authorAnalyticsService;
        }

        [HttpGet("export-ml-data")]
        public async Task<IActionResult> ExportMlData()
        {
            try
            {
                byte[] csvData = await _bookAnalyticsService.GetMlReadyBooksDatasetAsync();
                string fileName = $"tgbooks_ml_export_{DateTime.UtcNow:yyyyMMdd}.csv";

                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal pipeline disruption while building book matrix: {ex.Message}");
            }
        }

        [HttpGet("export-author-ml-data")]
        public async Task<IActionResult> ExportAuthorMlData()
        {
            try
            {
                byte[] csvData = await _authorAnalyticsService.GetMlReadyAuthorsDatasetAsync();
                string fileName = $"tgauthors_ml_export_{DateTime.UtcNow:yyyyMMdd}.csv";

                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal pipeline disruption while building author matrix: {ex.Message}");
            }
        }
    }
}