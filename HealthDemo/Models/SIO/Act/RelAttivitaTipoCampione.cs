using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class RelAttivitaTipoCampione : ModelErp {
public const string Description = "Tipo di campione rilevante per un certo tipo di attività";
public const string SqlTableName = "REL_ATTIVITA_TIPO_CAMPIONE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Ac1Icode";
public const string SqlRowIdName = "AC__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "AC_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "AC_XDATA";
public const string SqlXdataIcodeName = "AC_X__ICODE";
public const string SqlXdataDeletedName = "AC_X__DELETED";
public const string SqlXdataTimestampName = "AC_X__TIMESTAMP";
public const string SqlXdataCdateName = "AC_X__CDATE";
public const string SqlXdataCtimeName = "AC_X__CTIME";
public const string SqlXdataCagentName = "AC_X__CAGENT";
public const string SqlXdataCunitName = "AC_X__CUNIT";
public const string SqlXdataMdateName = "AC_X__MDATE";
public const string SqlXdataMtimeName = "AC_X__MTIME";
public const string SqlXdataMagentName = "AC_X__MAGENT";
public const string SqlXdataMunitName = "AC_X__MUNIT";
public const string SqlXdataHomeName = "AC_X__HOME";
public const string SqlXdataVersionName = "AC_X__VERSION";
public const string SqlXdataInactiveName = "AC_X__INACTIVE";
public const string SqlXdataExtattName = "AC_X__EXTATT";
public const string SqlXdataMrefName = "AC_X__MREF";
public const string SqlXdataSeqName = "AC_X__SEQ";
public const string SqlXdataDescrName = "AC_X__DESCR";
public const string SqlXdataFmtName = "AC_X__FMT";
public const string SqlXdataXdurlName = "AC_X__XDURL";
public const string SqlXdataXdatumName = "AC_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 10; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Ac"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ac1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

[Key]
[Display(Name = "Ac1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("AC__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ac1Icode { get; set; }
[Display(Name = "Ac1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("AC__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ac1Deleted { get; set; }
[Display(Name = "Ac1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("AC__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ac1Timestamp { get; set; }
[Display(Name = "Ac1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AC__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ac1Cdate { get; set; }
[Display(Name = "Ac1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AC__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ac1Ctime { get; set; }
[Display(Name = "Ac1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AC__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ac1Cagent { get; set; }
[Display(Name = "Ac1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AC__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ac1Cunit { get; set; }
[Display(Name = "Ac1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AC__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ac1Mdate { get; set; }
[Display(Name = "Ac1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AC__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ac1Mtime { get; set; }
[Display(Name = "Ac1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AC__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ac1Magent { get; set; }
[Display(Name = "Ac1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AC__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ac1Munit { get; set; }
[Display(Name = "Ac1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("AC__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ac1Home { get; set; }
[Display(Name = "Ac1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("AC__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ac1Version { get; set; }
[Display(Name = "Ac1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("AC__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ac1Inactive { get; set; }
[Display(Name = "Ac1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("AC__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ac1Extatt { get; set; }


[Display(Name = "Id Attivita", ShortName="", Description = "Codice del tipo di attività", Prompt="")]
[ErpDogField("AC_ID_ATTIVITA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Av1Icode", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("Attivita", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? AcIdAttivita  { get; set; }
public HealthDemo.Models.SIO.Act.Attivita? AcIdAttivitaObj  { get; set; }

[Display(Name = "Id Tipo Campione", ShortName="", Description = "Codice del tipo di campione", Prompt="")]
[ErpDogField("AC_ID_TIPO_CAMPIONE", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Tp1Icode", SqlFieldProperties="prop() xref(TIPO_CAMPIONE.TP__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoCampione", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? AcIdTipoCampione  { get; set; }
public HealthDemo.Models.SIO.Act.TipoCampione? AcIdTipoCampioneObj  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("AC_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? AcNote  { get; set; }

[Display(Name = "Tipo", ShortName="", Description = "Generato da / Necessario per l'esecuzione", Prompt="")]
[ErpDogField("AC_TIPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("E")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "G", "E" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AcTipo  { get; set; }

[Display(Name = "Campione Preferenziale", ShortName="", Description = "Tipo di campione preferenziale (predefinito) per quel tipo di attività", Prompt="")]
[ErpDogField("AC_CAMPIONE_PREFERENZIALE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("N")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AcCampionePreferenziale  { get; set; }

[Display(Name = "Campione Specifico", ShortName="", Description = "Se 'Y', è necessario un campione dedicato, e il campione non può essere condiviso tra diverse attività (predefinito N)", Prompt="")]
[ErpDogField("AC_CAMPIONE_SPECIFICO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("N")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AcCampioneSpecifico  { get; set; }

[Display(Name = "Regole Campionamento", ShortName="", Description = "Criteri da adottare quando si raccolgono più campioni (informazioni testuali, dedicate dall'utente)", Prompt="")]
[ErpDogField("AC_REGOLE_CAMPIONAMENTO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(240, ErrorMessage = "Inserire massimo 240 caratteri")]
[DataType(DataType.Text)]
public string? AcRegoleCampionamento  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioAc1Icode|K|AC__ICODE","sioAc1RecDate|N|AC__MDATE,AC__CDATE"
        ,"sioAcIdAttivitaacTipo|N|AC_ID_ATTIVITA,AC_TIPO"
        ,"sioAcIdTipoCampione|N|AC_ID_TIPO_CAMPIONE"
    };
}
}
}
