using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    public class RoomsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetAllRooms()
        {
            return Ok(new
            {
                success = true,
                count = HotelDatabase.Rooms.Count,
                Data = HotelDatabase.Rooms
            });
        }

        [HttpGet("{id}")]
        public ActionResult<Room> GetRoom(int id)
        {
            var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "חדר לא נמצא"
                });
            }

            return Ok(new
            {
                success = true,
                count = HotelDatabase.Rooms.Count,
                data = room
            });
        }

        [HttpPost]
        public ActionResult<Room> CreateRoom([FromBody] Room newRoom)
        {
            // בדיקות תקינות
            if (string.IsNullOrEmpty(newRoom.RoomNumber) ||
                string.IsNullOrEmpty(newRoom.Type) ||
                newRoom.PricePerNight <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "חסרים שדות חובה או שדות לא תקינים"
                });
            }

            // יצירת ID אוטומטי
            newRoom.Id = HotelDatabase.Rooms.Any()
                ? HotelDatabase.Rooms.Max(r => r.Id) + 1
                : 1;

            // ברירת מחדל לסטטוס
            if (string.IsNullOrEmpty(newRoom.Status))
            {
                newRoom.Status = "available";
            }

            // הוספה לרשימה
            HotelDatabase.Rooms.Add(newRoom);

            // החזרת 201 Created עם הנתיב לחדר החדש
            return CreatedAtAction(
                nameof(GetRoom),
                new { id = newRoom.Id },
                new
                {
                    success = true,
                    message = "חדר נוסף בהצלחה",
                    data = newRoom
                });
        }


        [HttpPut("{id}")]
        public ActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
        {
            // חיפוש החדר
            var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "חדר לא נמצא"
                });
            }

            // עדכון השדות (רק אם נשלחו)
            if (!string.IsNullOrEmpty(updatedRoom.RoomNumber))
                room.RoomNumber = updatedRoom.RoomNumber;

            if (!string.IsNullOrEmpty(updatedRoom.Type))
                room.Type = updatedRoom.Type;

            if (updatedRoom.PricePerNight > 0)
                room.PricePerNight = updatedRoom.PricePerNight;

            if (updatedRoom.Floor > 0)
                room.Floor = updatedRoom.Floor;

            if (!string.IsNullOrEmpty(updatedRoom.Status))
                room.Status = updatedRoom.Status;

            return Ok(new
            {
                success = true,
                message = "חדר עודכן בהצלחה",
                data = room
            });
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteRoom(int id)
        {
            var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "חדר לא נמצא"
                });
            }

            // מחיקה מהרשימה
            HotelDatabase.Rooms.Remove(room);

            return Ok(new
            {
                success = true,
                message = "חדר נמחק בהצלחה",
                data = room
            });
        }


        [HttpPut("{id}/status")]
        public ActionResult UpdateRoomStatus(int id, [FromBody] RoomStatusUpdate statusUpdate)
        {
            // רשימת סטטוסים תקינים
            var validStatuses = new[] { "available", "occupied", "maintenance" };

            if (string.IsNullOrEmpty(statusUpdate.Status) ||
                !validStatuses.Contains(statusUpdate.Status))
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"סטטוס לא תקין. אפשרויות: {string.Join(", ", validStatuses)}"
                });
            }

            var room = HotelDatabase.Rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "חדר לא נמצא"
                });
            }

            // עדכון הסטטוס בלבד
            room.Status = statusUpdate.Status;

            return Ok(new
            {
                success = true,
                message = "סטטוס החדר עודכן",
                data = room
            });
        }
    }

    // מחלקת עזר לעדכון סטטוס
    public class RoomStatusUpdate
    {
        public string Status { get; set; }
    }

}
    /* ..קונטרול לנהיול אורחים במלון*/



