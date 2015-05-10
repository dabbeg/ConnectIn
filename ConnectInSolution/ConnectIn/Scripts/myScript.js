﻿
$(document).ready(function () {
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

    
    // Asynchronus post deletion
    $(".deletePostBtn").click(function () {
        var val = $(this).siblings("input[name=postId]").val();
        $.post("/Status/RemovePost", { "postId": val }, function () {
            $("#post-" + val).fadeOut(500);
        });
    });

    $.get("/Home/BirthdayCounter", function (bdayCounter) {
        $("#birthdayBubble").hide();

        if (bdayCounter > 0) {
            if (!readCookie('bdayTest')) {
                $('#birthdayBubble').show();
                $("#birthdayBubble").text(bdayCounter);
            }
        } 
        
        $("a#birthdayClick").click(function () {
            $('#birthdayBubble').hide();
            createCookie('bdayTest', true, 1);
        });
        function createCookie(name, value, days) {
            if (days) {
                var date = new Date();
                //on midnight, close
                //date.setTime(date.getFullYear(), date.getMonth(), date.getDate() + 1, 0, 0, 0);
                //in 24 hours
                //date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                //20 sek cookie
                Console.log(Date.getTime());
                date.setTime(date.getTime() + (days * 20 * 1000));
                var expires = "; expires=" + date.toGMTString();
               // $.cookie("birthdayBubble", "5", { expires: expirationDate });

            }
            else var expires = "";
            document.cookie = name + "=" + value + expires + "; path=/";
        }

        function readCookie(name) {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
            }
            return null;
        }

        function eraseCookie(name) {
            createCookie(name, "", -1);
        }
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

