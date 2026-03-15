
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
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections;
using SharpCompress.Compressors.Xz;
using System.Text.RegularExpressions;
using Quartz;
using Google.Api;
using Newtonsoft.Json.Linq;


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

        public AutocompleteServerAttribute(string controller, string action, string preloadAction, int maxSelections = 0)
        {
            Controller = controller;
            Action = action;
            PreloadAction = preloadAction;
            MaxSelections = maxSelections;
        }
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class AutocompleteClientAttribute : Attribute
    {
        public string Controller { get; }
        public string Action { get; }
        public int MaxSelections { get; set; } = 0;

        public AutocompleteClientAttribute(string controller, string action, int maxSelections = 0)
        {
            Controller = controller;
            Action = action;
            MaxSelections = maxSelections;
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

                output.Attributes.SetAttribute("class", "autocomplete-input form-control");
                output.Attributes.SetAttribute("autocomplete", "off");
                output.Attributes.SetAttribute("data-max-selections", attributeServer.MaxSelections);
                output.Attributes.SetAttribute("data-controller", attributeServer.Controller);
                output.Attributes.SetAttribute("data-action", attributeServer.Action);
                output.Attributes.SetAttribute("data-preload-action", attributeServer.PreloadAction);
                output.Attributes.SetAttribute("data-pre-selected", preSelectedValuesJson);
                output.Attributes.SetAttribute("data-id", prefixInputId);
                output.Attributes.SetAttribute("data-name", prefixInputName);
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

                    content.AppendLine($@"
                        <div class='form-check form-switch d-inline-block mb-2'>
                            <input class='form-check-input' type='{inputType}' name='{prefixInputName}' id='{id}' value='{value}' {checkedAttr} {readonlyAttr} onchange='handleMaxSelections(""{prefixInputName}"", {maxSelections})'>
                            <label class='form-check-label' for='{id}'>{label}</label> &nbsp; &nbsp; 
                        </div>");
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
    public sealed class QuillEditorAttribute : Attribute
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

        public QuillEditorAttribute() { }
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

            var quillEditorAttr = property?.GetCustomAttribute<QuillEditorAttribute>();

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

    /**********************************************************************

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public sealed class ErpTableAttribute : Attribute
        {
            public string PartitionSqlFieldName { get; set; } = "";
            public string PartitionValue { get; set; } = "";
            public string Options { get; set; } = "";
            public ErpTableAttribute()
            {
            }
        }



        [HtmlTargetElement("erp-table", Attributes = "asp-for")]
        public class ErpTableTagHelper : TagHelper
        {
            //--- parametri asp-for eg: <input asp-for="MyProperty" asp-readonly="N" asp-visible="Y" asp-minchars="4" />
            [HtmlAttributeName("asp-readonly")]
            public char? Readonly { get; set; }

            [HtmlAttributeName("asp-visible")]
            public char? Visible { get; set; }

            [ViewContext]
            [HtmlAttributeNotBound]
            public ViewContext ViewContext { get; set; }
            //---


            [HtmlAttributeName("asp-for")]
            public ModelExpression For { get; set; }

            [HtmlAttributeName("allow-add")]
            public bool AllowAdd { get; set; } = true;

            [HtmlAttributeName("allow-edit")]
            public bool AllowEdit { get; set; } = true;

            [HtmlAttributeName("allow-delete")]
            public bool AllowDelete { get; set; } = true;

            // Questi vengono riempiti dai figli
            [HtmlAttributeNotBound]
            public TagHelperContent ColumnsContent { get; set; } = new DefaultTagHelperContent();

            [HtmlAttributeNotBound]
            public TagHelperContent EditColumnsContent { get; set; }

            [HtmlAttributeNotBound]
            public TagHelperContent ActionsContent { get; set; }

            //[HtmlAttributeNotBound]
            //public List<(string For, string Label)> ColumnDefinitions { get; set; }
            public class ColumnDef
            {
                public string For { get; set; }
                public string Label { get; set; }
                public bool Visible { get; set; } = true;
                public string DefaultValue { get; set; } = null; // supporta costanti/Razor già risolte
            }
            [HtmlAttributeNotBound]
            public List<ColumnDef> ColumnDefinitions { get; set; }

            public override void Init(TagHelperContext context)
            {
                // Mettiamo un “token” nel contesto, accessibile dai figli
                context.Items[typeof(ErpTableTagHelper)] = this;
            }


            public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
            {

                var containerType = For.ModelExplorer.Metadata.ContainerType; //For.ModelExplorer.Container?.ModelType;
                var propertyName = For.ModelExplorer.Metadata.PropertyName;
                var property = containerType?.GetProperty(propertyName);
                //xx//if (property == null) { output.SuppressOutput(); return; }  // lavoro lo stesso anche senza attributo [ErpTable(..)] specificato nel modello, perchè nel View è definito un tag specializzato <erp-table>

                var attributeErpTable = property?.GetCustomAttributes(typeof(ErpTableAttribute), false).FirstOrDefault() as ErpTableAttribute;
                var attributeErpTable_Options = attributeErpTable?.Options ?? "";

                //-------------------------------------
                //calcola prefix id name (Accedi al valore di HtmlFieldPrefix)
                //-------------------------------------
                var prefix = (ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix ?? "").Trim();
                var prefixInputName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
                var prefixInputId = TagBuilder.CreateSanitizedId(prefixInputName, "_");

                //-------------------------------------
                //calcola restrizioni visibilità pagina
                //-------------------------------------
                DogManager.FieldAttr attrField = UtilHelper.fieldAttrTagHelper(prefix, For.Name, "", ViewContext);
                char readonlyFlag = Readonly ?? attrField.Readonly;
                char visibleFlag = Visible ?? attrField.Visible;
                //-------------------------------------


                //#############################################################################################################
                //#############################################################################################################


                //============================================================================================================
                // === DICTIONARY SUPPORT ===
                var dict = For.Model as System.Collections.IDictionary;
                if (dict == null)
                {
                    // crea un dizionario vuoto compatibile
                    var keyType = For.ModelExplorer.ModelType.GetGenericArguments().FirstOrDefault()
                                  ?? typeof(string);
                    var valueType = For.ModelExplorer.ModelType.GetGenericArguments().Skip(1).FirstOrDefault()
                                  ?? typeof(object);
                    var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                    dict = (System.Collections.IDictionary)Activator.CreateInstance(dictType);
                }

                var elementType = For.ModelExplorer.ModelType.GetGenericArguments().Skip(1).FirstOrDefault()
                                  ?? typeof(object);



                // crea entry fittizia solo per il template se non esiste
                bool placeholderInserted = false;
                const string TEMPLATE_KEY = "0";
                if (!dict.Contains(TEMPLATE_KEY))
                {
                    var fakeItem = Activator.CreateInstance(elementType);
                    dict[TEMPLATE_KEY] = fakeItem;
                    placeholderInserted = true;
                }



                //============================================================================================================

                // *** PASSO CHIAVE ***: forza l’esecuzione dei figli prima di procedere
                Console.WriteLine("erp-table before");
                await output.GetChildContentAsync();
                Console.WriteLine("erp-table after");


                // rimuovo entry fittizia
                if (placeholderInserted)
                {
                    dict.Remove(TEMPLATE_KEY);
                }




                Console.WriteLine($"ColumnDefinitions: {ColumnDefinitions != null}, count: {ColumnDefinitions?.Count ?? 0}");

                string[] sys_tokens = { "[SYS]","[DEL]","[TMS]", "[CDATE]", "[CTIME]", "[CAGENT]", "[CUNIT]", "[MDATE]", "[MTIME]", "[MAGENT]", "[MUNIT]", "[HOME]", "[VERSION]", "[INACTIVE]", "[EXTATT]" };
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

                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance; //BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                //xx//var prefix = For.Name;

                output.TagName = "table";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.SetAttribute("class", "table table-striped erptbl");
                output.Attributes.SetAttribute("data-prefix", prefixInputName);
                output.Attributes.SetAttribute("icodePropName", icodePropName);
                output.Attributes.SetAttribute("timestampPropName", timestampPropName);
                output.Attributes.SetAttribute("deletedPropName", deletedPropName);
                output.Content.Clear();   // <- fondamentale per eliminare <erp-table> e i figli

                // HEAD -----------------------------------------------------------------
                var thead = new TagBuilder("thead");
                var trHead = new TagBuilder("tr");
                //if (ColumnDefinitions != null && ColumnDefinitions.Any() && ColumnsContent != null && ColumnsContent.IsEmptyOrWhiteSpace == false)
                if (ColumnDefinitions?.Any() == true && !ColumnsContent.IsEmptyOrWhiteSpace)
                {
                    trHead.InnerHtml.AppendHtml(ColumnsContent);
                    if (AllowEdit || AllowDelete)
                    {
                        var thA = new TagBuilder("th");
                        thA.InnerHtml.Append("Azioni");
                        trHead.InnerHtml.AppendHtml(thA);
                    }
                }
                else
                {
                    // fallback minimale se l’utente non ha definito <erp-table-columns>
                    foreach (var col in UtilHelper.GetAllErpDogFields(elementType, bindingFlags)) //foreach (var col in elementType.GetProperties())
                    {
                        if (hiddenFieldNames.Contains(col.Prop.Name)) continue; 
                        //if (col.SqlFieldOptions.Contains("[SYS]")) continue;    //scarto i campi di sistema
                        if (sys_tokens.Any(t => col.SqlFieldOptions.Contains(t))) continue;    //scarto i campi di sistema

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

                // BODY -----------------------------------------------------------------
                var tbody = new TagBuilder("tbody");
                foreach (System.Collections.DictionaryEntry entry in dict)
                {
                    var rowKey = entry.Key?.ToString() ?? "";       // <===== LA KEY DEL DIZIONARIO
                    var item = entry.Value;
                    var tr = new TagBuilder("tr");
                    //key//tr.Attributes["data-key"] = rowKey;
                    tr.Attributes["data-new-key"] = rowKey;

                    // Colonne visibili e nascoste
                    if (ColumnDefinitions != null && ColumnDefinitions.Any())
                    {
                        ////////// Se abbiamo definizioni di colonne, usa quelle
                        ////////foreach (var colDef in ColumnDefinitions)
                        ////////{
                        ////////    var td = new TagBuilder("td");
                        ////////    td.InnerHtml.AppendHtml(BuildHiddenInput(item, elementType, colDef.For, prefixInputName, rowKey, icodePropName, timestampPropName, deletedPropName));
                        ////////    tr.InnerHtml.AppendHtml(td);
                        ////////}

                        ////////// ---- Colonna nascosta con hidden ----
                        ////////var tdHidden = new TagBuilder("td");
                        ////////tdHidden.Attributes["style"] = "display:none";

                        ////////// Costruisci l’elenco delle proprietà usate come colonne visibili
                        ////////var visibleProps = ColumnDefinitions?.Select(c => c.For).ToHashSet() ?? new HashSet<string>();

                        ////////// Hidden di servizio
                        ////////foreach (var fieldName in hiddenFieldNames)
                        ////////{
                        ////////    if (visibleProps.Contains(fieldName)) continue;
                        ////////    tdHidden.InnerHtml.AppendHtml(BuildHiddenInput(item, elementType, fieldName, prefixInputName, rowKey, icodePropName, timestampPropName, deletedPropName));
                        ////////}
                        ////////tr.InnerHtml.AppendHtml(tdHidden);  // AGGIUNGI la colonna nascosta alla <tr>














                        // VISIBILI
                        foreach (var colDef in ColumnDefinitions.Where(c => c.Visible))
                        {
                            var td = new TagBuilder("td");
                            // Usa già la tua BuildHiddenInput per creare hidden+span “visuale” (manteniamo la compatibilità)
                            td.InnerHtml.AppendHtml(
                                BuildHiddenInput(item, elementType, colDef.For, prefixInputName, rowKey,
                                                 icodePropName, timestampPropName, deletedPropName)
                            );
                            // Se vuoi che il DefaultValue si rifletta nel display (span), puoi applicarlo qui
                            // solo se il valore corrente è nullo/vuoto. In genere è più coerente applicarlo
                            // nella template row (vedi più sotto) e lasciare intatte le righe “data”.
                            tr.InnerHtml.AppendHtml(td);
                        }

                        // HIDDEN (servizio)
                        var tdHidden = new TagBuilder("td");
                        tdHidden.Attributes["style"] = "display:none";

                        // campi speciali (già presenti nel tuo codice)
                        var visibleProps = ColumnDefinitions?.Where(c => c.Visible).Select(c => c.For).ToHashSet() ?? new HashSet<string>();
                        foreach (var fieldName in hiddenFieldNames)
                        {
                            if (visibleProps.Contains(fieldName)) continue;
                            tdHidden.InnerHtml.AppendHtml(
                                BuildHiddenInput(item, elementType, fieldName, prefixInputName, rowKey,
                                                 icodePropName, timestampPropName, deletedPropName)
                            );
                        }

                        // HIDDEN per le colonne con Visible=false
                        foreach (var colDef in ColumnDefinitions.Where(c => !c.Visible))
                        {
                            // NB: se colDef.DefaultValue != null, usala; altrimenti prendi il valore reale del model
                            var fullName = $"{prefixInputName}[{rowKey}].{colDef.For}";
                            var input = new TagBuilder("input");
                            input.Attributes["type"] = "hidden";
                            input.Attributes["name"] = fullName;

                            // valore di default (se definito) → priorità se mancano dati reali
                            if (!string.IsNullOrEmpty(colDef.DefaultValue))
                                input.Attributes["value"] = colDef.DefaultValue;
                            else
                            {
                                // leggi il valore reale dell'oggetto item
                                var prop = elementType.GetProperty(colDef.For);
                                var val = prop?.GetValue(item)?.ToString() ?? "";
                                input.Attributes["value"] = val;
                            }
                            tdHidden.InnerHtml.AppendHtml(input);

                            var spanVal = new TagBuilder("span");
                            spanVal.AddCssClass("text-danger");
                            spanVal.Attributes["data-valmsg-for"] = fullName;
                            spanVal.Attributes["data-valmsg-replace"] = "true";
                            tdHidden.InnerHtml.AppendHtml(spanVal);
                        }

                        tr.InnerHtml.AppendHtml(tdHidden);









                    }
                    else
                    {
                        // Fallback al comportamento originale
                        foreach (var col in UtilHelper.GetAllErpDogFields(elementType, bindingFlags)) //foreach (var col in elementType.GetProperties())
                        {
                            if (hiddenFieldNames.Contains(col.Prop.Name)) continue;
                            //if (col.SqlFieldOptions.Contains("[SYS]")) continue;    //scarto i campi di sistema
                            if (sys_tokens.Any(t => col.SqlFieldOptions.Contains(t))) continue;    //scarto i campi di sistema

                            var td = new TagBuilder("td");
                            td.InnerHtml.AppendHtml(BuildHiddenInput(item, elementType, col.Prop.Name, prefixInputName, rowKey, icodePropName, timestampPropName, deletedPropName));
                            tr.InnerHtml.AppendHtml(td);
                        }

                        // ---- Colonna nascosta con hidden ----
                        var tdHidden = new TagBuilder("td");
                        tdHidden.Attributes["style"] = "display:none";

                        // Hidden di servizio
                        foreach (var fieldName in hiddenFieldNames)
                        {
                            tdHidden.InnerHtml.AppendHtml(BuildHiddenInput(item, elementType, fieldName, prefixInputName, rowKey, icodePropName, timestampPropName, deletedPropName));
                        }
                        tr.InnerHtml.AppendHtml(tdHidden); // AGGIUNGI la colonna nascosta alla <tr>

                    }

                    // Colonna Actions
                    if (AllowEdit || AllowDelete)
                    {
                        var tdActions = new TagBuilder("td");

                        // pulsanti
                        if (ActionsContent is null || ActionsContent.IsEmptyOrWhiteSpace)
                        {
                            if (AllowEdit) tdActions.InnerHtml.AppendHtml("<button type='button' class='btn btn-primary erptbl-modify'>✏</button> ");
                            if (AllowDelete) tdActions.InnerHtml.AppendHtml("<button type='button' class='btn btn-danger erptbl-delete'>🗑</button> ");
                        }
                        else
                        {
                            tdActions.InnerHtml.AppendHtml(ActionsContent);
                        }

                        tr.InnerHtml.AppendHtml(tdActions);
                    }

                    tbody.InnerHtml.AppendHtml(tr);
                }

                // TEMPLATE (editor row) -----------------------------------------------
                var tpl = new TagBuilder("tr");
                tpl.Attributes["id"] = $"tpl_{prefixInputId}";
                tpl.Attributes["data-template"] = "1";         // aggiungi
                tpl.Attributes["data-edit-key"] = "";
                tpl.Attributes["style"] = "display:none";

                if (ColumnDefinitions != null && ColumnDefinitions.Any() && EditColumnsContent != null && EditColumnsContent.IsEmptyOrWhiteSpace == false)
                {
                    ////////// Se abbiamo definizioni di colonne, usa quelle
                    ////////tpl.InnerHtml.AppendHtml(EditColumnsContent);
                    //////////__KEY__//var htmlTpl = ReplaceIndex0WithKeyPlaceholder(EditColumnsContent.GetContent(), prefixInputName);
                    //////////__KEY__//tpl.InnerHtml.AppendHtml(htmlTpl);

                    ////////// ---- Colonna nascosta con hidden ----
                    ////////var tdHidden = new TagBuilder("td");
                    ////////tdHidden.Attributes["style"] = "display:none";

                    ////////// Costruisci l’elenco delle proprietà usate come colonne visibili
                    ////////var visibleProps = ColumnDefinitions?.Select(c => c.For).ToHashSet() ?? new HashSet<string>();

                    ////////// Hidden di servizio
                    ////////foreach (var fieldName in hiddenFieldNames)
                    ////////{
                    ////////    if (visibleProps.Contains(fieldName)) continue;
                    ////////    var emptyItem = Activator.CreateInstance(elementType);
                    ////////    tdHidden.InnerHtml.AppendHtml(BuildHiddenInput(emptyItem, elementType, fieldName, prefixInputName, "0", icodePropName, timestampPropName, deletedPropName));
                    ////////    //__KEY__//tdHidden.InnerHtml.AppendHtml(BuildHiddenInput(emptyItem, elementType, fieldName, prefixInputName, "__KEY__", icodePropName, timestampPropName, deletedPropName));
                    ////////}
                    ////////tpl.InnerHtml.AppendHtml(tdHidden); // AGGIUNGI la colonna nascosta alla <tr>







                    foreach (var colDef in ColumnDefinitions)
                    {
                        var td = new TagBuilder("td");

                        if (colDef.Visible)
                        {
                            // SE hai un template custom definito in EditColumnsContent
                            var templateHtml = ExtractTemplateFor(colDef.For); // funzione helper
                            if (templateHtml != null)
                                td.InnerHtml.AppendHtml(templateHtml);
                            else
                                td.InnerHtml.AppendHtml($@"<input name='{prefixInputName}[0].{colDef.For}' ...>");
                        }
                        else
                        {
                            // hidden
                            td.InnerHtml.AppendHtml($@"<input type='hidden' name='{prefixInputName}[0].{colDef.For}' value='{colDef.DefaultValue ?? ""}' />");
                        }

                        tpl.InnerHtml.AppendHtml(td);
                    }







                    // VISIBILI (editor cells già in EditColumnsContent oppure fallback)
                    foreach (var colDef in ColumnDefinitions.Where(c => c.Visible))
                    {
                        var td = new TagBuilder("td");

                        // Se usi EditColumnsContent custom lo lasci invariato; se sei in fallback:
                        td.InnerHtml.AppendHtml($@"
                                                    <div class='form-group'>
                                                      <input type='text' name='{prefixInputName}[0].{colDef.For}'
                                                             value='{(colDef.DefaultValue ?? "")}' />
                                                    </div>");
                        tpl.InnerHtml.AppendHtml(td);
                    }

                    // HIDDEN
                    var tdHidden = new TagBuilder("td");
                    tdHidden.Attributes["style"] = "display:none";

                    // hidden “di servizio” (action/vars/icode/tms/del) → come tuo codice attuale
                    // ...

                    // HIDDEN per le colonne non visibili
                    foreach (var colDef in ColumnDefinitions.Where(c => !c.Visible))
                    {
                        var fullName = $"{prefixInputName}[0].{colDef.For}";
                        var input = new TagBuilder("input");
                        input.Attributes["type"] = "hidden";
                        input.Attributes["name"] = fullName;
                        input.Attributes["value"] = colDef.DefaultValue ?? ""; // default applicato
                        tdHidden.InnerHtml.AppendHtml(input);

                        var spanVal = new TagBuilder("span");
                        spanVal.AddCssClass("text-danger");
                        spanVal.Attributes["data-valmsg-for"] = fullName;
                        spanVal.Attributes["data-valmsg-replace"] = "true";
                        tdHidden.InnerHtml.AppendHtml(spanVal);
                    }
                    tpl.InnerHtml.AppendHtml(tdHidden);








                }
                else
                {
                    // fallback: input text per tutte le prop semplici
                    foreach (var col in UtilHelper.GetAllErpDogFields(elementType, bindingFlags)) //foreach (var col in elementType.GetProperties())
                    {
                        if (hiddenFieldNames.Contains(col.Prop.Name)) continue;
                        //if (col.SqlFieldOptions.Contains("[SYS]")) continue;    //scarto i campi di sistema
                        if (sys_tokens.Any(t => col.SqlFieldOptions.Contains(t))) continue;    //scarto i campi di sistema

                        var td = new TagBuilder("td");
                        td.InnerHtml.AppendHtml($@"
                        <div class='form-group'>
                            <input name='{prefixInputName}[0].{col.Prop.Name}' class='form-control' />
                            <span class='text-danger' data-valmsg-for='{prefixInputName}[0].{col.Prop.Name}' data-valmsg-replace='true'></span>
                        </div>");
                        tpl.InnerHtml.AppendHtml(td);
                    }

                    // ---- Colonna nascosta con hidden ----
                    var tdHidden = new TagBuilder("td");
                    tdHidden.Attributes["style"] = "display:none";

                    // Hidden di servizio
                    foreach (var fieldName in hiddenFieldNames)
                    {
                        var emptyItem = Activator.CreateInstance(elementType);
                        tdHidden.InnerHtml.AppendHtml(BuildHiddenInput(emptyItem, elementType, fieldName, prefixInputName, "0", icodePropName, timestampPropName, deletedPropName));
                        //__KEY__//tdHidden.InnerHtml.AppendHtml(BuildHiddenInput(emptyItem, elementType, fieldName, prefixInputName, "__KEY__", icodePropName, timestampPropName, deletedPropName));
                    }
                    tpl.InnerHtml.AppendHtml(tdHidden); // AGGIUNGI la colonna nascosta alla <tr>
                }

                // Colonna Actions
                if (AllowEdit || AllowAdd)
                {
                    var tdTplActions = new TagBuilder("td");
                    tdTplActions.InnerHtml.AppendHtml("<button type='button' class='btn btn-success erptbl-save'>💾</button> <button type='button' class='btn btn-secondary erptbl-cancel'>✖</button>");
                    tpl.InnerHtml.AppendHtml(tdTplActions);
                }

                tbody.InnerHtml.AppendHtml(tpl);

                // FOOTER (add) --------------------------------------------------------
                var tfoot = new TagBuilder("tfoot");
                if (AllowAdd)
                {
                    tfoot.InnerHtml.AppendHtml($@"
                    <tr><td colspan='99'>
                        <button type='button' class='btn btn-success erptbl-add' data-prefix='{prefixInputName}'>+ Aggiungi riga</button>
                    </td></tr>");
                }

                output.Content.AppendHtml(thead);
                output.Content.AppendHtml(tbody);
                output.Content.AppendHtml(tfoot);
            }

            private TagBuilder BuildHiddenInput(
                object item,
                Type elementType,
                string fieldName,
                string prefixInputName,
                string key,                    // <-- string al posto di int
                string icodePropName,
                string timestampPropName,
                string deletedPropName)
            {
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance; //BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

                //< div class='form-group'>
                //  <input class='form-control' />
                //</div>");
                var div = new TagBuilder("div");
                div.AddCssClass("form-group");
                //---
                var prop = elementType.GetProperty(fieldName);
                if (prop == null) return div; // ritorna un div vuoto se la proprietà non esiste
                var fullName = $"{prefixInputName}[{key}].{prop.Name}";
                var idPrefix = TagBuilder.CreateSanitizedId($"{prefixInputName}[{key}]", "_");
                var id = fullName.Replace(".", "_").Replace("[", "_").Replace("]", "_");
                var value = prop.GetValue(item);
                //---
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
                    input.AddCssClass("ModelAction");
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
                    input.Attributes["value"] = value?.ToString() ?? "";
                }


                div.InnerHtml.AppendHtml(input);

                // Anche nel fallback, tenta di usare getLabelForField
                string displayValue = value?.ToString() ?? "";
                var getLabelMethod = elementType.GetMethod("getLabelForField", bindingFlags);
                if (getLabelMethod != null)
                {
                    try
                    {
                        var label = getLabelMethod.Invoke(item, new object[] { prop.Name });
                        if (label != null && !string.IsNullOrEmpty(label.ToString()))
                        {
                            displayValue = label.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Se fallisce, usa il valore raw
                        Console.WriteLine($"Error calling getLabelForField for {prop.Name}: {ex.Message}");
                    }
                }
                //------------
                var span = new TagBuilder("span");
                span.Attributes["data-field"] = prop.Name;
                span.Attributes["data-row-key"] = key;   // <-- al posto di data-row-index
                span.InnerHtml.AppendHtml(displayValue); // ✅ Renderizza HTML anche nel fallback
                div.InnerHtml.AppendHtml(span);
                //------------
                var spanVal = new TagBuilder("span");
                spanVal.AddCssClass("text-danger");
                spanVal.Attributes["data-valmsg-for"] = fullName;
                spanVal.Attributes["data-valmsg-replace"] = "true";
                div.InnerHtml.AppendHtml(spanVal);
                //------------
                //------------
                return div;
            }

        }

        ////////// erp-table-columns
        ////////[HtmlTargetElement("erp-table-columns", ParentTag = "erp-table")]
        ////////public class ErpTableColumnsTagHelper : TagHelper
        ////////{
        ////////    public override int Order => 10; // Esegui dopo i figli (erp-table-col)

        ////////    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        ////////    {
        ////////        Console.WriteLine("erp-table-columns processing");

        ////////        var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
        ////////        if (table != null)
        ////////        {
        ////////            // Ottieni il contenuto HTML grezzo
        ////////            var childContent = await output.GetChildContentAsync();
        ////////            var htmlContent = childContent.GetContent();

        ////////            Console.WriteLine($"HTML Content: {htmlContent}");

        ////////            // Parse manualmente i tag <erp-table-col>
        ////////            var columns = new List<(string For, string Label)>();
        ////////            var lines = htmlContent.Split(new[] { "<erp-table-col" }, StringSplitOptions.RemoveEmptyEntries);

        ////////            foreach (var line in lines.Skip(1)) // Salta il primo che è vuoto o commenti
        ////////            {
        ////////                // Estrai for="..." e label="..."
        ////////                var forMatch = System.Text.RegularExpressions.Regex.Match(line, @"for=""([^""]+)""");
        ////////                var labelMatch = System.Text.RegularExpressions.Regex.Match(line, @"label=""([^""]+)""");

        ////////                if (forMatch.Success && labelMatch.Success)
        ////////                {
        ////////                    var forValue = forMatch.Groups[1].Value;
        ////////                    var labelValue = labelMatch.Groups[1].Value;
        ////////                    columns.Add((forValue, labelValue));
        ////////                    Console.WriteLine($"Found column: {forValue} - {labelValue}");
        ////////                }
        ////////            }

        ////////            if (columns.Any())
        ////////            {
        ////////                // Genera gli header <th> basati sulle colonne definite
        ////////                var headerContent = new DefaultTagHelperContent();
        ////////                foreach (var col in columns)
        ////////                {
        ////////                    var th = new TagBuilder("th");
        ////////                    th.InnerHtml.Append(col.Label);
        ////////                    headerContent.AppendHtml(th);
        ////////                }

        ////////                table.ColumnsContent = headerContent;
        ////////                table.ColumnDefinitions = columns;
        ////////                Console.WriteLine($"Columns set: {columns.Count}");
        ////////            }
        ////////            else
        ////////            {
        ////////                Console.WriteLine("No columns found - using default");
        ////////            }
        ////////        }
        ////////        output.SuppressOutput();
        ////////    }
        ////////}

        [HtmlTargetElement("erp-table-edit-columns", ParentTag = "erp-table")]
        public class ErpTableEditColumnsTagHelper : TagHelper
        {
            public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
            {
                var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
                if (table == null)
                {
                    output.SuppressOutput();
                    return;
                }

                var childContent = await output.GetChildContentAsync();
                var htmlContent = childContent.GetContent();

                // Raccogli i template dei campi (MA SENZA CREARE <td>)
                var templates = new Dictionary<string, string>();
                var pattern = @"<erp-table-edit-col\s+for=""([^""]+)""[^>]*>(.*?)</erp-table-edit-col>";
                var matches = System.Text.RegularExpressions.Regex.Matches(
                    htmlContent, pattern, System.Text.RegularExpressions.RegexOptions.Singleline);

                foreach (System.Text.RegularExpressions.Match match in matches)
                    templates[match.Groups[1].Value] = match.Groups[2].Value.Trim();

                // Salva i template nel padre
                table.EditColumnsContent = new DefaultTagHelperContent();
                foreach (var kv in templates)
                    table.EditColumnsContent.AppendHtml($"<!--template:{kv.Key}-->{kv.Value}");

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

            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                // ✅ Recupera l’istanza del PADRE <erp-table> messa nel suo Init()
                var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
                if (table != null)
                {
                    table.ColumnDefinitions ??= new List<ErpTableTagHelper.ColumnDef>();
                    table.ColumnDefinitions.Add(new ErpTableTagHelper.ColumnDef
                    {
                        For = For,
                        Label = Label ?? For,
                        Visible = Visible ?? true,
                        DefaultValue = DefaultValue
                    });
                }

                // Non deve renderizzare nulla
                output.SuppressOutput();
            }
        }

        // erp-table-edit-columns
        [HtmlTargetElement("erp-table-edit-columns", ParentTag = "erp-table")]
        public class ErpTableEditColumnsTagHelper : TagHelper
        {
            public override int Order => 10; // Esegui dopo i figli (erp-table-edit-col)

            public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
            {
                Console.WriteLine("erp-table-edit-columns processing");

                var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
                if (table != null)
                {
                    // Ottieni il contenuto HTML grezzo
                    var childContent = await output.GetChildContentAsync();
                    var htmlContent = childContent.GetContent();

                    // Parse manualmente i tag <erp-table-edit-col>
                    var editColumns = new Dictionary<string, string>();

                    // Usa regex per estrarre ogni blocco <erp-table-edit-col for="...">...</erp-table-edit-col>
                    var pattern = @"<erp-table-edit-col\s+for=""([^""]+)""[^>]*>(.*?)</erp-table-edit-col>";
                    var matches = System.Text.RegularExpressions.Regex.Matches(htmlContent, pattern,
                        System.Text.RegularExpressions.RegexOptions.Singleline);

                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        var forValue = match.Groups[1].Value;
                        var content = match.Groups[2].Value.Trim();
                        editColumns[forValue] = content;
                        Console.WriteLine($"Found edit column: {forValue}");
                    }

                    if (editColumns.Any() && table.ColumnDefinitions != null)
                    {
                        Console.WriteLine($"EditColumns found: {editColumns.Count}, ColumnDefs: {table.ColumnDefinitions.Count}");

                        // Genera le celle <td> dell'editor
                        var editContent = new DefaultTagHelperContent();

                        foreach (var colDef in table.ColumnDefinitions)
                        {
                            var td = new TagBuilder("td");

                            if (editColumns.ContainsKey(colDef.For))
                            {
                                td.InnerHtml.AppendHtml(editColumns[colDef.For]);
                            }
                            else
                            {
                                // Fallback
                                td.InnerHtml.AppendHtml($@"
                                    <div class='form-group'>
                                        <input class='form-control' />
                                    </div>");
                            }

                            editContent.AppendHtml(td);
                        }

                        // Aggiungi colonne di servizio (action, vars, etc.)
                        foreach (var kvp in editColumns)
                        {
                            if (table.ColumnDefinitions == null || !table.ColumnDefinitions.Any(c => c.For == kvp.Key))
                            {
                                var td = new TagBuilder("td");
                                td.Attributes["style"] = "display:none";
                                td.InnerHtml.AppendHtml(kvp.Value);
                                editContent.AppendHtml(td);
                            }
                        }

                        table.EditColumnsContent = editContent;
                    }
                    else
                    {
                        Console.WriteLine("No edit columns or column definitions");
                    }
                }
                output.SuppressOutput();
            }
        }

        // erp-table-actions
        [HtmlTargetElement("erp-table-actions", ParentTag = "erp-table")]
        public class ErpTableActionsTagHelper : TagHelper
        {
            public string Mode { get; set; } = "all"; // add | edit | delete | all | none

            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                var table = context.Items[typeof(ErpTableTagHelper)] as ErpTableTagHelper;
                var html = new System.Text.StringBuilder();

                if (Mode is "edit" or "all") html.Append("<button type='button' class='btn btn-primary erptbl-modify'>✏</button> ");
                if (Mode is "delete" or "all") html.Append("<button type='button' class='btn btn-danger erptbl-delete'>🗑</button> ");

                if (table != null)
                    table.ActionsContent = new DefaultTagHelperContent().AppendHtml(html.ToString());

                output.SuppressOutput();
            }
        }
    *******************************************************************/

    // ===================================================================
    // NOTA: I tag <erp-table-col> e <erp-table-edit-col> vengono processati
    // direttamente tramite parsing HTML nei loro parent TagHelper
    // Non servono TagHelper separati per questi elementi
    // ===================================================================


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
                    Filterable = Filter == true
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
