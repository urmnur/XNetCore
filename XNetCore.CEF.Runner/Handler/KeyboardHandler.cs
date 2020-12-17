using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.CEF.Runner
{
    class KeyboardHandler : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            return false;
            try
            {
                if ((int)windowsKeyCode == (int)Keys.F12)
                {
                    new CefApp().ShowDevTools();
                    return true;
                }
                if ((int)windowsKeyCode == (int)Keys.F11)
                {
                    new CefApp().FullScreen();
                    return true;
                }
            }
            catch { };
        }

        

    }
}
