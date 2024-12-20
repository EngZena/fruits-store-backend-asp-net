using AutoMapper;
using FruitsStoreBackendASPNET.Dtos;
using FruitsStoreBackendASPNET.Enums;
using FruitsStoreBackendASPNET.Models;
using FruitsStoreBackendASPNET.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FruitsStoreBackendASPNET.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FruitController(IFruitRepository fruitRepository) : ControllerBase
    {
        IFruitRepository _fruitRepository = fruitRepository;

        IMapper _mapper = new Mapper(
            new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FruitDto, Fruit>();
            })
        );

        [HttpGet("GetFruits")]
        public IEnumerable<Fruit> GetFruits()
        {
            return _fruitRepository.GetFruits();
        }

        [HttpGet("GetFruitsByType/{FruitType}")]
        public IActionResult GetFruitsByType(string FruitType)
        {
            if (!Enum.TryParse<FruitType>(FruitType, true, out var fruitType))
            {
                return BadRequest("Invalid fruit type provided.");
            }
            return Ok(_fruitRepository.GetFruitsByType(fruitType));
        }

        [HttpGet("GetSingleFruit/{FruitId}")]
        public Fruit GetSingleFruit(Guid FruitId)
        {
            return _fruitRepository.GetSingleFruit(FruitId);
        }

        [HttpGet("GetFruitsCreatedByUserId/{UserId}")]
        public IEnumerable<Fruit> GetFruitsCreatedByUserId(Guid UserId)
        {
            return _fruitRepository.GetFruitsCreatedByUserId(UserId);
        }

        [HttpPost("AddFruit")]
        public IActionResult AddFruit(FruitDto fruitDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId != null && Guid.TryParse(userId, out Guid userGuid))
            {
                fruitDto.AddedBy = userGuid;
                Fruit fruit = _mapper.Map<Fruit>(fruitDto);
                fruit.CreatedAt = DateTime.UtcNow;
                fruit.UpdatedAt = DateTime.UtcNow;
                if (!Enum.IsDefined(typeof(FruitType), fruit.FruitType))
                {
                    return BadRequest("Invalid fruit type.");
                }
                if (_fruitRepository.AddEntity(fruit))
                {
                    return Ok("Fruit added successfully!");
                }
            }

            return BadRequest("Unable to add fruit. User ID not found in the token.");
        }

        [HttpPatch("EditFruit")]
        public IActionResult EditFruit(Fruit fruit)
        {
            Fruit? fruitDb = _fruitRepository.GetSingleFruit(fruit.FruitId);
            if (fruitDb != null)
            {
                fruitDb.Name = fruit.Name;
                fruitDb.FruitType = fruit.FruitType;
                fruitDb.Price = fruit.Price;
                fruitDb.ImageBase64 = fruit.ImageBase64;
                if (_fruitRepository.SaveChanges())
                {
                    return Ok("Fruit updated successfully!");
                }
                throw new Exception("Failed to Update Fruit");
            }
            throw new Exception("Failed to Get Fruit");
        }

        [HttpDelete("DeleteFruit/{FruitId}")]
        public IActionResult DeleteFruit(Guid FruitId)
        {
            Fruit? fruitDb = _fruitRepository.GetSingleFruit(FruitId);
            if (fruitDb != null)
            {
                _fruitRepository.RemoveEntity<Fruit>(fruitDb);

                if (_fruitRepository.SaveChanges())
                {
                    return Ok("Fruit deleted successfully!");
                }
                throw new Exception("Failed to Delete Fruit");
            }
            throw new Exception("Failed to Get Fruit");
        }
    }
}
