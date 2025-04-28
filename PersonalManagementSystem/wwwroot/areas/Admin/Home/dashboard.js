$(function () {
    var $btns = $('.note-link').click(function () {
        if (this.id == 'all-category') {
            $('.single-note-item').show();
        } else {
            var category = this.id;
            $('.single-note-item').hide();
            $('.' + category).show();
        }
        $btns.removeClass('active');
        $(this).addClass('active');
    });
});
