const gradeDropdownButton = document.getElementById('gradeDropdownButton');
const gradeDropdown = document.getElementById('gradeDropdown');
const gradesContainer = document.getElementById('gradesContainer');
const groupsSection = document.getElementById('groupsSection');
const img = document.getElementById('img');
const input = document.getElementById('input');

// Days of the week array
const daysOfWeek = ['Saturday', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'];

// Show preview of uploaded thumbnail image
input.addEventListener('change', function () {
    if (input.files && input.files[0]) {
        img.src = URL.createObjectURL(input.files[0]);
    }
});

// Generate grades 7 to 12 checkboxes inside the dropdown
for (let grade = 7; grade <= 12; grade++) {
    const li = document.createElement('li');
    li.classList.add('grade-item');
    li.innerHTML = `
                <div class="dropdown-item" data-grade="${grade}">
                    <div class="form-check m-0">
                        <input class="form-check-input grade-checkbox" type="checkbox" name="SelectedGrades" value="${grade}" id="grade${grade}">
                        <label class="form-check-label" for="grade${grade}">Grade ${grade}</label>
                    </div>
                </div>
            `;
    gradeDropdown.appendChild(li);
}

let gradeGroupsMap = {};

gradeDropdown.addEventListener('click', (e) => {
    const target = e.target;
    if (target.classList.contains('grade-checkbox') || target.closest('.grade-item')) {
        e.stopPropagation();
        let checkbox;
        if (target.classList.contains('grade-checkbox')) {
            checkbox = target;
        } else {
            checkbox = target.querySelector('.grade-checkbox');
        }
        checkbox.checked = !checkbox.checked;
        checkbox.dispatchEvent(new Event('change'));
    }
});

function handleGradeChange() {
    const selectedGrades = Array.from(document.querySelectorAll('.grade-checkbox'))
        .filter(checkbox => checkbox.checked)
        .map(checkbox => checkbox.value);

    gradesContainer.innerHTML = '';
    gradeGroupsMap = {}; // Reset map

    if (selectedGrades.length === 0) {
        groupsSection.classList.add('d-none');
        gradeDropdownButton.textContent = 'Select Grades';
        return;
    }

    groupsSection.classList.remove('d-none');

    selectedGrades.forEach(grade => {
        const gradeBox = document.createElement('div');
        gradeBox.className = 'grade-box mb-4 p-3 border rounded';
        gradeBox.dataset.grade = grade;

        gradeBox.innerHTML = `
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h5 class="mb-0">Grade ${grade}</h5>
                    </div>
                    <table class="table table-bordered align-middle">
                        <thead>
                            <tr>
                                <th style="width: 40%;">Group Name</th>
                                <th style="width: 20%;">Price ($)</th>
                                <th style="width: 30%;">Days</th>
                                <th style="width: 10%;">Actions</th>
                            </tr>
                        </thead>
                        <tbody id="groupList-${grade}"></tbody>
                    </table>
                    <button type="button" class="btn btn-sm btn-success" onclick="addNewGroup('${grade}')">+ Add Group</button>
                `;

        gradesContainer.appendChild(gradeBox);

        gradeGroupsMap[grade] = 0; // Initialize count for this grade
        addNewGroup(grade); // Add default first group
    });

    updateDropdownButtonText();
}

function addNewGroup(grade) {
    const groupList = document.getElementById(`groupList-${grade}`);
    const groupCount = ++gradeGroupsMap[grade]; // Increment and use

    // Generate checkboxes for days
    let daysCheckboxesHtml = daysOfWeek.map((day, index) => `
                <div class="form-check form-check-inline">
                    <input class="form-check-input" type="checkbox" 
                           id="GradeGroups_${grade}_${groupCount - 1}_Days_${index}" 
                           name="GradeGroups[${grade}][${groupCount - 1}].Days" 
                           value="${day}">
                    <label class="form-check-label" for="GradeGroups_${grade}_${groupCount - 1}_Days_${index}">${day}</label>
                </div>
            `).join('');

    const tr = document.createElement('tr');
    tr.dataset.groupId = groupCount;

    tr.innerHTML = `
                <td>
                    <input type="text" class="form-control" name="GradeGroups[${grade}][${groupCount - 1}].GroupName" placeholder="Enter group name" />
                </td>
                <td>
                    <input type="number" class="form-control" name="GradeGroups[${grade}][${groupCount - 1}].Price" min="0" step="0.01" placeholder="Price" />
                </td>
                <td>
                    <div>${daysCheckboxesHtml}</div>
                </td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-outline-danger" onclick="deleteGroup(this)">🗑</button>
                </td>
            `;

    groupList.appendChild(tr);
    updateGroupNumbers(groupList);
}

function deleteGroup(button) {
    const tr = button.closest('tr');
    const tbody = tr.parentElement;
    const gradeBox = tbody.closest('.grade-box');
    const grade = gradeBox.dataset.grade;

    if (tbody.children.length === 1) {
        gradeBox.style.opacity = '0';
        gradeBox.style.transform = 'translateY(10px)';
        gradeBox.style.transition = 'all 0.3s ease';
        setTimeout(() => {
            gradeBox.remove();
            const gradeCheckbox = document.getElementById(`grade${grade}`);
            if (gradeCheckbox) gradeCheckbox.checked = false;
            delete gradeGroupsMap[grade]; // Remove grade from map
            handleGradeChange(); // Re-evaluate selected grades and UI
        }, 300);
    } else {
        tr.remove();
        updateGroupNumbers(tbody);
    }
    updateDropdownButtonText();
}

// Ensure the indexes in name attributes remain sequential
function updateGroupNumbers(tbody) {
    const gradeBox = tbody.closest('.grade-box');
    const grade = gradeBox.dataset.grade;

    const trs = Array.from(tbody.querySelectorAll('tr'));
    trs.forEach((tr, idx) => {
        const inputsGroupName = tr.querySelectorAll(`input[name^="GradeGroups[${grade}]"]`);
        inputsGroupName.forEach(input => {
            const name = input.name;
            const newName = name.replace(/GradeGroups\[\d+\]\[\d+\]/, `GradeGroups[${grade}][${idx}]`);
            input.name = newName;

            // Also update ids for Days checkboxes and their labels
            if (input.type === 'checkbox') {
                input.id = `GradeGroups_${grade}_${idx}_Days_${input.value ? daysOfWeek.indexOf(input.value) : ''}`;
                const label = tbody.querySelector(`label[for="${input.id}"]`);
                if (label) label.htmlFor = input.id;
            }
        });
    });

    // Update count map to the current number of groups
    gradeGroupsMap[grade] = trs.length;
}

function updateDropdownButtonText() {
    const selectedGradesCount = Object.keys(gradeGroupsMap).length;
    if (selectedGradesCount === 0) {
        gradeDropdownButton.textContent = 'Select Grades';
    } else if (selectedGradesCount === 1) {
        const grade = Object.keys(gradeGroupsMap)[0];
        gradeDropdownButton.textContent = `Grade ${grade} selected`;
    } else {
        gradeDropdownButton.textContent = `${selectedGradesCount} Grades selected`;
    }
}

document.querySelectorAll('.grade-checkbox').forEach(cb => {
    cb.addEventListener('change', handleGradeChange);
});

// Prevent submit if no grades selected
document.getElementById('courseForm').addEventListener('submit', function (e) {
    if (Object.keys(gradeGroupsMap).length === 0) {
        alert('Please select at least one grade and add at least one group.');
        e.preventDefault();
        return;
    }
});