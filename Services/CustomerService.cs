using CinemaApp.Models;
using CinemaApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _customerRepository;
        private readonly AuditLogService _auditLogService;

        public CustomerService(CustomerRepository customerRepository)
            : this(customerRepository, new AuditLogService())
        {
        }

        public CustomerService(CustomerRepository customerRepository, AuditLogService auditLogService)
        {
            _customerRepository = customerRepository;
            _auditLogService = auditLogService;
        }

        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n===== CUSTOMER MANAGEMENT =====");
                Console.WriteLine("1. Add Customer");
                Console.WriteLine("2. View All Customers");
                Console.WriteLine("3. Search Customer By ID");
                Console.WriteLine("0. Back");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        AddCustomer();
                        break;

                    case "2":
                        ViewAllCustomers();
                        break;

                    case "3":
                        SearchCustomer();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
        }

        private void AddCustomer()
        {
            Console.WriteLine("\n===== ADD CUSTOMER =====");

            Console.Write("Enter customer name: ");
            string name = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            {
                Console.WriteLine("Enter a valid customer name!");
                return;
            }

            Console.Write("Enter email: ");
            string email = (Console.ReadLine() ?? "").Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email) ||
                email.Length > 150 ||
                !email.Contains('@') ||
                !email.Contains('.'))
            {
                Console.WriteLine("Enter a valid email!");
                return;
            }

            if (_customerRepository.EmailExists(email))
            {
                Console.WriteLine("This email is already registered!");
                return;
            }

            Console.Write("Enter phone number: ");
            string phone = (Console.ReadLine() ?? "").Trim();

            if (phone.Length < 10 || phone.Length > 15 ||
                !phone.All(char.IsDigit))
            {
                Console.WriteLine("Enter a valid phone number (10-15 digits)!");
                return;
            }

            Customer customer = new Customer
            {
                CustomerName = name,
                Email = email,
                PhoneNumber = phone
            };

            try
            {
                _customerRepository.AddCustomer(customer);
                _auditLogService.Log("Customer", "Added", $"Customer ID: {customer.CustomerId} | Name: {customer.CustomerName}");
                Console.WriteLine("Customer added successfully!");
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Could not add customer. Check the email address.");
            }
        }

        private void ViewAllCustomers()
        {
            Console.WriteLine("\n===== ALL CUSTOMERS =====");

            List<Customer> customers =
                _customerRepository.GetAllCustomers();

            if (customers.Count == 0)
            {
                Console.WriteLine("No customers found!");
                return;
            }

            foreach (Customer customer in customers)
            {
                Console.WriteLine("----------------------");
                Console.WriteLine($"ID: {customer.CustomerId}");
                Console.WriteLine($"Name: {customer.CustomerName}");
                Console.WriteLine($"Email: {customer.Email}");
                Console.WriteLine($"Phone: {customer.PhoneNumber}");
            }
        }

        private void SearchCustomer()
        {
            Console.Write("\nEnter Customer ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid Customer ID!");
                return;
            }

            Customer? customer = _customerRepository.GetCustomerById(id);

            if (customer == null)
            {
                Console.WriteLine("Customer not found!");
                return;
            }

            Console.WriteLine($"ID: {customer.CustomerId}");
            Console.WriteLine($"Name: {customer.CustomerName}");
            Console.WriteLine($"Email: {customer.Email}");
            Console.WriteLine($"Phone: {customer.PhoneNumber}");
        }
    }
}