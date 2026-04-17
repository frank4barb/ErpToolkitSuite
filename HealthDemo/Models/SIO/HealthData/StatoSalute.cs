using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.HealthData {
public class StatoSalute : ModelErp {
public const string Description = "Dati sanitari - Condizioni di salute generiche";
public const string SqlTableName = "STATO_SALUTE";
public const string SqlTableNameExt = "STATO_SALUTE";
public const string SqlTableProperties = "";
public const string RowIdName = "Ss1Icode";
public const string SqlRowIdName = "SS__ICODE";
public const string SqlRowIdNameExt = "SS__ICODE";
public const string SqlPrefix = "SS_";
public const string SqlPrefixExt = "SS_";
public const string SqlXdataTableName = "SS_XDATA";
public const string SqlXdataIcodeName = "SS_X__ICODE";
public const string SqlXdataDeletedName = "SS_X__DELETED";
public const string SqlXdataTimestampName = "SS_X__TIMESTAMP";
public const string SqlXdataCdateName = "SS_X__CDATE";
public const string SqlXdataCtimeName = "SS_X__CTIME";
public const string SqlXdataCagentName = "SS_X__CAGENT";
public const string SqlXdataCunitName = "SS_X__CUNIT";
public const string SqlXdataMdateName = "SS_X__MDATE";
public const string SqlXdataMtimeName = "SS_X__MTIME";
public const string SqlXdataMagentName = "SS_X__MAGENT";
public const string SqlXdataMunitName = "SS_X__MUNIT";
public const string SqlXdataHomeName = "SS_X__HOME";
public const string SqlXdataVersionName = "SS_X__VERSION";
public const string SqlXdataInactiveName = "SS_X__INACTIVE";
public const string SqlXdataExtattName = "SS_X__EXTATT";
public const string SqlXdataMrefName = "SS_X__MREF";
public const string SqlXdataSeqName = "SS_X__SEQ";
public const string SqlXdataDescrName = "SS_X__DESCR";
public const string SqlXdataFmtName = "SS_X__FMT";
public const string SqlXdataXdurlName = "SS_X__XDURL";
public const string SqlXdataXdatumName = "SS_X__XDATUM";
public const string SqlXdataTableNameExt = "SS_XDATA";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 40; //Internal Table Code
public const string TBAREA = "Dati clinici"; //Table Area
public const string PREFIX = "Ss"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Ss1Icode; } 
public override string labelText() { return $@"{Ss1Icode} - {SsNote}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(Ss1Icode)}</strong> {HttpUtility.HtmlEncode(SsNote)}"; }

//1026-1025//[Y] REL_PRESTAZIONE_DATO_CLINICO.PD_ID_DATO_CLINICO
[Display(Name = "RelPrestazioneDatoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.RelPrestazioneDatoClinico>? XrefPdIdDatoClinico { get; set; } = null;
[Key]
[Display(Name = "Ss1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("SS__ICODE", SqlFieldNameExt="SS__ICODE", SqlFieldOptions="[SID] [LABEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ss1Icode { get; set; }
[Display(Name = "Ss1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("SS__DELETED", SqlFieldNameExt="SS__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ss1Deleted { get; set; }
[Display(Name = "Ss1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("SS__TIMESTAMP", SqlFieldNameExt="SS__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Ss1Timestamp { get; set; }
[Display(Name = "Ss1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("SS__CDATE", SqlFieldNameExt="SS__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ss1Cdate { get; set; }
[Display(Name = "Ss1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("SS__CTIME", SqlFieldNameExt="SS__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ss1Ctime { get; set; }
[Display(Name = "Ss1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("SS__CAGENT", SqlFieldNameExt="SS__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ss1Cagent { get; set; }
[Display(Name = "Ss1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("SS__CUNIT", SqlFieldNameExt="SS__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ss1Cunit { get; set; }
[Display(Name = "Ss1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("SS__MDATE", SqlFieldNameExt="SS__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Ss1Mdate { get; set; }
[Display(Name = "Ss1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("SS__MTIME", SqlFieldNameExt="SS__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Ss1Mtime { get; set; }
[Display(Name = "Ss1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("SS__MAGENT", SqlFieldNameExt="SS__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ss1Magent { get; set; }
[Display(Name = "Ss1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("SS__MUNIT", SqlFieldNameExt="SS__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ss1Munit { get; set; }
[Display(Name = "Ss1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("SS__HOME", SqlFieldNameExt="SS__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ss1Home { get; set; }
[Display(Name = "Ss1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("SS__VERSION", SqlFieldNameExt="SS__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Ss1Version { get; set; }
[Display(Name = "Ss1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("SS__INACTIVE", SqlFieldNameExt="SS__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Ss1Inactive { get; set; }
[Display(Name = "Ss1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("SS__EXTATT", SqlFieldNameExt="SS__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Ss1Extatt { get; set; }


[Display(Name = "Classe", ShortName="", Description = "Classe del dato sanitario: 3: condizioni di salute generiche", Prompt="")]
[ErpDogField("SS_CLASSE", SqlFieldNameExt="SS_CLASSE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("3")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? SsClasse  { get; set; }

[Display(Name = "Id Paziente", ShortName="", Description = "Codice del paziente a cui si riferisce il dato sanitario", Prompt="")]
[ErpDogField("SS_ID_PAZIENTE", SqlFieldNameExt="SS_ID_PAZIENTE", SqlFieldOptions="[MANDATORY]", Xref="Pa1Icode", SqlFieldProperties="prop() xref(PAZIENTE.PA__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteServer("Paziente", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? SsIdPaziente  { get; set; }
public HealthDemo.Models.SIO.Patient.Paziente? SsIdPazienteObj  { get; set; }

[Display(Name = "Id Episodio", ShortName="", Description = "Codice del contatto a cui il Dato Sanitario si riferisce", Prompt="")]
[ErpDogField("SS_ID_EPISODIO", SqlFieldNameExt="SS_ID_EPISODIO", SqlFieldOptions="", Xref="Ep1Icode", SqlFieldProperties="prop() xref(EPISODIO.EP__ICODE) xdup() multbxref()")]
[AutocompleteServer("Episodio", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? SsIdEpisodio  { get; set; }
public HealthDemo.Models.SIO.Patient.Episodio? SsIdEpisodioObj  { get; set; }

[Display(Name = "Id Tipo Dato Clinico", ShortName="", Description = "Codice del tipo di Dato Sanitario", Prompt="")]
[ErpDogField("SS_ID_TIPO_DATO_CLINICO", SqlFieldNameExt="SS_ID_TIPO_DATO_CLINICO", SqlFieldOptions="[MANDATORY]", Xref="Tc1Icode", SqlFieldProperties="prop() xref(TIPO_DATO_CLINICO.TC__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? SsIdTipoDatoClinico  { get; set; }
public HealthDemo.Models.SIO.HealthData.TipoDatoClinico? SsIdTipoDatoClinicoObj  { get; set; }

[Display(Name = "Id Gruppo Dato Clinico", ShortName="", Description = "Classe del tipo di dato sanitario", Prompt="")]
[ErpDogField("SS_ID_GRUPPO_DATO_CLINICO", SqlFieldNameExt="SS_ID_GRUPPO_DATO_CLINICO", SqlFieldOptions="", Xref="Cc1Icode", SqlFieldProperties="prop() xref(CATEGORIA_DATO_CLINICO.CC__ICODE) xdup(TIPO_DATO_CLINICO.TC_ID_CATEGORIA_DATO_CLINICO[STATO_SALUTE.SS_ID_TIPO_DATO_CLINICO]) multbxref()")]
[AutocompleteClient("CategoriaDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? SsIdGruppoDatoClinico  { get; set; }
public HealthDemo.Models.SIO.HealthData.CategoriaDatoClinico? SsIdGruppoDatoClinicoObj  { get; set; }

[Display(Name = "Valore Minimo", ShortName="", Description = "Valori numerici minimi (se applicabile)", Prompt="")]
[ErpDogField("SS_VALORE_MINIMO", SqlFieldNameExt="SS_VALORE_MINIMO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? SsValoreMinimo  { get; set; }

[Display(Name = "Valore Massimo", ShortName="", Description = "Valori numerici massimi (se applicabile)", Prompt="")]
[ErpDogField("SS_VALORE_MASSIMO", SqlFieldNameExt="SS_VALORE_MASSIMO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? SsValoreMassimo  { get; set; }

[Display(Name = "Valore Scelta", ShortName="", Description = "Valore carattere [se applicabile, in base al tipo di risultato]", Prompt="")]
[ErpDogField("SS_VALORE_SCELTA", SqlFieldNameExt="SS_VALORE_SCELTA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? SsValoreScelta  { get; set; }

[Display(Name = "Valore Testo", ShortName="", Description = "Valore testuale, se applicabile", Prompt="")]
[ErpDogField("SS_VALORE_TESTO", SqlFieldNameExt="SS_VALORE_TESTO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(4000, ErrorMessage = "Inserire massimo 4000 caratteri")]
[DataType(DataType.Text)]
public string? SsValoreTesto  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note [se applicabile, in base al tipo di risultato]", Prompt="")]
[ErpDogField("SS_NOTE", SqlFieldNameExt="SS_NOTE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? SsNote  { get; set; }

[Display(Name = "Codice Referto", ShortName="", Description = "Criterio di codifica/unità di misura adottato (se applicabile)", Prompt="")]
[ErpDogField("SS_CODICE_REFERTO", SqlFieldNameExt="SS_CODICE_REFERTO", SqlFieldOptions="[XID]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? SsCodiceReferto  { get; set; }

[Display(Name = "Data Acquisizione", ShortName="", Description = "Data di acquisizione del dato sanitario", Prompt="")]
[ErpDogField("SS_DATA_ACQUISIZIONE", SqlFieldNameExt="SS_DATA_ACQUISIZIONE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? SsDataAcquisizione  { get; set; }

[Display(Name = "Ora Acquisizione", ShortName="", Description = "Ora di acquisizione del dato sanitario", Prompt="")]
[ErpDogField("SS_ORA_ACQUISIZIONE", SqlFieldNameExt="SS_ORA_ACQUISIZIONE", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? SsOraAcquisizione  { get; set; }

[Display(Name = "Stato Dato Clinico", ShortName="", Description = "Stato del dato: P[reliminare] - C[onfermato] - A[nnullato]", Prompt="")]
[ErpDogField("SS_STATO_DATO_CLINICO", SqlFieldNameExt="SS_STATO_DATO_CLINICO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("P")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "P", "C", "A" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? SsStatoDatoClinico  { get; set; }

[Display(Name = "Data Validazione", ShortName="", Description = "Data di convalida del dato sanitario", Prompt="")]
[ErpDogField("SS_DATA_VALIDAZIONE", SqlFieldNameExt="SS_DATA_VALIDAZIONE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? SsDataValidazione  { get; set; }

[Display(Name = "Ora Validazione", ShortName="", Description = "Ora di convalida del dato sanitario", Prompt="")]
[ErpDogField("SS_ORA_VALIDAZIONE", SqlFieldNameExt="SS_ORA_VALIDAZIONE", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? SsOraValidazione  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Numero di sequenza del dato nel report originale", Prompt="")]
[ErpDogField("SS_SEQUENZA", SqlFieldNameExt="SS_SEQUENZA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? SsSequenza  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioSs1Icode|K|SS__ICODE","sioSs1RecDate|N|SS__MDATE,SS__CDATE"
        ,"sioSsIdEpisodiossIdTipoDatoClinicossDataAcquisizione|N|SS_ID_EPISODIO,SS_ID_TIPO_DATO_CLINICO,SS_DATA_ACQUISIZIONE"
        ,"sioSsIdPazientessDataAcquisizione|N|SS_ID_PAZIENTE,SS_DATA_ACQUISIZIONE"
        ,"sioSsIdTipoDatoClinicossStatoDatoClinicossDataAcquisizione|N|SS_ID_TIPO_DATO_CLINICO,SS_STATO_DATO_CLINICO,SS_DATA_ACQUISIZIONE"
        ,"sioSsCodiceReferto|N|SS_CODICE_REFERTO"
    };
}
}
}
