using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Resource {
public class SelAttrezzatura : ModelErp {
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
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 96; //Internal Table Code
public const string TBAREA = "Risorse"; //Table Area
public const string PREFIX = "At"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//127-124//[Y] REL_PRESTAZIONE_USA.PU_ID_RISORSA
//1182-1179//[Y] REL_ATTIVITA_USA.AU_ID_RISORSA

[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("AT_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup(ATTREZZATURA.AT__ICODE[AT__ICODE] {AT_CODICE=' '}) multbxref()")]
[DataType(DataType.Text)]
public string? SelAtCodice  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe di risorsa E[quipaggiamenti]", Prompt="")]
[ErpDogField("AT_CLASSE_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS_CLASSE_RISORSA[ATTREZZATURA.AT_ID_TIPO_RISORSA]) multbxref()")]
[DataType(DataType.Text)]
public string? SelAtClasseRisorsa  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("AT_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAtDescrizione  { get; set; }

[Display(Name = "Id Tipo Risorsa", ShortName="", Description = "Codice del tipo di attrezzatura", Prompt="")]
[ErpDogField("AT_ID_TIPO_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 10)]
[DataType(DataType.Text)]
public List<string> SelAtIdTipoRisorsa  { get; set; } = new List<string>();

[Display(Name = "Costo Unitario Uso", ShortName="", Description = "Costo unitario per l'utilizzo", Prompt="")]
[ErpDogField("AT_COSTO_UNITARIO_USO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
public double? SelAtCostoUnitarioUso  { get; set; }

[Display(Name = "Misura Unitaria Uso", ShortName="", Description = "Unità di misura per l'utilizzo", Prompt="")]
[ErpDogField("AT_MISURA_UNITARIA_USO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAtMisuraUnitariaUso  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("AT_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAtNote  { get; set; }

[Display(Name = "Disponibilita", ShortName="", Description = "Descrizione testuale dello stato attuale di disponibilità", Prompt="")]
[ErpDogField("AT_DISPONIBILITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAtDisponibilita  { get; set; }

[Display(Name = "Telefono Fornitore", ShortName="", Description = "Numero di telefono collegato all'attrezzatura", Prompt="")]
[ErpDogField("AT_TELEFONO_FORNITORE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAtTelefonoFornitore  { get; set; }

[Display(Name = "Numero Seriale", ShortName="", Description = "Numero di serie", Prompt="")]
[ErpDogField("AT_NUMERO_SERIALE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAtNumeroSeriale  { get; set; }

[Display(Name = "Riferimenti Assistenza", ShortName="", Description = "Riferimento al fornitore per l'assistenza", Prompt="")]
[ErpDogField("AT_RIFERIMENTI_ASSISTENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAtRiferimentiAssistenza  { get; set; }

[Display(Name = "Telefono Assistenza", ShortName="", Description = "Numero di telefono per l'assistenza", Prompt="")]
[ErpDogField("AT_TELEFONO_ASSISTENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelAtTelefonoAssistenza  { get; set; }

[Display(Name = "Data Ultima Manutenzione", ShortName="", Description = "Data dell'ultimo intervento di manutenzione", Prompt="")]
[ErpDogField("AT_DATA_ULTIMA_MANUTENZIONE", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DateRange]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateRange SelAtDataUltimaManutenzione  { get; set; } = new DateRange();

[Display(Name = "Frequenza Manutenzione", ShortName="", Description = "Frequenza della manutenzione periodica [numero di ore di funzionamento]", Prompt="")]
[ErpDogField("AT_FREQUENZA_MANUTENZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
public short? SelAtFrequenzaManutenzione  { get; set; }

[Display(Name = "Uso Medio Giornaliero", ShortName="", Description = "Numero medio di ore effettive di lavoro al giorno", Prompt="")]
[ErpDogField("AT_USO_MEDIO_GIORNALIERO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
public double? SelAtUsoMedioGiornaliero  { get; set; }

[Display(Name = "Data Prossima Manutenzione", ShortName="", Description = "Data della prossima manutenzione prevedibile", Prompt="")]
[ErpDogField("AT_DATA_PROSSIMA_MANUTENZIONE", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DateRange]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateRange SelAtDataProssimaManutenzione  { get; set; } = new DateRange();

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
    return new List<string>() { "sioAt1Icode|K|AT__ICODE","sioAt1RecDate|N|AT__MDATE,AT__CDATE"
        ,"sioAtIdTipoRisorsa|N|AT_ID_TIPO_RISORSA"
        ,"sioAt1Versionat1Deleted|U|AT__VERSION,AT__DELETED"
        ,"sioAtCodiceat1Versionat1Deleted|U|AT_CODICE,AT__VERSION,AT__DELETED"
    };
}
}
}
