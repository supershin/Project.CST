function toggleRadio(radio) {
    // Check if the radio button was already checked
    if (radio.checked && radio.dataset.wasChecked) {
        // If so, uncheck it
        radio.checked = false;
        radio.dataset.wasChecked = "";
    } else {
        // Uncheck all radios in the same group
        var radios = document.getElementsByName(radio.name);
        for (var i = 0; i < radios.length; i++) {
            radios[i].dataset.wasChecked = "";
        }
        // Set the current radio as checked
        radio.dataset.wasChecked = "true";
    }
}

function openModal(UnitFormID, GroupID, FormID, PC_ID) {
    // Fetch images for RoleID = 1
    $.ajax({
        url: baseUrl + 'PMApprove/GetImages',
        type: 'GET',
        data: { UnitFormID: UnitFormID, GroupID: GroupID, FormID: FormID, RoleID: 1 },
        success: function (images) {
            var containerRole1 = $('#image-container-role-1');
            containerRole1.empty(); // Clear any previous images

            images.forEach(function (image) {
                var imageHtml = `
                    <div class="col-6 position-relative">
                        <a data-fslightbox="gallery1" href="${image.FilePath}">
                            <img src="${image.FilePath}" alt="..." class="img-thumbnail">
                        </a>
                    </div>
                `;
                containerRole1.append(imageHtml);
            });

            refreshFsLightbox(); // Reinitialize fslightbox for Role 1 images
        },
        error: function (error) {
            console.error('Failed to fetch images for RoleID 1:', error);
        }
    });

    // Fetch images for RoleID = 2
    $.ajax({
        url: baseUrl + 'PMApprove/GetImages',
        type: 'GET',
        data: { UnitFormID: UnitFormID, GroupID: GroupID, FormID: FormID, RoleID: 2 },
        success: function (images) {
            var containerRole2 = $('#image-container-role-2');
            containerRole2.empty(); // Clear any previous images

            images.forEach(function (image) {
                var imageHtml = `
                    <div class="col-6 position-relative">
                        <a data-fslightbox="gallery2" href="${image.FilePath}">
                            <img src="${image.FilePath}" alt="..." class="img-thumbnail">
                        </a>
                    </div>
                `;
                containerRole2.append(imageHtml);
            });

            refreshFsLightbox(); // Reinitialize fslightbox for Role 2 images
        },
        error: function (error) {
            console.error('Failed to fetch images for RoleID 2:', error);
        }
    });

    // Fetch images for Unlock
    $.ajax({
        url: baseUrl + 'PJMApprove/GetImagesUnlock',
        type: 'GET',
        data: { UnitFormID: UnitFormID, PassConditionID: PC_ID },
        success: function (images) {
            var containerUnlock = $('#image-container-Unlock');
            containerUnlock.empty(); // Clear any previous images

            images.forEach(function (image) {
                var imageHtml = `
                    <div class="col-6 position-relative">
                        <a data-fslightbox="galleryUnlock" href="${image.FilePath}">
                            <img src="${image.FilePath}" alt="..." class="img-thumbnail">
                        </a>
                    </div>
                `;
                containerUnlock.append(imageHtml);
            });

            refreshFsLightbox(); // Reinitialize fslightbox for Unlock images
        },
        error: function (error) {
            console.error('Failed to fetch images for Unlock:', error);
        }
    });

    // Show the modal after all AJAX calls have been made
    var myModal = new bootstrap.Modal(document.getElementById('exampleModal'));
    myModal.show();
}

function deleteImage(resourceId) {
    Swal.fire({
        title: '',
        text: "ต้องการลลบรูปภาพหรือไม่?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'ลบรูปภาพ',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.isConfirmed) {
            // Perform the deletion via an AJAX call
            $.ajax({
                url: baseUrl + 'FormCheckList/DeleteImage',
                type: 'POST',
                data: { resourceId: resourceId },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('ลบรูปภาพสำเร็จ', '', 'success');
                        // Remove the image from the DOM
                        $(`[onclick="deleteImage('${resourceId}')"]`).closest('.position-relative').remove();
                    } else {
                        Swal.fire('Error!', response.message, 'error');
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire('Error!', 'An error occurred while processing your request.', 'error');
                }
            });
        }
    });
}

