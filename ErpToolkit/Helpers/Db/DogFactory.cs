
using static ErpToolkit.Helpers.Db.DatabaseManager;

namespace ErpToolkit.Helpers.Db
{
    //------------------- 
    //Data Object Gateway
    //-------------------
    // Funzioni di gestione accesso al Database, con il supporto del Data Model 
    public class DogFactory : IDisposable
    {

        public class DogId {
            private string _modelName, _modelMode, _connectionStringName; private DbTyp _dbType; private string _dbRoot; private string _dbHome;
            public string ModelName { get { return _modelName; } }
            public string ModelMode { get { return _modelMode; } }   //indica come interpretare il Modello. Se _modelMode == "FREE" allora il modello non prevede i campi standard _deleted _timestamp, ecc. e non gestisce le date come stringhe
            public DbTyp DbType { get { return _dbType; } }
            public string ConnectionStringName { get { return _connectionStringName; } }
            public string DbRoot { get { return _dbRoot; } }
            public string DbHome { get { return _dbHome; } }
            public DogId(string modelName, string modelMode, DbTyp dbType, string connectionStringName, string dbRoot, string dbHome)
            {
                _modelName = modelName; _modelMode = modelMode; _dbType = dbType; _connectionStringName = connectionStringName; _dbRoot = dbRoot; _dbHome = dbHome;
            }
            public DogId(string connectionStringFull_NameTypeModel, string dbRoot, string databaseName = "")
            {
                //connectionStringFull_NameTypeModel
                if (String.IsNullOrWhiteSpace(connectionStringFull_NameTypeModel)) throw new ArgumentException("Errore: DogId: connectionStringFull_NameTypeModel vuota.");
                string[] comp = connectionStringFull_NameTypeModel.Split("__");
                if (comp.Length != 3 && comp.Length != 4) throw new ArgumentException("Errore: DogId: connectionStringFull_NameTypeModel bad syntax: #<DB name>__<DB type>__<Model Name>[__<ModelMode>] with ModelMode in (\"FREE\")");
                if (Enum.TryParse(comp[1], out DbTyp testType) == false) throw new ArgumentException("Errore: DogId: connectionStringFull_NameTypeModel unknown <DB type> in: #<DB name>__<DB type>__<Model Name>");
                _dbType = testType;
                _modelName = comp[2]; _connectionStringName = connectionStringFull_NameTypeModel;
                _modelMode = ""; if (comp.Length == 4) _modelMode = comp[3].ToUpper();
                //dbRoot dbHome
                _dbRoot = dbRoot.Trim();
                if (_dbRoot.Length != 4) throw new ArgumentException("Errore: DogId: wrong dbRoot length.");
                _dbHome = _dbRoot; // per il momento li pongo uguali
            }
        }

        private IDictionary<string, DogManager> _itemsDOG = new Dictionary<string, DogManager>();  //

        public DogFactory()
        {
        }
        ~DogFactory()
        {
            Dispose();
        }
        public void Dispose()
        {
            // Rilascia risorse non gestite
            if (_itemsDOG != null)
            {
                foreach (var key in _itemsDOG.Keys) { _itemsDOG[key].Dispose(); _itemsDOG.Remove(key); }
                _itemsDOG.Clear(); _itemsDOG = null;
            }
            //.........
            GC.SuppressFinalize(this);
        }

        private static readonly object _lockObject = new object();

        public DogManager GetDog(DogId dogId, string databaseName = "")
        {
            if (dogId == null) throw new ArgumentException("Errore: GetDog: dogId=null.");
            return GetDog(dogId.ModelName, dogId.ModelMode, dogId.DbType, dogId.ConnectionStringName, dogId.DbRoot, dogId.DbHome, databaseName);
        }

        public DogManager GetDog(string modelName, string modelMode, DbTyp dbType, string connectionStringName, string dbRoot, string dbHome, string databaseName = "")
        {
            if (String.IsNullOrWhiteSpace(modelName)) throw new ArgumentException("Errore: GetDog: modelName vuota.");
            if (String.IsNullOrWhiteSpace(connectionStringName)) throw new ArgumentException("Errore: GetDog: connectionStringName vuota.");
            dbRoot = dbRoot.Trim();
            if (dbRoot.Length != 4) throw new ArgumentException("Errore: GetDog: wrong dbRoot length.");
            if (String.IsNullOrWhiteSpace(dbHome)) dbHome = dbRoot;
            string key = modelName + "***" + dbType.ToString() + "***" + connectionStringName;
            lock (_lockObject)
            {
                DogManager dog = null; if (_itemsDOG.ContainsKey(key)) dog = _itemsDOG[key];
                if (dog == null)
                {
                    dog = new DogManager(modelName, modelMode, dbType, connectionStringName, dbRoot, dbHome);
                    if (dog == null) throw new ArgumentException($"Errore: impossibile creare db {dbType.ToString()} {databaseName}  ({connectionStringName}) ");
                    _itemsDOG[key] = dog;
                }
                return dog;
            }
        }

    }
}