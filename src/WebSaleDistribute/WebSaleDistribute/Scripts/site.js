var isMobile = false; //initiate as false
var reloadMethod;
var PageReloadTimeoutCookieName;
var rows_selected = []; // Array holding selected row IDs

/* LOADER */
jQuery(document).ready(function () {

    /* set toastr options */
    toastr.options = {
        "closeButton": false,
        "debug": true,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "15000",
        "extendedTimeOut": "5000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "containerId": 'toast-container',
        "rtl": 'true'
    }

    /* Detect Mobile or Desktop */
    // device detection
    if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent)
        || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4)))
        isMobile = true;

    $("#MobileVersion").html(function () {
        return isMobile ? "Mobile" : "Desktop";
    });

    // reload page reload time from cookie after all ajax gets or posts
    // ref: http://www.w3schools.com/jquery/ajax_ajaxsetup.asp
    $.ajaxSetup({
        complete: function (result) { setPageReloadTimer(null); jQuery(".status").fadeOut("slow"); },
        error: function (xhr, status, error) {
            try {
                var err = JSON.parse(xhr.responseText);
                //toastr.error(err.ExceptionMessage, err.ExceptionType);
                toastr.error(err.ExceptionMessage);
            } catch (err) {
                toastr.error(xhr.responseText);
            }
        }
    });

    $(document).bind("ajaxSend", function () {
        jQuery(".status").fadeIn("slow");
    }).bind("ajaxComplete", function () {
        jQuery(".status").fadeOut("slow");
    });

    jQuery(".status").fadeOut("slow");

    $(".close").click(function () {
        $(".alert").alert();
    });
});

//
//With jquery.fileDownload.js
//Promise use allows for a very customized experience easily
//
$(document).on("click", "a.fileDownload", function () {
    try {
        //if (!isMobile)
        $("#loading").fadeIn();

        $.fileDownload($(this).prop('href'))
            .done(function () { jQuery(".status").fadeOut("slow"); })
            .fail(function () { jQuery(".status").fadeOut("slow"); });
    }
    catch (err) {
        alert(err);
    }
    finally {
        setTimeout(function () {
            jQuery(".status").fadeOut("slow");
        }, 2000);
    }
    return false; //this is critical to stop the click event which will trigger a normal file download
});


window.onbeforeunload = function () {
    $("#loading").fadeIn();
}

//Remove the formatting to get integer data for summation
function intVal(i) {
    return typeof i === 'string' ?
        i.replace(/[\$,]/g, '') * 1 :
        typeof i === 'number' ?
        i : 0;
};

// add sum method to any array class
var sum = function (array, prop) {
    var total = 0
    for (var i = 0, _len = array.length; i < _len; i++) {
        total += intVal(array[i][prop]);
    }
    return total.toLocaleString('fa-IR');
}


var convertToNumericByCamma = function (nTd, sData, oData, iRow, iCol) {
    var $currencyCell = $(nTd);
    var commaValue = $currencyCell.text().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    $currencyCell.text(commaValue);
}

