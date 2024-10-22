﻿using Mailo.Data;
using Mailo.Data.Enums;
using Mailo.IRepo;
using Mailo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        public EmployeeController(IUnitOfWork unitOfWork, AppDbContext db)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        { 
            return View(await _unitOfWork.employees.GetAll());
        }
        public async Task<IActionResult> New()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.employees.Insert(employee);
                TempData["Success"] = "Employee Has Been Added Successfully";
                return RedirectToAction("Index");
            }
            return View(employee);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id != 0)
            {
                return View(await _unitOfWork.employees.GetByID(id));
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.employees.Update(employee);
                TempData["Success"] = "Employee Has Been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(employee);
        }
        public async Task<IActionResult> Delete(int id = 0)
        {
            if (id != 0)
            {
                return View(await _unitOfWork.employees.GetByID(id));
            }
            return NotFound();
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(Employee employee)
        {
            
            if (employee != null)
            {
                _unitOfWork.employees.Delete(employee);
                TempData["Success"] = "Employee Has Been Deleted Successfully";
                return RedirectToAction("Index");
            }
            return NotFound();

        }
        
        public async Task<IActionResult> ViewOrders()
        {
            try
            {
                var orders = await _unitOfWork.orders.GetAll();
                return View(orders.Where(o => (o.OrderStatus == OrderStatus.Pending && o.EmpID == null)).ToList());

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "No Available Orders";
                return View(ex);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOrder(Order order)
        {
            //Order order = await _unitOfWork.orders.GetByID(order);
            Employee employee = await _db.Employees.Where(x => x.Email == User.Identity.Name).FirstOrDefaultAsync();
            order.EmpID = employee.ID;
            _unitOfWork.orders.Update(order);

            TempData["Success"] = "Order Has Been Accepted Successfully";
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> ViewRequiredOrders()
        {
            Employee employee = await _db.Employees.FirstOrDefaultAsync(x => x.Email == User.Identity.Name); 
            try
            {
                var orders = await _unitOfWork.orders.GetAll();
                return View(orders.Where(o => o.EmpID == employee.ID).ToList());

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There is no orders";
                return View(ex);
            }
        }
        public async Task<IActionResult> EditOrder(int OrderId)
        {
            return View(await _unitOfWork.orders.GetByID(OrderId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.orders.Update(order);
                TempData["Success"] = "Order Has Been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(order);
            
        }
    }
}