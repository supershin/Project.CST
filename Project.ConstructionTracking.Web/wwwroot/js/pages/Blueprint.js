let canvas, ctx;
let backgroundImage;
let SelectedProjectID;
let ClickProjectFloorPlanID;
// Store markers and polygons
let markers = [];
let polygons = [];
let currentPolygon = [];
let activeTool = "marker"; // Default tool

// Variables for the modal
let unitModal = new bootstrap.Modal(document.getElementById("unitModal"));
let modalSaveButton = document.getElementById("modalSaveButton");
let unitDropdown = document.getElementById("unitDropdown");
let tempX, tempY;

// ✅ Tool Buttons
const toolButtons = {
    marker: document.getElementById("markerTool"),
    polygon: document.getElementById("polygonTool"),
    undo: document.getElementById("undoButton")
};

// ✅ Set Active Navigation
function setActiveNav() {
    const navItems = document.querySelectorAll('.nav-item');

    navItems.forEach(item => item.classList.remove('active'));

    const activeItem = document.getElementById('nav-ProjectBluePrint');
    if (activeItem) {
        activeItem.classList.add('active');
    }
}

// ✅ Tool Selection
function switchTool(tool) {
    activeTool = tool;
    console.log(`Active tool: ${tool}`);
}

// ✅ Upload Blueprint Image
function uploadBlueprint() {
    const fileInput = document.getElementById("imageFile");
    const file = fileInput.files[0];
    if (!file) {
        alert("Please select a blueprint image to upload.");
        return;
    }

    const formData = new FormData();
    formData.append("imageFile", file);

    fetch(baseUrl + "ProjectBluePrint/UploadBlueprint", {
        method: "POST",
        body: formData,
    })
        .then((response) => response.json())
        .then((data) => {
            if (data.success) {
                loadCanvas(data.imagePath);
            } else {
                alert(data.message);
            }
        })
        .catch((err) => console.error("Error uploading blueprint:", err));
}

// ✅ Load Canvas with the Uploaded Image
function loadCanvas(imagePath) {
    canvas = document.getElementById("blueprintCanvas");
    ctx = canvas.getContext("2d");

    const image = new Image();
    image.src = imagePath;

    image.onload = () => {
        canvas.width = image.width;
        canvas.height = image.height;
        ctx.drawImage(image, 0, 0);

        backgroundImage = image; // Save the image in a global variable
    };

    initCanvasInteraction();
}

// ✅ Initialize Canvas Interaction
function initCanvasInteraction() {
    canvas.addEventListener("click", (event) => {
        const rect = canvas.getBoundingClientRect();
        const x = event.clientX - rect.left;
        const y = event.clientY - rect.top;

        if (activeTool === "marker") {
            addMarker(x, y);
        } else if (activeTool === "polygon") {
            addPolygonPoint(x, y);
        }
    });

    // Keyboard shortcut for Undo (Ctrl + Z)
    document.addEventListener("keydown", (event) => {
        if (event.ctrlKey && event.key === "z") {
            undoLastAction();
        }
    });
}

// ✅ Add Marker
function addMarker(x, y) {
    tempX = x;
    tempY = y;

    loadDropdownOptions(() => {
        const selectedText = unitDropdown.options[unitDropdown.selectedIndex].text;
        const selectedID = unitDropdown.options[unitDropdown.selectedIndex].value;
        markers.push({ x: tempX, y: tempY, name: selectedText, UnitID: selectedID });
        debugger
        drawCanvas();
    });

    unitModal.show();
}

// ✅ Add Polygon Point
function addPolygonPoint(x, y) {
    currentPolygon.push({ x, y });
    drawCanvas();

    // Close the polygon if the first and last points are near
    if (currentPolygon.length > 2) {
        const firstPoint = currentPolygon[0];
        const lastPoint = currentPolygon[currentPolygon.length - 1];

        if (Math.abs(firstPoint.x - lastPoint.x) < 10 && Math.abs(firstPoint.y - lastPoint.y) < 10) {
            completePolygon();
        }
    }
}

// ✅ Complete Polygon
function completePolygon() {
    loadDropdownOptions(() => {
        const selectedText = unitDropdown.options[unitDropdown.selectedIndex].text;
        const selectedID = unitDropdown.options[unitDropdown.selectedIndex].value;
        polygons.push({ points: [...currentPolygon], name: selectedText, UnitID: selectedID });
        currentPolygon = [];
        drawCanvas();
    });

    unitModal.show();
}

