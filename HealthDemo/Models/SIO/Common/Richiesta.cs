using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Common {
public class Richiesta : ModelErp {
public const string Description = "Comunicazione e/o richiesta di prestazioni";
public const string SqlTableName = "RICHIESTA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Ri1Icode";
public const string SqlRowIdName = "RI__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "RI_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "RI_XDATA";
public const string SqlXdataIcodeName = "RI_X__ICODE";
public const string SqlXdataDeletedName = "RI_X__DELETED";
public const string SqlXdataTimestampName = "RI_X__TIMESTAMP";
public const string SqlXdataCdateName = "RI_X__CDATE";
public const string SqlXdataCtimeName = "RI_X__CTIME";
public const string SqlXdataCagentName = "RI_X__CAGENT";
public const string SqlXdataCunitName = "RI_X__CUNIT";
public const string SqlXdataMdateName = "RI_X__MDATE";
public const string SqlXdataMtimeName = "RI_X__MTIME";
public const string SqlXdataMagentName = "RI_X__MAGENT";
public const string SqlXdataMunitName = "RI_X__MUNIT";
public const string SqlXdataHomeName = "RI_X__HOME";
public const string SqlXdataVersionName = "RI_X__VERSION";
public const string SqlXdataInactiveName = "RI_X__INACTIVE";
public const string SqlXdataExtattName = "RI_X__EXTATT";
public const string SqlXdataMrefName = "RI_X__MREF";
public const string SqlXdataSeqName = "RI_X__SEQ";
public const string SqlXdataDescrName = "RI_X__DESCR";
public const string SqlXdataFmtName = "RI_X__FMT";
public const string SqlXdataXdurlName = "RI_X__XDURL";
public const string SqlXdataXdatumName = "RI_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 46; //Internal Table Code
public const string TBAREA = "Organizzazione ospedaliera"; //Table Area
public const string PREFIX = "Ri"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ri1Icode; } 
public override string labelText() { return $@"{Ri1Icode} - {RiOggetto}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(Ri1Icode)}</strong> {HttpUtility.HtmlEncode(RiOggetto)}"; }

//24-2//[N] PRESTAZIONE.PR_ID_RICHIESTA
[Display(Name = "Prestazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Prestazione>? XrefPrIdRichiesta { get; set; } = null;
[Key]
[Display(Name = "Ri1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("RI__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID] [LABEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ri1Icode { get; set; }
[Display(Name = "Ri1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("RI__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ri1Deleted { get; set; }
[Display(Name = "Ri1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("RI__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ri1Timestamp { get; set; }
[Display(Name = "Ri1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("RI__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ri1Cdate { get; set; }
[Display(Name = "Ri1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("RI__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ri1Ctime { get; set; }
[Display(Name = "Ri1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("RI__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ri1Cagent { get; set; }
[Display(Name = "Ri1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("RI__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ri1Cunit { get; set; }
[Display(Name = "Ri1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("RI__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ri1Mdate { get; set; }
[Display(Name = "Ri1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("RI__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ri1Mtime { get; set; }
[Display(Name = "Ri1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("RI__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ri1Magent { get; set; }
[Display(Name = "Ri1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("RI__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ri1Munit { get; set; }
[Display(Name = "Ri1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("RI__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ri1Home { get; set; }
[Display(Name = "Ri1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("RI__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ri1Version { get; set; }
[Display(Name = "Ri1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("RI__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ri1Inactive { get; set; }
[Display(Name = "Ri1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("RI__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ri1Extatt { get; set; }


[Display(Name = "Id Unita Richiedente", ShortName="", Description = "Codice dell'unità che ha originato la comunicazione", Prompt="")]
[ErpDogField("RI_ID_UNITA_RICHIEDENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"1\")}")]
[DataType(DataType.Text)]
public string? RiIdUnitaRichiedente  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? RiIdUnitaRichiedenteObj  { get; set; }

[Display(Name = "Id Postazione Richiedente", ShortName="", Description = "Codice del punto di servizio che ha originato la comunicazione", Prompt="")]
[ErpDogField("RI_ID_POSTAZIONE_RICHIEDENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"2\")}")]
[DataType(DataType.Text)]
public string? RiIdPostazioneRichiedente  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? RiIdPostazioneRichiedenteObj  { get; set; }

[Display(Name = "Id Istituto Richiedente", ShortName="", Description = "Codice dell'organizzazione che ha originato la comunicazione", Prompt="")]
[ErpDogField("RI_ID_ISTITUTO_RICHIEDENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"0\")}")]
[DataType(DataType.Text)]
public string? RiIdIstitutoRichiedente  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? RiIdIstitutoRichiedenteObj  { get; set; }

[Display(Name = "Id Operatore Richiedente", ShortName="", Description = "Codice (se disponibile) dell'agente che ha effettivamente inserito la comunicazione", Prompt="")]
[ErpDogField("RI_ID_OPERATORE_RICHIEDENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"3\")}")]
[DataType(DataType.Text)]
public string? RiIdOperatoreRichiedente  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? RiIdOperatoreRichiedenteObj  { get; set; }

[Display(Name = "Data Richiesta", ShortName="", Description = "Data non prima della quale la comunicazione deve essere trasmessa / Data di completamento quando eseguita", Prompt="")]
[ErpDogField("RI_DATA_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? RiDataRichiesta  { get; set; }

[Display(Name = "Ora Richiesta", ShortName="", Description = "Ora non prima della quale la comunicazione deve essere trasmessa / Ora di completamento quando eseguita", Prompt="")]
[ErpDogField("RI_ORA_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? RiOraRichiesta  { get; set; }

[Display(Name = "Urgenza", ShortName="", Description = "Livello di urgenza da 1 a 5 [1: il più alto]", Prompt="")]
[ErpDogField("RI_URGENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? RiUrgenza  { get; set; }

[Display(Name = "Oggetto", ShortName="", Description = "Oggetto della comunicazione", Prompt="")]
[ErpDogField("RI_OGGETTO", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? RiOggetto  { get; set; }

[Display(Name = "Stato Richiesta", ShortName="", Description = "Stato della comunicazione: In attesa / Sospesa / Completata (o annullata) / X: trasmessa solo a alcuni indirizzi", Prompt="")]
[ErpDogField("RI_STATO_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("P")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "P", "C", "X", "H", "A" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? RiStatoRichiesta  { get; set; }

[Display(Name = "Classe Richiesta", ShortName="", Description = "Classe della comunicazione: Da 0 a 9 riservata al sistema A a Z riservata agli utenti", Prompt="")]
[ErpDogField("RI_CLASSE_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RICHIESTA.TI_GRUPPO[RICHIESTA.RI_ID_TIPO_RICHIESTA]) multbxref()")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Z" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? RiClasseRichiesta  { get; set; }

[Display(Name = "Id Tipo Richiesta", ShortName="", Description = "Codice del tipo specifico di comunicazione", Prompt="")]
[ErpDogField("RI_ID_TIPO_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ti1Icode", SqlFieldProperties="prop() xref(TIPO_RICHIESTA.TI__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoRichiesta", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? RiIdTipoRichiesta  { get; set; }
public HealthDemo.Models.SIO.Common.TipoRichiesta? RiIdTipoRichiestaObj  { get; set; }

[Display(Name = "Id Paziente", ShortName="", Description = "Codice del paziente principale a cui si riferisce la comunicazione (se presente)", Prompt="")]
[ErpDogField("RI_ID_PAZIENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Pa1Icode", SqlFieldProperties="prop() xref(PAZIENTE.PA__ICODE) xdup() multbxref()")]
[AutocompleteServer("Paziente", "AutocompleteGetSelect", "AutocompletePreLoad", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? RiIdPaziente  { get; set; }
public HealthDemo.Models.SIO.Patient.Paziente? RiIdPazienteObj  { get; set; }

[Display(Name = "Id Episodio", ShortName="", Description = "Codice del contatto del paziente principale a cui si riferisce la comunicazione (se presente)", Prompt="")]
[ErpDogField("RI_ID_EPISODIO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Ep1Icode", SqlFieldProperties="prop() xref(EPISODIO.EP__ICODE) xdup() multbxref()")]
[AutocompleteServer("Episodio", "AutocompleteGetSelect", "AutocompletePreLoad", 1, ExtraFilter:"{In(\"EP_ID_PAZIENTE\", \"RiIdPaziente\")}", ExtraFields: "RiIdPaziente")]
[DataType(DataType.Text)]
public string? RiIdEpisodio  { get; set; }
public HealthDemo.Models.SIO.Patient.Episodio? RiIdEpisodioObj  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioRi1Icode|K|RI__ICODE","sioRi1RecDate|N|RI__MDATE,RI__CDATE"
        ,"sioRiDataRichiesta|N|RI_DATA_RICHIESTA"
        ,"sioRiIdOperatoreRichiedente|N|RI_ID_OPERATORE_RICHIEDENTE"
        ,"sioRiIdTipoRichiestariStatoRichiesta|N|RI_ID_TIPO_RICHIESTA,RI_STATO_RICHIESTA"
        ,"sioRiIdEpisodio|N|RI_ID_EPISODIO"
        ,"sioRiIdPaziente|N|RI_ID_PAZIENTE"
        ,"sioRiIdIstitutoRichiedente|N|RI_ID_ISTITUTO_RICHIEDENTE"
        ,"sioRiIdPostazioneRichiedente|N|RI_ID_POSTAZIONE_RICHIEDENTE"
        ,"sioRiIdUnitaRichiedente|N|RI_ID_UNITA_RICHIEDENTE"
    };
}
}
}
