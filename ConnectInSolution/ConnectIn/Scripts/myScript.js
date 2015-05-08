$(document).ready(function () {
    $(".movie-views .btn input:radio").change(function () {
        if ($(this).val() == 'everyone') {
            return 1;
        } else if ($(this).val() == 'bestFriends') {
            return 2;
        } else {
            return 3;
        };
    });


    // Changes the value of the hidden input box in the profile picker
    // So that the id of the photo selected will be in the value attribute.
    $(".row a").click(function () {
        document.getElementById("photoId").value = $(this).attr("id");
    });
});