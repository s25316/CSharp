using Lesson5.Models.DTO;
using Lesson5.Servises;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lesson5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private IProductWarehouseDBService _product;


        public WarehousesController(IProductWarehouseDBService product)
        {
            _product = product;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductWarehouseDTO product)
        {
            return Ok(await _product.PostProductWarehouseAsync(product));
        }

    }
}
