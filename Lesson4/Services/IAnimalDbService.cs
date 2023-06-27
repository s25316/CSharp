using Lesson4.Models;
using Lesson4.Models.DTO;

namespace Lesson4.Services
{
    public interface IAnimalDbService
    {

        Task<IList<Animal>> GetAllAnimalsAsync(string orderBy);

        Task<bool> PostAnimal(AnimalDTO animal);

        Task<bool> PutAnimal(int idAnimal, AnimalDTO animal);

        Task<bool> DeleteAnimal(int idAnimal);
    }
}
