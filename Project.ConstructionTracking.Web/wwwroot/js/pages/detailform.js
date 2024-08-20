let tblForm;
let tblGroup;
let tblPackage;
let tblCheck;

let getFormID;
let getGroupID;
let getPackageID;

const detail = {
    init: (id) => {

        detail.AjaxGrid(id);

        $('#multiple-select-field').select2({
            dropdownParent: $("#partial-modal-form")
        });

        var coll = document.getElementsByClassName("collapsible");
        for (var i = 0; i < coll.length; i++) {
            coll[i].addEventListener("click", function () {
                this.classList.toggle("active");
                var content = this.nextElementSibling;
                if (content.style.display === "block") {
                    content.style.display = "none";
                } else {
                    content.style.display = "block";
                }
            });
        }
            
        const buttons = document.querySelectorAll('#section-button .btn');

        buttons.forEach(button => {
            button.addEventListener('click', function () {
                // Get the target section from the button's data-bs-target attribute
                const targetSelector = button.getAttribute('data-bs-target');
                const targetElement = document.querySelector(targetSelector);

                if (targetElement) {
                    // Scroll to the target element with smooth behavior
                    targetElement.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });

        $("#modal-form").click(() => {
            $('#form-head').text('Create Master Form');
            $('#form-name').val('');    // Clear the form name input
            $('#form-description').val('');  // Clear the description input
            $('#form-progress').val(''); // Clear the progress input
            $('#form-duration').val(''); // Clear the duration input

            // Clear the QC select element
            var qcSelect = $('#multiple-select-field');
            qcSelect.val(null).trigger('change'); // Clear selected options
            $("#partial-modal-form").modal('show');
            return false;
        });

        $("#btn-save").click(() => {
            let typeData = 3;
            var data = null;
            var formID = $('#formID').val();
            var groupID = $('#groupID').val();
            var packageID = $('#packageID').val();
            var checklistID = $('#checklistID').val();

            if ( packageID && checklistID) {
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    CheckListID: $("#checklistID").val(),
                    PackageID: $("#packageID").val(),
                    PackageName: $("#packageName").val()
                }
                console.log('check')
                detail.DeleteCheckList(data);
            } else if ( groupID && packageID) {
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    PackageID: $("#packageID").val(),
                    GroupID: $("#groupID").val(),
                    GroupName: $("#groupName").val(),
                }
                console.log('pack')
                detail.DeletePackage(data);
            } else if (formID && groupID) {
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    GroupId: $("#groupID").val(),
                    FormID: $("#formID").val(),
                    FormName: $("#formName").val()
                }
                console.log('group')
                detail.DeleteGroup(data);
            } else if (formID) {
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    FormID: $("#formID").val()
                }
                console.log('form')
                detail.DeleteForm(data);
            } 
        })

        $('#form-save').click(() => {
            let typeData;
            var data = null;
            if ($('#form-id').val() === null || $('#form-id').val() === "") {
                typeData = 1;
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    FormName: $("#form-name").val(),
                    FormDesc: $("#form-description").val(),
                    Progress: $("#form-progress").val(),
                    Duration: $("#form-duration").val(),
                    QcList: $("#multiple-select-field").val()
                }
                detail.ActionForm(data);
            }
            else {
                typeData = 2;
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    FormID: $('#form-id').val(),
                    FormName: $("#form-name").val(),
                    FormDesc: $("#form-description").val(),
                    Progress: $("#form-progress").val(),
                    Duration: $("#form-duration").val(),
                    QcList: $("#multiple-select-field").val()
                }
                detail.ActionForm(data);
            }
        })

        $('#group-save').click(() => {
            let typeData;
            var data = null;
            if ($('#group-id').val() === null || $('#group-id').val() === "") {
                typeData = 1;
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    FormName: $('#g-form-name').val(),
                    FormID: $('#g-form-id').val(),
                    GroupName: $('#group-name').val(),
                    GroupID: $('#group-id').val()
                }
                detail.ActionGroup(data);
            }
            else {
                typeData = 2;
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    FormName: $('#g-form-name').val(),
                    FormID: $('#g-form-id').val(),
                    GroupName: $('#group-name').val(),
                    GroupID: $('#group-id').val()
                }
                detail.ActionGroup(data);
            }
        })

        $('#package-save').click(() => {
            let typeData;
            var data = null;
            if ($('#package-id').val() === null || $('#package-id').val() === "") {
                typeData = 1;
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    PackageName: $('#package-name').val(),
                    PackageID: $('#package-id').val(),
                    GroupName: $('#p-form-name').val(),
                    GroupID: $('#p-form-id').val()
                }
                detail.ActionPackage(data);
            }
            else {
                typeData = 2;
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    PackageName: $('#package-name').val(),
                    PackageID: $('#package-id').val(),
                    GroupName: $('#p-form-name').val(),
                    GroupID: $('#p-form-id').val()
                }
                detail.ActionPackage(data);
            }
        })

        $('#check-save').click(() => {
            let typeData;
            var data = null;
            if ($('#check-id').val() === null || $('#check-id').val() === "") {
                typeData = 1;
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    PackageName: $('#c-form-name').val(),
                    PackageID: $('#c-form-id').val(),
                    CheckListName: $('#check-name').val(),
                    CheckListID: $('#check-id').val()
                }
                detail.ActionCheckList(data);
            }
            else {
                typeData = 2;
                data = {
                    TypeData: typeData,
                    FormTypeID: id,
                    PackageName: $('#c-form-name').val(),
                    PackageID: $('#c-form-id').val(),
                    CheckListName: $('#check-name').val(),
                    CheckListID: $('#check-id').val()
                }
                detail.ActionCheckList(data);
            }
        })
    },
    AjaxGrid: function (id) {
        tblForm = $('#tbl-list-f').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'MasterForm/AjaxGridFormList',
                type: "POST",
                data: function (json) {
                    json.formTypeID = id;
                    json = $.param(json);

                    return json;
                },
                "dataSrc": function (res) {
                    //app.ajaxVerifySession(res);                    
                    return res.data;
                },
                complete: function (res) {
                    const checkboxesF = document.querySelectorAll('.checkbox-f');
                    FCheckboxChange(checkboxesF, 'Lv.2 - FormGroup');

                    $(document).on('click', "button[data-action='form-edit']", function (e) {
                        var formId = $(e.currentTarget).attr('data-id');

                        var data = {
                            FormId: formId,
                        };

                        $.ajax({
                            url: baseUrl + 'masterform/getformdetail',
                            type: 'post',
                            dataType: 'json',
                            data: data,
                            success: function (resp) {
                                if (resp.success) {
                                    
                                    $('#form-head').text('Edit Master Form');
                                    $('#form-id').val(resp.data.ID);
                                    $('#form-name').val(resp.data.Name);
                                    $('#form-description').val(resp.data.Description);
                                    $('#form-progress').val(resp.data.Progress);
                                    $('#form-duration').val(resp.data.DurationDay);

                                    var qcSelect = $('#multiple-select-field');
                                    qcSelect.val(resp.data.QcLists.map(qc => qc.ID)).trigger('change');

                                    // Show the modal
                                    $("#partial-modal-form").modal('show');
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
                        var formId = $(e.currentTarget).attr('data-id');

                        $('#formID').val('');
                        $('#groupID').val('');
                        $('#packageID').val('');
                        $('#checklistID').val('');

                        $('#formID').val(formId);

                        $('#partial-confirm-delete').modal('show');
                    });
                }
            },
            "ordering": true,
            "order": [[0, "ASC"]],
            "columns": [
                {
                    'data': 'ID',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<input type="checkbox" class="checkbox-f" data-id="' + data.ID + '" />';
                        return html;
                    }
                },
                { 'data': 'Name', "className": "text-center " },
                { 'data': 'Progress', "className": "text-center " },
                { 'data': 'DurationDay', "className": "text-center " },
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
    FormGroup: function (id, value) {
        tblGroup = $('#tbl-list-fg').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'MasterForm/AjaxGridFormGroupList',
                type: "POST",
                data: function (json) {
                    json.formID = id;
                    json = $.param(json);
                    return json;
                },
                "dataSrc": function (res) {
                    return res.data;
                },
                complete: function (res) {
                    // Unbind the click event before binding a new one
                    $(document).off('click', "button[data-action='form-g-edit']");

                    // Bind the click event
                    $(document).on('click', "button[data-action='form-g-edit']", function (e) {
                        // Clear previous data before assigning new values
                        $('#g-form-head').text('');
                        $('#g-form-name').val('');
                        $('#g-form-id').val('');
                        $('#group-name').val('');
                        $('#group-id').val('');

                        // Retrieve data
                        var groupId = $(e.currentTarget).attr('data-id');
                        console.log(id ,value)
                        var data = {
                            GroupId: groupId,
                        };

                        // Perform AJAX request to get group details
                        $.ajax({
                            url: baseUrl + 'masterform/getgroupdetail',
                            type: 'post',
                            dataType: 'json',
                            data: data,
                            success: function (resp) {
                                if (resp.success) {
                                    // Populate the modal with retrieved data
                                    $('#g-form-head').text('Edit Master Form Group');
                                    $('#g-form-name').val(value);
                                    $('#g-form-id').val(id);
                                    $('#group-name').val(resp.data.Name);
                                    $('#group-id').val(resp.data.ID);

                                    // Show the modal
                                    $("#partial-modal-group").modal('show');
                                }
                            },
                            error: function (xhr, status, error) {
                                alert(" Coding Error ");
                            },
                        });
                        return false;
                    });

                    // Other bindings like form-delete can also be handled similarly
                    $(document).on('click', "button[data-action='form-g-delete']", function (e) {
                        var groupId = $(e.currentTarget).attr('data-id');

                        //clear data in modal delete
                        $('#formID').val('');
                        $('#groupID').val('');
                        $('#packageID').val('');
                        $('#checklistID').val('');

                        $('#groupID').val(groupId);
                        $('#formID').val(id);
                        $('#formName').val(value);
                        $('#partial-confirm-delete').modal('show');
                    });

                    // Bind checkbox changes
                    const checkboxesG = document.querySelectorAll('.checkbox-g');
                    FGCheckboxChange(checkboxesG, 'Lv.3 - FormPackage');
                }
            },
            "ordering": true,
            "order": [[0, "ASC"]],
            "columns": [
                {
                    'data': 'ID',
                    'width': '15%',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        return '<input type="checkbox" class="checkbox-g" data-id="' + data.ID + '" />';
                    }
                },
                {
                    'data': 'Name',
                    'width': 'auto',
                    "className": "text-center "
                },
                {
                    'data': 'ID',
                    'orderable': false,
                    'width': '20%',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<button data-action="form-g-edit" data-id="' + data.ID + '" class="btn bg-black-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<span>';
                        html += '<button data-action="form-g-delete" data-id ="' + data.ID + '" class="btn bg-red-lt btn-icon btn-rounded">'
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>';
                        html += '</span>';
                        return html;
                    }
                }
            ]
        });
    },
    FormPackage: function (id, value) {
        tblPackage = $('#tbl-list-fp').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'MasterForm/AjaxGridFormPackageList',
                type: "POST",
                data: function (json) {
                    json.groupID = id;
                    json = $.param(json);

                    return json;
                },
                "dataSrc": function (res) {
                    //app.ajaxVerifySession(res);                    
                    return res.data;
                },
                complete: function (res) {
                    // Unbind the click event before binding a new one
                    $(document).off('click', "button[data-action='form-p-edit']");

                    $(document).on('click', "button[data-action='form-p-edit']", function (e) {
                        //e.preventDefault(); // Prevent default action

                        var packageId = $(e.currentTarget).attr('data-id');

                        var data = {
                            packageId: packageId,
                        };

                        $.ajax({
                            url: baseUrl + 'masterform/GetPackageDetail',
                            type: 'post',
                            dataType: 'json',
                            data: data,
                            success: function (resp) {
                                if (resp.success) {
                                    // Populate the modal with retrieved data
                                    $('#p-form-head').text('Edit Master Form Group');
                                    $('#p-form-name').val(value);
                                    $('#p-form-id').val(id);
                                    $('#package-name').val(resp.data.Name);
                                    $('#package-id').val(resp.data.ID);

                                    // Show the modal
                                    $("#partial-modal-package").modal('show');
                                }
                            },
                            error: function (xhr, status, error) {
                                // do something
                                alert(" Coding Error ")
                            },
                        });
                        return false;
                    });
                    $(document).on('click', "button[data-action='form-p-delete']", function (e) {
                        //e.preventDefault(); // Prevent default action

                        var packageId = $(e.currentTarget).attr('data-id');
                        $('#formID').val('');
                        $('#formName').val('')
                        $('#groupID').val('');
                        $('#packageID').val('');
                        $('#checklistID').val('');

                        $('#groupID').val(id);
                        $('#groupName').val(value);
                        $('#packageID').val(packageId);
                
                        $('#partial-confirm-delete').modal('show');
                    });

                    const checkboxesP = document.querySelectorAll('.checkbox-p');
                    FPCheckboxChange(checkboxesP, 'Lv.4 - FormCheckList');
                }
            },
            "ordering": true,
            "order": [[0, "ASC"]],
            "columns": [
                {
                    'data': 'ID',
                    'width': '15%',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<input type="checkbox" class="checkbox-p" data-id="' + data.ID + '" />';
                        return html;
                    }
                },
                { 'data': 'Name', "className": "text-center " },
                {
                    'data': 'ID',
                    'orderable': false,
                    'width': '20px',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<button  data-action="form-p-edit" data-id="' + data.ID + '" class="btn bg-black-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<span>';
                        html += '<button data-action="form-p-delete" data-id ="' + data.ID + '"class="btn bg-red-lt btn-icon btn-rounded">'
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>';
                        html += '</span>';
                        return html;
                    }
                },
            ]
        });
    },
    FormCheck: function (id, value) {
        tblCheck = $('#tbl-list-fcl').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'MasterForm/AjaxGridFormCheckList',
                type: "POST",
                data: function (json) {
                    json.packageID = id;
                    json = $.param(json);

                    return json;
                },
                "dataSrc": function (res) {
                    //app.ajaxVerifySession(res);                    
                    return res.data;
                },
                complete: function (res) {
                    // Unbind the click event before binding a new one
                    $(document).off('click', "button[data-action='form-c-edit']");

                    $(document).on('click', "button[data-action='form-c-edit']", function (e) {
                        //e.preventDefault(); // Prevent default action

                        var checkId = $(e.currentTarget).attr('data-id');

                        var data = {
                            checkId: checkId,
                        };

                        $.ajax({
                            url: baseUrl + 'masterform/GetCheckDetail',
                            type: 'post',
                            dataType: 'json',
                            data: data,
                            success: function (resp) {
                                if (resp.success) {
                                    // Populate the modal with retrieved data
                                    $('#c-form-head').text('Edit Master Form Group');
                                    $('#c-form-name').val(value);
                                    $('#c-form-id').val(id);
                                    $('#check-name').val(resp.data.Name);
                                    $('#check-id').val(resp.data.ID);

                                    // Show the modal
                                    $("#partial-modal-checklist").modal('show');
                                }
                            },
                            error: function (xhr, status, error) {
                                // do something
                                alert(" Coding Error ")
                            },
                        });
                        return false;
                    });
                    $(document).on('click', "button[data-action='form-c-delete']", function (e) {
                        //e.preventDefault(); // Prevent default action

                        var checkId = $(e.currentTarget).attr('data-id');

                        $('#formID').val('');
                        $('#formName').val('')
                        $('#groupID').val('');
                        $('#groupName').val('')
                        $('#packageID').val('');
                        $('#checklistID').val('');

                        $('#packageID').val(id);
                        $('#packageName').val(value);
                        $('#checklistID').val(checkId);
                        $('#partial-confirm-delete').modal('show');
                    });

                }
            },
            "ordering": true,
            "order": [[0, "ASC"]],
            "columns": [
                { 'data': 'Name', "className": "text-center " },
                {
                    'data': 'ID',
                    'orderable': false,
                    'width': '20px',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<button  data-action="form-c-edit" data-id="' + data.ID + '" class="btn bg-black-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<span>';
                        html += '<button data-action="form-c-delete" data-id ="' + data.ID + '"class="btn bg-red-lt btn-icon btn-rounded">'
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>';
                        html += '</span>';
                        return html;
                    }
                },
            ]
        });
    },
    ActionForm: function (model) {
        $.ajax({
            url: baseUrl + 'MasterForm/ActionForm',
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
    },
    DeleteForm: function (model) {
        $.ajax({
            url: baseUrl + 'MasterForm/DeleteForm',
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
    },
    ActionGroup: function (model) {
        $.ajax({
            url: baseUrl + 'MasterForm/ActionGroup',
            type: 'post',
            dataType: 'json',
            data: model,
            success: function (resp) {
                if (resp.success) {
                    if ($.fn.DataTable.isDataTable('#tbl-list-fg')) {
                        $('#tbl-list-fg').DataTable().clear().destroy();
                    }
                    
                    detail.FormGroup(model.FormID, model.FormName);
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
    DeleteGroup: function (model) {
        $.ajax({
            url: baseUrl + 'MasterForm/DeleteGroup',
            type: 'post',
            dataType: 'json',
            data: model,
            success: function (resp) {
                if (resp.success) {
                    if ($.fn.DataTable.isDataTable('#tbl-list-fg')) {
                        $('#tbl-list-fg').DataTable().clear().destroy();
                    }
                    detail.FormGroup(model.FormID, model.FormName);
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
    ActionPackage: function (model) {
        $.ajax({
            url: baseUrl + 'MasterForm/ActionPackage',
            type: 'post',
            dataType: 'json',
            data: model,
            success: function (resp) {
                if (resp.success) {
                    if ($.fn.DataTable.isDataTable('#tbl-list-fp')) {
                        $('#tbl-list-fp').DataTable().clear().destroy();
                    }
                    detail.FormPackage(model.GroupID, model.GroupName);
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
    DeletePackage: function (model) {
        $.ajax({
            url: baseUrl + 'MasterForm/DeletePackage',
            type: 'post',
            dataType: 'json',
            data: model,
            success: function (resp) {
                if (resp.success) {
                    if ($.fn.DataTable.isDataTable('#tbl-list-fp')) {
                        $('#tbl-list-fp').DataTable().clear().destroy();
                    }
                    detail.FormPackage(model.GroupID, model.GroupName);
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
    ActionCheckList: function ( model) {
        $.ajax({
            url: baseUrl + 'MasterForm/ActionCheckList',
            type: 'post',
            dataType: 'json',
            data: model,
            success: function (resp) {
                if (resp.success) {
                    if ($.fn.DataTable.isDataTable('#tbl-list-fcl')) {
                        $('#tbl-list-fcl').DataTable().clear().destroy();
                    }
                    detail.FormCheck(model.PackageID, model.PackageName);
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
    DeleteCheckList: function (model) {
        $.ajax({
            url: baseUrl + 'MasterForm/DeleteCheckList',
            type: 'post',
            dataType: 'json',
            data: model,
            success: function (resp) {
                if (resp.success) {
                    if ($.fn.DataTable.isDataTable('#tbl-list-fcl')) {
                        $('#tbl-list-fcl').DataTable().clear().destroy();
                    }
                    detail.FormCheck(model.PackageID, model.PackageName);
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

function FCheckboxChange(checkboxes, prefixText) {
    let anyChecked = false; // Flag to track if any checkbox is checked
    var formName;
    var value;
    var idValue;

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            // Remove highlight from all rows in this group
            value = null;
            idValue = null;
            const rows = Array.from(checkboxes).map(cb => cb.closest('tr')).filter(row => row);
            rows.forEach(row => row.classList.remove('highlight'));

            // Destroy DataTables if they exist
            ['#tbl-list-fg', '#tbl-list-fp', '#tbl-list-fcl'].forEach(selector => {
                if ($.fn.DataTable.isDataTable(selector)) {
                    $(selector).DataTable().clear().destroy();
                }
            });

            // Hide all content sections initially
            const sections = {
                section: document.getElementById('section-2'),
                sectionFP: document.getElementById('section-3'),
                sectionFCL: document.getElementById('section-4')
            };

            Object.values(sections).forEach(section => {
                if (section) {
                    const headerDiv = section.querySelector('.collapsible');
                    if (headerDiv) {
                        if (section.id == 'section-2') {
                            headerDiv.firstChild.textContent = `${prefixText}`;
                        } else if (section.id == 'section-3') {
                            headerDiv.firstChild.textContent = `Lv.3 - FormPackage`;
                        } else {
                            headerDiv.firstChild.textContent = `Lv.4 - FormCheckList`;
                        }
                    }
                    const contentDiv = section.querySelector('.content-cl');
                    if (contentDiv) {
                        contentDiv.style.display = 'none';
                    }
                }
            });

            if (checkbox.checked) {
                anyChecked = true; // Set flag to true when a checkbox is checked

                const row = checkbox.closest('tr');
                if (row) {
                    row.classList.add('highlight');

                    // Retrieve value from the row
                    value = row.cells[1].textContent.trim(); // Adjust cell index as needed
                    idValue = checkbox.getAttribute('data-id');
                    getFormID = idValue;
                    formName = value;

                    // Update collapsible section
                    if (sections.section) {
                        const header = sections.section.querySelector('.collapsible');
                        if (header) {
                            header.firstChild.textContent = `${prefixText} : ${value}`;
                        }
                        const contentDiv = sections.section.querySelector('.content-cl');
                        if (contentDiv) {
                            contentDiv.style.display = 'block';
                        }
                    }

                    // Handle FP and FCL sections
                    if (sections.sectionFP) {
                        const headerFP = sections.sectionFP.querySelector('.collapsible');
                        var text = "Lv.3 - FormPackage";
                        if (headerFP) {
                            headerFP.firstChild.textContent = `${text}`;
                        }
                    }

                    if (sections.sectionFCL) {
                        const headerFCL = sections.sectionFCL.querySelector('.collapsible');
                        var text = "Lv.4 - FormCheckList";
                        if (headerFCL) {
                            headerFCL.firstChild.textContent = `${text}`;
                        }
                    }

                    // Uncheck all other checkboxes
                    checkboxes.forEach(cb => {
                        if (cb !== checkbox) {
                            cb.checked = false;
                        }
                    });

                    // Now call the detail.FormGroup with the current idValue
                    detail.FormGroup(idValue, value);

                    // Setup the modal click event
                    $("#modal-group").off('click').on('click', () => {
                        // Clear modal inputs
                        $('#g-form-head').text('Create Master Form Group');
                        $('#g-form-name').val(formName);
                        $('#g-form-id').val(getFormID);
                        $('#group-name').val('');
                        $('#group-id').val('');
                        $("#partial-modal-group").modal('show');
                        return false;
                    });
                }
            } else {
                anyChecked = false; // Set flag to false when the checkbox is unchecked

                // Unbind the modal click event if no checkboxes are checked
                if (!anyChecked) {
                    $("#modal-group").off('click');
                }
            }
        });
    });

    // Initial setup for modal click event
    if (anyChecked) {
        $("#modal-group").on('click', () => {
            $("#partial-modal-group").modal('show');
            return false;
        });
    } else {
        $("#modal-group").off('click');
    }
}
function FGCheckboxChange(checkboxes, prefixText) {
    let anyChecked2 = false; // Flag to track if any checkbox is checked
    var groupName;
    var valueFG;
    var idValueFG;
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            // Remove highlight from all rows in this group
            valueFG = null;
            idValueFG = null;

            const rows = Array.from(checkboxes).map(cb => cb.closest('tr')).filter(row => row);
            rows.forEach(row => row.classList.remove('highlight'));

            // Destroy DataTables if they exist
            [ '#tbl-list-fp', '#tbl-list-fcl'].forEach(selector => {
                if ($.fn.DataTable.isDataTable(selector)) {
                    $(selector).DataTable().clear().destroy();
                }
            });

            // Handle sections
            const sections = {
                sectionFP: document.getElementById('section-3'),
                sectionFCL: document.getElementById('section-4')
            };

            // Hide all content sections initially
            Object.values(sections).forEach(section => {
                if (section) {
                    const headerDiv = section.querySelector('.collapsible');
                    if (headerDiv) {
                        if (section.id == 'section-3') {
                            headerDiv.firstChild.textContent = `${prefixText}`;
                        }
                        else {
                            headerDiv.firstChild.textContent = `Lv.4 - FormCheckList`;
                        }
                    }
                    const contentDiv = section.querySelector('.content-cl');
                    if (contentDiv) {
                        contentDiv.style.display = 'none';
                    }
                }
            });

            // Highlight the row of the checked checkbox and update section
            if (checkbox.checked) {
                anyChecked2 = true; // Set flag to true when a checkbox is checked
                const row = checkbox.closest('tr');
                if (row) {
                    row.classList.add('highlight');

                    // Retrieve value from the row (e.g., second cell's text content)
                    const value = row.cells[1].textContent.trim(); // Adjust cell index as needed
                    const idValue = checkbox.getAttribute('data-id');
                    getGroupID = idValue;
                    groupName = value;

                    // Handle FP section
                    if (sections.sectionFP) {
                        const headerFP = sections.sectionFP.querySelector('.collapsible');
                        if (headerFP) {
                            headerFP.firstChild.textContent = `${prefixText} : ${value}`;
                        }
                        const contentDivFP = sections.sectionFP.querySelector('.content-cl');
                        if (contentDivFP) {
                            contentDivFP.style.display = 'block'; // Hide if no checkbox is checked
                        }
                    }

                    // Handle FCL section
                    if (sections.sectionFCL) {
                        const headerFCL = sections.sectionFCL.querySelector('.collapsible');
                        var text = "Lv.4 - FormCheckList"
                        if (headerFCL) {
                            headerFCL.firstChild.textContent = `${text}`;
                        }
                        const contentDivFCL = sections.sectionFCL.querySelector('.content-cl');
                        if (contentDivFCL) {
                            contentDivFCL.style.display = 'none'; // Hide if no checkbox is checked
                        }
                    }

                    // Uncheck all checkboxes except the current one
                    checkboxes.forEach(cb => {
                        if (cb !== checkbox) {
                            cb.checked = false;
                        }
                    });

                    // Update detail form
                    detail.FormPackage(idValue, value);

                    // Setup the modal click event
                    $("#modal-package").off('click').on('click', () => {
                        // Clear modal inputs
                        $('#p-form-head').text('Create Master Form Package');
                        $('#p-form-name').val(groupName);
                        $('#p-form-id').val(getGroupID);
                        $('#package-name').val('');
                        $('#package-id').val('');
                        $("#partial-modal-package").modal('show');
                        return false;
                    });
                } 
            } else {
                anyChecked2 = false; // Set flag to false when the checkbox is unchecked

                // Unbind the modal click event if no checkboxes are checked
                if (!anyChecked2) {
                    $("#modal-package").off('click');
                }
            }
        });
    });

    // Initial setup for modal click event
    if (anyChecked2) {
        $("#modal-package").on('click', () => {
            $("#partial-modal-package").modal('show');
            return false;
        });
    } else {
        $("#modal-package").off('click');
    }
}
function FPCheckboxChange(checkboxes, prefixText) {
    let anyChecked3 = false; // Flag to track if any checkbox is checked
    var packageName;
    var valueFP;
    var idValueFP;
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            // Remove highlight from all rows in this group
            valueFP = null;
            idValueFP = null;

            const rows = Array.from(checkboxes).map(cb => cb.closest('tr')).filter(row => row);
            rows.forEach(row => row.classList.remove('highlight'));

            // Destroy DataTables if they exist
            [ '#tbl-list-fcl'].forEach(selector => {
                if ($.fn.DataTable.isDataTable(selector)) {
                    $(selector).DataTable().clear().destroy();
                }
            });

            // Handle sections
            const sections = {
                sectionFCL: document.getElementById('section-4')
            };

            // Hide all content sections initially
            Object.values(sections).forEach(section => {
                if (section) {
                    const headerDiv = section.querySelector('.collapsible');
                    if (headerDiv) {
                        headerDiv.firstChild.textContent = `${prefixText}`;
                    }
                    const contentDiv = section.querySelector('.content-cl');
                    if (contentDiv) {
                        contentDiv.style.display = 'none';
                    }
                }
            });

            // Highlight the row of the checked checkbox and update section
            if (checkbox.checked) {
                anyChecked3 = true; // Set flag to true when a checkbox is checked
                const row = checkbox.closest('tr');
                if (row) {
                    row.classList.add('highlight');

                    // Retrieve value from the row (e.g., second cell's text content)
                    const value = row.cells[1].textContent.trim(); // Adjust cell index as needed
                    const idValue = checkbox.getAttribute('data-id');
                    getPackageID = idValue;
                    packageName = value;

                    // Handle FCL section
                    if (sections.sectionFCL) {
                        const headerFCL = sections.sectionFCL.querySelector('.collapsible');
                        if (headerFCL) {
                            headerFCL.firstChild.textContent = `${prefixText} : ${value}`;
                        }
                        const contentDivFCL = sections.sectionFCL.querySelector('.content-cl');
                        if (contentDivFCL) {
                            contentDivFCL.style.display = 'block'; // Hide if no checkbox is checked
                        }
                    }

                    // Uncheck all checkboxes except the current one
                    checkboxes.forEach(cb => {
                        if (cb !== checkbox) {
                            cb.checked = false;
                        }
                    });

                    // Update detail form
                    detail.FormCheck(idValue, value);

                    // Setup the modal click event
                    $("#modal-checklist").off('click').on('click', () => {
                        // Clear modal inputs
                        $('#c-form-head').text('Create Master Form CheckList');
                        $('#c-form-name').val(packageName);
                        $('#c-form-id').val(getPackageID);
                        $('#check-name').val('');
                        $('#check-id').val('');
                        $("#partial-modal-checklist").modal('show');
                        return false;
                    });
                }
            } else {
                anyChecked3 = false; // Set flag to false when the checkbox is unchecked

                // Unbind the modal click event if no checkboxes are checked
                if (!anyChecked3) {
                    $("#modal-checklist").off('click');
                }
            }
        });
    });

    // Initial setup for modal click event
    if (anyChecked3) {
        $("#modal-checklist").on('click', () => {
            $("#partial-modal-checklist").modal('show');
            return false;
        });
    } else {
        $("#modal-checklist").off('click');
    }
}