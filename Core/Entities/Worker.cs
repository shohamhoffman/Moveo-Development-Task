using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Entities
{
    // Responsible of deleting an appointment 3 min after it creation.
    public class Worker : IWorker
    {
        private readonly IServiceProvider _serviceProvider;
        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10000);
                // Using service provider because the worker is a singelton and the repo is scoped.
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
                    var appointments = await repo.GetAppointmentsAsync();
                    foreach (var appointment in appointments)
                    {
                        if (appointment.EndTime <= DateTime.Now)
                        {
                            await repo.DeleteAppointmentAsync(appointment.Id);
                        }
                    }
                };
            }
        }
    }
}