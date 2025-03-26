using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ErpToolkit.Models {
    public abstract class ModelErp {

        //xx// SGANCIO DAL MODELLO IL CONCETTO DI VISIBILITA'
        //xx////attributi di visualizzazione dei campi definiti a run-time
        //xx//public Dictionary<string, DogHelper.FieldAttr> AttrFields { get; set; } = new Dictionary<string, DogHelper.FieldAttr>();

        // proprietà necessarie per la mantain del record
        public char? action { get; set; } = null; 
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();

        //metodi obbligatori
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
