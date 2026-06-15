namespace KardiaAPI.Model
{
    public class PatientRecordingsModel
    {
        public string totalCount { get; set; }
        public List<recordingsList> recordings { get; set; }
        public PageInfo pageInfo { get; set; }
    }
    public class recordingsList
    {
        public string id { get; set; }
        public string patientID { get; set; }
        public int duration { get; set; }
        public int heartRate { get; set; }
        public string note { get; set; }
        public string recordedAt { get; set; }
        public List<string> tags { get; set; }
    }
}
