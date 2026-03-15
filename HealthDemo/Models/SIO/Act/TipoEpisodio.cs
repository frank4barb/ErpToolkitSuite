using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class TipoEpisodio : ModelErp {
public const string Description = "Classe di episodi";
public const string SqlTableName = "TIPO_EPISODIO";
public const string SqlTableNameExt = "TIPO_EPISODIO";
public const string SqlTableProperties = "";
public const string RowIdName = "Te1Icode";
public const string SqlRowIdName = "TE__ICODE";
public const string SqlRowIdNameExt = "TE__ICODE";
public const string SqlPrefix = "TE_";
public const string SqlPrefixExt = "TE_";
public const string SqlXdataTableName = "TE_XDATA";
public const string SqlXdataTableNameExt = "TE_XDATA";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 6; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Te"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Te1Icode; } 
public override string labelText() { return $@"{TeCodice} - {TeDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(TeCodice)}</strong> {HttpUtility.HtmlEncode(TeDescrizione)}"; }

//12-2//[N] PRESTAZIONE.PR_TIPO_EPISODIO
[Display(Name = "Prestazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Prestazione>? XrefPrTipoEpisodio { get; set; } = null;
//599-593//[N] EPISODIO.EP_ID_TIPO_EPISODIO
[Display(Name = "Episodio", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Episodio>? XrefEpIdTipoEpisodio { get; set; } = null;
[Key]
[Display(Name = "Te1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("TE__ICODE", SqlFieldNameExt="TE__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Te1Icode { get; set; }
[Display(Name = "Te1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("TE__DELETED", SqlFieldNameExt="TE__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Te1Deleted { get; set; }
[Display(Name = "Te1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("TE__TIMESTAMP", SqlFieldNameExt="TE__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Te1Timestamp { get; set; }
[Display(Name = "Te1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TE__CDATE", SqlFieldNameExt="TE__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Te1Cdate { get; set; }
[Display(Name = "Te1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TE__CTIME", SqlFieldNameExt="TE__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Te1Ctime { get; set; }
[Display(Name = "Te1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TE__CAGENT", SqlFieldNameExt="TE__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Te1Cagent { get; set; }
[Display(Name = "Te1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TE__CUNIT", SqlFieldNameExt="TE__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Te1Cunit { get; set; }
[Display(Name = "Te1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TE__MDATE", SqlFieldNameExt="TE__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Te1Mdate { get; set; }
[Display(Name = "Te1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TE__MTIME", SqlFieldNameExt="TE__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Te1Mtime { get; set; }
[Display(Name = "Te1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TE__MAGENT", SqlFieldNameExt="TE__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Te1Magent { get; set; }
[Display(Name = "Te1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TE__MUNIT", SqlFieldNameExt="TE__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Te1Munit { get; set; }
[Display(Name = "Te1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("TE__HOME", SqlFieldNameExt="TE__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Te1Home { get; set; }
[Display(Name = "Te1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("TE__VERSION", SqlFieldNameExt="TE__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Te1Version { get; set; }
[Display(Name = "Te1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("TE__INACTIVE", SqlFieldNameExt="TE__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Te1Inactive { get; set; }
[Display(Name = "Te1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("TE__EXTATT", SqlFieldNameExt="TE__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Te1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TE_CODICE", SqlFieldNameExt="TE_CODICE", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_EPISODIO.TE__ICODE[TE__ICODE] {TE_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TeCodice  { get; set; }

[Display(Name = "Classe", ShortName="", Description = "Classe di contatto 1=Ricovero - 2=Day-hospital - 3=Ambulatorio", Prompt="")]
[ErpDogField("TE_CLASSE", SqlFieldNameExt="TE_CLASSE", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? TeClasse  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TE_DESCRIZIONE", SqlFieldNameExt="TE_DESCRIZIONE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TeDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TE_NOTE", SqlFieldNameExt="TE_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TeNote  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTe1Icode|K|TE__ICODE","sioTe1RecDate|N|TE__MDATE,TE__CDATE"
        ,"sioTeClassete1Versionte1Deleted|U|TE_CLASSE,TE__VERSION,TE__DELETED"
        ,"sioTeCodicete1Versionte1Deleted|U|TE_CODICE,TE__VERSION,TE__DELETED"
    };
}
}
}
