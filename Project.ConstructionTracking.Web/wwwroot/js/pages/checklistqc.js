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
            var data = gatherAllValues();

            showConfirmationAlert(
                '',
                "ต้องการบันทึกข้อมูลหรือไม่?",
                'warning',
                'บันทึกข้อมูล',
                'ยกเลิก',
                () => {
                    checklistqc.saveQcCheckList(data);
                }
            )
        });

        $('#submit-qc-checklist').click((event) => {
            event.preventDefault(); // Prevent the default form submission

            var allRadiosValid = true;
            var unselectedDetails = [];

            // Loop through each checklist detail to check radio selection
            document.querySelectorAll("[name^='radios-']").forEach(function (radioGroup) {
                var detailID = radioGroup.getAttribute('name').split('-')[1];
                var isChecked = document.querySelector('input[name="radios-' + detailID + '"]:checked');

                if (!isChecked) {
                    allRadiosValid = false;
                    unselectedDetails.push(detailID);  // Keep track of the unselected radio groups
                }
            });

            var remark = $('#main-remark').val().trim();
            var hasImages = images.length > 0;
            var uploadImages = document.getElementById('file-input').files;
            console.log(uploadImages)
            var hasSignature = checklistqc.getSignatureData() || signImage.trim() !== "";

            // Validate remark, images, and signature
            if (remark === "") {
                showErrorAlert('ผิดพลาด!', 'กรุณาตรวจสอบความคิดเห็น');
                return; // Exit early
            }
            if (!hasImages && uploadImages.length == 0) {
                showErrorAlert('ผิดพลาด!', 'กรุณาอัปโหลดอย่างน้อย 1 รูปภาพ'); // Alert in Thai: Please upload at least one image
                return; // Exit early
            }
            if (!hasSignature) {
                showErrorAlert('ผิดพลาด!', 'กรุณาเซ็นต์ลายเซ็นต์ผู้ควบคุมงาน');
                return; // Exit early
            }

            var data = gatherAllValues();

            // Confirm submission if action radio is checked
            if ($('input[name="action-radio"]').is(':checked')) {
                showConfirmationAlert(
                    '',
                    "ต้องการส่งข้อมูลหรือไม่?",
                    'warning',
                    'ส่งข้อมูล',
                    'ยกเลิก',
                    () => {
                        checklistqc.submitQcCheckList(data);
                    }
                );
            } else if (!allRadiosValid) {
                // If not all radio groups are selected
                showErrorAlert('ผิดพลาด!', 'กรุณาตรวจรายการตรวจสอบ QC ให้ครบทุกข้อ');
            } else {
                // Confirm submission for normal case
                showConfirmationAlert(
                    '',
                    "ต้องการส่งข้อมูลหรือไม่?",
                    'warning',
                    'ส่งข้อมูล',
                    'ยกเลิก',
                    () => {
                        checklistqc.submitQcCheckList(data);
                    }
                );
            }
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
    saveQcCheckList: (data) => {
        $.ajax({
            url: baseUrl + 'QCCheckList/SaveQcCheckList',
            type: 'POST',
            dataType: 'json',
            data: data,
            processData: false,  // Required to prevent jQuery from processing the data (FormData handles this)
            contentType: false,  // Required to let the browser set the `Content-Type` with `multipart/form-data`
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'ทำการบันทึกข้อมูลสำเร็จ',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            var controller = 'QCCheckList/CheckListDetail?';
                            var params = `id=${resp.data.QcID}&projectid=${resp.data.ProjectID}&unitid=${resp.data.UnitID}&qcchecklistid=${resp.data.CheckListID}&seq=${resp.data.Seq}&qctypeid=${resp.data.QcTypeID}`;  // Use backticks for template literals
                            window.location.href = baseUrl + controller + params
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: "ทำการแก้ไขข้อมูลไม่สำเร็จ",
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr, status, error) {
                // do something
                alert(" Coding Error ")
            },
        });
        return false;
    },
    submitQcCheckList: (data) => {
        $.ajax({
            url: baseUrl + 'QCCheckList/SubmitQcCheckList',
            type: 'POST',
            dataType: 'json',
            data: data,
            processData: false,  // Required to prevent jQuery from processing the data (FormData handles this)
            contentType: false,  // Required to let the browser set the `Content-Type` with `multipart/form-data`
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'ทำการบันทึกข้อมูลสำเร็จ',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            var controller = 'QCCheckList/CheckListDetail?';
                            var params = `id=${resp.data.QcID}&projectid=${resp.data.ProjectID}&unitid=${resp.data.UnitID}&qcchecklistid=${resp.data.CheckListID}&seq=${resp.data.Seq}&qctypeid=${resp.data.QcTypeID}`;  // Use backticks for template literals
                            window.location.href = baseUrl + controller + params
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: "ทำการบันทึกข้อมูลไม่สำเร็จ",
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr, status, error) {
                // do something
                alert(" Coding Error ")
            },
        });
        return false;
    },
}

