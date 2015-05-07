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
});