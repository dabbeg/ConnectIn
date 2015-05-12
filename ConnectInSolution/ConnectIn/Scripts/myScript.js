
$(document).ready(function () {
    // Changes the value of the hidden input box in the profile picker
    // So that the id of the photo selected will be in the value attribute.
    $(".row a").click(function() {
        document.getElementById("photoId").value = $(this).attr("id");
        document.getElementById("photoId2").value = $(this).attr("id");
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
            $("#bestFriend-" + json.friendId).empty();
            $("#bestFriend-" + json.friendId).append(img);
            var span = $("<span></span>").addClass("glyphicon bffam").text(" Best Friend");
            $("#bestFriend-" + json.friendId).append(span);
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
            $("#family-" + json.friendId).empty();
            $("#family-" + json.friendId).append(img);
            var span = $("<span></span>").addClass("glyphicon bffam").text(" Family");
            $("#family-" + json.friendId).append(span);
        });
    });

    function deleteComment(object) {
        var val = $(object).siblings("input[name=commentId]").val();
        $.post("/Status/RemoveComment", { "commentId": val }, function () {
            $("#comment-" + val).fadeOut(500, function() {
                $("#comment-" + val).remove();
            });
        });
    }

    // Asynchronus comment deletion
    $(".deleteComment").click(function () {
        deleteComment(this);
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

    // Asynchronous private settings
    $(".privacy").click(function () {
        var img = $("<img id='privacyimg'>");
        var userId = $(this).siblings("input[name=userId]").val();
        var json = {
            "userId": userId
        };
        $.post("/Account/Privacy", json, function(data) {
            var span;
            var p;
            if (data.privacy === 0) {
                img.attr("src", "/Content/images/Unlock.png");
                span = $("<span></span>").addClass("glyphicon bffam").text(" Public");
                p = $("<span></span>").text("Everyone can view your profile and posts");
            } else if (data.privacy === 1) {
                img.attr("src", "/Content/images/Lock.png");
                span = $("<span></span>").addClass("glyphicon bffam").text(" Private");
                p = $("<span></span>").text("Only your friends can view your profile and posts.");
            } else {
                img.attr("src", "/Content/images/ExtremeLock.png");
                span = $("<span></span>").addClass("glyphicon bffam").text(" Extreme Privacy");
                p = $("<span></span>").text("Only your best friends and family can view your profile and posts.");
            }
            $("#privacyText").empty();
            $(".privacy").empty();
            $(".privacy").append(img);
            $(".privacy").append(span);
            $("#privacyText").append(p).fadeIn(200);
        });
    });

    $.get("/Home/BirthdayCounter", function(bdayCounter) {
        $("#birthdayBubble").hide();

        if (bdayCounter > 0) {
            if (!readCookie('birthdayCookie')) {
                $('#birthdayBubble').show();
                $("#birthdayBubble").text(bdayCounter);
            }
        }

        $("a#birthdayClick").click(function() {
            $('#birthdayBubble').hide();
            createCookie('birthdayCookie', true, 1);

        });

        function notCounter() {
            $.get("/Home/NotificationCounter", function(counter) {
                if (counter > 0) {
                    $("#notificationBubble").show();
                    $("#notificationBubble").text(counter);
                } else {
                    $("#notificationBubble").hide();
                }
            });
        };

        // Asynchronous remove friend
        $(".removeFriend").click(function() {
            var json = {
                "userId": $(this).siblings("input[name=userId]").val(),
                "friendId": $(this).siblings("input[name=friendId]").val()
            };
            $.post("/Friend/Remove", json, function() {
                $("#removeFriend-" + json.friendId).fadeOut(700);
            });
        });

        // Asynchonous remove searched friend
        $(".removeFriendSearch").click(function() {
            var json = {
                "userId": $(this).siblings("input[name=userId]").val(),
                "friendId": $(this).siblings("input[name=friendId]").val()
            };

            $.post("/Friend/Remove", json, function() {
                $("#removeFriend-" + json.friendId).empty();
                $("#removeFriend-" + json.friendId).append("<div id=\"addFriend-@item.User.UserId\"><input type=\"hidden\" name=\"userId\" value=\"@User.Identity.GetUserId()\"/><input type=\"hidden\" name=\"friendId\" value=\"@item.User.UserId\"/><button type=\"submit\" class=\"btn btn-success addFriend\"><span class=\"glyphicon glyphicon-plus\"></span>&nbsp;Add friend</button></div>");
                $(".bforfamily").hide();
                $("#removeFriend-" + json.friendId).on("click", ".addFriend", function () {
                    var button = $("<button></button>").addClass("btn").text("Pending");
                    $.post("/Friend/Add", json, function () {
                        $("#removeFriend-" + json.friendId).empty();
                        $("#removeFriend-" + json.friendId).append(button);
                    });
                });
            });
        });

        // Asynchronous add friend
        $(".addFriend").click(function () {
            var button = $("<button></button>").addClass("btn").text("Pending");
            var json = {
                "userId": $(this).siblings("input[name=userId]").val(),
                "friendId": $(this).siblings("input[name=friendId]").val()
            };
            $.post("/Friend/Add", json, function () {
                $("#addFriend-" + json.friendId).empty();
                $("#addFriend-" + json.friendId).append(button);
            });
        });

        // Asynchronous accept friend
        $(".acceptFriend").click(function() {
            var json = {
                "notificationId" : $(this).siblings("input[name=notificationId]").val()
            };
            $.post("/Friend/AcceptFriendRequest", json, function() {
                $("#friendsacre").fadeOut(700);
                notCounter();
            });
        });

        // Asynchronous reject user
        $(".rejectFriend").click(function () {
            var json = {
                "notificationId": $(this).siblings("input[name=notificationId]").val()
            };
            $.post("/Friend/DeclineFriendRequest", json, function () {
                $("#friendsacre").fadeOut(700);
                notCounter();
            });
        });

        function createCookie(name, value, days) {
            var expires;
            if (days) {
                var date = new Date();
                var currentDate = new Date();
                // 1 ms before midnight
                date.setTime(date.getTime() + (days * (23-currentDate.getHours()) * (59-currentDate.getMinutes()) * (59-currentDate.getSeconds()) * (999-currentDate.getMilliseconds())));
                 expires = "; expires=" + date.toGMTString();
            } else {
                expires = "";
            }
            document.cookie = name + "=" + value + expires + "; path=/";
        }

        function readCookie(name) {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') {
                     c = c.substring(1, c.length);
                }
                if (c.indexOf(nameEQ) == 0) {
                     return c.substring(nameEQ.length, c.length);
                }
            }
            return null;
        }
    });

    $.get("/Home/NotificationCounter", function(counter) {
        if (counter > 0) {
            $("#notificationBubble").show();
            $("#notificationBubble").text(counter);
        } else {
            $("#notificationBubble").hide();
        }
    });

    // Asynchronus Posts
    $("#submitNewsFeedStatus").click(function () {
        var json = {
            "status": $("#newsfeedstatus").val(),
            "amount": $("#posts > div").length,
            "idOfGroup": $("input[name=idOfGroup]").val()
        };

        if (json.status != "") {
            $("#newsfeedstatus").val("");
            $.post("/Status/AddPost", json, function (data) {
                for (var i = 0; i < data.length; i++) {
                    var newDate = new Date();
                    var parsed = parseInt(data[i].DateInserted.match(/\d+/), 10);
                    newDate.setTime(parsed);
                    var model = {
                        "name": data[i].User.Name,
                        "userId": data[i].User.UserId,
                        "profilePicture": data[i].User.ProfilePicture,
                        "postId": data[i].PostId,
                        "date": $.format.date(newDate, "M/d/yyyy h:mm:ss a"),
                        "text": data[i].Body,
                        "likes": data[i].LikeDislikeComment.Likes,
                        "dislikes": data[i].LikeDislikeComment.Dislikes,
                        "comments": data[i].LikeDislikeComment.Comments,
                        "likePic": data[i].LikePic,
                        "dislikePic": data[i].DislikePic
                    };

                    var template = $("#postTemplate").tmpl(model);
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

    // Asynchronus Comments
    $("#submitcomment").click(function() {
        var json = {        
            "status": $("#commentstatus").val(),
            "amount": $("#allcomments > div").length,
            "postId": $("#reactionButtons input[name=postId]").val()
        };

        if (json.status != "") {
            $("#commentstatus").val("");
            $.post("/Status/AddComment", json, function(data) {
                for (var i = 0; i < data.length; i++) {
                    var newDate = new Date();
                    var parsed = parseInt(data[i].DateInserted.match(/\d+/), 10);
                    newDate.setTime(parsed);
                    var model = {
                        "userId": data[i].User.UserId,
                        "name": data[i].User.Name,
                        "profilePicture": data[i].User.ProfilePicture,
                        "commentId": data[i].CommentId,
                        "text": data[i].Body,
                        "date": $.format.date(newDate, "M/d/yyyy h:mm:ss a")
                    };

                    var template = $("#commentTemplate").tmpl(model);

                    template.on("click", ".deleteComment", function () {
                        deleteComment(this);
                    });
                
                    $(template).hide().prependTo("#allcomments").fadeIn(500);
                    if (data[i].IsUserCommentOwner) {
                        $("#reactionCommentButtons").append("<button type='button' class='btn btn-danger deletePostButton deleteComment'>Delete Comment</button>");
                        $("#reactionCommentButtons").append("<input type='hidden' name='commentId' value='" + data[i].CommentId.toString() + "'/>");
                    }
                }
            });
        }
    });
});

