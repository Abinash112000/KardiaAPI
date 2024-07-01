namespace KardiaAPI.Model
{
    public class RecordingsModel
    {
        public int totalCount { get; set; }

        public List<recordings> recordings { get; set; }

        public PageInfo pageInfo { get; set; }
    }
    public class recordings
    {
        public string id { get; set; }

        public string patientID { get; set; }

        public string algorithmDetermination { get; set; }

        public int duration { get; set; }

        public int heartRate { get; set; }

        public string note { get; set; }
        public string recordedAt { get; set; }

        public List<string> tags { get; set; }
    }
}