// ✅ Load Dropdown Options
function loadDropdownOptions(callback) {
    fetch(baseUrl + `ProjectBluePrint/GetDDLUnitList?projectId=${SelectedProjectID}`)
        .then(response => response.json())
        .then(data => {
            unitDropdown.innerHTML = "";
            data.forEach(unit => {
                const option = document.createElement("option");
                option.value = unit.ValueGuid;
                option.textContent = unit.Text;
                unitDropdown.appendChild(option);
            });

            // When the Save button is clicked
            modalSaveButton.onclick = () => {              
                unitModal.hide();  // Close the modal using Bootstrap's hide() method
                callback();  // Execute the save function
            };
        })
        .catch(error => console.error("Error fetching unit list:", error));
}

// ✅ Draw Canvas
function drawCanvas() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (backgroundImage) {
        ctx.drawImage(backgroundImage, 0, 0);
    }

    drawMarkers();
    polygons.forEach(polygon => drawPolygon(polygon, "green"));
    drawPolygon({ points: currentPolygon }, "blue");
}

// ✅ Draw Markers
function drawMarkers() {
    markers.forEach(marker => {
        ctx.beginPath();
        ctx.arc(marker.x, marker.y, 8, 0, Math.PI * 2); // Increased radius from 5 to 8
        ctx.fillStyle = "red";
        ctx.fill();
        ctx.closePath();

        ctx.font = "14px Arial"; // Increased font size from 12px to 14px
        ctx.fillStyle = "black";
        ctx.fillText(marker.name, marker.x + 10, marker.y - 10); // Adjusted text position for better alignment
    });
}


// ✅ Draw Polygons
function drawPolygon(polygon, color) {
    if (polygon.points.length < 2) return;

    ctx.beginPath();
    ctx.moveTo(polygon.points[0].x, polygon.points[0].y);

    for (let i = 1; i < polygon.points.length; i++) {
        ctx.lineTo(polygon.points[i].x, polygon.points[i].y);
    }

    ctx.closePath();
    ctx.strokeStyle = color;
    ctx.lineWidth = 2;
    ctx.stroke();

    const centerX = polygon.points.reduce((sum, point) => sum + point.x, 0) / polygon.points.length;
    const centerY = polygon.points.reduce((sum, point) => sum + point.y, 0) / polygon.points.length;

    ctx.font = "12px Arial";
    ctx.fillStyle = "black";
    ctx.fillText(polygon.name, centerX, centerY - 10);
}

// ✅ Undo Last Action
function undoLastAction() {
    if (activeTool === "marker" && markers.length > 0) {
        markers.pop();
    } else if (activeTool === "polygon" && currentPolygon.length > 0) {
        currentPolygon.pop();
    } else if (polygons.length > 0 && currentPolygon.length === 0) {
        polygons.pop();
    }

    drawCanvas();
}

// ✅ SaveBlueprintElements
function saveBlueprintElements() {
    const elements = [];

    // Add markers
    markers.forEach(marker => {
        elements.push({
            ProjectFloorPlanID: ClickProjectFloorPlanID,
            ElementType: 39,
            Coordinates: [{ X: marker.x, Y: marker.y }],
            UnitID: marker.UnitID,
            UserID: "6616524D-8AFD-4925-B956-CB24F1F6DE7D"
        });
    });

    // Add polygons
    polygons.forEach(polygon => {
        elements.push({
            ProjectFloorPlanID: ClickProjectFloorPlanID,
            ElementType: 40,
            Coordinates: polygon.points,
            UnitID: polygon.UnitID,
            UserID: "6616524D-8AFD-4925-B956-CB24F1F6DE7D"
        });
    });

    fetch(baseUrl + "ProjectBluePrint/SaveBlueprintElements", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(elements)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            alert("Data saved successfully!");
        } else {
            alert("Failed to save data.");
        }
    })
    .catch(error => console.error("Error saving data:", error));
}


