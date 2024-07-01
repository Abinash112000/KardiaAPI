namespace KardiaAPI.Model
{
    public class TeamPatientModel
    {
        public List<team> teams { get; set; }
    }
    public class team
    {
        public string id { get; set; }
        public string teamId { get; set; }
        public string fieldName { get; set; }
        public string displayName { get; set; }
        public bool isRequired { get; set; }
        public bool isVisible { get; set; }
        public bool isCustom { get; set; }
        public bool isKeyIdentifier { get; set; }
        public bool kardiaRXVisible { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
    }
}
