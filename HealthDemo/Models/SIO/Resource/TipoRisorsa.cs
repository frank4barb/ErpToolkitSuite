using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Resource {
public class TipoRisorsa : ModelErp {
public const string Description = "Tipi di risorse disponibili/utilizzate nell'organizzazione sanitaria";
public const string SqlTableName = "TIPO_RISORSA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Ts1Icode";
public const string SqlRowIdName = "TS__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "TS_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "TS_XDATA";
public const string SqlXdataIcodeName = "TS_X__ICODE";
public const string SqlXdataDeletedName = "TS_X__DELETED";
public const string SqlXdataTimestampName = "TS_X__TIMESTAMP";
public const string SqlXdataCdateName = "TS_X__CDATE";
public const string SqlXdataCtimeName = "TS_X__CTIME";
public const string SqlXdataCagentName = "TS_X__CAGENT";
public const string SqlXdataCunitName = "TS_X__CUNIT";
public const string SqlXdataMdateName = "TS_X__MDATE";
public const string SqlXdataMtimeName = "TS_X__MTIME";
public const string SqlXdataMagentName = "TS_X__MAGENT";
public const string SqlXdataMunitName = "TS_X__MUNIT";
public const string SqlXdataHomeName = "TS_X__HOME";
public const string SqlXdataVersionName = "TS_X__VERSION";
public const string SqlXdataInactiveName = "TS_X__INACTIVE";
public const string SqlXdataExtattName = "TS_X__EXTATT";
public const string SqlXdataMrefName = "TS_X__MREF";
public const string SqlXdataSeqName = "TS_X__SEQ";
public const string SqlXdataDescrName = "TS_X__DESCR";
public const string SqlXdataFmtName = "TS_X__FMT";
public const string SqlXdataXdurlName = "TS_X__XDURL";
public const string SqlXdataXdatumName = "TS_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 20; //Internal Table Code
public const string TBAREA = "Risorse"; //Table Area
public const string PREFIX = "Ts"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ts1Icode; } 
public override string labelText() { return $@"{TsCodice} - {TsDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(TsCodice)}</strong> {HttpUtility.HtmlEncode(TsDescrizione)}"; }

//126-124//[Y] REL_PRESTAZIONE_USA.PU_ID_TIPO_RISORSA
[Display(Name = "RelPrestazioneUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelPrestazioneUsa>? XrefPuIdTipoRisorsa { get; set; } = null;
//780-775//[N] FARMACO.FM_ID_TIPO_RISORSA
[Display(Name = "Farmaco", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Resource.Farmaco>? XrefFmIdTipoRisorsa { get; set; } = null;
//826-822//[N] ATTREZZATURA.AT_ID_TIPO_RISORSA
[Display(Name = "Attrezzatura", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Resource.Attrezzatura>? XrefAtIdTipoRisorsa { get; set; } = null;
//1063-1061//[N] SALA.SA_ID_TIPO_RISORSA
[Display(Name = "Sala", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Resource.Sala>? XrefSaIdTipoRisorsa { get; set; } = null;
//1096-1092//[N] MATERIALE.MT_ID_TIPO_RISORSA
[Display(Name = "Materiale", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Resource.Materiale>? XrefMtIdTipoRisorsa { get; set; } = null;
//1181-1179//[Y] REL_ATTIVITA_USA.AU_ID_TIPO_RISORSA
[Display(Name = "RelAttivitaUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaUsa>? XrefAuIdTipoRisorsa { get; set; } = null;
//2040-2036//[N] PERSONALE.PE_ID_TIPO_RISORSA
[Display(Name = "Personale", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Resource.Personale>? XrefPeIdTipoRisorsa { get; set; } = null;
//2136-2134//[N] TIPO_RISORSA.TS_ID_GRUPPO
[Display(Name = "TipoRisorsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Resource.TipoRisorsa>? XrefTsIdGruppo { get; set; } = null;
[Key]
[Display(Name = "Ts1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("TS__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ts1Icode { get; set; }
[Display(Name = "Ts1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("TS__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ts1Deleted { get; set; }
[Display(Name = "Ts1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("TS__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ts1Timestamp { get; set; }
[Display(Name = "Ts1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TS__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ts1Cdate { get; set; }
[Display(Name = "Ts1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TS__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ts1Ctime { get; set; }
[Display(Name = "Ts1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TS__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ts1Cagent { get; set; }
[Display(Name = "Ts1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TS__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ts1Cunit { get; set; }
[Display(Name = "Ts1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TS__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ts1Mdate { get; set; }
[Display(Name = "Ts1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TS__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ts1Mtime { get; set; }
[Display(Name = "Ts1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TS__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ts1Magent { get; set; }
[Display(Name = "Ts1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TS__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ts1Munit { get; set; }
[Display(Name = "Ts1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("TS__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ts1Home { get; set; }
[Display(Name = "Ts1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("TS__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ts1Version { get; set; }
[Display(Name = "Ts1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("TS__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ts1Inactive { get; set; }
[Display(Name = "Ts1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("TS__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ts1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TS_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS__ICODE[TS__ICODE] {TS_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TsCodice  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe: E[quipments] - L[ocations] - S[taff] - M[aterial] - [G]eneric", Prompt="")]
[ErpDogField("TS_CLASSE_RISORSA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("M")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "E", "L", "M", "D", "S", "G" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? TsClasseRisorsa  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Codice del super-tipo di risorsa (cioè l'aggregazione nella gerarchia), se presente", Prompt="")]
[ErpDogField("TS_ID_GRUPPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? TsIdGruppo  { get; set; }
public HealthDemo.Models.SIO.Resource.TipoRisorsa? TsIdGruppoObj  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TS_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TsDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TS_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TsNote  { get; set; }

[Display(Name = "Unita Di Misura", ShortName="", Description = "Unità di misura", Prompt="")]
[ErpDogField("TS_UNITA_DI_MISURA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TsUnitaDiMisura  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTs1Icode|K|TS__ICODE","sioTs1RecDate|N|TS__MDATE,TS__CDATE"
        ,"sioTsClasseRisorsats1Versionts1Deleted|U|TS_CLASSE_RISORSA,TS__VERSION,TS__DELETED"
        ,"sioTsIdGruppo|N|TS_ID_GRUPPO"
        ,"sioTsCodicets1Versionts1Deleted|U|TS_CODICE,TS__VERSION,TS__DELETED"
    };
}
}
}
