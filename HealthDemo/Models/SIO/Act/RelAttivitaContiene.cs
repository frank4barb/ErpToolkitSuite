using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class RelAttivitaContiene : ModelErp {
public const string Description = "Corrispondenze tra tassonomie di attività";
public const string SqlTableName = "REL_ATTIVITA_CONTIENE";
public const string SqlTableNameExt = "REL_ATTIVITA_CONTIENE";
public const string SqlTableProperties = "";
public const string RowIdName = "Aa1Icode";
public const string SqlRowIdName = "AA__ICODE";
public const string SqlRowIdNameExt = "AA__ICODE";
public const string SqlPrefix = "AA_";
public const string SqlPrefixExt = "AA_";
public const string SqlXdataTableName = "AA_XDATA";
public const string SqlXdataIcodeName = "AA_X__ICODE";
public const string SqlXdataDeletedName = "AA_X__DELETED";
public const string SqlXdataTimestampName = "AA_X__TIMESTAMP";
public const string SqlXdataCdateName = "AA_X__CDATE";
public const string SqlXdataCtimeName = "AA_X__CTIME";
public const string SqlXdataCagentName = "AA_X__CAGENT";
public const string SqlXdataCunitName = "AA_X__CUNIT";
public const string SqlXdataMdateName = "AA_X__MDATE";
public const string SqlXdataMtimeName = "AA_X__MTIME";
public const string SqlXdataMagentName = "AA_X__MAGENT";
public const string SqlXdataMunitName = "AA_X__MUNIT";
public const string SqlXdataHomeName = "AA_X__HOME";
public const string SqlXdataVersionName = "AA_X__VERSION";
public const string SqlXdataInactiveName = "AA_X__INACTIVE";
public const string SqlXdataExtattName = "AA_X__EXTATT";
public const string SqlXdataMrefName = "AA_X__MREF";
public const string SqlXdataSeqName = "AA_X__SEQ";
public const string SqlXdataDescrName = "AA_X__DESCR";
public const string SqlXdataFmtName = "AA_X__FMT";
public const string SqlXdataXdurlName = "AA_X__XDURL";
public const string SqlXdataXdatumName = "AA_X__XDATUM";
public const string SqlXdataTableNameExt = "AA_XDATA";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 206; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Aa"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Aa1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

[Key]
[Display(Name = "Aa1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("AA__ICODE", SqlFieldNameExt="AA__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Aa1Icode { get; set; }
[Display(Name = "Aa1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("AA__DELETED", SqlFieldNameExt="AA__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Aa1Deleted { get; set; }
[Display(Name = "Aa1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("AA__TIMESTAMP", SqlFieldNameExt="AA__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Aa1Timestamp { get; set; }
[Display(Name = "Aa1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AA__CDATE", SqlFieldNameExt="AA__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Aa1Cdate { get; set; }
[Display(Name = "Aa1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AA__CTIME", SqlFieldNameExt="AA__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Aa1Ctime { get; set; }
[Display(Name = "Aa1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AA__CAGENT", SqlFieldNameExt="AA__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Aa1Cagent { get; set; }
[Display(Name = "Aa1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AA__CUNIT", SqlFieldNameExt="AA__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Aa1Cunit { get; set; }
[Display(Name = "Aa1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AA__MDATE", SqlFieldNameExt="AA__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Aa1Mdate { get; set; }
[Display(Name = "Aa1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AA__MTIME", SqlFieldNameExt="AA__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Aa1Mtime { get; set; }
[Display(Name = "Aa1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AA__MAGENT", SqlFieldNameExt="AA__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Aa1Magent { get; set; }
[Display(Name = "Aa1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AA__MUNIT", SqlFieldNameExt="AA__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Aa1Munit { get; set; }
[Display(Name = "Aa1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("AA__HOME", SqlFieldNameExt="AA__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Aa1Home { get; set; }
[Display(Name = "Aa1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("AA__VERSION", SqlFieldNameExt="AA__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Aa1Version { get; set; }
[Display(Name = "Aa1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("AA__INACTIVE", SqlFieldNameExt="AA__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Aa1Inactive { get; set; }
[Display(Name = "Aa1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("AA__EXTATT", SqlFieldNameExt="AA__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Aa1Extatt { get; set; }


[Display(Name = "Id Attivita Padre", ShortName="", Description = "Identificatore del tipo di attività della prima tassonomia (cioè quella che viene aggregata)", Prompt="")]
[ErpDogField("AA_ID_ATTIVITA_PADRE", SqlFieldNameExt="AA_ID_ATTIVITA_PADRE", SqlFieldOptions="", Xref="Av1Icode", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
[AutocompleteClient("Attivita", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? AaIdAttivitaPadre  { get; set; }
public HealthDemo.Models.SIO.Act.Attivita? AaIdAttivitaPadreObj  { get; set; }

[Display(Name = "Id Attivita Figlio", ShortName="", Description = "Identificatore del tipo di attività in cui la prima è stata aggregata", Prompt="")]
[ErpDogField("AA_ID_ATTIVITA_FIGLIO", SqlFieldNameExt="AA_ID_ATTIVITA_FIGLIO", SqlFieldOptions="", Xref="Av1Icode", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
[AutocompleteClient("Attivita", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? AaIdAttivitaFiglio  { get; set; }
public HealthDemo.Models.SIO.Act.Attivita? AaIdAttivitaFiglioObj  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Numero di sequenza di TOAY rispetto a FROMAY", Prompt="")]
[ErpDogField("AA_SEQUENZA", SqlFieldNameExt="AA_SEQUENZA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? AaSequenza  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note testuali", Prompt="")]
[ErpDogField("AA_NOTE", SqlFieldNameExt="AA_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? AaNote  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioAa1Icode|K|AA__ICODE","sioAa1RecDate|N|AA__MDATE,AA__CDATE"
        ,"sioAaIdAttivitaPadre|N|AA_ID_ATTIVITA_PADRE"
        ,"sioAaIdAttivitaFiglio|N|AA_ID_ATTIVITA_FIGLIO"
    };
}
}
}
