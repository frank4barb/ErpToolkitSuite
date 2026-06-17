using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class Attivita : ModelErp {
public const string Description = "Tipi di attività che possono essere richieste e/o eseguite";
public const string SqlTableName = "ATTIVITA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Av1Icode";
public const string SqlRowIdName = "AV__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "AV_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "AV_XDATA";
public const string SqlXdataIcodeName = "AV_X__ICODE";
public const string SqlXdataDeletedName = "AV_X__DELETED";
public const string SqlXdataTimestampName = "AV_X__TIMESTAMP";
public const string SqlXdataCdateName = "AV_X__CDATE";
public const string SqlXdataCtimeName = "AV_X__CTIME";
public const string SqlXdataCagentName = "AV_X__CAGENT";
public const string SqlXdataCunitName = "AV_X__CUNIT";
public const string SqlXdataMdateName = "AV_X__MDATE";
public const string SqlXdataMtimeName = "AV_X__MTIME";
public const string SqlXdataMagentName = "AV_X__MAGENT";
public const string SqlXdataMunitName = "AV_X__MUNIT";
public const string SqlXdataHomeName = "AV_X__HOME";
public const string SqlXdataVersionName = "AV_X__VERSION";
public const string SqlXdataInactiveName = "AV_X__INACTIVE";
public const string SqlXdataExtattName = "AV_X__EXTATT";
public const string SqlXdataMrefName = "AV_X__MREF";
public const string SqlXdataSeqName = "AV_X__SEQ";
public const string SqlXdataDescrName = "AV_X__DESCR";
public const string SqlXdataFmtName = "AV_X__FMT";
public const string SqlXdataXdurlName = "AV_X__XDURL";
public const string SqlXdataXdatumName = "AV_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 11; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Av"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Av1Icode; } 
public override string labelText() { return $@"{AvCodice} - {AvDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(AvCodice)}</strong> {HttpUtility.HtmlEncode(AvDescrizione)}"; }

