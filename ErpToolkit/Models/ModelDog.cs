using System.ComponentModel.DataAnnotations;

namespace ErpToolkit.Models
{

    //Contiene la somma dei modelli con accesso al DB: ie: Modelli Persistenti
    // ModelErp: modelli persistenti definiti dall'utente
    // ModelXdata: modello persistente interno al toolkit, per la gestione dei dati estesi
    public abstract class ModelDog
    {
        // proprietà necessarie per la mantain e list del record
        public char? action { get; set; } = null;   // [R]ead, [A]dd, [M]odify, [D]elete
        public string options { get; set; } = "";


        //metodi obbligatori
        public abstract string labelText(); // metodo astratto: label per output Text da usare per visualizzare l'oggetto (deve per forza essere implementato)
        public override string ToString() { return labelText(); } //output Text da usare per visualizzare l'oggetto
        public abstract string labelHtml(); // metodo astratto: label per output Html da usare per visualizzare l'oggetto (deve per forza essere implementato)
        public string ToHtml() { return labelHtml(); } //output Html da usare per visualizzare l'oggetto
        //????//public abstract string ToHtml(); // metodo astratto: output Html da usare per visualizzare l'oggetto (deve per forza essere implementato)
        public abstract object getIcode(); // metodo astratto (deve per forza essere implementato)
        //public abstract string getTimestamp(); // metodo astratto 
        //public abstract string getDeleted(); // metodo astratto 



        //public sealed record modelErp(ModelErp Value) : ModelDog;
        //public sealed record modelXdata(ModelXdata Value) : ModelDog;

        //public static ModelDog from(ModelErp value) => new modelErp(value);
        //public static ModelDog from(ModelXdata value) => new modelXdata(value);
    }
}
