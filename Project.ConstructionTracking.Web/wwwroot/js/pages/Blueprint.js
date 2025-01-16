/*const uploadButton = document.getElementById("uploadButton");*/
/*const fileInput = document.getElementById("imageFile");*/
let canvas, ctx;
let backgroundImage;
/*let PathbackgroundImage;*/

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
                // Save the uploaded image path for later use
/*                PathbackgroundImage = data.imagePath;*/

                // Load the uploaded image onto the canvas
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

// ✅ SaveBlueprintElements
function saveBlueprintElements() {
    const elements = [];

    // Add markers
    markers.forEach(marker => {
        elements.push({
            ProjectID: "0CC60DA9-9AC5-4DF6-871E-B10FB0257B4B",
            ElementType: 39,
            Coordinates: [{ X: marker.x, Y: marker.y }],
 /*           PathProjectImage = PathbackgroundImage,*/
            UnitID: marker.UnitID,
            UserID: "6616524D-8AFD-4925-B956-CB24F1F6DE7D"
        });
    });

    // Add polygons
    polygons.forEach(polygon => {
        elements.push({
            ProjectID: "0CC60DA9-9AC5-4DF6-871E-B10FB0257B4B",
            ElementType: 40,
            Coordinates: polygon.points,
  /*          PathProjectImage = PathbackgroundImage,*/
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

    // Set the background image
    const image = new Image();
    image.src = "/images/Screenshot 2025-01-14 101158.jpg"; // Hardcoded image path for now

    image.onload = () => {
        canvas.width = image.width;
        canvas.height = image.height;
        ctx.drawImage(image, 0, 0);

        // Fetch saved elements from the server
        fetch(baseUrl + `ProjectBluePrint/GetBlueprintElements?projectId=0CC60DA9-9AC5-4DF6-871E-B10FB0257B4B`)
            .then(response => response.json())
            .then(data => {
                data.forEach(element => {
                    if (element.ElementTypeName === "Marker") {
                        markers.push({
                            x: element.Coordinates[0].X,
                            y: element.Coordinates[0].Y,
                            name: element.UnitName
                        });
                    } else if (element.ElementTypeName === "Polygon") {
                        polygons.push({
                            points: element.Coordinates.map(coord => ({
                                x: coord.X,
                                y: coord.Y
                            })), // Ensure coordinates are mapped properly
                            name: element.UnitName
                        });
                    }
                });

                drawCanvas(); // Redraw the canvas with updated elements
            })
            .catch(error => console.error("Error loading blueprint elements:", error));
    };
}

