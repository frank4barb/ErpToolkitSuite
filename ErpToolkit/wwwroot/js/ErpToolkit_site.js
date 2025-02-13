// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



// ***********************************************************************************************
// EDIT DELETE MODAL


//open and fill modal edit dialog

//eg: loadModalWithContent('editModal', '/Datatable/EditCustomer', 'SDA33DW1AFS')
function loadModalWithContent(modalDialogId, modalAction, strId) {
    openModalWithContent(modalDialogId, modalAction, {
        'Id': strId
    });
}
//eg: updateModalWithContent('editModal', '/Datatable/SaveCustomer', {Campo1='xxxxx',Campo2='xxxx', ecc...})
function updateModalWithContent(modalDialogId, modalAction, jsonObject) {
    openModalWithContent(modalDialogId, modalAction, {
        'data': jsonObject
    });
}
//eg: updateModalWithContent('editModal', '/Datatable/SaveCustomer')
function updateModalWithContentForm(button, prefix, modalDialogId, jsonObject) {
    var fullprefix = prefix +".";
    // Trova il form più vicino al pulsante
    let form = button.closest('form');
    if (!form) {
        console.error("Nessun form trovato!");
        return;
    }
    let modalAction = form.action;
    // Usa FormData per raccogliere i dati modificati
    let formData = new FormData(form);

    formData.forEach((value, key) => {
        if (key.startsWith(fullprefix)) key = key.substring(fullprefix.length);
        if (jsonObject[key] === undefined) { /*skip*/ }
        else if (jsonObject[key] == null && value == '') { /*skip*/ }
        else { jsonObject[key] = value; /*update*/ }
    });

    openModalWithContent(modalDialogId, modalAction, {
        'data': jsonObject
    });
}


function openModalWithContent(modalDialogId, modalAction, jsonParams) {
    fetch(modalAction, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(jsonParams)
    })
        .then(response => response.text())
        .then(html => {
            document.getElementById(modalDialogId).innerHTML = html;  //inserisco contenuto in dialog modale

            //AZIONI DA FARE AL CLICK BOTTONE
            var isModalACTION_CLOSE = $('#' + modalDialogId).find('[name$="IsModalACTION"]').val() == 'CLOSE';
            if (isModalACTION_CLOSE == true) { $('#' + modalDialogId).modal('hide'); } //nascondi modal
            else { $('#' + modalDialogId).modal('show'); } //mostra modal
            var isPageACTION_RELOAD = $('#' + modalDialogId).find('[name$="IsPageACTION"]').val() == 'RELOAD';
            if (isPageACTION_RELOAD == true) { location.reload(true); } //ricarica pagina dal server (ie: no cache)
            var isPageREDIRECT = $('#' + modalDialogId).find('[name$="IsPageREDIRECT"]').val();
            if (isPageREDIRECT != undefined && isPageREDIRECT != "") { location.href = isPageREDIRECT; } //ridireziona su altra pagina

            // Una volta completato il caricamento della PartialView
            initializeAfterLoadPageAndPartial(); // Richiama la funzione anche dopo il caricamento della PartialView
        })
        .catch(error => console.error('Errore:', error));
}



// ***********************************************************************************************
// SCELTA SINGOLA O MULTIPLA

// funzione richiamata da SwitchGroupTagHelper


function handleMaxSelections(groupName, maxSelections) {
    if (maxSelections <= 1) return;
    var group = document.querySelectorAll(`input[name="${groupName}"]`);
    var checkedCount = 0;

    group.forEach(function (item) {
        if (item.checked) {
            checkedCount++;
        }
    });

    if (checkedCount >= maxSelections) {
        group.forEach(function (item) {
            if (!item.checked) {
                item.disabled = true;
            }
        });
    } else {
        group.forEach(function (item) {
            item.disabled = false;
        });
    }
}




// ***********************************************************************************************
// AUTOCOMPLETE CLIENT E SERVER ...chiamate da AutocompleteTagHelper (ErpComponentTagHelper)


