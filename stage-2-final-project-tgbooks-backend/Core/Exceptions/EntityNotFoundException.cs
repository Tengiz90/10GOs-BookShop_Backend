namespace stage_2_final_project_tgbooks_backend.Core.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, object key)
            : base($"{entityName} with identifier '{key}' was not found.")
        {
        }
    }
}