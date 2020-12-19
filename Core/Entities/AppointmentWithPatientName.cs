using System;

namespace Core.Entities
{
    public class AppointmentWithPatientName
    {
        public AppointmentWithPatientName(
            string id, string patientId, string doctorId, DateTime startTime, DateTime endTime,
            string patientName
        )
        {
            Id = id;
            PatientId = patientId;
            DoctorId = doctorId;
            StartTime = startTime;
            EndTime = endTime;
            PatientName = patientName;
        }

        public string Id { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PatientName { get; set; }
    }
}