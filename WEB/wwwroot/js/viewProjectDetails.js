// Function to detect content overflow and toggle arrow visibility
function checkOverflow() {
    const container = document.getElementById('topic-container'); // The container holding the topic content
    const scrollLeft = document.getElementById('scroll-left'); // Left arrow for scrolling
    const scrollRight = document.getElementById('scroll-right'); // Right arrow for scrolling

    // Check if the container's content overflows horizontally
    if (container.scrollWidth > container.clientWidth) {
        scrollLeft.style.display = 'flex'; // Show the left scroll arrow
        scrollRight.style.display = 'flex'; // Show the right scroll arrow
    } else {
        scrollLeft.style.display = 'none'; // Hide the left scroll arrow
        scrollRight.style.display = 'none'; // Hide the right scroll arrow
    }
}

// Event listener for scrolling the content to the left when the left arrow is clicked
document.getElementById('scroll-left').addEventListener('click', function () {
    const container = document.getElementById('topic-container');
    container.scrollBy({
        left: -200, // Scroll left by 200px
        behavior: 'smooth' // Smooth scrolling effect
    });
});

// Event listener for scrolling the content to the right when the right arrow is clicked
document.getElementById('scroll-right').addEventListener('click', function () {
    const container = document.getElementById('topic-container');
    container.scrollBy({
        left: 200, // Scroll right by 200px
        behavior: 'smooth' // Smooth scrolling effect
    });
});

// Run the overflow check when the page loads
window.onload = checkOverflow;

// Run the overflow check whenever the window is resized
window.onresize = checkOverflow;

// jQuery function to handle image clicks in a project image carousel
$(document).ready(function () {
    $('#projectImagesCarousel img').on('click', function () {
        var index = $(this).closest('.carousel-item').index(); // Get the index of the clicked image
        $('#projectImagesCarousel').carousel(index); // Jump to the clicked image in the carousel
        $('#imageModal').modal('show'); // Show the modal with the image
        $('#modalImage').attr('src', $(this).attr('src')); // Set the clicked image as the source in the modal
    });
});
