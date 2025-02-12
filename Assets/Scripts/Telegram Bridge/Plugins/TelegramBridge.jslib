mergeInto(LibraryManager.library,
    { 
        ShowBrowserAlertJs: function (message)
        {
            window.alert(UTF8ToString(message));
        },

        ShowTelegramAlertJs: function (message)
        {
            window.Telegram.WebApp.showAlert(UTF8ToString(message));
        },

        SendDataToBotJs: function (message)
        {
            window.Telegram.WebApp.sendData(UTF8ToString(message));
        },

        OpenInvoiceJs: function (url, index)
        {
            window.Telegram.WebApp.openInvoice(UTF8ToString(url), (status) => 
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