document.querySelector('.search-box input').addEventListener('input', function(e) {
            const searchTerm = e.target.value.toLowerCase();
            const rows = document.querySelectorAll('.table tbody tr');
            
            rows.forEach(row => {
                const studentName = row.querySelector('.student-info span').textContent.toLowerCase();
                if (studentName.includes(searchTerm)) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });

        // Add functionality for action buttons
        document.querySelectorAll('.action-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                const row = this.closest('tr');
                const statusSpan = row.querySelector('.status');
                
                if (this.classList.contains('block')) {
                    statusSpan.textContent = 'Blocked';
                    statusSpan.className = 'status blocked';
                    this.textContent = 'Unblock';
                    this.className = 'action-btn unblock';
                } else {
                    statusSpan.textContent = 'Active';
                    statusSpan.className = 'status active';
                    this.textContent = 'Block';
                    this.className = 'action-btn block';
                }
            });
        });

        // Add functionality for pagination
        document.querySelectorAll('.pagination button').forEach(btn => {
            btn.addEventListener('click', function() {
                if (!this.textContent.match(/[‹›]/)) {
                    document.querySelectorAll('.pagination button').forEach(b => b.classList.remove('active'));
                    this.classList.add('active');
                }
            });
        });