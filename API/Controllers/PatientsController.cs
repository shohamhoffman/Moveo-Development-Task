using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PatientsController : BaseApiController
    {
        private readonly IPatientRepository _repo;
        public PatientsController(IPatientRepository repo)
        {
            _repo = repo;

        }

        [HttpGet]
        [Route("appointments")]
        [Authorize(Roles = "patient")]
        public async Task<List<Appointment>> GetPatientAppointmentsForDoctor(string patientEmail, string doctorId)
        {
            return await _repo.GetPatientAppointmentsForDoctorAsync(patientEmail, doctorId);
        }

        [HttpPost]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
        {
            return await _repo.CreatePatientAsync(patient.Id, patient.Email, patient.Name);
        }
    }
}