

const detailCom = {
    init: () => {
        detailCom.AjaxGrid();

        $('#multiple-select-project').select2();

        $("#vendor-edit").click(() => {
            $("#modal-vendor-e").modal('show');
            return false;
        });
    },
    AjaxGrid: function () {

    }
}