using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Costs {
public class SelDiagnosi : ModelErp {
public const string Description = "Classificazioni diagnostiche adottate nelle organizzazioni sanitarie (ad esempio, DRG, AVG, ICD9, ecc.)";
public const string SqlTableName = "DIAGNOSI";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Dg1Icode";
public const string SqlRowIdName = "DG__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "DG_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "DG_XDATA";
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 63; //Internal Table Code
public const string TBAREA = "Controllo di gestione"; //Table Area
public const string PREFIX = "Dg"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//622-593//[N] EPISODIO.EP_ID_DIAGNOSI_AMMISSIONE
//625-593//[N] EPISODIO.EP_ID_DIAGNOSI_DIMISSIONE
//2597-2591//[N] DIAGNOSI.DG_ID_GRUPPO

[Display(Name = "Tipo Diagnosi", ShortName="", Description = "Codice del tipo di classificazione a cui l'istanza appartiene", Prompt="")]
[ErpDogField("DG_TIPO_DIAGNOSI", SqlFieldNameExt="", SqlFieldOptions="", Xref="Td1Icode", SqlFieldProperties="prop() xref(TIPO_DIAGNOSI.TD__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("TipoDiagnosi", "AutocompleteGetAll", 10, ExtraFilter:"")]
[DataType(DataType.Text)]
public List<string> SelDgTipoDiagnosi  { get; set; } = new List<string>();

[Display(Name = "Classe", ShortName="", Description = "Classificazione di aggregazione diagnostica definita dall'utente: 1: DRG 2: ICD9 3: ICD9-CM 4: APG, 5: AFO; 6: Specialità HC, ecc.", Prompt="")]
[ErpDogField("DG_CLASSE", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelDgClasse  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione", Prompt="")]
[ErpDogField("DG_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelDgDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("DG_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelDgNote  { get; set; }

[Display(Name = "Codice", ShortName="", Description = "Codice definito dall'utente per la classificazione", Prompt="")]
[ErpDogField("DG_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup(DIAGNOSI.DG__ICODE[DG__ICODE] {DG_CODICE=' '}) multbxref()")]
[DataType(DataType.Text)]
public string? SelDgCodice  { get; set; }

[Display(Name = "Id Gruppo", ShortName="", Description = "Identificatore del codice di aggregazione nella gerarchia (se presente)", Prompt="")]
[ErpDogField("DG_ID_GRUPPO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Dg1Icode", SqlFieldProperties="prop() xref(DIAGNOSI.DG__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Diagnosi", "AutocompleteGetAll", 10, ExtraFilter:"")]
[DataType(DataType.Text)]
public List<string> SelDgIdGruppo  { get; set; } = new List<string>();

[Display(Name = "Tipo Drg", ShortName="", Description = "Tipo di DRG [M]edico - [C]hirurgico", Prompt="")]
[ErpDogField("DG_TIPO_DRG", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("M")]
[MultipleChoices(new[] { "M", "S" }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelDgTipoDrg  { get; set; } = new List<string>();

[Display(Name = "Tipo Icd9", ShortName="", Description = "Tipo di ICD9-CM [D]iagnostico - [O]perativo (se applicabile)", Prompt="")]
[ErpDogField("DG_TIPO_ICD9", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[MultipleChoices(new[] { "D", "O", " " }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelDgTipoIcd9  { get; set; } = new List<string>();

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
    return new List<string>() { "sioDg1Icode|K|DG__ICODE","sioDg1RecDate|N|DG__MDATE,DG__CDATE"
        ,"sioDgTipoDiagnosidg1Versiondg1Deleted|U|DG_TIPO_DIAGNOSI,DG__VERSION,DG__DELETED"
        ,"sioDgIdGruppo|N|DG_ID_GRUPPO"
        ,"sioDgCodicedg1Versiondg1Deleted|U|DG_CODICE,DG__VERSION,DG__DELETED"
    };
}
}
}
