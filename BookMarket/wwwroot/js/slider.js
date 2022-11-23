$(document).ready(function () {
    $('.slider_wrapper').each(function (i, elem) {
        let cnt = 0;
        let next = $('.next' ,elem);
        let prev = $('.prev' , elem);
        let slider_line = $('.slider-line', elem);
        let card = $(".slider-card").first();
        let card_image = $(".slider-card img").first();
        

        next.click(function () {
            cnt--;

            let offset = calculateOffset();

            if (cnt != 0) prev.css("display", "inherit");
            if (cnt == -4) next.css("display", "none");
            slider_line.css('left', offset * cnt + 'px');
        });

        prev.click(function () {
            cnt++;

            let offset = calculateOffset();
            
            if (cnt == 0) prev.css("display", "none");
            if (cnt != -4) next.css("display", "inherit");
            slider_line.css('left', offset * cnt + 'px');
        });

        function calculateOffset() {
            let width = card_image.css('width');
            let margin = card.css('margin-right');

            let offset = parseInt(width.substring(0, width.length - 2));
            offset += parseInt(margin.substring(0, margin.length - 2));

            return offset;
        }
    });

    
});