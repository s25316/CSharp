using Lesson5.Models.DTO;

namespace Lesson5.Servises
{
    public interface IProductWarehouseDBService
    {
        Task<RequestDTO> PostProductWarehouseAsync(ProductWarehouseDTO product);
        Task PostProductByProcedureAsync(ProductWarehouseDTO product);
    }
}
