namespace Samples
{
    using System;

    /// <summary>
    /// Main samples entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Arguments from command line.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Uncomment test to run... ");

            TestZipkin.Run();
            // TestApplicationInsights.Run();
            // TestPrometheus.Run();
            // TestHttpClient.Run();
        }
    }
}
