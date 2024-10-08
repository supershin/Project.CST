        function adjustTextareaRows() {
            const textareas = document.querySelectorAll('.custom-textarea');
            textareas.forEach(textarea => {
                if (window.innerWidth <= 576) {
                    textarea.setAttribute('rows', 3);
                } else {++
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

            var modalTitle = document.getElementById('insert-new-qc5-Label');
            if (action === 'add') {
                modalTitle.textContent = 'เพิ่มข้อมูลรายการ';
                $('#autocomplete1, #autocomplete2, #autocomplete3').val('').prop('disabled', false);
                $('#defectAreaId, #defectTypeId, #defectDescriptionId').val('');
                $('#otherDefectInput, #commentTextarea').val('');
                $('#majorDefectCheckbox').prop('checked', false);
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
                showErrorAlert('คำเตือน!', 'กรุณาเลือกตำแหน่ง');
                return;
            }
            if (!defectTypeId) {
                showErrorAlert('คำเตือน!', 'กรุณาเลือกหมวดงาน');
                return;
            }
            if (!defectDescriptionId) {
                showErrorAlert('คำเตือน!', 'กรุณาเลือกรายการ defect');
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
                        showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', function () {
                            window.location.reload(); 
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
        });

        function filterCards() {
            var searchInput = document.getElementById('searchInput').value.toLowerCase();
            var statusFilter = document.getElementById('statusFilter').value;

            var cards = document.querySelectorAll('.card-item');

            cards.forEach(function (card) {
                var defectText = card.getAttribute('data-defect').toLowerCase();
                var remarkText = card.getAttribute('data-remark').toLowerCase();
                var cardStatus = card.getAttribute('data-status');

                // Check if the card matches the search term and the selected filter
                var matchesSearch = defectText.includes(searchInput) || remarkText.includes(searchInput);
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
                data: { DefectID: defectID }, // Pass the DefectID to the controller
                success: function (response) {
                    if (response) {

                        $('#dropdown1Edit').val(response.DefectAreaID).trigger('change');

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

                                        ddlDefectDescription.val(response.DefectDescriptionID).prop('disabled', false); // Set the selected value and enable it
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
                        $('#commentTextareaEdit').val(response.Remark).prop('disabled', actionTypeEn === "submit"); // Disable remark if "submit"
                        $('#QC5DefectID').val(response.DefectID);
                        $('#majorDefectCheckboxEdit').prop('checked', response.IsMajorDefect).prop('disabled', actionTypeEn === "submit"); // Disable checkbox if "submit"

                        // Clear existing images in the modal
                        $('#imagePreview2').empty();

                        // If there are images, populate the image list
                        if (response.listImageNotpass && response.listImageNotpass.length > 0) {
                            response.listImageNotpass.forEach(function (image) {
                                let removeButtonHTML = ''; // Initialize empty remove button

                                // Only show the RemoveImage button if actionTypeEn is not "submit"
                                if (actionTypeEn !== "submit") {
                                    removeButtonHTML = `<button type="button" class="remove-button" onclick="RemoveImage('${image.ResourceID}')">✖</button>`;
                                }

                                $('#imagePreview2').append(`
                                    <div class="position-relative d-inline-block mb-3">
                                        <a data-fslightbox="gallery" href="${image.FilePath}">
                                            <img src="${image.FilePath}" alt="รูปภาพ Defact" class="img-thumbnail" style="width: 90px; height: 90px; border-radius: 50%; object-fit: cover;">
                                        </a>
                                        ${removeButtonHTML}  <!-- Conditionally render remove button -->
                                    </div>
                                `);
                            });

                            // Reinitialize the fslightbox after appending new content
                            refreshFsLightbox();
                        }

                        // Check if there are 5 or more images and hide the dropzone
                        if ($('#imagePreview2 .position-relative').length >= 5) {
                            $('#drop-zone-edit').hide(); // Hide the dropzone if there are 5 or more images
                        } else {
                            $('#drop-zone-edit').show(); // Show the dropzone if less than 5 images
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
                        url: '/QC5Check/GetDDLDefectType', // The controller action for fetching defect types
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
                        url: '/QC5Check/GetDDLDefectDescription', // The controller action for fetching defect descriptions
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

            // Get hidden input values
            var projectId = document.getElementById('hdProject').value;
            var unitId = document.getElementById('hdUnitId').value;
            var seq = document.getElementById('hdSeq').value;

            if (!defectAreaId) {
                showErrorAlert('คำเตือน!', 'กรุณาเลือกตำแหน่ง');
                return;
            }
            if (!defectTypeId) {
                showErrorAlert('คำเตือน!', 'กรุณาเลือกหมวดงาน');
                return;
            }
            if (!defectDescriptionId) {
                showErrorAlert('คำเตือน!', 'กรุณาเลือกรายการ defect');
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
                        showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ');
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
            var dropZone = document.getElementById("drop-zone-edit");
            var fileInput = document.getElementById("file-input-edit");
            var previewContainer = document.getElementById("preview-container-edit");
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
        });

        function RemoveImage(resourceID) {

            showLoadingAlert();

            $.ajax({
                url: baseUrl + 'QC5Check/RemoveImage',
                type: 'POST',
                data: { resourceID: resourceID },
                success: function (response) {
                    Swal.close(); 
                    if (response.success) {
                        $(`button[onclick="RemoveImage('${resourceID}')"]`).parent().remove();
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

            showLoadingAlert();

            $.ajax({
                url: baseUrl + 'QC5Check/RemoveDefectQC5Unit',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    Swal.close();
                    if (response.success) {
                        showSuccessAlert('สำเร็จ!', 'ลบข้อมูลสำเร็จ', function () {
                            window.location.reload();
                        });
                    } else {
                        showErrorAlert('ลบข้อมูลไม่สำเร็จ', response.message || 'เกิดข้อผิดพลาดในการลบข้อมูล');
                    }
                },
                error: function (xhr, status, error) {
                    Swal.close();
                    showErrorAlert('เกิดข้อผิดพลาด!', error);
                }
            });
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
            showLoadingAlert('กำลังบันทึก...', 'กรุณารอสักครู่');

            var QCUnitCheckListID = document.getElementById('hdQC5UnitChecklistID').value;
            var QCUnitCheckListActionID = document.getElementById('hdQC5UnitChecklistActionID').value;
            var QCStatusID = selectedRadioQC5Status;  // Ensure this value is set
            var ActionType = 'save';
            var QCRemark = document.getElementById('QC5Remark').value;
            var files = $('#file-input-save-submit')[0].files;
            // var signData = unitEquipment.getSignatureData();  

            var formData = new FormData();
            formData.append('QCUnitCheckListID', QCUnitCheckListID);
            formData.append('QCUnitCheckListActionID', QCUnitCheckListActionID);
            formData.append('QCStatusID', QCStatusID);  // Ensure this is the correct value
            formData.append('ActionType', ActionType);
            formData.append('QCRemark', QCRemark);


            for (var i = 0; i < files.length; i++) {
                formData.append('Images', files[i]);
            }

            //if (signData) {
            //    var signatureData = {
            //        MimeType: signData.MimeType,
            //        StorageBase64: signData.StorageBase64
            //    };
            //    formData.append('Sign', JSON.stringify(signatureData));
            //}

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

        function openSignatureModal() {
            var myModal = new bootstrap.Modal(document.getElementById('signatureModal'), {
                keyboard: false
            });
            myModal.show();
        }

        function saveSignature() {
            showLoadingAlert('กำลังบันทึก...', 'กรุณารอสักครู่');

            var QCUnitCheckListID = document.getElementById('hdQC5UnitChecklistID').value;

            var signData = unitEquipment.getSignatureData();  // Accessing it from unitEquipment object

            var formData = new FormData();
            formData.append('QCUnitCheckListID', QCUnitCheckListID);

            // Append signature data as JSON
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
                contentType: false,  // Set contentType to false for FormData
                processData: false,  // Set processData to false for FormData
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

        function SubmitUnitQC5() {
            showLoadingAlert('กำลังบันทึก...', 'กรุณารอสักครู่');

            var QCUnitCheckListID = document.getElementById('hdQC5UnitChecklistID').value;
            var QCUnitCheckListActionID = document.getElementById('hdQC5UnitChecklistActionID').value;
            var QCStatusID = selectedRadioQC5Status;  // Ensure this value is set
            var ActionType = 'submit';
            var QCRemark = document.getElementById('QC5Remark').value;
            var files = $('#file-input-save-submit')[0].files;
            // var signData = unitEquipment.getSignatureData();  

            var formData = new FormData();
            formData.append('QCUnitCheckListID', QCUnitCheckListID);
            formData.append('QCUnitCheckListActionID', QCUnitCheckListActionID);
            formData.append('QCStatusID', QCStatusID);  // Ensure this is the correct value
            formData.append('ActionType', ActionType);
            formData.append('QCRemark', QCRemark);


            for (var i = 0; i < files.length; i++) {
                formData.append('Images', files[i]);
            }

            //if (signData) {
            //    var signatureData = {
            //        MimeType: signData.MimeType,
            //        StorageBase64: signData.StorageBase64
            //    };
            //    formData.append('Sign', JSON.stringify(signatureData));
            //}

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

        function toggleRadioForRadioCheck(radio, id) {
            if (radio.checked && radio.dataset.wasChecked) {
                // Uncheck the radio if clicked while already checked
                radio.checked = false;
                radio.dataset.wasChecked = "";

                // Send null to the radioChanged function to indicate uncheck
                radioChanged(id, null);
            } else {
                // Uncheck all radios in the same group
                var radios = document.getElementsByName(radio.name);
                for (var i = 0; i < radios.length; i++) {
                    radios[i].dataset.wasChecked = "";
                }

                // Mark the clicked radio as checked
                radio.dataset.wasChecked = "true";

                // Send the checked value to the radioChanged function
                radioChanged(id, radio.value);
            }
        }


        function radioChanged(id, value) {
            $.ajax({
                url: baseUrl + 'QC5Check/SelectedQCUnitCheckListDefectStatus',
                type: 'POST',
                data: {
                    ID: id,
                    StatusID: value
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

                    // Fetch updated list
                    fetchUpdatedList();  // Call the function to reload the list
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

// Fetch the updated list via AJAX and replace the current content
//function fetchUpdatedList() {
//    console.log('fetchUpdatedList')
//    $.ajax({
//        url: baseUrl + 'QC5Check/GetQCUnitCheckListDefects',  // Adjust to match your URL
//        type: 'GET',
//        data: {
//            QC5UnitChecklistID: $('#hdQC5UnitChecklistID').val(),
//            Seq: $('#hdSeq').val()
//        },
//        success: function (response) {
//            // Replace the current list content with the updated data
//            $('.list-group').html(response);  // Replace list items
//        },
//        error: function (error) {
//            console.error("Error fetching updated list:", error);
//        }
//    });
//}

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







