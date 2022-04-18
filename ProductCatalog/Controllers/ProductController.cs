using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductCatalog.Data;
using ProductCatalog;
using ProductCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ProductCatalog.Repository.IRepository;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProduct _productCatalog;
        public ProductController(ILogger<ProductController> logger, IProduct productCatalog)
        {
            _logger = logger;
            _productCatalog = productCatalog;
        }

        #region Show Product
        // api/product
        [HttpGet]
        public IActionResult GetProductList(string sortOrder)
        {
            try
            {
                sortOrder = sortOrder?.ToLower();

                var productlist = _productCatalog.GetAll();
                IQueryable<Product> products = sortOrder switch
                {
                    "desc" => productlist.OrderByDescending(p => p.Price).AsQueryable(),
                    "asc" => productlist.OrderBy(p => p.Price).AsQueryable(),
                    _ => productlist.AsQueryable(),
                };
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in GetProductList : ", ex);
                return StatusCode(500, "An error has occured.");
            }
           
        }

        // api/product/#productId
        [HttpGet("[action]")]
        public IActionResult GetProductById(int productId)
        {
            try
            {
                IEnumerable<Product> selectProducts = _productCatalog.GetAll().Where<Product>(q => q.ProductId == productId);
                if (selectProducts == null || !selectProducts.Any())
                {
                    return NotFound("No product found against this id");
                }
                else
                    return Ok(selectProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in GetProductList while getting product for product ID. " + productId +" : ", ex);
                return StatusCode(500, "An error has occured while getting product for product ID. " + productId);
            }            
        }

        [HttpGet("[action]")]
        public IActionResult MyProducts(int productId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                IEnumerable<Product> selectProducts = _productCatalog.GetAll().Where<Product>(q => q.ProductId == productId);
                if (selectProducts == null || !selectProducts.Any())
                {
                    return NotFound("No product found against this id");
                }
                else
                    return Ok(selectProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in GetProductList while getting product for product ID. " + productId + " : ", ex);
                return StatusCode(500, "An error has occured while getting product for product ID. " + productId);
            }
        }

        // api/product/GetProductCategory/#productId
        [HttpGet("[action]")]
        [Authorize]
        public IActionResult GetProductCategory(string category)
        {
            try
            {
                IEnumerable<Product> selectProducts = _productCatalog?.GetAll()?.Where<Product>(q => q.Category == category);
                if (selectProducts == null || !selectProducts.Any())
                {
                    return NotFound("No product found against this category");
                }
                else
                {
                    return Ok(selectProducts);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in GetProductListByCategory : ", ex);
                return StatusCode(500, "An error has occured.");
            }
        }

        #endregion

        // api/product/ProductPaging?pageNo=#pageNo&pageSize=#pageSize
        [HttpGet("[action]")]
        public IActionResult ProductPaging(int? pageNo, int? pageSize)
        {
            try
            {
                IEnumerable<Product> products = _productCatalog.GetAll();
                int currentPageSize = pageSize ?? 3;
                int currentPageNo = pageNo ?? 1;
                return Ok(products.Skip((currentPageNo - 1) * currentPageSize).Take(currentPageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in ProductPaging : ", ex);
                return StatusCode(500, "An error has occured.");
            }           
        }

        #region Crud Operation
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            try
            {
                //if (User.Identity.IsAuthenticated)
                //{
                //    //var d=new object();
                //    //dynamic d = null;
                //   // var d= (dynamic)null;

                //    var E = new List<SignUpRequestModel>();
                //    foreach (var claim in User.Claims)
                //    {
                //        SignUpRequestModel d=new SignUpRequestModel();
                //        d.Email=claim.Type;
                //        d.Client_id=claim.Value;
                //        d.Connection = claim.Issuer;
                //        E.Add(d);
                //    }
                //}
                //var id = this.User.FindFirstValue("sub");
                string userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                product.UserId = userId;

                _productCatalog.Add(product);
                _productCatalog.Save();
                return StatusCode(StatusCodes.Status201Created);

            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in adding product details : ", ex);
                return StatusCode(500, "An error has occured.");
            }
        }

        [HttpPut("{productId}")]
        public IActionResult Put(int productId, [FromBody] Product product)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                Product selectedProduct = _productCatalog.GetFirstorDefault(x => x.ProductId == productId);
                    //_productCatalog.GetAll().FirstOrDefault.Find(productId);
                if (selectedProduct == null)
                    return NotFound("No Records Found for this ID - " + productId);

                if (userId != selectedProduct.UserId)
                    return BadRequest("Sorry....You don't have access to Update this record");

                else
                {
                    selectedProduct.Name = product.Name;
                    selectedProduct.Description = product.Description;
                    selectedProduct.Category = product.Category;
                    selectedProduct.SubCategory = product.SubCategory;
                    selectedProduct.Price = product.Price;
                    selectedProduct.DateAdded = product.DateAdded;
                    selectedProduct.ExpiryDate = product.ExpiryDate;
                    _productCatalog.Save();
                    return Ok("Record Updated Successfully in the database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in update product details : ", ex);
                return StatusCode(500, "An error has occured.");
            }            
        }

        [HttpDelete("{productId}")]
        public IActionResult Delete(int productId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                Product selectedProduct = _productCatalog.GetFirstorDefault(i=>i.ProductId==productId);
                if (selectedProduct == null)
                    return NotFound("No Records Found for this ID - " + productId);

                if (userId != selectedProduct.UserId)
                    return BadRequest("Sorry....You don't have access to delete this record");
                else
                {
                    _productCatalog.Remove(selectedProduct);
                    _productCatalog.Save();
                    return Ok("Record Deleted successfully!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured in delete product : ", ex);
                return StatusCode(500, "An error has occured while deleting product.");
            }           
        }
        #endregion
    }
}
