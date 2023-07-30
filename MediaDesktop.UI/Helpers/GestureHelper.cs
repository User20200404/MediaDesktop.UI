using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Input;
using Windows.Foundation;

namespace MediaDesktop.UI.Helpers
{
    class GestureHelper
    {
        GestureRecognizer recognizer;
        UIElement element;
        UIElement reference;
        TransformGroup cumulativeTransform;
        MatrixTransform previousTransform;
        CompositeTransform deltaTransform;

        public event TypedEventHandler<GestureRecognizer, ManipulationStartedEventArgs> ManipulationStarted;
        public event TypedEventHandler<GestureRecognizer, ManipulationUpdatedEventArgs> ManipulationUpdated;
        public event TypedEventHandler<GestureRecognizer, ManipulationCompletedEventArgs> ManipulationCompleted;
        public event TypedEventHandler<GestureRecognizer, ManipulationInertiaStartingEventArgs> ManipulationInertiaStarting;

        public GestureHelper(GestureRecognizer gestureRecognizer, UIElement target, UIElement referenceFrame)
        {
            recognizer = gestureRecognizer;
            element = target;
            reference = referenceFrame;
            // Initialize the transforms that will be used to manipulate the shape
            InitializeTransforms();
            // The GestureSettings property dictates what manipulation events the
            // Gesture Recognizer will listen to.  This will set it to a limited
            // subset of these events.
            recognizer.GestureSettings = GenerateDefaultSettings();
            // Set up pointer event handlers. These receive input events that are used by the gesture recognizer.
            element.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            element.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(OnPointerMoved), true);
            element.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            element.AddHandler(UIElement.PointerCanceledEvent, new PointerEventHandler(OnPointerCanceled), true);
            // Set up event handlers to respond to gesture recognizer output
            recognizer.ManipulationStarted += OnManipulationStarted;
            recognizer.ManipulationUpdated += OnManipulationUpdated;
            recognizer.ManipulationCompleted += OnManipulationCompleted;
            recognizer.ManipulationInertiaStarting += OnManipulationInertiaStarting;
        }

        public void InitializeTransforms()
        {
            cumulativeTransform = new TransformGroup();
            deltaTransform = new CompositeTransform();
            previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };
            cumulativeTransform.Children.Add(previousTransform);
            cumulativeTransform.Children.Add(deltaTransform);
            element.RenderTransform = cumulativeTransform;
        }

        // Return the default GestureSettings for this sample
        GestureSettings GenerateDefaultSettings()
        {
            return GestureSettings.ManipulationTranslateX |
            GestureSettings.ManipulationTranslateY |
            GestureSettings.ManipulationRotate |
            GestureSettings.ManipulationTranslateInertia |
            GestureSettings.ManipulationRotateInertia;
        }

        // Route the pointer pressed event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            // Set the pointer capture to the element being interacted with so that only it
            // will fire pointer-related events
            element.CapturePointer(args.Pointer);
            // Feed the current point into the gesture recognizer as a down event
            recognizer.ProcessDownEvent(args.GetCurrentPoint(reference));
        }

        // Route the pointer moved event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            // Feed the set of points into the gesture recognizer as a move event
            recognizer.ProcessMoveEvents(args.GetIntermediatePoints(reference));
        }

        // Route the pointer released event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            // Feed the current point into the gesture recognizer as an up event
            recognizer.ProcessUpEvent(args.GetCurrentPoint(reference));
            // Release the pointer
            element.ReleasePointerCapture(args.Pointer);
        }

        // Route the pointer canceled event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerCanceled(object sender, PointerRoutedEventArgs args)
        {
            recognizer.CompleteGesture();
            element.ReleasePointerCapture(args.Pointer);
        }

        // When a manipulation begins, change the color of the object to reflect
        // that a manipulation is in progress
        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ManipulationStarted?.Invoke(sender as GestureRecognizer, e);
        }

        // Process the change resulting from a manipulation
        void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
        {
            ManipulationUpdated?.Invoke(sender as GestureRecognizer, e);
        }

        // When a manipulation that's a result of inertia begins, change the color of the
        // the object to reflect that inertia has taken over
        void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            ManipulationInertiaStarting?.Invoke(sender as GestureRecognizer, e);
        }

        // When a manipulation has finished, reset the color of the object
        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ManipulationCompleted?.Invoke(sender as GestureRecognizer, e);
        }

        // Modify the GestureSettings property to only allow movement on the X axis
        public void LockToXAxis()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateY | GestureSettings.ManipulationTranslateX;
            recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateY;
        }

        // Modify the GestureSettings property to only allow movement on the Y axis
        public void LockToYAxis()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateY | GestureSettings.ManipulationTranslateX;
            recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateX;
        }

        // Modify the GestureSettings property to allow movement on both the X and Y axes
        public void MoveOnXAndYAxes()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateX | GestureSettings.ManipulationTranslateY;
        }

        // Modify the GestureSettings property to enable or disable inertia based on the passed-in value
        public void UseInertia(bool inertia)
        {
            if (!inertia)
            {
                recognizer.CompleteGesture();
                recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateInertia | GestureSettings.ManipulationRotateInertia;
            }
            else
            {
                recognizer.GestureSettings |= GestureSettings.ManipulationTranslateInertia | GestureSettings.ManipulationRotateInertia;
            }
        }

        public void Reset()
        {
            element.RenderTransform = null;
            recognizer.CompleteGesture();
            InitializeTransforms();
            recognizer.GestureSettings = GenerateDefaultSettings();

        }
    }
}
