using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Models;
using TmsApi.Middleware;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        // Add Entity Framework Core with SQLite
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.Services.AddOptions<PaymentOption>()
            .BindConfiguration("Payments")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TmsApi v1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseCors("AllowAll");

        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "An error occurred" });
            });
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/api/assessments/results", () =>
        {
            return Results.Ok(new
            {
                courseCode = "CS-101",
                studentId = "S-001",
                letterGrade = "A"
            });
        });

        app.MapPost("/api/enrollments", async (ApplicationDbContext context, EnrollmentRequest request) =>
        {
            var enrollment = new TmsApi.Data.Enrollment
            {
                StudentId = request.StudentId,
                CourseCode = request.CourseCode,
                EnrollmentDate = DateTime.UtcNow
            };
            context.Enrollments.Add(enrollment);
            await context.SaveChangesAsync();
            return Results.Created($"/api/enrollments/{enrollment.Id}", enrollment);
        });

        app.MapGet("/api/enrollments", async (ApplicationDbContext context) =>
        {
            return Results.Ok(await context.Enrollments.ToListAsync());
        });

        app.Run();
    }
}

internal class PaymentOption
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

internal record EnrollmentRequest(string StudentId, string CourseCode);