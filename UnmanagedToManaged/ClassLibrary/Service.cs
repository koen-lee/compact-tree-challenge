using System;

namespace ClassLibrary
{
    public class Service
    {
        public Service()
        {
            Console.WriteLine("ManagedCode - Service has been istantiated");
        }

        public void Process(int message)
        {
            Console.WriteLine("ManagedCode - Process : {0}", message);
        }
    }
}
