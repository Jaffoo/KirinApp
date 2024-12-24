﻿using System;
using KirinAppCore;
using KirinAppCore.Model;
using KirinAppCore.Test;

class Program
{
    private static ManualResetEvent waitForInput = new ManualResetEvent(false);
    [STAThread]
    static void Main()
    {
        WinConfig winConfig = new WinConfig()
        {
            AppName = "Test",
            Height = 800,
            Width = 1000,
            AppType = WebAppType.Http,
            BlazorComponent = typeof(App),
            Url = "https://ops.zink.asia:28238/websites/runtimes/dotnet",
            RawString = "<span style='color:red'>这个是字符串</span>",
            Icon = "logo.ico",
            Debug = true,
        };
        var kirinApp = new KirinApp(winConfig);
        kirinApp.Loaded += async (_, _) =>
        {
            await Task.Delay(100);
            kirinApp.OpenDevTool();
            kirinApp.SendWebMessage("你好12312");
            await kirinApp.ExecuteJavaScript("console.log('hellow kirinApp')");
            var res = await kirinApp.ExecuteJavaScriptWithResult("1+2");
            Console.WriteLine(res);
        };
        kirinApp.Created += (_, _) =>
        {
            Console.WriteLine(000);
        };
        kirinApp.OnLoad += (_, _) =>
        {
            Console.WriteLine(111);
        };
        kirinApp.OnCreate += (_, _) =>
        {
            Console.WriteLine(222);
        };
        kirinApp.OnClose += (_, _) =>
        {
            Console.WriteLine(222);
            return true;
        };
        kirinApp.PositionChange += (s, e) =>
        {
            Console.WriteLine(e.X + ":" + e.Y);
        };
        kirinApp.WebMessageReceived += (_, e) =>
        {
            Console.WriteLine(e.Message);
        };
        kirinApp.Run();
    }
}
