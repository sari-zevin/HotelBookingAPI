namespace HotelBookingAPI.Models
{
    public class Guest
    {
        public int Id { get; set; }

        // שם פרטי
        public string FirstName { get; set; }

        // שם משפחה
        public string LastName { get; set; }

        // תעודת זהות / דרכון
        public string IdNumber { get; set; }

        // טלפון
        public string Phone { get; set; }

        // אימייל (אופציונלי)
        public string? Email { get; set; } // ה-? אומר שזה יכול להיות null
    }
}
