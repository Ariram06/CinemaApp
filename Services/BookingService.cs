using CinemaApp.Models;
using CinemaApp.Repository;

namespace CinemaApp.Services
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly ShowRepository _showRepository;
        private readonly AuditLogService _auditLogService;

        public BookingService(
            BookingRepository bookingRepository,
            CustomerRepository customerRepository,
            ShowRepository showRepository)
            : this(bookingRepository, customerRepository, showRepository, new AuditLogService())
        {
        }

        public BookingService(
            BookingRepository bookingRepository,
            CustomerRepository customerRepository,
            ShowRepository showRepository,
            AuditLogService auditLogService)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _showRepository = showRepository;
            _auditLogService = auditLogService;
        }

        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n========== BOOKING MANAGEMENT ==========");
                Console.WriteLine("1. Create Booking");
                Console.WriteLine("2. View All Bookings");
                Console.WriteLine("3. Search Booking By ID");
                Console.WriteLine("4. View Bookings By Customer");
                Console.WriteLine("5. View Bookings By Show");
                Console.WriteLine("6. Cancel Booking");
                Console.WriteLine("7. View Available Seats");
                Console.WriteLine("0. Back");

                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        CreateBooking();
                        break;

                    case "2":
                        ViewAllBookings();
                        break;

                    case "3":
                        SearchBooking();
                        break;

                    case "4":
                        ViewBookingsByCustomer();
                        break;

                    case "5":
                        ViewBookingsByShow();
                        break;

                    case "6":
                        CancelBooking();
                        break;

                    case "7":
                        ViewAvailableSeats();
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

        // CREATE BOOKING
        private void CreateBooking()
        {
            Console.WriteLine("\n========== CREATE BOOKING ==========");

            Console.Write("Enter Customer ID: ");

            if (!int.TryParse(Console.ReadLine(), out int customerId) ||
                customerId <= 0)
            {
                Console.WriteLine("Invalid Customer ID!");
                return;
            }

            Customer? customer =
                _customerRepository.GetCustomerById(customerId);

            if (customer == null)
            {
                Console.WriteLine("Customer not found!");
                return;
            }

            Console.Write("Enter Show ID: ");

            if (!int.TryParse(Console.ReadLine(), out int showId) ||
                showId <= 0)
            {
                Console.WriteLine("Invalid Show ID!");
                return;
            }

            Show? show = _showRepository.GetShowById(showId);

            if (show == null)
            {
                Console.WriteLine("Show not found!");
                return;
            }

            if (show.StartTime <= DateTime.Now)
            {
                Console.WriteLine("This show has already started!");
                return;
            }

            List<Seat> availableSeats =
                _bookingRepository.GetAvailableSeats(showId);

            if (availableSeats.Count == 0)
            {
                Console.WriteLine("No available seats for this show!");
                return;
            }

            Console.WriteLine("\nAvailable Seats:");

            foreach (Seat seat in availableSeats)
            {
                Console.WriteLine(
                    $"ID: {seat.SeatId} | " +
                    $"Row: {seat.RowNumber} | " +
                    $"Seat: {seat.SeatNumber} | " +
                    $"Type: {seat.SeatType}");
            }

            Console.WriteLine("\nEnter seat IDs separated by commas.");
            Console.WriteLine("Example: 1,2,3");
            Console.Write("Seat IDs: ");

            string seatInput = Console.ReadLine() ?? "";

            string[] values = seatInput.Split(
                ',',
                StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries);

            if (values.Length == 0)
            {
                Console.WriteLine("Select at least one seat!");
                return;
            }

            List<int> seatIds = new List<int>();

            foreach (string value in values)
            {
                if (!int.TryParse(value, out int seatId) || seatId <= 0)
                {
                    Console.WriteLine("Invalid seat ID entered!");
                    return;
                }

                seatIds.Add(seatId);
            }

            if (seatIds.Distinct().Count() != seatIds.Count)
            {
                Console.WriteLine("You entered a duplicate seat ID!");
                return;
            }

            decimal totalAmount = show.TicketPrice * seatIds.Count;

            Console.WriteLine("\n========== BOOKING SUMMARY ==========");
            Console.WriteLine($"Customer: {customer.CustomerName}");
            Console.WriteLine($"Movie: {show.Movie?.Title ?? "Unknown"}");
            Console.WriteLine($"Show ID: {show.ShowId}");
            Console.WriteLine($"Ticket Price: {show.TicketPrice:C}");
            Console.WriteLine($"Number of Seats: {seatIds.Count}");
            Console.WriteLine($"Total Amount: {totalAmount:C}");

            Console.Write("Confirm booking? (y/n): ");

            string confirmation = Console.ReadLine() ?? "";

            if (!confirmation.Equals("y",
                StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Booking cancelled by user.");
                return;
            }

            bool result = _bookingRepository.CreateBooking(
                customerId,
                showId,
                seatIds);

            if (result)
            {
                Booking? newBooking = _bookingRepository.GetBookingsByCustomer(customerId).FirstOrDefault();
                int newBookingId = newBooking?.BookingId ?? 0;
                _auditLogService.Log("Booking", "Created", $"Booking ID: {newBookingId} | Customer ID: {customerId}");

                Console.WriteLine("Booking created successfully!");
            }
            else
            {
                Console.WriteLine(
                    "Booking failed. One or more seats may already be booked.");
            }
        }

        // VIEW ALL BOOKINGS
        private void ViewAllBookings()
        {
            Console.WriteLine("\n========== ALL BOOKINGS ==========");

            List<Booking> bookings =
                _bookingRepository.GetAllBookings();

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found!");
                return;
            }

            foreach (Booking booking in bookings)
            {
                DisplayBooking(booking);
            }
        }

        // SEARCH BOOKING
        private void SearchBooking()
        {
            Console.Write("\nEnter Booking ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid Booking ID!");
                return;
            }

            Booking? booking = _bookingRepository.GetBookingById(id);

            if (booking == null)
            {
                Console.WriteLine("Booking not found!");
                return;
            }

            DisplayBooking(booking);
        }

        // VIEW BOOKINGS BY CUSTOMER
        private void ViewBookingsByCustomer()
        {
            Console.Write("\nEnter Customer ID: ");

            if (!int.TryParse(Console.ReadLine(), out int customerId) ||
                customerId <= 0)
            {
                Console.WriteLine("Invalid Customer ID!");
                return;
            }

            Customer? customer =
                _customerRepository.GetCustomerById(customerId);

            if (customer == null)
            {
                Console.WriteLine("Customer not found!");
                return;
            }

            List<Booking> bookings =
                _bookingRepository.GetBookingsByCustomer(customerId);

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found for this customer!");
                return;
            }

            foreach (Booking booking in bookings)
            {
                DisplayBooking(booking);
            }
        }

        // VIEW BOOKINGS BY SHOW
        private void ViewBookingsByShow()
        {
            Console.Write("\nEnter Show ID: ");

            if (!int.TryParse(Console.ReadLine(), out int showId) ||
                showId <= 0)
            {
                Console.WriteLine("Invalid Show ID!");
                return;
            }

            Show? show = _showRepository.GetShowById(showId);

            if (show == null)
            {
                Console.WriteLine("Show not found!");
                return;
            }

            List<Booking> bookings =
                _bookingRepository.GetBookingsByShow(showId);

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found for this show!");
                return;
            }

            foreach (Booking booking in bookings)
            {
                DisplayBooking(booking);
            }
        }

        // CANCEL BOOKING
        private void CancelBooking()
        {
            Console.WriteLine("\n========== CANCEL BOOKING ==========");

            Console.Write("Enter Booking ID: ");

            if (!int.TryParse(Console.ReadLine(), out int bookingId) ||
                bookingId <= 0)
            {
                Console.WriteLine("Invalid Booking ID!");
                return;
            }

            Booking? booking =
                _bookingRepository.GetBookingById(bookingId);

            if (booking == null)
            {
                Console.WriteLine("Booking not found!");
                return;
            }

            if (booking.BookingStatus == "Cancelled")
            {
                Console.WriteLine("This booking is already cancelled!");
                return;
            }

            DisplayBooking(booking);

            Console.Write("Are you sure you want to cancel? (y/n): ");

            string confirmation = Console.ReadLine() ?? "";

            if (!confirmation.Equals("y",
                StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Cancellation stopped.");
                return;
            }

            bool result = _bookingRepository.CancelBooking(bookingId);

            if (result)
            {
                _auditLogService.Log("Booking", "Cancelled", $"Booking ID: {bookingId}");

                Console.WriteLine(
                    "Booking cancelled successfully. Seats are available again!");
            }
            else
            {
                Console.WriteLine("Unable to cancel this booking!");
            }
        }

        // VIEW AVAILABLE SEATS
        private void ViewAvailableSeats()
        {
            Console.Write("\nEnter Show ID: ");

            if (!int.TryParse(Console.ReadLine(), out int showId) ||
                showId <= 0)
            {
                Console.WriteLine("Invalid Show ID!");
                return;
            }

            Show? show = _showRepository.GetShowById(showId);

            if (show == null)
            {
                Console.WriteLine("Show not found!");
                return;
            }

            List<Seat> seats =
                _bookingRepository.GetAvailableSeats(showId);

            Console.WriteLine("\n========== AVAILABLE SEATS ==========");

            if (seats.Count == 0)
            {
                Console.WriteLine("No available seats!");
                return;
            }

            foreach (Seat seat in seats)
            {
                Console.WriteLine(
                    $"Seat ID: {seat.SeatId} | " +
                    $"Row: {seat.RowNumber} | " +
                    $"Number: {seat.SeatNumber} | " +
                    $"Type: {seat.SeatType}");
            }
        }

        // COMMON BOOKING DISPLAY
        private void DisplayBooking(Booking booking)
        {
            Console.WriteLine("\n--------------------------------");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine(
                $"Customer: {booking.Customer?.CustomerName ?? "Unknown"}");
            Console.WriteLine(
                $"Movie: {booking.Show?.Movie?.Title ?? "Unknown"}");
            Console.WriteLine($"Show ID: {booking.ShowId}");
            Console.WriteLine($"Booking Date: {booking.BookingDate}");
            Console.WriteLine($"Total Amount: {booking.TotalAmount:C}");
            Console.WriteLine($"Status: {booking.BookingStatus}");

            Console.WriteLine("Seats:");

            foreach (BookingSeat bookingSeat in booking.BookingSeats)
            {
                if (bookingSeat.Seat != null)
                {
                    Console.WriteLine(
                        $"Row {bookingSeat.Seat.RowNumber}, " +
                        $"Seat {bookingSeat.Seat.SeatNumber}");
                }
            }

            Console.WriteLine("--------------------------------");
        }
    }
}