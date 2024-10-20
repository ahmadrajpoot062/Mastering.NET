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

/*Searching Projects*/
$(document).ready(function () {
    function searchProjects() {
        var projectName = $('#projectName').val();
        $.get('/Home/SearchProjects', { projectName: projectName }, function (result) {
            $('#allProjectsDiv').html(result).fadeIn('slow');
        });
    }

    // Delay function to throttle user input
    let debounceTimer;
    $('#projectName').on('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function () {
            searchProjects();
        }, 300); // Delay of 300ms before triggering the search to avoid too many requests
    });

    // Trigger search when search button is clicked
    $('#searchProjects').click(function () {
        searchProjects();
    });

    // Trigger search when Enter key is pressed in the input field
    $('#projectName').keypress(function (e) {
        if (e.which === 13) { // 13 is the Enter key code
            searchProjects();
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

// JavaScript/jQuery to set the current project page value when the modal is shown
$('#addProjectModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var currentPage = button.data('currentproject-page');
    var modal = $(this);
    modal.find('#currentProjectPage').val(currentPage);
});

function addTutorial() {
    var name = $('#tutorialName').val();
    var currentPage = $('#currentPage').val();

    $.ajax({
        type: "POST",
        url: "/Admin/AddTutorial",
        data: { tutorialName: name },

        success: function (result) {
            $('#offCanvasDiv').html(result).fadeIn('slow');
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

function addProject() {
    var formData = new FormData(document.getElementById("addProjectForm"));
    var currentProjectPage = $('#currentProjectPage').val();

    $.ajax({
        url: '/Admin/AddProject',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function () {
            loadProjectPage(currentProjectPage);
            $('#addProjectModal').modal('hide');
            $('#addProjectForm')[0].reset();
        },
        error: function (xhr, status, error) {
            console.log("Error: " + error);
        }
    });
}

function loadProjectPage(pageNumber) {
    $.ajax({
        url: '/Home/ProjectsPartialViewUpdate',
        type: 'GET',
        data: { page: pageNumber },
        success: function (result) {
            $('#allProjectsDiv').html(result);
        },
        error: function () {
            alert('Error loading Projects.');
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

function sendMessage() {
    var formData = new FormData(document.getElementById("sendMessageForm"));

    fetch('/Home/sendMessage', {
        method: 'POST',
        body: new URLSearchParams(formData)
    }).then(response => {
        if (response.ok) {
            alert("Your message has been successfully sent!");
            document.getElementById("sendMessageForm").reset();
        } else {
            alert("There was an error submitting your message.");
        }
    }).catch(error => {
        console.error('Error:', error);
        alert("There was an error submitting your message.");
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
            success: function (result) {
                $('#offCanvasDiv').html(result).fadeIn('slow');
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

var sureModal = document.getElementById('SureModal');
sureModal.addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    var projectId = button.getAttribute('data-id');
    var projectTitle = button.getAttribute('data-title');
    var projectPage = button.getAttribute('data-currentProject-page');
    var projectCount = button.getAttribute('data-ProjectCount');

    var modalTitleElement = sureModal.querySelector('#modalProjectTitle');
    modalTitleElement.textContent = projectTitle;

    var confirmDeleteButton = sureModal.querySelector('#confirmDeleteProjectButton');
    confirmDeleteButton.onclick = function () {
        deleteProject(projectId, projectPage, projectCount);
    };
});

function deleteProject(projectId, projectPage, projectCount) {
    $.ajax({
        url: '/Admin/DeleteProject',
        type: 'POST',
        data: { id: projectId },
        success: function () {
            if (projectPage != 1 && projectCount == 1) {
                loadProjectPage(projectPage - 1);
            } else {
                loadProjectPage(projectPage);
            }
            $('#SureModal').modal('hide');
        },
        error: function () {
            alert('An error occurred while deleting the project');
        }
    });
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('td[data-href]').forEach(row => {
        row.addEventListener('click', function () {
            window.location.href = this.dataset.href; // Redirects to the action URL
        });
    });
});
