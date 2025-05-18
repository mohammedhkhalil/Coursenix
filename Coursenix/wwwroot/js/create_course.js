// In wwwroot/js/create_course.js

const gradeLevelsInput = document.getElementById('gradeLevels');
const groupsSection = document.getElementById('groupsSection');
const gradesContainer = document.getElementById('gradesContainer');
const img = document.getElementById('img');
const input = document.getElementById('input');
const courseForm = document.getElementById('courseForm'); // Get the form element by its ID

// Handle thumbnail image preview
if (input && img) { // Check if elements exist
    input.addEventListener('change', function () {
        if (this.files && this.files[0]) {
            img.src = URL.createObjectURL(this.files[0]);
            img.style.display = 'block'; // Make sure image is visible
            img.alt = 'Thumbnail Preview'; // Add alt text
        } else {
            img.src = '';
            img.style.display = 'none'; // Hide if no file selected
            img.alt = '';
        }
    });
}


// Variables (reset when gradeLevels changes)
// Use a map to track the next available index for *each* grade
let gradeNextIndexMap = {}; // Map: Grade -> Next Index (0-based)

// Event Listeners
if (gradeLevelsInput) {
    gradeLevelsInput.addEventListener('input', handleGradeChange);
}
// The form submit is handled by ASP.NET Core MVC now via the <button type="submit">
// and the form's asp-controller/asp-action. Client-side validation is handled by _ValidationScriptsPartial.
// Remove any JS submit handler that prevents default form submission unless needed for AJAX or custom validation.
// if (courseForm) { courseForm.addEventListener('submit', handleFormSubmit); } // <-- Remove or modify if needed


