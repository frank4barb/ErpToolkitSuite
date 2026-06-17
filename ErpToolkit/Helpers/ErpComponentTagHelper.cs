
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ErpToolkit.Helpers.Db;
using Microsoft.AspNetCore.Http;
using System.Collections;
using Mysqlx.Crud;
using ErpToolkit.Models;
using Microsoft.Extensions.Primitives;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using System;


// VALIDATE FIELD AT SERVER SIDE


namespace ErpToolkit.Helpers
{

    // Struttura scelta
    public class Choice
    {
        public string label { get; set; }
        public string value { get; set; }
    }















    //*****************************************************************************************************************************************************
    //
    // VARS
    //
    // Gestisce le variavili di ambiente della pagina e/o modello

    // usage
    //-----
    //public class .......
    //{
    //[Vars("XML")]
    //public IDictionary<string, string> vars { get; set; } = new Dictionary<string, string>();    //}
    //-------------
    //<html>
    //<head>
    //  <link href = "https://cdn.jsdelivr.net/npm/bootstrap@5.1.0/dist/css/bootstrap.min.css" rel="stylesheet">
    //  <script src = "https://code.jquery.com/jquery-3.6.0.min.js" ></ script >
    //</head>
    //<body>
    //    <div>
    //          <input asp-for="Vars" class="form-control" />
    //    </ div >
    //</ body >
    //</ html >

    //1. Creare l'Attributo Personalizzato Vars("XML")
    //   Definisci un attributo personalizzato che contenga le informazioni necessarie per vars
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class VarsAttribute : Attribute
    {
        public string Format { get; }
        public VarsAttribute(string format)
        {
            Format = format;
        }
    }


    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class HiddenVarsTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        ////--- parametri asp-for eg: <input asp-for="MyProperty" asp-readonly="N" asp-visible="Y" asp-minchars="4" />
        //[HtmlAttributeName("asp-readonly")]
        //public char? Readonly { get; set; }

        //[HtmlAttributeName("asp-visible")]
        //public char? Visible { get; set; }
        ////---


        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var containerType = For.ModelExplorer.Metadata.ContainerType; //For.ModelExplorer.Container?.ModelType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            if (property == null) { output.SuppressOutput(); return; }  // oppure log errore

            var attributeVars = property.GetCustomAttribute<VarsAttribute>();
            var format = attributeVars?.Format ?? "XML";  //default: XML

            //-------------------------------------
            //calcola prefix id name (Accedi al valore di HtmlFieldPrefix)
            //-------------------------------------

            var TagHtmlName = For.Name;
            var TagHtmlId = TagHtmlName.Replace(".", "_").Replace("[", "_").Replace("]", "_");

            ////var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            ////var prefixInputId = !string.IsNullOrEmpty(prefix) ? $"{prefix}_{TagHtmlId}" : TagHtmlId;
            ////var prefixInputName = !string.IsNullOrEmpty(prefix) ? $"{prefix}.{TagHtmlName}" : TagHtmlName;
            var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");

            //-------------------------------------
            //calcola restrizioni visibilità pagina
            //-------------------------------------
            // --> valori non modificabili e fissati a codice
            char readonlyFlag = 'Y';
            char visibleFlag = 'N';
            //-------------------------------------

