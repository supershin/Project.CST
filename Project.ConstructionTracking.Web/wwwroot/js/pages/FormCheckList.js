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

document.addEventListener("DOMContentLoaded", function () {
    var pcRadio = document.getElementById("pc-radio");
    var remarkPC = document.getElementById("Remark-PC");
    var PM_StatusActionType = document.getElementById("PM_StatusActionType").value;
    var UnitFormStatusIDchk = document.getElementById("UnitFormStatusIDchk").value;

    function disableAllRadios() {
        var allRadios = document.querySelectorAll(".form-check-input[type='radio']:not(#pc-radio)");
        allRadios.forEach(function (radio) {
            radio.disabled = true; // Disable all other radios
        });
    }

    function enableAllRadios() {
        var allRadios = document.querySelectorAll(".form-check-input[type='radio']:not(#pc-radio)");
        allRadios.forEach(function (radio) {
            radio.disabled = false; // Enable all radios
        });
    }

    function toggleRemarkPC() {
        if (pcRadio.checked) {
            remarkPC.disabled = false; // Enable Remark-PC when pc-radio is checked
        } else {
            remarkPC.disabled = true; // Disable Remark-PC when pc-radio is not checked
        }
    }

    pcRadio.addEventListener("click", function (e) {
        var allRadios = document.querySelectorAll(".form-check-input[type='radio']:not(#pc-radio)");
        var allChecked = true;

        // Check if all other radios with value="9" are checked
        allRadios.forEach(function (radio) {
            if (radio.value == "9" && !radio.checked) {
                allChecked = false;
            }
        });

        if (!allChecked) {
            e.preventDefault(); // Prevent the pc-radio from being checked
            pcRadio.checked = false;
            Swal.fire({
                title: 'Warning!',
                text: 'กรุณาเลือกการตรวจให้ผ่านทุกงาน',
                icon: 'warning',
                confirmButtonText: 'OK'
            });
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
        toggleRemarkPC(); // Call function to toggle Remark-PC based on pc-radio state
    });

    if (pcRadio.checked || PM_StatusActionType === "submit") {
        disableAllRadios();
    } else {
        enableAllRadios();
    }

    if (PM_StatusActionType === "submit") {
        if (["2", "3", "4", "6", "7", "9"].includes(UnitFormStatusIDchk)) {
            document.querySelectorAll('textarea[data-package-id]').forEach(function (textarea) {
                textarea.disabled = true;
            });
            pcRadio.disabled = true;
            remarkPC.disabled = true; // Ensure Remark-PC is disabled when PM_StatusActionType is submit
        }
    }

    toggleRemarkPC(); // Initial call to set the correct state of Remark-PC on page load
});


$('#saveButton').on('click', function () {
    var packages = [];
    var checklists = [];
    var pcCheck = null;
    var valid = true; // Variable to track validation status

    // Check if both the file input and the preview container are empty
    var files = $('#file-input')[0].files;
    var imagpreview = parseInt($('#hd_imageListcnt').val(), 10) || 0;

    if (files.length === 0 && imagpreview === 0) {
        Swal.fire({
            title: 'Error!',
            text: 'กรุณาอัปโหลดรูปภาพอย่างน้อยหนึ่งรูปก่อนบันทึก.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        valid = false; // Mark the validation as failed
    }

    // Collect data for each package
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

        // Validation to ensure the package is not empty
        if (package.UserName && package.UserRole && package.ProjectId && package.UnitId && package.FormID && package.GroupID && package.PackageID) {
            packages.push(package);

            // Collect data for each checklist
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

    // Validate that at least one package is available
    if (packages.length === 0) {
        Swal.fire({
            title: 'Error!',
            text: 'กรุณากรอกข้อมูลในทุกฟิลด์ที่จำเป็นก่อนบันทึก.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        valid = false; // Mark the validation as failed
    }

    var pcRadioChecked = $('#pc-radio').is(':checked');
    if (pcRadioChecked) {
        var remarkPC = $('#Remark-PC').val();
        if (!remarkPC) {
            Swal.fire({
                title: 'Error!',
                text: 'กรุณาระบุเหตุผลเพื่อขออนุมัติการผ่านแบบมีเงื่อนไข.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            valid = false; // Mark the validation as failed
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
                if (res.success) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'บันทึกข้อมูลสำเร็จ',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: 'บันทึกข้อมูลไม่สำเร็จ',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    title: 'Error!',
                    text: 'An error occurred while saving the data.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
    }

    return false;
});


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
                        Swal.fire({
                            title: 'ลบรูปภาพสำเร็จ',
                            icon: 'success',
                            confirmButtonText: 'OK'
                        }).then(() => {
                            window.location.reload(); // Reload the page after clicking OK
                        });
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






