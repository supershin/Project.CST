function fetchData(selectedValue, selectedValue2) {
    console.log('Fetching data with:', selectedValue, selectedValue2);

    var formId = selectedValue;
    var typeId = parseInt(selectedValue2);

    var data = {
        formId: formId,
        typeId: typeId
    };

    console.log(data);
    $.ajax({
        url: 'FormOverall/ProjectFromList',
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            console.log(data);
            return data;
        },
        error: function () {
            alert('Error loading data.');
            console.log('fail');
        }
    });
}

function updateTable(data) {
    var tableBody = $('#tableBody');
    tableBody.empty();  // Clear existing table rows

    tableBody.dataTable({

    })
}

document.addEventListener("DOMContentLoaded", function () {
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
});

function editButtonAction() {
    alert('Edit button clicked');
}

function addButtonAction() {
    var modal = document.getElementById('modal-report');
    modal.show();
}

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

const formOverall = {
    init: () => {
        var selectedValue = $('#select-project-id').val();
        console.log('Selected value:', selectedValue);

        var selectedValue2 = $('#select-unit-type').val();
        console.log('unit value :', selectedValue2);

        fetchData(selectedValue, selectedValue2);

        $('#select-project-id').change(function () {
            selectedValue = $(this).val();
            console.log('Selected value:', selectedValue);
            fetchData(selectedValue, selectedValue2);
        });

        $('#select-unit-type').change(function () {
            selectedValue2 = $(this).val();
            console.log('unit value:', selectedValue2);
            fetchData(selectedValue, selectedValue2);
        });
    },
    AjaxGrid: function () {
        tbl
    }
}

//document.getElementById('modal-form').addEventListener('click', function (event) {
//    event.stopPropagation();

//        this.setAttribute('data-bs-toggle', 'modal');
//        this.setAttribute('data-bs-target', '#partial-modal-form');

//        // Manually trigger the modal
//        var modal = new bootstrap.Modal(document.getElementById('partial-modal-form'));
//        modal.show();

//        isModalOpen = true; // Set the flag to true when modal is shown

//        // Reset the flag when the modal is hidden
//        document.getElementById('partial-modal-form').addEventListener('hidden.bs.modal', function () {
//            isModalOpen = false;
//        });
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

