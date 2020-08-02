using System;
using System.Collections.Generic;
using System.Text;

namespace SecdLisp
{
    public enum Op
    {
        /// <summary>
        /// Push nil
        /// </summary>
        NIL,
        /// <summary>
        /// Load a value
        /// </summary>
        LD,
        /// <summary>
        /// Load a constant
        /// </summary>
        LDC,
        /// <summary>
        /// Load a function
        /// </summary>
        LDF,
        /// <summary>
        /// Store a value
        /// </summary>
        ST,
        /// <summary>
        /// Apply a function
        /// </summary>
        AP,
        /// <summary>
        /// Return
        /// </summary>
        RTN, 
        /// <summary>
        /// Dummy environment for recurive calls
        /// </summary>
        DUM, 
        /// <summary>
        /// Apply recursive function
        /// </summary>
        RAP,
        /// <summary>
        /// Select branch
        /// </summary>
        SEL,
        /// <summary>
        /// Rejoin after alternative branches
        /// </summary>
        JOIN,
        /// <summary>
        /// Cons
        /// </summary>
        CONS,
        /// <summary>
        /// Car
        /// </summary>
        CAR,
        /// <summary>
        /// Cdr
        /// </summary>
        CDR,
        /// <summary>
        /// Is atom predicate
        /// </summary>
        ATOM, 
        /// <summary>
        /// Is int predicate
        /// </summary>
        INT,
        /// <summary>
        /// Is symbol predicate
        /// </summary>
        SYM, 
        /// <summary>
        /// Is string predicate
        /// </summary>
        STR, 
        /// <summary>
        /// Eq predicate
        /// </summary>
        EQ,
        /// <summary>
        /// Eql predicate
        /// </summary>
        EQL,
        /// <summary>
        /// Add integers
        /// </summary>
        ADD,
        /// <summary>
        /// Subtract integers
        /// </summary>
        SUB,
        /// <summary>
        /// Multiply integers
        /// </summary>
        MUL,
        /// <summary>
        /// Divide integers
        /// </summary>
        DIV,
        /// <summary>
        /// Remainder after division
        /// </summary>
        REM,
        /// <summary>
        /// Less or equal
        /// </summary>
        LEQ,
        /// <summary>
        /// Logical and
        /// </summary>
        AND,
        /// <summary>
        /// Logical or
        /// </summary>
        OR,
        /// <summary>
        /// Logical not
        /// </summary>
        NOT,
        /// <summary>
        /// Print value
        /// </summary>
        PRINT,
        /// <summary>
        /// Try to evaluate expression
        /// </summary>
        TRY,
        /// <summary>
        /// Resume on failure
        /// </summary>
        FAIL, 
        /// <summary>
        /// Signal an error condition
        /// </summary>
        ERR,
        /// <summary>
        /// Stop execution
        /// </summary>
        STOP,
        /// <summary>
        /// Test for primitive function
        /// </summary>
        PRIM,
        /// <summary>
        /// Call a primitive function
        /// </summary>
        PCALL
    };


    public class VM
    {
        //public const Lisp nil = null;

        Lisp S;
        Lisp E;
        Lisp C;
        Lisp D;
        Lisp R;

        Op op;
        bool has_arg;
        Lisp arg;

        bool stop;
        bool error;

        StringBuilder prtout;

        public string OpCode { get { return op.ToString() + "  " + (has_arg ? (arg == null ? "NIL" : arg.ToString()) : ""); } }

        Lisp T = Symbol.Get("t");
        Lisp FAIL = Symbol.Get("fail");

        public VM()
        {
            Clear();
        }

        public void Clear()
        {
            S = E = C = D = R = null;
            prtout = new StringBuilder();
        }

        public void SetAll(string s, string e, string c, string d)
        {
            Sexp xp;
            xp = new Sexp(s);
            S = xp.Read();
            xp = new Sexp(e);
            E = xp.Read();
            xp = new Sexp(c);
            C = xp.Read();
            xp = new Sexp(d);
            D = xp.Read();
        }

        public void SetProgram(Lisp prog)
        {
            C = prog;
            stop = false;
            error = false;
        }

        public void SetInput(Lisp val)
        {
            // Push item onto stack
            S = new Cons(val, null);
        }

        public void Push(Lisp x, ref Lisp stack)
        {
            Cons c = new Cons(x, stack);
            stack = c;
        }

        public Lisp Pop(ref Lisp stack)
        {
            Cons c = stack as Cons;
            if (c != null)
            {
                Lisp t = c.Car;
                stack = c.Cdr;
                return t;
            }
            else
            {
                throw new ApplicationException("Can't pop an atom");
            }
        }

        public Lisp Top(Lisp stack)
        {
            Cons c = stack as Cons;
            if (c != null)
                return c.Car;
            else
                throw new ApplicationException("Can't pop an atom");
        }

        public bool Running()
        {
            return !stop;
        }

        public bool Errored()
        {
            return error;
        }

        public string Printed()
        {
            return prtout.ToString();
        }

        public Op Step()
        {
            Lisp instr = Pop(ref C);
            Lisp l, t, f, a1, a2;
            int i1, i2;
            Cons c;

            op = (instr as Opcode).op;
            has_arg = false;

            if (stop)
                return op;

            switch (op)
            {
                case Op.NIL:
                    Push(Lisp.NIL, ref S);
                    break;

                case Op.LD:
                    arg = Top(C);
                    has_arg = true;
                    Load();
                    break;

                case Op.LDC:
                    arg = Top(C);
                    has_arg = true;
                    Push(Pop(ref C), ref S);
                    break;

                case Op.ST:
                    arg = Top(C);
                    has_arg = true;
                    Store();
                    break;

                case Op.LDF:
                    arg = Top(C);
                    has_arg = true;
                    Push(new Cons(Pop(ref C), E), ref S);
                    break;

                case Op.AP:
                    Push(C, ref D);
                    Push(E, ref D);
                    E = Pop(ref S);
                    C = Pop(ref E);
                    Push(Pop(ref S), ref E);
                    Push(S, ref D);
                    S = Lisp.NIL;
                    break;

                case Op.RTN:
                    S = new Cons(Top(S), Pop(ref D));
                    E = Pop(ref D);
                    C = Pop(ref D);
                    break;

                case Op.DUM:
                    E = new ConsC(null, E);
                    break;

                case Op.RAP:
                    Push(C, ref D);
                    Push(Top(E), ref D);
                    E = Pop(ref S);
                    C = Pop(ref E);
                    (E as Cons).Car = Pop(ref S);
                    Push(S, ref D);
                    S = null;
                    break;

                case Op.SEL:
                    l = Pop(ref S);
                    t = Pop(ref C);
                    f = Pop(ref C);
                    Push(C, ref D);
                    C = l == Lisp.NIL ? f : t;
                    break;

                case Op.JOIN:
                    C = Pop(ref D);
                    break;

                case Op.CAR:
                    c = Pop(ref S) as Cons;
                    Push(c.Car, ref S);
                    break;

                case Op.CDR:
                    c = Pop(ref S) as Cons;
                    Push(c.Cdr, ref S);
                    break;

                case Op.ATOM:
                    l = Pop(ref S);
                    Push(Lisp.Bool(!(l is Cons)), ref S);
                    break;

                case Op.INT:
                    l = Pop(ref S);
                    Push(Lisp.Bool(l is Integer), ref S);
                    break;

                case Op.SYM:
                    l = Pop(ref S);
                    Push(Lisp.Bool(l is Symbol), ref S);
                    break;

                case Op.STR:
                    l = Pop(ref S);
                    Push(Lisp.Bool(l is Str), ref S);
                    break;

                case Op.CONS:
                    a1 = Pop(ref S);
                    a2 = Pop(ref S);
                    Push(new Cons(a1, a2), ref S);
                    break;

                case Op.EQ:
                    a2 = Pop(ref S);
                    a1 = Pop(ref S);
                    Push(Lisp.Eq(a1, a2), ref S);
                    break;

                case Op.EQL:
                    a2 = Pop(ref S);
                    a1 = Pop(ref S);
                    Push(Lisp.Eql(a1, a2), ref S);
                    break;

                case Op.ADD:
                    i2 = (Pop(ref S) as Integer).ivalue;
                    i1 = (Pop(ref S) as Integer).ivalue;
                    Push(new Integer(i1 + i2), ref S);
                    break;

                case Op.SUB:
                    i2 = (Pop(ref S) as Integer).ivalue;
                    i1 = (Pop(ref S) as Integer).ivalue;
                    Push(new Integer(i1 - i2), ref S);
                    break;

                case Op.MUL:
                    i2 = (Pop(ref S) as Integer).ivalue;
                    i1 = (Pop(ref S) as Integer).ivalue;
                    Push(new Integer(i1 * i2), ref S);
                    break;

                case Op.DIV:
                    i2 = (Pop(ref S) as Integer).ivalue;
                    i1 = (Pop(ref S) as Integer).ivalue;
                    Push(new Integer(i1 / i2), ref S);
                    break;

                case Op.REM:
                    i2 = (Pop(ref S) as Integer).ivalue;
                    i1 = (Pop(ref S) as Integer).ivalue;
                    Push(new Integer(i1 % i2), ref S);
                    break;

                case Op.LEQ:
                    i2 = (Pop(ref S) as Integer).ivalue;
                    i1 = (Pop(ref S) as Integer).ivalue;
                    Push(Lisp.Bool(i1 <= i2), ref S);
                    break;

                case Op.AND:
                    a2 = (Pop(ref S));
                    a1 = (Pop(ref S));
                    Push(Lisp.Bool((a1 != Lisp.NIL) && (a2 != Lisp.NIL)), ref S);
                    break;

                case Op.OR:
                    a2 = (Pop(ref S));
                    a1 = (Pop(ref S));
                    Push(Lisp.Bool((a1 != Lisp.NIL) || (a2 != Lisp.NIL)), ref S);
                    break;

                case Op.NOT:
                    Push(Lisp.Bool(Pop(ref S) == Lisp.NIL), ref S);
                    break;

                case Op.TRY:
                    a1 = Pop(ref C);  // First try
                    a2 = Pop(ref C);  // Second try
                    Push(C, ref D);   // Push continuation for JOIN
                    C = a1;
                    Push(D, ref R);   // Build Resumption
                    Push(a2, ref R);
                    Push(E, ref R);
                    Push(S, ref R);
                    break;

                case Op.FAIL:
                    if (R == null) // Complete failure
                    {
                        S = new Cons(FAIL, Lisp.NIL);
                        E = D = Lisp.NIL;
                        C = new Cons(new Integer((int)Op.STOP), Lisp.NIL);
                    }
                    else  // Try resuming
                    {
                        S = Pop(ref R);
                        E = Pop(ref R);
                        C = Pop(ref R);
                        D = Pop(ref R);
                    }
                    break;

                case Op.STOP:
                    stop = true;
                    break;

                case Op.PRINT:
                    l = Pop(ref S);
                    c = l as Cons;
                    if (c == null)
                    {
                        prtout.AppendLine(l.ToString());
                        //Console.WriteLine(l);
                    }
                    else
                    {
                        while (c != null)
                        {
                            prtout.Append($"{c.Car} ");
                            //Console.Write(c.Car);
                            //Console.Write(" ");
                            c = c.Cdr as Cons;
                        }
                        prtout.AppendLine();
                        Console.WriteLine();
                    }
                    break;

                case Op.ERR:
                    Console.WriteLine("******ERROR******");
                    l = Pop(ref S);
                    c = l as Cons;
                    if (c == null)
                    {
                        prtout.AppendLine(l.ToString());
                        //Console.WriteLine(l);
                    }
                    else
                    {
                        while (c != null)
                        {
                            prtout.Append($"{c.Car} ");
                            Console.Write(c.Car);
                            Console.Write(" ");
                            c = c.Cdr as Cons;
                        }
                        prtout.AppendLine();
                        Console.WriteLine();
                    }
                    stop = true;
                    error = true;
                    break;

                case Op.PRIM:
                    l = Pop(ref S);
                    a1 = null;
                    if (l is Symbol)
                    {
                        l = (l as Symbol).GlobalValue;
                        if (l is Primitive)
                            a1 = new Integer((l as Primitive).id);
                    }
                    Push(a1, ref S);  // return nil or primitive id
                    break;
                /*
                        case Op.PCALL:
                          l = Pop(ref S);
                          arg = Top(C);
                          Push(Primitives.Call(arg, l), S);
                          break;
                */
                default:
                    throw new ApplicationException("Unknown Opcode");
            }
            return op;
        }

