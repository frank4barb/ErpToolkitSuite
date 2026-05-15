using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Patient {
public class Distretto : ModelErp {
public const string Description = "Distretto territoriale (circoscrizione)";
public const string SqlTableName = "DISTRETTO";
public const string SqlTableNameExt = "DISTRETTO";
public const string SqlTableProperties = "";
public const string RowIdName = "Di1Icode";
public const string SqlRowIdName = "DI__ICODE";
public const string SqlRowIdNameExt = "DI__ICODE";
public const string SqlPrefix = "DI_";
public const string SqlPrefixExt = "DI_";
public const string SqlXdataTableName = "DI_XDATA";
public const string SqlXdataIcodeName = "DI_X__ICODE";
public const string SqlXdataDeletedName = "DI_X__DELETED";
public const string SqlXdataTimestampName = "DI_X__TIMESTAMP";
public const string SqlXdataCdateName = "DI_X__CDATE";
public const string SqlXdataCtimeName = "DI_X__CTIME";
public const string SqlXdataCagentName = "DI_X__CAGENT";
public const string SqlXdataCunitName = "DI_X__CUNIT";
public const string SqlXdataMdateName = "DI_X__MDATE";
public const string SqlXdataMtimeName = "DI_X__MTIME";
public const string SqlXdataMagentName = "DI_X__MAGENT";
public const string SqlXdataMunitName = "DI_X__MUNIT";
public const string SqlXdataHomeName = "DI_X__HOME";
public const string SqlXdataVersionName = "DI_X__VERSION";
public const string SqlXdataInactiveName = "DI_X__INACTIVE";
public const string SqlXdataExtattName = "DI_X__EXTATT";
public const string SqlXdataMrefName = "DI_X__MREF";
public const string SqlXdataSeqName = "DI_X__SEQ";
public const string SqlXdataDescrName = "DI_X__DESCR";
public const string SqlXdataFmtName = "DI_X__FMT";
public const string SqlXdataXdurlName = "DI_X__XDURL";
public const string SqlXdataXdatumName = "DI_X__XDATUM";
public const string SqlXdataTableNameExt = "DI_XDATA";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 128; //Internal Table Code
public const string TBAREA = "Accoglienza"; //Table Area
public const string PREFIX = "Di"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Di1Icode; } 
public override string labelText() { return $@"{DiCodice} - {DiNome}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(DiCodice)}</strong> {HttpUtility.HtmlEncode(DiNome)}"; }

//1308-1286//[N] PAZIENTE.PA_ID_DISTRETTO_RES
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdDistrettoRes { get; set; } = null;
//1321-1286//[N] PAZIENTE.PA_ID_DISTRETTO_DOM
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdDistrettoDom { get; set; } = null;
[Key]
[Display(Name = "Di1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("DI__ICODE", SqlFieldNameExt="DI__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Di1Icode { get; set; }
[Display(Name = "Di1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("DI__DELETED", SqlFieldNameExt="DI__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Di1Deleted { get; set; }
[Display(Name = "Di1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("DI__TIMESTAMP", SqlFieldNameExt="DI__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Di1Timestamp { get; set; }
[Display(Name = "Di1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("DI__CDATE", SqlFieldNameExt="DI__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Di1Cdate { get; set; }
[Display(Name = "Di1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("DI__CTIME", SqlFieldNameExt="DI__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Di1Ctime { get; set; }
[Display(Name = "Di1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("DI__CAGENT", SqlFieldNameExt="DI__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Di1Cagent { get; set; }
[Display(Name = "Di1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("DI__CUNIT", SqlFieldNameExt="DI__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Di1Cunit { get; set; }
[Display(Name = "Di1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("DI__MDATE", SqlFieldNameExt="DI__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Di1Mdate { get; set; }
[Display(Name = "Di1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("DI__MTIME", SqlFieldNameExt="DI__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Di1Mtime { get; set; }
[Display(Name = "Di1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("DI__MAGENT", SqlFieldNameExt="DI__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Di1Magent { get; set; }
[Display(Name = "Di1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("DI__MUNIT", SqlFieldNameExt="DI__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Di1Munit { get; set; }
[Display(Name = "Di1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("DI__HOME", SqlFieldNameExt="DI__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Di1Home { get; set; }
[Display(Name = "Di1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("DI__VERSION", SqlFieldNameExt="DI__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Di1Version { get; set; }
[Display(Name = "Di1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("DI__INACTIVE", SqlFieldNameExt="DI__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Di1Inactive { get; set; }
[Display(Name = "Di1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("DI__EXTATT", SqlFieldNameExt="DI__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Di1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice utente del distretto (CAP)", Prompt="")]
[ErpDogField("DI_CODICE", SqlFieldNameExt="DI_CODICE", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(DISTRETTO.DI__ICODE[DI__ICODE] {DI_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? DiCodice  { get; set; }

[Display(Name = "Nome", ShortName="", Description = "Descrizione estesa del distretto", Prompt="")]
[ErpDogField("DI_NOME", SqlFieldNameExt="DI_NOME", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? DiNome  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note sul distretto", Prompt="")]
[ErpDogField("DI_NOTE", SqlFieldNameExt="DI_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? DiNote  { get; set; }

[Display(Name = "Id Comune", ShortName="", Description = "Città in cui si trova il distretto", Prompt="")]
[ErpDogField("DI_ID_COMUNE", SqlFieldNameExt="DI_ID_COMUNE", SqlFieldOptions="", Xref="Cm1Icode", SqlFieldProperties="prop() xref(COMUNE.CM__ICODE) xdup() multbxref()")]
[AutocompleteClient("Comune", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? DiIdComune  { get; set; }
public HealthDemo.Models.SIO.Patient.Comune? DiIdComuneObj  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioDi1Icode|K|DI__ICODE","sioDi1RecDate|N|DI__MDATE,DI__CDATE"
        ,"sioDiIdComune|N|DI_ID_COMUNE"
        ,"sioDi1Versiondi1Deleted|U|DI__VERSION,DI__DELETED"
        ,"sioDiCodicedi1Versiondi1Deleted|U|DI_CODICE,DI__VERSION,DI__DELETED"
    };
}
}
}
