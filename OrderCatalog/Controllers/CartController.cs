using CartService.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderCatalog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrderCatalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICart _cart;
        public CartController(ICart cart,ILogger<CartController> logger)
        {
            _cart = cart ?? throw new ArgumentNullException(nameof(cart));
            _logger = logger;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(IEnumerable<Cart>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddToCart([FromBody] Cart cart)
        {
            try 
            { 
                string userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                if (userId != cart.UserId)
                    return Unauthorized("Access denied");
                return Ok(await _cart.InsertCart(cart));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured While add in Cart : ", ex);
                return StatusCode(500, "An error has occured.");
            }
        }

        [HttpGet]
        public IActionResult GetProductList(string sortOrder)
        {
            try
            {
                sortOrder = sortOrder?.ToLower();

                var cartList = _cart.GetAll();
                IQueryable<Cart> cART = sortOrder switch
                {
                    "desc" => cartList.OrderByDescending(p => p.CartID).AsQueryable(),
                    "asc" => cartList.OrderBy(p => p.CartID).AsQueryable(),
                    _ => cartList.AsQueryable(),
                };
                return Ok(cART);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in GetAllCart Deatils: ", ex);
                return StatusCode(500, "An error has occured.");
            }

        }



        [HttpGet("[action]")]
        public  IActionResult GetCartDeatils(string UserName)
        {
            try 
            {
                return Ok(_cart.GetByUser(UserName));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in GetCartDetails by Username : ", ex);
                return StatusCode(500, "An error has occured.");
            }
            //throw new NotImplementedException("Not implemented exception");
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> DeleteByUser(string UserName)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                if (userId != UserName)
                    return Unauthorized("Access denied");
                var value =  _cart.DeleteByUser(UserName);
                if (value)
                    return Ok();
                _logger.LogError("Cannot able to delete Cart (DeleteByUser): {UserId}", UserName);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in Delete based on  Username : ", ex);
                return StatusCode(500, "An error has occured.");
            }
        }


        [HttpDelete("[action]")]
        public async Task<ActionResult> DeleteByCartID(int CartId)
        {
            try
            {

                var value = await _cart.DeleteByID(CartId);
                if (value)
                    return Ok();
                _logger.LogError("Cannot able to delete Cart (DeleteByCartID): {UserId}", CartId);
                return BadRequest();
            }

            catch (Exception ex)
            {
                _logger.LogError("Exception occured in Delete based on  Cart : ", ex);
                return StatusCode(500, "An error has occured.");
            }
}

        [HttpPut]
        public async Task<IActionResult> Put(int CartId,[FromBody]Cart cart)
        {
            try 
            { 
                if(CartId != cart.CartID)
                    return BadRequest("CartId is not Valid");
                await _cart.UpdateCart(cart);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in Update: ", ex);
                return StatusCode(500, "An error has occured.");
    }
}
    }
}
