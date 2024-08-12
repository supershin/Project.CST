

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

    document.addEventListener("DOMContentLoaded", function () {
        var pcRadio = document.getElementById("pc-radio");

        pcRadio.addEventListener("click", function (e) {
            var allRadios = document.querySelectorAll(".form-check-input[type='radio']:not(#pc-radio)");
            var allChecked = true;

            allRadios.forEach(function (radio) {
                if (radio.value == "1" && !radio.checked) {
                    allChecked = false;
                }
            });

            if (!allChecked) {
                e.preventDefault();
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
                    allRadios.forEach(function (radio) {
                        radio.disabled = false;
                    });
                } else {
                    var radios = document.getElementsByName(pcRadio.name);
                    for (var i = 0; i < radios.length; i++) {
                        radios[i].dataset.wasChecked = "";
                    }
                    pcRadio.dataset.wasChecked = "true";
                    allRadios.forEach(function (radio) {
                        radio.disabled = true;
                    });
                }
            }
        });
    });

//    $('#saveButton').on('click', function () {
//        var packages = [];
//        var checklists = [];
//        var pcCheck = null; // สำหรับเก็บข้อมูล PC Check

//        // Collect data for each package
//        $('div.container-xl.mb-3').each(function () {
//            var UserName = $(this).find('input[id="UserName"]').val();
//            var UserRole = $(this).find('input[id="UserRole"]').val();
//            var projectId = $(this).find('input[id="hd_projectId_IUD"]').val();
//            var unitId = $(this).find('input[id="hd_unitId_IUD"]').val();
//            var UnitFormID = $(this).find('input[id="UnitFormID"]').val() || null;
//            var UnitFormActionID = $(this).find('input[id="UnitFormActionID"]').val() || -99;
//            var package_id = $(this).find('input[id="PackagesID"]').val();
//            var Unitpackage_id = $(this).find('input[id="UnitPackagesID"]').val() || -99;
//            var group_id = $(this).find('input[data-action="chk-check-list"]').first().attr('group-id');
//            var form_id = $(this).find('input[data-action="chk-check-list"]').first().attr('form-id');
//            var remark = $(this).find('textarea.form-control').val();

//            // Check if the necessary fields are not empty
//            if (UserName && UserRole && projectId && unitId && form_id && group_id && package_id) {
//                var package = {
//                    UserName: UserName,
//                    UserRole: UserRole,
//                    ProjectId: projectId,
//                    UnitId: unitId,
//                    UnitFormID: UnitFormID,
//                    UnitFormActionID: UnitFormActionID,
//                    UnitPackageID: Unitpackage_id,
//                    FormID: form_id,
//                    GroupID: group_id,
//                    PackageID: package_id,
//                    Remark: remark
//                };
//                packages.push(package);

//                // Collect data for each checklist
//                $(this).find('div.card.card-link.card-link-pop').each(function () {
//                    var checklist_id = $(this).find('input[data-action="chk-check-list"]').first().attr('check-list-id');
//                    var Unit_checklist_id = $(this).find('input[data-action="chk-check-list"]').first().attr('Unit-check-list-id') || -99;
//                    var radio_value = $(this).find('input[data-action="chk-check-list"]:checked').val() || 0;

//                    // Check if the necessary fields are not empty
//                    if (checklist_id) {
//                        var checklist = {
//                            FormID: form_id,
//                            GroupID: group_id,
//                            PackageID: package_id,
//                            UnitPackageID: Unitpackage_id,
//                            CheckListID: checklist_id,
//                            UnitChecklistID: Unit_checklist_id,
//                            RadioValue: radio_value
//                        };
//                        checklists.push(checklist);
//                    }
//                });
//            }

//            // ตรวจสอบว่า PC Radio ถูกเช็คหรือไม่
//            var pcRadioChecked = $('#pc-radio').is(':checked');
//            if (pcRadioChecked) {
//                var pcRemark = $('#Remark-PC').val();
//                pcCheck = {
//                    UnitFormID: packages[0]?.UnitFormID,
//                    GroupID: packages[0]?.GroupID,
//                    Remark: pcRemark
//                };
//            }
//        });

//        var data = {
//            Packages: packages,
//            CheckLists: checklists,
//            PcCheck: pcCheck // เพิ่มข้อมูล PC Check ใน data
//        };

///*        console.log(data);*/

//        $.ajax({
//            url: baseUrl + 'FormCheckList/UpdateStatusV1',
//            type: 'POST',
//            dataType: 'json',
//            data: data,
//            success: function (res) {
//                if (res.success) {
//                    Swal.fire({
//                        title: 'Success!',
//                        text: 'บันทึกข้อมูลสำเร็จ',
//                        icon: 'success',
//                        confirmButtonText: 'OK'
//                    }).then((result) => {
//                        if (result.isConfirmed) {
//                            window.location.reload();
//                        }
//                    });
//                } else {
//                    Swal.fire({
//                        title: 'Error!',
//                        text: 'บันทึกข้อมูลไม่สำเร็จ',
//                        icon: 'error',
//                        confirmButtonText: 'OK'
//                    });
//                }
//            },
//            error: function (xhr, status, error) {
//                Swal.fire({
//                    title: 'Error!',
//                    text: 'An error occurred while saving the data.',
//                    icon: 'error',
//                    confirmButtonText: 'OK'
//                });
//            }
//        });


//        return false;
//    });

$('#saveButton').on('click', function () {
    var packages = [];
    var checklists = [];
    var pcCheck = null;

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

    var pcRadioChecked = $('#pc-radio').is(':checked');
    if (pcRadioChecked) {
        pcCheck = {
            UnitFormID: packages[0]?.UnitFormID,
            GroupID: packages[0]?.GroupID,
            Remark: $('#Remark-PC').val(),
            IsChecked: true
        };
    }

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

    // Append images directly under the FormChecklistIUDModel
    var files = $('#file-input')[0].files;
    for (var i = 0; i < files.length; i++) {
        data.append('Images', files[i]);
    }

    console.log(data);

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

    return false;
});



function openModal() {
    var myModal = new bootstrap.Modal(document.getElementById('exampleModal'));
    myModal.show();
}

//function deleteImage(fileName) {
//    // Show a confirmation dialog using SweetAlert2
//    Swal.fire({
//        title: 'Are you sure?',
//        text: "Do you really want to delete this image?",
//        icon: 'warning',
//        showCancelButton: true,
//        confirmButtonColor: '#d33',
//        cancelButtonColor: '#3085d6',
//        confirmButtonText: 'Yes, delete it!',
//        cancelButtonText: 'Cancel'
//    }).then((result) => {
//        if (result.isConfirmed) {
//            // Simulate success for now (replace with actual AJAX call later)
//            Swal.fire(
//                'Deleted!',
//                'The image has been deleted.',
//                'success'
//            );

//            // Remove the image from the DOM
//            $(`[onclick="deleteImage('${fileName}')"]`).closest('.position-relative').remove();
//        }
//    });
//}

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





