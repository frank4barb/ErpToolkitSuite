using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HealthDemo.Models.SIO.Common {
public class SelOrganizzazione : ModelErp {
public const string Description = "Struttura sanitaria: centro sanitario, unità organizzativa, individuo, componente software, ecc.";
public const string SqlTableName = "ORGANIZZAZIONE";
public const string SqlTableNameExt = "";
public const string SqlTableProperties = "";
public const string RowIdName = "Or1Icode";
public const string SqlRowIdName = "OR__ICODE";
public const string SqlRowIdNameExt = "";
public const string SqlPrefix = "OR_";
public const string SqlPrefixExt = "";
public const string SqlXdataTableName = "OR_XDATA";
public const string SqlXdataTableNameExt = "";
public const string MODEL = "SIO"; //Data Model Name of the Class
public const string CATEG = "SEL"; //Data Model Name of the Class
public const int INTCODE = 2; //Internal Table Code
public const string TBAREA = "Organizzazione ospedaliera"; //Table Area
public const string PREFIX = "Or"; //Table Prefix
public const string LIVEDESC = "D"; //Table type: Live or Description
public const string IS_RELTABLE = "N"; //Is Relation Table: Yes or No
public override object getIcode() { return null; } 
public override string labelText() { return $""; }
public override string labelHtml() { return $""; }

//19-2//[N] PRESTAZIONE.PR_ID_OPERATORE_RICHIEDENTE
//20-2//[N] PRESTAZIONE.PR_ID_UNITA_RICHIEDENTE
//21-2//[N] PRESTAZIONE.PR_ID_POSTAZIONE_RICHIEDENTE
//44-2//[N] PRESTAZIONE.PR_ID_OPERATORE_ESECUTORE
//45-2//[N] PRESTAZIONE.PR_ID_UNITA_ESECUTRICE
//46-2//[N] PRESTAZIONE.PR_ID_POSTAZIONE_ESECUTRICE
//57-2//[N] PRESTAZIONE.PR_ID_OPERATORE_PIANIFICATORE
//327-327//[Y] REL_ORGANIZZAZIONE_CONTIENE.OO_ID_ORGANIZZAZIONE_PADRE
//328-327//[Y] REL_ORGANIZZAZIONE_CONTIENE.OO_ID_ORGANIZZAZIONE_FIGLIO
//527-524//[N] RICHIESTA.RI_ID_UNITA_RICHIEDENTE
//528-524//[N] RICHIESTA.RI_ID_POSTAZIONE_RICHIEDENTE
//529-524//[N] RICHIESTA.RI_ID_ISTITUTO_RICHIEDENTE
//532-524//[N] RICHIESTA.RI_ID_OPERATORE_RICHIEDENTE
//601-593//[N] EPISODIO.EP_ID_UNITA_INGRESSO
//615-593//[N] EPISODIO.EP_ID_CORSIA
//616-593//[N] EPISODIO.EP_ID_REPARTO
//666-593//[N] EPISODIO.EP_ID_REPARTO_LA
//669-593//[N] EPISODIO.EP_ID_REPARTO_PREH
//1132-1131//[Y] REL_ATTIVITA_RICHIESTA_DA.AR_ID_ISTITUTO
//1133-1131//[Y] REL_ATTIVITA_RICHIESTA_DA.AR_ID_UNITA
//1134-1131//[Y] REL_ATTIVITA_RICHIESTA_DA.AR_ID_POSTAZIONE
//1135-1131//[Y] REL_ATTIVITA_RICHIESTA_DA.AR_ID_OPERATORE
//1740-1730//[N] CAMPIONE.CP_ID_POSIZIONE_ATTUALE
//1792-1769//[N] ORGANIZZAZIONE.OR_ID_ISTITUTO
//1793-1769//[N] ORGANIZZAZIONE.OR_ID_UNITA
//1794-1769//[N] ORGANIZZAZIONE.OR_ID_POSTAZIONE
//1993-1992//[Y] REL_ATTIVITA_EROGATA_DA.AE_ID_UNITA

[Display(Name = "Classe Assistenza", ShortName="", Description = "Classe dell'agente: 0=Centro - 1=Unità - 2=Punto di Servizio (PS) - 3=Individuo 4=Agente SW (da A a Z, definito dall'utente)", Prompt="")]
[ErpDogField("OR_CLASSE_ASSISTENZA", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelOrClasseAssistenza  { get; set; }

[Display(Name = "Codice", ShortName="", Description = "Identificatore dell'agente", Prompt="")]
[ErpDogField("OR_CODICE", SqlFieldNameExt="", SqlFieldOptions="[UID]", Xref="", SqlFieldProperties="prop() xref() xdup(ORGANIZZAZIONE.OR__ICODE[OR__ICODE] {OR_CODICE=' '}) multbxref()")]
[DataType(DataType.Text)]
public string? SelOrCodice  { get; set; }

[Display(Name = "Descrizione", ShortName="", Description = "Descrizione dell'agente", Prompt="")]
[ErpDogField("OR_DESCRIZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelOrDescrizione  { get; set; }

[Display(Name = "Note", ShortName="", Description = "Note sull'agente", Prompt="")]
[ErpDogField("OR_NOTE", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelOrNote  { get; set; }

[Display(Name = "Email", ShortName="", Description = "Indirizzo e-mail dell'agente", Prompt="")]
[ErpDogField("OR_EMAIL", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelOrEmail  { get; set; }

[Display(Name = "Tipo Assistenza", ShortName="", Description = "Tipo dell'agente nella classificazione generale", Prompt="")]
[ErpDogField("OR_TIPO_ASSISTENZA", SqlFieldNameExt="", SqlFieldOptions="[MANDATORY]", Xref="Tz1Icode", SqlFieldProperties="prop() xref(TIPO_ORGANIZZAZIONE.TZ__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("TipoOrganizzazione", "AutocompleteGetAll", 10)]
[DataType(DataType.Text)]
public List<string> SelOrTipoAssistenza  { get; set; } = new List<string>();

[Display(Name = "Telefono", ShortName="", Description = "Numero di telefono dell'agente (quando applicabile)", Prompt="")]
[ErpDogField("OR_TELEFONO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelOrTelefono  { get; set; }

[Display(Name = "Id Personale", ShortName="", Description = "Codice del membro del personale interno corrispondente, se applicabile (solo per classe = 3)", Prompt="")]
[ErpDogField("OR_ID_PERSONALE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Pe1Icode", SqlFieldProperties="prop() xref(PERSONALE.PE__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Personale", "AutocompleteGetAll", 10)]
[DataType(DataType.Text)]
public List<string> SelOrIdPersonale  { get; set; } = new List<string>();

[Display(Name = "Id Istituto", ShortName="", Description = "Codice del centro sanitario (classe = 0) a cui appartiene l'agente (se applicabile)", Prompt="")]
[ErpDogField("OR_ID_ISTITUTO", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 10)]
[DataType(DataType.Text)]
public List<string> SelOrIdIstituto  { get; set; } = new List<string>();

[Display(Name = "Id Unita", ShortName="", Description = "Codice dell'unità (classe = 1) a cui appartiene l'agente (se applicabile)", Prompt="")]
[ErpDogField("OR_ID_UNITA", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 10)]
[DataType(DataType.Text)]
public List<string> SelOrIdUnita  { get; set; } = new List<string>();

[Display(Name = "Id Postazione", ShortName="", Description = "Codice del punto di servizio interno (classe = 2) a cui appartiene l'agente (se applicabile)", Prompt="")]
[ErpDogField("OR_ID_POSTAZIONE", SqlFieldNameExt="", SqlFieldOptions="", Xref="Or1Icode", SqlFieldProperties="prop() xref(ORGANIZZAZIONE.OR__ICODE) xdup() multbxref()")]
[DefaultValue("")]
[AutocompleteClient("Organizzazione", "AutocompleteGetAll", 10)]
[DataType(DataType.Text)]
public List<string> SelOrIdPostazione  { get; set; } = new List<string>();

[Display(Name = "Pwd Crypt", ShortName="", Description = "Password (criptata), priva di significato se è implementata l'autenticazione tramite certificati", Prompt="")]
[ErpDogField("OR_PWD_CRYPT", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelOrPwdCrypt  { get; set; }

[Display(Name = "Attivo", ShortName="", Description = "Codice specificante se l'agente è logicamente attivo nell'organizzazione o è stato (temporaneamente) disabilitato (vuoto=attivo)", Prompt="")]
[ErpDogField("OR_ATTIVO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelOrAttivo  { get; set; }

[Display(Name = "Identificativo", ShortName="", Description = "Riferimento di contatto, quando applicabile", Prompt="")]
[ErpDogField("OR_IDENTIFICATIVO", SqlFieldNameExt="", SqlFieldOptions="", Xref="", SqlFieldProperties="prop() xref() xdup() multbxref()")]
[DataType(DataType.Text)]
public string? SelOrIdentificativo  { get; set; }

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
    return new List<string>() { "sioOr1Icode|K|OR__ICODE","sioOr1RecDate|N|OR__MDATE,OR__CDATE"
        ,"sioOrIdIstitutoorIdUnitaorIdPostazione|N|OR_ID_ISTITUTO,OR_ID_UNITA,OR_ID_POSTAZIONE"
        ,"sioOrIdPostazione|N|OR_ID_POSTAZIONE"
        ,"sioOrIdPersonale|N|OR_ID_PERSONALE"
        ,"sioOrTipoAssistenza|N|OR_TIPO_ASSISTENZA"
        ,"sioOrCodiceor1Versionor1Deleted|U|OR_CODICE,OR__VERSION,OR__DELETED"
        ,"sioOrIdUnita|N|OR_ID_UNITA"
        ,"sioOr1Version|U|OR__VERSION"
    };
}
}
}
