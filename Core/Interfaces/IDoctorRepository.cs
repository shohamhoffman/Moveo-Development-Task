using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IReadOnlyList<Doctor>> GetDoctorsAsync(string filter);
        Task<Doctor> CreateDoctorAsync(string doctorId, string doctorEmail, string doctorName, string doctorField);
        Task<List<Appointment>> GetDoctorAppointmentsAsync(string id);
        Task<bool> IsAvailableAsync(string id);
        Task<List<AppointmentWithPatientName>> GetDoctorAppointmentsByDoctorEmailAsync(string email);
        Task<bool> IsDoctorAvailableAtAsync(string id, DateTime appointmentStartTime, DateTime appointmentEndTime);
    }
}