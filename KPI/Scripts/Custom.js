$(function () {
    $.noConflict();

    showHideLoader();

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

function LoadVendorDataGeneric(obj, dropdownId) {

    var $vendorData = $('#' + dropdownId);    // Cache the state dropdown

    // Clear the state dropdown
    $vendorData.empty();
    $vendorData.append('<option value="">--Select Company and Location--</option>'); // Add the default option

    var vendorId = $(obj).val();

    if (vendorId != null && vendorId != '' && vendorId != undefined) {

        $.ajax({
            url: '/Purchase/GetVendorWithLocationData',
            type: 'POST',
            data: { vendorType: vendorId },
            dataType: 'json',
            success: function (response) {
                // Loop through the response and populate the state dropdown
                $.each(response, function (index, elem) {
                    $vendorData.append($('<option>', {
                        value: elem.VendorId,
                        text: elem.VendorName
                    }));
                });
            },
            error: function (xhr, status, error) {
                console.error("Error fetching states: " + error);
            }
        });
    }
    else {
        return false;
    }

}

function DeleteLineItemRow(obj) {
    $(obj).closest('tr').remove();
}

function showHideLoader() {
    $(document).ajaxStart(function () {
        $("#loader").show(); // Show the loader
    }).ajaxStop(function () {
        $("#loader").hide(); // Hide the loader
    });
}