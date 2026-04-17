using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Resource {
public class Farmaco : ModelErp {
public const string Description = "Risorse: farmaci";
public const string SqlTableName = "FARMACO";
public const string SqlTableNameExt = "FARMACO";
public const string SqlTableProperties = "";
public const string RowIdName = "Fm1Icode";
public const string SqlRowIdName = "FM__ICODE";
public const string SqlRowIdNameExt = "FM__ICODE";
public const string SqlPrefix = "FM_";
public const string SqlPrefixExt = "FM_";
public const string SqlXdataTableName = "FM_XDATA";
public const string SqlXdataIcodeName = "FM_X__ICODE";
public const string SqlXdataDeletedName = "FM_X__DELETED";
public const string SqlXdataTimestampName = "FM_X__TIMESTAMP";
public const string SqlXdataCdateName = "FM_X__CDATE";
public const string SqlXdataCtimeName = "FM_X__CTIME";
public const string SqlXdataCagentName = "FM_X__CAGENT";
public const string SqlXdataCunitName = "FM_X__CUNIT";
public const string SqlXdataMdateName = "FM_X__MDATE";
public const string SqlXdataMtimeName = "FM_X__MTIME";
public const string SqlXdataMagentName = "FM_X__MAGENT";
public const string SqlXdataMunitName = "FM_X__MUNIT";
public const string SqlXdataHomeName = "FM_X__HOME";
public const string SqlXdataVersionName = "FM_X__VERSION";
public const string SqlXdataInactiveName = "FM_X__INACTIVE";
public const string SqlXdataExtattName = "FM_X__EXTATT";
public const string SqlXdataMrefName = "FM_X__MREF";
public const string SqlXdataSeqName = "FM_X__SEQ";
public const string SqlXdataDescrName = "FM_X__DESCR";
public const string SqlXdataFmtName = "FM_X__FMT";
public const string SqlXdataXdurlName = "FM_X__XDURL";
public const string SqlXdataXdatumName = "FM_X__XDATUM";
public const string SqlXdataTableNameExt = "FM_XDATA";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 98; //Internal Table Code
public const string TBAREA = "Risorse"; //Table Area
public const string PREFIX = "Fm"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Fm1Icode; } 
public override string labelText() { return $@"{FmCodice} - {FmDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(FmCodice)}</strong> {HttpUtility.HtmlEncode(FmDescrizione)}"; }

//127-124//[Y] REL_PRESTAZIONE_USA.PU_ID_RISORSA
[Display(Name = "RelPrestazioneUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelPrestazioneUsa>? XrefPuIdRisorsa { get; set; } = null;
//1182-1179//[Y] REL_ATTIVITA_USA.AU_ID_RISORSA
[Display(Name = "RelAttivitaUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaUsa>? XrefAuIdRisorsa { get; set; } = null;
[Key]
[Display(Name = "Fm1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("FM__ICODE", SqlFieldNameExt="FM__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Fm1Icode { get; set; }
[Display(Name = "Fm1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("FM__DELETED", SqlFieldNameExt="FM__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Fm1Deleted { get; set; }
[Display(Name = "Fm1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("FM__TIMESTAMP", SqlFieldNameExt="FM__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Fm1Timestamp { get; set; }
[Display(Name = "Fm1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("FM__CDATE", SqlFieldNameExt="FM__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Fm1Cdate { get; set; }
[Display(Name = "Fm1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("FM__CTIME", SqlFieldNameExt="FM__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Fm1Ctime { get; set; }
[Display(Name = "Fm1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("FM__CAGENT", SqlFieldNameExt="FM__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Fm1Cagent { get; set; }
[Display(Name = "Fm1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("FM__CUNIT", SqlFieldNameExt="FM__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Fm1Cunit { get; set; }
[Display(Name = "Fm1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("FM__MDATE", SqlFieldNameExt="FM__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Fm1Mdate { get; set; }
[Display(Name = "Fm1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("FM__MTIME", SqlFieldNameExt="FM__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Fm1Mtime { get; set; }
[Display(Name = "Fm1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("FM__MAGENT", SqlFieldNameExt="FM__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Fm1Magent { get; set; }
[Display(Name = "Fm1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("FM__MUNIT", SqlFieldNameExt="FM__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Fm1Munit { get; set; }
[Display(Name = "Fm1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("FM__HOME", SqlFieldNameExt="FM__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Fm1Home { get; set; }
[Display(Name = "Fm1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("FM__VERSION", SqlFieldNameExt="FM__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Fm1Version { get; set; }
[Display(Name = "Fm1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("FM__INACTIVE", SqlFieldNameExt="FM__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Fm1Inactive { get; set; }
[Display(Name = "Fm1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("FM__EXTATT", SqlFieldNameExt="FM__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Fm1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("FM_CODICE", SqlFieldNameExt="FM_CODICE", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(FARMACO.FM__ICODE[FM__ICODE] {FM_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? FmCodice  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe di risorsa D: farmaci", Prompt="")]
[ErpDogField("FM_CLASSE_RISORSA", SqlFieldNameExt="FM_CLASSE_RISORSA", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS_CLASSE_RISORSA[FARMACO.FM_ID_TIPO_RISORSA]) multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue("D")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? FmClasseRisorsa  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("FM_DESCRIZIONE", SqlFieldNameExt="FM_DESCRIZIONE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? FmDescrizione  { get; set; }

[Display(Name = "Id Tipo Risorsa", ShortName="", Description = "Codice del tipo di farmaco", Prompt="")]
[ErpDogField("FM_ID_TIPO_RISORSA", SqlFieldNameExt="FM_ID_TIPO_RISORSA", SqlFieldOptions="[MANDATORY]", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? FmIdTipoRisorsa  { get; set; }
public HealthDemo.Models.SIO.Resource.TipoRisorsa? FmIdTipoRisorsaObj  { get; set; }

[Display(Name = "Costo Unitario Uso", ShortName="", Description = "Costo unitario per l'utilizzo", Prompt="")]
[ErpDogField("FM_COSTO_UNITARIO_USO", SqlFieldNameExt="FM_COSTO_UNITARIO_USO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? FmCostoUnitarioUso  { get; set; }

[Display(Name = "Misura Unitaria Uso", ShortName="", Description = "Unità di misura per l'utilizzo", Prompt="")]
[ErpDogField("FM_MISURA_UNITARIA_USO", SqlFieldNameExt="FM_MISURA_UNITARIA_USO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? FmMisuraUnitariaUso  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("FM_NOTE", SqlFieldNameExt="FM_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? FmNote  { get; set; }

[Display(Name = "Disponibilita", ShortName="", Description = "Descrizione testuale dello stato attuale di disponibilità", Prompt="")]
[ErpDogField("FM_DISPONIBILITA", SqlFieldNameExt="FM_DISPONIBILITA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? FmDisponibilita  { get; set; }

[Display(Name = "Quantita Disponibile", ShortName="", Description = "Quantità attualmente disponibile", Prompt="")]
[ErpDogField("FM_QUANTITA_DISPONIBILE", SqlFieldNameExt="FM_QUANTITA_DISPONIBILE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? FmQuantitaDisponibile  { get; set; }

[Display(Name = "Quantita Minima Magazzino", ShortName="", Description = "Quantità minima che deve essere disponibile", Prompt="")]
[ErpDogField("FM_QUANTITA_MINIMA_MAGAZZINO", SqlFieldNameExt="FM_QUANTITA_MINIMA_MAGAZZINO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? FmQuantitaMinimaMagazzino  { get; set; }

[Display(Name = "Uso Medio Giornaliero", ShortName="", Description = "Utilizzo medio al giorno (numero di unità)", Prompt="")]
[ErpDogField("FM_USO_MEDIO_GIORNALIERO", SqlFieldNameExt="FM_USO_MEDIO_GIORNALIERO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? FmUsoMedioGiornaliero  { get; set; }

[Display(Name = "Data Ultimo Ordine", ShortName="", Description = "Data dell'ultimo ordine", Prompt="")]
[ErpDogField("FM_DATA_ULTIMO_ORDINE", SqlFieldNameExt="FM_DATA_ULTIMO_ORDINE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? FmDataUltimoOrdine  { get; set; }

[Display(Name = "Quantita Ultimo Ordine", ShortName="", Description = "Quantità dell'ultimo ordine", Prompt="")]
[ErpDogField("FM_QUANTITA_ULTIMO_ORDINE", SqlFieldNameExt="FM_QUANTITA_ULTIMO_ORDINE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? FmQuantitaUltimoOrdine  { get; set; }

[Display(Name = "Data Prossimo Ordine", ShortName="", Description = "Data prevista per il prossimo ordine", Prompt="")]
[ErpDogField("FM_DATA_PROSSIMO_ORDINE", SqlFieldNameExt="FM_DATA_PROSSIMO_ORDINE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? FmDataProssimoOrdine  { get; set; }

[Display(Name = "Quantita Media Ordine", ShortName="", Description = "Quantità media per ordine", Prompt="")]
[ErpDogField("FM_QUANTITA_MEDIA_ORDINE", SqlFieldNameExt="FM_QUANTITA_MEDIA_ORDINE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? FmQuantitaMediaOrdine  { get; set; }

[Display(Name = "Riferimenti Fornitore", ShortName="", Description = "Riferimento per il fornitore", Prompt="")]
[ErpDogField("FM_RIFERIMENTI_FORNITORE", SqlFieldNameExt="FM_RIFERIMENTI_FORNITORE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? FmRiferimentiFornitore  { get; set; }

[Display(Name = "Codice Nazionale", ShortName="", Description = "Codice nazionale", Prompt="")]
[ErpDogField("FM_CODICE_NAZIONALE", SqlFieldNameExt="FM_CODICE_NAZIONALE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? FmCodiceNazionale  { get; set; }

[Display(Name = "Codice Aic", ShortName="", Description = "Codice ministeriale in base 10", Prompt="")]
[ErpDogField("FM_CODICE_AIC", SqlFieldNameExt="FM_CODICE_AIC", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? FmCodiceAic  { get; set; }

[Display(Name = "Data Inizio Autorizzazione", ShortName="", Description = "Data di inizio dell'autorizzazione governativa", Prompt="")]
[ErpDogField("FM_DATA_INIZIO_AUTORIZZAZIONE", SqlFieldNameExt="FM_DATA_INIZIO_AUTORIZZAZIONE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? FmDataInizioAutorizzazione  { get; set; }

[Display(Name = "Data Fine Autorizzazione", ShortName="", Description = "Data di fine dell'autorizzazione governativa", Prompt="")]
[ErpDogField("FM_DATA_FINE_AUTORIZZAZIONE", SqlFieldNameExt="FM_DATA_FINE_AUTORIZZAZIONE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? FmDataFineAutorizzazione  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioFm1Icode|K|FM__ICODE","sioFm1RecDate|N|FM__MDATE,FM__CDATE"
        ,"sioFmIdTipoRisorsa|N|FM_ID_TIPO_RISORSA"
        ,"sioFm1Versionfm1Deleted|U|FM__VERSION,FM__DELETED"
        ,"sioFmCodicefm1Versionfm1Deleted|U|FM_CODICE,FM__VERSION,FM__DELETED"
    };
}
}
}
