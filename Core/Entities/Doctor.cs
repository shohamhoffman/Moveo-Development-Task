namespace Core.Entities
{
    public class Doctor
    {
        public Doctor()
        {
        }

        public Doctor(string id, string email, string name, string doctorField)
        {
            Id = id;
            Email = email;
            Name = name;
            DoctorField = doctorField;
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string DoctorField { get; set; }
    }

}