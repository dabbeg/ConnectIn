
$(document).ready(function () {
    // Changes the value of the hidden input box in the profile picker
    // So that the id of the photo selected will be in the value attribute.
    $(".row span").click(function() {
        $("input[name=photoId]").val($(this).attr("id"));
    });
    
    /**
     ******************************* Newsfeed *******************************
     */
    // Everyone, Best friends, and Family filters
    $(".newsFeedFilters input[name=filters]:radio").change(function() {

        // Get current filter
        var currFilter = this.value;

        // Show and hide statuses according to the selected filter 
        if (currFilter === "Everyone") {
            $.get("/NewsFeed/Everyone", function (data) {
                var i;
                for (i = 0; i < data.length; i++) {
                    $("#post-" + data[i]).hide();
                }
                for (i = 0; i < data.length; i++) {
                    $("#post-" + data[i]).fadeIn(700);
                }
            });
        }
        if (currFilter === "BestFriends") {
            $.get("/NewsFeed/Everyone", function(everyone) {
                $.get("/NewsFeed/BestFriends", function (bestFriends) {
                    var i;
                    for (i = 0; i < everyone.length; i++) {
                        $("#post-" + everyone[i]).hide();
                    }
                    for (i = 0; i < bestFriends.length; i++) {
                        $("#post-" + bestFriends[i]).fadeIn(700);
                    }
                });
            });

        }
        if (currFilter === "Family") {
            $.get("/NewsFeed/Everyone", function(everyone) {
                $.get("/NewsFeed/Family", function (family) {
                    var i;
                    for (i = 0; i < everyone.length; i++) {
                        $("#post-" + everyone[i]).hide();
                    }
                    for (i = 0; i < family.length; i++) {
                        $("#post-" + family[i]).fadeIn(700);
                    }
                });
            });
        }
    });

    /**
     ******************************* Status *******************************
     */
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

    // Asynchronous comment deletion
    function deleteComment(object) {
        var val = $(object).siblings("input[name=commentId]").val();
        $.post("/Status/RemoveComment", { "commentId": val }, function () {
            $("#comment-" + val).fadeOut(500, function () {
                $("#comment-" + val).remove();
            });
        });
    }

    // Asynchronus comment deletion
    $(".deleteComment").click(function () {
        deleteComment(this);
    });

    // Asynchronous post deletion
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

    // Asynchronus Posts
    $("#submitNewsFeedStatus").click(function () {
        var json = {
            "status": $("#newsfeedstatus").val(),
            "amount": $("#posts > div").length,
            "idOfGroup": $("input[name=idOfGroup]").val()
        };

        if (json.status !== "") {
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
                        "date": $.format.date(newDate, "d.M.yyyy h:mm:ss"),
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
    $("#submitcomment").click(function () {
        var json = {
            "status": $("#commentstatus").val(),
            "amount": $("#allcomments > div").length,
            "postId": $("#reactionButtons input[name=postId]").val()
        };

        if (json.status !== "") {
            $("#commentstatus").val("");
            $.post("/Status/AddComment", json, function (data) {
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
                        "date": $.format.date(newDate, "d.M.yyyy h:mm:ss")
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

    // Show more or show less
    $("div .ellipsis-text pre").each(function () {
        if ($(this).height() > 60) {
            $(this).siblings(".more").show();
        }
    });

    $("div .ellipsis-text pre").css("height", "70px").css("overflow", "hidden").css("display", "block");

    $("div .ellipsis-text").on("click", "span", function () {
        var height = $(this).siblings("pre")[0].scrollHeight;
        if ($(this).hasClass("more")) {
            $(this).siblings("pre").animate({ height: height }, { duration: 1000 });
            $(this).hide();
            $(this).siblings(".less").show();
        } else {
            $(this).siblings("pre").animate({ height: "70px" }, { duration: 1000 });
            $(this).hide();
            $(this).siblings(".more").show();
        }
    });


    /**
     ******************************* Friends *******************************
     */
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

    // Notification counter when friend is accepted or declined
    function notCounter() {
        $.get("/Home/NotificationCounter", function (counter) {
            if (counter > 0) {
                $("#notificationBubble").show();
                $("#notificationBubble").text(counter);
            } else {
                $("#notificationBubble").hide();
            }
        });
    };

    // Asynchronous remove friend
    $(".removeFriend").click(function () {
        var json = {
            "userId": $(this).siblings("input[name=userId]").val(),
            "friendId": $(this).siblings("input[name=friendId]").val()
        };
        $.post("/Friend/Remove", json, function () {
            $("#removeFriend-" + json.friendId).fadeOut(700);
        });
    });

    // Asynchonous remove searched friend
    $(".removeFriendSearch").click(function () {
        var json = {
            "userId": $(this).siblings("input[name=userId]").val(),
            "friendId": $(this).siblings("input[name=friendId]").val()
        };

        $.post("/Friend/Remove", json, function () {
            $("#removeFriend-" + json.friendId).empty();
            $("#removeFriend-" + json.friendId).append("<div id=\"addFriend-@item.User.UserId\"><input type=\"hidden\" name=\"userId\" value=\"@User.Identity.GetUserId()\"/><input type=\"hidden\" name=\"friendId\" value=\"@item.User.UserId\"/><button type=\"submit\" class=\"btn btn-success addFriend\"><span class=\"glyphicon glyphicon-plus\"></span>&nbsp;Add friend</button></div>");
            $(".bforfamily-" + json.friendId).hide();
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
    $(".acceptFriend").click(function () {
        var json = {
            "notificationId": $(this).siblings("input[name=notificationId]").val()
        };
        $.post("/Friend/AcceptFriendRequest", json, function () {
            $("#friendsacre-" + json.notificationId).fadeOut(700);
            notCounter();
        });
    });

    // Asynchronous reject user
    $(".rejectFriend").click(function () {
        var json = {
            "notificationId": $(this).siblings("input[name=notificationId]").val()
        };
        $.post("/Friend/DeclineFriendRequest", json, function () {
            $("#friendsacre-" + json.notificationId).fadeOut(700);
            notCounter();
        });
    });

    /**
     ******************************* User *******************************
     */
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

    /*Cookie source : http://stackoverflow.com/questions/1458724/how-to-set-unset-cookie-with-jquery */
    $.get("/Home/BirthdayCounter", function(bdayCounter) {
        $("#birthdayBubble").hide();

        function readCookie(name) {
            var nameEq = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) === ' ') {
                    c = c.substring(1, c.length);
                }
                if (c.indexOf(nameEq) === 0) {
                    return c.substring(nameEq.length, c.length);
                }
            }
            return null;
        }

        if (bdayCounter > 0) {
            if (!readCookie('birthdayCookie')) {
                $('#birthdayBubble').show();
                $("#birthdayBubble").text(bdayCounter);
            }
        }

        function createCookie(name, value, days) {
            var expires;
            if (days) {
                var date = new Date();
                var currentDate = new Date();
                // 1 ms before midnight
                date.setTime(date.getTime() + (days * (23 - currentDate.getHours()) * (59 - currentDate.getMinutes()) * (59 - currentDate.getSeconds()) * (999 - currentDate.getMilliseconds())));
                expires = "; expires=" + date.toGMTString();
            } else {
                expires = "";
            }
            document.cookie = name + "=" + value + expires + "; path=/";
        }

        $("a#birthdayClick").click(function() {
            $('#birthdayBubble').hide();
            createCookie('birthdayCookie', true, 1);

        });
    });

    // Inject a div to divide the photos
    $("#photoContainer > :nth-child(5n)").after("<div class='clearfix visible-xs-block'></div>");

    var userLoggedIn = $("#userLoggedInId").val();
    var profileVisiting = $("#currentVisitingProfileId").val();

    if ($("#profilePhotoContainer > div").length === 0) {
        if (userLoggedIn === profileVisiting) {
            $("#profilePhotoContainer").append("<h5>Please upload a photo to select as your profile photo</h5>").css("color", "red");
        } else {
            $("#profilePhotoContainer").append("<h5>Your friend has no profile pictures to show.</h5>").css("color", "red");
        }
    }

    if ($("#coverPhotoContainer > div").length === 0) {
        if (userLoggedIn === profileVisiting) {
            $("#coverPhotoContainer").append("<h5>Please upload a photo to select as your cover photo</h5>").css("color", "red");
        } else {
            $("#coverPhotoContainer").append("<h5>Your friend has no cover photos to show.</h5>").css("color", "red");
        }
    }

    $("#pickCoverPhoto").attr("disabled", true);
    $("#pickProfilePhoto").attr("disabled", true);
    $("#deleteCoverPhoto").attr("disabled", true);
    $("#deleteProfilePhoto").attr("disabled", true);


    $("span.thumbnail").click(function () {
        $("span.thumbnail").removeClass("selected-photo").addClass("thumbnail-photo");
        $(this).removeClass("thumbnail-photo").addClass("selected-photo");

        var photoId = $(this).attr("id");
        var url = "/Photo/IsProfilePhoto?photoId=" + photoId;
        $.get(url, function (data) {
            if (data) {
                $("#pickCoverPhoto").attr("disabled", true);
                $("#pickProfilePhoto").attr("disabled", false);
                $("#deleteCoverPhoto").attr("disabled", true);
                $("#deleteProfilePhoto").attr("disabled", false);
            } else {
                $("#pickCoverPhoto").attr("disabled", false);
                $("#pickProfilePhoto").attr("disabled", true);
                $("#deleteCoverPhoto").attr("disabled", false);
                $("#deleteProfilePhoto").attr("disabled", true);
            }
        });
    });

    function deletePhoto(photoId) {
        $.post("/Photo/DeletePhoto", { "photoId": photoId }, function () {
            $(".row div span").each(function () {
                if ($(this).attr("id") === photoId) {
                    $(this).parent().remove();
                }
            });

        });
    }

    $("#deleteProfilePhoto").click(function () {
        $("#deleteProfilePhoto").attr("disabled", true);
        $("#pickProfilePhoto").attr("disabled", true);
        var photoId = $(this).siblings("input[name=photoId]").val();
        deletePhoto(photoId);
    });

    $("#deleteCoverPhoto").click(function () {
        $("#deleteCoverPhoto").attr("disabled", true);
        $("#pickCoverPhoto").attr("disabled", true);
        var photoId = $(this).siblings("input[name=photoId]").val();
        deletePhoto(photoId);
    });

    $("#cropForm").submit(function () {
        $(this).find(".btn-crop").attr("disabled", true);
    });

    $(document).on('change', '.btn-file :file', function () {
        var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger('fileselect', [numFiles, label]);
    });

    $('.btn-file :file').on('fileselect', function (event, numFiles, label) {
        console.log(numFiles);
        console.log(label);
        $("input[type=text]").val(label);
    });

    $(".btn-upload").attr("disabled", true);
    $("input[name=Image]").change(function () {
        if ($(this).val() !== "") {
            $(".btn-upload").attr("disabled", false);
        }
    });


    /**
     ******************************* Groups *******************************
     */
    // Asynchronous remove group notification
    $(".groupNotification").click(function () {
        var json = {
            "groupNotificationId": $(this).siblings("input[name=groupNotificationId]").val()
        };
        $.post("/Friend/HideGroupNotification", json, function () {
            $("#groupNotification-" + json.groupNotificationId).fadeOut(700);
            notCounter();
        });
    });

    $(".addMembersToGroup").click(function () {
        var members = $("input[name=newFriendsInGroup]:checked").map(
            function () { return this.value; }).get().join(",");
        if (members !== "") {
            var json = {
                "newFriendsInGroup": members,
                "idOfGroup": $(this).siblings("input[name=idOfGroup]").val()
            };
            $.post("/Group/AddFriend/", json, function (data) {
                var list = members.split(",");
                for (var i = 0; i < list.length; i++) {
                    var a = "<a href=\"/Home/Profile?id=" + list[i] + "\">" + data[i].Name + "</a>";
                    $("#groupCheck-" + list[i]).hide();
                    var table = "<tr><td>" + a + "</td><td>" + data[i].Work + "</td><td>" + data[i].UserName + "</td></tr>";
                    $("#membersinGroup").append(table);
                }
            });
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
    /****************************************************************************************/
});
    

