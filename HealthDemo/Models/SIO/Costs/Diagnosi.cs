using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Costs {
public class Diagnosi : ModelErp {
public const string Description = "Classificazioni diagnostiche adottate nelle organizzazioni sanitarie (ad esempio, DRG, AVG, ICD9, ecc.)";
public const string SqlTableName = "DIAGNOSI";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Dg1Icode";
public const string SqlRowIdName = "DG__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "DG_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "DG_XDATA";
public const string SqlXdataIcodeName = "DG_X__ICODE";
public const string SqlXdataDeletedName = "DG_X__DELETED";
public const string SqlXdataTimestampName = "DG_X__TIMESTAMP";
public const string SqlXdataCdateName = "DG_X__CDATE";
public const string SqlXdataCtimeName = "DG_X__CTIME";
public const string SqlXdataCagentName = "DG_X__CAGENT";
public const string SqlXdataCunitName = "DG_X__CUNIT";
public const string SqlXdataMdateName = "DG_X__MDATE";
public const string SqlXdataMtimeName = "DG_X__MTIME";
public const string SqlXdataMagentName = "DG_X__MAGENT";
public const string SqlXdataMunitName = "DG_X__MUNIT";
public const string SqlXdataHomeName = "DG_X__HOME";
public const string SqlXdataVersionName = "DG_X__VERSION";
public const string SqlXdataInactiveName = "DG_X__INACTIVE";
public const string SqlXdataExtattName = "DG_X__EXTATT";
public const string SqlXdataMrefName = "DG_X__MREF";
public const string SqlXdataSeqName = "DG_X__SEQ";
public const string SqlXdataDescrName = "DG_X__DESCR";
public const string SqlXdataFmtName = "DG_X__FMT";
public const string SqlXdataXdurlName = "DG_X__XDURL";
public const string SqlXdataXdatumName = "DG_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 63; //Internal Table Code
public const string TBAREA = "Controllo di gestione"; //Table Area
public const string PREFIX = "Dg"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Dg1Icode; } 
public override string labelText() { return $@"{DgCodice} - {DgDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(DgCodice)}</strong> {HttpUtility.HtmlEncode(DgDescrizione)}"; }

//622-593//[N] EPISODIO.EP_ID_DIAGNOSI_AMMISSIONE
[Display(Name = "Episodio", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Episodio>? XrefEpIdDiagnosiAmmissione { get; set; } = null;
//625-593//[N] EPISODIO.EP_ID_DIAGNOSI_DIMISSIONE
[Display(Name = "Episodio", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Patient.Episodio>? XrefEpIdDiagnosiDimissione { get; set; } = null;
//2597-2591//[N] DIAGNOSI.DG_ID_GRUPPO
[Display(Name = "Diagnosi", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Costs.Diagnosi>? XrefDgIdGruppo { get; set; } = null;
[Key]
[Display(Name = "Dg1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("DG__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dg1Icode { get; set; }
[Display(Name = "Dg1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("DG__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Dg1Deleted { get; set; }
[Display(Name = "Dg1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("DG__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Dg1Timestamp { get; set; }
[Display(Name = "Dg1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("DG__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Dg1Cdate { get; set; }
[Display(Name = "Dg1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("DG__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Dg1Ctime { get; set; }
[Display(Name = "Dg1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("DG__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dg1Cagent { get; set; }
[Display(Name = "Dg1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("DG__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dg1Cunit { get; set; }
[Display(Name = "Dg1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("DG__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Dg1Mdate { get; set; }
[Display(Name = "Dg1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("DG__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Dg1Mtime { get; set; }
[Display(Name = "Dg1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("DG__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dg1Magent { get; set; }
[Display(Name = "Dg1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("DG__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dg1Munit { get; set; }
[Display(Name = "Dg1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("DG__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dg1Home { get; set; }
[Display(Name = "Dg1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("DG__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dg1Version { get; set; }
[Display(Name = "Dg1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("DG__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Dg1Inactive { get; set; }
[Display(Name = "Dg1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("DG__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Dg1Extatt { get; set; }


[Display(Name = "Tipo Diagnosi", ShortName="", Description = "Codice del tipo di classificazione a cui l'istanza appartiene", Prompt="")]
[ErpDogField("DG_TIPO_DIAGNOSI", SqlFieldNameExt="", SqlFieldOptions="", Xref="Td1Icode", SqlFieldProperties="prop() xref(TIPO_DIAGNOSI.TD__ICODE) xdup() multbxref()")]
[AutocompleteClient("TipoDiagnosi", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? DgTipoDiagnosi  { get; set; }
public HealthDemo.Models.SIO.Costs.TipoDiagnosi? DgTipoDiagnosiObj  { get; set; }

[Display(Name = "Classe", ShortName="", Description = "Classificazione di aggregazione diagnostica definita dall'utente: 1: DRG 2: ICD9 3: ICD9-CM 4: APG, 5: AFO; 6: Specialità HC, ecc.", Prompt="")]
[ErpDogField("DG_CLASSE", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? DgClasse  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione", Prompt="")]
[ErpDogField("DG_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? DgDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("DG_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? DgNote  { get; set; }

[Display(Name = "Codice", ShortName="", Description = "Codice definito dall'utente per la classificazione", Prompt="")]
[ErpDogField("DG_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(DIAGNOSI.DG__ICODE[DG__ICODE] {DG_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? DgCodice  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Identificatore del codice di aggregazione nella gerarchia (se presente)", Prompt="")]
[ErpDogField("DG_ID_GRUPPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Dg1Icode", SqlFieldProperties="prop() xref(DIAGNOSI.DG__ICODE) xdup() multbxref()")]
[AutocompleteClient("Diagnosi", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? DgIdGruppo  { get; set; }
public HealthDemo.Models.SIO.Costs.Diagnosi? DgIdGruppoObj  { get; set; }

[Display(Name = "Tipo Drg", ShortName="", Description = "Tipo di DRG [M]edico - [C]hirurgico", Prompt="")]
[ErpDogField("DG_TIPO_DRG", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("M")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "M", "S" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? DgTipoDrg  { get; set; }

[Display(Name = "Tipo Icd9", ShortName="", Description = "Tipo di ICD9-CM [D]iagnostico - [O]perativo (se applicabile)", Prompt="")]
[ErpDogField("DG_TIPO_ICD9", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "D", "O", " " }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? DgTipoIcd9  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioDg1Icode|K|DG__ICODE","sioDg1RecDate|N|DG__MDATE,DG__CDATE"
        ,"sioDgTipoDiagnosidg1Versiondg1Deleted|U|DG_TIPO_DIAGNOSI,DG__VERSION,DG__DELETED"
        ,"sioDgIdGruppo|N|DG_ID_GRUPPO"
        ,"sioDgCodicedg1Versiondg1Deleted|U|DG_CODICE,DG__VERSION,DG__DELETED"
    };
}
}
}
