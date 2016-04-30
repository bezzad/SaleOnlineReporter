/* LOADER */
jQuery(document).ready(function () {

    jQuery(".status").fadeOut("slow");
});

window.onbeforeunload = function () {
    $("#loading").fadeIn();
}

// Call DataTable() func in all of the jquery tables 
$(document).ready(function () {
    loadDataTables();
})

function loadDataTables() {
    $('.dataTables').DataTable({
        //help: https://www.datatables.net/manual/options
        "lengthMenu": [[10, 25, 50, 100, 500, -1], [10, 25, 50, 100, 500, "همه"]]
        //select: true,
        //paging: false
    });
}