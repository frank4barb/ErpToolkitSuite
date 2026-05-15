using CsvHelper.Configuration;
using CsvHelper;
using System.Data;
using System.Globalization;
using static ErpToolkit.Helpers.ErpError;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using System.Data.Entity;
using ErpToolkit.Models;
using System.Collections.Concurrent;
using System.IO;
using System;
using System.Data.Common;
using System.Transactions;
using Quartz.Util;
using Amazon.SecurityToken.Model;
using Org.BouncyCastle.Utilities;

namespace ErpToolkit.Helpers.Db
{
    // Funzioni di gestione accesso al Database, indipendentemente dal DBMS
    public class DatabaseManager : IDisposable
    {
        public enum DbTyp { SqlServer, Sybase, MySql, PostgreSql, SQLite, Oracle, IRIS, MongoDb }

        private readonly DbTyp _databaseType;
        private readonly IDatabase _database;
        private readonly string _connectionString;
        private static NLog.ILogger _logger;

        private Stack<string> _transactionStack = new Stack<string>();
        private Timer _transactionTimeoutTimer;
        private string _transactionId = null;

        private string _dumpLastSql = "";
        private readonly int _auditBeforeTimeoutSeconds = 5;

        // Proprietà configurabili
        public DbTyp DatabaseType { get { return _databaseType; } }
        public int PageSize { get; set; } = 1000;  //ReadBlob, WriteBlob
        public int MaxRetries { get; set; } = 2;
        public int DelayBetweenRetriesMs { get; set; } = 1000;
        public int TimeoutSeconds { get; set; } = 30;
        public int TransactionTimeoutSeconds { get; set; } = 60;
        public int MaxRecords { get; set; } = 10000;
        public bool EnableTrace { get; set; } = false;
        public bool EnableTraceTimeout { get; set; } = true;
        public long MaxFileLengthBytes { get; set; } = 100 * 1024 * 1024;  // 100 Mb

        internal DatabaseManager(DbTyp databaseType, IDatabase database, string connectionString)
        {
            //SetUpNLog();
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();
            //set database
            _databaseType = databaseType;
            _database = database;
            _connectionString = connectionString;
        }
        ~DatabaseManager()
        {
            Dispose();
        }
        public void Dispose()
        {
            _database?.Dispose(); CleanupTransaction();
            GC.SuppressFinalize(this);
        }



        //private void TryReconnectDB()
        //{
        //    try
        //    {
        //        _logger.Info("Tentativo di reconnect al database...");

        //        if (_transactionStack.Count == 0)
        //        {
        //            IDatabase newDatabase = DatabaseFactory.ConnectDB(_databaseType, _connectionString);
        //            if (newDatabase != null)
        //            {
        //                _database?.Dispose();        // Chiudi la connessione attuale
        //                _database = newDatabase;
        //                _logger.Info("Reconnect completato con successo.");
        //            }
        //            else
        //            {
        //                _logger.Error("Errore durante il reconnect al database.[newDatabase==null]");
        //            }
        //        }
        //        else
        //        {
        //            _logger.Info("Transazione attiva: skippo tentativo di reconnect al database...");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Errore durante il reconnect al database.");
        //        // skip exception //throw;
        //    }
        //}


        //***************************************************************************************************************************************************
        //*** STATIC INTERNAL ULILS
        //***************************************************************************************************************************************************


        //genera timestamp
        internal static byte[] GenerateTimestamp()
        {
            byte[] timestamp = new byte[8];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(timestamp);
            }
            return timestamp;
        }





        //public

        public string BeginTransaction(string? transactionId, string transactionName = "")
        {
            if (String.IsNullOrEmpty(transactionName) || _transactionStack.Contains(transactionName)) transactionName = $"SAVEPOINT_{_transactionStack.Count}";
            else transactionName = transactionName = transactionName.Length <= 32 ? transactionName : transactionName.Substring(0, 32);
            if (_transactionStack.Count == 0)
            {
                _database.BeginTransaction(transactionName);
                _transactionId = Guid.NewGuid().ToString();
                _transactionTimeoutTimer = new Timer(TransactionTimeoutCallback, null, TransactionTimeoutSeconds * 1000, Timeout.Infinite);
            }
            else
            {
                if (_transactionId != transactionId) RollBackDefaulTransaction("BeginTransaction");
                _database.SavePointTransaction(transactionName);
            }
            _transactionStack.Push(transactionName);
            return _transactionId;
        }
        public void CommitTransaction(string transactionId, string transactionName = "")
        {
            if (String.IsNullOrEmpty(transactionName)) transactionName = $"SAVEPOINT_{_transactionStack.Count}";
            else transactionName = transactionName = transactionName.Length <= 32 ? transactionName : transactionName.Substring(0, 32);
            if (_transactionStack.Count == 0 || _transactionId != transactionId || _transactionStack.Peek() != transactionName) RollBackDefaulTransaction("CommitTransaction");

            _transactionStack.Pop();  //elimina savepoint in coda
            if (_transactionStack.Count == 0)
            {
                _database.CommitTransaction(transactionName);
                CleanupTransaction();
            }
        }
        public void RollbackTransaction(string transactionId, string transactionName = "")
        {
            if (String.IsNullOrEmpty(transactionName)) transactionName = $"SAVEPOINT_{_transactionStack.Count}";
            else transactionName = transactionName = transactionName.Length <= 32 ? transactionName : transactionName.Substring(0, 32);
            if (_transactionStack.Count == 0 || _transactionId != transactionId || _transactionStack.Peek() != transactionName) RollBackDefaulTransaction("RollbackTransaction");

            _transactionStack.Pop();   //elimina savepoint in coda
            if (_transactionStack.Count > 0)
            {
                _database.RollbackSavePoint(transactionName);
            }
            else
            {
                _database.RollbackTransaction(transactionName);
                CleanupTransaction();
            }
        }

        //private

        private void CleanupTransaction()
        {
            _transactionStack?.Clear(); _transactionId = null;
            _transactionTimeoutTimer?.Dispose(); _transactionTimeoutTimer = null;
        }
        private void RollBackDefaulTransaction(string action)
        {
            _database?.RollbackTransaction("Transaction_Default");
            CleanupTransaction();
            throw new DatabaseException(ERR_DB_BADTRAN, "{action} attempted for the wrong transaction.");
        }
        private void TransactionTimeoutCallback(object state)
        {
            _database?.RollbackTransaction("Transaction_Timeout");
            throw new DatabaseException(ERR_DB_TIMEOUT, "Transaction timeout reached.");
        }

        //***************************************************************************************************************************************************
        //*** QUERY - MANTAIN
        //***************************************************************************************************************************************************

        //public

