// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.





//----------------------------------------------------------------------------
// Funzioni TagHelper
//----------------------------------------------------------------------------

// Funzione per pulire i controlli di un TagHelper in base alla classe dell'input
function cleanTagHelper(tagId) {
    const input = document.getElementById(tagId);
    if (!input) return;

    //controlli taghelper 
    if (input.classList.contains("autocomplete-input")) {  //div.autocomplete-wrapper (taghelper)
        etkAutocompleteClean(input); // Pulisce input e contenitore selezioni
    } else if (input.classList.contains("form-check-input")) { //div.switch-group (taghelper)
        etkSwitchGroupClean(input); // Pulisce input checked
    } else if (input.classList.contains("date-range-input")) { //div.date-range (taghelper)
        alert("funzione date-range non implementata");

        //controlli standard 
    } else if (input.classList.contains("datetime-picker")) {
        input.value = "";
    } else if (input.classList.contains("time-picker")) {
        input.value = "";
    } else if (input.classList.contains("toggle-switch")) {
        input.checked = false;
    } else if (input.tagName === "TEXTAREA") {
        input.value = "";
    } else if (input.tagName === "SELECT") {
        input.selectedIndex = 0;
    } else {
        // default text, date, time, hidden, etc.
        input.value = "";
    }
}

// Funzione per riempire i controlli di un TagHelper in base alla classe dell'input
function fillTagHelper(tagId, value) {
    try {
        const input = document.getElementById(tagId);
        if (!input) return;

        //controlli taghelper 
        if (input.classList.contains("autocomplete-input")) {   //div.autocomplete-wrapper (taghelper)
            etkAutocompletePreSelectString(input, value); // Inizializza input e contenitore selezioni
        } else if (input.classList.contains("form-check-input")) {  //div.switch-group (taghelper)
            etkSwitchGroupPreSelectString(input, value); // Inizializza input e checcked
        } else if (input.classList.contains("date-range-input")) { //div.date-range (taghelper)
            alert("funzione date-range non implementata");

            //controlli standard 
        } else if (input.classList.contains("datetime-picker")) {
            input.value = value;
        } else if (input.classList.contains("time-picker")) {
            input.value = value;
        } else if (input.classList.contains("toggle-switch")) {
            input.checked = false;
        } else if (input.tagName === "TEXTAREA") {
            input.value = value;
        } else if (input.tagName === "SELECT") {
            const option = Array.from(input.options).find(o => o.value === value);
            if (option) option.selected = true;
        } else {
            // default text, date, time, hidden, etc.
            input.value = value;
        }
    }
    catch (err) {
        console.error("❌ ERRORE in fillTagHelper per", tagId, err);
        etkAddFieldError(tagId, "Errore nel caricamento del valore.");
    }

}




// Funzione per ottenere il valore selezionato di un TagHelper in base alla classe dell'input
function getSelectedValue(tagId) {
    const input = document.getElementById(tagId);
    if (!input) return "";

    //controlli taghelper 
    if (input.classList.contains("autocomplete-input")) {   //div.autocomplete-wrapper (taghelper)
        const arr = etkAutocompleteGetChoices(input);
        return (arr && arr.length > 0) ? arr[0].value : "";
    } else if (input.classList.contains("form-check-input")) {  //div.switch-group (taghelper)
        const arr = etkSwitchGroupGetChoices(input);
        return (arr && arr.length > 0) ? arr[0].value : "";
    } else if (input.classList.contains("date-range-input")) { //div.date-range (taghelper)
        alert("funzione date-range non implementata");
        return "";

        //controlli standard 
    } else if (input.classList.contains("datetime-picker")) {
        return input.value || "";
    } else if (input.classList.contains("time-picker")) {
        return input.value || "";
    } else if (input.classList.contains("toggle-switch")) {
        return input.value || "";
    } else if (input.tagName === "TEXTAREA") {
        return input.value || "";
    } else if (input.tagName === "SELECT") {
        return input.value || "";
    } else {
        // default text, date, time, hidden, etc.
        return input.value || "";
    }
}
// Funzione per ottenere la label selezionata di un TagHelper in base alla classe dell'input
function getSelectedLabel(tagId) {
    const input = document.getElementById(tagId);
    if (!input) return "";

    //controlli taghelper 
    if (input.classList.contains("autocomplete-input")) {   //div.autocomplete-wrapper (taghelper)
        const arr = etkAutocompleteGetChoices(input);
        return (arr && arr.length > 0) ? arr[0].label.replace(/\{\}/g, ",\n") : "";
    } else if (input.classList.contains("form-check-input")) {  //div.switch-group (taghelper)
        const arr = etkSwitchGroupGetChoices(input);
        return (arr && arr.length > 0) ? arr[0].label : "";
    } else if (input.classList.contains("date-range-input")) { //div.date-range (taghelper)
        alert("funzione date-range non implementata");
        return "";

        //controlli standard 
    } else if (input.classList.contains("datetime-picker")) {
        return input.value || "";
    } else if (input.classList.contains("time-picker")) {
        return input.value || "";
    } else if (input.classList.contains("toggle-switch")) {
        return input.value || "";
    } else if (input.tagName === "TEXTAREA") {
        return input.value || "";
    } else if (input.tagName === "SELECT") {
        return input.value || "";
    } else {
        // default text, date, time, hidden, etc.
        return input.value || "";
    }
}

//function reapplyStriping(table) { // Forza il reapplicamento della striscia alternata
//    const allRows = table.querySelectorAll("tbody tr");
//    // Filtra solo le righe visibili
//    const visibleRows = Array.from(allRows).filter(row => { return row.offsetParent !== null; });  // Esclude display:none
//    // Applica striping solo alle visibili
//    visibleRows.forEach((row, index) => {
//        row.classList.remove("table-active");
//        if (index % 2 === 0) {
//            row.classList.add("table-active");
//        }
//    });
//}

// ***********************************************************************************************

// ***********************************************************************************************
// SHOW TOASTS & FIELDERRORS

