using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ===============================================
// הוספת שירותים לקונטיינר
// ===============================================

// הוספת Controllers
builder.Services.AddControllers();

// הוספת CORS (כדי לאפשר קריאות מדפדפן)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()    // מאפשר מכל מקור
                  .AllowAnyMethod()    // מאפשר כל HTTP method
                  .AllowAnyHeader();   // מאפשר כל header
        });
});

// הוספת Swagger לתיעוד (אופציונלי אבל מומלץ מאוד!)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===============================================
// הגדרת Pipeline של הבקשות
// ===============================================

// אם אנחנו בסביבת פיתוח - הפעל Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// שימוש ב-CORS
app.UseCors("AllowAll");

// מיפוי Controllers
app.MapControllers();

// הודעה ידידותית בהפעלה
Console.WriteLine("?? Hotel Booking API is running!");
Console.WriteLine("?? Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("?? API Base URL: http://localhost:5000/api");

// הפעלת האפליקציה
app.Run();