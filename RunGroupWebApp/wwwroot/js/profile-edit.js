// Form validation
(function () {
    'use strict'
    var forms = document.querySelectorAll('.needs-validation')
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault()
                    event.stopPropagation()
                }
                form.classList.add('was-validated')
            }, false)
        })
})()

// Profile image preview
document.getElementById('ProfilePhotoFile').addEventListener('change', function (e) {
    if (e.target.files && e.target.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('profileImage').setAttribute('src', e.target.result);
        }
        reader.readAsDataURL(e.target.files[0]);
    }
});