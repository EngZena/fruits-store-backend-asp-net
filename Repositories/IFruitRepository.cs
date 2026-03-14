using fruits_store_backend_asp_net.Enums;
using fruits_store_backend_asp_net.Models;

namespace fruits_store_backend_asp_net.Repositories
{
    public interface IFruitRepository
    {
        public Task<bool> SaveChanges();

        public Task<bool> AddFruit<T>(T entityToAdd);
        public bool EditFruit<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToRemove);

        public Task<IEnumerable<Fruit>> GetFruits();
        public Task<IEnumerable<Fruit>> GetFruitsByType(FruitType FruitTypeId);

        public Task<IEnumerable<Fruit>> GetFruitsCreatedByUserId(Guid UserId);

        public Task<Fruit> GetSingleFruit(Guid FruitId);
    }
}
