function adjustTextareaRows() {
    const textareas = document.querySelectorAll('.custom-textarea');
    textareas.forEach(textarea => {
        if (window.innerWidth <= 576) {
            textarea.setAttribute('rows', 3);
        } else {
            ++
                textarea.setAttribute('rows', 2);
        }
    });
}

document.addEventListener('DOMContentLoaded', adjustTextareaRows);

window.addEventListener('resize', adjustTextareaRows);

let selectedRadioQC5Status = '';

window.onload = function () {
    var radios = document.getElementsByName('radios-QC5-status');
    for (var i = 0; i < radios.length; i++) {
        if (radios[i].checked) {
            selectedRadioQC5Status = radios[i].value; // Set the selected value based on the pre-checked radio
            radios[i].dataset.wasChecked = "true"; // Mark it as checked
            break;
        }
    }
};

function toggleRadio(radio) {
    if (radio.checked && radio.dataset.wasChecked) {
        // Uncheck the radio if it's clicked when already checked
        radio.checked = false;
        radio.dataset.wasChecked = "";
        selectedRadioQC5Status = ''; // Unset the selected value
    } else {
        // Uncheck all radios in the same group and set the clicked one as checked
        var radios = document.getElementsByName(radio.name);
        for (var i = 0; i < radios.length; i++) {
            radios[i].dataset.wasChecked = "";
        }
        radio.dataset.wasChecked = "true";
        selectedRadioQC5Status = radio.value; // Set the selected value (radio's value)
    }
}


document.addEventListener('DOMContentLoaded', function () {
    // Initialize the first dropdown
    $('#dropdown1').on('change', function () {
        var selectedDefectAreaId = $(this).val();
        var ddlDefectType = $('#dropdown2'); // Reference to the second dropdown
        var searchTerm = ''; // Optional search term, empty by default

        // Clear existing options in the second and third dropdowns
        ddlDefectType.empty().append('<option value="">กรุณาเลือก</option>').prop('disabled', true);
        $('#dropdown3').empty().append('<option value="">กรุณาเลือก</option>').prop('disabled', true);

        if (selectedDefectAreaId) {
            $.ajax({
                url: baseUrl + 'QC5Check/GetDDLDefectType',
                data: { defectAreaId: selectedDefectAreaId, searchTerm: searchTerm },
                success: function (data) {
                    $.each(data, function (index, item) {
                        ddlDefectType.append($('<option>', { value: item.Value, text: item.Text }));
                    });
                    ddlDefectType.prop('disabled', false); // Enable the second dropdown after loading data
                },
                error: function () {
                    console.log("Error fetching defect types.");
                }
            });
        }
    });

    // When the second dropdown changes
    $('#dropdown2').on('change', function () {
        var selectedDefectTypeId = $(this).val();
        var ddlDefectDescription = $('#dropdown3'); // Reference to the third dropdown
        var searchTerm = ''; // Optional search term, empty by default

        // Clear existing options in the third dropdown
        ddlDefectDescription.empty().append('<option value="">กรุณาเลือก</option>').prop('disabled', true);

        if (selectedDefectTypeId) {
            $.ajax({
                url: baseUrl + 'QC5Check/GetDDLDefectDescription',
                data: { defectTypeId: selectedDefectTypeId, searchTerm: searchTerm },
                success: function (data) {
                    $.each(data, function (index, item) {
                        ddlDefectDescription.append($('<option>', { value: item.Value, text: item.Text }));
                    });
                    ddlDefectDescription.prop('disabled', false); // Enable the third dropdown after loading data
                },
                error: function () {
                    console.log("Error fetching defect descriptions.");
                }
            });
        }
    });
});

function openModalDataQC(action, data = null) {
    var myModal = new bootstrap.Modal(document.getElementById('insert-new-qc5'));
    // Hide the fixed button
    var fixedButton = document.querySelector('.fixedButton');
    fixedButton.style.display = 'none';  // Hide the button

    var modalTitle = document.getElementById('insert-new-qc5-Label');
    if (action === 'add') {
        modalTitle.textContent = 'เพิ่มข้อมูลรายการ';
        //$('#autocomplete1, #autocomplete2, #autocomplete3').val('').prop('disabled', false);
        // Clear and reset dropdowns dynamically
        $('#dropdown1').val('').trigger('change');  // Reset first dropdown to default and trigger change event
        $('#dropdown2').empty().append('<option value="">กรุณาเลือก</option>').prop('disabled', true);  // Reset second dropdown
        $('#dropdown3').empty().append('<option value="">กรุณาเลือก</option>').prop('disabled', true);  // Reset third dropdown
        $('#otherDefectInput, #commentTextarea').val('');
        $('#majorDefectCheckbox').prop('checked', false);
        $('#file-input').val('');
        $('#preview-container').empty();  // Clear preview container
    }

    myModal.show();
}


document.getElementById('saveButton').addEventListener('click', function () {
    onSaveButtonClick();
});

