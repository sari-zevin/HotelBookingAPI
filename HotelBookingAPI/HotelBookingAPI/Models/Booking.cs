namespace HotelBookingAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // מזהה האורח (Foreign Key)
        public int GuestId { get; set; }

        // מזהה החדר (Foreign Key)
        public int RoomId { get; set; }

        // תאריך צ'ק-אין
        public DateTime CheckIn { get; set; }

        // תאריך צ'ק-אאוט
        public DateTime CheckOut { get; set; }

        // מספר אורחים
        public int NumberOfGuests { get; set; }

        // מחיר כולל
        public decimal TotalPrice { get; set; }

        // סטטוס: pending, confirmed, cancelled, completed
        public string Status { get; set; }

        // הערות מיוחדות (אופציונלי)
        public string? Notes { get; set; }
    }
}