function handleGradeChange() {
    if (!gradeLevelsInput || !groupsSection || !gradesContainer) return; // Basic safety check

    const gradesText = gradeLevelsInput.value;
    // Split by comma, trim whitespace, filter out empty strings
    const grades = gradesText.split(',').map(g => g.trim()).filter(g => g !== '');

    // Clear previous content
    gradesContainer.innerHTML = '';
    gradeNextIndexMap = {}; // Reset index map

    // Show/hide the groups section
    if (grades.length === 0) {
        groupsSection.classList.add('d-none');
        return;
    } else {
        groupsSection.classList.remove('d-none');
    }


    // Create a box for each grade entered
    grades.forEach(grade => {
        // Sanitize grade string to be safe for IDs and data attributes
        const safeGrade = grade.replace(/[^a-zA-Z0-9-_]/g, '');

        const gradeBox = document.createElement('div');
        gradeBox.className = 'grade-box mb-4 p-3 border rounded';
        gradeBox.dataset.grade = grade; // Store original grade string
        gradeBox.id = `gradeBox-${safeGrade}`; // Give it an ID


        // Use a template literal for innerHTML for easier structure
        // Use safeGrade in IDs to avoid issues with special characters
        gradeBox.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h5 class="mb-0">Grade ${grade}</h5>
                <button type="button" class="btn btn-sm btn-success" onclick="addNewGroup('${grade}')">+ Add Group</button>
            </div>
            <div class="groups-container" id="groupList-${safeGrade}">
                 <!-- Dynamic groups for this grade will be added here -->
            </div>
        `;
        gradesContainer.appendChild(gradeBox);

        // Initialize the next index for this grade and add a default group
        gradeNextIndexMap[grade] = 0; // Start with index 0 for the first group
        addNewGroup(grade); // Add one group by default for this grade
    });
}

function addNewGroup(grade) {
    // Find the container for this specific grade using its safe ID
    const safeGrade = grade.replace(/[^a-zA-Z0-9-_]/g, '');
    const groupContainer = document.getElementById(`groupList-${safeGrade}`);

    if (!groupContainer) {
        console.error(`Group container not found for grade: ${grade}`);
        return;
    }

    // Get the current index for this grade and then increment it for the next group
    const groupIndex = gradeNextIndexMap[grade];
    gradeNextIndexMap[grade]++; // Increment for the next call

    const groupDiv = document.createElement('div');
    groupDiv.className = 'group-card fade-in p-3 my-2';
    // Store the grade and index as data attributes
    groupDiv.dataset.grade = grade;
    groupDiv.dataset.groupIndex = groupIndex; // Store the unique index for this group instance

    // **CRITICAL:** Add 'name' attributes using the model binding convention: Groups[index].PropertyName
    // Ensure each input within a group has a unique name across the *entire form*
    // The model binder uses the index to build the List<GroupViewModel>
    groupDiv.innerHTML = `
        <div class="d-flex justify-content-between">
            <strong class="group-title">Group ${groupIndex + 1}</strong> <!-- Use index+1 for display number -->
            <button type="button" class="btn btn-sm btn-outline-danger delete-group-btn" onclick="deleteGroup(this)">🗑</button>
        </div>
        <!-- Hidden field for Grade - Essential for model binding -->
        <input type="hidden" name="Groups[${groupIndex}].Grade" value="${grade}" />

        <div class="mb-2">
            <label>Group Name</label>
            <!-- Use name="Groups[${groupIndex}].GroupName" if your GroupViewModel/Model has GroupName -->
            <input type="text" class="form-control" name="Groups[${groupIndex}].GroupName" placeholder="Enter group name">
             <!-- Placeholder for validation message -->
             <span data-valmsg-for="Groups[${groupIndex}].GroupName" class="text-danger"></span>
        </div>
        <div class="mb-2">
            <label>Days</label>
            <input type="text" class="form-control" name="Groups[${groupIndex}].Days" placeholder="Tuesday & Thursday">
             <!-- Placeholder for validation message -->
             <span data-valmsg-for="Groups[${groupIndex}].Days" class="text-danger"></span>
        </div>
        <div class="row mb-3">
            <div class="col-md-6 mb-2">
                <label>Time From</label>
                <input type="time" class="form-control" name="Groups[${groupIndex}].StartTime">
                 <!-- Placeholder for validation message -->
                 <span data-valmsg-for="Groups[${groupIndex}].StartTime" class="text-danger"></span>
            </div>
            <div class="col-md-6 mb-2">
                <label>Time To</label>
                <input type="time" class="form-control" name="Groups[${groupIndex}].EndTime">
                 <!-- Placeholder for validation message -->
                 <span data-valmsg-for="Groups[${groupIndex}].EndTime" class="text-danger"></span>
            </div>
        </div>
         <!-- Add placeholders for any other validation messages for group fields -->
         <!-- <span data-valmsg-for="Groups[${groupIndex}].Grade" class="text-danger"></span> -->
    `;

    groupContainer.appendChild(groupDiv);

    // After adding a group, re-run client-side validation unobtrusive setup
    // This ensures the new inputs are included in client-side validation checks
    // Requires jQuery Validation Unobtrusive scripts (_ValidationScriptsPartial)
    if (typeof $.validator !== 'undefined' && typeof $.validator.unobtrusive !== 'undefined') {
        $.validator.unobtrusive.parse(groupDiv); // Parse the newly added groupDiv for validation attributes
    }

    // updateGroupNumbers is not strictly needed for model binding names, but keeps the visible title correct
    // It needs to re-index the visual numbers *within this grade's container*
    updateGroupNumbers(groupContainer);
}

function deleteGroup(button) {
    const groupElement = button.closest('.group-card');
    const groupsContainer = groupElement.parentElement;
    const gradeBox = groupsContainer.closest('.grade-box');
    const grade = gradeBox ? gradeBox.dataset.grade : null; // Get the original grade

    if (!groupElement || !groupsContainer || !gradeBox || grade === null) {
        console.error("Could not determine parent elements or grade for deletion.");
        return;
    }

    const totalGroupsInGrade = groupsContainer.querySelectorAll('.group-card').length;

    // Handle fade-out animation and removal
    groupElement.style.opacity = '0';
    groupElement.style.transform = 'translateY(10px)';
    groupElement.style.transition = 'all 0.3s ease';

    setTimeout(() => {
        groupElement.remove(); // Remove the element from the DOM

        // Decrement the count for this grade *after* removing the element
        // Note: This count was used for adding *new* groups. Re-indexing names is needed for submission.
        // gradeGroupsMap[grade]--; // No longer needed if using reIndexGroupNames

        // If this was the last group for this grade box, remove the grade box
        if (groupsContainer.querySelectorAll('.group-card').length === 0) {
            gradeBox.style.opacity = '0';
            gradeBox.style.transform = 'translateY(10px)';
            gradeBox.style.transition = 'all 0.3s ease';
            // Need a separate timeout for the grade box removal
            setTimeout(() => {
                gradeBox.remove();
                // If removing the last grade box, check if groups section should be hidden
                if (gradesContainer && gradesContainer.querySelectorAll('.grade-box').length === 0) {
                    groupsSection.classList.add('d-none');
                }
                // Remove the grade from the index map as its box is gone
                delete gradeNextIndexMap[grade]; // Clean up the map
            }, 300);

        } else {
            // If there are still groups in this grade box, update their visual numbers
            updateGroupNumbers(groupsContainer);
            // **CRITICAL:** Re-index the names of the remaining group inputs for correct model binding
            reIndexGroupNames(groupsContainer);
        }

        // After deletion and potential re-indexing, re-parse the form for client-side validation
        // This is important if validation summaries or other elements need updating
        if (courseForm && typeof $.validator !== 'undefined' && typeof $.validator.unobtrusive !== 'undefined') {
            $.validator.unobtrusive.parse(courseForm);
        }

    }, 300); // Matches fade-out transition time
}

// Helper function to update visible group numbers (Group 1, Group 2, etc.) *within a container*
function updateGroupNumbers(container) {
    const groupCards = container.querySelectorAll('.group-card');
    groupCards.forEach((card, index) => {
        const title = card.querySelector('.group-title');
        if (title) { // Check if title element exists
            title.textContent = `Group ${index + 1}`;
        }
        // Update the data-group-index attribute as well
        card.dataset.groupIndex = index;
    });
}

// **CRITICAL:** Helper function to re-index input names after a group is deleted
// This is necessary for correct ASP.NET Core model binding of the List<GroupViewModel>
// If a group at index 5 is deleted, the group at index 6 needs its name changed to index 5.
function reIndexGroupNames(container) {
    const groupCards = container.querySelectorAll('.group-card');
    groupCards.forEach((card, index) => {
        // Update the name attribute for each relevant input field within the group card
        card.querySelectorAll('input[name^="Groups["]').forEach(input => {
            const oldName = input.getAttribute('name');
            if (oldName) { // Only update if the input has a name attribute starting with "Groups["
                // Example old name: Groups[5].Days
                // Example new name: Groups[3].Days (if index changed from 5 to 3)
                const propertyName = oldName.split('.').pop(); // Get 'Days', 'StartTime', etc.
                input.setAttribute('name', `Groups[${index}].${propertyName}`);
            }
        });

        // Also update the hidden grade field name
        const hiddenGradeInput = card.querySelector('input[type="hidden"][name^="Groups["]');
        if (hiddenGradeInput) {
            hiddenGradeInput.name = `Groups[${index}].Grade`;
        }
        // Re-parse the individual card for validation after re-indexing
        if (typeof $.validator !== 'undefined' && typeof $.validator.unobtrusive !== 'undefined') {
            $.validator.unobtrusive.parse(card);
        }
    });
    // After re-indexing within a grade container, update the next available index for this grade
    // based on the number of cards remaining.
    const gradeBox = container.closest('.grade-box');
    if (gradeBox) {
        const grade = gradeBox.dataset.grade;
        gradeNextIndexMap[grade] = groupCards.length; // The next group will be added at this index
    }

}


// Optional: If you want to pre-populate the form with data from the model on page load
// (Useful if this view is also used for editing or returning with errors)
// This requires server-side code to populate the model.Groups before rendering the view.

document.addEventListener('DOMContentLoaded', function() {
    // Check if model data exists (e.g., rendered from the server on validation failure)
    // This would require the server to serialize the model.Groups into a JS variable or hidden fields.
    // Example: Add a hidden field like <input type="hidden" id="jsonData" value="@Json.Serialize(Model.Groups)" />
    // const jsonDataElement = document.getElementById('jsonData');
    // if (jsonDataElement && jsonDataElement.value) {
    //     const groupsData = JSON.parse(jsonDataElement.value);
    //     // Logic to iterate through groupsData and call addNewGroup() for each group
    //     // Needs careful handling to add groups to the correct grade box and populate fields.
    // }
});



// The form submit event listener in the HTML (<button type="submit">)
// will now handle the submission. The model binder will try to populate
// the CreateCourseViewModel based on the input names generated by JS.
// _ValidationScriptsPartial handles client-side validation before submission.
// You don't need a separate JS submit handler unless you want to perform
// AJAX submission or very custom client-side logic.

// Example handleFormSubmit function (if you needed client-side validation/AJAX):

function handleFormSubmit(event) {
    // Prevent default form submission if using AJAX or custom validation
     event.preventDefault();

    console.log('Form submitted');

    // Perform client-side validation checks manually if needed
    // if (!validateForm()) { // Implement your validation logic
    //    return; // Stop submission if validation fails
    // }

    // If using AJAX to submit the form:
    // const formData = new FormData(event.target); // Get form data
    // fetch('/Courses/Create', { method: 'POST', body: formData, headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() } })
    //    .then(response => response.json()) // Or response.text() depending on controller return
    //    .then(data => { console.log('Success:', data); })
    //    .catch((error) => { console.error('Error:', error); });
}
