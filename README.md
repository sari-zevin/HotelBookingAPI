# HotelBookingAPI


# 🏨 Hotel Booking API

מערכת ניהול הזמנות למלון - RESTful API

## 📝 תיאור הפרויקט

מערכת לניהול הזמנות במלון. באמצעות המערכת ניתן לנהל חדרים, אורחים והזמנות בצורה מסודרת ויעילה. המערכת מאפשרת לעובדי המלון לעקוב אחרי תפוסת החדרים, לנהל את פרטי האורחים וליצור ולעדכן הזמנות.

## 🔷 ישויות

המערכת מכילה שלוש ישויות מרכזיות:

### 1. Room (חדר)
מייצג חדר פיזי במלון
- **Id** - מזהה ייחודי
- **RoomNumber** - מספר החדר
- **Type** - סוג החדר (single, double, suite)
- **PricePerNight** - מחיר ללילה
- **Floor** - קומה
- **Status** - סטטוס (available, occupied, maintenance)

### 2. Guest (אורח)
מייצג אורח/לקוח המלון
- **Id** - מזהה ייחודי
- **FirstName** - שם פרטי
- **LastName** - שם משפחה
- **IdNumber** - תעודת זהות/דרכון
- **Phone** - טלפון
- **Email** - אימייל (אופציונלי)

### 3. Booking (הזמנה)
מייצג הזמנה - קשר בין אורח לחדר בתאריכים מסוימים
- **Id** - מזהה ייחודי
- **GuestId** - מזהה האורח (Foreign Key)
- **RoomId** - מזהה החדר (Foreign Key)
- **CheckIn** - תאריך צ'ק-אין
- **CheckOut** - תאריך צ'ק-אאוט
- **NumberOfGuests** - מספר אורחים
- **TotalPrice** - מחיר כולל
- **Status** - סטטוס (pending, confirmed, cancelled, completed)
- **Notes** - הערות מיוחדות (אופציונלי)

## 🔗 קשרים בין ישויות
```
Guest (אורח) ←→ Booking (הזמנה) ←→ Room (חדר)
```

- אורח אחד יכול לבצע מספר הזמנות לאורך זמן
- חדר אחד יכול להיות בהזמנות שונות בתאריכים שונים
- הזמנה מחברת בין אורח אחד לחדר אחד בתאריכים ספציפיים

## 🛣️ API Endpoints

### Rooms (חדרים)

| Method | Endpoint | תיאור |
|--------|----------|-------|
| GET | `/api/rooms` | שליפת רשימת כל החדרים |
| GET | `/api/rooms/{id}` | שליפת חדר ספציפי לפי מזהה |
| POST | `/api/rooms` | הוספת חדר חדש |
| PUT | `/api/rooms/{id}` | עדכון פרטי חדר |
| DELETE | `/api/rooms/{id}` | מחיקת חדר |
| PUT | `/api/rooms/{id}/status` | עדכון סטטוס חדר בלבד |

### Guests (אורחים)

| Method | Endpoint | תיאור |
|--------|----------|-------|
| GET | `/api/guests` | שליפת רשימת כל האורחים |
| GET | `/api/guests/{id}` | שליפת אורח ספציפי לפי מזהה |
| POST | `/api/guests` | הוספת אורח חדש |
| PUT | `/api/guests/{id}` | עדכון פרטי אורח |
| DELETE | `/api/guests/{id}` | מחיקת אורח |
| GET | `/api/guests/{id}/bookings` | שליפת כל ההזמנות של אורח ספציפי |

### Bookings (הזמנות)

| Method | Endpoint | תיאור |
|--------|----------|-------|
| GET | `/api/bookings` | שליפת רשימת כל ההזמנות |
| GET | `/api/bookings/{id}` | שליפת הזמנה ספציפית לפי מזהה |
| POST | `/api/bookings` | יצירת הזמנה חדשה |
| PUT | `/api/bookings/{id}` | עדכון פרטי הזמנה |
| DELETE | `/api/bookings/{id}` | ביטול/מחיקת הזמנה |
| PUT | `/api/bookings/{id}/status` | עדכון סטטוס הזמנה בלבד |
| GET | `/api/bookings/upcoming` | שליפת הזמנות קרובות (check-in היום או מחר) |

## 📋 דוגמאות שימוש

### שליפת כל החדרים
```http
GET /api/rooms
```

**תשובה:**
```json
{
  "success": true,
  "count": 3,
  "data": [
    {
      "id": 1,
      "roomNumber": "101",
      "type": "single",
      "pricePerNight": 300,
      "floor": 1,
      "status": "available"
    }
  ]
}
```

### הוספת אורח חדש
```http
POST /api/guests
Content-Type: application/json

{
  "firstName": "יוסי",
  "lastName": "כהן",
  "idNumber": "123456789",
  "phone": "050-1234567",
  "email": "yossi@example.com"
}
```

### יצירת הזמנה חדשה
```http
POST /api/bookings
Content-Type: application/json

{
  "guestId": 1,
  "roomId": 2,
  "checkIn": "2025-12-01",
  "checkOut": "2025-12-05",
  "numberOfGuests": 2,
  "notes": "בקשה לקומה גבוהה"
}
```

## 🔍 סינונים ופרמטרים

### חיפוש חדרים זמינים
```http
GET /api/rooms?status=available
```

### סינון הזמנות לפי סטטוס
```http
GET /api/bookings?status=confirmed
```

## ⚙️ טכנולוגיות

- **ASP.NET Core Web API** - Framework
- **C#** - שפת תכנות
- **Swagger** - תיעוד אינטראקטיבי
- מאגר נתונים זמני (List) - בשלב הפיתוח

## 🚀 הרצת הפרויקט

1. שכפלו את הפרויקט:
```bash
git clone https://github.com/YOUR_USERNAME/HotelBookingAPI.git
```

2. פתחו ב-Visual Studio 2022

3. הריצו את הפרויקט (F5)

4. גשו ל-Swagger UI:
```
http://localhost:5269/swagger
```

## 📝 הערות

- בשלב זה הנתונים נשמרים בזיכרון בלבד (לא נשמרים לאחר כיבוי השרת)
- בשלב הבא יתווסף מסד נתונים קבוע (SQL Server + Entity Framework)
- כל Controller מנהל List משלו שמתפקד כתחליף זמני לטבלת מסד נתונים

## 📌 Routes נוספים מעבר לבסיסיים

מעבר לפעולות ה-CRUD הבסיסיות, המערכת מספקת:

1. **עדכון סטטוס חדר** - עדכון מהיר של סטטוס החדר בלבד
2. **היסטוריית הזמנות לאורח** - צפייה בכל ההזמנות של אורח ספציפי
3. **עדכון סטטוס הזמנה** - עדכון מהיר של סטטוס ההזמנה
4. **הזמנות קרובות** - סינון מובנה להזמנות עתידיות קרובות

## 👨‍💻 מחבר

שרי זוין Web API

## 📄 רישיון

פרויקט לימודי - 2025
