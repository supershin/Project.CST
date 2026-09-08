const projectImage = {
    selectedFile: null,

    init: () => {
        $('#btn-search-project-image').click(() => {
            projectImage.AjaxList();
            return false;
        });

        $('#form-search-project-image').submit(() => {
            projectImage.AjaxList();
            return false;
        });

        $(document).on('click', "button[data-action='upload-project-image']", function (e) {
            const btn = $(e.currentTarget);

            $('#upload-project-id').val(btn.attr('data-id'));
            $('#upload-project-name').text(btn.attr('data-code') + ' - ' + btn.attr('data-name'));

            projectImage.ResetUploadForm();
            $('#modal-upload-project-image').modal('show');
            return false;
        });

        $(document).on('click', "button[data-action='remove-project-image']", function (e) {
            const btn = $(e.currentTarget);
            const projectId = btn.attr('data-id');

            Swal.fire({
                title: 'ยืนยันการลบรูปภาพ',
                text: 'ต้องการลบรูปภาพของโครงการ ' + btn.attr('data-name') + ' ใช่หรือไม่',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'ลบรูปภาพ',
                cancelButtonText: 'ยกเลิก'
            }).then((result) => {
                if (result.isConfirmed) {
                    projectImage.RemoveProjectImage(projectId);
                }
            });
            return false;
        });

        // เลือกไฟล์ผ่าน drop zone (กันคลิกซ้อนจาก input ที่อยู่ภายใน drop zone)
        $('#drop-zone').click(function (e) {
            if (e.target.id === 'file-input') {
                return;
            }
            $('#file-input').click();
        });

        $('#drop-zone').on('dragover', function (e) {
            e.preventDefault();
            $(this).addClass('dragover');
        });

        $('#drop-zone').on('dragleave drop', function () {
            $(this).removeClass('dragover');
        });

        $('#drop-zone').on('drop', function (e) {
            e.preventDefault();
            const files = e.originalEvent.dataTransfer.files;
            if (files.length > 0) {
                projectImage.SetPreview(files[0]);
            }
        });

        $('#file-input').change(function () {
            if (this.files.length > 0) {
                projectImage.SetPreview(this.files[0]);
            }
        });

        $('#btn-save-project-image').click(() => {
            const projectId = $('#upload-project-id').val();

            if (!projectImage.selectedFile) {
                Swal.fire({ title: 'กรุณาเลือกรูปภาพ', icon: 'warning', confirmButtonText: 'OK' });
                return false;
            }

            projectImage.SaveProjectImage(projectId, projectImage.selectedFile);
            return false;
        });

        projectImage.AjaxList();
    },

    ResetUploadForm: function () {
        projectImage.selectedFile = null;
        $('#file-input').val('');
        $('#preview-image').attr('src', '');
        $('#preview-wrapper').hide();
    },

    SetPreview: function (file) {
        projectImage.selectedFile = file;

        const reader = new FileReader();
        reader.onload = function (e) {
            $('#preview-image').attr('src', e.target.result);
            $('#preview-wrapper').show();
        };
        reader.readAsDataURL(file);
    },

    AjaxList: function () {
        $.ajax({
            url: baseUrl + 'ProjectImage/GetProjectImageList',
            type: 'get',
            data: { strSearch: $('#strSearch').val() },
            success: function (resp) {
                $('#project-image-list').html(resp);
            },
            error: function () {
                Swal.fire({
                    title: 'Error!',
                    text: 'ไม่สามารถโหลดข้อมูลโครงการได้',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
        return false;
    },

    SaveProjectImage: function (projectId, file) {
        const formData = new FormData();
        formData.append('ProjectID', projectId);
        formData.append('Image', file);

        $('#btn-save-project-image').prop('disabled', true);

        $.ajax({
            url: baseUrl + 'ProjectImage/SaveProjectImage',
            type: 'post',
            data: formData,
            processData: false,
            contentType: false,
            success: function (resp) {
                $('#btn-save-project-image').prop('disabled', false);

                if (resp.success) {
                    $('#modal-upload-project-image').modal('hide');
                    Swal.fire({
                        title: 'Success!',
                        text: resp.message,
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        projectImage.AjaxList();
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: resp.message,
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function () {
                $('#btn-save-project-image').prop('disabled', false);
                Swal.fire({
                    title: 'Error!',
                    text: 'ทำรายการไม่สำเร็จ',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
        return false;
    },

    RemoveProjectImage: function (projectId) {
        $.ajax({
            url: baseUrl + 'ProjectImage/RemoveProjectImage',
            type: 'post',
            dataType: 'json',
            data: { ProjectID: projectId },
            success: function (resp) {
                if (resp.success) {
                    Swal.fire({
                        title: 'Success!',
                        text: resp.message,
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        projectImage.AjaxList();
                    });
                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: resp.message,
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function () {
                Swal.fire({
                    title: 'Error!',
                    text: 'ทำรายการไม่สำเร็จ',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
        return false;
    },
}
