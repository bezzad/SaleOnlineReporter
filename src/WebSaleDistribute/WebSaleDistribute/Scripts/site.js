/* LOADER */
jQuery(document).ready(function () {
    // reload page reload time from cookie after all ajax gets or posts
    // ref: http://www.w3schools.com/jquery/ajax_ajaxsetup.asp
    $.ajaxSetup({ complete: function (result) { setPageReloadTimer(null); } }); 
    jQuery(".status").fadeOut("slow");
});

window.onbeforeunload = function () {
    $("#loading").fadeIn();
}


function loadDataTables() {
    $('.dataTables').DataTable({
        //help: https://www.datatables.net/manual/options
        "lengthMenu": [[10, 25, 50, 100, 500, -1], [10, 25, 50, 100, 500, "همه"]]
        //select: true,
        //paging: false
    });
}

function getUrlSync(url) {
    return $.ajax({
        type: "GET",
        url: url,
        cache: false,
        async: false // jQuery.ajaxSetup({ async: false });
    }).responseJSON;
}

var reloadMethod;
var PageReloadTimeoutCookieName;
// Refresh page after session time out for redirect to login page
function setPageReloadTimer(cookieName) {
    if (cookieName !== null)
        PageReloadTimeoutCookieName = cookieName;

    var millisecond = getCookie(PageReloadTimeoutCookieName);

    if (millisecond === null) return;

    //alert((millisecond/1000).toString().substring(0,3));

    if (reloadMethod !== null) clearTimeout(reloadMethod);

    var reloadMethod = setTimeout(function () {
        window.location.reload(true);
    }, millisecond);
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function checkCookie() {
    var user = getCookie("username");
    if (user != "") {
        alert("Welcome again " + user);
    } else {
        user = prompt("Please enter your name:", "");
        if (user != "" && user != null) {
            setCookie("username", user, 365);
        }
    }
}