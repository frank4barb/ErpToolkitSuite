using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.HealthData {
public class SelCategoriaDatoClinico : ModelErp {
public const string Description = "Classificazione dei tipi di dati sanitari";
public const string SqlTableName = "CATEGORIA_DATO_CLINICO";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Cc1Icode";
public const string SqlRowIdName = "CC__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "CC_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "CC_XDATA";
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 16; //Internal Table Code
public const string TBAREA = "Dati clinici"; //Table Area
public const string PREFIX = "Cc"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//478-473//[N] CATEGORIA_DATO_CLINICO.CC_ID_GRUPPO
//847-845//[N] RISULTATO_ESAME.RE_ID_GRUPPO_DATO_CLINICO
//952-946//[N] TIPO_DATO_CLINICO.TC_ID_CATEGORIA_DATO_CLINICO
//982-978//[N] STATO_SALUTE.SS_ID_GRUPPO_DATO_CLINICO
//1200-1196//[N] DOCUMENTO_CLINICO.DC_ID_GRUPPO_DATO_CLINICO
//2168-2164//[N] PARAMETRO_VITALE.PV_ID_GRUPPO_DATO_CLINICO

[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("CC_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup(CATEGORIA_DATO_CLINICO.CC__ICODE[CC__ICODE] {CC_CODICE=' '}) multbxref()")]
[DataType(DataType.Text)]
public string? SelCcCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("CC_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCcDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("CC_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCcNote  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Codice della superclasse che raggruppa la classe attuale", Prompt="")]
[ErpDogField("CC_ID_GRUPPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Cc1Icode", SqlFieldProperties="prop() xref(CATEGORIA_DATO_CLINICO.CC__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("CategoriaDatoClinico", "AutocompleteGetAll", 10)]
[DataType(DataType.Text)]
public List<string> SelCcIdGruppo  { get; set; } = new List<string>();

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
    return new List<string>() { "sioCc1Icode|K|CC__ICODE","sioCc1RecDate|N|CC__MDATE,CC__CDATE"
        ,"sioCcIdGruppo|N|CC_ID_GRUPPO"
        ,"sioCc1Versioncc1Deleted|U|CC__VERSION,CC__DELETED"
        ,"sioCcCodicecc1Versioncc1Deleted|U|CC_CODICE,CC__VERSION,CC__DELETED"
    };
}
}
}
