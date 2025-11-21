using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        // ===============================================
        // 1. GET: api/guests - שליפת כל האורחים
        // ===============================================
        [HttpGet]
        public ActionResult<IEnumerable<Guest>> GetAllGuests()
        {
            return Ok(new
            {
                success = true,
                count = HotelDatabase.Guests.Count,
                data = HotelDatabase.Guests
            });
        }

        // ===============================================
        // 2. GET: api/guests/5 - שליפת אורח לפי ID
        // ===============================================
        [HttpGet("{id}")]
        public ActionResult<Guest> GetGuest(int id)
        {
            var guest = HotelDatabase.Guests.FirstOrDefault(g => g.Id == id);

            if (guest == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "אורח לא נמצא"
                });
            }

            return Ok(new
            {
                success = true,
                data = guest
            });
        }

        // ===============================================
        // 3. POST: api/guests - הוספת אורח חדש
        // ===============================================
        [HttpPost]
        public ActionResult<Guest> CreateGuest([FromBody] Guest newGuest)
        {
            // בדיקות תקינות
            if (string.IsNullOrEmpty(newGuest.FirstName) ||
                string.IsNullOrEmpty(newGuest.LastName) ||
                string.IsNullOrEmpty(newGuest.IdNumber) ||
                string.IsNullOrEmpty(newGuest.Phone))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "חסרים שדות חובה: FirstName, LastName, IdNumber, Phone"
                });
            }

            // בדיקה אם תעודת זהות כבר קיימת
            var existingGuest = HotelDatabase.Guests
                .FirstOrDefault(g => g.IdNumber == newGuest.IdNumber);

            if (existingGuest != null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "אורח עם תעודת זהות זו כבר קיים במערכת"
                });
            }

            // יצירת ID אוטומטי
            newGuest.Id = HotelDatabase.Guests.Any()
                ? HotelDatabase.Guests.Max(g => g.Id) + 1
                : 1;

            // הוספה לרשימה
            HotelDatabase.Guests.Add(newGuest);

            return CreatedAtAction(
                nameof(GetGuest),
                new { id = newGuest.Id },
                new
                {
                    success = true,
                    message = "אורח נוסף בהצלחה",
                    data = newGuest
                });
        }

        // ===============================================
        // 4. PUT: api/guests/5 - עדכון אורח
        // ===============================================
        [HttpPut("{id}")]
        public ActionResult UpdateGuest(int id, [FromBody] Guest updatedGuest)
        {
            var guest = HotelDatabase.Guests.FirstOrDefault(g => g.Id == id);

            if (guest == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "אורח לא נמצא"
                });
            }

            // עדכון השדות (רק אם נשלחו)
            if (!string.IsNullOrEmpty(updatedGuest.FirstName))
                guest.FirstName = updatedGuest.FirstName;

            if (!string.IsNullOrEmpty(updatedGuest.LastName))
                guest.LastName = updatedGuest.LastName;

            if (!string.IsNullOrEmpty(updatedGuest.IdNumber))
                guest.IdNumber = updatedGuest.IdNumber;

            if (!string.IsNullOrEmpty(updatedGuest.Phone))
                guest.Phone = updatedGuest.Phone;

            // Email יכול להיות null, לכן נעדכן אותו גם אם הוא ריק
            guest.Email = updatedGuest.Email;

            return Ok(new
            {
                success = true,
                message = "אורח עודכן בהצלחה",
                data = guest
            });
        }

        // ===============================================
        // 5. DELETE: api/guests/5 - מחיקת אורח
        // ===============================================
        [HttpDelete("{id}")]
        public ActionResult DeleteGuest(int id)
        {
            var guest = HotelDatabase.Guests.FirstOrDefault(g => g.Id == id);

            if (guest == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "אורח לא נמצא"
                });
            }

            // בדיקה אם לאורח יש הזמנות פעילות
            var activeBookings = HotelDatabase.Bookings
                .Where(b => b.GuestId == id && b.Status != "cancelled")
                .ToList();

            if (activeBookings.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "לא ניתן למחוק אורח עם הזמנות פעילות",
                    activeBookingsCount = activeBookings.Count
                });
            }

            // מחיקה מהרשימה
            HotelDatabase.Guests.Remove(guest);

            return Ok(new
            {
                success = true,
                message = "אורח נמחק בהצלחה",
                data = guest
            });
        }

        // ===============================================
        // ROUTE נוסף: GET: api/guests/5/bookings
        // שליפת כל ההזמנות של אורח ספציפי
        // ===============================================
        [HttpGet("{id}/bookings")]
        public ActionResult GetGuestBookings(int id)
        {
            // וידוא שהאורח קיים
            var guest = HotelDatabase.Guests.FirstOrDefault(g => g.Id == id);

            if (guest == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "אורח לא נמצא"
                });
            }

            // שליפת כל ההזמנות של האורח
            var guestBookings = HotelDatabase.Bookings
                .Where(b => b.GuestId == id)
                .ToList();

            return Ok(new
            {
                success = true,
                guest = new
                {
                    id = guest.Id,
                    name = $"{guest.FirstName} {guest.LastName}"
                },
                count = guestBookings.Count,
                data = guestBookings
            });
        }
    }
}
