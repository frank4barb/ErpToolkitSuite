using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Act {
public class SelCampione : ModelErp {
public const string Description = "Campione effettivo raccolto durante le attività quotidiane";
public const string SqlTableName = "CAMPIONE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Cp1Icode";
public const string SqlRowIdName = "CP__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "CP_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "CP_XDATA";
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 101; //Internal Table Code
public const string TBAREA = "Attività"; //Table Area
public const string PREFIX = "Cp"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//119-119//[Y] REL_PRESTAZIONE_CAMPIONE.PC_ID_CAMPIONE

[Display(Name = "Id Tipo Campione", ShortName="", Description = "Codice del tipo di campione", Prompt="")]
[ErpDogField("CP_ID_TIPO_CAMPIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Tp1Icode", SqlFieldProperties="prop() xref(TIPO_CAMPIONE.TP__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("TipoCampione", "AutocompleteGetAll", 10, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelCpIdTipoCampione  { get; set; } = new List<string>();

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione del campione", Prompt="")]
[ErpDogField("CP_DESCRIZIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCpDescrizione  { get; set; }

[Display(Name = "Codice Univoco", ShortName="", Description = "Codice del campione definito dall'utente", Prompt="")]
[ErpDogField("CP_CODICE_UNIVOCO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup(CAMPIONE.CP__ICODE[CP__ICODE] {CP_CODICE_UNIVOCO=' '}) multbxref()")]
[DataType(DataType.Text)]
public string? SelCpCodiceUnivoco  { get; set; }

[Display(Name = "Data Prelievo", ShortName="", Description = "Data di raccolta", Prompt="")]
[ErpDogField("CP_DATA_PRELIEVO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DateRange]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateRange SelCpDataPrelievo  { get; set; } = new DateRange();

[Display(Name = "Ora Prelievo", ShortName="", Description = "Ora di raccolta", Prompt="")]
[ErpDogField("CP_ORA_PRELIEVO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? SelCpOraPrelievo  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note", Prompt="")]
[ErpDogField("CP_NOTE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCpNote  { get; set; }

[Display(Name = "Id Episodio", ShortName="", Description = "Codice del contatto", Prompt="")]
[ErpDogField("CP_ID_EPISODIO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ep1Icode", SqlFieldProperties="prop() xref(EPISODIO.EP__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteServer("Episodio", "AutocompleteGetSelect", "AutocompletePreLoad", 10, ExtraFilter:"{In(\"EP_ID_PAZIENTE\", \"SelCpIdPaziente\")}", ExtraFields: "SelCpIdPaziente")]
[DataType(DataType.Text)]
public List<string> SelCpIdEpisodio  { get; set; } = new List<string>();

[Display(Name = "Id Paziente", ShortName="", Description = "Codice del paziente", Prompt="")]
[ErpDogField("CP_ID_PAZIENTE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCpIdPaziente  { get; set; }

[Display(Name = "Codice Assoluto", ShortName="", Description = "Codice esterno del campione (ad esempio, generato da strumenti)", Prompt="")]
[ErpDogField("CP_CODICE_ASSOLUTO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCpCodiceAssoluto  { get; set; }

[Display(Name = "Stato Campione", ShortName="", Description = "Stato del campione durante il suo ciclo di vita", Prompt="")]
[ErpDogField("CP_STATO_CAMPIONE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("0")]
[MultipleChoices(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelCpStatoCampione  { get; set; } = new List<string>();

[Display(Name = "Id Posizione Attuale", ShortName="", Description = "Posizione del campione nell'organizzazione", Prompt="")]
[ErpDogField("CP_ID_POSIZIONE_ATTUALE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 10, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"2\")}", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelCpIdPosizioneAttuale  { get; set; } = new List<string>();

[Display(Name = "Desc Posizione Attuale", ShortName="", Description = "Posizione attuale testuale del campione", Prompt="")]
[ErpDogField("CP_DESC_POSIZIONE_ATTUALE", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCpDescPosizioneAttuale  { get; set; }

[Display(Name = "Data Cambiamento Stato", ShortName="", Description = "Data dell'ultima modifica dello stato del campione", Prompt="")]
[ErpDogField("CP_DATA_CAMBIAMENTO_STATO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCpDataCambiamentoStato  { get; set; }

[Display(Name = "Ora Cambiamento Stato", ShortName="", Description = "Ora dell'ultima modifica dello stato del campione", Prompt="")]
[ErpDogField("CP_ORA_CAMBIAMENTO_STATO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCpOraCambiamentoStato  { get; set; }

[Display(Name = "Note Cambiamento Stato", ShortName="", Description = "Note sull'ultima modifica dello stato del campione", Prompt="")]
[ErpDogField("CP_NOTE_CAMBIAMENTO_STATO", SqlFieldCustomCond="", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelCpNoteCambiamentoStato  { get; set; }

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
    return new List<string>() { "sioCp1Icode|K|CP__ICODE","sioCp1RecDate|N|CP__MDATE,CP__CDATE"
        ,"sioCpCodiceAssoluto|U|CP_CODICE_ASSOLUTO"
        ,"sioCpIdEpisodio|N|CP_ID_EPISODIO"
        ,"sioCpIdPosizioneAttuale|N|CP_ID_POSIZIONE_ATTUALE"
        ,"sioCpIdTipoCampionecpIdPaziente|N|CP_ID_TIPO_CAMPIONE,CP_ID_PAZIENTE"
        ,"sioCpCodiceUnivococp1Versioncp1Deleted|U|CP_CODICE_UNIVOCO,CP__VERSION,CP__DELETED"
    };
}
}
}
