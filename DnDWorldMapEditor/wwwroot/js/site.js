// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let gridSpaceDetailsModal = new bootstrap.Modal(document.getElementById('gridSpaceDetailsModal'));
let gridSpaceModalContent = $('#gridSpaceDetailsModalContent');

function loadGridSpaceModal(row, col, id){
    $.ajax({
        url: "/GridSpace/GetGridSpaceDetails/",
        type: 'GET',
        data: {row: row, col: col, worldMapId: id},
        success: function(data){
            gridSpaceModalContent.html(data);
            gridSpaceDetailsModal.show();
            
        },
        error: function(){
            alert("Error loading content.");
        }
    });
}

function RefreshModal(id){
    $.ajax({
        url: "/GridSpace/GetGridSpaceDetailsById/",
        type: 'GET',
        data: {gridSpaceId: id},
        success: function(data){
            gridSpaceModalContent.load(data);
        },
        error: function(){
            alert("Error loading content.");
        }
    });
}



function UpdateCompletedStatus(id) {
    $.ajax({
        url: "/GridSpace/UpdateGridEncounterCompletedStatus/",
        type: 'POST',
        data: {gridEncounterId: id},
        success: function () {
        },
        error: function () {
            alert("Error loading content.");
        }
    });

}

function RemoveGridEncounter(element, id) {
    $.ajax({
        url: "/GridSpace/RemoveGridEncounterFromGridSpace/",
        type: 'POST',
        data: {gridEncounterId: id},
        success: function (data) {
            element.parentElement.parentElement.style.display = 'none';
        },
        error: function () {
            alert("Error removing Grid Encounter.")
        }
    });
}

function RemoveGridCharacter( element, id) {
    $.ajax({
        url: "/GridSpace/RemoveGridCharacterFromGridSpace/",
        type: 'POST',
        data: {gridCharacterId: id},
        success: function (data) {
            element.parentElement.parentElement.style.display = 'none';
        },
        error: function () {
            alert("Error removing Grid Character.")
        }
    });
}

function AddEncounter(id) {
    const encounterId = $('#encounterSelectionElement').val();
    $.ajax({
        url: "/GridSpace/AddEncounterToGridSpace/",
        type: 'POST',
        data: {gridSpaceId: id, encounterId: encounterId},
        success: function (data) {
            document.getElementById('GridEncountersDisplay').innerHTML += data;
        },
        error: function () {
            alert("Error loading content.");
        }
    });
}

function AddCharacter(id) {
    const characterId = $('#characterSelectionElement').val();
    $.ajax({
        url: "/GridSpace/AddCharacterToGridSpace/",
        type: 'POST',
        data: {gridSpaceId: id, characterId: characterId},
        success: function (data) {
            document.getElementById('GridCharactersDisplay').innerHTML += data;
        },
        error: function () {
            alert("Error loading content.");
        }
    });
}

function UpdateAccessibility(id) {
    $.ajax({
        url: "/GridSpace/UpdateAccessibility/",
        type: 'POST',
        data: {gridSpaceId: id},
        success: function (data) {
        },
        error: function () {
            alert("Error loading content.");
        }
    });
}


function UpdateDescription(id) {
    const updatedDescription = $('#GridSpaceDescriptionInput').val();
    $.ajax({
        url: "/GridSpace/UpdateDescription/",
        type: 'POST',
        data: {gridSpaceId: id, updatedDescription: updatedDescription},
        success: function (data) {
            document.getElementById("DescriptionDisplayInfo").innerHTML = updatedDescription;
        },
        error: function () {
            alert("Error loading content.");
        }
    });


}
function UpdateHistory(id) {
    const updatedHistory = $('#GridSpaceHistoryInput').val();
    $.ajax({
        url: "/GridSpace/UpdateHistory/",
        type: 'POST',
        data: {gridSpaceId: id, updatedHistory: updatedHistory},
        success: function (data) {
            document.getElementById("HistoryDisplayInfo").innerHTML = updatedHistory;
            
        },
        error: function () {
            alert("Error loading content.");
        }
    });


}

function UpdateNotes(id) {
    const updatedNotes = $('#GridSpaceNotesInput').val();
    $.ajax({
        url: "/GridSpace/UpdateNotes/",
        type: 'POST',
        data: {gridSpaceId: id, updatedNotes: updatedNotes},
        success: function (data) {
            document.getElementById("NotesDisplayInfo").innerHTML = updatedNotes;
        },
        error: function () {
            alert("Error loading content.");
        }
    });

}







function loadGridSpaceDescriptionEditForm() {
    const gridSpaceId = document.getElementById("GridSpaceDescriptionEditBtn").dataset.id;
    $.ajax({
        url: "/GridSpace/GetGridSpaceDescriptionForm/",
        type: 'GET',
        data: {gridSpaceId: gridSpaceId},
        success: function (data) {
            $('#gridSpaceDescriptionEditModalContent').html(data);
            var modal = new bootstrap.Modal(document.getElementById('gridSpaceDescriptionEditModal'));
            modal.show();
        },
        error: function () {
            alert("Error loading Edit Description Form!")
        }
    });
}

function loadGridSpaceNotesEditForm() {
    const gridSpaceId = document.getElementById("GridSpaceNotesEditBtn").dataset.id;
    $.ajax({
        url: "/GridSpace/GetGridSpaceNotesForm/",
        type: 'GET',
        data: {gridSpaceId: gridSpaceId},
        success: function (data) {
            $('#gridSpaceNotesEditModalContent').html(data);
            var modal = new bootstrap.Modal(document.getElementById('gridSpaceNotesEditModal'));
            modal.show();
        },
        error: function () {
            alert("Error loading Edit Notes Form!")
        }
    });
}

function loadGridSpaceHistoryEditForm() {
    const gridSpaceId = document.getElementById("GridSpaceHistoryEditBtn").dataset.id;
    $.ajax({
        url: "/GridSpace/GetGridSpaceHistoryForm/",
        type: 'GET',
        data: {gridSpaceId: gridSpaceId},
        success: function (data) {
            $('#gridSpaceHistoryEditModalContent').html(data);
            var modal = new bootstrap.Modal(document.getElementById('gridSpaceHistoryEditModal'));
            modal.show();
        },
        error: function () {
            alert("Error loading Edit History Form!")
        }
    });
}



