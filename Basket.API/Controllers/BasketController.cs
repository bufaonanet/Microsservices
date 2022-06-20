using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository repository, 
                                DiscountGrpcService discountGrpcService)
        {
            _repository = repository ?? 
                throw new ArgumentNullException(nameof(repository));
            
            _discountGrpcService = discountGrpcService ?? 
                throw new ArgumentNullException(nameof(discountGrpcService));
        }

        [HttpGet("{userName}", Name ="GetBasket")]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //TODO
            foreach (var item in basket.Items)
            {
                var coupon = await  _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "GetBasket")]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await _repository.DeletBasket(userName);
            return Ok();
        }
    }
}
