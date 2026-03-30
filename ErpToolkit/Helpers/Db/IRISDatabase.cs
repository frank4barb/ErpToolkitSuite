using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Drawing;
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

            // In IRIS non abbiamo uno SPID come SQL Server.
            // Restituiamo sempre null: verrà calcolato nell’audit thread.
            return -1;  //restutuisco un valore per non interrompere il processo di loggin  // return null;
        }
        public bool IsCommandRequestActive(IDbConnection conn, object spid)
        {
            // IRIS non permette un vero "is this statement still executing?"
            // Ma essendo che questo metodo viene chiamato solo dopo waitMs e solo
            // se la query non ha ancora terminato, possiamo dire che è "attiva".
            return true;
        }
        //public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid)
        //{
        //    return new LiveSessionSnapshot
        //    {
        //        Status = "RUN",
        //        WaitType = null,
        //        TotalElapsedMs = -1,
        //        BlockingSessionId = null,
        //        SqlText = null,
        //        QueryPlanXml = "-- Nessun privilegio per Audit"
        //    };
        //}

        public LiveSessionSnapshot GetCommandAuditSnapshot(IDbConnection conn, object spid, string sqlText, IDictionary<string, object> parameters)
        {
            //
            // 1. Chiave cache basata su SQL normalizzato + lista nomi parametri
            //
            string cacheKey = BuildExplainCacheKey(sqlText, parameters);

            //
            // 2. Cache HIT → ritorna immediatamente
            //
            if (TryGetFromCache(cacheKey, out var cached))
            {
                return new LiveSessionSnapshot
                {
                    Status = $"Cached-Compiled <Hash>{cached.Hash}</Hash>",
                    WaitType = null,
                    TotalElapsedMs = -1,
                    BlockingSessionId = null,
                    SqlText = cached.NormalizedSql,
                    QueryPlanXml = cached.PlanXml
                };
            }

            //
            // 3. CACHE MISS → esegui EXPLAIN
            //
            var irisCmd = (InterSystems.Data.IRISClient.IRISCommand)conn.CreateCommand();
            irisCmd.CommandText = "EXPLAIN " + sqlText;
            irisCmd.CommandTimeout = 30;

            foreach (var kv in parameters)
            {
                var p = irisCmd.CreateParameter();
                p.ParameterName = kv.Key;
                p.Value = kv.Value ?? DBNull.Value;
                irisCmd.Parameters.Add(p);
            }

            using var reader = irisCmd.ExecuteReader();

            if (!reader.Read())
                return default;

            string normalizedSql = reader["SQL"]?.ToString() ?? sqlText;
            string planXml = reader["Plan"]?.ToString() ?? "";

            // Estrai hash
            string hash = null;
            var m = System.Text.RegularExpressions.Regex.Match(planXml, "<Hash>(.*?)</Hash>");
            if (m.Success) hash = m.Groups[1].Value;

            //
            // 4. Inserisci in cache
            //
            var entry = new ExplainCacheEntry
            {
                NormalizedSql = normalizedSql,
                PlanXml = planXml,
                Hash = hash
            };

            AddToCache(cacheKey, entry);

            //
            // 5. Ritorna risultati
            //
            return new LiveSessionSnapshot
            {
                Status = $"Compiled <Hash>{hash}</Hash>",
                WaitType = null,
                TotalElapsedMs = -1,
                BlockingSessionId = null,
                SqlText = planXml,
                QueryPlanXml = normalizedSql
            };
        }

        //Definizione della classe Entry
        //------------------------------
        class ExplainCacheEntry
        {
            public string NormalizedSql { get; init; }
            public string PlanXml { get; init; }
            public string Hash { get; init; }
        }

        //Definizione della LRU Cache
        //---------------------------

        // Max cache entries
        private const int MAX_CACHE_SIZE = 100;

        // Dictionary: accesso lock-free
        private static readonly ConcurrentDictionary<string, LinkedListNode<(string Key, ExplainCacheEntry Entry)>> _cacheDict
            = new();

        // Lista LRU: richiede lock
        private static readonly LinkedList<(string Key, ExplainCacheEntry Entry)> _lruList
            = new();

        // Lock per modificare LRU
        private static readonly object _lruLock = new();

        //Funzione: TryGetFromCache (lettura ultra‑veloce)
        private bool TryGetFromCache(string key, out ExplainCacheEntry entry)
        {
            entry = null;

            if (_cacheDict.TryGetValue(key, out var node))
            {
                lock (_lruLock)
                {
                    // Aggiorna LRU → sposta in testa
                    _lruList.Remove(node);
                    _lruList.AddFirst(node);
                }

                entry = node.Value.Entry;
                return true;
            }

            return false;
        }

        //Funzione: AddToCache (gestisce limite e LRU)
        private void AddToCache(string key, ExplainCacheEntry entry)
        {
            lock (_lruLock)
            {
                // Se già esiste, aggiorna la posizione
                if (_cacheDict.TryGetValue(key, out var existingNode))
                {
                    _lruList.Remove(existingNode);
                    _lruList.AddFirst(existingNode);
                    return;
                }

                // Crea nuovo nodo
                var newNode = new LinkedListNode<(string Key, ExplainCacheEntry Entry)>((key, entry));
                _lruList.AddFirst(newNode);
                _cacheDict[key] = newNode;

                // Rimuovi vecchio se superiamo MAX
                if (_cacheDict.Count > MAX_CACHE_SIZE)
                {
                    var last = _lruList.Last;
                    if (last != null)
                    {
                        _lruList.RemoveLast();
                        _cacheDict.TryRemove(last.Value.Key, out _);
                    }
                }
            }
        }

        //Funzione helper per generare la chiave cache
        //(uguale per query con stessi parametri, indipendente dai valori)
        private string BuildExplainCacheKey(string sqlText, IDictionary<string, object> parameters)
        {
            // Normalizza: elimina doppi spazi, tabs, newline
            string norm = System.Text.RegularExpressions.Regex
                            .Replace(sqlText, @"\s+", " ")
                            .Trim()
                            .ToUpperInvariant();

            // Parametri: ordina per nome
            var sortedParams = parameters.Keys
                                         .OrderBy(k => k, StringComparer.OrdinalIgnoreCase);

            string paramList = string.Join(",", sortedParams);

            return norm + "||PARAMS||" + paramList;
        }

        //public async Task<DataTable> ExecuteWithTimeoutAsync(
        //    string sql,
        //    int timeoutMs,
        //    Action<string> onTimeoutHash)
        //{
        //    using (var conn = new IRISConnection(_connectionString))
        //    {
        //        await conn.OpenAsync();

        //        // 1. Recupero hash della query compilata
        //        string hash = GetQueryHash(conn, sql);

        //        // 2. Esecuzione con timeout
        //        using (var cmd = new IRISCommand(sql, conn))
        //        {
        //            var cts = new CancellationTokenSource();
        //            var token = cts.Token;

        //            var executeTask = Task.Run(() =>
        //            {
        //                var dt = new DataTable();
        //                using (var da = new IRISDataAdapter(cmd))
        //                {
        //                    da.Fill(dt);
        //                }
        //                return dt;
        //            }, token);

        //            if (await Task.WhenAny(executeTask, Task.Delay(timeoutMs)) == executeTask)
        //            {
        //                // Finita in tempo
        //                return executeTask.Result;
        //            }
        //            else
        //            {
        //                // Timeout → salvo hash
        //                onTimeoutHash?.Invoke(hash);
        //                cts.Cancel();
        //                throw new TimeoutException("Query timeout");
        //            }
        //        }
        //    }
        //}

        //private string GetQueryHash(IRISConnection conn, string sql)
        //{
        //    // Chiamata ObjectScript per preparare la query e restituire il suo hash
        //    string objscript =
        //        "Set stmt=##class(%SQL.Statement).%New()" +
        //        " Do stmt.%Prepare($zv($$Normalize^%SQLUTIL($$ReplaceLiteral^%SQLUTIL(\"" + sql.Replace("\"", "\"\"") + "\"))))" +
        //        " Quit stmt.%GetHash()";

        //    using (var cmd = new IRISCommand("DO $SYSTEM.SQL.ShellCommand(\"" + objscript + "\")", conn))
        //    {
        //        // Restituisce un singolo valore
        //        cmd.CommandType = CommandType.Text;
        //        var hash = cmd.ExecuteScalar()?.ToString();
        //        return hash;
        //    }
        //}

        //public DataTable execQuery(string sql, ref string errMsg)
        //{
        //    string connectionString = ""; int MaxRecords = 200; bool isTimeout = false;
        //    errMsg = "";
        //    DataTable dt = new DataTable();
        //    IRISConnection connection = new IRISConnection(connectionString);
        //    try
        //    {
        //        string traceALL = ErpContext.Instance.GetString("#traceDbALL");
        //        //"Info: in execQuery: " + sql);

        //        IRISDataAdapter adapter = new IRISDataAdapter(sql, connection);

        //        adapter.SelectCommand.CommandTimeout = 60; // Default Is 60 seconds
        //        adapter.TableMappings.Add("Table", "QUERY");
        //        IRISCommandBuilder icb = new IRISCommandBuilder(adapter);
        //        DataSet ds = new DataSet();
        //        adapter.Fill(ds, 0, MaxRecords, "QUERY");
        //        dt = ds.Tables["QUERY"];
        //    }
        //    catch (Exception ex)
        //    {
        //        // --gestione tipo errore
        //        try
        //        {
        //            var irisEx = (InterSystems.Data.IRISClient.IRISException)ex.GetBaseException();
        //            if (irisEx == null == false && irisEx.NativeError == 450) isTimeout = true;
        //        }
        //        catch (Exception xx)
        //        {
        //        }
        //        // -----------------------
        //        errMsg = ex.Message;
        //        DataRow[] err = dt.GetErrors();
        //        System.Text.StringBuilder msg = new System.Text.StringBuilder();
        //        foreach (var e in err)
        //            msg.Append(" - " + e.RowError);
        //        errMsg += msg.ToString();
        //        //"Errore " + errMsg + " in execQuery: " + sql);
        //        dt = null/* TODO Change to default(_) if this is not a reference type */;
        //    }
        //    finally
        //    {
        //        // always close connection.
        //        connection.Close();
        //    }

        //    return dt;
        //}



    }
}

