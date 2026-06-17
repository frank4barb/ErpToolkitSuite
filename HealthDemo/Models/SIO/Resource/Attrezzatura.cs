using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Resource {
public class Attrezzatura : ModelErp {
public const string Description = "Risorse: attrezzature";
public const string SqlTableName = "ATTREZZATURA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "At1Icode";
public const string SqlRowIdName = "AT__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "AT_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "AT_XDATA";
public const string SqlXdataIcodeName = "AT_X__ICODE";
public const string SqlXdataDeletedName = "AT_X__DELETED";
public const string SqlXdataTimestampName = "AT_X__TIMESTAMP";
public const string SqlXdataCdateName = "AT_X__CDATE";
public const string SqlXdataCtimeName = "AT_X__CTIME";
public const string SqlXdataCagentName = "AT_X__CAGENT";
public const string SqlXdataCunitName = "AT_X__CUNIT";
public const string SqlXdataMdateName = "AT_X__MDATE";
public const string SqlXdataMtimeName = "AT_X__MTIME";
public const string SqlXdataMagentName = "AT_X__MAGENT";
public const string SqlXdataMunitName = "AT_X__MUNIT";
public const string SqlXdataHomeName = "AT_X__HOME";
public const string SqlXdataVersionName = "AT_X__VERSION";
public const string SqlXdataInactiveName = "AT_X__INACTIVE";
public const string SqlXdataExtattName = "AT_X__EXTATT";
public const string SqlXdataMrefName = "AT_X__MREF";
public const string SqlXdataSeqName = "AT_X__SEQ";
public const string SqlXdataDescrName = "AT_X__DESCR";
public const string SqlXdataFmtName = "AT_X__FMT";
public const string SqlXdataXdurlName = "AT_X__XDURL";
public const string SqlXdataXdatumName = "AT_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 96; //Internal Table Code
public const string TBAREA = "Risorse"; //Table Area
public const string PREFIX = "At"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)At1Icode; } 
public override string labelText() { return $@"{AtCodice} - {AtDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(AtCodice)}</strong> {HttpUtility.HtmlEncode(AtDescrizione)}"; }

