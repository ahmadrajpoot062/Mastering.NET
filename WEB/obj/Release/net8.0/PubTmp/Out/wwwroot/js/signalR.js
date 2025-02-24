const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start().catch(err => console.error(err));

// Handle notification from the server
connection.on("ReceiveNotification", (id, topicName, lectureTitle) => {
    // Update toastr onclick for redirect
    toastr.options.onclick = function () {
        // Build the URL dynamically on the client-side
        const url = `/Home/Topic?id=${id}&topicName=${encodeURIComponent(topicName)}`;
        window.location.href = url;
    };

    toastr.info(`
    <span style="font-size: 15px; font-weight: bold; font-family: Arial, sans-serif;">
        <i class="fas fa-book"></i> New Material Uploaded
    </span><br>
    <span style="font-size: 13px; font-family: Arial, sans-serif;">
        "${lectureTitle}" added in Topic "${topicName}"
    </span><br>
    <span style="font-size: 10px;">
        Mastering.NET
    </span>
`);


});

// Configure toastr options with longer durations
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-bottom-right", // You can change this to other positions like toast-top-right, etc.
    "preventDuplicates": false,
    "showDuration": "1000",     // Increase show duration to 1 second (1000 ms)
    "hideDuration": "2000",     // Increase hide duration to 1.5 seconds (1500 ms)
    "timeOut": "12000",         // Toast stays visible for 10 seconds (10000 ms)
    "extendedTimeOut": "4000",  // Add 2 seconds (2000 ms) on hover
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

