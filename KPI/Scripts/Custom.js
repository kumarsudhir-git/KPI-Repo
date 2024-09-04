$(function () {
    $.noConflict();

    $('.AddDataTable').DataTable({
        "scrollCollapse": false,
        "paging": true,
        "select": false,
        "orderBy": false
    });

    $('.AllowNumbersOnly').on('keypress', function (e) {
        debugger
        // Check if the pressed key is a number
        if (e.which < 48 || e.which > 57) {
            e.preventDefault();
        }
        //var regex = new RegExp("^[a-zA-Z0-9]+$");
        //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        //if (!regex.test(key)) {
        //    event.preventDefault();
        //    return false;
        //}
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
