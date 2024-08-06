const vendor = {
    init: () => {
        vendor.AjaxGrid();

        $("#vendor-create").click(() => {
            $("#modal-vendor-c").modal('show');
            return false;
        });

        $("#vendor-edit").click(() => {
            $("#modal-vendor-e").modal('show');
            return false;
        });
    },
    AjaxGrid: function () {

    }
}