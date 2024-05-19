namespace RegistryManager
{
    public class Reg
    {
        private static readonly object _localMachineLock = new object();

        private static LocalMachine _localMachineInstance = new LocalMachine();

        public static LocalMachine LocalMachine
        {
            get
            {
                if (_localMachineInstance == null)
                {
                    lock (_localMachineLock)
                    {
                        if (_localMachineInstance == null)
                        {
                            _localMachineInstance = new LocalMachine();
                        }
                    }
                }

                return _localMachineInstance;
            }
        }
    }
}
