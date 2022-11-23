$(document).ready(function () {
    var reit_blocks = $('.book-card-raiting-number');
    reit_blocks.each(function () { 
        var reit = $(this).attr('id');
        var color = 'black';
        if (reit > 8) color = 'green';
        else if (reit > 7) color = '#4cff00';
        $(this).css('color', color);
    });
});
