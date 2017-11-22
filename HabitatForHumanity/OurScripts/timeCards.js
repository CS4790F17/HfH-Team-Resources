function loadAjaxEdit(divToLoad, timeCardId) {
    var pth = '/Admin/EditTimeCard/' + timeCardId;
    $.ajax({
        url: pth,
        success: function (result) {
            $(divToLoad).removeClass('well-sm');
            $(divToLoad).addClass('well-lg');
            $(divToLoad).html(result);
            //$("input").first().focus();
        }
    });
}
function save() {
    alert("Saved");
}