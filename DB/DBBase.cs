using System.Data;
using System.Data.SqlClient;

namespace BildungsBericht.DB
{
    public class DBBase
    {
        int MaxDBRetry = 3;

        string connectionString = string.Empty;
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        SqlConnection connection = null;
        public SqlConnection Connection
        {
            get { return connection; }
        }

        public DBBase()
        {
        }

        public DBBase( string iConnectionString )
        {
            ConnectionString = iConnectionString;
        }

        public SqlTransaction trans = null;


        /// <summary>
        /// Open DB Connection
        /// </summary>
        public void ConnectionOpen()
        {
            connection = new SqlConnection( ConnectionString );
            connection.Open();
        }

        /// <summary>
        /// Close DB Connection
        /// </summary>
        public void ConnectionClose()
        {
            if( Connection != null )
            {
                Connection.Close();
                connection = null;
            }
        }

        /// <summary>
        /// Get an new transaction
        /// </summary>
        /// <returns></returns>
        private void BeginTransactionInternal()
        {
            try
            {
                //Trace.HMI.Log( TL.L8, TT.I, TS.S, "BeginTransation" );
                trans = Connection.BeginTransaction();
            }
            catch( Exception ex )
            {
                //Trace.HMI.Log( TL.L0, TT.E, TS.S, "{0}", ex.Message );
                if( Connection.State == ConnectionState.Closed ) throw new Exception( "7777_01" );
                throw ex;
            }
        }

