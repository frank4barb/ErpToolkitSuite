using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Costs {
public class TipoDiagnosi : ModelErp {
public const string Description = "Tipi generali di classificazioni diagnostiche.";
public const string SqlTableName = "TIPO_DIAGNOSI";
public const string SqlTableNameExt = "TIPO_DIAGNOSI";
public const string SqlTableProperties = "";
public const string RowIdName = "Td1Icode";
public const string SqlRowIdName = "TD__ICODE";
public const string SqlRowIdNameExt = "TD__ICODE";
public const string SqlPrefix = "TD_";
public const string SqlPrefixExt = "TD_";
public const string SqlXdataTableName = "TD_XDATA";
public const string SqlXdataIcodeName = "TD_X__ICODE";
public const string SqlXdataDeletedName = "TD_X__DELETED";
public const string SqlXdataTimestampName = "TD_X__TIMESTAMP";
public const string SqlXdataCdateName = "TD_X__CDATE";
public const string SqlXdataCtimeName = "TD_X__CTIME";
public const string SqlXdataCagentName = "TD_X__CAGENT";
public const string SqlXdataCunitName = "TD_X__CUNIT";
public const string SqlXdataMdateName = "TD_X__MDATE";
public const string SqlXdataMtimeName = "TD_X__MTIME";
public const string SqlXdataMagentName = "TD_X__MAGENT";
public const string SqlXdataMunitName = "TD_X__MUNIT";
public const string SqlXdataHomeName = "TD_X__HOME";
public const string SqlXdataVersionName = "TD_X__VERSION";
public const string SqlXdataInactiveName = "TD_X__INACTIVE";
public const string SqlXdataExtattName = "TD_X__EXTATT";
public const string SqlXdataMrefName = "TD_X__MREF";
public const string SqlXdataSeqName = "TD_X__SEQ";
public const string SqlXdataDescrName = "TD_X__DESCR";
public const string SqlXdataFmtName = "TD_X__FMT";
public const string SqlXdataXdurlName = "TD_X__XDURL";
public const string SqlXdataXdatumName = "TD_X__XDATUM";
public const string SqlXdataTableNameExt = "TD_XDATA";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 114; //Internal Table Code
public const string TBAREA = "Controllo di gestione"; //Table Area
public const string PREFIX = "Td"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Td1Icode; } 
public override string labelText() { return $@"{TdCodice} - {TdDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(TdCodice)}</strong> {HttpUtility.HtmlEncode(TdDescrizione)}"; }

//2591-2591//[N] DIAGNOSI.DG_TIPO_DIAGNOSI
[Display(Name = "Diagnosi", ShortName = "", Description = "Classificazioni diagnostiche adottate nelle organizzazioni sanitarie (ad esempio, DRG, AVG, ICD9, ecc.)", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Costs.Diagnosi>? XrefDgTipoDiagnosi { get; set; } = null;
//3136-3131//[N] TIPO_DIAGNOSI.TD_ID_GRUPPO
[Display(Name = "TipoDiagnosi", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Costs.TipoDiagnosi>? XrefTdIdGruppo { get; set; } = null;
[Key]
[Display(Name = "Td1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("TD__ICODE", SqlFieldNameExt="TD__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Td1Icode { get; set; }
[Display(Name = "Td1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("TD__DELETED", SqlFieldNameExt="TD__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Td1Deleted { get; set; }
[Display(Name = "Td1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("TD__TIMESTAMP", SqlFieldNameExt="TD__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Td1Timestamp { get; set; }
[Display(Name = "Td1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TD__CDATE", SqlFieldNameExt="TD__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Td1Cdate { get; set; }
[Display(Name = "Td1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TD__CTIME", SqlFieldNameExt="TD__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Td1Ctime { get; set; }
[Display(Name = "Td1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TD__CAGENT", SqlFieldNameExt="TD__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Td1Cagent { get; set; }
[Display(Name = "Td1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TD__CUNIT", SqlFieldNameExt="TD__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Td1Cunit { get; set; }
[Display(Name = "Td1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TD__MDATE", SqlFieldNameExt="TD__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Td1Mdate { get; set; }
[Display(Name = "Td1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TD__MTIME", SqlFieldNameExt="TD__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Td1Mtime { get; set; }
[Display(Name = "Td1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TD__MAGENT", SqlFieldNameExt="TD__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Td1Magent { get; set; }
[Display(Name = "Td1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TD__MUNIT", SqlFieldNameExt="TD__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Td1Munit { get; set; }
[Display(Name = "Td1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("TD__HOME", SqlFieldNameExt="TD__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Td1Home { get; set; }
[Display(Name = "Td1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("TD__VERSION", SqlFieldNameExt="TD__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Td1Version { get; set; }
[Display(Name = "Td1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("TD__INACTIVE", SqlFieldNameExt="TD__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Td1Inactive { get; set; }
[Display(Name = "Td1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("TD__EXTATT", SqlFieldNameExt="TD__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Td1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TD_CODICE", SqlFieldNameExt="TD_CODICE", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_DIAGNOSI.TD__ICODE[TD__ICODE] {TD_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TdCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TD_DESCRIZIONE", SqlFieldNameExt="TD_DESCRIZIONE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TdDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TD_NOTE", SqlFieldNameExt="TD_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TdNote  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Superclasse che raggruppa la classificazione corrente", Prompt="")]
[ErpDogField("TD_ID_GRUPPO", SqlFieldNameExt="TD_ID_GRUPPO", SqlFieldOptions="", Xref="Td1Icode", SqlFieldProperties="prop() xref(TIPO_DIAGNOSI.TD__ICODE) xdup() multbxref()")]
[AutocompleteClient("TipoDiagnosi", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? TdIdGruppo  { get; set; }
public HealthDemo.Models.SIO.Costs.TipoDiagnosi? TdIdGruppoObj  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTd1Icode|K|TD__ICODE","sioTd1RecDate|N|TD__MDATE,TD__CDATE"
        ,"sioTdIdGruppo|N|TD_ID_GRUPPO"
        ,"sioTd1Versiontd1Deleted|U|TD__VERSION,TD__DELETED"
        ,"sioTdCodicetd1Versiontd1Deleted|U|TD_CODICE,TD__VERSION,TD__DELETED"
    };
}
}
}
