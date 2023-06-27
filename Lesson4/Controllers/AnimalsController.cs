using Lesson4.Models;
using Lesson4.Models.DTO;
using Lesson4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lesson4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalDbService _dbService;
        public AnimalsController (IAnimalDbService dbService) 
        {
            _dbService = dbService;
        }

        [HttpGet]
        public async Task <IActionResult> GetAllAnimalsAsync([FromQuery] string orderBy) 
        {
            IList<Animal> animals = await _dbService.GetAllAnimalsAsync(orderBy);
            return Ok(animals);
        }

        [HttpPost]
        public async Task<IActionResult> PostAnimalAsync(AnimalDTO animal)
        {
            return await _dbService.PostAnimal(animal) ? Ok() : BadRequest();
        }

        [HttpPut("{idAnimal}")]
        public async Task<IActionResult> PutAnimalAsync(int idAnimal, AnimalDTO animal)
        {
            return await _dbService.PutAnimal(idAnimal, animal) ? Ok() : BadRequest();
        }

        [HttpDelete("{idAnimal}")]
        public async Task<IActionResult> DeleteAnimalAsync(int idAnimal)
        {
            return await _dbService.DeleteAnimal(idAnimal) ? Ok() : BadRequest();
        }
    }
}
