using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.HealthData {
public class CategoriaDatoClinico : ModelErp {
public const string Description = "Classificazione dei tipi di dati sanitari";
public const string SqlTableName = "CATEGORIA_DATO_CLINICO";
public const string SqlTableNameExt = "CATEGORIA_DATO_CLINICO";
public const string SqlTableProperties = "";
public const string RowIdName = "Cc1Icode";
public const string SqlRowIdName = "CC__ICODE";
public const string SqlRowIdNameExt = "CC__ICODE";
public const string SqlPrefix = "CC_";
public const string SqlPrefixExt = "CC_";
public const string SqlXdataTableName = "CC_XDATA";
public const string SqlXdataIcodeName = "CC_X__ICODE";
public const string SqlXdataDeletedName = "CC_X__DELETED";
public const string SqlXdataTimestampName = "CC_X__TIMESTAMP";
public const string SqlXdataCdateName = "CC_X__CDATE";
public const string SqlXdataCtimeName = "CC_X__CTIME";
public const string SqlXdataCagentName = "CC_X__CAGENT";
public const string SqlXdataCunitName = "CC_X__CUNIT";
public const string SqlXdataMdateName = "CC_X__MDATE";
public const string SqlXdataMtimeName = "CC_X__MTIME";
public const string SqlXdataMagentName = "CC_X__MAGENT";
public const string SqlXdataMunitName = "CC_X__MUNIT";
public const string SqlXdataHomeName = "CC_X__HOME";
public const string SqlXdataVersionName = "CC_X__VERSION";
public const string SqlXdataInactiveName = "CC_X__INACTIVE";
public const string SqlXdataExtattName = "CC_X__EXTATT";
public const string SqlXdataMrefName = "CC_X__MREF";
public const string SqlXdataSeqName = "CC_X__SEQ";
public const string SqlXdataDescrName = "CC_X__DESCR";
public const string SqlXdataFmtName = "CC_X__FMT";
public const string SqlXdataXdurlName = "CC_X__XDURL";
public const string SqlXdataXdatumName = "CC_X__XDATUM";
public const string SqlXdataTableNameExt = "CC_XDATA";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 16; //Internal Table Code
public const string TBAREA = "Dati clinici"; //Table Area
public const string PREFIX = "Cc"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Cc1Icode; } 
public override string labelText() { return $@"{CcCodice} - {CcDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(CcCodice)}</strong> {HttpUtility.HtmlEncode(CcDescrizione)}"; }

//478-473//[N] CATEGORIA_DATO_CLINICO.CC_ID_GRUPPO
[Display(Name = "CategoriaDatoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.CategoriaDatoClinico>? XrefCcIdGruppo { get; set; } = null;
//847-845//[N] RISULTATO_ESAME.RE_ID_EPISODIO
[Display(Name = "RisultatoEsame", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.RisultatoEsame>? XrefReIdEpisodio { get; set; } = null;
//952-946//[N] TIPO_DATO_CLINICO.TC_ID_CATEGORIA_DATO_CLINICO
[Display(Name = "TipoDatoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.TipoDatoClinico>? XrefTcIdCategoriaDatoClinico { get; set; } = null;
//982-978//[N] STATO_SALUTE.SS_ID_GRUPPO_DATO_CLINICO
[Display(Name = "StatoSalute", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.StatoSalute>? XrefSsIdGruppoDatoClinico { get; set; } = null;
//1200-1196//[N] DOCUMENTO_CLINICO.DC_ID_GRUPPO_DATO_CLINICO
[Display(Name = "DocumentoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.DocumentoClinico>? XrefDcIdGruppoDatoClinico { get; set; } = null;
//2168-2164//[N] PARAMETRO_VITALE.PV_ID_GRUPPO_DATO_CLINICO
[Display(Name = "ParametroVitale", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.ParametroVitale>? XrefPvIdGruppoDatoClinico { get; set; } = null;
[Key]
[Display(Name = "Cc1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("CC__ICODE", SqlFieldNameExt="CC__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cc1Icode { get; set; }
[Display(Name = "Cc1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("CC__DELETED", SqlFieldNameExt="CC__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Cc1Deleted { get; set; }
[Display(Name = "Cc1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("CC__TIMESTAMP", SqlFieldNameExt="CC__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Cc1Timestamp { get; set; }
[Display(Name = "Cc1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("CC__CDATE", SqlFieldNameExt="CC__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Cc1Cdate { get; set; }
[Display(Name = "Cc1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("CC__CTIME", SqlFieldNameExt="CC__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Cc1Ctime { get; set; }
[Display(Name = "Cc1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("CC__CAGENT", SqlFieldNameExt="CC__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cc1Cagent { get; set; }
[Display(Name = "Cc1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("CC__CUNIT", SqlFieldNameExt="CC__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cc1Cunit { get; set; }
[Display(Name = "Cc1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("CC__MDATE", SqlFieldNameExt="CC__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Cc1Mdate { get; set; }
[Display(Name = "Cc1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("CC__MTIME", SqlFieldNameExt="CC__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Cc1Mtime { get; set; }
[Display(Name = "Cc1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("CC__MAGENT", SqlFieldNameExt="CC__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cc1Magent { get; set; }
[Display(Name = "Cc1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("CC__MUNIT", SqlFieldNameExt="CC__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cc1Munit { get; set; }
[Display(Name = "Cc1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("CC__HOME", SqlFieldNameExt="CC__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cc1Home { get; set; }
[Display(Name = "Cc1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("CC__VERSION", SqlFieldNameExt="CC__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cc1Version { get; set; }
[Display(Name = "Cc1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("CC__INACTIVE", SqlFieldNameExt="CC__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Cc1Inactive { get; set; }
[Display(Name = "Cc1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("CC__EXTATT", SqlFieldNameExt="CC__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Cc1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("CC_CODICE", SqlFieldNameExt="CC_CODICE", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(CATEGORIA_DATO_CLINICO.CC__ICODE[CC__ICODE] {CC_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? CcCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("CC_DESCRIZIONE", SqlFieldNameExt="CC_DESCRIZIONE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? CcDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("CC_NOTE", SqlFieldNameExt="CC_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? CcNote  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Codice della superclasse che raggruppa la classe attuale", Prompt="")]
[ErpDogField("CC_ID_GRUPPO", SqlFieldNameExt="CC_ID_GRUPPO", SqlFieldOptions="", Xref="Cc1Icode", SqlFieldProperties="prop() xref(CATEGORIA_DATO_CLINICO.CC__ICODE) xdup() multbxref()")]
[AutocompleteClient("CategoriaDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? CcIdGruppo  { get; set; }
public HealthDemo.Models.SIO.HealthData.CategoriaDatoClinico? CcIdGruppoObj  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioCc1Icode|K|CC__ICODE","sioCc1RecDate|N|CC__MDATE,CC__CDATE"
        ,"sioCcIdGruppo|N|CC_ID_GRUPPO"
        ,"sioCc1Versioncc1Deleted|U|CC__VERSION,CC__DELETED"
        ,"sioCcCodicecc1Versioncc1Deleted|U|CC_CODICE,CC__VERSION,CC__DELETED"
    };
}
}
}
