using fruits_store_backend_asp_net.Data;
using fruits_store_backend_asp_net.Enums;
using fruits_store_backend_asp_net.Models;
using Microsoft.EntityFrameworkCore;

namespace fruits_store_backend_asp_net.Repositories
{
    public class FruitRepository(IConfiguration config) : IFruitRepository
    {
        DataContextEF _entityFramework = new DataContextEF(config);

        public async Task<bool> AddFruit<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                await _entityFramework.AddAsync(entityToAdd);
                return _entityFramework.SaveChanges() > 0;
            }
            return false;
        }

        public bool EditFruit<T>(T entityToUpdate)
        {
            if (entityToUpdate != null)
            {
                _entityFramework.Update(entityToUpdate);
                return _entityFramework.SaveChanges() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<Fruit>> GetFruits()
        {
            IEnumerable<Fruit> Fruits = await _entityFramework.Fruits.ToListAsync<Fruit>();
            return Fruits;
        }

        public async Task<IEnumerable<Fruit>> GetFruitsByType(FruitType FruitType)
        {
            IEnumerable<Fruit> Fruits = await _entityFramework
                .Fruits.Where(f => f.FruitType == FruitType)
                .ToListAsync<Fruit>();
            return Fruits;
        }

        public async Task<IEnumerable<Fruit>> GetFruitsCreatedByUserId(Guid UserId)
        {
            IEnumerable<Fruit> Fruits = await _entityFramework
                .Fruits.Where(fruit => fruit.AddedBy == UserId)
                .ToListAsync<Fruit>();
            return Fruits;
        }

        public async Task<Fruit> GetSingleFruit(Guid FruitId)
        {
            Fruit? Fruit = await _entityFramework
                .Fruits.Where(u => u.FruitId == FruitId)
                .FirstOrDefaultAsync<Fruit>();

            if (Fruit != null)
            {
                return Fruit;
            }

            throw new Exception("Failed to Get Fruit");
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }

        public async Task<bool> SaveChanges()
        {
            return await _entityFramework.SaveChangesAsync() > 0;
        }
    }
}
