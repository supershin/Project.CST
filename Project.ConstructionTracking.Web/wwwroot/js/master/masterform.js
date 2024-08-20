const form = {
    init: () => {
        form.AjaxGrid();

        $("#form-create").click(() => {
            $("#modal-form-c").modal('show');
            return false;
        });

        $("#createFormType").click(() => {
            let type = 1;
            var model = {
                projectTypeId: $('#selectProjectType').val(),
                formTypeId: null,
                formTypeName: $('#nameFormType').val(),
                formTypeDesc: $('#descFormType').val(),
            }
            form.ActionFormType(type, model)
        });

        $("#editFormType").click(() => {
            let type = 2;
            var model = {
                projectTypeId: $('#modalProjectType').val(),
                formTypeId: $('#modalFormTypeId').val(),
                formTypeName: $('#modalFormTypeName').val(),
                formTypeDesc: $('#modalFormTypeDesc').val(),
            }
            console.log(model)
            form.ActionFormType(type, model) 
        });

        $('#delete-save').click(() => {
            let type = 3;
            var model = {
                TypeData: type,
                FormTypeID: $('#formTypeID').val()
            }
            form.DeleteFormType(model);
        })
    },
    AjaxGrid: function () {
        tblUnit = $('#tbl-table-mform').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'MasterForm/JsonAjaxGridFormTypeList',
                type: "POST",
                data: function (json) {
                    var datastring = $("#form-search").serialize();
                    json = datastring + '&' + $.param(json);

                    return json;
                },
                "dataSrc": function (res) {
                    //app.ajaxVerifySession(res);                    
                    return res.data;
                },
                complete: function (res) {
                    $("button[data-action='form-detail']").unbind('click').click((e) => {

                        let formTypeId = $(e.currentTarget).attr('data-id');

                        const newUrl = baseUrl + 'masterform/detail?ID=' + formTypeId;
                        // Redirect ไปยัง URL ใหม่
                        window.location.href = newUrl;

                        return false;
                    });
                    $(document).on('click', "button[data-action='form-edit']", function (e) {
                        //e.preventDefault(); // Prevent default action

                        var formTypeId = $(e.currentTarget).attr('data-id');
                        
                        var data = {
                            FormTypeId: formTypeId,
                        };

                        $.ajax({
                            url: baseUrl + 'masterform/getdetail',
                            type: 'post',
                            dataType: 'json',
                            data: data,
                            success: function (resp) {
                                if (resp.success) {
                                    $('#modalProjectType').val(resp.data.ProjectTypeId); // Set selected option
                                    $('#modalFormTypeName').val(resp.data.FormTypeName);
                                    $('#modalFormTypeDesc').val(resp.data.Description);

                                    $('#modalFormTypeId').val(resp.data.FormTypeId);

                                    // Show the modal
                                    $("#modal-form-e").modal('show');
                                }
                            },
                            error: function (xhr, status, error) {
                                // do something
                                alert(" Coding Error ")
                            },
                        });
                        return false;
                    });
                    $(document).on('click', "button[data-action='form-delete']", function (e) {
                        //e.preventDefault(); // Prevent default action

                        var formTypeId = $(e.currentTarget).attr('data-id');

                        $('#formTypeID').val(formTypeId);

                        $('#partial-confirm-delete-FT').modal('show');
                        
                    });

                }
            },
            "ordering": true,
            "order": [[3, "desc"]],
            "columns": [
                { 'data': 'Name', "className": "text-center " },
                { 'data': 'FormName', "className": "text-center " },
                { 'data': 'Description', "className": "text-center " },
                { 'data': 'UpdateDate', "className": "text-center " },
                {
                    'data': 'ID',
                    'orderable': false,
                    'width': '20px',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<button  data-action="form-edit" data-id="' + data.ID + '" class="btn bg-black-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<span>';
                        html += '<button  data-action="form-detail" data-id="' + data.ID + '" class="btn bg-blue-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-solid fa-list-check"></i>';
                        html += '</button>';
                        html += '</span>';
                        html += '<span>';
                        html += '<button data-action="form-delete" data-id ="' + data.ID + '"class="btn bg-red-lt btn-icon btn-rounded">'
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>';
                        html += '</span>';
                        return html;
                    }
                },
            ]
        });
    },
    ActionFormType: function (type, model) {
        var data = {
            TypeData: type,
            ProjectTypeId: model.projectTypeId,
            FormTypeID: model.formTypeId,
            FormTypeName: model.formTypeName,
            FormTypeDesc: model.formTypeDesc
        };
        $.ajax({
            url: baseUrl + 'MasterForm/ActionFormType',
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
    DeleteFormType: function (model) {
        $.ajax({
            url: baseUrl + 'MasterForm/DeleteFormType',
            type: 'post',
            dataType: 'json',
            data: model,
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
    }
}