function showAlert(message, type = "info") {
    console.log("Toast:", type, message);

    if (typeof message !== "string") return;
    const text = message.trim();
    if (text.length === 0) return;

    showSystemAlert(message, type); // se già usi Bootstrap Toast puoi richiamarlo qui
    //alert(text); // fallback minimale (sostituiscilo con il tuo toast)
}
function showDelayToast(message, type = "info") {

    const container = document.getElementById("toast-container");
    if (!container) return;

    const toastEl = document.createElement("div");
    toastEl.className = `toast align-items-center text-bg-${type} border-0`;
    toastEl.setAttribute("role", "alert");
    toastEl.setAttribute("aria-live", "assertive");
    toastEl.setAttribute("aria-atomic", "true");

    toastEl.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto"
                    data-bs-dismiss="toast"></button>
        </div>
    `;

    container.appendChild(toastEl);

    const toast = new bootstrap.Toast(toastEl, { delay: 4000 });
    toast.show();

    toastEl.addEventListener("hidden.bs.toast", () => toastEl.remove());
}
function showSystemAlert(message, type) {

    if (!message || !message.trim()) return;

    const overlay = document.getElementById("overlay-alert");
    const box = overlay.querySelector(".overlay-alert-box");
    const msg = document.getElementById("overlay-alert-message");
    const icon = document.getElementById("overlay-alert-icon");
    const btn = document.getElementById("overlay-alert-close");

    // reset
    overlay.className = "overlay-alert";
    overlay.classList.add(type);

    msg.innerText = message;

    // icone coerenti (Bootstrap Icons o emoji)
    const icons = {
        info: "ℹ️",
        success: "✅",
        warning: "⚠️",
        error: "❌"
    };

    icon.innerText = icons[type] || "";

    overlay.classList.remove("d-none");

    // chiusura
    btn.onclick = hide;
    overlay.onclick = e => {
        if (e.target === overlay) hide();
    };

    document.addEventListener("keydown", escHandler);

    function escHandler(e) {
        if (e.key === "Escape") hide();
    }

    function hide() {
        overlay.classList.add("d-none");
        document.removeEventListener("keydown", escHandler);
    }
}

function showFieldErrors(container, fieldErrors) {

    clearFieldErrors(container);

    fieldErrors.forEach(err => {

        const fieldName = err.field;

        const input = container.querySelector(`[name="${CSS.escape(fieldName)}"]`);
        const msg = container.querySelector(`[data-valmsg-for="${CSS.escape(fieldName)}"]`);

        input?.classList.add("is-invalid");

        if (msg) {
            msg.innerText = err.message;
            msg.classList.add("text-danger");
        }
    });
}
function clearFieldErrors(container) {
    container.querySelectorAll(".is-invalid")
        .forEach(e => e.classList.remove("is-invalid"));

    container.querySelectorAll("[data-valmsg-for]")
        .forEach(e => e.innerText = "");
}



// ***********************************************************************************************
// SHOW-HIDE PAGE BLOCKER

function showPageBlocker() {
    const blocker = document.getElementById('page-blocker');
    if (blocker) blocker.style.display = 'block';
}
function hidePageBlocker() {

    var checkAllTagHelpers = true; // controlla che tutti i TagHelpers siano caricati prima di nascondere il blocco della pagina
    if (checkAllTagHelpers) {
        // Avvia il controllo periodico se ci sono campi asp-loaded
        if (allLoaded() === false) {
            if (window.pageLoadChecker) clearInterval(window.pageLoadChecker);
            window.pageLoadChecker = setInterval(checkAllFieldsLoaded, 300);
        } else {
            // Nessun campo con gestione asp-loaded !== "N"
            // nascondo il blocco della pagina
            const blocker = document.getElementById('page-blocker');
            if (blocker) blocker.style.display = 'none';
        }
    }
    else {
        // nascondo il blocco della pagina
        const blocker = document.getElementById('page-blocker');
        if (blocker) blocker.style.display = 'none';
    }

    // Funzione di verifica caricamento completo
    function checkAllFieldsLoaded() {
        if (allLoaded()) {
            console.log("✅ Tutti i campi sono caricati");
            clearInterval(window.pageLoadChecker);
            // nascondo il blocco della pagina
            const blocker = document.getElementById('page-blocker');
            if (blocker) blocker.style.display = 'none';
        }
    }
    function allLoaded() {
        const components = document.querySelectorAll("[asp-loaded]");
        if (components.length === 0) return true;  // nessun controllo gestito
        for (const el of components) {
            if (el.getAttribute("asp-loaded") !== "Y")
                return false;
        }
        return true;
    }


}


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

function updateModalWithContentForm(button, prefix, modalDialogId, jsonObject) {
    var fullprefix = prefix + ".";
    if (typeof jsonObject === 'string') { jsonObject = JSON.parse(jsonObject); }  //verifica che jsonObject sia un oggetto json

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
        if (key.startsWith(fullprefix)) {
            key = key.substring(fullprefix.length);
        }
        // Aggiunge anche chiavi nuove
        setJsonValue(jsonObject, key, value);
    });

    openModalWithContent(modalDialogId, modalAction, {
        'data': jsonObject
    });
}
function setJsonValue(obj, keyPath, value) {
    const keys = keyPath.replace(/\[(\d+)\]/g, '.$1').split('.'); // es: "xrefFrom.PcIdPrestazione[0].PcIdCampione" → ["xrefFrom", "PcIdPrestazione", "0", "PcIdCampione"]
    let current = obj;

    for (let i = 0; i < keys.length; i++) {
        const key = keys[i];

        if (i === keys.length - 1) {
            // Ultimo elemento: assegna il valore
            current[key] = value;
        } else {
            const nextKey = keys[i + 1];
            const isArrayIndex = !isNaN(nextKey);

            if (!(key in current)) {
                current[key] = isArrayIndex ? [] : {};
            }

            if (isArrayIndex && !Array.isArray(current[key])) {
                current[key] = [];
            }

            current = current[key];
        }
    }
}


//---------------------------

function updateModalWithContentForm2(button, prefix, modalDialogId) {
    var fullprefix = prefix + ".";
    var jsonObject = JSON.parse("{}");  //oggetto json vuoto

    // Trova il form più vicino al pulsante
    let form = button.closest('form');
    if (!form) {
        console.error("Nessun form trovato!");
        return;
    }
    let modalAction = form.action;
    // Usa FormData per raccogliere i dati modificati
    let formData = new FormData(form);
    // Rimuovo i prefissi e aggiorno l'oggetto jsonObject => trasformo FormData in JSON
    formData.forEach((value, key) => {
        if (key.startsWith(fullprefix)) {
            key = key.substring(fullprefix.length);
        }
        // Aggiunge anche chiavi nuove
        setJsonValue(jsonObject, key, value);
    });

    openModalWithContent_int(modalDialogId, modalAction, { 'data': jsonObject }, false); // <-- asForm=false
}


//---------------------------

function openModalWithContent(modalDialogId, modalAction, jsonParams) { openModalWithContent_int(modalDialogId, modalAction, jsonParams, false); }
function openModalWithContent_int(modalDialogId, modalAction, jsonParams, asForm = false) {

    console.log('openModalWithContent.modalDialogId:', modalDialogId); // ✅ LOG
    console.log('openModalWithContent.modalAction:', modalAction); // ✅ LOG
    console.log('openModalWithContent.jsonParams:', jsonParams); // ✅ LOG

    let fetchOptions = {
        method: 'POST',
        headers: {}
    };

    if (asForm) {
        // Caso A: invio come FormData
        let formData = jsonParams instanceof FormData ? jsonParams : new FormData();
        fetchOptions.body = formData;
        // AntiForgeryToken va messo dentro il formData
        if (!formData.has("__RequestVerificationToken")) {
            let token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            formData.append("__RequestVerificationToken", token);
        }
    } else {
        // Caso B: invio come JSON
        fetchOptions.headers["Content-Type"] = "application/json";
        fetchOptions.headers["RequestVerificationToken"] = document.querySelector('input[name="__RequestVerificationToken"]').value;
        fetchOptions.body = JSON.stringify(jsonParams);
    }

    fetch(modalAction, fetchOptions)
        .then(async response => {

            const contentType = response.headers.get("content-type");

            if (contentType && contentType.includes("application/json")) {
                return { isJson: true, data: await response.json() };
            }

            return { isJson: false, data: await response.text() };
        })
        .then(result => {

            // ============================
            // CASO JSON → TOAST / ERRORI
            // ============================
            if (result.isJson) {

                const json = result.data;

                //SUCCESS

                if (json.success) {
                    showDelayToast(json.message ?? "Operazione completata", "success");
                    $('#' + modalDialogId).modal('hide');
                    return;
                }

                //ERROR

                // errore globale
                if (json.message) {
                    showAlert(json.message ?? "Correggi i campi", "warning");
                }
                // errori di campo
                if (json.fieldErrors) {
                    const modal = document.getElementById(modalDialogId);
                    showFieldErrors(modal, json.fieldErrors);
                }
                return;
            }

            // ============================
            // CASO HTML → PARTIAL
            // ============================

            const html = result.data;

            console.log('Contenuto HTML caricato nella modale:', html); // ✅ LOG

            if (html === undefined || html == null || html == "") {
                console.error("Errore nel caricamento della modale: la risposta HTML è vuota o nulla. Return state: " + result.status);
                showAlert("Errore nel caricamento della modale: la risposta HTML è vuota o nulla. Return state: " + result.status);
                return;
            }

            // PATCH — intercetta errori del server
            if (html.includes("System.") && html.includes("Exception")) {

                console.error("Errore server nella modale:", html);

                // Estrae solo le prime righe dell’errore per il popup
                let textErr = html
                    .replace(/<[^>]+>/g, '')      // rimuovi HTML
                    .split('\n')                  // separa righe
                    .map(x => x.trim())           // togli spazi
                    .filter(x => x.length > 0);   // rimuovi righe vuote

                // prende massimo 5 righe
                let shortErr = textErr.slice(0, 5).join('<br>');

                showAlert(
                    "❌ Errore durante il caricamento della finestra.\n\n" +
                    "Dettaglio tecnico (solo prime righe):\n" +
                    "---\n" + shortErr + "\n---",
                    "danger"
                );

                return;
            }



            document.getElementById(modalDialogId).innerHTML = html;  //inserisco contenuto in dialog modale

            //AZIONI DA FARE AL CLICK BOTTONE
            var isModalACTION_CLOSE = $('#' + modalDialogId).find('[name$="IsModalACTION"]').val() == 'CLOSE';
            if (isModalACTION_CLOSE == true) { $('#' + modalDialogId).modal('hide'); } //nascondi modal
            else {

                try {
                    //alert('inizio modale');
                    showPageBlocker(); // Mostra il blocco della pagina durante il caricamento della modale
                    //$('#' + modalDialogId).modal('show'); // Se la modale non è già visibile, la mostro
                    //$('#' + modalDialogId).on('shown.bs.modal', function () {
                    //    hidePageBlocker();
                    //    //alert('fine modale');
                    //}); // Rimuove l’overlay quando la modale è completamente visibile

                    // Se la modale non è visibile → la apro normalmente
                    if (!$('#' + modalDialogId).hasClass('show')) {
                        document.activeElement?.blur(); // sposta il focus su un elemento neutro prima di aprire
                        $('#' + modalDialogId).off('shown.bs.modal').on('shown.bs.modal', function () {
                            hidePageBlocker();
                        });
                        $('#' + modalDialogId).modal('show');
                    }
                    // Se la modale è già visibile → aggiorno solo il contenuto
                    else {
                        hidePageBlocker(); // nascondo l’overlay appena finito il refresh
                    }

                } catch (e) {
                    console.error("Errore durante modal('show'):", e);  // ✅ LOG "Illegal invocation" in Bootstrap crash on querySelector.call([object Object], ...)
                    hidePageBlocker(); // Rimuove l’overlay in ogni caso
                }

            } //mostra modal
            var isPageACTION_RELOAD = $('#' + modalDialogId).find('[name$="IsPageACTION"]').val() == 'RELOAD';
            if (isPageACTION_RELOAD == true) { location.reload(true); } //ricarica pagina dal server (ie: no cache)
            var isPageREDIRECT = $('#' + modalDialogId).find('[name$="IsPageREDIRECT"]').val();
            if (isPageREDIRECT != undefined && isPageREDIRECT != "") { location.href = isPageREDIRECT; } //ridireziona su altra pagina

            // Una volta completato il caricamento della PartialView
            initializeAfterLoadPageAndPartial(); // Richiama la funzione anche dopo il caricamento della PartialView

        })
        .catch(error => {
            console.error('Errore:', error);
            alert('Errore durante il caricamento della finestra modale: ' + error);
        });
}



// ***********************************************************************************************
// GENERA ICODE

async function generateIcode(controlName) {
    try {
        const response = await $.get('/ErpUtilities/GenerateIcode');
        if (response.error) {
            if (controlName) showValidationMessage(controlName, response.error);
            return null;
        }
        console.log('generateIcode:', response.icode);
        return response.icode;
    } catch (error) {
        console.error("Errore nella generazione dell'icode:", error);
        if (controlName) showValidationMessage(controlName, error);
        return null;
    }
}

// ***********************************************************************************************
// VARIABILI DI MODELLO ...in abbinamento a HiddenVarsTagHelper (ErpComponentTagHelper)

//// VARS etk TOOLS

// Recpera la varibile di modello associata all'attuale tagHtmlName
function etkRelatedHiddenVarsGet(tagHtmlName, key) {
    const varTagHtmlName = `${etkToolsExtractPathWithoutProperty(tagHtmlName)}vars`;
    const varKey = `@${etkToolsExtractPropertyOnly(tagHtmlName)}-${key}`;
    return etkHiddenVarsGet(varTagHtmlName, varKey);
}
// Aggiunge/Modifica la varibile di modello associata all'attuale tagHtmlName
function etkRelatedHiddenVarsPut(tagHtmlName, key, value, createIfMissing = false) {
    const varTagHtmlName = `${etkToolsExtractPathWithoutProperty(tagHtmlName)}vars`;
    const varKey = `@${etkToolsExtractPropertyOnly(tagHtmlName)}-${key}`;
    etkHiddenVarsPut(varTagHtmlName, varKey, value, createIfMissing);
}
// Accoda chiave alla varibile di modello associata all'attuale tagHtmlName
function etkRelatedHiddenVarsAppendValue(tagHtmlName, key, value, createIfMissing = false) {
    const varTagHtmlName = `${etkToolsExtractPathWithoutProperty(tagHtmlName)}vars`;
    const varKey = `@${etkToolsExtractPropertyOnly(tagHtmlName)}-${key}`;
    etkHiddenVarsAppendValue(varTagHtmlName, varKey, value, createIfMissing);
}
// Cancella chiave alla varibile di modello associata all'attuale tagHtmlName
function etkRelatedHiddenVarsDeleteValue(tagHtmlName, key, value) {
    const varTagHtmlName = `${etkToolsExtractPathWithoutProperty(tagHtmlName)}vars`;
    const varKey = `@${etkToolsExtractPropertyOnly(tagHtmlName)}-${key}`;
    etkHiddenVarsDeleteValue(varTagHtmlName, varKey, value);
}

////--------


const etkSeparatoreDiLista = "~~";

/**
 * Recupera il valore di una chiave dal campo JSON hidden
 * Esempio: varTagHtmlName = "Customer.Address[0].vars", varKey = "Lang"
 */
function etkHiddenVarsGet(varTagHtmlName, varKey) {
    const input = document.querySelector(
        `input[type="hidden"][name="${varTagHtmlName}"]`
    );

    if (!input || !input.value)
        return null;

    try {
        let inputValue = input.value;
        //verifico se il valore è codificato in base64url
        var result = etkTryFromBase64Url(inputValue); 
        if (result.ok) { inputValue = result.value; }
        //parse json
        const json = JSON.parse(inputValue);
        if (json === null) return null;
        return json[varKey] ?? null;
    }
    catch (err) {
        console.error("❌ JSON PARSE ERROR in ModelVars:");
        console.error(" • Campo HTML Name:", varTagHtmlName);
        console.error(" • Key:", varKey);
        console.error(" • Valore contenuto:", input.value);
        console.error(" • Errore:", err);

        // Segnala errore nel campo associato
        etkAddFieldError(varTagHtmlName, varKey, "Errore nei dati interni (JSON non valido).");

        return null; // Previene cascata di errori
    }
}
function etkAddFieldError(varTagHtmlName, varKey, message) {

    // Es: "XrefPcIdPrestazione[1].vars" → "XrefPcIdPrestazione[1].PcIdTipoCampione"
    // Qui devi applicare la tua logica per mappare 'vars' → campo reale
    const fieldName = etkEstraiTraAtEDash(varKey); // estrae "PcIdTipoCampione" da "@PcIdTipoCampione-qualcosa"
    const baseField = varTagHtmlName.replace(".vars", "." + fieldName);

    const span = document.querySelector(
        `span[data-valmsg-for="${baseField}"]`
    );

    if (span) {
        span.textContent = message;
        span.classList.add("text-danger");
    }

    // Log extra
    console.warn("⚠ Validazione campo fallita:", baseField, message);
}
function etkEstraiTraAtEDash(str) {
    // Trova la prima '@' (se non c’è, usa 0)
    let start = str.indexOf('@');
    if (start === -1) start = 0;
    else start += 1; // non includere '@'
    // Trova l’ultimo '-' (se non c’è, usa la lunghezza)
    let end = str.lastIndexOf('-');
    if (end === -1) end = str.length;
    // Estrai e restituisci il risultato
    return str.substring(start, end);
}



/**
 * Inserisce o aggiorna una chiave nel campo JSON hidden
 * Se non esiste, lo crea e lo aggiunge al body
 */
function etkHiddenVarsPut(varTagHtmlName, varKey, value, createIfMissing = false) {
    let input = document.querySelector(`input[type="hidden"][name="${varTagHtmlName}"]`);

    let json = {};
    if (input && input.value) {
        try {
            let inputValue = input.value;
            //verifico se il valore è codificato in base64url
            var result = etkTryFromBase64Url(inputValue);
            if (result.ok) { inputValue = result.value; }
            //parse json
            if (inputValue !== 'null') json = JSON.parse(inputValue);
        } catch (e) {
            console.warn("etkHiddenVarsPut: Invalid JSON in hidden input:", input.value);
        }
    }

    // Se l'input non esiste e non vogliamo crearlo, esci
    if (!input && !createIfMissing) {
        return;
    }

    json[varKey] = value;

    if (!input && createIfMissing) {
        input = document.createElement("input");
        input.type = "hidden";
        input.name = varTagHtmlName;
        input.id = etkToolsGenerateHtmlId(varTagHtmlName);
        input.setAttribute("data-format", "JSON");
        document.body.appendChild(input);
    }

    if (input) {
        input.value = etkToBase64Url(JSON.stringify(json));
    }
}
/**
 * Inserisce o aggiorna una chiave nel campo JSON hidden
 * Se la variabile già esiste, accoda il valore con "~~"
 */
function etkHiddenVarsAppendValue(varTagHtmlName, varKey, value, createIfMissing = false) {
    if (value === null) { return; }
    const valueList = etkHiddenVarsGet(varTagHtmlName, varKey);
    if (valueList === null) {
        etkHiddenVarsPut(varTagHtmlName, varKey, value.toString().trim(), createIfMissing);
        return;
    }
    etkHiddenVarsPut(varTagHtmlName, varKey, valueList.trim() + etkSeparatoreDiLista + value.toString().trim(), createIfMissing);  // accoda con etkSeparatoreDiLista
}
/**
 * Elimina la chiave dalla lista separata da etkSeparatoreDiLista, se esiste
 */
function etkHiddenVarsDeleteValue(varTagHtmlName, varKey, value) {
    if (value === null) { return; }
    const trimmedValue = value.trim();
    const valueList = etkHiddenVarsGet(varTagHtmlName, varKey);
    if (valueList !== null) {
        const values = valueList
            .split(etkSeparatoreDiLista)
            .map(v => v.trim())
            .filter(v => v !== trimmedValue);

        const updatedValueList = values.join(etkSeparatoreDiLista);
        etkHiddenVarsPut(varTagHtmlName, varKey, updatedValueList, true); // true per createIfMissing
    }
}


/**
 * Converte un nome di proprietà in un ID HTML valido
 * Esempio: "Customer.Address[0].Street" → "Customer_Address_0_Street"
 */
function etkToolsGenerateHtmlId(tagHtmlName) {
    return tagHtmlName.replace(/\./g, '_').replace(/\[/g, '_').replace(/\]/g, '_');
}
/**
 * Estrae tutto dal name tranne il nome della proprietà finale
 * Esempio: "Customer.Address[0].Street" → "Customer.Address[0]"
 */
function etkToolsExtractPathWithoutProperty(tagHtmlName) {
    const lastDotIndex = tagHtmlName.lastIndexOf(".");
    return lastDotIndex !== -1 ? tagHtmlName.substring(0, lastDotIndex + 1) : "";
}
/**
 * Estrae solo il nome della proprietà finale da un tagHtmlName
 * Esempio: "Customer.Address[0].Street" → "Street"
 */
function etkToolsExtractPropertyOnly(tagHtmlName) {
    const lastDotIndex = tagHtmlName.lastIndexOf(".");
    return lastDotIndex !== -1
        ? tagHtmlName.substring(lastDotIndex + 1)
        : tagHtmlName;
}

//############################################################################################################
// UTIL BASE 64

// ========================
// UTF-8 helpers
// ========================
const etkEnc = new TextEncoder();
const etkDec = new TextDecoder();

/**
 * String → Base64 (standard, RFC 4648)
 */
function etkToBase64(str) {
    if (str == null || str === "") return "";
    const bytes = etkEnc.encode(str);
    // Convert bytes to binary string for btoa
    let bin = "";
    for (let i = 0; i < bytes.length; i++) bin += String.fromCharCode(bytes[i]);
    return btoa(bin);
}

/**
 * Base64 (standard) → String (UTF-8)
 */
function etkFromBase64(b64) {
    if (!b64) return "";
    const bin = atob(b64);
    const bytes = new Uint8Array(bin.length);
    for (let i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i);
    return etkDec.decode(bytes);
}

/**
 * Check se sembra Base64 standard valida (con decode di prova)
 */
function etkIsBase64(b64) {
    if (!b64 || typeof b64 !== "string") return false;
    if (b64.length % 4 !== 0) return false;
    const re = /^[A-Za-z0-9+/]+={0,2}$/;
    if (!re.test(b64)) return false;
    try { etkFromBase64(b64); return true; } catch { return false; }
}

// ========================
// Base64 URL-safe
// ========================

/**
 * String → Base64 URL-safe (senza padding "=")
 */
function etkToBase64Url(str) {
    const b64 = etkToBase64(str);
    return b64.replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/g, "");
}

/**
 * Base64 URL-safe → String
 */
function etkFromBase64Url(b64url) {
    if (!b64url) return "";
    let b64 = b64url.replace(/-/g, "+").replace(/_/g, "/");
    // Ripristina padding
    const mod4 = b64.length % 4;
    if (mod4 === 2) b64 += "==";
    else if (mod4 === 3) b64 += "=";
    else if (mod4 !== 0) throw new Error("Base64Url non valido");
    return etkFromBase64(b64);
}

/**
 * Check Base64 URL-safe
 */
function etkIsBase64Url(b64url) {
    if (!b64url || typeof b64url !== "string") return false;
    if (!/^[A-Za-z0-9\-_]+$/.test(b64url)) return false;
    try { etkFromBase64Url(b64url); return true; } catch { return false; }
}

// ========================
// Safe wrappers (try/catch)
// ========================
function etkTryFromBase64(b64) {
    try { return { ok: true, value: etkFromBase64(b64) }; }
    catch (e) { return { ok: false, error: e }; }
}

function etkTryFromBase64Url(b64url) {
    try { return { ok: true, value: etkFromBase64Url(b64url) }; }
    catch (e) { return { ok: false, error: e }; }
}



// ***********************************************************************************************
// SCELTA SINGOLA O MULTIPLA ...chiamate da SwitchGroupTagHelper (ErpComponentTagHelper)


// SWITCH-GROUP etk TOOLS
function etkSwitchGroupPreSelect(inputDOM, valuesArray) {
    var group = document.querySelectorAll(`input[name="${inputDOM.name}"]`);
    group.forEach(function (item) {
        if (valuesArray.includes(item.value)) {
            item.checked = true;
        }
    });
    //    if (valuesArray.includes(inputDOM.value)) {
    //        inputDOM.checked = true;
    //    }
}
function etkSwitchGroupPreSelectString(inputDOM, str) {
    var group = document.querySelectorAll(`input[name="${inputDOM.name}"]`);
    group.forEach(function (item) {
        item.checked = (item.value === str);
    });
    //    inputDOM.checked = (inputDOM.value === str);
}
function etkSwitchGroupClean(inputDOM) {
    var group = document.querySelectorAll(`input[name="${inputDOM.name}"]`);
    group.forEach(function (item) {
        item.checked = false;
    });
    //    inputDOM.checked = false;
}

function etkSwitchGroupGetChoices(inputDOM) {
    const container = inputDOM.closest('.switch-group');
    if (!container) return [];

    //const checked = container.querySelectorAll('input[type="checkbox"]:checked');  //!!! il controllo non è di tipo checkbox ma può anche essere di tipo radio
    const checked = container.querySelectorAll(`input[name="${inputDOM.name}"]:checked`);
    return Array.from(checked).map(item => {
        const clone = item.cloneNode(true);
        const labelItem = container.querySelector(`label[for="${clone.id}"]`);
        return {
            value: clone.value || "",
            label: labelItem ? labelItem.textContent.trim() : ""
        };
    });
}




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

// AUTOCOMPLETE etk TOOLS
function etkAutocompletePreSelect(inputDOM, valuesArray) {
    var input = (inputDOM instanceof jQuery) ? inputDOM : $(inputDOM); // Wrappa l'elemento DOM in jQuery

    //---Blocco iniziale
    input.attr('asp-loaded', 'Y');   // <<<<<<<------- per il momrnto non lego il check del caricamento della pagina a un controllo richiamato da js
    input.prop("disabled", "disabled"); //disabilita input per caricamento
    //---

    input.data('pre-selected', { preSelected: valuesArray });  // !!! devo sempre lavorare con un elemento jQuery quando modifico gli attributi di input
    etkAutocompleteInitialize(input);
}
function etkAutocompletePreSelectString(inputDOM, str) {
    const arr = new Array(); arr.push(str);
    etkAutocompletePreSelect(inputDOM, arr);
}
function etkAutocompleteClean(inputDOM) {
    const arr = new Array();
    etkAutocompletePreSelect(inputDOM, arr);
}
function etkAutocompleteGetChoices(inputDOM) {
    var input = (inputDOM instanceof jQuery) ? inputDOM : $(inputDOM); // Wrappa l'elemento DOM in jQuery
    var selectedItemsDivId = input.data('selected-items-div-id');
    var selectedItemsDiv = $('#' + selectedItemsDivId);
    if (!selectedItemsDiv) { const arr = new Array(); return arr; } // return empty array
    return Array.from(selectedItemsDiv.find('.selected-item'))
        .map(item => {
            // Cloniamo l'elemento per non toccare il DOM reale
            const clone = item.cloneNode(true);
            const remove = clone.querySelector(".remove-item");
            if (remove) remove.remove(); // elimina la X
            return { value: clone.dataset.value || "", label: clone.dataset.label || "" };
        })
}


//inizializza controllo Autocomplete
function etkAutocompleteInitialize(inputDOM) {
    var input = (inputDOM instanceof jQuery) ? inputDOM : $(inputDOM); // Wrappa l'elemento DOM in jQuery
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

        // Fine gestione loaded, considero qui il TagHelper caricato
        input.attr('asp-loaded', 'Y');
        input.prop("disabled", false); //riabilita input quando caricato
        console.log("Campo", input.data('name'), "caricato");
        //---

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
            if (selectedItemsDiv && selectedItemsDiv[0]) selectedItemsDiv[0].innerHTML = "";  //cancello contenuto selected
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

                                console.log("JSON ricevuto da ajax:", data); // <-- Questo è il JSON già deserializzato

                                data.forEach(function (item) {
                                    if (item && !isItemAlreadySelected(item.value, selectedItemsDiv)) {
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
                var filtered = allChoices.filter(c => c.label && (' ' + c.label.toUpperCase() + ' ').includes(term));
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
        // Cerca il tag span di validazione associato al campo specifico (utilizzando data-valmsg-for)
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
        //----- Rimuovo la label nelle Vars: con key=FieldLabel  -----
        var labelToRemove = $(this).parent().data('label');
        etkRelatedHiddenVarsDeleteValue(input.data('name'), "FieldLabel", labelToRemove);
        //----------------------------------------------------------------------------
        toggleInputVisibility(input, selectedItemsDiv, maxSelections);


        // ▼▼▼ AGGIUNGI: notifica change per ricalcolo mandatory ▼▼▼
        try { (input instanceof jQuery ? input : $(input)).trigger('change'); } catch { }
    });

    function isItemAlreadySelected(value, selectedItemsDiv) {
        //return selectedItemsDiv.find('.selected-item[data-value="' + value + '"]').length > 0;
        return selectedItemsDiv.find('.selected-item').filter(function () { return $(this).data('value') === value; }).length > 0;
    }

    function addSelectedItem(label, value, input, selectedItemsDiv) {
        //        var itemDiv = $('<div class="selected-item" data-value="' + value + '">' + label + ' <span class="remove-item">&times;</span></div>');

        var itemDiv = $('<div>', {
            class: 'selected-item',
            'data-value': value,
            'data-label': label // jQuery gestisce l'escaping
        }).append(label + ' <span class="remove-item">&times;</span>');

        var inputField = $('<input type="hidden" name="' + input.data('name') + '" value="' + value + '" />');
        itemDiv.append(inputField);
        selectedItemsDiv.append(itemDiv);
        //----- Gestione del parametro readonly quando vado ad inserire un item -----
        if (input.data('readonly') == 'Y') {
            selectedItemsDiv.find('.remove-item').remove(); // Rimuove i pulsanti di rimozione
        }
        //----- Gestione della label nelle Vars: con key=FieldLabel  -----
        etkRelatedHiddenVarsAppendValue(input.data('name'), "FieldLabel", label, false);
        //----------------------------------------------------------------------------
        toggleInputVisibility(input, selectedItemsDiv, maxSelections);


        // ▼▼▼ AGGIUNGI: forza evento per clear dell'errore mandatory ▼▼▼
        try { (input instanceof jQuery ? input : $(input)).trigger('change'); } catch { }
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

}

function initializeAfterLoadPageAndPartial() {
    $('.autocomplete-input').each(function () {
        var input = $(this);
        etkAutocompleteInitialize(input);
    });
}

$(document).ready(function () {
    initializeAfterLoadPageAndPartial(); // Chiamata all'inizio del caricamento della pagina
});








// =====================================================================================================
// ===================================  ⚙️  NUOVA SEZIONE: ERP TABLE  ==================================
// =====================================================================================================
// Compatibilità piena tra tabella "vecchia" e nuova <erp-table> (TagHelper).
// - Riconosce il template:  tr#templateRow   (vecchia)
//                          tr#tpl_{prefix}  (nuova)
// - Normalizza i pulsanti Azioni (emoji → bottoni .ModifyRow/.DeleteRow/.SaveRow/.CancelRow)
// - Inizializza delega eventi per TUTTE le tabelle presenti in pagina/modale

//--------
// Init
//--------

// Normalizza i pulsanti di azione nelle celle "Azioni" (nuova <erp-table>)
function etkEnsureActionButtonsMarkup(table) {
    if (!table) return;
    const rows = table.querySelectorAll('tbody > tr');

    rows.forEach((tr) => {
        const tds = tr.querySelectorAll('td');
        if (tds.length === 0) return;
        const lastTd = tds[tds.length - 1];

        // Template row (? ✖) → bottoni Save/Cancel
        if (/^tpl_/.test(tr.id)) {
            if (!lastTd.querySelector('.erptbl-save') && !lastTd.querySelector('.erptbl-cancel')) {
                lastTd.classList.add('action-buttons', 'text-center');
                lastTd.innerHTML = [
                    '<button type="button" class="btn btn-sm btn-success erptbl-save"><i class="bi bi-save"></i></button>',
                    '<button type="button" class="btn btn-sm btn-secondary erptbl-cancel"><i class="bi bi-x-circle"></i></button>'
                ].join(' ');
            }
            return;
        }

        // Righe dati (✏ ?) → bottoni Modify/Delete
        if (!lastTd.querySelector('.erptbl-modify') && !lastTd.querySelector('.erptbl-delete')) {
            lastTd.classList.add('action-buttons', 'text-center');
            lastTd.innerHTML = [
                '<button type="button" class="btn btn-sm btn-outline-primary erptbl-modify"><i class="bi bi-pencil-square"></i></button>',
                '<button type="button" class="btn btn-sm btn-outline-danger erptbl-delete"><i class="bi bi-trash3"></i></button>'
            ].join(' ');
        }
    });
}

// Aggiunge il pulsante "Aggiungi riga" nel <tfoot> per la nuova <erp-table>
function etkEnsureAddButton(table) {
    if (!table) return;
    const tfootCell = table.querySelector('tfoot td');
    if (!tfootCell) return;

    if (tfootCell.querySelector('.erptbl-add')) return;   // già presente

    //const tpl = etkFindTemplateRow(table);
    //const icodeName = tpl?.querySelector('input.ModelIcode')?.name || '';

    const btn = document.createElement('button');
    btn.type = 'button';
    btn.className = 'btn btn-sm btn-success mb-2 erptbl-add';
    btn.innerHTML = '<i class="bi bi-plus-lg"></i> Aggiungi riga';
    //if (icodeName) btn.name = icodeName; // per compatibilità con generateIcode(name)
    const div = document.createElement('div');
    div.className = 'erppager-box';
    div.style = 'position:absolute; right:0; top:0;';
    tfootCell.innerHTML = '';
    tfootCell.appendChild(btn);
    tfootCell.appendChild(div);
}

// Inizializza TUTTE le tabelle in pagina (nuove e vecchie)
function etkTableInit() {
    // Nuove <erp-table>
    document.querySelectorAll('table.erptbl').forEach((tbl) => {
        etkEnsureActionButtonsMarkup(tbl);
        etkEnsureAddButton(tbl);
    });
}


//--------
// MANDATORY: HELPERS
//--------



//--------
// Utilità
//--------

// Trova la riga template della tabella corrente.
function etkFindTemplateRow(scope) {
    const table = scope.closest ? scope.closest('table') : scope;
    if (!table) return document.querySelector('#templateRow');
    return table.querySelector("tbody tr[id^='tpl_']")
    || table.querySelector("#templateRow")
    || document.querySelector('#templateRow');
}
// Riconosce se un <input> è di un tuo TagHelper
function etkIsTagHelperInput(input) {
    if (!input) return false;
    return (
    input.classList.contains('autocomplete-input') ||
    input.classList.contains('form-check-input')   ||
    input.classList.contains('date-range-input')   ||
    input.classList.contains('datetime-picker')    ||
    input.classList.contains('time-picker')        ||
    input.classList.contains('toggle-switch')
    );
}

// Ritorna tutti gli INPUT/SELECT/TEXTAREA “TagHelper” presenti in una riga
function etkGetTagHelperInputsInRow(row) {
    const candidates = row.querySelectorAll('input, select, textarea');
    return Array.from(candidates).filter(etkIsTagHelperInput);
}
    function isUndeleteEnabled(table) {
    return (table?.dataset?.allowUndelete === "true");
}

// 1) Sposta la template row dopo "rowAfter"
(function redefine_moveTemplateRowTo() {
    window.moveTemplateRowTo = function (rowAfter) {
        const table = rowAfter.closest ? rowAfter.closest('table') : null;
        const templateRow = etkFindTemplateRow(table || document);

        if (!templateRow) { console.error('Template row non trovata.'); return false; }
        if (templateRow.dataset.editingRow === "true") {
            alert("È possibile modificare o aggiungere solo una riga alla volta.");
            return false;
        }

        const tbody = (rowAfter.tagName === 'TBODY') ? rowAfter
            : rowAfter.closest('table').querySelector('tbody');

        if (rowAfter.tagName === 'TBODY') tbody.appendChild(templateRow);
        else tbody.insertBefore(templateRow, rowAfter.nextSibling);

        templateRow.style.display = "";
        const actions = templateRow.querySelector(".action-buttons") || templateRow.querySelector("td:last-child");
        if (actions) {
            actions.classList.add('action-buttons', 'text-center');
            const hasButtons = actions.querySelector('.erptbl-save') || actions.querySelector('.erptbl-cancel');
            if (!hasButtons) {
                actions.innerHTML = [
                    '<button type="button" class="btn btn-sm btn-success erptbl-save"><i class="bi bi-save"></i></button>',
                    '<button type="button" class="btn btn-sm btn-secondary erptbl-cancel"><i class="bi bi-x-circle"></i></button>'
                ].join(' ');
            }
        }

        templateRow.dataset.editingRow = "true";
        return true;
    };
})();

// 2) Aggiunta riga
(function redefine_handleAddRow() {
    window.handleAddRow = function (button) {
        const table = button.closest('table');
        const tbody = table.querySelector('tbody');
        const rowAfter = tbody.lastElementChild || tbody;
        if (!moveTemplateRowTo(rowAfter)) return;

        const templateRow = etkFindTemplateRow(table);

        generateIcode(button.name).then(icode => {
            // Pulisce TagHelper
            etkGetTagHelperInputsInRow(templateRow).forEach(input => {
                if (input.id && typeof cleanTagHelper === 'function') cleanTagHelper(input.id);
            });

            // Pulisce campi normali
            const rawInputs = templateRow.querySelectorAll("input, select, textarea");
            rawInputs.forEach(input => {
                if (etkIsTagHelperInput(input)) return;
                if (input.classList.contains("generated-hidden")) return; 
                if (input.classList.contains("DefaultValue")) return; //non cancello il valore delle colonne invisibile con valore pre-impostato: <erp-table-col for="PcIdPrestazione" visible="false" value="@Model.Pr1Icode" /> 
                input.value = "";

                if (input.classList.contains("ModelAction")) input.value = "XA";
                if (input.classList.contains("ModelIcode") && icode) input.value = "";
                if (input.classList.contains("ModelIndex") && icode) input.value = "";
            });

            // usa Icode come KEY
            templateRow.dataset.editKey = icode || "";
        });
    };
})();

// 3) Modifica riga
(function redefine_handleModifyRow() {
    window.handleModifyRow = function (button) {
        const row = button.closest("tr");
        if (!moveTemplateRowTo(row)) return;

        const table = row.closest('table');
        const templateRow = etkFindTemplateRow(table);

        const newKey = row.dataset.newKey || '';
        const tIcode = row.querySelector('.ModelIcode')?.value || '';
        const key = newKey || tIcode || '';
        if (key == '') return;

        const hiddenInputs = row.querySelectorAll('input[type=hidden]');

        // TagHelper
        etkGetTagHelperInputsInRow(templateRow).forEach(input => {
            const targetName = input.name.replace(/\[0\](?!.*\[0\])/, `[${key}]`);
            const hidden = Array.from(hiddenInputs).find(h => h.name === targetName);
            if (hidden && input.id && typeof fillTagHelper === 'function') {
                fillTagHelper(input.id, hidden.value);
            }
        });

        // Campi normali
        const rawInputs = templateRow.querySelectorAll("input, select, textarea");
        rawInputs.forEach(input => {
            if (etkIsTagHelperInput(input)) return;
            if (input.classList.contains("generated-hidden")) return;
            const targetName = input.name.replace(/\[0\](?!.*\[0\])/, `[${key}]`);
            const hidden = Array.from(hiddenInputs).find(h => h.name === targetName);
            if (hidden) input.value = hidden.value;

            if (input.classList.contains("ModelAction")) {
                if (input.value === "A") input.value = "XA";
                else if (input.value === "M") input.value = "XM";
                else if (input.value === "D") input.value = "XD";
                else input.value = "XM";
            }
        });

        row.style.display = "none";
        templateRow.dataset.editKey = key;
    };
})();

// 4) Salvataggio riga
(function redefine_handleSaveRow() {
    window.handleSaveRow = function (button) {
        const table = button.closest('table');
        const tbody = table.querySelector("tbody");
        const templateRow = etkFindTemplateRow(table);


        // --------------- VALIDAZIONE OBBLIGATORI (Add/Modify) ---------------
        const v = etkValidateMandatory(templateRow);
        if (!v.ok) {
            alert("È necessario compilare tutti i campi evidenziati");
            // focus sul primo errore
            ////////try { v.firstError?.focus(); } catch { }
            try {
                v.firstError?.focus();
                v.firstError?.scrollIntoView({ behavior: 'smooth', block: 'center' });
            } catch { }
            return; // BLOCCA il salvataggio
        }
        // --------------------------------------------------------------------


        const key = templateRow.dataset.editKey || '';
        if (key == '') return;

        const clonedRow = templateRow.cloneNode(true);
        clonedRow.removeAttribute("id");
        clonedRow.removeAttribute("style");
        clonedRow.removeAttribute("data-template");
        clonedRow.removeAttribute("data-editing-row");
        clonedRow.removeAttribute("data-edit-key");
        clonedRow.dataset.newKey = key;

        const actionInput = clonedRow.querySelector(".ModelAction");
        if (actionInput) {
            if (actionInput.value === "XA") actionInput.value = "A";
            else if (actionInput.value === "XM") actionInput.value = "M";
            else if (actionInput.value === "XD") actionInput.value = "D";
        }
        const icodeInput = clonedRow.querySelector(".ModelIcode"); if (icodeInput) icodeInput.value = key;
        const indexInput = clonedRow.querySelector(".ModelIndex"); if (indexInput) indexInput.value = key;

        // TagHelper → hidden + span
        etkGetTagHelperInputsInRow(clonedRow).forEach(input => {
            if (input.classList.contains("ModelVars")) return;

            const hidden = document.createElement("input");
            hidden.type = "hidden";
            input.classList.forEach(cls => { if (cls !== "generated-hidden") hidden.classList.add(cls); });
            hidden.classList.add("generated-hidden");
            hidden.name = input.name.replace(/\[0\](?!.*\[0\])/, `[${key}]`);
            hidden.value = (typeof getSelectedValue === 'function' ? getSelectedValue(input.id) : "") || input.value || "";

            const span = document.createElement("span");
            span.innerHTML = (typeof getSelectedLabel === 'function' ? getSelectedLabel(input.id) : "") || input.value || "";

            const spanValidation = document.createElement("span");
            spanValidation.classList.add("text-danger");
            spanValidation.setAttribute("data-valmsg-for", hidden.name);

            const formGroup = input.closest("div.form-group") || input.parentElement;
            formGroup.innerHTML = "";
            formGroup.appendChild(span);
            formGroup.appendChild(hidden);
            formGroup.appendChild(spanValidation);
        });

        // Campi normali → hidden + span
        const rawInputs = clonedRow.querySelectorAll("input, select, textarea");
        rawInputs.forEach(input => {
            if (etkIsTagHelperInput(input)) return;
            if (input.classList.contains("generated-hidden")) return;
            if (input.classList.contains("ModelVars")) return;

            const hidden = document.createElement("input");
            hidden.type = "hidden";
            input.classList.forEach(cls => { if (cls !== "generated-hidden") hidden.classList.add(cls); });
            hidden.name = input.name.replace(/\[0\](?!.*\[0\])/, `[${key}]`);
            hidden.value = input.value || "";

            const span = document.createElement("span");
            span.textContent = input.value || "";
            if (input.style.display === "none") span.style.display = "none";

            const formGroup = input.closest("div.form-group");
            if (formGroup) {
                formGroup.innerHTML = "";
                formGroup.appendChild(hidden);
                formGroup.appendChild(span);
            } else {
                const parent = input.parentElement;
                if (parent) { parent.replaceChild(span, input); parent.appendChild(hidden); }
            }
        });

        // ModelVars
        const rawInputs2 = clonedRow.querySelectorAll("input, select, textarea");
        rawInputs2.forEach(input => {
            if (!input.classList.contains("ModelVars")) return;
            const hidden = document.createElement("input");
            input.classList.forEach(cls => { if (cls !== "generated-hidden") hidden.classList.add(cls); });
            hidden.type = "hidden";
            hidden.name = input.name.replace(/\[0\](?!.*\[0\])/, `[${key}]`);
            hidden.value = "{}";
            const varsInput = templateRow.querySelector(".ModelVars");
            if (varsInput) { hidden.value = varsInput.value || "{}"; varsInput.value = "{}"; }

            const formGroup = input.closest("div.form-group");
            if (formGroup) { formGroup.innerHTML = ""; formGroup.appendChild(hidden); }
            else { const parent = input.parentElement; if (parent) parent.appendChild(hidden); }
        });

        // Bottoni azione (normali: modify/delete)
        const actions = clonedRow.querySelector(".action-buttons") || clonedRow.querySelector("td:last-child");
        if (actions) {
            actions.classList.add('action-buttons', 'text-center');
            actions.innerHTML = [
                '<button type="button" class="btn btn-sm btn-outline-primary erptbl-modify"><i class="bi bi-pencil-square"></i></button>',
                '<button type="button" class="btn btn-sm btn-outline-danger erptbl-delete"><i class="bi bi-trash3"></i></button>'
            ].join(' ');
        }

        // Sostituisco o aggiungo
        if (templateRow.dataset.editKey) {
            const targetKey = templateRow.dataset.editKey;
            const oldRow = tbody.querySelector(`tr[data-new-key='${targetKey}']`);
            if (oldRow) oldRow.remove();
        }
        tbody.insertBefore(clonedRow, templateRow);

        templateRow.style.display = "none";
        templateRow.dataset.editingRow = "false";
        templateRow.dataset.editKey = "";

        // NEW: forza sort+paging immediati
        if (window.ErpTables && typeof ErpTables.refresh === 'function') {
            ErpTables.refresh(table);
        }

    };
})();

// 5) Annulla
(function redefine_handleCancelRow() {
    window.handleCancelRow = function (button) {
        const table = button.closest('table');
        const templateRow = etkFindTemplateRow(table);
        const tbody = table.querySelector('tbody');

        if (templateRow.dataset.editKey) {
            const targetKey = templateRow.dataset.editKey;
            const oldRow = tbody.querySelector(`tr[data-new-key='${targetKey}']`);
            if (oldRow) oldRow.style.display = "";
        }
        templateRow.style.display = "none";
        templateRow.dataset.editingRow = "false";
        templateRow.dataset.editKey = "";
    };
})();


// 6) Elimina (con supporto "undelete")
(function redefine_handleDeleteRow() {
    window.handleDeleteRow = function (button) {
        const row = button.closest("tr");
        const table = row.closest('table');
        const allowUndelete = (table?.dataset?.allowUndelete === "true");

        const hiddenAction = row.querySelector('input.ModelAction');
        if (!hiddenAction) { row.remove(); return; }

        if (allowUndelete) {
            // Ricorda l'azione precedente e applica "D"
            row.dataset.prevAction = hiddenAction.value || '';
            hiddenAction.value = 'D';

            // Evidenzia rosso + sbarrato
            row.classList.add('erptbl-deleted');
            row.style.display = ""; // resta visibile

            // Azioni: solo "undelete"
            const actions = row.querySelector(".action-buttons") || row.querySelector("td:last-child");
            if (actions) {
                actions.classList.add('action-buttons', 'text-center');
                actions.innerHTML = '<button type="button" class="btn btn-sm btn-outline-success erptbl-undelete" title="Ripristina"><i class="bi bi-arrow-counterclockwise"></i></button>';
            }
        } else {
            // Comportamento precedente (nascondi)
            hiddenAction.value = 'D';
            row.style.display = "none";
        }

        // NEW: ricalcola pagine dopo hide
        if (window.ErpTables && typeof ErpTables.refresh === 'function') {
            ErpTables.refresh(table);
        }

    };
})();

// 7) Undelete
(function define_handleUndeleteRow() {
    window.handleUndeleteRow = function (button) {
        const row = button.closest("tr");
        const table = row.closest('table');
        const hiddenAction = row.querySelector('input.ModelAction');
        if (!hiddenAction) return;

        const prev = row.dataset.prevAction || '';
        hiddenAction.value = prev || 'M';
        row.dataset.prevAction = '';

        // Rimuovi evidenze rosse e sbarrato
        row.classList.remove('erptbl-deleted');
        row.style.display = "";

        // Bottoni normali
        const actions = row.querySelector(".action-buttons") || row.querySelector("td:last-child");
        if (actions) {
            actions.classList.add('action-buttons', 'text-center');
            actions.innerHTML = [
                '<button type="button" class="btn btn-sm btn-outline-primary erptbl-modify"><i class="bi bi-pencil-square"></i></button>',
                '<button type="button" class="btn btn-sm btn-outline-danger erptbl-delete"><i class="bi bi-trash3"></i></button>'
            ].join(' ');
        }

        // NEW: ricalcola ordinamento/paging
        if (window.ErpTables && typeof ErpTables.refresh === 'function') {
            ErpTables.refresh(table);
        }

    };
})();



// 8) Delega click (se non l'hai già)
document.addEventListener('click', function (ev) {
    const btn = ev.target.closest('.erptbl-modify, .erptbl-delete, .erptbl-undelete, .erptbl-save, .erptbl-cancel, .erptbl-add');
    if (!btn) return;
    if (btn.classList.contains('erptbl-modify'))  return handleModifyRow(btn);
    if (btn.classList.contains('erptbl-delete'))  return handleDeleteRow(btn);
    if (btn.classList.contains('erptbl-undelete'))return handleUndeleteRow(btn);
    if (btn.classList.contains('erptbl-save'))    return handleSaveRow(btn);
    if (btn.classList.contains('erptbl-cancel'))  return handleCancelRow(btn);
    if (btn.classList.contains('erptbl-add'))     return handleAddRow(btn);
});






// =============================
// ERP TABLE: Sort / Filter / Pager (client-only)
// =============================
(function () {

    // ------- Utilities -------
    const HTML_TAG_RE = /<.*?>/g;
    const STRIP_HTML = s => (s || "").replace(HTML_TAG_RE, "");
    const decode = (s) => {
        const div = document.createElement('div');
        div.innerHTML = s || "";
        return div.textContent || div.innerText || "";
    }

    const parseState = (s) => (s || "none").toLowerCase();
    // ciclo stati per sort: none → asc → desc → none
    const SORT_CYCLE = ["none", "asc", "desc"];
    function nextSortState(curr) {
        const i = SORT_CYCLE.indexOf((curr || "none").toLowerCase());
        return SORT_CYCLE[(i + 1) % SORT_CYCLE.length];
    }
    // dir (+1/-1) e priority (1..3 o 9999 per none/asc/desc senza numero)
    function stateToSpec(state) {
        state = parseState(state);
        if (state === "none") return { dir: 0, prio: 9999 };
        if (state === "asc") return { dir: +1, prio: 9999 };
        if (state === "desc") return { dir: -1, prio: 9999 };
        const m = state.match(/^(asc|desc)([123])$/);
        if (m) return { dir: m[1] === "asc" ? +1 : -1, prio: parseInt(m[2], 10) };
        return { dir: 0, prio: 9999 };
    }
    // "asc2" -> { base:"asc", seq:2 } ; "desc" -> { base:"desc", seq:0 } ; "none" -> { base:"none", seq:0 }
    function parseInitialState(str) {
        const s = (str || "none").toLowerCase();
        const m = s.match(/^(asc|desc)(\d+)$/);
        if (m) return { base: m[1], seq: parseInt(m[2], 10) || 0 };
        if (s === "asc" || s === "desc") return { base: s, seq: 0 };
        return { base: "none", seq: 0 };
    }

    // helper per comporre l’icona con eventuale numero
    function iconFor(baseState, seq, isMulti) {
        const num = (n) => n > 0 ? `<sup class="ms-1 small">${n}</sup>` : "";
        switch ((baseState || "none").toLowerCase()) {
            case "asc": return `<i class="bi bi-sort-up"></i>${isMulti ? num(seq) : ""}`;
            case "desc": return `<i class="bi bi-sort-down"></i>${isMulti ? num(seq) : ""}`;
            default: return `<i class="bi bi-arrow-down-up"></i>`; // none
        }
    }
    // trova tutti i <tr> "dati" (esclude la template-row)
    function getDataRows(table) {
        const rows = Array.from(table.querySelectorAll("tbody > tr"));
        return rows.filter(r => r.getAttribute("data-template") !== "1");
    }

    // indice della colonna "for" tra le visibili
    function getColumnIndex(table, colFor) {
        // trova l'header th con data-col-for = colFor e conta la posizione tra i th visibili
        const ths = table.querySelectorAll("thead th");
        let idx = -1, visibleCount = 0;
        for (const th of ths) {
            //xx//if (th.innerText.trim() === "Azioni") continue; // non una colonna dati
            if (th.hasAttribute("data-col-for")) {
                if (th.dataset.colFor === colFor) idx = visibleCount;
                visibleCount++;
            }
        }
        return idx;
    }

    // estrae testo di visualizzazione dal <span> (ignorando HTML)
    function getCellText(row, colIndex) {
        const tds = row.querySelectorAll("td");
        let dataTdIndex = -1, seen = 0;
        for (let i = 0; i < tds.length; i++) {
            // ultima cella è azioni: ha .action-buttons o è l'ultima
            if (tds[i].querySelector('.action-buttons')) continue;
            // nascoste tecniche (style display:none) sono "colonna nascosta", non contano tra le visibili
            if (tds[i].style.display === "none") continue;

            if (seen === colIndex) { dataTdIndex = i; break; }
            seen++;
        }
        if (dataTdIndex === -1) return "";

        const span = tds[dataTdIndex].querySelector("span");
        const raw = span ? span.innerHTML : tds[dataTdIndex].innerHTML;
        return decode(STRIP_HTML(raw)).trim();
    }

    // Applica filtro, sort e paging (in quest'ordine logico: filtro → sort → paging)
    function refreshView(table) {
        const state = table.__erpState__;
        if (!state) return;

        // 1) Righe totali
        const rows = state.allRows = getDataRows(table);

        // 2) FILTRO (se attivo)
        let filtered = rows;
        if (state.filter.enabled && state.filter.text) {
            const needle = state.filter.text.toLowerCase();
            const filtCols = state.columns.filter(c => c.filterable);
            filtered = rows.filter(row => {
                // OR su tutte le colonne filterable
                for (const col of filtCols) {
                    const idx = getColumnIndex(table, col.for);
                    if (idx < 0) continue;
                    const val = getCellText(row, idx).toLowerCase();
                    if (val.includes(needle)) return true;
                }
                return false;
            });
        }

        // 3) SORT (exclusive o multi)
        let sorted = filtered.slice();

        // costruisci elenco colonne 'exclusive' attive (asc/desc)
        const exclusiveActive = state.columns.find(c => c.sortMode === "exclusive" && (c.baseState === "asc" || c.baseState === "desc"));

        if (exclusiveActive) {
            const spec = [{ col: exclusiveActive, dir: (exclusiveActive.baseState === "asc" ? +1 : -1) }];
            sorted.sort((r1, r2) => compareRows(table, r1, r2, spec));
        } else {
            // multi: usa l'ordine globale (illimitato) definito in sorter.order
            const spec = state.sorter.order.map(o => {
                const col = state.columns.find(c => c.for === o.for);
                return { col, dir: o.dir };
            }).filter(s => !!s.col); // difensivo

            if (spec.length > 0) {
                sorted.sort((r1, r2) => compareRows(table, r1, r2, spec));
            }
        }

        state.viewRows = sorted; //st.viewRows = sorted;
        applyPagingAndPaint(table);

        // 4) PAGING
        applyPagingAndPaint(table);
    }

    function compareRows(table, r1, r2, specs) {
        for (const s of specs) {
            const type = s.col.sortType || "string";
            const k1 = getSortKey(table, r1, s.col, type);
            const k2 = getSortKey(table, r2, s.col, type);

            let c = 0;
            if (type === "number" || type === "date" || type === "time" || type === "datetime") {
                c = (k1 < k2) ? -1 : (k1 > k2) ? 1 : 0;
            } else {
                // string & autocomplete
                c = (k1 || "").localeCompare((k2 || ""), undefined, { sensitivity: 'accent', numeric: false });
            }

            if (c !== 0) return s.dir > 0 ? c : -c;
        }
        return 0;
    }

    function applyPagingAndPaint(table) {
        const state = table.__erpState__;
        const rows = state.viewRows || [];

        const tbody = table.querySelector('tbody');
        if (tbody && rows.length > 0) {
            const frag = document.createDocumentFragment();
            rows.forEach(r => frag.appendChild(r));
            tbody.appendChild(frag);
        }

        const pageSize = state.pager.enabled ? state.pager.pageSize : rows.length;
        const totalPages = Math.max(1, Math.ceil(rows.length / Math.max(1, pageSize)));
        state.pager.totalPages = totalPages;
        // correttezza del pageIndex
        state.pager.pageIndex = Math.min(state.pager.pageIndex, totalPages - 1);

        // nascondi tutte
        getDataRows(table).forEach(r => r.style.display = "none");

        // mostra pagina corrente
        const start = state.pager.pageIndex * pageSize;
        const end = state.pager.enabled ? Math.min(start + pageSize, rows.length) : rows.length;
        for (let i = start; i < end; i++) rows[i].style.display = "";

        // aggiorna UI pager
        renderPagerUI(table);
    }

    function getOrderSeqOf(table, colFor) {
        const st = table.__erpState__;
        const i = st.sorter.order.findIndex(x => x.for === colFor);
        return i >= 0 ? (i + 1) : 0;
    }

    function reindexOrder(table) {
        const st = table.__erpState__;

        st.columns.forEach(col => {
            const idx = st.sorter.order.findIndex(o => o.for === col.for);
            col.orderSeq = (idx >= 0 ? idx + 1 : 0);
        });

        updateSortIcons(table);
    }
    function updateSortIcons(table) {
        const st = table.__erpState__;
        st.columns.forEach(c => {
            if (!c.btn) return;
            const isMulti = (c.sortMode === "multi");
            const seq = isMulti ? getOrderSeqOf(table, c.for) : 0;
            c.btn.dataset.state = c.baseState;
            c.btn.innerHTML = iconFor(c.baseState, seq, isMulti);
        });
    }


    // Prova a leggere un value "grezzo" dal TD (se c'è un input hidden lo preferiamo alla label)
    function getCellRawValue(row, colIndex, type) {
        const tds = row.querySelectorAll("td");
        let dataTdIndex = -1, seen = 0;
        for (let i = 0; i < tds.length; i++) {
            if (tds[i].querySelector('.action-buttons')) continue;
            if (tds[i].style.display === "none") continue;
            if (seen === colIndex) { dataTdIndex = i; break; }
            seen++;
        }
        if (dataTdIndex === -1) return "";

        const td = tds[dataTdIndex];

        // 1) preferisci un hidden (valore non formattato) se presente
        if ((type || "string").toLowerCase() !== "autocomplete") {
            const hid = td.querySelector('input[type="hidden"]:not(.ModelVars)');
            if (hid && typeof hid.value === "string") return hid.value.trim();
        }

        // 2) fallback: testo visualizzato (quello che già fai)
        const span = td.querySelector("span");
        const raw = span ? span.innerHTML : td.innerHTML;
        const HTML_TAG_RE = /<.*?>/g;
        const decode = (s) => { const d = document.createElement('div'); d.innerHTML = s || ""; return d.textContent || d.innerText || ""; };
        return decode((raw || "").replace(HTML_TAG_RE, "")).trim();
    }

    // Parser numero: gestisci "1.234,56" e "1,234.56"
    function parseNumberLike(s) {
        s = (s || "").toString().trim();
        if (!s) return { ok: false, n: 0 };
        // prova con virgola decimale
        let t = s.replace(/\./g, '').replace(',', '.');
        let n = Number(t);
        if (!Number.isNaN(n)) return { ok: true, n };
        // prova con punto decimale
        t = s.replace(/,/g, '');
        n = Number(t);
        if (!Number.isNaN(n)) return { ok: true, n };
        return { ok: false, n: 0 };
    }

    // Parser date "furbo": dd/MM/yyyy, yyyy-MM-dd, ecc.
    function parseDateLike(s) {
        s = (s || "").trim();
        if (!s) return { ok: false, t: 0 };
        // ISO o valori compatibili con Date
        let d = new Date(s);
        if (!Number.isNaN(d.getTime())) return { ok: true, t: d.getTime() };
        // dd/MM/yyyy
        const m = s.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
        if (m) {
            const dd = parseInt(m[1], 10), mm = parseInt(m[2], 10) - 1, yy = parseInt(m[3], 10);
            d = new Date(yy, mm, dd, 0, 0, 0, 0);
            if (!Number.isNaN(d.getTime())) return { ok: true, t: d.getTime() };
        }
        return { ok: false, t: 0 };
    }

    // Parser time HH:mm[:ss]
    function parseTimeLike(s) {
        s = (s || "").trim();
        const m = s.match(/^(\d{1,2}):(\d{2})(?::(\d{2}))?$/);
        if (!m) return { ok: false, t: 0 };
        const hh = parseInt(m[1], 10), mi = parseInt(m[2], 10), ss = parseInt(m[3] || "0", 10);
        return { ok: true, t: hh * 3600 + mi * 60 + ss }; // secondi dal giorno
    }

    // Parser datetime: tenta ISO, poi "dd/MM/yyyy HH:mm[:ss]"
    function parseDateTimeLike(s) {
        s = (s || "").trim();
        let d = new Date(s);
        if (!Number.isNaN(d.getTime())) return { ok: true, t: d.getTime() };
        const m = s.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})[ T](\d{1,2}):(\d{2})(?::(\d{2}))?$/);
        if (m) {
            const dd = parseInt(m[1], 10), mm = parseInt(m[2], 10) - 1, yy = parseInt(m[3], 10);
            const hh = parseInt(m[4], 10), mi = parseInt(m[5], 10), ss = parseInt(m[6] || "0", 10);
            d = new Date(yy, mm, dd, hh, mi, ss, 0);
            if (!Number.isNaN(d.getTime())) return { ok: true, t: d.getTime() };
        }
        return { ok: false, t: 0 };
    }

    // Restituisce una "chiave ordinabile" in base al tipo
    function getSortKey(table, row, col, type) {
        const idx = getColumnIndex(table, col.for);           // funzione già presente nel file
        const raw = idx >= 0 ? getCellRawValue(row, idx, type) : ""; // ⟵ usa il raw
        switch ((type || "string").toLowerCase()) {
            case "number": {
                const r = parseNumberLike(raw);
                return r.ok ? r.n : Number.NEGATIVE_INFINITY;
            }
            case "date": {
                const r = parseDateLike(raw);
                return r.ok ? r.t : Number.NEGATIVE_INFINITY;
            }
            case "time": {
                const r = parseTimeLike(raw);
                return r.ok ? r.t : Number.NEGATIVE_INFINITY;
            }
            case "datetime": {
                const r = parseDateTimeLike(raw);
                return r.ok ? r.t : Number.NEGATIVE_INFINITY;
            }
            default:
                // stringa case-insensitive, con accent insensitivity
                return (raw || "").toLocaleLowerCase();
        }
    }


    // ------- UI: Sort toggles in THEAD -------
    function buildSortToggles(table) {
        const ths = table.querySelectorAll("thead th[data-col-for]");
        const columns = [];
        ths.forEach((th, idx) => {
            const colFor = th.dataset.colFor;
            const sortMode = th.dataset.sortMode || "none"; // "multi" | "exclusive" | "none"
            const initial = th.dataset.sortInitial || "none";
            const filterable = th.dataset.filterable === "true";

            // NEW: leggo il tipo dal th; default "string"
            const sortType = (th.dataset.sortType || "string").toLowerCase();   // "string" | "autocomplete" | "number" | "date" | "time" | "datetime"

            const { base, seq } = parseInitialState(initial);
            const colState = {
                for: colFor,
                thIndex: idx,
                sortMode,
                baseState: base,     // <-- stato base "none|asc|desc"
                orderSeq: seq,       // <-- sequenza (0 se non assegnata)
                filterable,
                sortType,     // <-- NEW
                btn: null
            };
            columns.push(colState);

            // mostra toggle solo se sortMode != "none"
            if (sortMode !== "none") {
                const btn = document.createElement("button");
                btn.type = "button";
                btn.className = "btn btn-link btn-sm erpsort-toggle";
                btn.dataset.colFor = colFor;
                btn.dataset.mode = sortMode;
                btn.dataset.state = base;  // solo base-state
                btn.title = "Ordina";

                // icona con numero se multi e seq > 0
                btn.innerHTML = iconFor(base, seq, sortMode === "multi");

                const wrap = document.createElement("span");
                wrap.style.float = "right";
                wrap.appendChild(btn);
                th.appendChild(wrap);

                colState.btn = btn;
            }
        });

        table.__erpState__.columns = columns;

        // inizializza l’ordine globale a partire da eventuali seq iniziali
        const preset = columns
            .filter(c => c.sortMode === "multi" && c.orderSeq > 0 && (c.baseState === "asc" || c.baseState === "desc"))
            .sort((a, b) => a.orderSeq - b.orderSeq)
            .map(c => ({ for: c.for, dir: c.baseState === "asc" ? +1 : -1 }));

        table.__erpState__.sorter.order = preset;

        // normalizza le seq (1..N) in base all’ordine effettivo
        reindexOrder(table);
    }

    function handleSortClick(table, btn) {
        const colFor = btn.dataset.colFor;
        const mode = btn.dataset.mode || "multi";
        const state = btn.dataset.state || "none";
        const st = table.__erpState__;
        const col = st.columns.find(c => c.for === colFor);
        if (!col) return;

        // 1) Cicla stato (none → asc → desc → none)
        const next = nextSortState(state);
        col.baseState = next;

        if (mode === "exclusive") {
            // ============= EXCLUSIVE =============
            // Spegni tutte le altre colonne
            st.columns.forEach(c => {
                if (c.for !== col.for) c.baseState = "none";
            });
            // Ordine globale: solo questa colonna (se attiva)
            st.sorter.order = [];
            if (next === "asc" || next === "desc") {
                st.sorter.order.push({
                    for: col.for,
                    dir: (next === "asc" ? +1 : -1)
                });
            }
            // Ricalcola sequenze e icone, poi refresh
            reindexOrder(table);
            btn.dataset.state = col.baseState;
            updateSortIcons(table);
            refreshView(table);
            return;
        }

        // ============= MULTI (comportamento esistente) =============
        // Ricostruzione completa dell’ordine multi in base agli stati correnti
        st.sorter.order = [];
        // Itera in ordine 1..N di sequenza
        var isColInOrder = false;
        for (let seq = 1; seq <= st.columns.length; seq++) {
            const c = st.columns.find(x => x.orderSeq === seq);
            if (!c) continue;
            if (c.baseState !== "asc" && c.baseState !== "desc") continue;
            st.sorter.order.push({
                for: c.for,
                dir: c.baseState === "asc" ? +1 : -1
            });
            if (c.for === col.for) isColInOrder = true;
        }
        if (!isColInOrder && col.baseState !== "none") {
            st.sorter.order.push({
                for: col.for,
                dir: col.baseState === "asc" ? +1 : -1
            });
        }

        // 3) Ricalcolo sequenze numeriche e icone
        reindexOrder(table);
        // 4) Aggiorna bottone cliccato
        btn.dataset.state = col.baseState;
        // 5) Applica sort e refresh
        refreshView(table);
    }


    // ------- UI: Filter toggle in header Azioni -------
    function buildFilterUI(table) {
        if (table.dataset.editFilter !== "true") return;

        const box = table.querySelector("thead th:last-child .erpfilt-box");
        if (!box) return;

        const btn = document.createElement("button");
        btn.type = "button";
        btn.className = "btn btn-sm btn-outline-secondary erpfilt-toggle";
        btn.title = "Filtro";
        btn.textContent = "Filtro";

        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control form-control-sm erpfilt-input";
        input.placeholder = "Filtra…";
        input.style.display = "none";
        input.style.marginTop = "4px";
        input.maxLength = 128;

        box.appendChild(btn);
        box.appendChild(input);

        btn.addEventListener("click", () => {
            const vis = input.style.display !== "none";
            input.style.display = vis ? "none" : "";
            if (!vis) input.focus();
        });

        input.addEventListener("input", () => {
            table.__erpState__.filter.text = (input.value || "").trim();
            table.__erpState__.filter.enabled = table.__erpState__.filter.text.length > 0;
            refreshView(table);
        });
    }

    // ------- UI: Pager in footer -------
    function buildPagerUI(table) {
        const pageSizeAttr = parseInt(table.dataset.maxLines || "0", 10);
        if (!(pageSizeAttr > 0)) return;

        const box = table.querySelector(".erppager-box");
        if (!box) return;

        // toggle pager + prev/next + status
        box.innerHTML = [
            '<div class="btn-group btn-group-sm" role="group">',
            '<button type="button" class="btn btn-outline-secondary erppager-toggle" title="Abilita/Disabilita paginazione">Pag</button>',
            '<button type="button" class="btn btn-outline-secondary erppager-prev" title="Pagina precedente">«</button>',
            '<span class="erppager-status" style="padding:0 .5rem;">1/1</span>',
            '<button type="button" class="btn btn-outline-secondary erppager-next" title="Pagina successiva">»</button>',
            '</div>'
        ].join('');

        box.addEventListener("click", (ev) => {
            const st = table.__erpState__;
            if (ev.target.closest(".erppager-toggle")) {
                st.pager.enabled = !st.pager.enabled;
                applyPagingAndPaint(table);
            }
            if (ev.target.closest(".erppager-prev")) {
                if (st.pager.pageIndex > 0) {
                    st.pager.pageIndex--;
                    applyPagingAndPaint(table);
                }
            }
            if (ev.target.closest(".erppager-next")) {
                if (st.pager.pageIndex < st.pager.totalPages - 1) {
                    st.pager.pageIndex++;
                    applyPagingAndPaint(table);
                }
            }
        });
    }

    function renderPagerUI(table) {
        const box = table.querySelector(".erppager-box");
        if (!box) return;
        const st = table.__erpState__;
        const status = box.querySelector(".erppager-status");
        if (status) {
            status.textContent = st.pager.enabled
                ? `${Math.min(st.pager.pageIndex + 1, st.pager.totalPages)}/${st.pager.totalPages}`
                : `1/1`;
        }
    }

    // ------- Init -------
    function initTable(table) {
        if (table.__erpState__) return;

        table.__erpState__ = {
            columns: [],      // [{for, thIndex, sortMode, baseState, orderSeq, filterable, btn}]
            allRows: [],
            viewRows: [],
            filter: { enabled: false, text: "" },
            pager: {
                enabled: !!parseInt(table.dataset.maxLines || "0", 10),
                pageSize: parseInt(table.dataset.maxLines || "0", 10) || 0,
                pageIndex: 0, totalPages: 1
            },
            sorter: { order: [] } // <<======== nuovo: [{ for, dir(+1/-1) }] in sequenza
        };

        buildSortToggles(table);
        buildFilterUI(table);
        buildPagerUI(table);

        // Deleghe: click sui sort toggle
        table.addEventListener("click", (ev) => {
            const btn = ev.target.closest(".erpsort-toggle");
            if (btn) { handleSortClick(table, btn); }
        });

        // Prima verniciatura
        refreshView(table);
    }



    function initAll() {
        document.querySelectorAll("table.erptbl").forEach(initTable);
    }

    if (document.readyState === "loading")
        document.addEventListener("DOMContentLoaded", initAll);
    else
        initAll();


    // --- PUBLIC API: esportata in window ---
    // inizializza tutte le tabelle nell'ambito passato (document di default)
    function __erp_initAll(scope = document) {
        scope.querySelectorAll("table.erptbl").forEach(initTable);
        scope.querySelectorAll("table.erptbl").forEach(t => refreshView(t));
    }
    // inizializza dentro un container (es. una modale)
    function __erp_initIn(scope) {
        __erp_initAll(scope || document);
    }
    // refresh singola tabella
    function __erp_refresh(table) {
        if (!table || !table.__erpState__) return;
        refreshView(table);
    }
    // refresh di tutte quelle dentro un container
    function __erp_refreshAllIn(scope = document) {
        scope.querySelectorAll("table.erptbl").forEach(__erp_refresh);
    }

    // Esporta in globale
    window.ErpTables = {
        initAll: __erp_initAll,
        initIn: __erp_initIn,
        refresh: __erp_refresh,
        refreshAllIn: __erp_refreshAllIn,
        // (opzionale) utile se vuoi pilotare direttamente
        initTable,
        refreshView
    };

})();


function erpTableInitIn(scope) {
    if (window.ErpTables && typeof ErpTables.initIn === 'function') {
        ErpTables.initIn(scope || document);
    }
}

// Bootstrap 5
document.addEventListener('shown.bs.modal', function (ev) {
    const modalBody = ev.target;
    erpTableInitIn(modalBody);
    if (window.ErpTables && typeof ErpTables.refreshAllIn === 'function') {
        ErpTables.refreshAllIn(modalBody);
    }
});
// se carichi contenuto via AJAX nella modale:
function onModalContentLoaded(modalEl) {
    erpTableInitIn(modalEl);
}

//==============================================================================================
//==============================================================================================
//==============================================================================================


// =============================
// ✅ ADVANCED MANDATORY SECTION (UNIFIED)
// =============================

//function etkIsHidden(el) {
//    if (!el) return true;

//    if (!["INPUT", "SELECT", "TEXTAREA"].includes(el.tagName))
//        return true;

//    const style = window.getComputedStyle(el);

//    if (style.display === "none") return true;
//    if (style.visibility === "hidden") return true;

//    // MODALI: se fade ma non ancora "show", NON considerare hidden
//    const modal = el.closest(".modal");
//    if (modal && modal.classList.contains("fade") && !modal.classList.contains("show"))
//        return false;

//    return false;
//}
//function etkIsHidden(el) {
//    if (!el) return true;

//    //if (!["INPUT", "SELECT", "TEXTAREA"].includes(el.tagName))
//    //    return true;

//    // ✅ 1. Se un antenato è display:none → l’elemento è hidden
//    let p = el;
//    while (p) {
//        const style = window.getComputedStyle(p);
//        if (style.display === "none" || style.visibility === "hidden")
//            return true;

//        p = p.parentElement;
//    }

//    // ✅ 2. Riconosci la TEMPLATE ROW nascosta (tr[data-template])
//    const tr = el.closest("tr");
//    if (tr && tr.style.display === "none") return true;

//    // ✅ 3. Gestione delle modali: se fade ma non ancora show → NON hidden
//    const modal = el.closest(".modal");
//    if (modal && modal.classList.contains("fade") && !modal.classList.contains("show"))
//        return false;

//    // ✅ 4. Fallback → visibile
//    return false;
//}


function etkIsHidden(el) {
    if (!el) return true;

    if (!["INPUT", "SELECT", "TEXTAREA"].includes(el.tagName))
        return true;

    // ✅ 3. Gestione delle modali: se fade ma non ancora show → NON hidden
    const modal = el.closest(".modal");
    if (modal && modal.classList.contains("fade") && !modal.classList.contains("show"))
        return false;


    // ✅ 1. Se un antenato è display:none → l’elemento è hidden
    let p = el;
    while (p) {
        const style = window.getComputedStyle(p);
        if (style.display === "none" || style.visibility === "hidden")
            return true;

        p = p.parentElement;
    }

    //// ✅ 2. Riconosci la TEMPLATE ROW nascosta (tr[data-template])
    //const tr = el.closest("tr");
    //if (tr && tr.style.display === "none") return true;


    // ✅ 4. Fallback → visibile
    return false;
}


function etkGetEffectiveValue(input) {
    if (!input) return "";

    // AUTOCOMPLETE
    if (input.classList.contains("autocomplete-input") && typeof getSelectedValue === "function") {
        return (getSelectedValue(input.id) || "").trim();
    }

    // SWITCH / RADIO GROUP
    if (input.classList.contains("form-check-input")) {
        const checked = document.querySelectorAll(`input[name="${CSS.escape(input.name)}"]:checked`);
        return checked.length > 0 ? (checked[0].value || "").trim() : "";
    }

    // TOGGLE SWITCH
    if (input.classList.contains("toggle-switch")) {
        return input.checked ? (input.value || "1") : "";
    }

    // DATE, TIME, DATETIME → gestisci valori "fake"
    if (["date", "time", "datetime-local"].includes(input.type)) {
        const v = (input.value || "").trim();
        if (v === "" || v === "0001-01-01" || v === "0001-01-01T00:00") return "";
        return v;
    }

    // CAMPI TESTO → gli spazi NON sono validi
    const v2 = (input.value || "").trim();
    return v2 === "" ? "" : v2;
}


function etkMarkMandatory(scope = document) {
    scope.querySelectorAll("[data-mandatory='true']").forEach(el => {
        if (etkIsHidden(el)) return;

        el.classList.add("erptbl-mandatory");

        const v = etkGetEffectiveValue(el);
        if (v === "") el.classList.add("erptbl-mandatory-error");
        else el.classList.remove("erptbl-mandatory-error");
    });
}


function etkValidateMandatory(scope = document) {
    let ok = true;
    let first = null;

    scope.querySelectorAll("[data-mandatory='true']").forEach(el => {
        if (etkIsHidden(el)) return;

        const v = etkGetEffectiveValue(el);

        // SWITCH-GROUP
        if (el.classList.contains("form-check-input")) {
            const name = el.name;
            const group = document.querySelectorAll(`input[name="${CSS.escape(name)}"]`);
            const any = Array.from(group).some(x => x.checked);

            if (!any) {
                ok = false;
                group.forEach(x => x.classList.add("erptbl-mandatory-error"));
                if (!first) first = el;
            }

            return;
        }

        // TUTTI GLI ALTRI CAMPI
        if (v === "") {
            ok = false;
            el.classList.add("erptbl-mandatory-error");
            if (!first) first = el;
        }
    });

    return { ok, firstError: first };
}

document.addEventListener("input", function (ev) {
    const el = ev.target;
    if (el.getAttribute("data-mandatory") === "true")
        etkMaybeClearMandatoryError(el);
}, true);

document.addEventListener("change", function (ev) {
    const el = ev.target;
    if (el.getAttribute("data-mandatory") === "true")
        etkMaybeClearMandatoryError(el);
}, true);

function etkMaybeClearMandatoryError(input) {
    if (!input) return;

    // ✅ Se NON è un campo mandatory → NON fare nulla
    if (input.getAttribute("data-mandatory") !== "true") return;

    const v = etkGetEffectiveValue(input);

    // ✅ SWITCH-GROUP / RADIO GROUP
    if (input.classList.contains("form-check-input")) {
        const name = input.name;
        const group = document.querySelectorAll(`input[name="${CSS.escape(name)}"]`);
        const any = Array.from(group).some(x => x.checked);

        group.forEach(x => {
            if (!any) x.classList.add("erptbl-mandatory-error");
            else x.classList.remove("erptbl-mandatory-error");
        });

        return;
    }

    // ✅ QUALSIASI ALTRO TIPO DI INPUT (text, date, time, textarea, autocomplete, ecc.)
    if (v === "") input.classList.add("erptbl-mandatory-error");
    else input.classList.remove("erptbl-mandatory-error");
}

//document.addEventListener("submit", function (ev) {
//    const form = ev.target.closest("form");
//    if (!form) return;

//    const v = etkValidateMandatory(form);
//    if (!v.ok) {
//        ev.preventDefault();
//        alert("È necessario compilare tutti i campi evidenziati.");
//        try {
//            v.firstError?.focus();
//            v.firstError?.scrollIntoView({ behavior: "smooth", block: "center" });
//        } catch { }
//    }
//});

//==============================================================================================
//==============================================================================================


function etkValidateAndSubmitModal(btn, prefix, modalDialogId) {
    const form = btn.closest("form");
    const v = etkValidateMandatory(form);

    if (!v.ok) {
        alert("È necessario compilare tutti i campi evidenziati.");
        v.firstError?.focus();
        v.firstError?.scrollIntoView({ behavior: "smooth", block: "center" });
        return;
    }

    // Se la validazione è OK → chiamata originale
    updateModalWithContentForm2(btn, prefix, modalDialogId);
}


//==============================================================================================
//==============================================================================================
//==============================================================================================



// ===============================================
// ✅ Speech To Text per tutti i textarea microfono
// ===============================================
function etkStartSpeechToText(textareaId) {
    const textarea = document.getElementById(textareaId);
    if (!textarea) return;

    let Speech = window.SpeechRecognition || window.webkitSpeechRecognition;
    if (!Speech) {
        alert("Il riconoscimento vocale non è supportato dal browser.");
        return;
    }

    const recognition = new Speech();
    recognition.lang = "it-IT";
    recognition.interimResults = false;
    recognition.continuous = false;

    recognition.onresult = function (event) {
        let text = event.results[0][0].transcript;
        textarea.value = (textarea.value + " " + text).trim();

        // trigger eventi JS di validazione mandatory
        textarea.dispatchEvent(new Event("input", { bubbles: true }));
        textarea.dispatchEvent(new Event("change", { bubbles: true }));

    ////////    // ✅ auto resize immediato
    ////////    etkAutoResize(textarea);
    };

    recognition.start();
}

// =====================================================
// ✅ EVENTO GLOBALE per tutte le icone microfono create
// =====================================================
document.addEventListener("click", function (ev) {
    const btn = ev.target.closest(".etk-mic-btn");
    if (!btn) return;

    const targetId = btn.getAttribute("data-target");

    // Effetto visivo ON/OFF
    btn.classList.add("etk-mic-active");

    etkStartSpeechToText(targetId);

    // ritorna al colore normale quando finisce
    setTimeout(() => btn.classList.remove("etk-mic-active"), 1500);
});

// ===========================================
// ✅ AUTO-RESIZE TEXTAREA (1 riga → auto espansa)
// ===========================================
////////function etkAutoResize(textarea) {
////////    if (!textarea) return;

////////    // reset per calcolare altezza reale
////////    textarea.style.height = "auto";

////////    // calcola nuova altezza
////////    textarea.style.height = (textarea.scrollHeight) + "px";
////////}

// ===========================================
// ✅ Attivazione automatica su tutti i textarea
// ===========================================
////////document.addEventListener("input", function (ev) {
////////    if (ev.target.tagName === "TEXTAREA") {
////////        etkAutoResize(ev.target);
////////    }
////////});




//==============================================================================================
//==============================================================================================
//==============================================================================================


// =======================================================
// ✅ DocViewer – visuallizzazione rapida di documenti e immagini (es. da documenti collegati)
// =======================================================
window.ErpDocViewer = {
    init: function () {
        document
            .querySelectorAll('.erp-doc-image')
            .forEach(img => {
                img.addEventListener('click', () => {
                    window.open(img.src, '_blank');
                });
            });
    }
};

document.addEventListener('DOMContentLoaded', ErpDocViewer.init);








//// =======================================================
//// ✅ DocContainer – visualizza più documenti con tabulatore e, gestione upload e delete (es. da documenti collegati)
//// =======================================================


document.addEventListener("DOMContentLoaded", function () {

    // ── LAZY LOADING VIEWER ────────────────────────────────────

    // Caso 1: pagina normale (non modal) — carica i viewer attivi subito
    document.querySelectorAll('.tab-pane.active [data-role="viewer-placeholder"]')
        .forEach(el => _loadViewer(el));

    // Caso 2: dentro un modal — carica quando il modal diventa visibile
    document.addEventListener('shown.bs.modal', function (e) {
        e.target.querySelectorAll('.tab-pane.active [data-role="viewer-placeholder"]')
            .forEach(el => _loadViewer(el));
    });

    // Caso 3: cambio tab — carica il viewer del tab appena attivato
    document.addEventListener('shown.bs.tab', function (e) {
        const tabPane = document.querySelector(e.target.dataset.bsTarget);
        if (!tabPane) return;
        tabPane.querySelectorAll('[data-role="viewer-placeholder"]')
            .forEach(el => _loadViewer(el));
    });

    // Caso 4: modal che viene riaperto — resetta il flag loaded
    // così i viewer vengono ricaricati se il modal è stato chiuso e riaperto
    document.addEventListener('hidden.bs.modal', function (e) {
        e.target.querySelectorAll('[data-role="viewer-placeholder"]')
            .forEach(el => {
                el.dataset.loaded = "false";
                el.innerHTML = `
                        <div class="d-flex align-items-center justify-content-center h-100 text-muted"
                             style="min-height:${el.dataset.height ?? 500}px;">
                            <span class="spinner-border spinner-border-sm me-2"></span> Caricamento...
                        </div>`;
            });
    });

    document.addEventListener("click", function (e) {

        // ── APRI pannello ADD ──────────────────────────────────
        if (e.target.closest('[data-action="add"]')) {
            const container = e.target.closest('.erp-doc-container');
            const panel = container.querySelector('[data-role="xdata-add-panel"]');
            if (!panel) return;

            _resetPanel(panel, container, "add");
            _loadTipoSelect(panel, container);

            bootstrap.Collapse.getOrCreateInstance(panel, { toggle: false }).show();
            panel.scrollIntoView({ behavior: "smooth", block: "nearest" });
        }

        // ── APRI pannello EDIT ─────────────────────────────────
        if (e.target.closest('[data-action="edit"]')) {
            const btn = e.target.closest('[data-action="edit"]');
            const container = btn.closest('.erp-doc-container');
            const panel = container.querySelector('[data-role="xdata-add-panel"]');
            if (!panel) return;

            _resetPanel(panel, container, "edit", {
                icode: btn.dataset.icode,
                ts: btn.dataset.ts,
                descr: btn.dataset.descr,
                fmt: btn.dataset.fmt,
            });
            _loadTipoSelect(panel, container, btn.dataset.fmt);

            bootstrap.Collapse.getOrCreateInstance(panel, { toggle: false }).show();
            panel.scrollIntoView({ behavior: "smooth", block: "nearest" });
        }

        // ── CHIUDI pannello ────────────────────────────────────
        if (e.target.closest('[data-action="cancel-add"]')) {
            const panel = e.target.closest('[data-role="xdata-add-panel"]');
            if (!panel) return;
            bootstrap.Collapse.getOrCreateInstance(panel, { toggle: false }).hide();
            _clearPanel(panel);
        }

        // ── SUBMIT (ADD o EDIT) ────────────────────────────────
        if (e.target.closest('[data-action="submit-add"]')) {
            const btn = e.target.closest('[data-action="submit-add"]');
            const container = btn.closest('.erp-doc-container');
            const panel = container.querySelector('[data-role="xdata-add-panel"]');
            const isEdit = panel.dataset.mode === "edit";
            const url = isEdit ? container.dataset.updateUrl : container.dataset.addUrl;

            // GUARD: blocca se url è vuoto
            if (!url) {
                console.error("URL mancante. dataset:", container.dataset);
                alert(isEdit ? "UpdateUrl non configurato." : "AddUrl non configurato.");
                return;
            }


            // Validazione
            let valid = true;
            panel.querySelectorAll('[required]').forEach(el => {
                el.classList.remove('is-invalid');
                const empty = el.type === 'file' ? el.files.length === 0 : !el.value.trim();
                if (empty) { el.classList.add('is-invalid'); valid = false; }
            });
            if (!valid) return;

            // Costruisci FormData con nomi che corrispondono ESATTAMENTE ai parametri C#
            const formData = new FormData();
            ////////formData.append("icode", btn.dataset.icode ?? "");
            ////////formData.append("timestampHex", btn.dataset.ts ?? "");
            formData.append("icode", panel.querySelector('[data-role="field-icode"]')?.value ?? "");
            formData.append("timestampHex", panel.querySelector('[data-role="field-ts"]')?.value ?? "");
            formData.append("mref", panel.querySelector('input[name="recordIcode"]')?.value ?? "");
            formData.append("descr", panel.querySelector('input[name="descrizione"]')?.value ?? "");
            formData.append("fmt", panel.querySelector('select[name="tipo"]')?.value ?? "");

            const fileInput = panel.querySelector('input[name="file"]');
            if (fileInput?.files?.length > 0) {
                formData.append("file", fileInput.files[0]);
            } else {
                _showError(container, "Selezionare un file.");
                btn.disabled = false;
                btn.innerHTML = '<i class="bi bi-upload me-1"></i> ' + (isEdit ? "Aggiorna" : "Carica");
                return;
            }

            // Feedback
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> ' + (isEdit ? "Aggiornamento..." : "Caricamento...");

            // Pulisce errori precedenti
            _setValidationError(panel, "");

            // ← ANTIFORGERY TOKEN
            const token = _getAntiforgeryToken();

            fetch(url, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token ?? ''
                },
                body: formData
            })
                .then(async r => {
                    const contentType = r.headers.get('content-type') ?? '';

                    if (contentType.includes('application/json')) {
                        const json = await r.json();
                        if (json.error) throw new Error(json.error ?? `Errore ${r.status}`);
                        //location.reload();
                        //return;


                        //--------------------------------------------
                        // Chiudi pannello
                        const panel = container.querySelector('[data-role="xdata-add-panel"]');
                        const descr = panel?.querySelector('input[name="descrizione"]')?.value ?? "";
                        const fmt = panel?.querySelector('select[name="tipo"]')?.value ?? "";
                        if (panel) {
                            bootstrap.Collapse.getOrCreateInstance(panel, { toggle: false }).hide();
                            _clearPanel(panel);
                        }

                        //--------- riabilita il pulsante dopo operazione riuscita -------
                        btn.disabled = false;
                        btn.innerHTML = isEdit
                            ? '<i class="bi bi-save me-1"></i> Aggiorna'
                            : '<i class="bi bi-upload me-1"></i> Carica';
                        //-----------------------------------------------------------------

                        if (typeof showDelayToast === 'function')
                            showDelayToast(json.info ?? "Operazione completata", "success");

                        // ← Aggiorna solo DOM, nessuna chiamata al server
                        if (isEdit) {
                            // ← Passa json con icode e timestampHex
                            _domAfterUpdate(container, json, descr, fmt);

                        } else {
                            // ← Passa json con icode e timestampHex
                            _domAfterAdd(container, json, descr, fmt);

                        }
                        return;
                        //--------------------------------------------

                    }

                    if (r.ok) { location.reload(); return; }

                    const msg = await r.text().catch(() => `Errore ${r.status}`);
                    throw new Error(msg || `Errore ${r.status}`);
                })
                .catch(err => {
                    console.error(err);
                    btn.disabled = false;
                    //btn.innerHTML = '<i class="bi bi-trash me-1"></i> Elimina'; 
                    btn.innerHTML = '<i class="bi bi-repeat me-1"></i> Riprova'; 
                    _setValidationError(panel, err.message);
                });


        }

        // ── DELETE ─────────────────────────────────────────────
        if (e.target.closest('[data-action="delete"]')) {
            const btn = e.target.closest('[data-action="delete"]');
            const container = btn.closest('.erp-doc-container');

            if (!confirm("Confermi l'eliminazione del documento?")) return;

            // Costruisci FormData con nomi che corrispondono ESATTAMENTE ai parametri C#
            const formData = new FormData();
            formData.append("icode", btn.dataset.icode ?? "");
            formData.append("timestampHex", btn.dataset.ts ?? "");

            // Feedback
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Eliminazione...';

            // Pulisce errori precedenti nel pannello se aperto
            const panel = container.querySelector('[data-role="xdata-add-panel"]');
            if (panel) _setValidationError(panel, "");

            // ← ANTIFORGERY TOKEN
            const token = _getAntiforgeryToken();

            fetch(container.dataset.deleteUrl, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token ?? ''
                },
                body: formData
            })
                .then(async r => {
                    const contentType = r.headers.get('content-type') ?? '';

                    if (contentType.includes('application/json')) {
                        const json = await r.json();
                        if (json.error) throw new Error(json.error ?? `Errore ${r.status}`);
                        //location.reload();
                        //return;



                        //--------------------------------------------
                        if (typeof showDelayToast === 'function')
                            showDelayToast(json.info ?? "Documento eliminato", "success");

                        // ← Aggiorna solo DOM
                        ////_domAfterDelete(container, btn.dataset.icode);
                        _domAfterDelete(container, json.icode?.toString() ?? btn.dataset.icode);
                        return;
                        //--------------------------------------------



                    }

                    if (r.ok) { location.reload(); return; }

                    const msg = await r.text().catch(() => `Errore ${r.status}`);
                    throw new Error(msg || `Errore ${r.status}`);
                })
                .catch(err => {
                    console.error(err);
                    btn.disabled = false;
                    btn.innerHTML = '<i class="bi bi-trash me-1"></i> Elimina';
                    _setValidationError(panel, err.message);
                });

        }

    });

    // ── HELPER: carica viewer lazy ─────────────────────────────
    function _loadViewer(placeholder) {
        // Evita doppio caricamento
        if (placeholder.dataset.loaded === "true") return;
        placeholder.dataset.loaded = "true";

        const src = placeholder.dataset.src;
        const mime = placeholder.dataset.mime ?? "";
        const height = placeholder.dataset.height ?? 500;

        if (!src) {
            placeholder.innerHTML = `
                <div class="p-3 text-center text-muted">
                    <i class="bi bi-file-earmark-x fs-1 d-block mb-2"></i>
                    Documento non disponibile.
                </div>`;
            return;
        }

        let html = "";

        if (mime.startsWith("image/")) {
            html = `<img src="${src}" style="max-width:100%;" />`;
        }
        else if (mime === "application/pdf") {
            html = `
                <object data="${src}" type="application/pdf" width="100%" height="${height}">
                    <p class="text-muted small p-2">
                        <i class="bi bi-file-earmark-pdf me-1"></i>
                        Il browser non supporta la visualizzazione PDF.
                        <a href="${src}" target="_blank" class="ms-1">Apri il file</a>
                    </p>
                </object>`;
        }
        else if (mime.startsWith("audio/")) {
            html = `<audio src="${src}" controls style="width:100%;"></audio>`;
        }
        else if (mime.startsWith("video/")) {
            html = `<video src="${src}" controls style="max-width:100%;height:${height}px;"></video>`;
        }
        else if (mime.startsWith("text/")) {
            html = `<iframe src="${src}" style="width:100%;height:${height}px;border:none;"></iframe>`;
        }
        else {
            html = `
                <div class="p-3 text-center text-muted">
                    <i class="bi bi-file-earmark-x fs-1 d-block mb-2"></i>
                    Formato non supportato (<code>${mime}</code>).
                    <a href="${src}" target="_blank" class="d-block mt-2">
                        <i class="bi bi-download me-1"></i> Scarica il file
                    </a>
                </div>`;
        }

        placeholder.innerHTML = html;
    }

    // ── HELPERS ────────────────────────────────────────────────

    //function _resetPanel(panel, container, mode, data = {}) {
    //    panel.dataset.mode = mode;
    //    const isEdit = mode === "edit";

    //    // Titolo e colore pannello
    //    const title = panel.querySelector('[data-role="panel-title"]');
    //    if (title) {
    //        title.innerHTML = isEdit
    //            ? '<i class="bi bi-pencil me-1"></i> Modifica documento'
    //            : '<i class="bi bi-upload me-1"></i> Carica nuovo documento';
    //    }
    //    panel.classList.toggle('is-edit', isEdit);

    //    // Pulsante submit
    //    const submitBtn = panel.querySelector('[data-role="submit-btn"]');
    //    if (submitBtn) {
    //        submitBtn.innerHTML = isEdit
    //            ? '<i class="bi bi-save me-1"></i> Aggiorna'
    //            : '<i class="bi bi-upload me-1"></i> Carica';
    //    }

    //    // Valorizza hidden fields
    //    _setField(panel, 'field-icode', data.icode ?? "");
    //    _setField(panel, 'field-ts', data.ts ?? "");
    //    _setField(panel, 'field-descr', data.descr ?? "");

    //    // File sempre required
    //    const fileInput = panel.querySelector('input[type="file"]');
    //    if (fileInput) fileInput.value = "";
    //}
    function _resetPanel(panel, container, mode, data = {}) {
        panel.dataset.mode = mode;
        const isEdit = mode === "edit";

        // Titolo e colore pannello
        const title = panel.querySelector('[data-role="panel-title"]');
        if (title) {
            title.innerHTML = isEdit
                ? '<i class="bi bi-pencil me-1"></i> Modifica documento'
                : '<i class="bi bi-upload me-1"></i> Carica nuovo documento';
        }
        panel.classList.toggle('is-edit', isEdit);

        // Pulsante submit — resetta SEMPRE disabled e testo
        const submitBtn = panel.querySelector('[data-role="submit-btn"]');
        if (submitBtn) {
            submitBtn.disabled = false;    // ← AGGIUNGERE: riabilita sempre all'apertura
            submitBtn.innerHTML = isEdit
                ? '<i class="bi bi-save me-1"></i> Aggiorna'
                : '<i class="bi bi-upload me-1"></i> Carica';
        }

        // Valorizza hidden fields
        _setField(panel, 'field-icode', data.icode ?? "");
        _setField(panel, 'field-ts', data.ts ?? "");
        _setField(panel, 'field-descr', data.descr ?? "");

        // File sempre svuotato
        const fileInput = panel.querySelector('input[type="file"]');
        if (fileInput) fileInput.value = "";
    }

    function _loadTipoSelect(panel, container, selectedFmt = null) {
        const typeUrl = container.dataset.typeUrl;
        const sel = panel.querySelector('select[name="tipo"]');
        if (!sel || !typeUrl) return;

        // Ricarica sempre per avere lista aggiornata
        sel.innerHTML = '<option value="">-- Caricamento... --</option>';
        fetch(typeUrl)
            .then(r => r.json())
            .then(items => {
                sel.innerHTML = '<option value="">-- Seleziona tipo --</option>';
                items.forEach(i => {
                    const opt = new Option(i.text, i.value);
                    if (selectedFmt && i.value === selectedFmt) opt.selected = true;
                    sel.add(opt);
                });
            })
            .catch(() => { sel.innerHTML = '<option value="">Errore nel caricamento</option>'; });
    }

    function _clearPanel(panel) {
        panel.dataset.mode = "";
        panel.classList.remove('is-edit');
        panel.querySelectorAll('input:not([type="hidden"]), select').forEach(el => {
            el.value = "";
            el.classList.remove('is-invalid');
        });
        _setField(panel, 'field-icode', "");
        _setField(panel, 'field-ts', "");
    }

    function _setField(panel, role, value) {
        const el = panel.querySelector(`[data-role="${role}"]`);
        if (el) el.value = value;
    }

    //helper per leggere il token antiforgery da un meta tag (se presente)
    function _getAntiforgeryToken() {
        // Cerca nel form principale della pagina
        const input = document.querySelector('input[name="__RequestVerificationToken"]');
        if (input) return input.value;
        return null;
    }

    // Helper: scrive/cancella il messaggio di errore nello span di validazione
    function _setValidationError(panel, message) {
        const span = panel.querySelector('[data-role="validation-error"]');
        if (!span) return;
        span.textContent = message;
        span.classList.toggle('d-none', !message);
    }


    // ---------------------------------------------------------------

    // ── HELPER: aggiorna DOM dopo ADD ─────────────────────────────
    //function _domAfterAdd(container, descr, fmt) {
    //    const uid = container.querySelector('[data-role="xdata-add-panel"]')?.id ?? Date.now();
    //    const tabList = container.querySelector('ul.nav-tabs');
    //    const tabContent = container.querySelector('.tab-content');
    //    const height = container.querySelector('[data-role="viewer-placeholder"]')?.dataset.height ?? 500;

    //    // Genera id univoco per il nuovo tab
    //    const newIndex = tabList ? tabList.querySelectorAll('.nav-item').length : 0;
    //    const newId = `xdata-new-${Date.now()}`;

    //    // Rimuovi eventuale messaggio "nessun documento"
    //    const emptyMsg = container.querySelector('.card-body > p.text-muted');
    //    if (emptyMsg) emptyMsg.remove();

    //    // Crea tab header
    //    const li = document.createElement('li');
    //    li.className = 'nav-item';
    //    li.setAttribute('role', 'presentation');
    //    li.innerHTML = `
    //    <button class="nav-link"
    //            data-bs-toggle="tab"
    //            data-bs-target="#${newId}"
    //            type="button"
    //            role="tab"
    //            data-icode=""
    //            data-ts=""
    //            data-descr="${descr}"
    //            data-fmt="${fmt}">
    //        ${descr}
    //    </button>`;

    //    // Crea tab-pane con placeholder — il viewer verrà caricato al click
    //    const pane = document.createElement('div');
    //    pane.className = 'tab-pane fade p-2';
    //    pane.id = newId;
    //    pane.setAttribute('role', 'tabpanel');
    //    pane.innerHTML = `
    //    <div class="erp-docviewer"
    //         data-role="viewer-placeholder"
    //         data-src=""
    //         data-mime=""
    //         data-height="${height}"
    //         data-loaded="true"
    //         style="min-height:${height}px;">
    //        <div class="p-3 text-center text-muted">
    //            <i class="bi bi-check-circle-fill text-success fs-1 d-block mb-2"></i>
    //            Documento caricato. Ricaricare la pagina per visualizzarlo.
    //        </div>
    //    </div>`;

    //    // Se tabList non esiste ancora (primo documento), creala
    //    if (!tabList) {
    //        _rebuildTabStructure(container);
    //        return;
    //    }

    //    tabList.appendChild(li);
    //    tabContent.appendChild(pane);

    //    // Attiva il nuovo tab
    //    bootstrap.Tab.getOrCreateInstance(li.querySelector('button')).show();
    //}

    function _domAfterAdd(container, json, descr, fmt) {
        const tabList = container.querySelector('ul.nav-tabs');
        const tabContent = container.querySelector('.tab-content');
        const height = container.querySelector('[data-role="viewer-placeholder"]')
            ?.dataset.height ?? 500;
        const newId = `xdata-new-${Date.now()}`;

        // Rimuovi eventuale messaggio "nessun documento"
        container.querySelector('.card-body > p.text-muted')?.remove();

        // Costruisci src del viewer se abbiamo icode
        const controllerName = container.dataset.controllerName ?? "";
        const src = json.icode
            ? `/${controllerName}/ViewXdata?icode=${encodeURIComponent(json.icode)}`
            : "";

        // Crea tab header
        const li = document.createElement('li');
        li.className = 'nav-item';
        li.setAttribute('role', 'presentation');
        li.innerHTML = `
        <button class="nav-link"
                data-bs-toggle="tab"
                data-bs-target="#${newId}"
                type="button"
                role="tab"
                data-icode="${json.icode ?? ""}"
                data-ts="${json.timestampHex ?? ""}"
                data-descr="${descr}"
                data-fmt="${fmt}">
            ${descr || "Nuovo documento"}
        </button>`;

        // Crea tab-pane con viewer placeholder già pronto
        const pane = document.createElement('div');
        pane.className = 'tab-pane fade p-2';
        pane.id = newId;
        pane.setAttribute('role', 'tabpanel');

        if (src) {
            // Abbiamo src — imposta placeholder che verrà caricato subito

            ////////pane.innerHTML = `
            ////////<div class="erp-docviewer"
            ////////     data-role="viewer-placeholder"
            ////////     data-src="${src}"
            ////////     data-mime="${json.mime ?? ''}"
            ////////     data-height="${height}"
            ////////     data-loaded="false"
            ////////     style="min-height:${height}px;">
            ////////    <div class="d-flex align-items-center justify-content-center h-100 text-muted"
            ////////         style="min-height:${height}px;">
            ////////        <span class="spinner-border spinner-border-sm me-2"></span> Caricamento...
            ////////    </div>
            ////////</div>`;

            pane.innerHTML = `
            <div class="d-flex justify-content-end gap-2 pt-2 px-2">
                <button type="button"
                        class="btn btn-sm btn-outline-secondary"
                        data-action="edit"
                        data-icode="${json.icode ?? ''}"
                        data-ts="${json.timestampHex ?? ''}"
                        data-descr="${descr}"
                        data-fmt="${fmt}">
                    <i class="bi bi-pencil me-1"></i> Modifica
                </button>
                <button type="button"
                        class="btn btn-sm btn-outline-danger"
                        data-action="delete"
                        data-icode="${json.icode ?? ''}"
                        data-ts="${json.timestampHex ?? ''}">
                    <i class="bi bi-trash me-1"></i> Elimina
                </button>
            </div>
            <div class="erp-docviewer"
                 data-role="viewer-placeholder"
                 data-src="${src}"
                 data-mime="${json.mime ?? ''}"
                 data-height="${height}"
                 data-loaded="false"
                 style="min-height:${height}px;">
                <div class="d-flex align-items-center justify-content-center h-100 text-muted"
                     style="min-height:${height}px;">
                    <span class="spinner-border spinner-border-sm me-2"></span> Caricamento...
                </div>
            </div>`;

        } else {
            // Nessun icode dal server — messaggio fallback
            pane.innerHTML = `
            <div class="erp-docviewer p-3 text-center text-muted">
                <i class="bi bi-check-circle-fill text-success fs-1 d-block mb-2"></i>
                Documento caricato correttamente.
            </div>`;
        }

        // Crea struttura tab se non esiste (primo documento)
        if (!tabList || !tabContent) {
            _rebuildTabStructure(container, li, pane);
            return;
        }

        tabList.appendChild(li);
        tabContent.appendChild(pane);

        // Attiva il nuovo tab — questo triggera shown.bs.tab che chiama _loadViewer
        bootstrap.Tab.getOrCreateInstance(li.querySelector('button')).show();
    }

    // Helper: crea struttura tab da zero (primo documento)
    function _rebuildTabStructure(container, li, pane) {
        const cardBody = container.querySelector('.card-body');
        const addPanel = cardBody?.querySelector('[data-role="xdata-add-panel"]');

        const ul = document.createElement('ul');
        ul.className = 'nav nav-tabs mb-0';
        ul.setAttribute('role', 'tablist');
        ul.appendChild(li);

        const tabContent = document.createElement('div');
        tabContent.className = 'tab-content border border-top-0 rounded-bottom mb-3';
        tabContent.appendChild(pane);

        cardBody.insertBefore(tabContent, addPanel);
        cardBody.insertBefore(ul, tabContent);

        // Attiva il tab
        li.querySelector('button').classList.add('active');
        pane.classList.add('show', 'active');

        // Carica viewer
        pane.querySelectorAll('[data-role="viewer-placeholder"]')
            .forEach(el => _loadViewer(el));
    }



    // ── HELPER: aggiorna DOM dopo UPDATE ──────────────────────────
    //function _domAfterUpdate(container, json, descr, fmt) {
    //    const icode = json.icode ?? "";
    //    const btn = container.querySelector(`button.nav-link[data-icode="${icode}"]`);
    //    if (!btn) return;

    //    // Aggiorna label e dataset del tab (incluso il nuovo timestamp)
    //    btn.textContent = descr;
    //    btn.dataset.descr = descr;
    //    btn.dataset.fmt = fmt;
    //    btn.dataset.ts = json.timestampHex ?? btn.dataset.ts; // ← aggiorna ts per edit successivi

    //    // Aggiorna anche i pulsanti edit/delete nel pane (se presenti)
    //    const pane = container.querySelector(btn.dataset.bsTarget);
    //    if (!pane) return;

    //    pane.querySelectorAll('[data-action="edit"], [data-action="delete"]').forEach(b => {
    //        b.dataset.ts = json.timestampHex ?? b.dataset.ts;  // ← timestamp aggiornato
    //        if (b.dataset.action === "edit") {
    //            b.dataset.descr = descr;
    //            b.dataset.fmt = fmt;
    //        }
    //    });

    //    // Ricarica il viewer con il nuovo file
    //    // Aggiunge ?t=timestamp all'URL per busting della cache del browser
    //    const controllerName = container.dataset.controllerName ?? "";
    //    const newSrc = icode
    //        ? `/${controllerName}/ViewXdata?icode=${encodeURIComponent(icode)}&t=${Date.now()}`
    //        : "";

    //    const placeholder = pane.querySelector('[data-role="viewer-placeholder"]');
    //    if (placeholder && newSrc) {
    //        const height = placeholder.dataset.height ?? 500;
    //        placeholder.dataset.src = newSrc;
    //        placeholder.dataset.mime = json.mime ?? placeholder.dataset.mime;
    //        placeholder.dataset.loaded = "false"; // ← resetta: permette a _loadViewer di agire
    //        placeholder.innerHTML = `
    //        <div class="d-flex align-items-center justify-content-center h-100 text-muted"
    //             style="min-height:${height}px;">
    //            <span class="spinner-border spinner-border-sm me-2"></span> Caricamento...
    //        </div>`;
    //        _loadViewer(placeholder); // ← carica subito il nuovo PDF
    //    }
    //}

    function _domAfterUpdate(container, json, descr, fmt) {
        const icode = json.icode ?? "";
        const btn = container.querySelector(`button.nav-link[data-icode="${icode}"]`);
        if (!btn) return;

        // Aggiorna label e dataset del tab
        btn.textContent = descr;
        btn.dataset.descr = descr;
        btn.dataset.fmt = fmt;
        btn.dataset.ts = json.timestampHex ?? btn.dataset.ts;

        const pane = container.querySelector(btn.dataset.bsTarget);
        if (!pane) return;

        // Aggiorna dataset dei pulsanti edit/delete nel pane
        pane.querySelectorAll('[data-action="edit"], [data-action="delete"]').forEach(b => {
            b.dataset.ts = json.timestampHex ?? b.dataset.ts;
            if (b.dataset.action === "edit") {
                b.dataset.descr = descr;
                b.dataset.fmt = fmt;
            }
        });

        const controllerName = container.dataset.controllerName ?? "";
        // Cache busting: aggiunge ?t=timestamp così il browser non serve il vecchio PDF
        const newSrc = icode
            ? `/${controllerName}/ViewXdata?icode=${encodeURIComponent(icode)}&t=${Date.now()}`
            : "";
        if (!newSrc) return;

        const height = container.dataset.height ?? 500;

        // ── CASO 1: tab generato lato server (TagHelper) ──────────────────────────
        // Struttura: .erp-docviewer > <object> | <img> | <audio> | <video>
        // Non ha viewer-placeholder — sostituisce direttamente il contenuto del viewer
        const serverViewer = pane.querySelector('.erp-docviewer');
        const placeholder = pane.querySelector('[data-role="viewer-placeholder"]');

        if (!placeholder && serverViewer) {
            // Rileva il tipo dal mime restituito dal server (json.mime)
            // oppure dall'attributo type dell'<object> esistente come fallback
            const mimeFromHtml = serverViewer.querySelector('img') ? 'image/'
                : serverViewer.querySelector('audio') ? 'audio/'
                    : serverViewer.querySelector('video') ? 'video/'
                        : 'application/pdf';
            const mime = json.mime ?? serverViewer.querySelector('object')?.getAttribute('type') ?? mimeFromHtml;

            serverViewer.innerHTML = _buildViewerInner(newSrc, mime, height);
            return;
        }

        // ── CASO 2: tab generato lato client (_domAfterAdd) ───────────────────────
        // Struttura: [data-role="viewer-placeholder"]
        if (placeholder) {
            placeholder.dataset.src = newSrc;
            placeholder.dataset.mime = json.mime ?? placeholder.dataset.mime;
            placeholder.dataset.loaded = "false";
            placeholder.innerHTML = `
            <div class="d-flex align-items-center justify-content-center h-100 text-muted"
                 style="min-height:${height}px;">
                <span class="spinner-border spinner-border-sm me-2"></span> Caricamento...
            </div>`;
            _loadViewer(placeholder);
        }
    }

    // Helper: genera l'HTML interno del viewer in base al mime type
    // (speculare a RenderViewer in ErpComponentTagHelper.cs)
    function _buildViewerInner(src, mime, height) {
        if (mime.startsWith('image/'))
            return `<img src="${src}" class="erp-doc-image" style="max-width:100%;" />`;

        if (mime === 'application/pdf')
            return `
            <object data="${src}" type="application/pdf" width="100%" height="${height}">
                <p class="text-muted small p-2">
                    <i class="bi bi-file-earmark-pdf me-1"></i>
                    Il browser non supporta la visualizzazione PDF.
                    <a href="${src}" target="_blank" class="ms-1">Apri il file</a>
                </p>
            </object>`;

        if (mime.startsWith('audio/'))
            return `<audio src="${src}" controls style="width:100%;"></audio>`;

        if (mime.startsWith('video/'))
            return `<video src="${src}" controls style="max-width:100%;height:${height}px;"></video>`;

        if (mime.startsWith('text/'))
            return `<iframe src="${src}" style="width:100%;height:${height}px;border:none;"></iframe>`;

        return `
        <div class="p-3 text-center text-muted">
            <i class="bi bi-file-earmark-x fs-1 d-block mb-2"></i>
            Formato non supportato (<code>${mime}</code>).
            <a href="${src}" target="_blank" class="d-block mt-2">
                <i class="bi bi-download me-1"></i> Scarica il file
            </a>
        </div>`;
    }


    // ── HELPER: aggiorna DOM dopo DELETE ──────────────────────────
    function _domAfterDelete(container, icode) {
        const btn = container.querySelector(`button.nav-link[data-icode="${icode}"]`);
        if (!btn) return;

        const li = btn.closest('.nav-item');
        const paneId = btn.dataset.bsTarget;
        const pane = container.querySelector(paneId);
        const wasActive = btn.classList.contains('active');

        // Trova tab precedente o successivo da attivare
        let nextBtn = null;
        if (wasActive) {
            const allBtns = [...container.querySelectorAll('ul.nav-tabs button.nav-link')]
                .filter(b => b !== btn);
            nextBtn = allBtns[0] ?? null;
        }

        // Rimuovi tab e pane
        li?.remove();
        pane?.remove();

        // Attiva tab adiacente se era attivo quello eliminato
        if (wasActive && nextBtn) {
            bootstrap.Tab.getOrCreateInstance(nextBtn).show();
            const nextPaneId = nextBtn.dataset.bsTarget;
            const nextPane = container.querySelector(nextPaneId);
            if (nextPane) {
                nextPane.querySelectorAll('[data-role="viewer-placeholder"]')
                    .forEach(el => _loadViewer(el));
            }
        }

        // Se non ci sono più tab, mostra messaggio vuoto
        const remaining = container.querySelectorAll('ul.nav-tabs .nav-item');
        if (remaining.length === 0) {
            const tabList = container.querySelector('ul.nav-tabs');
            const tabContent = container.querySelector('.tab-content');
            tabList?.remove();
            tabContent?.remove();
            const cardBody = container.querySelector('.card-body');
            if (cardBody) {
                // Inserisci messaggio vuoto prima del pannello add
                const addPanel = cardBody.querySelector('[data-role="xdata-add-panel"]');
                const msg = document.createElement('p');
                msg.className = 'text-muted mb-0 small';
                msg.innerHTML = '<i class="bi bi-inbox me-1"></i> Nessun documento disponibile.';
                cardBody.insertBefore(msg, addPanel);
            }
        }
    }




});




