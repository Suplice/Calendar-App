function changeProfilePicture() {
    $('#ProfilePictureUpdateModal').modal('show');
}


function UpdateProfilePicture(formData) {
    $.ajax({
        url: "/Account/ChangeProfilePicture",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            console.log("successfully changed profile picture");

            var newImageUrl = response.newProfilePictureUrl;

            $('#profilePicture').attr('src', newImageUrl);



            $('#ProfilePictureUpdateModal').modal('hide');
        },

        error: function (xhr, error) {
            console.error("Error changing Profile Picture", error);
            console.log(xhr.responseText);

            displayValidationErrors(xhr.responseJSON);
        }
    })
}

function handleUpdateProfilePictureSubmit(event) {
    event.preventDefault();

    var form = $('#ProfilePictureUpdateForm')[0];
    var formData = new FormData(form);

    UpdateProfilePicture(formData);
}


function clearProfilePictureDataOnClose() {
    document.getElementById('ProfilePictureUpdateForm').reset();
    document.getElementById('ProfilePicture-UpdateValidation').innerText = '';
}