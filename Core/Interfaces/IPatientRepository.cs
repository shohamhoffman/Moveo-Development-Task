using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient> CreatePatientAsync(string patientId, string email, string name);
        Task<List<Appointment>> GetPatientAppointmentsForDoctorAsync(string patientEmail, string doctorId);
    }
}