        public DataTable ExecuteQuery(string sql, IDictionary<string, object> parameters, string? transactionId, int maxRecords, string options)
        {
            if (_transactionId != transactionId) RollBackDefaulTransaction("ExecuteQuery");
            IDbConnection connection = _database.NewConnection(); lock (_dumpLastSql) { _dumpLastSql = ""; }
            //--- Segnaleremo il completamento della query con questo handle
            ManualResetEventSlim done = null;
            if (EnableTraceTimeout) done = new ManualResetEventSlim(false);
            //---
            try
            {
                using (IDbCommand command = _database.NewCommand(sql, connection)) // la transazione viene passata nel NewCommand
                {
                    command.CommandTimeout = TimeoutSeconds;
                    string _dumpSql = AddParametersToCommand(command, parameters); lock (_dumpLastSql) { _dumpLastSql = _dumpSql; }
                    //--- Avvia audit cancellabile
                    if (EnableTraceTimeout) StartAuditMonitorIfStillRunning(connection, TimeoutSeconds, _auditBeforeTimeoutSeconds, done, _dumpSql, sql, parameters);
                    //---
                    DataTable result = _database.QueryReader(command, maxRecords); //eseguo senza retry
                    if (result.Rows.Count > maxRecords)
                    {
                        throw new InvalidOperationException($"Query returned more than the allowed {maxRecords} records.");
                    }
                    //--- Segnala completamento (audit non partirà)
                    if (EnableTraceTimeout) done.Set();
                    //---
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($@"DatabaseManager.ExecuteQuery: System.Exception: {ex.Message}");
                HandleException(ex, ERR_DB_ERROR, "Database operation failed.");
                throw; // Rethrow to ensure we do not swallow the exception
            }
            finally { _database.ReleaseConnection(connection); } // si chiude se non c'è transazione
        }
        //ExecuteQueryXdata: i campi della select sql devono essere nello stesso ordine di ModelXdata (es. SELECT Icode, Deleted, Timestamp, Cdate, Ctime, Cagent, Cunit, Mdate, Mtime, Magent, Munit, Home, Version, Inactive, Extatt, Mref, Seq, Descr, Fmt, Xdurl, Xdatum FROM MyTable)
        public Dictionary<object, ModelXdata> ExecuteQueryXdata(Dictionary<object, ModelXdata> dict, string sql, IDictionary<string, object> parameters, string? transactionId, int maxRecords, long maxBlobSize, string options)
        {
            if (_transactionId != transactionId) RollBackDefaulTransaction("ExecuteQueryXdata");
            IDbConnection connection = _database.NewConnection(); lock (_dumpLastSql) { _dumpLastSql = ""; }
            //--- Segnaleremo il completamento della query con questo handle
            ManualResetEventSlim done = null;
            if (EnableTraceTimeout) done = new ManualResetEventSlim(false);
            //---
            try
            {
                using (IDbCommand command = _database.NewCommand(sql, connection)) // la transazione viene passata nel NewCommand
                {
                    command.CommandTimeout = TimeoutSeconds;
                    string _dumpSql = AddParametersToCommand(command, parameters); lock (_dumpLastSql) { _dumpLastSql = _dumpSql; }
                    //--- Avvia audit cancellabile
                    if (EnableTraceTimeout) StartAuditMonitorIfStillRunning(connection, TimeoutSeconds, _auditBeforeTimeoutSeconds, done, _dumpSql, sql, parameters);
                    //---
                    //DataTable result = _database.QueryReader(command, maxRecords); //eseguo senza retry
                    //if (result.Rows.Count > maxRecords)
                    //{
                    //    throw new InvalidOperationException($"Query returned more than the allowed {maxRecords} records.");
                    //}
                    using var reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                    if (dict == null) { dict = new Dictionary<object, ModelXdata>(); }
                    int i = 0;
                    while (reader.Read())
                    {
                        if (i++ >= maxRecords) break;
                        //  1. Campi NON blob
                        object? icode = reader.IsDBNull(0) ? null : reader.GetValue(0); if (icode is string icodeStr) icode = icodeStr.TrimEnd(); // trim solo per stringhe, non per altri tipi (es. numerici o guid)
                        string? deleted = reader.IsDBNull(1) ? null : reader.GetString(1)?.TrimEnd() ?? " "; if (deleted == null || deleted != "Y") deleted = "N"; 
                        byte[]? timestamp = new byte[8]; long timestampBytesRead = reader.GetBytes(2, 0, timestamp, 0, timestamp.Length);
                        string? cdate = reader.IsDBNull(3) ? null : reader.GetString(3) ?? "    /  /  ";
                        string? ctime = reader.IsDBNull(4) ? null : reader.GetString(4) ?? "  :  :  ";
                        string? cagent = reader.IsDBNull(5) ? null : reader.GetString(5)?.TrimEnd() ?? "";
                        string? cunit = reader.IsDBNull(6) ? null : reader.GetString(6)?.TrimEnd() ?? "";
                        string? mdate = reader.IsDBNull(7) ? null : reader.GetString(7) ?? "    /  /  ";
                        string? mtime = reader.IsDBNull(8) ? null : reader.GetString(8) ?? "  :  :  ";
                        string? magent = reader.IsDBNull(9) ? null : reader.GetString(9)?.TrimEnd() ?? "";
                        string? munit = reader.IsDBNull(10) ? null : reader.GetString(10)?.TrimEnd() ?? "";
                        string? home = reader.IsDBNull(11) ? null : reader.GetString(11)?.TrimEnd() ?? "";
                        string? version = reader.IsDBNull(12) ? null : reader.GetString(12)?.TrimEnd() ?? "";
                        string? inactive = reader.IsDBNull(13) ? null : reader.GetString(13)?.TrimEnd() ?? " ";
                        string? extatt = reader.IsDBNull(14) ? null : reader.GetString(14)?.TrimEnd() ?? "";
                        object? mref = reader.IsDBNull(15) ? null : reader.GetValue(15); if (mref is string mrefStr) mref = mrefStr.TrimEnd(); // trim solo per stringhe, non per altri tipi (es. numerici o guid)
                        short? seq = reader.IsDBNull(16) ? null : reader.GetInt16(16);
                        string? descr = reader.IsDBNull(17) ? null : reader.GetString(17)?.TrimEnd() ?? "";
                        string? fmt = reader.IsDBNull(18) ? null : reader.GetString(18)?.TrimEnd() ?? "";
                        string? xdurl = reader.IsDBNull(19) ? null : reader.GetString(19)?.TrimEnd() ?? "";

                        long blobSize = -1; byte[] xdatum = null; string mimeOfXdatum = "";
                        if (!reader.IsDBNull(20))
                        {
                            //  2. Lettura parziale del blob (es. primi 16 byte)
                            blobSize = reader.GetBytes(20, 0, null, 0, 0); // ottieni la dimensione totale del blob
                            long bytesToReadLong = (maxBlobSize == -1) ? blobSize : (maxBlobSize == 0) ? (long)16: Math.Min(blobSize, maxBlobSize); //Decidi quanti byte leggere davvero
                            if (bytesToReadLong > int.MaxValue) throw new InvalidOperationException($"Blob troppo grande ({bytesToReadLong} bytes) per essere caricato in memoria."); // Protezione fondamentale (array size)
                            if (bytesToReadLong > MaxFileLengthBytes) throw new InvalidOperationException($"Blob ({bytesToReadLong} bytes) supera il limite massimo MaxFileLengthBytes ({MaxFileLengthBytes})."); // Protezione fondamentale (array size)
                            int bytesToRead = (int)bytesToReadLong; xdatum = new byte[bytesToRead]; //  Alloca SOLO lo spazio necessario
                            long bytesRead = reader.GetBytes(20, 0, xdatum, 0, xdatum.Length);  // Leggi il blob (tutto o parziale)
                            if (bytesRead != xdatum.Length) throw new InvalidOperationException("Lettura blob incompleta."); // 6) (opzionale) verifica
                            //  3. Scrittura campi derivati
                            mimeOfXdatum = UtilHelper.DetectMime(xdatum);
                            if (maxBlobSize == 0) { xdatum = null; }    // se maxBlobSize=0, allora non voglio caricare il blob, ma solo sapere se esiste e qual è il suo tipo (mime), quindi lo setto a null per risparmiare memoria
                        }


                        //  4. Controlli
                        if (icode == null) throw new InvalidOperationException($"Xdata.Icode non può essere null.");
                        if (mref == null) throw new InvalidOperationException($"Xdata.Mref non può essere null.");
                        if (fmt == null) throw new InvalidOperationException($"Xdata.Fmt non può essere null.");
                        dict[icode] = new ModelXdata
                        {
                            Icode = icode,
                            Deleted = deleted,
                            Timestamp = timestamp,
                            Cdate = cdate,
                            Ctime = ctime,
                            Cagent = cagent,
                            Cunit = cunit,
                            Mdate = mdate,
                            Mtime = mtime,
                            Magent = magent,
                            Munit = munit,
                            Home = home,
                            Version = version,
                            Inactive = inactive,
                            Extatt = extatt,
                            Mref = mref,
                            Seq = seq,
                            Descr = descr,
                            Fmt = fmt,
                            Xdurl = xdurl,
                            Xdatum = xdatum,
                            _mimeXdatum = mimeOfXdatum,
                            _sizeXdatum = blobSize,
                        };

                        // ora puoi passare alla riga successiva
                    }

                    //--- Segnala completamento (audit non partirà)
                    if (EnableTraceTimeout) done.Set();
                    //---
                    return dict;
                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine($@"DatabaseManager.ExecuteQueryXdata: System.Exception: {ex.Message}");
                HandleException(ex, ERR_DB_ERROR, "Database operation failed.");
                throw; // Rethrow to ensure we do not swallow the exception
            }
            finally { _database.ReleaseConnection(connection); } // si chiude se non c'è transazione
        }





        internal int ExecuteNonQuery(string sql, IDictionary<string, object> parameters, string transactionId)
        {
            if (_transactionId != transactionId) RollBackDefaulTransaction("ExecuteNonQuery");
            IDbConnection connection = _database.NewConnection(); lock (_dumpLastSql) { _dumpLastSql = ""; }
            //--- Segnaleremo il completamento della query con questo handle
            ManualResetEventSlim done = null;
            if (EnableTraceTimeout) done = new ManualResetEventSlim(false);
            //---
            try
            {
                using (IDbCommand command = _database.NewCommand(sql, connection)) // la transazione viene passate nel NewCommand
                {
                    command.CommandTimeout = TimeoutSeconds;
                    string _dumpSql = AddParametersToCommand(command, parameters); lock (_dumpLastSql) { _dumpLastSql = _dumpSql; }
                    //--- Avvia audit cancellabile
                    if (EnableTraceTimeout) StartAuditMonitorIfStillRunning(connection, TimeoutSeconds, _auditBeforeTimeoutSeconds, done, _dumpSql, sql, parameters);
                    //---
                    int result = command.ExecuteNonQuery();
                    //--- Segnala completamento (audit non partirà)
                    if (EnableTraceTimeout) done.Set();
                    //---
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($@"DatabaseManager.ExecuteNonQuery: System.Exception: {ex.Message}");
                HandleException(ex, ERR_DB_ERROR, "Database operation failed.");
                throw; // Rethrow to ensure we do not swallow the exception
            }
            finally { _database.ReleaseConnection(connection); } // la connessione viene chiusa se non c'è transazione
        }
        //internal async Task<int> ExecuteNonQueryAsync(string sql, IDictionary<string, object> parameters, string transactionId)
        //{
        //    if (_transactionId != transactionId) RollBackDefaulTransaction("ExecuteNonQueryAsync");
        //    IDbConnection connection = _database.NewConnection(); lock (_dumpLastSql) { _dumpLastSql = ""; }
        //    //--- Segnaleremo il completamento della query con questo handle
        //    ManualResetEventSlim done = null;
        //    if (EnableTraceTimeout) done = new ManualResetEventSlim(false);
        //    //---
        //    try
        //    {
        //        using (IDbCommand command = _database.NewCommand(sql, connection)) // la transazione viene passate nel NewCommand
        //        {
        //            command.CommandTimeout = TimeoutSeconds;
        //            string _dumpSql = AddParametersToCommand(command, parameters); lock (_dumpLastSql) { _dumpLastSql = _dumpSql; }
        //            //--- Avvia audit cancellabile
        //            if (EnableTraceTimeout) StartAuditMonitorIfStillRunning(connection, TimeoutSeconds, _auditBeforeTimeoutSeconds, done, _dumpSql, sql, parameters);
        //            //---


        //            //----------------------------------------------------------------------------
        //            int result = 0; //int result = command.ExecuteNonQuery();
        //            // punto cruciale: esecuzione streaming
        //            if (command is DbCommand dbCommand)
        //            {
        //                // provider moderni
        //                result = await dbCommand.ExecuteNonQueryAsync();
        //            }
        //            else
        //            {
        //                // fallback sincrono
        //                result = command.ExecuteNonQuery();
        //            }
        //            //----------------------------------------------------------------------------


        //            //--- Segnala completamento (audit non partirà)
        //            if (EnableTraceTimeout) done.Set();
        //            //---
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.WriteLine($@"DatabaseManager.ExecuteNonQuery: System.Exception: {ex.Message}");
        //        HandleException(ex, ERR_DB_ERROR, "Database operation failed.");
        //        throw; // Rethrow to ensure we do not swallow the exception
        //    }
        //    finally { _database.ReleaseConnection(connection); } // la connessione viene chiusa se non c'è transazione
        //}

        public bool RecordExists(string tableName, string keyField, object keyValue, string transactionId)
        {
            if (_transactionId != transactionId) RollBackDefaulTransaction("RecordExists");
            string sql = $"SELECT COUNT(1) FROM {tableName} WHERE {keyField} = @keyValue";
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "keyValue", keyValue } };
            IDbConnection connection = _database.NewConnection(); lock (_dumpLastSql) { _dumpLastSql = ""; }
            //--- Segnaleremo il completamento della query con questo handle
            ManualResetEventSlim done = null;
            if (EnableTraceTimeout) done = new ManualResetEventSlim(false);
            //---
            try
            {
                using (IDbCommand command = _database.NewCommand(sql, connection)) // la transazione viene passate nel NewCommand
                {
                    command.CommandTimeout = TimeoutSeconds;
                    string _dumpSql = AddParametersToCommand(command, parameters); lock (_dumpLastSql) { _dumpLastSql = _dumpSql; }
                    //--- Avvia audit cancellabile
                    if (EnableTraceTimeout) StartAuditMonitorIfStillRunning(connection, TimeoutSeconds, _auditBeforeTimeoutSeconds, done, _dumpSql, sql, parameters);
                    //---
                    bool result = (int)command.ExecuteScalar() > 0;
                    //--- Segnala completamento (audit non partirà)
                    if (EnableTraceTimeout) done.Set();
                    //---
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($@"DatabaseManager.RecordExists: System.Exception: {ex.Message}");
                HandleException(ex, ERR_DB_ERROR, "Database operation failed.");
                throw; // Rethrow to ensure we do not swallow the exception
            }
            finally { _database.ReleaseConnection(connection); } // la connessione viene chiusa se non c'è transazione
        }
        public byte[] ReadBlob(string tableName, string keyField, object keyValue, string blobField, int pageNumber, string transactionId)
        {
            if (_transactionId != transactionId) RollBackDefaulTransaction("ReadBlob");
            int offset = pageNumber * PageSize; string sql = "";
            if (pageNumber < 0) { sql = $"SELECT {blobField} FROM {tableName} WHERE {keyField} = @keyValue"; }
            else { sql = $"SELECT SUBSTRING({blobField}, {offset + 1}, {PageSize}) FROM {tableName} WHERE {keyField} = @keyValue"; }
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "keyValue", keyValue } };
            IDbConnection connection = _database.NewConnection(); lock (_dumpLastSql) { _dumpLastSql = ""; }
            //--- Segnaleremo il completamento della query con questo handle
            ManualResetEventSlim done = null;
            if (EnableTraceTimeout) done = new ManualResetEventSlim(false);
            //---
            try
            {
                using (IDbCommand command = _database.NewCommand(sql, connection)) // la transazione viene passate nel NewCommand
                {
                    command.CommandTimeout = TimeoutSeconds;
                    string _dumpSql = AddParametersToCommand(command, parameters); lock (_dumpLastSql) { _dumpLastSql = _dumpSql; }
                    //--- Avvia audit cancellabile
                    if (EnableTraceTimeout) StartAuditMonitorIfStillRunning(connection, TimeoutSeconds, _auditBeforeTimeoutSeconds, done, _dumpSql, sql, parameters);
                    //---
                    byte[] result = command.ExecuteScalar() as byte[];
                    //--- Segnala completamento (audit non partirà)
                    if (EnableTraceTimeout) done.Set();
                    //---
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($@"DatabaseManager.ReadBlob: System.Exception: {ex.Message}");
                HandleException(ex, ERR_DB_ERROR, "Database operation failed.");
                throw; // Rethrow to ensure we do not swallow the exception
            }
            finally { _database.ReleaseConnection(connection); } // la connessione viene chiusa se non c'è transazione
        }
        public void WriteBlob(string tableName, string keyField, object keyValue, string blobField, byte[] data, int pageNumber, string transactionId)
        {
            if (_transactionId != transactionId) RollBackDefaulTransaction("WriteBlob");
            int offset = pageNumber * PageSize;
            int length = Math.Min(PageSize, data.Length - offset);
            string sql = $"UPDATE {tableName} SET {blobField}.WRITE(@data, {offset}, {length}) WHERE {keyField} = @keyValue";
            IDictionary<string, object> parameters = new Dictionary<string, object> { { "data", data.Skip(offset).Take(length).ToArray() }, { "keyValue", keyValue } };
            IDbConnection connection = _database.NewConnection(); lock (_dumpLastSql) { _dumpLastSql = ""; }
            //--- Segnaleremo il completamento della query con questo handle
            ManualResetEventSlim done = null;
            if (EnableTraceTimeout) done = new ManualResetEventSlim(false);
            //---
            try
            {
                using (IDbCommand command = _database.NewCommand(sql, connection)) // la transazione viene passate nel NewCommand
                {
                    command.CommandTimeout = TimeoutSeconds;
                    string _dumpSql = AddParametersToCommand(command, parameters); lock (_dumpLastSql) { _dumpLastSql = _dumpSql; }
                    //--- Avvia audit cancellabile
                    if (EnableTraceTimeout) StartAuditMonitorIfStillRunning(connection, TimeoutSeconds, _auditBeforeTimeoutSeconds, done, _dumpSql, sql, parameters);
                    //---
                    int affectedRows = command.ExecuteNonQuery();
                    //--- Segnala completamento (audit non partirà)
                    if (EnableTraceTimeout) done.Set();
                    //---
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($@"DatabaseManager.WriteBlob: System.Exception: {ex.Message}");
                HandleException(ex, ERR_DB_ERROR, "Database operation failed.");
                throw; // Rethrow to ensure we do not swallow the exception
            }
            finally { _database.ReleaseConnection(connection); } // la connessione viene chiusa se non c'è transazione
        }

        public void DeleteRecord(string tableName, string keyField, IDictionary<string, object> fields, string transactionId)
        {
            if (_transactionId != transactionId) RollBackDefaulTransaction("DeleteRecord");
            string sql = $"DELETE FROM {tableName} WHERE {keyField} = @keyField";
            var parameters = new Dictionary<string, object>
                    {
                        { keyField, fields[keyField] }
                    };
            int affectedRows = ExecuteNonQuery(sql, parameters, transactionId);
        }

        //private

        //---
        private string AddParametersToCommand(IDbCommand command, IDictionary<string, object> parameters)
        {
            string dumpSql = command.CommandText;
            if (parameters == null)
            {
                if (EnableTrace)
                {
                    _logger.Trace($"Executing SQL: {command.CommandText} with parameters: null");
                }
            }
            foreach (var param in parameters)
            {
                IDbDataParameter parameter = command.CreateParameter(); //command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                parameter.ParameterName = $"@{param.Key}"; parameter.Value = param.Value ?? DBNull.Value;
                if (parameter.Value is DateOnly) parameter.DbType = DbType.Date;
                else if (parameter.Value is TimeOnly || parameter.Value is TimeSpan) parameter.DbType = DbType.Time;
                else if (parameter.Value is DateTime) parameter.DbType = DbType.DateTime;
                else if (parameter.Value is byte[]) parameter.DbType = DbType.Binary;
                else if (parameter.Value is Stream)
                {
                    parameter.DbType = DbType.Binary;

                    // Cast a SqlParameter per usare SqlDbType direttamente
                    if (parameter is System.Data.SqlClient.SqlParameter sqlParam)
                    {
                        sqlParam.SqlDbType = System.Data.SqlDbType.VarBinary; // oppure .Image se colonna legacy
                    }

                }
                command.Parameters.Add(parameter);
                dumpSql = ReplaceParameter(dumpSql, parameter);
            }
            if (EnableTrace)
            {
                _logger.Trace($"Executing SQL: {command.CommandText} with parameters: {string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}"))}");
            }
            return dumpSql;
        }
        private string ReplaceParameter(string sql, IDbDataParameter parameter)
        {
            string paramValue = "";
            if (parameter.Value == null) { paramValue = "NULL"; }
            else if (parameter.Value is System.DBNull) { paramValue = "NULL"; }
            else if (parameter.Value is string) { paramValue = $"'{parameter.Value.ToString().Replace("'", "''")}'"; }
            else if (parameter.Value is char) { paramValue = $"'{parameter.Value.ToString().Replace("'", "''")}'"; }
            else if (parameter.Value is DateTime) { paramValue = $"'{((DateTime)parameter.Value).ToString("yyyy-MM-dd HH:mm:ss")}'"; }
            else if (parameter.Value is DateTimeOffset) { paramValue = $"'{((DateTimeOffset)parameter.Value).ToString("yyyy-MM-dd HH:mm:ss zzz")}'"; }
            else if (parameter.Value is DateOnly) { paramValue = $"'{((DateOnly)parameter.Value).ToString("yyyy-MM-dd")}'"; }
            else if (parameter.Value is TimeOnly) { paramValue = $"'{((TimeOnly)parameter.Value).ToString("HH:mm:ss")}'"; }
            else if (parameter.Value is TimeSpan) { paramValue = $"'{parameter.Value.ToString()}'"; }
            else if (parameter.Value is bool) { paramValue = (bool)parameter.Value ? "1" : "0"; }
            else if (parameter.Value is int || parameter.Value is long || parameter.Value is float
                            || parameter.Value is double || parameter.Value is decimal) { paramValue = parameter.Value.ToString(); }
            else if (parameter.Value is byte[] b)
            {
                paramValue = "0x"; for (int i = 0; i < b.Length; i++) { if (i >= 8) { paramValue += "..."; break; } paramValue += b[i].ToString("X2"); }
            }
            else if (parameter.Value is char[] c)
            {
                paramValue = ""; for (int i = 0; i < c.Length; i++) { if (i >= 8) { paramValue += "..."; break; } paramValue += c[i].ToString(); }
            }
            else { paramValue = "#!#_NOT_PRINTABLE_#!#"; }
            return Regex.Replace(sql, $@"{Regex.Escape(parameter.ParameterName)}(?!\d)", paramValue, RegexOptions.None); //return sql.Replace(parameter.ParameterName, paramValue);
        }

        //---
        private void HandleException(Exception ex, int errorCode, string message)
        {
            // --------------------------------------------------------------------
            // QUI E' POSSIBILE INSERIRE LE LOGICHE DI GESTIONE DELL'ERRORE
            // --------------------------------------------------------------------

            //DatabaseException(<<codice numerico errore>>, <<messaggio errore che verrà visualizzato a video>>, <<eccezione lanciata da DBMS>>);

            //////_logger.Error(ex, $"{message} ErrorCode: {errorCode}");
            //////if (!_database.HandleException(ex)) throw new DatabaseException(ERR_DB_ERROR, "{message} ({errorCode})", ex);

            try { _database.HandleException(ex); }
            catch (DatabaseException ex1)
            {
                string dbmsMessage = ex?.Message ?? "";
                _logger.Error(ex, $"{ex1.Message} ErrorCode: {ex1.ErrorCode} HResult: {ex.HResult} \nDbmsMess: {dbmsMessage} \nSQL: {_dumpLastSql}\n\n");
                throw; // Rethrow to ensure we do not swallow the exception
            }
            throw new DatabaseException(ERR_DB_ERROR, $"{message} ({errorCode})", ex);
        }


        //***************************************************************************************************************************************************
        //*** IMPORT-EXPORT CSV
        //***************************************************************************************************************************************************

        //public

        public void ExportTableToCsv(string tableName, string filePath, string whereClause = null, int chunkSize = 10000)
        {
            int offset = 0;
            bool hasMoreData = true;
            int fileCount = 1;
            string baseFilePath = filePath;

            while (hasMoreData)
            {
                string sql = $"SELECT * FROM {tableName} {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)} ORDER BY (SELECT NULL) OFFSET {offset} ROWS FETCH NEXT {chunkSize} ROWS ONLY";
                var dataTable = ExecuteQuery(sql, new Dictionary<string, object>(), null, chunkSize, "");
                string currentFilePath = fileCount == 1 ? filePath : $"{baseFilePath}_{fileCount}.csv";

                WriteDataTableToCsv(dataTable, currentFilePath);

                if (new FileInfo(currentFilePath).Length >= MaxFileLengthBytes)
                {
                    fileCount++;
                }

                hasMoreData = dataTable.Rows.Count == chunkSize;
                offset += chunkSize;
            }
        }
        public void ImportCsvToTable(string tableName, string filePath)
        {
            int fileCount = 1;
            string currentFilePath = filePath;
            bool moreFilesToProcess = true;

            while (moreFilesToProcess)
            {
                var dataTable = new DataTable();
                moreFilesToProcess = LoadCsvChunkIntoDataTable(currentFilePath, ref dataTable);

                while (dataTable.Rows.Count > 0)
                {
                    BulkInsertDataTable(tableName, dataTable);
                    moreFilesToProcess = LoadCsvChunkIntoDataTable(currentFilePath, ref dataTable);
                }

                fileCount++;
                currentFilePath = $"{filePath}_{fileCount}.csv";

                moreFilesToProcess = File.Exists(currentFilePath);
            }
        }

        //private

        private void BulkInsertDataTable(string tableName, DataTable dataTable)
        {
            try
            {
                string[] columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                string insertCols = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ";
                StringBuilder sql = new StringBuilder();
                var parameters = new Dictionary<string, object>();
                if (1 == 0)   // eseguo n comandi di insert
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string insertValues = $"({string.Join(",", row.ItemArray)})";
                        sql.Append(insertCols).Append(insertValues).Append("; \n");
                    }
                }
                else       // eseguo un solo comando di insert eg: INSERT INTO items (embedding) VALUES ('[1,2,3]'), ('[4,5,6]');
                {
                    sql.Append(insertCols);
                    for (int r = 0; r < dataTable.Rows.Count; r++)
                    {
                        var row = dataTable.Rows[r];
                        if (r != 0) sql.Append(',');
                        sql.Append('(');
                        for (int c = 0; c < columnNames.Length; c++)
                        {
                            if (c != 0) sql.Append(',');
                            sql.Append($"@{columnNames[c]}__{r + 1}");
                            parameters[$"@{columnNames[c]}__{r + 1}"] = row[columnNames[c]];  //parameters[$"@{columnNames[c]}__{r+1}"] = EncodeSpecialFields(row[columnNames[c]]);
                        }
                        sql.Append(')');
                    }
                    sql.Append("; \n");
                }
                int affectedRows = ExecuteNonQuery(sql.ToString(), parameters, null);
                if (affectedRows != dataTable.Rows.Count) throw new DatabaseException(ERR_DB_ERROR, " {dataTable.Rows-affectedRows} records non inseriti.", null);
            }
            catch (Exception ex)
            {
                HandleException(ex, ERR_DB_BADDATA, "Failed to bulk insert data.");
            }
        }

