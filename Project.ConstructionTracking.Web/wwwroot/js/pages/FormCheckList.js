function toggleRadio(radio) {
    if (radio.checked && radio.dataset.wasChecked) {
        radio.checked = false;
        radio.dataset.wasChecked = "";
    } else {
        var radios = document.getElementsByName(radio.name);
        for (var i = 0; i < radios.length; i++) {
            radios[i].dataset.wasChecked = "";
        }
        radio.dataset.wasChecked = "true";
    }
}


const form_check_list = {
    init: () => {
        $("#btn-save-pe").click(() => {
            form_check_list.saveCheckList();
            return false;
        });
    },
    setFormCheckList: () => {
        var checklists = [];

        $("input[data-action='chk-check-list']:checked").each(function (key, item) {

            var form_id = $(this).attr("form-id");

            var group_id = $(this).attr("group-id");
            var package_id = $(this).attr("package-id");
            var checklist_id = $(this).attr("check-list-id");
            var checklist_status_id = $(this).val();
            var remak = $("#input-remark-" + checklist_id).val();


            var checklist = {
                FormID: form_id,
                GroupID: group_id,
                PackageID: package_id,
                CheckListID: checklist_id,
                CheckListStatusID: checklist_status_id,
                Remark: remak
            };
            checklists.push(checklist)
        });
        return checklists;
    },
    saveCheckList: () => {

        var data = {
            FormID: 1,
            CheckList: form_check_list.setFormCheckList()
        };

        $.ajax({
            url: baseUrl + 'FormCheckList/UpdateStatus',
            type: 'post',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                //if (data.Success) {
                //    Application.PNotify(data.Message, "success");
                //    window.location.reload();
                //}
                //else {
                //    Application.PNotify(data.Message, "error");
                //}
                //Application.LoadWait(false);
            },
            data: JSON.stringify(data)
        });
        return false;
    }
}