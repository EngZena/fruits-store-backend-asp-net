using AutoMapper;
using fruits_store_backend_asp_net.Dtos;
using fruits_store_backend_asp_net.Enums;
using fruits_store_backend_asp_net.Models;
using fruits_store_backend_asp_net.Repositories;

namespace fruits_store_backend_asp_net.Services
{
    public class FruitService(IFruitRepository fruitRepository) : IFruitService
    {
        IFruitRepository _fruitRepository = fruitRepository;

        IMapper _mapper = new Mapper(
            new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FruitDto, Fruit>();
            })
        );

        public async Task<IEnumerable<Fruit>> GetFruits()
        {
            return await _fruitRepository.GetFruits();
        }

        public async Task<IEnumerable<Fruit>> GetFruitsByType(FruitType fruitType)
        {
            return await _fruitRepository.GetFruitsByType(fruitType);
        }

        public async Task<Fruit> GetSingleFruit(Guid FruitId)
        {
            return await _fruitRepository.GetSingleFruit(FruitId);
        }

        public async Task<IEnumerable<Fruit>> GetFruitsCreatedByUserId(Guid UserId)
        {
            return await _fruitRepository.GetFruitsCreatedByUserId(UserId);
        }

        public async Task<IEnumerable<Fruit>> GetFruitsCreatedByCurrentUser(Guid userGuid)
        {
            return await _fruitRepository.GetFruitsCreatedByUserId(userGuid);
        }

        public async Task<(bool success, string details)> AddFruit(FruitDto fruitDto, Guid userId)
        {
            try
            {
                if (!Enum.IsDefined(typeof(FruitType), fruitDto.FruitType))
                {
                    return (false, "Invalid fruit type.");
                }
                Fruit newFruit = _mapper.Map<Fruit>(fruitDto);
                newFruit.AddedBy = userId;
                newFruit.CreatedAt = DateTime.UtcNow;
                await _fruitRepository.AddFruit(newFruit);
                return (true, "Fruit added Successfully");
            }
            catch (Exception exception)
            {
                throw new Exception($"Unable to add fruit. {exception}");
            }
        }

        public async Task<(bool success, string details)> EditFruit(
            EditFruitDto editFruitDto,
            Guid userId
        )
        {
            try
            {
                Fruit? fruitDb = await _fruitRepository.GetSingleFruit(editFruitDto.FruitId);
                if (fruitDb != null)
                {
                    fruitDb.Name = editFruitDto.Name ?? fruitDb.Name;
                    fruitDb.FruitType = editFruitDto.FruitType ?? fruitDb.FruitType;
                    fruitDb.Price = editFruitDto.Price ?? fruitDb.Price;
                    fruitDb.ImageBase64 = editFruitDto.ImageBase64 ?? fruitDb.ImageBase64;
                    fruitDb.UpdatedAt = DateTime.UtcNow;
                    fruitDb.UpdatedBy = userId;
                    if (await _fruitRepository.SaveChanges())
                    {
                        return (true, "Fruit updated successfully!");
                    }
                    return (false, "Failed to Update Fruit");
                }
                else
                {
                    return (
                        false,
                        "Oops! We couldn't find the fruit in our database. Please double-check the ID or create a new entry if needed."
                    );
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to Update Fruit. exception {exception}");
            }
        }

        public async Task<(bool success, string details)> DeleteFruit(Guid FruitId, Guid userId)
        {
            Fruit? fruitDb = await _fruitRepository.GetSingleFruit(FruitId);
            if (fruitDb != null)
            {
                if (userId != fruitDb.AddedBy)
                {
                    return (
                        false,
                        "Unable to delete fruit. You can only remove fruits that you have added yourself."
                    );
                }
                _fruitRepository.RemoveEntity<Fruit>(fruitDb);

                if (await _fruitRepository.SaveChanges())
                {
                    return (true, "Fruit deleted successfully!");
                }
                return (false, "Failed to Delete Fruit");
            }
            return (false, "Failed to Get Fruit");
        }
    }
}
