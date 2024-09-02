
const company = {
    init: () => {
        $("#company-create").click(() => {
            $("#modal-company-c").modal('show');
            return false;
        });

        $("#company-save").click(() => {
            var companyName = $('#company-name').val();
            var data = {
                CompanyVendorName: companyName
            };

            company.CreateCompany(data);

            return false;
        });

        $("#btn-search").click(() => {
            if ($.fn.DataTable.isDataTable('#tbl-company')) {
                $('#tbl-company').DataTable().clear().destroy();
            }

            company.AjaxGrid();

            return false;
        });

        $('#company-vendor-save').click(() => {
            var companyId = $('#companyID').val();

            var data = {
                CompanyID: companyId
            }
            company.DeleteCompany(data);
        });

        company.AjaxGrid();
    },
    AjaxGrid: function () {
        tblCompany = $('#tbl-company').dataTable({
            "dom": '<<t>lip>',
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: baseUrl + 'mastercompany/GetListMasterCompany',
                type: "POST",
                data: function (json) {
                    var datastring = $("#form-search").serialize();
                    json = datastring + '&' + $.param(json);

                    return json;
                },
                "dataSrc": function (res) {                 
                    return res.data;
                },
                complete: function (res) {
                    $(document).on('click', "button[data-action='company-detail']", function (e) {
                        var companyId = $(e.currentTarget).attr('data-id');

                        window.location.href = baseUrl + 'mastercompany/detail?companyID=' + companyId;
                    });
                    $(document).on('click', "button[data-action='company-delete']", function (e) {
                        //e.preventDefault(); // Prevent default action
                        var companyID = $(e.currentTarget).attr('data-id');
                        $('#companyID').val(companyID);
                        $('#partial-confirm-delete').modal('show');
                    });
                }
            },
            "ordering": true,
            "order": [[1, "DESC"]],
            "columns": [
                { 'data': 'Name', "className": "text-left " },
                { 'data': 'UpdateDate', "className": "text-center " },
                {
                    'data': 'ID',
                    'orderable': false,
                    'width': '20px',
                    "className": "text-center",
                    'mRender': function (ID, type, data, obj) {
                        var html = '<button  data-action="company-detail" data-id="' + data.ID + '" class="btn bg-black-lt btn-icon btn-rounded" style="margin-right:10px;">';
                        html += '<i class="fa-regular fa-pen-to-square"></i>';
                        html += '</button>';
                        html += '<span>';
                        html += '<button data-action="company-delete" data-id ="' + data.ID + '"class="btn bg-red-lt btn-icon btn-rounded">'
                        html += '<i class="fa-regular fa-trash-can"></i>';
                        html += '</button>';
                        html += '</span>';
                        return html;
                    }
                },
            ]
        });
    },
    CreateCompany: function (data) {
        $.ajax({
            url: baseUrl + 'mastercompany/CreateCompanyVendor',
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
    DeleteCompany: function (data) {
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