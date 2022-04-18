using CartService.Repositories.Interface;
using OrderCatalog.Model;
using System.Collections.Generic;
using Moq;
using OrderCatalog.Controllers;
using Microsoft.Extensions.Logging;
using System.Linq;
using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Microsoft.AspNetCore.Mvc;

namespace XUnitTestDotNetTest
{

    [TestClass]
    public class CartTest
    {

        List<Cart>? cartValues;
        Mock<ICart>? cartRepository;
        CartController? cartController;
        Mock<ILogger<CartController>>?  _logger;
        Cart? cv;


        [TestInitialize]
        [Fact]
        public void Test1()
        {
            //Setup
            cartValues = InsertCartJson();
            cv= UpdateCartJson();
            //Arrange
            cartRepository = new Mock<ICart>();
            _logger = new Mock<ILogger<CartController>>();
                //ILogger<CartController> logger = _logger.Object;
            cartController = new CartController(cartRepository.Object, _logger.Object);



            cartRepository.Setup(m => m.GetByUser("auth0|624c0de8453275006e96e752")).Returns(GetExpectedCart);

            cartRepository.Setup(m => m.InsertCart(It.IsAny<Cart>())).Returns(
                (Cart target) =>
                {
                    cartValues.Add(target);

                    return true;
                });


            cartRepository.Setup(m => m.DeleteByUser(It.IsAny<string>())).Returns(
                (string UserID) =>
                {
                    var product = cartValues.Where(p => p.UserId == UserID).FirstOrDefault();

                    if (product == null)
                    {
                        return false;
                    }

                    cartValues.Remove(product);

                    return true;
                });


        }


        [TestMethod]
        [Fact]
        public void Get_CartByUser()
        {
            //Act
            //var actualProducts = mockProductRepository.Object.GetProducts();
            var actualProducts = cartController.GetCartDeatils("auth0|624c0de8453275006e96e752");

            //Assert
            //Xunit.Assert.Same(GetExpectedCart, actualProducts);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(cartValues, actualProducts);
        }

        [TestMethod]
        public async void Add_Cart()
        {
            //var productCount = cartRepository.Object.GetByUser(3).Count;
            //int productCount = cartController.GetCartDeatils(3).Count;

           // Assert.AreEqual(2, productCount);

            //Prepare
            Cart newProduct = GetNewCart(14,3, 2, "auth0|624c0de8453275006e96e752");
            //Act
            //mockProductRepository.Object.AddProduct(newProduct);
            var data = await cartController.AddToCart(newProduct);

            //Act  
            
            Xunit.Assert.IsType<OkObjectResult>(data);

            //productCount = cartRepository.Object.GetByUser(3).Count;
            //productCount = cartController.GetProducts().Count;
            //Assert
            //Assert.AreEqual(3, productCount);
        }
        //[Fact]
        //public async void Task_Update_ValidData_Return_OkResult()
        //{
        //    //Arrange  
        //   // var controller = new PostController(repository);
        //    var postId = 2;

        //    //Act  
        //   // var existingPost = await cartController.GetPost(postId);
        //    var existingPost = cartController.GetCartDeatils(3);
        // //   var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
        //   // var result = okResult.Value.Should().BeAssignableTo<Cart>().Subject;

        //    Cart cart= new Cart();
        //    cart.Quantity = 100;
        //    cart.CartID= result.CartID;
        //    cart.UserId = result.UserId;
        //    cart.ProductId= result.ProductId;

        //    var updatedData = await cartController.Put(cart.CartID,cart);

        //    //Assert  
        //    Xunit.Assert.IsType<OkResult>(updatedData);
        //}


        private static List<Cart> GetExpectedCart()
        {
            return new List<Cart>()
            {
                new Cart()
                {
                    CartID = 9,
                    ProductId = 3,
                    Quantity=45,
                    UserId="auth0|624c0de8453275006e96e752"
                },
                new Cart()
                {
                    CartID = 10,
                    ProductId = 0,
                    Quantity=0,
                    UserId="auth0|624c0de8453275006e96e752"
                }
            };
        }

        private static List<Cart> InsertCartJson()
        {
            return new List<Cart>()
            {
                new Cart()
                {
                    CartID = 9,
                    ProductId = 3,
                    Quantity=45,
                    UserId="auth0|624c0de8453275006e96e752"
                },
                new Cart()
                {
                    CartID = 10,
                    ProductId = 0,
                    Quantity=0,
                    UserId="auth0|624c0de8453275006e96e752"
                }
            };
        }
        private static Cart UpdateCartJson()
        {
            return new Cart()
            {
                CartID = 9,
                ProductId = 3,
                Quantity = 45,
                UserId = "auth0|624c0de8453275006e96e752"
            };
        }

        private static Cart GetNewCart(int cartID, int productId, int quantity,string userId)
        {
            return new Cart()
            {
                CartID = cartID,
                ProductId = productId,
                Quantity=quantity,
                UserId=userId
            };
        }



    }
}