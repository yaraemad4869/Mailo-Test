using Mailo.Data;
using Mailo.Data.Enums;
using Mailo.IRepo;
using Mailo.Models;
using Mailo.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepo _order;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        public CartController(ICartRepo order, IUnitOfWork unitOfWork, AppDbContext db)
        {
            _order = order;
            _db = db;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            User? user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Login","Account");
            }

            Order? cart = await _order.GetOrCreateCart(user);
            if (cart == null || cart.OrderProducts == null )
            {
                TempData["ErrorMessage"] = "Cart is empty";
                return View();
            }

            return View(cart.OrderProducts.Select(op => op.product).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            User? user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            var cart = await _order.GetOrCreateCart(user);
            if (cart !=null)
            {
                cart.OrderProducts.Clear();
                _unitOfWork.orders.Delete(cart);
            }
            else
            {
                TempData["ErrorMessage"] = "Cart is already empty";
                return View();

            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            User? user = await _db.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found";
                return View();
            }

            var cart = await _order.GetOrCreateCart(user);

            if (cart == null)
            {
                cart = new Order
                {
                    UserID = user.ID,
                    OrderPrice = product.TotalPrice,
                    OrderAddress = user.Address,
                    OrderProducts = new List<OrderProduct>()
                };

                _unitOfWork.orders.Insert(cart);
                await _unitOfWork.CommitChangesAsync();
                cart.OrderProducts.Add(new OrderProduct
                {
                    ProductID = product.ID,
                    OrderID = cart.ID
                });

                _unitOfWork.orders.Update(cart); 
                await _unitOfWork.CommitChangesAsync(); 
            }
            else
            {
                OrderProduct? existingOrderProduct = cart.OrderProducts
                    .Where(op => op.ProductID == product.ID)
                    .FirstOrDefault();

                if (existingOrderProduct != null)
                {
                    TempData["ErrorMessage"] = "Product is already in cart";
                    return View();
                }
                else
                {
                    cart.OrderPrice += product.TotalPrice;

                    cart.OrderProducts.Add(new OrderProduct
                    {
                        ProductID = product.ID,
                        OrderID = cart.ID
                    });

                    _unitOfWork.orders.Update(cart);
                    await _unitOfWork.CommitChangesAsync(); 
                }
            }

            return RedirectToAction("Index_U","User");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            User? user = await _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefaultAsync();
            var cart = await _order.GetOrCreateCart(user);
            if (cart == null)
            {
                TempData["ErrorMessage"] = "Cart is empty";
                return View();
            }
            else{
                var orderProduct = cart.OrderProducts.FirstOrDefault(op => op.ProductID == productId);
                if (orderProduct != null)
                {
                    var product = await _unitOfWork.products.GetByID(productId);
                    cart.OrderPrice -= product.TotalPrice;
                    cart.OrderProducts.Remove(orderProduct);
                    if (cart.OrderProducts == null || !cart.OrderProducts.Any())
                    {
                        await ClearCart();
                    }
                    _db.SaveChanges();
                }
                else
                {
                    TempData["ErrorMessage"] = "Product not found";
                    return View();

                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewOrder()
        {
            var user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var existingOrderItem = await _order.GetOrder(user);

            if (existingOrderItem == null || (existingOrderItem.OrderStatus != OrderStatus.New))
            {
                TempData["ErrorMessage"] = "Cart is already ordered";
                return View();
            }
            var products = await _db.OrderProducts.Where(op => op.OrderID == existingOrderItem.ID)
                .Select(op => op.product)
                .ToListAsync();
            foreach (var product in products)
            {
                product.Quantity -= 1;
            }
            existingOrderItem.OrderStatus = OrderStatus.Pending;
            _unitOfWork.orders.Update(existingOrderItem);
            TempData["Success"] = "Cart Has Been Ordered Successfully";
            return RedirectToAction("Index");
        }
        
    }

}