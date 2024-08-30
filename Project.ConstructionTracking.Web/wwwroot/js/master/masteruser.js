
const user = {
    init: () => {

        $("#user-create").click(() => {
            window.location.href = baseUrl + 'masteruser/create';
            return false;
        });

        

        user.UserList();
    },
    UserList: function () {
        tblUser = $('#tbl-user-list').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'masteruser/userlist',
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
                    $("button[data-action='user-edit']").unbind('click').click((e) => {
                        let userId = $(e.currentTarget).attr('data-id');

                        var encodedStringBtoA = btoa(userId)

                        const newUrl = baseUrl + 'masteruser/detail?param=' + encodedStringBtoA;
                        // Redirect ไปยัง URL ใหม่
                        window.location.href = newUrl;

                        return false;
                    });
                    $(document).on('click', "button[data-action='user-delete']", function (e) {
                        //e.preventDefault(); // Prevent default action

                        var formTypeId = $(e.currentTarget).attr('data-id');

                        $('#formTypeID').val(formTypeId);

                        $('#partial-confirm-delete-FT').modal('show');

                    });

                }
            },
            "ordering": true,
            "order": [[0, "desc"]],
            "columns": [
                { 'data': 'FirstName', "className": "text-center " },
                { 'data': 'LastName', "className": "text-center " },
                { 'data': 'Email', "className": "text-center " },
                { 'data': 'Mobile', "className": "text-center " },
                { 'data': 'PositionName', "className": "text-center " },
                { 'data': 'RoleName', "className": "text-center " },
                {
                    'data': 'UserID',
                    'orderable': false,
                    'width': '20px',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<button  data-action="user-edit" data-id="' + data.UserID + '" class="btn bg-black-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<span>';
                        html += '<button data-action="user-delete" data-id ="' + data.UserID + '"class="btn bg-red-lt btn-icon btn-rounded">'
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>';
                        html += '</span>';
                        return html;
                    }
                },
            ]
        });
    }
}
