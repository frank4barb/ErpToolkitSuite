using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class RelOrganizzazioneContiene : ModelErp {
public const string Description = "Relazioni generiche esistenti tra diverse strutture";
public const string SqlTableName = "REL_ORGANIZZAZIONE_CONTIENE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Oo1Icode";
public const string SqlRowIdName = "OO__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "OO_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "OO_XDATA";
public const string SqlXdataIcodeName = "OO_X__ICODE";
public const string SqlXdataDeletedName = "OO_X__DELETED";
public const string SqlXdataTimestampName = "OO_X__TIMESTAMP";
public const string SqlXdataCdateName = "OO_X__CDATE";
public const string SqlXdataCtimeName = "OO_X__CTIME";
public const string SqlXdataCagentName = "OO_X__CAGENT";
public const string SqlXdataCunitName = "OO_X__CUNIT";
public const string SqlXdataMdateName = "OO_X__MDATE";
public const string SqlXdataMtimeName = "OO_X__MTIME";
public const string SqlXdataMagentName = "OO_X__MAGENT";
public const string SqlXdataMunitName = "OO_X__MUNIT";
public const string SqlXdataHomeName = "OO_X__HOME";
public const string SqlXdataVersionName = "OO_X__VERSION";
public const string SqlXdataInactiveName = "OO_X__INACTIVE";
public const string SqlXdataExtattName = "OO_X__EXTATT";
public const string SqlXdataMrefName = "OO_X__MREF";
public const string SqlXdataSeqName = "OO_X__SEQ";
public const string SqlXdataDescrName = "OO_X__DESCR";
public const string SqlXdataFmtName = "OO_X__FMT";
public const string SqlXdataXdurlName = "OO_X__XDURL";
public const string SqlXdataXdatumName = "OO_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 115; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Oo"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Oo1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

[Key]
[Display(Name = "Oo1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("OO__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Oo1Icode { get; set; }
[Display(Name = "Oo1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("OO__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Oo1Deleted { get; set; }
[Display(Name = "Oo1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("OO__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Oo1Timestamp { get; set; }
[Display(Name = "Oo1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("OO__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Oo1Cdate { get; set; }
[Display(Name = "Oo1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("OO__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Oo1Ctime { get; set; }
[Display(Name = "Oo1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("OO__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Oo1Cagent { get; set; }
[Display(Name = "Oo1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("OO__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Oo1Cunit { get; set; }
[Display(Name = "Oo1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("OO__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Oo1Mdate { get; set; }
[Display(Name = "Oo1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("OO__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Oo1Mtime { get; set; }
[Display(Name = "Oo1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("OO__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Oo1Magent { get; set; }
[Display(Name = "Oo1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("OO__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Oo1Munit { get; set; }
[Display(Name = "Oo1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("OO__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Oo1Home { get; set; }
[Display(Name = "Oo1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("OO__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Oo1Version { get; set; }
[Display(Name = "Oo1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("OO__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Oo1Inactive { get; set; }
[Display(Name = "Oo1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("OO__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Oo1Extatt { get; set; }


[Display(Name = "Id Organizzazione Padre", ShortName="", Description = "Codice del primo agente correlato all'altro", Prompt="")]
[ErpDogField("OO_ID_ORGANIZZAZIONE_PADRE", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? OoIdOrganizzazionePadre  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? OoIdOrganizzazionePadreObj  { get; set; }

[Display(Name = "Id Organizzazione Figlio", ShortName="", Description = "Codice del secondo agente correlato al primo", Prompt="")]
[ErpDogField("OO_ID_ORGANIZZAZIONE_FIGLIO", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? OoIdOrganizzazioneFiglio  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? OoIdOrganizzazioneFiglioObj  { get; set; }

[Display(Name = "Regola Di Inclusione", ShortName="", Description = "Ruolo della relazione tra i due agenti", Prompt="")]
[ErpDogField("OO_REGOLA_DI_INCLUSIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? OoRegolaDiInclusione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note opzionali riguardo alla relazione tra gli agenti", Prompt="")]
[ErpDogField("OO_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? OoNote  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioOo1Icode|K|OO__ICODE","sioOo1RecDate|N|OO__MDATE,OO__CDATE"
        ,"sioOoIdOrganizzazionePadreooIdOrganizzazioneFiglioooRegolaDiInclusione|N|OO_ID_ORGANIZZAZIONE_PADRE,OO_ID_ORGANIZZAZIONE_FIGLIO,OO_REGOLA_DI_INCLUSIONE"
        ,"sioOoIdOrganizzazioneFiglioooIdOrganizzazionePadre|N|OO_ID_ORGANIZZAZIONE_FIGLIO,OO_ID_ORGANIZZAZIONE_PADRE"
    };
}
}
}
