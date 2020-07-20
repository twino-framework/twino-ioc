using System;
using Twino.Ioc;

namespace Sample.Ioc
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceContainer container = new ServiceContainer();
            
            //optional, for checking missing references and circular references.
            //if there is a missing ref or circularity, throws exception
            container.CheckServices();
      
            Console.WriteLine("Hello World!");
        }
    }
}