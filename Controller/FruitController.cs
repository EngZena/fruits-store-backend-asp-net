using AutoMapper;
using fruits_store_backend_asp_net.Dtos;
using fruits_store_backend_asp_net.Enums;
using fruits_store_backend_asp_net.Models;
using fruits_store_backend_asp_net.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fruits_store_backend_asp_net.Controllers
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

        /// <summary>
        /// Retrieves All Fruits
        /// </summary>
        /// <returns>A list of fruits.</returns>
        [HttpGet("GetFruits")]
        public IEnumerable<Fruit> GetFruits()
        {
            return _fruitRepository.GetFruits();
        }

        /// <summary>
        /// Retrieves a specific fruit by Fruit Type
        /// </summary>
        /// <param name="FruitType">WINTER_FRUITS</param>
        /// <returns>The requested fruit if found.</returns>
        [HttpGet("GetFruitsByType/{FruitType}")]
        public IActionResult GetFruitsByType(string FruitType)
        {
            if (!Enum.TryParse<FruitType>(FruitType, true, out var fruitType))
            {
                return BadRequest("Invalid fruit type provided.");
            }
            return Ok(_fruitRepository.GetFruitsByType(fruitType));
        }

        /// <summary>
        /// Retrieves a specific fruit by ID
        /// </summary>
        /// <param name="FruitId">00000000-0000-0000-0000-000000000000</param>
        /// <returns>The requested fruit if found.</returns>
        [HttpGet("GetSingleFruit/{FruitId}")]
        [ProducesResponseType(typeof(Fruit), 200)] // Success response
        public Fruit GetSingleFruit(Guid FruitId)
        {
            return _fruitRepository.GetSingleFruit(FruitId);
        }

        /// <summary>
        /// Retrieves all fruits added by User Id
        /// </summary>
        /// <param name="UserId">00000000-0000-0000-0000-000000000000</param>
        /// <returns>The requested fruits added by User Id if found.</returns>
        [HttpGet("GetFruitsCreatedByUserId/{UserId}")]
        public IEnumerable<Fruit> GetFruitsCreatedByUserId(Guid UserId)
        {
            return _fruitRepository.GetFruitsCreatedByUserId(UserId);
        }

        /// <summary>
        /// Creates New Fruit
        /// </summary>
        /// <param name="fruitDto">Fruit object</param>
        [HttpPost("AddFruit")]
        public IActionResult AddFruit(FruitDto fruitDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId != null && Guid.TryParse(userId, out Guid userGuid))
            {
                Fruit fruit = _mapper.Map<Fruit>(fruitDto);
                fruit.AddedBy = userGuid;
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

        /// <summary>
        /// Updates a specific fruit by ID and details
        /// </summary>
        /// <param name="fruit">Fruit object</param>
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

        /// <summary>
        /// Deletes a specific fruit by ID
        /// </summary>
        /// <param name="FruitId">00000000-0000-0000-0000-000000000000</param>
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
