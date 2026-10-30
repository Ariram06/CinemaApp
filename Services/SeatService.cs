using CinemaApp.Models;
using CinemaApp.Repository;

namespace CinemaApp.Services
{
    public class SeatService
    {
        private readonly SeatRepository _seatRepository;
        private readonly ShowRepository _showRepository;
        private readonly AuditLogService _auditLogService;

        public SeatService(
            SeatRepository seatRepository,
            ShowRepository showRepository)
            : this(seatRepository, showRepository, new AuditLogService())
        {
        }

        public SeatService(
            SeatRepository seatRepository,
            ShowRepository showRepository,
            AuditLogService auditLogService)
        {
            _seatRepository = seatRepository;
            _showRepository = showRepository;
            _auditLogService = auditLogService;
        }

        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n===== SEAT MANAGEMENT =====");
                Console.WriteLine("1. Add Seat");
                Console.WriteLine("2. View All Seats");
                Console.WriteLine("3. Search Seat By ID");
                Console.WriteLine("4. View Seats By Show");
                Console.WriteLine("5. Update Seat");
                Console.WriteLine("6. Delete Seat");
                Console.WriteLine("0. Back");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        AddSeat();
                        break;

                    case "2":
                        ViewAllSeats();
                        break;

                    case "3":
                        SearchSeat();
                        break;

                    case "4":
                        ViewSeatsByShow();
                        break;

                    case "5":
                        UpdateSeat();
                        break;

                    case "6":
                        DeleteSeat();
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

        // ADD SEAT
        private void AddSeat()
        {
            Console.WriteLine("\n===== ADD SEAT =====");

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

            Console.Write("Enter Row Number (A, B, C...): ");
            string rowNumber = (Console.ReadLine() ?? "")
                .Trim()
                .ToUpper();

            if (string.IsNullOrWhiteSpace(rowNumber) ||
                rowNumber.Length > 5)
            {
                Console.WriteLine("Invalid row number!");
                return;
            }

            Console.Write("Enter Seat Number: ");

            if (!int.TryParse(Console.ReadLine(), out int seatNumber) ||
                seatNumber <= 0)
            {
                Console.WriteLine("Seat number must be greater than zero!");
                return;
            }

            Console.Write("Enter Seat Type (Regular/Premium): ");
            string seatType = (Console.ReadLine() ?? "")
                .Trim();

            if (seatType.Equals("Regular",
                StringComparison.OrdinalIgnoreCase))
            {
                seatType = "Regular";
            }
            else if (seatType.Equals("Premium",
                StringComparison.OrdinalIgnoreCase))
            {
                seatType = "Premium";
            }
            else
            {
                Console.WriteLine("Enter Regular or Premium only!");
                return;
            }

            bool exists = _seatRepository.IsSeatPositionTaken(
                showId, rowNumber, seatNumber);

            if (exists)
            {
                Console.WriteLine("This seat position already exists!");
                return;
            }

            Seat seat = new Seat
            {
                ShowId = showId,
                RowNumber = rowNumber,
                SeatNumber = seatNumber,
                SeatType = seatType
            };

            
                _seatRepository.AddSeat(seat);
                _auditLogService.Log("Seat", "Added", $"Seat ID: {seat.SeatId} | Show ID: {seat.ShowId} | Row: {seat.RowNumber} | Seat: {seat.SeatNumber}");
                Console.WriteLine("Seat added successfully!");
            
           
        }

        // VIEW ALL SEATS
        private void ViewAllSeats()
        {
            Console.WriteLine("\n===== ALL SEATS =====");

            List<Seat> seats = _seatRepository.GetAllSeats();

            if (seats.Count == 0)
            {
                Console.WriteLine("No seats found!");
                return;
            }

            foreach (Seat seat in seats)
            {
                Console.WriteLine("----------------------");
                Console.WriteLine($"Seat ID: {seat.SeatId}");
                Console.WriteLine($"Show ID: {seat.ShowId}");
                Console.WriteLine($"Movie: {seat.Show?.Movie?.Title ?? "Unknown"}");
                Console.WriteLine($"Row: {seat.RowNumber}");
                Console.WriteLine($"Seat Number: {seat.SeatNumber}");
                Console.WriteLine($"Seat Type: {seat.SeatType}");
            }
        }

        // SEARCH SEAT
        private void SearchSeat()
        {
            Console.WriteLine("\n===== SEARCH SEAT =====");

            Console.Write("Enter Seat ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid Seat ID!");
                return;
            }

