
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
        
        var data = {
            ProjectID: document.getElementById('projectId').value,
            FormID: document.getElementById('FormID').value,
            UnitFormID: document.getElementById('UnitFormID').value,
            UnitID: document.getElementById('unitId').value,
            FormGrade: selectedRadioValue,
            Act: 'Submit',
            VendorID: document.getElementById('selectedVendorValue').value,
            Sign: unitEquipment.getSignatureData(),
/*            SignJM: unitEquipment.getSignatureDataJM()*/
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

    const data = {
        ProjectID: document.getElementById('projectId').value,
        FormID: document.getElementById('FormID').value,
        UnitFormID: document.getElementById('UnitFormID').value,
        UnitID: document.getElementById('unitId').value,
        FormGrade: selectedRadioValue ,
        Act:'Save'
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