//$(document).ready(function () {
//    $('.autocomplete-input').each(function () {
//        var input = $(this);
//        var resultsDivId = input.data('name') + 'AutocompleteResults';
//        var selectedItemsDivId = input.data('selected-items-div-id');
//        var resultsDiv = $('#' + resultsDivId);
//        var selectedItemsDiv = $('#' + selectedItemsDivId);
//        var maxSelections = input.data('max-selections');
//        var minChars = input.data('min-chars'); // Ottieni il numero di caratteri minimo
//        var mode = input.data('mode'); // Ottieni la modalità di autocomplete
//        var allChoices = [];

//        resultsDiv.hide();

//        if (mode === 'autocompleteClient') {
//            // Modalità client: Precarica tutte le scelte
//            var controller = input.data('controller');
//            var action = input.data('action');

//            $.get('/' + controller + '/' + action, function (data) {
//                if (data.error) {
//                    var validationMessage = $('span[data-valmsg-for=\"' + input.data('name') +'\"]');
//                    validationMessage.text(data.error);
//                    validationMessage.show();
//                }
//                else {
//                    allChoices = data;
//                    console.log('All choices loaded:', allChoices);
//                }

//                // Carica i valori pre-selezionati
//                var preSelectedList = input.data('pre-selected');
//                var preSelected = preSelectedList.preSelected;
//                //var preSelected = JSON.parse(preSelectedJson);
//                if (preSelected) {
//                    preSelected.forEach(function (value) {
//                        var item = allChoices.find(c => c.value === value);
//                        if (item) {
//                            if (!isItemAlreadySelected(value, selectedItemsDiv)) {
//                                addSelectedItem(item.label, item.value, input, selectedItemsDiv);
//                            }
//                        }
//                    });
//                    toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//                }
//            });

//            input.on('input', function () {
//                var term = $(this).val().toLowerCase();
//                resultsDiv.empty();
//                if (term.length >= minChars) {
//                    var filtered = allChoices.filter(c => c.label.toLowerCase().includes(term));
//                    if (filtered.length) {
//                        resultsDiv.show();
//                        filtered.forEach(function (item) {
//                            resultsDiv.append('<div class="autocomplete-item" data-value="' + item.value + '" data-label="' + item.label + '">' + item.label + '</div>');
//                        });
//                        adjustResultsDivWidth(input, resultsDiv);
//                    } else {
//                        resultsDiv.hide();
//                    }
//                } else {
//                    resultsDiv.hide();
//                }
//            });

//        } else if (mode === 'autocompleteServer') {
//            // Modalità server: Richiede i dati ad ogni variazione del termine di ricerca
//            var controller = input.data('controller');
//            var action = input.data('action');
//            var preloadAction = input.data('preload-action');

//            // Carica i valori pre-selezionati
//            var preSelectedList = input.data('pre-selected');
//            var preSelected = preSelectedList.preSelected;
//            if (preSelected) {
//                $.post('/' + controller + '/' + preloadAction, { values: preSelected }, function (data) {
//                    if (data.error) {
//                        var validationMessage = $('span[data-valmsg-for=\"' + input.data('name') + '\"]');
//                        validationMessage.text(data.error);
//                        validationMessage.show();
//                    }
//                    else {
//                        data.forEach(function (item) {
//                            if (!isItemAlreadySelected(value, selectedItemsDiv)) {
//                                addSelectedItem(item.label, item.value, input, selectedItemsDiv);
//                            }
//                        });
//                        toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//                    }
//                });
//            }

