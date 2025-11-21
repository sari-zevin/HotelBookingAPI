using HotelBookingAPI.Models;

namespace HotelBookingAPI.Data
{
    public class HotelDatabase
    {
        // רשימת חדרים - static כדי שתישאר בזיכרון לאורך החיים של האפליקציה
        public static List<Room> Rooms { get; set; } = new List<Room>
        {
            new Room
            {
                Id = 1,
                RoomNumber = "101",
                Type = "single",
                PricePerNight = 300,
                Floor = 1,
                Status = "available"
            },
            new Room
            {
                Id = 2,
                RoomNumber = "102",
                Type = "double",
                PricePerNight = 450,
                Floor = 1,
                Status = "available"
            },
            new Room
            {
                Id = 3,
                RoomNumber = "201",
                Type = "suite",
                PricePerNight = 800,
                Floor = 2,
                Status = "occupied"
            }
        };

        // רשימת אורחים
        public static List<Guest> Guests { get; set; } = new List<Guest>
        {
            new Guest
            {
                Id = 1,
                FirstName = "יוסי",
                LastName = "כהן",
                IdNumber = "123456789",
                Phone = "050-1234567",
                Email = "yossi@example.com"
            },
            new Guest
            {
                Id = 2,
                FirstName = "רחל",
                LastName = "לוי",
                IdNumber = "987654321",
                Phone = "052-9876543",
                Email = "rachel@example.com"
            }
        };

        // רשימת הזמנות
        public static List<Booking> Bookings { get; set; } = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                GuestId = 1,
                RoomId = 3,
                CheckIn = new DateTime(2025, 11, 25),
                CheckOut = new DateTime(2025, 11, 28),
                NumberOfGuests = 2,
                TotalPrice = 2400,
                Status = "confirmed",
                Notes = "בקשה למיטת תינוק"
            }
        };
    }
}
