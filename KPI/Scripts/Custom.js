$(function () {
    $.noConflict();

    showHideLoader();
    HideShowGMSInfoTextBox();
    IniTializeMultiSelect();
    InitializeDatePicker();
    InitializeDataTable();
    InitializeNumericsOnly();
    InitializeSelect2();
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
function HideShowGMSInfoTextBox(obj) {

    var GMSId = $(obj).val();

    if (GMSId != null && GMSId != "" && GMSId != undefined) {
        if (GMSId == "GSM002" || GMSId == "GSM003") {
            $('#GMSInfoTxtBoxDiv').show();
        }
        else {
            $('#GMSInfoTxtBoxDiv').hide();
        }
    }
}
function IniTializeMultiSelect() {
    $('.multiselectDropDown').selectpicker({
        includeSelectAllOption: true,
        //nonSelectedText: 'Select Multiple',
        enableFiltering: true,
        buttonWidth: '100%',
        noneSelectedText: 'Select Multiple', // Placeholder text when no option is selected
        selectedTextFormat: 'count > 4', // Display count when more than 3 items are selected
        //countSelectedText: '{0} selected' // Format for displaying selected items
        countSelectedText: function (numSelected, numTotal) {
            // Customize the selected count text
            if (numSelected === 0) {
                return 'Nothing selected';
            } else {
                if (numSelected == numTotal) {
                    return (numSelected - 1) + ' of ' + (numTotal - 1) + ' selected';
                }
                else {
                    return (numSelected) + ' of ' + (numTotal - 1) + ' selected';
                }
            }
        }
    });      

    // Handle "Select All" option
    $('.multiselectDropDown').on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
        var $this = $(this);
        var selectAllOption = $this.find('option[value="0"]');

        if (clickedIndex == 0) {
            if (isSelected) {
                // Select all options except the "Select All" itself
                $this.find('option').not(selectAllOption).prop('selected', true);
            }
            else {
                $this.find('option').not(selectAllOption).prop('selected', false);
            }
        }
        else {
            // If "Select All" is not selected
            var selectedOptions = $this.find('option:selected').not(selectAllOption);
            if (selectedOptions.length === $this.find('option').length - 1) {
                // If all options except "Select All" are selected
                selectAllOption.prop('selected', true);
            } else {
                // Otherwise, uncheck the "Select All"
                selectAllOption.prop('selected', false);
            }
        }

        // Refresh the selectpicker to update the UI
        $this.selectpicker('refresh');
    })

    // Refresh the selectpicker to update the UI
    $('.multiselectDropDown').selectpicker('refresh');
}
function InitializeDatePicker() {
    $('.datepicker').datepicker({
        dateFormat: 'dd-M-yy',  // Example format: 2024-10-12
        autoclose: true,
        todayHighlight: true
    });
}
function InitializeDataTable() {
    $('.AddDataTable').DataTable({
        "scrollCollapse": false,
        "paging": true,
        "select": false,
        "orderBy": false
    });
}
function InitializeNumericsOnly() {
    $('.AllowNumbersOnly').on('keypress', function (e) {

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
}
function InitializeSelect2() {
    $('.select2').select2({
        placeholder: function () {
            return $(this).data('placeholder');
        },
        allowClear: true,
        width: 'resolve'
    });
}