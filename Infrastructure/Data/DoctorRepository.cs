using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly StoreContext _context;
        private readonly AppIdentityDbContext _idContext;
        public DoctorRepository(StoreContext context, AppIdentityDbContext idContext)
        {
            _idContext = idContext;
            _context = context;
        }

        public async Task<Doctor> CreateDoctorAsync(string doctorId, string doctorEmail, string doctorName, string doctorField)
        {
            var doctor = new Doctor(doctorId, doctorEmail, doctorName, doctorField);

            _context.Set<Doctor>().Add(doctor);
            await _context.SaveChangesAsync();

            return doctor;
        }

        public async Task<IReadOnlyList<Doctor>> GetDoctorsAsync(string filter = "false")
        {
            // Validate that the filter is ONLY true / false.
            // If not, assign it false.
            if (filter != "true" && filter != "false")
                filter = "false";

            var doctors = await _context.Doctors.ToListAsync();

            // If the filter is true (return only available doctors),
            // check for each doctor if he is available or not.
            // If the doctor is not available, remove it from the doctors list.
            if (filter == "true")
            {
                for (int i = 0; i < doctors.Count(); i++)
                {
                    if (!await IsAvailableAsync(doctors.ElementAt(i).Id))
                    {
                        doctors.Remove(doctors.ElementAt(i));
                    }
                }
            }

            return doctors;
        }

        public async Task<bool> IsAvailableAsync(string id)
        {
            var available = true;
            var waitingList = await GetDoctorAppointmentsAsync(id);

            // For each appointment of the doctor, check if there is appointment right now.
            // If so, then the doctor is not available.
            foreach (var appointment in waitingList)
            {
                if (appointment.IsStarted)
                {
                    available = false;
                }
            }

            return available;
        }

        public async Task<List<Appointment>> GetDoctorAppointmentsAsync(string id)
        {
            // Gets the doctors' appointments ordered by start time.
            var doctorSortedWaitingList = await _context.Appointments
                .Where(d => d.DoctorId == id).OrderBy(d => d.StartTime).ToListAsync();

            return doctorSortedWaitingList;
        }

        public async Task<bool> IsDoctorAvailableAtAsync(string id, DateTime appointmentStartTime, DateTime appointmentEndTime)
        {
            // Check if the doctor available at certain time.
            // If the start time or the end time is between another appointments' time, the doctor is not available.
            var doctorAppointments = await GetDoctorAppointmentsAsync(id);
            foreach (var appointment in doctorAppointments)
            {
                if (appointmentStartTime >= appointment.StartTime && appointmentStartTime < appointment.EndTime
                    || appointmentEndTime > appointment.StartTime && appointmentEndTime <= appointment.EndTime)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<List<AppointmentWithPatientName>> GetDoctorAppointmentsByDoctorEmailAsync(string email)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Email == email);

            var appointments = await GetDoctorAppointmentsAsync(doctor.Id);

            var appointmentsWithPatientName = new List<AppointmentWithPatientName>();

            // Creating a appointments that also contain the patinet name.
            foreach (var appointment in appointments)
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == appointment.PatientId);
                appointmentsWithPatientName.Add(
                    new AppointmentWithPatientName(
                        appointment.Id,
                        appointment.PatientId,
                        appointment.DoctorId,
                        appointment.StartTime,
                        appointment.EndTime,
                        patient.Name
                    )
                );
            }

            return appointmentsWithPatientName;
        }
    }
}