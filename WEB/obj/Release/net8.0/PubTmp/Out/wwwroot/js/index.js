//delete messges
$(document).ready(function () {
    // Delegate event to handle clicks on dynamically loaded delete icons
    $(document).on('click', '.msg-delete-icon', function (event) {
        event.stopPropagation(); // Prevent row click
        var contactId = $(this).data('id');

        // Show confirmation dialog
        if (confirm("Are you sure you want to delete this message?")) {
            // Perform the AJAX request to delete the contact
            $.ajax({
                url: '/Admin/MessageDelete',  // Adjust the URL if needed
                type: 'POST',
                data: { id: contactId },
                success: function (result) {
                    // Update the messages section with the new content after deletion
                    $('#msgs').html(result).fadeIn('slow');
                },
                error: function (xhr, status, error) {
                    alert("An error occurred: " + error);
                }
            });
        }
    });

    // Handling row clicks
    $(document).on('click', 'td[data-href]', function (event) {
        // Ensure the trash icon doesn't trigger the row click
        if (!$(event.target).closest('.msg-delete-icon').length) {
            window.location.href = $(this).data('href');
        }
    });
});

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

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('td[data-href]').forEach(row => {
        row.addEventListener('click', function () {
            window.location.href = this.dataset.href; // Redirects to the action URL
        });
    });
});
