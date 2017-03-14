using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace error_ha
{
    
        enum ErrorKind
    {
        primary,
        logical,
        relation,
        factor,
        term,
        ineger,
        oper,
        //parser,


    }
    [Serializable]
    class ErrorIn : System.Exception
    {
        public ErrorIn(ErrorKind ek) : base(String.Format("error in parsing: {0}", ek.ToString()))
        {
  
        }
    }

    [Serializable]
    class RightExprError : System.Exception
    {
        public RightExprError(String str) : base(String.Format(str))
        {

        }
    }
    [Serializable]
    class LeftExprError : System.Exception
    {
        public LeftExprError(String str) : base(String.Format(str))
        {

        }
    }
    class InvalidCharacter : System.Exception
    {
        public InvalidCharacter(char ch) : base(String.Format(string.Concat("Invalid character ", ch.ToString())))
        {

        }
    }



    abstract class Expression
        {
            protected long value;
            protected bool b_val = false;
            protected bool b_set = false;
            protected Expression left, right;
            public abstract long calculate();
            //public Expression(long v)
            //{
            //  value = v;
            //}
            //public abstract long get();
            //{
            //  return value;
            //}
            public virtual String toJSON()
            {
                if (this.left == null)
            {
                throw (new LeftExprError("Cannot serialize invalid characters at left"));
            }
                String left = this.left.toJSON();
            if (this.right == null)
            {
                throw (new RightExprError("Cannot serialize invalid charactrs at right"));
            }
                String right = this.right.toJSON();
                //String left = get_left().toJSON();

                StringBuilder str = new StringBuilder();
                str.Append("{").Append("\"operator\" : ").Append("\"").Append(get_operator()).Append("\", ").Append("\"left\" : ").Append(left).Append(", ").Append("\"right\" : ").Append(right).Append("}");

                return str.ToString();
            }
            public bool result_bool()
            {
                return b_val;
            }
            public void set(long v)
            {
                value = v;
            }
            public abstract bool get_bool();
            //        {

            //  return b_val;
            //}
            protected abstract String get_operator();
            protected void set_bool()
            {
                b_set = true;
            }

        }
        class Logical : Expression
        {
            public enum Opcode { and, or, xor, none }
            public Opcode op;
            public Logical(Opcode o, Expression r, Expression rig)
            {
                bool r1 = r.get_bool();
                bool r2 = rig.get_bool();
                this.left = r;
                this.right = rig;
                this.op = o;
                set_bool();
                if (op == Logical.Opcode.and)
                {
                    b_val = (r1 && r2);
                }
                if (op == Logical.Opcode.or)
                {
                    b_val = (r1 || r2);
                }
                if (op == Logical.Opcode.xor)
                {
                    b_val = (r1 ^ r2);
                }
            }
            public override long calculate()
            {
            long left = this.left.calculate();
            long right = this.right.calculate();
            if (op == Opcode.and)
            {
                if (left > 0 && right > 0)
                    return 1;
                else
                    return 0;
            }
            else
            if (op == Opcode.or)
            {
                if (left > 0 || right > 0)
                    return 1;
                else
                    return 0;
            }
            else
            if (op == Opcode.xor)
            {
                if ((left > 0 && right == 0) || (left == 0 && right > 0))
                    return 1;
                else
                    return 0;
            }
            else
                return 0;
       

            }
            protected override String get_operator()
            {
                if (op == Opcode.and)
                    return "&";
                else if (op == Opcode.or)
                    return "|";
                else if (op == Opcode.xor)
                    return "^";
                else if (op == Opcode.none)
                    return null;
                return null;
            }
            public override bool get_bool()
            {
                return b_val;
            }
            public void add_rel(Logical.Opcode op, Expression a)
            {
                bool r1 = a.get_bool();
                if (op == Logical.Opcode.and)
                {
                    b_val = (b_val && r1);
                }
                if (op == Logical.Opcode.or)
                {
                    b_val = (b_val || r1);
                }
                if (op == Logical.Opcode.xor)
                {
                    b_val = (b_val ^ r1);
                }
            }
        }

        class Relation : Expression
        {
            public enum Opcode { less, less_eq, greater, greater_eq, equal, not_equal, none }
            public Opcode op;
           
            public Relation(Opcode op, Expression a, Expression b)
            {
                //double v1 = a.get();
                //double v2 = b.get();
                this.left = a;
                this.right = b;
                this.op = op;
                set_bool();

                
            }
            public override long calculate()
            {
            /*if (this.left == null)
            {
                throw (new LeftExprError("left expression is null"));
            }
            */
                long left = this.left.calculate();
            /*if (this.right == null)
            {
                throw (new RightExprError("Invalid characters in the right part, after operand"));
            }
            */
                long right = this.right.calculate();
                if (op == Opcode.less)
                {
                    if (left < right)
                        return 1;
                    else
                        return 0;
                }
                else
                    if (op == Opcode.less_eq)
                {
                    if (left <= right)
                        return 1;
                    else
                        return 0;
                }
                else
                    if (op == Opcode.equal)
                {
                    if (left == right)
                        return 1;
                    else
                        return 0;
                }
                else
                    if (op == Opcode.not_equal)
                {
                    if (left != right)
                        return 1;
                    else
                        return 0;
                }
                else
                    if (op == Opcode.greater)
                {
                    if (left > right)
                        return 1;
                    else
                        return 0;
                }
                else
                    if (op == Opcode.greater_eq)
                {
                    if (left >= right)
                        return 1;
                    else
                        return 0;
                }
                else
                    return 0;
            }
            protected override string get_operator()
            {
                if (op == Opcode.equal)
                    return "=";
                else if (op == Opcode.greater)
                    return ">";
                else if (op == Opcode.greater_eq)
                    return ">=";
                else if (op == Opcode.less)
                    return "<";
                else if (op == Opcode.less_eq)
                    return "<=";
                else if (op == Opcode.none)
                    return null;
                else if (op == Opcode.not_equal)
                    return "/=";
                return null;
            }
            public override bool get_bool()
            {
                return b_val;
            }
        }
        abstract class Primary : Expression
        {
        }
        class Factor : Expression
        {
            public enum Opcode { times, devided, none }
            public Opcode op;
        

        public Factor(Opcode op, Expression a, Expression b)
        {
            
            this.op = op;
            this.left = a;
            this.right = b;
        }  
            public override long calculate()
            {
                long left = this.left.calculate();
                long right = this.right.calculate();
                if (op == Opcode.devided)
                {
                    return left / right;
                }
                else
                    if (op == Opcode.times)
                {
                    return left * right;
                }
                else
                    return 0;
            }
            public override bool get_bool()
            {
                return b_val;
            }
            protected override string get_operator()
            {
                if (op == Opcode.devided)
                    return "/";
                else if (op == Opcode.times)
                    return "*";
                else if (op == Opcode.none)
                    return null;
                return null;
            }

        }
        class Term : Expression
        {
            public enum Opcode { plus, minus, none }
            public Opcode op;
    
            public Term(Opcode op, Expression a, Expression b)
            {
                this.op = op;
                this.left = a;
                this.right = b;
                
            }
            public void add_fact(Term.Opcode op, Expression a)
            {
                if (op == Term.Opcode.plus)
                {
                    value += a.calculate();
                }
                else if (op == Term.Opcode.minus)
                {
                    value -= a.calculate();
                }
            }
            public override long calculate()
            {

                if (this.left == null)
            {
                throw (new LeftExprError("In term null reference, right part consist of invalid characters"));
            }
                long left = this.left.calculate();
            /*
             * 
             * 
             * 
             * here null reference exception when right part is incorrect 5++5
               4+b
             
             */
             if (this.right == null)
            {
                throw (new RightExprError("In term right part consists of invalid characters"));
            }
            long right = this.right.calculate();
           
                if (op == Opcode.plus)
                {
                    return left + right;
                }
                else
                if (op == Opcode.minus)
                {
                    return left - right;
                }
                else
                    return 0;

            }
            public override bool get_bool()
            {
                return b_val;
            }
            protected override string get_operator()
            {
                if (op == Opcode.minus)
                    return "-";
                else if (op == Opcode.plus)
                    return "+";
                else if (op == Opcode.none)
                    return null;
                return null;
            }
        }
        class Integer : Primary
        {
            private long value;

            public Integer()
            {
            }
            public Integer(long v)
            {
                this.value = v;
            }
            public void set(long v)
            {
                this.value = v;
            }
           
            public override long calculate()
            {
                return value;
            }
            public override bool get_bool()
            {
                return b_val;
            }
            protected override string get_operator()
            {
                return null;
            }
            public override string toJSON()
            {
                return value + "";
            }
        }
        class Parser
        {
            private string input;
            private int pos;
            public Parser(string s)
            {
                pos = 0;
                input = s;
                input.Replace("\\s+", "");
            }
            public Expression parse()
            {
                return parseLogical();
            }
            private Expression parseLogical()
            {
                Expression result = parseRelation();
                //Expression log = null;
                while (true)
                {
                    Logical.Opcode op = parseLogOperator();
                    if (op != Logical.Opcode.none)
                    {
           
                        Expression right = parseRelation();
                       
                        result = new Logical(op, result, right);
                     
                        continue;
                        
                    }
                    else
                        break;
                }
             
                return result;
          
            }
            private Expression parseRelation()
            {
                Expression result = parseTerm();

                while (true)
                {






                Relation.Opcode op = parseRelOperator();
                    if (op != Relation.Opcode.none)
                    {
                        Expression right = parseTerm();
                        result = new Relation(op, result, right);
                    }
                    else
                        break;
                }
                return result;
            }
            private Expression parseTerm()
            {
                Expression result = parseFactor();
                while (true)
                {
                    Term.Opcode op = parseTermOperator();
                    if (op != Term.Opcode.none)
                    {
             
                        Expression right = parseFactor();
                        result = new Term(op, result, right);
                    }
                    else
                        break;
                }
                return result;
            }
            private Expression parseFactor()
            {
                Expression result = parsePrimary();
                while (true)
                {
                    Factor.Opcode op = parseFactorOperator();
                    if (op != Factor.Opcode.none)
                    {
                        Expression right = parsePrimary();
                        //if (fact == null)
                        //{
                        result = new Factor(op, result, right);
                        //}
                        //else
                        //  fact = new Factor(op, fact, right);
                        //continue;
                    }
                    else
                        break;
                }
                //if (fact == null)
                return result;
                //return fact;
            }
            private Expression parsePrimary()
            {
                char n = nextChar();
                if (n.CompareTo('#') == 0)
                    return null;
                Expression result = null;
                if (Char.IsDigit(n))
                    result = parseInteger();
                else if (n.CompareTo('(') == 0)
                {
                    skipNextChar();
                    result = parse();
                    skipNextChar();
                    //return new Primary(expression.calculate());
                }
                else
                {
                throw (new InvalidCharacter(n));

            } //error;
                return result;
            }
            private Integer parseInteger()
            {
                Integer result = new Integer();
                while ((nextChar().CompareTo('#') != 0) && Char.IsDigit(nextChar()))
                {
                    long k = result.calculate() * 10 + (skipNextChar() - '0');
                    result.set(k);
                }
                return result;
            }
            private Logical.Opcode parseLogOperator()
            {
                Char op = nextChar();
                if ((op.CompareTo('#') == 0) || ((op.CompareTo('|') != 0) && (op.CompareTo('^') != 0) && (op.CompareTo('&') != 0)))
                    return Logical.Opcode.none;
                op = skipNextChar();
                if (op.CompareTo('|') == 0)
                    return Logical.Opcode.or;
                if (op.CompareTo('^') == 0)
                    return Logical.Opcode.xor;
                if (op.CompareTo('&') == 0)
                    return Logical.Opcode.and;
                return Logical.Opcode.none;
            }
            private Relation.Opcode parseRelOperator()


        {
                StringBuilder oper = new StringBuilder();
                Char op = nextChar();
                if ((op.CompareTo('#') == 0) || ((op.CompareTo('>') != 0) && (op.CompareTo('<') != 0) && (op.CompareTo('=') != 0) && (op.CompareTo('/') != 0)))
                {
                    return Relation.Opcode.none;
                }
                op = skipNextChar();
                oper.Append(op);
                op = nextChar();
                if ((op.CompareTo('#') != 0) && (op.CompareTo('=') == 0))
                {
                    op = skipNextChar();
                    oper.Append(op);
                }
                String oper_complete = oper.ToString();
                if (oper_complete.CompareTo("=") == 0)
                {
                    return Relation.Opcode.equal;
                }
                if (oper_complete.CompareTo("<=") == 0)
                {
                    return Relation.Opcode.less_eq;
                }
                if (oper_complete.CompareTo("<") == 0)
                {
                    return Relation.Opcode.less;
                }
                if (oper_complete.CompareTo("/=") == 0)
                {
                    return Relation.Opcode.not_equal;
                }
                if (oper_complete.CompareTo("=>") == 0)
                {
                    return Relation.Opcode.greater_eq;
                }
                if (oper_complete.CompareTo(">") == 0)
                {
                    return Relation.Opcode.greater;
                }
                return Relation.Opcode.none;
            }
            private Term.Opcode parseTermOperator()
            {
                Char op = nextChar();
                if ((op.CompareTo('#') == 0) || ((op.CompareTo('+') != 0) && (op.CompareTo('-') != 0)))
                    return Term.Opcode.none;
                op = skipNextChar();
                if (op.CompareTo('+') == 0)
                    return Term.Opcode.plus;
                if (op.CompareTo('-') == 0)
                    return Term.Opcode.minus;
                return Term.Opcode.none;
            }
            private Factor.Opcode parseFactorOperator()
            {
                Char op = nextChar();
                if ((op.CompareTo('#') == 0) || ((op.CompareTo('/') != 0) && (op.CompareTo('*') != 0)))
                    return Factor.Opcode.none;
                op = skipNextChar();
                Char nextChar1 = nextChar();
                if ((nextChar1.CompareTo('#') != 0) && (nextChar1.CompareTo('=') == 0))
                {
                    pos--;
                    return Factor.Opcode.none;
                }
                if (op.CompareTo('/') == 0)
                {
                    return Factor.Opcode.devided;
                }
                if (op.CompareTo('*') == 0)
                {
                    return Factor.Opcode.times;
                }
                return Factor.Opcode.none;

            }
            private Char skipNextChar()
            {
                char b = '#';
                if (pos == input.Length)
                    return b;
                else
                    return input[pos++];




        }
            private Char nextChar()
            {
                char b = '#';
                if (pos == input.Length)
                    return b;
                else
                    return input[pos];
            }
        }
        class Program
        {
            static void Main(string[] args)
            {
                Console.Write("Type the arichmetic expression\n");

                string input = Console.ReadLine();
            try
            {
                Parser parser = new Parser(input);
                Expression expressionTree = parser.parse();
                Console.Write("hi");
                Console.Write("Tree is build hope");
                if (expressionTree != null)
                {
                    if (expressionTree.result_bool())
                    {
                        Console.Write("Tree is build hope");
                        Console.Write(expressionTree.get_bool());
                    }
                    else
                        Console.Write(expressionTree.calculate());
                }
                /*
                 * 
                 * error invalid character
                   a+b 
             */
                Console.Write(expressionTree.toJSON());
            }
            catch(RightExprError r)
            {
                Console.WriteLine("RightExprError: {0}", r.Message);
            }
            catch (LeftExprError r)
            {
                Console.WriteLine("LeftExprError: {0}", r.Message);
            }
            catch (ErrorIn r)
            {
                Console.WriteLine("ErrorIn: {0}", r.Message);
            }
            catch (InvalidCharacter r)
            {
                Console.WriteLine("InvalidCharacter: {0}", r.Message);
            }
            Console.ReadLine();
            }
        }

    }

