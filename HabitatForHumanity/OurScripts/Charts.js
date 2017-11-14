$(document).ready(function () {
    getHoursChart("Month");
    getDemographicsPie("All");
    getBadPunches();
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
    var path = "/Admin/GetHoursDemogPieBy/?gender=" + gender
    $.ajax({
        url: path,
        success: function (result) {
            $("#divHoursDemog").html(result);
        }
    });
}

function getBadPunches() {
    $.ajax({
        url: "/Admin/GetBadPunches",
        success: function (result) {
            $("#badPunches").html(result);
        }
    });
}





