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
        placeholder: 'กรุณาเลือก Unit',
        maxItems: null // Allow multiple selections
    });

    // Handle change event for project dropdown to dynamically populate unit dropdown
    $('#DDLProjectID').on('change', function () {
        var selectedProjectId = $(this).val();

        if (selectedProjectId) {
            // AJAX call to fetch units based on selected project
            $.ajax({
                url: baseUrl + 'UnitPayment/GetDDLUnitPass', // Ensure the URL matches your endpoint
                type: 'GET',
                data: { ProjectID: selectedProjectId },
                success: function (response) {
                    var ddlUnitSelectize = $ddlUnit[0].selectize; // Access the Selectize instance
                    ddlUnitSelectize.clearOptions(); // Clear existing options
/*                    ddlUnitSelectize.addOption({ ValueGuid: '', Text: 'กรุณาเลือกหน่วย' }); // Add default option*/

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
           /* ddlUnitSelectize.addOption({ ValueGuid: '', Text: 'กรุณาเลือกหน่วย' });*/
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

function openModalGRPayment(UnitFormID) {

    showLoadingScreen();

    $.ajax({
        url: baseUrl + 'UnitPayment/GetUnitFormGRDetail',
        type: 'GET',
        data: { UnitFormID: UnitFormID },
        success: function (response) {           
            if (response) {
                /*$('#projectName').val(response.ProjectName).prop('disabled', false);*/
                $('#projectName').val(response.ProjectName);
                $('#hdProjectID').val(response.ProjectID);
                $('#unitcode').val(response.UnitCode);
                $('#hdUnitID').val(response.UnitID);
                $('#hdUnitFormID').val(response.UnitFormID);
                $('#formName').val(response.FormName);
                $('#companyvenderName').val(response.CompanyVenderName);
                $('#venderName').val(response.VenderName);
                var ModalGRPayment = new bootstrap.Modal(document.getElementById('ModalGRPayment'));
                ModalGRPayment.show();
                Swal.close();
            }
        },
        error: function () {
            Swal.close();
            showErrorAlert('ผิดพลาด!', 'โหลดข้อมูลไม่สำเร็จ');
        }
    });
}


function onClickSaveGRPayment() {
    var ProjectID = document.getElementById('hdProjectID').value;
    var UnitID = document.getElementById('hdUnitID').value;
    var UnitFormID = document.getElementById('hdUnitFormID').value;
    var PONO = document.getElementById('poInput').value;
    var GRNO = document.getElementById('grInput').value;
    var PercentPayment = document.getElementById('percentInput').value;
    var Remark = document.getElementById('remark').value;


    if (!PONO) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาระบุ PO');
        return;
    }
    if (!GRNO) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาระบุ GR');
        return;
    }
    if (!PercentPayment) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาระบุเปอร์เซ็นต์');
        return;
    }
    if (!Remark) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาหมายเหตุให้กับทาง vendor portal ทราบ');
        return;
    }


    var formData = new FormData();
    formData.append('ProjectID', ProjectID);
    formData.append('UnitID', UnitID);
    formData.append('UnitFormID', UnitFormID);
    formData.append('GRNO', GRNO);
    formData.append('PONO', PONO);
    formData.append('Remark', Remark);
    formData.append('PercentPayment', PercentPayment);

    showLoadingAlert();

    $.ajax({
        url: baseUrl + 'UnitPayment/SaveUnitFormGRPaymentData',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            Swal.close();
            if (response.success) {
                showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ');
            } else {
                showErrorAlertNotCloseModal('บันทึกข้อมูลไม่สำเร็จ', response.message || 'เกิดข้อผิดพลาดในการบันทึกข้อมูล');
            }
        },
        error: function (xhr, status, error) {
            Swal.close();
            showErrorAlertNotCloseModal('เกิดข้อผิดพลาด!', error);
        }
    });
}
