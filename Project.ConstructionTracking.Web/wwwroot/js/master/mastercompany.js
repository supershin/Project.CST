
const company = {
    init: () => {
        company.AjaxGrid();

        $("#company-create").click(() => {
            $("#modal-company-c").modal('show');
            return false;
        });

        $("#company-edit").click(() => {
            $("#modal-company-e").modal('show');
            return false;
        });
    },
    AjaxGrid: function () {

    }
}