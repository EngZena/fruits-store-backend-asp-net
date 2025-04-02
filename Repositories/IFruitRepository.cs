using fruits_store_backend_asp_net.Enums;
using fruits_store_backend_asp_net.Models;

namespace fruits_store_backend_asp_net.Repositories
{
    public interface IFruitRepository
    {
        public bool SaveChanges();

        public bool AddEntity<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToRemove);

        public IEnumerable<Fruit> GetFruits();
        public IEnumerable<Fruit> GetFruitsByType(FruitType FruitTypeId);

        public IEnumerable<Fruit> GetFruitsCreatedByUserId(Guid UserId);

        public Fruit GetSingleFruit(Guid FruitId);
    }
}
