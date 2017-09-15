namespace Microsoft.Orleans.ApplicationParts
{
    /// <summary>
    /// A part of an Orleans application.
    /// </summary>
    public abstract class ApplicationPart
    {
        /// <summary>
        /// Gets the <see cref="ApplicationPart"/> name.
        /// </summary>
        public abstract string Name { get; }
    }
}
