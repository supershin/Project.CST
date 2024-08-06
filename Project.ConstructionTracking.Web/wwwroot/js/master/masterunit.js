const unit = {
    init: () => {
        unit.AjaxGrid();

        $("#unit-create").click(() => {
            $("#modal-unit-c").modal('show');
            return false;
        });

        $("#unit-edit").click(() => {
            $("#modal-unit-e").modal('show');
            return false;
        });
    },
    AjaxGrid: function () {

    }
}