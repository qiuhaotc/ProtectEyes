namespace ChangeRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoStartUtils.SelfRunning(bool.Parse(args[0]), args[1].Replace("_", " "), args[2]);
        }
    }
}
