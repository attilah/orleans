namespace Orleans.Configuration
{
    /// <summary>
    /// The TracingOptions type contains various tracing-related configuration parameters.
    /// For production use, the default value of these parameters should be fine.
    /// </summary>
    public class TraceOptions
    {
        /// <summary>
        /// The TraceFileName property specifies the name of a file that trace output should be written to.
        /// </summary>
        public string TraceFileName { get; set; }

        /// <summary>
        /// The TraceFilePattern property specifies the pattern name of a file that trace output should be written to.
        /// </summary>
        public string TraceFilePattern { get; set; } = "{0}-{1}.log";
    }
}
