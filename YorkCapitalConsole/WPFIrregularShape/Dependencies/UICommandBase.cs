using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFIrregularShape.Dependencies
{
    public abstract class UICommandBase : Window
    {
        //<summary> 
        //Command to Minimize the windows 
        //As you can see the command also hooks to the F3 key of the keyboard 
        //</summary> 
        public static RoutedUICommand MinimizeCmd = new RoutedUICommand("MinimizeCmd",
                "MinimizeCmd", typeof(UICommandBase), new InputGestureCollection(
                    new InputGesture[] { new KeyGesture(Key.F3, ModifierKeys.None, "Minimize Cmd") }
                    ));

        /// <summary> 
        /// Command to Maximize the windows 
        ///As you can see the command also hooks to the F4 key of the keyboard 
        /// </summary> 
        public static RoutedUICommand MaximizeCmd = new RoutedUICommand("MaximizeCmd",
                "MaximizeCmd", typeof(UICommandBase), new InputGestureCollection(
                    new InputGesture[] { new KeyGesture(Key.F4, ModifierKeys.None, "Maximize Cmd") }
                    ));

        /// <summary> 
        /// Command to Restore the windows 
        /// As you can see the command also hooks to the F5 key of the keyboard 
        /// </summary> 
        public static RoutedUICommand RestoreCmd = new RoutedUICommand("RestoreCmd",
                "RestoreCmd", typeof(UICommandBase), new InputGestureCollection(
                    new InputGesture[] { new KeyGesture(Key.F5, ModifierKeys.None, "Restore Cmd") }
                    ));

        /// <summary> 
        /// Command to Close the windows 
        /// As you can see the command also hooks to the F6 key of the keyboard 
        /// </summary> 
        public static RoutedUICommand CloseCmd = new RoutedUICommand("CloseCmd",
                "CloseCmd", typeof(UICommandBase), new InputGestureCollection(
                    new InputGesture[] { new KeyGesture(Key.F6, ModifierKeys.None, "Close Cmd") }
                    ));


        public UICommandBase()
        {

        }

    }
}
