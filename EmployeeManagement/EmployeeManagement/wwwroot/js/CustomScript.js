function confirmDelete(uniqueid, isDeleteClicked) {
    var DeleteSpan = 'DeleteSpan_' + uniqueid;
    var confirmDeleteSpan = 'confirmDeleteSpan_' + uniqueid;
    if (isDeleteClicked) {
        $('#' + DeleteSpan).hide();
        $('#' + confirmDeleteSpan).show();
    }
    else {
        $('#' + DeleteSpan).show();
        $('#' + confirmDeleteSpan).hide();
    }
}