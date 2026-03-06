using stage_2_final_project_tgbooks_backend.Data.Interfaces;
using stage_2_final_project_tgbooks_backend.Responses.Models.Categories;
using stage_2_final_project_tgbooks_backend.Services.Interfaces;

namespace stage_2_final_project_tgbooks_backend.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IDatabaseManager _databaseManager;

        public CategoryService(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public ICollection<GetCategory> GetAllCategoriesAsync()
        {
            return _databaseManager.GetCategories().Select(c =>
            new GetCategory
            {
                Id = c.Id,
                Type = c.Type,
            }).ToList();
        
        }
    }
}