//            input.on('input', function () {
//                var term = $(this).val().toLowerCase();
//                resultsDiv.empty();
//                if (term.length >= minChars) {
//                    $.get('/' + controller + '/' + action, { term: term }, function (data) {
//                        if (data.error) {
//                            var validationMessage = $('span[data-valmsg-for=\"' + input.data('name') + '\"]');
//                            validationMessage.text(data.error);
//                            validationMessage.show();
//                        }
//                        else if (data.length) {
//                            resultsDiv.show();
//                            data.forEach(function (item) {
//                                resultsDiv.append('<div class="autocomplete-item" data-value="' + item.value + '" data-label="' + item.label + '">' + item.label + '</div>');
//                            });
//                            adjustResultsDivWidth(input, resultsDiv);
//                        } else {
//                            resultsDiv.hide();
//                        }
//                    });
//                } else {
//                    resultsDiv.hide();
//                }
//            });
//        }

//        function adjustResultsDivWidth(input, resultsDiv) {
//            resultsDiv.css('width', input.outerWidth() + 'px');
//        }

//        input.on('focus', function () {
//            adjustResultsDivWidth(input, resultsDiv);
//        });

//        var isSelectingItem = false;

//        $(document).on('mousedown', '.autocomplete-item', function () {
//            isSelectingItem = true;
//        });

//        $(document).on('mouseup', '.autocomplete-item', function () {
//            isSelectingItem = false;
//        });

//        input.on('blur', function () {
//            setTimeout(function () {
//                if (!isSelectingItem) {
//                    resultsDiv.hide();
//                }
//            }, 100);
//        });

//        $(document).on('click', '#' + resultsDivId + ' .autocomplete-item', function () {
//            var label = $(this).data('label');
//            var value = $(this).data('value');
//            if (!isItemAlreadySelected(value, selectedItemsDiv)) {
//                addSelectedItem(label, value, input, selectedItemsDiv);
//            }
//            input.val('');
//            resultsDiv.hide();
//        });

//        $(document).on('click', '#' + selectedItemsDivId + ' .remove-item', function () {
//            $(this).parent().remove();
//            toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//        });

//        function isItemAlreadySelected(value, selectedItemsDiv) {
//            return selectedItemsDiv.find('.selected-item[data-value="' + value + '"]').length > 0;
//        }

//        function addSelectedItem(label, value, input, selectedItemsDiv) {
//            var itemDiv = $('<div class="selected-item" data-value="' + value + '">' + label + ' <span class="remove-item">&times;</span></div>');
//            var inputField = $('<input type="hidden" name="' + input.data('name') + '" value="' + value + '" />');
//            itemDiv.append(inputField);
//            selectedItemsDiv.append(itemDiv);
//            toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//        }

//        function toggleInputVisibility(input, selectedItemsDiv, maxSelections) {
//            var selectedCount = selectedItemsDiv.children().length;
//            if (maxSelections > 0 && selectedCount >= maxSelections) {
//                input.hide();
//            } else {
//                input.show();
//            }
//        }

//        // Initial toggle in case there are pre-selected items
//        toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//    });
//});



//$(document).ready(function () {
//    $('.autocomplete-input').each(function () {
//        var input = $(this);
//        var resultsDivId = input.data('name') + 'AutocompleteResults';
//        var selectedItemsDivId = input.data('selected-items-div-id');
//        var resultsDiv = $('#' + resultsDivId);
//        var selectedItemsDiv = $('#' + selectedItemsDivId);
//        var maxSelections = input.data('max-selections');
//        var minChars = input.data('min-chars');
//        var mode = input.data('mode');
//        var readonly = input.data('readonly') || 'N'; // Default readonly="N"
//        var visible = input.data('visible') || 'Y'; // Default visible="Y"
//        var allChoices = [];
//        var cache = {};

//        resultsDiv.hide();

//        function loadChoices(callback) {
//            if (mode === 'autocompleteClient') {
//                var controller = input.data('controller');
//                var action = input.data('action');

//                $.get('/' + controller + '/' + action, function (data) {
//                    if (data.error) {
//                        showValidationMessage(input.data('name'), data.error);
//                    } else {
//                        allChoices = data;
//                        console.log('All choices loaded:', allChoices);
//                        callback();
//                    }
//                });
//            } else {
//                callback();
//            }
//        }

