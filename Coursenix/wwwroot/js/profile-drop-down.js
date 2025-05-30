document.addEventListener('DOMContentLoaded', function () {
    // User Profile Dropdown Toggle
    const userProfile = document.getElementById('cnUserProfile');
    const userDropdown = document.getElementById('cnUserDropdown');

    if (userProfile && userDropdown) {
        userProfile.addEventListener('click', function (e) {
            e.stopPropagation();
            userDropdown.classList.toggle('show');
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', function (e) {
            if (!userProfile.contains(e.target) && userDropdown.classList.contains('show')) {
                userDropdown.classList.remove('show');
            }
        });

        // Prevent dropdown from closing when clicking inside it
        userDropdown.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    } else {
        console.warn("User profile or dropdown element not found. Check IDs: cnUserProfile, cnUserDropdown");
    }
});