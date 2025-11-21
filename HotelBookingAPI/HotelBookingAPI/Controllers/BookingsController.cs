using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {

        [HttpGet]
        public ActionResult<IEnumerable<Booking>> GetAllBookings([FromQuery] string? status)
        {
            // אם נשלח פרמטר status ב-query string, נסנן לפיו
            var bookings = HotelDatabase.Bookings.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                bookings = bookings.Where(b => b.Status == status);
            }

            var bookingsList = bookings.ToList();

            return Ok(new
            {
                success = true,
                count = bookingsList.Count,
                data = bookingsList
            });
        }

        // ===============================================
        // 2. GET: api/bookings/5 - שליפת הזמנה ספציפית
        // (כולל פרטי האורח והחדר)
        // ===============================================
        [HttpGet("{id}")]
        public ActionResult<Booking> GetBooking(int id)
        {
            var booking = HotelDatabase.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "הזמנה לא נמצאה"
                });
            }

            // הוספת פרטי האורח והחדר
            var guest = HotelDatabase.Guests.FirstOrDefault(g => g.Id == booking.GuestId);
            var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == booking.RoomId);

            return Ok(new
            {
                success = true,
                data = new
                {
                    booking.Id,
                    booking.GuestId,
                    booking.RoomId,
                    booking.CheckIn,
                    booking.CheckOut,
                    booking.NumberOfGuests,
                    booking.TotalPrice,
                    booking.Status,
                    booking.Notes,
                    guestDetails = guest,
                    roomDetails = room
                }
            });
        }

        // ===============================================
        // 3. POST: api/bookings - יצירת הזמנה חדשה
        // ===============================================
        [HttpPost]
        public ActionResult<Booking> CreateBooking([FromBody] Booking newBooking)
        {
            // בדיקות תקינות
            if (newBooking.GuestId <= 0 ||
                newBooking.RoomId <= 0 ||
                newBooking.CheckIn == default ||
                newBooking.CheckOut == default)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "חסרים שדות חובה: GuestId, RoomId, CheckIn, CheckOut"
                });
            }

            // בדיקה שהאורח קיים
            var guest = HotelDatabase.Guests.FirstOrDefault(g => g.Id == newBooking.GuestId);
            if (guest == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "אורח לא נמצא במערכת"
                });
            }

            // בדיקה שהחדר קיים
            var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == newBooking.RoomId);
            if (room == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "חדר לא נמצא במערכת"
                });
            }

            // בדיקה שהחדר זמין
            if (room.Status != "available")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "החדר אינו זמין כרגע",
                    currentStatus = room.Status
                });
            }

            // בדיקת תאריכים תקינים
            if (newBooking.CheckOut <= newBooking.CheckIn)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "תאריך צ'ק-אאוט חייב להיות אחרי תאריך צ'ק-אין"
                });
            }

            // חישוב מספר לילות ומחיר כולל
            var nights = (newBooking.CheckOut - newBooking.CheckIn).Days;
            var totalPrice = nights * room.PricePerNight;

            // יצירת ההזמנה
            newBooking.Id = HotelDatabase.Bookings.Any()
                ? HotelDatabase.Bookings.Max(b => b.Id) + 1
                : 1;

            newBooking.TotalPrice = totalPrice;
            newBooking.Status = "pending"; // ברירת מחדל - ממתין לאישור

            if (newBooking.NumberOfGuests <= 0)
            {
                newBooking.NumberOfGuests = 1;
            }

            // הוספה לרשימה
            HotelDatabase.Bookings.Add(newBooking);

            // עדכון סטטוס החדר לתפוס
            room.Status = "occupied";

            return CreatedAtAction(
                nameof(GetBooking),
                new { id = newBooking.Id },
                new
                {
                    success = true,
                    message = "הזמנה נוצרה בהצלחה",
                    data = newBooking,
                    calculatedNights = nights
                });
        }

        // ===============================================
        // 4. PUT: api/bookings/5 - עדכון הזמנה
        // ===============================================
        [HttpPut("{id}")]
        public ActionResult UpdateBooking(int id, [FromBody] Booking updatedBooking)
        {
            var booking = HotelDatabase.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "הזמנה לא נמצאה"
                });
            }

            // לא ניתן לעדכן הזמנה שבוטלה או הושלמה
            if (booking.Status == "cancelled" || booking.Status == "completed")
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"לא ניתן לעדכן הזמנה עם סטטוס: {booking.Status}"
                });
            }

            // עדכון תאריכים (אם נשלחו)
            if (updatedBooking.CheckIn != default)
            {
                booking.CheckIn = updatedBooking.CheckIn;
            }

            if (updatedBooking.CheckOut != default)
            {
                booking.CheckOut = updatedBooking.CheckOut;
            }

            // בדיקת תקינות תאריכים
            if (booking.CheckOut <= booking.CheckIn)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "תאריך צ'ק-אאוט חייב להיות אחרי תאריך צ'ק-אין"
                });
            }

            // חישוב מחדש של המחיר הכולל
            var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == booking.RoomId);
            if (room != null)
            {
                var nights = (booking.CheckOut - booking.CheckIn).Days;
                booking.TotalPrice = nights * room.PricePerNight;
            }

            // עדכון שאר השדות
            if (updatedBooking.NumberOfGuests > 0)
            {
                booking.NumberOfGuests = updatedBooking.NumberOfGuests;
            }

            if (!string.IsNullOrEmpty(updatedBooking.Notes))
            {
                booking.Notes = updatedBooking.Notes;
            }

            if (!string.IsNullOrEmpty(updatedBooking.Status))
            {
                booking.Status = updatedBooking.Status;
            }

            return Ok(new
            {
                success = true,
                message = "הזמנה עודכנה בהצלחה",
                data = booking
            });
        }

        // ===============================================
        // 5. DELETE: api/bookings/5 - ביטול/מחיקת הזמנה
        // ===============================================
        [HttpDelete("{id}")]
        public ActionResult DeleteBooking(int id)
        {
            var booking = HotelDatabase.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "הזמנה לא נמצאה"
                });
            }

            // שחרור החדר (עדכון סטטוס לזמין)
            var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == booking.RoomId);
            if (room != null)
            {
                room.Status = "available";
            }

            // מחיקה מהרשימה
            HotelDatabase.Bookings.Remove(booking);

            return Ok(new
            {
                success = true,
                message = "הזמנה בוטלה ונמחקה בהצלחה",
                data = booking
            });
        }

        // ===============================================
        // ROUTE נוסף 1: PUT: api/bookings/5/status
        // עדכון סטטוס הזמנה בלבד
        // ===============================================
        [HttpPut("{id}/status")]
        public ActionResult UpdateBookingStatus(int id, [FromBody] BookingStatusUpdate statusUpdate)
        {
            var validStatuses = new[] { "pending", "confirmed", "cancelled", "completed" };

            if (string.IsNullOrEmpty(statusUpdate.Status) ||
                !validStatuses.Contains(statusUpdate.Status))
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"סטטוס לא תקין. אפשרויות: {string.Join(", ", validStatuses)}"
                });
            }

            var booking = HotelDatabase.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "הזמנה לא נמצאה"
                });
            }

            // אם ההזמנה מבוטלת - שחרר את החדר
            if (statusUpdate.Status == "cancelled" || statusUpdate.Status == "completed")
            {
                var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == booking.RoomId);
                if (room != null)
                {
                    room.Status = "available";
                }
            }

            booking.Status = statusUpdate.Status;

            return Ok(new
            {
                success = true,
                message = "סטטוס ההזמנה עודכן",
                data = booking
            });
        }

        // ===============================================
        // ROUTE נוסף 2: GET: api/bookings/upcoming
        // שליפת הזמנות קרובות (check-in היום או מחר)
        // ===============================================
        [HttpGet("upcoming")]
        public ActionResult GetUpcomingBookings()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var upcomingBookings = HotelDatabase.Bookings
                .Where(b => b.CheckIn.Date == today || b.CheckIn.Date == tomorrow)
                .Where(b => b.Status == "confirmed")
                .ToList();

            return Ok(new
            {
                success = true,
                count = upcomingBookings.Count,
                data = upcomingBookings
            });
        }
    }

    // מחלקת עזר לעדכון סטטוס הזמנה
    public class BookingStatusUpdate
    {
        public string Status { get; set; }
    }

}