// ✅ Load Blueprint Elements
function loadBlueprintElements() {
    const canvas = document.getElementById("blueprintCanvas");
    const ctx = canvas.getContext("2d");

    // Reset markers and polygons
    markers = [];
    polygons = [];

    fetch(baseUrl + `ProjectBluePrint/GetBlueprintElements?projectId=${ClickProjectFloorPlanID}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(element => {
                if (element.ElementTypeName === "Marker") {
                    markers.push({
                        x: element.Coordinates[0].X,
                        y: element.Coordinates[0].Y,
                        name: element.UnitName,
                        UnitID: element.UnitID
                    });
                } else if (element.ElementTypeName === "Polygon") {
                    polygons.push({
                        points: element.Coordinates.map(coord => ({
                            x: coord.X,
                            y: coord.Y
                        })), // Ensure coordinates are mapped properly
                        name: element.UnitName,
                        UnitID: element.UnitID
                    });
                }
            });

            drawCanvas(); // Redraw the canvas with updated elements
        })
        .catch(error => console.error("Error loading blueprint elements:", error));
}

// ✅ Selecte onchange DDL Project
function handleProjectChange() {
    const projectSelect = $("#projectSelect");
    const projectId = projectSelect.val(); // Get selected project ID
    const container = $("#projectImageContainer"); // Target container for fetched data
    const partialContainer = $("#partialContainer"); // Container for the partial view
    SelectedProjectID = projectId;

    if (!projectId) {
        // If no project is selected, hide the partial container and clear other content
        partialContainer.hide();
        container.empty().html("<p class='text-center'>กรุณาเลือกโครงการ</p>");
        return;
    }    

    $.ajax({
        url: `${baseUrl}ProjectBluePrint/GetListImageProjectFloorPlan`,
        type: "GET", // Use HTTP GET method
        data: { ProjectID: projectId }, // Send the project ID as query parameters
        beforeSend: function () {
            // Clear the container before the request
            container.empty();
        },
        success: function (html) {
            container.html(html); // Update the container with the fetched partial
            partialContainer.show();
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
            container.html("<p class='text-danger'>เกิดข้อผิดพลาดในการโหลดข้อมูล</p>");
        },
    });
}

// ✅ Dropzone insert new image of Project Blue print
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

// ✅ On Click Insert New Image Project Floor Plan
function onClickInsertImageProjectFloorPlan() {
    var files = $('#file-input')[0].files;
    var formData = new FormData();
    formData.append('ProjectID', SelectedProjectID);

    // Rescale images before uploading
    if (files.length > 0) {
        var promises = [];

        for (var i = 0; i < files.length; i++) {
            promises.push(resizeImage(files[i], 1255, 665));
        }

        Promise.all(promises).then(function (resizedFiles) {
            // Append resized images to FormData
            for (var j = 0; j < resizedFiles.length; j++) {
                formData.append('Images', resizedFiles[j], resizedFiles[j].name);
            }

            showLoadingAlert();

            // Perform the AJAX request
            $.ajax({
                url: baseUrl + 'ProjectBluePrint/InsertImageProjectFloorPlan',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    Swal.close();
                    if (response.success) {
                        showSuccessAlert('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', function () {
                            // Reload or update UI if necessary
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
        }).catch(function (error) {
            console.error('Error resizing images:', error);
        });
    }
}


function resizeImage(file, targetWidth, targetHeight) {
    return new Promise(function (resolve, reject) {
        var reader = new FileReader();

        reader.onload = function (event) {
            var img = new Image();
            img.onload = function () {
                var canvas = document.createElement('canvas');
                var ctx = canvas.getContext('2d');

                // Set canvas size to target dimensions
                canvas.width = targetWidth;
                canvas.height = targetHeight;

                // Draw the image to the canvas with the new size
                ctx.drawImage(img, 0, 0, targetWidth, targetHeight);

                // Convert the canvas back to a Blob
                canvas.toBlob(function (blob) {
                    // Resolve with a File object to maintain file properties
                    var resizedFile = new File([blob], file.name, { type: file.type });
                    resolve(resizedFile);
                }, file.type);
            };

            img.onerror = function () {
                reject('Failed to load image for resizing');
            };

            img.src = event.target.result;
        };

        reader.onerror = function () {
            reject('Failed to read file for resizing');
        };

        reader.readAsDataURL(file);
    });
}


// ✅ Click Open Show Image Project Floor Plan
function onClickOpenShowImageProjectFloorPlan(imagePath, ProjectFloorPlanID) {
    ClickProjectFloorPlanID = ProjectFloorPlanID;
    loadCanvas(imagePath);
    loadBlueprintElements()
}


