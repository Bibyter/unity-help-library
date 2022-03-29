
public static class StaticInjector
{
    public static T GetLink<T>() where T : class
    {
        return Bibyter.DependencyInjection.SceneLinkRegistrator.GetLink<T>();
    }

    public static T GetLink<T>(string name) where T : class
    {
        return Bibyter.DependencyInjection.SceneLinkRegistrator.GetNamedLink<T>(name);
    }
}
