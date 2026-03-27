using System.Data;
using System.Data.Common;
using InterSystems.Data.IRISClient;
using static ErpToolkit.Helpers.Db.DatabaseManager;
using static ErpToolkit.Helpers.ErpError;

namespace ErpToolkit.Helpers.Db
{
    public class IRISDatabase : IDatabase, IDisposable
    {
        private string _connectionString;
        private IRISTransaction _transaction = null;
        private static readonly object _lock = new object();

        // Codice da eseguire solo alla prima istanziazione della classe (
        static IRISDatabase()
        {
            // Serve per la gestione della classe DbContext() con IRIS
            var irisFactory = new IRISFactory();
            DbProviderFactories.RegisterFactory("InterSystems.Data.IRISClient", irisFactory);
        }

        public IRISDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }
        ~IRISDatabase()
        {
            Dispose();
        }
        public void Dispose()
        {
            try { RollbackTransaction("Dispose"); } catch (Exception ex) { /*skip*/ }
            GC.SuppressFinalize(this);
        }

        //Gestione connessione
        private IRISConnection OpenConnection()
        {
            IRISConnection connection = null;
            lock (_lock)
            {
                connection = new IRISConnection(_connectionString);
                connection.Open();
                return connection;
            }
        }
        private void CloseConnection(IRISConnection connection)
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
            if (_transaction == null) CloseConnection((IRISConnection)connection);
        }

        public IDbCommand NewCommand(string sql, IDbConnection connection)
        {
            return (IDbCommand)new IRISCommand(sql, (IRISConnection)connection, _transaction);
        }
        public DataTable QueryReader(IDbCommand command, int maxRecords)
        {
            using (IRISDataAdapter adapter = new IRISDataAdapter((IRISCommand)command))
            {
                DataTable result = new DataTable();
                if (maxRecords < 0) adapter.Fill(result);
                else adapter.Fill(0, maxRecords, result); // restituisce maxRecords righe  
                return result;
            }
        }

        //*******************************************************************************************************

        // Gestione transazioni
        public void BeginTransaction(string transactionName)
        {
            IRISConnection connection = OpenConnection();
            try
            {
                _transaction = connection.BeginTransaction();
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
            IRISConnection connection = (IRISConnection)_transaction.Connection;
            try { _transaction.Commit(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }
        public void RollbackTransaction(string transactionName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "RollbackTransaction attempted for the wrong transaction ({transactionName}).");
            IRISConnection connection = (IRISConnection)_transaction.Connection;
            try { _transaction.Rollback(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }

        //*******************************************************************************************************

        //errori per cui conviene fare un retry
        public bool IsTransient(Exception ex)
        {
            if (ex is IRISException irisEx)
            {
                return irisEx.ErrorCode == -140 || irisEx.ErrorCode == -121; // Timeout or Deadlock
            }
            return false;
        }

        // decodifica errore per IRIS
        //public bool HandleException(Exception ex)
        //{
        //    if (ex is IRISException irisEx)
        //    {
        //        switch (irisEx.ErrorCode)  //??????????????????????????????
        //        {
        //            case 101: // Invalid object name
        //                throw new DatabaseException(ERR_DB_UNKNOWN, "Invalid object name.", ex);
        //            case -2:  // Timeout expired
        //                throw new DatabaseException(ERR_DB_TIMEOUT, "Timeout expired.", ex);
        //            //case 2601:
        //            //case 2627:
        //            //    throw new DatabaseException(ERR_DB_DUPLICATION, "Unique constraint violated.", ex);
        //            //case 547:
        //            //    throw new DatabaseException(ERR_DB_DEPENDENCY, "Cannot delete or update due to foreign key constraint.", ex);
        //            //case 1205:
        //            //    throw new DatabaseException(ERR_DB_DEADLOCK, "Deadlock encountered.", ex);
        //            //case 208:
        //            //    throw new DatabaseException(ERR_DB_UNKNOWN, "Invalid object name.", ex);
        //            //case -2:
        //            //    throw new DatabaseException(ERR_DB_TIMEOUT, "Timeout expired.", ex);
        //            default:
        //                throw new DatabaseException(ERR_DB_BADCOLUMN, "An SQL error occurred.", ex);
        //        }
        //    }
        //    else return false;
        //}
        public bool HandleException(Exception ex)
        {
            if (ex is IRISException irisEx)
            {
                // IRIS di InterSystems - personalizzazione necessaria per SQLCODE specifici.
                switch (irisEx.ErrorCode) //switch (irisEx.Sqlcode)
                {
                    case 119:
                    case 120:
                        throw new DatabaseException(ERR_DB_DUPLICATION, "Violazione del vincolo univoco.", ex);
                    case 121:
                    case 122:
                    case 123:
                    case 124:
                        throw new DatabaseException(ERR_DB_DEPENDENCY, "Violazione del vincolo di chiave esterna.", ex);
                    case 114:
                        throw new DatabaseException(ERR_DB_DEADLOCK, "Deadlock.", ex);
                    case 87:
                        throw new DatabaseException(ERR_DB_UNKNOWN, "Tabella non esistente.", ex);
                    case 29:
                    case 88:
                        throw new DatabaseException(ERR_DB_BADCOLUMN, "Colonna non esistente.", ex); // Nome campo inesistente
                    case 450:
                        throw new DatabaseException(ERR_DB_TIMEOUT, "Timeout.", ex);
                    default:
                        throw new DatabaseException(ERR_DB_ERROR, $"Errore IRIS ({irisEx.ErrorCode}).", ex);
                }
            }
            else return false;
        }


        //*******************************************************************************************************
        //*******************************************************************************************************

        // AUDIT
        //------

        ////  Permessi minimi per farlo funzionare
        ////  IRIS: visibilità su %SYS.ProcessQuery; per esaminare altri processi può servire %Admin_Manage:Use. [docs.inter...ystems.com]

        //public object GetCommandSpid(IDbConnection conn)
        //{
        //    // Euristico: PID del processo corrente in base a utente/namespace
        //    const string sql = @"
        //                            SELECT Pid
        //                            FROM %SYS.ProcessQuery
        //                            WHERE UserName = USER AND NameSpace = DATABASE()
        //                            ORDER BY SecondsConnected DESC
        //                            FETCH FIRST 1 ROWS ONLY;";
        //    using var cmd = this.NewCommand(sql, conn);
        //    if (conn.State != ConnectionState.Open) conn.Open();
        //    var o = cmd.ExecuteScalar();
        //    return (o == null || o == DBNull.Value) ? null : Convert.ToInt32(o);
        //}

        //public bool IsCommandRequestActive(IDbConnection conn, object spid)
        //{
        //    const string sql = @"SELECT 1 FROM %SYS.ProcessQuery WHERE Pid = @pid";
        //    using var cmd = this.NewCommand(sql, conn);
        //    var p = cmd.CreateParameter(); p.ParameterName = "@pid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
        //    using var rdr = cmd.ExecuteReader();
        //    return rdr.Read();
        //}

        //public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid)
        //{
        //    // Stato processo; piano solo via EXPLAIN sulla query (non live)
        //    const string sql = @"
        //                        SELECT 
        //                          Pid, UserName, NameSpace, State, Routine, CurrentSrcLine, SecondsConnected
        //                        FROM %SYS.ProcessQuery
        //                        WHERE Pid = @pid";
        //    using var cmd = this.NewCommand(sql, conn);
        //    var p = cmd.CreateParameter(); p.ParameterName = "@pid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
        //    using var rdr = cmd.ExecuteReader();
        //    if (!rdr.Read()) return null;

        //    var state = rdr["State"] as string;
        //    long elapsedMs = rdr["SecondsConnected"] == DBNull.Value ? 0 : Convert.ToInt64(rdr["SecondsConnected"]) * 1000;

        //    return new LiveSessionSnapshot
        //    {
        //        Status = state ?? "RUN",
        //        WaitType = null,
        //        TotalElapsedMs = elapsedMs,
        //        BlockingSessionId = null,
        //        SqlText = null,
        //        QueryPlanXml = "-- Per il piano usa EXPLAIN sulla query originale"
        //    };

        //}

        //  Permessi minimi per farlo funzionare
        //  IRIS: visibilità su %SYS.ProcessQuery; per esaminare altri processi può servire %Admin_Manage:Use. [docs.inter...ystems.com]

        //////////public object GetCommandSpid(IDbConnection conn)
        //////////{
        //////////    // Euristico: PID del processo corrente in base a utente/namespace
        //////////    const string sql = "select TOP 1 Pid from INFORMATION_SCHEMA.CURRENT_CONNECTIONS;";
        //////////    using var cmd = this.NewCommand(sql, conn);
        //////////    if (conn.State != ConnectionState.Open) conn.Open();
        //////////    var o = cmd.ExecuteScalar();
        //////////    return (o == null || o == DBNull.Value) ? null : Convert.ToInt32(o);
        //////////}

        //////////public bool IsCommandRequestActive(IDbConnection conn, object spid)
        //////////{
        //////////    const string sql = "SELECT 1 FROM INFORMATION_SCHEMA.CURRENT_CONNECTIONS WHERE Pid = @pid";
        //////////    using var cmd = this.NewCommand(sql, conn);
        //////////    var p = cmd.CreateParameter(); p.ParameterName = "@pid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
        //////////    using var rdr = cmd.ExecuteReader();
        //////////    return rdr.Read();
        //////////}

        //////////public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid)
        //////////{
        //////////    // Stato processo; piano solo via EXPLAIN sulla query (non live)
        //////////    const string sql = @"
        //////////                        SELECT 
        //////////                          Pid, UserName, NameSpace, State, Routine, CurrentSrcLine, SecondsConnected
        //////////                        FROM %SYS.ProcessQuery
        //////////                        WHERE Pid = @pid";
        //////////    using var cmd = this.NewCommand(sql, conn);
        //////////    var p = cmd.CreateParameter(); p.ParameterName = "@pid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
        //////////    using var rdr = cmd.ExecuteReader();
        //////////    if (!rdr.Read()) return null;

        //////////    var state = rdr["State"] as string;
        //////////    long elapsedMs = rdr["SecondsConnected"] == DBNull.Value ? 0 : Convert.ToInt64(rdr["SecondsConnected"]) * 1000;

        //////////    return new LiveSessionSnapshot
        //////////    {
        //////////        Status = state ?? "RUN",
        //////////        WaitType = null,
        //////////        TotalElapsedMs = elapsedMs,
        //////////        BlockingSessionId = null,
        //////////        SqlText = null,
        //////////        QueryPlanXml = "-- Per il piano usa EXPLAIN sulla query originale"
        //////////    };

        //////////}



        public object GetCommandSpid(IDbConnection conn)
        {
           return null;
        }
        public bool IsCommandRequestActive(IDbConnection conn, object spid)
        {
            return true;
        }
        public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid)
        {
            return new LiveSessionSnapshot
            {
                Status = "RUN",
                WaitType = null,
                TotalElapsedMs = -1,
                BlockingSessionId = null,
                SqlText = null,
                QueryPlanXml = "-- Nessun privilegio per Audit"
            };
        }





    }


    }
}

