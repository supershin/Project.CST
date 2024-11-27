function openModal(UnitFormID, Group_ID) {
    $.ajax({
        url: baseUrl + 'PMApprove/GetImages', 
        type: 'GET',
        data: { UnitFormID: UnitFormID, GroupID: Group_ID, RoleID: 1 },
        success: function (images) {

            var modalBody = $('#exampleModal .modal-body .row');
            modalBody.empty(); 

            images.forEach(function (image) {
                var imageHtml = `
                    <div class="col-6 position-relative">
                        <a data-fslightbox="gallery" href="${baseUrl}${image.FilePath}">
                            <img src="${baseUrl}${image.FilePath}" alt="..." class="img-thumbnail">
                        </a>
                    </div>
                `;
                modalBody.append(imageHtml);
            });

            refreshFsLightbox();

            var myModal = new bootstrap.Modal(document.getElementById('exampleModal'));
            myModal.show();
        },
        error: function (error) {
            console.error('Failed to fetch images:', error);
        }
    });
}


function toggleRadio(radio, itemId) {
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

    if (radio.value === "7") {
        var mainApprovalRadios = document.getElementsByName('radios-inline-approval');
        for (var j = 0; j < mainApprovalRadios.length; j++) {
            if (mainApprovalRadios[j].value === "5") {
                mainApprovalRadios[j].checked = true;
            }
        }
    }

    if (radio.name === "radios-inline-approval" && (radio.value === "4" || radio.value === "6")) {
        var conditionalRadios = document.querySelectorAll(`input[name^="radios-inline-"][value="7"][data-pc-flag-active="True"]`);
        for (var k = 0; k < conditionalRadios.length; k++) {
            if (conditionalRadios[k].checked) {
                showErrorAlert('คำเตือน!', 'ยังมีการไม่อนุมัติแบบมีเงื่อนไข');
                radio.checked = false;
                return; 
            }
        }
    }

}


document.addEventListener("DOMContentLoaded", function () {
    var dropZone = document.getElementById("drop-zone");
    var fileInput = document.getElementById("file-input");
    var previewContainer = document.getElementById("preview-container");

    dropZone.addEventListener("click", function () {
        fileInput.click();
    });

    fileInput.addEventListener("change", function () {
        if (fileInput.files.length) {
            previewFiles(fileInput.files);
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
        fileInput.files = e.dataTransfer.files;
        previewFiles(fileInput.files);
    });

    function previewFiles(files) {
        previewContainer.innerHTML = ""; // Clear previous previews
        Array.from(files).forEach(file => {
            if (file.type.startsWith("image/")) {
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
                    removeButton.addEventListener("click", function () {
                        previewImage.remove();
                    });

                    previewImage.appendChild(img);
                    previewImage.appendChild(removeButton);
                    previewContainer.appendChild(previewImage);
                };
            }
        });
    }
});


function validateGroups() {
    var allGroupsChecked = true;  
    $("input[data-action='gr-pass']").each(function () {
        let group_id = $(this).attr("group-id"); 
        let pcFlagActive = $(this).attr('data-pc-flag-active');  
        //debugger
        if (pcFlagActive === "True") {
            var groupRadios = $(`input[data-action='gr-pass'][group-id='${group_id}']`);
            //debugger
            var anyRadioChecked = false;
            groupRadios.each(function () {
                if ($(this).is(":checked")) {
                    anyRadioChecked = true; 
                }
            });

            if (!anyRadioChecked) {
                allGroupsChecked = false; 
                return false; 
            }
        }
    });

    return allGroupsChecked;
}


function saveOrSubmit(actionType) {
    var remarkElement = document.getElementById("mainRemark");
    var listimagecnt = document.getElementById("listimagecnt").value;
    var files = document.getElementById("file-input").files;
    var mainStatus = document.querySelector('input[name="radios-inline-approval"]:checked');

    if (actionType === "submit") {
        if (!validateGroups()) {
            showErrorAlert('คำเตือน!', 'กรุณาเลือกตัวเลือกผ่านเพื่อส่ง PJM Head หรือ ไม่ผ่าน ทุกรายการกลุ่มงาน');
            return;
        }

        var radioGroups = document.querySelectorAll('input[data-action="gr-pass"]');
        for (var i = 0; i < radioGroups.length; i++) {
            var radio = radioGroups[i];
            if (radio.value === "7" && radio.checked) {
                var groupId = radio.getAttribute('group-id');
                var remarkTextarea = document.getElementById(`remark_${groupId}`);
                if (!remarkTextarea || remarkTextarea.value.trim() === "") {
                    scrollToElement(remarkTextarea);
                    showErrorAlert('คำเตือน!', 'กรุณาระบุเหตุผล เมื่อไม่ผ่านการอนุมัติแบบมีเงื่อนไข');
                    return;
                }
            }
        }

        if (!mainStatus) {
            showErrorAlert('คำเตือน!', 'กรุณาระบุตัวเลือกอนุมัติหรือไม่อนุมัติ');
            return;
        }

        if (mainStatus.value === "5" && (!remarkElement || remarkElement.value.trim() === "")) {
            scrollToElement(remarkElement);
            showErrorAlert('คำเตือน!', 'กรุณาระบุหมายเหตุ เมื่อไม่อนุมัติงวดงานนี้');
            return;
        }

        //if (_qcStatusID === 1) {
        //    showErrorAlert('คำเตือน!', 'QC ยังตรวจไม่ผ่าน');
        //    return;
        //}

    }

    if (actionType === "submit") {
        showConfirmationAlert(
            'ยืนยันการดำเนินการ',
            'คุณต้องการยืนยันงวดงานนี้หรือไม่',
            'warning',
            'ใช่',
            'ยกเลิก',
            () => performAjaxRequest(actionType)
        );
    } else {
        performAjaxRequest(actionType);
    }
}


