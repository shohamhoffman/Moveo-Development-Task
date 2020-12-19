using System;

namespace Core.Entities
{
    public class Appointment
    {
        public Appointment()
        {
        }

        public Appointment(string id, string patientId, string doctorId, DateTime startTime, DateTime endTime)
        {
            Id = id;
            PatientId = patientId;
            DoctorId = doctorId;
            StartTime = startTime;
            EndTime = endTime;
            IsStarted = false;
        }

        public string Id { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsStarted { get; set; }
    }
}