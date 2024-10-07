//using BestReg.Converters;
//using BestReg.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Threading.Tasks;

//public class SyncService
//{
//    private readonly IServiceScopeFactory _scopeFactory;

//    public SyncService(IServiceScopeFactory scopeFactory)
//    {
//        _scopeFactory = scopeFactory;
//    }

//    // Sync data between different SQL tables or systems
//    public async Task SyncSqlDataAsync()
//    {
//        using (var scope = _scopeFactory.CreateScope())
//        {
//            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//            // Example: Fetch data from one table (e.g., Users)
//            var sqlData = await dbContext.Users.ToListAsync();

//            // Example logic: Sync between different SQL systems or data manipulation
//            foreach (var item in sqlData)
//            {
//                // Add your logic here to process or sync data between different SQL entities
//                var converter = new ApplicationUserConverter();
//                var processedData = converter.ToEntity(item);
//                // Here you would save or process the data further if needed.
//            }

//            // Save changes if any modifications are made
//            await dbContext.SaveChangesAsync();
//        }
//    }

//    // You can add additional SQL-based sync operations if needed
//}
