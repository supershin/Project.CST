

const detailCom = {
    init: () => {
        var companyId = $('#company-id').val();
        detailCom.AjaxGrid(companyId);

        $('#multiple-select-project').select2();

        $('#save-mapping').click(() => {
            event.preventDefault(); // Prevent form submission
            var isValid = true;

            // Validate company name
            var companyName = document.getElementById('company-name').value.trim();
            if (companyName === '') {
                setError('company-name', 'กรุณากรอกชื่อบริษัท');
                isValid = false;
            } else {
                clearError('company-name');
            }

            if (isValid) {
                var selectedProjects = $('#multiple-select-project').val();

                var data = {
                    CompanyVendorID: companyId,
                    ProJectIDList: selectedProjects
                }
                console.log(data);

                detailCom.MappingProject(data);
            }
        })

        $("#create-vendor").click(() => {
            event.preventDefault(); // Prevent form submission
            var isValid = true;

            // Validate vendor name
            var vendorName = document.getElementById('vendor-name').value.trim();
            if (vendorName === '') {
                setError('vendor-name', 'กรุณากรอกชื่อผู้ควบคุมงาน');
                isValid = false;
            } else {
                clearError('vendor-name');
            }

            // Validate vendor email
            var vendorEmail = document.getElementById('vendor-email').value.trim();
            if (vendorEmail === '') {
                setError('vendor-email', 'กรุณากรอกอีเมลล์');
                isValid = false;
            } else {
                clearError('vendor-email');
            }

            if (isValid) {
                var vendorName = $('#vendor-name').val();
                var vendorEmail = $('#vendor-email').val();

                var data = {
                    CompanyVendorID: companyId,
                    Name: vendorName,
                    Email: vendorEmail
                }

                detailCom.CreateVendor(data);
            }
        });

        $('#edit-vendor').click(() => {
            event.preventDefault();

            // Get values from inputs
            var vendorName = document.getElementById('m-vendor-name').value.trim();
            var vendorEmail = document.getElementById('m-vendor-email').value.trim();

            // Validation flags
            var isValid = true;

            // Clear previous error messages
            clearErrorsModal();

            // Validate vendor name
            if (vendorName === '') {
                showErrorModal('m-vendor-name', 'กรุณาใส่ชื่อผู้ควบคุมงาน');
                isValid = false;
            }

            // Validate vendor email
            if (vendorEmail === '') {
                showErrorModal('m-vendor-email', 'กรุณากรอกข้อมูลอีเมลล์');
                isValid = false;
            }

            // If valid, allow the form to be submitted or close the modal
            if (isValid) {
                var vendorID = $('#m-vendor-id').val();
                var name = $('#m-vendor-name').val();
                var email = $('#m-vendor-email').val();

                var data = {
                    VendorID: vendorID,
                    Name: name,
                    Email: email
                }

                detailCom.EditVendor(data);
            }
        });

        $('#company-vendor-save').click(() => {
            var vendorId = $('#vendorID').val();

            var data = {
                VendorID: vendorId
            }
            detailCom.DeleteVendor(data);
        });
    },
    AjaxGrid: function (companyId) {
        tblVendor = $('#tbl-vendor').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'mastercompany/GetListMasterVendor',
                type: "POST",
                data: function (json) {
                    var datastring = $("#form-search").serialize();
                    json.CompanyId = companyId;
                    json = datastring + '&' + $.param(json);

                    return json;
                },
                "dataSrc": function (res) {
                    return res.data;
                },
                complete: function (res) {
                    $(document).on('click', "button[data-action='vendor-detail']", function (e) {
                        var vendorId = $(e.currentTarget).attr('data-id');
                        var data = {
                            VendorID: vendorId
                        }

                        detailCom.DetailVendor(data);
                    });
                    $(document).on('click', "button[data-action='vendor-delete']", function (e) {
                        //e.preventDefault(); // Prevent default action
                        var vendorId = $(e.currentTarget).attr('data-id');
                        $('#vendorID').val(vendorId);
                        $('#partial-confirm-delete').modal('show');
                    });
                }
            },
            "ordering": true,
            "order": [[0, "ASC"]],
            "columns": [
                { 'data': 'Name', "className": "text-center " },
                { 'data': 'Email', "className": "text-center " },
                { 'data': 'UpdateDate', "className": "text-center " },
                {
                    'data': 'ID',
                    'orderable': false,
                    'width': '20px',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<button  data-action="vendor-detail" data-id="' + data.ID + '" class="btn bg-black-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<span>';
                        html += '<button data-action="vendor-delete" data-id ="' + data.ID + '"class="btn bg-red-lt btn-icon btn-rounded">'
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>';
                        html += '</span>';
                        return html;
                    }
                },
            ]
        });
    },
    MappingProject: function (data) {
        $.ajax({
            url: baseUrl + 'MasterCompany/MappingCompanyVendorProject',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    window.location.reload();
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
    CreateVendor: function (data) {
        $.ajax({
            url: baseUrl + 'MasterCompany/CreateVendor',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    $('#vendor-name').val('');
                    $('#vendor-email').val('');

                    if ($.fn.DataTable.isDataTable('#tbl-vendor')) {
                        $('#tbl-vendor').DataTable().clear().destroy();
                    }

                    detailCom.AjaxGrid(resp.data.CompanyVendorID);
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
    DetailVendor: function (data) {
        $.ajax({
            url: baseUrl + 'MasterCompany/ActionVendor',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    clearErrorsModal();
                    $('#m-vendor-id').val(resp.data.VendorID);
                    $('#m-vendor-name').val(resp.data.Name);
                    $('#m-vendor-email').val(resp.data.Email);
                    $('#modal-vendor-e').modal('show');
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
    EditVendor: function (data) {
        $.ajax({
            url: baseUrl + 'MasterCompany/ActionVendor',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'ทำการแก้ไขข้อมูลสำเร็จ',
                        showConfirmButton: false,
                        timer: 1500
                    }).then(() => {
                        window.location.reload();
                    });
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
    DeleteVendor: function (data) {
        $.ajax({
            url: baseUrl + 'MasterCompany/ActionDelete',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'ทำการลบข้อมูลสำเร็จ',
                        showConfirmButton: false,
                        timer: 1500
                    }).then(() => {
                        window.location.reload();
                    });
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
    }
}

function setError(elementId, message) {
    var inputElement = document.getElementById(elementId);
    var errorElement = document.getElementById(elementId + '-error');

    inputElement.classList.add('input-error'); // Add red border
    errorElement.textContent = message; // Display error message
}

function clearError(elementId) {
    var inputElement = document.getElementById(elementId);
    var errorElement = document.getElementById(elementId + '-error');

    inputElement.classList.remove('input-error'); // Remove red border
    errorElement.textContent = ''; // Clear error message
}

function clearErrorsModal() {
    var errorElements = document.querySelectorAll('.error-message');
    errorElements.forEach(function (element) {
        element.textContent = '';
    });

    var inputElements = document.querySelectorAll('.form-control');
    inputElements.forEach(function (element) {
        element.classList.remove('is-invalid');
    });

}

// Function to show an error message
function showErrorModal(inputId, message) {
    var inputElement = document.getElementById(inputId);
    var errorElement = document.createElement('div');
    errorElement.className = 'error-message text-danger';
    errorElement.textContent = message;
    inputElement.parentElement.appendChild(errorElement);

    // Add class to style the input box
    inputElement.classList.add('is-invalid');
}