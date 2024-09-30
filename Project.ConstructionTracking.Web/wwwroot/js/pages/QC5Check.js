﻿        function adjustTextareaRows() {
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

        $(() => {
            addCheckedToCheckedRadio();
            toggleCollapseSection();
            $('input[type="radio"].allow-deselect').click(function (event) {
                if ($(this).hasClass('checked')) {
                    this.checked = false;
                }
                addCheckedToCheckedRadio();
                toggleCollapseSection();
            });
        });

        function addCheckedToCheckedRadio() {
            $('input[type="radio"].allow-deselect').each(function () {
                $(this).toggleClass('checked', this.checked);
            });
        }

        function toggleCollapseSection() {
            if ($('input[type="radio"].allow-deselect:checked').length > 0) {
                $('#collapseSection').collapse('show');
            } else {
                $('#collapseSection').collapse('hide');
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
                        // Set the value for Dropdown1Edit and trigger the change event to load Dropdown2Edit options
                        $('#dropdown1Edit').val(response.DefectAreaID).trigger('change');

                        // Call to populate Dropdown2Edit and then set the selected value
                        $.ajax({
                            url: baseUrl + 'QC5Check/GetDDLDefectType', // Fetch defect types based on the selected defect area
                            data: { defectAreaId: response.DefectAreaID },
                            success: function (data) {
                                var ddlDefectType = $('#dropdown2Edit');
                                ddlDefectType.empty().append('<option value="">กรุณาเลือก</option>');

                                $.each(data, function (index, item) {
                                    ddlDefectType.append($('<option>', { value: item.Value, text: item.Text }));
                                });

                                ddlDefectType.val(response.DefectTypeID).prop('disabled', false); // Set the selected value and enable it

                                // Call to populate Dropdown3Edit and then set the selected value
                                $.ajax({
                                    url: baseUrl + 'QC5Check/GetDDLDefectDescription', // Fetch defect descriptions based on the selected defect type
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
                        $('#commentTextareaEdit').val(response.Remark);
                        $('#QC5DefectID').val(response.DefectID);
                        $('#majorDefectCheckboxEdit').prop('checked', response.IsMajorDefect);

                        // Clear existing images in the modal
                        $('#imagePreview2').empty();

                        // If there are images, populate the image list
                        if (response.listImageNotpass && response.listImageNotpass.length > 0) {
                            response.listImageNotpass.forEach(function (image) {
                                $('#imagePreview2').append(`
                                    <div class="position-relative d-inline-block mb-3">
                                        <a data-fslightbox="gallery" href="${image.FilePath}">
                                            <img src="${image.FilePath}" alt="รูปภาพ Defact" class="img-thumbnail" style="width: 90px; height: 90px; border-radius: 50%; object-fit: cover;">
                                        </a>
                                        <button type="button" class="remove-button" onclick="RemoveImage('${image.ResourceID}')">✖</button>
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
