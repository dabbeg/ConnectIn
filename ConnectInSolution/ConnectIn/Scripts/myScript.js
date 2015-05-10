$(document).ready(function() {

    // Changes the value of the hidden input box in the profile picker
    // So that the id of the photo selected will be in the value attribute.
    $(".row a").click(function() {
        document.getElementById("photoId").value = $(this).attr("id");
    });


    // Everyone, Best friends, and Family filters
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

    function like(object) {
        var btnId = "#" + object.id;
        $.post("/Status/Like", { "postId": $(object).siblings("input[name=postId]").val() }, function (data) {
            if (data.action == null) { // User has not liked or disliked
                smiley(true, data.likes, btnId);
            } else if (data.action.Like) { // User has liked
                smiley(false, data.likes, btnId);
            } else if (data.action.Dislike) { // User has disliked
                smiley(true, data.likes, btnId);
                sadface(false, data.dislikes, "#" + $(btnId).siblings(".dislikeBtn").attr("id"));
            }
        });
    }

    function dislike(object) {
        var btnId = "#" + object.id;
        $.post("/Status/Dislike", { "postId": $(object).siblings("input[name=postId]").val() }, function (data) {
            if (data.action == null) { // User has not liked or disliked
                sadface(true, data.dislikes, btnId);
            } else if (data.action.Dislike) { // User has liked
                sadface(false, data.dislikes, btnId);
            } else if (data.action.Like) { // User has disliked
                sadface(true, data.dislikes, btnId);
                smiley(false, data.likes, "#" + $(btnId).siblings(".likeBtn").attr("id"));
            }
        });
    }

    // Asynchronus like
    $(".likeBtn").click(function () {
        like(this);
    });

    // Asynchronus dislike
    $(".dislikeBtn").click(function () {
        dislike(this);
    });

    function deletePost(object) {
        var val = $(object).siblings("input[name=postId]").val();
        $.post("/Status/RemovePost", { "postId": val }, function () {
            $("#post-" + val).fadeOut(500, function () {
                $("#post-" + val).remove();
            });
        });
    }
    
    // Asynchronus post deletion
    $(".deletePostBtn").click(function () {
        deletePost(this);
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


    // Asynchronus Posts
    $("#submitNewsFeedStatus").click(function () {
        var json = {
            "status": $("#newsfeedstatus").val(),
            "location": "newsfeed",
            "amount": $("#posts > div").length
        };

        if (json.status != "") {
            $("#newsfeedstatus").val("");
            $.post("/Status/AddPost", json, function (data) {
                for (var i = 0; i < data.length; i++) {
                    var model = {
                        "name": data[i].User.Name,
                        "userId": data[i].User.UserId,
                        "profilePicture": data[i].User.ProfilePicture,
                        "postId": data[i].PostId,
                        "date": data[i].DateInserted,
                        "text": data[i].Body,
                        "likes": data[i].LikeDislikeComment.Likes,
                        "dislikes": data[i].LikeDislikeComment.Dislikes,
                        "comments": data[i].LikeDislikeComment.Comments,
                        "likePic": data[i].LikePic,
                        "dislikePic": data[i].DislikePic,
                    };

                    //var template = $.tmpl(templateFile, model);
                    var template = $("#template").tmpl(model);
                    template.on("click", ".likeBtn", function () {
                        like(this);
                    });

                    template.on("click", ".dislikeBtn", function () {
                        dislike(this);
                    });

                    template.on("click", ".deletePostBtn", function () {
                        deletePost(this);
                    });

                    $(template).hide().prependTo("#posts").fadeIn(500);
                    if (data[i].isUserPostOwner) {
                        $("#reactionButtons").append("<button type='button' class='btn btn-danger deletePostButton deletePostBtn'>Delete Post</button>");
                    }
                }
            });
        }
    });
});

