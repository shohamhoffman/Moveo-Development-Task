export interface IAppointment {
    id: string;
    patientId: string;
    doctorId: string;
    startTime: Date;
    endTime: Date;
    isStarted: boolean;
}
