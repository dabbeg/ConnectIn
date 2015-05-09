$(document).ready(function () {
<<<<<<< HEAD
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
=======
>>>>>>> 6d2628ddc989f77d2f20f3bb983104ec0c675fb0

    // Changes the value of the hidden input box in the profile picker
    // So that the id of the photo selected will be in the value attribute.
    $(".row a").click(function () {
        document.getElementById("photoId").value = $(this).attr("id");
    });


    // Everyone, Best friends, and Family
    $(".newsFeedFilters input[name=filters]:radio").change(function () {

        // Get current filter
        var currFilter = this.value;

        // Show and hide statuses according to the selected filter 
        if (currFilter == "Everyone") {
            $.get("/NewsFeed/Everyone", function (data) {
                for (var i = 0; i < data.length; i++) {
                    $("#post-" + data[i]).fadeOut();
                }
                for (var i = 0; i < data.length; i++) {
                    $("#post-" + data[i]).fadeIn(500);
                }
            });
        }
        if (currFilter == "BestFriends") {
            $.get("/NewsFeed/Everyone", function (data) {
                for (var i = 0; i < data.length; i++) {
                    $("#post-" + data[i]).fadeOut();
                }
                $.get("/NewsFeed/BestFriends", function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $("#post-" + data[i]).fadeIn();
                    }
                });
            });
            
        }
        if (currFilter == "Family") {
            $.get("/NewsFeed/Everyone", function (data) {
                for (var i = 0; i < data.length; i++) {
                    $("#post-" + data[i]).fadeOut();
                }
                $.get("/NewsFeed/Family", function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $("#post-" + data[i]).fadeIn();
                    }
                });
            });
        }
    });

});

