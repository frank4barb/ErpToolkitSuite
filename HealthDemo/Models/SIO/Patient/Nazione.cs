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
public const string SqlTableNameExt = "NAZIONE";
public const string SqlTableProperties = "";
public const string RowIdName = "Nz1Icode";
public const string SqlRowIdName = "NZ__ICODE";
public const string SqlRowIdNameExt = "NZ__ICODE";
public const string SqlPrefix = "NZ_";
public const string SqlPrefixExt = "NZ_";
public const string SqlXdataTableName = "NZ_XDATA";
public const string SqlXdataTableNameExt = "NZ_XDATA";
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
[ErpDogField("NZ__ICODE", SqlFieldNameExt="NZ__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Icode { get; set; }
[Display(Name = "Nz1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("NZ__DELETED", SqlFieldNameExt="NZ__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Nz1Deleted { get; set; }
[Display(Name = "Nz1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("NZ__TIMESTAMP", SqlFieldNameExt="NZ__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Nz1Timestamp { get; set; }
[Display(Name = "Nz1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("NZ__CDATE", SqlFieldNameExt="NZ__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Nz1Cdate { get; set; }
[Display(Name = "Nz1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("NZ__CTIME", SqlFieldNameExt="NZ__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Nz1Ctime { get; set; }
[Display(Name = "Nz1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("NZ__CAGENT", SqlFieldNameExt="NZ__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Cagent { get; set; }
[Display(Name = "Nz1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("NZ__CUNIT", SqlFieldNameExt="NZ__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Cunit { get; set; }
[Display(Name = "Nz1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("NZ__MDATE", SqlFieldNameExt="NZ__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Nz1Mdate { get; set; }
[Display(Name = "Nz1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("NZ__MTIME", SqlFieldNameExt="NZ__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Nz1Mtime { get; set; }
[Display(Name = "Nz1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("NZ__MAGENT", SqlFieldNameExt="NZ__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Magent { get; set; }
[Display(Name = "Nz1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("NZ__MUNIT", SqlFieldNameExt="NZ__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Munit { get; set; }
[Display(Name = "Nz1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("NZ__HOME", SqlFieldNameExt="NZ__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Home { get; set; }
[Display(Name = "Nz1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("NZ__VERSION", SqlFieldNameExt="NZ__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Nz1Version { get; set; }
[Display(Name = "Nz1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("NZ__INACTIVE", SqlFieldNameExt="NZ__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Nz1Inactive { get; set; }
[Display(Name = "Nz1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("NZ__EXTATT", SqlFieldNameExt="NZ__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Nz1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice ufficiale (esterno) del paese", Prompt="")]
[ErpDogField("NZ_CODICE", SqlFieldNameExt="NZ_CODICE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? NzCodice  { get; set; }

[Display(Name = "Nome", ShortName="", Description = "Nome esteso", Prompt="")]
[ErpDogField("NZ_NOME", SqlFieldNameExt="NZ_NOME", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(25, ErrorMessage = "Inserire massimo 25 caratteri")]
[DataType(DataType.Text)]
public string? NzNome  { get; set; }

[Display(Name = "Cod Istat", ShortName="", Description = "Codice statistico", Prompt="")]
[ErpDogField("NZ_COD_ISTAT", SqlFieldNameExt="NZ_COD_ISTAT", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? NzCodIstat  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("NZ_NOTE", SqlFieldNameExt="NZ_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
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
