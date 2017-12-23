namespace Fabric.Logging
{
    public class Global {

        private static Global _instance;

        private Global() {
            Initialise();
        }

        public static Global Instance => _instance ?? (_instance = new Global());

        public IGlobalLogger Logger { get; set; }

        public void Initialise() {
            Logger = new ConsoleGlobalLogger();
        }
    }
}
