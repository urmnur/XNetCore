using CefSharp;
using CefSharp.WinForms;

namespace XNetCore.CEF.Runner
{
    internal class ContextMenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            for (int i = 0; i < model.Count; i++)
            {
                var commandId = model.GetCommandIdAt(i);
                if (commandId == CefMenuCommand.Print)
                {
                    model.RemoveAt(i);
                    i--;
                    continue;
                }
                if (commandId == CefMenuCommand.Back)
                {
                    model.RemoveAt(i);
                    i--;
                    continue;
                }
                if (commandId == CefMenuCommand.Forward)
                {
                    model.RemoveAt(i);
                    i--;
                    continue;
                }
                if (commandId == CefMenuCommand.ViewSource)
                {
                    model.RemoveAt(i);
                    i--;
                    continue;
                    model.AddItem((CefMenuCommand)26501, "开发者工具(F12)");
                    model.AddItem((CefMenuCommand)26502, "全屏(F11)");
                    continue;
                }
            }
            while (true)
            {
                if (model.Count > 0 && model.GetCommandIdAt(0) == CefMenuCommand.NotFound)
                {
                    model.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
        }
        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if (commandId == (CefMenuCommand)26501)
            {
                new CefApp().ShowDevTools();
                return true;
            }
            if (commandId == (CefMenuCommand)26502)
            {
                new CefApp().FullScreen();
                return true;
            }
            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }

    }
}