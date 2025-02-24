/*Searching Topics*/
$(document).ready(function () {
    function searchTopics() {
        var topicName = $('#topicName').val();
        $.get('/Home/Search', { topicName: topicName }, function (result) {
            $('#allTopicsDiv').html(result).fadeIn('slow');
        });
    }

    // Delay function to throttle user input
    let debounceTimer;
    $('#topicName').on('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function () {
            searchTopics();
        }, 300); // Delay of 300ms before triggering the search to avoid too many requests
    });

    // Trigger search when search button is clicked
    $('#search').click(function () {
        searchTopics();
    });

    // Trigger search when Enter key is pressed in the input field
    $('#topicName').keypress(function (e) {
        if (e.which === 13) { // 13 is the Enter key code
            searchTopics();
            e.preventDefault(); // Prevent form submission
        }
    });
});

// JavaScript/jQuery to set the current topic page value when the modal is shown
$('#addTutorialModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var currentPage = button.data('current-page');
    var modal = $(this);  // Set the value to the hidden input field inside the modal
    modal.find('#currentPage').val(currentPage);
});


//add topic
function addTutorial() {
    var name = $('#tutorialName').val();
    var currentPage = $('#currentPage').val();

    $.ajax({
        type: "POST",
        url: "/Admin/AddTutorial",
        data: { tutorialName: name },

        success: function () {
            topicNavBarUpdate();
            loadPage(currentPage);
            $('#addTutorialModal').modal('hide');
            $('#tutorialName').val('');
        },
        error: function () {
            alert('Failed to Add Tutorial.');
        }
    });
}

function loadPage(pageNumber) {
    $.ajax({
        url: '/Home/topicsPartialViewUpdate',
        type: 'GET',
        data: { page: pageNumber },
        success: function (result) {
            $('#allTopicsDiv').html(result);
        },
        error: function () {
            alert('Error loading topics.');
        }
    });
}

function topicNavBarUpdate() {
    $.ajax({
        type: "GET",
        url: "/Home/topicNavBarUpdate",
        success: function (result) {
            $('#topic-wrapper').html(result).fadeIn('slow');
            checkOverflow();
        },
        error: function () {
            alert('Failed to Update Topic Navbar.');
        }
    });
}



let tutorialIdToDelete = null;

$(document).on('click', '.delete-tutorial-icon', function () {
    tutorialIdToDelete = $(this).data('tutorial-id');
    currentPage = $(this).data('current-page');
    topicCount = $(this).data('topiccount');
});

$('#confirmDeleteTutorialButton').on('click', function () {
    if (tutorialIdToDelete) {
        $.ajax({
            url: '/Admin/DeleteTutorial',
            type: 'POST',
            data: {
                id: tutorialIdToDelete
            },
            success: function () {
                topicNavBarUpdate();
                if (currentPage != 1 && topicCount == 1) {
                    loadPage(currentPage - 1);
                }
                else {
                    loadPage(currentPage);
                }

                $('#confirmDeleteTutorialModal').modal('hide');
            },
            error: function (xhr, status, error) {
                alert('An error occurred while deleting the Tutorial.');
            }
        });
    }
});

