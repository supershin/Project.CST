const project = {
    init: () => {
        project.AjaxGrid();

        $("#project-create").click(() => {
            $("#modal-c").modal('show');
            return false;
        });
    },
    AjaxGrid: function () {
        tblUnit = $('#tbl-table').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'MasterProject/JsonAjaxGridProjectList',
                type: "POST",
                data: function (json) {
                  //￼ var datastring = $("#form-search").serialize();
                  //  json = datastring + '&' + $.param(json);

                  //  return json;
                },
                "dataSrc": function (res) {
                    //app.ajaxVerifySession(res);                    
                    return res.data;
                },
                complete: function (res) {
                    $(document).on('click', "button[data-action='edit-project']", function (e) {
                        $("#modal-e").modal('show');
                        return false;
                    });
                }
            },
            "ordering": true,
            "order": [[2, "desc"]],
            "columns": [
                //{
                //    'data': 'ID',
                //    'orderable': false,
                //    'width': '20px',
                //    "className": "text-center",
                //    'mRender': function (ID, type, data, obj) {
                //        var html = '<button  data-action="list-ap-detail" data-id="' + data.ID + '" class="btn bg-red-lt btn-icon btn-rounded">';
                //        html += '<i class="fa-regular fa-file"></i>';
                //        html += '</button>';

                //        return html;
                //    }
                //},
                //{
                //    'data': 'ID',
                //    'orderable': false,
                //    'width': '20px',
                //    "className": "text-center",
                //    'mRender': function (ID, type, data, obj) {
                //        if (data.Status == 2) {

                //            var html = '<button data-action="print-report" data-id ="' + data.ID + '"class="btn bg-red-lt btn-icon btn-rounded">'
                //            html += '<i class="fa-solid fa-print"></i>';
                //            html += '</button>';


                //            return html;
                //        }
                //        else {
                //            return "";
                //        }
                //    }
                //},
                { 'data': 'ProjectCode', "className": "text-center " },
                { 'data': 'ProjectName', "className": "text-center " },
                { 'data': 'UpdateDate', "className": "text-center " },
                {
                    'data': 'ProjectCode',
                    "className": "text-center",
                    "render": function (data, type, row, meta) {
                        var html = '<button  data-action="edit-project" data-id="' + data.ProjectCode + '" class="btn bg-skyblue-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<button  data-action="delete-project" data-id="' + data.ProjectCode + '" class="btn bg-red-lt btn-icon btn-rounded">';
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>'
                        return html;
                    }
                }
            ]
        });
    },
}