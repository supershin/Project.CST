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
