using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        // Seeding the store context with doctor data data.
        public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (!context.Doctors.Any())
                {
                    var doctorsData = File.ReadAllText("../Infrastructure/Data/SeedData/Doctors.json");

                    var doctors = JsonSerializer.Deserialize<List<Doctor>>(doctorsData);

                    foreach (var doc in doctors)
                    {
                        context.Doctors.Add(doc);
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}