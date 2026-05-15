using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Resource {
public class Personale : ModelErp {
public const string Description = "Risorse: personale";
public const string SqlTableName = "PERSONALE";
public const string SqlTableNameExt = "PERSONALE";
public const string SqlTableProperties = "";
public const string RowIdName = "Pe1Icode";
public const string SqlRowIdName = "PE__ICODE";
public const string SqlRowIdNameExt = "PE__ICODE";
public const string SqlPrefix = "PE_";
public const string SqlPrefixExt = "PE_";
public const string SqlXdataTableName = "PE_XDATA";
public const string SqlXdataIcodeName = "PE_X__ICODE";
public const string SqlXdataDeletedName = "PE_X__DELETED";
public const string SqlXdataTimestampName = "PE_X__TIMESTAMP";
public const string SqlXdataCdateName = "PE_X__CDATE";
public const string SqlXdataCtimeName = "PE_X__CTIME";
public const string SqlXdataCagentName = "PE_X__CAGENT";
public const string SqlXdataCunitName = "PE_X__CUNIT";
public const string SqlXdataMdateName = "PE_X__MDATE";
public const string SqlXdataMtimeName = "PE_X__MTIME";
public const string SqlXdataMagentName = "PE_X__MAGENT";
public const string SqlXdataMunitName = "PE_X__MUNIT";
public const string SqlXdataHomeName = "PE_X__HOME";
public const string SqlXdataVersionName = "PE_X__VERSION";
public const string SqlXdataInactiveName = "PE_X__INACTIVE";
public const string SqlXdataExtattName = "PE_X__EXTATT";
public const string SqlXdataMrefName = "PE_X__MREF";
public const string SqlXdataSeqName = "PE_X__SEQ";
public const string SqlXdataDescrName = "PE_X__DESCR";
public const string SqlXdataFmtName = "PE_X__FMT";
public const string SqlXdataXdurlName = "PE_X__XDURL";
public const string SqlXdataXdatumName = "PE_X__XDATUM";
public const string SqlXdataTableNameExt = "PE_XDATA";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 95; //Internal Table Code
public const string TBAREA = "Risorse"; //Table Area
public const string PREFIX = "Pe"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Pe1Icode; } 
public override string labelText() { return $@"{PeCodice} - {PeDescrizione}"; }
public override string labelHtml() { return $@"<strong>{HttpUtility.HtmlEncode(PeCodice)}</strong> {HttpUtility.HtmlEncode(PeDescrizione)}"; }

//127-124//[Y] REL_PRESTAZIONE_USA.PU_ID_RISORSA
[Display(Name = "RelPrestazioneUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelPrestazioneUsa>? XrefPuIdRisorsa { get; set; } = null;
//1182-1179//[Y] REL_ATTIVITA_USA.AU_ID_RISORSA
[Display(Name = "RelAttivitaUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaUsa>? XrefAuIdRisorsa { get; set; } = null;
//1788-1769//[N] ORGANIZZAZIONE.OR_ID_PERSONALE
[Display(Name = "Organizzazione", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Common.Organizzazione>? XrefOrIdPersonale { get; set; } = null;
[Key]
[Display(Name = "Pe1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("PE__ICODE", SqlFieldNameExt="PE__ICODE", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pe1Icode { get; set; }
[Display(Name = "Pe1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("PE__DELETED", SqlFieldNameExt="PE__DELETED", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pe1Deleted { get; set; }
[Display(Name = "Pe1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("PE__TIMESTAMP", SqlFieldNameExt="PE__TIMESTAMP", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Pe1Timestamp { get; set; }
[Display(Name = "Pe1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PE__CDATE", SqlFieldNameExt="PE__CDATE", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pe1Cdate { get; set; }
[Display(Name = "Pe1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PE__CTIME", SqlFieldNameExt="PE__CTIME", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pe1Ctime { get; set; }
[Display(Name = "Pe1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PE__CAGENT", SqlFieldNameExt="PE__CAGENT", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pe1Cagent { get; set; }
[Display(Name = "Pe1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PE__CUNIT", SqlFieldNameExt="PE__CUNIT", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pe1Cunit { get; set; }
[Display(Name = "Pe1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PE__MDATE", SqlFieldNameExt="PE__MDATE", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pe1Mdate { get; set; }
[Display(Name = "Pe1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PE__MTIME", SqlFieldNameExt="PE__MTIME", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pe1Mtime { get; set; }
[Display(Name = "Pe1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PE__MAGENT", SqlFieldNameExt="PE__MAGENT", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pe1Magent { get; set; }
[Display(Name = "Pe1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PE__MUNIT", SqlFieldNameExt="PE__MUNIT", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pe1Munit { get; set; }
[Display(Name = "Pe1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("PE__HOME", SqlFieldNameExt="PE__HOME", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pe1Home { get; set; }
[Display(Name = "Pe1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("PE__VERSION", SqlFieldNameExt="PE__VERSION", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pe1Version { get; set; }
[Display(Name = "Pe1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("PE__INACTIVE", SqlFieldNameExt="PE__INACTIVE", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pe1Inactive { get; set; }
[Display(Name = "Pe1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("PE__EXTATT", SqlFieldNameExt="PE__EXTATT", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Pe1Extatt { get; set; }


[Display(Name = "Codice", ShortName="", Description = "ID del membro dello staff nell'organizzazione", Prompt="")]
[ErpDogField("PE_CODICE", SqlFieldNameExt="PE_CODICE", SqlFieldOptions="[MANDATORY] [LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue("")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? PeCodice  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe di risorse S[tato]", Prompt="")]
[ErpDogField("PE_CLASSE_RISORSA", SqlFieldNameExt="PE_CLASSE_RISORSA", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS_CLASSE_RISORSA[PERSONALE.PE_ID_TIPO_RISORSA]) multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue("S")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[DataType(DataType.Text)]
public string? PeClasseRisorsa  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("PE_DESCRIZIONE", SqlFieldNameExt="PE_DESCRIZIONE", SqlFieldOptions="[LABEL]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(50, ErrorMessage = "Inserire massimo 50 caratteri")]
[DataType(DataType.Text)]
public string? PeDescrizione  { get; set; }

[Display(Name = "Id Tipo Risorsa", ShortName="", Description = "Codice del tipo di membro dello staff (classificazione operativa)", Prompt="")]
[ErpDogField("PE_ID_TIPO_RISORSA", SqlFieldNameExt="PE_ID_TIPO_RISORSA", SqlFieldOptions="[MANDATORY]", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 1)]
[DataType(DataType.Text)]
public string? PeIdTipoRisorsa  { get; set; }
public HealthDemo.Models.SIO.Resource.TipoRisorsa? PeIdTipoRisorsaObj  { get; set; }

[Display(Name = "Costo Unitario Uso", ShortName="", Description = "Costo unitario per l'utilizzo", Prompt="")]
[ErpDogField("PE_COSTO_UNITARIO_USO", SqlFieldNameExt="PE_COSTO_UNITARIO_USO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? PeCostoUnitarioUso  { get; set; }

[Display(Name = "Misura Unitaria Uso", ShortName="", Description = "Unità di misura per l'utilizzo", Prompt="")]
[ErpDogField("PE_MISURA_UNITARIA_USO", SqlFieldNameExt="PE_MISURA_UNITARIA_USO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? PeMisuraUnitariaUso  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("PE_NOTE", SqlFieldNameExt="PE_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? PeNote  { get; set; }

[Display(Name = "Disponibilita", ShortName="", Description = "Descrizione testuale dello stato attuale di disponibilità", Prompt="")]
[ErpDogField("PE_DISPONIBILITA", SqlFieldNameExt="PE_DISPONIBILITA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(20, ErrorMessage = "Inserire massimo 20 caratteri")]
[DataType(DataType.Text)]
public string? PeDisponibilita  { get; set; }

[Display(Name = "Telefono", ShortName="", Description = "Numero di telefono dell'ufficio del membro dello staff", Prompt="")]
[ErpDogField("PE_TELEFONO", SqlFieldNameExt="PE_TELEFONO", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(15, ErrorMessage = "Inserire massimo 15 caratteri")]
[DataType(DataType.Text)]
public string? PeTelefono  { get; set; }

[Display(Name = "Cellulare", ShortName="", Description = "Numero di telefono privato", Prompt="")]
[ErpDogField("PE_CELLULARE", SqlFieldNameExt="PE_CELLULARE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(15, ErrorMessage = "Inserire massimo 15 caratteri")]
[DataType(DataType.Text)]
public string? PeCellulare  { get; set; }

[Display(Name = "Nome", ShortName="", Description = "Nome del membro dello staff", Prompt="")]
[ErpDogField("PE_NOME", SqlFieldNameExt="PE_NOME", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(30, ErrorMessage = "Inserire massimo 30 caratteri")]
[DataType(DataType.Text)]
public string? PeNome  { get; set; }

[Display(Name = "Cognome", ShortName="", Description = "Cognome del membro dello staff", Prompt="")]
[ErpDogField("PE_COGNOME", SqlFieldNameExt="PE_COGNOME", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(30, ErrorMessage = "Inserire massimo 30 caratteri")]
[DataType(DataType.Text)]
public string? PeCognome  { get; set; }

[Display(Name = "Codice Fiscale", ShortName="", Description = "Identificatore nazionale (ad esempio, codice fiscale) del membro dello staff", Prompt="")]
[ErpDogField("PE_CODICE_FISCALE", SqlFieldNameExt="PE_CODICE_FISCALE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
[StringLength(36, ErrorMessage = "Inserire massimo 36 caratteri")]
[DataType(DataType.Text)]
public string? PeCodiceFiscale  { get; set; }

[Display(Name = "Email", ShortName="", Description = "Indirizzo email del membro dello staff", Prompt="")]
[ErpDogField("PE_EMAIL", SqlFieldNameExt="PE_EMAIL", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(40, ErrorMessage = "Inserire massimo 40 caratteri")]
[DataType(DataType.Text)]
public string? PeEmail  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioPe1Icode|K|PE__ICODE","sioPe1RecDate|N|PE__MDATE,PE__CDATE"
        ,"sioPeCodiceFiscale|N|PE_CODICE_FISCALE"
        ,"sioPeCodicepe1Deleted|U|PE_CODICE,PE__DELETED"
        ,"sioPeIdTipoRisorsa|N|PE_ID_TIPO_RISORSA"
        ,"sioPe1Versionpe1Deleted|U|PE__VERSION,PE__DELETED"
    };
}
}
}
