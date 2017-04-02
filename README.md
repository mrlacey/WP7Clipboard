# WP7Clipboard

*Project Description*
{project:description}

It allows you to use code like this:

{code:c#}
Clipboard.SetText("some text I want accessible in another app!");

if (Clipboard.ContainsText())
{
    string myVar = Clipboard.GetText();
}
{code:c#}

Please use the *ProxyClipboard* class to minimize disk usage. See the demo app for an example of its usage.

*This is still Beta* - Please note that the _Image_ support is still under development/testing but _Text_ related functionality should work fine.

This code was originally ported from the app [url:ScratchPad|http://blog.mrlacey.co.uk/2010/12/cut-and-paste-with-current-wp7dev-tools.html] by [url:Matt Lacey|http://blog.mrlacey.co.uk/2011/03/wp7clipboard-clipboard-api-for-wp7dev.html]. You can get it in the [url:Marketplace|http://social.zune.net/redirect?type=phoneApp&id=c225d285-9304-e011-9264-00237de2db9e] or watch a video of it at [url:YouTube|http://www.youtube.com/watch?v=zx7tDgW5c-k&feature=player_embedded].
