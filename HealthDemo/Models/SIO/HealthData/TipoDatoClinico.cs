using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.HealthData {
public class TipoDatoClinico : ModelErp {
public const string Description = "Classificazioni generali dei tipi di dati sanitari";
public const string SqlTableName = "TIPO_DATO_CLINICO";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Tc1Icode";
public const string SqlRowIdName = "TC__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "TC_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "TC_XDATA";
public const string SqlXdataIcodeName = "TC_X__ICODE";
public const string SqlXdataDeletedName = "TC_X__DELETED";
public const string SqlXdataTimestampName = "TC_X__TIMESTAMP";
public const string SqlXdataCdateName = "TC_X__CDATE";
public const string SqlXdataCtimeName = "TC_X__CTIME";
public const string SqlXdataCagentName = "TC_X__CAGENT";
public const string SqlXdataCunitName = "TC_X__CUNIT";
public const string SqlXdataMdateName = "TC_X__MDATE";
public const string SqlXdataMtimeName = "TC_X__MTIME";
public const string SqlXdataMagentName = "TC_X__MAGENT";
public const string SqlXdataMunitName = "TC_X__MUNIT";
public const string SqlXdataHomeName = "TC_X__HOME";
public const string SqlXdataVersionName = "TC_X__VERSION";
public const string SqlXdataInactiveName = "TC_X__INACTIVE";
public const string SqlXdataExtattName = "TC_X__EXTATT";
public const string SqlXdataMrefName = "TC_X__MREF";
public const string SqlXdataSeqName = "TC_X__SEQ";
public const string SqlXdataDescrName = "TC_X__DESCR";
public const string SqlXdataFmtName = "TC_X__FMT";
public const string SqlXdataXdurlName = "TC_X__XDURL";
public const string SqlXdataXdatumName = "TC_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 14; //Internal Table Code
public const string TBAREA = "Dati clinici"; //Table Area
public const string PREFIX = "Tc"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Tc1Icode; } 
public override string labelText() { return $@"{TcCodice} - {TcDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(TcCodice)}</strong> {HttpUtility.HtmlEncode(TcDescrizione)}"; }

//849-845//[N] RISULTATO_ESAME.RE_ID_GRUPPO_DATO_CLINICO
[Display(Name = "RisultatoEsame", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.RisultatoEsame>? XrefReIdGruppoDatoClinico { get; set; } = null;
//967-946//[N] TIPO_DATO_CLINICO.TC_ID_GRUPPO
[Display(Name = "TipoDatoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.TipoDatoClinico>? XrefTcIdGruppo { get; set; } = null;
//981-978//[N] STATO_SALUTE.SS_ID_TIPO_DATO_CLINICO
[Display(Name = "StatoSalute", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.StatoSalute>? XrefSsIdTipoDatoClinico { get; set; } = null;
//1199-1196//[N] DOCUMENTO_CLINICO.DC_ID_TIPO_DATO_CLINICO
[Display(Name = "DocumentoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.DocumentoClinico>? XrefDcIdTipoDatoClinico { get; set; } = null;
//2167-2164//[N] PARAMETRO_VITALE.PV_ID_TIPO_DATO_CLINICO
[Display(Name = "ParametroVitale", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.ParametroVitale>? XrefPvIdTipoDatoClinico { get; set; } = null;
[Key]
[Display(Name = "Tc1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("TC__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tc1Icode { get; set; }
[Display(Name = "Tc1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("TC__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Tc1Deleted { get; set; }
[Display(Name = "Tc1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("TC__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Tc1Timestamp { get; set; }
[Display(Name = "Tc1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TC__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Tc1Cdate { get; set; }
[Display(Name = "Tc1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TC__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Tc1Ctime { get; set; }
[Display(Name = "Tc1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TC__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tc1Cagent { get; set; }
[Display(Name = "Tc1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TC__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tc1Cunit { get; set; }
[Display(Name = "Tc1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TC__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Tc1Mdate { get; set; }
[Display(Name = "Tc1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TC__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Tc1Mtime { get; set; }
[Display(Name = "Tc1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TC__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tc1Magent { get; set; }
[Display(Name = "Tc1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TC__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tc1Munit { get; set; }
[Display(Name = "Tc1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("TC__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tc1Home { get; set; }
[Display(Name = "Tc1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("TC__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tc1Version { get; set; }
[Display(Name = "Tc1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("TC__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Tc1Inactive { get; set; }
[Display(Name = "Tc1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("TC__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Tc1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TC_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_DATO_CLINICO.TC__ICODE[TC__ICODE] {TC_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TcCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TC_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TcDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TC_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TcNote  { get; set; }

[Display(Name = "Id Categoria Dato Clinico", ShortName="", Description = "Codice della classe dell'elemento del record sanitario", Prompt="")]
[ErpDogField("TC_ID_CATEGORIA_DATO_CLINICO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Cc1Icode", SqlFieldProperties="prop() xref(CATEGORIA_DATO_CLINICO.CC__ICODE) xdup() multbxref()")]
[AutocompleteClient("CategoriaDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? TcIdCategoriaDatoClinico  { get; set; }
public HealthDemo.Models.SIO.HealthData.CategoriaDatoClinico? TcIdCategoriaDatoClinicoObj  { get; set; }

[Display(Name = "Unita Di Misura", ShortName="", Description = "Unità di misura", Prompt="")]
[ErpDogField("TC_UNITA_DI_MISURA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TcUnitaDiMisura  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Codice del tipo aggregato di HRI di cui questo elemento fa parte", Prompt="")]
[ErpDogField("TC_ID_GRUPPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Tc1Icode", SqlFieldProperties="prop() xref(TIPO_DATO_CLINICO.TC__ICODE) xdup() multbxref()")]
[AutocompleteClient("TipoDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? TcIdGruppo  { get; set; }
public HealthDemo.Models.SIO.HealthData.TipoDatoClinico? TcIdGruppoObj  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Ordine sequenziale degli HD aggregati (se presente)", Prompt="")]
[ErpDogField("TC_SEQUENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? TcSequenza  { get; set; }

[Display(Name = "Attributi1", ShortName="", Description = "Flag operativi, gestiti dall'applicazione", Prompt="")]
[ErpDogField("TC_ATTRIBUTI1", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(240, ErrorMessage = "Inserire massimo 240 caratteri")]
[DataType(DataType.Text)]
public string? TcAttributi1  { get; set; }

[Display(Name = "Attributi2", ShortName="", Description = "Ulteriori flag operativi, gestiti dalle applicazioni", Prompt="")]
[ErpDogField("TC_ATTRIBUTI2", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(240, ErrorMessage = "Inserire massimo 240 caratteri")]
[DataType(DataType.Text)]
public string? TcAttributi2  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTc1Icode|K|TC__ICODE","sioTc1RecDate|N|TC__MDATE,TC__CDATE"
        ,"sioTcIdCategoriaDatoClinico|N|TC_ID_CATEGORIA_DATO_CLINICO"
        ,"sioTcIdGruppotcSequenza|N|TC_ID_GRUPPO,TC_SEQUENZA"
        ,"sioTc1Versiontc1Deleted|U|TC__VERSION,TC__DELETED"
        ,"sioTcCodicetc1Versiontc1Deleted|U|TC_CODICE,TC__VERSION,TC__DELETED"
    };
}
}
}
