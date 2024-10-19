using AutoMapper;
using FruitsStoreBackendASPNET.Dtos;
using FruitsStoreBackendASPNET.Models;
using FruitsStoreBackendASPNET.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FruitsStoreBackendASPNET.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FruitController(IFruitRepository fruitRepository)
        : ControllerBase
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
            IEnumerable<Fruit> fruits = _fruitRepository.GetFruits();
            return fruits;
        }

        [HttpGet("GetSingleFruit/{FruitId}")]
        public Fruit GetSingleFruit(Guid FruitId)
        {
            return _fruitRepository.GetSingleFruit(FruitId);
        }

    }
}