function onSaveButtonClick() {
    var defectAreaId = document.getElementById('dropdown1').value;
    var defectTypeId = document.getElementById('dropdown2').value;
    var defectDescriptionId = document.getElementById('dropdown3').value;
    var comment = document.getElementById('commentTextarea').value;
    var files = $('#file-input')[0].files;
    var isMajorDefect = document.getElementById('majorDefectCheckbox').checked;

    // Get hidden input values
    var projectId = document.getElementById('hdProject').value;
    var unitId = document.getElementById('hdUnitId').value;
    var seq = document.getElementById('hdSeq').value;

    // Validate required fields
    if (!defectAreaId) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาเลือกตำแหน่ง');
        return;
    }
    if (!defectTypeId) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาเลือกหมวดงาน');
        return;
    }
    if (!defectDescriptionId) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาเลือกรายการ defect');
        return;
    }

    // Create FormData object and append data
    var formData = new FormData();
    formData.append('ProjectID', projectId); // Use hidden input value
    formData.append('UnitID', unitId); // Use hidden input value
    formData.append('DefectAreaID', defectAreaId);
    formData.append('DefectTypeID', defectTypeId);
    formData.append('DefectDescriptionID', defectDescriptionId);
    formData.append('Remark', comment);
    formData.append('IsMajorDefect', isMajorDefect);
    formData.append('Seq', seq); // Use hidden input value

    //debugger

    // Append images to FormData
    for (var i = 0; i < files.length; i++) {
        formData.append('Images', files[i]);
    }


    showLoadingAlert();

    $.ajax({
        url: baseUrl + 'QC5Check/SaveDefectData',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            Swal.close();
            if (response.success) {
                var mainPassRadio = document.getElementById('radio3');
                if (mainPassRadio && mainPassRadio.checked) {
                    mainPassRadio.checked = false;
                    mainPassRadio.dataset.wasChecked = "";
                    selectedRadioQC5Status = '';
                }
                //debugger
                //console.log(isMajorDefect);
                if (isMajorDefect === true) {
                    var mainPassWithConditionRadio = document.getElementById('radio2');
                    mainPassWithConditionRadio.checked = false;
                    mainPassWithConditionRadio.dataset.wasChecked = "";
                    selectedRadioQC5Status = '';
                }

                var mainNotpassRadio = document.getElementById('radio1');
                mainNotpassRadio.checked = true;
                //mainNotpassRadio.dataset.wasChecked = "check";

                //debugger
                showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', function () {
                    const clearButton = document.getElementById('clearnewinsertdefect');
                    clearButton.click();
                    showFixedButton();
                    fetchUpdatedList();
                    fetchUpdatedSummary();
                });
            } else {
                showErrorAlert('บันทึกข้อมูลไม่สำเร็จ', response.message || 'เกิดข้อผิดพลาดในการบันทึกข้อมูล');
            }
        },
        error: function (xhr, status, error) {
            Swal.close();
            showErrorAlert('เกิดข้อผิดพลาด!', error);
        }
    });
}


