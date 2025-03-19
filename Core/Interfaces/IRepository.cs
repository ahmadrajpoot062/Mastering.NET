namespace Core.Interfaces
{
    public interface IRepository<TEntity>
    {
        Task Add(TEntity entity);
        Task<TEntity> GetById(int id);
        Task<List<TEntity>> GetAll();
        Task Delete(int id);
        Task Update(TEntity entity);
    }
}
