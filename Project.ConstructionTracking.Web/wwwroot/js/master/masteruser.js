
const user = {
    init: () => {
        user.AjaxGrid();

        $("#user-create").click(() => {
            window.location.href = baseUrl + 'masteruser/create';
            return false;
        });

        $("#user-edit").click(() => {
            window.location.href = baseUrl + 'masteruser/edit';
            return false;
        });
    },
    AjaxGrid: function () {

    }
}
