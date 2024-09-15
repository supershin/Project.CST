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
                        <a data-fslightbox="gallery" href="${image.FilePath}">
                            <img src="${image.FilePath}" alt="..." class="img-thumbnail">
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

function saveOrSubmit(actionType) {
    var remarkElement = document.getElementById("mainRemark");
    var listimagecnt = document.getElementById("listimagecnt").value;
    var files = document.getElementById("file-input").files;
    var mainStatus = document.querySelector('input[name="radios-inline-approval"]:checked');

    // Initialize a flag to check if all groups have at least one choice selected
    var allGroupsChecked = true;
    var radioGroups = {};

    // Collect radio groups by name
    document.querySelectorAll('input[data-action="gr-pass"]').forEach(function (radio) {
        var groupName = radio.getAttribute('name');
        if (!radioGroups[groupName]) {
            radioGroups[groupName] = false; // Initialize as unchecked
        }
        if (radio.checked) {
            radioGroups[groupName] = true; // Mark as checked if any radio in this group is selected
        }
    });

    // Check if all groups have at least one radio selected
    for (var groupName in radioGroups) {
        if (!radioGroups[groupName]) {
            allGroupsChecked = false;
            break;
        }
    }

    if (actionType === "submit") {
        // Validate that all groups have at least one radio selected
        if (!allGroupsChecked) {
            showErrorAlert('คำเตือน!', 'กรุณาเลือกตัวเลือกการตรวจสอบอย่างน้อยหนึ่งรายการในแต่ละกลุ่ม');
            return;
        }

        // Validate conditional remark for radio buttons
        var radioGroups = document.querySelectorAll('input[data-action="gr-pass"]');
        for (var i = 0; i < radioGroups.length; i++) {
            var radio = radioGroups[i];
            if (radio.value === "7" && radio.checked) {
                var groupId = radio.getAttribute('group-id');
                var remarkTextarea = document.getElementById(`remark_${groupId}`);
                if (!remarkTextarea || remarkTextarea.value.trim() === "") {
                    scrollToElement(remarkTextarea); // Scroll to the textarea
                    showErrorAlert('คำเตือน!', 'กรุณาระบุเหตุผล เมื่อไม่ผ่านการอนุมัติแบบมีเงื่อนไข');
                    return;
                }
            }
        }

        // Validate main remark
        if (!mainStatus) {
            showErrorAlert('คำเตือน!', 'กรุณาระบุตัวเลือกอนุมัติหรือไม่อนุมัติ');
            return;
        }

        // Check if the remarkElement exists before accessing its value
        if (mainStatus.value === "5" && (!remarkElement || remarkElement.value.trim() === "")) {
            scrollToElement(remarkElement); 
            showErrorAlert('คำเตือน!', 'กรุณาระบุหมายเหตุ เมื่อไม่อนุมัติงวดงานนี้');           
            return;
        }
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
            Swal.close();
            if (res.success) {
                showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', () => {
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


