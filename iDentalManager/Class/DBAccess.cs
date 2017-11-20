using System.Data;
using System.Data.SqlClient;

namespace iDentalManager.Class
{
    public class DBAccess
    {
        private static string ConnectionString;

        private static string serverIP;
        public static string ServerIP
        {
            get { return serverIP; }
            private set
            {
                serverIP = value;
                ConnectionString = @"server=" + serverIP + @"\IDENTAL;database=iDental;uid=sa;pwd=0939566880;Connection Timeout=1;";
            }
        }

        /// <summary>
        /// SQL Connection 設定
        /// </summary>
        private static SqlConnection sqlConnection;
        public static bool CheckConnection(string serverIP)
        {
            try
            {
                ServerIP = serverIP;
                sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();
                sqlConnection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Execute sqlstring command by inline sql
        /// </summary>
        /// <param name="Sqlcmd"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(string Sqlcmd, IDataParameter[] parameters)
        {
            DataSet dataSet = new DataSet();
            sqlConnection.Open();
            SqlDataAdapter sqlDA = new SqlDataAdapter();
            sqlDA.SelectCommand = ExecuteQueryCommand(Sqlcmd, parameters);
            sqlDA.Fill(dataSet);
            sqlConnection.Close();

            return dataSet;
        }
        public static SqlCommand ExecuteQueryCommand(string Sqlcmd, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(Sqlcmd, sqlConnection);
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Execute sqlstring command by inline sql
        /// </summary>
        /// <param name="Sqlcmd"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static void ExecuteNonQuery(string Sqlcmd, IDataParameter[] parameters)
        {
            try
            {
                ExecuteNonQueryCommand(Sqlcmd, parameters);
            }
            catch
            {
                sqlConnection.Close();
            }
        }
        public static int ExecuteNonQueryCommand(string Sqlcmd, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(Sqlcmd, sqlConnection);
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command.ExecuteNonQuery();
        }
    }
}