        private bool LoadCsvChunkIntoDataTable(string filePath, ref DataTable dataTable)
        {
            const int batchSize = 5000; // Carica i dati in blocchi di 5000 righe
            using (StreamReader reader = new StreamReader(filePath))
            using (CsvReader csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                //csv.Configuration.HasHeaderRecord = dataTable.Columns.Count == 0;

                if (dataTable.Columns.Count == 0)  //if (csv.Configuration.HasHeaderRecord)
                {
                    foreach (string header in csv.Context.Reader.HeaderRecord)
                    {
                        dataTable.Columns.Add(header);
                    }
                }

                var records = csv.GetRecords<dynamic>().Take(batchSize);
                foreach (var record in records)
                {
                    var row = dataTable.NewRow();
                    foreach (var field in record)
                    {
                        row[field.Key] = field.Value;
                    }
                    dataTable.Rows.Add(row);
                }
            }

            return dataTable.Rows.Count == batchSize;
        }
        private void WriteDataTableToCsv(DataTable dataTable, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            using (CsvWriter csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                if (new FileInfo(filePath).Length == 0) // Se il file è vuoto, scrivi l'intestazione
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }
                    csv.NextRecord();
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        csv.WriteField(row[column]);
                    }
                    csv.NextRecord();
                }
            }
        }

        //***************************************************************************************************************************************************
        //*** STREAMING
        //***************************************************************************************************************************************************

        //----------------------------------------
        // READ BLOB IN STREAMING (per evitare di caricare tutto il blob in memoria, utile per blob di grandi dimensioni)
        //----------------------------------------

        public DogManager.BlobStreamResult OpenBlobStream(string tableName, string keyField, object keyValue, string blobField, long startOffset)
        {
            if (_transactionId != null) throw new InvalidOperationException($"OpenBlobStream: Lettura Blob in streaming durante transazione ({_transactionId}).");
            string sql = $"SELECT {blobField} FROM {tableName} WHERE {keyField} = @keyValue";
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "keyValue", keyValue } };
            IDbConnection connection = _database.NewConnection(); lock (_dumpLastSql) { _dumpLastSql = ""; }
            //--- Segnaleremo il completamento della query con questo handle
            ManualResetEventSlim done = null;
            if (EnableTraceTimeout) done = new ManualResetEventSlim(false);
            //---
            try
            {
                IDbCommand command = _database.NewCommand(sql, connection); // la transazione viene passate nel NewCommand
                command.CommandTimeout = 0;  // <-- 0 = nessun timeout lato ADO.NET per streaming
                                                //     il timeout lato applicazione va gestito separatamente
                string _dumpSql = AddParametersToCommand(command, parameters); lock (_dumpLastSql) { _dumpLastSql = _dumpSql; }
                //--- Avvia audit cancellabile
                if (EnableTraceTimeout) StartAuditMonitorIfStillRunning(connection, TimeoutSeconds, _auditBeforeTimeoutSeconds, done, _dumpSql, sql, parameters);
                //---
                //byte[] result = command.ExecuteScalar() as byte[];
                IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection); // la connessione viene chiusa a fine lettura streaming
                if (!reader.Read()) {
                    reader.Dispose(); command.Dispose(); connection.Dispose();
                    throw new InvalidOperationException("Blob non trovato"); 
                }
                //DetectMimeFromHeader


                //const int HEADER_SIZE = 16;
                //byte[] header = new byte[HEADER_SIZE];
                //reader.GetBytes(0, 0, header, 0, header.Length);
                //string blobMime = UtilHelper.DetectMime(header);

                string blobMime = "";

                //GetBlobSize
                long blobSize = reader.GetBytes(0, 0, null, 0, 0);
                // STREAM UNIVERSALE (fallback GetBytes)
                Stream blobStream = null; byte[] blobBytes = null;

                const int DOCUMENT_SIZE = 1024 * 1024;  // se blobSize < 1 Mb carico il documento tutto insieme, altrimenti vado in streaming
                if (blobSize < DOCUMENT_SIZE)
                {
                    blobBytes = new byte[blobSize]; reader.GetBytes(0, 0, blobBytes, 0, blobBytes.Length);
                    blobMime = UtilHelper.DetectMime(blobBytes);    //DetectMimeFromHeader
                    reader.Dispose();
                    command.Dispose();
                    if (EnableTraceTimeout) done.Set();  // <-- corretto: qui la lettura è davvero finita
                }
                else
                {
                    //DetectMimeFromHeader
                    const int HEADER_SIZE = 16;
                    byte[] header = new byte[HEADER_SIZE];
                    reader.GetBytes(0, 0, header, 0, header.Length);
                    blobMime = UtilHelper.DetectMime(header);
                    // get Stream
                    // passa reader, command e done al Task -> li chiude lui nel finally
                    blobStream = CreateUniversalBlobStream(reader, command, 0, header, startOffset,
                                    EnableTraceTimeout ? done : null); // passa done al Task
                                                                        // <-- NON chiamare done.Set() qui: lo fa il Task quando finisce
                                                                        // <-- NON fare Dispose di command/reader qui: lo fa il Task nel finally
                }

                //---
                return new DogManager.BlobStreamResult
                {
                    Stream = blobStream,
                    Bytes = blobBytes,
                    ContentType = blobMime,
                    Length = blobSize
                };
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($@"DatabaseManager.OpenBlobStream: System.Exception: {ex.Message}");
                HandleException(ex, ERR_DB_ERROR, "Database operation failed.");
                throw; // Rethrow to ensure we do not swallow the exception
            }
        }
        //private static Stream CreateUniversalBlobStream(IDataReader reader, int blobOrdinal, byte[] header, long startOffset)
        //{
        //    var stream = new BlockingStream();
        //    Task.Run(() =>
        //    {
        //        try
        //        {
        //            // -------------------------------------------------
        //            // 1) Restituisci SOLO la parte utile dell'header
        //            // -------------------------------------------------
        //            if (startOffset < header.Length)
        //            {
        //                int headerStart = (int)startOffset;
        //                int headerCount = header.Length - headerStart;

        //                stream.Write(header, headerStart, headerCount);
        //            }

        //            // -------------------------------------------------
        //            // 2) Continua dal DB al primo byte NON coperto dall'header
        //            // -------------------------------------------------
        //            const int BUFFER_SIZE = 81920; // 80 KB
        //            byte[] buffer = new byte[BUFFER_SIZE];

        //            long offset = Math.Max(startOffset, header.Length);
        //            long read;
        //            while ((read = reader.GetBytes(blobOrdinal, offset, buffer, 0, buffer.Length)) > 0)
        //            {
        //                stream.Write(buffer, 0, (int)read);
        //                offset += read;
        //            }
        //        }
        //        finally { stream.Complete(); }
        //    });
        //    return stream;
        //}
        //--- FIX 2 + 3: CreateUniversalBlobStream -- riceve command e done --------
        private static Stream CreateUniversalBlobStream(
            IDataReader reader, IDbCommand command,
            int blobOrdinal, byte[] header, long startOffset,
            ManualResetEventSlim? done)          // <-- nuovo parametro
        {
            var stream = new BlockingStream();
            Task.Run(() =>
            {
                try
                {
                    // -------------------------------------------------
                    // 1) Restituisci SOLO la parte utile dell'header
                    // -------------------------------------------------
                    if (startOffset < header.Length)
                    {
                        int headerStart = (int)startOffset;
                        stream.Write(header, headerStart, header.Length - headerStart);
                    }
                    // -------------------------------------------------
                    // 2) Continua dal DB al primo byte NON coperto dall'header
                    // -------------------------------------------------
                    const int BUFFER_SIZE = 81920;
                    byte[] buffer = new byte[BUFFER_SIZE];
                    long offset = Math.Max(startOffset, header.Length);
                    long read;
                    while ((read = reader.GetBytes(blobOrdinal, offset, buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, (int)read);
                        offset += read;
                    }
                }
                finally
                {
                    stream.Complete();
                    done?.Set();        // <-- segnala audit solo quando lo streaming è davvero finito
                    reader.Dispose();   // <-- chiude reader e connessione (CloseConnection)
                    command.Dispose();  // <-- chiude il command solo dopo la lettura completa
                }
            });
            return stream;
        }
        private sealed class BlockingStream : Stream
        {
            private readonly BlockingCollection<byte[]> _queue = new BlockingCollection<byte[]>();
            private byte[]? _current; private int _offset;
            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override void Write(byte[] buffer, int offset, int count)
            {
                var chunk = new byte[count];
                Buffer.BlockCopy(buffer, offset, chunk, 0, count);
                _queue.Add(chunk);
            }

            public void Complete() => _queue.CompleteAdding();

            //public override int Read(byte[] buffer, int offset, int count)
            //{
            //    if (_current == null || _offset >= _current.Length)
            //    {
            //        if (!_queue.TryTake(out _current)) return 0;
            //        _offset = 0;
            //    }
            //    int bytes = Math.Min(count, _current.Length - _offset);
            //    Buffer.BlockCopy(_current, _offset, buffer, offset, bytes);
            //    _offset += bytes;
            //    return bytes;
            //}
            // --- FIX 1: BlockingStream.Read -- TryTake bloccante --------------------------------
            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_current == null || _offset >= _current.Length)
                {
                    // PRIMA: if (!_queue.TryTake(out _current)) return 0;   non bloccante = EOF prematuro
                    // DOPO:
                    if (!_queue.TryTake(out _current, Timeout.Infinite)) return 0; // bloccante fino a Complete()
                    _offset = 0;
                }
                int bytes = Math.Min(count, _current.Length - _offset);
                Buffer.BlockCopy(_current, _offset, buffer, offset, bytes);
                _offset += bytes;
                return bytes;
            }
            public override void Flush() { }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
        }


        //----------------------------------------
        // WRITE BLOB IN STREAMING (per evitare di caricare tutto il blob in memoria, utile per blob di grandi dimensioni)
        //----------------------------------------







        //***************************************************************************************************************************************************
        //*** MANTAIN
        //***************************************************************************************************************************************************


        ////////public void MantainRecord(char action, string tableName, string keyField, string timestampField, string deleteField, IDictionary<string, object> fields, string options, string transactionId)
        ////////{
        ////////    int recNum = 1;
        ////////    VerifyTransactionId("MantainRecord", transactionId);
        ////////    string sql = SqlMantain(recNum, action, tableName, keyField, timestampField, deleteField, ref fields, options);
        ////////    var parameters = ParametersMantain(recNum, fields, options);

        ////////    int affectedRows = ExecuteNonQuery(sql, parameters, transactionId);
        ////////    //if (affectedRows != recNum) throw new DatabaseException(ERR_DB_TIMESTAMP, "Timestamp non valido o errore in insert/update.", null);
        ////////    if (affectedRows != 1)
        ////////    {
        ////////        if (action == 'A') throw new DatabaseException(ERR_DB_ERROR, "Record non inserito.", null);
        ////////        else throw new DatabaseException(ERR_DB_TIMESTAMP, "Timestamp non valido.", null);
        ////////    }
        ////////}
        ////////private void VerifyTransactionId(string funcName, string transactionId)
        ////////{
        ////////    if (_transactionId != transactionId) RollBackDefaulTransaction(funcName);
        ////////}
        ////////private string SqlMantain(int recNum, char action, string tableName, string keyField, string timestampField, string deleteField, ref IDictionary<string, object> fields, string options)
        ////////{
        ////////    string sql;

        ////////    if (!("AMD").Contains(action)) throw new DatabaseException(ERR_BAD_INPUT, "Valore azione errato.", null);
        ////////    if (string.IsNullOrEmpty(tableName)) throw new DatabaseException(ERR_NO_INPUT, "Nome tabella non presente.", null);
        ////////    if (!fields.ContainsKey(keyField)) throw new DatabaseException(ERR_NO_INPUT, "Identificativo univoco non presente.", null);
        ////////    if (String.IsNullOrEmpty((string)fields[keyField])) throw new DatabaseException(ERR_BAD_IDEN, "Identificativo univoco vuoto.", null);

        ////////    if (action == 'A')  //Add
        ////////    {
        ////////        sql = $"INSERT INTO {tableName} ({string.Join(", ", fields.Keys)}) VALUES ({string.Join(", ", fields.Keys.Select(k => "@{k}__{recNum}"))})";
        ////////    }
        ////////    else if (action == 'M')  //Modify
        ////////    {
        ////////        if (!fields.ContainsKey(timestampField)) throw new DatabaseException(ERR_NO_INPUT, "Timestamp non presente.", null);
        ////////        if (fields.ContainsKey($"OldTimestamp")) throw new DatabaseException(ERR_BAD_INPUT, "Il campo OldTimestamp non è consentito.", null);

        ////////        sql = $"UPDATE {tableName} SET {string.Join(", ", fields.Where(f => f.Key != keyField).Select(f => $"{f.Key} = @{f.Key}__{recNum}"))} WHERE {keyField} = @{keyField}__{recNum} and {timestampField} = @OldTimestamp__{recNum}";
        ////////        byte[] oldTimestamp = (byte[])fields[timestampField];  // salvo valore vecchi timestamp
        ////////        fields[timestampField] = GenerateTimestamp(); // genero valore nuovo timestamp
        ////////        fields[$"OldTimestamp"] = oldTimestamp;  // aggiungo il parametro relativo al vecchio timestamp
        ////////    }
        ////////    else if (action == 'D')  //Delete ==> Delete logico non fisico.
        ////////                             //La cancellazione logica consente di replicare in modo asincrono l'azione su altri DB.
        ////////                             //Per non vincolare l'integrità referenziale devo cancellare dal record tutte le chiavi esterne.  Assumo che queste cancellazioni siano passate nei fields 
        ////////    {
        ////////        if (!fields.ContainsKey(deleteField)) throw new DatabaseException(ERR_NO_INPUT, "Timestamp non presente.", null);
        ////////        if (fields.ContainsKey($"OldTimestamp")) throw new DatabaseException(ERR_BAD_INPUT, "Il campo OldTimestamp non è consentito.", null);

        ////////        sql = $"UPDATE {tableName} SET {deleteField} = 'Y', {string.Join(", ", fields.Where(f => f.Key != keyField && f.Key != deleteField).Select(f => $"{f.Key} = @{f.Key}__{recNum}"))} WHERE {keyField} = @{keyField}__{recNum} and {timestampField} = @OldTimestamp__{recNum}";
        ////////        byte[] oldTimestamp = (byte[])fields[timestampField];  // salvo valore vecchi timestamp
        ////////        fields[timestampField] = GenerateTimestamp(); // genero valore nuovo timestamp
        ////////        fields[$"OldTimestamp"] = oldTimestamp;  // aggiungo il parametro relativo al vecchio timestamp
        ////////    }
        ////////    else throw new DatabaseException(ERR_BAD_INPUT, "Azione non presente.", null);
        ////////    return sql;
        ////////}
        ////////private Dictionary<string, object> ParametersMantain(int recNum, IDictionary<string, object> fields, string options)
        ////////{
        ////////    var parameters = new Dictionary<string, object>();

        ////////    foreach (var field in fields)
        ////////    {
        ////////        parameters[$"@{field.Key}__{recNum}"] = field.Value;    // parameters[$"@{field.Key}__{recNum}"] = EncodeSpecialFields(field.Value);
        ////////    }
        ////////    return parameters;
        ////////}

        //***************************************************************************************************************************************************
        //*** AUDIT
        //***************************************************************************************************************************************************

        // Esegue una query SQL con monitoraggio della sessione in caso di timeout

        /// <summary>
        /// Avvia un thread che aspetta fino a T - delta; se la query è già finita (done.Set()), non fa nulla.
        /// Altrimenti verifica che la sessione sia ancora attiva prima di fare l’audit.
        /// </summary>
        private void StartAuditMonitorIfStillRunning(IDbConnection workConn, int timeoutSeconds, int auditBeforeTimeoutSeconds,
                                                     ManualResetEventSlim done, string originalSql, string sqlText, IDictionary<string, object> parameters)
        {
            // Ricava lo SPID corrente per SQL Server
            object spid = _database.GetCommandSpid(workConn);

            if (spid != null)   // Audit solo se != null
            {

                new Thread(() =>
                {
                    IDbConnection monitorConn = null;
                    try
                    {
                        // Attendi fino a pochi secondi prima del timeout, ma esci se la query termina prima
                        int waitMs = Math.Max(0, (timeoutSeconds - auditBeforeTimeoutSeconds) * 1000);
                        if (done.Wait(waitMs)) return; // la query ha terminato: NON eseguire audit

                        // Double-check lato server: la richiesta è ancora attiva?
                        monitorConn = _database.NewConnection();

                        if (!_database.IsCommandRequestActive(monitorConn, spid))
                        {
                            // Non è più attiva: NON fare audit
                            return;
                        }

                        // Esegui snapshot audit (wait info + SQL + piano XML)
                        var snap = _database.GetCommandAuditSnapshot(monitorConn, spid, sqlText, parameters);
                        if (snap != null)
                        {
                            string sqlQuery = snap.SqlText ?? originalSql ?? "--(sql non disponibile)";
                            string sqlPlan = string.IsNullOrEmpty(snap.QueryPlanXml) ? "--(non disponibile)" : snap.QueryPlanXml;

                            // Info
                            _logger.Trace($"Audit SQL Timeout:\n" +
                                $"Query: {sqlQuery}\n" +
                                $"Status: {snap.Status}\n" +
                                $"WaitType: {snap.WaitType}\n" +
                                $"ElapsedMs: {snap.TotalElapsedMs}\n" +
                                $"BlockingSessionId: {snap.BlockingSessionId}\n" +
                                $"SqlPlan: {sqlPlan}\n");

                        }
                    }
                    catch (Exception ex)
                    {
                        // log soft: nessun impatto sulla query
                        Console.Error.WriteLine($"[Audit] {ex.GetType().Name}: {ex.Message}");
                    }
                    finally { if (monitorConn != null) _database.ReleaseConnection(monitorConn); } // si chiude se non c'è transazione
                })
                {
                    IsBackground = true, // non blocca lo shutdown del processo
                    Name = "AuditMonitorThread"
                }.Start();

            }

        }
        public class LiveSessionSnapshot
        {
            public string Status { get; set; }
            public string WaitType { get; set; }
            public long TotalElapsedMs { get; set; }
            public int? BlockingSessionId { get; set; }
            public string SqlText { get; set; }
            public string QueryPlanXml { get; set; }
        }


        //---------------------------------------------------------------------------------------------------------------------------------------------


        //private Stopwatch LogCommandBefore(IDbCommand _command)
        //{

        //    _command.CommandText = $@"
        //                                SET STATISTICS TIME ON;
        //                                SET STATISTICS IO ON;
        //                                {_command.CommandText};
        //                                SET STATISTICS TIME OFF;
        //                                SET STATISTICS IO OFF;
        //                            ";


        //    Console.WriteLine("----- Executing SQL Command -----");
        //    Console.WriteLine(_command.CommandText);
        //    foreach (IDataParameter param in _command.Parameters)
        //    {
        //        Console.WriteLine($"Param: {param.ParameterName} = {param.Value}");
        //    }
        //    Console.WriteLine($"Timeout: {_command.CommandTimeout} seconds");
        //    Console.WriteLine("---------------------------------");
        //    return Stopwatch.StartNew();
        //}
        //private void LogCommandAfterOK(IDbConnection _connection, Stopwatch stopwatch)
        //{
        //    stopwatch.Stop();
        //    Console.WriteLine($"[INFO] NonQuery executed in {stopwatch.ElapsedMilliseconds} ms");
        //    Console.WriteLine("[SQL INFO] " + _connection.);
        //}
        //private void LogCommandAfterKO(IDbConnection _connection, Stopwatch stopwatch, Exception ex)
        //{
        //    stopwatch.Stop();
        //    Console.WriteLine($"[ERROR] Exception after {stopwatch.ElapsedMilliseconds} ms: {ex.Message}");
        //}



        //--------------------------------------------------------------------------------------------------------------
        // ############################################################################################




    }
}