//==============================================================================================
//==============================================================================================
//==============================================================================================


// =======================================================
// ✅ LLaMA – Toggle Text To Speech (per singola istanza)
// =======================================================
document.addEventListener("change", function (ev) {
    const chk = ev.target.closest(".llama-tts-toggle");
    if (!chk) return;

    const uid = chk.dataset.target;
    const wrapper = chk.closest("[data-llama]");
    if (!wrapper) return;

    wrapper.dataset.tts = chk.checked ? "1" : "0";
});

// =======================================================
// ✅ Browser Text‑to‑Speech
// =======================================================
function etkSpeakText(text) {
    if (!window.speechSynthesis) return;

    window.speechSynthesis.cancel(); // interrompe eventuale parlato precedente

    const utter = new SpeechSynthesisUtterance(text);
    utter.lang = "it-IT";
    utter.rate = 1;
    utter.pitch = 1;

    window.speechSynthesis.speak(utter);
}


// =======================================================
// ✅ Integrazione con lo streaming LLaMA
// =======================================================
async function etkLlamaSend(wrapper) {
    const endpoint = wrapper.dataset.endpoint;
    const sessionId = wrapper.dataset.sessionId;
    const enableTts = wrapper.dataset.tts === "1";

    const textarea = wrapper.querySelector("textarea");
    const output = wrapper.querySelector(".llama-response");

    const prompt = textarea.value.trim();
    if (!prompt) return;

    output.textContent = "";
    let fullText = "";

    const response = await fetch(endpoint, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            model: "local-model",
            stream: true,
            session_id: sessionId,
            messages: [{ role: "user", content: prompt }]
        })
    });

    const reader = response.body.getReader();
    const decoder = new TextDecoder();

    while (true) {
        const { value, done } = await reader.read();
        if (done) break;

        const chunk = decoder.decode(value);
        chunk.split("\n").forEach(line => {
            if (!line.startsWith("data: ")) return;
            const data = line.substring(6);
            if (data === "[DONE]") return;

            try {
                const json = JSON.parse(data);
                const token = json.choices?.[0]?.delta?.content;
                if (token) {
                    output.textContent += token;
                    fullText += token;
                }
            } catch { }
        });
    }

    // ✅ SOLO SE l'utente ha abilitato TTS
    if (enableTts && fullText.trim() !== "") {
        etkSpeakText(fullText);
    }
}









//==============================================================================================
//==============================================================================================
//==============================================================================================




/* -------------------------
 * Bootstrap init dopo load
 * ------------------------- */

// Estende la tua funzione esistente: oltre ai TagHelper, inizializza anche le tabelle
if (typeof initializeAfterLoadPageAndPartial === 'function') {
    const __oldInit = initializeAfterLoadPageAndPartial;
    window.initializeAfterLoadPageAndPartial = function () {
        __oldInit();           // Autocomplete/SwitchGroup
        etkTableInit();        // <erp-table> e tabella vecchia
        etkMarkMandatory(document);
    };
}

// Prima load pagina
document.addEventListener('DOMContentLoaded', etkTableInit);

// =====================================================================================================
// =================================  /FINE  NUOVA SEZIONE: ERP TABLE  =================================
// =====================================================================================================


