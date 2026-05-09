public class ManagerLocatable : BaseLocatable<ManagerLocator>
{
    public T GetLocatable<T>() where T : ManagerLocatable =>
        Locator.GetLocatable<T>();
}