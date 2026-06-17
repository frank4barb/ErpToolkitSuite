using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class TipoAttivita : ModelErp {
public const string Description = "Tassonomie e classe di tipi di attività";
public const string SqlTableName = "TIPO_ATTIVITA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Ta1Icode";
public const string SqlRowIdName = "TA__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "TA_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "TA_XDATA";
public const string SqlXdataIcodeName = "TA_X__ICODE";
public const string SqlXdataDeletedName = "TA_X__DELETED";
public const string SqlXdataTimestampName = "TA_X__TIMESTAMP";
public const string SqlXdataCdateName = "TA_X__CDATE";
public const string SqlXdataCtimeName = "TA_X__CTIME";
public const string SqlXdataCagentName = "TA_X__CAGENT";
public const string SqlXdataCunitName = "TA_X__CUNIT";
public const string SqlXdataMdateName = "TA_X__MDATE";
public const string SqlXdataMtimeName = "TA_X__MTIME";
public const string SqlXdataMagentName = "TA_X__MAGENT";
public const string SqlXdataMunitName = "TA_X__MUNIT";
public const string SqlXdataHomeName = "TA_X__HOME";
public const string SqlXdataVersionName = "TA_X__VERSION";
public const string SqlXdataInactiveName = "TA_X__INACTIVE";
public const string SqlXdataExtattName = "TA_X__EXTATT";
public const string SqlXdataMrefName = "TA_X__MREF";
public const string SqlXdataSeqName = "TA_X__SEQ";
public const string SqlXdataDescrName = "TA_X__DESCR";
public const string SqlXdataFmtName = "TA_X__FMT";
public const string SqlXdataXdurlName = "TA_X__XDURL";
public const string SqlXdataXdatumName = "TA_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 3; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Ta"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ta1Icode; } 
public override string labelText() { return $@"{TaCodice} -  {TaDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(TaCodice)}</strong> {HttpUtility.HtmlEncode(TaDescrizione)}"; }

//2-2//[N] PRESTAZIONE.PR_ID_TIPO_ATTIVITA
[Display(Name = "Prestazione", ShortName = "", Description = "Prestazione effettuata: dettaglio delle attività effettivamente eseguite durante il lavoro quotidiano nell'organizzazione", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Prestazione>? XrefPrIdTipoAttivita { get; set; } = null;
//102-83//[N] ATTIVITA.AV_ID_TIPO_ATTIVITA
[Display(Name = "Attivita", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Attivita>? XrefAvIdTipoAttivita { get; set; } = null;
//2106-2099//[N] TIPO_ATTIVITA.TA_ID_GRUPPO
[Display(Name = "TipoAttivita", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.TipoAttivita>? XrefTaIdGruppo { get; set; } = null;
[Key]
[Display(Name = "Ta1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("TA__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ta1Icode { get; set; }
[Display(Name = "Ta1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("TA__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ta1Deleted { get; set; }
[Display(Name = "Ta1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("TA__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ta1Timestamp { get; set; }
[Display(Name = "Ta1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TA__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ta1Cdate { get; set; }
[Display(Name = "Ta1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TA__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ta1Ctime { get; set; }
[Display(Name = "Ta1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TA__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ta1Cagent { get; set; }
[Display(Name = "Ta1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TA__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ta1Cunit { get; set; }
[Display(Name = "Ta1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TA__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ta1Mdate { get; set; }
[Display(Name = "Ta1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TA__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ta1Mtime { get; set; }
[Display(Name = "Ta1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TA__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ta1Magent { get; set; }
[Display(Name = "Ta1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TA__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ta1Munit { get; set; }
[Display(Name = "Ta1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("TA__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ta1Home { get; set; }
[Display(Name = "Ta1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("TA__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ta1Version { get; set; }
[Display(Name = "Ta1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("TA__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ta1Inactive { get; set; }
[Display(Name = "Ta1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("TA__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ta1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TA_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_ATTIVITA.TA__ICODE[TA__ICODE] {TA_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TaCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TA_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TaDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TA_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TaNote  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Superclasse che raggruppa la classificazione corrente", Prompt="")]
[ErpDogField("TA_ID_GRUPPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Ta1Icode", SqlFieldProperties="prop() xref(TIPO_ATTIVITA.TA__ICODE) xdup() multbxref()")]
[AutocompleteClient("TipoAttivita", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? TaIdGruppo  { get; set; }
public HealthDemo.Models.SIO.Act.TipoAttivita? TaIdGruppoObj  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTa1Icode|K|TA__ICODE","sioTa1RecDate|N|TA__MDATE,TA__CDATE"
        ,"sioTaIdGruppo|N|TA_ID_GRUPPO"
        ,"sioTa1Versionta1Deleted|U|TA__VERSION,TA__DELETED"
        ,"sioTaCodiceta1Versionta1Deleted|U|TA_CODICE,TA__VERSION,TA__DELETED"
        ,"sioTa1Version|U|TA__VERSION"
    };
}
}
}
