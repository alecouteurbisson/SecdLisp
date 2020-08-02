using System;
using System.Collections.Generic;
using System.Text;

namespace SecdLisp
{
    public class Sexp
    {
        string sexp;
        string rest;
        int ptr;

        public Sexp(string s)
        {
            sexp = s + " ";
            ptr = 0;
            eatwhite();
        }

        private char eatwhite()
        {
            while ((sexp[ptr] == ' ') ||
                  (sexp[ptr] == '\r') ||
                  (sexp[ptr] == '\n') ||
                  (sexp[ptr] == '\t') ||
                  (sexp[ptr] == ';'))
            {
                if (sexp[ptr] == ';')
                {
                    while (sexp[ptr] != '\n')
                        ptr++;
                }
                else
                    ptr++;
            }

            rest = sexp.Substring(ptr);
            return sexp[ptr];
        }

        public Lisp Read()
        {
            char c = eatwhite();

            if (c == '$')
                return ReadOpcode();

            if (c == '"')
                return ReadStr();

            if (char.IsLetter(c))
                return ReadSymbol();

            if (char.IsDigit(c))
                return ReadInteger();

            if (c == '(')
            {
                ptr++;
                return ReadList();
            }

            throw new ApplicationException("Bad Read");
        }

        private Lisp ReadOpcode()
        {
            int end = 1;
            while (char.IsLetter(rest[end]))
                end++;
            string name = rest.Substring(1, end - 1);
            ptr += end;
            return new Opcode((Op)Enum.Parse(typeof(Op), name, true));
        }

        private Lisp ReadStr()
        {
            int end = 1;
            while (rest[end] != '"')
                ++end;
            string txt = rest.Substring(1, end - 1);
            ptr += end + 1;
            return new Str(txt);
        }

        private Lisp ReadInteger()
        {
            int end = 1;
            while (char.IsDigit(rest[end]))
                end++;
            string num = rest.Substring(0, end);
            ptr += end;
            return new Integer(int.Parse(num));
        }

        private bool IsSymbolChar(char c)
        {
            return char.IsLetterOrDigit(c) || (c == '_');
        }

        private Lisp ReadSymbol()
        {
            int end = 1;
            while (IsSymbolChar(rest[end]))
                end++;
            string sym = rest.Substring(0, end);
            ptr += end;
            return Symbol.Get(string.Intern(sym));
        }

        private Lisp ReadList()
        {
            Lisp t;

            char c = eatwhite();
            if (c == ')')
            {
                ptr++;
                return null;
            }
            if (c == '.')
            {
                ptr++;
                t = Read();
                if (eatwhite() != ')')
                    throw new ApplicationException("Illegal dotted pair");
                ptr++;
                return t;
            }

            Lisp h = Read();

            t = ReadList();
            return new Cons(h, t);
        }
    }
}
