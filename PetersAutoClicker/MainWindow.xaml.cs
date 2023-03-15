using System;
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
using System.Data;
using Microsoft.EntityFrameworkCore;

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

        //the database setup
        private static DatabaseHandler context = new DatabaseHandler();
        
        //keystate
        private bool hotkeystate = false;

        //main key i want (tested and this works as well)
        private uint mykey;

        //the delay
        private int delay;

        //total amount of clicks since application start
        private int totalclicks = 0;

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

        public void SetSavedKey()
        {
            var  keylist = context.Hotkeys.ToList();

            //we need to do a nullcheck here

            if (keylist == null)
            {
                mykey = (uint)Keys.F8.GetHashCode();
                
            }
            else
            {
                foreach (HotkeyEntity x in keylist)
                {
                    if(x.Storedhotkey == 0)
                    {
                        mykey = (uint)Keys.F8.GetHashCode();
                    }
                    else
                    {
                        mykey = x.Storedhotkey;
                    }
                    
                }
            }

        }

        public void SetDelay()
        {
            //and a nullcheck in here
            var delaylist = context.Hotkeys.ToList();

            //delay amount is in ms
            if (delaylist == null)
            {
                delay = 1000;
                delayslider.Value = delay;
                delayamountlabel.Content = $"Current Delay: {delay}";
            }
            else
            {
                foreach (HotkeyEntity x in delaylist)
                {
                    if (x.Delay == 0)
                    {
                        delay = 1000;
                        delayslider.Value = delay;
                        delayamountlabel.Content = $"Current Delay: {delay}";
                    }
                    else
                    {
                        delay = x.Delay;
                        delayslider.Value = delay;
                        delayamountlabel.Content = $"Current Delay: {delay}";
                    }
                    
                }
            }
        }


        //when app is initialized, creates the hook from the window and calls our RegisterHotKey overload method
        protected override void OnSourceInitialized(EventArgs e)
        {
            SetSavedKey();
            SetDelay();
            base.OnSourceInitialized(e);
            populateComboBox();
            var helper = new WindowInteropHelper(this);
            mouseObject = new MouseHandler();
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
            
        }

        //method is used to clear hotkey table
        public void ClearHotkeyTable()
        {
            foreach(var x in context.Hotkeys)
            {
                context.Hotkeys.Remove(x);
            }

            context.SaveChanges();
        }

        
        

        protected void populateComboBox()
        {
            var keyList = new List<string>() { "F8", "Keypad +", "Keypad -", "Keypad *", "Keypad /" };
            
            keycombobox.ItemsSource = keyList;
            
            /* Add is 107
             * Multiply is 106
             * Divide is 111
             * Subtract is 109
             * F8 is 119
             */
            switch(mykey)
            {
                case 119:
                    keycombobox.SelectedItem = keyList[0];
                    break;
                case 107:
                    keycombobox.SelectedItem = keyList[1];
                    break;
                case 109: 
                    keycombobox.SelectedItem = keyList[2];
                    break;
                case 106:
                    keycombobox.SelectedItem = keyList[3];
                    break;
                case 111:
                    keycombobox.SelectedItem = keyList[4];
                    break;
                default:
                    keycombobox.SelectedItem = keyList[0];
                    break;
                
            }

            //display current key
            currentkeylabel.Content = "Current Key: " + keycombobox.SelectedItem.ToString();

            //display current clicks
            amountclickslabel.Content = "Total Clicks: " + totalclicks.ToString();
        }

        //cleanup method for closing the app, removes hook and nulls the window source, unregisters hotkey then closes, and final saves hotkey and delay
        protected override void OnClosed(EventArgs e)
        {
            saveHotkeyandDelay(mykey, delay);
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
                currentkeylabel.Content = "Current Key: " + keycombobox.SelectedItem.ToString();
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

        public void saveHotkeyandDelay(uint key,int d)
        {
            ClearHotkeyTable();
            var savedkey = new HotkeyEntity { Storedhotkey = key, Delay = d};
            context.Hotkeys.Add(savedkey);
            context.SaveChanges();
        }

        private void setkeybtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedkey = keycombobox.SelectedValue.ToString();
            switch(selectedkey)
            {
                case "F8":
                    mykey = (uint)Keys.F8.GetHashCode();
                    UnregisterHotKey();
                    RegisterHotKey();
                    break;
                case "Keypad +":
                    mykey = (uint)Keys.Add.GetHashCode();
                    UnregisterHotKey();
                    RegisterHotKey();
                    break;
                case "Keypad -":
                    mykey = (uint)Keys.Subtract.GetHashCode();
                    UnregisterHotKey();
                    RegisterHotKey();
                    break;
                case "Keypad *":
                    mykey = (uint)Keys.Multiply.GetHashCode();
                    UnregisterHotKey();
                    RegisterHotKey();
                    break;
                case "Keypad /":
                    mykey = (uint)Keys.Divide.GetHashCode();
                    UnregisterHotKey();
                    RegisterHotKey();
                    break;
                default:
                    System.Windows.Forms.MessageBox.Show("Character not allowed, hotkey set to default F8");
                    mykey = (uint)Keys.F8.GetHashCode();
                    UnregisterHotKey();
                    RegisterHotKey();
                    break;
            }

            saveHotkeyandDelay(mykey,delay);
        }

        

        private void delayslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            delayamountlabel.Content = $"Current Delay: {e.NewValue}";
            delay = (int)e.NewValue;
        }

        private void speedbtn_Click(object sender, RoutedEventArgs e)
        {
            amountclickslabel.Content = "Total Clicks: " + totalclicks;
            totalclicks++;
        }
    }
}
