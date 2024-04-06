$(function () {
    $.noConflict();

    $('.AddDataTable').DataTable({
        "scrollCollapse": false,
        "paging": true,
        "select": false,
        "orderBy": false
    });
})

function AddLineItemRowGeneric(url, methodType, selector, IsAppendData = true) {

    $.ajax({
        url: url,
        method: methodType,
        contentType: "application/json",
        success: function (data) {

            if (data != null) {
                if (IsAppendData) {
                    $(selector).append(data);
                }
                else {
                    $(selector).html(data);
                }
                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");
            }
            else {
                alert("Server error");
                return false;
            }
        },
        error: function (result) {
            console.log(result);
            alert('Error occured');
            return false;
        }
    })
}

function DeleteLineItemRow(obj) {
    $(obj).closest('tr').remove();
}