//        loadChoices(function () {
//            var preSelectedList = input.data('pre-selected');
//            var preSelected = preSelectedList.preSelected;
//            if (preSelected) {

//                if (mode === 'autocompleteClient') {
//                    preSelected.forEach(function (value) {
//                        var item = allChoices.find(c => c.value === value);
//                        if (item && !isItemAlreadySelected(value, selectedItemsDiv)) {
//                            addSelectedItem(item.label, item.value, input, selectedItemsDiv);
//                        }
//                    });
//                    toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//                }
//                else {
//                    if (preSelected.length > 0) {
//                        // Modalità server: Richiede i dati ad ogni variazione del termine di ricerca
//                        var controller = input.data('controller');
//                        var preloadAction = input.data('preload-action');

//                        $.ajax({
//                            url: '/' + controller + '/' + preloadAction,
//                            type: 'POST',
//                            data: JSON.stringify(preSelected),  // Serializziamo l'array come JSON
//                            contentType: 'application/json; charset=utf-8',  // Impostiamo il Content-Type su JSON
//                            dataType: 'json',  // Ci aspettiamo una risposta JSON dal server
//                            success: function (data) {
//                                if (data.error) {
//                                    var validationMessage = $('span[data-valmsg-for="' + input.data('name') + '"]');
//                                    validationMessage.text(data.error);
//                                    validationMessage.show();
//                                } else {
//                                    data.forEach(function (item) {
//                                        if (item && !isItemAlreadySelected(item, selectedItemsDiv)) {
//                                            addSelectedItem(item.label, item.value, input, selectedItemsDiv);
//                                        }
//                                    });
//                                    toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//                                }
//                            }
//                        });

//                    }
//                }

//            }
//        });

//        input.on('input', function () {
//            var term = $(this).val().toUpperCase();
//            resultsDiv.empty();
//            if (term.length >= minChars) {
//                if (mode === 'autocompleteClient') {
//                    var filtered = allChoices.filter(c => (' ' + c.label.toUpperCase() + ' ').includes(term));
//                    showResults(filtered);
//                } else if (mode === 'autocompleteServer') {
//                    if (cache[term]) {
//                        showResults(cache[term]);
//                    } else {
//                        var controller = input.data('controller');
//                        var action = input.data('action');
//                        $.get('/' + controller + '/' + action, { term: term }, function (data) {
//                            if (data.error) {
//                                showValidationMessage(input.data('name'), data.error);
//                            } else {
//                                cache[term] = data;
//                                showResults(data);
//                            }
//                        });
//                    }
//                }
//            } else {
//                resultsDiv.hide();
//            }
//        });

//        // Gestione del click sull'icona
//        input.next('.autocomplete-icon').on('click', function () {
//            if (resultsDiv.is(':visible')) {
//                resultsDiv.hide();
//            } else {
//                if (mode === 'autocompleteClient') {
//                    showResults(allChoices);
//                } else if (mode === 'autocompleteServer') {
//                    var controller = input.data('controller');
//                    var action = input.data('action'); var term = '%';
//                    $.get('/' + controller + '/' + action, { term: term }, function (data) {
//                        if (data.error) {
//                            showValidationMessage(input.data('name'), data.error);
//                        } else {
//                            showResults(data);
//                        }
//                    });
//                }
//            }
//        });

//        function showResults(items) {
//            if (items.length) {
//                resultsDiv.empty();
//                resultsDiv.show();
//                items.forEach(function (item, index) {
//                    resultsDiv.append('<div class="autocomplete-item" tabindex="0" role="option" data-value="' + item.value + '" data-label="' + item.label + '">' + item.label + '</div>');
//                });
//                adjustResultsDivWidth(input, resultsDiv);
//            } else {
//                resultsDiv.hide();
//            }
//        }


//        function adjustResultsDivWidth(input, resultsDiv) {
//            resultsDiv.css('width', input.outerWidth() + 'px');
//        }

