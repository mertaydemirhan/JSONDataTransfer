namespace DataTransfer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Control.CheckForIllegalCrossThreadCalls = false;
            if(args.Count() > 0) { 
            
                Application.Run(new Form1(args[0]));
            }
            else
                Application.Run(new Form1(""));
        }
    }
}