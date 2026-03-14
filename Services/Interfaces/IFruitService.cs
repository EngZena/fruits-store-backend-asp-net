using fruits_store_backend_asp_net.Dtos;
using fruits_store_backend_asp_net.Enums;
using fruits_store_backend_asp_net.Models;

namespace fruits_store_backend_asp_net.Services
{
    public interface IFruitService
    {
        public Task<IEnumerable<Fruit>> GetFruits();
        public Task<IEnumerable<Fruit>> GetFruitsByType(FruitType fruitType);
        public Task<IEnumerable<Fruit>> GetFruitsCreatedByUserId(Guid UserId);
        public Task<IEnumerable<Fruit>> GetFruitsCreatedByCurrentUser(Guid userGuid);

        public Task<Fruit> GetSingleFruit(Guid FruitId);

        public Task<(bool success, string details)> AddFruit(FruitDto fruit, Guid userId);
        public Task<(bool success, string details)> EditFruit(
            EditFruitDto editFruitDto,
            Guid userId
        );
        public Task<(bool success, string details)> DeleteFruit(Guid FruitId, Guid userId);
    }
}
