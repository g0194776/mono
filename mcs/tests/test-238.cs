using System;
using System.Diagnostics;

class TestClass {
        [Conditional ("UNDEFINED_CONDITION")]
        static void ConditionalMethod ()
        {
            Environment.Exit (1);
        }
    
        static int Main()
        {
            ConditionalMethod ();
            Console.WriteLine ("Succeeded");
            return 0;
        }
}
