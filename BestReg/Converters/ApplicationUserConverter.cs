using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using BestReg.Data;  // Assuming ApplicationUser is in the Data namespace

namespace BestReg.Converters  // Adjust the namespace based on your project structure
{
    public class ApplicationUserConverter : IFirestoreConverter<ApplicationUser>
    {
        public ApplicationUser FromFirestore(object value)
        {
            var dict = value as Dictionary<string, object>;
            if (dict == null) throw new ArgumentException("Invalid ApplicationUser data");

            return new ApplicationUser
            {
                Id = dict["Id"] as string,
                FirstName = dict["FirstName"] as string,
                LastName = dict["LastName"] as string,
                Email = dict["Email"] as string,
                IDNumber = dict["IDNumber"] as string,
                QrCodeBase64 = dict.ContainsKey("QrCodeBase64") ? dict["QrCodeBase64"] as string : null
            };
        }

        public object ToFirestore(ApplicationUser value)
        {
            return new Dictionary<string, object>
            {
                { "Id", value.Id },
                { "FirstName", value.FirstName },
                { "LastName", value.LastName },
                { "Email", value.Email },
                { "IDNumber", value.IDNumber },
                { "QrCodeBase64", value.QrCodeBase64 }
            };
        }
    }
}