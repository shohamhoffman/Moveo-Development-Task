using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAppointmentsAsync();
        Task<Appointment> CreateAppointmentAsync(
            string appointmentId,
            string patientId,
            string doctorId,
            DateTime startTime,
            DateTime endTime
        );
        Task<Appointment> CheckIfAppointmentArrivedAsync(string id);
        Task DeleteAppointmentAsync(string appointmentId);
    }
}