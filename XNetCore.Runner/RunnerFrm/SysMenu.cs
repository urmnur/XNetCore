using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    class SysMenu
    {
        internal enum ItemFlags
        {
            // The item ... 
            mfUnchecked = 0x00000000, // ... is not checked 
            mfString = 0x00000000, // ... contains a string as label 
            mfDisabled = 0x00000002, // ... is disabled 
            mfGrayed = 0x00000001, // ... is grayed 
            mfChecked = 0x00000008, // ... is checked 
            mfPopup = 0x00000010, // ... Is a popup menu. Pass the 
                                  // menu handle of the popup 
                                  // menu into the ID parameter. 
            mfBarBreak = 0x00000020, // ... is a bar break 
            mfBreak = 0x00000040, // ... is a break 
            mfByPosition = 0x00000400, // ... is identified by the position 
            mfByCommand = 0x00000000, // ... is identified by its ID 
            mfSeparator = 0x00000800 // ... is a seperator (String and 
                                     // ID parameters are ignored). 
        }
        internal enum WindowMessages
        {
            wmSysCommand = 0x0112
        }
        // 提示：C＃把函数声明为外部的，而且使用属性DllImport来指定DLL 
        //和任何其他可能需要的参数。 
        // 首先，我们需要GetSystemMenu() 函数 
        // 注意这个函数没有Unicode 版本 
        [DllImport("USER32", EntryPoint = "GetSystemMenu", SetLastError = true,
        CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr apiGetSystemMenu(IntPtr WindowHandle, int bReset);
        // 还需要AppendMenu()。 既然 .NET 使用Unicode, 
        // 我们应该选取它的Unicode版本。 
        [DllImport("USER32", EntryPoint = "AppendMenuW", SetLastError = true,
        CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.Winapi)]
        private static extern int apiAppendMenu(IntPtr MenuHandle, int Flags, int NewID, String Item);
        //还可能需要InsertMenu() 
        [DllImport("USER32", EntryPoint = "InsertMenuW", SetLastError = true,
        CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.Winapi)]
        private static extern int apiInsertMenu(IntPtr hMenu, int Position, int Flags, int NewId, String Item);
        private IntPtr m_SysMenu = IntPtr.Zero; // 系统菜单句柄 

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int apiSendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);


        private SysMenu()
        { }

        // 在给定的位置（以0为索引开始值）插入一个分隔条 
        public bool InsertSeparator(int Pos)
        {
            return (InsertMenu(Pos, ItemFlags.mfSeparator | ItemFlags.mfByPosition, 0, ""));
        }
        // 简化的InsertMenu()，前提――Pos参数是一个0开头的相对索引位置 
        public bool InsertMenu(int Pos, int ID, String Item)
        {
            return (InsertMenu(Pos, ItemFlags.mfByPosition | ItemFlags.mfString, ID, Item));
        }
        // 在给定位置插入一个菜单项。具体插入的位置取决于Flags 
        public bool InsertMenu(int Pos, ItemFlags Flags, int ID, String Item)
        {
            return (apiInsertMenu(m_SysMenu, Pos, (Int32)Flags, ID, Item) == 0);
        }
        // 添加一个分隔条 
        public bool AppendSeparator()
        {
            return AppendMenu(0, "", ItemFlags.mfSeparator);
        }
        // 使用ItemFlags.mfString 作为缺省值 
        public bool AppendMenu(int ID, String Item)
        {
            return AppendMenu(ID, Item, ItemFlags.mfString);
        }
        // 被取代的函数 
        public bool AppendMenu(int ID, String Item, ItemFlags Flags)
        {
            return (apiAppendMenu(m_SysMenu, (int)Flags, ID, Item) == 0);
        }
        // 检查是否一个给定的ID在系统菜单ID范围之内 
        public static bool VerifyItemID(int ID)
        {
            return (bool)(ID < 0xF000 && ID > 0);
        }

        //从一个Form对象检索一个新对象 
        public static SysMenu FromForm(Form frm)
        {
            var cSysMenu = new SysMenu();
            cSysMenu.m_SysMenu = apiGetSystemMenu(frm.Handle, 0);
            if (cSysMenu.m_SysMenu == IntPtr.Zero)
            {
                // 一旦失败，引发一个异常 
                throw new Exception("没有找到系统菜单");
            }
            return cSysMenu;
        }
        //当前窗口菜单还原 
        public static void ResetSystemMenu(Form Frm)
        {
            apiGetSystemMenu(Frm.Handle, 1);
        }

        public static int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam)
        {
            return apiSendMessage(hwnd, wMsg, wParam, lParam);
        }
    }

    class MessageFilter : IMessageFilter
    {
        const int WM_SYSCOMMAND = 0x112;
        const int TECHSTAR_MSG = 13015;
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != WM_SYSCOMMAND)
            {
                return false;
            }
            if (m.WParam.ToInt32() == TECHSTAR_MSG + 0)
            {
                showSysLogFrm();
            }
            return false;
        }


        public void Run(Form frm)
        {
            var hSysMenu = SysMenu.FromForm(frm);
            hSysMenu.AppendSeparator();
            hSysMenu.AppendMenu(TECHSTAR_MSG + 0, "系统日志");
        }



        private void showSysLogFrm()
        {
            if (SysLogFrm.Instance == null)
            {
                return;
            }
            if (SysLogFrm.Instance.InvokeRequired)
            {
                SysLogFrm.Instance.BeginInvoke(new Action(() => { showSysLogFrm(); }));
                return;
            }
            SysLogFrm.Instance.ShowDialog();
        }

    }
}



