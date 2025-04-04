using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CustomerInfoApp.Models;
using CustomerInfoApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;
using System.Drawing.Printing;

namespace CustomerInfoApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MasterContext _masterContext = new MasterContext();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(int page = 1, string sortDirection = "asc", string searchName = null, string searchNum = null)
    {
        int listSize = 10;

        IQueryable<CustomerInfo> customersQuery = _masterContext.CustomerInfos;

        if (!string.IsNullOrEmpty(searchName))
        {
            customersQuery = customersQuery.Where(x => x.Name.ToLower().Contains(searchName.ToLower()));
        }

        if (!string.IsNullOrEmpty(searchNum))
        {
            customersQuery = customersQuery.Where(x => x.Vatnumber.ToLower().Contains(searchNum.ToLower()));
        }

        customersQuery = sortDirection == "asc"
            ? customersQuery.OrderBy(c => c.Name)
            : customersQuery.OrderByDescending(c => c.Name);

        var totalCustomers = customersQuery.Count();
        var totalPages = (int)Math.Ceiling(totalCustomers / (double)listSize);

        var customers = customersQuery
            .Skip((page - 1) * listSize)
            .Take(listSize)
            .ToList();

        var model = new CustomerListViewModel(customers, page, totalPages, sortDirection, searchName, searchNum);
        return View(model);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CustomerInfo customer)
    {
        if (customer.Name == null || customer.Address == null) 
        {
            return View();
        }

        _masterContext.CustomerInfos.Add(customer);
        _masterContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var customer = _masterContext.CustomerInfos.Find(id);
        return View(customer);
    }

    [HttpPost]
    public IActionResult Edit(CustomerInfo customer)
    {
        var updateInfo = _masterContext.CustomerInfos.Find(customer.CustomerId);
        updateInfo.Name = customer.Name;
        updateInfo.Address = customer.Address;
        updateInfo.TelephoneNumber = customer.TelephoneNumber;
        updateInfo.Vatnumber = customer.Vatnumber;
        updateInfo.ContactPersonEmail = customer.ContactPersonEmail;
        updateInfo.ContactPersonName = customer.ContactPersonName;
        _masterContext.Entry(updateInfo).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _masterContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id) 
    {
        var customerInfo = _masterContext.CustomerInfos.Find(id);

        return View(customerInfo);
    }

    [HttpPost]
    public IActionResult Delete(CustomerInfo customer)
    {

        _masterContext.Remove(customer);
        _masterContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
