$(document).ready(function () {
    // Initialize Selectize for the project dropdown (single select)
    $('#DDLProjectID').selectize({
        valueField: 'Value',
        labelField: 'Text',
        searchField: 'Text',
        create: false,
        sortField: 'text',
        placeholder: 'กรุณาเลือก', // Placeholder text
        maxItems: 1 // Single select
    });

    // Initialize Selectize for the unit dropdown (multi-select)
    var $ddlUnit = $('#ddlunit').selectize({
        valueField: 'ValueGuid',
        labelField: 'Text',
        searchField: 'Text',
        create: false,
        sortField: 'Text',
        placeholder: 'กรุณาเลือกหน่วย',
        maxItems: null // Allow multiple selections
    });

    // Handle change event for project dropdown to dynamically populate unit dropdown
    $('#DDLProjectID').on('change', function () {
        var selectedProjectId = $(this).val();

        if (selectedProjectId) {
            // AJAX call to fetch units based on selected project
            $.ajax({
                url: '/UnitPayment/GetDDLUnitPass', // Ensure the URL matches your endpoint
                type: 'GET',
                data: { ProjectID: selectedProjectId },
                success: function (response) {
                    var ddlUnitSelectize = $ddlUnit[0].selectize; // Access the Selectize instance
                    ddlUnitSelectize.clearOptions(); // Clear existing options
                    ddlUnitSelectize.addOption({ ValueGuid: '', Text: 'กรุณาเลือกหน่วย' }); // Add default option

                    // Add new options from the response
                    response.forEach(function (item) {
                        ddlUnitSelectize.addOption({ ValueGuid: item.ValueGuid, Text: item.Text });
                    });

                    ddlUnitSelectize.refreshOptions(false); // Refresh the dropdown options
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching unit data:', error);
                    alert('เกิดข้อผิดพลาดในการดึงข้อมูล');
                }
            });
        } else {
            // Clear the unit dropdown if no project is selected
            var ddlUnitSelectize = $ddlUnit[0].selectize;
            ddlUnitSelectize.clearOptions();
            ddlUnitSelectize.addOption({ ValueGuid: '', Text: 'กรุณาเลือกหน่วย' });
        }
    });

    // Initialize DataTable
    setDatatable();
});

function setDatatable() {
    $('#tbunitpayment').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": false,
        "info": true,
        "autoWidth": false,
        "responsive": false,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        "language": {
            "lengthMenu": "Show _MENU_ rows per page",
            "zeroRecords": "No matching records found",
            "info": "Showing page _PAGE_ of _PAGES_",
            "infoEmpty": "No records available",
            "infoFiltered": "(filtered from _MAX_ total records)"
        }
    });
}
