using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Patient {
public class Nazione : ModelErp {
public const string Description = "Nazioni";
public const string SqlTableName = "NAZIONE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Nz1Icode";
public const string SqlRowIdName = "NZ__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "NZ_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "NZ_XDATA";
public const string SqlXdataIcodeName = "NZ_X__ICODE";
public const string SqlXdataDeletedName = "NZ_X__DELETED";
public const string SqlXdataTimestampName = "NZ_X__TIMESTAMP";
public const string SqlXdataCdateName = "NZ_X__CDATE";
public const string SqlXdataCtimeName = "NZ_X__CTIME";
public const string SqlXdataCagentName = "NZ_X__CAGENT";
public const string SqlXdataCunitName = "NZ_X__CUNIT";
public const string SqlXdataMdateName = "NZ_X__MDATE";
public const string SqlXdataMtimeName = "NZ_X__MTIME";
public const string SqlXdataMagentName = "NZ_X__MAGENT";
public const string SqlXdataMunitName = "NZ_X__MUNIT";
public const string SqlXdataHomeName = "NZ_X__HOME";
public const string SqlXdataVersionName = "NZ_X__VERSION";
public const string SqlXdataInactiveName = "NZ_X__INACTIVE";
public const string SqlXdataExtattName = "NZ_X__EXTATT";
public const string SqlXdataMrefName = "NZ_X__MREF";
public const string SqlXdataSeqName = "NZ_X__SEQ";
public const string SqlXdataDescrName = "NZ_X__DESCR";
public const string SqlXdataFmtName = "NZ_X__FMT";
public const string SqlXdataXdurlName = "NZ_X__XDURL";
public const string SqlXdataXdatumName = "NZ_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 58; //Internal Table Code
public const string TBAREA = "Accoglienza"; //Table Area
public const string PREFIX = "Nz"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Nz1Icode; } 
public override string labelText() { return $@"{NzCodice} - {NzNome}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(NzCodice)}</strong> {HttpUtility.HtmlEncode(NzNome)}"; }

//1300-1286//[N] PAZIENTE.PA_ID_NAZIONE_NASCITA
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdNazioneNascita { get; set; } = null;
//1301-1286//[N] PAZIENTE.PA_ID_CITTADINANZA
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdCittadinanza { get; set; } = null;
//1309-1286//[N] PAZIENTE.PA_ID_NAZIONE_DOM
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdNazioneDom { get; set; } = null;
//1355-1286//[N] PAZIENTE.PA_ID_NAZIONE_RES
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdNazioneRes { get; set; } = null;
[Key]
[Display(Name = "Nz1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("NZ__ICODE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Icode { get; set; }
[Display(Name = "Nz1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("NZ__DELETED", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Nz1Deleted { get; set; }
[Display(Name = "Nz1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("NZ__TIMESTAMP", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Nz1Timestamp { get; set; }
[Display(Name = "Nz1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("NZ__CDATE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Nz1Cdate { get; set; }
[Display(Name = "Nz1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("NZ__CTIME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Nz1Ctime { get; set; }
[Display(Name = "Nz1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("NZ__CAGENT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Cagent { get; set; }
[Display(Name = "Nz1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("NZ__CUNIT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Cunit { get; set; }
[Display(Name = "Nz1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("NZ__MDATE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Nz1Mdate { get; set; }
[Display(Name = "Nz1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("NZ__MTIME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Nz1Mtime { get; set; }
[Display(Name = "Nz1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("NZ__MAGENT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Magent { get; set; }
[Display(Name = "Nz1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("NZ__MUNIT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Munit { get; set; }
[Display(Name = "Nz1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("NZ__HOME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Home { get; set; }
[Display(Name = "Nz1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("NZ__VERSION", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Version { get; set; }
[Display(Name = "Nz1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("NZ__INACTIVE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Nz1Inactive { get; set; }
[Display(Name = "Nz1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("NZ__EXTATT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Nz1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice ufficiale (esterno) del paese", Prompt="")]
[ErpDogField("NZ_CODICE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? NzCodice  { get; set; }

[Display(Name = "Nome", ShortName="", Description = "Nome esteso", Prompt="")]
[ErpDogField("NZ_NOME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(25, ErrorMessage = "Inserire massimo 25 caratteri")]
[DataType(DataType.Text)]
public string? NzNome  { get; set; }

[Display(Name = "Cod Istat", ShortName="", Description = "Codice statistico", Prompt="")]
[ErpDogField("NZ_COD_ISTAT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? NzCodIstat  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("NZ_NOTE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? NzNote  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioNz1Icode|K|NZ__ICODE","sioNz1RecDate|N|NZ__MDATE,NZ__CDATE"
        ,"sioNzNomenz1Versionnz1Deleted|U|NZ_NOME,NZ__VERSION,NZ__DELETED"
        ,"sioNzCodicenz1Versionnz1Deleted|U|NZ_CODICE,NZ__VERSION,NZ__DELETED"
    };
}
}
}
