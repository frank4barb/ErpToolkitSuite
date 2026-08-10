using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class SelTipoCampione : ModelErp {
public const string Description = "Tipo di campione";
public const string SqlTableName = "TIPO_CAMPIONE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Tp1Icode";
public const string SqlRowIdName = "TP__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "TP_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "TP_XDATA";
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 100; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Tp"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//123-119//[Y] REL_PRESTAZIONE_CAMPIONE.PC_ID_TIPO_CAMPIONE
//371-370//[Y] REL_ATTIVITA_TIPO_CAMPIONE.AC_ID_TIPO_CAMPIONE
//1730-1730//[N] CAMPIONE.CP_ID_TIPO_CAMPIONE

[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TP_CODICE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_CAMPIONE.TP__ICODE[TP__ICODE] {TP_CODICE=' '}) multbxref()")]
[DataType(DataType.Text)]
public string? SelTpCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TP_DESCRIZIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelTpDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TP_NOTE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelTpNote  { get; set; }

[Display(Name = "Contesto", ShortName="", Description = "Identificazione del contesto o dei contesti in cui il tipo di campione ha particolare rilevanza", Prompt="")]
[ErpDogField("TP_CONTESTO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelTpContesto  { get; set; }

[Display(Name = "Contenitore", ShortName="", Description = "Descrizione del contenitore", Prompt="")]
[ErpDogField("TP_CONTENITORE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelTpContenitore  { get; set; }

[Display(Name = "Attributi", ShortName="", Description = "Flag operativi, gestiti dall'applicazione", Prompt="")]
[ErpDogField("TP_ATTRIBUTI", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelTpAttributi  { get; set; }

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
    return new List<string>() { "sioTp1Icode|K|TP__ICODE","sioTp1RecDate|N|TP__MDATE,TP__CDATE"
        ,"sioTpContesto|N|TP_CONTESTO"
        ,"sioTp1Versiontp1Deleted|U|TP__VERSION,TP__DELETED"
        ,"sioTpCodicetp1Versiontp1Deleted|U|TP_CODICE,TP__VERSION,TP__DELETED"
    };
}
}
}
