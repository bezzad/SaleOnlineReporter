/* LOADER */
jQuery(document).ready(function () {

    var zerif_frame = jQuery('iframe.zerif_google_map');
    var zerif_frameSrc = new Array();

    if (zerif_frame.length) {
        jQuery.each(zerif_frame, function (i, f) {
            zerif_frameSrc[i] = jQuery(f).attr('src');
            /*remove the src attribute so window will ignore these iframes*/
            jQuery(f).attr('src', '');
        });
    }

    function zerif_display_iframe_map() {
        if (zerif_frame.length) {
            jQuery.each(zerif_frame, function (a, x) {
                /*put the src attribute value back*/
                jQuery(x).attr('src', zerif_frameSrc[a]);
            });
        }
    }

    jQuery(".status").fadeOut();
    jQuery(".preloader").delay(1000).fadeOut("slow");
    setTimeout(zerif_display_iframe_map, 500);
});