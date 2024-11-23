$("#print-button").click(function () {
    printTicket()
    location.reload();
});

function printTicket() {
    var printContent = document.getElementById('ticket-content').innerHTML;
    var originalContent = document.body.innerHTML;
    document.body.innerHTML = printContent;
    window.print();
    document.body.innerHTML = originalContent;
}
