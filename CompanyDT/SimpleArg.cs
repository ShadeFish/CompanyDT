using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleArgLib
{
    class Argument
    {
        public string name;
        public string value;
        public static Argument Empty = new Argument(string.Empty, string.Empty);

        public Argument() { }
        public Argument(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return "Name: " + name + "\n" +
                   "Value: " + value + "\n";
        }
    }

    class SimpleArg
    {
        /* INPUT SYNTAX EXAMPLE:
            myApp.exe -argumentName(value) -argumentName

            OUTPUT:
            1. argumentName:value
            2. -arguementName:""
        */
        public static Argument[] Parse(string[] args)
        {
            List<Argument> arguments = new List<Argument>();

            foreach (string arg in args)
            {
                /* ARGUMENT WITH VALUE */
                if (arg.Contains("(") && arg.Contains(")") && arg.Contains("-"))
                {
                    int bracketStartIndex = arg.IndexOf('(');
                    int bracketStopIndex = arg.IndexOf(')');
                    string name = arg.Substring(1, bracketStartIndex - 1);
                    string value = arg.Substring(bracketStartIndex + 1, arg.Length - bracketStartIndex - 2);
                    arguments.Add(new Argument(name, value));
                }

                /* ARGUMENT WITOHUT VALUE */
                if (!arg.Contains("(") && !arg.Contains(")") && arg.Contains("-"))
                {
                    arguments.Add(new Argument(arg.Substring(1, arg.Length - 1), ""));
                }
            }
            return arguments.ToArray();
        }
    }


}
