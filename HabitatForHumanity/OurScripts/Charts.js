$(document).ready(function () {
    getHoursChart("Month");
    getDemographicsPie("All");
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

function getDemographicsPie(gender) {
    var path = "/Admin/GetHoursByDemogPieBy/?gender=" + gender
    $.ajax({
        url: path,
        success: function (result) {
            $("#divHoursDemog").html(result);
        }
    });
}





