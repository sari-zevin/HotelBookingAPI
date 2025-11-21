namespace HotelBookingAPI.Models
{
    public class Room
    {
        // מזהה ייחודי של החדר
        public int Id { get; set; }

        // מספר החדר (למשל: "101", "205")
        public string RoomNumber { get; set; }

        // סוג החדר: single, double, suite
        public string Type { get; set; }

        // מחיר ללילה
        public decimal PricePerNight { get; set; }

        // קומה
        public int Floor { get; set; }

        // סטטוס: available, occupied, maintenance
        public string Status { get; set; }
    }
}


