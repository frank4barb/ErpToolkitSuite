using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class SelAttivita : ModelErp {
public const string Description = "Tipi di attività che possono essere richieste e/o eseguite";
public const string SqlTableName = "ATTIVITA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Av1Icode";
public const string SqlRowIdName = "AV__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "AV_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "AV_XDATA";
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 11; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Av"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//3-2//[N] PRESTAZIONE.PR_ID_ATTIVITA_RICHIESTA
//4-2//[N] PRESTAZIONE.PR_ID_ATTIVITA_ESEGUITA
//91-83//[N] ATTIVITA.AV_ID_GRUPPO
//370-370//[Y] REL_ATTIVITA_TIPO_CAMPIONE.AC_ID_ATTIVITA
//1131-1131//[Y] REL_ATTIVITA_RICHIESTA_DA.AR_ID_ATTIVITA
//1179-1179//[Y] REL_ATTIVITA_USA.AU_ID_ATTIVITA
//1992-1992//[Y] REL_ATTIVITA_EROGATA_DA.AE_ID_ATTIVITA
//2203-2203//[Y] REL_ATTIVITA_CONTIENE.AA_ID_ATTIVITA_PADRE
//2204-2203//[Y] REL_ATTIVITA_CONTIENE.AA_ID_ATTIVITA_FIGLIO

[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("AV_CODICE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup(ATTIVITA.AV__ICODE[AV__ICODE] {AV_CODICE=' '}) multbxref()")]
[DataType(DataType.Text)]
public string? SelAvCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("AV_DESCRIZIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAvDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("AV_NOTE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAvNote  { get; set; }

[Display(Name = "Filtro Regime Erogazione", ShortName="", Description = "Maschera con le classi di contatti per cui l'attività può essere eseguita", Prompt="")]
[ErpDogField("AV_FILTRO_REGIME_EROGAZIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAvFiltroRegimeErogazione  { get; set; }

[Display(Name = "Costo Medio", ShortName="", Description = "Costo totale (medio) per l'esecuzione", Prompt="")]
[ErpDogField("AV_COSTO_MEDIO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
public double? SelAvCostoMedio  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Codice dell'attività di cui questa è una sotto-attività", Prompt="")]
[ErpDogField("AV_ID_GRUPPO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="Av1Icode", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Attivita", "AutocompleteGetAll", 10, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelAvIdGruppo  { get; set; } = new List<string>();

[Display(Name = "Attivita Preferenziale", ShortName="", Description = "Attività preferenziale eseguita quando il servizio viene richiesto Sì [Y] / No [N]", Prompt="")]
[ErpDogField("AV_ATTIVITA_PREFERENZIALE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("N")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelAvAttivitaPreferenziale  { get; set; } = new List<string>();

[Display(Name = "Durata Validita", ShortName="", Description = "Livello clinico di validità (cioè il numero di ore durante le quali non ha senso clinico replicare l'attività)", Prompt="")]
[ErpDogField("AV_DURATA_VALIDITA", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
public short? SelAvDurataValidita  { get; set; }

[Display(Name = "Durata Media", ShortName="", Description = "Tempo medio del ciclo completo dell'attività [ore]", Prompt="")]
[ErpDogField("AV_DURATA_MEDIA", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
public short? SelAvDurataMedia  { get; set; }

[Display(Name = "In Evidenza", ShortName="", Description = "Evidenziare gli atti effettivi per scopi di ricerca o speciali Sì [Y] - No [N]", Prompt="")]
[ErpDogField("AV_IN_EVIDENZA", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("N")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelAvInEvidenza  { get; set; } = new List<string>();

[Display(Name = "Id Tipo Attivita", ShortName="", Description = "Codice della classe generale di attività predefinita", Prompt="")]
[ErpDogField("AV_ID_TIPO_ATTIVITA", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ta1Icode", SqlFieldProperties="prop() xref(TIPO_ATTIVITA.TA__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("TipoAttivita", "AutocompleteGetAll", 10, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelAvIdTipoAttivita  { get; set; } = new List<string>();

[Display(Name = "Routine", ShortName="", Description = "Pianificazione routinaria (cioè automatica) Sì [Y] - No [N]", Prompt="")]
[ErpDogField("AV_ROUTINE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("Y")]
[MultipleChoices(new[] { "Y", "N" }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelAvRoutine  { get; set; } = new List<string>();

[Display(Name = "Note Estese", ShortName="", Description = "Nota estesa", Prompt="")]
[ErpDogField("AV_NOTE_ESTESE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAvNoteEstese  { get; set; }

[Display(Name = "Attributi1", ShortName="", Description = "Flag per scopi operativi, gestiti autonomamente dalle applicazioni", Prompt="")]
[ErpDogField("AV_ATTRIBUTI1", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAvAttributi1  { get; set; }

[Display(Name = "Attributi2", ShortName="", Description = "Ulteriore insieme di flag operativi, gestiti dalle applicazioni", Prompt="")]
[ErpDogField("AV_ATTRIBUTI2", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAvAttributi2  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        // verifica se almeno un campo indicizzato è valorizzato (test per validazioni complesse del modello) 
        bool found = false; 
        foreach (var prop in UtilHelper.getPropertiesWithXref(this.GetType())) { 
            if (DogManager.getPropertyValue(this, prop.Trim()) != null) found = true; 
            if (DogManager.getPropertyValue(this, prop.Trim() + "[0]") != null) found = true; 
        } 
        foreach (var idx in ListIndexes()) { 
            string fldLst = idx.Split("|")[2]; 
            foreach (var fld in fldLst.Split(",")) { 
                if (DogManager.getPropertyValue(this, UtilHelper.sqlFieldName2PropertyName(this.GetType(), fld.Trim())) != null) found = true; 
                if (DogManager.getPropertyValue(this, UtilHelper.sqlFieldName2PropertyName(this.GetType(), fld.Trim()) + "[0]") != null) found = true; 
                if (DogManager.getPropertyValue(this, UtilHelper.sqlFieldName2PropertyName(this.GetType(), fld.Trim()) + ".StartDate") != null) found = true; 
                if (DogManager.getPropertyValue(this, UtilHelper.sqlFieldName2PropertyName(this.GetType(), fld.Trim()) + ".EndDate") != null) found = true; 
            } 
        } 
        if (!found) { isValidate = false;  modelState.AddModelError(prefix ?? string.Empty, "Deve essere compilato almeno un campo indicizzato."); } 
        //-- 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioAv1Icode|K|AV__ICODE","sioAv1RecDate|N|AV__MDATE,AV__CDATE"
        ,"sioAvIdGruppo|N|AV_ID_GRUPPO"
        ,"sioAvIdTipoAttivita|N|AV_ID_TIPO_ATTIVITA"
        ,"sioAv1Versionav1Deleted|U|AV__VERSION,AV__DELETED"
        ,"sioAvCodiceav1Versionav1Deleted|U|AV_CODICE,AV__VERSION,AV__DELETED"
    };
}
}
}