            Seat? seat = _seatRepository.GetSeatById(id);

            if (seat == null)
            {
                Console.WriteLine("Seat not found!");
                return;
            }

            Console.WriteLine($"Seat ID: {seat.SeatId}");
            Console.WriteLine($"Show ID: {seat.ShowId}");
            Console.WriteLine($"Row: {seat.RowNumber}");
            Console.WriteLine($"Seat Number: {seat.SeatNumber}");
            Console.WriteLine($"Seat Type: {seat.SeatType}");
        }

        // VIEW SEATS BY SHOW
        private void ViewSeatsByShow()
        {
            Console.WriteLine("\n===== VIEW SEATS BY SHOW =====");

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

            List<Seat> seats = _seatRepository.GetSeatsByShow(showId);

            if (seats.Count == 0)
            {
                Console.WriteLine("No seats available for this show!");
                return;
            }

            Console.WriteLine($"Movie: {show.Movie?.Title ?? "Unknown"}");

            foreach (Seat seat in seats)
            {
                Console.WriteLine(
                    $"Row: {seat.RowNumber} | " +
                    $"Seat: {seat.SeatNumber} | " +
                    $"Type: {seat.SeatType}");
            }
        }

        // UPDATE SEAT
        private void UpdateSeat()
        {
            Console.WriteLine("\n===== UPDATE SEAT =====");

            Console.Write("Enter Seat ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid Seat ID!");
                return;
            }

            Seat? seat = _seatRepository.GetSeatById(id);

            if (seat == null)
            {
                Console.WriteLine("Seat not found!");
                return;
            }

            Console.Write("Enter New Show ID: ");

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

            Console.Write("Enter New Row Number: ");

            string rowNumber = (Console.ReadLine() ?? "")
                .Trim()
                .ToUpper();

            if (string.IsNullOrWhiteSpace(rowNumber) ||
                rowNumber.Length > 5)
            {
                Console.WriteLine("Invalid row number!");
                return;
            }

            Console.Write("Enter New Seat Number: ");

            if (!int.TryParse(Console.ReadLine(), out int seatNumber) ||
                seatNumber <= 0)
            {
                Console.WriteLine("Invalid seat number!");
                return;
            }

            Console.Write("Enter New Seat Type (Regular/Premium): ");

            string seatType = (Console.ReadLine() ?? "").Trim();

            if (seatType.Equals("Regular",
                StringComparison.OrdinalIgnoreCase))
            {
                seatType = "Regular";
            }
            else if (seatType.Equals("Premium",
                StringComparison.OrdinalIgnoreCase))
            {
                seatType = "Premium";
            }
            else
            {
                Console.WriteLine("Enter Regular or Premium only!");
                return;
            }

            bool exists = _seatRepository.IsSeatPositionTaken(
                showId, rowNumber, seatNumber, id);

            if (exists)
            {
                Console.WriteLine("This seat position is already occupied!");
                return;
            }

            seat.ShowId = showId;
            seat.RowNumber = rowNumber;
            seat.SeatNumber = seatNumber;
            seat.SeatType = seatType;

          
                _seatRepository.UpdateSeat(seat);
                _auditLogService.Log("Seat", "Updated", $"Seat ID: {seat.SeatId} | Show ID: {seat.ShowId} | Row: {seat.RowNumber} | Seat: {seat.SeatNumber}");
                Console.WriteLine("Seat updated successfully!");
            
           
            
        }

        // DELETE SEAT
        private void DeleteSeat()
        {
            Console.WriteLine("\n===== DELETE SEAT =====");

            Console.Write("Enter Seat ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Invalid Seat ID!");
                return;
            }

            Seat? seat = _seatRepository.GetSeatById(id);

            if (seat == null)
            {
                Console.WriteLine("Seat not found!");
                return;
            }

            Console.WriteLine(
                $"Row {seat.RowNumber}, Seat {seat.SeatNumber}");

            Console.Write("Are you sure you want to delete? (y/n): ");

            string confirmation = Console.ReadLine() ?? "";

            if (confirmation.Equals("y",
                StringComparison.OrdinalIgnoreCase))
            {
                _seatRepository.DeleteSeat(seat);
                _auditLogService.Log("Seat", "Deleted", $"Seat ID: {seat.SeatId}");
                Console.WriteLine("Seat deleted successfully!");
            }
            else
            {
                Console.WriteLine("Delete cancelled!");
            }
        }
    }
}