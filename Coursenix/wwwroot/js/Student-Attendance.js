document.addEventListener('DOMContentLoaded', function() {
    // Search functionality
    const searchInput = document.getElementById('studentSearch');
    const studentRows = document.querySelectorAll('.attendance-table tbody tr');

    searchInput.addEventListener('input', function() {
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

    // Get all delete buttons
    const deleteButtons = document.querySelectorAll('.delete-btn');
    
    // Add click event listeners to delete buttons
    deleteButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Get the parent row
            const row = this.closest('tr');
            
            // Add a visual effect before removal
            row.style.backgroundColor = '#ffebee';
            row.style.transition = 'background-color 0.3s';
            
            // Ask for confirmation
            if (confirm('Are you sure you want to delete this student?')) {
                setTimeout(() => {
                    row.style.opacity = '0';
                    row.style.transition = 'opacity 0.5s';
                    
                    // Remove the row after animation
                    setTimeout(() => {
                        row.remove();
                    }, 500);
                }, 300);
            } else {
                // Reset background if cancelled
                setTimeout(() => {
                    row.style.backgroundColor = '';
                }, 300);
            }
        });
    });
    
    // Save button functionality
    const saveButton = document.querySelector('.save-btn');
    saveButton.addEventListener('click', function() {
        // Show saving indicator
        this.innerHTML = 'Saving...';
        this.disabled = true;
        
        // Simulate saving data
        setTimeout(() => {
            this.innerHTML = 'Saved!';
            this.style.backgroundColor = '#388e3c';
            
            // Reset button after a while
            setTimeout(() => {
                this.innerHTML = 'Save';
                this.disabled = false;
                this.style.backgroundColor = '';
            }, 2000);
        }, 1500);
    });
    
    // Cancel button functionality
    const cancelButton = document.querySelector('.cancel-btn');
    cancelButton.addEventListener('click', function() {
        if (confirm('Are you sure you want to cancel? Any unsaved changes will be lost.')) {
            // Reset any changes or redirect
            alert('Changes canceled');
        }
    });
    
    // Add color coding for absence percentages
    const absenceElements = document.querySelectorAll('.absence');
    absenceElements.forEach(element => {
        // If class is not already set by HTML
        if (!element.classList.contains('none') && 
            !element.classList.contains('low') && 
            !element.classList.contains('medium') && 
            !element.classList.contains('high') && 
            !element.classList.contains('critical')) {
            
            const percentage = parseFloat(element.textContent);
            
            if (percentage === 0) {
                element.classList.add('none');
            } else if (percentage <= 15) {
                element.classList.add('low');
            } else if (percentage <= 30) {
                element.classList.add('medium');
            } else if (percentage <= 45) {
                element.classList.add('high');
            } else {
                element.classList.add('critical');
            }
        }
    });
    
    // Make table rows hoverable with a subtle effect
    const tableRows = document.querySelectorAll('.attendance-table tbody tr');
    tableRows.forEach(row => {
        row.addEventListener('mouseover', function() {
            this.style.boxShadow = '0 0 5px rgba(0,0,0,0.1)';
        });
        
        row.addEventListener('mouseout', function() {
            this.style.boxShadow = '';
        });
    });
    
    // Sample code for sorting table by column (can be extended)
    const tableHeaders = document.querySelectorAll('.attendance-table th');
    tableHeaders.forEach((header, index) => {
        header.addEventListener('click', function() {
            sortTable(index);
        });
        
        // Add cursor pointer style
        header.style.cursor = 'pointer';
    });
    
    function sortTable(columnIndex) {
        const table = document.querySelector('.attendance-table');
        let switching = true;
        let direction = 'asc';
        let switchcount = 0;
        
        while (switching) {
            switching = false;
            const rows = table.rows;
            
            for (let i = 1; i < (rows.length - 1); i++) {
                let shouldSwitch = false;
                const x = rows[i].getElementsByTagName('td')[columnIndex];
                const y = rows[i + 1].getElementsByTagName('td')[columnIndex];
                
                // Check if the two rows should switch
                if (direction === 'asc') {
                    if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                        shouldSwitch = true;
                        break;
                    }
                } else if (direction === 'desc') {
                    if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                        shouldSwitch = true;
                        break;
                    }
                }
            }
            
            if (shouldSwitch) {
                rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
                switching = true;
                switchcount++;
            } else {
                if (switchcount === 0 && direction === 'asc') {
                    direction = 'desc';
                    switching = true;
                }
            }
        }
    }
}); 