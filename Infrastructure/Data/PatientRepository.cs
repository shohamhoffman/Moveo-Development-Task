using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class PatientRepository : IPatientRepository
    {
        private readonly StoreContext _context;
        public PatientRepository(StoreContext context)
        {
            _context = context;

        }

        public async Task<List<Appointment>> GetPatientAppointmentsForDoctorAsync(string patientEmail, string doctorId)
        {
            // Gets the patient id by email.
            var patientId = await GetPatientIdByEmailAsync(patientEmail);

            // Return the appointments that have the same patient id as the patient
            // and the same doctor id as the doctor, which means-
            // Return the patient appointment for a specific doctor.
            return await _context.Appointments.Where(
                a => (a.PatientId == patientId) && (a.DoctorId == doctorId)
            ).ToListAsync();
        }
        public async Task<Patient> CreatePatientAsync(string patientId, string email, string name)
        {
            var patient = new Patient(patientId, email, name);

            _context.Set<Patient>().Add(patient);
            await _context.SaveChangesAsync();

            return patient;
        }

        private async Task<string> GetPatientIdByEmailAsync(string patientEmail)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Email == patientEmail);

            return patient.Id;
        }
    }
}