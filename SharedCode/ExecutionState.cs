using System;

namespace Tortoise.Shared
{
    /// <summary>
    /// This class is used for the Return value Functions or Methods, when the possibility of failure is there. 
    /// This class represents a Void Type.
    /// </summary>
    public struct ExecutionState
    {


        /// <summary>
        /// This returns a Succeeded result.
        /// </summary
        public static ExecutionState Succeeded() { return new ExecutionState(true); }
        /// <summary>
        /// This returns a Failed result.
        /// </summary>
        public static ExecutionState Failed() { return new ExecutionState(false); }
        /// <summary>
        /// This returns a Succeeded result. It allows for a human readable string to be passed along.
        /// </summary
        public static ExecutionState Succeeded(string reason) { return new ExecutionState(true, reason); }
        /// <summary>
        /// This returns a Failed result. It allows for a human readable string to be passed along.
        /// </summary>
        public static ExecutionState Failed(string reason) { return new ExecutionState(false, reason); }

        public bool Sucess;
        public string Reason;

        public ExecutionState(bool sucess)
        {
            Sucess = sucess;
            Reason = "";
        }
        public ExecutionState(bool sucess, string reason)
        {
            Sucess = sucess;
            Reason = reason;
        }

        public static bool operator true(ExecutionState exestate)
        {
            return exestate.Sucess;
        }
        public static bool operator false(ExecutionState exestate)
        {
            return !exestate.Sucess;
        }

        public static bool operator !(ExecutionState exestate)
        {
            return !exestate.Sucess;
        }
    }

    /// <summary>
    /// This class is used for the Return value of Functions or Methods, when the possibility of failure is there.
    /// This class allows a Result to be passed through it so it may replace non void types.
    /// </summary>
    public struct ExecutionState<T>
    {
        /// <summary>
        /// This returns a Succeeded result.
        /// </summary>
        public static ExecutionState<T> Succeeded(T result) { return new ExecutionState<T>(true, result); }
        /// <summary>
        /// This returns a Failed result, using the default of T as the result's value.
        /// </summary>
        public static ExecutionState<T> Failed() { return new ExecutionState<T>(false, default(T)); }
        /// <summary>
        /// This returns a Succeeded result. It allows for a human readable string to be passed along.
        /// </summary>
        public static ExecutionState<T> Succeeded(T result, string reason) { return new ExecutionState<T>(true, result, reason); }
        /// <summary>
        /// This returns a Failed result, using the default of T as the result's value. It allows for a human readable string to be passed along.
        /// </summary>
        public static ExecutionState<T> Failed(string reason) { return new ExecutionState<T>(false, default(T), reason); }

        public bool Sucess;
        public T Result;
        public string Reason;

        public ExecutionState(bool sucess, T result)
        {
            Sucess = sucess;
            Result = result;
            Reason = "";
        }

        public ExecutionState(bool sucess, T result, string reason)
        {
            Sucess = sucess;
            Result = result;
            Reason = reason;
        }


        public static bool operator true(ExecutionState<T> exestate)
        {
            return exestate.Sucess;
        }
        public static bool operator false(ExecutionState<T> exestate)
        {
            return !exestate.Sucess;
        }

        public static bool operator !(ExecutionState<T> exestate)
        {
            return !exestate.Sucess;
        }
    }
}
