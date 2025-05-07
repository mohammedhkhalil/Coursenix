document.addEventListener("DOMContentLoaded", () => {
  // References to form elements
  const form = document.getElementById("signup-form");
  const fullNameInput = document.getElementById("full-name");
  const emailInput = document.getElementById("email");
  const phoneInput = document.getElementById("phone");
  const passwordInput = document.getElementById("password");
  const confirmPasswordInput = document.getElementById("confirm-password");
  const termsCheckbox = document.getElementById("terms");
  const biographyTextarea = document.getElementById("biography");
  const bioWordCountEl = document.getElementById("bio-word-count");
  const MAX_BIO_WORDS = 200;
  var cnt = 0;

  biographyTextarea.addEventListener("input", updateBioWordCount);

  function updateBioWordCount() {
    // Split on whitespace, filter out empty strings
    const words = biographyTextarea.value
      .trim()
      .split(/\s+/)
      .filter((w) => w.length > 0);

    const count = words.length;
    bioWordCountEl.textContent = count;

    // Optionally enforce max words
    if (count > MAX_BIO_WORDS) {
      // Trim to the first MAX_BIO_WORDS words
      biographyTextarea.value = words.slice(0, MAX_BIO_WORDS).join(" ");
      bioWordCountEl.textContent = MAX_BIO_WORDS;
      displayError(
        "biography-error",
        `Biography must not exceed ${MAX_BIO_WORDS} words`
      );
    } else {
      // Clear any previous error once back under the limit
      document.getElementById("biography-error").textContent = "";
    }
    cnt = count;
  }

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

  // Add required attribute validation on the client side
  const validateInputs = () => {
    let isValid = true;

    // Reset all error messages
    document.querySelectorAll(".error-message").forEach((el) => {
      el.textContent = "";
    });

    // Validate full name
    if (!fullNameInput.value.trim()) {
      displayError("full-name-error", "Full name is required");
      isValid = false;
      fullNameInput.classList.add("invalid-input");
    } else if (fullNameInput.value.trim().length < 3) {
      displayError(
        "full-name-error",
        "Full name must be at least 3 characters long"
      );
      isValid = false;
      fullNameInput.classList.add("invalid-input");
    } else {
      fullNameInput.classList.remove("invalid-input");
    }

    // Validate email
    if (!emailInput.value.trim()) {
      displayError("email-error", "Email address is required");
      isValid = false;
      emailInput.classList.add("invalid-input");
    } else if (!isValidEmail(emailInput.value)) {
      displayError("email-error", "Please enter a valid email address");
      isValid = false;
      emailInput.classList.add("invalid-input");
    } else {
      emailInput.classList.remove("invalid-input");
    }

    // Validate phone number
    if (!phoneInput.value.trim()) {
      displayError("phone-error", "Phone number is required");
      isValid = false;
      phoneInput.classList.add("invalid-input");
    } else if (!isValidPhone(phoneInput.value)) {
      displayError("phone-error", "Please enter a valid phone number");
      isValid = false;
      phoneInput.classList.add("invalid-input");
    } else {
      phoneInput.classList.remove("invalid-input");
    }

    // Validate password
    if (!passwordInput.value) {
      displayError("password-error", "Password is required");
      isValid = false;
      passwordInput.classList.add("invalid-input");
    } else if (passwordInput.value.length < 8) {
      displayError(
        "password-error",
        "Password must be at least 8 characters long"
      );
      isValid = false;
      passwordInput.classList.add("invalid-input");
    } else {
      var format = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;
      var upperCaseLetters = /[A-Z]/g;
      var lowerCaseLetters = /[a-z]/g;
      var numbers = /[0-9]/g;
      if (!passwordInput.value.match(format)) {
        displayError(
          "password-error",
          "Password must has at least one special character"
        );
        isValid = false;
        passwordInput.classList.add("invalid-input");
      } else if (!passwordInput.value.match(upperCaseLetters)) {
        displayError(
          "password-error",
          "Password must has at least one upper case character"
        );
        isValid = false;
        passwordInput.classList.add("invalid-input");
      } else if (!passwordInput.value.match(lowerCaseLetters)) {
        displayError(
          "password-error",
          "Password must has at least one lower case character"
        );
        isValid = false;
        passwordInput.classList.add("invalid-input");
      } else if (!passwordInput.value.match(numbers)) {
        displayError("password-error", "Password must has at least one number");
        isValid = false;
        passwordInput.classList.add("invalid-input");
      } else {
        passwordInput.classList.remove("invalid-input");
      }
    }

    // Validate password confirmation
    if (!confirmPasswordInput.value) {
      displayError("confirm-password-error", "Please confirm your password");
      isValid = false;
      confirmPasswordInput.classList.add("invalid-input");
    } else if (passwordInput.value !== confirmPasswordInput.value) {
      displayError("confirm-password-error", "Passwords do not match");
      isValid = false;
      confirmPasswordInput.classList.add("invalid-input");
    } else {
      confirmPasswordInput.classList.remove("invalid-input");
    }

    // Validate terms acceptance
    if (!termsCheckbox.checked) {
      displayError(
        "terms-error",
        "You must agree to the Terms of Service and Privacy Policy"
      );
      isValid = false;
      termsCheckbox.classList.add("invalid-checkbox");
    } else {
      termsCheckbox.classList.remove("invalid-checkbox");
    }

    return isValid;
  };

  // Form validation and submission
  form.addEventListener("submit", (e) => {
    e.preventDefault();

    const isValid = validateInputs();

    // If all validations pass, submit the form
    if (isValid) {
      // In a real application, this would typically involve an API call
      // For this example, we'll simulate a successful submission
      alert("Account created successfully! Redirecting to login page...");
      form.reset();
      // In a real application, you would redirect the user to a success page or login page
      // window.location.href = 'login.html';
    } else {
      // Scroll to the first error
      const firstError = document.querySelector(".error-message:not(:empty)");
      if (firstError) {
        firstError.scrollIntoView({ behavior: "smooth", block: "center" });
      }
    }
  });

  // Helper function to display error messages
  function displayError(elementId, message) {
    const errorElement = document.getElementById(elementId);
    errorElement.textContent = message;
  }

  // Helper function to validate email format
  function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  // Helper function to validate phone number
  function isValidPhone(phone) {
    // This is a simple validation that allows various formats
    // In a real application, you might want to use a more specific regex
    // or a library for phone validation based on your requirements
    const phoneRegex = /^[+]?[\s./0-9]{10,15}$/;
    return phoneRegex.test(phone);
  }
});
