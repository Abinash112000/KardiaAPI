using KardiaApi.Logic.Model;

namespace KardiaAPI.Model
{
    public class PatientDetailsModel
    {
        public int? totalCount { get; set; }
        public List<Patients> patients { get; set; }
        public PageInfo pageInfo { get; set; }
    }
    //public class Patients
    //{
    //    public string? id { get; set; }
    //    public string? mrn { get; set; }
    //    public string? dob { get; set; }
    //    public string? email { get; set; }
    //    public string? firstname { get; set; }
    //    public string? lastname { get; set; }
    //    public int? sex { get; set; }
    //    public string? phone { get; set; }
    //    public string? connectionTemplateId { get; set; }
    //}
    public class PageInfo
    {
        public string? startCursor { get; set; }
        public string? endCursor { get; set; }
        public bool? hasNextPage { get; set; }
    }
}
