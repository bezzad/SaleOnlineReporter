$(function () {
    $('.panelitem').lobiPanel({
        close: {
            tooltip: 'بستن'
        },
        reload: {
            tooltip: 'بروزرسانی'
        },
        minimize: {
            tooltip: 'کوچک کردن'
        },
        expand: {
            tooltip: 'بزرگ کردن'
        },
        unpin: {
            icon: 'glyphicon glyphicon-pushpin',
            tooltip: 'چسباندن / درآوردن'
        },
        close: false,
        unpin: false,
        editTitle: false,
        reload: false,
    });

    $('.spanel').lobiPanel({
        expand: {
            tooltip: 'بزرگ کردن'
        },
        unpin: {
            icon: 'glyphicon glyphicon-remove',
            tooltip: 'بستن پنجره'
        },
        close: false,
        reload: false,
        editTitle: false,
        minimize: false
    });



});

function showSettingPanel(id) {

    var sharpId = '#' + id;
    var sharpParentId = '#parent-' + id;

    $(sharpId).on('onUnpin.lobiPanel', function (ev, lobiPanel) {
        $(sharpId).removeAttr("style");
    });

    $(sharpId).on('onPin.lobiPanel', function (ev, lobiPanel) {
        $(sharpId).attr("style", "display: none;");
    });

    $(sharpParentId).on('beforeClose.lobiPanel', function (ev, lobiPanel) {
        $(sharpId).attr("style", "display: none;");
    });

    //get the LobiPanel instance
    var instance = $(sharpId).data('lobiPanel');
    var parent_instance = $(sharpParentId).data('lobiPanel');

    //fetch paren position
    var position = parent_instance.getPosition();

    //call the methods
    instance.unpin();

    //method chaining is also available
    SetToCenter(instance, instance.getWidth(), instance.getHeight());
    instance.load();

    instance.bringToFront();    
}

function SetToCenter(lobiPanel, w, h) {
    // Fixes dual-screen position                         Most browsers      Firefox
    var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
    var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;

    var width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    var left = ((width / 2) - (w / 2)) + dualScreenLeft;
    var top = ((height / 2) - (h / 2)) + dualScreenTop;

    lobiPanel.setPosition(left, top);
}