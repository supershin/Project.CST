const unit = {
    init: () => {

        unit.UnitList("");

        $('#dropdown-select-project').change(() => {
            if ($.fn.DataTable.isDataTable('#tbl-unit-list')) {
                $('#tbl-unit-list').DataTable().clear().destroy();
            }

            var selectProjectID = $('#dropdown-select-project').val();
            unit.UnitList(selectProjectID);
        })
        $("#unit-create").click(() => {
            $("#modal-unit-c").modal('show');
            return false;
        });

        $("#unit-edit").click(() => {
            $("#modal-unit-e").modal('show');
            return false;
        });
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
            "order": [[11, "desc"]], // Ordering by the "UpdateDate" column (index 11)
            "columns": [
                { 'data': 'ProjectCode', "className": "text-center" },
                { 'data': 'ProjectName', "className": "text-center" },
                { 'data': 'UnitCode', "className": "text-center" },
                { 'data': 'UnitTypeName', "className": "text-center" },
                { 'data': 'UnitAddress', "className": "text-center" },
                { 'data': 'UnitArea', "className": "text-center" },
                { 'data': 'UnitStatusDesc', "className": "text-center" },
                { 'data': 'UnitVendor', "className": "text-center" },
                { 'data': 'UnitPO', "className": "text-center" },
                { 'data': 'UnitStartDate', "className": "text-center" },
                { 'data': 'UnitEndDate', "className": "text-center" },
                { 'data': 'UpdateDate', "className": "text-center" },
                {
                    'data': 'UnitId',
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
    }
}