$(document).ready(function () {
    // Initialize Selectize for the project dropdown (single select)
    $('#DDLProjectID').selectize({
        valueField: 'Value',
        labelField: 'Text',
        searchField: 'Text',
        create: false,
        sortField: 'text',
        placeholder: 'ทั้งหมด', // Placeholder text
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
            $('#poInput').val(response.PONo);

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

async function openModalViewGRPayment(UnitFormID) {
    try {
        showLoadingScreen();

        const response = await $.ajax({
            url: baseUrl + 'UnitPayment/GetUnitFormGRDetail',
            type: 'GET',
            data: { UnitFormID: UnitFormID }
        });

        if (response) {
            $('#viewProjectName').text(response.ProjectName);
            $('#viewUnitcode').text(response.UnitCode);
            $('#viewFormname').text(response.FormName);
            $('#viewVenderCompany').text(response.CompanyVenderName + ' (' + response.VenderName + ')');

            // Clear the table body before appending new data
            $('#TableBodyViewGRPayment').empty();

            // Fetch table data
            await fetchListViewTableGRPaymentTable(UnitFormID);

            // Show modal
            const ModalViewGRPayment = new bootstrap.Modal(document.getElementById('ModalViewGRPayment'));
            ModalViewGRPayment.show();
            Swal.close();
        }
    } catch (error) {
        Swal.close();
        showErrorAlert('ผิดพลาด!', 'โหลดข้อมูลไม่สำเร็จ');
    }
}

async function fetchListViewTableGRPaymentTable(UnitFormID) {
    try {
        const response = await $.ajax({
            url: baseUrl + 'UnitPayment/FetchListUnitFormViewGRPaymentTable',
            type: 'GET',
            data: { UnitFormID: UnitFormID }
        });

        if (!response || response.length === 0) {
            $('#TableBodyViewGRPayment').html('<tr><td colspan="7" class="text-center">ไม่มีข้อมูล</td></tr>');
            return;
        }

        let rowIndex = 1; // Initialize running number
        $('#TableBodyViewGRPayment').empty(); // Clear table before appending
        response.forEach((item) => {

        const syncBadge = item.SyncStatusID === 31
            ? `<span class="badge bg-success rounded-pill text-white">${item.SyncStatusName}</span>`
            : `<span class="badge bg-danger rounded-pill text-white">${item.SyncStatusName}</span><br>${item.SyncMessage || ''}`;

            $('#TableBodyViewGRPayment').append(`
                <tr>
                    <td>${rowIndex}</td>
                    <td>${item.PONO}</td>
                    <td>${item.GRNO}</td>
                    <td><span class="text-primary">${item.PercentPayment} %</span></td>
                    <td>${item.Remark}</td>
                    <td>${syncBadge}</td>
                    <td>
                        <i class="fa-regular fa-calendar"></i> ${item.UpdateDate || ''}
                        <br>
                        <i class="fa-regular fa-user"></i> ${item.CreateBy || ''}
                    </td>
                </tr>
            `);
            rowIndex++; // Increment the counter
        });
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
    const PercentPayment = document.getElementById('DDLPercentPaymentID').value;
    const Remark = document.getElementById('remark').value.trim();

    if (!PONO) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาระบุ PO');
        return;
    }
    if (!GRNO) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาระบุ GR');
        return;
    }
    if (PercentPayment === "-1") {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาเลือกเปอร์เซ็นต์');
        return;
    }

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
                if (response.success) {
                    Swal.close();
                    await fetchListUnitFormGRPaymentTable(UnitFormID);
                    await searchByProjectAndUnit();
                    document.getElementById('poInput').value = "";
                    document.getElementById('grInput').value = "";
                    document.getElementById('DDLPercentPaymentID').value = "-1";
                    document.getElementById('remark').value = "";

                    if (response.message === 'เบิกงวดงานนี้ครบ 100% แล้ว') {
                        const closeButton = document.getElementById('closeModalGRPayment');
                        closeButton.click();
                        await openModalViewGRPayment(UnitFormID);                       
                    }

                    showSuccessAlert('สำเร็จ!', response.message);

                } else {
                    Swal.close();
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
                    const searchButton = document.getElementById('searchButton');
                    searchButton.click();
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
    formData.append('UnitFormID', UnitFormID);

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

                    await searchByProjectAndUnit();
                  
                    if (response.message === 'เบิกงวดงานนี้ครบ 100% แล้ว') {
                        const closeButton = document.getElementById('closeModalGRPayment');
                        closeButton.click();
                        await openModalViewGRPayment(UnitFormID);
                    }

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

async function searchByProjectAndUnit() {

    const selectedProjectId = document.getElementById('DDLProjectID').value;
    const selectedStatusGrPayment = document.getElementById('DDLStatusGrPayment').value;
    const unitSearchValue = document.getElementById('txtunitsearch').value;

    showLoadingScreen();

    $.ajax({
        url: baseUrl + 'UnitPayment/SearchClick', 
        type: 'POST',
        data: {
            projectId: selectedProjectId,
            unitSearch: unitSearchValue,
            status: selectedStatusGrPayment
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

function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        // alert('Copied: ' + text);
    }).catch(err => {
        console.error('Could not copy text: ', err);
    });
}

document.getElementById('Export').addEventListener('click', ExportToExcel);

async function ExportToExcel() {

    const selectedProjectId = document.getElementById('DDLProjectID').value;
    const unitSearchValue = document.getElementById('txtunitsearch').value;

    showLoadingScreen();

    const exportUrl = `UnitPayment/ExportToExcel?projectId=${encodeURIComponent(selectedProjectId)}&unitSearch=${encodeURIComponent(unitSearchValue)}`;

    fetch(exportUrl)
        .then(response => {
            if (!response.ok) {
                Swal.close();
                showAlertandClose('Export Excel ไม่สำเร็จ');
                throw new Error('Error occurred during file download.');
            }
            Swal.close();
            return response.blob();
        })
        .then(blob => {
            Swal.close();
            const link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = `รายงานการSysn_online_billing.xlsx`;
            link.click();
        })
        .catch(error => {
            Swal.close();
            console.error('Export failed:', error);
            showAlertandClose('การส่งออกข้อมูลล้มเหลว');
        });
}