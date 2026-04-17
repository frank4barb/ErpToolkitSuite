using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;
using System.Reflection;
using System.Reflection.Metadata;
using static ErpToolkit.Helpers.Db.DogManager;

namespace ErpToolkit.Models {
    public abstract class ModelErp  {

        //xx// SGANCIO DAL MODELLO IL CONCETTO DI VISIBILITA'
        //xx////attributi di visualizzazione dei campi definiti a run-time
        //xx//public Dictionary<string, DogHelper.FieldAttr> AttrFields { get; set; } = new Dictionary<string, DogHelper.FieldAttr>();

        // gestione cache
        private DogCache _dogCache = null;
        private int _dogCacheReadId = -1;  //numero d'ordine di lettura in cache
        private int _dogCacheMntId = -1;  //numero d'ordine di modifica in cache (add,upd,del). Se = -1, allora il record non è stato modificato
        private int _depth = -1;  //profondità di inclusione dell'oggetto (serve per evitare loop infiniti in caso di inclusione di oggetti che si richiamano a vicenda, come ad esempio un cliente che ha un ordine, e l'ordine ha un cliente, e così via)
        internal void addDogCache(ref DogCache dogCache) { this._dogCache = dogCache; this._dogCacheReadId = this._dogCache.GetReadID(); }
        public void forceMnt() { if (_dogCache == null) { throw new ArgumentNullException(nameof(_dogCache)); } this._dogCacheMntId = this._dogCache.GetMntID(); }
        public void resetMnt() { this._dogCacheMntId = -1; }
        internal int orderMnt() { return this._dogCacheMntId; }

        public int depth { get; internal set; } = -1; //profondità di inclusione dell'oggetto (esternamente può essere solo letto)

        //public int depth() { return this._depth; }
        //internal void setDepth(int depth) //profondità di inclusione dell'oggetto
        //{
        //    if (depth < 0) { throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be greater than or equal to 0."); }
        //    this._depth = depth;
        //}

        //variabile che consente di caricare la stringa json originale ricevuta dal client (utile per confrontare i valori cambiati)
        public string jsonOriginal = null;


        // proprietà necessarie per la mantain e list del record
        public char? action { get; set; } = null;   // [R]ead, [A]dd, [M]odify, [D]elete
        public string options { get; set; } = "";


        //[Vars("XML")]
        //public Dictionary<string, string> vars { get; set; } = new Dictionary<string, string>();  // @xxxx variabili volatili per passare parametri vari (es. @Lang, @UserId, @CompanyId, ecc.)
        //                                                                                         // xxxxx variabili permanenti che vengono salvate su DB in _extatt

        private IDictionary<string, string> _vars = new Dictionary<string, string>(); // @xxxx variabili volatili per passare parametri vari (es. @Lang, @UserId, @CompanyId, ecc.)

        [Vars("XML")]
        public IDictionary<string, string> vars
        {
            get => _vars ??= new Dictionary<string, string>();
            set => _vars = value ?? new Dictionary<string, string>();
        }

        public string getLabelForField(string fieldName) { string s = $"@{fieldName}-FieldLabel";  return (this.vars.ContainsKey(s)) ? this.vars[s] : ""; }

        internal IDictionary<string, List<ModelErp>> xrefFrom { get; set; } = new Dictionary<string, List<ModelErp>>();  //se effettuo una lista con tabelle esterne, le memorizzo qui come xrefFrom, cioè da dove sono arrivato (per esempio: se sono in un record di un cliente, e ho una lista di ordini, gli ordini sono xrefFrom del cliente)
        internal List<ModelErp>? Xref(string xrefProperty) 
        {
            //if (xrefFrom != null && xrefFrom.TryGetValue(xrefProperty, out var list)) return list.Cast<RelPrestazioneCampione>().ToList();
            if (xrefFrom != null && xrefFrom.TryGetValue(xrefProperty, out var list)) return list;
            return null;  //return new List<RelPrestazioneCampione>();
        }

        //lista di dati estesi (Xdata) che sono associati all'oggetto
        public Dictionary<object, ModelXdata>? Xdata { get; set; } = null;


        //metodi obbligatori
        public abstract string labelText(); // metodo astratto: label per output Text da usare per visualizzare l'oggetto (deve per forza essere implementato)
        public override string ToString() { return labelText(); } //output Text da usare per visualizzare l'oggetto
        public abstract string labelHtml(); // metodo astratto: label per output Html da usare per visualizzare l'oggetto (deve per forza essere implementato)
        public string ToHtml() { return labelHtml(); } //output Html da usare per visualizzare l'oggetto
        //????//public abstract string ToHtml(); // metodo astratto: output Html da usare per visualizzare l'oggetto (deve per forza essere implementato)
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
        public virtual string ViewQueryXdataFromWhere()
        {
            return "";
        }


    }
}
