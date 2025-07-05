using K4os.Hash.xxHash;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data.Entity.Infrastructure;
using static ErpToolkit.Helpers.Db.DogManager;

namespace ErpToolkit.Models {
    public abstract class ModelErp {

        //xx// SGANCIO DAL MODELLO IL CONCETTO DI VISIBILITA'
        //xx////attributi di visualizzazione dei campi definiti a run-time
        //xx//public Dictionary<string, DogHelper.FieldAttr> AttrFields { get; set; } = new Dictionary<string, DogHelper.FieldAttr>();

        // gestione cache
        private DogCache _dogCache = null;
        private int _dogCacheReadId = -1;  //numero d'ordine di lettura in cache
        private int _dogCacheMntId = -1;  //numero d'ordine di modifica in cache (add,upd,del). Se = -1, allora il record non è stato modificato
        internal void addToCache(ref DogCache dogCache) { this._dogCache = dogCache; this._dogCacheReadId = this._dogCache.GetReadID(); }
        public void forceMnt() { if (_dogCache == null) { throw new ArgumentNullException(nameof(_dogCache)); } this._dogCacheMntId = this._dogCache.GetMntID(); }
        public void resetMnt() { this._dogCacheMntId = -1; }
        internal int orderMnt() { return this._dogCacheMntId; }


        // proprietà necessarie per la mantain e list del record
        public char? action { get; set; } = null;
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, List<ModelErp>> xrefFrom { get; set; } = new Dictionary<string, List<ModelErp>>();  //se effettuo una lista con tabelle esterne, le memorizzo qui come xrefFrom, cioè da dove sono arrivato (per esempio: se sono in un record di un cliente, e ho una lista di ordini, gli ordini sono xrefFrom del cliente)

        //metodi obbligatori
        public abstract string ToHtml(); // metodo astratto: output Html da usare per visualizzare l'oggetto (deve per forza essere implementato)
        public abstract object getIcode(); // metodo astratto (deve per forza essere implementato)
        //public abstract string getTimestamp(); // metodo astratto 
        //public abstract string getDeleted(); // metodo astratto 

        //metodi virtuali (se non implementati si usa il default)
        public virtual bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null)
        {
            return true;
        }
        public virtual string ViewQueryFromWhere()
        {
            return "";
        }


    }
}
