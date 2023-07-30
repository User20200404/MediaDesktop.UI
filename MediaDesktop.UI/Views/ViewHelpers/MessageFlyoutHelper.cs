using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserDataTasks;

namespace MediaDesktop.UI.Views.ViewHelpers
{

    public class MessageParameter
    {
        public object MessageContent { get; set; } = "Default";
        public TimeSpan ShowAniDuration { get; set; } = TimeSpan.FromMilliseconds(200);
        public TimeSpan HideAniDuration { get; set; } = TimeSpan.FromMilliseconds(200);
        public TimeSpan HoldDuration { get; set; } = TimeSpan.FromMilliseconds(1000);
        public bool IsShowAnimationEnabled { get; set; } = true;
        public bool IsHideAnimationEnabled { get; set; } = true;
        public MessageParameter() 
        {
        }
    }

    /// <summary>
    /// Provides functionality for showing text message for a certain duration.
    /// </summary>
    public class MessageFlyoutHelper
    {
        bool isWorking = false;
        Compositor aniCompositor;
        ContentControl element;
        ConcurrentQueue<MessageParameter> taskQueue = new ConcurrentQueue<MessageParameter>();
        ScalarKeyFrameAnimation sharedAnimation;
        public MessageFlyoutHelper(Compositor aniCompositor,ContentControl relatedElement)
        {
            this.aniCompositor = aniCompositor;
            this.element = relatedElement;
            element.Opacity = 0;
            sharedAnimation = aniCompositor.CreateScalarKeyFrameAnimation();
            sharedAnimation.Target = "Opacity";
        }

        public void Enqueue(MessageParameter param)
        {
            taskQueue.Enqueue(param);
            Debug.WriteLine("Task Enqueued. Count is " + taskQueue.Count);
            if (!isWorking)
            {
                TaskLoop();
            }
        }

        private void ElementDispatcherEnqueue(Microsoft.UI.Dispatching.DispatcherQueueHandler callBack)
        {
            element.DispatcherQueue.TryEnqueue(callBack);
        }

        private async void TaskLoop()
        {
            while (taskQueue.Count > 0)
            {
                isWorking = true;
                taskQueue.TryDequeue(out MessageParameter param);
                Debug.WriteLine("Task Dequeued. Count is now " + taskQueue.Count);

                ElementDispatcherEnqueue(() => {
                    if (param.MessageContent is string str)
                    {
                        param.MessageContent = new TextBlock() { Text = str, FontSize = 15 };
                    }
                    if (param.MessageContent is FrameworkElement fwElement)
                    {
                        fwElement.Margin = new Thickness(10);
                    }

                    element.Content = param.MessageContent;
                });
           
                TimeSpan totalDuration = TimeSpan.Zero;
                if (param.IsShowAnimationEnabled)
                {
                    totalDuration += param.ShowAniDuration;
                }
                if(param.IsHideAnimationEnabled)
                {
                    totalDuration += param.HideAniDuration;
                }
                totalDuration += param.HoldDuration;
                sharedAnimation.Duration = totalDuration;

                double showKeyPoint = param.ShowAniDuration / totalDuration;
                double hideKeyPoint = 1 - (param.HideAniDuration / totalDuration);
                sharedAnimation.InsertKeyFrame(0, 0);
                sharedAnimation.InsertKeyFrame((float)showKeyPoint,1);
                sharedAnimation.InsertKeyFrame((float)hideKeyPoint,1);
                sharedAnimation.InsertKeyFrame(1, 0);

                ElementDispatcherEnqueue(() => 
                {
                    element.Visibility = Visibility.Visible;
                    element.StartAnimation(sharedAnimation);
                });
                await Task.Delay(totalDuration);
                ElementDispatcherEnqueue(() => { 
                    element.StopAnimation(sharedAnimation);
                    element.Visibility = Visibility.Collapsed;
                });
            }
            isWorking = false;
        }
    }
}
