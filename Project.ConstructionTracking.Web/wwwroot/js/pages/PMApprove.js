function openModal(UnitFormID, Group_ID) {
    $.ajax({
        url: baseUrl + 'PMApprove/GetImages', // Replace with your actual controller and action
        type: 'GET',
        data: { UnitFormID: UnitFormID, GroupID: Group_ID, RoleID: 1 },
        success: function (images) {
            // Update the modal with the fetched images
            var modalBody = $('#exampleModal .modal-body .row');
            modalBody.empty(); // Clear any previous images

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

            // Reinitialize fslightbox for the newly added images
            refreshFsLightbox(); // Call this function to reinitialize the lightbox

            // Show the modal
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

    // Auto check "ไม่อนุมัติหลัก" when "ไม่อนุมัติPC" is checked
    if (radio.value === "7") {
        var mainApprovalRadios = document.getElementsByName('radios-inline-approval');
        for (var j = 0; j < mainApprovalRadios.length; j++) {
            if (mainApprovalRadios[j].value === "5") {
                mainApprovalRadios[j].checked = true;
            }
        }
    }

    // Prevent checking "อนุมัติหลัก" if any "ไม่อนุมัติPC" is selected
    if (radio.value === "4") {
        var conditionalRadios = document.querySelectorAll('input[type="radio"][value="7"]');
        for (var k = 0; k < conditionalRadios.length; k++) {
            if (conditionalRadios[k].checked) {
                Swal.fire({
                    title: 'Warning!',
                    text: 'ยังมีการไม่อนุมัติแบบมีเงื่อนไข',
                    icon: 'warning',
                    confirmButtonText: 'OK'
                });
                radio.checked = false; // Prevent checking "อนุมัติหลัก"
                return; // Exit the function to stop further execution
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
    var data = new FormData();

    // Option 1: Getting values from hidden fields
    data.append("UnitFormID", document.getElementById("UnitFormID").value);
    data.append("ProjectID", document.getElementById("ProjectID").value);
    data.append("UnitID", document.getElementById("UnitID").value);
    data.append("UnitCode", document.getElementById("UnitCode").value);
    data.append("FormID", document.getElementById("FormID").value);
   
    // Add ActionType (save or submit)
    data.append("ActionType", actionType);

  
    // Add main form status and remark
    var mainStatus = document.querySelector('input[name="radios-inline-approval"]:checked');
    if (mainStatus) {
        data.append("UnitFormStatus", mainStatus.value);
    }
    data.append("Remark", document.getElementById("mainRemark").value);
    
    // Collect images if any
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

        // Check if this group_id is already added
        let existingCondition = passConditions.find(pc => pc.Group_ID === group_id);

        // If not already added, add it
        if (!existingCondition) {
            passConditions.push({
                PassConditionsID: PassConditionsElement ? PassConditionsElement.value : null,
                PassConditionsvalue: PassConditionsval,
                Group_ID: group_id,
                Remark: remarkElement ? remarkElement.value : '',
            });
        } else {
            // Update the existing entry if the radio is checked
            if (PassConditionsval !== null) {
                existingCondition.PassConditionsvalue = PassConditionsval;
            }
        }
    });


    if (passConditions.length > 0) {
        data.append("PassConditionsIUD", JSON.stringify(passConditions));
    }

    //console.log(data);
    
    // Send the data using AJAX
    $.ajax({
        url: baseUrl + 'PMApprove/SaveOrSubmit', // Replace with your actual controller action
        type: 'POST',
        contentType: false,
        processData: false,
        data: data,
        success: function (res) {
            if (res.success) {
                Swal.fire({
                    title: 'Success!',
                    text: 'Data has been successfully ' + actionType + 'd.',
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
                    text: 'Failed to ' + actionType + ' the data.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error!',
                text: 'An error occurred while processing the request.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
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
