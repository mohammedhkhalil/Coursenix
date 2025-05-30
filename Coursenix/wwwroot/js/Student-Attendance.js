document.addEventListener('DOMContentLoaded', function () {
    // Search functionality
    const searchInput = document.getElementById('studentSearch');
    const studentRows = document.querySelectorAll('.attendance-table tbody tr');

    searchInput.addEventListener('input', function () {
        const searchTerm = this.value.toLowerCase().trim();

        studentRows.forEach(row => {
            const studentName = row.querySelector('td:first-child').textContent.toLowerCase();
            if (studentName.includes(searchTerm)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    });

    document.addEventListener('DOMContentLoaded', function () {
        const deleteButtons = document.querySelectorAll('.delete-btn');
        const selectedIds = [];

        deleteButtons.forEach(button => {
            button.addEventListener('click', function () {
                const studentId = this.getAttribute('data-student-id'); // ?? ??? id ?? ?? data attribute
                const row = this.closest('tr');

                row.style.backgroundColor = '#ffebee';
                row.style.transition = 'background-color 0.3s';

                if (confirm('Are you sure you want to delete this student?')) {
                    setTimeout(() => {
                        row.style.opacity = '0';
                        row.style.transition = 'opacity 0.5s';

                        setTimeout(() => {
                            row.remove();

                            // Add to selected ids list
                            if (!selectedIds.includes(studentId)) {
                                selectedIds.push(studentId);
                            }
                        }, 500);
                    }, 300);
                } else {
                    setTimeout(() => {
                        row.style.backgroundColor = '';
                    }, 300);
                }
            });
        });

        const saveButton = document.querySelector('.save-btn');
        const groupId = document.getElementById('pageMain').dataset.groupId;

        saveButton.addEventListener('click', function (e) {
            e.preventDefault();

            if (confirm('Are you sure you want to save changes?')) {
                fetch('/Student/DeleteStudents', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        groupId: groupId,
                        selectedIds: selectedIds
                    })

                })
                    .then(response => {
                        if (response.ok) {
                            alert("Changes have been saved successfully");
                        } else {
                            alert("Error while saving");
                        }
                    });
            }
        });
    });

    // Make table rows hoverable with a subtle effect
    const tableRows = document.querySelectorAll('.attendance-table tbody tr');
    tableRows.forEach(row => {
        row.addEventListener('mouseover', function () {
            this.style.boxShadow = '0 0 5px rgba(0,0,0,0.1)';
        });

        row.addEventListener('mouseout', function () {
            this.style.boxShadow = '';
        });
    });
});