//127-124//[Y] REL_PRESTAZIONE_USA.PU_ID_RISORSA
[Display(Name = "RelPrestazioneUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelPrestazioneUsa>? XrefPuIdRisorsa { get; set; } = null;
//1182-1179//[Y] REL_ATTIVITA_USA.AU_ID_RISORSA
[Display(Name = "RelAttivitaUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaUsa>? XrefAuIdRisorsa { get; set; } = null;
[Key]
[Display(Name = "At1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("AT__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? At1Icode { get; set; }
[Display(Name = "At1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("AT__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? At1Deleted { get; set; }
[Display(Name = "At1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("AT__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? At1Timestamp { get; set; }
[Display(Name = "At1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AT__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? At1Cdate { get; set; }
[Display(Name = "At1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AT__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? At1Ctime { get; set; }
[Display(Name = "At1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AT__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? At1Cagent { get; set; }
[Display(Name = "At1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AT__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? At1Cunit { get; set; }
[Display(Name = "At1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AT__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? At1Mdate { get; set; }
[Display(Name = "At1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AT__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? At1Mtime { get; set; }
[Display(Name = "At1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AT__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? At1Magent { get; set; }
[Display(Name = "At1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AT__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? At1Munit { get; set; }
[Display(Name = "At1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("AT__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? At1Home { get; set; }
[Display(Name = "At1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("AT__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? At1Version { get; set; }
[Display(Name = "At1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("AT__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? At1Inactive { get; set; }
[Display(Name = "At1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("AT__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? At1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("AT_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup(ATTREZZATURA.AT__ICODE[AT__ICODE] {AT_CODICE=' '}) multbxref()")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? AtCodice  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe di risorsa E[quipaggiamenti]", Prompt="")]
[ErpDogField("AT_CLASSE_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS_CLASSE_RISORSA[ATTREZZATURA.AT_ID_TIPO_RISORSA]) multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue("E")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? AtClasseRisorsa  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("AT_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? AtDescrizione  { get; set; }

[Display(Name = "Id Tipo Risorsa", ShortName="", Description = "Codice del tipo di attrezzatura", Prompt="")]
[ErpDogField("AT_ID_TIPO_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? AtIdTipoRisorsa  { get; set; }
public HealthDemo.Models.SIO.Resource.TipoRisorsa? AtIdTipoRisorsaObj  { get; set; }

[Display(Name = "Costo Unitario Uso", ShortName="", Description = "Costo unitario per l'utilizzo", Prompt="")]
[ErpDogField("AT_COSTO_UNITARIO_USO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? AtCostoUnitarioUso  { get; set; }

[Display(Name = "Misura Unitaria Uso", ShortName="", Description = "Unità di misura per l'utilizzo", Prompt="")]
[ErpDogField("AT_MISURA_UNITARIA_USO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? AtMisuraUnitariaUso  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("AT_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? AtNote  { get; set; }

[Display(Name = "Disponibilita", ShortName="", Description = "Descrizione testuale dello stato attuale di disponibilità", Prompt="")]
[ErpDogField("AT_DISPONIBILITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? AtDisponibilita  { get; set; }

[Display(Name = "Telefono Fornitore", ShortName="", Description = "Numero di telefono collegato all'attrezzatura", Prompt="")]
[ErpDogField("AT_TELEFONO_FORNITORE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(15, ErrorMessage = "Inserire massimo 15 caratteri")]
[DataType(DataType.Text)]
public string? AtTelefonoFornitore  { get; set; }

[Display(Name = "Numero Seriale", ShortName="", Description = "Numero di serie", Prompt="")]
[ErpDogField("AT_NUMERO_SERIALE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? AtNumeroSeriale  { get; set; }

[Display(Name = "Riferimenti Assistenza", ShortName="", Description = "Riferimento al fornitore per l'assistenza", Prompt="")]
[ErpDogField("AT_RIFERIMENTI_ASSISTENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? AtRiferimentiAssistenza  { get; set; }

[Display(Name = "Telefono Assistenza", ShortName="", Description = "Numero di telefono per l'assistenza", Prompt="")]
[ErpDogField("AT_TELEFONO_ASSISTENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(15, ErrorMessage = "Inserire massimo 15 caratteri")]
[DataType(DataType.Text)]
public string? AtTelefonoAssistenza  { get; set; }

[Display(Name = "Data Ultima Manutenzione", ShortName="", Description = "Data dell'ultimo intervento di manutenzione", Prompt="")]
[ErpDogField("AT_DATA_ULTIMA_MANUTENZIONE", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? AtDataUltimaManutenzione  { get; set; }

[Display(Name = "Frequenza Manutenzione", ShortName="", Description = "Frequenza della manutenzione periodica [numero di ore di funzionamento]", Prompt="")]
[ErpDogField("AT_FREQUENZA_MANUTENZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? AtFrequenzaManutenzione  { get; set; }

[Display(Name = "Uso Medio Giornaliero", ShortName="", Description = "Numero medio di ore effettive di lavoro al giorno", Prompt="")]
[ErpDogField("AT_USO_MEDIO_GIORNALIERO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? AtUsoMedioGiornaliero  { get; set; }

[Display(Name = "Data Prossima Manutenzione", ShortName="", Description = "Data della prossima manutenzione prevedibile", Prompt="")]
[ErpDogField("AT_DATA_PROSSIMA_MANUTENZIONE", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? AtDataProssimaManutenzione  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioAt1Icode|K|AT__ICODE","sioAt1RecDate|N|AT__MDATE,AT__CDATE"
        ,"sioAtIdTipoRisorsa|N|AT_ID_TIPO_RISORSA"
        ,"sioAt1Versionat1Deleted|U|AT__VERSION,AT__DELETED"
        ,"sioAtCodiceat1Versionat1Deleted|U|AT_CODICE,AT__VERSION,AT__DELETED"
    };
}
}
}
