$(document).ready(function () {
    $('body').on('mouseenter', '.display', function(e) {
        var $this = $(e.currentTarget),
            $root = $this;
        if ($this.data('parent')) {
            $root = $this.closest($this.data('parent'));
        }

        $root.find($this.data('toggle')).show();
    });
    
    $('body').on('mouseleave', '.display', function (e) {
        var $this = $(e.currentTarget),
            $root = $this;
        if ($this.data('parent')) {
            $root = $this.closest($this.data('parent'));
        }

        $root.find($this.data('toggle')).hide();
    });

    $('body').on('click', 'ul.leftnavlist .hasChild', function(e) {
        e.stopPropagation();
        
        if ($(e.target).is('a') == false) {
            var $this = $(e.currentTarget);

            $this.toggleClass('active');
        }

        
    });

    Cufon.replace('.cufon');
});