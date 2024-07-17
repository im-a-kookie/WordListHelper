using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WordListBuilder.WordListControl.Hooks
{
    /// <summary>
    /// Provides a global mouse interface that works irregardles of
    /// the DragDrop event capturing input from controls.
    /// <para>Uses Win32 Api Input hooks and therefore is
    /// only available on Windows platforms.</para>
    /// <para>Many definitions cited from <see href="https://www.pinvoke.net/default.aspx"></see></para>
    /// </summary>
    internal class MouseHooks
    {

        /// <summary>
        /// MSLL Hook Structure for mouse hooks/>
        /// </summary>

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public nuint dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }


        /// <summary>
        /// Callback function for capturing mouse hooks in SetWindowsHookEx
        /// </summary>
        public delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

        internal static class NativeMethods
        {
            /// <summary>
            /// Sets a low level windows hook for the given hook type
            /// </summary>
            /// <param name="hookType">The type of the hook</param>
            /// <param name="callback">The callback</param>
            /// <param name="moduleHandle">The handle of the module to use</param>
            /// <param name="dwThreadId">Thread ID (not important)</param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(HookType hookType, MouseHookHandler callback, IntPtr moduleHandle, uint dwThreadId);

            /// <summary>
            /// Unhooks the hook. Use the pointer returned from <see cref="SetWindowsHookEx(HookType, MouseHookHandler, nint, uint)".
            /// </summary>
            /// <param name="hhk"></param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            /// <summary>
            /// Hands the input hook along the input chain for the next handler or window.
            /// 
            /// <para></para>The result of this function should be returned by a hook for it to remain uninvasive.
            /// </summary>
            /// <param name="hookHandle"></param>
            /// <param name="messageCode"></param>
            /// <param name="wParam"></param>
            /// <param name="lParam"></param>
            /// <returns></returns>

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hookHandle, int messageCode, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// Gets the handle for a module, needed for setting the hook callback stuff
            /// </summary>
            /// <param name="lpModuleName"></param>
            /// <returns></returns>

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);


            /// <summary>
            /// Gets the window from the given point
            /// </summary>
            /// <param name="Point"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            public static extern IntPtr WindowFromPoint(POINT Point);
        }


        /// <summary>
        /// Creates a mouse hook for the control provided to the constructor
        /// </summary>
        public class MouseHook
        {
            /// <summary>
            /// A list of controls that are currently being hooked
            /// </summary>
            Dictionary<IntPtr, List<MouseWheelCallback>> controls = [];
            /// <summary>
            /// The hook pointer from <see cref="NativeMethods.SetWindowsHookEx(HookType, MouseHookHandler, nint, uint)"/>
            /// </summary>
            IntPtr hook = IntPtr.Zero;
            /// <summary>
            /// Confine the hook behaviours to an instantiated object. Not super
            /// meaningful but it lets us catch deconstructors
            /// </summary>
            static MouseHook hookHandler = new MouseHook();

            /// <summary>
            /// Applies the mouse wheel hook to the given control.
            /// </summary>
            /// <param name="c">The control to be hooked</param>
            /// <param name="ma">The callback to use when the event triggers</param>
            public static void ApplyMouseWheelHook(Control c, MouseWheelCallback ma)
            {
                //Delegate to a class for mutexing purposes
                lock(hookHandler)
                {
                    List<MouseWheelCallback> l;
                    if(!hookHandler.controls.TryGetValue(c.Handle, out l))
                    {
                        l = new();
                        hookHandler.controls.Add(c.Handle, l);
                    }
                    //now register the callback
                    if (!l.Contains(ma)) l.Add(ma);
                    
                    //If we need to set the hook, then set the hook
                    if (hookHandler.hook == IntPtr.Zero)
                    {
                        using (ProcessModule module = Process.GetCurrentProcess().MainModule!)
                            hookHandler.hook = NativeMethods.SetWindowsHookEx(HookType.WH_MOUSE_LL, MouseEvent, NativeMethods.GetModuleHandle(module.ModuleName), 0);
                    }

                    //and make sure to clear it out when the control dies
                    c.Disposed += (a, b) =>
                    {
                        lock(hookHandler)
                        {
                            //make sure the sender is actually a control (does the handle survive?)
                            if (a is Control c)
                            {
                                hookHandler.controls.Remove(c.Handle);
                                if (hookHandler.controls.Count == 0)
                                {
                                    NativeMethods.UnhookWindowsHookEx(hookHandler.hook);
                                    hookHandler.hook = IntPtr.Zero;
                                }
                            }
                        }
                    };
                }
            }

            /// <summary>
            /// Provides a callback for the mouse events
            /// </summary>
            /// <param name="code">Not important</param>
            /// <param name="wParam"></param>
            /// <param name="lParam"></param>
            /// <returns></returns>
            public static IntPtr MouseEvent(int code, IntPtr wParam, IntPtr lParam)
            {
                //we only care for mouse wheel in this scenario
                if((MouseMessages)wParam != MouseMessages.WM_MOUSEWHEEL)
                    return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

                //get the structure with the event information
                MSLLHOOKSTRUCT flags = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                int delta = (flags.mouseData / (1 << 16));

                //now see if the mouse has been placed over a given control
                IntPtr belowMouse = NativeMethods.WindowFromPoint(flags.pt);
                if(hookHandler.controls.TryGetValue(belowMouse, out var l))
                {
                    //the mouse is correctly placed, so we can pass the event to the control
                    foreach (var x in l) x.Invoke(delta);
                }
                //Call the next hook
                return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            }

            /// <summary>
            /// Make sure to release the hook when the hook object is deconstructed
            /// </summary>
            ~MouseHook()
            {
                if(hook != IntPtr.Zero)
                {
                    NativeMethods.UnhookWindowsHookEx(hook);
                }
            }

            public delegate void MouseWheelCallback(int delta);

        }

    }
}
