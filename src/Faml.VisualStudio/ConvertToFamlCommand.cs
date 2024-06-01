using System;
using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Faml.DevEnv;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Faml.VisualStudio {
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ConvertToFamlCommand {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("3e461da2-be68-4dbd-b81b-2c3fabd4bcac");

        private readonly FamlPackage _package;

        private readonly DTE2 _dte2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertToFamlCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ConvertToFamlCommand(FamlPackage package) {
            _package = package;

            _dte2 = _package.Dte2;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null) {
                var menuCommandId = new CommandID(CommandSet, CommandId);
                var oleMenuCommand = new OleMenuCommand(this.MenuItemCallback, menuCommandId);
                oleMenuCommand.BeforeQueryStatus += OnBeforeQueryStatus;
                commandService.AddCommand(oleMenuCommand);
            }
        }

        void OnBeforeQueryStatus(object sender, EventArgs e) {
            var myCommand = sender as OleMenuCommand;

            object[] selectedItems = (object[]) _dte2.ToolWindows.SolutionExplorer.SelectedItems;

            foreach (UIHierarchyItem selectedUiHierarchyItem in selectedItems) {
                if (selectedUiHierarchyItem.Object is ProjectItem projectItem) {
                    if (projectItem.Name.EndsWith(".xaml")) {
                        //myCommand.Enabled =   true;
                        myCommand.Visible = true;
                    }
                    else {
                        //myCommand.Enabled =   false;
                        myCommand.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ConvertToFamlCommand Instance {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => this._package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(FamlPackage package) {
            Instance = new ConvertToFamlCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e) {
            object[] selectedItems = (object[])_dte2.ToolWindows.SolutionExplorer.SelectedItems;

            foreach (UIHierarchyItem selectedUiHierarchyItem in selectedItems) {
                if (selectedUiHierarchyItem.Object is ProjectItem projectItem)
                    ConvertToFaml(projectItem);
            }
        }

        private void ConvertToFaml(ProjectItem projectItem) {
            ThreadHelper.ThrowIfNotOnUIThread();

            Property fullPathProperty = projectItem.Properties.Item("FullPath");
            if (fullPathProperty == null)
                return;

            string fullPath = (string) fullPathProperty.Value;

            if (!fullPath.EndsWith(".xaml"))
                return;

            string famlFullPath = fullPath.Substring(0, fullPath.Length - ".xaml".Length) + ".faml";

            if (File.Exists(famlFullPath))
                return;

            try {
                string lux;
                using (TextReader reader = new StreamReader(fullPath)) {
                    lux = new XamlToFaml().Convert(reader, 0);
                }

                using (TextWriter writer = File.CreateText(famlFullPath)) {
                    writer.Write(lux);
                }

                EnvDTE.Project project = projectItem.ContainingProject;
                project.ProjectItems.AddFromFile(famlFullPath);
            }
            catch (Exception e) {
                // TODO: Show error message box
                return;
            }
        }
    }
}
