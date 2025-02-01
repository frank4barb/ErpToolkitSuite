using Org.BouncyCastle.Crypto;
using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace ErpToolkit.Helpers.Db
{
    public class IRISClientWrapper
    {
        private string _assemblyPath = "InterSystems.Data.IRISClient.dll";
        private Assembly _assembly;

        public Type IRISExceptionType { get; private set; }
        public Type IRISConnectionType { get; private set; }
        public Type IRISDataAdapterType { get; private set; }
        public Type IRISCommandType { get; private set; }
        public Type IRISTransactionType { get; private set; }

        public IRISClientWrapper()
        {
            if (File.Exists(_assemblyPath))
            {
                _assembly = Assembly.LoadFrom(_assemblyPath);
                LoadTypes();
            }
            else
            {
                throw new FileNotFoundException("Assembly not found.", _assemblyPath);
            }
        }

        private void LoadTypes()
        {
            IRISExceptionType = _assembly.GetType("InterSystems.Data.IRISClient.IRISException");
            IRISConnectionType = _assembly.GetType("InterSystems.Data.IRISClient.IRISConnection");
            IRISDataAdapterType = _assembly.GetType("InterSystems.Data.IRISClient.IRISDataAdapter");
            IRISCommandType = _assembly.GetType("InterSystems.Data.IRISClient.IRISCommand");
            IRISTransactionType = _assembly.GetType("InterSystems.Data.IRISClient.IRISTransaction");

            if (IRISExceptionType == null || IRISConnectionType == null || IRISDataAdapterType == null ||
                IRISCommandType == null || IRISTransactionType == null)
            {
                throw new Exception("One or more types could not be loaded from the assembly.");
            }
        }

        //IRISConnection
        public object CreateIRISConnection(string connectionString)
        {
            return Activator.CreateInstance(IRISConnectionType, connectionString);
        }
        public void OpenIRISConnection(object irisConnectionInstance)
        {
            MethodInfo openMethod = IRISConnectionType.GetMethod("Open");
            if (openMethod == null) throw new Exception("Open method not found in IRISConnection type.");
            openMethod.Invoke(irisConnectionInstance, null);
        }
        public void CloseIRISConnection(object irisConnectionInstance)
        {
            MethodInfo openMethod = IRISConnectionType.GetMethod("Close");
            if (openMethod == null) throw new Exception("Close method not found in IRISConnection type.");
            openMethod.Invoke(irisConnectionInstance, null);
        }
        public void DisposeIRISConnection(object irisConnectionInstance)
        {
            MethodInfo openMethod = IRISConnectionType.GetMethod("Dispose");
            if (openMethod == null) throw new Exception("Dispose method not found in IRISConnection type.");
            openMethod.Invoke(irisConnectionInstance, null);
        }
        public object BeginTransactionIRISConnection(object irisConnectionInstance)
        {
            MethodInfo openMethod = IRISConnectionType.GetMethod("BeginTransaction");
            if (openMethod == null) throw new Exception("BeginTransaction method not found in IRISConnection type.");
            return openMethod.Invoke(irisConnectionInstance, null);
        }
        public ConnectionState GetStateIRISConnection(object irisConnectionInstance)
        {
            var stateProperty = IRISConnectionType.GetProperty("State");
            if (stateProperty == null) throw new Exception("State property not found in IRISConnection type.");
            return (ConnectionState)stateProperty.GetValue(irisConnectionInstance);
        }




        //IRISCommand
        public object CreateIRISCommand(string sql, object connection)
        {
            return Activator.CreateInstance(IRISCommandType, sql, connection);
        }
        public object CreateIRISCommand(string sql, object connection, object transaction)
        {
            return Activator.CreateInstance(IRISCommandType, sql, connection, transaction);
        }

        //IRISDataAdapter
        public IDisposable CreateIRISDataAdapter()
        {
            return (IDisposable)Activator.CreateInstance(IRISDataAdapterType);
        }
        public IDisposable CreateIRISDataAdapter(object command)
        {
            return (IDisposable)Activator.CreateInstance(IRISDataAdapterType, command);
        }
        public void FillIRISDataAdapter(object irisDataAdapterInstance, DataTable result)
        {
            MethodInfo openMethod = IRISDataAdapterType.GetMethod("Fill");
            if (openMethod == null) throw new Exception("Fill method not found in IRISDataAdapter type.");
            openMethod.Invoke(irisDataAdapterInstance, new object[] { result });
        }
        public void FillIRISDataAdapter(object irisDataAdapterInstance, int maxRecords, DataTable result)
        {
            MethodInfo openMethod = IRISDataAdapterType.GetMethod("Fill");
            if (openMethod == null) throw new Exception("Fill method not found in IRISDataAdapter type.");
            openMethod.Invoke(irisDataAdapterInstance, new object[] { 0, maxRecords, result });
        }

        //IRISTransaction
        public object CreateIRISTransaction()
        {
            return Activator.CreateInstance(IRISTransactionType);
        }
        public void SaveIRISTransaction(object irisTransactionInstance, string savePointName)
        {
            MethodInfo openMethod = IRISTransactionType.GetMethod("Save");
            if (openMethod == null) throw new Exception("Save method not found in IRISTransaction type.");
            openMethod.Invoke(irisTransactionInstance, new object[] { savePointName });
        }
        public void RollbackIRISTransaction(object irisTransactionInstance, string savePointName)
        {
            MethodInfo openMethod = IRISTransactionType.GetMethod("Rollback");
            if (openMethod == null) throw new Exception("Rollback method not found in IRISTransaction type.");
            openMethod.Invoke(irisTransactionInstance, new object[] { savePointName });
        }
        public void RollbackIRISTransaction(object irisTransactionInstance)
        {
            MethodInfo openMethod = IRISTransactionType.GetMethod("Rollback");
            if (openMethod == null) throw new Exception("Rollback method not found in IRISTransaction type.");
            openMethod.Invoke(irisTransactionInstance, null);
        }
        public void CommitIRISTransaction(object irisTransactionInstance)
        {
            MethodInfo openMethod = IRISTransactionType.GetMethod("Commit");
            if (openMethod == null) throw new Exception("Commit method not found in IRISTransaction type.");
            openMethod.Invoke(irisTransactionInstance, null);
        }
        public void DisposeIRISTransaction(object irisTransactionInstance)
        {
            MethodInfo openMethod = IRISTransactionType.GetMethod("Dispose");
            if (openMethod == null) throw new Exception("Dispose method not found in IRISTransaction type.");
            openMethod.Invoke(irisTransactionInstance, null);
        }
        public IDbConnection GetConnectionIRISTransaction(object irisTransactionInstance)
        {
            var connectionProperty = IRISTransactionType.GetProperty("Connection");
            if (connectionProperty == null) throw new Exception("Connection property not found in IRISTransaction type.");
            return (IDbConnection)connectionProperty.GetValue(irisTransactionInstance);
        }

        //IRISException
        public object CreateIRISException(string message)
        {
            return Activator.CreateInstance(IRISExceptionType, message);
        }
        public bool IsIRISException(Exception ex)
        {
            return ex.GetType() == IRISExceptionType;
        }
        public int GetErrorCodeIRISException(Exception ex)
        {
            var errorCodeProperty = IRISExceptionType.GetProperty("ErrorCode");
            if (errorCodeProperty == null) throw new Exception("ErrorCode property not found in IRISException type.");
            return (int)errorCodeProperty.GetValue(ex);
        }





        }
}
