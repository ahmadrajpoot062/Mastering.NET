using Core.Interfaces;

namespace Application.Services
{
    public class GenericService<TEntity>
    {
        private readonly IRepository<TEntity> _repository;
        public GenericService(IRepository<TEntity> repository)
        {
            _repository = repository;
        }
        public async Task Add(TEntity entity)
        {
            await _repository.Add(entity);
        }
        public async Task<TEntity> GetById(int id)
        {
            return await _repository.GetById(id);
        }
        public async Task<List<TEntity>> GetAll()
        {
            return await _repository.GetAll();
        }
        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
        public async Task Update(TEntity entity)
        {
            await _repository.Update(entity);
        }
    }
}
