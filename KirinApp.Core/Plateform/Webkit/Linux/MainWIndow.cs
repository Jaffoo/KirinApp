﻿using KirinAppCore.Model;
using KirinAppCore.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components.WebView;
using Newtonsoft.Json.Linq;
using KirinAppCore.Plateform.Webkit.Linux.Models;

namespace KirinAppCore.Plateform.Webkit.Linux;

/// <summary>
/// Windows实现类
/// </summary>
internal class MainWIndow : IWindow
{
    private WebManager? WebManager { get; set; }
    private SchemeConfig? SchemeConfig { get; set; }
    private const uint GDK_EVENT_CONFIGURE = 0x2000; // Configure event

    #region 事件
    public override event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;
    public override event EventHandler<EventArgs>? OnCreate;
    public override event EventHandler<EventArgs>? Created;
    public override event EventHandler<EventArgs>? OnLoad;
    public override event EventHandler<EventArgs>? Loaded;
    public override event EventHandler<SizeChangeEventArgs>? SizeChangeEvent;
    #endregion

    #region 窗体方法
    protected override void Create()
    {
        OnCreate?.Invoke(this, new());
        GtkApi.XInitThreads();
        IntPtr argv = IntPtr.Zero;
        int argc = 0;
        GtkApi.gtk_init(ref argc, ref argv);

        Handle = GtkApi.gtk_window_new(Config.Chromeless ? 1 : 0);
        GtkApi.gtk_window_set_title(Handle, Config.AppName);
        GtkApi.gtk_window_set_resizable(Handle, Config.ResizeAble);

        if (!string.IsNullOrWhiteSpace(Config.Icon))
        {
            var icon = new Icon(Config.Icon);
            if (icon != null)
                GtkApi.gtk_window_set_icon(Handle, GtkApi.gdk_pixbuf_scale_simple(icon.Handle, 64, 64, GdkInterpType.GDK_INTERP_BILINEAR));
        }
        if (Config.Chromeless) GtkApi.gtk_window_set_decorated(Handle, true);

        if (Config.Size != null)
        {
            Config.Width = Config.Size.Value.Width;
            Config.Height = Config.Size.Value.Height;
        }
        if (Config.Center)
        {
            Config.Left = (MainMonitor!.Width - Config.Width) / 2;
            Config.Top = (MainMonitor!.Height - Config.Height) / 2;
        }
        GtkApi.gtk_window_set_default_size(Handle, Config.Width, Config.Height);
        var color = ColorTranslator.FromHtml("#FFFFFF");
        var rgb = new GdkRGBA()
        {
            Red = color.R,
            Green = color.G,
            Blue = color.B,
            Alpha = color.A
        };
        IntPtr result = Marshal.AllocHGlobal(Marshal.SizeOf(rgb));
        Marshal.StructureToPtr(rgb, result, false);
        GtkApi.gtk_widget_override_background_color(Handle, 0, result);
        if (Config.Center)
            GtkApi.gtk_window_set_position(Handle, (int)GtkWindowPosition.GtkWinPosCenter);
        else
            GtkApi.gtk_window_move(Handle, Config.Left, Config.Top);
        if (Config.MinimumSize != null)
        {
            Config.MinimumHeigh = Config.MinimumSize.Value.Height;
            Config.MinimumWidth = Config.MinimumSize.Value.Width;
        }
        if (Config.MaximumSize != null)
        {
            Config.MaximumHeigh = Config.MaximumSize.Value.Height;
            Config.MaximumWidth = Config.MaximumSize.Value.Width;
        }
        var geometry = new GeometryInfo()
        {
            MinWidth = Config.MinimumWidth,
            MinHeight = Config.MinimumHeigh,
            MaxWidth = Config.MaximumWidth,
            MaxHeight = Config.MaximumHeigh
        };
        GtkApi.gtk_window_set_geometry_hints(Handle, IntPtr.Zero, ref geometry, GdkWindowHints.GDK_HINT_MIN_SIZE | GdkWindowHints.GDK_HINT_MAX_SIZE);

        GtkApi.gtk_widget_add_events(Handle, GDK_EVENT_CONFIGURE);
        GtkApi.g_signal_connect(Handle, "configure-event", Marshal.GetFunctionPointerForDelegate(new Action<IntPtr, IntPtr>(OnWindowConfigure)), IntPtr.Zero);

        Created?.Invoke(this, new());
    }
    private void OnWindowConfigure(IntPtr widget, IntPtr eventPtr)
    {
        int width, height;
        GtkApi.gtk_window_get_size(widget, out width, out height);
        SizeChangeEvent?.Invoke(widget, new SizeChangeEventArgs() { Width = width, Height = height });
    }
    public override void Show()
    {
        GtkApi.gtk_widget_show_all(Handle);
        State = WindowState.Normal;
    }

    public override void Hide()
    {
        GtkApi.gtk_widget_hide_all(Handle);
        State = WindowState.Hide;
    }

