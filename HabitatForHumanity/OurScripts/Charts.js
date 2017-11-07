$(document).ready(function () {
    loadChart($("divHoursMonthChart"));

});

function loadChart(target) {
    $.ajax({
        url: '/Admin/GetHoursMonthChart',
        success: function (result) {
            $("#divHoursMonthChart").html(result);
        }
    });
}