document.addEventListener("DOMContentLoaded", function () {
    var dropZone = document.getElementById("drop-zone");
    var fileInput = document.getElementById("file-input");
    var previewContainer = document.getElementById("preview-container");
    var filesArray = [];

    // Set the max file count limit
    const MAX_FILE_COUNT = 5;

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
            if (filesArray.length < MAX_FILE_COUNT) {
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

    document.getElementById('clearnewinsertdefect').addEventListener('click', function () {
        previewContainer.innerHTML = '';
        filesArray = [];
        updateFileInput();
    });

    document.getElementById('clearcancelnewinsertdefect').addEventListener('click', function () {
        previewContainer.innerHTML = '';
        filesArray = [];
        updateFileInput();
    });

});

function filterCards() {
    var searchInput = document.getElementById('searchInput').value.toLowerCase();
    var statusFilter = document.getElementById('statusFilter').value;

    var cards = document.querySelectorAll('.card-item');

    cards.forEach(function (card) {
        var refseqText = card.getAttribute('data-refseq').toLowerCase();
        var defectText = card.getAttribute('data-defect').toLowerCase();
        var remarkText = card.getAttribute('data-remark').toLowerCase();
        var cardStatus = card.getAttribute('data-status');

        // Check if the card matches the search term and the selected filter
        var matchesSearch = refseqText.includes(searchInput) || defectText.includes(searchInput) || remarkText.includes(searchInput);
        var matchesFilter = !statusFilter || cardStatus === statusFilter;

        // Show or hide the card based on the search and filter conditions
        if (matchesSearch && matchesFilter) {
            card.style.display = 'block';
        } else {
            card.style.display = 'none';
        }
    });
}

function openModalEditQC(defectID) {
    $.ajax({
        url: baseUrl + 'QC5Check/GetQC5DefactEdit',
        type: 'GET',
        data: { DefectID: defectID },
        success: function (response) {
            if (response) {

                // Hide the fixed button
                var fixedButton = document.querySelector('.fixedButton');
                if (fixedButton) {
                    // Only try to hide the button if it exists
                    fixedButton.style.display = 'none';
                    clearFileInputAndPreview()
                }

                // Populate the dropdowns and other fields
                $('#dropdown1Edit').val(response.DefectAreaID).trigger('change');

                // AJAX call for defect type and description
                $.ajax({
                    url: baseUrl + 'QC5Check/GetDDLDefectType',
                    data: { defectAreaId: response.DefectAreaID },
                    success: function (data) {
                        var ddlDefectType = $('#dropdown2Edit');
                        ddlDefectType.empty().append('<option value="">กรุณาเลือก</option>');
                        $.each(data, function (index, item) {
                            ddlDefectType.append($('<option>', { value: item.Value, text: item.Text }));
                        });
                        ddlDefectType.val(response.DefectTypeID).prop('disabled', false);

                        $.ajax({
                            url: baseUrl + 'QC5Check/GetDDLDefectDescription',
                            data: { defectTypeId: response.DefectTypeID },
                            success: function (data) {
                                var ddlDefectDescription = $('#dropdown3Edit');
                                ddlDefectDescription.empty().append('<option value="">กรุณาเลือก</option>');
                                $.each(data, function (index, item) {
                                    ddlDefectDescription.append($('<option>', { value: item.Value, text: item.Text }));
                                });
                                ddlDefectDescription.val(response.DefectDescriptionID).prop('disabled', false);
                            },
                            error: function () {
                                console.log('Error fetching defect descriptions.');
                            }
                        });
                    },
                    error: function () {
                        console.log('Error fetching defect types.');
                    }
                });

                // Populate other fields in the modal
                $('#commentTextareaEdit').val(response.Remark).prop('disabled', actionTypeEn === "submit");
                $('#QC5DefectID').val(response.DefectID);
                $('#majorDefectCheckboxEdit').prop('checked', response.IsMajorDefect).prop('disabled', actionTypeEn === "submit");

                // Populate existing images in the modal
                $('#imagePreview2').empty();
                if (response.listImageNotpass && response.listImageNotpass.length > 0) {
                    response.listImageNotpass.forEach(function (image) {
                        let removeButtonHTML = actionTypeEn !== "submit" ? `<button type="button" class="remove-button" onclick="RemoveImage('${image.ResourceID}')">✖</button>` : '';
                        $('#imagePreview2').append(`
                            <div class="position-relative d-inline-block mb-3">
                                <a data-fslightbox="gallery" href="${baseUrl + image.FilePath}">
                                    <img src="${baseUrl + image.FilePath}" alt="รูปภาพ Defact" class="img-thumbnail" style="width: 90px; height: 90px; border-radius: 50%; object-fit: cover;">
                                </a>
                                ${removeButtonHTML}
                            </div>
                        `);
                    });
                    refreshFsLightbox();
                }

                // Hide or show dropzone based on the number of images
                if ($('#imagePreview2 .position-relative').length >= 5) {
                    $('#drop-zone-edit').hide();
                } else {
                    $('#drop-zone-edit').show();
                }

                // Show the modal
                var myModal = new bootstrap.Modal(document.getElementById('Edit-qc5'));
                myModal.show();
            }
        },
        error: function () {
            alert('Error fetching data. Please try again.');
        }
    });
}


function clearFileInputAndPreview() {

    // Reference the specific file input and preview container for the 'edit' modal
    var fileInputEdit = document.getElementById("file-input-edit");
    var previewContainerEdit = document.getElementById("preview-container-edit");

    // Reset filesArray
    filesArray = [];
    // Clear the file input for the dropzone
    fileInputEdit.value = '';
    // Clear the preview container
    previewContainerEdit.innerHTML = '';


    //// Reset the files array
    //filesArray = [];

    //// Clear the file input field
    //$('#file-input-edit').val('');  // This clears the input field

    //// Clear the file input files list using DataTransfer
    //var dataTransfer = new DataTransfer();
    //$('#file-input-edit')[0].files = dataTransfer.files;  // Reset file input

    //// Clear the preview container
    //$('#preview-container-edit').empty();  // Clear any previewed images
}


document.addEventListener('DOMContentLoaded', function () {
    // Initialize the first dropdown (Dropdown1Edit)
    $('#dropdown1Edit').on('change', function () {
        var selectedDefectAreaId = $(this).val();
        var ddlDefectType = $('#dropdown2Edit'); // Reference to the second dropdown (Dropdown2Edit)
        var searchTerm = ''; // Optional search term, empty by default

        // Clear existing options in the second and third dropdowns
        ddlDefectType.empty().append('<option value="">กรุณาเลือก</option>').prop('disabled', true);
        $('#dropdown3Edit').empty().append('<option value="">กรุณาเลือก</option>').prop('disabled', true);

        if (selectedDefectAreaId) {
            // Fetch Defect Types based on selected Defect Area (Dropdown1Edit)
            $.ajax({
                url: baseUrl + 'QC5Check/GetDDLDefectType', // The controller action for fetching defect types
                data: { defectAreaId: selectedDefectAreaId, searchTerm: searchTerm },
                success: function (data) {
                    $.each(data, function (index, item) {
                        ddlDefectType.append($('<option>', { value: item.Value, text: item.Text }));
                    });
                    ddlDefectType.prop('disabled', false); // Enable the second dropdown after loading data
                },
                error: function () {
                    console.log("Error fetching defect types.");
                }
            });
        }
    });

    // Initialize the second dropdown (Dropdown2Edit)
    $('#dropdown2Edit').on('change', function () {
        var selectedDefectTypeId = $(this).val();
        var ddlDefectDescription = $('#dropdown3Edit'); // Reference to the third dropdown (Dropdown3Edit)
        var searchTerm = ''; // Optional search term, empty by default

        // Clear existing options in the third dropdown
        ddlDefectDescription.empty().append('<option value="">กรุณาเลือก</option>').prop('disabled', true);

        if (selectedDefectTypeId) {
            // Fetch Defect Descriptions based on selected Defect Type (Dropdown2Edit)
            $.ajax({
                url: baseUrl + 'QC5Check/GetDDLDefectDescription', // The controller action for fetching defect descriptions
                data: { defectTypeId: selectedDefectTypeId, searchTerm: searchTerm },
                success: function (data) {
                    $.each(data, function (index, item) {
                        ddlDefectDescription.append($('<option>', { value: item.Value, text: item.Text }));
                    });
                    ddlDefectDescription.prop('disabled', false); // Enable the third dropdown after loading data
                },
                error: function () {
                    console.log("Error fetching defect descriptions.");
                }
            });
        }
    });
});


document.getElementById('EditButton').addEventListener('click', function () {
    onEditButtonClick();
});

function onEditButtonClick() {
    var DefectID = document.getElementById('QC5DefectID').value;
    var defectAreaId = document.getElementById('dropdown1Edit').value;
    var defectTypeId = document.getElementById('dropdown2Edit').value;
    var defectDescriptionId = document.getElementById('dropdown3Edit').value;
    var comment = document.getElementById('commentTextareaEdit').value;
    var files = $('#file-input-edit')[0].files;
    var isMajorDefect = document.getElementById('majorDefectCheckboxEdit').checked;

    var projectId = document.getElementById('hdProject').value;
    var unitId = document.getElementById('hdUnitId').value;
    var seq = document.getElementById('hdSeq').value;


    if (!defectAreaId) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาเลือกตำแหน่ง');
        return;
    }
    if (!defectTypeId) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาเลือกหมวดงาน');
        return;
    }
    if (!defectDescriptionId) {
        showErrorAlertNotCloseModal('คำเตือน!', 'กรุณาเลือกรายการ defect');
        return;
    }


    var oldImagesCount = $('#imagePreview2 .position-relative').length;
    var newImagesCount = files.length;
    var totalImagesCount = oldImagesCount + newImagesCount;


    if (totalImagesCount > 5) {
        showErrorAlertNotCloseModal('คำเตือน!', 'คุณสามารถอัปโหลดรูปภาพได้ไม่เกิน 5 รูปต่อรายการ');
        return;
    }

    var formData = new FormData();
    formData.append('ID', DefectID);
    formData.append('ProjectID', projectId);
    formData.append('UnitID', unitId);
    formData.append('DefectAreaID', defectAreaId);
    formData.append('DefectTypeID', defectTypeId);
    formData.append('DefectDescriptionID', defectDescriptionId);
    formData.append('Remark', comment);
    formData.append('IsMajorDefect', isMajorDefect);
    formData.append('Seq', seq);

    for (var i = 0; i < files.length; i++) {
        formData.append('Images', files[i]);
    }

    showLoadingAlert();

    $.ajax({
        url: baseUrl + 'QC5Check/EditDefectData',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            Swal.close();
            if (response.success) {
                if (isMajorDefect === true) {
                    var mainPassWithConditionRadio = document.getElementById('radio2');
                    mainPassWithConditionRadio.checked = false;
                    mainPassWithConditionRadio.dataset.wasChecked = "";
                    selectedRadioQC5Status = '';
                }
                showFixedButton();
                fetchUpdatedList();
                showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ');
                const clearButton = document.getElementById('clearAllButton');
                clearButton.click();
            } else {
                showErrorAlertNotCloseModal('บันทึกข้อมูลไม่สำเร็จ', response.message || 'เกิดข้อผิดพลาดในการบันทึกข้อมูล');
            }
        },
        error: function (xhr, status, error) {
            Swal.close();
            showErrorAlertNotCloseModal('เกิดข้อผิดพลาด!', error);
        }
    });
}

