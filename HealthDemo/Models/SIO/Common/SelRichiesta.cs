using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Common {
public class SelRichiesta : ModelErp {
public const string Description = "Comunicazione e/o richiesta di prestazioni";
public const string SqlTableName = "RICHIESTA";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Ri1Icode";
public const string SqlRowIdName = "RI__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "RI_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "RI_XDATA";
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 46; //Internal Table Code
public const string TBAREA = "Organizzazione ospedaliera"; //Table Area
public const string PREFIX = "Ri"; //Table Prefix
public const string LIVEDESC = "L"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//24-2//[N] PRESTAZIONE.PR_ID_RICHIESTA

[Display(Name = "Id Unita Richiedente", ShortName="", Description = "Codice dell'unità che ha originato la comunicazione", Prompt="")]
[ErpDogField("RI_ID_UNITA_RICHIEDENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 10, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"1\")}", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelRiIdUnitaRichiedente  { get; set; } = new List<string>();

[Display(Name = "Id Postazione Richiedente", ShortName="", Description = "Codice del punto di servizio che ha originato la comunicazione", Prompt="")]
[ErpDogField("RI_ID_POSTAZIONE_RICHIEDENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 10, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"2\")}", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelRiIdPostazioneRichiedente  { get; set; } = new List<string>();

[Display(Name = "Id Istituto Richiedente", ShortName="", Description = "Codice dell'organizzazione che ha originato la comunicazione", Prompt="")]
[ErpDogField("RI_ID_ISTITUTO_RICHIEDENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 10, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"0\")}", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelRiIdIstitutoRichiedente  { get; set; } = new List<string>();

[Display(Name = "Id Operatore Richiedente", ShortName="", Description = "Codice (se disponibile) dell'agente che ha effettivamente inserito la comunicazione", Prompt="")]
[ErpDogField("RI_ID_OPERATORE_RICHIEDENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 10, ExtraFilter:"{EqVal(\"OR_CLASSE_ASSISTENZA\", \"3\")}", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelRiIdOperatoreRichiedente  { get; set; } = new List<string>();

[Display(Name = "Data Richiesta", ShortName="", Description = "Data non prima della quale la comunicazione deve essere trasmessa / Data di completamento quando eseguita", Prompt="")]
[ErpDogField("RI_DATA_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="[DATE]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DateRange]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateRange SelRiDataRichiesta  { get; set; } = new DateRange();

[Display(Name = "Ora Richiesta", ShortName="", Description = "Ora non prima della quale la comunicazione deve essere trasmessa / Ora di completamento quando eseguita", Prompt="")]
[ErpDogField("RI_ORA_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="[TIME]", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue(" ")]
[DataType(DataType.Time)]
[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
public TimeOnly? SelRiOraRichiesta  { get; set; }

[Display(Name = "Urgenza", ShortName="", Description = "Livello di urgenza da 1 a 5 [1: il più alto]", Prompt="")]
[ErpDogField("RI_URGENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelRiUrgenza  { get; set; }

[Display(Name = "Oggetto", ShortName="", Description = "Oggetto della comunicazione", Prompt="")]
[ErpDogField("RI_OGGETTO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelRiOggetto  { get; set; }

[Display(Name = "Stato Richiesta", ShortName="", Description = "Stato della comunicazione: In attesa / Sospesa / Completata (o annullata) / X: trasmessa solo a alcuni indirizzi", Prompt="")]
[ErpDogField("RI_STATO_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DefaultValue("P")]
[MultipleChoices(new[] { "P", "C", "X", "H", "A" }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelRiStatoRichiesta  { get; set; } = new List<string>();

[Display(Name = "Classe Richiesta", ShortName="", Description = "Classe della comunicazione: Da 0 a 9 riservata al sistema A a Z riservata agli utenti", Prompt="")]
[ErpDogField("RI_CLASSE_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup(TIPO_RICHIESTA.TI_GRUPPO[RICHIESTA.RI_ID_TIPO_RICHIESTA]) multbxref()")]
[DefaultValue(" ")]
[MultipleChoices(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Z" }, LabelChoices = null, MaxSelections=-1, LabelClassName="")]
[DataType(DataType.Text)]
public List<string> SelRiClasseRichiesta  { get; set; } = new List<string>();

[Display(Name = "Id Tipo Richiesta", ShortName="", Description = "Codice del tipo specifico di comunicazione", Prompt="")]
[ErpDogField("RI_ID_TIPO_RICHIESTA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Ti1Icode", SqlFieldProperties="prop() xref(TIPO_RICHIESTA.TI__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("TipoRichiesta", "AutocompleteGetAll", 10, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelRiIdTipoRichiesta  { get; set; } = new List<string>();

[Display(Name = "Id Paziente", ShortName="", Description = "Codice del paziente principale a cui si riferisce la comunicazione (se presente)", Prompt="")]
[ErpDogField("RI_ID_PAZIENTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Pa1Icode", SqlFieldProperties="prop() xref(PAZIENTE.PA__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteServer("Paziente", "AutocompleteGetSelect", "AutocompletePreLoad", 10, ExtraFilter:"", ExtraFields: "")]
[DataType(DataType.Text)]
public List<string> SelRiIdPaziente  { get; set; } = new List<string>();

[Display(Name = "Id Episodio", ShortName="", Description = "Codice del contatto del paziente principale a cui si riferisce la comunicazione (se presente)", Prompt="")]
[ErpDogField("RI_ID_EPISODIO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Ep1Icode", SqlFieldProperties="prop() xref(EPISODIO.EP__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteServer("Episodio", "AutocompleteGetSelect", "AutocompletePreLoad", 10, ExtraFilter:"{In(\"EP_ID_PAZIENTE\", \"SelRiIdPaziente\")}", ExtraFields: "SelRiIdPaziente")]
[DataType(DataType.Text)]
public List<string> SelRiIdEpisodio  { get; set; } = new List<string>();

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
    return new List<string>() { "sioRi1Icode|K|RI__ICODE","sioRi1RecDate|N|RI__MDATE,RI__CDATE"
        ,"sioRiDataRichiesta|N|RI_DATA_RICHIESTA"
        ,"sioRiIdOperatoreRichiedente|N|RI_ID_OPERATORE_RICHIEDENTE"
        ,"sioRiIdTipoRichiestariStatoRichiesta|N|RI_ID_TIPO_RICHIESTA,RI_STATO_RICHIESTA"
        ,"sioRiIdEpisodio|N|RI_ID_EPISODIO"
        ,"sioRiIdPaziente|N|RI_ID_PAZIENTE"
        ,"sioRiIdIstitutoRichiedente|N|RI_ID_ISTITUTO_RICHIEDENTE"
        ,"sioRiIdPostazioneRichiedente|N|RI_ID_POSTAZIONE_RICHIEDENTE"
        ,"sioRiIdUnitaRichiedente|N|RI_ID_UNITA_RICHIEDENTE"
    };
}
}
}
