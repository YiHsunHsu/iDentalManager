using iDentalManager.Class;
using System.Data;
using System.Data.SqlClient;

namespace iDentalManager.DataTables
{
    public class DTAgencys
    {
        private string sqlcmd;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agencyCode"></param>
        /// <returns></returns>
        public DataTable QueryAgency(string agencyCode)
        {
            sqlcmd = @"SELECT * FROM Agencys WHERE Agency_Code = @Agency_Code";
            SqlParameter[] parameters = {
                                        new SqlParameter("@Agency_Code", SqlDbType.VarChar)
                                        };
            parameters[0].Value = agencyCode;
            DataTable dt = DBAccess.ExecuteQuery(sqlcmd, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="agencyCode"></param>
        /// <returns></returns>
        public DataTable UpdateAgencyStatus(string Status, string agencyCode)
        {
            switch (Status)
            {
                case "RUN"://啟用
                    sqlcmd = @"UPDATE Agencys 
                                SET Agency_IsVerify = 1,
                                    Agency_TrialPeriod = NULL,
                                    Agency_IsTry = 0,
                                    UpdateTime = DEFAULT
                                WHERE Agency_Code = @Agency_Code";
                    break;
                case "STOP"://停用
                    sqlcmd = @"UPDATE Agencys 
                                SET Agency_IsVerify = 0,
                                    Agency_TrialPeriod = NULL,
                                    Agency_IsTry = 0,
                                    UpdateTime = DEFAULT
                                WHERE Agency_Code = @Agency_Code";
                    break;
                case "TRY"://試用
                    sqlcmd = @"UPDATE Agencys 
                                SET Agency_IsVerify = 1,
                                    Agency_TrialPeriod = DATEADD(MONTH,1, GETDATE()),
                                    Agency_IsTry = 1,
                                    UpdateTime = DEFAULT
                                WHERE Agency_Code = @Agency_Code";
                    break;
            }
            sqlcmd += @" SELECT * FROM Agencys WHERE Agency_Code = @Agency_Code";

            SqlParameter[] parameters = {
                                        new SqlParameter("@Agency_Code", SqlDbType.VarChar)
                                        };
            parameters[0].Value = agencyCode;
            DataTable dt = DBAccess.ExecuteQuery(sqlcmd, parameters).Tables[0];
            return dt;
        }

        /// <summary>
        /// 建立註冊碼
        /// </summary>
        /// <returns></returns>
        public DataTable InsertAgency(string agencyCode, string serverIP, string verificationCode)
        {
            sqlcmd = @"INSERT INTO Agencys(Agency_Code, Agency_ServerIP, Agency_VerificationCode, Agency_IsVerify)
                        VALUES(@Agency_Code, @Agency_ServerIP, @Agency_VerificationCode, '1')
                            SELECT * FROM Agencys WHERE Agency_Code = @Agency_Code";
            SqlParameter[] parameters = {
                                        new SqlParameter("@Agency_Code", SqlDbType.VarChar),
                                        new SqlParameter("@Agency_ServerIP", SqlDbType.VarChar),
                                        new SqlParameter("@Agency_VerificationCode", SqlDbType.VarChar)
                                        };
            parameters[0].Value = agencyCode;
            parameters[1].Value = serverIP;
            parameters[2].Value = verificationCode;
            DataTable dt = DBAccess.ExecuteQuery(sqlcmd, parameters).Tables[0];
            return dt;
        }
    }
}
