const unit = {
    init: () => {
        $('#dropdown-select-project').change(() => {
            if ($.fn.DataTable.isDataTable('#tbl-unit-list')) {
                $('#tbl-unit-list').DataTable().clear().destroy();
            }

            var selectProjectID = $('#dropdown-select-project').val();

            unit.UnitList(selectProjectID);

            return false;
        });

        $('#btn-search').click(() => {
            if ($.fn.DataTable.isDataTable('#tbl-unit-list')) {
                $('#tbl-unit-list').DataTable().clear().destroy();
            }

            var selectProjectID = $('#dropdown-select-project').val();
            unit.UnitList(selectProjectID);

            return false;
        })
        $("#unit-create").click(() => {
            // Clear form fields in the modal
            $("#modal-unit-c").find("input[type=text], textarea, select").val("");

            // Optional: Reset any specific fields or states
            // Example: Resetting a specific select dropdown
            $("#modal-unit-c").find("select").prop("selectedIndex", 0);

            $("#modal-unit-c").modal('show');
            return false;
        });

        $('#create-unit-save').click(() => {
            event.preventDefault(); // Prevent form submission for validation
            let isValid = true;

            // Clear previous error messages and reset styles
            document.querySelectorAll('.form-control').forEach(input => {
                input.style.borderColor = ''; // Reset border color
            });
            document.querySelectorAll('.text-danger').forEach(span => {
                span.textContent = ''; // Clear error messages
            });

            // Validate Project List
            const projectList = document.getElementById('u-project-list');
            if (projectList.value === '0') {
                isValid = false;
                projectList.style.borderColor = 'red';
                document.getElementById('u-project-list-error').textContent = 'กรุณาเลือกโครงการ';
            }

            // Validate Unit Type List
            const unitTypeList = document.getElementById('u-unit-type-list');
            if (unitTypeList.value === '0') {
                isValid = false;
                unitTypeList.style.borderColor = 'red';
                document.getElementById('u-unit-type-list-error').textContent = 'กรุณาเลือกประเภทยูนิต';
            }

            // Validate Model Type List
            const modelTypeList = document.getElementById('u-model-type-list');
            if (modelTypeList.value === '0') {
                isValid = false;
                modelTypeList.style.borderColor = 'red';
                document.getElementById('u-model-type-list-error').textContent = 'กรุณาเลือกประเภทโมเดล';
            }

            // Validate Unit Code
            const unitCode = document.getElementById('u-unit-code');
            if (unitCode.value.trim() === '') {
                isValid = false;
                unitCode.style.borderColor = 'red';
                document.getElementById('u-unit-code-error').textContent = 'กรุณากรอกรหัสยูนิต';
            }

            // Validate Address Number
            const addrNo = document.getElementById('u-addr-no');
            if (addrNo.value.trim() === '') {
                isValid = false;
                addrNo.style.borderColor = 'red';
                document.getElementById('u-addr-no-error').textContent = 'กรุณากรอกเลขที่อยู่';
            }

            // Validate Area
            const area = document.getElementById('u-area');
            if (area.value.trim() === '' || parseFloat(area.value) <= 0) {
                isValid = false;
                area.style.borderColor = 'red';
                document.getElementById('u-area-error').textContent = 'กรุณากรอกเนื้อที่ที่ถูกต้อง';
            }

            if (isValid) {
                // Get form values
                var projectID = $('#u-project-list').val();
                var unitTypeID = $('#u-unit-type-list').val();
                var modelTypeID = $('#u-model-type-list').val();
                var unitCodes = $('#u-unit-code').val();
                var addrNos = $('#u-addr-no').val();
                var areas = $('#u-area').val();

                var data = {
                    ProjectID: projectID,
                    UnitTypeID: unitTypeID,
                    ModelTypeID: modelTypeID,
                    UnitCode: unitCodes,
                    UnitAddress: addrNos,
                    UnitArea: areas
                }

                unit.CreateUnit(data);
            }
        });

        $('#edit-unit-save').click(() => {
            event.preventDefault(); // Prevent form submission for validation
            let isValid = true;

            // Clear previous error messages and reset styles
            document.querySelectorAll('.form-control').forEach(input => {
                input.style.borderColor = ''; // Reset border color
            });
            document.querySelectorAll('.text-danger').forEach(span => {
                span.textContent = ''; // Clear error messages
            });

            // Validate Vendor
            const vendor = document.getElementById('company-vendor');
            if (vendor.value.trim() === '') {
                isValid = false;
                vendor.style.borderColor = 'red';
                document.getElementById('company-vendor-error').textContent = 'กรุณาเลือกบริษัทผู้รับเหมา';
            }

            // Validate PO Number
            const poNo = document.getElementById('po-no');
            if (poNo.value.trim() === '') {
                isValid = false;
                poNo.style.borderColor = 'red';
                document.getElementById('po-no-error').textContent = 'กรุณากรอก PO';
            }

            // Validate Start Date
            const startDate = document.getElementById('start-date');
            if (startDate.value.trim() === '') {
                isValid = false;
                startDate.style.borderColor = 'red';
                document.getElementById('start-date-error').textContent = 'กรุณากรอกวันที่เริ่มทำงาน';
            }

            if (isValid) {
                var unitId = $('#edit-unit-id').val();
                var projectId = $('#edit-project-id').val();
                var companyVendorId = $('#company-vendor').val();
                var poNos = $('#po-no').val();
                var startDates = $('#start-date').val();

                var data = {
                    UnitID: unitId,
                    ProjectID: projectId,
                    CompanyVendorID: companyVendorId,
                    PONo: poNos,
                    StartDate: startDates
                }

                unit.EditUnit(data);
            }
        });

        $('#unit-delete-save').click(() => {
            var unitId = $('#delete-unit-id').val();
            var data = {
                unitID: unitId
            }

            unit.DeleteUnit(data);
        })

        unit.UnitList();
    },
    UnitList: function (selectedProjectID) {
        tblUnit = $('#tbl-unit-list').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'MasterUnit/UnitList',
                type: "POST",
                data: function (json) {
                    var datastring = $("#form-search").serialize();
                    json.ProjectID = selectedProjectID
                    json = datastring + '&' + $.param(json);

                    return json;
                },
                "dataSrc": function (res) {
                    //app.ajaxVerifySession(res);                    
                    return res.data;
                },
                complete: function (res) {
                    $(document).on('click', "button[data-action='edit-unit']", function (e) {
                        var unitId = $(e.currentTarget).attr('data-id');
                        
                        var data = {
                            unitID: unitId
                        }

                        unit.GetDetailUnit(data);
                    });
                    $(document).on('click', "button[data-action='delete-unit']", function (e) {
                        var unitId = $(e.currentTarget).attr('data-id');
                        
                        $('#delete-unit-id').val(unitId);
                        $('#partial-confirm-delete-unit').modal('show');
                        return false;
                    });
                }
            },
            "ordering": true,
            "order": [[11, "desc"]], // Ordering by the "UpdateDate" column (index 11)
            "columns": [
                { 'data': 'ProjectCode', "className": "text-center" },
                { 'data': 'ProjectName', "className": "text-center" },
                { 'data': 'UnitCode', "className": "text-center" },
                { 'data': 'UnitTypeName', "className": "text-center" },
                { 'data': 'UnitAddress', "className": "text-center" },
                { 'data': 'UnitArea', "className": "text-center" },
                { 'data': 'UnitStatusDesc', "className": "text-center" },
                { 'data': 'UnitVendorName', "className": "text-center" },
                { 'data': 'UnitPO', "className": "text-center" },
                { 'data': 'UnitStartDate', "className": "text-center" },
                { 'data': 'UnitEndDate', "className": "text-center" },
                { 'data': 'UpdateDate', "className": "text-center" },
                {
                    'data': 'UnitID',
                    "className": "text-center",
                    "render": function (data, type, row, meta) {
                        var html = '<button data-action="edit-unit" data-id="' + data + '" class="btn bg-skyblue-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<button data-action="delete-unit" data-id="' + data + '" class="btn bg-red-lt btn-icon btn-rounded">';
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>';
                        return html;
                    }
                }
            ]
        });
    },
    GetDetailUnit: function (data) {
        $.ajax({
            url: baseUrl + 'MasterUnit/GetDetail',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {

                    $('#edit-unit-id').val(resp.data.UnitID);
                    $('#edit-project-id').val(resp.data.ProjectID);

                    // Populate vendor dropdown
                    var vendorSelect = $('#company-vendor');
                    vendorSelect.empty(); // Clear existing options
                    vendorSelect.append('<option value="">กรุณาเลือกผู้ควบคุมงาน</option>'); // Default option

                    $.each(resp.data.CompanyVendorList, function (index, vendor) {
                        vendorSelect.append('<option value="' + vendor.VendorID + '">' + vendor.VendorName + '</option>');
                    });

                    // Set other values in the modal with null checks
                    if (resp.data.CompanyVendorID !== null) {
                        $('#company-vendor').val(resp.data.CompanyVendorID);
                    } else {
                        $('#company-vendor').val(''); // Set to empty if null
                    }

                    $('#po-no').val(resp.data.PONo || ''); // Set PO number or empty if null

                    if (resp.data.StartDate) {
                        var startDate = new Date(resp.data.StartDate);
                        var day = ("0" + startDate.getDate()).slice(-2);
                        var month = ("0" + (startDate.getMonth() + 1)).slice(-2);
                        var year = startDate.getFullYear();
                        $('#start-date').val(year + '-' + month + '-' + day); // Format to yyyy-MM-dd
                    } else {
                        $('#start-date').val(''); // Set to empty if null
                    }

                    $("#modal-unit-e").modal('show');
                }
                else {
                    alert("Error: " + resp.message);
                }
            },
            error: function (xhr, status, error) {
                // do something
                alert(" Coding Error ")
            },
        });
        return false;
    },
    CreateUnit: function (data) {
        $.ajax({
            url: baseUrl + 'MasterUnit/CreateUnit',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'ทำการสร้างข้อมูลสำเร็จ',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        title: "ทำรายการไม่สำเร็จ",
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr, status, error) {
                // do something
                alert(" Coding Error ")
            },
        });
        return false;
    },
    EditUnit: function (data) {
        $.ajax({
            url: baseUrl + 'MasterUnit/EditUnit',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) { 
                if (resp.success) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'ทำการแก้ไขข้อมูลสำเร็จ',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: "ทำการแก้ไขข้อมูลไม่สำเร็จ",
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr, status, error) {
                // do something
                alert(" Coding Error ")
            },
        });
        return false;
    },
    DeleteUnit: function (data) {
        $.ajax({
            url: baseUrl + 'MasterUnit/DeleteUnit',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'ทำการลบข้อมูลสำเร็จ',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: "ทำรายการไม่สำเร็จ",
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr, status, error) {
                // do something
                alert(" Coding Error ")
            },
        });
        return false;
    },
}