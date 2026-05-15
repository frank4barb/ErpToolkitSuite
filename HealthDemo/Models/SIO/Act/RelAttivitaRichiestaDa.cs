using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class RelAttivitaRichiestaDa : ModelErp {
public const string Description = "Tipi di attività che possono essere richiesti da un certo operatore/struttura sanitaria";
public const string SqlTableName = "REL_ATTIVITA_RICHIESTA_DA";
public const string SqlTableNameExt = "REL_ATTIVITA_RICHIESTA_DA";
public const string SqlTableProperties = "";
public const string RowIdName = "Ar1Icode";
public const string SqlRowIdName = "AR__ICODE";
public const string SqlRowIdNameExt = "AR__ICODE";
public const string SqlPrefix = "AR_";
public const string SqlPrefixExt = "AR_";
public const string SqlXdataTableName = "AR_XDATA";
public const string SqlXdataIcodeName = "AR_X__ICODE";
public const string SqlXdataDeletedName = "AR_X__DELETED";
public const string SqlXdataTimestampName = "AR_X__TIMESTAMP";
public const string SqlXdataCdateName = "AR_X__CDATE";
public const string SqlXdataCtimeName = "AR_X__CTIME";
public const string SqlXdataCagentName = "AR_X__CAGENT";
public const string SqlXdataCunitName = "AR_X__CUNIT";
public const string SqlXdataMdateName = "AR_X__MDATE";
public const string SqlXdataMtimeName = "AR_X__MTIME";
public const string SqlXdataMagentName = "AR_X__MAGENT";
public const string SqlXdataMunitName = "AR_X__MUNIT";
public const string SqlXdataHomeName = "AR_X__HOME";
public const string SqlXdataVersionName = "AR_X__VERSION";
public const string SqlXdataInactiveName = "AR_X__INACTIVE";
public const string SqlXdataExtattName = "AR_X__EXTATT";
public const string SqlXdataMrefName = "AR_X__MREF";
public const string SqlXdataSeqName = "AR_X__SEQ";
public const string SqlXdataDescrName = "AR_X__DESCR";
public const string SqlXdataFmtName = "AR_X__FMT";
public const string SqlXdataXdurlName = "AR_X__XDURL";
public const string SqlXdataXdatumName = "AR_X__XDATUM";
public const string SqlXdataTableNameExt = "AR_XDATA";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 15; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Ar"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ar1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

[Key]
[Display(Name = "Ar1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("AR__ICODE", SqlFieldNameExt="AR__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ar1Icode { get; set; }
[Display(Name = "Ar1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("AR__DELETED", SqlFieldNameExt="AR__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ar1Deleted { get; set; }
[Display(Name = "Ar1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("AR__TIMESTAMP", SqlFieldNameExt="AR__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ar1Timestamp { get; set; }
[Display(Name = "Ar1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AR__CDATE", SqlFieldNameExt="AR__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ar1Cdate { get; set; }
[Display(Name = "Ar1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AR__CTIME", SqlFieldNameExt="AR__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ar1Ctime { get; set; }
[Display(Name = "Ar1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AR__CAGENT", SqlFieldNameExt="AR__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ar1Cagent { get; set; }
[Display(Name = "Ar1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AR__CUNIT", SqlFieldNameExt="AR__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ar1Cunit { get; set; }
[Display(Name = "Ar1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AR__MDATE", SqlFieldNameExt="AR__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ar1Mdate { get; set; }
[Display(Name = "Ar1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AR__MTIME", SqlFieldNameExt="AR__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ar1Mtime { get; set; }
[Display(Name = "Ar1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AR__MAGENT", SqlFieldNameExt="AR__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ar1Magent { get; set; }
[Display(Name = "Ar1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AR__MUNIT", SqlFieldNameExt="AR__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ar1Munit { get; set; }
[Display(Name = "Ar1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("AR__HOME", SqlFieldNameExt="AR__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ar1Home { get; set; }
[Display(Name = "Ar1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("AR__VERSION", SqlFieldNameExt="AR__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ar1Version { get; set; }
[Display(Name = "Ar1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("AR__INACTIVE", SqlFieldNameExt="AR__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ar1Inactive { get; set; }
[Display(Name = "Ar1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("AR__EXTATT", SqlFieldNameExt="AR__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ar1Extatt { get; set; }


[Display(Name = "Id Attivita", ShortName="", Description = "Codice dell'attività che può essere eseguita", Prompt="")]
[ErpDogField("AR_ID_ATTIVITA", SqlFieldNameExt="AR_ID_ATTIVITA", SqlFieldOptions="", Xref="Av1Icode", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
[AutocompleteClient("Attivita", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? ArIdAttivita  { get; set; }
public HealthDemo.Models.SIO.Act.Attivita? ArIdAttivitaObj  { get; set; }

[Display(Name = "Id Istituto", ShortName="", Description = "Codice dell'organizzazione che può eseguire l'atto", Prompt="")]
[ErpDogField("AR_ID_ISTITUTO", SqlFieldNameExt="AR_ID_ISTITUTO", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? ArIdIstituto  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? ArIdIstitutoObj  { get; set; }

[Display(Name = "Id Unita", ShortName="", Description = "Codice dell'unità che può eseguire l'atto", Prompt="")]
[ErpDogField("AR_ID_UNITA", SqlFieldNameExt="AR_ID_UNITA", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? ArIdUnita  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? ArIdUnitaObj  { get; set; }

[Display(Name = "Id Postazione", ShortName="", Description = "Codice del punto di servizio (SP) che può eseguire l'atto", Prompt="")]
[ErpDogField("AR_ID_POSTAZIONE", SqlFieldNameExt="AR_ID_POSTAZIONE", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? ArIdPostazione  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? ArIdPostazioneObj  { get; set; }

[Display(Name = "Id Operatore", ShortName="", Description = "Codice dell'agente che può eseguire l'atto", Prompt="")]
[ErpDogField("AR_ID_OPERATORE", SqlFieldNameExt="AR_ID_OPERATORE", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? ArIdOperatore  { get; set; }
public HealthDemo.Models.SIO.Common.Organizzazione? ArIdOperatoreObj  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note testuali", Prompt="")]
[ErpDogField("AR_NOTE", SqlFieldNameExt="AR_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? ArNote  { get; set; }

[Display(Name = "Richiesta Frequente", ShortName="", Description = "Attività frequentemente richiesta (Sì/No)", Prompt="")]
[ErpDogField("AR_RICHIESTA_FREQUENTE", SqlFieldNameExt="AR_RICHIESTA_FREQUENTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("N")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? ArRichiestaFrequente  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioAr1Icode|K|AR__ICODE","sioAr1RecDate|N|AR__MDATE,AR__CDATE"
        ,"sioArIdOperatore|N|AR_ID_OPERATORE"
        ,"sioArIdAttivita|N|AR_ID_ATTIVITA"
        ,"sioArIdIstituto|N|AR_ID_ISTITUTO"
        ,"sioArIdPostazione|N|AR_ID_POSTAZIONE"
        ,"sioArIdUnita|N|AR_ID_UNITA"
    };
}
}
}
