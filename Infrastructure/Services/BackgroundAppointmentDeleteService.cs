using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services
{
    public class BackgroundAppointmentDeleteService : BackgroundService
    {
        private readonly IWorker _worker;
        private readonly IAppointmentRepository _repo;
        public BackgroundAppointmentDeleteService(IWorker worker, IServiceProvider serviceProvider)
        {
            _worker = worker;
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                _repo = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _worker.DoWork(stoppingToken);
        }
    }
}