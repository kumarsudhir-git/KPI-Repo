/*This function is used to convert date to desired date format in DataTables*/
Date.prototype.toShortDateFormat = function () {
    let monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    let day = pad_with_zeroes(this.getDate(), 2);

    let monthIndex = this.getMonth();
    let monthName = monthNames[monthIndex];

    let year = this.getFullYear();

    return `${day}-${monthName}-${year}`;
}


/*This function is used to add zero in from of date in DataTables - if date is 1 diigit number*/
function pad_with_zeroes(number, length) {
    var my_string = '' + number;
    while (my_string.length < length) {
        my_string = '0' + my_string;
    }

    return my_string;
}