namespace Whaledevelop.DiContainer
{
    public static class DiContainerUtility
    {
        public static IDiContainer MainContainer => ProjectContext.Instance.MainContainer;

        internal static IDiContainer GetOrCreateContainerById(string containerId, IDiContainer baseContainer = null)
        {
            return GetContainerById(containerId) ?? ProjectContext.Instance.CreateContainer(containerId, baseContainer);
        }

        public static IDiContainer CreateContainer(string containerId, IDiContainer baseContainer = null)
        {
            return ProjectContext.Instance.CreateContainer(containerId, baseContainer);
        }

        public static IDiContainer GetContainerById(string containerId)
        {
            return ProjectContext.Instance.GetContainer(containerId);
        }
    }
}