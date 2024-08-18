const detail = {
    init: () => {

        $('#multiple-select-field').select2({
            dropdownParent: $("#partial-modal-form")
        });

        var coll = document.getElementsByClassName("collapsible");
        for (var i = 0; i < coll.length; i++) {
            coll[i].addEventListener("click", function () {
                this.classList.toggle("active");
                var content = this.nextElementSibling;
                if (content.style.display === "block") {
                    content.style.display = "none";
                } else {
                    content.style.display = "block";
                }
            });
        }
            
        const buttons = document.querySelectorAll('#section-button .btn');

        buttons.forEach(button => {
            button.addEventListener('click', function () {
                // Get the target section from the button's data-bs-target attribute
                const targetSelector = button.getAttribute('data-bs-target');
                const targetElement = document.querySelector(targetSelector);

                if (targetElement) {
                    // Scroll to the target element with smooth behavior
                    targetElement.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });

        $("#modal-form").click(() => {
            $("#partial-modal-form").modal('show');
            return false;
        });
        
        $("#modal-group").click(() => {
            $("#partial-modal-group").modal('show');
            return false;
        });
        
        $("#modal-package").click(() => {
            $("#partial-modal-package").modal('show');
            return false;
        });

        $("#modal-checklist").click(() => {
            $("#partial-modal-checklist").modal('show');
            return false;
        });

        $("#show-form-edit").click(() => {
            $("#partial-modal-form").modal('show');
            return false;
        });

        $("#show-group-edit").click(() => {
            $("#partial-modal-group").modal('show');
            return false;
        });

        $("#show-package-edit").click(() => {
            $("#partial-modal-package").modal('show');
            return false;
        });

        $("#show-checklist-edit").click(() => {
            $("#partial-modal-checklist").modal('show');
            return false;
        });

        const checkboxesF = document.querySelectorAll('.checkbox-f');
        handleCheckboxChange(checkboxesF, 'section-2', 'Lv.2 - FormGroup');

        const checkboxesG = document.querySelectorAll('.checkbox-g');
        handleCheckboxChange(checkboxesG, 'section-3', 'Lv.3 - FormPackage');

        const checkboxesP = document.querySelectorAll('.checkbox-p');
        handleCheckboxChange(checkboxesP, 'section-4', 'Lv.4 - FormCheckList');
    },
    AjaxGrid: function () {

    }
}

function handleCheckboxChange(checkboxes, sectionId, prefixText) {
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            // Remove highlight from all rows in this group
            const rows = Array.from(checkboxes).map(cb => cb.closest('tr')).filter(row => row);
            rows.forEach(row => row.classList.remove('highlight'));
            const section = document.getElementById(sectionId);
            if (section) {
                const header = section.querySelector('.collapsible');
                if (header) {
                    header.firstChild.textContent = `${prefixText}`;
                }
            }
            // Uncheck all checkboxes except the current one
            checkboxes.forEach(cb => {
                if (cb !== checkbox) {
                    cb.checked = false;
                }
            });

            // Highlight the row of the checked checkbox and update section
            if (checkbox.checked) {
                const row = checkbox.closest('tr');
                if (row) {
                    row.classList.add('highlight');

                    // Retrieve value from the row (e.g., first cell's text content)
                    const value = row.cells[1].textContent.trim(); // Adjust cell index as needed

                    // Update the collapsible section
                    const section = document.getElementById(sectionId);
                    if (section) {
                        const header = section.querySelector('.collapsible');
                        if (header) {
                            header.firstChild.textContent = `${prefixText} : ${value}`;
                        }
                    }
                }
            }
        });
    });
}


//function fetchData(selectedValue, selectedValue2) {
//    console.log('Fetching data with:', selectedValue, selectedValue2);

//    var formId = selectedValue;
//    var typeId = parseInt(selectedValue2);

//    var data = {
//        formId: formId,
//        typeId: typeId
//    };

//    console.log(data);
//    $.ajax({
//        url: 'FormOverall/ProjectFromList',
//        type: 'POST',
//        data: JSON.stringify(data),
//        dataType: 'json',
//        contentType: 'application/json; charset=utf-8',
//        success: function (data) {
//            console.log(data);
//            return data;
//        },
//        error: function () {
//            alert('Error loading data.');
//            console.log('fail');
//        }
//    });
//}

//function updateTable(data) {
//    var tableBody = $('#tableBody');
//    tableBody.empty();  // Clear existing table rows

//    tableBody.dataTable({

