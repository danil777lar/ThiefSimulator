mergeInto(LibraryManager.library,
    { 
        ShowBrowserAlertJs: function (message)
        {
            window.alert(Pointer_stringify(message));
        },

        ShowTelegramAlertJs: function (message)
        {
            window.Telegram.WebApp.showAlert(Pointer_stringify(message));
        },

        SendDataToBotJs: function (message)
        {
            window.Telegram.WebApp.sendData(Pointer_stringify(message));
        },

        OpenInvoiceJs: function (url, index)
        {
            window.Telegram.WebApp.openInvoice(Pointer_stringify(url), (status) => 
            {
                feedback = `${index.toString()}/${status.toString()}`;
                window.unityInstance.SendMessage("TelegramBridge", "CatchInvoiceCallback", feedback);
            });
        },

        GetTelegramUserIdJs: function()
        {
            return window.Telegram.WebApp.initDataUnsafe.user.id;
        },
    }
);