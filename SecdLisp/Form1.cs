using System;
using System.Windows.Forms;
using System.IO;

namespace SecdLisp
{
    public partial class Form1 : Form
    {
        VM vm;

        Lisp compiler;

        public Form1()
        {
            InitializeComponent();
            string secdComp = File.ReadAllText("../../../Compilers/compiler7.secd");
            // Read the compiler object
            Sexp sx = new Sexp(secdComp);
            compiler = sx.Read();

            vm = new VM();
        }

        private void Compile(object sender, EventArgs e)
        {
            vm.Clear();
            vm.SetProgram(compiler);

            Sexp src = new Sexp(txtSource.Text);
            Lisp ssrc = src.Read();
            vm.SetInput(new Cons(ssrc, null));
            Run();
            txtObject.Text = vm.Printed() + vm.STop;
        }


        private void RunLisp(object sender, EventArgs e)
        {
            vm.Clear();
            Sexp sx = new Sexp(txtObject.Text);
            Lisp prog = sx.Read();
            vm.SetProgram(prog);
            string argstr = txtArgs.Text.Trim();
            if (argstr != "")
            {
                Sexp args = new Sexp(argstr);
                vm.SetInput(args.Read());
            }
            else
            {
                vm.SetInput(null);
            }
            Run();
            txtResult.Text = vm.Printed() + vm.STop;
        }

        private void Run()
        {
            Op op;
            do
            {
                op = vm.Step();
            } while (vm.Running());
        }
    }
}