using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Patient {
public class Paziente : ModelErp {
public const string Description = "Pazienti rilevanti per l'organizzazione sanitaria";
public const string SqlTableName = "PAZIENTE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Pa1Icode";
public const string SqlRowIdName = "PA__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "PA_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "PA_XDATA";
public const string SqlXdataIcodeName = "PA_X__ICODE";
public const string SqlXdataDeletedName = "PA_X__DELETED";
public const string SqlXdataTimestampName = "PA_X__TIMESTAMP";
public const string SqlXdataCdateName = "PA_X__CDATE";
public const string SqlXdataCtimeName = "PA_X__CTIME";
public const string SqlXdataCagentName = "PA_X__CAGENT";
public const string SqlXdataCunitName = "PA_X__CUNIT";
public const string SqlXdataMdateName = "PA_X__MDATE";
public const string SqlXdataMtimeName = "PA_X__MTIME";
public const string SqlXdataMagentName = "PA_X__MAGENT";
public const string SqlXdataMunitName = "PA_X__MUNIT";
public const string SqlXdataHomeName = "PA_X__HOME";
public const string SqlXdataVersionName = "PA_X__VERSION";
public const string SqlXdataInactiveName = "PA_X__INACTIVE";
public const string SqlXdataExtattName = "PA_X__EXTATT";
public const string SqlXdataMrefName = "PA_X__MREF";
public const string SqlXdataSeqName = "PA_X__SEQ";
public const string SqlXdataDescrName = "PA_X__DESCR";
public const string SqlXdataFmtName = "PA_X__FMT";
public const string SqlXdataXdurlName = "PA_X__XDURL";
public const string SqlXdataXdatumName = "PA_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 51; //Internal Table Code
public const string TBAREA = "Accoglienza"; //Table Area
public const string PREFIX = "Pa"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Pa1Icode; } 
public override string labelText() { return $@"{PaCodSanitario} - {PaCognome} - {PaNome}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(PaCodSanitario)}</strong> {HttpUtility.HtmlEncode(PaCognome)} - {HttpUtility.HtmlEncode(PaNome)}"; }

//9-2//[N] PRESTAZIONE.PR_ID_PAZIENTE
[Display(Name = "Prestazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Prestazione>? XrefPrIdPaziente { get; set; } = null;
//562-524//[N] RICHIESTA.RI_ID_PAZIENTE
[Display(Name = "Richiesta", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Common.Richiesta>? XrefRiIdPaziente { get; set; } = null;
//596-593//[N] EPISODIO.EP_ID_PAZIENTE
[Display(Name = "Episodio", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Episodio>? XrefEpIdPaziente { get; set; } = null;
//846-845//[N] RISULTATO_ESAME.RE_ID_PAZIENTE
[Display(Name = "RisultatoEsame", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.RisultatoEsame>? XrefReIdPaziente { get; set; } = null;
//979-978//[N] STATO_SALUTE.SS_ID_PAZIENTE
[Display(Name = "StatoSalute", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.StatoSalute>? XrefSsIdPaziente { get; set; } = null;
//1197-1196//[N] DOCUMENTO_CLINICO.DC_ID_PAZIENTE
[Display(Name = "DocumentoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.DocumentoClinico>? XrefDcIdPaziente { get; set; } = null;
//2165-2164//[N] PARAMETRO_VITALE.PV_ID_PAZIENTE
[Display(Name = "ParametroVitale", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.ParametroVitale>? XrefPvIdPaziente { get; set; } = null;
[Key]
[Display(Name = "Pa1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("PA__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pa1Icode { get; set; }
[Display(Name = "Pa1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("PA__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pa1Deleted { get; set; }
[Display(Name = "Pa1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("PA__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Pa1Timestamp { get; set; }
[Display(Name = "Pa1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PA__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pa1Cdate { get; set; }
[Display(Name = "Pa1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PA__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pa1Ctime { get; set; }
[Display(Name = "Pa1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PA__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pa1Cagent { get; set; }
[Display(Name = "Pa1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PA__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pa1Cunit { get; set; }
[Display(Name = "Pa1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PA__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pa1Mdate { get; set; }
[Display(Name = "Pa1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PA__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pa1Mtime { get; set; }
[Display(Name = "Pa1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PA__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pa1Magent { get; set; }
[Display(Name = "Pa1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PA__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pa1Munit { get; set; }
[Display(Name = "Pa1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("PA__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pa1Home { get; set; }
[Display(Name = "Pa1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("PA__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pa1Version { get; set; }
[Display(Name = "Pa1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("PA__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pa1Inactive { get; set; }
[Display(Name = "Pa1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("PA__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Pa1Extatt { get; set; }


[Display(Name = "Cod Fiscale", ShortName="", Description = "Identificatore nazionale del paziente/individuo", Prompt="")]
[ErpDogField("PA_COD_FISCALE", SqlFieldNameExt="", SqlFieldOptions="[XID]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(16, ErrorMessage = "Inserire massimo 16 caratteri")]
[DataType(DataType.Text)]
public string? PaCodFiscale  { get; set; }

[Display(Name = "Cod Sanitario", ShortName="", Description = "Identificatore permanente del paziente nell'organizzazione sanitaria", Prompt="")]
[ErpDogField("PA_COD_SANITARIO", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(PAZIENTE.PA__ICODE[PA__ICODE] {PA_COD_SANITARIO=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(16, ErrorMessage = "Inserire massimo 16 caratteri")]
[DataType(DataType.Text)]
public string? PaCodSanitario  { get; set; }

[Display(Name = "Nome", ShortName="", Description = "Nome del paziente", Prompt="")]
[ErpDogField("PA_NOME", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? PaNome  { get; set; }

[Display(Name = "Cognome", ShortName="", Description = "Cognome del paziente", Prompt="")]
[ErpDogField("PA_COGNOME", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? PaCognome  { get; set; }

[Display(Name = "Sesso", ShortName="", Description = "Sesso M / F / N", Prompt="")]
[ErpDogField("PA_SESSO", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "M", "F", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? PaSesso  { get; set; }

[Display(Name = "Data Nascita", ShortName="", Description = "Data di nascita", Prompt="")]
[ErpDogField("PA_DATA_NASCITA", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? PaDataNascita  { get; set; }

[Display(Name = "Ora Nascita", ShortName="", Description = "Ora di nascita", Prompt="")]
[ErpDogField("PA_ORA_NASCITA", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? PaOraNascita  { get; set; }

[Display(Name = "Id Comune Nascita", ShortName="", Description = "Codice del comune di nascita", Prompt="")]
[ErpDogField("PA_ID_COMUNE_NASCITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="Cm1Icode", SqlFieldProperties="prop() xref(COMUNE.CM__ICODE) xdup() multbxref()")]
[AutocompleteClient("Comune", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdComuneNascita  { get; set; }
public HealthDemo.Models.SIO.Patient.Comune? PaIdComuneNascitaObj  { get; set; }

[Display(Name = "Id Nazione Nascita", ShortName="", Description = "Codice del paese di nascita", Prompt="")]
[ErpDogField("PA_ID_NAZIONE_NASCITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="Nz1Icode", SqlFieldProperties="prop() xref(NAZIONE.NZ__ICODE) xdup() multbxref()")]
[AutocompleteClient("Nazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdNazioneNascita  { get; set; }
public HealthDemo.Models.SIO.Patient.Nazione? PaIdNazioneNascitaObj  { get; set; }

[Display(Name = "Id Cittadinanza", ShortName="", Description = "Codice del paese di cittadinanza", Prompt="")]
[ErpDogField("PA_ID_CITTADINANZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="Nz1Icode", SqlFieldProperties="prop() xref(NAZIONE.NZ__ICODE) xdup() multbxref()")]
[AutocompleteClient("Nazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdCittadinanza  { get; set; }
public HealthDemo.Models.SIO.Patient.Nazione? PaIdCittadinanzaObj  { get; set; }

[Display(Name = "Indirizzo Res", ShortName="", Description = "Indirizzo legale: strada (linea 1)", Prompt="")]
[ErpDogField("PA_INDIRIZZO_RES", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(40, ErrorMessage = "Inserire massimo 40 caratteri")]
[DataType(DataType.Text)]
public string? PaIndirizzoRes  { get; set; }

[Display(Name = "Num Civico Res", ShortName="", Description = "Indirizzo legale: numero civico", Prompt="")]
[ErpDogField("PA_NUM_CIVICO_RES", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(5, ErrorMessage = "Inserire massimo 5 caratteri")]
[DataType(DataType.Text)]
public string? PaNumCivicoRes  { get; set; }

[Display(Name = "Cap Res", ShortName="", Description = "Indirizzo legale: codice postale", Prompt="")]
[ErpDogField("PA_CAP_RES", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(7, ErrorMessage = "Inserire massimo 7 caratteri")]
[DataType(DataType.Text)]
public string? PaCapRes  { get; set; }

[Display(Name = "Id Comune Res", ShortName="", Description = "Indirizzo legale: codice del comune", Prompt="")]
[ErpDogField("PA_ID_COMUNE_RES", SqlFieldNameExt="", SqlFieldOptions="", Xref="Cm1Icode", SqlFieldProperties="prop() xref(COMUNE.CM__ICODE) xdup() multbxref()")]
[AutocompleteClient("Comune", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdComuneRes  { get; set; }
public HealthDemo.Models.SIO.Patient.Comune? PaIdComuneResObj  { get; set; }

[Display(Name = "Id Distretto Res", ShortName="", Description = "Indirizzo legale : Codice di distretto", Prompt="")]
[ErpDogField("PA_ID_DISTRETTO_RES", SqlFieldNameExt="", SqlFieldOptions="", Xref="Di1Icode", SqlFieldProperties="prop() xref(DISTRETTO.DI__ICODE) xdup() multbxref()")]
[AutocompleteClient("Distretto", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdDistrettoRes  { get; set; }
public HealthDemo.Models.SIO.Patient.Distretto? PaIdDistrettoResObj  { get; set; }

[Display(Name = "Id Nazione Dom", ShortName="", Description = "Codice del paese in cui il paziente risiede", Prompt="")]
[ErpDogField("PA_ID_NAZIONE_DOM", SqlFieldNameExt="", SqlFieldOptions="", Xref="Nz1Icode", SqlFieldProperties="prop() xref(NAZIONE.NZ__ICODE) xdup() multbxref()")]
[AutocompleteClient("Nazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdNazioneDom  { get; set; }
public HealthDemo.Models.SIO.Patient.Nazione? PaIdNazioneDomObj  { get; set; }

[Display(Name = "Mail", ShortName="", Description = "Indirizzo email del paziente", Prompt="")]
[ErpDogField("PA_MAIL", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? PaMail  { get; set; }

[Display(Name = "Indirizzo Dom", ShortName="", Description = "Indirizzo di residenza: strada (linea 1)", Prompt="")]
[ErpDogField("PA_INDIRIZZO_DOM", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(40, ErrorMessage = "Inserire massimo 40 caratteri")]
[DataType(DataType.Text)]
public string? PaIndirizzoDom  { get; set; }

[Display(Name = "Num Civico Dom", ShortName="", Description = "Indirizzo di residenza: numero civico", Prompt="")]
[ErpDogField("PA_NUM_CIVICO_DOM", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(5, ErrorMessage = "Inserire massimo 5 caratteri")]
[DataType(DataType.Text)]
public string? PaNumCivicoDom  { get; set; }

[Display(Name = "Cap Dom", ShortName="", Description = "Indirizzo di residenza: codice postale", Prompt="")]
[ErpDogField("PA_CAP_DOM", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(7, ErrorMessage = "Inserire massimo 7 caratteri")]
[DataType(DataType.Text)]
public string? PaCapDom  { get; set; }

[Display(Name = "Id Comune Dom", ShortName="", Description = "Indirizzo di residenza: codice del comune", Prompt="")]
[ErpDogField("PA_ID_COMUNE_DOM", SqlFieldNameExt="", SqlFieldOptions="", Xref="Cm1Icode", SqlFieldProperties="prop() xref(COMUNE.CM__ICODE) xdup() multbxref()")]
[AutocompleteClient("Comune", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdComuneDom  { get; set; }
public HealthDemo.Models.SIO.Patient.Comune? PaIdComuneDomObj  { get; set; }

[Display(Name = "Telefono", ShortName="", Description = "Indirizzo di residenza: numero di telefono (1)", Prompt="")]
[ErpDogField("PA_TELEFONO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(15, ErrorMessage = "Inserire massimo 15 caratteri")]
[DataType(DataType.Text)]
public string? PaTelefono  { get; set; }

[Display(Name = "Cellulare", ShortName="", Description = "Indirizzo di residenza: numero di telefono (2)", Prompt="")]
[ErpDogField("PA_CELLULARE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(15, ErrorMessage = "Inserire massimo 15 caratteri")]
[DataType(DataType.Text)]
public string? PaCellulare  { get; set; }

[Display(Name = "Id Distretto Dom", ShortName="", Description = "Indirizzo di residenza : Codice di distretto", Prompt="")]
[ErpDogField("PA_ID_DISTRETTO_DOM", SqlFieldNameExt="", SqlFieldOptions="", Xref="Di1Icode", SqlFieldProperties="prop() xref(DISTRETTO.DI__ICODE) xdup() multbxref()")]
[AutocompleteClient("Distretto", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdDistrettoDom  { get; set; }
public HealthDemo.Models.SIO.Patient.Distretto? PaIdDistrettoDomObj  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note generiche sul paziente", Prompt="")]
[ErpDogField("PA_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? PaNote  { get; set; }

[Display(Name = "Data Decesso", ShortName="", Description = "Data di morte", Prompt="")]
[ErpDogField("PA_DATA_DECESSO", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? PaDataDecesso  { get; set; }

[Display(Name = "Ora Decesso", ShortName="", Description = "Ora di morte", Prompt="")]
[ErpDogField("PA_ORA_DECESSO", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? PaOraDecesso  { get; set; }

[Display(Name = "Id Nazione Res", ShortName="", Description = "Codice del comune di residenza del paziente", Prompt="")]
[ErpDogField("PA_ID_NAZIONE_RES", SqlFieldNameExt="", SqlFieldOptions="", Xref="Nz1Icode", SqlFieldProperties="prop() xref(NAZIONE.NZ__ICODE) xdup() multbxref()")]
[AutocompleteClient("Nazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PaIdNazioneRes  { get; set; }
public HealthDemo.Models.SIO.Patient.Nazione? PaIdNazioneResObj  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioPa1Icode|K|PA__ICODE","sioPa1RecDate|N|PA__MDATE,PA__CDATE"
        ,"sioPaIdDistrettoRes|N|PA_ID_DISTRETTO_RES"
        ,"sioPaIdDistrettoDom|N|PA_ID_DISTRETTO_DOM"
        ,"sioPaIdComuneNascita|N|PA_ID_COMUNE_NASCITA"
        ,"sioPaIdComuneRes|N|PA_ID_COMUNE_RES"
        ,"sioPaIdComuneDom|N|PA_ID_COMUNE_DOM"
        ,"sioPaIdCittadinanza|N|PA_ID_CITTADINANZA"
        ,"sioPaIdNazioneNascita|N|PA_ID_NAZIONE_NASCITA"
        ,"sioPaCodFiscalepa1Versionpa1Deleted|U|PA_COD_FISCALE,PA__VERSION,PA__DELETED"
        ,"sioPaCodSanitariopa1Versionpa1Deleted|U|PA_COD_SANITARIO,PA__VERSION,PA__DELETED"
        ,"sioPaCognomepaNome|N|PA_COGNOME,PA_NOME"
    };
}
}
}
