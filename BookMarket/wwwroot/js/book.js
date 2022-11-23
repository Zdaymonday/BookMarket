$(document).ready(function () {
    $('.book-review-container').each(function (e) {
        let more_button = $('.read-more-button', this);
        let hide_button = $('.hide-button', this);
        let text_block = $('.review-textblock-bottom', this);

        let noContent = text_block.text().length < 3;
        if (noContent) more_button.css('display', 'none');

        more_button.click(function () {
            more_button.css('display', 'none');
            hide_button.css('display', 'inherit');
            text_block.css('height', '100%');
        });

        hide_button.click(function () {
            more_button.css('display', 'inherit');
            hide_button.css('display', 'none');
            text_block.css('height', '0');
        });
    });

    let price_block = $('.text-price');
    let outOfStock = price_block.text().length > 11;
    if (outOfStock) {
        $('.put-in-cart-btn').attr('disabled', 'disabled');
        
    }

});