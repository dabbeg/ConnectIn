$(document).ready(function () {

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
            $.get("/NewsFeed/Everyone", function (data, status) {
                for (var i = 0; i < data.length; i++) {
                    $("#post-" + data[i].PostId).show();
                }
            });
        }
        if (currFilter == "BestFriends") {
            $.get("/NewsFeed/EveryoneAndBestFriends", function (data, status) {
                alert(status);
                for (var i = 0; i < data.First.length; i++) {
                    $("#post-" + data.First[i].PostId).hide();
                }
                for (var i = 0; i < data.Second.length; i++) {
                    $("#post-" + data.Second[i].PostId).show();
                }
            }).done(function () {
                alert("second success");
            }).fail(function (error) {
                alert("error " + error);
            }).always(function () {
                alert("finished " + status);
            });
        }
        if (currFilter == "Family") {
            $.get("/NewsFeed/EveryoneAndFamily", function (data, status) {
                for (var i = 0; i < data.First.length; i++) {
                    $("#post-" + data.First[i].PostId).hide();
                }
                for (var i = 0; i < data.Second.length; i++) {
                    $("#post-" + data.Second[i].PostId).show();
                }
            });
        }
    });

});

