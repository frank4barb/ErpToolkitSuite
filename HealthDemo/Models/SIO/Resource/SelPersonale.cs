using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Resource {
public class SelPersonale : ModelErp {
public const string Description = "Risorse: personale";
public const string SqlTableName = "PERSONALE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Pe1Icode";
public const string SqlRowIdName = "PE__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "PE_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "PE_XDATA";
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 95; //Internal Table Code
public const string TBAREA = "Risorse"; //Table Area
public const string PREFIX = "Pe"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//127-124//[Y] REL_PRESTAZIONE_USA.PU_ID_RISORSA
//1182-1179//[Y] REL_ATTIVITA_USA.AU_ID_RISORSA
//1788-1769//[N] ORGANIZZAZIONE.OR_ID_PERSONALE

[Display(Name = "Codice", ShortName="", Description = "ID del membro dello staff nell'organizzazione", Prompt="")]
[ErpDogField("PE_CODICE", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeCodice  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe di risorse S[tato]", Prompt="")]
[ErpDogField("PE_CLASSE_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS_CLASSE_RISORSA[PERSONALE.PE_ID_TIPO_RISORSA]) multbxref()")]
[DataType(DataType.Text)]
public string? SelPeClasseRisorsa  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("PE_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeDescrizione  { get; set; }

[Display(Name = "Id Tipo Risorsa", ShortName="", Description = "Codice del tipo di membro dello staff (classificazione operativa)", Prompt="")]
[ErpDogField("PE_ID_TIPO_RISORSA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 10, ExtraFilter:"")]
[DataType(DataType.Text)]
public List<string> SelPeIdTipoRisorsa  { get; set; } = new List<string>();

[Display(Name = "Costo Unitario Uso", ShortName="", Description = "Costo unitario per l'utilizzo", Prompt="")]
[ErpDogField("PE_COSTO_UNITARIO_USO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("")]
public double? SelPeCostoUnitarioUso  { get; set; }

[Display(Name = "Misura Unitaria Uso", ShortName="", Description = "Unità di misura per l'utilizzo", Prompt="")]
[ErpDogField("PE_MISURA_UNITARIA_USO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeMisuraUnitariaUso  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("PE_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeNote  { get; set; }

[Display(Name = "Disponibilita", ShortName="", Description = "Descrizione testuale dello stato attuale di disponibilità", Prompt="")]
[ErpDogField("PE_DISPONIBILITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeDisponibilita  { get; set; }

[Display(Name = "Telefono", ShortName="", Description = "Numero di telefono dell'ufficio del membro dello staff", Prompt="")]
[ErpDogField("PE_TELEFONO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeTelefono  { get; set; }

[Display(Name = "Cellulare", ShortName="", Description = "Numero di telefono privato", Prompt="")]
[ErpDogField("PE_CELLULARE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeCellulare  { get; set; }

[Display(Name = "Nome", ShortName="", Description = "Nome del membro dello staff", Prompt="")]
[ErpDogField("PE_NOME", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeNome  { get; set; }

[Display(Name = "Cognome", ShortName="", Description = "Cognome del membro dello staff", Prompt="")]
[ErpDogField("PE_COGNOME", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeCognome  { get; set; }

[Display(Name = "Codice Fiscale", ShortName="", Description = "Identificatore nazionale (ad esempio, codice fiscale) del membro dello staff", Prompt="")]
[ErpDogField("PE_CODICE_FISCALE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeCodiceFiscale  { get; set; }

[Display(Name = "Email", ShortName="", Description = "Indirizzo email del membro dello staff", Prompt="")]
[ErpDogField("PE_EMAIL", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelPeEmail  { get; set; }

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
    return new List<string>() { "sioPe1Icode|K|PE__ICODE","sioPe1RecDate|N|PE__MDATE,PE__CDATE"
        ,"sioPeCodiceFiscale|N|PE_CODICE_FISCALE"
        ,"sioPeCodicepe1Deleted|U|PE_CODICE,PE__DELETED"
        ,"sioPeIdTipoRisorsa|N|PE_ID_TIPO_RISORSA"
        ,"sioPe1Versionpe1Deleted|U|PE__VERSION,PE__DELETED"
    };
}
}
}
