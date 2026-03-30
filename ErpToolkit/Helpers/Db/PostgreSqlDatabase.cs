using System.Data;
using System.Data.Common;
using Npgsql;
using NpgsqlTypes;
using static ErpToolkit.Helpers.Db.DatabaseManager;
using static ErpToolkit.Helpers.ErpError;

namespace ErpToolkit.Helpers.Db
{
    public class PostgreSqlDatabase : IDatabase, IDisposable
    {
        private string _connectionString;
        private NpgsqlTransaction _transaction = null;
        private static readonly object _lock = new object();

        public PostgreSqlDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }
        ~PostgreSqlDatabase()
        {
            Dispose();
        }
        public void Dispose()
        {
            try { RollbackTransaction("Dispose"); } catch (Exception ex) { /*skip*/ }
            GC.SuppressFinalize(this);
        }

        //Gestione connessione
        private NpgsqlConnection OpenConnection()
        {
            NpgsqlConnection connection = null;
            lock (_lock)
            {
                connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                return connection;
            }
        }
        private void CloseConnection(NpgsqlConnection connection)
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
            if (_transaction == null) CloseConnection((NpgsqlConnection)connection);
        }

        public IDbCommand NewCommand(string sql, IDbConnection connection)
        {
            return (IDbCommand)new NpgsqlCommand(sql, (NpgsqlConnection)connection, _transaction);
        }
        public DataTable QueryReader(IDbCommand command, int maxRecords)
        {
            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter((NpgsqlCommand)command))
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
            NpgsqlConnection connection = OpenConnection();
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
            NpgsqlConnection connection = _transaction.Connection;
            try { _transaction.Commit(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }
        public void RollbackTransaction(string transactionName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "RollbackTransaction attempted for the wrong transaction ({transactionName}).");
            NpgsqlConnection connection = _transaction.Connection;
            try { _transaction.Rollback(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }

        //*******************************************************************************************************

        //errori per cui conviene fare un retry
        public bool IsTransient(Exception ex)
        { 
            if (ex is NpgsqlException npgsqlEx)  //???????????????????????????????????
            {
                //return npgsqlEx.ErrorCode == -2 || npgsqlEx.ErrorCode == 1205; // Timeout or Deadlock

                //case "40001": // Serialization failure
                //case "40P01": // Deadlock detected
                //return npgsqlEx.SqlState == "40001" || npgsqlEx.SqlState == "40P01";
                return npgsqlEx.SqlState == "40001" || npgsqlEx.SqlState == "57014" || npgsqlEx.SqlState == "40P01";
            }
            return false;
        }

        // decodifica errore per sqlserver
        public bool HandleException(Exception ex)
        {
            if (ex is NpgsqlException npgsqlEx)  //???????????????????????????????????
            {
                switch (npgsqlEx.SqlState)
                {
                    case "23505":
                        throw new DatabaseException(ERR_DB_DUPLICATION, "Violazione del vincolo univoco.", ex);
                    case "23503":
                        throw new DatabaseException(ERR_DB_DEPENDENCY, "Violazione del vincolo di chiave esterna.", ex);
                    case "40P01":
                        throw new DatabaseException(ERR_DB_DEADLOCK, "Deadlock.", ex);
                    case "42P01":
                        throw new DatabaseException(ERR_DB_UNKNOWN, "Tabella non esistente.", ex);
                    case "42703":
                        throw new DatabaseException(ERR_DB_BADCOLUMN, "Colonna non esistente.", ex); // Nome campo inesistente
                    case "57014":
                        throw new DatabaseException(ERR_DB_TIMEOUT, "Timeout.", ex);
                    default:
                        throw new DatabaseException(ERR_DB_ERROR, "Errore PostgreSQL.", ex);
                }
            }
            else return false;
        }


        //*******************************************************************************************************
        //*******************************************************************************************************

        // AUDIT
        //------

        //  Permessi minimi per farlo funzionare
        //  PostgreSQL: accesso a pg_stat_activity; funzione pg_blocking_pids() disponibile da 9.6. [postgresql.org], [stackoverflow.com]

        public object GetCommandSpid(IDbConnection conn)
        {
            using var cmd = this.NewCommand("SELECT pg_backend_pid();", conn);
            if (conn.State != ConnectionState.Open) conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool IsCommandRequestActive(IDbConnection conn, object spid)
        {
            const string sql = @"
                                SELECT 1
                                FROM pg_stat_activity
                                WHERE pid = @pid AND state = 'active';";
            using var cmd = this.NewCommand(sql, conn);
            var p = cmd.CreateParameter(); p.ParameterName = "@pid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
            using var rdr = cmd.ExecuteReader();
            return rdr.Read();
        }

        public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid, string sqlText, IDictionary<string, object> parameters)
        {
            // Snapshot live (piano stimato non live)
            const string sql = @"
                                SELECT 
                                    query AS sql_text,
                                    now() - query_start AS duration,
                                    wait_event_type,
                                    wait_event,
                                    state
                                FROM pg_stat_activity
                                WHERE pid = @pid;";
            using var cmd = this.NewCommand(sql, conn);
            var p = cmd.CreateParameter(); p.ParameterName = "@pid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var sql_Text = reader["sql_text"] as string;
            long elapsedMs = 0;
            if (reader["duration"] != DBNull.Value) elapsedMs = (long)((TimeSpan)reader["duration"]).TotalMilliseconds;

            return new LiveSessionSnapshot
            {
                Status = reader["state"] as string ?? "active",
                WaitType = reader["wait_event_type"] as string,
                TotalElapsedMs = elapsedMs,
                BlockingSessionId = null,
                SqlText = sql_Text,
                QueryPlanXml = "-- Usa EXPLAIN (FORMAT TEXT) sulla stessa query per il piano stimato"
            };
        }



    }
}



