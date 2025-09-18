$(document).ready(function () {

  
    // Mobile menu toggle
    $('.mobile-menu-toggle').click(function () {
        if (isSweetAlertOpen()) return;
        $('.nav-menu').slideToggle();
    });

    // Sidebar toggle para dispositivos móviles
    $('.sidebar-toggle').click(function () {       
        console.log('Sidebar toggle clicked');
        $('.sidebar').toggleClass('open');
        $('.main-content').toggleClass('expanded');
    });

    $(document).on('click', '.sidebar-close', function () {
        $('.sidebar').removeClass('open');
        $('.main-content').removeClass('expanded');
    });


    $('.has-submenu > a').click(function (e) {       
        e.preventDefault();

        const parent = $(this).parent();
        const submenu = parent.find('.submenu').first();

        if (submenu.is(':visible')) {
            submenu.slideUp();
            parent.removeClass('open');
        } else {
            submenu.slideDown();
            parent.addClass('open');
        }
    });



});


