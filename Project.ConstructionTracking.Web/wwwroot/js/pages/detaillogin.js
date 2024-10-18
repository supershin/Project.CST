let signaturePad;

const detailLogin = {
    init: (userId) => {
        detailLogin.initSignaturePad();

        $("#btn-sign").click(() => {
            $('#modal-sign').modal('show');
            return false;
        });

        $("#btn-save-sign").click(() => {
            $('#modal-sign').modal('hide');
            if (!detailLogin.getSignatureData()) {
                $('#success-icon').hide();
            } else {
                $('#success-icon').show();
            }
            return false;
        });

        $('#edit-user-save').click(() => {
            event.preventDefault(); // Prevent form submission for validation
            let isValid = true;

            // Clear previous error messages and reset styles
            document.querySelectorAll('.form-control').forEach(input => {
                input.style.borderColor = ''; // Reset border color
            });
            document.querySelectorAll('.text-danger').forEach(span => {
                span.textContent = ''; // Clear error messages
            });

            // Validate Name
            const userName = document.getElementById('user-name');
            if (userName.value.trim() === '') {
                isValid = false;
                userName.style.borderColor = 'red';
                document.getElementById('user-name-error').textContent = 'กรุณากรอกชื่อ';
            }

            // Validate Last Name
            const userLastName = document.getElementById('user-last-name');
            if (userLastName.value.trim() === '') {
                isValid = false;
                userLastName.style.borderColor = 'red';
                document.getElementById('user-last-name-error').textContent = 'กรุณากรอกนามสกุล';
            }

            // Validate Email
            const userEmail = document.getElementById('user-email');
            if (userEmail.value.trim() === '') {
                isValid = false;
                userEmail.style.borderColor = 'red';
                document.getElementById('user-email-error').textContent = 'กรุณากรอกอีเมลล์';
            }

            // Validate Mobile
            const userMobile = document.getElementById('user-mobile');
            if (userMobile.value.trim() === '') {
                isValid = false;
                userMobile.style.borderColor = 'red';
                document.getElementById('user-mobile-error').textContent = 'กรุณากรอกหมายเลขโทรศัพท์';
            }

            // Validate BU
            const userBU = document.getElementById('user-bu-id');
            if (userBU.value === '0') {
                isValid = false;
                userBU.style.borderColor = 'red';
                document.getElementById('user-bu-id-error').textContent = 'กรุณาเลือก BU';
            }

            // Validate Position
            const userPosition = document.getElementById('user-position');
            if (userPosition.value.trim() === '') {
                isValid = false;
                userPosition.style.borderColor = 'red';
                document.getElementById('user-position-error').textContent = 'กรุณากรอกตำแหน่ง';
            }

            // Validate Role
            const userRole = document.getElementById('user-role');
            if (userRole.value === '0') {
                isValid = false;
                userRole.style.borderColor = 'red';
                document.getElementById('user-role-error').textContent = 'กรุณาเลือก Role';
            }

            // Validate Password
            const userPassword = document.getElementById('user-password');

            // Validate Password Confirmation
            const userPasswordConfirm = document.getElementById('user-password-confirm');
            if (userPassword.value.trim() !== userPasswordConfirm.value.trim()) {
                isValid = false;
                userPasswordConfirm.style.borderColor = 'red';
                document.getElementById('user-password-confirm-error').textContent = 'รหัสผ่านไม่ตรงกัน';
            }

            //// Validate Signature
            //if (!detailLogin.getSignatureData() ) {
            //    isValid = false;
            //    document.getElementById('user-sign-error').textContent = 'กรุณาตรวจสอบลายเซ็นต์';
            //}

            if (isValid) {
                const data = {
                    UserID: userId,
                    FirstName: userName.value.trim(),
                    LastName: userLastName.value.trim(),
                    Email: userEmail.value.trim(),
                    MobileNo: userMobile.value.trim(),
                    BUID: userBU.value,
                    JobPosition: userPosition.value.trim(),
                    RoleID: userRole.value,
                    Password: userPassword.value.trim(),
                    ConfirmPassword: userPasswordConfirm.value.trim(),
                    SignUser: detailLogin.getSignatureData()
                };

                detailLogin.EditUser(data);
            }
        });

        $('#open-image-btn').click(() => {
            $('#imageModal').modal('show');
        });
    },
    EditUser: function (data) {
        $.ajax({
            url: baseUrl + 'masteruser/UserEdit',
            type: 'post',
            dataType: 'json',
            data: data,
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'ทำการแก้ไขข้อมูลสำเร็จ',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            //window.location.href = baseUrl + 'masteruser/index';
                            window.location.reload();
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: "ทำการแก้ไขข้อมูลไม่สำเร็จ",
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr, status, error) {
                // do something
                alert(" Coding Error ")
            },
        });
        return false;
    },
    initSignaturePad: () => {
        $('#modal-sign').on('shown.bs.modal', function (e) {
            if (signaturePad == null) {
                let canvas = $("#signature-pad canvas");
                let parentWidth = $(canvas).parent().outerWidth();
                let parentHeight = $(canvas).parent().outerHeight();
                canvas.attr("width", parentWidth + 'px')
                    .attr("height", parentHeight + 'px');
                signaturePad = new SignaturePad(canvas[0], {
                    backgroundColor: 'rgb(255, 255, 255)'
                });
            }
        });

        $(document).on('click', '#modal-sign .clear', function () {
            $('#success-icon').hide();
            signaturePad.clear();
        });

    },
    getSignatureData: () => {
        let dataURL;
        let contentType;
        let storage;
        if (signaturePad && !signaturePad.isEmpty()) {
            dataURL = signaturePad.toDataURL();
            var parts = dataURL.split(';base64,');
            contentType = parts[0].split(":")[1];
            storage = parts[1];
        }
        return storage;
    },
}
