using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class RelPrestazioneCampione : ModelErp {
public const string Description = "Campioni utilizzati e/o generati da una prestazione";
public const string SqlTableName = "REL_PRESTAZIONE_CAMPIONE";
public const string SqlTableNameExt = "REL_PRESTAZIONE_CAMPIONE";
public const string SqlTableProperties = "";
public const string RowIdName = "Pc1Icode";
public const string SqlRowIdName = "PC__ICODE";
public const string SqlRowIdNameExt = "PC__ICODE";
public const string SqlPrefix = "PC_";
public const string SqlPrefixExt = "PC_";
public const string SqlXdataTableName = "PC_XDATA";
public const string SqlXdataTableNameExt = "PC_XDATA";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 102; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Pc"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Pc1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

[Key]
[Display(Name = "Pc1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("PC__ICODE", SqlFieldNameExt="PC__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pc1Icode { get; set; }
[Display(Name = "Pc1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("PC__DELETED", SqlFieldNameExt="PC__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pc1Deleted { get; set; }
[Display(Name = "Pc1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("PC__TIMESTAMP", SqlFieldNameExt="PC__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Pc1Timestamp { get; set; }
[Display(Name = "Pc1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PC__CDATE", SqlFieldNameExt="PC__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pc1Cdate { get; set; }
[Display(Name = "Pc1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PC__CTIME", SqlFieldNameExt="PC__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pc1Ctime { get; set; }
[Display(Name = "Pc1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PC__CAGENT", SqlFieldNameExt="PC__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pc1Cagent { get; set; }
[Display(Name = "Pc1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PC__CUNIT", SqlFieldNameExt="PC__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pc1Cunit { get; set; }
[Display(Name = "Pc1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PC__MDATE", SqlFieldNameExt="PC__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pc1Mdate { get; set; }
[Display(Name = "Pc1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PC__MTIME", SqlFieldNameExt="PC__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pc1Mtime { get; set; }
[Display(Name = "Pc1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PC__MAGENT", SqlFieldNameExt="PC__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pc1Magent { get; set; }
[Display(Name = "Pc1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PC__MUNIT", SqlFieldNameExt="PC__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pc1Munit { get; set; }
[Display(Name = "Pc1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("PC__HOME", SqlFieldNameExt="PC__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pc1Home { get; set; }
[Display(Name = "Pc1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("PC__VERSION", SqlFieldNameExt="PC__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pc1Version { get; set; }
[Display(Name = "Pc1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("PC__INACTIVE", SqlFieldNameExt="PC__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pc1Inactive { get; set; }
[Display(Name = "Pc1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("PC__EXTATT", SqlFieldNameExt="PC__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Pc1Extatt { get; set; }


[Display(Name = "Id Campione", ShortName="", Description = "Codice del campione", Prompt="")]
[ErpDogField("PC_ID_CAMPIONE", SqlFieldNameExt="PC_ID_CAMPIONE", SqlFieldOptions="[MANDATORY]", Xref="Cp1Icode", SqlFieldProperties="prop() xref(CAMPIONE.CP__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteServer("Campione", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? PcIdCampione  { get; set; }
public HealthDemo.Models.SIO.Act.Campione? PcIdCampioneObj  { get; set; }

[Display(Name = "Id Prestazione", ShortName="", Description = "Codice dell'atto", Prompt="")]
[ErpDogField("PC_ID_PRESTAZIONE", SqlFieldNameExt="PC_ID_PRESTAZIONE", SqlFieldOptions="[MANDATORY]", Xref="Pr1Icode", SqlFieldProperties="prop() xref(PRESTAZIONE.PR__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteServer("Prestazione", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? PcIdPrestazione  { get; set; }
public HealthDemo.Models.SIO.Act.Prestazione? PcIdPrestazioneObj  { get; set; }

[Display(Name = "Tipo", ShortName="", Description = "Generato da / Necessario per l'esecuzione [G/E]", Prompt="")]
[ErpDogField("PC_TIPO", SqlFieldNameExt="PC_TIPO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("E")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "G", "E" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? PcTipo  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("PC_NOTE", SqlFieldNameExt="PC_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? PcNote  { get; set; }

[Display(Name = "Id Tipo Campione", ShortName="", Description = "Tipo di campione", Prompt="")]
[ErpDogField("PC_ID_TIPO_CAMPIONE", SqlFieldNameExt="PC_ID_TIPO_CAMPIONE", SqlFieldOptions="", Xref="Tp1Icode", SqlFieldProperties="prop() xref(TIPO_CAMPIONE.TP__ICODE) xdup(CAMPIONE.CP_ID_TIPO_CAMPIONE[REL_PRESTAZIONE_CAMPIONE.PC_ID_CAMPIONE] {PC_ID_TIPO_CAMPIONE=' '}) multbxref()")]
[AutocompleteClient("TipoCampione", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PcIdTipoCampione  { get; set; }
public HealthDemo.Models.SIO.Act.TipoCampione? PcIdTipoCampioneObj  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioPc1Icode|K|PC__ICODE","sioPc1RecDate|N|PC__MDATE,PC__CDATE"
        ,"sioPcIdPrestazionepcIdCampionepcTipopc1Versionpc1Deleted|U|PC_ID_PRESTAZIONE,PC_ID_CAMPIONE,PC_TIPO,PC__VERSION,PC__DELETED"
        ,"sioPcIdPrestazionepcTipo|N|PC_ID_PRESTAZIONE,PC_TIPO"
        ,"sioPcIdTipoCampione|N|PC_ID_TIPO_CAMPIONE"
        ,"sioPcIdCampione|N|PC_ID_CAMPIONE"
        ,"sioPcIdTipoCampionepcIdPrestazionepcTipo|N|PC_ID_TIPO_CAMPIONE,PC_ID_PRESTAZIONE,PC_TIPO"
    };
}
}
}