//    })
//}

////function editButtonAction() {
////    alert('Edit button clicked');
////}

////function addButtonAction() {
////    var modal = document.getElementById('modal-report');
////    modal.show();
////}

//const checkboxes = document.querySelectorAll('.checkbox1');

//checkboxes.forEach(checkbox => {
//    checkbox.addEventListener('change', function () {
//        // Remove highlight from all rows
//        document.querySelectorAll('tr').forEach(row => {
//            row.classList.remove('highlight');
//        });

//        // Uncheck all checkboxes except the current one
//        checkboxes.forEach(cb => {
//            if (cb !== checkbox) {
//                cb.checked = false;
//            }
//        });

//        // Add highlight to the checked checkbox's row
//        if (checkbox.checked) {
//            checkbox.parentNode.parentNode.classList.add('highlight');
//        }
//    });
//});

//const formOverall = {
//    init: () => {
//        var selectedValue = $('#select-project-id').val();
//        console.log('Selected value:', selectedValue);

//        var selectedValue2 = $('#select-unit-type').val();
//        console.log('unit value :', selectedValue2);

//        fetchData(selectedValue, selectedValue2);

//        $('#select-project-id').change(function () {
//            selectedValue = $(this).val();
//            console.log('Selected value:', selectedValue);
//            fetchData(selectedValue, selectedValue2);
//        });

//        $('#select-unit-type').change(function () {
//            selectedValue2 = $(this).val();
//            console.log('unit value:', selectedValue2);
//            fetchData(selectedValue, selectedValue2);
//        });
//    },
//    AjaxGrid: function () {
//        tbl
//    }
//}

//document.getElementById('modal-form').addEventListener('click', function (event) {
//    event.stopPropagation();

//    this.setAttribute('data-bs-toggle', 'modal');
//    this.setAttribute('data-bs-target', '#partial-modal-form');

//    // Manually trigger the modal
//    var modal = new bootstrap.Modal(document.getElementById('partial-modal-form'));
//    modal.show();

//    isModalOpen = true; // Set the flag to true when modal is shown

//    // Reset the flag when the modal is hidden
//    document.getElementById('partial-modal-form').addEventListener('hidden.bs.modal', function () {
//        isModalOpen = false;
//    });
//});

//// Listen for clicks on the entire document
//document.addEventListener('click', function () {
//    // Find all elements with the modal-backdrop class and remove them
//    var backdrops = document.querySelectorAll('.modal-backdrop');
//    backdrops.forEach(function (backdrop) {
//        backdrop.classList.remove('fade', 'show');
//        backdrop.remove();
//    });
//});

//document.getElementById('modal-group').addEventListener('click', function (event) {
//    event.stopPropagation();

//    this.setAttribute('data-bs-toggle', 'modal');
//    this.setAttribute('data-bs-target', '#partial-modal-group');

//    // Manually trigger the modal
//    var modal = new bootstrap.Modal(document.getElementById('partial-modal-group'));
//    modal.show();

//    isModalOpen = true; // Set the flag to true when modal is shown

//    // Reset the flag when the modal is hidden
//    document.getElementById('partial-modal-group').addEventListener('hidden.bs.modal', function () {
//        isModalOpen = false;
//    });
//});

//document.getElementById('modal-package').addEventListener('click', function (event) {
//    event.stopPropagation();

//    this.setAttribute('data-bs-toggle', 'modal');
//    this.setAttribute('data-bs-target', '#partial-modal-package');

//    // Manually trigger the modal
//    var modal = new bootstrap.Modal(document.getElementById('partial-modal-package'));
//    modal.show();

//    isModalOpen = true; // Set the flag to true when modal is shown

//    // Reset the flag when the modal is hidden
//    document.getElementById('partial-modal-package').addEventListener('hidden.bs.modal', function () {
//        isModalOpen = false;
//    });
//});

//document.getElementById('modal-checklist').addEventListener('click', function (event) {
//    event.stopPropagation();

//    this.setAttribute('data-bs-toggle', 'modal');
//    this.setAttribute('data-bs-target', '#partial-modal-clist');

//    // Manually trigger the modal
//    var modal = new bootstrap.Modal(document.getElementById('partial-modal-clist'));
//    modal.show();

//    isModalOpen = true; // Set the flag to true when modal is shown

//    // Reset the flag when the modal is hidden
//    document.getElementById('partial-modal-clist').addEventListener('hidden.bs.modal', function () {
//        isModalOpen = false;
//    });
//});
