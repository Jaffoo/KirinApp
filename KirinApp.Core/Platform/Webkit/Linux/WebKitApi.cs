﻿using System.Linq.Expressions;
using KirinAppCore.Interface;
using KirinAppCore.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using KirinAppCore.Platform.Linux;

namespace KirinAppCore.Plateform.Linux;

internal class WebKit(string libName) : IWebKit
{
    private IntPtr LibPtr
    {
        get
        {
            IntPtr handle = dlopen(libName, 1);
            if (handle == IntPtr.Zero)
            {
                throw new Exception($"Failed to load library that is '{libName}'.");
            }

            return handle;
        }
    }

    #region api

    [DllImport("libdl.so.2", SetLastError = true)]
    private static extern IntPtr dlopen(string filename, int flags);

    [DllImport("libdl.so.2", SetLastError = true)]
    private static extern IntPtr dlsym(IntPtr handle, string symbol);

    protected void LoadFunction<TDelegate>(string functionName, params object[] parameters)
    {
        IntPtr funcAddress = dlsym(LibPtr, functionName);
        if (funcAddress == IntPtr.Zero)
        {
            throw new Exception($"Failed to get function address for {functionName}");
        }

        // 将函数指针转换为委托
        var functionDelegate = Marshal.GetDelegateForFunctionPointer<TDelegate>(funcAddress);

        // 调用函数并返回结果
        ((Delegate)(object)functionDelegate!).DynamicInvoke(parameters);
    }

    protected TResult LoadFunction<TResult, TDelegate>(string functionName, params object[] parameters)
    {
        IntPtr funcAddress = dlsym(LibPtr, functionName);
        if (funcAddress == IntPtr.Zero)
        {
            throw new Exception($"Failed to get function address for {functionName}");
        }

        // 将函数指针转换为委托
        var functionDelegate = Marshal.GetDelegateForFunctionPointer<TDelegate>(funcAddress);

        // 调用函数并返回结果
        return (TResult)((Delegate)(object)functionDelegate!).DynamicInvoke(parameters)!;
    }

