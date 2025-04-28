using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using WebRazor.Models;

namespace WebRazor.Pages.Admin.Customers
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext _context;

        [BindProperty(SupportsGet = true)]
        public string search { get; set; }

        public IndexModel(PRN221DBContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true, Name = "currentPage")]
        public int currentPage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int month { get; set; }

        public int totalPages { get; set; }

        public const int pageSize = 5;

        public int totalCustomer { get; set; }

        public List<Models.Customer> Customers { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return Forbid();
            }
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            totalCustomer = GetTotalCustomer();

            totalPages = (int)Math.Ceiling((double)totalCustomer / pageSize);

            Customers = GetAllCustomer();

            return Page();
        }

        private int GetTotalCustomer()
        {
            var list = _context.Customers.ToList();
            if (list.Count == 0)
            {
                return 0;
            }

            if (month < 1)
            {
                if (string.IsNullOrEmpty(search))
                {
                    return list.Count;
                }
                return list.Where(c => c.ContactName.ToLower().Contains(search.ToLower())).ToList().Count;
            }

            // Add a default return statement here
            return 0;
        }

        private List<Models.Customer> GetAllCustomer()
        {
            var list = _context.Customers.ToList();
            if (list.Count == 0)
            {
                return new List<Models.Customer>();
            }
            if (month < 1)
            {
                if (string.IsNullOrEmpty(search))
                {
                    return list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                }
                return list.Where(c => c.ContactName.ToLower().Contains(search.ToLower())).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            }

            // Add a default return statement here
            return new List<Models.Customer>();
        }
    }
}
