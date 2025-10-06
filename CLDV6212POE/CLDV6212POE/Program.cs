using CLDV6212POE.Services;
using System.Configuration;

    public class Program
    {
        public static void Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //register TableStorageService with configuration
            builder.Services.AddSingleton(new TableStorageService(configuration.GetConnectionString("AzureStorage")));

            //register BlobService with configuration
            builder.Services.AddSingleton(new BlobService(configuration.GetConnectionString("AzureStorage")));

            //register QueueService with configuration
            builder.Services.AddSingleton<QueueService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new QueueService(connectionString, "orders");
            });

            //register AzureFileShareService with configuration
            builder.Services.AddSingleton<AzureFileShareService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new AzureFileShareService(connectionString, "ordershare");
            });

            builder.Services.AddHttpClient<FunctionService>(client =>
            {
                client.BaseAddress = new Uri("https://YOUR_FUNCTION_APP.azurewebsites.net/");
            });
            builder.Services.AddSingleton<FunctionService>();


        var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }



