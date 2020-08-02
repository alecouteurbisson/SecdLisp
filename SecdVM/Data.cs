using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace SecdLisp
{
    public abstract class Lisp
    {
        protected static bool escaped = false;
        public static Symbol T;
        public static Symbol NIL = null;
        public static Symbol UNDEFINED;

        public static string ToEscapedString(Lisp l)
        {
            escaped = true;
            string s = l.ToString();
            escaped = false;
            return s;
        }

        public static Lisp Eq(Lisp a, Lisp b)
        {
            return Bool(ReferenceEquals(a, b));
        }

        public static Lisp Eql(Lisp a, Lisp b)
        {
            if (ReferenceEquals(a, b))
                return Lisp.T;
            if ((a == null) || (b == null))
                return Lisp.NIL;
            return a.Eql(b);
        }

        protected abstract Lisp Eql(Lisp l);

        public static Lisp Bool(bool b)
        {
            return b ? T : NIL;
        }

        public virtual string ToEscapedString()
        {
            return ToString();
        }
    }

    public class Symbol : Lisp
    {
        readonly string name;
        private static Dictionary<string, Symbol> symbols;
        private static string nil;
        private Lisp globalValue;

        static Symbol()
        {
            symbols = new Dictionary<string, Symbol>();
            nil = string.Intern("nil");
            T = Symbol.Get("t");
            T.GlobalValue = T;
            UNDEFINED = Symbol.Get("undefined");
        }

        private Symbol(string name)
        {
            this.name = name;
            symbols.Add(name, this);
            globalValue = UNDEFINED;
        }

        public static Symbol Get(string name)
        {
            Symbol sym = null;
            if (name == nil)
                return sym;
            symbols.TryGetValue(name, out sym);
            if (sym == null)
                return new Symbol(name);
            else
                return sym;
        }

        public Lisp GlobalValue
        {
            get { return globalValue; }
            set { globalValue = value; }
        }

        protected override Lisp Eql(Lisp l)
        {
            if (l is Symbol)
                return Bool((l as Symbol).name == name);
            else
                return NIL;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class Integer : Lisp
    {
        public Int32 ivalue;

        public Integer(Int32 i)
        {
            ivalue = i;
        }

        protected override Lisp Eql(Lisp l)
        {
            if (l is Integer)
                return Bool((l as Integer).ivalue == ivalue);
            else
                return NIL;
        }

        public override string ToString()
        {
            return ivalue.ToString();
        }
    }

    public class Str : Lisp
    {
        public string svalue;

        public Str(string s)
        {
            svalue = s;
        }

        protected override Lisp Eql(Lisp l)
        {
            if (l is Str)
                return Bool((l as Str).svalue == svalue);
            else
                return NIL;
        }

        public override string ToString()
        {
            if (escaped)
                return "\"" + svalue + "\"";
            else
                return svalue;
        }
    }

    public class Primitive : Lisp
    {
        public int id;

        public Primitive(int id)
        {
            this.id = id;
        }

        protected override Lisp Eql(Lisp l)
        {
            if (l is Primitive)
                return Bool((l as Primitive).id == id);
            else
                return NIL;
        }

        public override string ToString()
        {
            return "#<primitive_" + id.ToString() + ">";
        }
    }

    public class Opcode : Lisp
    {
        public Op op;

        public Opcode(Op o)
        {
            op = o;
        }

        protected override Lisp Eql(Lisp l)
        {
            if (l is Opcode)
                return Bool((l as Opcode).op == op);
            else
                return NIL;
        }

        public override string ToString()
        {
            return "$" + op.ToString();
        }
    }

    public class Cons : Lisp
    {
        Lisp car;
        Lisp cdr;

        public Cons(Lisp a, Lisp d)
        {
            car = a;
            cdr = d;
        }
        public Lisp Car { get { return car; } set { car = value; } }
        public Lisp Cdr { get { return cdr; } }

        protected override Lisp Eql(Lisp l)
        {
            return Eq(this, l);
        }

        // Cons to string
        public override string ToString()
        {
            if (car == null)
                return "(nil" + CdrToString(cdr);

            return "(" + car.ToString() + CdrToString(cdr);
        }

        // cdr to string
        private string CdrToString(Lisp l)
        {
            if (l == null)
                return ")";

            if (l is ConsC)  // Don't chase circular references
                return "@)";

            if (l is Cons)
            {
                Cons lc = l as Cons;
                if (lc.Car == null)
                    return " nil" + CdrToString(lc.cdr);
                else
                    return " " + lc.car.ToString() + CdrToString(lc.cdr);
            }
            // atomic cdr
            return " . " + l.ToString() + ")";
        }
    }

    // This is a special Cons that denotes a circular reference
    public class ConsC : Cons
    {
        public ConsC(Lisp a, Lisp d)
          : base(a, d)
        { }
    }
}
