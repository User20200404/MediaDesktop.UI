using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Helpers
{
    /// <summary>
    /// Provides event handler (ignores e.Handled property) for an element.
    /// </summary>
    /// <typeparam name="ArgType"></typeparam>
    public class GlobalRoutedEventHelper<ArgType, REventHandlerType> : IDisposable where ArgType : RoutedEventArgs where REventHandlerType : Delegate
    {
        private UIElement targetElement;
        private REventHandlerType handler;
        private RoutedEvent targetEvent;

        /// <summary>
        /// Occurs when target routedEvent is passed to target element. 
        /// </summary>
        public event EventHandler<ArgType> RoutedEventTriggered;
        private void eventTrigger(object sender, ArgType args)
        {
            RoutedEventTriggered?.Invoke(sender, args);
        }

        /// <summary>
        /// Releases all event handler of target element and unregister all events attached to this object.
        /// </summary>
        public void Dispose()
        {
            targetElement.RemoveHandler(targetEvent, handler);
        }

        /// <summary>
        /// Initializes a handler that ignores e.Handled property for <paramref name="rootElement"/>.
        /// </summary>
        /// <param name="rootElement">The element which handler will attach to. Should be a root element if you want to handle global event.</param>
        /// <param name="eventToHandle">The type of the routed event to handle.</param>
        public GlobalRoutedEventHelper(UIElement rootElement, RoutedEvent eventToHandle)
        {
            targetElement = rootElement;
            targetEvent = eventToHandle;
            MethodInfo info = this.GetType().GetMethod("eventTrigger",BindingFlags.NonPublic|BindingFlags.Instance);
            handler = (REventHandlerType)Delegate.CreateDelegate(typeof(REventHandlerType),this,info);
            targetElement.AddHandler(eventToHandle, handler, true);
        }
    }
}
