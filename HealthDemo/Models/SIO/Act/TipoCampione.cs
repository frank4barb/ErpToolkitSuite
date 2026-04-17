using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class TipoCampione : ModelErp {
public const string Description = "Tipo di campione";
public const string SqlTableName = "TIPO_CAMPIONE";
public const string SqlTableNameExt = "TIPO_CAMPIONE";
public const string SqlTableProperties = "";
public const string RowIdName = "Tp1Icode";
public const string SqlRowIdName = "TP__ICODE";
public const string SqlRowIdNameExt = "TP__ICODE";
public const string SqlPrefix = "TP_";
public const string SqlPrefixExt = "TP_";
public const string SqlXdataTableName = "TP_XDATA";
public const string SqlXdataIcodeName = "TP_X__ICODE";
public const string SqlXdataDeletedName = "TP_X__DELETED";
public const string SqlXdataTimestampName = "TP_X__TIMESTAMP";
public const string SqlXdataCdateName = "TP_X__CDATE";
public const string SqlXdataCtimeName = "TP_X__CTIME";
public const string SqlXdataCagentName = "TP_X__CAGENT";
public const string SqlXdataCunitName = "TP_X__CUNIT";
public const string SqlXdataMdateName = "TP_X__MDATE";
public const string SqlXdataMtimeName = "TP_X__MTIME";
public const string SqlXdataMagentName = "TP_X__MAGENT";
public const string SqlXdataMunitName = "TP_X__MUNIT";
public const string SqlXdataHomeName = "TP_X__HOME";
public const string SqlXdataVersionName = "TP_X__VERSION";
public const string SqlXdataInactiveName = "TP_X__INACTIVE";
public const string SqlXdataExtattName = "TP_X__EXTATT";
public const string SqlXdataMrefName = "TP_X__MREF";
public const string SqlXdataSeqName = "TP_X__SEQ";
public const string SqlXdataDescrName = "TP_X__DESCR";
public const string SqlXdataFmtName = "TP_X__FMT";
public const string SqlXdataXdurlName = "TP_X__XDURL";
public const string SqlXdataXdatumName = "TP_X__XDATUM";
public const string SqlXdataTableNameExt = "TP_XDATA";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 100; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Tp"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Tp1Icode; } 
public override string labelText() { return $@"{TpCodice} - {TpDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(TpCodice)}</strong> {HttpUtility.HtmlEncode(TpDescrizione)}"; }

//123-119//[Y] REL_PRESTAZIONE_CAMPIONE.PC_ID_TIPO_CAMPIONE
[Display(Name = "RelPrestazioneCampione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelPrestazioneCampione>? XrefPcIdTipoCampione { get; set; } = null;
//371-370//[Y] REL_ATTIVITA_TIPO_CAMPIONE.AC_ID_TIPO_CAMPIONE
[Display(Name = "RelAttivitaTipoCampione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaTipoCampione>? XrefAcIdTipoCampione { get; set; } = null;
//1730-1730//[N] CAMPIONE.CP_ID_TIPO_CAMPIONE
[Display(Name = "Campione", ShortName = "", Description = "Campione effettivo raccolto durante le attività quotidiane", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.Campione>? XrefCpIdTipoCampione { get; set; } = null;
[Key]
[Display(Name = "Tp1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("TP__ICODE", SqlFieldNameExt="TP__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tp1Icode { get; set; }
[Display(Name = "Tp1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("TP__DELETED", SqlFieldNameExt="TP__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Tp1Deleted { get; set; }
[Display(Name = "Tp1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("TP__TIMESTAMP", SqlFieldNameExt="TP__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Tp1Timestamp { get; set; }
[Display(Name = "Tp1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TP__CDATE", SqlFieldNameExt="TP__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Tp1Cdate { get; set; }
[Display(Name = "Tp1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("TP__CTIME", SqlFieldNameExt="TP__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Tp1Ctime { get; set; }
[Display(Name = "Tp1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TP__CAGENT", SqlFieldNameExt="TP__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tp1Cagent { get; set; }
[Display(Name = "Tp1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("TP__CUNIT", SqlFieldNameExt="TP__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tp1Cunit { get; set; }
[Display(Name = "Tp1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TP__MDATE", SqlFieldNameExt="TP__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Tp1Mdate { get; set; }
[Display(Name = "Tp1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("TP__MTIME", SqlFieldNameExt="TP__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Tp1Mtime { get; set; }
[Display(Name = "Tp1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TP__MAGENT", SqlFieldNameExt="TP__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tp1Magent { get; set; }
[Display(Name = "Tp1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("TP__MUNIT", SqlFieldNameExt="TP__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tp1Munit { get; set; }
[Display(Name = "Tp1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("TP__HOME", SqlFieldNameExt="TP__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tp1Home { get; set; }
[Display(Name = "Tp1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("TP__VERSION", SqlFieldNameExt="TP__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Tp1Version { get; set; }
[Display(Name = "Tp1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("TP__INACTIVE", SqlFieldNameExt="TP__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Tp1Inactive { get; set; }
[Display(Name = "Tp1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("TP__EXTATT", SqlFieldNameExt="TP__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Tp1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TP_CODICE", SqlFieldNameExt="TP_CODICE", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_CAMPIONE.TP__ICODE[TP__ICODE] {TP_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? TpCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TP_DESCRIZIONE", SqlFieldNameExt="TP_DESCRIZIONE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TpDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TP_NOTE", SqlFieldNameExt="TP_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TpNote  { get; set; }

[Display(Name = "Contesto", ShortName="", Description = "Identificazione del contesto o dei contesti in cui il tipo di campione ha particolare rilevanza", Prompt="")]
[ErpDogField("TP_CONTESTO", SqlFieldNameExt="TP_CONTESTO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? TpContesto  { get; set; }

[Display(Name = "Contenitore", ShortName="", Description = "Descrizione del contenitore", Prompt="")]
[ErpDogField("TP_CONTENITORE", SqlFieldNameExt="TP_CONTENITORE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? TpContenitore  { get; set; }

[Display(Name = "Attributi", ShortName="", Description = "Flag operativi, gestiti dall'applicazione", Prompt="")]
[ErpDogField("TP_ATTRIBUTI", SqlFieldNameExt="TP_ATTRIBUTI", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? TpAttributi  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTp1Icode|K|TP__ICODE","sioTp1RecDate|N|TP__MDATE,TP__CDATE"
        ,"sioTpContesto|N|TP_CONTESTO"
        ,"sioTp1Versiontp1Deleted|U|TP__VERSION,TP__DELETED"
        ,"sioTpCodicetp1Versiontp1Deleted|U|TP_CODICE,TP__VERSION,TP__DELETED"
    };
}
}
}
