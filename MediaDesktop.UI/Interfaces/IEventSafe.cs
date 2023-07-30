using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Interfaces
{
    internal interface IEventSafe
    {
        /// <summary>
        /// Should register certain events of an extern object.
        /// </summary>
        protected void EventStartup();
        /// <summary>
        /// Should unregister certain events of an extern object.
        /// </summary>
        protected void EventLogOff();
    }

    internal interface IEventSafeSealed
    {
        /// <summary>
        /// Should register certain events of an extern object.
        /// </summary>
        void EventStartup();

        /// <summary>
        /// Should unregister certain events of an extern object.
        /// </summary>
        void EventLogOff();
    }
}
