using AutoMapper;
using fruits_store_backend_asp_net.Dtos;
using fruits_store_backend_asp_net.Enums;
using fruits_store_backend_asp_net.Models;
using fruits_store_backend_asp_net.Repositories;
using fruits_store_backend_asp_net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fruits_store_backend_asp_net.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FruitController(IFruitRepository fruitRepository, IFruitService fruitService)
        : ControllerBase
    {
        IFruitRepository _fruitRepository = fruitRepository;
        IFruitService _fruitService = fruitService;

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
        public async Task<IEnumerable<Fruit>> GetFruits()
        {
            return await _fruitService.GetFruits();
        }

        /// <summary>
        /// Retrieves fruits by Fruit Type
        /// </summary>
        /// <param name="FruitType">WINTER_FRUITS</param>
        /// <returns>The requested fruit if found.</returns>
        [HttpGet("GetFruitsByType/{FruitType}")]
        public async Task<IActionResult> GetFruitsByType(string FruitType)
        {
            if (!Enum.TryParse<FruitType>(FruitType, true, out var fruitType))
            {
                return BadRequest("Invalid fruit type provided.");
            }
            return Ok(await _fruitService.GetFruitsByType(fruitType));
        }

        /// <summary>
        /// Retrieves a specific fruit by ID
        /// </summary>
        /// <param name="FruitId">00000000-0000-0000-0000-000000000000</param>
        /// <returns>The requested fruit if found.</returns>
        [HttpGet("GetSingleFruit/{FruitId}")]
        [ProducesResponseType(typeof(Fruit), 200)] // Success response
        public async Task<Fruit> GetSingleFruit(Guid FruitId)
        {
            return await _fruitService.GetSingleFruit(FruitId);
        }

        /// <summary>
        /// Retrieves all fruits added by User Id
        /// </summary>
        /// <param name="UserId">00000000-0000-0000-0000-000000000000</param>
        /// <returns>The requested fruits added by User Id if found.</returns>
        [HttpGet("GetFruitsCreatedByUserId/{UserId}")]
        public async Task<IEnumerable<Fruit>> GetFruitsCreatedByUserId(Guid UserId)
        {
            return await _fruitService.GetFruitsCreatedByUserId(UserId);
        }

        /// <summary>
        /// Retrieves all fruits added by Current User
        /// </summary>
        [HttpGet("GetFruitsCreatedByCurrentUser")]
        public async Task<IEnumerable<Fruit>> GetFruitsCreatedByCurrentUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId != null && Guid.TryParse(userId, out Guid userGuid))
            {
                return await _fruitService.GetFruitsCreatedByUserId(userGuid);
            }
            throw new Exception("Failed to Get Fruit");
        }

        /// <summary>
        /// Creates New Fruit
        /// </summary>
        /// <param name="fruitDto">Fruit object</param>
        [HttpPost("AddFruit")]
        public async Task<IActionResult> AddFruit(FruitDto fruitDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId != null && Guid.TryParse(userId, out Guid userGuid))
                {
                    var (success, details) = await _fruitService.AddFruit(fruitDto, userGuid);
                    if (success)
                    {
                        return Ok("Fruit added successfully");
                    }
                    else
                    {
                        return BadRequest($"{details}");
                    }
                }
                return BadRequest("Unable to add fruit. User ID not found in the token.");
            }
            catch (Exception exception)
            {
                throw new Exception($"Unable to add fruit. {exception}");
            }
        }

        /// <summary>
        /// Updates a specific fruit by ID and details
        /// </summary>
        /// <param name="editFruitDto">Fruit object</param>
        [HttpPatch("EditFruit")]
        public async Task<IActionResult> EditFruit(EditFruitDto editFruitDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId != null && Guid.TryParse(userId, out Guid userGuid))
            {
                var (success, details) = await _fruitService.EditFruit(editFruitDto, userGuid);
                if (success)
                {
                    return Ok("Fruit updated successfully");
                }
                else
                {
                    return BadRequest($"{details}");
                }
            }
            else
            {
                return BadRequest("Unable to update fruit. User ID not found in the token.");
            }
            throw new Exception("Failed to update Fruit");
        }

        /// <summary>
        /// Delete a fruit added by the current user using the fruit ID
        /// </summary>
        /// <param name="FruitId">00000000-0000-0000-0000-000000000000</param>
        [HttpDelete("DeleteFruit/{FruitId}")]
        public async Task<IActionResult> DeleteFruit(Guid FruitId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId != null && Guid.TryParse(userId, out Guid userGuid))
            {
                var (success, details) = await _fruitService.DeleteFruit(FruitId, userGuid);
                if (success)
                {
                    return Ok("Fruit deleted successfully");
                }
                else
                {
                    return BadRequest($"{details}");
                }
            }
            return BadRequest("Unable to delete fruit. User ID not found in the token.");
        }
    }
}
