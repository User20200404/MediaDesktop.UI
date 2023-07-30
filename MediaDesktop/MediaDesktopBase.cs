using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WindowManager;

namespace MediaDesktop
{
    public class MediaDesktopBase
    {
        #region Private Fields
        private Process explorerProcess;
        private IntPtr progManHandle;
        private IntPtr workerWHandle;
        private MediaAttachedPosition mediaAttachedPosition;
        private Form attachmentHandler;
        #endregion

        #region PrivateProperties
        #endregion

        #region PublicProperties
        /// <summary>
        /// The <see cref="Process"/> instance of the explorer process attached to.
        /// </summary>
        public Process ExplorerProcessAttached
        {
            get { return explorerProcess; }
        }

        /// <summary>
        /// The handle of "Program Manager" window that belongs to <see cref="ExplorerProcessAttached"/>
        /// </summary>
        public IntPtr ProgramManagerWindowHandle
        {
            get { return progManHandle; }
        }

        /// <summary>
        /// The handle of "WorkerW" window that belongs to <see cref="ExplorerProcessAttached"/>
        /// </summary>
        public IntPtr WorkerWHandle
        {
            get { return workerWHandle; }
        }

        /// <summary>
        /// The form window where media player is set as a child of it.
        /// </summary>
        public Form AttachmentHandlerForm
        {
            get { return attachmentHandler; }
        }
        /// <summary>
        /// Get or set the position where media is played. Changing this value after the mediaplay starts is not supported.
        /// </summary>
        public MediaAttachedPosition MediaAttachedPosition
        {
            get { return mediaAttachedPosition; }
            set { mediaAttachedPosition = value; }
        }
        #endregion



        /// <summary>
        /// Check and initialize the environment needed for media desktop.
        /// </summary>
        /// <exception cref="ProgramManagerNotFoundException"></exception>
        /// <exception cref="WorkerWNotFoundException"></exception>
        private void Startup()
        {
            ReceiverForm.Instance.Show();
            ReceiverForm.Instance.ScreenSolutionChanged += Instance_ScreenSolutionChanged;

            //[MAY THROW EXCEPTION]
            progManHandle = Validation.GetProgramManagerWindow();

            explorerProcess = APIsEncapsulated.GetProcessByWindowHandle(progManHandle);

            //[MAY BLOCK THE THREAD] Send the message that makes explorer.exe window layered.
            SystemAPIs.SendMessageA(progManHandle, 1324, IntPtr.Zero, IntPtr.Zero);

            workerWHandle = Validation.GetWorkerWWindow();

            attachmentHandler = new Form()
            {
                BackColor = Color.Pink,
                FormBorderStyle = FormBorderStyle.None,
            };
            SystemWindow handlerWindow = SystemWindow.GetByHandle(attachmentHandler.Handle);
            handlerWindow.LayeredAttributes.Alpha = 255;
        }

        private void Instance_ScreenSolutionChanged(object sender, EventArgs e)
        {
            ResizeHandlerToScreen();
        }


        /// <summary>
        /// Attach the media to the desktop for playing.
        /// </summary>
        /// <exception cref="MediaAttachedPositionNotImplementedException"></exception>
        public void AttachToDesktop()
        {
            IntPtr targetHandle;
            switch (mediaAttachedPosition)
            {
                case MediaAttachedPosition.AttachToWorkerW:
                    targetHandle = workerWHandle;
                    break;
                case MediaAttachedPosition.AttachToProgMan:
                    APIsEncapsulated.HideWindow(workerWHandle);
                    targetHandle = progManHandle;
                    break;
                default:
                    throw new MediaAttachedPositionNotImplementedException("The position for media to attach is needed, you have to set the property \"MediaAttachedPosition\" first.");
            }
            
            SystemAPIs.SetParent(attachmentHandler.Handle, targetHandle);
            ResizeHandlerToScreen();
        }

        public void ResizeHandlerToScreen()
        {
            APIsEncapsulated.GetScreenResolution(out int width, out int height);
            ResizeHandler(width, height);
        }

        public void ResizeHandler(int width,int height)
        {
            attachmentHandler.Show();
            attachmentHandler.Width = width + 5;
            attachmentHandler.Height = height + 5;
            attachmentHandler.Top = 0;
            attachmentHandler.Left = 0;
        }

        /// <summary>
        /// Create an instance of <see cref="MediaDesktopBase"/> and initialize the environment required for desktop mediaplay.
        /// </summary>
        public MediaDesktopBase()
        {
            Startup();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
    }

    /// <summary>
    /// Decides the position where media is played.
    /// </summary>
    public enum MediaAttachedPosition
    {
        /// <summary>
        /// The default value, always causes <see cref="MediaAttachedPositionNotImplementedException"/>, you will have to choose another value instead.
        /// </summary>
       NotImplemented = 0,

        /// <summary>
        /// The media is played as a child window of "Program Manager" window. This may causes bug where other windows would be placed under desktop icons when showing desktop.
        /// </summary>
        AttachToProgMan = 1,

        /// <summary>
        /// The media is played as a child window of "WorkerW" window. This option is mostly used, but you will have to reset the wallpapaer manually after the media is closed, to avoid painting bugs.
        /// </summary>
        AttachToWorkerW = 2
    }
}
