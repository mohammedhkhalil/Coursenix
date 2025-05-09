//document.addEventListener("DOMContentLoaded", () => {
//    // References to form elements
//    const form = document.getElementById("signup-form");
//    const fullNameInput = document.getElementById("full-name");
//    const emailInput = document.getElementById("email");
//    const phoneInput = document.getElementById("phone");
//    const parentPhoneInput = document.getElementById("parent-phone");
//    const gradeSelect = document.getElementById("grade");
//    const passwordInput = document.getElementById("password");
//    const confirmPasswordInput = document.getElementById("confirm-password");
//    const termsCheckbox = document.getElementById("terms");

//    // Toggle password visibility
//    document.querySelectorAll(".toggle-password").forEach((icon) => {
//        icon.addEventListener("click", () => {
//            const targetId = icon.getAttribute("data-target");
//            const targetInput = document.getElementById(targetId);

//            // Toggle input type between password and text

//            if (targetInput.type === "password") {
//                targetInput.type = "text";
//                icon.classList.replace("fa-eye-slash", "fa-eye");
//            } else {
//                targetInput.type = "password";
//                icon.classList.replace("fa-eye", "fa-eye-slash");
//            }
//        });
//    });


//    // Add required attribute validation on the client side
//    const validateInputs = () => {
//        let isValid = true;
//        // Reset all error messages
//        document.querySelectorAll(".error-message").forEach((el) => {
//            el.textContent = "";
//        });
//        // Validate terms acceptance
//        if (!termsCheckbox.checked) {
//            displayError(
//                "terms-error",
//                "You must agree to the Terms of Service and Privacy Policy"
//            );
//            isValid = false;
//            termsCheckbox.classList.add("invalid-checkbox");
//        } else {
//            termsCheckbox.classList.remove("invalid-checkbox");
//        }

//        return isValid;
//    };
//    // Helper function to display error messages
//    function displayError(elementId, message) {
//        const errorElement = document.getElementById(elementId);
//        errorElement.textContent = message;
//    }

//});
