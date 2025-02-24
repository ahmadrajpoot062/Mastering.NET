$(document).ready(function () {
    function searchBlazorProjects() {
        var projectName = $('#blazorProjectName').val();
        $.get('/Home/SearchBlazorProjects', { projectName: projectName }, function (result) {
            $('#blazorProjectsDiv').html(result).fadeIn('slow');
        });
    }

    // Delay function to throttle user input
    let debounceTimer;
    $('#blazorProjectName').on('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function () {
            searchBlazorProjects();
        }, 300); // Delay of 300ms before triggering the search to avoid too many requests
    });

    // Trigger search when search button is clicked
    $('#searchBlazorProjects').click(function () {
        searchBlazorProjects();
    });

    // Trigger search when Enter key is pressed in the input field
    $('#blazorProjectName').keypress(function (e) {
        if (e.which === 13) { // 13 is the Enter key code
            searchBlazorProjects();
            e.preventDefault(); // Prevent form submission
        }
    });
});

// JavaScript/jQuery to set the current project page value when the modal is shown
$('#addBlazorProjectModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var currentPage = button.data('currentproject-page');
    var modal = $(this);
    modal.find('#currentProjectPage').val(currentPage);
});

//add project
function addBlazorProject() {
    var formData = new FormData(document.getElementById("addBlazorProjectForm"));
    var currentProjectPage = $('#currentProjectPage').val();

    $.ajax({
        url: '/Admin/AddBlazorProject',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function () {
            loadBlazorProjectPage(currentProjectPage);
            $('#addBlazorProjectModal').modal('hide');
            $('#addBlazorProjectForm')[0].reset();
        },
        error: function (xhr, status, error) {
            console.log("Error: " + error);
        }
    });
}

function loadBlazorProjectPage(pageNumber) {
    $.ajax({
        url: '/Home/BlazorProjectsPartialViewUpdate',
        type: 'GET',
        data: { page: pageNumber },
        success: function (result) {
            $('#blazorProjectsDiv').html(result);
        },
        error: function () {
            alert('Error loading Blazor Projects.');
        }
    });
}

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
        deleteBlazorProject(projectId, projectPage, projectCount);
    };
});

function deleteBlazorProject(projectId, projectPage, projectCount) {
    $.ajax({
        url: '/Admin/DeleteBlazorProject',
        type: 'POST',
        data: { id: projectId },
        success: function () {
            if (projectPage != 1 && projectCount == 1) {
                loadBlazorProjectPage(projectPage - 1);
            } else {
                loadBlazorProjectPage(projectPage);
            }
            $('#SureModal').modal('hide');
        },
        error: function () {
            alert('An error occurred while deleting the project');
        }
    });
}