document.addEventListener("DOMContentLoaded", function () {
    var dropZone = document.getElementById("drop-zone-edit");
    var fileInput = document.getElementById("file-input-edit");
    var previewContainer = document.getElementById("preview-container-edit");
    var filesArray = []; // Array to keep track of all files
    const MAX_FILE_COUNT = 5;

    // When the drop zone is clicked, trigger the file input
    dropZone.addEventListener("click", function (e) {
        if (!e.target.classList.contains("remove-button")) {
            fileInput.click();
        }
    });

    // Handle file selection via file input
    fileInput.addEventListener("change", function () {
        if (fileInput.files.length) {
            addFilesToPreview(fileInput.files);
        }
    });

    // Handle drag and drop
    dropZone.addEventListener("dragover", function (e) {
        e.preventDefault();
        dropZone.classList.add("drop-zone--over");
    });

    dropZone.addEventListener("dragleave", function () {
        dropZone.classList.remove("drop-zone--over");
    });

    dropZone.addEventListener("drop", function (e) {
        e.preventDefault();
        dropZone.classList.remove("drop-zone--over");
        addFilesToPreview(e.dataTransfer.files);
    });

    // Add files to the preview and handle file removal
    function addFilesToPreview(files) {
        Array.from(files).forEach(file => {
            if (filesArray.length < MAX_FILE_COUNT) {
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

    document.getElementById('clearAllButton').addEventListener('click', function () {
        previewContainer.innerHTML = '';
        filesArray = [];
        updateFileInput();
    });

});

function RemoveImage(resourceID) {
    // Confirmation alert before proceeding
    showConfirmationAlert(
        'ยืนยันการลบรูปภาพ',
        'คุณต้องการลบรูปภาพนี้ใช่หรือไม่?',
        'warning',
        'ใช่',
        'ยกเลิก',
        function () {
            // If confirmed, proceed with image removal
            showLoadingAlert();

            $.ajax({
                url: baseUrl + 'QC5Check/RemoveImage',
                type: 'POST',
                data: { resourceID: resourceID },
                success: function (response) {
                    Swal.close(); // Close the loading indicator
                    if (response.success) {
                        // Remove the image container
                        $(`button[onclick="RemoveImage('${resourceID}')"]`).parent().remove();

                        // Show dropzone if less than 5 images are present
                        if ($('#imagePreview2 .position-relative').length < 5) {
                            $('#drop-zone-edit').show();
                        }

                        showSuccessAlert('สำเร็จ!', 'ลบรูปภาพสำเร็จ');
                    } else {
                        showErrorAlert('เกิดข้อผิดพลาด!', 'ไม่สามารถลบรูปภาพได้');
                    }
                },
                error: function () {
                    Swal.close();  // Close the loading indicator if the request fails
                    showErrorAlert('เกิดข้อผิดพลาด!', 'การลบรูปภาพล้มเหลว');
                }
            });
        }
    );
}


document.getElementById('RemoveQC5Button').addEventListener('click', function () {
    onRemoveQC5ButtonClick();
});

function onRemoveQC5ButtonClick() {
    var DefectID = document.getElementById('QC5DefectID').value;
    var projectId = document.getElementById('hdProject').value;
    var unitId = document.getElementById('hdUnitId').value;
    var seq = document.getElementById('hdSeq').value;

    var formData = new FormData();
    formData.append('ID', DefectID);
    formData.append('ProjectID', projectId);
    formData.append('UnitID', unitId);
    formData.append('Seq', seq);

    // Confirmation alert before proceeding
    showConfirmationAlert(
        'ยืนยันการลบข้อมูล', // Confirm deletion
        'คุณต้องการลบข้อมูลนี้ใช่หรือไม่?', // Do you want to delete this data?
        'warning',
        'ใช่',  // Yes
        'ยกเลิก',  // Cancel
        function () {
            // If confirmed, proceed with data removal
            showLoadingAlert();

            $.ajax({
                url: baseUrl + 'QC5Check/RemoveDefectQC5Unit',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    Swal.close(); // Close loading indicator
                    if (response.success) {
                        // Success alert and reload the page
                        showSuccessAlert('สำเร็จ!', 'ลบข้อมูลสำเร็จ', function () {
                            fetchUpdatedList();
                        });
                    } else {
                        showErrorAlert('ลบข้อมูลไม่สำเร็จ', response.message || 'เกิดข้อผิดพลาดในการลบข้อมูล');
                    }
                },
                error: function (xhr, status, error) {
                    Swal.close(); // Close loading indicator on error
                    showErrorAlert('เกิดข้อผิดพลาด!', error);
                }
            });
        }
    );
}


let signaturePad;

var unitEquipment = {

    init: () => {
        unitEquipment.initSignaturePad();
    },

    initSignaturePad: () => {
        let canvas = $("#signature-pad canvas");
        let parentWidth = $(canvas).parent().outerWidth();
        let parentHeight = $(canvas).parent().outerHeight();
        canvas.attr("width", parentWidth + 'px').attr("height", parentHeight + 'px');
        signaturePad = new SignaturePad(canvas[0], {
            backgroundColor: 'rgb(255, 255, 255)'
        });

        $(document).on('click', '.clear', function () {
            signaturePad.clear();
        });
    },

    getSignatureData: () => {
        let dataURL;
        let contentType;
        let storage;
        if (!signaturePad.isEmpty()) {
            dataURL = signaturePad.toDataURL();
            var parts = dataURL.split(';base64,');
            contentType = parts[0].split(":")[1];
            storage = parts[1];
        }
        return {
            MimeType: contentType,
            StorageBase64: storage
        };
    },
};

document.addEventListener("DOMContentLoaded", function () {
    var dropZone = document.getElementById("drop-zone-save-submit");
    var fileInput = document.getElementById("file-input-save-submit");
    var previewContainer = document.getElementById("preview-container-save-submit");
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

function saveUnitQC5() {

    var QCUnitCheckListID = document.getElementById('hdQC5UnitChecklistID').value;
    var QCUnitCheckListActionID = document.getElementById('hdQC5UnitChecklistActionID').value;
    var QCStatusID = selectedRadioQC5Status;
    var ActionType = 'save';
    var QCRemark = document.getElementById('QC5Remark').value;
    var files = $('#file-input-save-submit')[0].files;

    var formData = new FormData();
    formData.append('QCUnitCheckListID', QCUnitCheckListID);
    formData.append('QCUnitCheckListActionID', QCUnitCheckListActionID);
    formData.append('QCStatusID', QCStatusID);
    formData.append('ActionType', ActionType);
    formData.append('QCRemark', QCRemark);


    for (var i = 0; i < files.length; i++) {
        formData.append('Images', files[i]);
    }

    showLoadingAlert('กำลังบันทึก...', 'กรุณารอสักครู่');

    $.ajax({
        url: baseUrl + 'QC5Check/SaveSubmitQC5UnitCheckList',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            Swal.close();
            if (res.success) {
                showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', function () {
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

function openSignatureModal(signatureImagePath, signatureDate) {
    // Update the image source and signature date dynamically
    document.getElementById('signatureImage').src = baseUrl + signatureImagePath;
    document.getElementById('signatureDate').textContent = 'ลงลายเซ็นวันที่ : ' + signatureDate;

    // Show the modal
    var myModal = new bootstrap.Modal(document.getElementById('signatureModal'), {
        keyboard: false
    });
    myModal.show();
}

function saveSignature() {
    showLoadingAlert('กำลังบันทึก...', 'กรุณารอสักครู่');

    var QCUnitCheckListID = document.getElementById('hdQC5UnitChecklistID').value;
    var peName = document.getElementById('hdPeName').value;  // Get PEName from the hidden input
    var signData = unitEquipment.getSignatureData();  // Accessing it from unitEquipment object

    var formData = new FormData();
    formData.append('QCUnitCheckListID', QCUnitCheckListID);

    if (signData) {
        var signatureData = {
            MimeType: signData.MimeType,
            StorageBase64: signData.StorageBase64
        };
        formData.append('Sign', JSON.stringify(signatureData));  // Send the signature as a JSON string
    }

    $.ajax({
        url: baseUrl + 'QC5Check/SaveSignature',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            Swal.close();
            if (res.success) {
                // Update the signature image and date dynamically in the card
                $('#cardsignature').html(`
                    <div class="col-12">
                        <input type="text" id="PEUnit" class="form-control" value="PE/SE : ${peName}" readonly>
                    </div>
                    <div style="position: relative; display: inline-block;">
                        <a href="javascript:void(0);" onclick="openSignatureModal('${res.filePath}', '${res.signatureDate}');">
                            <img src="${baseUrl + res.filePath}" alt="Gallery Image 1" class="rounded" style="width:500px;height:360px;">
                            <span class="image-text">ลงลายเซ็นวันที่ : ${res.signatureDate}</span>
                        </a>
                    </div>
                `);

                $('#hdSigNatureData').val(res.filePath);

                // Close the signature modal
                $('#ClosesignatureModal').click();

                // Show success alert
                showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ');
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


function SubmitUnitQC5() {
    // Perform initial validation
    var QCUnitCheckListID = document.getElementById('hdQC5UnitChecklistID').value;
    var QCUnitCheckListActionID = document.getElementById('hdQC5UnitChecklistActionID').value;
    var QCStatusID = selectedRadioQC5Status;
    var QCRemark = document.getElementById('QC5Remark').value;
    var files = $('#file-input-save-submit')[0].files;
    var SigNatureData = document.getElementById('hdSigNatureData').value;
    var ChkPEUnit = document.getElementById('hdPEUnit').value;

    if (!ChkPEUnit) {
        showErrorAlert('คำเตือน!', 'Unit นี้ยังไม่ได้ระบุวิศกรควบคุมงาน');
        return;
    }

    /*    debugger*/

    if (!QCStatusID) {
        showErrorAlert('คำเตือน!', 'กรุณาเลือกสถานะของ QC รอบนี้');
        return;
    }

    if (!SigNatureData) {
        showErrorAlert('คำเตือน!', 'กรุณาระบุลายเซ็น');
        return;
    }

    if (QCStatusID === "3") {
        // Access ImageQC5UnitList which was declared in the Razor view
        if (ImageQC5UnitList.length + files.length === 0) {
            showErrorAlert('คำเตือน!', 'กรุณาเลือกเพิ่มรูปภาพและเหตุผลที่ไม่ให้ผ่าน');
            return;
        }
        else if (!QCRemark) {
            showErrorAlert('คำเตือน!', 'กรุณาเลือกเพิ่มรูปภาพและเหตุผลที่ไม่ให้ผ่าน');
            return;
        }
    }


    // Confirmation alert before proceeding
    showConfirmationAlert(
        'ยืนยันการดำเนินการ',
        'คุณต้องการยืนยันข้อมูลนี้ใช่หรือไม่?',
        'warning',
        'ใช่',
        'ยกเลิก',
        function () {
            // If confirmed, proceed with the submission
            showLoadingAlert('กำลังบันทึก...', 'กรุณารอสักครู่');

            var formData = new FormData();
            formData.append('QCUnitCheckListID', QCUnitCheckListID);
            formData.append('QCUnitCheckListActionID', QCUnitCheckListActionID);
            formData.append('QCStatusID', QCStatusID);
            formData.append('ActionType', 'submit');
            formData.append('QCRemark', QCRemark);

            for (var i = 0; i < files.length; i++) {
                formData.append('Images', files[i]);
            }

            $.ajax({
                url: baseUrl + 'QC5Check/SaveSubmitQC5UnitCheckList',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    Swal.close();
                    if (res.success) {
                        showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', function () {
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
    );
}


function toggleRadioForRadioCheck(radio, id, majordefect) {
    if (radio.checked && radio.dataset.wasChecked) {
        radio.checked = false;
        radio.dataset.wasChecked = "";

        radioChanged(id, null);
    } else {
        var radios = document.getElementsByName(radio.name);
        for (var i = 0; i < radios.length; i++) {
            radios[i].dataset.wasChecked = "";
        }

        radio.dataset.wasChecked = "True";

        radioChanged(id, radio.value);

        if (radio.value == 28) {
            var mainPassRadio = document.getElementById('radio3');
            if (mainPassRadio && mainPassRadio.checked) {
                mainPassRadio.checked = false;
                mainPassRadio.dataset.wasChecked = "";
                selectedRadioQC5Status = '';
            }
            if (majordefect == "True") {
                var mainPassWithConditionRadio = document.getElementById('radio2');
                mainPassWithConditionRadio.checked = false;
                mainPassWithConditionRadio.dataset.wasChecked = "";
                selectedRadioQC5Status = '';
            }
        }
    }
}


function radioChanged(id, value) {
    // Get hidden input values
    var projectId = document.getElementById('hdProject').value;
    var unitId = document.getElementById('hdUnitId').value;
    var seq = document.getElementById('hdSeq').value;

    $.ajax({
        url: baseUrl + 'QC5Check/SelectedQCUnitCheckListDefectStatus',
        type: 'POST',
        data: {
            ID: id,
            StatusID: value,
            ProjectID: projectId,
            UnitID: unitId,
            Seq: seq
        },
        success: function (response) {
            // Show SweetAlert for success
            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: 'อัปเดตสถานะสำเร็จ',
                showConfirmButton: false,
                timer: 1000,  // Show for 1 second
                toast: true  // Small popup in the corner
            });

            fetchUpdatedList();  // Call the function to reload the list
            fetchUpdatedSummary();
        },
        error: function (error) {
            // Show SweetAlert for failure
            Swal.fire({
                position: 'top-end',
                icon: 'error',
                title: 'อัปเดตสถานะไม่สำเร็จ',
                showConfirmButton: true,  // Allow clicking to close
                toast: true  // Small popup in the corner
            });
            console.error("Error updating status", error);
        }
    });
}

function fetchUpdatedList() {
    // Step 1: Store the current checked states
    let checkedStates = {};
    $('.form-check-input[type="radio"]').each(function () {
        if (this.checked) {
            checkedStates[$(this).attr('name')] = $(this).val();
        }
    });

    // Step 2: Fetch the updated list
    $.ajax({
        url: baseUrl + 'QC5Check/GetQCUnitCheckListDefects',  // Adjust to match your URL
        type: 'GET',
        data: {
            QC5UnitChecklistID: $('#hdQC5UnitChecklistID').val(),
            Seq: $('#hdSeq').val()  // Send Seq value to the server
        },
        success: function (response) {
            // Replace the current list content with the updated data
            $('.list-group').html(response);  // Replace list items

            // Step 3: Restore the checked states
            for (let name in checkedStates) {
                $('input[name="' + name + '"][value="' + checkedStates[name] + '"]').prop('checked', true);
            }
        },
        error: function (error) {
            console.error("Error fetching updated list:", error);
        }
    });
}

function fetchUpdatedSummary() {
    $.ajax({
        url: baseUrl + 'QC5Check/GetDataSummaryQC5',
        type: 'POST',
        data: {
            QC5UnitChecklistID: $('#hdQC5UnitChecklistID').val()
        },
        success: function (response) {
            $('#summaryContainer').html(response); // Update the summary container with new data
        },
        error: function (error) {
            console.error("Error fetching updated summary:", error);
        }
    });
}


function openModalUpdateDefectDetailQC(defectID) {

    $.ajax({
        url: baseUrl + 'QC5Check/GetQC5DefactEdit',
        type: 'GET',
        data: { DefectID: defectID }, // Pass the DefectID to the controller
        success: function (response) {
            if (response) {
                // Clear dropzone file input and preview container before binding new data
                $('#file-input-update').val('');  // Clear file input
                $('#preview-container-update').empty();  // Clear preview container
                // Hide the fixed button
                var fixedButton = document.querySelector('.fixedButton');
                if (fixedButton) {
                    // Only try to hide the button if it exists
                    fixedButton.style.display = 'none';
                }

                // Set the values for the text inputs
                $('#UpQC5DefectID').val(response.DefectID);  // Set the DefectID
                $('#defectAreaText').val(response.DefectAreaName);  // Set the DefectArea text
                $('#defectTypeText').val(response.DefectTypeName);  // Set the DefectType text
                $('#defectDescriptionText').val(response.DefectDescriptionName);  // Set the DefectDescription text

                // Populate other fields in the modal
                if (actionTypeEn !== "submit") {
                    if (response.Seq > response.RefSeq) {
                        if (response.StatusID !== "27") {
                            // Make the textarea editable (not disabled)
                            $('#commentTextareaupdate').val(response.Remark).prop('disabled', false);
                        } else {
                            // Make the textarea disabled if StatusID is 27
                            $('#commentTextareaupdate').val(response.Remark).prop('disabled', true);
                        }
                    } else {
                        // Make the textarea disabled if Seq is not greater than RefSeq
                        $('#commentTextareaupdate').val(response.Remark).prop('disabled', true);
                    }
                } else {
                    // Disable the textarea if actionTypeEn is "submit"
                    $('#commentTextareaupdate').val(response.Remark).prop('disabled', true);
                }

                $('#QC5DefectID').val(response.DefectID);
                if (actionTypeEn !== "submit") {
                    if (response.Seq > response.RefSeq) {
                        if (response.StatusID !== "27") {
                            // Enable and set the checkbox based on response.IsMajorDefect
                            $('#majorDefectCheckboxupdate').prop('checked', response.IsMajorDefect).prop('disabled', false);
                        } else {
                            // Disable the checkbox if StatusID is 27
                            $('#majorDefectCheckboxupdate').prop('checked', response.IsMajorDefect).prop('disabled', true);
                        }
                    } else {
                        // Disable the checkbox if Seq is not greater than RefSeq
                        $('#majorDefectCheckboxupdate').prop('checked', response.IsMajorDefect).prop('disabled', true);
                    }
                } else {
                    // Disable the checkbox if actionTypeEn is "submit"
                    $('#majorDefectCheckboxupdate').prop('checked', response.IsMajorDefect).prop('disabled', true);
                }


                // Clear existing images in the modal
                $('#imagePreview3').empty();

                // If there are images, populate the image list
                if (response.listImageNotpass && response.listImageNotpass.length > 0) {
                    response.listImageNotpass.forEach(function (image) {
                        let removeButtonHTML = ''; // Initialize empty remove button

                        // Only show the RemoveImage button if actionTypeEn is not "submit"
                        if (actionTypeEn !== "submit") {
                            if (response.Seq > response.RefSeq) {
                                if (response.StatusID !== "27") {
                                    removeButtonHTML = `<button type="button" class="remove-button" onclick="RemoveImage('${image.ResourceID}')">✖</button>`;
                                }
                            }
                        }

                        $('#imagePreview3').append(`
                                    <div class="position-relative d-inline-block mb-3">
                                        <a data-fslightbox="gallery" href="${baseUrl + image.FilePath}">
                                            <img src="${baseUrl + image.FilePath}" alt="รูปภาพ Defact" class="img-thumbnail" style="width: 90px; height: 90px; border-radius: 50%; object-fit: cover;">
                                        </a>
                                        ${removeButtonHTML}
                                    </div>
                                `);
                    });
                    // Reinitialize the fslightbox after appending new content
                    refreshFsLightbox();
                }

                if (response.Seq > response.RefSeq) {
                    if (response.StatusID !== "27") {
                        if ($('#imagePreview3 .position-relative').length >= 5) {
                            $('#drop-zone-update').hide();
                        } else {
                            $('#drop-zone-update').show();
                        }
                    }
                    else {
                        $('#drop-zone-update').hide();
                    }
                }
                else {
                    $('#drop-zone-update').hide();
                }

                // Show the modal
                var myModal = new bootstrap.Modal(document.getElementById('Update-detail-defect'));
                myModal.show();
            }
        },
        error: function () {
            alert('Error fetching data. Please try again.');
        }
    });
}


function validateMainStatus(mainRadio) {
    let allItems = document.querySelectorAll('.card-item[data-status]');
    let hasNotPass = false;
    allItems.forEach(function (item) {
        let status = item.getAttribute('data-status');
        if (status === '28') {
            hasNotPass = true;
        }
    });

    if (hasNotPass) {
        showErrorAlert('คำเตือน!', 'ยังมีรายการ Defect ที่ไม่ผ่าน');
        mainRadio.checked = false;
    } else {
        if (mainRadio.checked && mainRadio.dataset.wasChecked) {
            mainRadio.checked = false;
            mainRadio.dataset.wasChecked = "";
            selectedRadioQC5Status = null;
        } else {
            let radios = document.getElementsByName(mainRadio.name);
            for (let i = 0; i < radios.length; i++) {
                radios[i].dataset.wasChecked = "";
            }
            mainRadio.dataset.wasChecked = "true"; // Mark the current one as checked
            selectedRadioQC5Status = mainRadio.value;
        }
    }
}

function validateConditionalPass(mainRadio) {
    // Get all list items with data-is-major-defect and data-status attributes
    let allItems = document.querySelectorAll('.card-item[data-is-major-defect][data-status]');
    let hasMajorDefect = false;
    let hasNotPass = false;

    // Loop through the list items to check for major defects and not pass status
    allItems.forEach(function (item) {
        let isMajorDefect = item.getAttribute('data-is-major-defect') === 'True';
        let statusID = item.getAttribute('data-status');

        if (isMajorDefect && statusID == "28") {
            hasMajorDefect = true;
        }
    });

    // If any item has IsMajorDefect == true or StatusID == 28 (not pass), show an alert and prevent selection
    if (hasMajorDefect || hasNotPass) {
        showErrorAlert('คำเตือน!', 'ยังมีรายการ Major Defect ที่ไม่ผ่าน');
        // Prevent the radio button from being checked
        mainRadio.checked = false;
    } else {
        // If no major defects or not pass status, proceed with checking/unchecking behavior
        if (mainRadio.checked && mainRadio.dataset.wasChecked) {
            mainRadio.checked = false;  // Uncheck if already checked
            mainRadio.dataset.wasChecked = "";
            selectedRadioQC5Status = null;
        } else {
            // Uncheck all radios in the same group and set this as checked
            var radios = document.getElementsByName(mainRadio.name);
            for (var i = 0; i < radios.length; i++) {
                radios[i].dataset.wasChecked = "";
            }
            mainRadio.dataset.wasChecked = "true";  // Mark the current one as checked
            selectedRadioQC5Status = mainRadio.value;
        }
    }
}

function ClickNotReadyInspect(mainRadio) {
    // Check if the radio is already checked and has been clicked again
    if (mainRadio.checked && mainRadio.dataset.wasChecked) {
        // Uncheck the radio if it's clicked when already checked
        mainRadio.checked = false;
        mainRadio.dataset.wasChecked = "";
        selectedRadioQC5Status = '';  // Unset the selected value
    } else {
        // Uncheck all radios in the same group and set the clicked one as checked
        var radios = document.getElementsByName(mainRadio.name);
        for (var i = 0; i < radios.length; i++) {
            radios[i].dataset.wasChecked = "";
        }
        mainRadio.dataset.wasChecked = "true";
        selectedRadioQC5Status = mainRadio.value;  // Set the selected value
    }
}

function ClickNotPass(mainRadio) {
    // Check if the radio is already checked and has been clicked again
    if (mainRadio.checked && mainRadio.dataset.wasChecked) {
        // Uncheck the radio if it's clicked when already checked
        mainRadio.checked = false;
        mainRadio.dataset.wasChecked = "";
        selectedRadioQC5Status = '';  // Unset the selected value
    } else {
        // Uncheck all radios in the same group and set the clicked one as checked
        var radios = document.getElementsByName(mainRadio.name);
        for (var i = 0; i < radios.length; i++) {
            radios[i].dataset.wasChecked = "";
        }
        mainRadio.dataset.wasChecked = "true";
        selectedRadioQC5Status = mainRadio.value;  // Set the selected value
    }
}


document.getElementById('UpdateDefectButton').addEventListener('click', function () {
    onUpdateDefectButtonClick();
});


function onUpdateDefectButtonClick() {
    var DefectID = document.getElementById('UpQC5DefectID').value;
    var comment = document.getElementById('commentTextareaupdate').value;
    var files = $('#file-input-update')[0].files;

    // Get hidden input values
    var projectId = document.getElementById('hdProject').value;
    var unitId = document.getElementById('hdUnitId').value;
    var seq = document.getElementById('hdSeq').value;


    var formData = new FormData();
    formData.append('ID', DefectID);
    formData.append('ProjectID', projectId);
    formData.append('UnitID', unitId);
    formData.append('Remark', comment);
    formData.append('Seq', seq);

    for (var i = 0; i < files.length; i++) {
        formData.append('Images', files[i]);
    }


    var oldImagesCount = $('#imagePreview3 .position-relative').length;
    var newImagesCount = files.length;
    var totalImagesCount = oldImagesCount + newImagesCount;


    if (totalImagesCount > 5) {
        showErrorAlertNotCloseModal('คำเตือน!', 'คุณสามารถอัปโหลดรูปภาพได้ไม่เกิน 5 รูปต่อรายการ');
        return;
    }


    showLoadingAlert();

    $.ajax({
        url: baseUrl + 'QC5Check/UpdateDefectDetail',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            Swal.close();
            if (response.success) {
                showFixedButton()
                showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ');
                const clearButton = document.getElementById('closemodalupdatedefect');
                clearButton.click();
            } else {
                showErrorAlert('บันทึกข้อมูลไม่สำเร็จ', response.message || 'เกิดข้อผิดพลาดในการบันทึกข้อมูล');
            }
        },
        error: function (xhr, status, error) {
            Swal.close();
            showErrorAlert('เกิดข้อผิดพลาด!', error);
        }
    });
}


document.addEventListener("DOMContentLoaded", function () {
    var dropZone = document.getElementById("drop-zone-update");
    var fileInput = document.getElementById("file-input-update");
    var previewContainer = document.getElementById("preview-container-update");
    var filesArray = [];

    // Set the max file count limit
    const MAX_FILE_COUNT = 5;

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
            if (filesArray.length < MAX_FILE_COUNT) {
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

    document.getElementById('closemodalupdatedefect').addEventListener('click', function () {
        previewContainer.innerHTML = '';
        filesArray = [];
        updateFileInput();
    });

});


// Function to show the fixed button
function showFixedButton() {
    var fixedButton = document.querySelector('.fixedButton');
    fixedButton.style.display = 'block';  // Show the button again
}


function genPDF() {
    showLoadingAlert('กำลังบันทึก...', 'กรุณารอสักครู่');
    var projectId = document.getElementById('hdProject').value;
    var unitId = document.getElementById('hdUnitId').value;
    var QCUnitCheckListID = document.getElementById('hdQC5UnitChecklistID').value;

    $.ajax({
        url: baseUrl + 'QC5Check/PrintPDF',
        type: 'POST',
        data: {
            projectID: projectId,
            unitID: unitId,
            QCID: QCUnitCheckListID
        },
        success: function (res) {
            Swal.close();
            if (res.success) {
                showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', function () {
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












