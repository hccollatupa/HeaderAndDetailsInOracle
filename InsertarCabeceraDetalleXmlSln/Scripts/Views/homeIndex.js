var controller = function () {

    var saveForm = function (evt) {
        evt.preventDefault();

        var header = $('#tableHeader').tableToJSON();
        var details = $('#tableDetails').tableToJSON();
        var postData = { Id: header[0].Id, Fecha: header[0].Fecha, Details: details };

        //console.log(JSON.stringify(postData));

        $.ajax({
            async: true,
            type: "POST",
            contentType: 'application/json',
            url: "/Home/Save",
            data: JSON.stringify(postData),
            success: function (data) {
                console.log(data);
                
                if (data.Error) {
                    alert(data.Message);
                }
                else {
                    alert(data.Value.Message);
                }
            }
        });

        return false;
    };

    var initializeLookAndFeel = function () {
    };

    var initializeEvents = function () {
        //definitions
        var bindRequiredEvents = function () {
            //events that have to be binded even if dialog is readonly or partially readonly
            $('.form-actions button').click(saveForm);
        };

        //bind events
        bindRequiredEvents();
    };

    return {
        Initialize: function () {
            initializeLookAndFeel();
            initializeEvents();
        }
    };
}();

var view = function () {
    function bindButtons() {

    }

    var initialize = function () {
        bindButtons();
    };

    return {
        Initialize: initialize
    };
}();

$('document').ready(function () {
    controller.Initialize();
    view.Initialize();
});