//3-2//[N] PRESTAZIONE.PR_ID_ATTIVITA_RICHIESTA
[Display(Name = "Prestazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Prestazione>? XrefPrIdAttivitaRichiesta { get; set; } = null;
//4-2//[N] PRESTAZIONE.PR_ID_ATTIVITA_ESEGUITA
[Display(Name = "Prestazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Prestazione>? XrefPrIdAttivitaEseguita { get; set; } = null;
//91-83//[N] ATTIVITA.AV_ID_GRUPPO
[Display(Name = "Attivita", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Attivita>? XrefAvIdGruppo { get; set; } = null;
//370-370//[Y] REL_ATTIVITA_TIPO_CAMPIONE.AC_ID_ATTIVITA
[Display(Name = "RelAttivitaTipoCampione", ShortName = "", Description = "Tipo di campione rilevante per un certo tipo di attività", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaTipoCampione>? XrefAcIdAttivita { get; set; } = null;
//1131-1131//[Y] REL_ATTIVITA_RICHIESTA_DA.AR_ID_ATTIVITA
[Display(Name = "RelAttivitaRichiestaDa", ShortName = "", Description = "Tipi di attività che possono essere richiesti da un certo operatore/struttura sanitaria", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaRichiestaDa>? XrefArIdAttivita { get; set; } = null;
//1179-1179//[Y] REL_ATTIVITA_USA.AU_ID_ATTIVITA
[Display(Name = "RelAttivitaUsa", ShortName = "", Description = "Tipi e/o risorse individuali generalmente necessari per l'esecuzione di un'attività", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaUsa>? XrefAuIdAttivita { get; set; } = null;
//1992-1992//[Y] REL_ATTIVITA_EROGATA_DA.AE_ID_ATTIVITA
[Display(Name = "RelAttivitaErogataDa", ShortName = "", Description = "Strutture che possono eseguire un certo tipo di attività", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaErogataDa>? XrefAeIdAttivita { get; set; } = null;
//2203-2203//[Y] REL_ATTIVITA_CONTIENE.AA_ID_ATTIVITA_PADRE
[Display(Name = "RelAttivitaContiene", ShortName = "", Description = "Corrispondenze tra tassonomie di attività", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaContiene>? XrefAaIdAttivitaPadre { get; set; } = null;
//2204-2203//[Y] REL_ATTIVITA_CONTIENE.AA_ID_ATTIVITA_FIGLIO
[Display(Name = "RelAttivitaContiene", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaContiene>? XrefAaIdAttivitaFiglio { get; set; } = null;
[Key]
[Display(Name = "Av1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("AV__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Av1Icode { get; set; }
[Display(Name = "Av1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("AV__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Av1Deleted { get; set; }
[Display(Name = "Av1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("AV__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Av1Timestamp { get; set; }
[Display(Name = "Av1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AV__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Av1Cdate { get; set; }
[Display(Name = "Av1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AV__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Av1Ctime { get; set; }
[Display(Name = "Av1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AV__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Av1Cagent { get; set; }
[Display(Name = "Av1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AV__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Av1Cunit { get; set; }
[Display(Name = "Av1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AV__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Av1Mdate { get; set; }
[Display(Name = "Av1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AV__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Av1Mtime { get; set; }
[Display(Name = "Av1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AV__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Av1Magent { get; set; }
[Display(Name = "Av1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AV__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Av1Munit { get; set; }
[Display(Name = "Av1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("AV__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Av1Home { get; set; }
[Display(Name = "Av1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("AV__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Av1Version { get; set; }
[Display(Name = "Av1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("AV__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Av1Inactive { get; set; }
[Display(Name = "Av1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("AV__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Av1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("AV_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(ATTIVITA.AV__ICODE[AV__ICODE] {AV_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? AvCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("AV_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? AvDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("AV_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? AvNote  { get; set; }

[Display(Name = "Filtro Regime Erogazione", ShortName="", Description = "Maschera con le classi di contatti per cui l'attività può essere eseguita", Prompt="")]
[ErpDogField("AV_FILTRO_REGIME_EROGAZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(4, ErrorMessage = "Inserire massimo 4 caratteri")]
[DataType(DataType.Text)]
public string? AvFiltroRegimeErogazione  { get; set; }

[Display(Name = "Costo Medio", ShortName="", Description = "Costo totale (medio) per l'esecuzione", Prompt="")]
[ErpDogField("AV_COSTO_MEDIO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? AvCostoMedio  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Codice dell'attività di cui questa è una sotto-attività", Prompt="")]
[ErpDogField("AV_ID_GRUPPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Av1Icode", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
[AutocompleteClient("Attivita", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? AvIdGruppo  { get; set; }
public HealthDemo.Models.SIO.Act.Attivita? AvIdGruppoObj  { get; set; }

[Display(Name = "Attivita Preferenziale", ShortName="", Description = "Attività preferenziale eseguita quando il servizio viene richiesto Sì [Y] / No [N]", Prompt="")]
[ErpDogField("AV_ATTIVITA_PREFERENZIALE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("N")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AvAttivitaPreferenziale  { get; set; }

[Display(Name = "Durata Validita", ShortName="", Description = "Livello clinico di validità (cioè il numero di ore durante le quali non ha senso clinico replicare l'attività)", Prompt="")]
[ErpDogField("AV_DURATA_VALIDITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? AvDurataValidita  { get; set; }

[Display(Name = "Durata Media", ShortName="", Description = "Tempo medio del ciclo completo dell'attività [ore]", Prompt="")]
[ErpDogField("AV_DURATA_MEDIA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? AvDurataMedia  { get; set; }

[Display(Name = "In Evidenza", ShortName="", Description = "Evidenziare gli atti effettivi per scopi di ricerca o speciali Sì [Y] - No [N]", Prompt="")]
[ErpDogField("AV_IN_EVIDENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("N")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AvInEvidenza  { get; set; }

[Display(Name = "Id Tipo Attivita", ShortName="", Description = "Codice della classe generale di attività predefinita", Prompt="")]
[ErpDogField("AV_ID_TIPO_ATTIVITA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ta1Icode", SqlFieldProperties="prop() xref(TIPO_ATTIVITA.TA__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoAttivita", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? AvIdTipoAttivita  { get; set; }
public HealthDemo.Models.SIO.Act.TipoAttivita? AvIdTipoAttivitaObj  { get; set; }

[Display(Name = "Routine", ShortName="", Description = "Pianificazione routinaria (cioè automatica) Sì [Y] - No [N]", Prompt="")]
[ErpDogField("AV_ROUTINE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("Y")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AvRoutine  { get; set; }

[Display(Name = "Note Estese", ShortName="", Description = "Nota estesa", Prompt="")]
[ErpDogField("AV_NOTE_ESTESE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(240, ErrorMessage = "Inserire massimo 240 caratteri")]
[DataType(DataType.Text)]
public string? AvNoteEstese  { get; set; }

[Display(Name = "Attributi1", ShortName="", Description = "Flag per scopi operativi, gestiti autonomamente dalle applicazioni", Prompt="")]
[ErpDogField("AV_ATTRIBUTI1", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(240, ErrorMessage = "Inserire massimo 240 caratteri")]
[DataType(DataType.Text)]
public string? AvAttributi1  { get; set; }

[Display(Name = "Attributi2", ShortName="", Description = "Ulteriore insieme di flag operativi, gestiti dalle applicazioni", Prompt="")]
[ErpDogField("AV_ATTRIBUTI2", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(240, ErrorMessage = "Inserire massimo 240 caratteri")]
[DataType(DataType.Text)]
public string? AvAttributi2  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioAv1Icode|K|AV__ICODE","sioAv1RecDate|N|AV__MDATE,AV__CDATE"
        ,"sioAvIdGruppo|N|AV_ID_GRUPPO"
        ,"sioAvIdTipoAttivita|N|AV_ID_TIPO_ATTIVITA"
        ,"sioAv1Versionav1Deleted|U|AV__VERSION,AV__DELETED"
        ,"sioAvCodiceav1Versionav1Deleted|U|AV_CODICE,AV__VERSION,AV__DELETED"
    };
}
}
}