        public void BeginTransaction()
        {
            int NbIfRetry = 0;

            while( true )
            {
                try
                {
                    BeginTransactionInternal();
                    break;
                }
                catch( Exception ex )
                {
                    if( ex.Message == "7777_01" )
                    {
                        //Trace.HMI.Log( TL.L0, TT.E, TS.S, " ####### ERROR: DB Disconnected. " );
                        NbIfRetry++;
                        if( NbIfRetry <= MaxDBRetry )
                        {
                            while( NbIfRetry <= MaxDBRetry )
                            {
                                //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######  --> Try Reconnection {0}/{1}", NbIfRetry, MaxDBRetry );
                                try { ConnectionOpen(); }
                                catch( Exception ex1 )
                                {
                                    //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######          Reconnection {0}/{1} FAILED: {2}", NbIfRetry, MaxDBRetry, ex1.Message );
                                    NbIfRetry++;
                                    continue;
                                }
                                //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######          Reconnection {0}/{1} SUCCESSFULL", NbIfRetry, MaxDBRetry );
                                break;
                            }
                            if( NbIfRetry <= MaxDBRetry ) continue;
                            else break;
                        }
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Commit the transaction
        /// </summary>
        public void Commit()
        {
            try
            {
                trans.Commit();
                //Trace.HMI.Log( TL.L8, TT.I, TS.S, "Commit" );
            }
            catch( Exception ex )
            {
                //Here No reconnection when DB-Connection closed
                //The user become an Error Msg
                //When he refreshed the dialog (GetDataTable) or retry the action (BeginTransaction), then the DB will be reconnected
                //Trace.HMI.Log( TL.L0, TT.E, TS.S, "{0}", ex.Message );
                throw ex;
            }
            finally
            {
                trans = null;
            }
        }

        /// <summary>
        /// Rollback the transaction
        /// </summary>
        public void Rollback()
        {
            try
            {
                trans.Rollback();
                //Trace.HMI.Log( TL.L0, TT.I, TS.S, "Rollback" );
            }
            catch( Exception ex )
            {
                //Here No reconnection when DB-Connection closed
                //The user become an Error Msg
                //When he refreshed the dialog (GetDataTable) or retry the action (BeginTransaction), then the DB will be reconnected
                //Trace.HMI.Log( TL.L0, TT.E, TS.S, "{0}", ex.Message );
                throw ex;
            }
            finally
            {
                trans = null;
            }
        }

        private DataTable GetDataTableInternal( string sqlCmd )
        {
            DataTable dt = null;

            SqlDataReader reader = null;
            try
            {
                SqlCommand cmd = new SqlCommand( sqlCmd, Connection );

                if( trans != null ) cmd.Transaction = trans;

                reader = cmd.ExecuteReader();

                dt = new DataTable();
                dt.Load( reader );

                foreach( DataColumn col in dt.Columns )
                {
                    col.ReadOnly = false;
                }
            }
            catch( Exception ex )
            {
                //Trace.HMI.Log( TL.L0, TT.E, TS.S, "ERROR: {0} on Query {1}", ex.Message, sqlCmd );
                dt = null;
                if( Connection.State == ConnectionState.Closed ) throw new Exception( "7777_01" );
            }
            finally
            {
                if( reader != null ) reader.Close();
            }
            return dt;
        }

        /// <summary>
        /// returns a DataTable containing rows from specified Sql command
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        public DataTable GetDataTable( string sqlCmd )
        {
            DataTable dt = null;
            int NbIfRetry = 0;

            while( true )
            {
                try
                {
                    dt = GetDataTableInternal( sqlCmd );
                    break;
                }
                catch( Exception ex )
                {
                    if( ex.Message == "7777_01" )
                    {
                        //Trace.HMI.Log( TL.L0, TT.E, TS.S, " ####### ERROR: DB Disconnected. " );
                        NbIfRetry++;
                        if( NbIfRetry <= MaxDBRetry )
                        {
                            while( NbIfRetry <= MaxDBRetry )
                            {
                                //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######  --> Try Reconnection {0}/{1}", NbIfRetry, MaxDBRetry );
                                try { ConnectionOpen(); }
                                catch( Exception ex1 )
                                {
                                    //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######          Reconnection {0}/{1} FAILED: {2}", NbIfRetry, MaxDBRetry, ex1.Message );
                                    NbIfRetry++;
                                    continue;
                                }
                                //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######          Reconnection {0}/{1} SUCCESSFULL", NbIfRetry, MaxDBRetry );
                                break;
                            }
                            if( NbIfRetry <= MaxDBRetry ) continue;
                            else break;
                        }
                    }
                    break;
                }
            }
            return dt;
        }

        /// <summary>
        /// Execute a SQL command --> Automaticaly committed
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        public int ExecuteSql( string sqlCmd )
        {
            return ExecuteSql( sqlCmd, true, null );
        }

        /// <summary>
        /// Execute a SQL command
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        public int ExecuteSql( string sqlCmd, bool autoCommit )
        {
            return ExecuteSql( sqlCmd, autoCommit, null );
        }

        /// <summary>
        /// Execute a SQL command --> Automaticaly committed
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteSql( string sqlCmd, SqlParameter param )
        {
            SqlParameter[] paramarray = new SqlParameter[1];
            paramarray[0] = param;

            return ExecuteSql( sqlCmd, true, paramarray );
        }

        ///// <summary>
        ///// Execute a SQL command
        ///// </summary>
        ///// <param name="sqlCmd"></param>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public int ExecuteSql( string sqlCmd, bool autoCommit, SqlParameter param )
        //{
        //    SqlParameter[] paramarray = new SqlParameter[ 1 ];
        //    paramarray[ 0 ] = param;

        //    return ExecuteSql( sqlCmd, autoCommit, paramarray );
        //}

        /// <summary>
        /// Execute a SQL command --> Automaticaly committed
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteSql( string sqlCmd, SqlParameter[] paramarray )
        {
            return ExecuteSql( sqlCmd, true, paramarray );
        }

        /// <summary>
        /// Execute a Transactional SQL command
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="autoCommit"></param>
        /// <param name="trans"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteSql( string sqlCmd, bool autoCommit, SqlParameter[] paramArray )
        {
            int rowsUpdated = 0;
            try
            {
                SqlCommand cmd = new SqlCommand( sqlCmd, Connection );

                if( autoCommit ) BeginTransaction();

                //Trace.HMI.Log( TL.L7, TT.I, TS.S, "{0}", sqlCmd );

                cmd.Transaction = trans;

                if( paramArray != null )
                {
                    if( paramArray.Length > 0 )
                    {
                        for( int i = 0;i <= paramArray.Length - 1;i++ )
                            cmd.Parameters.Add( paramArray[i] );
                    }
                }
                try
                {
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
                catch( Exception ex )
                {
                    //Here No reconnection when DB-Connection closed
                    //The user become an Error Msg
                    //When he refreshed the dialog (GetDataTable) or retry the action (BeginTransaction), then the DB will be reconnected
                    //Trace.HMI.Log( TL.L0, TT.E, TS.S, "{0}", ex.Message );
                    if( autoCommit ) Rollback();
                    throw ex;
                }
                if( autoCommit ) Commit();
            }
            catch( Exception ex )
            {
                throw ex;
            }
            return rowsUpdated;
        }

        private object ExecuteScalarInternal( string sqlCmd )
        {
            object value = null;
            //Trace.HMI.Log( TL.L7, TT.I, TS.S, "{0}", sqlCmd );

            SqlCommand cmd = new SqlCommand( sqlCmd, Connection );

            if( trans != null )
                cmd.Transaction = trans;

            try
            {
                value = cmd.ExecuteScalar();
            }
            catch( Exception ex )
            {
                //Trace.HMI.Log( TL.L0, TT.E, TS.S, "ERROR {0} on query {1}", ex.Message, sqlCmd );
                value = null;
                if( Connection.State == ConnectionState.Closed ) throw new Exception( "7777_01" );
            }

            return value;
        }

        /// <summary>
        /// Execute a scalar operation
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        public object ExecuteScalar( string sqlCmd )
        {
            object value = null;
            int NbIfRetry = 0;

            while( true )
            {
                try
                {
                    value = ExecuteScalarInternal( sqlCmd );
                    break;
                }
                catch( Exception ex )
                {
                    if( ex.Message == "7777_01" )
                    {
                        //Trace.HMI.Log( TL.L0, TT.E, TS.S, " ####### ERROR: DB Disconnected. " );
                        NbIfRetry++;
                        if( NbIfRetry <= MaxDBRetry )
                        {
                            while( NbIfRetry <= MaxDBRetry )
                            {
                                //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######  --> Try Reconnection {0}/{1}", NbIfRetry, MaxDBRetry );
                                try { ConnectionOpen(); }
                                catch( Exception ex1 )
                                {
                                    //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######          Reconnection {0}/{1} FAILED: {2}", NbIfRetry, MaxDBRetry, ex1.Message );
                                    NbIfRetry++;
                                    continue;
                                }
                                //Trace.HMI.Log( TL.L0, TT.E, TS.S, " #######          Reconnection {0}/{1} SUCCESSFULL", NbIfRetry, MaxDBRetry );
                                break;
                            }
                            if( NbIfRetry <= MaxDBRetry ) continue;
                            else break;
                        }
                    }
                    break;
                }
            }
            return value;
        }
    }
}
