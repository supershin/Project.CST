﻿const form = {
    init: () => {
        form.AjaxGrid();

        $("#form-create").click(() => {
            $("#modal-form-c").modal('show');
            return false;
        });

        $("#form-edit").click(() => {
            $("#modal-form-e").modal('show');
            return false;
        });

    },
    AjaxGrid: function () {

    }
}