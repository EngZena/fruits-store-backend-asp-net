using FruitsStoreBackendASPNET.Models;

namespace FruitsStoreBackendASPNET.Repositories
{
    public interface IFruitRepository
    {
        public bool SaveChanges();

        public bool AddEntity<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToRemove);

        public IEnumerable<Fruit> GetFruits();

        public Fruit GetSingleFruit(Guid FruitId);
    }
}
