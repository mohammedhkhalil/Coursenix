const gradeLevelsInput = document.getElementById('gradeLevels');
const groupsSection = document.getElementById('groupsSection');
const gradesContainer = document.getElementById('gradesContainer');
const img=document.getElementById('img')
const input=document.getElementById('input')

input.addEventListener('change',function(){
    img.src=URL.createObjectURL(input.files[0]);
})
// Variables
let gradeGroupsMap = {};

// Event Listeners
gradeLevelsInput.addEventListener('input', handleGradeChange);
courseForm.addEventListener('submit', handleFormSubmit);

function handleGradeChange() {
  const gradesText = gradeLevelsInput.value;
  const grades = gradesText.split(',').map(g => g.trim()).filter(g => g !== '');

  gradesContainer.innerHTML = '';
  gradeGroupsMap = {};

  if (grades.length === 0) {
    groupsSection.classList.add('d-none');
    return;
  }

  groupsSection.classList.remove('d-none');

  grades.forEach(grade => {
    const gradeBox = document.createElement('div');
    gradeBox.className = 'grade-box mb-4 p-3 border rounded';
    gradeBox.dataset.grade = grade;

    gradeBox.innerHTML = `
      <div class="d-flex justify-content-between align-items-center mb-2">
        <h5 class="mb-0">Grade ${grade}</h5>
        <button type="button" class="btn btn-sm btn-success" onclick="addNewGroup('${grade}')">+ Add Group</button>
      </div>
      <div class="groups-container" id="groupList-${grade}"></div>
    `;
    gradesContainer.appendChild(gradeBox);

    gradeGroupsMap[grade] = 0;
    addNewGroup(grade); // default group
  });
}

function addNewGroup(grade) {
  const groupContainer = document.getElementById(`groupList-${grade}`);
  gradeGroupsMap[grade]++;
  const groupIndex = gradeGroupsMap[grade];

  const groupDiv = document.createElement('div');
  groupDiv.className = 'group-card fade-in p-3 my-2';
  groupDiv.dataset.groupId = groupIndex;

  groupDiv.innerHTML = `
    <div class="d-flex justify-content-between">
      <strong class="group-title">Group ${groupIndex}</strong>
      <button type="button" class="btn btn-sm btn-outline-danger" onclick="deleteGroup(this)">🗑</button>
    </div>
    <div class="mb-2">
      <label>Group Name</label>
      <input type="text" class="form-control" placeholder="Enter group name">
    </div>
    <div class="mb-2">
      <label>Days</label>
      <input type="text" class="form-control" placeholder="Tuesday & Thursday">
    </div>
    <div class="row mb-3">
      <div class="col-md-6 mb-2">
        <label>Time From</label>
        <input type="time" class="form-control">
      </div>
      <div class="col-md-6 mb-2">
        <label>Time To</label>
        <input type="time" class="form-control">
      </div>
    </div>
  `;

  groupContainer.appendChild(groupDiv);
  updateGroupNumbers(groupContainer);
}

function deleteGroup(button) {
  const groupElement = button.closest('.group-card');
  const groupsContainer = groupElement.parentElement;
  const gradeBox = groupsContainer.closest('.grade-box');

  const totalGroups = groupsContainer.querySelectorAll('.group-card').length;

  if (totalGroups === 1) {
    gradeBox.style.opacity = '0';
    gradeBox.style.transform = 'translateY(10px)';
    gradeBox.style.transition = 'all 0.3s ease';
    setTimeout(() => {
      gradeBox.remove();
    }, 300);
  } else {
    groupElement.style.opacity = '0';
    groupElement.style.transform = 'translateY(10px)';
    groupElement.style.transition = 'all 0.3s ease';
    setTimeout(() => {
      groupElement.remove();
      updateGroupNumbers(groupsContainer);
    }, 300);
  }
}

function updateGroupNumbers(container) {
  const groupCards = container.querySelectorAll('.group-card');
  groupCards.forEach((card, index) => {
    const title = card.querySelector('.group-title');
    title.textContent = `Group ${index + 1}`;
  });
}