    public override void Focus() => GtkApi.gtk_window_present(Handle);

    public override void MessageLoop() => GtkApi.gtk_main();

    public override Rect GetClientSize()
    {
        Rect rect = new();
        GtkApi.gtk_window_get_position(Handle, out rect.Left, out rect.Top);
        int width, height;
        GtkApi.gtk_window_get_size(Handle, out width, out height);
        rect.Width = width;
        rect.Height = height;
        rect.Bottom = MainMonitor.Height - rect.Top - height;
        rect.Right = MainMonitor.Width - rect.Left - width;
        return rect;
    }

    public override void Maximize()
    {
        GtkApi.gtk_window_maximize(Handle);
        State = WindowState.Maximize;
    }

    public override void Minimize()
    {
        GtkApi.gtk_window_iconify(Handle);
        State = WindowState.Minimize;
    }

    public override void SizeChange(IntPtr handle, int width, int height)
    {

    }

    private void CheckInitialDir(ref string initialDir)
    {

    }

    private void CheckFileFilter(Dictionary<string, string>? dic)
    {

    }

    public override (bool selected, DirectoryInfo? dir) OpenDirectory(string initialDir = "")
    {
        CheckInitialDir(ref initialDir);
        return (false, null);
    }

    public override (bool selected, FileInfo? file) OpenFile(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        CheckInitialDir(ref initialDir);
        CheckFileFilter(fileTypeFilter);
        return (false, null);
    }

    public override (bool selected, List<FileInfo>? files) OpenFiles(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        CheckInitialDir(ref initialDir);
        CheckFileFilter(fileTypeFilter);
        return (false, null);
    }

    public override MsgResult ShowDialog(string title, string msg, MsgBtns btn = MsgBtns.OK)
    {
        return MsgResult.OK;
    }

    /// <summary>
    /// 获取屏幕信息
    /// </summary>
    public override void SetScreenInfo()
    {
        IntPtr display = GtkApi.gdk_display_get_default();
        if (display == IntPtr.Zero) return;
        IntPtr monitor = GtkApi.gdk_display_get_monitor(display, 0);
        GdkRectangle rect;
        GtkApi.gdk_monitor_get_geometry(monitor, out rect);
        MainMonitor = new()
        {
            Height = rect.Height,
            Width = rect.Width,
        };
    }
    #endregion

    #region Webkit方法
    public override bool CheckAccess()
    {
        return Environment.CurrentManagedThreadId == Utils.MainThreadId;
    }

    public override async Task InvokeAsync(Func<Task> workItem)
    {
        if (CheckAccess()) await workItem();
        else
        {
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(workItem);
            Action action = (Action)Marshal.GetDelegateForFunctionPointer(actionPtr, typeof(Action));
            action.Invoke();
        }
    }

    public override void Invoke(Action workItem)
    {
        if (CheckAccess()) workItem();
        else
        {
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(workItem);
            //发送系统消息，在主线程执行
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        }
    }

    protected override async Task InitWebControl()
    {
        try
        {
            OnLoad?.Invoke(this, new());
            await Task.Delay(1);
            Loaded?.Invoke(this, new());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public override void ExecuteJavaScript(string js)
    {
        Task.Run(() =>
        {
            //while (CoreWebCon == null)
            //    Thread.Sleep(10);
            //Thread.Sleep(10);
            //IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            //{
            //    _ = CoreWebCon.CoreWebView2.ExecuteScriptAsync(js).Result;
            //});
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }

    public override string ExecuteJavaScriptWithResult(string js)
    {
        var tcs = new TaskCompletionSource<string>();
        Task.Run(() =>
        {
            //while (CoreWebCon == null)
            //    Task.Delay(10);
            //Task.Delay(10);
            //// 创建指向结果的委托
            //IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(new Action(async () =>
            //{
            //    string res = await CoreWebCon.CoreWebView2.ExecuteScriptAsync(js);
            //    tcs.SetResult(res); // 设置结果
            //}));

            //// 发送消息
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
        // 等待结果
        return tcs.Task.Result; // 返回结果
    }

    public override void OpenDevTool()
    {
        Task.Run(() =>
        {
            //while (CoreWebCon == null)
            //    Thread.Sleep(10);
            //Thread.Sleep(10);
            //IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() => CoreWebCon!.CoreWebView2.OpenDevToolsWindow());
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }

    public override void SendWebMessage(string message)
    {

    }

    public override void Reload()
    {
        Task.Run(() =>
        {
            //IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            //{
            //    ResourceRequest();
            //    CoreWebCon?.CoreWebView2.Reload();
            //});
            //if (Config.AppType == WebAppType.Http)
            //    actionPtr = Marshal.GetFunctionPointerForDelegate(() => CoreWebCon!.CoreWebView2.Navigate(Config.Url));
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }
    #endregion
}