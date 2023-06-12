using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Crypto;
using StorageOperations.Implementation;
using StorageOperations.Interface;

var host = new HostBuilder()
   .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        var config = builder.Build();
    })
    .ConfigureServices((ctx, s) =>
    {
        //Databse Connection
        //var connectionString = ctx.Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value.ToString();

        //s.AddDbContext<samplecontext>(options =>
        //{
        //    options.UseSqlServer(connectionString);
        //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        //});
        s.AddLogging();
      
        s.AddScoped<ITableStorageOperations, TableStorageOperations>();
   
    })
    .Build();

host.Run();
