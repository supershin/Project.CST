const summaryqc = {
    init: () => {
        $("button[data-action='Verify']").click((e) => {
            var qcCheckListID = $(e.currentTarget).attr('data-qc-checklist-id');
            var qcTypeID = $(e.currentTarget).attr('data-qc-type-id');
            var formID = $(e.currentTarget).attr('data-form-id');
            var formQcCheckListID = $(e.currentTarget).attr('data-form-qc-checklist-id');
            var seq = $(e.currentTarget).attr('data-seq');

            var data = {
                ProjectID: projectId,
                UnitID: unitId,
                QcCheckListID: qcCheckListID,
                QcTypeID: qcTypeID,
                FormID: formID,
                formQcCheckListID: formQcCheckListID
            }
            
            summaryqc.VerifyQcCheckList(data);
        });

        $("button[data-action='Status']").click((e) => {
            var projectId = $(e.currentTarget).attr('data-project-id');
            var unitId = $(e.currentTarget).attr('data-unit-id');
            var qcCheckListID = $(e.currentTarget).attr('data-qc-checklist-id');
            var qcTypeID = $(e.currentTarget).attr('data-qc-type-id');
            var formID = $(e.currentTarget).attr('data-form-id');
            var formQcCheckListID = $(e.currentTarget).attr('data-qc-unit-checklist-id');
            var seq = $(e.currentTarget).attr('data-seq');

            var data = {
                ProjectID: projectId,
                UnitID: unitId,
                QcCheckListID: qcCheckListID,
                QcTypeID: qcTypeID,
                FormID: formID,
                QcUnitCheckListID: formQcCheckListID,
                Seq: seq // Added Seq to match the button attributes
            };

            // Build the URL with the parameters
            var controller = 'QCCheckList/CheckListDetail?';
            var params = `id=${data.QcUnitCheckListID}&projectid=${data.ProjectID}&unitid=${data.UnitID}&qcchecklistid=${data.QcCheckListID}&seq=${data.Seq}&qctypeid=${data.QcTypeID}`;
            window.location.href = baseUrl + controller + params;
            
        });
    },
    VerifyQcCheckList: function (data) {
        $.ajax({
            url: baseUrl + 'QCCheckList/VerifyQcCheckList',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    if (resp.data.QcCheckList.ID == null) {
                        var controller = 'QCCheckList/CheckListDetail?';
                        var params = `projectid=${projectId}&unitid=${unitId}&qcchecklistid=${resp.data.QcCheckList.QcCheckListID}&seq=${resp.data.QcCheckList.Seq}&qctypeid=${resp.data.QcCheckList.QcTypeID}`;  // Use backticks for template literals
                        window.location.href = baseUrl + controller + params
                        
                    } else if (resp.data.QcCheckList.ID != null && resp.data.QcCheckList.Seq != null) {
                        var controller = 'QCCheckList/CheckListDetail?';
                        var params = `id=${resp.data.QcCheckList.ID}&projectid=${projectId}&unitid=${unitId}&qcchecklistid=${resp.data.QcCheckList.QcCheckListID}&seq=${resp.data.QcCheckList.Seq}&qctypeid=${resp.data.QcCheckList.QcTypeID}&iscreate=true`;  // Use backticks for template literals
                        window.location.href = baseUrl + controller + params
                    }
                } else {
                    showErrorAlert('Error!', resp.message);
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

