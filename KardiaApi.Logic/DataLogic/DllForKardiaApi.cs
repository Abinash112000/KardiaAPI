using System;
using System.Data;
using System.Data.SqlClient;

namespace KardiaApi.Logic.DataLogic
{
    public class DllForKardiaApi
    {
        private readonly string _connectionString;

        public DllForKardiaApi(string appConnectionString)
        {
            _connectionString = appConnectionString;
        }

        public DataTable GetUrl(string env, int protocolId)
        {
            string siteStage = string.Empty;
            if (env == "0")
            {
                siteStage = "Dev";
            }
            else if (env == "1")
            {
                siteStage = "Test";
            }
            else
            {
                siteStage = "Prod";
            }
            const string sqlQuery = @"
                SELECT URL + '/' + (
                    SELECT PROTOCOL_NAME
                    FROM V_CUSTOMER_SPONSOR_PROTOCOL
                    WHERE PR_UNIQUE_ID = @ProtocolId
                ) AS URL
                FROM TBL_Protocol_Site
                WHERE Protocol_ID = @ProtocolId AND Site_Stage = @SiteStage";

            return ExecuteQuery(
                sqlQuery,
                new SqlParameter("@ProtocolId", SqlDbType.Int) { Value = protocolId },
                new SqlParameter("@SiteStage", SqlDbType.VarChar, 10) { Value = siteStage });
        }

        //public int InsertPatientData(Patients patientsData)
        //{
        //    bool isSuccesfull = true;
        //    ObjDataAccessInfo objReturn = new ObjDataAccessInfo();

        //    string mrn = patientsData.mrn;
        //    int sex = patientsData.sex;
        //    string Gender = "";
        //    if(sex == 1)
        //    {
        //        Gender = "MALE";
        //    }
        //    else
        //    {
        //        Gender = "FEMALE";
        //    }
        //    char separator = '-';
        //    string[] authorsList = mrn.Split(separator);
        //    string siteId = authorsList[1];
        //    string PatientIntials = authorsList[3];
        //    string SubjId = authorsList[2];
        //    string ProtocolId = authorsList[0];
        //    //DataTable subjDetails = GetSubjID(siteId, PatientId, PatientIntials);
        //    DataTable targetLevelIdDetails = GetTargetLevelId(ProtocolId);
        //    //int usubjId = 0;
        //    int targetLvlId = 0;

        //    if (targetLevelIdDetails.Rows.Count > 0)
        //    {
        //        targetLvlId = Convert.ToInt32(targetLevelIdDetails.Rows[0]["TARGET_LEVEL_ID"]);
        //        //InsertData(targetLvlId, ProtocolId, Gender);
        //    }
        //    else
        //    {
        //        string sqlQuery = $@"INSERT INTO DM(MODULE_ID,EX_EXPORT,ROW_NO,TARGET_LEVEL,ECL_RELEASE_ID,ECL_PAGE_ID,SEQUENCE_NUMBER,TARGET_LEVEL_ID,PROTOCOL_ID,SEX,FIRST_ENTRY_DATETIME) 
        //            VALUES (1,0,0,3,-1,1,0,{SubjId},{ProtocolId},'{Gender}','{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss").ToUpper()}') ";
        //        objReturn = objDataAccess.ExecuteNonQuery(sqlQuery);
        //    }
        //    if (!objReturn.IsSuccessful)
        //        isSuccesfull = false;
        //    return targetLvlId;
        //}
        //public DataTable GetSubjID(string siteId , string patientId, string patientIntials)
        //{
        //    DataTable dtTable = new DataTable();
        //    ObjDataAccessInfo objReturn = new ObjDataAccessInfo();
        //    string SqlQuery = $@"SELECT USUBJID FROM _ECL_M_PATIENT WHERE ECL_SITE_ID ={siteId} AND PATIENT_ID={patientId} AND PATIENT_INITIALS ='{patientIntials}'";
        //    objReturn = objDataAccess.GetDataTableForSqlCommand(SqlQuery);
        //    dtTable = objReturn.Output;
        //    return dtTable;
        //}
        //public DataTable GetTargetLevelId(string protocolId)
        //{
        //    DataTable dtTable = new DataTable();
        //    ObjDataAccessInfo objReturn = new ObjDataAccessInfo();
        //    string SqlQuery = $@"SELECT TARGET_LEVEL_ID FROM DM WHERE PROTOCOL_ID = {protocolId} ";
        //    objReturn = objDataAccess.GetDataTableForSqlCommand(SqlQuery);
        //    dtTable = objReturn.Output;
        //    return dtTable;
        //}

        //public int InsertData(int usubjId, string protocolId,string Gender)
        //{
        //    ObjDataAccessInfo objReturn = new ObjDataAccessInfo();

        //    int targetLvlId = 0;
        //    if (targetLevelIdDetails.Rows.Count > 0)
        //    {
        //        targetLvlId = Convert.ToInt32(targetLevelIdDetails.Rows[0]["TARGET_LEVEL_ID"]);
        //    }
        //    else
        //    {
        //        string sqlQuery = $@"INSERT INTO DM(MODULE_ID,EX_EXPORT,ROW_NO,TARGET_LEVEL,ECL_RELEASE_ID,ECL_PAGE_ID,SEQUENCE_NUMBER,TARGET_LEVEL_ID,PROTOCOL_ID,SEX,FIRST_ENTRY_DATETIME) 
        //            VALUES (1,0,0,3,-1,1,0,{usubjId},{protocolId},'{Gender}','{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss").ToUpper()}') ";
        //        objReturn = objDataAccess.ExecuteNonQuery(sqlQuery);
        //    }
        //    return targetLvlId;
        //}

        public DataTable GetPortalRulesData()
        {
            return ExecuteQuery("SELECT * FROM _ECL_S_SETUP_OPTIONS");
        }

        public bool CheckGeneralAndKardiaData()
        {
            bool isPresent = false;

            return isPresent;
        }

        private DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            var table = new DataTable();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                command.Parameters.AddRange(parameters);
                adapter.Fill(table);
            }

            return table;
        }
    }
}
