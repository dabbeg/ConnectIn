$(document).ready(function() {

    // Changes the value of the hidden input box in the profile picker
    // So that the id of the photo selected will be in the value attribute.
    $(".row a").click(function() {
        document.getElementById("photoId").value = $(this).attr("id");
    });


    // Everyone, Best friends, and Family
    $(".newsFeedFilters input[name=filters]:radio").change(function() {

        // Get current filter
        var currFilter = this.value;

        // Show and hide statuses according to the selected filter 
        if (currFilter == "Everyone") {
            $.get("/NewsFeed/Everyone", function(data) {
                for (var i = 0; i < data.length; i++) {
                    $("#post-" + data[i]).hide();
                }
                for (var i = 0; i < data.length; i++) {
                    $("#post-" + data[i]).fadeIn(700);
                }
            });
        }
        if (currFilter == "BestFriends") {
            $.get("/NewsFeed/Everyone", function(everyone) {
                $.get("/NewsFeed/BestFriends", function(bestFriends) {
                    for (var i = 0; i < everyone.length; i++) {
                        $("#post-" + everyone[i]).hide();
                    }
                    for (var i = 0; i < bestFriends.length; i++) {
                        $("#post-" + bestFriends[i]).fadeIn(700);
                    }
                });
            });

        }
        if (currFilter == "Family") {
            $.get("/NewsFeed/Everyone", function(everyone) {
                $.get("/NewsFeed/Family", function(family) {
                    for (var i = 0; i < everyone.length; i++) {
                        $("#post-" + everyone[i]).hide();
                    }
                    for (var i = 0; i < family.length; i++) {
                        $("#post-" + family[i]).fadeIn(700);
                    }
                });
            });
        }
    });


    function smiley(isLiked, smiles, btnId) {
        $(btnId).empty();

        var img = $("<img id='likedislikeimg'>");
        if (isLiked) { // liked
            img.attr("src", "/Content/images/smileyGREEN.png");
        } else { // not liked
            img.attr("src", "/Content/images/smileySMALL.png");
        }
        img.attr("alt", "Like Picture");
        $(btnId).append(img);
        $(btnId).append(smiles + " Smiles");
    }


    function sadface(isDisliked, sadfaces, btnId) {
        $(btnId).empty();

        var img = $("<img id='likedislikeimg'>");
        if (isDisliked) { // disliked
            img.attr("src", "/Content/images/sadfaceRED.png");
        } else { // not disliked
            img.attr("src", "/Content/images/sadfaceSMALL.png");
        }
        img.attr("alt", "Dislike Picture");
        $(btnId).append(img);
        $(btnId).append(sadfaces + " Sadfaces");
    }

    // Asynchronus like
    $(".likeBtn").click(function () {
        var btnId = "#" + this.id;
        $.post("/Status/Like", { "postId": $(this).siblings("input[name=postId]").val() }, function (data) {
            if (data.action == null) { // User has not liked or disliked
                smiley(true, data.likes, btnId);
            } else if (data.action.Like) { // User has liked
                smiley(false, data.likes, btnId);
            } else if(data.action.Dislike) { // User has disliked
                smiley(true, data.likes, btnId);
                sadface(false, data.dislikes, "#" + $(btnId).siblings(".dislikeBtn").attr("id"));
            }
        });
    });

    // Asynchronus dislike
    $(".dislikeBtn").click(function () {
        var btnId = "#" + this.id;
        $.post("/Status/Dislike", { "postId": $(this).siblings("input[name=postId]").val() }, function (data) {
            if (data.action == null) { // User has not liked or disliked
                sadface(true, data.dislikes, btnId);
            } else if (data.action.Dislike) { // User has liked
                sadface(false, data.dislikes, btnId);
            } else if (data.action.Like) { // User has disliked
                sadface(true, data.dislikes, btnId);
                smiley(false, data.likes, "#" + $(btnId).siblings(".likeBtn").attr("id"));
            }
        });
    });

    // Asynchronus Best Friend selection
    $(".bestFriend").click(function () {
        var img = $("<img id='bffamimg'>");
        var json = {
            "friendId": $(this).siblings("input[name=friendId]").val()
        };
        $.post("/Home/BestFriend", json, function (data) {
            if (data.FullStar === 1) {
                img.attr("src", "/Content/images/fullstar.png");
            } else {
                img.attr("src", "/Content/images/emptystar.png");
            }
            $(".bestFriend").empty();
            $(".bestFriend").append(img);
            var span = $("<span></span>").addClass("glyphicon bffam").text(" Best Friend");
            $(".bestFriend").append(span);
        });
    });

    // Asynchronus Family selection
    $(".family").click(function () {
        var img = $("<img id='bffamimg'>");
        var json = {
            "friendId": $(this).siblings("input[name=friendId]").val()
        };
        $.post("/Home/Family", json, function (data) {
            if (data.FullStar === 1) {
                img.attr("src", "/Content/images/fullstar.png");
            } else {
                img.attr("src", "/Content/images/emptystar.png");
            }
            $(".family").empty();
            $(".family").append(img);
            var span = $("<span></span>").addClass("glyphicon bffam").text(" Family");
            $(".family").append(span);
        });
    });

    // Asynchronus comment deletion
    $(".deleteComment").click(function () {
        var val = $(this).siblings("input[name=commentId]").val();
        $.post("/Status/RemoveComment", { "commentId": val }, function () {
            $("#comment-" + val).fadeOut(200);
        });
    });
    
    // Asynchronus post deletion
    $(".deletePostBtn").click(function () {
        var val = $(this).siblings("input[name=postId]").val();
        $.post("/Status/RemovePost", { "postId": val }, function () {
            $("#comment-" + val).fadeOut(500);
        });
    });


    $.get("/Home/BirthdayCounter", function (bdayCounter) {
        var d = new Date();
        var time = d.getDate();
        console.log(time);
        if (bdayCounter > 0 ) {
            $("#birthdayBubble").show();
            $("#birthdayBubble").text(bdayCounter);
        } else {
            $("#birthdayBubble").hide();

        }
        $("a#birthdayClick").click(function(){
            $("#birthdayBubble").hide();
        });
    });

    $.get("/Home/NotificationCounter", function(counter) {
        if (counter > 0) {
            $("#notificationBubble").show();
            $("#notificationBubble").text(counter)
        } else {
            $("#notificationBubble").hide();
        }

    });
});

