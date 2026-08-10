using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.HealthData {
public class ParametroVitale : ModelErp {
public const string Description = "Dati sanitari - Parametri vitali";
public const string SqlTableName = "PARAMETRO_VITALE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Pv1Icode";
public const string SqlRowIdName = "PV__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "PV_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "PV_XDATA";
public const string SqlXdataIcodeName = "PV_X__ICODE";
public const string SqlXdataDeletedName = "PV_X__DELETED";
public const string SqlXdataTimestampName = "PV_X__TIMESTAMP";
public const string SqlXdataCdateName = "PV_X__CDATE";
public const string SqlXdataCtimeName = "PV_X__CTIME";
public const string SqlXdataCagentName = "PV_X__CAGENT";
public const string SqlXdataCunitName = "PV_X__CUNIT";
public const string SqlXdataMdateName = "PV_X__MDATE";
public const string SqlXdataMtimeName = "PV_X__MTIME";
public const string SqlXdataMagentName = "PV_X__MAGENT";
public const string SqlXdataMunitName = "PV_X__MUNIT";
public const string SqlXdataHomeName = "PV_X__HOME";
public const string SqlXdataVersionName = "PV_X__VERSION";
public const string SqlXdataInactiveName = "PV_X__INACTIVE";
public const string SqlXdataExtattName = "PV_X__EXTATT";
public const string SqlXdataMrefName = "PV_X__MREF";
public const string SqlXdataSeqName = "PV_X__SEQ";
public const string SqlXdataDescrName = "PV_X__DESCR";
public const string SqlXdataFmtName = "PV_X__FMT";
public const string SqlXdataXdurlName = "PV_X__XDURL";
public const string SqlXdataXdatumName = "PV_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 38; //Internal Table Code
public const string TBAREA = "Dati clinici"; //Table Area
public const string PREFIX = "Pv"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Pv1Icode; } 
public override string labelText() { return $@"{Pv1Icode} - {PvNote}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(Pv1Icode)}</strong> {HttpUtility.HtmlEncode(PvNote)}"; }

//1026-1025//[Y] REL_PRESTAZIONE_DATO_CLINICO.PD_ID_DATO_CLINICO
[Display(Name = "RelPrestazioneDatoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.RelPrestazioneDatoClinico>? XrefPdIdDatoClinico { get; set; } = null;
[Key]
[Display(Name = "Pv1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("PV__ICODE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[SID] [LABEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pv1Icode { get; set; }
[Display(Name = "Pv1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("PV__DELETED", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pv1Deleted { get; set; }
[Display(Name = "Pv1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("PV__TIMESTAMP", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Pv1Timestamp { get; set; }
[Display(Name = "Pv1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PV__CDATE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pv1Cdate { get; set; }
[Display(Name = "Pv1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PV__CTIME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pv1Ctime { get; set; }
[Display(Name = "Pv1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PV__CAGENT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pv1Cagent { get; set; }
[Display(Name = "Pv1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PV__CUNIT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pv1Cunit { get; set; }
[Display(Name = "Pv1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PV__MDATE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pv1Mdate { get; set; }
[Display(Name = "Pv1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PV__MTIME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pv1Mtime { get; set; }
[Display(Name = "Pv1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PV__MAGENT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pv1Magent { get; set; }
[Display(Name = "Pv1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PV__MUNIT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pv1Munit { get; set; }
[Display(Name = "Pv1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("PV__HOME", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pv1Home { get; set; }
[Display(Name = "Pv1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("PV__VERSION", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pv1Version { get; set; }
[Display(Name = "Pv1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("PV__INACTIVE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pv1Inactive { get; set; }
[Display(Name = "Pv1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("PV__EXTATT", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Pv1Extatt { get; set; }


[Display(Name = "Classe", ShortName="", Description = "Classe del dato sanitario: 1 - Parametri vitali", Prompt="")]
[ErpDogField("PV_CLASSE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("1")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? PvClasse  { get; set; }

[Display(Name = "Id Paziente", ShortName="", Description = "Codice del paziente a cui si riferisce il dato sanitario", Prompt="")]
[ErpDogField("PV_ID_PAZIENTE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Pa1Icode", SqlFieldProperties="prop() xref(PAZIENTE.PA__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteServer("Paziente", "AutocompleteGetSelect", "AutocompletePreLoad", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? PvIdPaziente  { get; set; }
public HealthDemo.Models.SIO.Patient.Paziente? PvIdPazienteObj  { get; set; }

[Display(Name = "Id Episodio", ShortName="", Description = "Codice del contatto a cui si riferisce il Dato Sanitario", Prompt="")]
[ErpDogField("PV_ID_EPISODIO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="Ep1Icode", SqlFieldProperties="prop() xref(EPISODIO.EP__ICODE) xdup() multbxref()")]
[AutocompleteServer("Episodio", "AutocompleteGetSelect", "AutocompletePreLoad", 1, ExtraFilter:"{In(\"EP_ID_PAZIENTE\", \"PvIdPaziente\")}", ExtraFields: "PvIdPaziente")]
[DataType(DataType.Text)]
public string? PvIdEpisodio  { get; set; }
public HealthDemo.Models.SIO.Patient.Episodio? PvIdEpisodioObj  { get; set; }

[Display(Name = "Id Tipo Dato Clinico", ShortName="", Description = "Codice del tipo di Dato Sanitario", Prompt="")]
[ErpDogField("PV_ID_TIPO_DATO_CLINICO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Tc1Icode", SqlFieldProperties="prop() xref(TIPO_DATO_CLINICO.TC__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoDatoClinico", "AutocompleteGetAll", 1, ExtraFilter:"{EqVal(\"TC_CLASSE\", \"1\")}", ExtraFields: "")]
[DataType(DataType.Text)]
public string? PvIdTipoDatoClinico  { get; set; }
public HealthDemo.Models.SIO.HealthData.TipoDatoClinico? PvIdTipoDatoClinicoObj  { get; set; }

[Display(Name = "Id Gruppo Dato Clinico", ShortName="", Description = "Classe del tipo di dati sanitari", Prompt="")]
[ErpDogField("PV_ID_GRUPPO_DATO_CLINICO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="Cc1Icode", SqlFieldProperties="prop() xref(CATEGORIA_DATO_CLINICO.CC__ICODE) xdup(TIPO_DATO_CLINICO.TC_ID_CATEGORIA_DATO_CLINICO[PARAMETRO_VITALE.PV_ID_TIPO_DATO_CLINICO]) multbxref()")]
[AutocompleteClient("CategoriaDatoClinico", "AutocompleteGetAll", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? PvIdGruppoDatoClinico  { get; set; }
public HealthDemo.Models.SIO.HealthData.CategoriaDatoClinico? PvIdGruppoDatoClinicoObj  { get; set; }

[Display(Name = "Valore Minimo", ShortName="", Description = "Valori numerici minimi (se applicabile)", Prompt="")]
[ErpDogField("PV_VALORE_MINIMO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? PvValoreMinimo  { get; set; }

[Display(Name = "Valore Massimo", ShortName="", Description = "Valori numerici massimi (se applicabile)", Prompt="")]
[ErpDogField("PV_VALORE_MASSIMO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? PvValoreMassimo  { get; set; }

[Display(Name = "Valore Scelta", ShortName="", Description = "Valore carattere (se applicabile, in base al tipo di risultato)", Prompt="")]
[ErpDogField("PV_VALORE_SCELTA", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? PvValoreScelta  { get; set; }

[Display(Name = "Valore Testo", ShortName="", Description = "Valore testuale, se applicabile in base al tipo di dato", Prompt="")]
[ErpDogField("PV_VALORE_TESTO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(4000, ErrorMessage = "Inserire massimo 4000 caratteri")]
[DataType(DataType.Text)]
public string? PvValoreTesto  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note (se applicabile, in base al tipo di risultato)", Prompt="")]
[ErpDogField("PV_NOTE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? PvNote  { get; set; }

[Display(Name = "Codice Referto", ShortName="", Description = "Criterio di codifica/unità di misura adottato (se applicabile)", Prompt="")]
[ErpDogField("PV_CODICE_REFERTO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[XID]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? PvCodiceReferto  { get; set; }

[Display(Name = "Data Acquisizione", ShortName="", Description = "Data di acquisizione del dato sanitario", Prompt="")]
[ErpDogField("PV_DATA_ACQUISIZIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? PvDataAcquisizione  { get; set; }

[Display(Name = "Ora Acquisizione", ShortName="", Description = "Ora di acquisizione del dato sanitario", Prompt="")]
[ErpDogField("PV_ORA_ACQUISIZIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? PvOraAcquisizione  { get; set; }

[Display(Name = "Stato Dato Clinico", ShortName="", Description = "Stato del dato: P[reliminare] - C[onfermato] - A[nnullato]", Prompt="")]
[ErpDogField("PV_STATO_DATO_CLINICO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("P")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "P", "C", "A" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? PvStatoDatoClinico  { get; set; }

[Display(Name = "Data Validazione", ShortName="", Description = "Data di convalida del dato sanitario", Prompt="")]
[ErpDogField("PV_DATA_VALIDAZIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? PvDataValidazione  { get; set; }

[Display(Name = "Ora Validazione", ShortName="", Description = "Ora di convalida del dato sanitario", Prompt="")]
[ErpDogField("PV_ORA_VALIDAZIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? PvOraValidazione  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Numero di sequenza del dato nel rapporto originale", Prompt="")]
[ErpDogField("PV_SEQUENZA", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? PvSequenza  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioPv1Icode|K|PV__ICODE","sioPv1RecDate|N|PV__MDATE,PV__CDATE"
        ,"sioPvIdEpisodiopvIdTipoDatoClinicopvDataAcquisizione|N|PV_ID_EPISODIO,PV_ID_TIPO_DATO_CLINICO,PV_DATA_ACQUISIZIONE"
        ,"sioPvIdPazientepvDataAcquisizione|N|PV_ID_PAZIENTE,PV_DATA_ACQUISIZIONE"
        ,"sioPvIdTipoDatoClinicopvStatoDatoClinicopvDataAcquisizione|N|PV_ID_TIPO_DATO_CLINICO,PV_STATO_DATO_CLINICO,PV_DATA_ACQUISIZIONE"
        ,"sioPvCodiceReferto|N|PV_CODICE_REFERTO"
    };
}
}
}
