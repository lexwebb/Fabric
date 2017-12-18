using System;

namespace Fabric
{
    public class Core {

        private static Core _instance;

        private Core() {}

        public static Core Instance => _instance ?? (_instance = new Core());

        public void Initialise() {
            throw new NotImplementedException();
        }
    }
}
