namespace Whaledevelop.DiContainer
{
    public static class DiContainerUtility
    {
        public static IDiInternalContainer MainContainer => ProjectContext.Instance.MainContainer;

        internal static IDiInternalContainer GetOrCreateContainerById(string containerId, IDiInternalContainer baseContainer = null)
        {
            return GetContainerById(containerId) ?? ProjectContext.Instance.CreateContainer(containerId, baseContainer);
        }

        public static IDiInternalContainer CreateContainer(string containerId, IDiInternalContainer baseContainer = null)
        {
            return ProjectContext.Instance.CreateContainer(containerId, baseContainer);
        }

        public static IDiInternalContainer GetContainerById(string containerId)
        {
            return ProjectContext.Instance.GetContainer(containerId);
        }
    }
}