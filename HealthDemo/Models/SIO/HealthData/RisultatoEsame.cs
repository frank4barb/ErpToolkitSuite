using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.HealthData {
public class RisultatoEsame : ModelErp {
public const string Description = "Dati sanitari - Risultato degli esami";
public const string SqlTableName = "RISULTATO_ESAME";
public const string SqlTableNameExt = "RISULTATO_ESAME";
public const string SqlTableProperties = "";
public const string RowIdName = "Re1Icode";
public const string SqlRowIdName = "RE__ICODE";
public const string SqlRowIdNameExt = "RE__ICODE";
public const string SqlPrefix = "RE_";
public const string SqlPrefixExt = "RE_";
public const string SqlXdataTableName = "RE_XDATA";
public const string SqlXdataIcodeName = "RE_X__ICODE";
public const string SqlXdataDeletedName = "RE_X__DELETED";
public const string SqlXdataTimestampName = "RE_X__TIMESTAMP";
public const string SqlXdataCdateName = "RE_X__CDATE";
public const string SqlXdataCtimeName = "RE_X__CTIME";
public const string SqlXdataCagentName = "RE_X__CAGENT";
public const string SqlXdataCunitName = "RE_X__CUNIT";
public const string SqlXdataMdateName = "RE_X__MDATE";
public const string SqlXdataMtimeName = "RE_X__MTIME";
public const string SqlXdataMagentName = "RE_X__MAGENT";
public const string SqlXdataMunitName = "RE_X__MUNIT";
public const string SqlXdataHomeName = "RE_X__HOME";
public const string SqlXdataVersionName = "RE_X__VERSION";
public const string SqlXdataInactiveName = "RE_X__INACTIVE";
public const string SqlXdataExtattName = "RE_X__EXTATT";
public const string SqlXdataMrefName = "RE_X__MREF";
public const string SqlXdataSeqName = "RE_X__SEQ";
public const string SqlXdataDescrName = "RE_X__DESCR";
public const string SqlXdataFmtName = "RE_X__FMT";
public const string SqlXdataXdurlName = "RE_X__XDURL";
public const string SqlXdataXdatumName = "RE_X__XDATUM";
public const string SqlXdataTableNameExt = "RE_XDATA";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 39; //Internal Table Code
public const string TBAREA = "Dati clinici"; //Table Area
public const string PREFIX = "Re"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Re1Icode; } 
public override string labelText() { return $@"{Re1Icode} - {ReNote}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(Re1Icode)}</strong> {HttpUtility.HtmlEncode(ReNote)}"; }

//1026-1025//[Y] REL_PRESTAZIONE_DATO_CLINICO.PD_ID_DATO_CLINICO
[Display(Name = "RelPrestazioneDatoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.RelPrestazioneDatoClinico>? XrefPdIdDatoClinico { get; set; } = null;
[Key]
[Display(Name = "Re1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("RE__ICODE", SqlFieldNameExt="RE__ICODE", SqlFieldOptions="[SID] [LABEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Re1Icode { get; set; }
[Display(Name = "Re1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("RE__DELETED", SqlFieldNameExt="RE__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Re1Deleted { get; set; }
[Display(Name = "Re1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("RE__TIMESTAMP", SqlFieldNameExt="RE__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Re1Timestamp { get; set; }
[Display(Name = "Re1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("RE__CDATE", SqlFieldNameExt="RE__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Re1Cdate { get; set; }
[Display(Name = "Re1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("RE__CTIME", SqlFieldNameExt="RE__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Re1Ctime { get; set; }
[Display(Name = "Re1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("RE__CAGENT", SqlFieldNameExt="RE__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Re1Cagent { get; set; }
[Display(Name = "Re1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("RE__CUNIT", SqlFieldNameExt="RE__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Re1Cunit { get; set; }
[Display(Name = "Re1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("RE__MDATE", SqlFieldNameExt="RE__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Re1Mdate { get; set; }
[Display(Name = "Re1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("RE__MTIME", SqlFieldNameExt="RE__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Re1Mtime { get; set; }
[Display(Name = "Re1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("RE__MAGENT", SqlFieldNameExt="RE__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Re1Magent { get; set; }
[Display(Name = "Re1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("RE__MUNIT", SqlFieldNameExt="RE__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Re1Munit { get; set; }
[Display(Name = "Re1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("RE__HOME", SqlFieldNameExt="RE__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Re1Home { get; set; }
[Display(Name = "Re1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("RE__VERSION", SqlFieldNameExt="RE__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Re1Version { get; set; }
[Display(Name = "Re1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("RE__INACTIVE", SqlFieldNameExt="RE__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Re1Inactive { get; set; }
[Display(Name = "Re1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("RE__EXTATT", SqlFieldNameExt="RE__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Re1Extatt { get; set; }


[Display(Name = "Classe", ShortName="", Description = "Classe del dato sanitario: 2: risultati degli esami", Prompt="")]
[ErpDogField("RE_CLASSE", SqlFieldNameExt="RE_CLASSE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("2")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? ReClasse  { get; set; }

[Display(Name = "Id Paziente", ShortName="", Description = "Codice del paziente a cui si riferisce il dato sanitario", Prompt="")]
[ErpDogField("RE_ID_PAZIENTE", SqlFieldNameExt="RE_ID_PAZIENTE", SqlFieldOptions="[MANDATORY]", Xref="Pa1Icode", SqlFieldProperties="prop() xref(PAZIENTE.PA__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteServer("Paziente", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? ReIdPaziente  { get; set; }
public HealthDemo.Models.SIO.Patient.Paziente? ReIdPazienteObj  { get; set; }

[Display(Name = "Id Episodio", ShortName="", Description = "Classe del tipo di dato sanitario", Prompt="")]
[ErpDogField("RE_ID_EPISODIO", SqlFieldNameExt="RE_ID_EPISODIO", SqlFieldOptions="", Xref="Cc1Icode", SqlFieldProperties="prop() xref(CATEGORIA_DATO_CLINICO.CC__ICODE) xdup(TIPO_DATO_CLINICO.TC_ID_CATEGORIA_DATO_CLINICO[RISULTATO_ESAME.RE_ID_GRUPPO_DATO_CLINICO]) multbxref()")]
[AutocompleteClient("CategoriaDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? ReIdEpisodio  { get; set; }
public HealthDemo.Models.SIO.HealthData.CategoriaDatoClinico? ReIdEpisodioObj  { get; set; }

[Display(Name = "Id Tipo Dato Clinico", ShortName="", Description = "Codice del contatto a cui si riferisce il Dato Sanitario", Prompt="")]
[ErpDogField("RE_ID_TIPO_DATO_CLINICO", SqlFieldNameExt="RE_ID_TIPO_DATO_CLINICO", SqlFieldOptions="", Xref="Ep1Icode", SqlFieldProperties="prop() xref(EPISODIO.EP__ICODE) xdup() multbxref()")]
[AutocompleteServer("Episodio", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? ReIdTipoDatoClinico  { get; set; }
public HealthDemo.Models.SIO.Patient.Episodio? ReIdTipoDatoClinicoObj  { get; set; }

[Display(Name = "Id Gruppo Dato Clinico", ShortName="", Description = "Codice del tipo di Dato Sanitario", Prompt="")]
[ErpDogField("RE_ID_GRUPPO_DATO_CLINICO", SqlFieldNameExt="RE_ID_GRUPPO_DATO_CLINICO", SqlFieldOptions="[MANDATORY]", Xref="Tc1Icode", SqlFieldProperties="prop() xref(TIPO_DATO_CLINICO.TC__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? ReIdGruppoDatoClinico  { get; set; }
public HealthDemo.Models.SIO.HealthData.TipoDatoClinico? ReIdGruppoDatoClinicoObj  { get; set; }

[Display(Name = "Valore Minimo", ShortName="", Description = "Valori numerici minimi (se applicabile)", Prompt="")]
[ErpDogField("RE_VALORE_MINIMO", SqlFieldNameExt="RE_VALORE_MINIMO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? ReValoreMinimo  { get; set; }

[Display(Name = "Valore Massimo", ShortName="", Description = "Valori numerici massimi (se applicabile)", Prompt="")]
[ErpDogField("RE_VALORE_MASSIMO", SqlFieldNameExt="RE_VALORE_MASSIMO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? ReValoreMassimo  { get; set; }

[Display(Name = "Valore Scelta", ShortName="", Description = "Valore carattere [se applicabile, in base al tipo di risultato]", Prompt="")]
[ErpDogField("RE_VALORE_SCELTA", SqlFieldNameExt="RE_VALORE_SCELTA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? ReValoreScelta  { get; set; }

[Display(Name = "Valore Testo", ShortName="", Description = "Valore testuale, se applicabile", Prompt="")]
[ErpDogField("RE_VALORE_TESTO", SqlFieldNameExt="RE_VALORE_TESTO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(4000, ErrorMessage = "Inserire massimo 4000 caratteri")]
[DataType(DataType.Text)]
public string? ReValoreTesto  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note [se applicabile, in base al tipo di risultato]", Prompt="")]
[ErpDogField("RE_NOTE", SqlFieldNameExt="RE_NOTE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? ReNote  { get; set; }

[Display(Name = "Codice Referto", ShortName="", Description = "Criterio di codifica/unità di misura adottato (se applicabile)", Prompt="")]
[ErpDogField("RE_CODICE_REFERTO", SqlFieldNameExt="RE_CODICE_REFERTO", SqlFieldOptions="[XID]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? ReCodiceReferto  { get; set; }

[Display(Name = "Data Acquisizione", ShortName="", Description = "Data di acquisizione del dato sanitario", Prompt="")]
[ErpDogField("RE_DATA_ACQUISIZIONE", SqlFieldNameExt="RE_DATA_ACQUISIZIONE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? ReDataAcquisizione  { get; set; }

[Display(Name = "Ora Acquisizione", ShortName="", Description = "Ora di acquisizione del dato sanitario", Prompt="")]
[ErpDogField("RE_ORA_ACQUISIZIONE", SqlFieldNameExt="RE_ORA_ACQUISIZIONE", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? ReOraAcquisizione  { get; set; }

[Display(Name = "Stato Dato Clinico", ShortName="", Description = "Stato del dato: P[reliminare] - C[onfermato] - A[nnullato]", Prompt="")]
[ErpDogField("RE_STATO_DATO_CLINICO", SqlFieldNameExt="RE_STATO_DATO_CLINICO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("P")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "P", "C", "A" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? ReStatoDatoClinico  { get; set; }

[Display(Name = "Data Validazione", ShortName="", Description = "Data di convalida del dato sanitario", Prompt="")]
[ErpDogField("RE_DATA_VALIDAZIONE", SqlFieldNameExt="RE_DATA_VALIDAZIONE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? ReDataValidazione  { get; set; }

[Display(Name = "Ora Validazione", ShortName="", Description = "Ora di convalida del dato sanitario", Prompt="")]
[ErpDogField("RE_ORA_VALIDAZIONE", SqlFieldNameExt="RE_ORA_VALIDAZIONE", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? ReOraValidazione  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Numero di sequenza del dato nel report originale", Prompt="")]
[ErpDogField("RE_SEQUENZA", SqlFieldNameExt="RE_SEQUENZA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? ReSequenza  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioRe1Icode|K|RE__ICODE","sioRe1RecDate|N|RE__MDATE,RE__CDATE"
        ,"sioReIdTipoDatoClinicoreIdGruppoDatoClinicoreDataAcquisizione|N|RE_ID_TIPO_DATO_CLINICO,RE_ID_GRUPPO_DATO_CLINICO,RE_DATA_ACQUISIZIONE"
        ,"sioReIdPazientereDataAcquisizione|N|RE_ID_PAZIENTE,RE_DATA_ACQUISIZIONE"
        ,"sioReIdGruppoDatoClinicoreStatoDatoClinicoreDataAcquisizione|N|RE_ID_GRUPPO_DATO_CLINICO,RE_STATO_DATO_CLINICO,RE_DATA_ACQUISIZIONE"
        ,"sioReCodiceReferto|N|RE_CODICE_REFERTO"
    };
}
}
}
