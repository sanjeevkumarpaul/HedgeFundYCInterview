using EvaluateExpression.Helpers;
using Extensions;
using Wrappers;

namespace EvaluateExpression
{
    /// <summary>
    /// Main class, guide to user(s) about the Evaluate methods that can be achived.
    /// Constructor is the key where it indicates this class can not be instansiated, but has an static instance which would act on other methods.
    /// </summary>
    public partial class Evaluate
    {
        private static Evaluate instance;
        private static object lockobj = new object();

        #region ^Ctor
        static Evaluate()
        {
            if (instance == null)
            lock (lockobj)
            {
                if (instance == null) instance = new Evaluate();
            }
        }
        #endregion ~Ctor
    }

    /// <summary>
    /// All public methods open to users.
    /// </summary>
    public partial class Evaluate
    {
        #region ^Mathematic Evaluations
        public static T Math<T>(string equation)
        {
            return new EvalMath().Calculate<T>(equation);
        }

        public static T Math<T>(string equation, object parameter)
        {
            return Math<T>( instance.SetParmeterValues( equation, parameter ));
        }
        #endregion ~Mathematic Evaluations
    }


    /// <summary>
    /// Private methods
    /// </summary>
    partial class Evaluate
    {
        /// <summary>
        /// Sets the values of the Paramters if happen to found on the parameter object.
        /// This can be any object, it reads all public property and tries to set by comparing the name for all of it.
        /// </summary>
        /// <param name="equation">string that represents an equation</param>
        /// <param name="parameter">Object that can hold the values for any of the paramter.</param>
        /// <returns></returns>
        private string SetParmeterValues(string equation, object parameter)
        {
            if (parameter != null)            
                parameter.PropertyNames().ForEach((prop) => { equation = equation.Replace(prop, parameter.GetVal(prop)); });              
            
            return equation;
        }
    }
}