            if (attributeVars != null)
            {

                //Blocco iniziale
                output.Attributes.SetAttribute("disabled", "disabled");
                output.Attributes.SetAttribute("asp-loaded", "N");

                // ogni chiave/valore viene convertita in json
                //output.TagName = null;
                //if (For.Model is IDictionary<string, string> preSelectedValues)
                //{
                //    foreach (var kvp in preSelectedValues)
                //    {
                //        var input = new TagBuilder("input");
                //        input.Attributes["type"] = "hidden";
                //        input.Attributes["name"] = $"{prefixInputName}[{kvp.Key}]";
                //        input.Attributes["id"] = $"{prefixInputId}_{kvp.Key}";
                //        input.Attributes["value"] = kvp.Value;
                //        input.Attributes["data-format"] = format;

                //        output.Content.AppendHtml(input);
                //    }
                //}
                output.TagName = "input";
                output.TagMode = TagMode.SelfClosing;
                output.Attributes.SetAttribute("type", "hidden");
                output.Attributes.SetAttribute("id", prefixInputId);
                output.Attributes.SetAttribute("name", prefixInputName);
                output.Attributes.SetAttribute("data-format", format);
                if (For.Model is IDictionary<string, string> varsDict)
                {
                    string serializedValue = DogManager.JsonSafeSerializeToBase64Url<IDictionary<string, string>>(varsDict); //string serializedValue = DogManager.JsonSafeSerialize<IDictionary<string, string>>(varsDict);
                    output.Attributes.SetAttribute("value", serializedValue);
                }
                else
                {
                    output.Attributes.SetAttribute("value", "");
                }
                output.Attributes.SetAttribute("asp-loaded", "Y");  // il controller non prevede il caricamento lato client
            }
        }

    }


    //*****************************************************************************************************************************************************
    //
    // AUTOCOMPLETE
    //
    // Carica dinamicamente le scelte dell'autocomplete in base a query eseguite lato server o client

    // usage
    //-----
    //public class .......
    //{
    //  [Display(Name = "Id Tipo Attivita", ShortName = "", Description = "Codice della classe generale di attività predefinita", Prompt = "")]
    //  [ErpDogField("AV_ID_TIPO_ATTIVITA", SqlFieldNameOLD = "AV_ID_TIPO_ATTIVITA", SqlFieldProperties = "prop() xref(TIPO_ATTIVITA.TA__ICODE) xdup() multbxref()")]
    //  [DefaultValue("")]
    //  [DataType(DataType.Text)]
    //  [AutocompleteServer("Attivita", "AutocompleteTipoAttivita", "PreLoadTipoAttivita", 3)]
    //  public List<string> AvIdTipoAttivita { get; set; } = new List<string>() { "15002", "009801" };
    //}
    //-------------
    //public class ....Controller : Controller
    //{
    //  [HttpGet]
    //  public JsonResult AutocompleteGetAllTipoAttivita()
    //  {
    //      List<Choice> list = new List<Choice>();
    //      try
    //      {
    //          string sql = "select ??????? as label, ???????? as value from ??????? where ?????????? ";
    //          DataTable dt = ErpContext.Instance.getSQLSERVERHelper("#connectionString_SQLSLocal").execQuery(sql);
    //          list = SQLSERVERHelper.ConvertDataTable<Choice>(dt, "");
    //          return Json(list);
    //      }
    //      catch (Exception ex)
    //      {
    //          return Json(new { error = "Problemi in accesso al DB: GetCities: " + ex.Message });
    //      }
    //  }
    //  [HttpGet]
    //  public JsonResult AutocompleteTipoAttivita(string term)
    //  {
    //      List<Choice> list = new List<Choice>();
    //      try
    //      {
    //          string sql = "select ??????? as label, ???????? as value from ??????? where ?????????? and upper(???????) like '%" + term.ToUpper() + "%'";
    //          DataTable dt = ErpContext.Instance.getSQLSERVERHelper("#connectionString_SQLSLocal").execQuery(sql);
    //          list = SQLSERVERHelper.ConvertDataTable<Choice>(dt, "");
    //          return Json(list);
    //      }
    //      catch (Exception ex)
    //      {
    //          return Json(new { error = "Problemi in accesso al DB: GetCities: " + ex.Message });
    //      }
    //  }
    //  [HttpPost]
    //  public JsonResult PreLoadTipoAttivita([FromBody] List<string> values)
    //  {
    //      List<Choice> list = new List<Choice>();
    //      try
    //      {
    //          string sql = "select ??????? as label, ???????? as value from ??????? where ?????????? and ??__ICODE in ('" + string.Join("', '", values.ToArray()) + "')";
    //          DataTable dt = ErpContext.Instance.getSQLSERVERHelper("#connectionString_SQLSLocal").execQuery(sql);
    //          list = SQLSERVERHelper.ConvertDataTable<Choice>(dt, "");
    //            return Json(list);
    //      }
    //      catch (Exception ex)
    //      {
    //          return Json(new { error = "Problemi in accesso al DB: GetCities: " + ex.Message });
    //      }
    //  }
    //  .......
    //}
    //-------------
    //<html>
    //<head>
    //  <link href = "https://cdn.jsdelivr.net/npm/bootstrap@5.1.0/dist/css/bootstrap.min.css" rel="stylesheet">
    //  <script src = "https://code.jquery.com/jquery-3.6.0.min.js" ></ script >
    //</head>
    //<body>
    //    <div>
    //          <h2>Autocomplete Example</h2>
    //          @Html.LabelFor(model => model.AvIdTipoAttivita, htmlAttributes: new { @class = "control-label" })
    //          <input asp-for="AvIdTipoAttivita" class="form-control" />
    //          @Html.ValidationMessageFor(model => model.AvIdTipoAttivita, "", new { @class = "text-danger" })
    //    </ div >
    //</ body >
    //</ html >

    //Per ottenere un comportamento di autocomplete che sia completamente determinato dalla presenza di un attributo sul modello, senza modificare direttamente la vista,
    //puoi seguire un approccio basato sull'uso di Tag Helpers insieme a riflessione per generare dinamicamente lo script di autocomplete.


    //1. Creare l'Attributo Personalizzato
    //   Definisci un attributo personalizzato che contenga le informazioni necessarie per l'autocomplete.
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class AutocompleteServerAttribute : Attribute
    {
        public string Controller { get; }
        public string Action { get; }
        public string PreloadAction { get; }
        public int MaxSelections { get; } = 0;
        public string ExtraFilter { get; } = ""; // Opzionale: filtro extra da passare al server per limitare i risultati (es. in base ad altri campi del modello)
        public string[] ExtraFields { get; } = Array.Empty<string>();// Opzionale: campi extra da includere nella richiesta al server (es. altri campi del modello da considerare nel filtro)

        public AutocompleteServerAttribute(string controller, string action, string preloadAction, int maxSelections = 0, string ExtraFilter = "", params string[] ExtraFields)
        {
            Controller = controller;
            Action = action;
            PreloadAction = preloadAction;
            MaxSelections = maxSelections;
            this.ExtraFilter = ExtraFilter ?? "";
            this.ExtraFields = ExtraFields ?? Array.Empty<string>();
        }
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class AutocompleteClientAttribute : Attribute
    {
        public string Controller { get; }
        public string Action { get; }
        public int MaxSelections { get; set; } = 0;
        public string ExtraFilter { get; } = ""; // Opzionale: filtro extra da passare al server per limitare i risultati (es. in base ad altri campi del modello)

        public AutocompleteClientAttribute(string controller, string action, int maxSelections = 0, string ExtraFilter = "")
        {
            Controller = controller;
            Action = action;
            MaxSelections = maxSelections;
            this.ExtraFilter = ExtraFilter ?? "";
        }
    }


    /************************************
        //2. Creare un Tag Helper
        //  Crea un Tag Helper che controlla la presenza dell'attributo personalizzato e genera il codice JavaScript per l'autocomplete.
        //[HtmlTargetElement("input", Attributes = "asp-for")]
        //[HtmlTargetElement("editor", TagStructure = TagStructure.WithoutEndTag, Attributes = ForAttributeName)]
        [HtmlTargetElement("input", Attributes = "asp-for")]
        public class AutocompleteTagHelper : TagHelper
        {
            [HtmlAttributeName("asp-for")]
            public ModelExpression For { get; set; }

            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                var property = For.Metadata.ContainerType.GetProperty(For.Name);
                var attributeServer = property.GetCustomAttributes(typeof(AutocompleteServerAttribute), false).FirstOrDefault() as AutocompleteServerAttribute;
                var attributeClient = property.GetCustomAttributes(typeof(AutocompleteClientAttribute), false).FirstOrDefault() as AutocompleteClientAttribute;

                if (attributeServer != null)
                {
                    var visibleInputName = For.Name + "Label";
                    var preSelectedValues = For.ModelExplorer.Model as List<string>;
                    var divId = $"{For.Name}SelectedItems";

                    var script = $@"
                    <script>
                        $(document).ready(function() {{
                            var input = $('#{visibleInputName}');
                            var resultsDiv = $('#autocompleteResults');
                            var selectedItemsDiv = $('#{divId}');
                            var maxSelections = {attributeServer.MaxSelections};
                            resultsDiv.hide();

                            var preSelected = {JsonConvert.SerializeObject(preSelectedValues)};
                            if (preSelected) {{
                                $.ajax({{
                                    url: '/{attributeServer.Controller}/{attributeServer.PreloadAction}',
                                    type: 'POST',
                                    contentType: 'application/json',
                                    data: JSON.stringify(preSelected),
                                    success: function(data) {{
                                        data.forEach(function(item, index) {{
                                            addSelectedItem(item.label, item.value, index);
                                        }});
                                    }}
                                }});
                            }}

                            input.on('input', function() {{
                                var term = $(this).val();
                                if (term.length >= 2) {{
                                    $.get('/{attributeServer.Controller}/{attributeServer.Action}?term=' + term, function(data) {{
                                        resultsDiv.empty();
                                        if (data.error) {{
                                            var validationMessage = $('[data-valmsg-for=""{For.Name}""]');
                                            validationMessage.text(data.error);
                                            validationMessage.show();
                                        }} else if (data.length) {{
                                            resultsDiv.show();
                                            data.forEach(function(item) {{
                                                resultsDiv.append('<div class=""autocomplete-item"" data-value=""' + item.value + '"" data-label=""' + item.label + '"">' + item.label + '</div>');
                                            }});
                                            adjustResultsDivWidth();
                                            $('.autocomplete-item').on('click', function() {{
                                                var label = $(this).data('label');
                                                var value = $(this).data('value');
                                                addSelectedItem(label, value, selectedItemsDiv.children().length);
                                                input.val('');
                                                resultsDiv.hide();
                                            }});
                                            var validationMessage = $('[data-valmsg-for=""{For.Name}""]');
                                            validationMessage.hide();
                                        }} else {{
                                            resultsDiv.hide();
                                        }}
                                    }});
                                }} else {{
                                    resultsDiv.hide();
                                }}
                            }});

                            function adjustResultsDivWidth() {{
                                resultsDiv.css('width', input.outerWidth() + 'px');
                            }}
                            input.on('focus', function() {{
                                adjustResultsDivWidth();
                            }});

                            input.on('blur', function() {{
                                input.val(''); resultsDiv.hide();
                            }});

                            $(document).on('click', '.remove-item', function() {{
                                $(this).parent().remove();
                                toggleInputVisibility();
                            }});

                            function addSelectedItem(label, value, index) {{
                                var itemDiv = $('<div class=""selected-item"" data-value=""' + value + '"">' + label + ' <span class=""remove-item"">&times;</span></div>');
                                var inputField = $('<input type=""hidden"" name=""{For.Name}[' + index + ']"" value=""' + value + '"" />');
                                itemDiv.append(inputField);
                                selectedItemsDiv.append(itemDiv);
                                toggleInputVisibility();
                            }}

                            function toggleInputVisibility() {{
                                var selectedCount = selectedItemsDiv.children().length;
                                if (maxSelections > 0 && selectedCount >= maxSelections) {{
                                    input.hide();
                                }} else {{
                                    input.show();
                                }}
                            }}

                            // Initial toggle in case there are pre-selected items
                            toggleInputVisibility();
                        }});
                    </script>
                    <div id='autocompleteResults' class='autocomplete-results' style='display: none;'></div>";

                    var selectedItemsDiv = $@"<div id='{divId}' class='selected-items'></div>";

                    output.Attributes.SetAttribute("id", visibleInputName);
                    output.Attributes.SetAttribute("name", visibleInputName);
                    output.Attributes.SetAttribute("value", "");

                    output.PostElement.AppendHtml(selectedItemsDiv);
                    output.PostElement.AppendHtml(script);
                }
                else if (attributeClient != null)
                {
                    var visibleInputName = For.Name + "Label";
                    var preSelectedValues = For.ModelExplorer.Model as List<string>;
                    var divId = $"{For.Name}SelectedItems";

                    var script = $@"
                    <script>
                        $(document).ready(function() {{
                            var input = $('#{visibleInputName}');
                            var resultsDiv = $('#autocompleteResults');
                            var selectedItemsDiv = $('#{divId}');
                            var maxSelections = {attributeClient.MaxSelections};
                            resultsDiv.hide();

                            var allChoices = [];

                            // Fetch all possible choices once
                            $.get('/{attributeClient.Controller}/{attributeClient.Action}', function(data) {{
                                allChoices = data;

                                // Process pre-selected values after all choices are loaded
                                var preSelected = {JsonConvert.SerializeObject(preSelectedValues)};
                                if (preSelected) {{
                                    preSelected.forEach(function(value) {{
                                        var item = allChoices.find(c => c.value === value);
                                        if (item) {{
                                            addSelectedItem(item.label, item.value);
                                        }}
                                    }});
                                    toggleInputVisibility();
                                }}
                            }});

                            input.on('input', function() {{
                                var term = $(this).val().toLowerCase();
                                resultsDiv.empty();
                                if (term.length >= 2) {{
                                    var filtered = allChoices.filter(c => c.label.toLowerCase().includes(term));
                                    if (filtered.length) {{
                                        resultsDiv.show();
                                        filtered.forEach(function(item) {{
                                            resultsDiv.append('<div class=""autocomplete-item"" data-value=""' + item.value + '"" data-label=""' + item.label + '"">' + item.label + '</div>');
                                        }});
                                        adjustResultsDivWidth();
                                    }} else {{
                                        resultsDiv.hide();
                                    }}
                                }} else {{
                                    resultsDiv.hide();
                                }}
                            }});

                            function adjustResultsDivWidth() {{
                                resultsDiv.css('width', input.outerWidth() + 'px');
                            }}
                            input.on('focus', function() {{
                                adjustResultsDivWidth();
                            }});

                            $(document).on('click', '.autocomplete-item', function() {{
                                var label = $(this).data('label');
                                var value = $(this).data('value');
                                addSelectedItem(label, value);
                                input.val('');
                                resultsDiv.hide();
                            }});

                            $(document).on('click', '.remove-item', function() {{
                                $(this).parent().remove();
                                toggleInputVisibility();
                            }});


                            var isSelectingItem = false;
                            $(document).on('mousedown', '.autocomplete-item', function() {{
                                isSelectingItem = true;
                            }});
                            $(document).on('mouseup', '.autocomplete-item', function() {{
                                isSelectingItem = false;
                            }});
                            input.on('blur', function() {{
                                input.val('');
                                setTimeout(function() {{
                                    if (!isSelectingItem) {{
                                        resultsDiv.hide();
                                    }}
                                }}, 100);
                            }});


                            function addSelectedItem(label, value) {{
                                var itemDiv = $('<div class=""selected-item"" data-value=""' + value + '"">' + label + ' <span class=""remove-item"">&times;</span></div>');
                                var inputField = $('<input type=""hidden"" name=""{For.Name}"" value=""' + value + '"" />');
                                itemDiv.append(inputField);
                                selectedItemsDiv.append(itemDiv);
                                toggleInputVisibility();
                            }}

                            function toggleInputVisibility() {{
                                var selectedCount = selectedItemsDiv.children().length;
                                if (maxSelections > 0 && selectedCount >= maxSelections) {{
                                    input.hide();
                                }} else {{
                                    input.show();
                                }}
                            }}

                            // Initial toggle in case there are pre-selected items
                            toggleInputVisibility();
                        }});
                    </script>
                    <div id='autocompleteResults' class='autocomplete-results' style='display: none;'></div>";

                    var selectedItemsDiv = $@"<div id='{divId}' class='selected-items'></div>";

                    output.Attributes.SetAttribute("id", visibleInputName);
                    output.Attributes.SetAttribute("name", visibleInputName);
                    output.Attributes.SetAttribute("value", "");

                    output.PostElement.AppendHtml(selectedItemsDiv);
                    output.PostElement.AppendHtml(script);
                }

            }
    ******************************/



    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class MultiSelectAutocompleteTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        //--- parametri asp-for eg: <input asp-for="MyProperty" asp-readonly="N" asp-visible="Y" asp-minchars="4" />
        [HtmlAttributeName("asp-readonly")]
        public char? Readonly { get; set; }

        [HtmlAttributeName("asp-visible")]
        public char? Visible { get; set; }

        [HtmlAttributeName("asp-minchars")]
        public int MinChars { get; set; } = 3; // Numero di caratteri predefinito (solo per server autocomplete)
        //---


        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
 
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var containerType = For.ModelExplorer.Metadata.ContainerType; //For.ModelExplorer.Container?.ModelType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            if (property == null) { output.SuppressOutput(); return; }  // oppure log errore
            //??//var property = For.Metadata.ContainerType.GetProperty(For.Name);

            var attributeServer = property.GetCustomAttributes(typeof(AutocompleteServerAttribute), false).FirstOrDefault() as AutocompleteServerAttribute;
            var attributeClient = property.GetCustomAttributes(typeof(AutocompleteClientAttribute), false).FirstOrDefault() as AutocompleteClientAttribute;
            var attributeErpDogField = property.GetCustomAttributes(typeof(ErpDogFieldAttribute), false).FirstOrDefault() as ErpDogFieldAttribute;
            var attributeErpDogField_Xref = attributeErpDogField?.Xref ?? "";

            //-------------------------------------
            //calcola prefix id name (Accedi al valore di HtmlFieldPrefix)
            //-------------------------------------

            //var TagHtmlName = For.Name;
            //var TagHtmlId = TagHtmlName.Replace(".", "_").Replace("[", "_").Replace("]", "_");

            //var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            //var prefixInputId = !string.IsNullOrEmpty(prefix) ? $"{prefix}_{TagHtmlId}" : TagHtmlId;
            //var prefixInputName = !string.IsNullOrEmpty(prefix) ? $"{prefix}.{TagHtmlName}" : TagHtmlName;

            //??//var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            //??//var prefixInputId = (prefix != "") ? prefix + "_" + For.Name : For.Name;
            //??//var prefixInputName = (prefix != "") ? prefix + "." + For.Name : For.Name;

            var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");

            //var modelPropertyName = $"{property?.ReflectedType?.FullName ?? ""}.{For.Name}";
            var modelPropertyName = For.Name;

            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            // Esempio TagHelper custom (semplificato)
            //^^^
            //output.TagName = "div";
            //output.Attributes.SetAttribute("class", "taghelper");
            //output.Attributes.SetAttribute("data-field", prefixInputName);
            //output.Content.AppendHtml($@"
            //<label for=""{prefixInputId}"">{For.Metadata.DisplayName}</label>
            //<input id=""{prefixInputId}"" name=""{prefixInputName}"" class=""form-control"" />
            //<span class=""text-danger"" data-valmsg-for=""{prefixInputName}"" data-valmsg-replace=""true""></span>");
            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^


            //-------------------------------------
            //calcola restrizioni visibilità pagina
            //-------------------------------------
            //??//DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, attributeErpDogField_Xref, ViewContext);
            DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, attributeErpDogField_Xref, ViewContext);
            char readonlyFlag = Readonly ?? attrField.Readonly;
            char visibleFlag = Visible ?? attrField.Visible;
            //-------------------------------------

            if (attributeServer != null)
            {
                //MinChars = 3; // configurato da variabile con default=3

                //Blocco iniziale
                output.Attributes.SetAttribute("disabled", "disabled");
                output.Attributes.SetAttribute("asp-loaded", "N");

                var preSelectedValues = new List<string>();
                if (For.ModelExplorer.Model is string) preSelectedValues = new List<string>() { For.ModelExplorer.Model as string };
                else if (For.ModelExplorer.Model is long) preSelectedValues = new List<string>() { For.ModelExplorer.Model.ToString() as string };  //le chiavi esterne possono essere solo string o long
                else preSelectedValues = For.ModelExplorer.Model as List<string>;

                var divId = $"{prefixInputId}SelectedItems";

                var preSelectedValuesJson = preSelectedValues != null ? "{ \"preSelected\": " + JsonConvert.SerializeObject(preSelectedValues) + " }" : "{ \"preSelected\": [] }";

                var selectedItemsDiv = $@"<div id='{divId}' class='selected-items'></div>";

                //--
                var extraFieldsJson = (attributeServer.ExtraFields != null && attributeServer.ExtraFields.Length > 0) ? JsonConvert.SerializeObject(attributeServer.ExtraFields) : "[]";
                output.Attributes.SetAttribute("data-extra-fields", extraFieldsJson);
                output.Attributes.SetAttribute("data-prefix", prefix);
                //--

                output.Attributes.SetAttribute("class", "autocomplete-input form-control");
                output.Attributes.SetAttribute("autocomplete", "off");
                output.Attributes.SetAttribute("data-max-selections", attributeServer.MaxSelections);
                output.Attributes.SetAttribute("data-controller", attributeServer.Controller);
                output.Attributes.SetAttribute("data-action", attributeServer.Action);
                output.Attributes.SetAttribute("data-preload-action", attributeServer.PreloadAction);
                output.Attributes.SetAttribute("data-pre-selected", preSelectedValuesJson);
                output.Attributes.SetAttribute("data-id", prefixInputId);
                output.Attributes.SetAttribute("data-name", prefixInputName);

                output.Attributes.SetAttribute("data-property-name", modelPropertyName);

                output.Attributes.SetAttribute("data-min-chars", MinChars);
                output.Attributes.SetAttribute("data-mode", "autocompleteServer");  // Modalità di autocomplete
                output.Attributes.SetAttribute("data-readonly", readonlyFlag);  // Readonly field value
                output.Attributes.SetAttribute("data-visible", visibleFlag);  // Visible field value
                output.Attributes.SetAttribute("data-selected-items-div-id", divId); // Aggiungi l'ID del div degli elementi selezionati
                output.Attributes.SetAttribute("value", ""); //pulisco valore campo

                // Aggiungi il wrapper per l'input e l'icona
                output.PreElement.AppendHtml($"<div id='{prefixInputId}AutocompleteWrapper' class='taghelper autocomplete-wrapper'>");
                output.PostElement.AppendHtml($"<div class='autocomplete-icon'><i class='bi bi-search' aria-hidden='true'></i></div></div>");
                //--

                output.PostElement.AppendHtml(selectedItemsDiv);
                output.PostElement.AppendHtml($"<div id='{prefixInputId}AutocompleteResults' class='autocomplete-results' style='display: none;'></div>"); // Aggiungi l'ID del div dei risultati dell'autocomplete
            }
            else if (attributeClient != null)
            {
                MinChars = 1;  // forzo a 1 per client tanto le scelte sono caricate localmente

                //Blocco iniziale
                output.Attributes.SetAttribute("disabled", "disabled");
                output.Attributes.SetAttribute("asp-loaded", "N");

                var preSelectedValues = new List<string>();
                if (For.ModelExplorer.Model is string) preSelectedValues = new List<string>() { For.ModelExplorer.Model as string };
                else if (For.ModelExplorer.Model is long) preSelectedValues = new List<string>() { For.ModelExplorer.Model.ToString() as string }; //le chiavi esterne possono essere solo string o long
                else preSelectedValues = For.ModelExplorer.Model as List<string>;
                var divId = $"{prefixInputId}SelectedItems";

                var preSelectedValuesJson = preSelectedValues != null ? "{ \"preSelected\": "+JsonConvert.SerializeObject(preSelectedValues)+" }" : "{ \"preSelected\": [] }";
                //var encodedPreSelectedValuesJson = HtmlEncoder.Default.Encode(preSelectedValuesJson);


                var selectedItemsDiv = $@"<div id='{divId}' class='selected-items'></div>";

                output.Attributes.SetAttribute("class", "autocomplete-input form-control");
                output.Attributes.SetAttribute("autocomplete", "off");
                output.Attributes.SetAttribute("data-max-selections", attributeClient.MaxSelections);
                output.Attributes.SetAttribute("data-controller", attributeClient.Controller);
                output.Attributes.SetAttribute("data-action", attributeClient.Action);
                output.Attributes.SetAttribute("data-pre-selected", preSelectedValuesJson);
                output.Attributes.SetAttribute("data-id", prefixInputId);
                output.Attributes.SetAttribute("data-name", prefixInputName);

                output.Attributes.SetAttribute("data-property-name", modelPropertyName);

                output.Attributes.SetAttribute("data-min-chars", MinChars);
                output.Attributes.SetAttribute("data-mode", "autocompleteClient");  // Modalità di autocomplete
                output.Attributes.SetAttribute("data-readonly", readonlyFlag);  // Readonly field value
                output.Attributes.SetAttribute("data-visible", visibleFlag);  // Visible field value
                output.Attributes.SetAttribute("data-selected-items-div-id", divId); // Aggiungi l'ID del div degli elementi selezionati
                output.Attributes.SetAttribute("value", ""); //pulisco valore campo

                // Aggiungi il wrapper per l'input e l'icona
                output.PreElement.AppendHtml($"<div id='{prefixInputId}AutocompleteWrapper' class='taghelper autocomplete-wrapper'>");
                output.PostElement.AppendHtml($"<div class='autocomplete-icon'><i class='bi bi-search' aria-hidden='true'></i></div></div>");
                //--

                output.PostElement.AppendHtml(selectedItemsDiv);
                output.PostElement.AppendHtml($"<div id='{prefixInputId}AutocompleteResults' class='autocomplete-results' style='display: none;'></div>"); // Aggiungi l'ID del div dei risultati dell'autocomplete

            }
        }



    }



    //*****************************************************************************************************************************************************
    //
    // CANCELLA LABEL
    //
    // Cancella le label degli Input invisibili

    [HtmlTargetElement("label", Attributes = "asp-for")]
    public class EliminaLabel_TagHelper : TagHelper  //public class EliminaLabel_DateRangeTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var containerType = For.ModelExplorer.Metadata.ContainerType; //For.ModelExplorer.Container?.ModelType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            if (property == null) { output.SuppressOutput(); return; }  // oppure log errore
            //??//var property = For.Metadata.ContainerType.GetProperty(For.Name);

            var attributeServer = property.GetCustomAttributes(typeof(AutocompleteServerAttribute), false).FirstOrDefault() as AutocompleteServerAttribute;
            var attributeClient = property.GetCustomAttributes(typeof(AutocompleteClientAttribute), false).FirstOrDefault() as AutocompleteClientAttribute;
            var attributeErpDogField = property.GetCustomAttributes(typeof(ErpDogFieldAttribute), false).FirstOrDefault() as ErpDogFieldAttribute;
            var attributeErpDogField_Xref = attributeErpDogField?.Xref ?? "";

            //-------------------------------------
            //calcola prefix id name (Accedi al valore di HtmlFieldPrefix)
            //-------------------------------------
            ////var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            ////var prefixInputId = (prefix != "") ? prefix + "_" + For.Name : For.Name;
            ////var prefixInputName = (prefix != "") ? prefix + "." + For.Name : For.Name;
            var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");
            //-------------------------------------
            //calcola restrizioni visibilità pagina
            //-------------------------------------
            DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, attributeErpDogField_Xref, ViewContext);
            //-------------------------------------

            // SE LABEL di DataRange ==> cancello sempre la label
            if (property != null && property.GetCustomAttributes(typeof(DateRangeAttribute), false).Length > 0)
            {
                output.SuppressOutput(); return;
            }

            // SE LABEL di INPUT non visibile ==> cancello la label
            if (attrField.Visible == 'N')
            {
                output.SuppressOutput(); return;
            }

        }
    }



    //*****************************************************************************************************************************************************
    //
    // INTERVALLO DI DATE
    //
    // Carica dinamicamente la coppia di date Inizio e Fine da applicare ai filtri di selezione

    //usage
    //[DateRange]
    //[Required]
    //public DateRange Intervallo_di_date { get; set; }


    // Modello
    public class DateRange
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }

    //Attributi

    public class DateRangeAttribute : ValidationAttribute
    {
        public string Options { get; set; } = ""; // contiene le opzioni di verifica separate da spazio. eg: BoundedRange
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateRange = value as DateRange;
            if (dateRange != null)
            {
                // intervallo di date limitato
                if (Options.Contains("BoundedRange") && (dateRange.StartDate == default || dateRange.EndDate == default))  
                {
                    return new ValidationResult("Entrambe le date devono essere compilate.", new[] { validationContext.MemberName });
                }

                if (dateRange.StartDate != default && dateRange.EndDate != default && dateRange.StartDate > dateRange.EndDate)
                {
                    return new ValidationResult("La data d'inizio deve precedere la data di fine.", new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }


    //[HtmlTargetElement("input", Attributes = "asp-for")]
    //public class DateRangeTagHelper : TagHelper
    //{
    //    [HtmlAttributeName("asp-for")]
    //    public ModelExpression For { get; set; }

    //    public override void Process(TagHelperContext context, TagHelperOutput output)
    //    {
    //        if (For.Metadata.ContainerType.GetProperty(For.Name).GetCustomAttributes(typeof(DateRangeAttribute), false).Length > 0)
    //        {
    //            var daterangeAttribute = (DateRangeAttribute)For.Metadata.ContainerType.GetProperty(For.Name).GetCustomAttributes(typeof(DateRangeAttribute), false)[0];
    //            string options = daterangeAttribute.Options;
    //            string displayName = For.Metadata.DisplayName ?? For.Name;
    //            string startDateLabel = $"{displayName}: Inizio";
    //            string endDateLabel = $"{displayName}: Fine";
    //            string format = options == "DateTime" ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy";  // future use: attualmente non implementato

    //            string startDateId = $"{For.Name}.StartDate";
    //            string endDateId = $"{For.Name}.EndDate";

    //            string content = $@"
    //            <div class='row'>
    //                <div class='col-md-6'>
    //                    <label for='{startDateId}'>{startDateLabel}</label>
    //                    <input class='form-control' type='date' data-val='true' data-val-length='Inserire massimo 10 caratteri' data-val-length-max='10' 
    //                                            id='{startDateId}' name='{startDateId}' value=''>
    //                    <input name='__Invariant' type='hidden' value='{startDateId}'>
    //                </div>
    //                <div class='col-md-6'>
    //                    <label for='{endDateId}'>{endDateLabel}</label>
    //                    <input class='form-control' type='date' data-val='true' data-val-length='Inserire massimo 10 caratteri' data-val-length-max='10' 
    //                                            id='{endDateId}' name='{endDateId}' value=''>
    //                    <input name='__Invariant' type='hidden' value='{endDateId}'>
    //                </div>
    //            </div>";

    //            output.Attributes.SetAttribute("type", "hidden");
    //            output.Attributes.SetAttribute("value", "");
    //            output.PostElement.AppendHtml(content);

    //        }
    //    }
    //}


    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class DateRangeTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var containerType = For.ModelExplorer.Metadata.ContainerType; //For.ModelExplorer.Container?.ModelType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            if (property == null) { output.SuppressOutput(); return; }  // oppure log errore
            //??//var property = For.Metadata.ContainerType.GetProperty(For.Name);

            var attributeErpDogField = property.GetCustomAttributes(typeof(ErpDogFieldAttribute), false).FirstOrDefault() as ErpDogFieldAttribute;
            var attributeErpDogField_Xref = attributeErpDogField?.Xref ?? "";

            if (property.GetCustomAttributes(typeof(DateRangeAttribute), false).Length > 0)
            {
                var daterangeAttribute = (DateRangeAttribute)property.GetCustomAttributes(typeof(DateRangeAttribute), false)[0];
                string options = daterangeAttribute.Options;
                string displayName = For.Metadata.DisplayName ?? For.Name;
                string startDateLabel = $"{displayName}: Inizio";
                string endDateLabel = $"{displayName}: Fine";
                //string format = options == "DateTime" ? "dd-MM-yyyy HH:mm" : "dd-MM-yyyy";  // future use: attualmente non implementato
                string format = options == "DateTime" ? "yyyy-MM-ddTHH:mm" : "yyyy-MM-dd";  // Formato ISO 8601 ( i browser si aspettano automaticamente un formato ISO 8601 (yyyy-MM-dd) per gli input di tipo date)


                //-------------------------------------
                //calcola prefix id name (Accedi al valore di HtmlFieldPrefix)
                //-------------------------------------
                ////var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
                ////var prefixInputId = (prefix != "") ? prefix + "_" + For.Name : For.Name;
                ////var prefixInputName = (prefix != "") ? prefix + "." + For.Name : For.Name;
                var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
                var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
                var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");
                //-------------------------------------
                //calcola restrizioni visibilità pagina
                //-------------------------------------
                DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, attributeErpDogField_Xref, ViewContext);
                //-------------------------------------


                string startDateId = $"{prefixInputId}_StartDate", startDateName = $"{prefixInputName}.StartDate";
                string endDateId = $"{prefixInputId}_EndDate", endDateName = $"{prefixInputName}.EndDate";


                // Recupera i valori dal modello
                DateTime? startDateValue = For.Model?.GetType().GetProperty("StartDate")?.GetValue(For.Model) as DateTime?;
                DateTime? endDateValue = For.Model?.GetType().GetProperty("EndDate")?.GetValue(For.Model) as DateTime?;
                if (startDateValue != null && (DateTime)startDateValue == default) startDateValue = null;
                if (endDateValue != null && (DateTime)endDateValue == default) endDateValue = null;

                // Formatta i valori per il campo di input
                string startDateFormatted = startDateValue?.ToString(format) ?? "";
                string endDateFormatted = endDateValue?.ToString(format) ?? "";

                string content = $@"
                <div class='row taghelper date-range'>
                    <div class='col-md-6'>
                        <label for='{startDateId}'>{startDateLabel}</label>
                        <input class='date-range-input form-control' type='date' data-val='true' id='{startDateId}' name='{startDateName}' value='{startDateFormatted}' {(attrField.Readonly == 'Y' ? "readonly" : "")}>
                        <input name='__Invariant' type='hidden' value='{startDateId}'>
                    </div>
                    <div class='col-md-6'>
                        <label for='{endDateId}'>{endDateLabel}</label>
                        <input class='date-range-input form-control' type='date' data-val='true' id='{endDateId}' name='{endDateName}' value='{endDateFormatted}' {(attrField.Readonly == 'Y' ? "readonly" : "")}>
                        <input name='__Invariant' type='hidden' value='{endDateId}'>
                    </div>
                </div>";


                if (attrField.Visible == 'N')
                {
                    // Se Visible è N, nascondiamo l'intero controllo
                    output.SuppressOutput();
                }
                else
                {
                    output.Attributes.SetAttribute("type", "hidden");
                    output.Attributes.SetAttribute("value", "");
                    output.PostElement.AppendHtml(content);
                }
            }
        }
    }


    //*****************************************************************************************************************************************************
    //
    // SCELTA SINGOLA O MULTIPLA
    //


    //public class YourViewModel
    //{
    //    [MultipleChoices(new[] { "A", "B", "C" }, maxSelections: 3, labelProviderAction: "GetLabels")]
    //    public List<string> EpClasseEpisodioMultiplo { get; set; } = new List<string>();
    //
    //    [MultipleChoices(new[] { "Choice 1", "Choice 2", "Choice 3" }, MaxSelections = 1)]
    //    public string EpClasseEpisodioSingolo { get; set; }
    //}


    //@model YourViewModel
    //<form>
    //    <switch-group asp-for="Model.EpClasseEpisodioMultiplo" readonly= "N" visible= "Y" >
    //    </switch-group>

    //    <button type = "submit" class="btn btn-primary">Submit</button>
    //</form>


    //1.Attributo Personalizzato
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class MultipleChoicesAttribute : Attribute
    {
        public string[] Choices { get; }
        public string[]? LabelChoices { get; set; } = null;
        public int MaxSelections { get; set; } = 1;  // disable controll if MaxSelections < 1
        public string LabelContoller { get; set; } = "Home";
        public string LabelAction { get; set; } = "GetLabels";
        public string LabelClassName { get; set; } = "";

        public MultipleChoicesAttribute(string[] choices, string[]? labelChoices = null, int maxSelections = 1, string labelContoller = "Home", string labelAction = "GetLabels", string labelClassName = "")
        {
            Choices = choices;
            LabelChoices = labelChoices;
            MaxSelections = maxSelections; 
            LabelContoller = labelContoller;
            LabelAction = labelAction;
            LabelClassName = labelClassName;
        }
    }

    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class SwitchGroupMultipleChoicesTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SwitchGroupMultipleChoicesTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //----------------------------------

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-for")]

        public ModelExpression For { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var containerType = For.ModelExplorer.Metadata.ContainerType; //For.ModelExplorer.Container?.ModelType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            if (property == null) { output.SuppressOutput(); return; }  // oppure log errore
            //??//var property = For.ModelExplorer.Container.ModelType.GetProperty(For.Name);

            var multipleChoicesAttribute = property.GetCustomAttributes(typeof(MultipleChoicesAttribute), false).FirstOrDefault() as MultipleChoicesAttribute;
            var attributeErpDogField = property.GetCustomAttributes(typeof(ErpDogFieldAttribute), false).FirstOrDefault() as ErpDogFieldAttribute;
            var attributeErpDogField_Xref = attributeErpDogField?.Xref ?? "";

            // LEGGI data-mandatory SENZA BINDER
            bool isMandatory =
                context.AllAttributes.ContainsName("data-mandatory") &&
                context.AllAttributes["data-mandatory"]?.Value?.ToString() == "true";


            if (multipleChoicesAttribute != null)
            {

                //-------------------------------------
                //calcola prefix id name (Accedi al valore di HtmlFieldPrefix)
                //-------------------------------------
                ////var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
                ////var prefixInputId = (prefix != "") ? prefix + "_" + For.Name : For.Name;
                ////var prefixInputName = (prefix != "") ? prefix + "." + For.Name : For.Name;
                var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
                var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
                var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");
                //-------------------------------------
                //calcola restrizioni visibilità pagina
                //-------------------------------------
                DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, attributeErpDogField_Xref, ViewContext);
                //-------------------------------------

                if (attrField.Visible == 'N')
                {
                    output.SuppressOutput();
                    return;
                }

                var choices = multipleChoicesAttribute.Choices;
                var maxSelections = multipleChoicesAttribute.MaxSelections; // disable controll if maxSelections < 1 
                var isMultiple = maxSelections != 1;
                //var name = For.Name;
                var readonlyAttr = attrField.Readonly == 'Y' ? "disabled" : "";
                var labels = choices;
                if (multipleChoicesAttribute.LabelChoices != null) labels = multipleChoicesAttribute.LabelChoices;

                // Se è specificata l'azione per ottenere le label, esegue la chiamata al controller
                if (!string.IsNullOrEmpty(multipleChoicesAttribute.LabelClassName))
                {
                    var labelResponse = GetLabelsFromController(multipleChoicesAttribute.LabelClassName, multipleChoicesAttribute.LabelAction, multipleChoicesAttribute.LabelClassName);

                    if (!string.IsNullOrEmpty(labelResponse.Item2))
                    {
                        // Gestione dell'errore
                        output.Content.SetHtmlContent($"<div class='alert alert-danger'>Errore: {labelResponse.Item1}</div>");
                        return;
                    }
                    var labelChoices = labelResponse.Item1;
                    labels = choices
                                .Select(choice => labelChoices.FirstOrDefault(item => item.value == choice)?.label)
                                .ToArray() ?? choices;
                }

                // Determina i valori pre-selezionati
                var selectedValues = new HashSet<string>();

                if (isMultiple && For.Model is IEnumerable<string> modelList)
                {
                    selectedValues = new HashSet<string>(modelList);
                }
                else if (For.Model is string modelValue)
                {
                    selectedValues.Add(modelValue);
                }

                // HTML per il gruppo di switch
                var content = new StringBuilder();
                content.AppendLine("<div class='taghelper switch-group'>");

                for (int i = 0; i < choices.Length; i++)
                {
                    //if (i > 0 && i % 6 == 0)
                    //{
                    //    content.AppendLine("<div class='w-100'></div>"); // Line break after 6 items
                    //}

                    string id = $"{prefixInputId}_{i}";
                    string value = choices[i].Trim();
                    string label = labels[i].Trim();
                    string inputType = isMultiple ? "checkbox" : "radio";
                    string checkedAttr = selectedValues.Contains(value) ? "checked" : "";

                    if (isMandatory)
                    {
                        content.AppendLine($@"
                        <div class='form-check form-switch d-inline-block mb-2'>
                            <input class='form-check-input' type='{inputType}' data-mandatory='true' name='{prefixInputName}' id='{id}' value='{value}' {checkedAttr} {readonlyAttr} onchange='handleMaxSelections(""{prefixInputName}"", {maxSelections})'>
                            <label class='form-check-label' for='{id}'>{label}</label> &nbsp; &nbsp; 
                        </div>");
                    }
                    else
                    {
                        content.AppendLine($@"
                        <div class='form-check form-switch d-inline-block mb-2'>
                            <input class='form-check-input' type='{inputType}' name='{prefixInputName}' id='{id}' value='{value}' {checkedAttr} {readonlyAttr} onchange='handleMaxSelections(""{prefixInputName}"", {maxSelections})'>
                            <label class='form-check-label' for='{id}'>{label}</label> &nbsp; &nbsp; 
                        </div>");
                    }

                }

                content.AppendLine("</div>");

                // JavaScript per gestire il numero massimo di selezioni
                if (isMultiple && maxSelections > 1)
                {
                    content.AppendLine($@"
                        <script>
                            document.addEventListener('DOMContentLoaded', function() {{
                                handleMaxSelections('{prefixInputName}', {maxSelections});
                            }});
                        </script>");
                }

                output.SuppressOutput();  // elimino il tag input e sostituisco con tag radio o checkbox
                output.PostElement.AppendHtml(content.ToString());

            }

        }

        private Tuple<List<Choice>?, string?> GetLabelsFromController(string controllerName, string actionName, string labelType)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{controllerName}/{actionName}?labelType={labelType}";

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    var result = response.Content.ReadAsStringAsync().Result;

                    // Prova a deserializzare prima come un dizionario (per catturare eventuali errori)  //ErpContext.Instance.DogFactory
                    var errorData = DogManager.JsonStaticSafeDeserialize<Dictionary<string, string>>(result); //var errorData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(result);

                    if (errorData != null && errorData.ContainsKey("error"))
                    {
                        // C'è un errore, restituisci il messaggio di errore
                        return new Tuple<List<Choice>?, string?>(null, $"Errore nella chiamata a {actionName}: {errorData["error"]}");
                    }

                    // Altrimenti, prova a deserializzare come una lista di scelte
                    var responseData = DogManager.JsonStaticSafeDeserialize<List<Choice>>(result); //var responseData = System.Text.Json.JsonSerializer.Deserialize<List<Choice>>(result);

                    if (responseData == null)
                    {
                        throw new Exception("La deserializzazione ha restituito un oggetto nullo.");
                    }

                    //var labels = responseData.Select(choice => choice.label).ToArray();
                    return new Tuple<List<Choice>?, string?>(responseData, null);
                }
            }
            catch (Exception ex)
            {
                return new Tuple<List<Choice>?, string?>(null, $"Errore nella chiamata a {actionName}: {ex.Message}");
            }
        }
    }

    //*****************************************************************************************************************************************************
    //
    // QUILL EDITOR
    //

    // ?????????????????? da verificare ????????????????????????????????????????


    //Step 1: Aggiungere Quill al progetto
    //Puoi includere Quill usando un CDN.Aggiungi questo nel tuo layout _Layout.cshtml:

    //html
    //Copia codice
    //<link href="https://cdn.quilljs.com/1.3.6/quill.snow.css" rel="stylesheet">
    //<script src = "https://cdn.quilljs.com/1.3.6/quill.min.js" ></ script >

    //Step 2: Utilizzare il TagHelper nella Vista
    //Ecco come puoi usare il TagHelper nella tua vista:

    //html
    //Copia codice
    //<input asp-for="Descrizione" />
    //<input asp-for="Commenti" />

    //Step 3: Modello e Salvataggio
    //(Nel modello, puoi definire la proprietà come string)
    //
    //public class YourViewModel
    //{
    //    [QuillEditor(Height = "500px", MaxLength = 5000, AllowImages = true, AllowCopyPaste = false)]
    //    public string Descrizione { get; set; }
    //
    //    [QuillEditor(Height = "300px", MaxLength = 10000, AllowImages = false, AllowCopyPaste = true)]
    //    public string Commenti { get; set; }
    //}
    //Quill salva il contenuto formattato come HTML, quindi puoi memorizzarlo direttamente in una stringa nel database.



    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ErpQuillEditorAttribute : Attribute
    {
        public string Height { get; set; } = "300px";                           // Altezza predefinita
        public int MaxLength { get; set; } = 10000;                             // Lunghezza massima predefinita
        public bool AllowImages { get; set; } = true;                           // Possibilità di inserire immagini
        public string[] ToolbarOptions { get; set; } = new string[]             // Opzioni di formattazione predefinite
        {
            "bold", "italic", "underline", "strike", "blockquote",
            "code-block", "header", "list", "script", "indent", "direction",
            "size", "color", "background", "font", "align", "link", "image", "video"
        };
        public bool AllowCopyPaste { get; set; } = true;                        // Opzione per abilitare/disabilitare copia-incolla

        public ErpQuillEditorAttribute() { }
    }


    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class QuillEditorTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var containerType = For.ModelExplorer.Metadata.ContainerType; //For.ModelExplorer.Container?.ModelType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            if (property == null) { output.SuppressOutput(); return; }  // oppure log errore
            //??//var property = For.Metadata.ContainerType.GetProperty(For.Name);

            var quillEditorAttr = property?.GetCustomAttribute<ErpQuillEditorAttribute>();

            if (quillEditorAttr == null)
            {
                return;
            }

            var name = For.Name;
            var uniqueEditorId = name.Replace(".", "_"); // Sostituisci i punti con underscore per creare un ID unico
            var value = For.Model?.ToString() ?? string.Empty;
            var height = quillEditorAttr.Height;
            var maxLength = quillEditorAttr.MaxLength;
            var allowImages = quillEditorAttr.AllowImages ? "true" : "false";
            var toolbarOptions = string.Join(", ", quillEditorAttr.ToolbarOptions.Select(o => $"'{o}'"));
            var allowCopyPaste = quillEditorAttr.AllowCopyPaste ? "true" : "false";

            output.TagName = "div";
            output.Attributes.SetAttribute("id", uniqueEditorId);
            output.Attributes.SetAttribute("style", $"height: {height};");

            output.PostElement.SetHtmlContent($@"
            <script>
                var quill_{uniqueEditorId} = new Quill('#{uniqueEditorId}', {{
                    theme: 'snow',
                    modules: {{
                        toolbar: [{toolbarOptions}],
                        imageDrop: {allowImages},
                    }},
                    readOnly: false
                }});

                quill_{uniqueEditorId}.on('text-change', function(delta, oldDelta, source) {{
                    var text = quill_{uniqueEditorId}.getText();
                    if (text.length > {maxLength}) {{
                        quill_{uniqueEditorId}.deleteText({maxLength}, text.length);
                    }}
                    document.querySelector('input[name=""{name}""]').value = quill_{uniqueEditorId}.root.innerHTML;
                }});

                // Gestione del copia-incolla
                if ({allowCopyPaste} === false) {{
                    quill_{uniqueEditorId}.root.addEventListener('copy', function(e) {{
                        e.preventDefault();
                    }});
                    quill_{uniqueEditorId}.root.addEventListener('paste', function(e) {{
                        e.preventDefault();
                    }});
                    quill_{uniqueEditorId}.root.addEventListener('cut', function(e) {{
                        e.preventDefault();
                    }});
                }}

                quill_{uniqueEditorId}.root.innerHTML = `{value}`;
            </script>
            <input type='hidden' name='{name}' value='{value}' />
            ");
        }
    }


    //stessa versione, ma non abbinabile ad un campo del DB tramite il tag <input>
    // ie: questa la posso usare indiscriminatamente su tutti i campi testo del db e/o su campi non abbinati al DB
    [HtmlTargetElement("erp-quill-editor", Attributes = "asp-for")]
    public class ErpQuillEditorTagHelper : TagHelper
    {
        // --- Attributi comuni ---
        [HtmlAttributeName("asp-readonly")] public char? Readonly { get; set; }
        [HtmlAttributeName("asp-visible")] public char? Visible { get; set; }

        [ViewContext][HtmlAttributeNotBound] public ViewContext ViewContext { get; set; }
        [HtmlAttributeName("asp-for")] public ModelExpression For { get; set; }

        [HtmlAttributeName("height")] public string Height { get; set; } = "300px";                           // Altezza predefinita
        [HtmlAttributeName("max-length")] public int MaxLength { get; set; } = 10000;                             // Lunghezza massima predefinita
        [HtmlAttributeName("allow-images")] public bool AllowImages { get; set; } = true;                           // Possibilità di inserire immagini
        [HtmlAttributeName("toolbar-options")] public string[] ToolbarOptions { get; set; } = new string[]             // Opzioni di formattazione predefinite
                                            {
                                                "bold", "italic", "underline", "strike", "blockquote",
                                                "code-block", "header", "list", "script", "indent", "direction",
                                                "size", "color", "background", "font", "align", "link", "image", "video"
                                            };
        [HtmlAttributeName("allow-copy-paste")] public bool AllowCopyPaste { get; set; } = true;                        // Opzione per abilitare/disabilitare copia-incolla

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // --- Dati riflessioni / attributi modello ---
            var containerType = For.ModelExplorer.Metadata.ContainerType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            var attrErpQuillEditor = property?.GetCustomAttributes(typeof(ErpQuillEditorAttribute), false).FirstOrDefault() as ErpQuillEditorAttribute;

            // --- Prefix per i name/id degli input ---
            var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");

            // --- Visibilità/readonly dal tuo sistema ---
            DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, "", ViewContext);
            char readonlyFlag = Readonly ?? attrField.Readonly;
            char visibleFlag = Visible ?? attrField.Visible;

            // gestione attributi di default
            var height = Height;
            var maxLength = MaxLength;
            var allowImages = AllowImages ? "true" : "false";
            var toolbarOptions = string.Join(", ", ToolbarOptions.Select(o => $"'{o}'"));
            var allowCopyPaste = AllowCopyPaste ? "true" : "false";
            if (attrErpQuillEditor != null) // se definiti prendo quelli del modello
            {
                height = attrErpQuillEditor.Height;
                maxLength = attrErpQuillEditor.MaxLength;
                allowImages = attrErpQuillEditor.AllowImages ? "true" : "false";
                toolbarOptions = string.Join(", ", attrErpQuillEditor.ToolbarOptions.Select(o => $"'{o}'"));
                allowCopyPaste = attrErpQuillEditor.AllowCopyPaste ? "true" : "false";
            }

            var value = For.Model?.ToString() ?? string.Empty;

            output.TagName = "div";
            output.Attributes.SetAttribute("id", prefixInputId);
            output.Attributes.SetAttribute("style", $"height: {height};");

            output.PostElement.SetHtmlContent($@"
            <script>
                var quill_{prefixInputId} = new Quill('#{prefixInputId}', {{
                    theme: 'snow',
                    modules: {{
                        toolbar: [{toolbarOptions}],
                        imageDrop: {allowImages},
                    }},
                    readOnly: false
                }});

                quill_{prefixInputId}.on('text-change', function(delta, oldDelta, source) {{
                    var text = quill_{prefixInputId}.getText();
                    if (text.length > {maxLength}) {{
                        quill_{prefixInputId}.deleteText({maxLength}, text.length);
                    }}
                    document.querySelector('input[name=""{prefixInputName}""]').value = quill_{prefixInputId}.root.innerHTML;
                }});

                // Gestione del copia-incolla
                if ({allowCopyPaste} === false) {{
                    quill_{prefixInputId}.root.addEventListener('copy', function(e) {{
                        e.preventDefault();
                    }});
                    quill_{prefixInputId}.root.addEventListener('paste', function(e) {{
                        e.preventDefault();
                    }});
                    quill_{prefixInputId}.root.addEventListener('cut', function(e) {{
                        e.preventDefault();
                    }});
                }}

                quill_{prefixInputId}.root.innerHTML = `{value}`;
            </script>
            <input type='hidden' name='{prefixInputName}' value='{value}' />
            ");
        }
    }



    //*****************************************************************************************************************************************************
    //
    // PROPRIETA' CON DIFFERENTI ATTRIBUTI DataType
    //
    // TagHelper generico che si applica a tutti gli elementi input, indipendentemente dal tipo di DataType.
    // Questo TagHelper può essere configurato per modificare l'output dell'elemento HTML in base ai valori di Visible e Readonly.

    //usage
    //DataType(DataType.Text)] , [DataType(DataType.Date)] , [DataType(DataType.Time)] , [DataType(DataType.EmailAddress)] , [DataType(DataType.PhoneNumber)] , [DataType(DataType.Text)] , [DataType(DataType.Currency)] , [DataType(DataType.Duration)] e [DataType(DataType.MultilineText)]
    //[Required]
    //public string? EpNote  { get; set; }


    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class GenericDataTypeTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var containerType = For.ModelExplorer.Metadata.ContainerType; //For.ModelExplorer.Container?.ModelType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            if (property == null) { output.SuppressOutput(); return; }  // oppure log errore
            //??//var property = For.Metadata.ContainerType.GetProperty(For.Name);

            var attributeErpDogField = property.GetCustomAttributes(typeof(ErpDogFieldAttribute), false).FirstOrDefault() as ErpDogFieldAttribute;
            var attributeErpDogField_Xref = attributeErpDogField?.Xref ?? "";

            if (property != null)
            {

                //-------------------------------------
                //calcola prefix id name (Accedi al valore di HtmlFieldPrefix)
                //-------------------------------------
                ////var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
                ////var prefixInputId = (prefix != "") ? prefix + "_" + For.Name : For.Name;
                ////var prefixInputName = (prefix != "") ? prefix + "." + For.Name : For.Name;
                var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
                var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
                var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");
                //-------------------------------------
                //calcola restrizioni visibilità pagina
                //-------------------------------------
                DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, attributeErpDogField_Xref, ViewContext);
                //-------------------------------------

                var dataTypeAttribute = property.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;

                if (dataTypeAttribute != null)
                {
                    if (attrField.Visible == 'N')
                    {
                        // Nascondi il controllo se Visible è "N"
                        output.SuppressOutput();
                    }
                    else
                    {
                        // Imposta il tipo di input HTML in base a DataType
                        string inputType = dataTypeAttribute.DataType switch
                        {
                            DataType.Text => "text",
                            DataType.Date => "date",
                            DataType.Time => "time",
                            DataType.DateTime => "date",
                            DataType.EmailAddress => "email",
                            DataType.PhoneNumber => "tel",
                            DataType.Currency => "text", // Non c'è un tipo specifico per la valuta in HTML5
                            DataType.Duration => "text", // Puoi personalizzare questo a seconda delle esigenze
                            DataType.MultilineText => "textarea", // Per i multiline, utilizzeremo un <textarea>
                            _ => "text" // Default
                        };

                        output.Attributes.SetAttribute("type", inputType);

                        if (attrField.Readonly == 'Y')
                        {
                            // Imposta l'attributo readonly se necessario
                            output.Attributes.SetAttribute("readonly", "readonly");
                        }

                        // Gestione speciale per textarea (multiline text), ecc..
                        if (inputType == "textarea")
                        {
                            output.TagName = "textarea";
                            output.Content.SetContent(For.Model?.ToString() ?? "");
                            output.Attributes.RemoveAll("type"); // Rimuove il tipo poiché non è necessario per <textarea>
                        }
                    }
                }
            }
        }
    }
    //*****************************************************************************************************************************************************
    //*****************************************************************************************************************************************************


    //    <erp-table asp-for="XrefPcIdPrestazione"
    //           allow-add="true"
    //           allow-edit="true"
    //           allow-delete="true">

    //    <erp-table-columns>
    //        <erp-table-col for="PcIdCampione" label="Campione" />
    //        <erp-table-col for="PcIdTipoCampione" label="Tipo Campione" />
    //        <erp-table-col for="PcTipo" label="Tipo" />
    //        <erp-table-col for="PcNote" label="Note" />
    //    </erp-table-columns>

    //    <erp-table-edit-columns>
    //        <erp-table-edit-col for="PcIdCampione">
    //            <input asp-for="XrefPcIdPrestazione[0].PcIdCampione" class="form-control" />
    //        </erp-table-edit-col>

    //        <erp-table-edit-col for="PcIdTipoCampione">
    //            <select asp-for="XrefPcIdPrestazione[0].PcIdTipoCampione"
    //                    asp-items="Model.TipiCampione" class="form-select">
    //            </select>
    //        </erp-table-edit-col>

    //        <erp-table-edit-col for="PcTipo">
    //            <input asp-for="XrefPcIdPrestazione[0].PcTipo" class="form-control" />
    //        </erp-table-edit-col>

    //        <erp-table-edit-col for="PcNote">
    //            <textarea asp-for="XrefPcIdPrestazione[0].PcNote" 
    //                      class="form-control"></textarea>
    //        </erp-table-edit-col>
    //    </erp-table-edit-columns>

    //    <erp-table-actions mode = "all" />
    //</ erp - table >


    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ErpTableAttribute : Attribute
    {
        public string PartitionSqlFieldName { get; set; } = "";
        public string PartitionValue { get; set; } = "";
        public string Options { get; set; } = "";
        public ErpTableAttribute() { }
    }


    [HtmlTargetElement("erp-table", Attributes = "asp-for")]
    public class ErpTableTagHelper : TagHelper
    {
        // --- Attributi comuni ---
        [HtmlAttributeName("asp-readonly")] public char? Readonly { get; set; }
        [HtmlAttributeName("asp-visible")] public char? Visible { get; set; }

        [ViewContext][HtmlAttributeNotBound] public ViewContext ViewContext { get; set; }
        [HtmlAttributeName("asp-for")] public ModelExpression For { get; set; }

        [HtmlAttributeName("allow-add")] public bool AllowAdd { get; set; } = true;
        [HtmlAttributeName("allow-edit")] public bool AllowEdit { get; set; } = true;
        [HtmlAttributeName("allow-delete")] public bool AllowDelete { get; set; } = true;
        [HtmlAttributeName("allow-undelete")] public bool AllowUndelete { get; set; } = false;
        [HtmlAttributeName("maxLine")] public int? MaxLine { get; set; }  // es: 10
        [HtmlAttributeName("editFilter")] public bool? EditFilter { get; set; }  // on/off

        // --- Metadati colonne e contenuti preparati dai figli ---
        public class ColumnDef
        {
            public string For { get; set; }
            public string Label { get; set; }
            public bool Visible { get; set; } = true;
            public string DefaultValue { get; set; } = null;

            // Ordinamento
            public string Sort { get; set; } = "none";            // none|asc|desc|asc1...desc3
            public string ExclusiveSort { get; set; } = "none";   // idem
            public string TypeSort { get; set; } = "string";   // "string" | "autocomplete" | "number" | "date" | "time" | "datetime"
            public bool SortSpecified { get; set; } = false;    // true se l'attributo "sort" è presente nel tag (anche se "none")
            public bool ExclusiveSortSpecified { get; set; } = false;
            public string SortType { get; set; } = "string";

            // Filtro
            public bool Filterable { get; set; } = false;       // se true, partecipa al filtro client
            // Mandatory
            //mm//public bool Mandatory { get; set; } = false;       // se true, partecipa il campo non può essere vuoto
        }
        [HtmlAttributeNotBound] public List<ColumnDef> ColumnDefinitions { get; set; } = new();
        [HtmlAttributeNotBound] public TagHelperContent ColumnsContent { get; set; } = new DefaultTagHelperContent();
        [HtmlAttributeNotBound] public TagHelperContent ActionsContent { get; set; }
        [HtmlAttributeNotBound] public Dictionary<string, string> EditTemplates { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        // (opzionale) se vuoi ancora esporre il blocco raw
        [HtmlAttributeNotBound] public TagHelperContent EditColumnsContent { get; set; }

        public override void Init(TagHelperContext context)
        {
            // Espone se stesso ai figli (colonne e editor)
            context.Items[typeof(ErpTableTagHelper)] = this;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // --- Dati riflessioni / attributi modello ---
            var containerType = For.ModelExplorer.Metadata.ContainerType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            var attrErpTable = property?.GetCustomAttributes(typeof(ErpTableAttribute), false).FirstOrDefault() as ErpTableAttribute;

            // --- Prefix per i name/id degli input ---
            var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");

            // --- Visibilità/readonly dal tuo sistema ---
            DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, "", ViewContext);
            char readonlyFlag = Readonly ?? attrField.Readonly;
            char visibleFlag = Visible ?? attrField.Visible;

            // === Dizionario sottostante ===
            var dict = For.Model as IDictionary;
            if (dict == null)
            {
                var keyType = For.ModelExplorer.ModelType.GetGenericArguments().FirstOrDefault() ?? typeof(string);
                var valueType = For.ModelExplorer.ModelType.GetGenericArguments().Skip(1).FirstOrDefault() ?? typeof(object);
                var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                dict = (IDictionary)Activator.CreateInstance(dictType);
            }
            var elementType = For.ModelExplorer.ModelType.GetGenericArguments().Skip(1).FirstOrDefault() ?? typeof(object);

            // Entry fittizia "[0]" solo per la template-row
            bool placeholderInserted = false;
            const string TEMPLATE_KEY = "0";
            if (!dict.Contains(TEMPLATE_KEY))
            {
                var fakeItem = Activator.CreateInstance(elementType);
                dict[TEMPLATE_KEY] = fakeItem;
                placeholderInserted = true;
            }

            // ⚙️ Esegui i figli (colonne + editor) per popolare ColumnDefinitions/EditTemplates
            await output.GetChildContentAsync();

            if (placeholderInserted) dict.Remove(TEMPLATE_KEY);

            // --- Individua nomi speciali ---
            string[] sysTokens = { "[SYS]", "[DEL]", "[TMS]", "[CDATE]", "[CTIME]", "[CAGENT]", "[CUNIT]", "[MDATE]", "[MTIME]", "[MAGENT]", "[MUNIT]", "[HOME]", "[VERSION]", "[INACTIVE]", "[EXTATT]" };
            string icodePropName = "", timestampPropName = "", deletedPropName = "";
            foreach (var x in UtilHelper.GetAllErpDogFields(elementType))
            {
                if (x.SqlFieldOptions.Contains("[SID]")) icodePropName = x.Prop.Name;
                else if (x.SqlFieldOptions.Contains("[TMS]")) timestampPropName = x.Prop.Name;
                else if (x.SqlFieldOptions.Contains("[DEL]")) deletedPropName = x.Prop.Name;
            }
            var hiddenFieldNames = new[] { "action", "vars", icodePropName, timestampPropName, deletedPropName }
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            // === Tabella wrapper ===
            output.TagName = "table";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "table table-striped erptbl");
            output.Attributes.SetAttribute("data-prefix", prefixInputName);
            output.Attributes.SetAttribute("icodePropName", icodePropName);
            output.Attributes.SetAttribute("timestampPropName", timestampPropName);
            output.Attributes.SetAttribute("deletedPropName", deletedPropName);
            output.Attributes.SetAttribute("data-allow-undelete", AllowUndelete ? "true" : "false"); // <-- nuovo
            if (MaxLine.HasValue && MaxLine.Value > 0) output.Attributes.SetAttribute("data-max-lines", MaxLine.Value.ToString());
            if (EditFilter == true) output.Attributes.SetAttribute("data-edit-filter", "true");
            output.Content.Clear();


            // AGGIUNGI QUESTE DUE RIGHE PER SCROLL ORIZONTALE SU TABELLE GRANDI
            output.PreElement.SetHtmlContent("<div class=\"table-responsive\">");
            output.PostElement.SetHtmlContent("</div>");


            // ===== HEAD =====
            var thead = new TagBuilder("thead");
            var trHead = new TagBuilder("tr");

            if (ColumnDefinitions?.Any() == true && !ColumnsContent.IsEmptyOrWhiteSpace)
            {
                trHead.InnerHtml.AppendHtml(ColumnsContent);
                if (AllowEdit || AllowDelete)
                {
                    var thA = new TagBuilder("th");

                    // Se richiesto il filtro client → aggiungi placeholder per toggle input
                    if (EditFilter == true)
                    {
                        // piccolo contenitore per toggle + input (gestiti interamente via JS)
                        thA.InnerHtml.AppendHtml("<div class='erpfilt-box' style='float:right;'></div>");
                    }
                    else {
                        thA.InnerHtml.Append("Azioni");
                    }

                    trHead.InnerHtml.AppendHtml(thA);
                }
            }
            else
            {
                // Fallback: auto colonne non di sistema
                foreach (var col in UtilHelper.GetAllErpDogFields(elementType, bindingFlags))
                {
                    if (hiddenFieldNames.Contains(col.Prop.Name)) continue;
                    if (sysTokens.Any(t => col.SqlFieldOptions.Contains(t))) continue;

                    var th = new TagBuilder("th");
                    th.InnerHtml.Append(col.Prop.Name);
                    trHead.InnerHtml.AppendHtml(th);
                }
                if (AllowEdit || AllowDelete)
                {
                    var thA = new TagBuilder("th");
                    thA.InnerHtml.Append("Azioni");
                    trHead.InnerHtml.AppendHtml(thA);
                }
            }
            thead.InnerHtml.AppendHtml(trHead);

            // ===== BODY =====
            var tbody = new TagBuilder("tbody");

            foreach (DictionaryEntry entry in dict)
            {
                var rowKey = entry.Key?.ToString() ?? "";
                var item = entry.Value;
                var tr = new TagBuilder("tr");
                tr.Attributes["data-new-key"] = rowKey;

                if (ColumnDefinitions != null && ColumnDefinitions.Any())
                {
                    // Colonne visibili (in ordine definito)
                    foreach (var colDef in ColumnDefinitions.Where(c => c.Visible))
                    {
                        var td = new TagBuilder("td");
                        td.InnerHtml.AppendHtml(
                            BuildHiddenInput(item, elementType, colDef.For, colDef.DefaultValue, "", prefixInputName, rowKey,
                                             icodePropName, timestampPropName, deletedPropName)
                        );
                        tr.InnerHtml.AppendHtml(td);
                    }

                    // Colonna nascosta con hidden di servizio (action/vars/SID/TMS/DEL) che non siano già visibili
                    var tdHidden = new TagBuilder("td");
                    tdHidden.Attributes["style"] = "display:none";

                    // Colonna nascosta con hidden: colonne non visibili
                    foreach (var colDef in ColumnDefinitions.Where(c => !c.Visible))
                    {
                        tdHidden.InnerHtml.AppendHtml(
                            BuildHiddenInput(item, elementType, colDef.For, colDef.DefaultValue, "DefaultValue", prefixInputName, rowKey,
                                             icodePropName, timestampPropName, deletedPropName)
                        );
                    }
                    // Colonna nascosta con hidden: di servizio
                    var visibleProps = ColumnDefinitions.Select(c => c.For).ToHashSet();  //var visibleProps = ColumnDefinitions.Where(c => c.Visible).Select(c => c.For).ToHashSet();
                    foreach (var fieldName in hiddenFieldNames)
                    {
                        if (visibleProps.Contains(fieldName)) continue;
                        tdHidden.InnerHtml.AppendHtml(
                            BuildHiddenInput(item, elementType, fieldName, "", "", prefixInputName, rowKey,
                                             icodePropName, timestampPropName, deletedPropName)
                        );
                    }

                    tr.InnerHtml.AppendHtml(tdHidden);
                }
                else
                {
                    // Fallback: tutte le proprietà non di sistema
                    foreach (var col in UtilHelper.GetAllErpDogFields(elementType, bindingFlags))
                    {
                        if (hiddenFieldNames.Contains(col.Prop.Name)) continue;
                        if (sysTokens.Any(t => col.SqlFieldOptions.Contains(t))) continue;

                        var td = new TagBuilder("td");
                        td.InnerHtml.AppendHtml(
                            BuildHiddenInput(item, elementType, col.Prop.Name, "", "", prefixInputName, rowKey,
                                             icodePropName, timestampPropName, deletedPropName)
                        );
                        tr.InnerHtml.AppendHtml(td);
                    }

                    // Hidden di servizio
                    var tdHidden = new TagBuilder("td");
                    tdHidden.Attributes["style"] = "display:none";
                    foreach (var fieldName in hiddenFieldNames)
                        tdHidden.InnerHtml.AppendHtml(
                            BuildHiddenInput(item, elementType, fieldName, "", "", prefixInputName, rowKey,
                                             icodePropName, timestampPropName, deletedPropName)
                        );
                    tr.InnerHtml.AppendHtml(tdHidden);
                }


                // Azione dell'elemento (se presente)
                var itemAction = elementType.GetProperty("action")?.GetValue(item)?.ToString();
                if (string.Equals(itemAction, "D", StringComparison.OrdinalIgnoreCase))
                {
                    // Evidenzia la riga eliminata (verde)
                    tr.AddCssClass("erptbl-deleted");
                }
                // Azioni
                if (AllowEdit || AllowDelete || AllowUndelete)
                {
                    var tdActions = new TagBuilder("td");
                    tdActions.AddCssClass("action-buttons text-center");

                    // Riga in stato "D" e undelete abilitato → mostra solo "undelete"
                    if (AllowUndelete && string.Equals(itemAction, "D", StringComparison.OrdinalIgnoreCase))
                    {
                        tdActions.InnerHtml.AppendHtml(
                            "<button type='button' class='btn btn-sm btn-outline-success erptbl-undelete' title='Ripristina'><i class='bi bi-arrow-counterclockwise'></i></button>"
                        );
                    }
                    else
                    {
                        if (AllowEdit)
                            tdActions.InnerHtml.AppendHtml("<button type='button' class='btn btn-sm btn-outline-primary erptbl-modify'><i class='bi bi-pencil-square'></i></button> ");
                        if (AllowDelete)
                            tdActions.InnerHtml.AppendHtml("<button type='button' class='btn btn-sm btn-outline-danger erptbl-delete'><i class='bi bi-trash3'></i></button> ");
                    }

                    tr.InnerHtml.AppendHtml(tdActions);
                }


                tbody.InnerHtml.AppendHtml(tr);
            }

            // ===== TEMPLATE (editor row) =====
            var tpl = new TagBuilder("tr");
            tpl.Attributes["id"] = $"tpl_{prefixInputId}";
            tpl.Attributes["data-template"] = "1";
            tpl.Attributes["data-edit-key"] = "";
            tpl.Attributes["style"] = "display:none";

            if (ColumnDefinitions != null && ColumnDefinitions.Any())
            {
                ////////// Colonne in ordine, rispettando Visible/Hidden

                // Colonne visibili (in ordine definito)
                foreach (var colDef in ColumnDefinitions.Where(c => c.Visible))
                {
                    var td = new TagBuilder("td");

                    // ⬇⬇⬇ AGGIUNGI QUESTA RIGA
                    //mm//if (colDef.Mandatory) td.Attributes["data-mandatory"] = "true";

                    // Usa il template custom se presente, altrimenti fallback
                    if (EditTemplates.TryGetValue(colDef.For, out var htmlTemplate) && !string.IsNullOrWhiteSpace(htmlTemplate))
                    {
                        td.InnerHtml.AppendHtml(htmlTemplate);
                    }
                    else
                    {
                        // Fallback editor semplice con default (se definito)
                        td.InnerHtml.AppendHtml($@"
                                                <div class='form-group'>
                                                  <input type='text' name='{prefixInputName}[0].{colDef.For}' value='{(colDef.DefaultValue ?? "")}' class='form-control' />
                                                  <span class='text-danger' data-valmsg-for='{prefixInputName}[0].{colDef.For}' data-valmsg-replace='true'></span>
                                                </div>");
                    }

                    tpl.InnerHtml.AppendHtml(td);
                }

                // Colonna nascosta con hidden di servizio (action/vars/SID/TMS/DEL) che non siano già visibili
                var tdHidden = new TagBuilder("td");
                tdHidden.Attributes["style"] = "display:none";

                // Colonna nascosta con hidden: colonne non visibili
                foreach (var colDef in ColumnDefinitions.Where(c => !c.Visible))
                {
                    var emptyItem = Activator.CreateInstance(elementType);
                    tdHidden.InnerHtml.AppendHtml(
                        BuildHiddenInput(emptyItem, elementType, colDef.For, colDef.DefaultValue, "DefaultValue", prefixInputName, "0",
                                         icodePropName, timestampPropName, deletedPropName)
                    );
                }
                // Colonna nascosta con hidden: di servizio
                var visibleProps = ColumnDefinitions.Select(c => c.For).ToHashSet();  //var visibleProps = ColumnDefinitions.Where(c => c.Visible).Select(c => c.For).ToHashSet();
                foreach (var fieldName in hiddenFieldNames)
                {
                    if (visibleProps.Contains(fieldName)) continue;
                    var emptyItem = Activator.CreateInstance(elementType);
                    tdHidden.InnerHtml.AppendHtml(
                        BuildHiddenInput(emptyItem, elementType, fieldName, "", "", prefixInputName, "0",
                                         icodePropName, timestampPropName, deletedPropName)
                    );
                }

                tpl.InnerHtml.AppendHtml(tdHidden);
            }
            else
            {
                // Fallback template: tutte le proprietà non di sistema
                foreach (var col in UtilHelper.GetAllErpDogFields(elementType, bindingFlags))
                {
                    if (hiddenFieldNames.Contains(col.Prop.Name)) continue;
                    if (sysTokens.Any(t => col.SqlFieldOptions.Contains(t))) continue;

                    var td = new TagBuilder("td");

                    td.InnerHtml.AppendHtml($@"
                                    <div class='form-group'>
                                      <input name='{prefixInputName}[0].{col.Prop.Name}' class='form-control' />
                                      <span class='text-danger' data-valmsg-for='{prefixInputName}[0].{col.Prop.Name}' data-valmsg-replace='true'></span>
                                    </div>");
                    tpl.InnerHtml.AppendHtml(td);
                }

                // Hidden di servizio
                var tdHidden = new TagBuilder("td");
                tdHidden.Attributes["style"] = "display:none";
                foreach (var fieldName in hiddenFieldNames)
                {
                    var emptyItem = Activator.CreateInstance(elementType);
                    tdHidden.InnerHtml.AppendHtml(
                        BuildHiddenInput(emptyItem, elementType, fieldName, "", "", prefixInputName, "0",
                                         icodePropName, timestampPropName, deletedPropName)
                    );
                }
                tpl.InnerHtml.AppendHtml(tdHidden);
            }

            // Azioni template
            if (AllowEdit || AllowAdd)
            {
                var tdTplActions = new TagBuilder("td");
                tdTplActions.InnerHtml.AppendHtml("<button type='button' class='btn btn-sm btn-success erptbl-save'><i class='bi bi-save'></i></button> <button type='button' class='btn btn-sm btn-secondary erptbl-cancel'><i class='bi bi-x-circle'></i></button>");  //tdTplActions.InnerHtml.AppendHtml("<button type='button' class='btn btn-success erptbl-save'>💾</button> <button type='button' class='btn btn-secondary erptbl-cancel'>✖</button>");
                tpl.InnerHtml.AppendHtml(tdTplActions);
            }

            tbody.InnerHtml.AppendHtml(tpl);

            // ===== FOOTER =====

            // FOOTER (pager + add) --------------------------------------------------------
            var tfoot = new TagBuilder("tfoot");

            // Creiamo la cella contenitore
            var tdFoot = new TagBuilder("td");
            tdFoot.Attributes["colspan"] = "99";
            tdFoot.Attributes["style"] = "position:relative; padding-top:8px; padding-bottom:8px;";

            // --------------------------
            // Bottone "Aggiungi riga"
            // --------------------------
            if (AllowAdd)
            {
                //btn.type = 'button';
                //btn.className = 'btn btn-sm btn-success mb-2 AddRow';
                //btn.innerHTML = '<i class="bi bi-plus-lg"></i> Aggiungi riga';
                tdFoot.InnerHtml.AppendHtml($@"
                            <button type='button' class='btn btn-sm btn-success mb-2 AddRow' 
                                    data-prefix='{prefixInputName}'>
                                <i class='bi bi-plus-lg'></i> Aggiungi riga
                            </button>");
            }

            // --------------------------
            // Contenitore Paginatore (vuoto)
            // → Lo JS lo riconosce come .erppager-box
            // → Verrà popolato solo se data-max-lines è presente
            // --------------------------
            tdFoot.InnerHtml.AppendHtml(@"
                        <div class='erppager-box' 
                             style='position:absolute; right:0; top:0;'>
                        </div>");

            // Aggiungi TR
            var trFoot = new TagBuilder("tr");
            trFoot.InnerHtml.AppendHtml(tdFoot);
            // Aggiungi al TFOOT
            tfoot.InnerHtml.AppendHtml(trFoot);



            //---------------------------------------------
            output.Content.AppendHtml(thead);
            output.Content.AppendHtml(tbody);
            output.Content.AppendHtml(tfoot);
        }

        // === Helper per creare hidden + span display (coerente con il tuo impianto) ===
        private TagBuilder BuildHiddenInput(
            object item,
            Type elementType,
            string fieldName,
            string fieldDefaultValue,
            string fieldDefaultClass,
            string prefixInputName,
            string key,
            string icodePropName,
            string timestampPropName,
            string deletedPropName)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            var div = new TagBuilder("div");
            div.AddCssClass("form-group");

            var prop = elementType.GetProperty(fieldName);
            if (prop == null) return div;

            var fullName = $"{prefixInputName}[{key}].{prop.Name}";
            var id = fullName.Replace(".", "_").Replace("[", "_").Replace("]", "_");
            var value = prop.GetValue(item);

            var input = new TagBuilder("input");
            input.Attributes["type"] = "hidden";
            input.Attributes["name"] = fullName;
            input.Attributes["id"] = id;

            if (prop.Name == "vars" && value is IDictionary<string, string> varsDict)
            {
                input.Attributes["value"] = DogManager.JsonSafeSerializeToBase64Url(varsDict);
                input.AddCssClass("ModelVars");
            }
            else if (prop.Name == "action")
            {
                input.Attributes["value"] = value?.ToString() ?? "";
                if (input.Attributes["value"] == "D") { input.AddCssClass("ModelAction tbl-deleted"); } // Evidenzia la riga eliminata (rosso + sbarrato via CSS)
                else { input.AddCssClass("ModelAction"); }
            }
            else if (prop.Name == timestampPropName && value is byte[] byteArray)
            {
                input.Attributes["value"] = UtilHelper.ByteArrayToHexString(byteArray);
                input.AddCssClass("ModelTimestamp");
            }
            else if (prop.Name == icodePropName)
            {
                input.Attributes["value"] = value?.ToString() ?? "";
                input.AddCssClass("ModelIcode");
            }
            else if (prop.Name == deletedPropName)
            {
                input.Attributes["value"] = value?.ToString() ?? "";
                input.AddCssClass("ModelDeleted");
            }
            else
            {
                if (!string.IsNullOrEmpty(fieldDefaultValue)) { input.Attributes["value"] = fieldDefaultValue; }
                else { input.Attributes["value"] = value?.ToString() ?? ""; }

                if (!string.IsNullOrEmpty(fieldDefaultClass)) { input.AddCssClass(fieldDefaultClass); }

            }

            div.InnerHtml.AppendHtml(input);

            // Display label-friendly (se disponibile)
            string displayValue = value?.ToString() ?? "";
            var getLabelMethod = elementType.GetMethod("getLabelForField", bindingFlags);
            if (getLabelMethod != null)
            {
                try
                {
                    var label = getLabelMethod.Invoke(item, new object[] { prop.Name });
                    if (label != null && !string.IsNullOrEmpty(label.ToString()))
                        displayValue = label.ToString();
                }
                catch { /* fallback raw value */ }
            }

            var span = new TagBuilder("span");
            span.Attributes["data-field"] = prop.Name;
            span.Attributes["data-row-key"] = key;
            span.InnerHtml.AppendHtml(displayValue);
            div.InnerHtml.AppendHtml(span);

            var spanVal = new TagBuilder("span");
            spanVal.AddCssClass("text-danger");
            spanVal.Attributes["data-valmsg-for"] = fullName;
            spanVal.Attributes["data-valmsg-replace"] = "true";
            div.InnerHtml.AppendHtml(spanVal);

            return div;
        }
    }
    [HtmlTargetElement("erp-table-columns", ParentTag = "erp-table")]
    public class ErpTableColumnsTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Esegui i figli (<erp-table-col>) che scriveranno direttamente nel padre
            await output.GetChildContentAsync();

            var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
            if (table?.ColumnDefinitions != null && table.ColumnDefinitions.Any())
            {
                var header = new DefaultTagHelperContent();
                foreach (var col in table.ColumnDefinitions.Where(c => c.Visible))
                {
                    var th = new TagBuilder("th");
                    th.InnerHtml.Append(col.Label ?? col.For);

                    // Data attributes per JS
                    th.Attributes["data-col-for"] = col.For;
                    th.Attributes["data-sort-mode"] = col.ExclusiveSortSpecified ? "exclusive"
                                                       : (col.SortSpecified ? "multi" : "none");
                    th.Attributes["data-sort-initial"] = col.ExclusiveSortSpecified ? col.ExclusiveSort
                                                           : (col.SortSpecified ? col.Sort : "none");
                    th.Attributes["data-sort-type"] = col.SortType;
                    th.Attributes["data-filterable"] = col.Filterable ? "true" : "false";
                    //mm//th.Attributes["data-mandatory"] = col.Mandatory ? "true" : "false";

                    header.AppendHtml(th);
                }
                table.ColumnsContent = header;
            }

            output.SuppressOutput();
        }
    }

    [HtmlTargetElement("erp-table-col", ParentTag = "erp-table-columns")]
    public class ErpTableColTagHelper : TagHelper
    {
        [HtmlAttributeName("for")] public string For { get; set; }
        [HtmlAttributeName("label")] public string Label { get; set; }
        [HtmlAttributeName("visible")] public bool? Visible { get; set; }
        [HtmlAttributeName("value")] public string DefaultValue { get; set; }

        // sorting
        [HtmlAttributeName("sort")] public string Sort { get; set; } = "none";
        [HtmlAttributeName("exclusiveSort")] public string ExclusiveSort { get; set; } = "none";
        [HtmlAttributeName("sortType")] public string SortType { get; set; } = "string";

        // filtro
        [HtmlAttributeName("filter")] public bool? Filter { get; set; }
        //mandatory
        //mm//[HtmlAttributeName("mandatory")] public bool Mandatory { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
            if (table != null)
            {
                bool sortSpecified = context.AllAttributes.ContainsName("sort");
                bool exclusiveSpecified = context.AllAttributes.ContainsName("exclusiveSort");

                table.ColumnDefinitions ??= new List<ErpTableTagHelper.ColumnDef>();
                table.ColumnDefinitions.Add(new ErpTableTagHelper.ColumnDef
                {
                    For = For,
                    Label = Label ?? For,
                    Visible = Visible ?? true,
                    DefaultValue = DefaultValue,
                    Sort = (Sort ?? "none").Trim(),
                    ExclusiveSort = (ExclusiveSort ?? "none").Trim(),
                    SortSpecified = sortSpecified,
                    ExclusiveSortSpecified = exclusiveSpecified,
                    SortType = SortType ?? "string",
                    Filterable = Filter == true   //mm//,
                    //mm//Mandatory = Mandatory == true
                });
            }
            output.SuppressOutput();
        }
    }

    [HtmlTargetElement("erp-table-edit-columns", ParentTag = "erp-table")]
    public class ErpTableEditColumnsTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Esegue i figli (<erp-table-edit-col>) che popoleranno table.EditTemplates
            await output.GetChildContentAsync();
            output.SuppressOutput(); // il padre costruirà le <td>
        }
    }

    [HtmlTargetElement("erp-table-edit-col", ParentTag = "erp-table-edit-columns")]
    public class ErpTableEditColTagHelper : TagHelper
    {
        [HtmlAttributeName("for")] public string For { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
            var content = await output.GetChildContentAsync();
            var html = content.GetContent();

            if (table != null && !string.IsNullOrWhiteSpace(For))
            {
                table.EditTemplates[For] = html;
                // (opzionale) disponibilità raw
                table.EditColumnsContent ??= new DefaultTagHelperContent();
                table.EditColumnsContent.AppendHtml($"<!--template:{For}-->");
            }

            output.SuppressOutput();
        }
    }

    [HtmlTargetElement("erp-table-actions", ParentTag = "erp-table")]
    public class ErpTableActionsTagHelper : TagHelper
    {
        public string Mode { get; set; } = "all"; // add|edit|delete|all|none

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
            var html = new System.Text.StringBuilder();

            if (Mode is "edit" or "all")
                html.Append("<button type='button' class='btn btn-sm btn-outline-primary erptbl-modify'><i class='bi bi-pencil-square'></i></button> "); //html.Append("<button type='button' class='btn btn-primary erptbl-modify'>✏</button> ");
            if (Mode is "delete" or "all")
                html.Append("<button type='button' class='btn btn-sm btn-outline-danger erptbl-delete'><i class='bi bi-trash3'></i></button> "); //html.Append("<button type='button' class='btn btn-danger erptbl-delete'>🗑</button> ");

            if (table != null)
                table.ActionsContent = new DefaultTagHelperContent().AppendHtml(html.ToString());

            output.SuppressOutput();
        }
    }


    // ===================================================================
    // NOTA: I tag <erp-table-col> e <erp-table-edit-col> vengono processati
    // direttamente tramite parsing HTML nei loro parent TagHelper
    // Non servono TagHelper separati per questi elementi
    // ===================================================================



    [HtmlTargetElement("erp-speech-to-text", Attributes = "asp-for")]
    public class ErpSpeechToText : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        // Pass-through optional attributes (class, rows, placeholder, etc.)
        [HtmlAttributeName("class")]
        public string CssClass { get; set; } = "form-control";

        [HtmlAttributeName("rows")]
        public int Rows { get; set; } = 4;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var fullId = TagBuilder.CreateSanitizedId(fullName, "_");

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "textarea-microfono-wrapper position-relative");

            // TEXTAREA
            string textarea = $@"
                                <textarea id=""{fullId}"" 
                                        name=""{fullName}""
                                        class=""{CssClass}"" 
                                        rows=""{Rows}"">{For.Model}</textarea>";

            // BOTTONE MICROFONO
            string micBtn = $@"
                                <button type=""button""
                                        class=""etk-mic-btn""
                                        data-target=""{fullId}""
                                        title=""Dettatura vocale"">
                                    <i class=""bi bi-mic-fill""></i>
                                </button>";
            output.Content.SetHtmlContent(textarea + micBtn);
        }
    }

    

    [HtmlTargetElement("erp-document-viewer")]
    public class ErpDocumentViewerTagHelper : TagHelper
    {
        public string ControllerName { get; set; }
        //public string BlobTable { get; set; }
        public string BlobId { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public int Height { get; set; } = 500;
        public bool Controls { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var fileExtension = MimeMapping.GetDefaultExtensionFromMime(ContentType);
            //var src = $"/StatoRichieste/ViewBlob/{BlobTable}/{fileExtension}/{BlobId}";
            //var src = $"/StatoRichieste/ViewBlob";
            var src = $"/{ControllerName}/ViewXdata?icode={Uri.EscapeDataString(BlobId)}";
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "erp-docviewer");

            if (ContentType.StartsWith("image/"))
            {
                output.Content.SetHtmlContent($"""
                    <img src="{src}" class="erp-doc-image" />
                """);
            }
            else if (ContentType == "application/pdf")
            {
                output.Content.SetHtmlContent($"""
                    <object data="{src}"
                            type="application/pdf"
                            width="100%"
                            height="{Height}">
                    </object>
                """);
            }
            else if (ContentType.StartsWith("audio/"))
            {
                output.Content.SetHtmlContent($"""
                    <audio src="{src}" controls />
                """);
            }
            else if (ContentType.StartsWith("video/"))
            {
                output.Content.SetHtmlContent($"""
                    <video src="{src}" controls style="max-width:100%;height:{Height}px"></video>
                """);
            }
            else if (ContentType.StartsWith("text/"))
            {
                output.Content.SetHtmlContent($"""
                    <iframe src="{src}"
                            class="erp-doc-text"
                            style="width:100%;height:{Height}px">
                    </iframe>
                """);
            }
            else
            {
                output.Content.SetHtmlContent("""
                    <div class="erp-doc-unsupported">
                        Formato non supportato
                    </div>
                """);
            }
        }
    }



    //[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    //public sealed class ErpDocumentContainerAttribute : Attribute
    //{
    //    public string xxxxPartitionSqlFieldName { get; set; } = "";
    //    public string xxxxxxxPartitionValue { get; set; } = "";
    //    public string Options { get; set; } = "";
    //    public ErpDocumentContainerAttribute() { }
    //}



    [HtmlTargetElement("erp-document-container")]
    public class ErpDocumentContainerTagHelper : TagHelper
    {
        // --- Attributi comuni ---
        [HtmlAttributeName("asp-readonly")] public char? Readonly { get; set; }
        [HtmlAttributeName("asp-visible")] public char? Visible { get; set; }

        [ViewContext][HtmlAttributeNotBound] public ViewContext ViewContext { get; set; }
        [HtmlAttributeName("asp-for")] public ModelExpression For { get; set; }

        //public Dictionary<object, ModelXdata>? Items { get; set; }
        public string RecordIcode { get; set; } = "";
        public string ControllerName { get; set; } = "";
        public bool AllowAdd { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowUpdate { get; set; }
        public string AddUrl { get; set; } = "";
        public string UpdateUrl { get; set; } = "";
        public string DeleteUrl { get; set; } = "";
        public string TypeSourceUrl { get; set; } = "";
        public int Height { get; set; } = 500;

        private static string ToTimestampHex(byte[]? ts)
            => ts == null || ts.Length == 0 ? "" : "0x" + BitConverter.ToString(ts).Replace("-", "");

        private string RenderViewer(string controllerName, string blobId, string contentType, int height)
        {
            var src = $"/{Uri.EscapeDataString(controllerName)}/ViewXdata?icode={Uri.EscapeDataString(blobId)}";

            if (contentType.StartsWith("image/"))
                return $"""<div class="erp-docviewer"><img src="{src}" class="erp-doc-image" style="max-width:100%;" /></div>""";

            if (contentType == "application/pdf")
                return $"""
                    <div class="erp-docviewer">
                        <object data="{src}" type="application/pdf" width="100%" height="{height}">
                            <p class="text-muted small p-2">
                                <i class="bi bi-file-earmark-pdf me-1"></i>
                                Il browser non supporta la visualizzazione PDF.
                                <a href="{src}" target="_blank" class="ms-1">Apri il file</a>
                            </p>
                        </object>
                    </div>
                """;

            if (contentType.StartsWith("audio/"))
                return $"""<div class="erp-docviewer p-2"><audio src="{src}" controls style="width:100%;"></audio></div>""";

            if (contentType.StartsWith("video/"))
                return $"""<div class="erp-docviewer"><video src="{src}" controls style="max-width:100%;height:{height}px;"></video></div>""";

            if (contentType.StartsWith("text/"))
                return $"""<div class="erp-docviewer"><iframe src="{src}" style="width:100%;height:{height}px;border:none;"></iframe></div>""";

            if (string.IsNullOrEmpty(contentType))
                return $"""
                    <div class="erp-docviewer p-3 text-center text-muted">
                        <i class="bi bi-file-earmark fs-1 d-block mb-2"></i>
                        <a href="{src}" target="_blank"><i class="bi bi-download me-1"></i> Scarica il file</a>
                    </div>
                """;

            return $"""
                <div class="erp-docviewer p-3 text-center text-muted">
                    <i class="bi bi-file-earmark-x fs-1 d-block mb-2"></i>
                    Formato non supportato (<code>{HtmlEncoder.Default.Encode(contentType)}</code>).
                    <a href="{src}" target="_blank" class="d-block mt-2"><i class="bi bi-download me-1"></i> Scarica</a>
                </div>
            """;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // --- Dati riflessioni / attributi modello ---
            var containerType = For.ModelExplorer.Metadata.ContainerType;
            var propertyName = For.ModelExplorer.Metadata.PropertyName;
            var property = containerType?.GetProperty(propertyName);
            //var attrErpDocumentContainer = property?.GetCustomAttributes(typeof(ErpDocumentContainerAttribute), false).FirstOrDefault() as ErpDocumentContainerAttribute;

            // --- Prefix per i name/id degli input ---
            var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
            var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
            var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");

            // --- Visibilità/readonly dal tuo sistema ---
            DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, "", ViewContext);
            char readonlyFlag = Readonly ?? attrField.Readonly;
            char visibleFlag = Visible ?? attrField.Visible;

            if (readonlyFlag=='Y') { AllowAdd = false; AllowDelete = false; AllowUpdate = false; }
            if (visibleFlag == 'N') { output.SuppressOutput(); return; }

        //--------------------------------------------------------------------------

            var enc = HtmlEncoder.Default;
            var uid = context.UniqueId;

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "erp-doc-container card mb-3 shadow-sm");
            output.Attributes.SetAttribute("data-record-icode", RecordIcode);
            output.Attributes.SetAttribute("data-add-url", AddUrl);
            output.Attributes.SetAttribute("data-update-url", UpdateUrl);
            output.Attributes.SetAttribute("data-delete-url", DeleteUrl);
            output.Attributes.SetAttribute("data-type-url", TypeSourceUrl);
            output.Attributes.SetAttribute("data-controller-name", ControllerName);

            var sb = new StringBuilder();

            // ── HEADER ──────────────────────────────────────────────
            sb.Append("<div class=\"card-header d-flex justify-content-between align-items-center py-2\">");
            sb.Append("<span class=\"fw-semibold\"><i class=\"bi bi-paperclip me-1\"></i> Documenti allegati</span>");
            if (AllowAdd)
            {
                sb.Append("<button type=\"button\" class=\"btn btn-sm btn-outline-primary\" data-action=\"add\">");
                sb.Append("<i class=\"bi bi-plus-lg me-1\"></i> Aggiungi</button>");
            }
            sb.Append("</div>");

            // ── BODY ─────────────────────────────────────────────────
            sb.Append("<div class=\"card-body p-3\">");

            Dictionary<object, ModelXdata>? Items = null;
            if (For.Model is Dictionary<object, ModelXdata>) { Items = For.Model as Dictionary<object, ModelXdata>; }

            if (Items == null)
            {
                sb.Append("<p class=\"text-muted mb-0 small\"><i class=\"bi bi-inbox me-1\"></i> Lista documenti non specificata.</p>");
            }
            else if (Items.Count == 0)
            {
                sb.Append("<p class=\"text-muted mb-0 small\"><i class=\"bi bi-inbox me-1\"></i> Nessun documento disponibile.</p>");
            }
            else
            {
                // ── Tab headers ──
                sb.Append("<ul class=\"nav nav-tabs mb-0\" role=\"tablist\">");
                int index = 0;
                foreach (var kvp in Items)
                {
                    var x = kvp.Value;
                    string active = index == 0 ? "active" : "";
                    string label = enc.Encode(x.Descr ?? kvp.Key.ToString() ?? $"Doc {index + 1}");
                    string tsHex = enc.Encode(ToTimestampHex(x.Timestamp));
                    string icode = enc.Encode(x.Icode.ToString());
                    string fmt = enc.Encode(x.Fmt ?? "");
                    string descr = enc.Encode(x.Descr ?? "");

                    sb.Append("<li class=\"nav-item\" role=\"presentation\">");
                    sb.Append($"<button class=\"nav-link {active}\"");
                    sb.Append($" data-bs-toggle=\"tab\"");
                    sb.Append($" data-bs-target=\"#xdata-{uid}-{index}\"");
                    sb.Append($" type=\"button\" role=\"tab\"");
                    sb.Append($" data-icode=\"{icode}\"");
                    sb.Append($" data-ts=\"{tsHex}\"");
                    sb.Append($" data-descr=\"{descr}\"");
                    sb.Append($" data-fmt=\"{fmt}\"");
                    sb.Append($">{label}</button>");
                    sb.Append("</li>");
                    index++;
                }
                sb.Append("</ul>");

                // ── Tab content ──
                // Bordo superiore rimosso sui tab-pane per fondersi con la cornice attiva
                sb.Append("<div class=\"tab-content border border-top-0 rounded-bottom mb-3\" id=\"tab-content-{uid}\">");
                index = 0;
                foreach (var kvp in Items)
                {
                    var x = kvp.Value;
                    string active = index == 0 ? "show active" : "";

                    // Il tab-pane attivo riceve la classe erp-doc-active-pane per la cornice colorata
                    sb.Append($"<div class=\"tab-pane fade {active} p-2\"");
                    sb.Append($" id=\"xdata-{uid}-{index}\"");
                    sb.Append($" role=\"tabpanel\">");

                    // Viewer
                    sb.Append(RenderViewer(ControllerName, x.Icode.ToString(), x._mimeXdatum ?? "", Height));

                    // Toolbar contestuale — chiaramente associata al documento nel riquadro
                    bool hasActions = AllowUpdate || AllowDelete;
                    if (hasActions)
                    {
                        sb.Append("<div class=\"erp-doc-toolbar d-flex align-items-center gap-2 mt-2 pt-2 border-top\">");
                        sb.Append("<span class=\"text-muted small me-auto\">");
                        sb.Append("<i class=\"bi bi-arrow-up-circle me-1\"></i>Azioni sul documento selezionato:</span>");

                        //--- Pannello Span errore delete — inizialmente nascosto
                        sb.Append("<span data-role=\"toolbar-error\" class=\"text-danger small w-100 d-none\"></span>");
                        //---

                        if (AllowUpdate)
                        {
                            string icode = enc.Encode(x.Icode.ToString());
                            string tsHex = enc.Encode(ToTimestampHex(x.Timestamp));
                            string descr = enc.Encode(x.Descr ?? "");
                            string fmt = enc.Encode(x.Fmt ?? "");

                            sb.Append("<button type=\"button\" class=\"btn btn-sm btn-outline-warning\" data-action=\"edit\"");
                            sb.Append($" data-icode=\"{icode}\"");
                            sb.Append($" data-ts=\"{tsHex}\"");
                            sb.Append($" data-descr=\"{descr}\"");
                            sb.Append($" data-fmt=\"{fmt}\">");
                            sb.Append("<i class=\"bi bi-pencil me-1\"></i> Modifica</button>");
                        }

                        if (AllowDelete)
                        {
                            string icode = enc.Encode(x.Icode.ToString());
                            string tsHex = enc.Encode(ToTimestampHex(x.Timestamp));

                            sb.Append("<button type=\"button\" class=\"btn btn-sm btn-outline-danger\" data-action=\"delete\"");
                            sb.Append($" data-icode=\"{icode}\"");
                            sb.Append($" data-ts=\"{tsHex}\">");
                            sb.Append("<i class=\"bi bi-trash me-1\"></i> Elimina</button>");
                        }

                        sb.Append("</div>"); // fine toolbar
                    }

                    sb.Append("</div>"); // fine tab-pane
                    index++;
                }
                sb.Append("</div>"); // fine tab-content
            }

            // ── Pannello ADD/EDIT unificato ──────────────────────────
            if (AllowAdd || AllowUpdate)
            {
                sb.Append("<div class=\"collapse mt-3 border rounded p-3 bg-light\" data-role=\"xdata-add-panel\">");

                // Titolo dinamico (cambia via JS tra "Carica" e "Modifica")
                sb.Append("<h6 class=\"mb-3 text-secondary\" data-role=\"panel-title\">");
                sb.Append("<i class=\"bi bi-upload me-1\"></i> Carica nuovo documento</h6>");

                sb.Append("<div data-role=\"xdata-form\">");

                // Campi hidden per icode e timestamp (vuoti in ADD, valorizzati in EDIT)
                sb.Append("<input type=\"hidden\" name=\"recordIcode\" value=\"" + enc.Encode(RecordIcode) + "\" />");
                sb.Append("<input type=\"hidden\" name=\"icode\"        data-role=\"field-icode\" value=\"\" />");
                sb.Append("<input type=\"hidden\" name=\"timestampHex\" data-role=\"field-ts\"    value=\"\" />");

                sb.Append("<div class=\"mb-2\">");
                sb.Append("<label class=\"form-label form-label-sm\">Descrizione</label>");
                sb.Append("<input type=\"text\" name=\"descrizione\" data-role=\"field-descr\"");
                sb.Append(" class=\"form-control form-control-sm\" placeholder=\"Inserire una descrizione...\" required />");
                sb.Append("</div>");

                sb.Append("<div class=\"mb-2\">");
                sb.Append("<label class=\"form-label form-label-sm\">Tipo documento</label>");
                sb.Append("<select name=\"tipo\" data-role=\"field-fmt\" class=\"form-select form-select-sm\" required></select>");
                sb.Append("</div>");

                sb.Append("<div class=\"mb-2\">");
                sb.Append("<label class=\"form-label form-label-sm\">File</label>");
                sb.Append("<input type=\"file\" name=\"file\" class=\"form-control form-control-sm\" required />");
                sb.Append("</div>");

                // --- pannello span per Errore PRIMA dei bottoni submit/annulla:
                sb.Append("<div class=\"mb-2\" data-role=\"validation-error-wrap\">");
                sb.Append("<span data-role=\"validation-error\" class=\"text-danger small d-none\"></span>");
                sb.Append("</div>");
                //----

                sb.Append("<div class=\"text-end mt-3 d-flex gap-2 justify-content-end\">");
                sb.Append("<button type=\"button\" class=\"btn btn-sm btn-outline-secondary\" data-action=\"cancel-add\">");
                sb.Append("<i class=\"bi bi-x-lg me-1\"></i> Annulla</button>");
                sb.Append("<button type=\"button\" class=\"btn btn-sm btn-primary\" data-action=\"submit-add\" data-role=\"submit-btn\">");
                sb.Append("<i class=\"bi bi-upload me-1\"></i> Carica</button>");
                sb.Append("</div>");

                sb.Append("</div>"); // fine xdata-form
                sb.Append("</div>"); // fine xdata-add-panel
            }

            sb.Append("</div>"); // fine card-body

            output.Content.SetHtmlContent(sb.ToString());
        }
    }



























    [HtmlTargetElement("erp-llama-chat")]
    public class ErpLlamaChatTagHelper : TagHelper
    {
        public string Placeholder { get; set; } = "Scrivi o parla…";
        public int Rows { get; set; } = 4;
        public string Endpoint { get; set; } = "http://localhost:8080/v1/chat/completions";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var uid = "llama_" + Guid.NewGuid().ToString("N");

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "llama-chat-wrapper");
            output.Attributes.SetAttribute("data-llama", "1");
            output.Attributes.SetAttribute("data-endpoint", Endpoint);
            output.Attributes.SetAttribute("data-session-id", Guid.NewGuid().ToString());
            output.Attributes.SetAttribute("data-tts", "0"); // ❌ default OFF

            output.Content.SetHtmlContent($@"
                <div class='llama-input'>
                    <textarea id='{uid}_prompt'
                              class='form-control'
                              rows='{Rows}'
                              placeholder='{Placeholder}'></textarea>

                    <button type='button'
                            class='etk-mic-btn'
                            data-target='{uid}_prompt'>🎤</button>

                    <button type='button'
                            class='llama-send-btn'
                            data-target='{uid}'>➤</button>
                </div>

                <label class='form-check mt-1'>
                    <input type='checkbox'
                           class='form-check-input llama-tts-toggle'
                           data-target='{uid}'>
                    🔊 Leggi la risposta
                </label>

                <pre id='{uid}_response' class='llama-response'></pre>
            ");
        }
    }





    //*****************************************************************************************************************************************************
    //*****************************************************************************************************************************************************
    //*****************************************************************************************************************************************************


    //https://blog.techdominator.com/article/using-html-helper-inside-tag-helpers.html

    public class Holder
    {
        public string Name { get; set; }
    }

    public class TemplateRendererTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private IHtmlHelper _htmlHelper;

        public TemplateRendererTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context
            , TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);

            /*
             * Create some data that are going 
             * to be passed to the view
             */
            _htmlHelper.ViewData["Name"] = "Ali";
            _htmlHelper.ViewBag.AnotherName = "Kamel";
            Holder model = new Holder { Name = "Charles Henry" };

            output.TagName = "div";
            /*
             * model is passed explicitly
             * ViewData and ViewBag are passed implicitly
             */
            output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("Template", model));
        }
    }

    [HtmlTargetElement("template-renderer-new-viewdata")]
    public class TemplateRendererWithNewViewDataTagHelper : TagHelper
    {

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private IHtmlHelper _htmlHelper;

        private IModelMetadataProvider _modelMetadataProvider;
        /*
         * This constructor requests the injection of a IModelMetadataProvider instance
         */
        public TemplateRendererWithNewViewDataTagHelper(IHtmlHelper htmlHelper,
            IModelMetadataProvider metadataProvider)
        {
            _htmlHelper = htmlHelper;
            _modelMetadataProvider = metadataProvider;
        }

        public override async Task ProcessAsync(TagHelperContext context,
            TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            // Actual instanciation of the new ViewData Dictionary
            ViewDataDictionary viewData = new ViewDataDictionary(_modelMetadataProvider, new ModelStateDictionary());

            Holder model = new Holder { Name = "Joel" };
            viewData["Name"] = "Jeff";

            output.TagName = "div";
            /*
             * model is passed explicitly
             * new ViewData instance needs to be explicitly
             */
            output.Content.SetHtmlContent(
                await _htmlHelper.PartialAsync("TemplateNewViewData", model, viewData));
        }
    }





}
