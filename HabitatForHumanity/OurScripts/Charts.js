$(document).ready(function () {
    getHoursChart("Month");
});

function getHoursChart(period) {
    var path = "/Admin/GetHoursChartBy/?period=" + period
    $.ajax({
        url: path,
        success: function (result) {
            $("#divHoursChart").html(result);
        }
    });
}





