using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Patient {
public class Episodio : ModelErp {
public const string Description = "Episodi";
public const string SqlTableName = "EPISODIO";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Ep1Icode";
public const string SqlRowIdName = "EP__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "EP_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "EP_XDATA";
public const string SqlXdataIcodeName = "EP_X__ICODE";
public const string SqlXdataDeletedName = "EP_X__DELETED";
public const string SqlXdataTimestampName = "EP_X__TIMESTAMP";
public const string SqlXdataCdateName = "EP_X__CDATE";
public const string SqlXdataCtimeName = "EP_X__CTIME";
public const string SqlXdataCagentName = "EP_X__CAGENT";
public const string SqlXdataCunitName = "EP_X__CUNIT";
public const string SqlXdataMdateName = "EP_X__MDATE";
public const string SqlXdataMtimeName = "EP_X__MTIME";
public const string SqlXdataMagentName = "EP_X__MAGENT";
public const string SqlXdataMunitName = "EP_X__MUNIT";
public const string SqlXdataHomeName = "EP_X__HOME";
public const string SqlXdataVersionName = "EP_X__VERSION";
public const string SqlXdataInactiveName = "EP_X__INACTIVE";
public const string SqlXdataExtattName = "EP_X__EXTATT";
public const string SqlXdataMrefName = "EP_X__MREF";
public const string SqlXdataSeqName = "EP_X__SEQ";
public const string SqlXdataDescrName = "EP_X__DESCR";
public const string SqlXdataFmtName = "EP_X__FMT";
public const string SqlXdataXdurlName = "EP_X__XDURL";
public const string SqlXdataXdatumName = "EP_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 53; //Internal Table Code
public const string TBAREA = "Accoglienza"; //Table Area
public const string PREFIX = "Ep"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ep1Icode; } 
public override string labelText() { return $@"{EpCodEpisodio} - {EpNote}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(EpCodEpisodio)}</strong> {HttpUtility.HtmlEncode(EpNote)}"; }

//10-2//[N] PRESTAZIONE.PR_ID_EPISODIO
[Display(Name = "Prestazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Prestazione>? XrefPrIdEpisodio { get; set; } = null;
//563-524//[N] RICHIESTA.RI_ID_EPISODIO
[Display(Name = "Richiesta", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Common.Richiesta>? XrefRiIdEpisodio { get; set; } = null;
//848-845//[N] RISULTATO_ESAME.RE_ID_EPISODIO
[Display(Name = "RisultatoEsame", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.RisultatoEsame>? XrefReIdEpisodio { get; set; } = null;
//980-978//[N] STATO_SALUTE.SS_ID_EPISODIO
[Display(Name = "StatoSalute", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.StatoSalute>? XrefSsIdEpisodio { get; set; } = null;
//1198-1196//[N] DOCUMENTO_CLINICO.DC_ID_EPISODIO
[Display(Name = "DocumentoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.DocumentoClinico>? XrefDcIdEpisodio { get; set; } = null;
//1736-1730//[N] CAMPIONE.CP_ID_EPISODIO
[Display(Name = "Campione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Campione>? XrefCpIdEpisodio { get; set; } = null;
//2166-2164//[N] PARAMETRO_VITALE.PV_ID_EPISODIO
[Display(Name = "ParametroVitale", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.ParametroVitale>? XrefPvIdEpisodio { get; set; } = null;
[Key]
[Display(Name = "Ep1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("EP__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ep1Icode { get; set; }
[Display(Name = "Ep1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("EP__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ep1Deleted { get; set; }
[Display(Name = "Ep1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("EP__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ep1Timestamp { get; set; }
[Display(Name = "Ep1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("EP__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ep1Cdate { get; set; }
[Display(Name = "Ep1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("EP__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ep1Ctime { get; set; }
[Display(Name = "Ep1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("EP__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ep1Cagent { get; set; }
[Display(Name = "Ep1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("EP__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ep1Cunit { get; set; }
[Display(Name = "Ep1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("EP__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ep1Mdate { get; set; }
[Display(Name = "Ep1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("EP__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ep1Mtime { get; set; }
[Display(Name = "Ep1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("EP__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ep1Magent { get; set; }
[Display(Name = "Ep1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("EP__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ep1Munit { get; set; }
[Display(Name = "Ep1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("EP__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ep1Home { get; set; }
[Display(Name = "Ep1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("EP__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ep1Version { get; set; }
[Display(Name = "Ep1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("EP__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ep1Inactive { get; set; }
[Display(Name = "Ep1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("EP__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ep1Extatt { get; set; }


[Display(Name = "Cod Episodio", ShortName="", Description = "Identificativo del contatto nell'organizzazione sanitaria", Prompt="")]
[ErpDogField("EP_COD_EPISODIO", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(EPISODIO.EP__ICODE[EP__ICODE] {EP_COD_EPISODIO=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(16, ErrorMessage = "Inserire massimo 16 caratteri")]
[DataType(DataType.Text)]
public string? EpCodEpisodio  { get; set; }

[Display(Name = "Id Paziente", ShortName="", Description = "Codice del paziente a cui si riferisce il contatto", Prompt="")]
[ErpDogField("EP_ID_PAZIENTE", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Pa1Icode", SqlFieldProperties="prop() xref(PAZIENTE.PA__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteServer("Paziente", "AutocompleteGetSelect", "AutocompletePreLoad", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? EpIdPaziente  { get; set; }
public HealthDemo.Models.SIO.Patient.Paziente? EpIdPazienteObj  { get; set; }

[Display(Name = "Sesso", ShortName="", Description = "Sesso del paziente al momento dell'ammissione", Prompt="")]
[ErpDogField("EP_SESSO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup(PAZIENTE.PA_SESSO[EPISODIO.EP_ID_PAZIENTE] {EP_SESSO=' '}) multbxref()")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "M", "F", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? EpSesso  { get; set; }

[Display(Name = "Classe Episodio", ShortName="", Description = "Classe di contatto 1=Permanenza 2=Day-hospital 3=Ambulatoriale 4-=definito dall'utente", Prompt="")]
[ErpDogField("EP_CLASSE_EPISODIO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_EPISODIO.TE_CLASSE[EPISODIO.EP_ID_TIPO_EPISODIO]) multbxref()")]
[DefaultValue("1")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? EpClasseEpisodio  { get; set; }

[Display(Name = "Id Tipo Episodio", ShortName="", Description = "Codice del tipo di contatto", Prompt="")]
[ErpDogField("EP_ID_TIPO_EPISODIO", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Te1Icode", SqlFieldProperties="prop() xref(TIPO_EPISODIO.TE__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoEpisodio", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? EpIdTipoEpisodio  { get; set; }
public HealthDemo.Models.SIO.Act.TipoEpisodio? EpIdTipoEpisodioObj  { get; set; }

[Display(Name = "Stato Episodio", ShortName="", Description = "Stato del contatto F[oreseen] - A[ctual, in progress] - C[ompleted] - D[eleted] - S[uspended]", Prompt="")]
[ErpDogField("EP_STATO_EPISODIO", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "F", "A", "C", "D", "S" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? EpStatoEpisodio  { get; set; }

[Display(Name = "Id Unita Ingresso", ShortName="", Description = "Identificativo dell'unità di assistenza che ha avviato il contatto", Prompt="")]
[ErpDogField("EP_ID_UNITA_INGRESSO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"1\")}")]
[DataType(DataType.Text)]
public string? EpIdUnitaIngresso  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? EpIdUnitaIngressoObj  { get; set; }

[Display(Name = "Data Inizio", ShortName="", Description = "Data di inizio del periodo di permanenza del contatto", Prompt="")]
[ErpDogField("EP_DATA_INIZIO", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? EpDataInizio  { get; set; }

[Display(Name = "Ora Inizio", ShortName="", Description = "Ora di inizio del periodo di permanenza del contatto", Prompt="")]
[ErpDogField("EP_ORA_INIZIO", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? EpOraInizio  { get; set; }

[Display(Name = "Data Fine", ShortName="", Description = "Data di fine del periodo di permanenza del contatto", Prompt="")]
[ErpDogField("EP_DATA_FINE", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? EpDataFine  { get; set; }

[Display(Name = "Ora Fine", ShortName="", Description = "Ora di fine del periodo di permanenza del contatto", Prompt="")]
[ErpDogField("EP_ORA_FINE", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? EpOraFine  { get; set; }

[Display(Name = "Cartella Ps", ShortName="", Description = "Identificativo del documento correlato dell'organizzazione di origine", Prompt="")]
[ErpDogField("EP_CARTELLA_PS", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? EpCartellaPs  { get; set; }

[Display(Name = "Id Corsia", ShortName="", Description = "Codice dell'ultima (o attuale) unità in cui è ubicato il paziente", Prompt="")]
[ErpDogField("EP_ID_CORSIA", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"1\")}")]
[DataType(DataType.Text)]
public string? EpIdCorsia  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? EpIdCorsiaObj  { get; set; }

[Display(Name = "Id Reparto", ShortName="", Description = "Codice dell'unità responsabile del paziente", Prompt="")]
[ErpDogField("EP_ID_REPARTO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup(EPISODIO.EP_ID_CORSIA[EP__ICODE] {EP_ID_REPARTO=' '}) multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"1\")}")]
[DataType(DataType.Text)]
public string? EpIdReparto  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? EpIdRepartoObj  { get; set; }

[Display(Name = "Letto", ShortName="", Description = "Letto assegnato al paziente", Prompt="")]
[ErpDogField("EP_LETTO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(15, ErrorMessage = "Inserire massimo 15 caratteri")]
[DataType(DataType.Text)]
public string? EpLetto  { get; set; }

[Display(Name = "Stanza", ShortName="", Description = "Stanza e altre strutture logistiche correlate al contatto", Prompt="")]
[ErpDogField("EP_STANZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? EpStanza  { get; set; }

[Display(Name = "Id Diagnosi Ammissione", ShortName="", Description = "Codice della diagnosi di ammissione", Prompt="")]
[ErpDogField("EP_ID_DIAGNOSI_AMMISSIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Dg1Icode", SqlFieldProperties="prop() xref(DIAGNOSI.DG__ICODE) xdup() multbxref()")]
[AutocompleteClient("Diagnosi", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? EpIdDiagnosiAmmissione  { get; set; }
public HealthDemo.Models.SIO.Costs.Diagnosi? EpIdDiagnosiAmmissioneObj  { get; set; }

[Display(Name = "Id Diagnosi Dimissione", ShortName="", Description = "Codice della diagnosi di dimissione", Prompt="")]
[ErpDogField("EP_ID_DIAGNOSI_DIMISSIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Dg1Icode", SqlFieldProperties="prop() xref(DIAGNOSI.DG__ICODE) xdup() multbxref()")]
[AutocompleteClient("Diagnosi", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? EpIdDiagnosiDimissione  { get; set; }
public HealthDemo.Models.SIO.Costs.Diagnosi? EpIdDiagnosiDimissioneObj  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note aggiuntive generiche", Prompt="")]
[ErpDogField("EP_NOTE", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? EpNote  { get; set; }

[Display(Name = "Id Atto Amministrativo", ShortName="", Description = "Identificativo dell'atto che descrive gli aspetti organizzativi attuali del contatto", Prompt="")]
[ErpDogField("EP_ID_ATTO_AMMINISTRATIVO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Pr1Icode", SqlFieldProperties="prop() xref(PRESTAZIONE.PR__ICODE) xdup() multbxref()")]
[AutocompleteServer("Prestazione", "AutocompleteGetSelect", "AutocompletePreLoad", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? EpIdAttoAmministrativo  { get; set; }
public HealthDemo.Models.SIO.Act.Prestazione? EpIdAttoAmministrativoObj  { get; set; }

[Display(Name = "Data Inizio La", ShortName="", Description = "Data di inizio del periodo di lista d'attesa del contatto", Prompt="")]
[ErpDogField("EP_DATA_INIZIO_LA", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? EpDataInizioLa  { get; set; }

[Display(Name = "Ora Inizio La", ShortName="", Description = "Ora di inizio del periodo di lista d'attesa del contatto", Prompt="")]
[ErpDogField("EP_ORA_INIZIO_LA", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? EpOraInizioLa  { get; set; }

[Display(Name = "Id Reparto La", ShortName="", Description = "Identificativo dell'unità di assistenza responsabile del periodo di lista d'attesa", Prompt="")]
[ErpDogField("EP_ID_REPARTO_LA", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"1\")}")]
[DataType(DataType.Text)]
public string? EpIdRepartoLa  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? EpIdRepartoLaObj  { get; set; }

[Display(Name = "Data Inizio Preh", ShortName="", Description = "Data di inizio del periodo di preospedalizzazione del contatto", Prompt="")]
[ErpDogField("EP_DATA_INIZIO_PREH", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? EpDataInizioPreh  { get; set; }

[Display(Name = "Ora Inizio Preh", ShortName="", Description = "Ora di inizio del periodo di preospedalizzazione del contatto", Prompt="")]
[ErpDogField("EP_ORA_INIZIO_PREH", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? EpOraInizioPreh  { get; set; }

[Display(Name = "Id Reparto Preh", ShortName="", Description = "Identificativo dell'unità di assistenza responsabile del periodo di preospedalizzazione", Prompt="")]
[ErpDogField("EP_ID_REPARTO_PREH", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"1\")}")]
[DataType(DataType.Text)]
public string? EpIdRepartoPreh  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? EpIdRepartoPrehObj  { get; set; }

[Display(Name = "Fase Episodio", ShortName="", Description = "Codice del tipo attuale (ultimo) fase del contatto (ad es. Lista d'attesa, Preospedalizzazione, In-staying, Home-care, Sospeso)", Prompt="")]
[ErpDogField("EP_FASE_EPISODIO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("I")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? EpFaseEpisodio  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioEp1Icode|K|EP__ICODE","sioEp1RecDate|N|EP__MDATE,EP__CDATE"
        ,"sioEpLetto|N|EP_LETTO"
        ,"sioEpDataFine|N|EP_DATA_FINE"
        ,"sioEpIdCorsiaepStatoEpisodio|N|EP_ID_CORSIA,EP_STATO_EPISODIO"
        ,"sioEpIdTipoEpisodioepDataInizio|N|EP_ID_TIPO_EPISODIO,EP_DATA_INIZIO"
        ,"sioEpIdAttoAmministrativo|N|EP_ID_ATTO_AMMINISTRATIVO"
        ,"sioEpIdDiagnosiDimissione|N|EP_ID_DIAGNOSI_DIMISSIONE"
        ,"sioEpCartellaPs|N|EP_CARTELLA_PS"
        ,"sioEpIdTipoEpisodio|N|EP_ID_TIPO_EPISODIO"
        ,"sioEpIdPaziente|N|EP_ID_PAZIENTE"
        ,"sioEpIdRepartoepStatoEpisodio|N|EP_ID_REPARTO,EP_STATO_EPISODIO"
        ,"sioEpDataInizio|N|EP_DATA_INIZIO"
        ,"sioEpStatoEpisodioepDataInizioepDataFine|N|EP_STATO_EPISODIO,EP_DATA_INIZIO,EP_DATA_FINE"
        ,"sioEpCodEpisodioep1Versionep1Deleted|U|EP_COD_EPISODIO,EP__VERSION,EP__DELETED"
    };
}
}
}
