// -- Global DOM refs
const gradeDropdownButton = document.getElementById('gradeDropdownButton');
const gradeDropdown = document.getElementById('gradeDropdown');
const gradesContainer = document.getElementById('gradesContainer');
const groupsSection = document.getElementById('groupsSection');
const img = document.getElementById('img');
const input = document.getElementById('input');

// Days of the week
const daysOfWeek = ['Saturday', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'];

// Preview thumbnail
input.addEventListener('change', () => {
    if (input.files?.[0]) img.src = URL.createObjectURL(input.files[0]);
});

/* ─────────────────────────────── Dropdown (grades) ───────────────────────── */

// Build grade check-boxes 7-12
for (let grade = 7; grade <= 12; grade++) {
    const li = document.createElement('li');
    li.className = 'grade-item';
    li.innerHTML = `
        <div class="dropdown-item" data-grade="${grade}">
            <div class="form-check m-0">
                <input class="form-check-input grade-checkbox"
                       type="checkbox"
                       name="SelectedGrades"
                       value="${grade}"
                       id="grade${grade}">
                <label class="form-check-label" for="grade${grade}">Grade ${grade}</label>
            </div>
        </div>`;
    gradeDropdown.appendChild(li);
}

const gradeGroupsMap = {};          // grade => groups count

// Toggle checkbox if user clicks anywhere inside list item
gradeDropdown.addEventListener('click', e => {
    let cb = e.target.closest('.grade-item')?.querySelector('.grade-checkbox')
        || e.target.closest('.grade-checkbox');
    if (!cb) return;
    e.stopPropagation();
    cb.checked = !cb.checked;
    cb.dispatchEvent(new Event('change'));
});

// Handle grades selecting / unselecting
function handleGradeChange() {
    const selected = [...document.querySelectorAll('.grade-checkbox')]
        .filter(cb => cb.checked)
        .map(cb => cb.value);

    // Reset UI
    gradesContainer.innerHTML = '';
    Object.keys(gradeGroupsMap).forEach(g => delete gradeGroupsMap[g]);

    if (!selected.length) {
        groupsSection.classList.add('d-none');
        gradeDropdownButton.textContent = 'Select Grades';
        return;
    }

    groupsSection.classList.remove('d-none');

    // For each grade create its box
    selected.forEach(grade => {
        // Card-like box
        const box = document.createElement('div');
        box.className = 'grade-box mb-4 p-3 border rounded';
        box.dataset.grade = grade;
        box.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h5 class="mb-0">Grade ${grade}</h5>
            </div>
            <table class="table table-bordered align-middle">
                <thead>
                    <tr>
                        <th style="width:25%;">Group Name</th>
                        <th style="width:15%;">Price ($)</th>
                        <th style="width:40%;">Days & Time</th>
                        <th style="width:10%;">Actions</th>
                    </tr>
                </thead>
                <tbody id="groupList-${grade}"></tbody>
            </table>
            <button type="button"
                    class="btn btn-sm btn-success"
                    onclick="addNewGroup('${grade}')">+ Add Group</button>`;
        gradesContainer.appendChild(box);

        gradeGroupsMap[grade] = 0;
        addNewGroup(grade);            // create first group automatically
    });

    updateDropdownButtonText();
}

/* ────────────────────────────── Groups per Grade ─────────────────────────── */

function addNewGroup(grade) {
    const list = document.getElementById(`groupList-${grade}`);
    const idx = gradeGroupsMap[grade]++;       // 0-based index

    const daysHtml = daysOfWeek.map((day, dIdx) => `
        <div class="form-check form-check-inline">
            <input class="form-check-input"
                   type="checkbox"
                   id="GradeGroups_${grade}_${idx}_Days_${dIdx}"
                   name="GradeGroups[${grade}][${idx}].Days"
                   value="${day}">
            <label class="form-check-label"
                   for="GradeGroups_${grade}_${idx}_Days_${dIdx}">${day}</label>
        </div>`).join('');

    const row = document.createElement('tr');
    row.innerHTML = `
        <td>
            <input type="text"
                   class="form-control"
                   name="GradeGroups[${grade}][${idx}].GroupName"
                   placeholder="Group name" />
        </td>
        <td>
            <input type="number"
                   class="form-control"
                   name="GradeGroups[${grade}][${idx}].Price"
                   min="0" step="0.01" placeholder="0.00" />
        </td>
        <td>
            <div class="mb-2">${daysHtml}</div>
            <div class="row g-1">
                <div class="col-6">
                    <label class="form-label small mb-0">Start</label>
                    <input type="time"
                           class="form-control"
                           name="GradeGroups[${grade}][${idx}].StartTime" />
                </div>
                <div class="col-6">
                    <label class="form-label small mb-0">End</label>
                    <input type="time"
                           class="form-control"
                           name="GradeGroups[${grade}][${idx}].EndTime" />
                </div>
            </div>
        </td>
        <td class="text-center">
            <button type="button"
                    class="btn btn-sm btn-outline-danger"
                    onclick="deleteGroup(this)">🗑</button>
        </td>`;
    list.appendChild(row);
}

function deleteGroup(btn) {
    const row = btn.closest('tr');
    const tbody = row.parentElement;
    const box = tbody.closest('.grade-box');
    const grade = box.dataset.grade;

    if (tbody.children.length === 1) {
        // remove whole grade box
        document.getElementById(`grade${grade}`).checked = false;
        box.remove();
        delete gradeGroupsMap[grade];
        handleGradeChange();
    } else {
        row.remove();
        updateGroupNumbers(tbody);
    }
    updateDropdownButtonText();
}

/* ───────────────────────────── Utilities ────────────────────────────────── */

// keeps names & ids consistent after removal
function updateGroupNumbers(tbody) {
    const grade = tbody.closest('.grade-box').dataset.grade;
    [...tbody.children].forEach((tr, idx) => {
        tr.querySelectorAll('input').forEach(inp => {
            const newName = inp.name.replace(
                /GradeGroups\[\d+]\[\d+]/,
                `GradeGroups[${grade}][${idx}]`);
            inp.name = newName;

            // days checkboxes: fix id + label
            if (inp.type === 'checkbox') {
                const dayIdx = daysOfWeek.indexOf(inp.value);
                const newId = `GradeGroups_${grade}_${idx}_Days_${dayIdx}`;
                tbody.querySelector(`label[for="${inp.id}"]`)?.setAttribute('for', newId);
                inp.id = newId;
            }
        });
    });
    gradeGroupsMap[grade] = tbody.children.length;
}

// button text
function updateDropdownButtonText() {
    const grades = Object.keys(gradeGroupsMap).length;
    gradeDropdownButton.textContent = grades === 0
        ? 'Select Grades'
        : grades === 1
            ? `Grade ${Object.keys(gradeGroupsMap)[0]} selected`
            : `${grades} Grades selected`;
}

// attach change listener once
document.querySelectorAll('.grade-checkbox')
    .forEach(cb => cb.addEventListener('change', handleGradeChange));

// stop form if no grade/group
document.getElementById('courseForm').addEventListener('submit', e => {
    if (!Object.keys(gradeGroupsMap).length) {
        alert('Please select at least one grade and add groups.');
        e.preventDefault();
    }
});