document.addEventListener("DOMContentLoaded", function () {
    // Get all buttons with id "button-collapse"
    const buttons = document.querySelectorAll('#button-collapse');
    const collapsible = document.querySelector('#target-group');
    const content = document.querySelector('.content-cl');

    const collapsible3 = document.querySelector('#target-package');
    const collapsible4 = document.querySelector('#target-checklist');

    // Attach click event listener to each button
    buttons.forEach(button => {
        button.addEventListener('click', function () {
            const targetGroup = document.getElementById('content-group');
            targetGroup.style.display = 'none';

            // Get the parent row of the clicked button
            const row = this.closest('tr');

            // Get the value from class "value-cell" in this row
            const value = row.querySelector('.value-cell').textContent;

            // Log or do something with the retrieved value
            console.log(value);

            // Check if the length of value exceeds 30 characters
            const truncatedValue = value.length > 30 ? `${value.slice(0, 24)}...` : value;

            // Update the content of the collapsible with ellipsis if necessary
            collapsible.innerHTML = `<i class="fa-solid fa-list-check"></i> &nbsp;Lv.2 ${truncatedValue}`;

            // Show the content-cl with id "target-group" and hide others
            targetGroup.style.display = 'block';

            const targetPackage = document.getElementById('content-package');
            targetPackage.style.display = 'none';

            const targetChecklist = document.getElementById('content-checklist');
            targetChecklist.style.display = 'none';
            
            if (targetPackage.style.display === 'none'){
                collapsible3.innerHTML = `<i class="fa-solid fa-list-check"></i> &nbsp;Lv.3 Form Package`;
                
            }
            if (targetChecklist.style.display === 'none') {
                collapsible4.innerHTML = `<i class="fa-solid fa-list-check"></i> &nbsp;Lv.4 Form Check List`;
            }

            // Apply text-overflow: ellipsis if necessary
            if (value.length > 30) {
                collapsible.style.textOverflow = 'ellipsis';
                collapsible.style.whiteSpace = 'nowrap';
                collapsible.style.overflow = 'hidden';
            } else {
                collapsible.style.textOverflow = 'initial';
                collapsible.style.whiteSpace = 'initial';
                collapsible.style.overflow = 'initial';
            }
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    // Get all buttons with id "button-collapse"
    const buttons = document.querySelectorAll('#button-collapse-2');
    const collapsible = document.querySelector('#tager-package');
    const content = document.querySelector('.content-cl');

    // Attach click event listener to each button
    buttons.forEach(button => {
        button.addEventListener('click', function () {
            const targetGroup = document.getElementById('content-package');
            targetGroup.style.display = 'none';

            // Get the parent row of the clicked button
            const row = this.closest('tr');
            
            // Get the value from class "value-cell" in this row
            let value = row.querySelector('.value-cell').textContent.trim();

            // Log or do something with the retrieved value
            console.log(value);

            // Check if the length of value exceeds 30 characters
            let truncatedValue = value.length > 30 ? `${value.slice(0, 24)}...` : value;
            console.log(truncatedValue);

            // Update the content of the collapsible
            collapsible.innerHTML = `<i class="fa-solid fa-list-check"></i> &nbsp;Lv.3 ${truncatedValue}`;

            // Show the content-cl with id "target-group" and hide others
            targetGroup.style.display = 'block';

            const targetChecklist = document.getElementById('content-checklist');
            targetChecklist.style.display = 'none';
            if (targetChecklist.style.display === 'none') {
                collapsible.innerHTML = `<i class="fa-solid fa-list-check"></i> &nbsp;Lv.3 Form Package`;
            }
            
            // Apply text-overflow: ellipsis if necessary
            if (value.length > 30) {
                collapsible.style.textOverflow = 'ellipsis';
                collapsible.style.whiteSpace = 'nowrap';
                collapsible.style.overflow = 'hidden';
            } else {
                collapsible.style.textOverflow = 'initial';
                collapsible.style.whiteSpace = 'initial';
                collapsible.style.overflow = 'initial';
            }
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    // Get all buttons with id "button-collapse"
    const buttons = document.querySelectorAll('#button-collapse-3');
    const collapsible = document.querySelector('#target-checklist');
    const content = document.querySelector('.content-cl');

    // Attach click event listener to each button
    buttons.forEach(button => {
        button.addEventListener('click', function () {
            const targetGroup = document.getElementById('content-checklist');
            targetGroup.style.display = 'none';

            // Get the parent row of the clicked button
            const row = this.closest('tr');

            // Get the value from class "value-cell" in this row
            let value = row.querySelector('.value-cell').textContent.trim();

            // Log or do something with the retrieved value
            console.log(value);

            // Check if the length of value exceeds 30 characters
            let truncatedValue = value.length > 30 ? `${value.slice(0, 24)}...` : value;
            console.log(truncatedValue);

            // Update the content of the collapsible
            collapsible.innerHTML = `<i class="fa-solid fa-list-check"></i> &nbsp;Lv.4 ${truncatedValue}`;

            // Show the content-cl with id "target-group" and hide others
            targetGroup.style.display = 'block';

            // Apply text-overflow: ellipsis if necessary
            if (value.length > 30) {
                collapsible.style.textOverflow = 'ellipsis';
                collapsible.style.whiteSpace = 'nowrap';
                collapsible.style.overflow = 'hidden';
            } else {
                collapsible.style.textOverflow = 'initial';
                collapsible.style.whiteSpace = 'initial';
                collapsible.style.overflow = 'initial';
            }
        });
    });
});