//function saveOrSubmit(actionType) {
//    var data = new FormData();
//    var allGroupsChecked = true;
//    var allPassed = true; // New flag to check if all cards are passed
//    var allRemarksFilled = true;

//    data.append("ActionType", actionType);
//    data.append("Remark", document.getElementById("mainRemark").value.trim());
//    data.append("UnitFormID", document.getElementById("UnitFormID").value);
//    data.append("ProjectID", document.getElementById("ProjectID").value);
//    data.append("UnitID", document.getElementById("UnitID").value);
//    data.append("FormID", document.getElementById("FormID").value);

//    var passConditions = [];

//    $("input[data-action='gr-pass']").each(function () {
//        let group_id = $(this).attr("group-id");
//        let passconID = $(this).attr("passconID");
//        let _statusID = $(this).is(":checked") ? $(this).val() : null;
//        let remark = document.getElementById(`remark_${group_id}`).value;
//        let FlagActive = document.getElementById(`Flag-Active-${group_id}`).value;

//        FlagActive = parseInt(FlagActive);

//        if (FlagActive === 1) {
//            let existingCondition = passConditions.find(pc => pc.Group_ID === group_id);

//            if (!existingCondition) {
//                passConditions.push({
//                    PC_ID: passconID,
//                    Group_ID: group_id,
//                    StatusID: _statusID,
//                    PC_FlagActive: FlagActive,
//                    PJM_Remark: remark ? remark.trim() : '',
//                });
//            } else {
//                if (_statusID !== null) {
//                    existingCondition.StatusID = _statusID;
//                }
//            }

//            // Check for "ไม่อนุมัติ" or missing remark
//            if (_statusID === "9") {
//                if (remark.trim() === "" || document.getElementById("mainRemark").value.trim() === "") {
//                    allRemarksFilled = false;
//                    scrollToElement(document.getElementById(`remark_${group_id}`));
//                }
//            }

//            // Check if all cards are approved (statusID === 8)
//            if (_statusID !== 8) {
//                allPassed = false; // Set to false if any card is not approved
//            }

//            alert(_statusID);

//            // Ensure that all cards are checked
//            let groupChecked = $("input[data-action='gr-pass'][group-id='" + group_id + "']:checked").length > 0;
//            if (!groupChecked) {
//                allGroupsChecked = false;
//            }
//        }
//    });

//    if (passConditions.length > 0) {
//        data.append("PassConditionsIUD", JSON.stringify(passConditions));
//    }

//    var files = document.getElementById("file-input").files;
//    for (var i = 0; i < files.length; i++) {
//        data.append("Images", files[i]);
//    }

//    // The main check for the "submit" action type
//    if (actionType === "submit") {
//        if (!allGroupsChecked) {
//            showErrorAlert('คำเตือน', 'กรุณาเลือกการอนุมัติหรือไม่อนุมัติในทุกงานที่ใช้งานอยู่');
//            return;
//        }
//        if (!allRemarksFilled) {
//            showErrorAlert('คำเตือน!', 'กรุณาระบุเหตุผลสำหรับการไม่อนุมัติในทุกงานและหมายเหตุงวดนี้');
//            return;
//        }

