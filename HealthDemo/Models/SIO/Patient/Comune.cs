using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Patient {
public class Comune : ModelErp {
public const string Description = "Comuni";
public const string SqlTableName = "COMUNE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Cm1Icode";
public const string SqlRowIdName = "CM__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "CM_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "CM_XDATA";
public const string SqlXdataIcodeName = "CM_X__ICODE";
public const string SqlXdataDeletedName = "CM_X__DELETED";
public const string SqlXdataTimestampName = "CM_X__TIMESTAMP";
public const string SqlXdataCdateName = "CM_X__CDATE";
public const string SqlXdataCtimeName = "CM_X__CTIME";
public const string SqlXdataCagentName = "CM_X__CAGENT";
public const string SqlXdataCunitName = "CM_X__CUNIT";
public const string SqlXdataMdateName = "CM_X__MDATE";
public const string SqlXdataMtimeName = "CM_X__MTIME";
public const string SqlXdataMagentName = "CM_X__MAGENT";
public const string SqlXdataMunitName = "CM_X__MUNIT";
public const string SqlXdataHomeName = "CM_X__HOME";
public const string SqlXdataVersionName = "CM_X__VERSION";
public const string SqlXdataInactiveName = "CM_X__INACTIVE";
public const string SqlXdataExtattName = "CM_X__EXTATT";
public const string SqlXdataMrefName = "CM_X__MREF";
public const string SqlXdataSeqName = "CM_X__SEQ";
public const string SqlXdataDescrName = "CM_X__DESCR";
public const string SqlXdataFmtName = "CM_X__FMT";
public const string SqlXdataXdurlName = "CM_X__XDURL";
public const string SqlXdataXdatumName = "CM_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 55; //Internal Table Code
public const string TBAREA = "Accoglienza"; //Table Area
public const string PREFIX = "Cm"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Cm1Icode; } 
public override string labelText() { return $@"{CmCodice} - {CmNome}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(CmCodice)}</strong> {HttpUtility.HtmlEncode(CmNome)}"; }

//748-744//[N] DISTRETTO.DI_ID_COMUNE
[Display(Name = "Distretto", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Distretto>? XrefDiIdComune { get; set; } = null;
//1299-1286//[N] PAZIENTE.PA_ID_COMUNE_NASCITA
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdComuneNascita { get; set; } = null;
//1307-1286//[N] PAZIENTE.PA_ID_COMUNE_RES
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdComuneRes { get; set; } = null;
//1318-1286//[N] PAZIENTE.PA_ID_COMUNE_DOM
[Display(Name = "Paziente", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Paziente>? XrefPaIdComuneDom { get; set; } = null;
[Key]
[Display(Name = "Cm1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("CM__ICODE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cm1Icode { get; set; }
[Display(Name = "Cm1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("CM__DELETED", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Cm1Deleted { get; set; }
[Display(Name = "Cm1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("CM__TIMESTAMP", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Cm1Timestamp { get; set; }
[Display(Name = "Cm1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("CM__CDATE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Cm1Cdate { get; set; }
[Display(Name = "Cm1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("CM__CTIME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Cm1Ctime { get; set; }
[Display(Name = "Cm1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("CM__CAGENT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cm1Cagent { get; set; }
[Display(Name = "Cm1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("CM__CUNIT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cm1Cunit { get; set; }
[Display(Name = "Cm1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("CM__MDATE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Cm1Mdate { get; set; }
[Display(Name = "Cm1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("CM__MTIME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Cm1Mtime { get; set; }
[Display(Name = "Cm1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("CM__MAGENT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cm1Magent { get; set; }
[Display(Name = "Cm1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("CM__MUNIT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cm1Munit { get; set; }
[Display(Name = "Cm1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("CM__HOME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cm1Home { get; set; }
[Display(Name = "Cm1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("CM__VERSION", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Cm1Version { get; set; }
[Display(Name = "Cm1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("CM__INACTIVE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Cm1Inactive { get; set; }
[Display(Name = "Cm1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("CM__EXTATT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Cm1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice nazionale della città", Prompt="")]
[ErpDogField("CM_CODICE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? CmCodice  { get; set; }

[Display(Name = "Nome", ShortName="", Description = "Nome esteso", Prompt="")]
[ErpDogField("CM_NOME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue("")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? CmNome  { get; set; }

[Display(Name = "Cod Istat", ShortName="", Description = "Codice statistico per la città", Prompt="")]
[ErpDogField("CM_COD_ISTAT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? CmCodIstat  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note testuali", Prompt="")]
[ErpDogField("CM_NOTE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? CmNote  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioCm1Icode|K|CM__ICODE","sioCm1RecDate|N|CM__MDATE,CM__CDATE"
        ,"sioCmNome|N|CM_NOME"
    };
}
}
}
