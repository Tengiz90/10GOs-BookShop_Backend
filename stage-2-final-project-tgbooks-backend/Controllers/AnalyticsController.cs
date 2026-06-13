using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IBookAnalyticsService _analyticsService;

        public AnalyticsController(IBookAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("export-ml-data")]
        public async Task<IActionResult> ExportMlData()
        {
            try
            {
                byte[] csvData = await _analyticsService.GetMlReadyBooksDatasetAsync();
                string fileName = $"tgbooks_ml_export_{DateTime.UtcNow:yyyyMMdd}.csv";

                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal pipeline disruption while building matrix: {ex.Message}");
            }
        }
    }
}
