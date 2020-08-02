using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SecdLisp
{
    class Program
    {
        // Arguments
        // Bootstrap C S [B]
        // Load program compilerC.secd
        // Load source  compilerS.lsp
        // Compile to compilerS.secd
        // If B present:
        // Load program compilerS.secd
        // Compile to compilerS.secd
        static void Main(string[] args)
        {
            VM vm;
            string c, s;
            bool b = args.Length == 3;
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid Args");
                return;
            }
            c = args[0];
            s = args[1];

            if (s == c)
            {
                Console.WriteLine("File idents must be different");
                return;
            }
            // Load compiled compiler 
            Lisp compiler = LoadFile("compiler" + c + ".secd");

            // Load compiler source
            Lisp source = LoadFile("compiler" + s + ".lsp");
            // Convert to argument list
            source = new Cons(source, Lisp.NIL);

            vm = new VM();
            vm.SetProgram(compiler);
            vm.SetInput(source);

            Op op;

            do op = vm.Step();
            while (vm.Running());

            if (vm.Errored())
                return;

            Console.WriteLine("Compilation complete");

            if (b)
            {
                Console.WriteLine("Bootstrapping");
                compiler = vm.Result();
                vm.Clear();
                vm.SetProgram(compiler);
                vm.SetInput(source);
                do op = vm.Step();
                while (vm.Running());

                if (vm.Errored())
                    return;

                Console.WriteLine("Bootstrap Compilation complete");
            }
            Console.WriteLine("Writing compiler object to compiler" + s + ".secd");
            if (vm.Result() == null)
                Console.WriteLine("Not writing null output");
            else
                SaveFile(vm.Result(), "compiler" + s + ".secd");
        }

        public static Lisp LoadFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string txt = sr.ReadToEnd();
            Sexp parser = new Sexp(txt);
            return parser.Read();
        }

        public static void SaveFile(Lisp data, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(fs, Encoding.ASCII, 20000);
            sw.Write(Lisp.ToEscapedString(data));
            sw.Close();
            fs.Close();
        }
    }
}
