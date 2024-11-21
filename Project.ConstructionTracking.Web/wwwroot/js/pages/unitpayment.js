$(document).ready(function () {
    // Initialize Selectize for the project dropdown (single select)
    $('#DDLProjectID').selectize({
        valueField: 'Value',
        labelField: 'Text',
        searchField: 'Text',
        create: false,
        sortField: 'text',
        placeholder: 'กรุณาเลือกโครงการ', // Placeholder text
        maxItems: 1 // Single select
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

async function openModalGRPayment(UnitFormID) {
    try {
        showLoadingScreen();

        // Fetch details for the form
        const response = await $.ajax({
            url: baseUrl + 'UnitPayment/GetUnitFormGRDetail',
            type: 'GET',
            data: { UnitFormID: UnitFormID }
        });

        if (response) {
            $('#projectName').val(response.ProjectName);
            $('#hdProjectID').val(response.ProjectID);
            $('#unitcode').val(response.UnitCode);
            $('#hdUnitID').val(response.UnitID);
            $('#hdUnitFormID').val(response.UnitFormID);
            $('#formName').val(response.FormName);
            $('#companyvenderName').val(response.CompanyVenderName);
            $('#venderName').val(response.VenderName);

            // Load the table data asynchronously
            await fetchListUnitFormGRPaymentTable(UnitFormID);

            const ModalGRPayment = new bootstrap.Modal(document.getElementById('ModalGRPayment'));
            ModalGRPayment.show();
            Swal.close();

        }
    } catch (error) {
        Swal.close();
        showErrorAlert('ผิดพลาด!', 'โหลดข้อมูลไม่สำเร็จ');
    }
}

async function fetchListUnitFormGRPaymentTable(UnitFormID) {
    try {
        const response = await $.ajax({
            url: baseUrl + 'UnitPayment/FetchListUnitFormGRPaymentTable',
            type: 'GET',
            data: { UnitFormID: UnitFormID }
        });

        // Replace the content of the table container with the new content
        $('#PartialTableListGRPayment').html(response);

    } catch (error) {
        showErrorAlert('ผิดพลาด!', 'โหลดตารางไม่สำเร็จ');
    }
}

async function onClickSaveGRPayment() {
    const ProjectID = document.getElementById('hdProjectID').value;
    const UnitID = document.getElementById('hdUnitID').value;
    const UnitFormID = document.getElementById('hdUnitFormID').value;
    const PONO = document.getElementById('poInput').value.trim();
    const GRNO = document.getElementById('grInput').value.trim();
    const PercentPayment = document.getElementById('percentInput').value.trim();
    const Remark = document.getElementById('remark').value.trim();

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
    //if (!Remark) {
    //    showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาหมายเหตุให้กับทาง vendor portal ทราบ');
    //    return;
    //}

    const formData = new FormData();
    formData.append('ProjectID', ProjectID);
    formData.append('UnitID', UnitID);
    formData.append('UnitFormID', UnitFormID);
    formData.append('GRNO', GRNO);
    formData.append('PONO', PONO);
    formData.append('Remark', Remark);
    formData.append('PercentPayment', PercentPayment);

    showConfirmationAlert(
        'ยืนยันการบันทึกเลข GR Payment',
        'คุณต้องการบันทึกเลข GR Payment นี้ใช่หรือไม่?',
        'warning',
        'ใช่',
        'ยกเลิก',
        async function () {
            // Show loading indicator
            showLoadingAlert();

            try {
                const response = await $.ajax({
                    url: baseUrl + 'UnitPayment/SaveUnitFormGRPaymentData',
                    type: 'POST',
                    data: formData,
                    contentType: false,
                    processData: false
                });

                Swal.close();
                if (response.success) {
                    await fetchListUnitFormGRPaymentTable(UnitFormID);
                    document.getElementById('poInput').value = "";
                    document.getElementById('grInput').value = "";
                    document.getElementById('percentInput').value = "";
                    document.getElementById('remark').value = "";
                    showSuccessAlert('สำเร็จ!', response.message);
                } else {
                    showErrorAlertNotCloseModal(response.message, 'บันทึกข้อมูลไม่สำเร็จ' || 'เกิดข้อผิดพลาดในการบันทึกข้อมูล');
                }
            } catch (error) {
                Swal.close();
                showErrorAlertNotCloseModal('เกิดข้อผิดพลาด!', error);
            }
        }
    );
}

async function onClickRemoveGRPayment(ID) {
    const UnitFormID = document.getElementById('hdUnitFormID').value;

    const formData = new FormData();
    formData.append('ID', ID);

    showConfirmationAlert(
        'ยืนยันการลบ GR Payment',
        'คุณต้องการลบ GR Payment นี้ใช่หรือไม่?',
        'warning',
        'ใช่',
        'ยกเลิก',
        async function () {
            // Show loading indicator
            showLoadingAlert();

            try {
                showLoadingAlert();

                const response = await $.ajax({
                    url: baseUrl + 'UnitPayment/RemoveUnitFormGRPaymentData',
                    type: 'POST',
                    data: formData,
                    contentType: false,
                    processData: false
                });

                Swal.close();
                if (response.success) {
                    await fetchListUnitFormGRPaymentTable(UnitFormID);
                    showSuccessAlert('สำเร็จ!', response.message);

                } else {
                    showErrorAlertNotCloseModal(response.message || 'ลบข้อมูลไม่สำเร็จ', 'เกิดข้อผิดพลาดในการลบข้อมูล');
                }
            } catch (error) {
                Swal.close();
                showErrorAlertNotCloseModal('เกิดข้อผิดพลาด!', error);
            }
        }
    );
}

async function onClickSyncGRPayment(ID) {
    const UnitFormID = document.getElementById('hdUnitFormID').value;

    const formData = new FormData();
    formData.append('ID', ID);

    showConfirmationAlert(
        'ยืนยันการ Sync GR Payment',
        'คุณต้องการ Sync GR Payment นี้ใช่หรือไม่?',
        'warning',
        'ใช่',
        'ยกเลิก',
        async function () {
            // Show loading indicator
            showLoadingAlert();

            try {
                showLoadingAlert();

                const response = await $.ajax({
                    url: baseUrl + 'UnitPayment/SyncUnitFormGRPaymentData',
                    type: 'POST',
                    data: formData,
                    contentType: false,
                    processData: false
                });

                Swal.close();
                if (response.success) {
                    await fetchListUnitFormGRPaymentTable(UnitFormID);
                    showSuccessAlert('สำเร็จ!', response.message);

                } else {
                    showErrorAlertNotCloseModal(response.message || 'Sync ข้อมูลไม่สำเร็จ', 'เกิดข้อผิดพลาดในการลบข้อมูล');
                }
            } catch (error) {
                Swal.close();
                showErrorAlertNotCloseModal('เกิดข้อผิดพลาด!', error);
            }
        }
    );
}

function onClickClearinputsaveGR() {
    document.getElementById('poInput').value = "";
    document.getElementById('grInput').value = "";
    document.getElementById('percentInput').value = "";
    document.getElementById('remark').value = "";
}

function searchByProjectAndUnit() {

    const selectedProjectId = document.getElementById('DDLProjectID').value;
    const unitSearchValue = document.getElementById('txtunitsearch').value;

    //if (!selectedProjectId) {
    //    showAlertandClose('กรุณาเลือกโครงการก่อนทำการค้นหา');
    //    return;
    //}

    showLoadingScreen();

    $.ajax({
        url: baseUrl + 'UnitPayment/SearchClick', 
        type: 'POST',
        data: {
            projectId: selectedProjectId,
            unitSearch: unitSearchValue
        },
        success: function (result) {

            $('#PartialTableReport').html(result);
            Swal.close(); 
        },
        error: function (xhr, status, error) {
            console.error('Error during the search:', error);
            Swal.close(); 
        }
    });
}

document.getElementById('searchButton').addEventListener('click', searchByProjectAndUnit);
