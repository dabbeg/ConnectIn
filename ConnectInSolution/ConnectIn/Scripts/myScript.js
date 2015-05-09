$(document).ready(function () {
    /*$("btn-default-like btn-default-dislike").click(function () {
        if ($(this).val() === "") {
            return 1;
        } else if ($(this).val() === "bestFriends") {
            return 2;
        } else {
            return 3;
        }
    });*/
    var likeGreen = document.getElementById("smiley-green");
    likeGreen.style.display = "none";
    function hide() {
        var likeGreen = document.getElementById("smiley-green");
        var likeBlue = document.getElementById("smiley-blue");
        if (likeGreen.style.display === "none") {
            likeGreen.style.display = "block";
            likeBlue.style.display = "none";
        }
        else {
            likeGreen.style.display = "none";
            likeBlue.style.display = "block";
        } 
    }

    // Changes the value of the hidden input box in the profile picker
    // So that the id of the photo selected will be in the value attribute.
    $(".row a").click(function () {
        document.getElementById("photoId").value = $(this).attr("id");
    });
});