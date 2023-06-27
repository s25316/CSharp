using Lesson5.Models.DTO;
using Lesson5.Servises;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lesson5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {
        private readonly IProductWarehouseDBService _product;


        public Warehouses2Controller(IProductWarehouseDBService product)
        {
            _product = product;
        }

        [HttpPost]
        public async Task<IActionResult> PostProductByProcedureAsync(ProductWarehouseDTO product)
        {
            await _product.PostProductByProcedureAsync(product);
            return Ok();
        }
    }
}