//        input.on('focus', function () {
//            adjustResultsDivWidth(input, resultsDiv);
//        });

//        var isSelectingItem = false;

//        $(document).on('mousedown', '.autocomplete-item', function () {
//            isSelectingItem = true;
//        });

//        $(document).on('mouseup', '.autocomplete-item', function () {
//            isSelectingItem = false;
//        });

//        input.on('blur', function () {
//            setTimeout(function () {
//                if (!isSelectingItem) {
//                    resultsDiv.hide();
//                }
//            }, 100);
//        });

//        $(document).on('click', '#' + resultsDivId + ' .autocomplete-item', function () {
//            var label = $(this).data('label');
//            var value = $(this).data('value');
//            if (!isItemAlreadySelected(value, selectedItemsDiv)) {
//                addSelectedItem(label, value, input, selectedItemsDiv);
//            }
//            input.val('');
//            resultsDiv.hide();
//        });

//        $(document).on('click', '#' + selectedItemsDivId + ' .remove-item', function () {
//            $(this).parent().remove();
//            toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//        });

//        function isItemAlreadySelected(value, selectedItemsDiv) {
//            return selectedItemsDiv.find('.selected-item[data-value="' + value + '"]').length > 0;
//        }

//        function addSelectedItem(label, value, input, selectedItemsDiv) {
//            var itemDiv = $('<div class="selected-item" data-value="' + value + '">' + label + ' <span class="remove-item">&times;</span></div>');
//            var inputField = $('<input type="hidden" name="' + input.data('name') + '" value="' + value + '" />');
//            itemDiv.append(inputField);
//            selectedItemsDiv.append(itemDiv);
//            toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//        }

//        function toggleInputVisibility(input, selectedItemsDiv, maxSelections) {
//            var selectedCount = selectedItemsDiv.children().length;
//            if (maxSelections > 0 && selectedCount >= maxSelections) {
//                input.hide();
//            } else {
//                input.show();
//            }
//        }

//        // Initial toggle in case there are pre-selected items
//        toggleInputVisibility(input, selectedItemsDiv, maxSelections);
//    });
//});



