let tblUnit;
const unit = {
    init: () => {
        
        $("#form-search").submit(function (e) {
            $("#divLoadingProgress").show();
            e.preventDefault();
            app.reloadTable(tblUnit);
            $("#divLoadingProgress").hide();
            return false;
        });

        unit.AjaxGrid();
     
    },
    AjaxGrid: function () {
      
        tblUnit = $('#tbl-list').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'Unit/GetUnitList',
                type: "POST",
                data: function (json) {
                    //app.loading(true, "#tbl-table");
                    //Make your callback here.                                        
                    var datastring = $("#form-search").serialize();
                    //json.ProjectID = project_id;
                    json = datastring + '&' + $.param(json);
                  
                    //return json;
                    return json;
                },
                "dataSrc": function (res) {
                    //app.ajaxVerifySession(res);  
                    return res.data;
                },
                complete: function (res) {           
                    //$("button[data-action='delete-unit']").unbind('click').click((e) => {
                    //    let id = $(e.currentTarget).attr('data-id');
                    //    unit.modalDelete(id);
                    //    return false;
                    //});
                    //$("button[data-action='create-quotation']").unbind('click').click((e) => {
                    //    let id = $(e.currentTarget).attr('data-unit-id');
                    //    unit.modalConfirmCreateQuotation(id);
                    //    return false;
                    //});
                }
            },
            "ordering": true,
            "order": [[0, "desc"]],
            "columns": [
                { 'data': 'UnitCode', "className": "text-center" },
                { 'data': 'UnitTypeName', "className": "text-center" },
                { 'data': 'StartDate', "className": "text-center" },
                { 'data': 'EndDate', "className": "text-center" },
                {
                    'data': 'FlagActive',
                    "className": "text-center",
                    "render": function (data) {
                        if (data) {
                            return '<span class="badge bg-success me-1" style="width: 16px; height: 16px;"></span> เสร็จสิ้น';
                        } else {
                            return '<span class="badge bg-danger me-1" style="width: 16px; height: 16px;"></span> ไม่เสร็จสิ้น';
                        }
                    }
                },
                {
                    'data': 'UnitID',
                    "render": function (data) {              
                        let html = '';
                        html += '<a href="Tracking/Index/' + data + '" class="btn fixed-size-btn" style="background-color: orange; color: white;">';
                        html += '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-building-up" viewBox="0 0 16 16">';
                        html += '<path d="M12.5 16a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7m.354-5.854 1.5 1.5a.5.5 0 0 1-.708.708L13 11.707V14.5a.5.5 0 0 1-1 0v-2.793l-.646.647a.5.5 0 0 1-.708-.708l1.5-1.5a.5.5 0 0 1 .708 0" />';
                        html += '<path d="M2 1a1 1 0 0 1 1-1h10a1 1 0 0 1 1 1v6.5a.5.5 0 0 1-1 0V1H3v14h3v-2.5a.5.5 0 0 1 .5-.5H8v4H3a1 1 0 0 1-1-1z" />';
                        html += '<path d="M4.5 2a.5.5 0 0 0-.5.5v1a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-1a.5.5 0 0 0-.5-.5zm3 0a.5.5 0 0 0-.5.5v1a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-1a.5.5 0 0 0-.5-.5zm3 0a.5.5 0 0 0-.5.5v1a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-1a.5.5 0 0 0-.5-.5zm-6 3a.5.5 0 0 0-.5.5v1a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-1a.5.5 0 0 0-.5-.5zm3 0a.5.5 0 0 0-.5.5v1a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-1a.5.5 0 0 0-.5-.5zm3 0a.5.5 0 0 0-.5.5v1a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-1a.5.5 0 0 0-.5-.5zm-6 3a.5.5 0 0 0-.5.5v1a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-1a.5.5 0 0 0-.5-.5zm3 0a.5.5 0 0 0-.5.5v1a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-1a.5.5 0 0 0-.5-.5z" /></svg></a>';
                        return html;
                    }
                },
                {
                    'data': 'UnitID',
                    "render": function (data) {
                        let html = '';
                        html += '<a href="Unitdetail/index/' + data + '" class="btn fixed-size-btn" style="background-color: blue; color: white;">';
                        html += '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-pencil-square" viewBox="0 0 16 16">';
                        html += '<path d="M15.502 1.94a.5.5 0 0 1 0 .706L14.459 3.69l-2-2L13.502.646a.5.5 0 0 1 .707 0l1.293 1.293zm-1.75 2.456-2-2L4.939 9.21a.5.5 0 0 0-.121.196l-.805 2.414a.25.25 0 0 0 .316.316l2.414-.805a.5.5 0 0 0 .196-.12l6.813-6.814z" />';
                        html += '<path fill-rule="evenodd" d="M1 13.5A1.5 1.5 0 0 0 2.5 15h11a1.5 1.5 0 0 0 1.5-1.5v-6a.5.5 0 0 0-1 0v6a.5.5 0 0 1-.5.5h-11a.5.5 0 0 1-.5-.5v-11a.5.5 0 0 1 .5-.5H9a.5.5 0 0 0 0-1H2.5A1.5 1.5 0 0 0 1 2.5z" /></svg></a>';
                        return html;
                    }
                },
            ]

        });
        $("#divLoadingProgress").hide();
    }
};