function loadDataTables(id, iDisplayLength, currencyColumns) {
    if (iDisplayLength === undefined) iDisplayLength = 10;

    if (currencyColumns === undefined || currencyColumns === null) currencyColumns = [];

    id = "#" + id;

    var table = $(id).DataTable({
        //select: true,
        //paging: false
        //help: https://www.datatables.net/manual/options
        "processing": true,
        "lengthMenu": [[10, 25, 50, 100, 500, -1], [10, 25, 50, 100, 500, "همه"]],
        "iDisplayLength": iDisplayLength,   // Set default of 10 rows
        "bAutoWidth": true, // smart column width calculation
        "bPaginate": true, // pagination
        "stateSave": true, // save the state of a table (its paging position, ordering state etc) so that is can be restored when the user reloads a page, or comes back to the page after visiting a sub-page.
        "sPaginationType": "full_numbers",
        "language": {
            "sProcessing": "درحال پردازش...",
            "sLengthMenu": "نمایش محتویات _MENU_",
            "sZeroRecords": "موردی یافت نشد",
            "sInfo": "نمایش _START_ تا _END_ از مجموع _TOTAL_ مورد",
            "sInfoEmpty": "تهی",
            "sInfoFiltered": "(فیلتر شده از مجموع _MAX_ مورد)",
            "sInfoPostFix": "",
            "sSearch": "جستجو:",
            "sUrl": "",
            "oPaginate": {
                "sFirst": "ابتدا",
                "sPrevious": "قبلی",
                "sNext": "بعدی",
                "sLast": "انتها"
            }
        },
        //
        "footerCallback": function (tfoot, data, start, end, display) {
            var api = this.api();
            var floatingDigit = 2;
            var haveFooter = false;

            //
            // Set sum value in footer
            //
            api.columns('.sum').every(function () {
                haveFooter = true;
                //------------------------
                var col = this;

                // Total over all pages
                total = col
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0);

                // Total over this page                
                pageTotal = api.column(col.index(), { page: 'current' })
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0);

                // Update footer
                $(col.footer()).html(numberWithCommas(pageTotal) + "<hr/>" + numberWithCommas(total));
                //------------------------
            });

            //
            // Set average value in footer
            //
            api.columns('.avg').every(function () {
                haveFooter = true;
                //------------------------
                var col = this;

                // Total over all pages
                avg = col
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0) / col.data().length;
                avg = Number((avg).toFixed(floatingDigit));

                // Total over this page                
                var cCol = api.column(col.index(), { page: 'current' });
                pageAvg = cCol
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0) / cCol.data().length;

                pageAvg = Number((pageAvg).toFixed(floatingDigit));

                // Update footer
                $(col.footer()).html("میانگین " + numberWithCommas(pageAvg) + "<hr/> میانگین کل " + numberWithCommas(avg));
                //------------------------
            });

            //
            // Set empty in footer
            //
            if (haveFooter) {
                api.columns('.empty').every(function () {
                    //------------------------
                    var col = this;

                    // Update footer
                    $(col.footer()).html("");
                    //------------------------
                });
            }
        },
        "aoColumnDefs": [{
            "aTargets": currencyColumns,
            "fnCreatedCell": convertToNumericByCamma
        }],
        "columnDefs": [{
            'targets': 0,
            'searchable': false,
            'orderable': false,
            'width': '1%',
            'className': 'dt-body-center',
            'render': function (data, type, full, meta) {
                return '<input type="checkbox">';
            }
        }],
        'rowCallback': function (row, data, dataIndex) {
            // Get row ID
            var rowId = data[0];

            // If row ID is in the list of selected row IDs
            if ($.inArray(rowId, rows_selected) !== -1) {
                $(row).find('input[type="checkbox"]').prop('checked', true);
                $(row).addClass('selected');
            }
        }
    });

    table.state.clear();

    $(id + ' tbody').on('click', 'tr', function (e) {
        if ($(this).hasClass('notCheckable')) {
            table.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });


    // Handle click on checkbox
    $(id + ' tbody').on('click', 'input[type="checkbox"]', function (e) {
        if ($(this).hasClass('notCheckable') === false) {
            var $row = $(this).closest('tr');

            // Get row data
            var data = table.row($row).data();

            // Get row ID
            var rowId = data[1];

            // Determine whether row ID is in the list of selected row IDs 
            var index = $.inArray(rowId, rows_selected);

            // If checkbox is checked and row ID is not in list of selected row IDs
            if (this.checked && index === -1) {
                rows_selected.push(rowId);

                // Otherwise, if checkbox is not checked and row ID is in list of selected row IDs
            } else if (!this.checked && index !== -1) {
                rows_selected.splice(index, 1);
            }

            if (this.checked) {
                $row.addClass('selected');
            } else {
                $row.removeClass('selected');
            }

            // Update state of "Select all" control
            updateDataTableSelectAllCtrl(table, id);

            // Prevent click event from propagating to parent
            e.stopPropagation();
        }
    });

    // Handle click on table cells with checkboxes
    $(id).on('click', 'tbody td, thead th:first-child', function (e) {
        $(this).parent().find('input[type="checkbox"]').trigger('click');
    });

    // Handle click on "Select all" control
    $('thead input[name="select_all"]', table.table().container()).on('click', function (e) {
        if (this.checked) {
            $(id + ' tbody input[type="checkbox"]:not(:checked)').trigger('click');
        } else {
            $(id + ' tbody input[type="checkbox"]:checked').trigger('click');
        }

        // Prevent click event from propagating to parent
        e.stopPropagation();
    });

    // Handle table draw event
    table.on('draw', function () {
        // Update state of "Select all" control
        updateDataTableSelectAllCtrl(table, id);
    });
}

