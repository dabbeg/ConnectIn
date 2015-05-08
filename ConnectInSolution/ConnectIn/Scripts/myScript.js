$(document).ready(function () {
    /*$(".movie-views .btn input:radio").change(function () {
        if ($(this).val() == 'everyone') {
            return 1;
        } else if ($(this).val() == 'bestFriends') {
            return 2;
        } else {
            return 3;
        };
    });*/


    // Changes the value of the hidden input box in the profile picker
    // So that the id of the photo selected will be in the value attribute.
    $(".row a").click(function () {
        document.getElementById("photoId").value = $(this).attr("id");
    });

   
});

// Everyone, Best friends, and Family
$(".newsFeedFilters input[name=filters]:radio").change(function () {
    $('input[name=filters]').attr("disabled", true);
    // Get current filter
    var currFilter = this.value;

    if (currFilter == "Everyone") {
        $.get("/NewsFeed/Everyone", function (data, status) {
            for (var i = 0; i < data.length; i++) {
                $("#post-" + data[i].PostId).show();
            }
        });
    }

    // If the filter is not Everyone then hide all statuses
    if (currFilter != "Everyone") {
        
    }

    //Show statuses for the correct filter
    
    if (currFilter == "BestFriends") {
        $.get("/NewsFeed/BestFriends", function (data, status) {
            for (var i = 0; i < data.First.length; i++) {
                $("#post-" + data.First[i].PostId).hide();
            }
            for (var i = 0; i < data.Second.length; i++) {
                $("#post-" + data.Second[i].PostId).show();
            }
        });
    }
    if (currFilter == "Family") {
        $.get("/NewsFeed/Everyone", function (data, status) {
            for (var i = 0; i < data.length; i++) {
                $("#post-" + data[i].PostId).hide();
            }
        });
        $.get("/NewsFeed/Family", function (data, status) {
            for (var i = 0; i < data.length; i++) {
                $("#post-" + data[i].PostId).show();
            }
        });
    }
    $('input[name=filters]').attr("disabled", false);
});