let signaturePad;
const checklistqc = {
    init: () => {
        checklistqc.initSignaturePad();

        $("#btn-sign").click(() => {
            $('#modal-sign').modal('show');
            return false;
        });

        $("#btn-save-sign").click(() => {
            $('#modal-sign').modal('hide');
            if (!detailLogin.getSignatureData()) {
                $('#success-icon').hide();
            } else {
                $('#success-icon').show();
            }
            return false;
        });

        $('input[name="action-radio"]').click(function () {
            var $radio = $(this);
            if ($radio.data('waschecked') == true) {
                $radio.prop('checked', false);
                $radio.data('waschecked', false);
                $('#checklist-container').show();
            } else {
                $radio.data('waschecked', true);
                $('#checklist-container').hide();
            }
        });

        $('#save-qc-checklist').click(() => {
            gatherAllValues();
        });
    },
    initSignaturePad: () => {
        $('#modal-sign').on('shown.bs.modal', function (e) {
            if (signaturePad == null) {
                let canvas = $("#signature-pad canvas");
                let parentWidth = $(canvas).parent().outerWidth();
                let parentHeight = $(canvas).parent().outerHeight();
                canvas.attr("width", parentWidth + 'px')
                    .attr("height", parentHeight + 'px');
                signaturePad = new SignaturePad(canvas[0], {
                    backgroundColor: 'rgb(255, 255, 255)'
                });
            }
        });

        $(document).on('click', '#modal-sign .clear', function () {
            $('#success-icon').hide();
            signaturePad.clear();
        });

    },
    getSignatureData: () => {
        let dataURL;
        let contentType;
        let storage;
        if (signaturePad && !signaturePad.isEmpty()) {
            dataURL = signaturePad.toDataURL();
            var parts = dataURL.split(';base64,');
            contentType = parts[0].split(":")[1];
            storage = parts[1];
        }
        return storage;
    },
}

function gatherAllValues() {
    let data = {
        projectID: projectId,  // Assuming this is in your model
        unitID: unitId,  // Assuming this is in your model
        checkListID: qcCheckListId,
        qcTypeID: qcTypeId,
        seq: seq,

        // Sign resource
        peID: peId,
        peSignResource: checklistqc.getSignatureData(), // Assuming this captures the signature resource

        // Action resource
        isNotReadyInspect: $('input[name="action-radio"]').is(':checked'),  // Checkbox input
        qcStatusID: 15 ,  // Pass/fail radio button
        remark: $('textarea').val(),  // General remarks
        images: Array.from($('#file-input')[0].files),  // Collect files from main file input

        // Checklist detail
        checkListItems: []
    };

    // Loop through the list of known checklist detail IDs
    listID.forEach(function (id) {
        // Get the selected radio button for this checklist detail ID
        let selectedRadio = document.querySelector(`input[name="radios-${id}"]:checked`);

        // Get the corresponding comment and files for this checklist detail
        let comment = document.getElementById('description-' + id)?.value || '';
        let fileInput = document.getElementById('file-input-' + id)?.files || [];

        // Check if a radio button is selected, or a comment is provided, or files are uploaded
        if (selectedRadio || comment || fileInput.length > 0) {
            // Determine if the selected radio is "pass" or "fail"
            let conditionPass = selectedRadio && selectedRadio.value === 'pass';
            let conditionNotPass = selectedRadio && selectedRadio.value === 'notpass';

            // Push the gathered values into the checklistItems array
            data.checkListItems.push({
                checkListDetailID: id,  // Checklist detail ID
                conditionPass: conditionPass,  // True if "pass" is selected
                conditionNotPass: conditionNotPass,  // True if "notpass" is selected
                detailRemark: comment,  // Remarks specific to this checklist detail
                detailImage: Array.from(fileInput)  // Convert the file list to an array
            });
        }
    });



    console.log(data);
    // Send the gathered data to the server using AJAX or handle it further as needed
}


