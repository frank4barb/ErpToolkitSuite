using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class RelAttivitaUsa : ModelErp {
public const string Description = "Tipi e/o risorse individuali generalmente necessari per l'esecuzione di un'attività";
public const string SqlTableName = "REL_ATTIVITA_USA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Au1Icode";
public const string SqlRowIdName = "AU__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "AU_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "AU_XDATA";
public const string SqlXdataIcodeName = "AU_X__ICODE";
public const string SqlXdataDeletedName = "AU_X__DELETED";
public const string SqlXdataTimestampName = "AU_X__TIMESTAMP";
public const string SqlXdataCdateName = "AU_X__CDATE";
public const string SqlXdataCtimeName = "AU_X__CTIME";
public const string SqlXdataCagentName = "AU_X__CAGENT";
public const string SqlXdataCunitName = "AU_X__CUNIT";
public const string SqlXdataMdateName = "AU_X__MDATE";
public const string SqlXdataMtimeName = "AU_X__MTIME";
public const string SqlXdataMagentName = "AU_X__MAGENT";
public const string SqlXdataMunitName = "AU_X__MUNIT";
public const string SqlXdataHomeName = "AU_X__HOME";
public const string SqlXdataVersionName = "AU_X__VERSION";
public const string SqlXdataInactiveName = "AU_X__INACTIVE";
public const string SqlXdataExtattName = "AU_X__EXTATT";
public const string SqlXdataMrefName = "AU_X__MREF";
public const string SqlXdataSeqName = "AU_X__SEQ";
public const string SqlXdataDescrName = "AU_X__DESCR";
public const string SqlXdataFmtName = "AU_X__FMT";
public const string SqlXdataXdurlName = "AU_X__XDURL";
public const string SqlXdataXdatumName = "AU_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 21; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Au"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Au1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

//1193-1179//[Y] REL_ATTIVITA_USA.AU_ID_GRUPPO
[Display(Name = "RelAttivitaUsa", ShortName = "", Description = "", Prompt = "")]
[ErpTable(Options = " XXX ")]
public Dictionary<string, HealthDemo.Models.SIO.Act.RelAttivitaUsa>? XrefAuIdGruppo { get; set; } = null;
[Key]
[Display(Name = "Au1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("AU__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Au1Icode { get; set; }
[Display(Name = "Au1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("AU__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Au1Deleted { get; set; }
[Display(Name = "Au1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("AU__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Au1Timestamp { get; set; }
[Display(Name = "Au1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AU__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Au1Cdate { get; set; }
[Display(Name = "Au1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("AU__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Au1Ctime { get; set; }
[Display(Name = "Au1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AU__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Au1Cagent { get; set; }
[Display(Name = "Au1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("AU__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Au1Cunit { get; set; }
[Display(Name = "Au1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AU__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Au1Mdate { get; set; }
[Display(Name = "Au1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("AU__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Au1Mtime { get; set; }
[Display(Name = "Au1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AU__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Au1Magent { get; set; }
[Display(Name = "Au1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("AU__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Au1Munit { get; set; }
[Display(Name = "Au1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("AU__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Au1Home { get; set; }
[Display(Name = "Au1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("AU__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Au1Version { get; set; }
[Display(Name = "Au1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("AU__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Au1Inactive { get; set; }
[Display(Name = "Au1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("AU__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Au1Extatt { get; set; }


[Display(Name = "Id Attivita", ShortName="", Description = "Codice del tipo di attività", Prompt="")]
[ErpDogField("AU_ID_ATTIVITA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Av1Icode", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("Attivita", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? AuIdAttivita  { get; set; }
public HealthDemo.Models.SIO.Act.Attivita? AuIdAttivitaObj  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe di risorsa: E[quipments] (Attrezzature) - L[ocations] (Luoghi) - S[taff] (Personale) - M[aterial] (Materiali) - B[ed] (Letti)", Prompt="")]
[ErpDogField("AU_CLASSE_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS_CLASSE_RISORSA[REL_ATTIVITA_USA.AU_ID_TIPO_RISORSA]) multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "E", "L", "M", "D", "S" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? AuClasseRisorsa  { get; set; }

[Display(Name = "Id Tipo Risorsa", ShortName="", Description = "Codice del tipo di risorsa", Prompt="")]
[ErpDogField("AU_ID_TIPO_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? AuIdTipoRisorsa  { get; set; }
public HealthDemo.Models.SIO.Resource.TipoRisorsa? AuIdTipoRisorsaObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa individuale", Prompt="")]
[ErpDogField("AU_ID_RISORSA_S", SqlFieldNameExt="", SqlFieldOptions="", Xref="Pe1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{AU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{AU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{AU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{AU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{AU_CLASSE_RISORSA='D'}) xdup() multbxref(AU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? AuIdRisorsaS  { get; set; }
public HealthDemo.Models.SIO.Resource.Personale? AuIdRisorsaSObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa individuale", Prompt="")]
[ErpDogField("AU_ID_RISORSA_M", SqlFieldNameExt="", SqlFieldOptions="", Xref="Mt1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{AU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{AU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{AU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{AU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{AU_CLASSE_RISORSA='D'}) xdup() multbxref(AU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? AuIdRisorsaM  { get; set; }
public HealthDemo.Models.SIO.Resource.Materiale? AuIdRisorsaMObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa individuale", Prompt="")]
[ErpDogField("AU_ID_RISORSA_E", SqlFieldNameExt="", SqlFieldOptions="", Xref="At1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{AU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{AU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{AU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{AU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{AU_CLASSE_RISORSA='D'}) xdup() multbxref(AU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? AuIdRisorsaE  { get; set; }
public HealthDemo.Models.SIO.Resource.Attrezzatura? AuIdRisorsaEObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa individuale", Prompt="")]
[ErpDogField("AU_ID_RISORSA_L", SqlFieldNameExt="", SqlFieldOptions="", Xref="Sa1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{AU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{AU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{AU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{AU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{AU_CLASSE_RISORSA='D'}) xdup() multbxref(AU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? AuIdRisorsaL  { get; set; }
public HealthDemo.Models.SIO.Resource.Sala? AuIdRisorsaLObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa individuale", Prompt="")]
[ErpDogField("AU_ID_RISORSA_D", SqlFieldNameExt="", SqlFieldOptions="", Xref="Fm1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{AU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{AU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{AU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{AU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{AU_CLASSE_RISORSA='D'}) xdup() multbxref(AU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? AuIdRisorsaD  { get; set; }
public HealthDemo.Models.SIO.Resource.Farmaco? AuIdRisorsaDObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa individuale", Prompt="")]
[ErpDogField("AU_ID_RISORSA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{AU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{AU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{AU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{AU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{AU_CLASSE_RISORSA='D'}) xdup() multbxref(AU_CLASSE_RISORSA)")]
public string? AuIdRisorsa  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Numero di sequenza della relazione", Prompt="")]
[ErpDogField("AU_SEQUENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? AuSequenza  { get; set; }

[Display(Name = "Quantita Media Usata", ShortName="", Description = "Quantità media utilizzata", Prompt="")]
[ErpDogField("AU_QUANTITA_MEDIA_USATA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? AuQuantitaMediaUsata  { get; set; }

[Display(Name = "Quantita Extra", ShortName="", Description = "Quantità extra da considerare", Prompt="")]
[ErpDogField("AU_QUANTITA_EXTRA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? AuQuantitaExtra  { get; set; }

[Display(Name = "Unita Di Misura", ShortName="", Description = "Unità di misura", Prompt="")]
[ErpDogField("AU_UNITA_DI_MISURA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? AuUnitaDiMisura  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note brevi", Prompt="")]
[ErpDogField("AU_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(80, ErrorMessage = "Inserire massimo 80 caratteri")]
[DataType(DataType.Text)]
public string? AuNote  { get; set; }

[Display(Name = "Costo Medio", ShortName="", Description = "Costo medio di tale utilizzo", Prompt="")]
[ErpDogField("AU_COSTO_MEDIO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? AuCostoMedio  { get; set; }

[Display(Name = "Descrizione Risorsa Usata", ShortName="", Description = "Descrizione testuale delle risorse utilizzate", Prompt="")]
[ErpDogField("AU_DESCRIZIONE_RISORSA_USATA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? AuDescrizioneRisorsaUsata  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Identificatore dell'istanza per la quale questa specifica rappresenta un'opzione (se applicabile)", Prompt="")]
[ErpDogField("AU_ID_GRUPPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Au1Icode", SqlFieldProperties="prop() xref(REL_ATTIVITA_USA.AU__ICODE) xdup() multbxref()")]
[AutocompleteClient("RelAttivitaUsa", "AutocompleteGetAll", 1, ExtraFilter:"")]
[DataType(DataType.Text)]
public string? AuIdGruppo  { get; set; }
public HealthDemo.Models.SIO.Act.RelAttivitaUsa? AuIdGruppoObj  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioAu1Icode|K|AU__ICODE","sioAu1RecDate|N|AU__MDATE,AU__CDATE"
        ,"sioAuIdTipoRisorsaauIdAttivita|N|AU_ID_TIPO_RISORSA,AU_ID_ATTIVITA"
        ,"sioAuIdAttivitaauIdTipoRisorsaauIdRisorsaau1Versionau1Deleted|U|AU_ID_ATTIVITA,AU_ID_TIPO_RISORSA,AU_ID_RISORSA,AU__VERSION,AU__DELETED"
        ,"sioAuIdRisorsaauIdAttivita|N|AU_ID_RISORSA,AU_ID_ATTIVITA"
        ,"sioAuIdTipoRisorsa|N|AU_ID_TIPO_RISORSA"
        ,"sioAuIdGruppo|N|AU_ID_GRUPPO"
    };
}
}
}
