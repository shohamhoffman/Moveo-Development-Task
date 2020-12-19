export interface IAppointmentWithPatientName {
    id: string;
    patientId: string;
    doctorId: string;
    startTime: Date;
    endTime: Date;
    patientName: string;
}
