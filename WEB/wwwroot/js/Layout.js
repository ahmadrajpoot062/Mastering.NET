
document.addEventListener('DOMContentLoaded', function () {
    var swiper = new Swiper('.swiper', {
        loop: true,
        speed: 600,
        autoplay: {
            delay: 5000,
        },
        slidesPerView: 'auto',
        pagination: {
            el: '.swiper-pagination',
            type: 'bullets',
            clickable: true,
        },
        breakpoints: {
            320: {
                slidesPerView: 1,
                spaceBetween: 40
            },
            1200: {
                slidesPerView: 3,
                spaceBetween: 1
            }
        }
    });
});


// Detect content overflow and toggle arrow visibility
function checkOverflow() {
    const container = document.getElementById('topic-container');
    const scrollLeft = document.getElementById('scroll-left');
    const scrollRight = document.getElementById('scroll-right');

    // Check if the container content overflows horizontally
    if (container.scrollWidth > container.clientWidth) {
        scrollLeft.style.display = 'flex';
        scrollRight.style.display = 'flex';
    } else {
        scrollLeft.style.display = 'none';
        scrollRight.style.display = 'none';
    }
}

// Scroll the topics when the arrows are clicked
document.getElementById('scroll-left').addEventListener('click', function () {
    const container = document.getElementById('topic-container');
    container.scrollBy({
        left: -200, // Scroll left by 200px
        behavior: 'smooth'
    });
});

document.getElementById('scroll-right').addEventListener('click', function () {
    const container = document.getElementById('topic-container');
    container.scrollBy({
        left: 200, // Scroll right by 200px
        behavior: 'smooth'
    });
});

// Run the overflow check on page load and on window resize
window.onload = checkOverflow;
window.onresize = checkOverflow;
    