
using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repository
{
    public class CustomerRepository
    {
        private readonly AppDbContext _db;

        public CustomerRepository(AppDbContext db)
        {
            _db = db;
        }

        // Add a new customer
        public void AddCustomer(Customer customer)
        {
            _db.Customers.Add(customer);
            _db.SaveChanges();
        }

        // Get all customers
        public List<Customer> GetAllCustomers()
        {
            return _db.Customers
                .AsNoTracking()
                .OrderBy(c => c.CustomerName)
                .ToList();
        }

        // Get customer by ID
        public Customer? GetCustomerById(int id)
        {
            return _db.Customers
                .FirstOrDefault(c => c.CustomerId == id);
        }

        // Check whether email already exists
        public bool EmailExists(string email)
        {
            return _db.Customers
                .Any(c => c.Email == email);
        }
    }
}
