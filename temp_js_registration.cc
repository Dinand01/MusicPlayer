void RegisterJavascriptObject(this AvaloniaCefBrowser browser, object nativeObject, string objectName)
{
    browser.Frame.RegisterJsObject(objectName, nativeObject);
}
