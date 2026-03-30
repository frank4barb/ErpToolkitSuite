using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using static ErpToolkit.Helpers.Db.DatabaseManager;
using static ErpToolkit.Helpers.ErpError;

namespace ErpToolkit.Helpers.Db
{
    public class OracleDatabase : IDatabase, IDisposable
    {
        private string _connectionString;
        private OracleTransaction _transaction = null;
        private static readonly object _lock = new object();

        public OracleDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }
        ~OracleDatabase()
        {
            Dispose();
        }
        public void Dispose()
        {
            try { RollbackTransaction("Dispose"); } catch (Exception ex) { /*skip*/ }
            GC.SuppressFinalize(this);
        }

        //Gestione connessione
        private OracleConnection OpenConnection()
        {
            OracleConnection connection = null;
            lock (_lock)
            {
                connection = new OracleConnection(_connectionString);
                connection.Open();
                return connection;
            }
        }
        private void CloseConnection(OracleConnection connection)
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
            if (_transaction == null) CloseConnection((OracleConnection)connection);
        }

        public IDbCommand NewCommand(string sql, IDbConnection connection)
        {
            OracleCommand command = new OracleCommand(sql, (OracleConnection)connection);
            if (_transaction != null) command.Transaction = _transaction;
            return (IDbCommand)command;
        }
        public DataTable QueryReader(IDbCommand command, int maxRecords)
        {
            using (OracleDataAdapter adapter = new OracleDataAdapter((OracleCommand)command))
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
            OracleConnection connection = OpenConnection();
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
            OracleConnection connection = _transaction.Connection;
            try { _transaction.Commit(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }
        public void RollbackTransaction(string transactionName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "RollbackTransaction attempted for the wrong transaction ({transactionName}).");
            OracleConnection connection = _transaction.Connection;
            try { _transaction.Rollback(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }

        //*******************************************************************************************************

        //errori per cui conviene fare un retry
        public bool IsTransient(Exception ex)
        {
            if (ex is OracleException oracleEx)  //??????????????????????????????????????????????
            {
                //return oracleEx.Number == -2 || oracleEx.Number == 1205; // Timeout or Deadlock
                //// Errori transitori tipici di Oracle
                //case 4068:  // SQL package state reset
                //case 1033:  // ORA-01033: ORACLE initialization or shutdown in progress
                //case 1034:  // ORA-01034: ORACLE not available
                return oracleEx.Number == 4068 || oracleEx.Number == 1033
                    || oracleEx.Number == 1034 || oracleEx.Number == 1013 || oracleEx.Number == 60;
            }
            return false;
        }

        // decodifica errore per sqlserver
        public bool HandleException(Exception ex)
        {
            if (ex is OracleException oracleEx)
            {
                switch (oracleEx.Number)
                {
                    case 1:
                        throw new DatabaseException(ERR_DB_DUPLICATION, "Violazione del vincolo univoco.", ex);
                    case 2292:
                        throw new DatabaseException(ERR_DB_DEPENDENCY, "Violazione del vincolo di chiave esterna.", ex);
                    case 60:
                        throw new DatabaseException(ERR_DB_DEADLOCK, "Deadlock.", ex);
                    case 942:
                        throw new DatabaseException(ERR_DB_UNKNOWN, "Tabella non esistente.", ex);
                    case 904:
                        throw new DatabaseException(ERR_DB_BADCOLUMN, "Colonna non esistente.", ex); // Nome campo inesistente
                    case 1013:
                        throw new DatabaseException(ERR_DB_TIMEOUT, "Timeout.", ex);
                    default:
                        throw new DatabaseException(ERR_DB_ERROR, "Errore Oracle.", ex);
                }
            }
            else return false;
        }


        //*******************************************************************************************************
        //*******************************************************************************************************

        // AUDIT
        //------

        //  Permessi minimi per farlo funzionare
        //  Oracle: SELECT_CATALOG_ROLE/privilegi su V$SQL_PLAN, V$SQL, V$SESSION per DBMS_XPLAN.DISPLAY_CURSOR. [docs.cloud...oracle.com]

        public object GetCommandSpid(IDbConnection conn)
        {
            using var cmd = this.NewCommand("SELECT SYS_CONTEXT('USERENV','SID') FROM DUAL", conn);
            if (conn.State != ConnectionState.Open) conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()); // SID come NUMBER -> int
        }

        public bool IsCommandRequestActive(IDbConnection conn, object spid)
        {
            const string sql = @"SELECT 1 FROM v$session WHERE sid = :sid AND status = 'ACTIVE'";
            using var cmd = this.NewCommand(sql, conn);
            var p = cmd.CreateParameter(); p.ParameterName = ":sid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
            using var rdr = cmd.ExecuteReader();
            return rdr.Read();
        }

        public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid, string sqlText, IDictionary<string, object> parameters)
        {
            // Recupera sql_id/child_number e piano con DBMS_XPLAN.DISPLAY_CURSOR
            const string s = @"
                                SELECT sid, sql_id, sql_child_number AS child_number, status
                                FROM v$session
                                WHERE sid = :sid";
            string sqlId = null; int child = 0; string status = null;

            using (var cmd = this.NewCommand(s, conn))
            {
                var p = cmd.CreateParameter(); p.ParameterName = ":sid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
                using var rdr = cmd.ExecuteReader();
                if (!rdr.Read()) return null;
                sqlId = rdr["sql_id"] as string;
                child = rdr["child_number"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["child_number"]);
                status = rdr["status"] as string;
            }

            string planText = null;
            if (!string.IsNullOrEmpty(sqlId))
            {
                const string plan = @"
                                        SELECT PLAN_TABLE_OUTPUT
                                        FROM TABLE(DBMS_XPLAN.DISPLAY_CURSOR(:sql_id, :child, 'ALLSTATS LAST'))";
                using var cmd = this.NewCommand(plan, conn);
                var p1 = cmd.CreateParameter(); p1.ParameterName = ":sql_id"; p1.Value = sqlId; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = ":child"; p2.Value = child; cmd.Parameters.Add(p2);
                using var rdr = cmd.ExecuteReader();
                var sb = new System.Text.StringBuilder();
                while (rdr.Read()) sb.AppendLine(rdr["PLAN_TABLE_OUTPUT"] as string);
                planText = sb.ToString();
            }

            return new LiveSessionSnapshot
            {
                Status = status ?? "ACTIVE",
                WaitType = null,
                TotalElapsedMs = 0,
                BlockingSessionId = null,
                SqlText = null, // puoi ottenere SQL da v$sqltext se necessario
                QueryPlanXml = planText
            };
        }



    }
}




