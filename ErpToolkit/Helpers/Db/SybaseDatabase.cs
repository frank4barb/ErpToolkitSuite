
using System.Data;
using System.Data.Common;
using AdoNetCore.AseClient;
using MySqlX.XDevAPI.Common;
using static ErpToolkit.Helpers.Db.DatabaseManager;
using static ErpToolkit.Helpers.ErpError;

namespace ErpToolkit.Helpers.Db
{
    public class SybaseDatabase : IDatabase, IDisposable
    {
        private string _connectionString;
        private AseTransaction _transaction = null;
        private static readonly object _lock = new object();

        public SybaseDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }
        ~SybaseDatabase()
        {
            Dispose();
        }
        public void Dispose()
        {
            try { RollbackTransaction("Dispose"); } catch (Exception ex) { /*skip*/ }
            GC.SuppressFinalize(this);
        }

        //Gestione connessione
        private AseConnection OpenConnection()
        {
            AseConnection connection = null;
            lock (_lock)
            {
                connection = new AseConnection(_connectionString);
                connection.Open();
                return connection;
            }
        }
        private void CloseConnection(AseConnection connection)
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
            if (_transaction == null) CloseConnection((AseConnection)connection);
        }

        public IDbCommand NewCommand(string sql, IDbConnection connection)
        {
            return (IDbCommand)new AseCommand(sql, (AseConnection)connection, _transaction);
        }
        public DataTable QueryReader(IDbCommand command, int maxRecords)
        {
            using (var reader = command.ExecuteReader())
            {
                //var dataTable = new DataTable();
                //dataTable.Load(reader);  //?????????????? manca filtro maxRecords
                //return dataTable;

                var dataTable = new DataTable();
                if (maxRecords < 0) { 
                    dataTable.Load(reader); 
                }
                else
                {
                    // Costruisci le colonne del DataTable
                    for (int i = 0; i < reader.FieldCount; i++) dataTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                    // Leggi il contenuto riga per riga
                    int recordCount = 0;
                    while (reader.Read() && recordCount < maxRecords)
                    {
                        var row = dataTable.NewRow();
                        foreach (DataColumn column in dataTable.Columns) row[column.ColumnName] = reader[column.ColumnName];
                        dataTable.Rows.Add(row);
                        recordCount++;
                    }
                }
                return dataTable;
            }
        }

        //*******************************************************************************************************

        // Gestione transazioni
        public void BeginTransaction(string transactionName)
        {
            AseConnection connection = OpenConnection();
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
            AseConnection connection = _transaction.Connection;
            try { _transaction.Commit(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }
        public void RollbackTransaction(string transactionName)
        {
            if (_transaction == null) throw new DatabaseException(ERR_DB_TRANSACTION, "RollbackTransaction attempted for the wrong transaction ({transactionName}).");
            AseConnection connection = _transaction.Connection;
            try { _transaction.Rollback(); _transaction.Dispose(); _transaction = null; }
            finally { CloseConnection(connection); }
        }

        //*******************************************************************************************************

        //errori per cui conviene fare un retry
        public bool IsTransient(Exception ex)
        {
            if (ex is AseException aseEx)  //uguali a sqlserver
            {
                return aseEx.HResult == -2 || aseEx.HResult == 1205; // Timeout or Deadlock
            }
            return false;
        }

        // decodifica errore per sqlserver
        public bool HandleException(Exception ex)
        {
            if (ex is AseException aseEx)
            {
                switch (aseEx.HResult)  //uguali a sqlserver
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
                        throw new DatabaseException(ERR_DB_ERROR, "Errore Sybase.", ex);
                }
            }
            else return false;
        }


        //*******************************************************************************************************
        //*******************************************************************************************************

        // AUDIT
        //------

        //  Permessi minimi per farlo funzionare
        //  Sybase ASE: sa_role per sp_showplan e MDA tables come monProcess. [help.sap.com], [help.sap.com]

        public object GetCommandSpid(IDbConnection conn)
        {
            using var cmd = this.NewCommand("SELECT @@spid;", conn);
            if (conn.State != ConnectionState.Open) conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool IsCommandRequestActive(IDbConnection conn, object spid)
        {
            // monProcess contiene processi in esecuzione/attesa
            const string sql = @"SELECT 1 FROM master..monProcess WHERE SPID = @spid;";
            using var cmd = this.NewCommand(sql, conn);
            var p = cmd.CreateParameter(); p.ParameterName = "@spid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
            using var rdr = cmd.ExecuteReader();
            return rdr.Read();
        }

        public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid)
        {
            // Stato + wait da monProcess; SQL corrente da monProcessStatement (se installata); piano via sp_showplan (non sempre catturabile come resultset)
            const string sql = @"
                                    SELECT TOP 1 
                                        Command, SecondsWaiting, WaitEventID, BlockingSPID
                                    FROM master..monProcess
                                    WHERE SPID = @spid;

                                    -- SQL corrente (se disponibile):
                                    -- SELECT SQLText FROM master..monProcessStatement WHERE SPID = @spid;
                                    ";
            using var cmd = this.NewCommand(sql, conn);
            var p = cmd.CreateParameter(); p.ParameterName = "@spid"; p.Value = Convert.ToInt32(spid); cmd.Parameters.Add(p);
            using var rdr = cmd.ExecuteReader();
            string status = null; long elapsedMs = 0; int? blocker = null;

            if (rdr.Read())
            {
                status = rdr["Command"] as string;
                elapsedMs = rdr["SecondsWaiting"] == DBNull.Value ? 0 : Convert.ToInt64(rdr["SecondsWaiting"]) * 1000;
                blocker = rdr["BlockingSPID"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["BlockingSPID"]);
            }

            // Piano: spesso è disponibile come messaggio via sp_showplan; lo indichiamo testualmente
            string planText = "-- Piano disponibile via sp_showplan {spid}, 'long' (messaggi server)";
            return new LiveSessionSnapshot
            {
                Status = status ?? "ACTIVE",
                WaitType = rdr["WaitEventID"] == DBNull.Value ? null : $"WaitEventID={rdr["WaitEventID"]}",
                TotalElapsedMs = elapsedMs,
                BlockingSessionId = blocker,
                SqlText = null, // puoi aggiungere lettura da monProcessStatement se presente
                QueryPlanXml = planText
            };
        }


    }
}