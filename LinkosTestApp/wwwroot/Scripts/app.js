$(document).ready(function () {

    //Імпорт 
    $('#importBtn').click(function () {
        $('#importFile').click();
    });

    $('#importFile').change(function () {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var content = e.target.result;
                jQuery.ajax({
                    url: '/api/groups/import',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(content),
                    success: function (response) {
                        showAlert(response, 'success');
                    },
                    error: function (xhr) {
                        showAlert('Помилка: ' + xhr.responseText, 'danger');
                    }
                });
            };
            reader.readAsText(file);
        }
    });

    //Перевірка наявності світла
    $('#checkAvailabilityBtn').click(function () {
        var groupNumber = $('#groupNumber').val();
        jQuery.ajax({
            url: '/api/groups/' + groupNumber + '/availability',
            type: 'GET',
            success: function (data) {
                var message = data.isElectricityAvailable ? 'Світло є.' : 'Світла немає.';
                $('#availabilityResult').text(message);
            },
            error: function (xhr) {
                showAlert('Помилка: ' + xhr.responseText, 'danger');
            }
        });
    });

    //Редагування графіка групи
    $('#loadGroupBtn').click(function () {
        var groupNumber = $('#editGroupNumber').val();
        jQuery.ajax({
            url: '/api/groups/' + groupNumber,
            type: 'GET',
            success: function (group) {
                $('#editGroupForm').show();
                var schedulesContainer = $('#schedulesContainer');
                schedulesContainer.empty();
                group.schedules.forEach(function (schedule, index) {
                    var scheduleDiv = $('<div class="form-inline mb-2">');
                    scheduleDiv.append('<input type="text" name="schedules[' + index + '].From" class="form-control mr-2" value="' + schedule.from + '" placeholder="Час початку (HH:mm)" />');
                    scheduleDiv.append('<input type="text" name="schedules[' + index + '].To" class="form-control mr-2" value="' + schedule.to + '" placeholder="Час завершення (HH:mm)" />');
                    schedulesContainer.append(scheduleDiv);
                });
            },
            error: function (xhr) {
                showAlert('Помилка: ' + xhr.responseText, 'danger');
            }
        });
    });

    $('#editGroupScheduleForm').submit(function (e) {
        e.preventDefault();
        var groupNumber = $('#editGroupNumber').val();
        var formData = $(this).serializeArray();
        var schedules = [];

        for (var i = 0; i < formData.length; i += 2) {
            var from = formData[i].value;
            var to = formData[i + 1].value;
            schedules.push({ from: from, to: to });
        }

        jQuery.ajax({
            url: '/api/groups/' + groupNumber,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(schedules),
            success: function (response) {
                showAlert(response, 'success');
            },
            error: function (xhr) {
                showAlert('Помилка: ' + xhr.responseText, 'danger');
            }
        });
    });

    //Експорт даних
    $('#exportBtn').click(function () {
        var groupNumber = $('#exportGroupNumber').val();
        var url = '/api/groups/export';
        if (groupNumber) {
            url += '?groupNumber=' + groupNumber;
        }
        window.location.href = url;
    });

    //Завантаження списку груп
    $('#loadGroupsBtn').click(function () {
        jQuery.ajax({
            url: '/api/groups',
            type: 'GET',
            success: function (groups) {
                var groupsList = $('#groupsList');
                groupsList.empty();
                groups.forEach(function (group) {
                    var groupDiv = $('<div class="card mb-3">');
                    var cardBody = $('<div class="card-body">');
                    cardBody.append('<h5 class="card-title">Група ' + group.number + '</h5>');
                    var scheduleList = $('<ul class="list-group list-group-flush">');
                    group.schedules.forEach(function (schedule) {
                        scheduleList.append('<li class="list-group-item">' + schedule.from + ' - ' + schedule.to + '</li>');
                    });
                    groupDiv.append(cardBody);
                    groupDiv.append(scheduleList);
                    groupsList.append(groupDiv);
                });
            },
            error: function (xhr) {
                showAlert('Помилка: ' + xhr.responseText, 'danger');
            }
        });
    });

    //Функція для відображення повідомлень
    function showAlert(message, type) {
        var alertDiv = $('<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">');
        alertDiv.text(message);
        alertDiv.append('<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>');
        $('.container').prepend(alertDiv);
    }
});
