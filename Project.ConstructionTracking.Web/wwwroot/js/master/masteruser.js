
const user = {
    init: () => {

        $("#user-create").click(() => {
            window.location.href = baseUrl + 'masteruser/create';
            return false;
        });

        $('#user-delete-save').click(() => {
            var userId = $('#delete-user-id').val();

            var data = {
                userId: userId
            }
            user.DeleteUser(data);
        })

        $("#btn-search").click(() => {
            if ($.fn.DataTable.isDataTable('#tbl-user-list')) {
                $('#tbl-user-list').DataTable().clear().destroy();
            }

            user.UserList();

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
                    $("button[data-action='user-delete']").unbind('click').click((e) => {
                        //e.preventDefault(); // Prevent default action

                        var userId = $(e.currentTarget).attr('data-id');

                        var data = {
                            userId: userId
                        }

                        $('#delete-user-id').val(userId);

                        $('#partial-confirm-delete-user').modal('show');

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
    },
    DeleteUser: function (data) {
        $.ajax({
            url: baseUrl + 'masteruser/UserDelete',
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
    }
}