using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain;
using OrderFood.Domain.Identity;
using OrderFood.Infrastructure.Data;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        private readonly OrderFoodContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(OrderFoodContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == user.Id);
            }

            var viewModel = new CartViewModel
            {
                Id = cart.Id,
                CartItems = cart.CartItems.Select(item => new CartItemModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Title = item.Product.Title,
                    ImageUrl = item.Product.ImageUrl,
                    Price = item.Product.Price,
                    Quantity = item.Quantity
                }).ToList(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(long productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == user.Id);
            }

            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            var index = cart.CartItems.FindIndex(ci => ci.ProductId == productId);

            if (index < 0)
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                };
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cart.CartItems[index].Quantity += quantity;
            }


            _context.Carts.Update(cart);

            await _context.SaveChangesAsync();

            SetFlash(FlashMessageType.Success, $"Product \"{product.Title}\" was successfully added to your cart.");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(long productId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return NotFound();
            }

            var index = cart.CartItems.FindIndex(ci => ci.ProductId == productId);

            if (index < 0)
            {
                return NotFound();
            }

            var product = cart.CartItems[index].Product;

            cart.CartItems.RemoveAt(index);

            _context.Carts.Update(cart);

            await _context.SaveChangesAsync();

            SetFlash(FlashMessageType.Success, $"Product \"{product.Title}\" was successfully removed from your cart.");

            return RedirectToAction("Index");
        }
    }
}
