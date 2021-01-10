using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderFood.Domain;
using OrderFood.Domain.Identity;
using OrderFood.Infrastructure.Data;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{
    [Authorize]
    public class CheckoutController : BaseController
    {
        private readonly OrderFoodContext _context;
        private readonly UserManager<User> _userManager;

        public IConfiguration Configuration { get; }

        public CheckoutController(OrderFoodContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            Configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var cart = await _context.Carts
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == user.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var cartModel = new CartViewModel
            {
                Id = cart.Id,
                CartItems = cart.CartItems.Select(cartItem => new CartItemModel
                {
                    Id = cartItem.Id,
                    ImageUrl = cartItem.Product.ImageUrl,
                    Price = cartItem.Product.Price,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Title = cartItem.Product.Title
                }).ToList(),
            };

            var checkoutModel = new CheckoutModel
            {
                Cart = cartModel
            };

            return View(checkoutModel);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(CheckoutModel checkoutModel)
        {
            var user = await _userManager.GetUserAsync(User);

            var cart = await _context.Carts
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == user.Id);

            var cartModel = new CartViewModel
            {
                Id = cart.Id,
                CartItems = cart.CartItems.Select(cartItem => new CartItemModel
                {
                    Id = cartItem.Id,
                    ImageUrl = cartItem.Product.ImageUrl,
                    Price = cartItem.Product.Price,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Title = cartItem.Product.Title
                }).ToList(),
            };

            checkoutModel.Cart = cartModel;

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Address = checkoutModel.Address,
                    ForDirections = checkoutModel.ForDirections,
                    Note = checkoutModel.Note,
                    Price = checkoutModel.Cart.TotalPrice(),
                    PaymentMethod = checkoutModel.PaymentMethod,
                    DateOrdered = DateTime.Now,
                    OrderItems = new List<Domain.OrderItem>(),
                };

                foreach (var item in cart.CartItems)
                {
                    var orderItem = new Domain.OrderItem
                    {
                        ProductId = item.ProductId,
                        Title = item.Product.Title,
                        Subtitle = item.Product.Subtitle,
                        ImageUrl = item.Product.ImageUrl,
                        Price = item.Product.Price,
                        Quantity = item.Quantity
                    };

                    order.OrderItems.Add(orderItem);
                }

                if (checkoutModel.PaymentMethod == EnumPaymentMethod.Online)
                {
                    Options options = new Options();
                    options.ApiKey = Configuration["Iyzico:ApiKey"];
                    options.SecretKey = Configuration["Iyzico:SecretKey"];
                    options.BaseUrl = "https://sandbox-api.iyzipay.com";

                    CreatePaymentRequest request = new CreatePaymentRequest();
                    request.Locale = Locale.TR.ToString();
                    request.ConversationId = new Random().Next(100000000, 999999999).ToString();
                    request.Price = order.Price.ToString();
                    request.PaidPrice = order.Price.ToString();
                    request.Currency = Currency.TRY.ToString();
                    request.Installment = 1;
                    request.BasketId = Guid.NewGuid().ToString();
                    request.PaymentChannel = PaymentChannel.WEB.ToString();
                    request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

                    PaymentCard paymentCard = new PaymentCard();
                    paymentCard.CardHolderName = checkoutModel.CardName;
                    paymentCard.CardNumber = checkoutModel.CardNumber;
                    paymentCard.ExpireMonth = checkoutModel.ExpirationMonth;
                    paymentCard.ExpireYear = checkoutModel.ExpirationYear;
                    paymentCard.Cvc = checkoutModel.CVC;
                    paymentCard.RegisterCard = 0;
                    request.PaymentCard = paymentCard;

                    // paymentCard.CardNumber = "5528790000000008";
                    // paymentCard.ExpireMonth = "12";
                    // paymentCard.ExpireYear = "2030";
                    // paymentCard.Cvc = "123";

                    Buyer buyer = new Buyer();
                    buyer.Id = user.Id;
                    buyer.Name = user.FirstName;
                    buyer.Surname = user.LastName;
                    buyer.GsmNumber = "+905350000000";
                    buyer.Email = user.Email;
                    buyer.IdentityNumber = "74300864791";
                    buyer.LastLoginDate = "2015-10-05 12:43:35";
                    buyer.RegistrationDate = "2013-04-21 15:12:09";
                    buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
                    buyer.Ip = "85.34.78.112";
                    buyer.City = "Istanbul";
                    buyer.Country = "Turkey";
                    buyer.ZipCode = "34732";
                    request.Buyer = buyer;

                    Address shippingAddress = new Address();
                    shippingAddress.ContactName = $"{user.FirstName} {user.LastName}";
                    shippingAddress.City = "Istanbul";
                    shippingAddress.Country = "Turkey";
                    shippingAddress.Description = checkoutModel.Address;
                    shippingAddress.ZipCode = "34742";
                    request.ShippingAddress = shippingAddress;

                    Address billingAddress = new Address();
                    billingAddress.ContactName = $"{user.FirstName} {user.LastName}";
                    billingAddress.City = "Istanbul";
                    billingAddress.Country = "Turkey";
                    billingAddress.Description = checkoutModel.Address;
                    billingAddress.ZipCode = "34742";
                    request.BillingAddress = billingAddress;

                    List<BasketItem> basketItems = new List<BasketItem>();

                    foreach (var item in cart.CartItems)
                    {
                        BasketItem basketItem = new BasketItem();
                        basketItem.Id = item.Id.ToString();
                        basketItem.Name = item.Product.Title;
                        basketItem.Category1 = "Food";
                        basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                        basketItem.Price = item.Product.Price.ToString();
                        basketItems.Add(basketItem);
                    }

                    request.BasketItems = basketItems;

                    Payment payment = Payment.Create(request, options);

                    if (payment.Status != "success")
                    {
                        SetFlash(FlashMessageType.Danger, payment.ErrorMessage);
                        return View(checkoutModel);
                    }
                }

                _context.Orders.Add(order);

                // Clear cart
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();

                SetFlash(FlashMessageType.Success, $"Your order for {order.Price}₺ has been received.");

                return View("Success");
            }

            return View(checkoutModel);
        }
    }
}
