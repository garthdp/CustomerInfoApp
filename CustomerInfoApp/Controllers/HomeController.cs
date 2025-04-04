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
    // initialising dbcontext 
    private MasterContext _masterContext = new MasterContext();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // List page, takes in different variables based on what the user does
    public IActionResult Index(int page = 1, string sortDirection = "asc", string searchName = null, string searchNum = null)
    {
        IQueryable<CustomerInfo> customersQuery = _masterContext.CustomerInfos;

        // checks if search is null or not, then adds rules to the query if not null
        if (!string.IsNullOrEmpty(searchName))
        {
            customersQuery = customersQuery.Where(x => x.Name.ToLower().Contains(searchName.ToLower()));
        }

        // checks if search is null or not, then adds rules to the query if not null
        if (!string.IsNullOrEmpty(searchNum))
        {
            customersQuery = customersQuery.Where(x => x.Vatnumber.ToLower().Contains(searchNum.ToLower()));
        }

        // adds order rule to list depending on users choice, ascending is default
        customersQuery = sortDirection == "asc"
            ? customersQuery.OrderBy(c => c.Name)
            : customersQuery.OrderByDescending(c => c.Name);

        // checks number of customers
        var totalCustomers = customersQuery.Count();

        // sets list size
        int listSize = 10;

        // gets the total number of pages 
        var totalPages = (int)Math.Ceiling(totalCustomers / (double)listSize);

        // gets list of customer based on rules previously set
        // it also skips customers if already show for example if on page 2 it will skip page one customers
        // it also only takes the correct amount using listSize
        var customers = customersQuery
            .Skip((page - 1) * listSize)
            .Take(listSize)
            .ToList();

        // makes model which is passed into view
        var model = new CustomerListViewModel(customers, page, totalPages, sortDirection, searchName, searchNum);
        return View(model);
    }

    //brings up create page
    public IActionResult Create()
    {
        return View();
    }

    // creates customer
    [HttpPost]
    public IActionResult Create(CustomerInfo customer)
    {
        _masterContext.CustomerInfos.Add(customer);
        _masterContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    //brings up edit page with user information already in fields.
    public IActionResult Edit(int id)
    {
        var customer = _masterContext.CustomerInfos.Find(id);
        return View(customer);
    }

    // updates user information
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

    // brings up delete page
    public IActionResult Delete(int id) 
    {
        var customerInfo = _masterContext.CustomerInfos.Find(id);

        return View(customerInfo);
    }

    // deletes customer
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
