namespace KardiaAPI.Model
{
    public class ScreeningStationRecordingsModel
    {
        public int totalCount { get; set; }
        public List<recording> recordings { get; set; }
        public PageInfo pageInfo { get; set; }
    }
    public class recording
    {
        public string id { get; set; }

        public string patientID { get; set; }

        public string algorithmDetermination { get; set; }

        public int duration { get; set; }

        public int heartRate { get; set; }

        public string note { get; set; }

        public List<string> tags { get; set; }

        public string recordedAt { get; set; }
    }
}
