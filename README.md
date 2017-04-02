# WP7Clipboard

**Project Description**

WP7Clipboard is a library to mimic a shared clipboard on Windows Phone 7. This enables the user to copy objects from one application and post them into another without having to use a webservice/etc.

It allows you to use code like this:

```csharp
Clipboard.SetText("some text I want accessible in another app!");

if (Clipboard.ContainsText())
{
    string myVar = Clipboard.GetText();
}
```

Please use the `ProxyClipboard` class to minimize disk usage. See the demo app for an example of its usage.

*This is still Beta* - Please note that the _Image_ support is still under development/testing but _Text_ related functionality should work fine.

This code was originally ported from the app [ScratchPad](http://blog.mrlacey.co.uk/2010/12/cut-and-paste-with-current-wp7dev-tools.html) by [Matt Lacey](http://blog.mrlacey.co.uk/2011/03/wp7clipboard-clipboard-api-for-wp7dev.html). You can get it in the [Marketplace](http://social.zune.net/redirect?type=phoneApp&id=c225d285-9304-e011-9264-00237de2db9e) or watch a video of it at [YouTube](http://www.youtube.com/watch?v=zx7tDgW5c-k&feature=player_embedded).
