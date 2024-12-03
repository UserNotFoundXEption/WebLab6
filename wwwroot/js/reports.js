//obsluga guzika create new report
function reportsForm() {
    const table = document.getElementById("dynamicTable").getElementsByTagName('tbody')[0];

    const newRow = table.insertRow();
    const reportCell = newRow.insertCell(0);
    const actionCell = newRow.insertCell(1);

    reportCell.innerHTML = `<input type="text" placeholder="Report" class="w3-input">`;
    actionCell.innerHTML = `
            <button onclick="reportsSaveButton(this)" class="w3-button w3-green"><i class="fa fa-save"></i> Send</button>
            <button onclick="reportsCancelButton(this)" class="w3-button w3-red"><i class="fa fa-trash"></i> Cancel</button>`;
}

//obsluga guzika cancel
function reportsCancelButton(button) {
    const row = button.parentNode.parentNode;
    row.parentNode.removeChild(row);
}

//obsluga guzika save
function reportsSaveButton(button) {
    const row = button.parentNode.parentNode;
    const reportCell = row.cells[0];
    const reportValue = reportCell.querySelector('input').value;
    reportsCancelButton(button);
    sendMessageAddReport(chatGetUserId(), reportValue);
}

//wywolane przez websocket
function reportsAddReportUser(reportId, report, status) {
    const table = document.getElementById("dynamicTable").getElementsByTagName('tbody')[0];

    const newRow = table.insertRow();
    newRow.setAttribute("report-id", reportId);
    const reportCell = newRow.insertCell(0);
    const statusCell = newRow.insertCell(1);
    const actionCell = newRow.insertCell(2);

    reportCell.innerHTML = `<span>${report}</span>`;
    statusCell.innerHTML = `<span>${status}</span>`
    actionCell.innerHTML = `
            <button onclick="reportsDeleteButton(this)" class="w3-button w3-red"><i class="fa fa-trash"></i> Delete</button>`;
}

//wywolane przez websocket
function reportsAddReportAdmin(reportId, userId, report, status) {
    const table = document.getElementById("dynamicTable").getElementsByTagName('tbody')[0];

    const newRow = table.insertRow();
    newRow.setAttribute("report-id", reportId);
    const userCell = newRow.insertCell(0);
    const reportCell = newRow.insertCell(1);
    const statusCell = newRow.insertCell(2);
    const actionCell = newRow.insertCell(3);

    userCell.innerHTML = `<span>${userId}</span>`;
    reportCell.innerHTML = `<span>${report}</span>`;
    statusCell.innerHTML = `<select class="w3-select" onchange="reportsHandleStatusChange(this)">
        <option value="Pending" ${status === "Pending" ? "selected" : ""}>Pending</option>
        <option value="Completed" ${status === "Completed" ? "selected" : ""}>Completed</option>
        <option value="In Progress" ${status === "In Progress" ? "selected" : ""}>In Progress</option>
    </select>`;
    actionCell.innerHTML = `
            <button onclick="reportsDeleteButton(this)" class="w3-button w3-red"><i class="fa fa-trash"></i> Remove</button>`;
}

//od razu po zmianie statusu, wywolywane z javascripta
function reportsHandleStatusChange(statusCell) {
    const status = statusCell.value;
    const reportId = statusCell.parentNode.parentNode.getAttribute("report-id")
    sendMessageEditReport(reportId, status)
}

//wywolane przez websocket
function reportsChangeStatus(reportId, status) {
    const report = reportsGetReport(reportId);
    if (window.location.pathname == '/admin') {
        report.cells[2].innerHTML = `<select class="w3-select" onchange="reportsHandleStatusChange(this)">
        <option value="Pending" ${status === "Pending" ? "selected" : ""}>Pending</option>
        <option value="Completed" ${status === "Completed" ? "selected" : ""}>Completed</option>
        <option value="In Progress" ${status === "In Progress" ? "selected" : ""}>In Progress</option>
    </select>`;
    }
    else {
        report.cells[1].innerHTML = `<span>${status}</span>`;
    }
}


//obsluga guzika
function reportsDeleteButton(button) {
    const report = button.parentNode.parentNode;
    sendMessageDeleteReport(report.getAttribute("report-id"));
}

//usuniecie wywolane przez websocket
function reportsDeleteReport(reportId) {
    const report = reportsGetReport(reportId);
    report.parentNode.removeChild(report);
}

function reportsGetReport(reportIdToDelete) {
    const reports = document.querySelectorAll("#dynamicTable tbody tr");
    for (const report of reports) {
        const reportId = report.getAttribute("report-id");
        if (reportId == reportIdToDelete) {
            if (report != null) {
                return report;
            }
        }
    }
}