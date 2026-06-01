using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.HealthData {
public class RelPrestazioneDatoClinico : ModelErp {
public const string Description = "Dettaglio delle relazioni tra prestazioni e dati sanitari (generazione, utilizzo)";
public const string SqlTableName = "REL_PRESTAZIONE_DATO_CLINICO";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Pd1Icode";
public const string SqlRowIdName = "PD__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "PD_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "PD_XDATA";
public const string SqlXdataIcodeName = "PD_X__ICODE";
public const string SqlXdataDeletedName = "PD_X__DELETED";
public const string SqlXdataTimestampName = "PD_X__TIMESTAMP";
public const string SqlXdataCdateName = "PD_X__CDATE";
public const string SqlXdataCtimeName = "PD_X__CTIME";
public const string SqlXdataCagentName = "PD_X__CAGENT";
public const string SqlXdataCunitName = "PD_X__CUNIT";
public const string SqlXdataMdateName = "PD_X__MDATE";
public const string SqlXdataMtimeName = "PD_X__MTIME";
public const string SqlXdataMagentName = "PD_X__MAGENT";
public const string SqlXdataMunitName = "PD_X__MUNIT";
public const string SqlXdataHomeName = "PD_X__HOME";
public const string SqlXdataVersionName = "PD_X__VERSION";
public const string SqlXdataInactiveName = "PD_X__INACTIVE";
public const string SqlXdataExtattName = "PD_X__EXTATT";
public const string SqlXdataMrefName = "PD_X__MREF";
public const string SqlXdataSeqName = "PD_X__SEQ";
public const string SqlXdataDescrName = "PD_X__DESCR";
public const string SqlXdataFmtName = "PD_X__FMT";
public const string SqlXdataXdurlName = "PD_X__XDURL";
public const string SqlXdataXdatumName = "PD_X__XDATUM";
public const string SqlXdataTableNameExt = "";
public const string SqlXdataIcodeTyp = "string";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "TAB"; //Data Model Name of the Class
public const int INTCODE = 80; //Internal Table Code
public const string TBAREA = "Dati clinici"; //Table Area
public const string PREFIX = "Pd"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "Y"; //Is Relation Table: Yes or No
public override object getIcode() { return (object)Pd1Icode; } 
public override string labelText() { return $@""; }
public override string labelHtml() { return $@""; }

[Key]
[Display(Name = "Pd1Icode", ShortName="", Description = "Identificatore univoco dell'istanza (definito automaticamente quando il record viene generato)", Prompt="")]
[ErpDogField("PD__ICODE", SqlFieldNameExt="", SqlFieldOptions="[SID]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pd1Icode { get; set; }
[Display(Name = "Pd1Deleted", ShortName="", Description = "Se 'Y', l'istanza è logicamente cancellata", Prompt="")]
[ErpDogField("PD__DELETED", SqlFieldNameExt="", SqlFieldOptions="[DEL]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pd1Deleted { get; set; }
[Display(Name = "Pd1Timestamp", ShortName="", Description = "Timestamp dell'ultima modifica dell'istanza", Prompt="")]
[ErpDogField("PD__TIMESTAMP", SqlFieldNameExt="", SqlFieldOptions="[TMS]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
//[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public byte[]? Pd1Timestamp { get; set; }
[Display(Name = "Pd1Cdate", ShortName="", Description = "Data di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PD__CDATE", SqlFieldNameExt="", SqlFieldOptions="[CDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pd1Cdate { get; set; }
[Display(Name = "Pd1Ctime", ShortName="", Description = "Ora di creazione iniziale dell'istanza", Prompt="")]
[ErpDogField("PD__CTIME", SqlFieldNameExt="", SqlFieldOptions="[CTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pd1Ctime { get; set; }
[Display(Name = "Pd1Cagent", ShortName="", Description = "Identificatore dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PD__CAGENT", SqlFieldNameExt="", SqlFieldOptions="[CAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pd1Cagent { get; set; }
[Display(Name = "Pd1Cunit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha creato inizialmente l'istanza", Prompt="")]
[ErpDogField("PD__CUNIT", SqlFieldNameExt="", SqlFieldOptions="[CUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pd1Cunit { get; set; }
[Display(Name = "Pd1Mdate", ShortName="", Description = "Data dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PD__MDATE", SqlFieldNameExt="", SqlFieldOptions="[MDATE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(10, ErrorMessage = "Inserire massimo 10 caratteri")]
public string? Pd1Mdate { get; set; }
[Display(Name = "Pd1Mtime", ShortName="", Description = "Ora dell'ultima modifica all'istanza da utente", Prompt="")]
[ErpDogField("PD__MTIME", SqlFieldNameExt="", SqlFieldOptions="[MTIME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(8, ErrorMessage = "Inserire massimo 8 caratteri")]
public string? Pd1Mtime { get; set; }
[Display(Name = "Pd1Magent", ShortName="", Description = "Identificatore dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PD__MAGENT", SqlFieldNameExt="", SqlFieldOptions="[MAGENT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pd1Magent { get; set; }
[Display(Name = "Pd1Munit", ShortName="", Description = "Identificatore dell'unità dell'agente che ha effettuato l'ultima modifica all'istanza", Prompt="")]
[ErpDogField("PD__MUNIT", SqlFieldNameExt="", SqlFieldOptions="[MUNIT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pd1Munit { get; set; }
[Display(Name = "Pd1Home", ShortName="", Description = "Posizione principale dell'istanza (cioè il nome del server contenente la copia master)", Prompt="")]
[ErpDogField("PD__HOME", SqlFieldNameExt="", SqlFieldOptions="[HOME]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pd1Home { get; set; }
[Display(Name = "Pd1Version", ShortName="", Description = "Versione dell'istanza", Prompt="")]
[ErpDogField("PD__VERSION", SqlFieldNameExt="", SqlFieldOptions="[VERSION]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(12, ErrorMessage = "Inserire massimo 12 caratteri")]
public string? Pd1Version { get; set; }
[Display(Name = "Pd1Inactive", ShortName="", Description = "Flag di inattività: se Y, l'istanza deve essere considerata come non attiva", Prompt="")]
[ErpDogField("PD__INACTIVE", SqlFieldNameExt="", SqlFieldOptions="[INACTIVE]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
public string? Pd1Inactive { get; set; }
[Display(Name = "Pd1Extatt", ShortName="", Description = "Attributi estesi, definibili dinamicamente come documento XML", Prompt="")]
[ErpDogField("PD__EXTATT", SqlFieldNameExt="", SqlFieldOptions="[EXTATT]", SqlFieldProperties="prop()")]
[DataType(DataType.Text)]
public string? Pd1Extatt { get; set; }


[Display(Name = "Classe Dato Clinico", ShortName="", Description = "Partizione del singolo dato sanitario", Prompt="")]
[ErpDogField("PD_CLASSE_DATO_CLINICO", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
[DefaultValue(" ")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "1", "2", "3", "4" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? PdClasseDatoClinico  { get; set; }

[Display(Name = "Id Dato Clinico", ShortName="", Description = "Identificativo del singolo dato sanitario", Prompt="")]
[ErpDogField("PD_ID_DATO_CLINICO_1", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Pv1Icode", SqlFieldProperties="prop() xref(PARAMETRO_VITALE.PV__ICODE{PD_CLASSE_DATO_CLINICO='1'} | RISULTATO_ESAME.RE__ICODE{PD_CLASSE_DATO_CLINICO='2'} | STATO_SALUTE.SS__ICODE{PD_CLASSE_DATO_CLINICO='3'} | DOCUMENTO_CLINICO.DC__ICODE{PD_CLASSE_DATO_CLINICO= '4'}) xdup() multbxref(PD_CLASSE_DATO_CLINICO)")]
[DataType(DataType.Text)]
public string? PdIdDatoClinico1  { get; set; }
public HealthDemo.Models.SIO.HealthData.ParametroVitale? PdIdDatoClinico1Obj  { get; set; }

[Display(Name = "Id Dato Clinico", ShortName="", Description = "Identificativo del singolo dato sanitario", Prompt="")]
[ErpDogField("PD_ID_DATO_CLINICO_2", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Re1Icode", SqlFieldProperties="prop() xref(PARAMETRO_VITALE.PV__ICODE{PD_CLASSE_DATO_CLINICO='1'} | RISULTATO_ESAME.RE__ICODE{PD_CLASSE_DATO_CLINICO='2'} | STATO_SALUTE.SS__ICODE{PD_CLASSE_DATO_CLINICO='3'} | DOCUMENTO_CLINICO.DC__ICODE{PD_CLASSE_DATO_CLINICO= '4'}) xdup() multbxref(PD_CLASSE_DATO_CLINICO)")]
[DataType(DataType.Text)]
public string? PdIdDatoClinico2  { get; set; }
public HealthDemo.Models.SIO.HealthData.RisultatoEsame? PdIdDatoClinico2Obj  { get; set; }

[Display(Name = "Id Dato Clinico", ShortName="", Description = "Identificativo del singolo dato sanitario", Prompt="")]
[ErpDogField("PD_ID_DATO_CLINICO_3", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ss1Icode", SqlFieldProperties="prop() xref(PARAMETRO_VITALE.PV__ICODE{PD_CLASSE_DATO_CLINICO='1'} | RISULTATO_ESAME.RE__ICODE{PD_CLASSE_DATO_CLINICO='2'} | STATO_SALUTE.SS__ICODE{PD_CLASSE_DATO_CLINICO='3'} | DOCUMENTO_CLINICO.DC__ICODE{PD_CLASSE_DATO_CLINICO= '4'}) xdup() multbxref(PD_CLASSE_DATO_CLINICO)")]
[DataType(DataType.Text)]
public string? PdIdDatoClinico3  { get; set; }
public HealthDemo.Models.SIO.HealthData.StatoSalute? PdIdDatoClinico3Obj  { get; set; }

[Display(Name = "Id Dato Clinico", ShortName="", Description = "Identificativo del singolo dato sanitario", Prompt="")]
[ErpDogField("PD_ID_DATO_CLINICO_4", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Dc1Icode", SqlFieldProperties="prop() xref(PARAMETRO_VITALE.PV__ICODE{PD_CLASSE_DATO_CLINICO='1'} | RISULTATO_ESAME.RE__ICODE{PD_CLASSE_DATO_CLINICO='2'} | STATO_SALUTE.SS__ICODE{PD_CLASSE_DATO_CLINICO='3'} | DOCUMENTO_CLINICO.DC__ICODE{PD_CLASSE_DATO_CLINICO= '4'}) xdup() multbxref(PD_CLASSE_DATO_CLINICO)")]
[DataType(DataType.Text)]
public string? PdIdDatoClinico4  { get; set; }
public HealthDemo.Models.SIO.HealthData.DocumentoClinico? PdIdDatoClinico4Obj  { get; set; }

[Display(Name = "Id Dato Clinico", ShortName="", Description = "Identificativo del singolo dato sanitario", Prompt="")]
[ErpDogField("PD_ID_DATO_CLINICO", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref(PARAMETRO_VITALE.PV__ICODE{PD_CLASSE_DATO_CLINICO='1'} | RISULTATO_ESAME.RE__ICODE{PD_CLASSE_DATO_CLINICO='2'} | STATO_SALUTE.SS__ICODE{PD_CLASSE_DATO_CLINICO='3'} | DOCUMENTO_CLINICO.DC__ICODE{PD_CLASSE_DATO_CLINICO= '4'}) xdup() multbxref(PD_CLASSE_DATO_CLINICO)")]
[Required(ErrorMessage = "Inserire un valore nel campo")]
public string? PdIdDatoClinico  { get; set; }

[Display(Name = "Id Prestazione", ShortName="", Description = "Identificativo dell'atto", Prompt="")]
[ErpDogField("PD_ID_PRESTAZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Pr1Icode", SqlFieldProperties="prop() xref(PRESTAZIONE.PR__ICODE) xdup() multbxref()")]
[AutocompleteServer("Prestazione", "AutocompleteGetSelect", "AutocompletePreLoad", 1)]
[DataType(DataType.Text)]
public string? PdIdPrestazione  { get; set; }
public HealthDemo.Models.SIO.Act.Prestazione? PdIdPrestazioneObj  { get; set; }

[Display(Name = "Tipo Relazione", ShortName="", Description = "Il Dato Sanitario è [G]enerato dall'atto - [R]ilevante per l'esecuzione", Prompt="")]
[ErpDogField("PD_TIPO_RELAZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("R")]
[StringLength(1, ErrorMessage = "Inserire massimo 1 caratteri")]
[MultipleChoices(new[] { "R", "G" }, LabelChoices = null, MaxSelections=1, LabelClassName="")]
public string? PdTipoRelazione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Ulteriori note testuali, relative al collegamento specifico", Prompt="")]
[ErpDogField("PD_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[StringLength(40, ErrorMessage = "Inserire massimo 40 caratteri")]
[DataType(DataType.Text)]
public string? PdNote  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioPd1Icode|K|PD__ICODE","sioPd1RecDate|N|PD__MDATE,PD__CDATE"
        ,"sioPdClasseDatoClinicopdIdDatoClinicopdIdPrestazionepd1Versionpd1Deleted|U|PD_CLASSE_DATO_CLINICO,PD_ID_DATO_CLINICO,PD_ID_PRESTAZIONE,PD__VERSION,PD__DELETED"
        ,"sioPdIdPrestazionepdClasseDatoClinicopdIdDatoClinico|N|PD_ID_PRESTAZIONE,PD_CLASSE_DATO_CLINICO,PD_ID_DATO_CLINICO"
        ,"sioPdIdDatoClinico|N|PD_ID_DATO_CLINICO"
    };
}
}
}
