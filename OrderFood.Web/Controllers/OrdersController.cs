using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain.Identity;
using OrderFood.Infrastructure.Data;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly OrderFoodContext _context;
        private readonly UserManager<User> _userManager;

        public OrdersController(OrderFoodContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders
                .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
                .Where(order => order.UserId == user.Id)
                .OrderByDescending(order => order.DateOrdered)
                .ToListAsync();

            var orderListViewModel = new OrderListViewModel
            {
                Orders = orders,
            };

            return View(orderListViewModel);
        }

        public async Task<IActionResult> Show(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.UserId != user.Id)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                if (!isAdmin)
                {
                    return NotFound();
                }
            }

            var orderModel = new OrderModel
            {
                Id = order.Id,
                FirstName = order.FirstName,
                LastName = order.LastName,
                UserName = order.UserName,
                Email = order.Email,
                Address = order.Address,
                ForDirections = order.ForDirections,
                Note = order.Note,
                Price = order.Price,
                DateOrdered = order.DateOrdered,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                OrderItems = order.OrderItems.Select(orderItem => new OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    Title = orderItem.Title,
                    Subtitle = orderItem.Title,
                    ImageUrl = orderItem.ImageUrl,
                    Price = orderItem.Price,
                    Quantity = orderItem.Quantity
                }).ToList(),
            };

            return View(orderModel);
        }
    }
}
