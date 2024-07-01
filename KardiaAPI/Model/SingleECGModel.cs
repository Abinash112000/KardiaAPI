namespace KardiaAPI.Model
{
    public class SingleECGModel
    {
        public string id { get; set; }

        public string patientID { get; set; }

        public int heartRate { get; set; }

        public int duration { get; set; }

        public string recordedAt { get; set; }

        public string algorithmDetermination { get; set; }

        public data data { get; set; }

        public deviceInfo deviceInfo { get; set; }
    }
    public class deviceInfo
    {
        public string hardwareType { get; set; }

        public string hardwareRevision { get; set; }

        public string firmwareRevision { get; set; }

        public string serialNumber { get; set; }
    }
    public class data
    {
        public raw raw { get; set; }

        public enhanced enhanced { get; set; }

    }

    public class raw
    {
        public int frequency { get; set; }

        public int mainsFrequency { get; set; }

        public samples samples { get; set; }

        public int amplitudeResolution { get; set; }

        public int numLeads { get; set; }
    }

    public class enhanced
    {
        public int frequency { get; set; }

        public int mainsFrequency { get; set; }

        public samples samples { get; set; }

        public int amplitudeResolution { get; set; }

        public int numLeads { get; set; }

    }

    public class samples
    {
        public List<int> leadI { get; set; }

        public List<int> AVR { get; set; }
    }
}
