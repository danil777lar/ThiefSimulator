using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using MoreMountains.Tools;
using UnityEngine;

public class TelegramBridge : MonoBehaviour
{
    private static TelegramBridge _instance;
    public static TelegramBridge Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TelegramBridge>();
                if (_instance == null)
                {
                    _instance = new GameObject("TelegramBridge").AddComponent<TelegramBridge>();
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }
    
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void ShowBrowserAlertJs(string message);
    [DllImport("__Internal")] private static extern void ShowTelegramAlertJs(string message);
    [DllImport("__Internal")] private static extern void SendDataToBotJs(string message);
    [DllImport("__Internal")] private static extern void OpenInvoiceJs(string url, int index);
    [DllImport("__Internal")] private static extern int GetTelegramUserIdJs();
    #endif
    
    private List<Action<string>> _callbacks = new List<Action<string>>();
    
    
    public void ShowBrowserAlert(string message)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowBrowserAlertJs(message);
#else
        Debug.Log($"TGBridge: Browser alert: {message}");
#endif
    }
    
    public void ShowTelegramAlert(string message)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowTelegramAlertJs(message);
#else
        Debug.Log($"TGBridge: Telegram alert: {message}");
#endif
    }

    public void SendDataToBot(string message)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SendDataToBotJs(message);
#else
        Debug.Log($"TGBridge: Send data to bot: {message}");
#endif
    }
    
    public void OpenInvoice(string url, Action<string> callback)
    {
        _callbacks.Add(callback);
        int index = _callbacks.Count - 1;
        
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenInvoiceJs(url, index);
#else
        Debug.Log($"TGBridge: Open invoice: {url}");
#endif
    }
    
    public int GetUserId()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return GetTelegramUserIdJs();
#else
        return -1;
#endif
    }

    private string _debug = "";
    private void Update()
    {
        MMDebug.DebugOnScreen(_debug);
    }
    
    public void CatchInvoiceCallback(string data)
    {
        _debug = $"Invoice callback: {data}";
        
        string[] splitted = data.Split("/");
        int index = int.Parse(splitted[0]);
        string callbackResult = splitted[1];
        
        _callbacks[index]?.Invoke(callbackResult);
    }
}
