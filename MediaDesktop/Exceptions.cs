using System;
using System.Collections.Generic;
using System.Text;

namespace MediaDesktop
{
    /// <summary>
    /// The exception that is thrown when "Program Manager" window is not found.
    /// </summary>
    public class ProgramManagerNotFoundException:Exception
    {
        public ProgramManagerNotFoundException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// The exception that is thrown when "WorkerW" window is not found.
    /// </summary>
    public class WorkerWNotFoundException : Exception
    {
        public WorkerWNotFoundException(string message) : base(message)
        {

        }
    }

    public class MediaAttachedPositionNotImplementedException:Exception
    {
        public MediaAttachedPositionNotImplementedException(string message) : base(message)
        {

        }
    }
}
