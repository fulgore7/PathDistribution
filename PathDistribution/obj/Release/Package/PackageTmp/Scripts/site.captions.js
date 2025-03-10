$(document).ready(function( ) {
    $('#content-description img').each(function () {
        var $parentimg = $(this);
        var $imagewidth = $parentimg.outerWidth();
        var $caption = $parentimg.attr('alt');

        if ($caption != '') {
            $parentimg.wrap('<div class="image-in-content" />');
            var wrapper = $parentimg.closest('div.image-in-content');
            wrapper.css({                
                "display": "block",
                "margin": "0 auto 0",
                "width:": $imagewidth + 'px',
                "float": $parentimg.css('float')
            });
            
            $parentimg.after('<div class="image-caption" style="clear: both;">' + $caption + '</div>');
        }
    });
});