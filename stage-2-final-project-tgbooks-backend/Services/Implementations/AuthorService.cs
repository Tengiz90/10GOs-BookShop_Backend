using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Requests.Models.Authors;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly IDatabaseManager _databaseManager;

        public AuthorService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }
        public async Task<ICollection<GetAuthor>> GetAuthorsAsync()
        {
            var authorsFromDb = await _databaseManager.GetAuthorsAsync();
            return authorsFromDb.Select(au => new GetAuthor
            {
                Id = au.Id,
                Name = au.Name,
            }).ToList();
        }
    }
}
