namespace MyExtension
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyToolWindowCommand : BaseCommand<MyToolWindowCommand>
    {

        GeometryDebugger geometryDebugger = new GeometryDebugger();

        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            if (geometryDebugger != null && !geometryDebugger.isInit)
            {
                geometryDebugger = new GeometryDebugger();
                geometryDebugger.isInit = true;
                geometryDebugger.init();
                return MyToolWindow.ShowAsync();
            }
            else
                return null;
        }
    }
}
