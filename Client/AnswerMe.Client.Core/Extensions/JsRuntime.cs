using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;
using Microsoft.JSInterop;
using Models.Shared.OneOfTypes;

namespace AnswerMe.Client.Core.Extensions;

public static class JsRuntime
{
    public static async Task<string> GenerateVideoBase64Thumbnail(this IJSRuntime jsRuntime, string elementId)
    {
        return await jsRuntime.InvokeAsync<string>("GenerateVideoBase64Thumbnail", elementId);
    }

    public static async Task CopyToClipboardAsync(this IJSRuntime jsRuntime, string text)
    {
        await jsRuntime.InvokeVoidAsync("CopyTextToClipboard", text);
    }
    
    public static async Task<string> GenerateImageBase64(this IJSRuntime jsRuntime, string elementId, int filePosition=0)
    {
        return await jsRuntime.InvokeAsync<string>("GenerateImageBase64", elementId,filePosition);
    }
    public static async Task ClickAsync(this IJSRuntime jsRuntime, string elementId)
    {
        await jsRuntime.InvokeVoidAsync("Click", elementId);
    }

    public static async Task<string> GetInputFileType(this IJSRuntime jsRuntime, string elementId)
    {
        return await jsRuntime.InvokeAsync<string>("GetInputFileType", elementId);
    }

    public static async Task ErrorListAsync(this IJSRuntime jsRuntime, List<ValidationFailed> validationFailedList)
    {
        await jsRuntime.InvokeVoidAsync("MessageShowList", validationFailedList);
    }

    public static async Task ShowErrorAsync(this IJSRuntime jsRuntime, string error)
    {
        await jsRuntime.InvokeVoidAsync("MessageShow", "error", "error", error);
    }

    public static async Task ShowSuccessAsync(this IJSRuntime jsRuntime, string successMessage)
    {
        await jsRuntime.InvokeVoidAsync("MessageShow", "success", "success", successMessage);
    }

    public static async Task AddClassAsync(this IJSRuntime jsRuntime, string elementId, string className)
    {
        await jsRuntime.InvokeVoidAsync("AddClass", elementId, className);
    }

    public static async Task ReplaceClassAsync(this IJSRuntime jsRuntime, string elementId, string oldClass,
        string newClass)
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

    public static async Task ScrollToEndAsync(this IJSRuntime jsRuntime, string elementId)
    {
        await jsRuntime.InvokeVoidAsync("ScrollToEnd", elementId);
    }


    public static async Task SubmitFormAsync(this IJSRuntime jsRuntime, string elementId)
    {
        await jsRuntime.InvokeVoidAsync("SubmitForm", elementId);
    }
}