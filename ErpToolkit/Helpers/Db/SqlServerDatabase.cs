
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using static ErpToolkit.Helpers.Db.DatabaseManager;
using static ErpToolkit.Helpers.ErpError;

namespace ErpToolkit.Helpers.Db
{
    public class SqlServerDatabase : IDatabase, IDisposable
    {
        private string _connectionString;
        private SqlTransaction _transaction = null;
        private static readonly object _lock = new object();

        private readonly bool sqlTrace = true;

        public SqlServerDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }
        ~SqlServerDatabase()
        {
            Dispose();
        }
        public void Dispose()
        {
            try { RollbackTransaction("Dispose"); } catch (Exception ex) { /*skip*/ }
            GC.SuppressFinalize(this);
        }

        //init options after connection open
        public void InitOptions(IDbConnection conn)
        {
            using var cmd = this.NewCommand("CHECKPOINT;", conn);  // esegue CHECKPOINT per liberare il transaction log di tipo SIMPLE in SqlServer e Sybase
            if (conn.State != ConnectionState.Open) conn.Open();
            cmd.ExecuteScalar();
        }


        //Gestione connessione
        private SqlConnection OpenConnection()
        {
            SqlConnection connection = null;
            lock (_lock)
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();

                //---
                if (sqlTrace) LogCommandInit(connection);       // TRACE !!!!!
                //---

                InitOptions(connection);    //esegue opzioni iniziali dopo apertura connessione (es. CHECKPOINT per liberare transaction log SIMPLE in SqlServer)

                return connection;
            }
        }
        private void CloseConnection(SqlConnection connection)
        {
            lock (_lock)
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }
        }

        //Gestione connessione comando reader
        public IDbConnection NewConnection()
        {
            //return (IDbConnection)new SqlConnection(_connectionString);
            if (_transaction != null) return _transaction.Connection;
            else return (IDbConnection)OpenConnection();
        }
        public void ReleaseConnection(IDbConnection connection)
        {
            //return (IDbConnection)new SqlConnection(_connectionString);
            if (_transaction == null) CloseConnection((SqlConnection)connection);
        }

        public IDbCommand NewCommand(string sql, IDbConnection connection)
        {
            return (IDbCommand)new SqlCommand(sql, (SqlConnection)connection, _transaction);
        }
        public DataTable QueryReader(IDbCommand command, int maxRecords)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter((SqlCommand)command))
            {


                //DataTable result = new DataTable();
                //if (maxRecords < 0) adapter.Fill(result);
                //else adapter.Fill(0, maxRecords, result); // restituisce maxRecords righe  
                //return result;




                DataTable result = new DataTable();



                Stopwatch stopwatch = null;
                if (sqlTrace) stopwatch = LogCommandBefore((SqlCommand)command); //TRACE !!!!!
                try
                {

                    if (maxRecords < 0) adapter.Fill(result);
                    else adapter.Fill(0, maxRecords, result); // restituisce maxRecords righe  

                    if (sqlTrace) LogCommandAfterOK(stopwatch); //TRACE !!!!!
                }
                catch (Exception ex)
                {
                    if (sqlTrace) LogCommandAfterKO(stopwatch, ex); //TRACE !!!!!
                    throw;
                }





                return result;
            }
        }

        //*******************************************************************************************************

        // Gestione transazioni
        public void BeginTransaction(string transactionName)
        {
            SqlConnection connection = OpenConnection();
            try
            {
                _transaction = connection.BeginTransaction(transactionName);
                if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "BeginTransaction attempted for the wrong transaction ({transactionName}).");
            }
            finally
            {
                if (_transaction == null) CloseConnection(connection);
            }
        }
        public void SavePointTransaction(string savePointName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "SavePointTransaction attempted for the wrong transaction ({savePointName}).");
            _transaction.Save(savePointName);
        }
        public void RollbackSavePoint(string savePointName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "RollbackSavePoint attempted for the wrong transaction ({savePointName}).");
            _transaction.Rollback(savePointName);
        }
        public void CommitSavePoint(string savePointName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "CommitSavePoint attempted for the wrong transaction ({savePointName}).");
        }
        public void CommitTransaction(string transactionName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "CommitTransaction attempted for the wrong transaction ({transactionName}).");
            SqlConnection connection = _transaction.Connection;
            try { _transaction.Commit(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }
        public void RollbackTransaction(string transactionName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "RollbackTransaction attempted for the wrong transaction ({transactionName}).");
            SqlConnection connection = _transaction.Connection;
            try { _transaction.Rollback(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }

        //*******************************************************************************************************

        //errori per cui conviene fare un retry
        public bool IsTransient(Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                return sqlEx.Number == -2 || sqlEx.Number == 1205; // Timeout or Deadlock
            }
            return false;
        }

        // decodifica errore per sqlserver
        public bool HandleException(Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 2601:
                    case 2627:
                        throw new DatabaseException(ERR_DB_DUPLICATION, "Violazione del vincolo univoco.", ex);
                    case 547:
                        throw new DatabaseException(ERR_DB_DEPENDENCY, "Violazione del vincolo di chiave esterna.", ex);
                    case 1205:
                        throw new DatabaseException(ERR_DB_DEADLOCK, "Deadlock.", ex);
                    case 208:
                        throw new DatabaseException(ERR_DB_UNKNOWN, "Tabella non esistente.", ex);
                    case 207:
                        throw new DatabaseException(ERR_DB_BADCOLUMN, "Colonna non esistente.", ex); // Nome campo inesistente
                    case -2:
                        throw new DatabaseException(ERR_DB_TIMEOUT, "Timeout.", ex);
                    default:
                        throw new DatabaseException(ERR_DB_ERROR, "Errore SQL Server.", ex);
                }
            }
            else return false;
        }


        //*******************************************************************************************************

        private void LogCommandInit(SqlConnection _connection)
        {

            _connection.InfoMessage += Connection_InfoMessage;

            // Esegui un comando che genera un messaggio informativo
            SqlCommand command = new SqlCommand($@"
                                        SET STATISTICS TIME ON;
                                        SET STATISTICS IO ON;
                                        PRINT 'Messaggio dopo l’associazione';
                                                    ", _connection);
            command.ExecuteNonQuery();
        }


        private Stopwatch LogCommandBefore(SqlCommand _command)
        {
            Console.WriteLine("----- Executing SQL Command -----");
            Console.WriteLine(_command.CommandText);
            foreach (IDataParameter param in _command.Parameters)
            {
                Console.WriteLine($"Param: {param.ParameterName} = {param.Value}");
            }
            Console.WriteLine($"Timeout: {_command.CommandTimeout} seconds");
            Console.WriteLine("---------------------------------");
            return Stopwatch.StartNew();
        }
        private void LogCommandAfterOK(Stopwatch stopwatch)
        {
            stopwatch.Stop();
            Console.WriteLine($"[INFO] NonQuery executed in {stopwatch.ElapsedMilliseconds} ms");
        }
        private void LogCommandAfterKO(Stopwatch stopwatch, Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"[ERROR] Exception after {stopwatch.ElapsedMilliseconds} ms: {ex.Message}");
        }
        // Metodo che gestisce l'evento InfoMessage
        private static void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            Console.WriteLine("[SQL INFO MESSAGE] " + e.Message);
        }



        //*******************************************************************************************************
        //*******************************************************************************************************

        // AUDIT
        //------

        //  Permessi minimi per farlo funzionare
        //  SQL Server: VIEW SERVER STATE per DMV(istanza), o VIEW DATABASE STATE su Azure SQL; facoltativo SHOWPLAN per SET SHOWPLAN_XML. [learn.microsoft.com], [learn.microsoft.com]

        public object GetCommandSpid(IDbConnection conn)
        {
            using var cmd = this.NewCommand("SELECT @@SPID;", conn);
            if (conn.State != ConnectionState.Open) conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool IsCommandRequestActive(IDbConnection conn, object spid)
        {
            const string sql = @"SELECT 1 FROM sys.dm_exec_requests WHERE session_id = @spid;";
            using var cmd = this.NewCommand(sql, conn);
            var p = cmd.CreateParameter(); p.ParameterName = "@spid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
            using var rdr = cmd.ExecuteReader();
            return rdr.Read();
        }

        public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid, string sqlText, IDictionary<string, object> parameters)
        {
            const string sql = @"
                                    SELECT 
                                        r.status,
                                        r.wait_type,
                                        r.total_elapsed_time,
                                        r.blocking_session_id,
                                        st.text AS sql_text,
                                        CAST(qp.query_plan AS NVARCHAR(MAX)) AS query_plan_xml
                                    FROM sys.dm_exec_requests AS r
                                    OUTER APPLY sys.dm_exec_sql_text(r.sql_handle) AS st
                                    OUTER APPLY sys.dm_exec_query_plan(r.plan_handle) AS qp
                                    WHERE r.session_id = @spid;";
            using var cmd = this.NewCommand(sql, conn);
            var p = cmd.CreateParameter(); p.ParameterName = "@spid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return new LiveSessionSnapshot
            {
                Status = reader["status"] as string,
                WaitType = reader["wait_type"] as string,
                TotalElapsedMs = reader["total_elapsed_time"] == DBNull.Value ? 0 : Convert.ToInt64(reader["total_elapsed_time"]),
                BlockingSessionId = reader["blocking_session_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["blocking_session_id"]),
                SqlText = reader["sql_text"] as string,
                QueryPlanXml = reader["query_plan_xml"] == DBNull.Value ? null : (reader["query_plan_xml"] as string)
            };
        }





    }
}