using DataAccess.Repositories.ParcelRepo;
using DataAccess.Repositories.QueueRepo;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ConsumerWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               .WriteTo.Console()
               .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();

            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();

                    //TODO: Move the "Data Source" to config file
                    services.AddDbContext<QueueDbContext>(options => options.UseSqlite("Data Source=../../Databases/EventsQueue.db"));
                    services.AddDbContext<ParcelDbContext>(options => options.UseSqlite("Data Source=../../Databases/Parcel.db"));

                    services.AddTransient<IQueueRepository, QueueRepository>();
                    services.AddTransient<IParcelRepository, ParcelRepository>();

                    services.AddLogging(builder =>
                    {
                        builder.AddSerilog();
                    });
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ParcelDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }

            host.Run();
        }
    }
}