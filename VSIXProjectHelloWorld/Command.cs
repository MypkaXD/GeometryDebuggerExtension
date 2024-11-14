﻿using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace VSIXProjectHelloWorld
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class Command
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("9c904bb4-a653-42e8-861f-073072e727f2");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private Command(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Command Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in Command's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new Command(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>

        //private bool isSet = false;
        //private System.Windows.Window window;
        //private GeometryDebugger geometryDebugger;

        //private void Execute(object sender, EventArgs e)
        //{
        //    ThreadHelper.ThrowIfNotOnUIThread();

        //    if (!isSet)
        //    {
        //        window = new System.Windows.Window();
        //        geometryDebugger = new GeometryDebugger(); // create UI with WPF
        //        isSet = true;
        //    }

        //    window.Content = geometryDebugger; // set content on our ui
        //    window.Show(); // show this window
        //}
        private bool isSet = false;
        private System.Windows.Window window;
        private GeometryDebugger geometryDebugger;

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            window = new System.Windows.Window();
            geometryDebugger = new GeometryDebugger(); // create UI with WPF

            window.Content = geometryDebugger; // set content on our ui
            window.Show(); // show this window

            window.Closing += ClosedGeometryDebugger;
        }

        private void ClosedGeometryDebugger(object sender, CancelEventArgs e)
        {
            geometryDebugger.CloseOpenGLContext();
        }

    }
}
