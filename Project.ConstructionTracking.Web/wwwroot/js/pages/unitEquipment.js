
let selectedRadioValue = '';

function toggleRadio(radio) {
    if (radio.checked && radio.dataset.wasChecked) {
        radio.checked = false;
        radio.dataset.wasChecked = "";
        selectedRadioValue = ''; // Unset the selected value if the radio is unchecked
    } else {
        var radios = document.getElementsByName(radio.name);
        for (var i = 0; i < radios.length; i++) {
            radios[i].dataset.wasChecked = "";
        }
        radio.dataset.wasChecked = "true";
        selectedRadioValue = radio.nextElementSibling.textContent; // Set the selected value
    }
}

let signaturePad;
let signaturePad_JM;

var unitEquipment = {
    init: () => {
       
        unitEquipment.initSignaturePad();
        unitEquipment.initSignaturePad_JM();

        $("#btn-vendor").click(() => {
            $('#modal-sign').modal('show');
            return false;
        });

        $("#btn-sign").click(() => {
            $('#modal-sign').modal();
            return false;
        });
        $("#btn-sign-jm").click(() => {
            $('#modal-sign-jm').modal();
            return false;
        });
        $("#btn-save-sign").click(() => {
            unitEquipment.saveUnitEquipmentSign();
            return false;
        });
        $("#btn-save-sign-jm").click(() => {
            $('#modal-sign-jm').modal('hide');
            return false;
        });
        $("#btn-save-unit-equipment").click(() => {
            unitEquipment.saveUnitEquipmentSign();
            return false;
        });
    },
    initSignaturePad: () => {
      
        $('#modal-sign').on('shown.bs.modal', function (e) {
           
            let canvas = $("#signature-pad canvas");
            let parentWidth = $(canvas).parent().outerWidth();
            let parentHeight = $(canvas).parent().outerHeight();
            canvas.attr("width", parentWidth + 'px')
                .attr("height", parentHeight + 'px');
            signaturePad = new SignaturePad(canvas[0], {
                backgroundColor: 'rgb(255, 255, 255)'
            });
        });

        $(document).on('click', '#modal-sign .clear', function () {
            signaturePad.clear();
        });

    },
    initSignaturePad_JM: () => {
        $('#modal-sign-jm').on('shown.bs.modal', function (e) {
            let canvas = $("#signature-pad-jm canvas");
            let parentWidth = $(canvas).parent().outerWidth();
            let parentHeight = $(canvas).parent().outerHeight();
            canvas.attr("width", parentWidth + 'px')
                .attr("height", parentHeight + 'px');
            signaturePad_JM = new SignaturePad(canvas[0], {
                backgroundColor: 'rgb(255, 255, 255)'
            });
        });
        $(document).on('click', '#modal-sign-jm .clear', function () {
            signaturePad_JM.clear();
        });
    },
    saveUnitEquipmentSign: () => {

        let isValid = true;
        let validationMessage = '';

        // Iterate through each card to validate
        $('div.card-link').each(function () {
            const statusUse = $(this).find('button.status-dot').data('status-use');
            const cntCheckListAll = parseInt($(this).find('.text1').text().split(' ')[1]);
            const cntCheckListUnit = parseInt($(this).find('.text2').text().split(' ')[1]);
            const cntCheckListNotPass = parseInt($(this).find('.text3').text().split(' ')[1]);

            // Validate the conditions
            if (statusUse !== 'success' && !(cntCheckListAll === cntCheckListUnit && cntCheckListNotPass === 0)) {
                isValid = false;
                validationMessage = 'กรุณาตรวจสอบสถานะของรายการทั้งหมดก่อนบันทึกข้อมูล';
                return false; // Exit loop early if validation fails
            }
        });

        if (!isValid) {
            Swal.fire({
                title: 'ข้อผิดพลาด!',
                text: validationMessage,
                icon: 'warning',
                confirmButtonText: 'ตกลง'
            });
            return;
        }

        var vendorId = document.getElementById('selectedVendorValue').value;
        var signData = unitEquipment.getSignatureData();

        // Validate if VendorID is null
        if (!vendorId) {
            Swal.fire({
                title: 'กรุณาระบุผู้รับเหมา',
                text: '',
                icon: 'warning',
                confirmButtonText: 'ตกลง'
            });
            return; // Stop execution if VendorID is null
        }

        // Validate if Sign is null or empty
        if (!signData || !signData.MimeType || !signData.StorageBase64) {
            Swal.fire({
                title: 'กรุณาให้ผู้รับเหมาเซ็น',
                text: '',
                icon: 'warning',
                confirmButtonText: 'ตกลง'
            });
            return; // Stop execution if Sign is invalid
        }

        var data = {
            ProjectID: document.getElementById('projectId').value,
            FormID: document.getElementById('FormID').value,
            UnitFormID: document.getElementById('UnitFormID').value,
            UnitID: document.getElementById('unitId').value,
            FormGrade: selectedRadioValue,
            Act: 'submit',
            VendorID: vendorId,
            Sign: signData,
            /*        SignJM: unitEquipment.getSignatureDataJM() */
        };

        console.log(data);

        $.ajax({
            url: baseUrl + 'FormGroup/UpdateSaveGrade',
            type: 'POST',
            dataType: 'json',
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
                            $('#modal-sign').modal('hide');
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
    getSignatureDataJM: () => {
        let dataURL;
        let contentType;
        let storage;
        if (!signaturePad_JM.isEmpty()) {
            dataURL = signaturePad_JM.toDataURL();
            var parts = dataURL.split(';base64,');
            contentType = parts[0].split(":")[1];
            storage = parts[1];
        }
        return {
            MimeType: contentType,
            StorageBase64: storage
        };
    }
}

function saveData() {
    let isValid = true;
    let validationMessage = '';

    // Iterate through each card to validate
    $('div.card-link').each(function () {
        const statusUse = $(this).find('button.status-dot').data('status-use');
        const cntCheckListAll = parseInt($(this).find('.text1').text().split(' ')[1]);
        const cntCheckListUnit = parseInt($(this).find('.text2').text().split(' ')[1]);
        const cntCheckListNotPass = parseInt($(this).find('.text3').text().split(' ')[1]);

        // Validate the conditions
        if (statusUse !== 'success' && !(cntCheckListAll === cntCheckListUnit && cntCheckListNotPass === 0)) {
            isValid = false;
            validationMessage = 'กรุณาตรวจสอบสถานะของรายการทั้งหมดก่อนบันทึกข้อมูล';
            return false; // Exit loop early if validation fails
        }
    });

    if (!isValid) {
        Swal.fire({
            title: 'ข้อผิดพลาด!',
            text: validationMessage,
            icon: 'warning',
            confirmButtonText: 'ตกลง'
        });
        return;
    }

    // If validation passes, proceed to save data
    const data = {
        ProjectID: document.getElementById('projectId').value,
        FormID: document.getElementById('FormID').value,
        UnitFormID: document.getElementById('UnitFormID').value,
        UnitID: document.getElementById('unitId').value,
        FormGrade: selectedRadioValue,
        Act: 'save'
    };

    console.log(data);

    $.ajax({
        url: baseUrl + 'FormGroup/UpdateSaveGrade',
        type: 'POST',
        dataType: 'json',
        data: data,
        success: function (res) {
            if (res.success) {
                Swal.fire({
                    title: 'สำเร็จ!',
                    text: 'บันทึกข้อมูลสำเร็จ',
                    icon: 'success',
                    confirmButtonText: 'ตกลง'
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Optionally reload the page or perform any additional actions
                    }
                });
            } else {
                Swal.fire({
                    title: 'ข้อผิดพลาด!',
                    text: 'บันทึกข้อมูลไม่สำเร็จ',
                    icon: 'error',
                    confirmButtonText: 'ตกลง'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'ข้อผิดพลาด!',
                text: 'เกิดข้อผิดพลาดในการบันทึกข้อมูล',
                icon: 'error',
                confirmButtonText: 'ตกลง'
            });
        }
    });
}

function previewImage(event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('imagePreview');
        output.src = reader.result;
        output.style.display = 'block';
    };
    reader.readAsDataURL(event.target.files[0]);
    document.getElementById('removeImage').style.display = 'inline-block';
}

function removeImage() {
    document.getElementById('imagePreview').style.display = 'none';
    document.getElementById('imageUpload').value = null;
    document.getElementById('removeImage').style.display = 'none';
}