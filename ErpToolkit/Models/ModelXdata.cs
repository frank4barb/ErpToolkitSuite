using ErpToolkit.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ErpToolkit.Models
{
    public class ModelXdata : ModelDog
    {
        public const string Description = "Pazienti rilevanti per l'organizzazione sanitaria";
        public const string SqlTableName = "";
        public const string SqlTableNameExt = "";
        public const string SqlTableProperties = "";
        public const string RowIdName = "";
        public const string SqlRowIdName = "";
        public const string SqlRowIdNameExt = "";
        public const string SqlPrefix = "";
        public const string SqlPrefixExt = "";
        public const string MODEL = ""; //Data Model Name of the Class
        public const string CATEG = ""; //Data Model Name of the Class
        public const int INTCODE = -1; //Internal Table Code
        public const string TBAREA = ""; //Table Area
        public const string PREFIX = ""; //Table Prefix
        public const string LIVEDESC = ""; //Table type: Live or Description
        public const string IS_RELTABLE = ""; //Is Relation Table: Yes or No
        public override object getIcode() { return (object)Icode; }
        public override string labelText() { return $@"{Descr}"; }
        public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(Descr)}</strong>"; }


        [Key]
        [Display(Name = "Icode", ShortName = "", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[SID]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        public object? Icode { get; set; }
        [Display(Name = "Deleted", ShortName = "", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[DEL]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
        public string? Deleted { get; set; }
        [Display(Name = "Timestamp", ShortName = "", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[TMS]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        //[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
        public byte[]? Timestamp { get; set; }
        [Display(Name = "Cdate", ShortName = "", Description = "Data di creazione iniziale dell'istanza", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[CDATE]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
        public string? Cdate { get; set; }
        [Display(Name = "Ctime", ShortName = "", Description = "Ora di creazione iniziale dell'istanza", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[CTIME]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
        public string? Ctime { get; set; }
        [Display(Name = "Cagent", ShortName = "", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[CAGENT]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        public string? Cagent { get; set; }
        [Display(Name = "Cunit", ShortName = "", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[CUNIT]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        public string? Cunit { get; set; }
        [Display(Name = "Mdate", ShortName = "", Description = "Data dell'ultima modifica all'istanza da utente", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[MDATE]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
        public string? Mdate { get; set; }
        [Display(Name = "Mtime", ShortName = "", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[MTIME]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
        public string? Mtime { get; set; }
        [Display(Name = "Magent", ShortName = "", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[MAGENT]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        public string? Magent { get; set; }
        [Display(Name = "Munit", ShortName = "", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[MUNIT]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        public string? Munit { get; set; }
        [Display(Name = "Home", ShortName = "", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[HOME]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        public string? Home { get; set; }
        [Display(Name = "Version", ShortName = "", Description = "Versione dell'istanza", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[VERSION]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        public string? Version { get; set; }
        [Display(Name = "Inactive", ShortName = "", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[INACTIVE]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        [StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
        public string? Inactive { get; set; }
        [Display(Name = "Extatt", ShortName = "", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[EXTATT]", SqlFieldProperties = "prop()")]
        [DataType(DataType.Text)]
        public string? Extatt { get; set; }


        [Display(Name = "Mref", ShortName = "", Description = "Codice dell'oggetto a cui si riferisce il dato", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[MANDATORY] [MREF]", SqlFieldProperties = "prop()")]
        [Required(ErrorMessage = "Inserire un valore nel campo")]
        [DataType(DataType.Text)]
        public object? Mref { get; set; }

        [Display(Name = "Seq", ShortName = "", Description = "Sequenza del dato", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[SEQ]", SqlFieldProperties = "prop()")]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        [DataType(DataType.Text)]
        public short? Seq { get; set; }

        [Display(Name = "Descr", ShortName = "", Description = "Descrizione del dato", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[LABEL] [DESCR]", SqlFieldProperties = "prop()")]
        [StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
        [DataType(DataType.Text)]
        public string? Descr { get; set; }

        [Display(Name = "Fmt", ShortName = "", Description = "Codice formato interno del dato", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[MANDATORY] [FMT]", SqlFieldProperties = "prop()")]
        [Required(ErrorMessage = "Inserire un valore nel campo")]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        [DataType(DataType.Text)]
        public string? Fmt { get; set; }

        [Display(Name = "Xdurl", ShortName = "", Description = "URL della locazione esterna del dato (se non archiviato in locale)", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[XDURL]", SqlFieldProperties = "prop()")]
        [StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
        [DataType(DataType.Text)]
        public string? Xdurl { get; set; }

        [Display(Name = "Xdatum", ShortName = "", Description = "Valore binario del dato", Prompt = "")]
        [ErpDogField(null, SqlFieldNameExt = null, SqlFieldOptions = "[XDATUM]", SqlFieldProperties = "prop()")]
        [StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
        [DataType(DataType.Text)]
        public byte[]? Xdatum { get; set; }
        public object? XdatumObj { get; set; }
        //------

        public string? _mimeXdatum { get; set; }
        public long? _sizeXdatum { get; set; }
        public Stream? _streamXdatum { get; set; }


        //------
        // FUNCTIONS
        //------
        public ModelXdata Clone() { return _Clone(false); }
        public ModelXdata CloneTruncate() { return _Clone(true); }

        private ModelXdata _Clone(bool isTruncate)
        {
            return new ModelXdata
            {
                Icode = this.Icode,
                Deleted = this.Deleted,
                Timestamp = (byte[]?)this.Timestamp?.Clone(),
                Cdate = this.Cdate,
                Ctime = this.Ctime,
                Cagent = this.Cagent,
                Cunit = this.Cunit,
                Mdate = this.Mdate,
                Mtime = this.Mtime,
                Magent = this.Magent,
                Munit = this.Munit,
                Home = this.Home,
                Version = this.Version,
                Inactive = this.Inactive,
                Extatt = this.Extatt,
                Mref = this.Mref,
                Seq = this.Seq,
                Descr = this.Descr,
                Fmt = this.Fmt,
                Xdurl = this.Xdurl,
                Xdatum = (isTruncate) ? null : (byte[]?)this.Xdatum?.Clone(),
                _mimeXdatum = this._mimeXdatum,
                _sizeXdatum = this._sizeXdatum,
                _streamXdatum = this._streamXdatum,
            };

        }
    }
}
