using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using System;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    public class DoctorsController : BaseApiController
    {
        private readonly IDoctorRepository _repo;
        public DoctorsController(IDoctorRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<List<Doctor>>> GetDoctors(string filter)
        {
            // Filter means - only available doctors or all the doctors.
            var doctors = await _repo.GetDoctorsAsync(filter);

            return Ok(doctors);
        }

        [HttpGet]
        [Route("appointments")]
        [Authorize]
        public async Task<List<Appointment>> GetDoctorAppointments(string id)
        {
            return await _repo.GetDoctorAppointmentsAsync(id);
        }

        [HttpGet]
        [Route("available")]
        [Authorize(Roles = "patient")]
        public async Task<bool> IsDoctorAvailable(string id)
        {
            return await _repo.IsAvailableAsync(id);
        }

        [HttpGet]
        [Route("availableat")]
        [Authorize(Roles = "patient")]
        public async Task<bool> IsDoctorAvailableAt(string id, DateTime appointmentStartTime, DateTime appointmentEndTime)
        {
            return await _repo.IsDoctorAvailableAtAsync(id, appointmentStartTime, appointmentEndTime);
        }

        [HttpGet]
        [Route("appointments/email")]
        [Authorize(Roles = "doctor")]
        public async Task<List<AppointmentWithPatientName>> GetDoctorAppointmentsByDoctorEmail(string email)
        {
            return await _repo.GetDoctorAppointmentsByDoctorEmailAsync(email);
        }

        [HttpPost]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<Doctor>> CreateDoctor(Doctor doctor)
        {
            return Ok(await _repo.CreateDoctorAsync(doctor.Id, doctor.Email, doctor.Name, doctor.DoctorField));
        }
    }
}