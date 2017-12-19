using System;

namespace Fabric.Core
{
    public class Global {

        private static Global _instance;

        private Global() {}

        public static Global Instance => _instance ?? (_instance = new Global());

        public void Initialise() {
            throw new NotImplementedException();
        }
    }
}
