using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFIrregularShape.Dependencies
{
    /// <summary> 
    /// Helper class for WPF commands 
    /// </summary> 
    public static class AvalonCommandsHelper
    {
        /// <summary> 
        /// Gets the Can Execute of a specific command 
        /// </summary> 
        /// <param name="commandSource">The command to verify</param> 
        /// <returns></returns> 
        public static bool CanExecuteCommandSource(ICommandSource commandSource)
        {
            ICommand baseCommand = commandSource.Command;
            if (baseCommand == null)
                return false;


            object commandParameter = commandSource.CommandParameter;
            IInputElement commandTarget = commandSource.CommandTarget;
            RoutedCommand command = baseCommand as RoutedCommand;
            if (command == null)
                return baseCommand.CanExecute(commandParameter);
            if (commandTarget == null)
                commandTarget = commandSource as IInputElement;
            return command.CanExecute(commandParameter, commandTarget);
        }


    }
}
