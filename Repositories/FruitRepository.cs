using FruitsStoreBackendASPNET.Data;
using FruitsStoreBackendASPNET.Models;

namespace FruitsStoreBackendASPNET.Repositories
{
    public class FruitRepository(IConfiguration config) : IFruitRepository
    {
        DataContextEF _entityFramework = new DataContextEF(config);

        public bool AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
                return _entityFramework.SaveChanges() > 0;
            }
            return false;
        }

        public IEnumerable<Fruit> GetFruits()
        {
            IEnumerable<Fruit> Fruits = _entityFramework.Fruits.ToList<Fruit>();
            return Fruits;
        }

        public IEnumerable<Fruit> GetFruitsCreatedByUserId(Guid UserId)
        {
            IEnumerable<Fruit> Fruits = _entityFramework.Fruits.Where(fruit => fruit.AddedBy == UserId).ToList<Fruit>();
            return Fruits;
        }

        public Fruit GetSingleFruit(Guid FruitId)
        {
            Fruit? Fruit = _entityFramework
                .Fruits.Where(u => u.FruitId == FruitId)
                .FirstOrDefault<Fruit>();

            if (Fruit != null)
            {
                return Fruit;
            }

            throw new Exception("Failed to Get User");
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }
    }
}
