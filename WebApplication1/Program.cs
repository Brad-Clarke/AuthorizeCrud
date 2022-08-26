using Microsoft.EntityFrameworkCore;
using ModelAuthorization;
using WebApplication1.Domain;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ICrudAuthorizationPolicyProvider>(_ => new BasicCrudAuthorizationPolicyProvider("user.admin"));

            builder.Services.AddDbContext<StudentDbContext>((p, o) =>
            {
                o.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ModelAuthorizationTesting;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=True");
                o.UseModelAuthorization();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<StudentDbContext>().Database.EnsureCreated();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}