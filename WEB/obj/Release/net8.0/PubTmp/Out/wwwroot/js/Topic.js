// Overflow detection and arrow toggle
function checkOverflow() {
    const container = document.getElementById('topic-container');
    const scrollLeft = document.getElementById('scroll-left');
    const scrollRight = document.getElementById('scroll-right');

    if (container.scrollWidth > container.clientWidth) {
        scrollLeft.style.display = 'flex';
        scrollRight.style.display = 'flex';
    } else {
        scrollLeft.style.display = 'none';
        scrollRight.style.display = 'none';
    }
}

document.getElementById('scroll-left').addEventListener('click', function () {
    const container = document.getElementById('topic-container');
    container.scrollBy({ left: -200, behavior: 'smooth' });
});

document.getElementById('scroll-right').addEventListener('click', function () {
    const container = document.getElementById('topic-container');
    container.scrollBy({ left: 200, behavior: 'smooth' });
});

window.onload = checkOverflow;
window.onresize = checkOverflow;

// Navbar toggle functionality
const toggleBtn = document.getElementById('toggleNavbar');
const leftNavbar = document.getElementById('leftNavbar');

const observer = new MutationObserver((mutationsList) => {
    mutationsList.forEach((mutation) => {
        if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
            if (leftNavbar.classList.contains('active')) {
                toggleBtn.style.animation = 'none';
                console.log('The "active" class is present on leftNavbar.');
            } else {
                toggleBtn.style.animation = 'pulse 0.3s infinite alternate';
                console.log('The "active" class is not present on leftNavbar.');
            }
        }
    });
});

// Start observing the leftNavbar for class changes
observer.observe(leftNavbar, { attributes: true });
toggleBtn.addEventListener('click', function () {
    leftNavbar.classList.toggle('active');
});



// AJAX load lecture
function loadLecture(lectureId) {
    $.ajax({
        url: '/Home/loadLecture',
        type: 'GET',
        data: { id: lectureId },
        success: function (response) {            
            $('#content-section').html(response).fadeIn('slow');
            if (window.innerWidth <= 992) {
                const leftNavbar = document.getElementById('leftNavbar');
                leftNavbar.classList.toggle('active'); // Hide sidebar
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

// AJAX add lecture
let tutorialId = null;
$(document).on('click', '.add-lecture-icon', function () {
    tutorialId = $(this).data('tutorial-id');
});

$('#addLectureButton').on('click', function () {
    if (tutorialId) {
        var formData = new FormData();
        formData.append('title', $('#title').val());
        formData.append('content', $('#content').val());
        formData.append('topicId', tutorialId);

        $.ajax({
            url: '/Admin/UploadLecture',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                $('#leftNavDiv').html(result).fadeIn('slow');
                $('#uploadLectureModel').modal('hide');
                $('#title').val('');
                $('#content').val('');
            },
            error: function (xhr, status, error) {
                alert('An error occurred while uploading the lecture.');
            }
        });
    } else {
        alert('No tutorial ID found.');
    }
});

// AJAX delete lecture
let lectureIdToDelete = null;
$(document).on('click', '.delete-icon', function () {
    lectureIdToDelete = $(this).data('lecture-id');
    topicId = $(this).data('topic-id');
});

$('#confirmDeleteButton').on('click', function () {
    if (lectureIdToDelete) {
        $.ajax({
            url: '/Admin/DeleteLecture',
            type: 'POST',
            data: { id: lectureIdToDelete, topicid: topicId },
            success: function (result) {
                $('#leftNavDiv').html(result).fadeIn('slow');
                var myhtml = '';
                $('#content-section').html(myhtml).fadeIn('slow');
                $('#lecture-name').html(myhtml).fadeIn('slow');
                $('#confirmDeleteModal').modal('hide');
            },
            error: function (xhr, status, error) {
                alert('An error occurred while deleting the lecture.');
            }
        });
    }
});

// Set active link
function setActive(selectedLink) {
    var links = document.querySelectorAll('.lecture-link');
    links.forEach(function (link) {
        link.classList.remove('active');
    });
    selectedLink.classList.add('active');
}
