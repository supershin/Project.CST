

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

    $('#saveButton').on('click', function () {
        var packages = [];
        var checklists = [];
        var pcCheck = null; // สำหรับเก็บข้อมูล PC Check

        // Collect data for each package
        $('div.container-xl.mb-3').each(function () {
            var UserName = $(this).find('input[id="UserName"]').val();
            var UserRole = $(this).find('input[id="UserRole"]').val();
            var projectId = $(this).find('input[id="hd_projectId_IUD"]').val();
            var unitId = $(this).find('input[id="hd_unitId_IUD"]').val();
            var UnitFormID = $(this).find('input[id="UnitFormID"]').val() || null;
            var UnitFormActionID = $(this).find('input[id="UnitFormActionID"]').val() || -99;
            var package_id = $(this).find('input[id="PackagesID"]').val();
            var Unitpackage_id = $(this).find('input[id="UnitPackagesID"]').val() || -99;
            var group_id = $(this).find('input[data-action="chk-check-list"]').first().attr('group-id');
            var form_id = $(this).find('input[data-action="chk-check-list"]').first().attr('form-id');
            var remark = $(this).find('textarea.form-control').val();

            // Check if the necessary fields are not empty
            if (UserName && UserRole && projectId && unitId && form_id && group_id && package_id) {
                var package = {
                    UserName: UserName,
                    UserRole: UserRole,
                    ProjectId: projectId,
                    UnitId: unitId,
                    UnitFormID: UnitFormID,
                    UnitFormActionID: UnitFormActionID,
                    UnitPackageID: Unitpackage_id,
                    FormID: form_id,
                    GroupID: group_id,
                    PackageID: package_id,
                    Remark: remark
                };
                packages.push(package);

                // Collect data for each checklist
                $(this).find('div.card.card-link.card-link-pop').each(function () {
                    var checklist_id = $(this).find('input[data-action="chk-check-list"]').first().attr('check-list-id');
                    var Unit_checklist_id = $(this).find('input[data-action="chk-check-list"]').first().attr('Unit-check-list-id') || -99;
                    var radio_value = $(this).find('input[data-action="chk-check-list"]:checked').val() || 0;

                    // Check if the necessary fields are not empty
                    if (checklist_id) {
                        var checklist = {
                            FormID: form_id,
                            GroupID: group_id,
                            PackageID: package_id,
                            UnitPackageID: Unitpackage_id,
                            CheckListID: checklist_id,
                            UnitChecklistID: Unit_checklist_id,
                            RadioValue: radio_value
                        };
                        checklists.push(checklist);
                    }
                });
            }

            // ตรวจสอบว่า PC Radio ถูกเช็คหรือไม่
            var pcRadioChecked = $('#pc-radio').is(':checked');
            if (pcRadioChecked) {
                var pcRemark = $('#Remark-PC').val();
                pcCheck = {
                    UnitFormID: packages[0]?.UnitFormID,
                    GroupID: packages[0]?.GroupID,
                    Remark: pcRemark
                };
            }
        });

        var data = {
            Packages: packages,
            CheckLists: checklists,
            PCCheck: pcCheck // เพิ่มข้อมูล PC Check ใน data
        };

        console.log(data);

        //$.ajax({
        //    url: baseUrl + 'FormCheckList/UpdateStatus',
        //    type: 'POST',
        //    dataType: 'json',
        //    data: data,
        //    success: function (res) {
        //        if (res.success) {
        //            alert("Data saved successfully!");
        //            window.location.reload();
        //        } else {
        //            alert("An error occurred: " + res.message);
        //        }
        //    },
        //    error: function (xhr, status, error) {
        //        alert("An error occurred while saving the data.");
        //    }
        //});

        return false;
    });

