using CinemaApp.Data;
using CinemaApp.Repository;
using CinemaApp.Services;

using var db = new AppDbContext();

// REPOSITORIES
MovieRepository movieRepository = new MovieRepository(db);
ShowRepository showRepository = new ShowRepository(db);
SeatRepository seatRepository = new SeatRepository(db);
CustomerRepository customerRepository = new CustomerRepository(db);
BookingRepository bookingRepository = new BookingRepository(db);

// AUDIT LOG SERVICE
AuditLogService auditLogService = new AuditLogService();

// SERVICES
MovieService movieService = new MovieService(
    movieRepository,
    auditLogService);

ShowService showService = new ShowService(
    showRepository,
    movieRepository,
    auditLogService);

SeatService seatService = new SeatService(
    seatRepository,
    showRepository,
    auditLogService);

CustomerService customerService = new CustomerService(
    customerRepository,
    auditLogService);

BookingService bookingService = new BookingService(
    bookingRepository,
    customerRepository,
    showRepository,
    auditLogService);

// MAIN MENU
bool running = true;

while (running)
{
    Console.WriteLine("\n========== CINEMA APP ==========");
    Console.WriteLine("1. Movie Management");
    Console.WriteLine("2. Show Management");
    Console.WriteLine("3. Seat Management");
    Console.WriteLine("4. Customer Management");
    Console.WriteLine("5. Booking Management");
    Console.WriteLine("0. Exit");

    Console.Write("Enter your choice: ");

    string choice = Console.ReadLine() ?? "";

    switch (choice)
    {
        case "1":
            movieService.Run();
            break;

        case "2":
            showService.Run();
            break;

        case "3":
            seatService.Run();
            break;

        case "4":
            customerService.Run();
            break;

        case "5":
            bookingService.Run();
            break;

        case "0":
            running = false;
            Console.WriteLine("Thank you for using Cinema App!");
            break;

        default:
            Console.WriteLine("Invalid choice!");
            break;
    }
}