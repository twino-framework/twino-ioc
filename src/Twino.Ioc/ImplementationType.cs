namespace Twino.Ioc
{
    /// <summary>
    /// Service creation and keep types
    /// </summary>
    public enum ImplementationType
    {
        /// <summary>
        /// For each call, new instance is created
        /// </summary>
        Transient,

        /// <summary>
        /// Instance is created only once, returns same object for each call
        /// </summary>
        Singleton,

        /// <summary>
        /// Instance is created only once for per scope.
        /// For different scopes, different instances are created
        /// </summary>
        Scoped
    }
}