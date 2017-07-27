var _contextAction;
var _password;

function sendRequest(init) {
    var url = "/Api/ChatBot/";
    var msg = $("#textInput").val();
    var valid = false;

    $("#textInput").prop("disabled", true);
    $("#chat_loader").show();

    if (init) {
        msg = 'hola';
    }

    if ($("#textInput").val() === "" && init === undefined) {
        toastr.error("Debe ingresar un texto", "Error")
        resetInputChat();
        return;
    }

    if (init != true) {
        if (_contextAction === false || _contextAction === undefined) {
            appendMessage(true, msg);
        }
    }


    if (_contextAction === "email") {
        if (validateEmail(msg)) {
            //NotificationToast("sucess", "Correo válido", "Confirmación");
            valid = true;
            appendMessage(true, msg);
        }
        else {
            NotificationToast("error", "Correo inválido", "Error");
            valid = false;
        }
    }

    if (_contextAction === "password") {
        if (msg.length < 8) {
            NotificationToast("error", "La clave debe ser mínimo de 8 caracteres", "Error");
            valid = false;
        }
        else {
            //NotificationToast("sucess", "Excelente!", "Confirmación");
            valid = true;
            _password = msg;
        }
    }

    if (_contextAction === "confirmation") {
        if (_password != msg) {
            NotificationToast("error", "La confirmación no coincide con la clave. Intente de nuevo", "Error");
            valid = false;
        }
        else {
            NotificationToast("sucess", "Clave modificada con éxito!", "Confirmación");
            valid = true;
        }
    }



    $.post(url, { msg: msg, isInit: init, isValid: valid }, function (result) {
        //var obj = JSON.parse(JSON.stringify(result))
        var obj = JSON.parse(result)
        var total = obj.output.text.length;

        _contextAction = obj.context.action;

        for (var i = 0; i < total; i++) {
            //NotificationToast("sucess", obj.output.text[i], "Watson");

            appendMessage(false, obj.output.text[i]);

        }

        $("#countChats").text(total);

        resetInputChat();

        if (_contextAction === "password" || _contextAction === "confirmation") {
            $("#textInput").attr("type", "password");
        }

    })
        .done(function () {
            resetInputChat();
        })
        .fail(function (error) {
            resetInputChat();

            NotificationToast("error", error.statusText, "Error");
        });

}

function resetInputChat() {
    $("#textInput").prop("disabled", false);
    $("#textInput").val(null);
    $("#chat_loader").hide();

    $("#textInput").attr("type", "text");
    $("#textInput").focus();
}


function appendMessage(isUser, message) {
    var nombre = "Watson";
    var clase = "direct-chat-msg";
    var imagen = "/images/user1-128x128.jpg";
    var alig1 = "left";
    var alig2 = "right";

    if (isUser) {
        nombre = "Usuario";
        clase = "direct-chat-msg right";
        imagen = "/images/user3-128x128.jpg";
        var alig1 = "right";
        var alig2 = "left";
    }

    var hora = currentTime();

    var element = "<div class='" + clase + "'>" +
        "<div class='direct-chat-info clearfix'>" +
        "<span class='direct-chat-name pull-" + alig1 + "'>" + nombre + "</span>" +
        "<span class='direct-chat-timestamp pull-" + alig2 + "'>" + hora + "</span>" +
        "</div>" +
        "<img class='direct-chat-img' src='" + imagen + "' alt= '" + nombre + "'>" +
        "<div class='direct-chat-text'>" + message +
        "</div>" +
        "</div>"

    $("#scrollingChat").append(element);

    $("#scrollingChat").scrollTop($("#scrollingChat")[0].scrollHeight);

}

