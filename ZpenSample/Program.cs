using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZpenSample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormZpenSample());
        }
    }
}



// Using Directives: These directives bring in namespaces that contain classes and methods used in the code. For example, System contains fundamental types and base types that define commonly-used value and reference data types, events and event handlers, interfaces, attributes, and processing exceptions. System.Windows.Forms contains classes for creating Windows-based applications that take full advantage of the rich user interface features available in the Microsoft Windows operating system.

// Namespace Declaration: The namespace keyword is used to declare a scope that contains a set of related objects. In this case, the Program.cs file is in the ZpenSample namespace.

// Program Class: This is a static class named Program. It contains the Main method, which serves as the entry point for the application.

// Main Method: The Main method is the starting point for the application. It is marked with the [STAThread] attribute, which indicates that the COM threading model for the application is single-threaded apartment (STA). Inside Main, the following actions are performed:

// Application.EnableVisualStyles(): This enables visual styles for the application's controls. It allows your application to take advantage of the visual styles provided by the operating system.
// Application.SetCompatibleTextRenderingDefault(false): This sets the application's default text rendering mode to be compatible with the operating system's default mode. In this case, it's set to false, indicating that GDI+ will be used for text rendering.
// Application.Run(new FormZpenSample()): This starts the application by creating and running an instance of the FormZpenSample form.