using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.HealthData {
public class DocumentoClinico : ModelErp {
public const string Description = "Dati sanitari - Altri tipi di dati";
public const string SqlTableName = "DOCUMENTO_CLINICO";
public const string SqlTableNameExt = "DOCUMENTO_CLINICO";
public const string SqlTableProperties = "";
public const string RowIdName = "Dc1Icode";
public const string SqlRowIdName = "DC__ICODE";
public const string SqlRowIdNameExt = "DC__ICODE";
public const string SqlPrefix = "DC_";
public const string SqlPrefixExt = "DC_";
public const string SqlXdataTableName = "DC_XDATA";
public const string SqlXdataTableNameExt = "DC_XDATA";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 41; //Internal Table Code
public const string TBAREA = "Dati clinici"; //Table Area
public const string PREFIX = "Dc"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Dc1Icode; } 
public override string labelText() { return $@"{Dc1Icode} - {DcNote}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(Dc1Icode)}</strong> {HttpUtility.HtmlEncode(DcNote)}"; }

//1026-1025//[Y] REL_PRESTAZIONE_DATO_CLINICO.PD_ID_DATO_CLINICO
[Display(Name = "RelPrestazioneDatoClinico", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.HealthData.RelPrestazioneDatoClinico>? XrefPdIdDatoClinico { get; set; } = null;
[Key]
[Display(Name = "Dc1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("DC__ICODE", SqlFieldNameExt="DC__ICODE", SqlFieldOptions="[SID] [LABEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dc1Icode { get; set; }
[Display(Name = "Dc1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("DC__DELETED", SqlFieldNameExt="DC__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Dc1Deleted { get; set; }
[Display(Name = "Dc1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("DC__TIMESTAMP", SqlFieldNameExt="DC__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Dc1Timestamp { get; set; }
[Display(Name = "Dc1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("DC__CDATE", SqlFieldNameExt="DC__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Dc1Cdate { get; set; }
[Display(Name = "Dc1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("DC__CTIME", SqlFieldNameExt="DC__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Dc1Ctime { get; set; }
[Display(Name = "Dc1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("DC__CAGENT", SqlFieldNameExt="DC__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dc1Cagent { get; set; }
[Display(Name = "Dc1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("DC__CUNIT", SqlFieldNameExt="DC__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dc1Cunit { get; set; }
[Display(Name = "Dc1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("DC__MDATE", SqlFieldNameExt="DC__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Dc1Mdate { get; set; }
[Display(Name = "Dc1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("DC__MTIME", SqlFieldNameExt="DC__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Dc1Mtime { get; set; }
[Display(Name = "Dc1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("DC__MAGENT", SqlFieldNameExt="DC__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dc1Magent { get; set; }
[Display(Name = "Dc1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("DC__MUNIT", SqlFieldNameExt="DC__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dc1Munit { get; set; }
[Display(Name = "Dc1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("DC__HOME", SqlFieldNameExt="DC__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dc1Home { get; set; }
[Display(Name = "Dc1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("DC__VERSION", SqlFieldNameExt="DC__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Dc1Version { get; set; }
[Display(Name = "Dc1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("DC__INACTIVE", SqlFieldNameExt="DC__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Dc1Inactive { get; set; }
[Display(Name = "Dc1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("DC__EXTATT", SqlFieldNameExt="DC__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Dc1Extatt { get; set; }


[Display(Name = "Classe", ShortName="", Description = "Classe del dato sanitario: 4: altri tipi di dati", Prompt="")]
[ErpDogField("DC_CLASSE", SqlFieldNameExt="DC_CLASSE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("4")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? DcClasse  { get; set; }

[Display(Name = "Id Paziente", ShortName="", Description = "Codice del paziente a cui si riferisce il dato sanitario", Prompt="")]
[ErpDogField("DC_ID_PAZIENTE", SqlFieldNameExt="DC_ID_PAZIENTE", SqlFieldOptions="[MANDATORY]", Xref="Pa1Icode", SqlFieldProperties="prop() xref(PAZIENTE.PA__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteServer("Paziente", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? DcIdPaziente  { get; set; }
public HealthDemo.Models.SIO.Patient.Paziente? DcIdPazienteObj  { get; set; }

[Display(Name = "Id Episodio", ShortName="", Description = "Codice del contatto a cui si riferisce il Dato Sanitario", Prompt="")]
[ErpDogField("DC_ID_EPISODIO", SqlFieldNameExt="DC_ID_EPISODIO", SqlFieldOptions="", Xref="Ep1Icode", SqlFieldProperties="prop() xref(EPISODIO.EP__ICODE) xdup() multbxref()")]
[AutocompleteServer("Episodio", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? DcIdEpisodio  { get; set; }
public HealthDemo.Models.SIO.Patient.Episodio? DcIdEpisodioObj  { get; set; }

[Display(Name = "Id Tipo Dato Clinico", ShortName="", Description = "Codice del tipo di Dato Sanitario", Prompt="")]
[ErpDogField("DC_ID_TIPO_DATO_CLINICO", SqlFieldNameExt="DC_ID_TIPO_DATO_CLINICO", SqlFieldOptions="[MANDATORY]", Xref="Tc1Icode", SqlFieldProperties="prop() xref(TIPO_DATO_CLINICO.TC__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? DcIdTipoDatoClinico  { get; set; }
public HealthDemo.Models.SIO.HealthData.TipoDatoClinico? DcIdTipoDatoClinicoObj  { get; set; }

[Display(Name = "Id Gruppo Dato Clinico", ShortName="", Description = "Classe del tipo di Dati di Salute", Prompt="")]
[ErpDogField("DC_ID_GRUPPO_DATO_CLINICO", SqlFieldNameExt="DC_ID_GRUPPO_DATO_CLINICO", SqlFieldOptions="", Xref="Cc1Icode", SqlFieldProperties="prop() xref(CATEGORIA_DATO_CLINICO.CC__ICODE) xdup(TIPO_DATO_CLINICO.TC_ID_CATEGORIA_DATO_CLINICO[DOCUMENTO_CLINICO.DC_ID_TIPO_DATO_CLINICO]) multbxref()")]
[AutocompleteClient("CategoriaDatoClinico", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? DcIdGruppoDatoClinico  { get; set; }
public HealthDemo.Models.SIO.HealthData.CategoriaDatoClinico? DcIdGruppoDatoClinicoObj  { get; set; }

[Display(Name = "Valore Minimo", ShortName="", Description = "Valori numerici minimi (se applicabile)", Prompt="")]
[ErpDogField("DC_VALORE_MINIMO", SqlFieldNameExt="DC_VALORE_MINIMO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? DcValoreMinimo  { get; set; }

[Display(Name = "Valore Massimo", ShortName="", Description = "Valori numerici massimi (se applicabile)", Prompt="")]
[ErpDogField("DC_VALORE_MASSIMO", SqlFieldNameExt="DC_VALORE_MASSIMO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? DcValoreMassimo  { get; set; }

[Display(Name = "Valore Scelta", ShortName="", Description = "Valore carattere (se applicabile, in base al tipo di risultato)", Prompt="")]
[ErpDogField("DC_VALORE_SCELTA", SqlFieldNameExt="DC_VALORE_SCELTA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? DcValoreScelta  { get; set; }

[Display(Name = "Valore Testo", ShortName="", Description = "Valore testuale, se applicabile", Prompt="")]
[ErpDogField("DC_VALORE_TESTO", SqlFieldNameExt="DC_VALORE_TESTO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(4000, ErrorMessage = "Inserire massimo 4000 caratteri")]
[DataType(DataType.Text)]
public string? DcValoreTesto  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note (se applicabile, in base al tipo di risultato)", Prompt="")]
[ErpDogField("DC_NOTE", SqlFieldNameExt="DC_NOTE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? DcNote  { get; set; }

[Display(Name = "Codice Referto", ShortName="", Description = "Criterio di codifica/unità di misura adottato (se applicabile)", Prompt="")]
[ErpDogField("DC_CODICE_REFERTO", SqlFieldNameExt="DC_CODICE_REFERTO", SqlFieldOptions="[XID]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? DcCodiceReferto  { get; set; }

[Display(Name = "Data Acquisizione", ShortName="", Description = "Data di acquisizione del dato sanitario", Prompt="")]
[ErpDogField("DC_DATA_ACQUISIZIONE", SqlFieldNameExt="DC_DATA_ACQUISIZIONE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? DcDataAcquisizione  { get; set; }

[Display(Name = "Ora Acquisizione", ShortName="", Description = "Ora di acquisizione del dato sanitario", Prompt="")]
[ErpDogField("DC_ORA_ACQUISIZIONE", SqlFieldNameExt="DC_ORA_ACQUISIZIONE", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? DcOraAcquisizione  { get; set; }

[Display(Name = "Stato Dato Clinico", ShortName="", Description = "Stato del dato: P[reliminare] - C[onfermato] - A[nnullato]", Prompt="")]
[ErpDogField("DC_STATO_DATO_CLINICO", SqlFieldNameExt="DC_STATO_DATO_CLINICO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("P")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "P", "C", "A" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? DcStatoDatoClinico  { get; set; }

[Display(Name = "Data Validazione", ShortName="", Description = "Data di convalida del dato sanitario", Prompt="")]
[ErpDogField("DC_DATA_VALIDAZIONE", SqlFieldNameExt="DC_DATA_VALIDAZIONE", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? DcDataValidazione  { get; set; }

[Display(Name = "Ora Validazione", ShortName="", Description = "Ora di convalida del dato sanitario", Prompt="")]
[ErpDogField("DC_ORA_VALIDAZIONE", SqlFieldNameExt="DC_ORA_VALIDAZIONE", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? DcOraValidazione  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Numero di sequenza del dato nel report originale", Prompt="")]
[ErpDogField("DC_SEQUENZA", SqlFieldNameExt="DC_SEQUENZA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? DcSequenza  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioDc1Icode|K|DC__ICODE","sioDc1RecDate|N|DC__MDATE,DC__CDATE"
        ,"sioDcIdEpisodiodcIdTipoDatoClinicodcDataAcquisizione|N|DC_ID_EPISODIO,DC_ID_TIPO_DATO_CLINICO,DC_DATA_ACQUISIZIONE"
        ,"sioDcIdPazientedcDataAcquisizione|N|DC_ID_PAZIENTE,DC_DATA_ACQUISIZIONE"
        ,"sioDcIdTipoDatoClinicodcStatoDatoClinicodcDataAcquisizione|N|DC_ID_TIPO_DATO_CLINICO,DC_STATO_DATO_CLINICO,DC_DATA_ACQUISIZIONE"
        ,"sioDcCodiceReferto|N|DC_CODICE_REFERTO"
    };
}
}
}