    private delegate void WebViewLoadUri(IntPtr webView, string uri);
    protected virtual void webkit_web_view_load_uri(IntPtr webView, string uri)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        LoadFunction<WebViewLoadUri>(methodName, webView, uri);
    }

    private delegate IntPtr UserScript(string source, int injectionTime, int injectionFlags, IntPtr whitelist, IntPtr blacklist);
    protected virtual IntPtr webkit_user_script_new(string source, int injectionTime, int injectionFlags, IntPtr whitelist, IntPtr blacklist)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, UserScript>(methodName, source, injectionTime, injectionFlags, whitelist, blacklist);
    }

    private delegate IntPtr UserContentManager();

    protected virtual IntPtr webkit_user_content_manager_new()
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, UserContentManager>(methodName);
    }

    private delegate IntPtr WebViewWithUserContentManager(IntPtr manager);
    protected virtual IntPtr webkit_web_view_new_with_user_content_manager(IntPtr manager)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, WebViewWithUserContentManager>(methodName, manager);
    }

    private delegate void UserContentManagerAddScript(IntPtr manager, IntPtr script);
    protected virtual void webkit_user_content_manager_add_script(IntPtr manager, IntPtr script)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        LoadFunction<UserContentManagerAddScript>(methodName, manager, script);
    }

    private delegate void UserScriptUnref(IntPtr script);
    protected virtual void webkit_user_script_unref(IntPtr script)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        LoadFunction<UserScriptUnref>(methodName, script);
    }

    private delegate void SettingSetEnableDev(IntPtr settings, bool enable);
    protected virtual void webkit_settings_set_enable_developer_extras(IntPtr settings, bool enable)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        LoadFunction<SettingSetEnableDev>(methodName, settings, enable);
    }

    private delegate IntPtr WebViewGetSetting(IntPtr webView);
    protected virtual IntPtr webkit_web_view_get_settings(IntPtr webView)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, WebViewGetSetting>(methodName, webView);
    }

    private delegate IntPtr WebContentGetDefault();
    protected virtual IntPtr webkit_web_context_get_default()
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, WebContentGetDefault>(methodName);
    }

    private delegate void WebContentRegisterUriScheme(IntPtr context, string scheme, IntPtr callback, IntPtr user_data, IntPtr destroy_notify);
    protected virtual void webkit_web_context_register_uri_scheme(IntPtr context, string scheme, IntPtr callback, IntPtr user_data, IntPtr destroy_notify)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        LoadFunction<WebContentRegisterUriScheme>(methodName, context, scheme, callback, user_data, destroy_notify);
    }

    private delegate IntPtr UriSchemeRequestGetUri(IntPtr request);
    protected virtual IntPtr webkit_uri_scheme_request_get_uri(IntPtr request)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, UriSchemeRequestGetUri>(methodName, request);
    }

    private delegate void UriSchemeRequestFinish(IntPtr request, IntPtr stream, int length, IntPtr mimeType);
    protected virtual void webkit_uri_scheme_request_finish(IntPtr request, IntPtr stream, int length, IntPtr mimeType)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        LoadFunction<UriSchemeRequestFinish>(methodName, request, stream, length, mimeType);
    }

    private delegate void UserContentManagerRegisterScriptMessageHandler(IntPtr manager, string name);
    protected virtual void webkit_user_content_manager_register_script_message_handler(IntPtr manager, string name)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        LoadFunction<UserContentManagerRegisterScriptMessageHandler>(methodName, manager, name);
    }

    private delegate IntPtr JsResultGetJsValue(IntPtr result);
    protected virtual IntPtr webkit_javascript_result_get_js_value(IntPtr result)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, JsResultGetJsValue>(methodName, result);
    }

    private delegate IntPtr JsValueToString(IntPtr value);
    protected virtual IntPtr jsc_value_to_string(IntPtr value)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, JsValueToString>(methodName, value);
    }

    private delegate bool JsValueIsString(IntPtr value);
    protected virtual bool jsc_value_is_string(IntPtr value)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<bool, JsValueIsString>(methodName, value);
    }

    private delegate IntPtr WebViewRunJs(IntPtr webView, string script, IntPtr cancellable, IntPtr callback, IntPtr userData);
    protected virtual IntPtr webkit_web_view_run_javascript(IntPtr webView, string script, IntPtr cancellable, IntPtr callback, IntPtr userData)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        return LoadFunction<IntPtr, WebViewRunJs>(methodName, webView, script, cancellable, callback, userData);
    }

    private delegate void ContextMenuRemoveAll(IntPtr menu);
    protected virtual void webkit_context_menu_remove_all(IntPtr menu)
    {
        MethodBase method = MethodBase.GetCurrentMethod()!;
        string methodName = method.Name;
        LoadFunction<ContextMenuRemoveAll>(methodName, menu);
    }

    #endregion

    protected IntPtr Handle { get; set; }
    protected WebManager? WebManager { get; set; }
    protected SchemeConfig? SchemeConfig { get; set; }
    protected IWindow? Window { get; set; }
    protected WinConfig Config => Window!.Config;
    public event EventHandler<WebMessageEvent>? WebMessageReceived;

    protected delegate void ContextMenuCallbackDelegate(IntPtr webView, IntPtr menu, IntPtr userData);

    protected delegate IntPtr UriSchemeCallbackFunc(IntPtr request, IntPtr user_data);

    protected UriSchemeCallbackFunc uriSchemeCallback = new((_, _) => 0);

    protected delegate void ScriptMessageReceivedDelegate(IntPtr webView, IntPtr message, IntPtr userData);

    public virtual void InitWebControl(IWindow? window = null)
    {
        try
        {
            if (window != null)
                Window = window;

            var contentManager = webkit_user_content_manager_new();
            Handle = webkit_web_view_new_with_user_content_manager(contentManager);
            GtkApi.gtk_container_add(Window!.Handle, Handle);

            if (Config.Debug)
            {
                IntPtr settings = webkit_web_view_get_settings(Handle);
                webkit_settings_set_enable_developer_extras(settings, true);
                webkit_settings_set_enable_developer_extras(settings, true);
            }
            else
            {
                void ContextMenuCallback(IntPtr webView, IntPtr menu, IntPtr userData) =>
                    webkit_context_menu_remove_all(menu);

                IntPtr settings = webkit_web_view_get_settings(Handle);
                webkit_settings_set_enable_developer_extras(settings, false);
                GtkApi.g_signal_connect_data(Handle, "context-menu",
                    Marshal.GetFunctionPointerForDelegate(new ContextMenuCallbackDelegate(ContextMenuCallback)),
                    IntPtr.Zero, IntPtr.Zero, 0);
            }

            if (Config.AppType != WebAppType.Http)
            {
                var url = $"http://localhost/";
                if (Config.AppType == WebAppType.Static) url += Config.Url;
                if (Config.AppType == WebAppType.Blazor) url += "blazorindex.html";
                SchemeConfig = new Uri(url).ParseScheme();
                var dispatcher = new WebDispatcher(window!);
                WebManager = new WebManager(window!, dispatcher,
                    window!.ServiceProvide!.GetRequiredService<JSComponentConfigurationStore>(), SchemeConfig);
                if (Config.AppType == WebAppType.Blazor)
                {
                    if (Config.BlazorComponent == null) throw new Exception("Blazor component not found!");
                    _ = dispatcher.InvokeAsync(async () =>
                    {
                        await WebManager.AddRootComponentAsync(Config.BlazorComponent!, Config.BlazorSelector,
                            ParameterView.Empty);
                    });
                }

                uriSchemeCallback = UriSchemeCallback;
                var context = webkit_web_context_get_default();
                webkit_web_context_register_uri_scheme(context, SchemeConfig.AppScheme,
                    Marshal.GetFunctionPointerForDelegate(uriSchemeCallback), IntPtr.Zero, IntPtr.Zero);

                var assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("KirinAppCore.wwwroot.webkit.document.js")!;
                var content = new StreamReader(stream).ReadToEnd();
                var script = webkit_user_script_new(content, 2, 0, IntPtr.Zero, IntPtr.Zero);
                webkit_user_content_manager_add_script(contentManager, script);
                webkit_user_script_unref(script);

                GtkApi.g_signal_connect_data(contentManager,
                    "script-message-received::KirinApp",
                    Marshal.GetFunctionPointerForDelegate(new ScriptMessageReceivedDelegate(ScriptMessageReceived)),
                    IntPtr.Zero, IntPtr.Zero, 0);
                webkit_user_content_manager_register_script_message_handler(contentManager, "KirinApp");
                WebManager.Navigate("/");
            }
            else
            {
                webkit_web_view_load_uri(Handle, Config.Url!);
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual string ExecuteJavaScript(string js)
    {
        var jsHandle = webkit_web_view_run_javascript(Handle, js, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        if (jsHandle == IntPtr.Zero) return "";
        var res = webkit_javascript_result_get_js_value(jsHandle);
        if (!jsc_value_is_string(res)) return "";
        var jsString = jsc_value_to_string(res);
        var result = Marshal.PtrToStringAnsi(jsString);
        return result ?? "";
    }

    public virtual void InjectJsObject(string name, object obj)
    {
        var js = $"window.external.{name}={JsonConvert.SerializeObject(obj)}";
        webkit_web_view_run_javascript(Handle, js, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }

    public virtual void OpenDevTool()
    {
        return;
    }

    public virtual void Reload()
    {
        if (Config.AppType != WebAppType.Http) WebManager!.Navigate("/");
        else webkit_web_view_load_uri(Handle, Config.Url!);
    }

    public virtual void Navigate(string url)
    {
        webkit_web_view_load_uri(Handle, url);
    }

    public virtual void SendWebMessage(string message)
    {
        var js = new StringBuilder();
        js.Append("window.__dispatchMessageCallback(\"");
        js.Append(message.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\""));
        js.Append("\")");
        webkit_web_view_run_javascript(Handle, js.ToString(), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }

    public virtual IntPtr UriSchemeCallback(IntPtr request, IntPtr user_data)
    {
        try
        {
            var contentType = "text/html; charset=UTF-8";
            byte[] byteArray;
            if (Window!.Config.AppType == WebAppType.RawString)
            {
                string pattern = @"<(\w+)([^>]*?)>(.*?)<\/\1>|<(\w+)([^>]*?)/>";
                if (!Regex.IsMatch(Window!.Config.RawString ?? "", pattern, RegexOptions.Singleline))
                    contentType = "text/plain; charset=UTF-8";
                byteArray = Encoding.UTF8.GetBytes(Window!.Config.RawString ?? "");
            }
            else
            {
                MemoryStream ms = new();
                var uri = Marshal.PtrToStringAnsi(webkit_uri_scheme_request_get_uri(request));
                var response = WebManager!.OnResourceRequested(SchemeConfig!, uri!);
                response.Content.CopyTo(ms);
                byteArray = ms.ToArray();
                contentType = response.Type;
            }

            IntPtr bytesPtr = Marshal.AllocHGlobal(byteArray.Length);
            Marshal.Copy(byteArray, 0, bytesPtr, byteArray.Length);
            var stream = GtkApi.g_memory_input_stream_new_from_data(bytesPtr, byteArray.Length, IntPtr.Zero);
            webkit_uri_scheme_request_finish(request, stream, byteArray.Length,
                Marshal.StringToCoTaskMemAnsi(contentType));
            return IntPtr.Zero;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual void ScriptMessageReceived(IntPtr webView, IntPtr message, IntPtr userData)
    {
        IntPtr jsResult = webkit_javascript_result_get_js_value(message);
        if (jsc_value_is_string(jsResult))
        {
            IntPtr jsString = jsc_value_to_string(jsResult);
            string messageString = Marshal.PtrToStringAnsi(jsString)!;
            WebManager!.OnMessageReceived(SchemeConfig!.AppOrigin, messageString);
            try
            {
                var jobject = JObject.Parse(messageString);
                if (jobject.ContainsKey("cmd"))
                {
                    var cmd = jobject["cmd"]!.ToString();
                    switch (cmd)
                    {
                        case "unMax": Window!.Maximize(false); break;
                        case "max": Window!.Maximize(); break;
                        case "min": Window!.Minimize(); break;
                        case "unMin": Window!.Minimize(false); break;
                        case "hide": Window!.Hide(); break;
                        case "show": Window!.Show(); break;
                        case "focus": Window!.Focus(); break;
                        case "close": Window!.Close(); break;
                        case "change":
                            Window!.Change(jobject["data"]!["width"]!.ToString().ToInt(),
                                jobject["data"]!["height"]!.ToString().ToInt()); break;
                        case "normal": Window!.Normal(); break;
                        case "move":
                            Window!.Move(jobject["data"]!["x"]!.ToString().ToInt(),
                                jobject["data"]!["y"]!.ToString().ToInt()); break;
                        case "moveTo":
                            Window!.MoveTo(jobject["data"]!["x"]!.ToString().ToInt(),
                                jobject["data"]!["y"]!.ToString().ToInt()); break;
                        default:
                            break;
                    }
                }

                return;
            }
            catch
            {
            }

            WebMessageEvent msg = new WebMessageEvent()
            {
                Message = messageString
            };
            WebMessageReceived?.Invoke(this, msg);
        }
    }
}

internal class WebKit40() : WebKit("libwebkit2gtk-4.0.so.37")
{
}

internal class WebKit41() : WebKit("libwebkit2gtk-4.1.so.0")
{
}