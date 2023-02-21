﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Interop;

namespace PetersAutoClicker
{
   //We figured it out, we can now develop an autoclicker
    public partial class MainWindow : Window
    {
        //i will use the F8 key on my keyboard

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        
        
        //keystate
        private bool hotkeystate = false;

        //main key i want (tested and this works as well)
        private uint mykey = (uint)Keys.F8.GetHashCode();

        // the window source used to setup the global hotkey
        private HwndSource _source;

        //an id to track the hotkey
        const int HOTKEY_ID = 9000;

        //MouseHandler class
        public MouseHandler mouseObject;


        public MainWindow()
        {
            InitializeComponent();
            
        }


        //when app is initialized, creates the hook from the window and calls our RegisterHotKey overload method
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            mouseObject = new MouseHandler();
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
            
        }

        //cleanup method for closing the app, removes hook and nulls the window source, unregisters hotkey then closes
        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }


        //our overload method, simply calls a helper to get window handle and registers hotkey with a label indicating whether it was registered or not
        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            if(!RegisterHotKey(helper.Handle,HOTKEY_ID,0,mykey))
            {
                reglabel.Content = "Hotkey was not able to be registered!";
            }
            else
            {
                reglabel.Content = "hotkey was able to be registered!";
            }
        }


        //unregisters the hotkey
        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }


        //our Hook, essentially a listener that checks if whatever key was pressed matches our registered hotkey, then performs the action if true
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            switch(msg)
            {
                case WM_HOTKEY:
                    switch(wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled= true;
                            break;
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        private async void OnHotKeyPressed()
        {
            //in ms
            var delay = 10;
            if(hotkeystate)
            {
                hotkeystate= false;
                statelabel.Content = "Hotkey is in OFF state";
            }    
            else
            {
                hotkeystate = true;
                statelabel.Content = "Hotkey is in ON state";
                
                while(hotkeystate)
                {
                    mouseObject.mouseLeftClick();
                    await Task.Delay(delay);
                }
                
                
            }
            
            
        }

        
    }
}
