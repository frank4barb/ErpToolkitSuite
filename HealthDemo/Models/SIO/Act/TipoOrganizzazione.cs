using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class TipoOrganizzazione : ModelErp {
public const string Description = "Classificazione delle strutture";
public const string SqlTableName = "TIPO_ORGANIZZAZIONE";
public const string SqlTableNameExt = "TIPO_ORGANIZZAZIONE";
public const string SqlTableProperties = "";
public const string RowIdName = "Tz1Icode";
public const string SqlRowIdName = "TZ__ICODE";
public const string SqlRowIdNameExt = "TZ__ICODE";
public const string SqlPrefix = "TZ_";
public const string SqlPrefixExt = "TZ_";
public const string SqlXdataTableName = "TZ_XDATA";
public const string SqlXdataIcodeName = "TZ_X__ICODE";
public const string SqlXdataDeletedName = "TZ_X__DELETED";
public const string SqlXdataTimestampName = "TZ_X__TIMESTAMP";
public const string SqlXdataCdateName = "TZ_X__CDATE";
public const string SqlXdataCtimeName = "TZ_X__CTIME";
public const string SqlXdataCagentName = "TZ_X__CAGENT";
public const string SqlXdataCunitName = "TZ_X__CUNIT";
public const string SqlXdataMdateName = "TZ_X__MDATE";
public const string SqlXdataMtimeName = "TZ_X__MTIME";
public const string SqlXdataMagentName = "TZ_X__MAGENT";
public const string SqlXdataMunitName = "TZ_X__MUNIT";
public const string SqlXdataHomeName = "TZ_X__HOME";
public const string SqlXdataVersionName = "TZ_X__VERSION";
public const string SqlXdataInactiveName = "TZ_X__INACTIVE";
public const string SqlXdataExtattName = "TZ_X__EXTATT";
public const string SqlXdataMrefName = "TZ_X__MREF";
public const string SqlXdataSeqName = "TZ_X__SEQ";
public const string SqlXdataDescrName = "TZ_X__DESCR";
public const string SqlXdataFmtName = "TZ_X__FMT";
public const string SqlXdataXdurlName = "TZ_X__XDURL";
public const string SqlXdataXdatumName = "TZ_X__XDATUM";
public const string SqlXdataTableNameExt = "TZ_XDATA";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 91; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Tz"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Tz1Icode; } 
public override string labelText() { return $@"{TzCodice} - {TzDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(TzCodice)}</strong> {HttpUtility.HtmlEncode(TzDescrizione)}"; }

//1777-1769//[N] ORGANIZZAZIONE.OR_TIPO_ASSISTENZA
[Display(Name = "Organizzazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Common.Organizzazione>? XrefOrTipoAssistenza { get; set; } = null;
//2132-2128//[N] TIPO_ORGANIZZAZIONE.TZ_GRUPPO
[Display(Name = "TipoOrganizzazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.TipoOrganizzazione>? XrefTzGruppo { get; set; } = null;
[Key]
[Display(Name = "Tz1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("TZ__ICODE", SqlFieldNameExt="TZ__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tz1Icode { get; set; }
[Display(Name = "Tz1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("TZ__DELETED", SqlFieldNameExt="TZ__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Tz1Deleted { get; set; }
[Display(Name = "Tz1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("TZ__TIMESTAMP", SqlFieldNameExt="TZ__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Tz1Timestamp { get; set; }
[Display(Name = "Tz1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TZ__CDATE", SqlFieldNameExt="TZ__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Tz1Cdate { get; set; }
[Display(Name = "Tz1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TZ__CTIME", SqlFieldNameExt="TZ__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Tz1Ctime { get; set; }
[Display(Name = "Tz1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TZ__CAGENT", SqlFieldNameExt="TZ__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tz1Cagent { get; set; }
[Display(Name = "Tz1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TZ__CUNIT", SqlFieldNameExt="TZ__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tz1Cunit { get; set; }
[Display(Name = "Tz1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TZ__MDATE", SqlFieldNameExt="TZ__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Tz1Mdate { get; set; }
[Display(Name = "Tz1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TZ__MTIME", SqlFieldNameExt="TZ__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Tz1Mtime { get; set; }
[Display(Name = "Tz1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TZ__MAGENT", SqlFieldNameExt="TZ__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tz1Magent { get; set; }
[Display(Name = "Tz1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TZ__MUNIT", SqlFieldNameExt="TZ__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tz1Munit { get; set; }
[Display(Name = "Tz1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("TZ__HOME", SqlFieldNameExt="TZ__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tz1Home { get; set; }
[Display(Name = "Tz1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("TZ__VERSION", SqlFieldNameExt="TZ__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tz1Version { get; set; }
[Display(Name = "Tz1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("TZ__INACTIVE", SqlFieldNameExt="TZ__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Tz1Inactive { get; set; }
[Display(Name = "Tz1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("TZ__EXTATT", SqlFieldNameExt="TZ__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Tz1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TZ_CODICE", SqlFieldNameExt="TZ_CODICE", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_ORGANIZZAZIONE.TZ__ICODE[TZ__ICODE] {TZ_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TzCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TZ_DESCRIZIONE", SqlFieldNameExt="TZ_DESCRIZIONE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TzDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TZ_NOTE", SqlFieldNameExt="TZ_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TzNote  { get; set; }

[Display(Name = "Gruppo", ShortName="", Description = "Classe di aggregazione (se presente)", Prompt="")]
[ErpDogField("TZ_GRUPPO", SqlFieldNameExt="TZ_GRUPPO", SqlFieldOptions="", Xref="Tz1Icode", SqlFieldProperties="prop() xref(TIPO_ORGANIZZAZIONE.TZ__ICODE) xdup() multbxref()")]
[AutocompleteClient("TipoOrganizzazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? TzGruppo  { get; set; }
public HealthDemo.Models.SIO.Act.TipoOrganizzazione? TzGruppoObj  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Numero di sequenza nell'aggregazione (se presente)", Prompt="")]
[ErpDogField("TZ_SEQUENZA", SqlFieldNameExt="TZ_SEQUENZA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? TzSequenza  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTz1Icode|K|TZ__ICODE","sioTz1RecDate|N|TZ__MDATE,TZ__CDATE"
        ,"sioTz1Versiontz1Deleted|U|TZ__VERSION,TZ__DELETED"
        ,"sioTzCodicetz1Versiontz1Deleted|U|TZ_CODICE,TZ__VERSION,TZ__DELETED"
    };
}
}
}