        private void Load()
        {
            Lisp source = Pop(ref C);
            if (source is Cons)
            {
                Cons c = source as Cons;
                Cons l = E as Cons;
                for (int i = (c.Car as Integer).ivalue; i > 0; i--)
                    l = l.Cdr as Cons;
                l = l.Car as Cons;
                for (int i = (c.Cdr as Integer).ivalue; i > 0; i--)
                    l = l.Cdr as Cons;
                Push(l.Car, ref S);
            }
            else if (source is Symbol)
            {
                Push((source as Symbol).GlobalValue, ref S);
            }
            else
            {
                throw new ApplicationException("Illegal load source");
            }
        }

        private void Store()
        {
            Lisp target = Pop(ref C);
            Lisp val = Pop(ref S);
            // Push(null, ref S);   // is this needed?
            if (target is Cons)
            {
                Cons c = target as Cons;
                Cons l = E as Cons;
                for (int i = (c.Car as Integer).ivalue; i > 0; i--)
                    l = l.Cdr as Cons;
                l = l.Car as Cons;
                for (int i = (c.Cdr as Integer).ivalue; i > 0; i--)
                    l = l.Cdr as Cons;
                l.Car = val;
            }
            else if (target is Symbol)
            {
                (target as Symbol).GlobalValue = val;
            }
            else
            {
                throw new ApplicationException("Illegal store target");
            }
        }

        public string Sval { get { return S == null ? "NIL" : S.ToString(); } }
        public string Eval { get { return E == null ? "NIL" : E.ToString(); } }
        public string Cval { get { return C == null ? "NIL" : C.ToString(); } }
        public string Dval { get { return D == null ? "NIL" : D.ToString(); } }

        public string STop
        {
            get
            {
                Cons c = S as Cons;
                if (c == null)
                    return "Empty";
                else
                    return (c.Car == Lisp.NIL) ? "nil" : c.Car.ToString();
            }
        }

        public Lisp Result()
        {
            Cons c = S as Cons;
            if (c == null)
                return Lisp.NIL;
            else
                return c.Car;
        }


        public void Show()
        {
            Console.WriteLine("S = {0}", S);
            Console.WriteLine("E = {0}", E);
            Console.WriteLine("C = {0}", C);
            Console.WriteLine("D = {0}", D);
            Console.WriteLine("----------------------------------------------------------");
        }
    }
}