function performAjaxRequest(actionType) {
    var data = new FormData();
    data.append("UnitFormID", document.getElementById("UnitFormID").value);
    data.append("ProjectID", document.getElementById("ProjectID").value);
    data.append("UnitID", document.getElementById("UnitID").value);
    data.append("UnitCode", document.getElementById("UnitCode").value);
    data.append("FormID", document.getElementById("FormID").value);
    data.append("ActionType", actionType);

    var mainStatus = document.querySelector('input[name="radios-inline-approval"]:checked');
    if (mainStatus) {
        data.append("UnitFormStatus", mainStatus.value);
    }
    data.append("Remark", document.getElementById("mainRemark").value);

    var files = document.getElementById("file-input").files;
    for (var i = 0; i < files.length; i++) {
        data.append("Images", files[i]);
    }

    var passConditions = [];
    $("input[data-action='gr-pass']").each(function () {
        let group_id = $(this).attr("group-id");
        var remarkElement = document.getElementById(`remark_${group_id}`);
        var PassConditionsElement = document.getElementById(`pass_ID_${group_id}`);
        let PassConditionsval = $(this).is(":checked") ? $(this).val() : null;

        let existingCondition = passConditions.find(pc => pc.Group_ID === group_id);
        if (!existingCondition) {
            passConditions.push({
                PassConditionsID: PassConditionsElement ? PassConditionsElement.value : null,
                PassConditionsvalue: PassConditionsval,
                Group_ID: group_id,
                Remark: remarkElement ? remarkElement.value : '',
            });
        } else {
            if (PassConditionsval !== null) {
                existingCondition.PassConditionsvalue = PassConditionsval;
            }
        }
    });

    if (passConditions.length > 0) {
        data.append("PassConditionsIUD", JSON.stringify(passConditions));
    }

    showLoadingAlert();

    $.ajax({
        url: baseUrl + 'PMApprove/SaveOrSubmit',
        type: 'POST',
        contentType: false,
        processData: false,
        data: data,
        success: function (res) {
            if (res.success) {        
                Swal.close();
                if (mainStatus && mainStatus.value === "4" && actionType === "submit") {
                    const pdfPath = res.pdfPath;
                    window.open(baseUrl + pdfPath, '_blank');
                    showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จและสร้าง PDF สำเร็จ', () => {
                        window.location.reload(); // Reload the page after success
                    });
                } else {
                    Swal.close();
                    showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', () => {
                        window.location.reload();
                    });
                }
            } else {
                Swal.close();
                showErrorAlert('ผิดพลาด!', res.message);
            }
        },
        error: function (xhr, status, error) {
            Swal.close();
            // Display the server's error message
            var errorMessage = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : error;
            showErrorAlert('ผิดพลาด!', errorMessage);
        }
    });
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
            $.ajax({
                url: baseUrl + 'FormCheckList/DeleteImage',
                type: 'POST',
                data: { resourceId: resourceId },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('ลบรูปภาพสำเร็จ', '', 'success');
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

function goToSumaryQC(ProjectId, ProjectName, UnitId) {
    window.open(baseUrl + `SummaryUnitQC/Index?projectId=${ProjectId}&projectName=${ProjectName}&unitId=${UnitId}`, '_blank');
}


function openModalPMPJMComment(UnitFormID, FormID, RoleID) {
    $.ajax({
        url: baseUrl + 'PMApprove/GetDetailCommentPmpjm',
        type: 'GET',
        data: { UnitFormID: UnitFormID, FormID: FormID, RoleID: RoleID },
        success: function (response) {
            if (response) {
                // Populate images
                var imageContainer = $('#image-container-2');
                imageContainer.empty();

                var modalTitle = RoleID === "2" ? "รูปภาพและคอมเม้น PM" : RoleID === "3" ? "รูปภาพและคอมเม้น PJM" : "Images and Text";
                $('#ModalDetailPMPjmLabel').text(modalTitle);

                if (response.images && response.images.length > 0) {
                    response.images.forEach(function (image) {
                        var imageHtml = `
                            <div class="col-6 position-relative">
                                <a data-fslightbox="gallery" href="${baseUrl}${image.FilePath}">
                                    <img src="${baseUrl}${image.FilePath}" alt="Image" class="img-thumbnail">
                                </a>
                            </div>
                        `;
                        imageContainer.append(imageHtml);
                    });

                    refreshFsLightbox();
                } else {
                    // Show "ไม่มีรูปภาพ" if no images are present
                    imageContainer.html('<label class="text-danger">ไม่มีรูปภาพ</label>');
                }

                // Populate other details
                $('#DetailPMPjmName').val(response.Username || "N/A");
                $('#DetailPMPjmDate').val(response.Date || "N/A");
                $('#DetailPMPjmRemark').val(response.Remark || "ไม่มีหมายเหตุ");

                // Show modal
                var myModal = new bootstrap.Modal(document.getElementById('ModalDetailPMPjm'));
                myModal.show();
            }
        },
        error: function (error) {
            console.error('Failed to fetch details:', error);
        }
    });
}
