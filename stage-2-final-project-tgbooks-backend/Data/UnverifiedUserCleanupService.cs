using Microsoft.EntityFrameworkCore;

namespace stage_2_final_project_tgbooks_backend.Data
{

    public class UnverifiedUserCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UnverifiedUserCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var cutoff = DateTime.UtcNow.AddMinutes(-20);

                var usersToDelete = await db.Users
                    .Where(u => !u.IsVerified && u.CreatedAt < cutoff)
                    .ToListAsync(stoppingToken);

                if (usersToDelete.Count > 0)
                {
                    db.Users.RemoveRange(usersToDelete);
                    await db.SaveChangesAsync(stoppingToken);
                }

                // Run every 20 minutes
                await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
            }
        }
    }
}
