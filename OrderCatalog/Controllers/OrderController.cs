using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderCatalog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderCatalog.Controllers
{
    [ApiController]
    [Route("api")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IEnumerable<Order> OrderReview()
        {
            throw new NotImplementedException("Not implemented exception");
        }

        // add other methods and caching functionalities as required.
    }
}
