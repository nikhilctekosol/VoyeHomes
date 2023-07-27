using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

    public class MySqlHelper
    {
        private string connectionString;
        private MySqlConnection MySqlConnection;
        private MySqlCommand MySqlCommand;

        private int commandTimeout = 30;

        private string server;
        private string database;
        private string user;
        private string password;
        private string port;       
        private string sslM;

        public MySqlHelper()
        {
            try
            {

                //server = "13.127.239.254";
                //database = "grocery_won";
                

                //user = "wonremote";
                //password = "roam@777@999@won@cart@999@4333@ajkglk@111@777@!";
                //port = "3306";
                //sslM = "none";
                
               connectionString= connectionString = VTravel.HostWeb.Startup.conStr;

            //connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5};Allow User Variables=True;CharSet = utf8;", server, port, user, password, database, sslM);

            MySqlConnection = new MySqlConnection(connectionString);
                MySqlCommand = new MySqlCommand();
                MySqlCommand.CommandTimeout = commandTimeout;
                MySqlCommand.Connection = MySqlConnection;
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing data class." + Environment.NewLine + ex.Message);
            }
        }

        public MySqlHelper(string conStr)
        {
            try
            {
                connectionString = conStr;
                MySqlConnection = new MySqlConnection(connectionString);
                MySqlCommand = new MySqlCommand();
                MySqlCommand.CommandTimeout = commandTimeout;
                MySqlCommand.Connection = MySqlConnection;
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing data class." + Environment.NewLine + ex.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                //Clean Up Connection Object
                if (MySqlConnection != null)
                {
                    if (MySqlConnection.State != ConnectionState.Closed)
                    {
                        MySqlConnection.Close();
                    }
                    MySqlConnection.Dispose();
                }

                //Clean Up Command Object
                if (MySqlCommand != null)
                {
                    MySqlCommand.Dispose();
                }

            }

            catch (Exception ex)
            {
                throw new Exception("Error disposing data class." + Environment.NewLine + ex.Message);
            }

        }

        public void CloseConnection()
        {
            if (MySqlConnection.State != ConnectionState.Closed) MySqlConnection.Close();
        }


        public int ExecuteCommandWithOUTParam(string Command, string outParam)
        {
            int newId = 0;
            try
            {
                MySqlCommand.CommandText = Command;
                MySqlCommand.CommandTimeout = commandTimeout;
                MySqlCommand.CommandType = CommandType.StoredProcedure;

                MySqlConnection.Open();

                MySqlCommand.Connection = MySqlConnection;
                MySqlCommand.ExecuteNonQuery();
                newId = Convert.ToInt32(MySqlCommand.Parameters[outParam].Value);
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            return newId;

        }

        public object GetExecuteScalarByCommand(string Command)
        {
            object o = new object();
            try
            {
                MySqlCommand.CommandText = Command;
                MySqlCommand.CommandTimeout = commandTimeout;
                MySqlCommand.CommandType = CommandType.StoredProcedure;

                MySqlConnection.Open();

                MySqlCommand.Connection = MySqlConnection;
                o = MySqlCommand.ExecuteScalar();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            return o;
        }

        public string GetScalerOutputValue(string ParamName)
        {
            return MySqlCommand.Parameters[ParamName].Value.ToString();
        }

        public void GetExecuteNonQueryByCommand(string Command)
        {
            try
            {
                MySqlCommand.CommandText = Command;
                MySqlCommand.CommandTimeout = commandTimeout;
                MySqlCommand.CommandType = CommandType.StoredProcedure;

                MySqlConnection.Open();

                MySqlCommand.Connection = MySqlConnection;
                MySqlCommand.ExecuteNonQuery();

                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public DataSet GetDatasetByCommand(string Command)
        {
            try
            {
                MySqlCommand.CommandText = Command;
                MySqlCommand.CommandTimeout = commandTimeout;
                MySqlCommand.CommandType = CommandType.StoredProcedure;

                MySqlConnection.Open();

                MySqlDataAdapter adpt = new MySqlDataAdapter(MySqlCommand);
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }


        public DataSet GetDatasetByCommand(string Command, CommandType commandType)
        {
            try
            {
                MySqlCommand.CommandText = Command;
                MySqlCommand.CommandTimeout = commandTimeout;
                MySqlCommand.CommandType = commandType;

                MySqlConnection.Open();

                MySqlDataAdapter adpt = new MySqlDataAdapter(MySqlCommand);
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataSet GetDatasetByMySql(string Command)
        {
            try
            {
                MySqlCommand.CommandText = Command;
                MySqlCommand.CommandTimeout = commandTimeout;

                MySqlConnection.Open();

                MySqlDataAdapter adpt = new MySqlDataAdapter(MySqlCommand);
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable GetDataTableByMySql(string Command)
        {
            try
            {
                MySqlCommand.CommandText = Command;
                MySqlCommand.CommandTimeout = commandTimeout;

                MySqlConnection.Open();

                MySqlDataAdapter adpt = new MySqlDataAdapter(MySqlCommand);
                DataTable dt = new DataTable();
                adpt.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public MySqlDataReader GetReaderByMySql(string strMySql)
        {
            MySqlConnection.Open();
            try
            {
                MySqlCommand myCommand = new MySqlCommand(strMySql, MySqlConnection);
                return myCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public MySqlDataReader GetReaderByCmd(string Command)
        {
            MySqlDataReader objMySqlDataReader = null;
            try
            {
                MySqlCommand.CommandText = Command;
                MySqlCommand.CommandType = CommandType.StoredProcedure;
                MySqlCommand.CommandTimeout = commandTimeout;

                MySqlConnection.Open();
                MySqlCommand.Connection = MySqlConnection;

                objMySqlDataReader = MySqlCommand.ExecuteReader();
                return objMySqlDataReader;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

        }

        public void AddParameterToMySqlCommand(string ParameterName, MySqlDbType ParameterType)
        {
            try
            {
                MySqlCommand.Parameters.Add(new MySqlParameter(ParameterName, ParameterType));
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddParameterToMySqlCommand(string ParameterName, MySqlDbType ParameterType, int ParameterSize)
        {
            try
            {
                MySqlCommand.Parameters.Add(new MySqlParameter(ParameterName, ParameterType, ParameterSize));
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddSetParameterToMySqlCommand(string ParameterName, MySqlDbType ParameterType, object Value)
        {
            try
            {
                MySqlCommand.Parameters.Add(new MySqlParameter(ParameterName, ParameterType));
                MySqlCommand.Parameters[ParameterName].Value = Value;
                MySqlCommand.Parameters[ParameterName].Direction = ParameterDirection.Input;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddOutputParameterToMySqlCommand(string ParameterName, MySqlDbType ParameterType)
        {
            try
            {
                MySqlCommand.Parameters.Add(new MySqlParameter(ParameterName, ParameterType));
                MySqlCommand.Parameters[ParameterName].Direction = ParameterDirection.Output;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetMySqlCommandParameterValue(string ParameterName, object Value)
        {
            try
            {
                MySqlCommand.Parameters[ParameterName].Value = Value;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetIndentityValue(int index)
        {
            return Convert.ToInt32(MySqlCommand.Parameters[index].Value.ToString());
        }

        public MySqlDataAdapter GetDataAdapterForTable(string query)
        {
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            try
            {
                MySqlCommand.CommandText = query;
                MySqlCommand.CommandTimeout = commandTimeout;

                MySqlConnection.Open();

                adpt = new MySqlDataAdapter(MySqlCommand);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return adpt;
        }
    }