function initializeAfterLoadPageAndPartial() {
    $('.autocomplete-input').each(function () {
        var input = $(this);
        //var inputId = input.data('id');
        //var inputName = input.data('name');
        var resultsDivId = input.data('id') + 'AutocompleteResults';
        var selectedItemsDivId = input.data('selected-items-div-id');
        var resultsDiv = $('#' + resultsDivId);
        var selectedItemsDiv = $('#' + selectedItemsDivId);
        var maxSelections = input.data('max-selections');
        var minChars = input.data('min-chars');
        var mode = input.data('mode');
        var readonly = input.data('readonly') || 'N'; // Default readonly="N"
        var visible = input.data('visible') || 'Y'; // Default visible="Y"
        var allChoices = [];
        var cache = {};

        resultsDiv.hide();

        function loadChoices(callback) {
            if (mode === 'autocompleteClient') {
                var controller = input.data('controller');
                var action = input.data('action');

                $.get('/' + controller + '/' + action, function (data) {
                    if (data.error) {
                        showValidationMessage(input.data('name'), data.error);
                    } else {
                        allChoices = data;
                        console.log('All choices loaded:', allChoices);
                        callback();
                    }
                });
            } else {
                callback();
            }
        }


        // Caricamento delle scelte e gestione del parametro visible
        loadChoices(function () {
            var preSelectedList = input.data('pre-selected');
            var preSelected = preSelectedList.preSelected;

            // Gestione del parametro visible
            if (visible === 'N') {

                //nasconde anche la label che è esterna al tag asp-for
                var label = document.querySelector('label[for="' + input.data('id') + '"]');
                if (label) { label.style.display = 'none'; }
                //---

                input.hide(); $('#' + input.data('id') + 'AutocompleteWrapper').hide();
                resultsDiv.hide();
                selectedItemsDiv.hide(); // Nasconde anche il div delle scelte selezionate
                return; // Esce dalla funzione per evitare ulteriori elaborazioni
            }

            // Gestione delle scelte pre-selezionate
            if (preSelected) {
                if (mode === 'autocompleteClient') {
                    preSelected.forEach(function (value) {
                        var item = allChoices.find(c => c.value === value);
                        if (item && !isItemAlreadySelected(value, selectedItemsDiv)) {
                            addSelectedItem(item.label, item.value, input, selectedItemsDiv);
                        }
                    });
                } else {

                    if (preSelected.length > 0) {
                        // Modalità server: Richiede i dati ad ogni variazione del termine di ricerca
                        var controller = input.data('controller');
                        var preloadAction = input.data('preload-action');

                        $.ajax({
                            url: '/' + controller + '/' + preloadAction,
                            type: 'POST',
                            data: JSON.stringify(preSelected),  // Serializziamo l'array come JSON
                            contentType: 'application/json; charset=utf-8',  // Impostiamo il Content-Type su JSON
                            dataType: 'json',  // Ci aspettiamo una risposta JSON dal server
                            success: function (data) {
                                if (data.error) {
                                    var validationMessage = $('span[data-valmsg-for="' + input.data('name') + '"]');
                                    validationMessage.text(data.error);
                                    validationMessage.show();
                                } else {
                                    data.forEach(function (item) {
                                        if (item && !isItemAlreadySelected(item, selectedItemsDiv)) {
                                            addSelectedItem(item.label, item.value, input, selectedItemsDiv);
                                        }
                                    });
                                }
                            }
                        });

                    }

                }
            }

            // Gestione del parametro readonly 
            if (readonly === 'Y') {
                if (preSelected.length > 0 && preSelected[0] != null) {  
                    $('#' + input.data('id') + 'AutocompleteWrapper').find('.autocomplete-icon').hide(); // Rimuove lente ricerca
                    input.hide(); // Nasconde la input-box se ci sono elementi pre-selezionati
                }
                else {
                    $('#' + input.data('id') + 'AutocompleteWrapper').find('.autocomplete-icon').hide(); // Rimuove lente ricerca
                    input.prop('readonly', true);  // Rende il campo di input non modificabile
                    input.css({
                        'background-color': '#e9ecef', // Colore di sfondo per indicare che è disabilitato
                        'cursor': 'not-allowed'
                    });
                }
            }

            toggleInputVisibility(input, selectedItemsDiv, maxSelections);
        });


        input.on('input', function () {
            var term = $(this).val().toUpperCase();
            resultsDiv.empty();
            if (term.length >= minChars) {
                if (mode === 'autocompleteClient') {
                    var filtered = allChoices.filter(c => (' ' + c.label.toUpperCase() + ' ').includes(term));
                    showResults(filtered);
                } else if (mode === 'autocompleteServer') {
                    if (cache[term]) {
                        showResults(cache[term]);
                    } else {
                        var controller = input.data('controller');
                        var action = input.data('action');
                        $.get('/' + controller + '/' + action, { term: term }, function (data) {
                            if (data.error) {
                                showValidationMessage(input.data('name'), data.error);
                            } else {
                                cache[term] = data;
                                showResults(data);
                            }
                        });
                    }
                }
            } else {
                resultsDiv.hide();
            }
        });

        // Gestione del click sull'icona
        input.next('.autocomplete-icon').on('click', function () {
            if (resultsDiv.is(':visible')) {
                resultsDiv.hide();
            } else {
                if (mode === 'autocompleteClient') {
                    showResults(allChoices);
                } else if (mode === 'autocompleteServer') {
                    var controller = input.data('controller');
                    var action = input.data('action'); var term = '%';
                    $.get('/' + controller + '/' + action, { term: term }, function (data) {
                        if (data.error) {
                            showValidationMessage(input.data('name'), data.error);
                        } else {
                            showResults(data);
                        }
                    });
                }
            }
        });

        function showResults(items) {
            if (items.length) {
                resultsDiv.empty();
                resultsDiv.show();
                items.forEach(function (item, index) {
                    resultsDiv.append('<div class="autocomplete-item" tabindex="0" role="option" data-value="' + item.value + '" data-label="' + item.label + '">' + item.label + '</div>');
                });
                adjustResultsDivWidth(input, resultsDiv);
            } else {
                resultsDiv.hide();
            }
        }

        function showValidationMessage(fieldName, message) {
            // Cerca il tag span di validazione associato al campo specifico (utilizzando asp-validation-for)
            var validationSpan = $('span[data-valmsg-for="' + fieldName + '"]');

            // Imposta il messaggio di errore all'interno del tag span
            validationSpan.text(message);

            // Aggiungi la classe 'text-danger' per applicare lo stile di errore (nel caso non sia presente)
            validationSpan.addClass('text-danger');

            // Mostra il messaggio di errore (se nascosto o gestito con altre classi)
            validationSpan.show();
        }

        function adjustResultsDivWidth(input, resultsDiv) {
            resultsDiv.css('width', input.outerWidth() + 'px');
        }

        input.on('focus', function () {
            adjustResultsDivWidth(input, resultsDiv);
        });

        var isSelectingItem = false;

        $(document).on('mousedown', '.autocomplete-item', function () {
            isSelectingItem = true;
        });

        $(document).on('mouseup', '.autocomplete-item', function () {
            isSelectingItem = false;
        });

        input.on('blur', function () {
            setTimeout(function () {
                if (!isSelectingItem) {
                    resultsDiv.hide();
                }
            }, 100);
        });

        $(document).on('click', '#' + resultsDivId + ' .autocomplete-item', function () {
            var label = $(this).data('label');
            var value = $(this).data('value');
            if (!isItemAlreadySelected(value, selectedItemsDiv)) {
                addSelectedItem(label, value, input, selectedItemsDiv);
            }
            input.val('');
            resultsDiv.hide();
        });

        $(document).on('click', '#' + selectedItemsDivId + ' .remove-item', function () {
            $(this).parent().remove();
            toggleInputVisibility(input, selectedItemsDiv, maxSelections);
        });

        function isItemAlreadySelected(value, selectedItemsDiv) {
            return selectedItemsDiv.find('.selected-item[data-value="' + value + '"]').length > 0;
        }

        function addSelectedItem(label, value, input, selectedItemsDiv) {
            var itemDiv = $('<div class="selected-item" data-value="' + value + '">' + label + ' <span class="remove-item">&times;</span></div>');
            var inputField = $('<input type="hidden" name="' + input.data('name') + '" value="' + value + '" />');
            itemDiv.append(inputField);
            selectedItemsDiv.append(itemDiv);
            //----- Gestione del parametro readonly quando vado ad inserire un item -----
            if (input.data('readonly') == 'Y') {
                selectedItemsDiv.find('.remove-item').remove(); // Rimuove i pulsanti di rimozione
            }
            //----------------------------------------------------------------------------
            toggleInputVisibility(input, selectedItemsDiv, maxSelections);
        }

        function toggleInputVisibility(input, selectedItemsDiv, maxSelections) {
            var selectedCount = selectedItemsDiv.children().length;
            if (maxSelections > 0 && selectedCount >= maxSelections) {
                input.hide(); $('#' + input.data('id') + 'AutocompleteWrapper').hide(); 
            } else {
                if (input.data('readonly') != 'Y') { input.show(); $('#' + input.data('id') + 'AutocompleteWrapper').show(); }
            }
        }

        // Initial toggle in case there are pre-selected items
        toggleInputVisibility(input, selectedItemsDiv, maxSelections);

    });
}



$(document).ready(function () {
    initializeAfterLoadPageAndPartial(); // Chiamata all'inizio del caricamento della pagina
});



