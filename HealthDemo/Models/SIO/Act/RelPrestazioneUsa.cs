using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class RelPrestazioneUsa : ModelErp {
public const string Description = "Risorse pianificate ed effettive utilizzate per l'esecuzione di una singola prestazione individuale";
public const string SqlTableName = "REL_PRESTAZIONE_USA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Pu1Icode";
public const string SqlRowIdName = "PU__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "PU_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "PU_XDATA";
public const string SqlXdataIcodeName = "PU_X__ICODE";
public const string SqlXdataDeletedName = "PU_X__DELETED";
public const string SqlXdataTimestampName = "PU_X__TIMESTAMP";
public const string SqlXdataCdateName = "PU_X__CDATE";
public const string SqlXdataCtimeName = "PU_X__CTIME";
public const string SqlXdataCagentName = "PU_X__CAGENT";
public const string SqlXdataCunitName = "PU_X__CUNIT";
public const string SqlXdataMdateName = "PU_X__MDATE";
public const string SqlXdataMtimeName = "PU_X__MTIME";
public const string SqlXdataMagentName = "PU_X__MAGENT";
public const string SqlXdataMunitName = "PU_X__MUNIT";
public const string SqlXdataHomeName = "PU_X__HOME";
public const string SqlXdataVersionName = "PU_X__VERSION";
public const string SqlXdataInactiveName = "PU_X__INACTIVE";
public const string SqlXdataExtattName = "PU_X__EXTATT";
public const string SqlXdataMrefName = "PU_X__MREF";
public const string SqlXdataSeqName = "PU_X__SEQ";
public const string SqlXdataDescrName = "PU_X__DESCR";
public const string SqlXdataFmtName = "PU_X__FMT";
public const string SqlXdataXdurlName = "PU_X__XDURL";
public const string SqlXdataXdatumName = "PU_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 36; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Pu"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Pu1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

[Key]
[Display(Name = "Pu1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("PU__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pu1Icode { get; set; }
[Display(Name = "Pu1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("PU__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pu1Deleted { get; set; }
[Display(Name = "Pu1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("PU__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Pu1Timestamp { get; set; }
[Display(Name = "Pu1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PU__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pu1Cdate { get; set; }
[Display(Name = "Pu1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PU__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pu1Ctime { get; set; }
[Display(Name = "Pu1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PU__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pu1Cagent { get; set; }
[Display(Name = "Pu1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PU__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pu1Cunit { get; set; }
[Display(Name = "Pu1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PU__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pu1Mdate { get; set; }
[Display(Name = "Pu1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PU__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pu1Mtime { get; set; }
[Display(Name = "Pu1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PU__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pu1Magent { get; set; }
[Display(Name = "Pu1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PU__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pu1Munit { get; set; }
[Display(Name = "Pu1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("PU__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pu1Home { get; set; }
[Display(Name = "Pu1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("PU__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pu1Version { get; set; }
[Display(Name = "Pu1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("PU__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pu1Inactive { get; set; }
[Display(Name = "Pu1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("PU__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Pu1Extatt { get; set; }


[Display(Name = "Id Prestazione", ShortName="", Description = "Codice dell'atto", Prompt="")]
[ErpDogField("PU_ID_PRESTAZIONE", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Pr1Icode", SqlFieldProperties="prop() xref(PRESTAZIONE.PR__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteServer("Prestazione", "AutocompleteGetSelect", "AutocompletePreLoad", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? PuIdPrestazione  { get; set; }
public HealthDemo.Models.SIO.Act.Prestazione? PuIdPrestazioneObj  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe di risorsa: E[quipment] - L[ocation] - M[aterial] - S[staff] - D[rug]", Prompt="")]
[ErpDogField("PU_CLASSE_RISORSA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS_CLASSE_RISORSA[REL_PRESTAZIONE_USA.PU_ID_TIPO_RISORSA] {PU_CLASSE_RISORSA=' '}) multbxref()")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "E", "L", "M", "D", "S" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? PuClasseRisorsa  { get; set; }

[Display(Name = "Id Tipo Risorsa", ShortName="", Description = "Codice del tipo di risorsa", Prompt="")]
[ErpDogField("PU_ID_TIPO_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 1, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public string? PuIdTipoRisorsa  { get; set; }
public HealthDemo.Models.SIO.Resource.TipoRisorsa? PuIdTipoRisorsaObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa effettiva (se applicabile)", Prompt="")]
[ErpDogField("PU_ID_RISORSA_S", SqlFieldNameExt="", SqlFieldOptions="", Xref="Pe1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{PU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{PU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{PU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{PU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{PU_CLASSE_RISORSA='D'}) xdup() multbxref(PU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? PuIdRisorsaS  { get; set; }
public HealthDemo.Models.SIO.Resource.Personale? PuIdRisorsaSObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa effettiva (se applicabile)", Prompt="")]
[ErpDogField("PU_ID_RISORSA_M", SqlFieldNameExt="", SqlFieldOptions="", Xref="Mt1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{PU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{PU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{PU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{PU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{PU_CLASSE_RISORSA='D'}) xdup() multbxref(PU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? PuIdRisorsaM  { get; set; }
public HealthDemo.Models.SIO.Resource.Materiale? PuIdRisorsaMObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa effettiva (se applicabile)", Prompt="")]
[ErpDogField("PU_ID_RISORSA_E", SqlFieldNameExt="", SqlFieldOptions="", Xref="At1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{PU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{PU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{PU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{PU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{PU_CLASSE_RISORSA='D'}) xdup() multbxref(PU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? PuIdRisorsaE  { get; set; }
public HealthDemo.Models.SIO.Resource.Attrezzatura? PuIdRisorsaEObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa effettiva (se applicabile)", Prompt="")]
[ErpDogField("PU_ID_RISORSA_L", SqlFieldNameExt="", SqlFieldOptions="", Xref="Sa1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{PU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{PU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{PU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{PU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{PU_CLASSE_RISORSA='D'}) xdup() multbxref(PU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? PuIdRisorsaL  { get; set; }
public HealthDemo.Models.SIO.Resource.Sala? PuIdRisorsaLObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa effettiva (se applicabile)", Prompt="")]
[ErpDogField("PU_ID_RISORSA_D", SqlFieldNameExt="", SqlFieldOptions="", Xref="Fm1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{PU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{PU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{PU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{PU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{PU_CLASSE_RISORSA='D'}) xdup() multbxref(PU_CLASSE_RISORSA)")]
[DataType(DataType.Text)]
public string? PuIdRisorsaD  { get; set; }
public HealthDemo.Models.SIO.Resource.Farmaco? PuIdRisorsaDObj  { get; set; }

[Display(Name = "Id Risorsa", ShortName="", Description = "Codice della risorsa effettiva (se applicabile)", Prompt="")]
[ErpDogField("PU_ID_RISORSA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE{PU_CLASSE_RISORSA='S'} | MATERIALE.MT__ICODE{PU_CLASSE_RISORSA='M'} | ATTREZZATURA.AT__ICODE{PU_CLASSE_RISORSA='E'} | SALA.SA__ICODE{PU_CLASSE_RISORSA='L'} | FARMACO.FM__ICODE{PU_CLASSE_RISORSA='D'}) xdup() multbxref(PU_CLASSE_RISORSA)")]
public string? PuIdRisorsa  { get; set; }

[Display(Name = "Sequenza", ShortName="", Description = "Numero di sequenza della relazione", Prompt="")]
[ErpDogField("PU_SEQUENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public short? PuSequenza  { get; set; }

[Display(Name = "In Evidenza", ShortName="", Description = "Se impostato su \"Y\", evidenzia le risorse che potrebbero essere sostituite durante il processo di acquisizione", Prompt="")]
[ErpDogField("PU_IN_EVIDENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "Y", "N", " " }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? PuInEvidenza  { get; set; }

[Display(Name = "Data Inizio Uso", ShortName="", Description = "Data di inizio dell'utilizzo", Prompt="")]
[ErpDogField("PU_DATA_INIZIO_USO", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? PuDataInizioUso  { get; set; }

[Display(Name = "Ora Inizio Uso", ShortName="", Description = "Ora di inizio dell'utilizzo", Prompt="")]
[ErpDogField("PU_ORA_INIZIO_USO", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? PuOraInizioUso  { get; set; }

[Display(Name = "Data Fine Uso", ShortName="", Description = "Data di fine dell'utilizzo", Prompt="")]
[ErpDogField("PU_DATA_FINE_USO", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("    /  /  ")]
[DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateOnly? PuDataFineUso  { get; set; }

[Display(Name = "Ora Fine Uso", ShortName="", Description = "Ora di fine dell'utilizzo", Prompt="")]
[ErpDogField("PU_ORA_FINE_USO", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? PuOraFineUso  { get; set; }

[Display(Name = "Quantita Prevista", ShortName="", Description = "Quantità pianificata da utilizzare", Prompt="")]
[ErpDogField("PU_QUANTITA_PREVISTA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? PuQuantitaPrevista  { get; set; }

[Display(Name = "Unita Di Misura Prevista", ShortName="", Description = "Unità di misura della quantità pianificata", Prompt="")]
[ErpDogField("PU_UNITA_DI_MISURA_PREVISTA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? PuUnitaDiMisuraPrevista  { get; set; }

[Display(Name = "Quantita Usata", ShortName="", Description = "Quantità effettiva utilizzata", Prompt="")]
[ErpDogField("PU_QUANTITA_USATA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? PuQuantitaUsata  { get; set; }

[Display(Name = "Unita Di Misura Usata", ShortName="", Description = "Unità di misura della quantità utilizzata", Prompt="")]
[ErpDogField("PU_UNITA_DI_MISURA_USATA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? PuUnitaDiMisuraUsata  { get; set; }

[Display(Name = "Quantita Restituita", ShortName="", Description = "Quantità eventualmente restituita al fornitore o al magazzino", Prompt="")]
[ErpDogField("PU_QUANTITA_RESTITUITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? PuQuantitaRestituita  { get; set; }

[Display(Name = "Unita Di Misura Restituita", ShortName="", Description = "Unità di misura della quantità restituita al fornitore o al magazzino", Prompt="")]
[ErpDogField("PU_UNITA_DI_MISURA_RESTITUITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
[DataType(DataType.Text)]
public string? PuUnitaDiMisuraRestituita  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note testuali aggiuntive", Prompt="")]
[ErpDogField("PU_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(40, ErrorMessage = "Inserire massimo 40 caratteri")]
[DataType(DataType.Text)]
public string? PuNote  { get; set; }

[Display(Name = "Costo Risorsa", ShortName="", Description = "Costo effettivo di tale utilizzo", Prompt="")]
[ErpDogField("PU_COSTO_RISORSA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
public double? PuCostoRisorsa  { get; set; }

[Display(Name = "Descrizione Risorsa Usata", ShortName="", Description = "Descrizione testuale delle risorse utilizzate", Prompt="")]
[ErpDogField("PU_DESCRIZIONE_RISORSA_USATA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(120, ErrorMessage = "Inserire massimo 120 caratteri")]
[DataType(DataType.Text)]
public string? PuDescrizioneRisorsaUsata  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioPu1Icode|K|PU__ICODE","sioPu1RecDate|N|PU__MDATE,PU__CDATE"
        ,"sioPuIdTipoRisorsapuIdPrestazionepuDataInizioUso|N|PU_ID_TIPO_RISORSA,PU_ID_PRESTAZIONE,PU_DATA_INIZIO_USO"
        ,"sioPuIdPrestazionepuDataInizioUsopuIdTipoRisorsapuIdRisorsa|N|PU_ID_PRESTAZIONE,PU_DATA_INIZIO_USO,PU_ID_TIPO_RISORSA,PU_ID_RISORSA"
        ,"sioPuIdRisorsapuIdPrestazionepuDataInizioUso|N|PU_ID_RISORSA,PU_ID_PRESTAZIONE,PU_DATA_INIZIO_USO"
        ,"sioPuDataInizioUso|N|PU_DATA_INIZIO_USO"
    };
}
}
}
