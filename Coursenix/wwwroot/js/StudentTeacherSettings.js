document.addEventListener('DOMContentLoaded', function() {
    // Simulated database password for demonstration (in a real app, this would be checked on the server)
    const STORED_PASSWORD = "asdfsadf1@S"; // This is just for demo purposes
    
    // Handle password visibility toggle
    const togglePasswordButtons = document.querySelectorAll('.toggle-password');
    
    togglePasswordButtons.forEach(button => {
        button.addEventListener('click', function() {
            const targetId = this.getAttribute('data-target');
            const passwordInput = document.getElementById(targetId);
            
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                this.classList.remove('fa-eye-slash');
                this.classList.add('fa-eye');
            } else {
                passwordInput.type = 'password';
                this.classList.remove('fa-eye');
                this.classList.add('fa-eye-slash');
            }
        });
    });
    // User Profile Dropdown Toggle
    const userProfile = document.getElementById('userProfile');
    const userDropdown = document.getElementById('userDropdown');

    userProfile.addEventListener('click', function(e) {
        e.stopPropagation();
        userDropdown.classList.toggle('show');
    });

    // Close dropdown when clicking outside
    document.addEventListener('click', function(e) {
        if (!userProfile.contains(e.target) && userDropdown.classList.contains('show')) {
            userDropdown.classList.remove('show');
        }
    });

    // Prevent dropdown from closing when clicking inside it
    userDropdown.addEventListener('click', function(e) {
        e.stopPropagation();
    });

    // Form validation
    const settingsForm = document.getElementById('settingsForm');
    
    settingsForm.addEventListener('submit', function(event) {
        event.preventDefault();
        
        // Reset error messages
        clearErrors();
        
        // Get all required form fields
        const fullName = document.getElementById('fullName').value;
        const phone = document.getElementById('phone').value;
        const parentPhone = document.getElementById('parentPhone').value;
        const currentPassword = document.getElementById('current-password').value;
        const newPassword = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirm-password').value;
        
        let formValid = true;
        
        // Check if all required fields are filled
        if (!fullName) {
            displayError('fullName', 'Full name is required');
            formValid = false;
        }
        
        if (!phone) {
            displayError('phone', 'Phone number is required');
            formValid = false;
        }
        
        if (!parentPhone) {
            displayError('parentPhone', 'Parent phone number is required');
            formValid = false;
        }
        
        // 1. Check if current password is entered and matches stored password
        if (!currentPassword) {
            displayError('current-password', 'Please enter your current password');
            formValid = false;
        } else if (currentPassword !== STORED_PASSWORD) {
            displayError('current-password', 'Current password is incorrect');
            formValid = false;
        }
        
        // Validate password fields
        // New password is optional but must follow complexity rules if provided
        if (newPassword) {
            // 3. Validate password complexity
            if (!validatePasswordComplexity(newPassword)) {
                displayError('password', 'Password must be at least 8 characters and contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character');
                formValid = false;
            }
            
            // Confirmation password is required only if new password is provided
            if (!confirmPassword) {
                displayError('confirm-password', 'Please confirm your password');
                formValid = false;
            } else if (newPassword !== confirmPassword) {
                // 2. Check if passwords match
                displayError('confirm-password', 'Passwords do not match');
                formValid = false;
            }
        } else {
            // If new password is empty, confirmation should also be empty
            if (confirmPassword) {
                displayError('confirm-password', 'Please leave this field empty if you are not changing your password');
                formValid = false;
            }
        }
        
        // If form is valid, submit it
        if (formValid) {
            // You can use AJAX to submit the form or simply allow the default form submission
            console.log('Form is valid, submitting...');
            
            // Display success message
            showNotification('Changes saved successfully!', 'success');
            
            // Clear password fields after successful submission
            document.getElementById('current-password').value = '';
            document.getElementById('password').value = '';
            document.getElementById('confirm-password').value = '';
        }
    });
    
    // Function to validate password complexity
    function validatePasswordComplexity(password) {
        // Password must be at least 8 characters long
        if (password.length < 8) {
            return false;
        }
        
        // Check for at least 1 lowercase letter
        if (!/[a-z]/.test(password)) {
            return false;
        }
        
        // Check for at least 1 uppercase letter
        if (!/[A-Z]/.test(password)) {
            return false;
        }
        
        // Check for at least 1 number
        if (!/[0-9]/.test(password)) {
            return false;
        }
        
        // Check for at least 1 special character
        if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) {
            return false;
        }
        
        return true;
    }
    
    // Function to display error message
    function displayError(inputId, message) {
        const input = document.getElementById(inputId);
        const errorElement = document.getElementById(`${inputId}-error`) || 
                            input.nextElementSibling.nextElementSibling;
        
        input.classList.add('error');
        errorElement.textContent = message;
        errorElement.style.display = 'block';
    }
    
    // Function to clear all error messages
    function clearErrors() {
        const errorMessages = document.querySelectorAll('.error-message');
        const inputFields = document.querySelectorAll('input');
        
        errorMessages.forEach(error => {
            error.textContent = '';
            error.style.display = 'none';
        });
        
        inputFields.forEach(input => {
            input.classList.remove('error');
        });
    }
    
    // Function to show notification
    function showNotification(message, type) {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;
        
        // Append to body
        document.body.appendChild(notification);
        
        // Show notification
        setTimeout(() => {
            notification.classList.add('show');
        }, 10);
        
        // Remove notification after 3 seconds
        setTimeout(() => {
            notification.classList.remove('show');
            setTimeout(() => {
                document.body.removeChild(notification);
            }, 300);
        }, 3000);
    }

    // Add event listeners to handle input field validation in real-time
    const fullNameField = document.getElementById('fullName');
    const phoneField = document.getElementById('phone');
    const parentPhoneField = document.getElementById('parentPhone');
    const currentPasswordField = document.getElementById('current-password');
    const passwordField = document.getElementById('password');
    const confirmPasswordField = document.getElementById('confirm-password');
    
    // Add validation for required text fields
    [fullNameField, phoneField, parentPhoneField].forEach(field => {
        field.addEventListener('input', function() {
            if (!this.value.trim()) {
                this.classList.add('error');
                // Create error message if it doesn't exist
                let errorElement = this.nextElementSibling;
                if (!errorElement || !errorElement.classList.contains('error-message')) {
                    errorElement = document.createElement('span');
                    errorElement.className = 'error-message';
                    errorElement.id = `${this.id}-error`;
                    this.parentNode.insertBefore(errorElement, this.nextSibling);
                }
                errorElement.textContent = `${this.previousElementSibling.textContent} is required`;
                errorElement.style.display = 'block';
            } else {
                this.classList.remove('error');
                const errorElement = document.getElementById(`${this.id}-error`) || this.nextElementSibling;
                if (errorElement && errorElement.classList.contains('error-message')) {
                    errorElement.textContent = '';
                    errorElement.style.display = 'none';
                }
            }
        });
    });
    
    // Current password validation - just check if it's entered, validation against DB happens on submit
    currentPasswordField.addEventListener('input', function() {
        const errorElement = document.getElementById('current-password-error') || this.nextElementSibling.nextElementSibling;
        
        if (!this.value) {
            this.classList.add('error');
            errorElement.textContent = 'Current password is required';
            errorElement.style.display = 'block';
        } else {
            this.classList.remove('error');
            errorElement.textContent = '';
            errorElement.style.display = 'none';
        }
    });
    
    // New password validation - optional but must follow rules if provided
    passwordField.addEventListener('input', function() {
        const errorElement = document.getElementById('password-error');
        if (!this.value)return;
        if (this.value && !validatePasswordComplexity(this.value)) {
            this.classList.add('error');
            errorElement.textContent = 'Password must be at least 8 characters and contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character';
            errorElement.style.display = 'block';
        } else {
            this.classList.remove('error');
            errorElement.textContent = '';
            errorElement.style.display = 'none';
            
            // Check confirmation password field
            validateConfirmPassword();
        }
    });
    
    // Confirmation password validation - required only if new password is provided
    confirmPasswordField.addEventListener('input', validateConfirmPassword);
    
    function validateConfirmPassword() {
        const newPassword = passwordField.value;
        const confirmValue = confirmPasswordField.value;
        const errorElement = document.getElementById('confirm-password-error');
        if (!newPassword)return;
        if (newPassword && !confirmValue) {
            confirmPasswordField.classList.add('error');
            errorElement.textContent = 'Please confirm your new password';
            errorElement.style.display = 'block';
        } else if (newPassword && confirmValue && newPassword !== confirmValue) {
            confirmPasswordField.classList.add('error');
            errorElement.textContent = 'Passwords do not match';
            errorElement.style.display = 'block';
        } else if (!newPassword && confirmValue) {
            confirmPasswordField.classList.add('error');
            errorElement.textContent = 'Please leave this field empty if you are not changing your password';
            errorElement.style.display = 'block';
        } else {
            confirmPasswordField.classList.remove('error');
            errorElement.textContent = '';
            errorElement.style.display = 'none';
        }
    }

    // Add basic CSS for notifications if not present in your CSS file
    const style = document.createElement('style');
    style.textContent = `
        .notification {
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 15px 20px;
            border-radius: 4px;
            color: white;
            font-weight: bold;
            opacity: 0;
            transform: translateY(-20px);
            transition: opacity 0.3s, transform 0.3s;
            z-index: 1000;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
        }
        
        .notification.show {
            opacity: 1;
            transform: translateY(0);
        }
        
        .notification.success {
            background-color: #4CAF50;
        }
        
        .notification.error {
            background-color: #f44336;
        }
        
        input.error {
            border-color: #f44336 !important;
        }
        
        .error-message {
            color: #f44336;
            font-size: 12px;
            margin-top: 5px;
            display: none;
        }
    `;
    document.head.appendChild(style);
});