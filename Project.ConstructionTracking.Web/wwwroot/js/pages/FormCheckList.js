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
                        e.stopPropagation(); 
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

        fileInput.value = '';

        const dataTransfer = new DataTransfer();
        filesArray.forEach(file => dataTransfer.items.add(file));
        fileInput.files = dataTransfer.files;
    }
});

document.addEventListener("DOMContentLoaded", function () {
    var pcRadio = document.getElementById("pc-radio");
    var remarkPC = document.getElementById("Remark-PC");
    var PM_StatusActionType = document.getElementById("PM_StatusActionType").value;
    var UnitFormStatusIDchk = document.getElementById("UnitFormStatusIDchk").value;
    var PC_StatusID = document.getElementById("PC_StatusID").value;

    function disableAllRadios() {
        var allRadios = document.querySelectorAll(".form-check-input[type='radio']:not(#pc-radio)");
        allRadios.forEach(function (radio) {
            radio.disabled = true;
        });
    }

    function enableAllRadios() {
        var allRadios = document.querySelectorAll(".form-check-input[type='radio']:not(#pc-radio)");
        allRadios.forEach(function (radio) {
            radio.disabled = false;
        });
    }

    function toggleRemarkPC() {
        if (pcRadio.checked) {
            remarkPC.disabled = false;
        } else {
            remarkPC.disabled = true;
        }
    }

    pcRadio.addEventListener("click", function (e) {
        var allRadios = document.querySelectorAll(".form-check-input[type='radio']:not(#pc-radio)");
        var groups = {};
        var validSelection = true;

        allRadios.forEach(function (radio) {
            if (!groups[radio.name]) {
                groups[radio.name] = {
                    selected: false,
                    invalid: false
                };
            }

            if (radio.checked) {
                groups[radio.name].selected = true;

                if (radio.value == "10") {
                    groups[radio.name].invalid = true;
                }
            }
        });

        for (var groupName in groups) {
            if (!groups[groupName].selected || groups[groupName].invalid) {
                validSelection = false;
                break;
            }
        }

        if (!validSelection) {
            e.preventDefault();
            pcRadio.checked = false; 
            showErrorAlert('Warning!', 'กรุณาเลือกผ่านหรือไม่ผ่านในทุกรายการ');
        } else {
            if (pcRadio.dataset.wasChecked) {
                pcRadio.checked = false;
                pcRadio.dataset.wasChecked = "";
                enableAllRadios();
            } else {
                var radios = document.getElementsByName(pcRadio.name);
                for (var i = 0; i < radios.length; i++) {
                    radios[i].dataset.wasChecked = "";
                }
                pcRadio.dataset.wasChecked = "true";
                disableAllRadios();
            }
        }
        toggleRemarkPC();
    });

    if (pcRadio.checked || userRole !== "1") {
        disableAllRadios();
    } else {
        enableAllRadios();
    }

    toggleRemarkPC();

    if (PM_StatusActionType === "submit" || userRole !== "1") {
        if (["2", "3", "4", "6", "7", "9", "10", "11", "12"].includes(UnitFormStatusIDchk) || PC_StatusID === "8" || userRole !== "1") {
            document.querySelectorAll('textarea[data-package-id]').forEach(function (textarea) {
                textarea.disabled = true;
            });
            disableAllRadios();
            pcRadio.disabled = true;
            remarkPC.disabled = true;
        }
    }
});