function gatherAllValues() {
    event.preventDefault();
    // Create a FormData object to handle form inputs and files
    var formData = new FormData();

    // Gather main form data
    formData.append('QcID', qcId); // Assuming you have `qcId` defined in your scope
    formData.append('ProjectID', projectId); // Assuming projectId is available
    formData.append('UnitID', unitId); // Assuming unitId is available
    formData.append('CheckListID', qcCheckListId); // Assuming qcCheckListId is available
    formData.append('QCTypeID', qcTypeId); // Assuming qcTypeId is available
    formData.append('Seq', seq); // Assuming seq is available
    formData.append('PeID', peId); // Assuming peId is available null)
    if (checklistqc.getSignatureData()) formData.append('PeSignResource', checklistqc.getSignatureData());
    formData.append('IsNotReadyInspect', $('input[name="action-radio"]').is(':checked'));
    formData.append('Remark', $('#main-remark').val()); // General remark field
    
    // Gather checklist detail data
    var checklistItems = [];
    listID.forEach(function (id) {
        var selectedRadio = document.querySelector(`input[name="radios-${id}"]:checked`);
        var detailRemark = $("#description-" + id).val();

        if (selectedRadio || detailRemark || document.getElementById(`file-input-${id}`).files.length > 0) {
            var checklistItem = {
                CheckListDetailID: id,
                ConditionPass: selectedRadio && selectedRadio.value === 'pass',
                ConditionNotPass: selectedRadio && selectedRadio.value === 'notpass',
                DetailRemark: detailRemark
            };

            // Collect associated images for each checklist item
            var fileInput = document.getElementById(`file-input-${id}`);
            if (fileInput && fileInput.files.length > 0) {
                for (var i = 0; i < fileInput.files.length; i++) {
                    formData.append(`CheckListItems[${checklistItems.length}].DetailImage`, fileInput.files[i]);
                }
            }

            checklistItems.push(checklistItem);
        }
    });

    // Add checklist items to FormData
    checklistItems.forEach((item, index) => {
        formData.append(`CheckListItems[${index}].CheckListDetailID`, item.CheckListDetailID);
        formData.append(`CheckListItems[${index}].ConditionPass`, item.ConditionPass);
        formData.append(`CheckListItems[${index}].ConditionNotPass`, item.ConditionNotPass);
        formData.append(`CheckListItems[${index}].DetailRemark`, item.DetailRemark);
    });

    // File handling for the main section (general file input)
    var mainFileInput = document.getElementById('file-input');
    if (mainFileInput && mainFileInput.files.length > 0) {
        for (var i = 0; i < mainFileInput.files.length; i++) {
            formData.append('Images', mainFileInput.files[i]);
        }
    }

    //console.log(formData);
    return formData;
    // Send the gathered data to the server using AJAX or handle it further as needed
}

function deleteImage(resourceId, detailId) {
    showConfirmationAlert(
        '',
        "ต้องการลบรูปภาพหรือไม่?",
        'warning',
        'ลบรูปภาพ',
        'ยกเลิก',
        () => {

            showLoadingAlert('กำลังลบรูปภาพ...', 'กรุณารอสักครู่');

            $.ajax({
                url: baseUrl + 'QCCheckList/DeleteImage',
                type: 'POST',
                data: { qcID: qcId, resourceID: resourceId, detailID: detailId },
                success: function (response) {
                    Swal.close();
                    if (response.success) {
                        showSuccessAlert('ลบรูปภาพสำเร็จ', '', () => {
                            window.location.reload();
                        });
                    } else {
                        showErrorAlert('Error!', response.message);
                    }
                },
                error: function (xhr, status, error) {
                    Swal.close();
                    showErrorAlert('ผิดพลาด!', 'ลบรูปภาพไม่สำเร็จ');
                }
            });
        }
    );
}