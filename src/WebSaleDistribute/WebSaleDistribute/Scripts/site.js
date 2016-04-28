/* LOADER */
jQuery(document).ready(function () {

    jQuery(".status").fadeOut("slow");  
});

window.onbeforeunload = function () {
    $("#loading").fadeIn();
}