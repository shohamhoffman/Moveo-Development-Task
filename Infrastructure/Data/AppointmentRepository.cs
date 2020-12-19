using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly StoreContext _context;
        private readonly IWorker _worker;
        public AppointmentRepository(StoreContext context, IWorker worker)
        {
            _context = context;
            _worker = worker;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments.ToListAsync();
        }

        public async Task<Appointment> CreateAppointmentAsync(
            string appointmentId, string patientId, string doctorId, DateTime startTime, DateTime endTime)
        {
            // Gets the patient id by his email.
            var patientIdByEmail = await _context.Patients.FirstOrDefaultAsync(p => p.Email == patientId);
            var appointment = new Appointment(
                appointmentId, patientIdByEmail.Id, doctorId,
                TimeZoneInfo.ConvertTimeBySystemTimeZoneId(startTime, "GMT Standard Time"),
                TimeZoneInfo.ConvertTimeBySystemTimeZoneId(endTime, "GMT Standard Time")
            );

            _context.Set<Appointment>().Add(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<Appointment> CheckIfAppointmentArrivedAsync(string email)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(d => d.Email == email);
            // Gets the patient appointments.
            List<Appointment> appointments = await _context.Appointments.Where(a => a.PatientId == patient.Id).ToListAsync();

            // Checks for each appointment, if the start time is equal to now.
            // If so, it means that the appointment has begun and it will return the appointment to the client.
            foreach (Appointment appointment in appointments)
            {
                var appointmentExcludingSeconds = new DateTime(
                    appointment.StartTime.Year,
                    appointment.StartTime.Month,
                    appointment.StartTime.Day,
                    appointment.StartTime.Hour,
                    appointment.StartTime.Minute,
                    0,
                    0
                );
                var nowExcludingSeconds = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    DateTime.Now.Hour,
                    DateTime.Now.Minute,
                    0,
                    0
                );

                if (!appointment.IsStarted
                    && DateTime.Compare(appointmentExcludingSeconds, nowExcludingSeconds) == 0)
                {
                    await changeAppointmentAvailability(appointment);
                    return appointment;
                }
            }

            // If no appointment starts now, return null.
            return null;
        }

        public async Task DeleteAppointmentAsync(string appointmentId)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
            // If the appointment is not found, return.
            // If the patient cancled his appointment for example.
            if (appointment == null) return;

            // If the appointment has already begun and didn't finish, return.
            if (appointment.IsStarted && appointment.EndTime >= DateTime.Now) return;

            _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();
        }

        private async Task changeAppointmentAvailability(Appointment appointment)
        {
            appointment.IsStarted = true;
            await _context.SaveChangesAsync();
        }
    }
}