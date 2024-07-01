function fetchData(selectedValue, selectedValue2) {
    console.log('Fetching data with:', selectedValue, selectedValue2);
    console.log('Url':,baseUrl);
    $.ajax({
        url: baseUrl + 'FormOverall/ProjectFromList',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            formId: selectedValue,
            typeId: selectedValue2
        }),
        success: function (response) {
            console.log('success');
            // Handle the response data here
            //updateUI(data);
            console.log(response);
            return response;
        },
        error: function () {
            alert('Error loading data.');
            console.log('fail');
        }
    });
}

function updateTable(data) {
    var tableBody = $('#tableBody');
    tableBody.empty();  // Clear existing table rows

    tableBody.dataTable({

    })
}

