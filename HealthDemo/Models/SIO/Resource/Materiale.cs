using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Resource {
public class Materiale : ModelErp {
public const string Description = "Risorse: materiali";
public const string SqlTableName = "MATERIALE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Mt1Icode";
public const string SqlRowIdName = "MT__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "MT_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "MT_XDATA";
public const string SqlXdataIcodeName = "MT_X__ICODE";
public const string SqlXdataDeletedName = "MT_X__DELETED";
public const string SqlXdataTimestampName = "MT_X__TIMESTAMP";
public const string SqlXdataCdateName = "MT_X__CDATE";
public const string SqlXdataCtimeName = "MT_X__CTIME";
public const string SqlXdataCagentName = "MT_X__CAGENT";
public const string SqlXdataCunitName = "MT_X__CUNIT";
public const string SqlXdataMdateName = "MT_X__MDATE";
public const string SqlXdataMtimeName = "MT_X__MTIME";
public const string SqlXdataMagentName = "MT_X__MAGENT";
public const string SqlXdataMunitName = "MT_X__MUNIT";
public const string SqlXdataHomeName = "MT_X__HOME";
public const string SqlXdataVersionName = "MT_X__VERSION";
public const string SqlXdataInactiveName = "MT_X__INACTIVE";
public const string SqlXdataExtattName = "MT_X__EXTATT";
public const string SqlXdataMrefName = "MT_X__MREF";
public const string SqlXdataSeqName = "MT_X__SEQ";
public const string SqlXdataDescrName = "MT_X__DESCR";
public const string SqlXdataFmtName = "MT_X__FMT";
public const string SqlXdataXdurlName = "MT_X__XDURL";
public const string SqlXdataXdatumName = "MT_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 97; //Internal Table Code
public const string TBAREA = "Risorse"; //Table Area
public const string PREFIX = "Mt"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Mt1Icode; } 
public override string labelText() { return $@"{MtCodice} - {MtDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(MtCodice)}</strong> {HttpUtility.HtmlEncode(MtDescrizione)}"; }

//127-124//[Y] REL_PRESTAZIONE_USA.PU_ID_RISORSA
[Display(Name = "RelPrestazioneUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelPrestazioneUsa>? XrefPuIdRisorsa { get; set; } = null;
//1182-1179//[Y] REL_ATTIVITA_USA.AU_ID_RISORSA
[Display(Name = "RelAttivitaUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaUsa>? XrefAuIdRisorsa { get; set; } = null;
[Key]
[Display(Name = "Mt1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("MT__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Mt1Icode { get; set; }
[Display(Name = "Mt1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("MT__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Mt1Deleted { get; set; }
[Display(Name = "Mt1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("MT__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Mt1Timestamp { get; set; }
[Display(Name = "Mt1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("MT__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Mt1Cdate { get; set; }
[Display(Name = "Mt1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("MT__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Mt1Ctime { get; set; }
[Display(Name = "Mt1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("MT__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Mt1Cagent { get; set; }
[Display(Name = "Mt1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("MT__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Mt1Cunit { get; set; }
[Display(Name = "Mt1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("MT__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Mt1Mdate { get; set; }
[Display(Name = "Mt1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("MT__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Mt1Mtime { get; set; }
[Display(Name = "Mt1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("MT__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Mt1Magent { get; set; }
[Display(Name = "Mt1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("MT__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Mt1Munit { get; set; }
[Display(Name = "Mt1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("MT__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Mt1Home { get; set; }
[Display(Name = "Mt1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("MT__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Mt1Version { get; set; }
[Display(Name = "Mt1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("MT__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Mt1Inactive { get; set; }
[Display(Name = "Mt1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("MT__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Mt1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("MT_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(MATERIALE.MT__ICODE[MT__ICODE] {MT_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? MtCodice  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe di risorsa: M[aterial] (Materiale)", Prompt="")]
[ErpDogField("MT_CLASSE_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS_CLASSE_RISORSA[MATERIALE.MT_ID_TIPO_RISORSA]) multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue("M")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? MtClasseRisorsa  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("MT_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? MtDescrizione  { get; set; }

[Display(Name = "Id Tipo Risorsa", ShortName="", Description = "Codice del tipo di materiale, secondo la classificazione operativa definita in A_TYRESOUR con radice #SIO#MATER", Prompt="")]
[ErpDogField("MT_ID_TIPO_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? MtIdTipoRisorsa  { get; set; }
public HealthDemo.Models.SIO.Resource.TipoRisorsa? MtIdTipoRisorsaObj  { get; set; }

[Display(Name = "Costo Unitario Uso", ShortName="", Description = "Costo unitario per l'utilizzo", Prompt="")]
[ErpDogField("MT_COSTO_UNITARIO_USO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? MtCostoUnitarioUso  { get; set; }

[Display(Name = "Misura Unitaria Uso", ShortName="", Description = "Unità di misura per l'utilizzo", Prompt="")]
[ErpDogField("MT_MISURA_UNITARIA_USO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? MtMisuraUnitariaUso  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("MT_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? MtNote  { get; set; }

[Display(Name = "Disponibilita", ShortName="", Description = "Descrizione testuale dei criteri di disponibilità/consegna", Prompt="")]
[ErpDogField("MT_DISPONIBILITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? MtDisponibilita  { get; set; }

[Display(Name = "Telefono Fornitore", ShortName="", Description = "Numero di telefono del fornitore", Prompt="")]
[ErpDogField("MT_TELEFONO_FORNITORE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(15, ErrorMessage = "Inserire massimo 15 caratteri")]
[DataType(DataType.Text)]
public string? MtTelefonoFornitore  { get; set; }

[Display(Name = "Quantita Disponibile", ShortName="", Description = "Quantità attualmente disponibile", Prompt="")]
[ErpDogField("MT_QUANTITA_DISPONIBILE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? MtQuantitaDisponibile  { get; set; }

[Display(Name = "Quantita Minima Magazzino", ShortName="", Description = "Quantità minima richiesta", Prompt="")]
[ErpDogField("MT_QUANTITA_MINIMA_MAGAZZINO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? MtQuantitaMinimaMagazzino  { get; set; }

[Display(Name = "Uso Medio Giornaliero", ShortName="", Description = "Utilizzo medio al giorno (numero di unità)", Prompt="")]
[ErpDogField("MT_USO_MEDIO_GIORNALIERO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? MtUsoMedioGiornaliero  { get; set; }

[Display(Name = "Data Ultimo Ordine", ShortName="", Description = "Data dell'ultimo ordine", Prompt="")]
[ErpDogField("MT_DATA_ULTIMO_ORDINE", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? MtDataUltimoOrdine  { get; set; }

[Display(Name = "Quantita Ultimo Ordine", ShortName="", Description = "Quantità dell'ultimo ordine", Prompt="")]
[ErpDogField("MT_QUANTITA_ULTIMO_ORDINE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? MtQuantitaUltimoOrdine  { get; set; }

[Display(Name = "Data Prossimo Ordine", ShortName="", Description = "Data prevista per il prossimo ordine", Prompt="")]
[ErpDogField("MT_DATA_PROSSIMO_ORDINE", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? MtDataProssimoOrdine  { get; set; }

[Display(Name = "Quantita Media Ordine", ShortName="", Description = "Quantità media per ordine", Prompt="")]
[ErpDogField("MT_QUANTITA_MEDIA_ORDINE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? MtQuantitaMediaOrdine  { get; set; }

[Display(Name = "Codice Nazionale", ShortName="", Description = "Codice nazionale", Prompt="")]
[ErpDogField("MT_CODICE_NAZIONALE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? MtCodiceNazionale  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioMt1Icode|K|MT__ICODE","sioMt1RecDate|N|MT__MDATE,MT__CDATE"
        ,"sioMtIdTipoRisorsa|N|MT_ID_TIPO_RISORSA"
        ,"sioMtTelefonoFornitore|N|MT_TELEFONO_FORNITORE"
        ,"sioMt1Versionmt1Deleted|U|MT__VERSION,MT__DELETED"
        ,"sioMtCodicemt1Versionmt1Deleted|U|MT_CODICE,MT__VERSION,MT__DELETED"
    };
}
}
}
