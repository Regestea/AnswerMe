using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;
using Microsoft.JSInterop;

namespace AnswerMe.Client.Core.Extensions;

public static class JsRuntime
{
    public static async Task AddClassAsync(this IJSRuntime jsRuntime, string elementId, string className)
    {
        await jsRuntime.InvokeVoidAsync("AddClass", elementId, className);
    }

    public static async Task ReplaceClassAsync(this IJSRuntime jsRuntime, string elementId, string oldClass, string newClass)
    {
        await jsRuntime.InvokeVoidAsync("ReplaceClass", elementId, oldClass, newClass);
    }

    public static async Task RemoveClassAsync(this IJSRuntime jsRuntime, string elementId, string className)
    {
        await jsRuntime.InvokeVoidAsync("RemoveClass", elementId, className);
    }

    public static async Task SetInnerTextAsync(this IJSRuntime jsRuntime, string elementId, string text)
    {
        await jsRuntime.InvokeVoidAsync("SetInnerText", elementId, text);
    }
    
    public static async Task ScrollToViewAsync(this IJSRuntime jsRuntime, string elementId)
    {
        await jsRuntime.InvokeVoidAsync("ScrollToView", elementId);
    }
}