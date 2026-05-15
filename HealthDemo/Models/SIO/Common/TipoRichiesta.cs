using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Common {
public class TipoRichiesta : ModelErp {
public const string Description = "Tipo di richieste";
public const string SqlTableName = "TIPO_RICHIESTA";
public const string SqlTableNameExt = "TIPO_RICHIESTA";
public const string SqlTableProperties = "";
public const string RowIdName = "Ti1Icode";
public const string SqlRowIdName = "TI__ICODE";
public const string SqlRowIdNameExt = "TI__ICODE";
public const string SqlPrefix = "TI_";
public const string SqlPrefixExt = "TI_";
public const string SqlXdataTableName = "TI_XDATA";
public const string SqlXdataIcodeName = "TI_X__ICODE";
public const string SqlXdataDeletedName = "TI_X__DELETED";
public const string SqlXdataTimestampName = "TI_X__TIMESTAMP";
public const string SqlXdataCdateName = "TI_X__CDATE";
public const string SqlXdataCtimeName = "TI_X__CTIME";
public const string SqlXdataCagentName = "TI_X__CAGENT";
public const string SqlXdataCunitName = "TI_X__CUNIT";
public const string SqlXdataMdateName = "TI_X__MDATE";
public const string SqlXdataMtimeName = "TI_X__MTIME";
public const string SqlXdataMagentName = "TI_X__MAGENT";
public const string SqlXdataMunitName = "TI_X__MUNIT";
public const string SqlXdataHomeName = "TI_X__HOME";
public const string SqlXdataVersionName = "TI_X__VERSION";
public const string SqlXdataInactiveName = "TI_X__INACTIVE";
public const string SqlXdataExtattName = "TI_X__EXTATT";
public const string SqlXdataMrefName = "TI_X__MREF";
public const string SqlXdataSeqName = "TI_X__SEQ";
public const string SqlXdataDescrName = "TI_X__DESCR";
public const string SqlXdataFmtName = "TI_X__FMT";
public const string SqlXdataXdurlName = "TI_X__XDURL";
public const string SqlXdataXdatumName = "TI_X__XDATUM";
public const string SqlXdataTableNameExt = "TI_XDATA";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 48; //Internal Table Code
public const string TBAREA = "Organizzazione ospedaliera"; //Table Area
public const string PREFIX = "Ti"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ti1Icode; } 
public override string labelText() { return $@"{TiCodice} - {TiDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(TiCodice)}</strong> {HttpUtility.HtmlEncode(TiDescrizione)}"; }

//542-524//[N] RICHIESTA.RI_ID_TIPO_RICHIESTA
[Display(Name = "Richiesta", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Common.Richiesta>? XrefRiIdTipoRichiesta { get; set; } = null;
[Key]
[Display(Name = "Ti1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("TI__ICODE", SqlFieldNameExt="TI__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ti1Icode { get; set; }
[Display(Name = "Ti1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("TI__DELETED", SqlFieldNameExt="TI__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ti1Deleted { get; set; }
[Display(Name = "Ti1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("TI__TIMESTAMP", SqlFieldNameExt="TI__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ti1Timestamp { get; set; }
[Display(Name = "Ti1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TI__CDATE", SqlFieldNameExt="TI__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ti1Cdate { get; set; }
[Display(Name = "Ti1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TI__CTIME", SqlFieldNameExt="TI__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ti1Ctime { get; set; }
[Display(Name = "Ti1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TI__CAGENT", SqlFieldNameExt="TI__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ti1Cagent { get; set; }
[Display(Name = "Ti1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TI__CUNIT", SqlFieldNameExt="TI__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ti1Cunit { get; set; }
[Display(Name = "Ti1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TI__MDATE", SqlFieldNameExt="TI__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ti1Mdate { get; set; }
[Display(Name = "Ti1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TI__MTIME", SqlFieldNameExt="TI__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ti1Mtime { get; set; }
[Display(Name = "Ti1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TI__MAGENT", SqlFieldNameExt="TI__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ti1Magent { get; set; }
[Display(Name = "Ti1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TI__MUNIT", SqlFieldNameExt="TI__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ti1Munit { get; set; }
[Display(Name = "Ti1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("TI__HOME", SqlFieldNameExt="TI__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ti1Home { get; set; }
[Display(Name = "Ti1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("TI__VERSION", SqlFieldNameExt="TI__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ti1Version { get; set; }
[Display(Name = "Ti1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("TI__INACTIVE", SqlFieldNameExt="TI__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ti1Inactive { get; set; }
[Display(Name = "Ti1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("TI__EXTATT", SqlFieldNameExt="TI__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ti1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TI_CODICE", SqlFieldNameExt="TI_CODICE", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RICHIESTA.TI__ICODE[TI__ICODE] {TI_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TiCodice  { get; set; }

[Display(Name = "Gruppo", ShortName="", Description = "Classe di comunicazione: 0 = Comunicazioni di sistema 1 = Messaggi utente - 2 = Relativi agli atti - Z = Utente-d", Prompt="")]
[ErpDogField("TI_GRUPPO", SqlFieldNameExt="TI_GRUPPO", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "0", "1", "2", "Z" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? TiGruppo  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione", Prompt="")]
[ErpDogField("TI_DESCRIZIONE", SqlFieldNameExt="TI_DESCRIZIONE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TiDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TI_NOTE", SqlFieldNameExt="TI_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TiNote  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTi1Icode|K|TI__ICODE","sioTi1RecDate|N|TI__MDATE,TI__CDATE"
        ,"sioTiGruppoti1Versionti1Deleted|U|TI_GRUPPO,TI__VERSION,TI__DELETED"
        ,"sioTiCodiceti1Versionti1Deleted|U|TI_CODICE,TI__VERSION,TI__DELETED"
    };
}
}
}
