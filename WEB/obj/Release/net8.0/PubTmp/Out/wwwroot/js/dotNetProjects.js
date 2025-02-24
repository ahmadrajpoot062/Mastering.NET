/*Searching Projects*/
$(document).ready(function () {
    function searchDotNetProjects() {
        var projectName = $('#dotNetProjectName').val();
        $.get('/Home/SearchDotNetProjects', { projectName: projectName }, function (result) {
            $('#dotNetProjectsDiv').html(result).fadeIn('slow');
        });
    }

    // Delay function to throttle user input
    let debounceTimer;
    $('#dotNetProjectName').on('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function () {
            searchDotNetProjects();
        }, 300); // Delay of 300ms before triggering the search to avoid too many requests
    });

    // Trigger search when search button is clicked
    $('#searchDotNetProjects').click(function () {
        searchDotNetProjects();
    });

    // Trigger search when Enter key is pressed in the input field
    $('#dotNetProjectName').keypress(function (e) {
        if (e.which === 13) { // 13 is the Enter key code
            searchDotNetProjects();
            e.preventDefault(); // Prevent form submission
        }
    });
});

// JavaScript/jQuery to set the current project page value when the modal is shown
$('#addDotNetProjectModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var currentPage = button.data('currentproject-page');
    var modal = $(this);
    modal.find('#currentProjectPage').val(currentPage);
});

//add project
function addDotNetProject() {
    var formData = new FormData(document.getElementById("addDotNetProjectForm"));
    var currentProjectPage = $('#currentProjectPage').val();

    $.ajax({
        url: '/Admin/AddDotNetProject',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function () {
            loadDotNetProjectPage(currentProjectPage);
            $('#addDotNetProjectModal').modal('hide');
            $('#addDotNetProjectForm')[0].reset();
        },
        error: function (xhr, status, error) {
            console.log("Error: " + error);
        }
    });
}

function loadDotNetProjectPage(pageNumber) {
    $.ajax({
        url: '/Home/DotNetProjectsPartialViewUpdate',
        type: 'GET',
        data: { page: pageNumber },
        success: function (result) {
            $('#dotNetProjectsDiv').html(result);
        },
        error: function () {
            alert('Error loading .NET Projects.');
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
        deleteDotNetProject(projectId, projectPage, projectCount);
    };
});

function deleteDotNetProject(projectId, projectPage, projectCount) {
    $.ajax({
        url: '/Admin/DeleteDotNetProject',
        type: 'POST',
        data: { id: projectId },
        success: function () {
            if (projectPage != 1 && projectCount == 1) {
                loadDotNetProjectPage(projectPage - 1);
            } else {
                loadDotNetProjectPage(projectPage);
            }
            $('#SureModal').modal('hide');
        },
        error: function () {
            alert('An error occurred while deleting the project');
        }
    });
}