function getTableSelectedRows() {
    return rows_selected;
}

function getTableFirstSelectedRow(id) {
    var theRow = null;
    $('#' + id).DataTable().rows('.selected').each(function () {
        theRow = this.data()[0];
    });
    return theRow
}

function getTableAllData(id) {
    var result = [];

    $('#' + id + ' > tbody > tr').each(function () { // read any rows

        var newRow = [];
        var cells = this.cells;

        for (var i = 0; i < cells.length; i++) {
            if ($('select', cells[i].innerHTML).length > 0) { // a combo box found!
                var newVal;
                var x = $('li.selected[data-original-index]', cells[i].innerHTML); // find selected option

                newVal = x.attr("data-original-index"); // get selected option value, if not selected then get undefined

                newRow.push(newVal);
            }
            else if (cells[i].outerText !== "موردی یافت نشد")
            { newRow.push(cells[i].outerText); }
        }

        //
        result.push(newRow);
    });

    return result;
}

//
// Updates "Select all" control in a data table
//
function updateDataTableSelectAllCtrl(table, id) {
    var tr = $(id + ' tbody tr'); // has checkable row or return
    if (tr.hasClass('notCheckable') === false) {
        var $table = table.table().node();
        var $chkbox_all = $('tbody input[type="checkbox"]', $table);
        var $chkbox_checked = $('tbody input[type="checkbox"]:checked', $table);
        var chkbox_select_all = $('thead input[name="select_all"]', $table).get(0);

        // If none of the checkboxes are checked
        if ($chkbox_checked.length === 0) {
            chkbox_select_all.checked = false;
            if ('indeterminate' in chkbox_select_all) {
                chkbox_select_all.indeterminate = false;
            }

            // If all of the checkboxes are checked
        } else if ($chkbox_checked.length === $chkbox_all.length) {
            chkbox_select_all.checked = true;
            if ('indeterminate' in chkbox_select_all) {
                chkbox_select_all.indeterminate = false;
            }

            // If some of the checkboxes are checked
        } else {
            chkbox_select_all.checked = true;
            if ('indeterminate' in chkbox_select_all) {
                chkbox_select_all.indeterminate = true;
            }
        }
    }
}


function getAsync(url, params) {

    if (url === null) {
        toastr.warning("آدرس خالی می باشد", "اخطار", { timeOut: 15000 });
        return;
    }

    toastr.info("لطفا منتظر بمانید", '', { timeOut: 3000 });

    $.get(url, params, function (data) {
        toastr.success(data);
    });
}

function postAsync(url, params) {

    if (url === null) {
        toastr.warning("آدرس خالی می باشد", "اخطار", { timeOut: 15000 });
        return;
    }

    toastr.info("لطفا منتظر بمانید", '', { timeOut: 3000 });

    $.post(url, JSON.stringify(params), function (data) {
        toastr.success(data);
    });

    //$.ajax({
    //    url: url,
    //    type: "Post",
    //    data: JSON.stringify(params),
    //    dataType: "json",
    //    contentType: 'application/json; charset=utf-8',
    //    success: function (data) { toastr.success(data); },
    //    error: function (msg) { toastr.error(msg.responseText); }
    //});
}

function get(url, params, updateElementId) {

    if (url === null) {
        toastr.warning("آدرس خالی می باشد", "اخطار", { timeOut: 15000 });
        return;
    }

    toastr.info("لطفا منتظر بمانید", '', { timeOut: 3000 });

    var jqxhr = $.get(url, params, function (data) {
        var el = $(document).find("#" + updateElementId).html(data);
        toastr.success("ثبت شد", '', { timeOut: 5000 });
    });
}

function post(url, params, updateElementId) {
    if (url === null) {
        toastr.warning("آدرس خالی می باشد", "اخطار", { timeOut: 15000 });
        return;
    }

    toastr.info("لطفا منتظر بمانید", '', { timeOut: 3000 });

    var jqxhr = $.post(url, JSON.stringify(params), function (data) {
        var el = $(document).find("#" + updateElementId).html(data);
        toastr.success("ثبت شد", '', { timeOut: 5000 });
    }, "json");
    //.fail(function (xhr, textStatus, errorThrown) {
    //    toastr.error(xhr.responseText);
    //});
}


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


/* Jquery function to create guids.
 * Guid code from 
 */
function generateGUID() {
    var d = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
};


function numberWithCommas(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}
