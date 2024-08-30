const project = {
    init: () => {
        $("#project-create").click(() => {
            $("#modal-c").find('.form-control, .form-select').removeClass('is-invalid');
            $("#modal-c").find('.invalid-feedback').text('');

            $("#modal-c").modal('show');
            return false;
        });

        $('#project-create-save').click(() => {
            event.preventDefault();
            let isValid = true;

            // BU Type validation
            const buType = document.getElementById('bu-type');
            const buTypeError = document.getElementById('bu-type-error');
            if (buType.value === "0") {
                buType.classList.add('is-invalid');
                buTypeError.textContent = 'กรุณาเลือก BU';
                isValid = false;
            } else {
                buType.classList.remove('is-invalid');
                buTypeError.textContent = '';
            }

            // Project Type validation
            const projectType = document.getElementById('project-type');
            const projectTypeError = document.getElementById('project-type-error');
            if (projectType.value === "0") {
                projectType.classList.add('is-invalid');
                projectTypeError.textContent = 'กรุณาเลือกประเภทโครงการ';
                isValid = false;
            } else {
                projectType.classList.remove('is-invalid');
                projectTypeError.textContent = '';
            }

            // Project Code validation
            const projectCode = document.getElementById('project-code');
            const projectCodeError = document.getElementById('project-code-error');
            if (projectCode.value.trim() === '') {
                projectCode.classList.add('is-invalid');
                projectCodeError.textContent = 'กรุณากรอกรหัสโครงการ';
                isValid = false;
            } else {
                projectCode.classList.remove('is-invalid');
                projectCodeError.textContent = '';
            }

            // Project Name validation
            const projectName = document.getElementById('project-name');
            const projectNameError = document.getElementById('project-name-error');
            if (projectName.value.trim() === '') {
                projectName.classList.add('is-invalid');
                projectNameError.textContent = 'กรุณากรอกชื่อโครงการ';
                isValid = false;
            } else {
                projectName.classList.remove('is-invalid');
                projectNameError.textContent = '';
            }

            // If the form is valid, submit the form or do further processing
            if (isValid) {
                var data = {
                    BUID: buType.value,
                    ProjectTypeID: projectType.value,
                    ProjectCode: projectCode.value.trim(),
                    ProjectName: projectName.value.trim()
                };

                project.CreateProject(data);
            }
        });

        $('#project-edit-save').click(() => {
            var buType = $('#e-bu-type').val();
            var projectType = $('#e-project-type').val();

            // Get the values from the Project Code and Project Name input fields
            var projectId = $('#e-project-id').val();
            var projectCode = $('#e-project-code').val();
            var projectName = $('#e-project-name').val();
            
            // Prepare an array to store model mappings
            var modelMappings = [];
            
            // Loop through each row in the Model Mapping table to get values
            $('#modal-format table tbody tr').each(function () {
                var modelID = $(this).find('td').eq(0).text();
                var formTypeID = $(this).find('.form-type-select').val(); // Get selected form type ID

                // Push the model data into the array
                modelMappings.push({
                    ModelID: modelID,
                    FormTypeID: formTypeID
                });
            });

            var data = {
                BUID: buType,
                ProjectTypeID: projectType,
                ProjectID: projectId,
                ProjectCode: projectCode,
                ProjectName: projectName,
                ModelMapping: modelMappings
            }

            project.EditProject(data);
        });


        $('#project-delete-save').click(() => {
            var projectId = $('#delete-project-id').val();

            project.DeleteProject(projectId);
        })

        project.AjaxGrid();
    },
    AjaxGrid: function () {
        tblProject = $('#tbl-project-list').dataTable({
        "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'MasterProject/JsonAjaxGridProjectList',
                type: "POST",
                data: function (json) {
                //  ￼ var datastring = $("#form-search").serialize();
                //json = datastring + '&' + $.param(json);

                //return json;
            },
            "dataSrc": function (res) {
                //app.ajaxVerifySession(res);                    
                return res.data;
            },
            complete: function (res) {
                $(document).on('click', "button[data-action='edit-project']", function (e) {
                    var projectId = $(e.currentTarget).attr('data-id');

                    var data = {
                        ProjectID: projectId
                    };

                    project.DetailProject(data);
                    return false;
                });
                $(document).on('click', "button[data-action='delete-project']", function (e) {
                    var projectId = $(e.currentTarget).attr('data-id');

                    $('#delete-project-id').val(projectId);
                    $('#partial-confirm-delete-project').modal('show');
                    return false;
                });
            }
        },
        "ordering": true,
        "order": [[2, "desc"]],
        "columns": [
            { 'data': 'ProjectCode', "className": "text-center " },
            { 'data': 'ProjectName', "className": "text-center " },
            { 'data': 'UpdateDate', "className": "text-center " },
            {
                'data': 'ProjectId',
                "className": "text-center",
                "render": function (data, type, row, meta) {
                    var html = '<button data-action="edit-project" data-id="' + data + '" class="btn bg-skyblue-lt btn-icon btn-rounded" style="margin-right:10px;">';
                    html += '<i class="fa-regular fa-pen-to-square"></i>';
                    html += '</button>';
                    html += '<button data-action="delete-project" data-id="' + data + '" class="btn bg-red-lt btn-icon btn-rounded">';
                    html += '<i class="fa-regular fa-trash-can"></i>';
                    html += '</button>';
                    return html;
                }
            }
        ]
        });
    },
    CreateProject: function (data) {
        $.ajax({
            url: baseUrl + 'MasterProject/CreateProject',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'ทำการสร้างข้อมูลสำเร็จ',
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
    DetailProject: function (data) {
        $.ajax({
            url: baseUrl + 'MasterProject/DetailProject',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    // Populate dropdowns and input fields
                    $('#e-bu-type').val(resp.data.BUID);
                    $('#e-project-type').val(resp.data.ProjectTypeID);
                    $('#e-project-id').val(resp.data.ProjectID);
                    $('#e-project-code').val(resp.data.ProjectCode);
                    $('#e-project-name').val(resp.data.ProjectName);
                    console.log(resp.data.ProjectCode);
                    console.log(resp.data.ProjectName);
                    // Clear previous rows in the table
                    $('#modal-format table tbody').empty();

                    // Ensure ModelTypeList and FormTypeList are not empty
                    if (resp.data.ModelTypeList && resp.data.ModelTypeList.length > 0 && resp.data.FormTypeList && resp.data.FormTypeList.length > 0) {
                        // Populate Model Mapping table
                        resp.data.ModelTypeList.forEach(function (model) {
                            var formTypeOptions = resp.data.FormTypeList.map(function (formType) {
                                return `<option value="${formType.FormTypeID}" ${formType.FormTypeID === model.FormTypeID ? 'selected' : ''}>${formType.FormTypeName}</option>`;
                            }).join('');

                            $('#modal-e table tbody').append(
                                `<tr>
                                <td hidden>${model.ModelID}</td>
                                <td>${model.ModelCode}</td>
                                <td>${model.ModelName}</td>
                                <td>${model.ModelTypeCode}</td>
                                <td>${model.ModelTypeName}</td>
                                <td>
                                    <select class="form-control form-type-select">
                                        <option value="0">กรุณาเลือกประเภทฟอร์ม</option>
                                        ${formTypeOptions}
                                    </select>
                                </td>
                            </tr>`
                            );
                        });
                    } else {
                        alert("ModelTypeList or FormTypeList is empty.");
                    }

                    // Show the modal
                    $("#modal-e").modal('show');
                } else {
                    alert("Error: " + resp.message);
                }
            },
            error: function (xhr, status, error) {
                alert("Coding Error: " + error);
            },
        });
        return false;
    },
    EditProject: function (data) {
        $.ajax({
            url: baseUrl + 'MasterProject/EditProject',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'ทำการสร้างข้อมูลสำเร็จ',
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
    DeleteProject: function (projectId) {
        $.ajax({
            url: baseUrl + 'MasterProject/DeleteProject',
            type: 'post',
            dataType: 'json',
            data: {
                projectId: projectId
            },
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'ทำการสร้างข้อมูลสำเร็จ',
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
}