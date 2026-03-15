using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Resource {
public class SelTipoRisorsa : ModelErp {
public const string Description = "Tipi di risorse disponibili/utilizzate nell'organizzazione sanitaria";
public const string SqlTableName = "TIPO_RISORSA";
public const string SqlTableNameExt = "TIPO_RISORSA";
public const string SqlTableProperties = "";
public const string RowIdName = "Ts1Icode";
public const string SqlRowIdName = "TS__ICODE";
public const string SqlRowIdNameExt = "TS__ICODE";
public const string SqlPrefix = "TS_";
public const string SqlPrefixExt = "TS_";
public const string SqlXdataTableName = "TS_XDATA";
public const string SqlXdataTableNameExt = "TS_XDATA";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 20; //Internal Table Code
public const string TBAREA = "Risorse"; //Table Area
public const string PREFIX = "Ts"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//126-124//[Y] REL_PRESTAZIONE_USA.PU_ID_TIPO_RISORSA
//780-775//[N] FARMACO.FM_ID_TIPO_RISORSA
//826-822//[N] ATTREZZATURA.AT_ID_TIPO_RISORSA
//1063-1061//[N] SALA.SA_ID_TIPO_RISORSA
//1096-1092//[N] MATERIALE.MT_ID_TIPO_RISORSA
//1181-1179//[Y] REL_ATTIVITA_USA.AU_ID_TIPO_RISORSA
//2040-2036//[N] PERSONALE.PE_ID_TIPO_RISORSA
//2136-2134//[N] TIPO_RISORSA.TS_ID_GRUPPO

[Display(Name = "Codice", ShortName="", Description = "Codice assegnato dall'utente", Prompt="")]
[ErpDogField("TS_CODICE", SqlFieldNameExt="TS_CODICE", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RISORSA.TS__ICODE[TS__ICODE] {TS_CODICE=' '}) multbxref()")]
[DataType(DataType.Text)]
public string? SelTsCodice  { get; set; }

[Display(Name = "Classe Risorsa", ShortName="", Description = "Classe: E[quipments] - L[ocations] - S[taff] - M[aterial] - [G]eneric", Prompt="")]
[ErpDogField("TS_CLASSE_RISORSA", SqlFieldNameExt="TS_CLASSE_RISORSA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("M")]
[MultipleChoices(new[] { "E", "L", "M", "D", "S", "G" }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelTsClasseRisorsa  { get; set; } = new List<string>();

[Display(Name = "Id Gruppo", ShortName="", Description = "Codice del super-tipo di risorsa (cioè l'aggregazione nella gerarchia), se presente", Prompt="")]
[ErpDogField("TS_ID_GRUPPO", SqlFieldNameExt="TS_ID_GRUPPO", SqlFieldOptions="", Xref="Ts1Icode", SqlFieldProperties="prop() xref(TIPO_RISORSA.TS__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("TipoRisorsa", "AutocompleteGetAll", 10)]
[DataType(DataType.Text)]
public List<string> SelTsIdGruppo  { get; set; } = new List<string>();

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione estesa", Prompt="")]
[ErpDogField("TS_DESCRIZIONE", SqlFieldNameExt="TS_DESCRIZIONE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelTsDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("TS_NOTE", SqlFieldNameExt="TS_NOTE", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelTsNote  { get; set; }

[Display(Name = "Unita Di Misura", ShortName="", Description = "Unità di misura", Prompt="")]
[ErpDogField("TS_UNITA_DI_MISURA", SqlFieldNameExt="TS_UNITA_DI_MISURA", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelTsUnitaDiMisura  { get; set; }

public override bool TryValidateInt(ModelStateDictionary modelState, string? prefix = null) 
    { 
        bool isValidate = true; 
        // verifica se almeno un campo indicizzato è valorizzato (test per validazioni complesse del modello) 
        bool found = false; 
        foreach (var idx in ListIndexes()) { 
            string fldLst = idx.Split("|")[2]; 
            foreach (var fld in fldLst.Split(",")) { 
                if (DogManager.getPropertyValue(this, "Sel" + UtilHelper.field2Property(fld.Trim())) != null) found = true; 
                if (DogManager.getPropertyValue(this, "Sel" + UtilHelper.field2Property(fld.Trim()) + "[0]") != null) found = true; 
                if (DogManager.getPropertyValue(this, "Sel" + UtilHelper.field2Property(fld.Trim()) + ".StartDate") != null) found = true; 
                if (DogManager.getPropertyValue(this, "Sel" + UtilHelper.field2Property(fld.Trim()) + ".EndDate") != null) found = true; 
            } 
        } 
        if (!found) { isValidate = false;  modelState.AddModelError(prefix ?? string.Empty, "Deve essere compilato almeno un campo indicizzato."); } 
        //-- 
        return isValidate; 
    } 

public static List<string> ListIndexes() { 
    return new List<string>() { "sioTs1Icode|K|TS__ICODE","sioTs1RecDate|N|TS__MDATE,TS__CDATE"
        ,"sioTsClasseRisorsats1Versionts1Deleted|U|TS_CLASSE_RISORSA,TS__VERSION,TS__DELETED"
        ,"sioTsIdGruppo|N|TS_ID_GRUPPO"
        ,"sioTsCodicets1Versionts1Deleted|U|TS_CODICE,TS__VERSION,TS__DELETED"
    };
}
}
}
