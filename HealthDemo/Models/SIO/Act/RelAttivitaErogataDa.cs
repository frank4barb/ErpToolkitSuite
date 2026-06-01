using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class RelAttivitaErogataDa : ModelErp {
public const string Description = "Strutture che possono eseguire un certo tipo di attività";
public const string SqlTableName = "REL_ATTIVITA_EROGATA_DA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Ae1Icode";
public const string SqlRowIdName = "AE__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "AE_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "AE_XDATA";
public const string SqlXdataIcodeName = "AE_X__ICODE";
public const string SqlXdataDeletedName = "AE_X__DELETED";
public const string SqlXdataTimestampName = "AE_X__TIMESTAMP";
public const string SqlXdataCdateName = "AE_X__CDATE";
public const string SqlXdataCtimeName = "AE_X__CTIME";
public const string SqlXdataCagentName = "AE_X__CAGENT";
public const string SqlXdataCunitName = "AE_X__CUNIT";
public const string SqlXdataMdateName = "AE_X__MDATE";
public const string SqlXdataMtimeName = "AE_X__MTIME";
public const string SqlXdataMagentName = "AE_X__MAGENT";
public const string SqlXdataMunitName = "AE_X__MUNIT";
public const string SqlXdataHomeName = "AE_X__HOME";
public const string SqlXdataVersionName = "AE_X__VERSION";
public const string SqlXdataInactiveName = "AE_X__INACTIVE";
public const string SqlXdataExtattName = "AE_X__EXTATT";
public const string SqlXdataMrefName = "AE_X__MREF";
public const string SqlXdataSeqName = "AE_X__SEQ";
public const string SqlXdataDescrName = "AE_X__DESCR";
public const string SqlXdataFmtName = "AE_X__FMT";
public const string SqlXdataXdurlName = "AE_X__XDURL";
public const string SqlXdataXdatumName = "AE_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 81; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Ae"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ae1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

[Key]
[Display(Name = "Ae1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("AE__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ae1Icode { get; set; }
[Display(Name = "Ae1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("AE__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ae1Deleted { get; set; }
[Display(Name = "Ae1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("AE__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ae1Timestamp { get; set; }
[Display(Name = "Ae1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AE__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ae1Cdate { get; set; }
[Display(Name = "Ae1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AE__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ae1Ctime { get; set; }
[Display(Name = "Ae1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AE__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ae1Cagent { get; set; }
[Display(Name = "Ae1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AE__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ae1Cunit { get; set; }
[Display(Name = "Ae1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AE__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ae1Mdate { get; set; }
[Display(Name = "Ae1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AE__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ae1Mtime { get; set; }
[Display(Name = "Ae1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AE__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ae1Magent { get; set; }
[Display(Name = "Ae1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AE__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ae1Munit { get; set; }
[Display(Name = "Ae1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("AE__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ae1Home { get; set; }
[Display(Name = "Ae1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("AE__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ae1Version { get; set; }
[Display(Name = "Ae1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("AE__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ae1Inactive { get; set; }
[Display(Name = "Ae1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("AE__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ae1Extatt { get; set; }


[Display(Name = "Id Attivita", ShortName="", Description = "Codice dell'attività", Prompt="")]
[ErpDogField("AE_ID_ATTIVITA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Av1Icode", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("Attivita", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? AeIdAttivita  { get; set; }
public HealthDemo.Models.SIO.Act.Attivita? AeIdAttivitaObj  { get; set; }

[Display(Name = "Id Unita", ShortName="", Description = "Codice dell'agente autorizzato a eseguire l'attività", Prompt="")]
[ErpDogField("AE_ID_UNITA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? AeIdUnita  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? AeIdUnitaObj  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note testuali", Prompt="")]
[ErpDogField("AE_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(40, ErrorMessage = "Inserire massimo 40 caratteri")]
[DataType(DataType.Text)]
public string? AeNote  { get; set; }

[Display(Name = "Modalita Di Pianificazione", ShortName="", Description = "Modalità di pianificazione predefinita [P]ianificazione - [R]andom", Prompt="")]
[ErpDogField("AE_MODALITA_DI_PIANIFICAZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("P")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "P", "R" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AeModalitaDiPianificazione  { get; set; }

[Display(Name = "Erogazione Frequente", ShortName="", Description = "Attività frequentemente richiesta (Sì - No)", Prompt="")]
[ErpDogField("AE_EROGAZIONE_FREQUENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("N")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AeErogazioneFrequente  { get; set; }

[Display(Name = "Attributi", ShortName="", Description = "Flag operativi autonomamente gestiti dall'applicazione", Prompt="")]
[ErpDogField("AE_ATTRIBUTI", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(240, ErrorMessage = "Inserire massimo 240 caratteri")]
[DataType(DataType.Text)]
public string? AeAttributi  { get; set; }

[Display(Name = "Filtro Regime Erogazione", ShortName="", Description = "Classi di contatti per cui viene svolta l'attività", Prompt="")]
[ErpDogField("AE_FILTRO_REGIME_EROGAZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(4, ErrorMessage = "Inserire massimo 4 caratteri")]
[DataType(DataType.Text)]
public string? AeFiltroRegimeErogazione  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioAe1Icode|K|AE__ICODE","sioAe1RecDate|N|AE__MDATE,AE__CDATE"
        ,"sioAeIdAttivitaaeIdUnita|N|AE_ID_ATTIVITA,AE_ID_UNITA"
        ,"sioAeIdUnitaaeIdAttivitaae1Versionae1Deleted|U|AE_ID_UNITA,AE_ID_ATTIVITA,AE__VERSION,AE__DELETED"
    };
}
}
}