//        showConfirmationAlert(
//            'ยืนยันการดำเนินการ',
//            'คุณต้องการยืนยันงวดงานนี้หรือไม่',
//            'warning',
//            'ใช่',
//            'ยกเลิก',
//            function () {
//                showLoadingAlert();
//                $.ajax({
//                    url: baseUrl + 'PJMApprove/SaveOrSubmit',
//                    type: 'POST',
//                    contentType: false,
//                    processData: false,
//                    data: data,
//                    success: function (res) {
//                        Swal.close();
//                        if (res.success) {
//                            // If all cards passed and the actionType is submit, generate PDF
//                            if (allPassed) {
//                                generatePDFAfterSave(data);
//                            }
//                            else {
//                                showSuccessAlert('สำเร็จ!', 'บันทึกสำเร็จ', () => {
//                                    window.location.reload();
//                                });
//                            }
//                        } else {
//                            showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
//                        }
//                    },
//                    error: function (xhr, status, error) {
//                        Swal.close();
//                        showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
//                    }
//                });
//            }
//        );
//    } else {
//        // If actionType is not "submit", proceed with a normal save
//        showLoadingAlert();
//        $.ajax({
//            url: baseUrl + 'PJMApprove/SaveOrSubmit',
//            type: 'POST',
//            contentType: false,
//            processData: false,
//            data: data,
//            success: function (res) {
//                Swal.close();
//                if (res.success) {
//                    showSuccessAlert('สำเร็จ!', 'บันทึกสำเร็จ', () => {
//                        window.location.reload();
//                    });
//                } else {
//                    showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
//                }
//            },
//            error: function (xhr, status, error) {
//                Swal.close();
//                showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
//            }
//        });
//    }
//}

function saveOrSubmit(actionType) {
    var data = new FormData();
    var allGroupsChecked = true;
    var allPassed = true; // Flag to check if all groups are passed
    var allRemarksFilled = true;

    data.append("ActionType", actionType);
    data.append("Remark", document.getElementById("mainRemark").value.trim());
    data.append("UnitFormID", document.getElementById("UnitFormID").value);
    data.append("ProjectID", document.getElementById("ProjectID").value);
    data.append("UnitID", document.getElementById("UnitID").value);
    data.append("FormID", document.getElementById("FormID").value);

    var passConditions = [];

    $("input[data-action='gr-pass']").each(function () {
        let group_id = $(this).attr("group-id");
        let passconID = $(this).attr("passconID");
        let _statusID = $(this).is(":checked") ? $(this).val() : null;
        let remark = document.getElementById(`remark_${group_id}`).value;
        let FlagActive = document.getElementById(`Flag-Active-${group_id}`).value;

        FlagActive = parseInt(FlagActive);

        if (FlagActive === 1) {
            let existingCondition = passConditions.find(pc => pc.Group_ID === group_id);

            if (!existingCondition) {
                passConditions.push({
                    PC_ID: passconID,
                    Group_ID: group_id,
                    StatusID: _statusID,
                    PC_FlagActive: FlagActive,
                    PJM_Remark: remark ? remark.trim() : '',
                });
            } else {
                if (_statusID !== null) {
                    existingCondition.StatusID = _statusID;
                }
            }

            // Check if not approved (value = 9), and remark is missing
            if (_statusID === "9") {
                if (remark.trim() === "" || document.getElementById("mainRemark").value.trim() === "") {
                    allRemarksFilled = false;
                    scrollToElement(document.getElementById(`remark_${group_id}`));
                }
            }

            // Ensure that all active groups are either approved (statusID === "8")
            let groupChecked = $("input[name='radios-inline-" + group_id + "']:checked").val();
            if (groupChecked === "8") {
                // Passed
            } else {
                allPassed = false; // If any group is not approved (checked value is not 8)
            }

            //alert(allPassed)

            if (!groupChecked) {
                allGroupsChecked = false; // Ensure all groups have a selection
            }
        }
    });

    if (passConditions.length > 0) {
        data.append("PassConditionsIUD", JSON.stringify(passConditions));
    }

    var files = document.getElementById("file-input").files;
    for (var i = 0; i < files.length; i++) {
        data.append("Images", files[i]);
    }

    // The main check for the "submit" action type
    if (actionType === "submit") {
        if (!allGroupsChecked) {
            showErrorAlert('คำเตือน', 'กรุณาเลือกการอนุมัติหรือไม่อนุมัติในทุกงานที่ใช้งานอยู่');
            return;
        }
        if (!allRemarksFilled) {
            showErrorAlert('คำเตือน!', 'กรุณาระบุเหตุผลสำหรับการไม่อนุมัติในทุกงานและหมายเหตุงวดนี้');
            return;
        }

        showConfirmationAlert(
            'ยืนยันการดำเนินการ',
            'คุณต้องการยืนยันงวดงานนี้หรือไม่',
            'warning',
            'ใช่',
            'ยกเลิก',
            function () {
                showLoadingAlert();
                $.ajax({
                    url: baseUrl + 'PJMApprove/SaveOrSubmit',
                    type: 'POST',
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (res) {
                        if (res.success) {
                            // If all cards are passed, call PDF generation
                            if (allPassed) {
                                generatePDFAfterSave(data);
                            }
                            else {
                                showSuccessAlert('สำเร็จ!', 'บันทึกสำเร็จ', () => {
                                    window.location.reload();
                                });
                            }
                        } else {
                            showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
                        }
                    },
                    error: function (xhr, status, error) {
                        Swal.close();
                        showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
                    }
                });
            }
        );
    } else {
        // If actionType is not "submit", proceed with a normal save
        showLoadingAlert();
        $.ajax({
            url: baseUrl + 'PJMApprove/SaveOrSubmit',
            type: 'POST',
            contentType: false,
            processData: false,
            data: data,
            success: function (res) {
                Swal.close();
                if (res.success) {
                    showSuccessAlert('สำเร็จ!', 'บันทึกสำเร็จ', () => {
                        window.location.reload();
                    });
                } else {
                    showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
                }
            },
            error: function (xhr, status, error) {
                Swal.close();
                showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
            }
        });
    }
}




