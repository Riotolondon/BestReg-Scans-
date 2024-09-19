using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestReg.Services
{
    public class RoleSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly FirestoreDb _firestoreDb;

        public RoleSyncService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _firestoreDb = FirestoreDb.Create("newchilddb");
        }

        public async Task SyncRolesToFirestoreAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = await roleManager.Roles.ToListAsync();
                var rolesCollection = _firestoreDb.Collection("roles");

                foreach (var role in roles)
                {
                    var roleDocument = rolesCollection.Document(role.Name);
                    var roleData = new Dictionary<string, object>
                    {
                        { "Name", role.Name }
                    };

                    await roleDocument.SetAsync(roleData);
                }
            }
        }
    }
}
