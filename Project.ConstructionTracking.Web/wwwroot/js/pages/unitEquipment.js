
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

var unitEquipment = {
    init: () => {
        unitEquipment.initSignaturePad();

        $("#btn-submit-form").click(() => {
            unitEquipment.submitUnitEquipmentSign();           
            return false;
        });

        $("#btn-save-form").click(() => {
            unitEquipment.saveUnitEquipmentSign();
            return false;
        });

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

    submitUnitEquipmentSign: () => {
        let isValid = true;
        let validationMessage = '';

        $('div.card-link').each(function () {
            const statusUse = $(this).find('button.status-dot').data('status-use');
            const cntCheckListAll = parseInt($(this).find('.text1').text().split(' ')[1]);
            const cntCheckListUnit = parseInt($(this).find('.text2').text().split(' ')[1]);
            const cntCheckListNotPass = parseInt($(this).find('.text3').text().split(' ')[1]);

            if (statusUse !== 'success' && !(cntCheckListAll === cntCheckListUnit && cntCheckListNotPass === 0)) {
                isValid = false;
                validationMessage = 'กรุณาตรวจสอบสถานะของรายการทั้งหมดก่อนบันทึกข้อมูล';
                return false;
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

        if (!vendorId) {
            Swal.fire({
                title: 'กรุณาระบุผู้รับเหมา',
                text: '',
                icon: 'warning',
                confirmButtonText: 'ตกลง'
            });
            return;
        }

        if (!signData || !signData.MimeType || !signData.StorageBase64) {
            Swal.fire({
                title: 'กรุณาให้ผู้รับเหมาเซ็น',
                text: '',
                icon: 'warning',
                confirmButtonText: 'ตกลง'
            });
            return;
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
        };

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

    saveUnitEquipmentSign: () => {
        var vendorId = document.getElementById('selectedVendorValue').value;
        var signData = unitEquipment.getSignatureData();

        var data = {
            ProjectID: document.getElementById('projectId').value,
            FormID: document.getElementById('FormID').value,
            UnitFormID: document.getElementById('UnitFormID').value,
            UnitID: document.getElementById('unitId').value,
            FormGrade: selectedRadioValue,
            Act: 'save',
            VendorID: vendorId,
            Sign: signData,
        };

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

window.onload = function () {
    var radios = document.getElementsByName('radios-inline');
    for (var i = 0; i < radios.length; i++) {
        if (radios[i].checked) {
            selectedRadioValue = radios[i].value; // Set the selected value based on the pre-checked radio
            radios[i].dataset.wasChecked = "true"; // Mark it as checked
            break;
        }
    }
};