$('#saveButton').on('click', function () {
    $(this).prop('disabled', true);

    var packages = [];
    var checklists = [];
    var pcCheck = null;
    var valid = true;

    var files = $('#file-input')[0].files;
    var imagpreview = parseInt($('#hd_imageListcnt').val(), 10) || 0;

    if (files.length === 0 && imagpreview === 0) {
        showErrorAlert('คำเตือน!', 'กรุณาอัปโหลดรูปภาพอย่างน้อยหนึ่งรูปก่อนบันทึก.');
        valid = false;
    }

    $('div.container-xl.mb-3').each(function (index) {
        var package = {
            UserName: $(this).find('input[id="UserName"]').val(),
            UserRole: $(this).find('input[id="UserRole"]').val(),
            ProjectId: $(this).find('input[id="hd_projectId_IUD"]').val(),
            UnitId: $(this).find('input[id="hd_unitId_IUD"]').val(),
            UnitFormID: $(this).find('input[id="UnitFormID"]').val() || null,
            UnitFormActionID: $(this).find('input[id="UnitFormActionID"]').val() || -99,
            UnitPackageID: $(this).find('input[id="UnitPackagesID"]').val() || -99,
            FormID: $(this).find('input[data-action="chk-check-list"]').first().attr('form-id'),
            GroupID: $(this).find('input[data-action="chk-check-list"]').first().attr('group-id'),
            PackageID: $(this).find('input[id="PackagesID"]').val(),
            Remark: $(this).find('textarea.form-control').val()
        };

        if (package.UserName && package.UserRole && package.ProjectId && package.UnitId && package.FormID && package.GroupID && package.PackageID) {
            packages.push(package);

            $(this).find('div.card.card-link.card-link-pop').each(function () {
                var checklist = {
                    FormID: package.FormID,
                    GroupID: package.GroupID,
                    PackageID: package.PackageID,
                    UnitPackageID: package.UnitPackageID,
                    CheckListID: $(this).find('input[data-action="chk-check-list"]').first().attr('check-list-id'),
                    UnitChecklistID: $(this).find('input[data-action="chk-check-list"]').first().attr('Unit-check-list-id') || -99,
                    RadioValue: $(this).find('input[data-action="chk-check-list"]:checked').val() || 0
                };

                if (checklist.CheckListID) {
                    checklists.push(checklist);
                }
            });
        }
    });

    if (packages.length === 0) {
        showErrorAlert('คำเตือน!', 'กรุณากรอกข้อมูลในทุกฟิลด์ที่จำเป็นก่อนบันทึก.');
        valid = false;
    }

    var pcRadioChecked = $('#pc-radio').is(':checked');
    if (pcRadioChecked) {
        var remarkPC = $('#Remark-PC').val();
        if (!remarkPC) {
            showErrorAlert('คำเตือน!', 'กรุณาระบุเหตุผลเพื่อขออนุมัติการผ่านแบบมีเงื่อนไข.');
            valid = false;
        } else {
            pcCheck = {
                UnitFormID: packages[0]?.UnitFormID,
                GroupID: packages[0]?.GroupID,
                Remark: remarkPC,
                IsChecked: true
            };
        }
    }

    if (valid) {
        showLoadingAlert();

        var data = new FormData();

        packages.forEach((pkg, index) => {
            for (const key in pkg) {
                data.append(`Packages[${index}].${key}`, pkg[key]);
            }
        });

        checklists.forEach((checklist, index) => {
            for (const key in checklist) {
                data.append(`CheckLists[${index}].${key}`, checklist[key]);
            }
        });

        if (pcCheck) {
            for (const key in pcCheck) {
                data.append(`PcCheck.${key}`, pcCheck[key]);
            }
        }

        for (var i = 0; i < files.length; i++) {
            data.append('Images', files[i]);
        }

        $.ajax({
            url: baseUrl + 'FormCheckList/UpdateStatusV1',
            type: 'POST',
            contentType: false,
            processData: false,
            data: data,
            success: function (res) {
                Swal.close();
                if (res.success) {
                    showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', function () {
                        window.location.reload();
                    });
                } else {
                    showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
                }
                $('#saveButton').prop('disabled', true);
            },
            error: function (xhr, status, error) {
                Swal.close();
                showErrorAlert('ผิดพลาด!', 'บันทึกข้อมูลไม่สำเร็จ');
                $('#saveButton').prop('disabled', true);
            }
        });
    } else {
        $('#saveButton').prop('disabled', false);
    }

    return false;
});

function deleteImage(resourceId) {
    showConfirmationAlert(
        '',
        "ต้องการลบรูปภาพหรือไม่?",
        'warning',
        'ลบรูปภาพ',
        'ยกเลิก',
        () => {

            showLoadingAlert('กำลังลบรูปภาพ...', 'กรุณารอสักครู่');

            $.ajax({
                url: baseUrl + 'FormCheckList/DeleteImage',
                type: 'POST',
                data: { resourceId: resourceId },
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








