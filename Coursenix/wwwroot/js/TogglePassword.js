document.addEventListener("DOMContentLoaded", () => {
  // References to form elements
  
  // Toggle password visibility
    document.querySelectorAll(".toggle-password").forEach((icon) => {
        icon.addEventListener("click", () => {
            const targetId = icon.getAttribute("data-target");
            const targetInput = document.getElementById(targetId);

            // Toggle input type between password and text
            if (targetInput.type === "password") {
                targetInput.type = "text";
                icon.classList.replace("fa-eye-slash", "fa-eye");
            } else {
                targetInput.type = "password";
                icon.classList.replace("fa-eye", "fa-eye-slash");
            }
        });
    });
});
