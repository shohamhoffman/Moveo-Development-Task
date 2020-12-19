using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class AppointmentsController : BaseApiController
    {
        private readonly IAppointmentRepository _repo;
        public AppointmentsController(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<Appointment>> CheckIfAppointmentArrived(string email)
        {
            return Ok(await _repo.CheckIfAppointmentArrivedAsync(email));
        }

        [HttpPost]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            return Ok(await _repo.CreateAppointmentAsync(
                appointment.Id,
                appointment.PatientId,
                appointment.DoctorId,
                appointment.StartTime,
                appointment.EndTime
            ));
        }

        [HttpDelete]
        [Authorize(Roles = "patient")]
        public async Task DeleteAppointment(string appointmentId)
        {
            await _repo.DeleteAppointmentAsync(appointmentId);
        }
    }
}