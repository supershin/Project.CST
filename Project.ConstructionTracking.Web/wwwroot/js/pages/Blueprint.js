const uploadButton = document.getElementById("uploadButton");
const fileInput = document.getElementById("imageFile");
let canvas, ctx;
let backgroundImage;

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
uploadButton.addEventListener("click", (event) => {
    event.preventDefault();

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
        .catch((err) => console.error(err));
});

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
        markers.push({ x: tempX, y: tempY, name: selectedText });
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
        polygons.push({ points: [...currentPolygon], name: selectedText });
        currentPolygon = [];
        drawCanvas();
    });

    unitModal.show();
}

// ✅ Load Dropdown Options
function loadDropdownOptions(callback) {
    const PROJECT_ID = "0CC60DA9-9AC5-4DF6-871E-B10FB0257B4B";

    fetch(baseUrl + `ProjectBluePrint/GetDDLUnitList?projectId=${PROJECT_ID}`)
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
                callback();  // Execute the save function
                unitModal.hide();  // Close the modal using Bootstrap's hide() method
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
        ctx.arc(marker.x, marker.y, 5, 0, Math.PI * 2);
        ctx.fillStyle = "red";
        ctx.fill();
        ctx.closePath();

        ctx.font = "12px Arial";
        ctx.fillStyle = "black";
        ctx.fillText(marker.name, marker.x + 8, marker.y - 8);
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
