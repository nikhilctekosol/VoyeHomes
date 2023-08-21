$(function () {
    $('.main-slider').slick({
        infinite: false,
        variableWidth: true,
        slidesToShow: 4,
        slidesToShow: 1,
        slidesToScroll: 1,
        swipe: true,
        // swipeToSlide: true,
    });

    $(document).ready(function () {
        $(window).scroll(function () {
            if ($(this).scrollTop() > 5) {
                $(".navbar-me").addClass("fixed-me");
            } else {
                $(".navbar-me").removeClass("fixed-me");
            }
        });
    });

    $(document).ready(function () {
        $('#myCollapsible').collapse({
            toggle: false
        })
    });


    $(document).ready(function () {
        $("#verify-btn").click(function () {
            $("#otp").hide();
            $("#verify-btn").hide();
            $("#thanks").show();
        });
    });

    if ($('.carousel2-1 .carousel').length > 0) {
        $('.carousel2-1 .carousel').slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            centerMode: true,
            autoplay: false,
            autoplaySpeed: 5000,
            centerPadding: '70px',
            arrows: true,
            dots: false,
            responsive: [
                {
                    breakpoint: 1200,
                    settings: {
                        centerPadding: '160px',
                        slidesToShow: 1
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        centerPadding: '30px',
                        slidesToShow: 1
                    }
                }
            ]
        });
    }

    // Slick lightbox
    if ($('.lightbox').length > 0) {
        $('.lightbox').slickLightbox({
            itemSelector: 'a.open-lightbox',
            caption: 'caption',
            navigateByKeyboard: true,
            layouts: {
                closeButton: '<button class="btn close"><i class="fas fa-times"></i></button>'
            }
        });
    }


    $('.main-slider').on('afterChange', function (event, slick, currentSlide, nextSlide) {
        console.log(currentSlide);
    });

});

$(document).ready(function () {
    $("#menu-close").click(function (e) {
        e.preventDefault();
        $("#sidebar-wrapper").toggleClass("active");
    });
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#sidebar-wrapper").toggleClass("active");
    });
    $('.menu-toggle').on('click', function () {
        $('.wrapper').toggleClass('menu--is-revealed');
    });
});

$(document).ready(function () {

    $('.master-slider').slick({
        dots: true,
        infinite: false,
        speed: 1000,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 2000,
        fade: true,
        speed: 500,
        infinite: true,
        cssEase: 'ease-in-out',
        touchThreshold: 100,
        arrows: false
    });
    $('.value-slider').slick({
        dots: false,
        infinite: true,
        speed: 500,
        slidesToShow: 4,
        slidesToScroll: 1,
        autoplay: false,
        arrows: false,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    variableWidth: true,
                    slidesToScroll: 1
                }
            }
        ]
    });
    $('.desti-slider').slick({
        dots: false,
        infinite: true,
        speed: 500,
        slidesToShow: 4,
        slidesToScroll: 1,
        autoplay: true,
        arrows: true,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    variableWidth: true,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 628,
                settings: {
                    slidesToShow: 2,
                    variableWidth: true,
                    slidesToScroll: 1
                }
            }
        ]
    });
    $('.desti-slider1').slick({
        dots: false,
        infinite: true,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: false,
        arrows: true,
        slidesPerRow: 6,
        rows: 2,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    variableWidth: true,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 628,
                settings: {
                    slidesToShow: 3,
                    variableWidth: true,
                    slidesToScroll: 1
                }
            }
        ],
        
    });
    $('.single-slider-auto').slick({
        dots: true,
        infinite: false,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        fade: true,
        speed: 500,
        infinite: true,
        cssEase: 'ease-in-out',
        touchThreshold: 100,
        arrows: false
    });
    $('.client-slider').slick({
        dots: false,
        infinite: false,
        speed: 500,
        slidesToShow: 2,
        slidesToScroll: 1,
        autoplay: true,
        arrows: false,
        responsive: [
            {
                breakpoint: 628,
                settings: {
                    slidesToShow: 1,
                    speed: 800,
                    dots: true,
                    slidesToScroll: 1
                }
            }
        ]
    });
    $('.client-slider-single').slick({
        dots: false,
        infinite: false,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        arrows: true,
        responsive: [
            {
                breakpoint: 628,
                settings: {
                    slidesToShow: 1,
                    speed: 800,
                    dots: true,
                    slidesToScroll: 1
                }
            }
        ]
    });
    $('.listing-slider').slick({
        dots: false,
        infinite: true,
        autoplay: false,
        centerMode: false,
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: true,
        variableWidth: true
    });


});






