
const company = {
    init: () => {
        company.AjaxGrid();

        $("#company-create").click(() => {
            $("#modal-company-c").modal('show');
            return false;
        });

        $("#company-edit").click(() => {
            window.location.href = baseUrl + 'mastercompany/detail';
            return false;
        });
    },
    AjaxGrid: function () {

    }
}