document.addEventListener("DOMContentLoaded", function () {
    var dropZone = document.getElementById("drop-zone");
    var fileInput = document.getElementById("file-input");
    var previewContainer = document.getElementById("preview-container");
    var filesArray = [];

    dropZone.addEventListener("click", function (e) {
        if (e.target.classList.contains("remove-button")) {
            return;
        }
        fileInput.click();
    });

    fileInput.addEventListener("change", function () {
        if (fileInput.files.length) {
            addFilesToPreview(fileInput.files);
        }
    });

    dropZone.addEventListener("dragover", function (e) {
        e.preventDefault();
        dropZone.classList.add("drop-zone--over");
    });

    dropZone.addEventListener("dragleave", function () {
        dropZone.classList.remove("drop-zone--over");
    });

    dropZone.addEventListener("drop", function (e) {
        e.preventDefault();
        addFilesToPreview(e.dataTransfer.files);
    });

    function addFilesToPreview(files) {
        Array.from(files).forEach(file => {
            // Check if the file is already in the array
            if (!filesArray.some(existingFile => existingFile.name === file.name && existingFile.size === file.size)) {
                filesArray.push(file);
                const reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = function (event) {
                    const img = document.createElement("img");
                    img.src = event.target.result;
                    const previewImage = document.createElement("div");
                    previewImage.className = "col-4 preview-image";

                    const removeButton = document.createElement("button");
                    removeButton.className = "remove-button";
                    removeButton.innerHTML = "&times;";
                    removeButton.addEventListener("click", function (e) {
                        e.stopPropagation(); // Stop event from bubbling up to the dropzone
                        filesArray = filesArray.filter(f => f !== file);
                        updateFileInput();
                        previewImage.remove();
                    });

                    previewImage.appendChild(img);
                    previewImage.appendChild(removeButton);
                    previewContainer.appendChild(previewImage);
                };
            }
        });
        updateFileInput();
    }

    function updateFileInput() {
        // Clear the file input so that it can be used to add more files
        fileInput.value = '';

        // Create a new DataTransfer object to hold the remaining files
        const dataTransfer = new DataTransfer();
        filesArray.forEach(file => dataTransfer.items.add(file));
        fileInput.files = dataTransfer.files;
    }
});
