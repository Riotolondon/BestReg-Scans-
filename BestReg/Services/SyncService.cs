using BestReg.Converters;
using BestReg.Data;
using BestReg.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public class SyncService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFirebaseService _firebaseService;

    public SyncService(IServiceScopeFactory scopeFactory, IFirebaseService firebaseService)
    {
        _scopeFactory = scopeFactory;
        _firebaseService = firebaseService;
    }

    // Sync data from SQL to Firebase
    public async Task SyncSqlToFirebaseAsync()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Fetch data from Azure SQL
            var sqlData = await dbContext.Users.ToListAsync();

            // Sync data to Firebase
            foreach (var item in sqlData)
            {
                var converter = new ApplicationUserConverter();
                var documentData = converter.ToFirestore(item);
                await _firebaseService.AddDocumentAsync(documentData);
            }
        }
    }

    // Sync data from Firebase to SQL
    public async Task SyncFirebaseToSqlAsync()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Fetch data from Firebase
            var firebaseData = await _firebaseService.GetAllDocumentsAsync<ApplicationUser>();

            // Sync data to Azure SQL
            foreach (var item in firebaseData)
            {
                var existingEntity = await dbContext.Users.FindAsync(item.Id);
                if (existingEntity == null)
                {
                    dbContext.Users.Add(item);
                }
                else
                {
                    dbContext.Entry(existingEntity).CurrentValues.SetValues(item);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
