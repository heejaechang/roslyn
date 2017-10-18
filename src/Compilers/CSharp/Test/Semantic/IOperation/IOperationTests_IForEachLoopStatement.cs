// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_SimpleForEachLoop()
        {
            string source = @"
class Program
{
    static void Main()
    {
        string[] pets = { ""dog"", ""cat"", ""bird"" };

        /*<bind>*/foreach (string value in pets)
        {
            System.Console.WriteLine(value);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (st ... }') (Parent: BlockStatement)
  Locals: Local_1: System.String value
  LoopControlVariable: 
    ILocalReferenceExpression: value (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.String, Constant: null) (Syntax: ForEachStatement, 'foreach (st ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'pets')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: pets ([0] OperationKind.LocalReferenceExpression, Type: System.String[]) (Syntax: IdentifierName, 'pets')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... ine(value);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... Line(value)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'value')
                ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'value')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithList()
        {
            string source = @"
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        List<string> list = new List<string>();
        list.Add(""a"");
        list.Add(""b"");
        list.Add(""c"");
        /*<bind>*/foreach (string item in list)
        {
            Console.WriteLine(item);
        }/*</bind>*/

    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([4] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (st ... }') (Parent: BlockStatement)
  Locals: Local_1: System.String item
  LoopControlVariable: 
    ILocalReferenceExpression: item (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.String, Constant: null) (Syntax: ForEachStatement, 'foreach (st ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.List<System.String>, IsImplicit) (Syntax: IdentifierName, 'list')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: list ([0] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.List<System.String>) (Syntax: IdentifierName, 'list')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(item);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(item)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'item')
                ILocalReferenceExpression: item ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'item')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithKeyValue()
        {
            string source = @"
using System;
using System.Collections.Generic;

class Program
{
    static Dictionary<int, int> _h = new Dictionary<int, int>();

    static void Main()
    {
        _h.Add(5, 4);
        _h.Add(4, 3);
        _h.Add(2, 1);

        /*<bind>*/foreach (KeyValuePair<int, int> pair in _h)
        {
            Console.WriteLine(""{0},{1}"", pair.Key, pair.Value);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([3] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (Ke ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Collections.Generic.KeyValuePair<System.Int32, System.Int32> pair
  LoopControlVariable: 
    ILocalReferenceExpression: pair (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>, Constant: null) (Syntax: ForEachStatement, 'foreach (Ke ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>, IsImplicit) (Syntax: IdentifierName, '_h')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IFieldReferenceExpression: System.Collections.Generic.Dictionary<System.Int32, System.Int32> Program._h (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>) (Syntax: IdentifierName, '_h')
          Instance Receiver: 
            null
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... air.Value);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String format, System.Object arg0, System.Object arg1)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... pair.Value)')
            Instance Receiver: 
              null
            Arguments(3):
              IArgument (ArgumentKind.Explicit, Matching Parameter: format) ([0] OperationKind.Argument) (Syntax: Argument, '""{0},{1}""')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""{0},{1}"") (Syntax: StringLiteralExpression, '""{0},{1}""')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg0) ([1] OperationKind.Argument) (Syntax: Argument, 'pair.Key')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'pair.Key')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    IPropertyReferenceExpression: System.Int32 System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>.Key { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'pair.Key')
                      Instance Receiver: 
                        ILocalReferenceExpression: pair ([0] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>) (Syntax: IdentifierName, 'pair')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg1) ([2] OperationKind.Argument) (Syntax: Argument, 'pair.Value')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'pair.Value')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    IPropertyReferenceExpression: System.Int32 System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>.Value { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'pair.Value')
                      Instance Receiver: 
                        ILocalReferenceExpression: pair ([0] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>) (Syntax: IdentifierName, 'pair')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithBreak()
        {
            string source = @"
class Program
{
    static void Main()
    {
        int[] numbers = { 1,2,3,4};

        /*<bind>*/foreach (int num in numbers)
        {
            if (num>3)
            {
                break;
            }
            System.Console.WriteLine(num);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (in ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 num
  LoopControlVariable: 
    ILocalReferenceExpression: num (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: null) (Syntax: ForEachStatement, 'foreach (in ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'numbers')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: numbers ([0] OperationKind.LocalReferenceExpression, Type: System.Int32[]) (Syntax: IdentifierName, 'numbers')
  Body: 
    IBlockStatement (2 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if (num>3) ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'num>3')
            Left: 
              ILocalReferenceExpression: num ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'num')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IBranchStatement (BranchKind.Break) ([0] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... eLine(num);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... teLine(num)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'num')
                ILocalReferenceExpression: num ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'num')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithContinue()
        {
            string source = @"
class Program
{
    static void Main()
    {
        int[] numbers = { 1,2,3,4};

        /*<bind>*/foreach (int num in numbers)
        {
            if (num>3)
            {
                continue;
            }
            System.Console.WriteLine(num);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (in ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 num
  LoopControlVariable: 
    ILocalReferenceExpression: num (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: null) (Syntax: ForEachStatement, 'foreach (in ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'numbers')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: numbers ([0] OperationKind.LocalReferenceExpression, Type: System.Int32[]) (Syntax: IdentifierName, 'numbers')
  Body: 
    IBlockStatement (2 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if (num>3) ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'num>3')
            Left: 
              ILocalReferenceExpression: num ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'num')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IBranchStatement (BranchKind.Continue) ([0] OperationKind.BranchStatement) (Syntax: ContinueStatement, 'continue;')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... eLine(num);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... teLine(num)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'num')
                ILocalReferenceExpression: num ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'num')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_QueryExpression()
        {
            string source = @"
class Program
{
    static void Main()
    {
        string[] letters = { ""d"", ""c"", ""a"", ""b"" };
        var sorted = from letter in letters
                     orderby letter
                     select letter;
        /*<bind>*/foreach (string value in sorted)
        {
            System.Console.WriteLine(value);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([2] OperationKind.LoopStatement, IsInvalid) (Syntax: ForEachStatement, 'foreach (st ... }') (Parent: BlockStatement)
  Locals: Local_1: System.String value
  LoopControlVariable: 
    ILocalReferenceExpression: value (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.String, Constant: null, IsInvalid) (Syntax: ForEachStatement, 'foreach (st ... }')
  Collection: 
    ILocalReferenceExpression: sorted ([0] OperationKind.LocalReferenceExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'sorted')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... ine(value);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... Line(value)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'value')
                ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'value')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_Struct()
        {
            string source = @"
using System.Reflection;

namespace DisplayStructContentsTest
{
    class Program
    {

        struct Employee
        {
            public string name;
            public int age;
            public string location;
        };

        static void Main(string[] args)
        {
            Employee employee;

            employee.name = ""name1"";
            employee.age = 35;
            employee.location = ""loc"";

            /*<bind>*/foreach (FieldInfo fi in employee.GetType().GetFields())
            {
                System.Console.WriteLine(fi.Name + "" = "" +
                System.Convert.ToString(fi.GetValue(employee)));
            }/*</bind>*/

        }
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([4] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (Fi ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Reflection.FieldInfo fi
  LoopControlVariable: 
    ILocalReferenceExpression: fi (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Reflection.FieldInfo, Constant: null) (Syntax: ForEachStatement, 'foreach (Fi ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: InvocationExpression, 'employee.Ge ... GetFields()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Reflection.FieldInfo[] System.Type.GetFields()) ([0] OperationKind.InvocationExpression, Type: System.Reflection.FieldInfo[]) (Syntax: InvocationExpression, 'employee.Ge ... GetFields()')
          Instance Receiver: 
            IInvocationExpression ( System.Type System.Object.GetType()) ([0] OperationKind.InvocationExpression, Type: System.Type) (Syntax: InvocationExpression, 'employee.GetType()')
              Instance Receiver: 
                ILocalReferenceExpression: employee ([0] OperationKind.LocalReferenceExpression, Type: DisplayStructContentsTest.Program.Employee) (Syntax: IdentifierName, 'employee')
              Arguments(0)
          Arguments(0)
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... mployee)));')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... employee)))')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'fi.Name + "" ... (employee))')
                IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: AddExpression, 'fi.Name + "" ... (employee))')
                  Left: 
                    IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: AddExpression, 'fi.Name + "" = ""')
                      Left: 
                        IPropertyReferenceExpression: System.String System.Reflection.MemberInfo.Name { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'fi.Name')
                          Instance Receiver: 
                            ILocalReferenceExpression: fi ([0] OperationKind.LocalReferenceExpression, Type: System.Reflection.FieldInfo) (Syntax: IdentifierName, 'fi')
                      Right: 
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: "" = "") (Syntax: StringLiteralExpression, '"" = ""')
                  Right: 
                    IInvocationExpression (System.String System.Convert.ToString(System.Object value)) ([1] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'System.Conv ... (employee))')
                      Instance Receiver: 
                        null
                      Arguments(1):
                        IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'fi.GetValue(employee)')
                          IInvocationExpression (virtual System.Object System.Reflection.FieldInfo.GetValue(System.Object obj)) ([0] OperationKind.InvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'fi.GetValue(employee)')
                            Instance Receiver: 
                              ILocalReferenceExpression: fi ([0] OperationKind.LocalReferenceExpression, Type: System.Reflection.FieldInfo) (Syntax: IdentifierName, 'fi')
                            Arguments(1):
                              IArgument (ArgumentKind.Explicit, Matching Parameter: obj) ([1] OperationKind.Argument) (Syntax: Argument, 'employee')
                                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'employee')
                                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                  Operand: 
                                    ILocalReferenceExpression: employee ([0] OperationKind.LocalReferenceExpression, Type: DisplayStructContentsTest.Program.Employee) (Syntax: IdentifierName, 'employee')
                                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_String()
        {
            string source = @"
class Class1
{
    public void M()
    {
        const string s = """";
        /*<bind>*/foreach (char c in s)
        {
            System.Console.WriteLine(c);
        }/*</bind>*/

    }
}

";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (ch ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Char c
  LoopControlVariable: 
    ILocalReferenceExpression: c (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Char, Constant: null) (Syntax: ForEachStatement, 'foreach (ch ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 's')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.String, Constant: """") (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(c);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Char value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(c)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c')
                ILocalReferenceExpression: c ([0] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'c')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithVar()
        {
            string source = @"
using System.Collections.Generic;
class Program
{
    static Dictionary<int, int> _f = new Dictionary<int, int>();

    static void Main()
    {
        _f.Add(1, 2);
        _f.Add(2, 3);
        _f.Add(3, 4);

        /*<bind>*/foreach (var pair in _f)
        {
            System.Console.WriteLine(""{0},{1}"", pair.Key, pair.Value);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([3] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (va ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Collections.Generic.KeyValuePair<System.Int32, System.Int32> pair
  LoopControlVariable: 
    ILocalReferenceExpression: pair (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>, Constant: null) (Syntax: ForEachStatement, 'foreach (va ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>, IsImplicit) (Syntax: IdentifierName, '_f')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IFieldReferenceExpression: System.Collections.Generic.Dictionary<System.Int32, System.Int32> Program._f (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>) (Syntax: IdentifierName, '_f')
          Instance Receiver: 
            null
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... air.Value);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String format, System.Object arg0, System.Object arg1)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... pair.Value)')
            Instance Receiver: 
              null
            Arguments(3):
              IArgument (ArgumentKind.Explicit, Matching Parameter: format) ([0] OperationKind.Argument) (Syntax: Argument, '""{0},{1}""')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""{0},{1}"") (Syntax: StringLiteralExpression, '""{0},{1}""')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg0) ([1] OperationKind.Argument) (Syntax: Argument, 'pair.Key')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'pair.Key')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    IPropertyReferenceExpression: System.Int32 System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>.Key { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'pair.Key')
                      Instance Receiver: 
                        ILocalReferenceExpression: pair ([0] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>) (Syntax: IdentifierName, 'pair')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg1) ([2] OperationKind.Argument) (Syntax: Argument, 'pair.Value')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'pair.Value')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    IPropertyReferenceExpression: System.Int32 System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>.Value { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'pair.Value')
                      Instance Receiver: 
                        ILocalReferenceExpression: pair ([0] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.KeyValuePair<System.Int32, System.Int32>) (Syntax: IdentifierName, 'pair')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_BadElementType()
        {
            string source = @"
class C
{
    static void Main()
    {
        System.Collections.IEnumerable sequence = null;
        /*<bind>*/foreach (MissingType x in sequence)
        {
            bool b = !x.Equals(null);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement, IsInvalid) (Syntax: ForEachStatement, 'foreach (Mi ... }') (Parent: BlockStatement)
  Locals: Local_1: MissingType x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: MissingType, Constant: null, IsInvalid) (Syntax: ForEachStatement, 'foreach (Mi ... }')
  Collection: 
    ILocalReferenceExpression: sequence ([0] OperationKind.LocalReferenceExpression, Type: System.Collections.IEnumerable) (Syntax: IdentifierName, 'sequence')
  Body: 
    IBlockStatement (1 statements, 1 locals) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Boolean b
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'bool b = !x ... uals(null);')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'b = !x.Equals(null)')
          Variables: Local_1: System.Boolean b
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= !x.Equals(null)')
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsImplicit) (Syntax: LogicalNotExpression, '!x.Equals(null)')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  IUnaryOperatorExpression (UnaryOperatorKind.Not) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object) (Syntax: LogicalNotExpression, '!x.Equals(null)')
                    Operand: 
                      IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?) (Syntax: InvocationExpression, 'x.Equals(null)')
                        Children(2):
                          IOperation:  ([0] OperationKind.None) (Syntax: SimpleMemberAccessExpression, 'x.Equals')
                            Children(1):
                              ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: MissingType) (Syntax: IdentifierName, 'x')
                          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_NullLiteralCollection()
        {
            string source = @"
class C
{
    static void Main()
    {
        /*<bind>*/foreach (int x in null)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForEachStatement, 'foreach (in ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: null, IsInvalid) (Syntax: ForEachStatement, 'foreach (in ... }')
  Collection: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null, IsInvalid) (Syntax: NullLiteralExpression, 'null')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_NoElementCollection()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        /*<bind>*/foreach (int x in args)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForEachStatement, 'foreach (in ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: null, IsInvalid) (Syntax: ForEachStatement, 'foreach (in ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'args')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: args ([0] OperationKind.ParameterReferenceExpression, Type: System.String[]) (Syntax: IdentifierName, 'args')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_ModifyIterationVariable()
        {
            string source = @"
class C
{
    void F(int[] a)
    {
        /*<bind>*/foreach (int x in a) { x++; }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForEachStatement, 'foreach (in ... a) { x++; }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: null, IsInvalid) (Syntax: ForEachStatement, 'foreach (in ... a) { x++; }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'a')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: a ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32[]) (Syntax: IdentifierName, 'a')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ x++; }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, 'x++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Object, IsInvalid) (Syntax: PostIncrementExpression, 'x++')
            Target: 
              IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'x')
                Children(1):
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'x')
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_Pattern()
        {
            string source = @"
class C
{
    void F(Enumerable e)
    {
        /*<bind>*/foreach (long x in e) { }/*</bind>*/
    }
}

class Enumerable
{
    public Enumerator GetEnumerator() { return new Enumerator(); }
}

class Enumerator
{
    public int Current { get { return 1; } }
    public bool MoveNext() { return false; }
}

";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (lo ... x in e) { }') (Parent: BlockStatement)
  Locals: Local_1: System.Int64 x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int64, Constant: null) (Syntax: ForEachStatement, 'foreach (lo ... x in e) { }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: Enumerable, IsImplicit) (Syntax: IdentifierName, 'e')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: e ([0] OperationKind.ParameterReferenceExpression, Type: Enumerable) (Syntax: IdentifierName, 'e')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ }')
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_ImplicitlyTypedString()
        {
            string source = @"
class C
{
    void F(string s)
    {
        /*<bind>*/foreach (var x in s) { }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (var x in s) { }') (Parent: BlockStatement)
  Locals: Local_1: System.Char x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Char, Constant: null) (Syntax: ForEachStatement, 'foreach (var x in s) { }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 's')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: s ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ }')
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_ExplicitlyTypedVar()
        {
            string source = @"
class C
{
    void F(var[] a)
    {
        /*<bind>*/foreach (var x in a) { }/*</bind>*/
    }

    class var { }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (var x in a) { }') (Parent: BlockStatement)
  Locals: Local_1: C.var x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: C.var, Constant: null) (Syntax: ForEachStatement, 'foreach (var x in a) { }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'a')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: a ([0] OperationKind.ParameterReferenceExpression, Type: C.var[]) (Syntax: IdentifierName, 'a')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ }')
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_DynamicEnumerable()
        {
            string source = @"
class C
{
    void F(dynamic d)
    {
        /*<bind>*/foreach (int x in d) { }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (int x in d) { }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: null) (Syntax: ForEachStatement, 'foreach (int x in d) { }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'd')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: d ([0] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ }')
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_TypeParameterConstrainedToInterface()
        {
            string source = @"
class C
{
    static void Test<T>() where T : System.Collections.IEnumerator
    {
        /*<bind>*/foreach (object x in new Enumerable<T>())
        {
            System.Console.WriteLine(x);
        }/*</bind>*/
    }
}

public class Enumerable<T>
{
    public T GetEnumerator() { return default(T); }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (ob ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Object x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Object, Constant: null) (Syntax: ForEachStatement, 'foreach (ob ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: Enumerable<T>, IsImplicit) (Syntax: ObjectCreationExpression, 'new Enumerable<T>()')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IObjectCreationExpression (Constructor: Enumerable<T>..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Enumerable<T>) (Syntax: ObjectCreationExpression, 'new Enumerable<T>()')
          Arguments(0)
          Initializer: 
            null
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(x);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Object value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(x)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'x')
                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_CastArrayToIEnumerable()
        {
            string source = @"
using System.Collections;

class C
{
    static void Main(string[] args)
    {
        /*<bind>*/foreach (string x in (IEnumerable)args) { }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (st ... e)args) { }') (Parent: BlockStatement)
  Locals: Local_1: System.String x
  LoopControlVariable: 
    ILocalReferenceExpression: x (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.String, Constant: null) (Syntax: ForEachStatement, 'foreach (st ... e)args) { }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: CastExpression, '(IEnumerable)args')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable) (Syntax: CastExpression, '(IEnumerable)args')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IParameterReferenceExpression: args ([0] OperationKind.ParameterReferenceExpression, Type: System.String[]) (Syntax: IdentifierName, 'args')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ }')
  NextVariables(0)
";

            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithThrow()
        {
            string source = @"
class Program
{
    static void Main()
    {
        int[] numbers = { 1, 2, 3, 4 };

        /*<bind>*/foreach (int num in numbers)
        {
            if (num > 3)
            {
                throw new System.Exception(""testing"");
            }
            System.Console.WriteLine(num);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachStatement, 'foreach (in ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 num
  LoopControlVariable: 
    ILocalReferenceExpression: num (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: null) (Syntax: ForEachStatement, 'foreach (in ... }')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'numbers')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: numbers ([0] OperationKind.LocalReferenceExpression, Type: System.Int32[]) (Syntax: IdentifierName, 'numbers')
  Body: 
    IBlockStatement (2 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if (num > 3 ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'num > 3')
            Left: 
              ILocalReferenceExpression: num ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'num')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'throw new S ... ""testing"");')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'throw new S ... ""testing"");')
                  IObjectCreationExpression (Constructor: System.Exception..ctor(System.String message)) ([0] OperationKind.ObjectCreationExpression, Type: System.Exception) (Syntax: ObjectCreationExpression, 'new System. ... (""testing"")')
                    Arguments(1):
                      IArgument (ArgumentKind.Explicit, Matching Parameter: message) ([0] OperationKind.Argument) (Syntax: Argument, '""testing""')
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""testing"") (Syntax: StringLiteralExpression, '""testing""')
                        InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                        OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Initializer: 
                      null
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... eLine(num);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... teLine(num)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'num')
                ILocalReferenceExpression: num ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'num')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
";
            VerifyOperationTreeForTest<ForEachStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithDeconstructDeclaration()
        {
            string source = @"
class X
{
    public static void M((int, int)[] x)
    {
        /*<bind>*/foreach (var (a, b) in x)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachVariableStatement, 'foreach (va ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 a
    Local_2: System.Int32 b
  LoopControlVariable: 
    IDeclarationExpression ([1] OperationKind.DeclarationExpression, Type: (System.Int32 a, System.Int32 b)) (Syntax: DeclarationExpression, 'var (a, b)')
      ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 a, System.Int32 b)) (Syntax: ParenthesizedVariableDesignation, '(a, b)')
        Elements(2):
          ILocalReferenceExpression: a (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'a')
          ILocalReferenceExpression: b (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'b')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'x')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: (System.Int32, System.Int32)[]) (Syntax: IdentifierName, 'x')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  NextVariables(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ForEachVariableStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithNestedDeconstructDeclaration()
        {
            string source = @"
class X
{
    public static void M((int, (int, int))[] x)
    {
        /*<bind>*/foreach (var (a, (b, c)) in x)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachVariableStatement, 'foreach (va ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 a
    Local_2: System.Int32 b
    Local_3: System.Int32 c
  LoopControlVariable: 
    IDeclarationExpression ([1] OperationKind.DeclarationExpression, Type: (System.Int32 a, (System.Int32 b, System.Int32 c))) (Syntax: DeclarationExpression, 'var (a, (b, c))')
      ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 a, (System.Int32 b, System.Int32 c))) (Syntax: ParenthesizedVariableDesignation, '(a, (b, c))')
        Elements(2):
          ILocalReferenceExpression: a (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'a')
          ITupleExpression ([1] OperationKind.TupleExpression, Type: (System.Int32 b, System.Int32 c)) (Syntax: ParenthesizedVariableDesignation, '(b, c)')
            Elements(2):
              ILocalReferenceExpression: b (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'b')
              ILocalReferenceExpression: c (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'c')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'x')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: (System.Int32, (System.Int32, System.Int32))[]) (Syntax: IdentifierName, 'x')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  NextVariables(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ForEachVariableStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithInvalidLoopControlVariable()
        {
            string source = @"
class X
{
    public static void M((int, int)[] x)
    /*<bind>*/{
        foreach (i, j in x)
        {
        }
    }/*</bind>*/
}
";
            string expectedOperationTree = @"
IBlockStatement (4 statements) ([Root] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ ... }') (Parent: )
  IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForEachVariableStatement, 'foreach (i')
    LoopControlVariable: 
      null
    Collection: 
      IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
        Children(0)
    Body: 
      IExpressionStatement ([1] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, '')
        Expression: 
          IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
            Children(0)
    NextVariables(0)
  IExpressionStatement ([1] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, 'j ')
    Expression: 
      IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'j')
        Children(0)
  IExpressionStatement ([2] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, 'x')
    Expression: 
      IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: (System.Int32, System.Int32)[], IsInvalid) (Syntax: IdentifierName, 'x')
  IBlockStatement (0 statements) ([3] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1515: 'in' expected
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_InExpected, ",").WithLocation(6, 19),
                // CS0230: Type and identifier are both required in a foreach statement
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_BadForeachDecl, ",").WithLocation(6, 19),
                // CS1525: Invalid expression term ','
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(6, 19),
                // CS1026: ) expected
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_CloseParenExpected, ",").WithLocation(6, 19),
                // CS1525: Invalid expression term ','
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(6, 19),
                // CS1002: ; expected
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_SemicolonExpected, ",").WithLocation(6, 19),
                // CS1513: } expected
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_RbraceExpected, ",").WithLocation(6, 19),
                // CS1002: ; expected
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_SemicolonExpected, "in").WithLocation(6, 23),
                // CS1513: } expected
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_RbraceExpected, "in").WithLocation(6, 23),
                // CS1002: ; expected
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_SemicolonExpected, ")").WithLocation(6, 27),
                // CS1513: } expected
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_RbraceExpected, ")").WithLocation(6, 27),
                // CS0103: The name 'j' does not exist in the current context
                //         foreach (i, j in x)
                Diagnostic(ErrorCode.ERR_NameNotInContext, "j").WithArguments("j").WithLocation(6, 21)
            };

            VerifyOperationTreeAndDiagnosticsForTest<BlockSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IForEachLoopStatement_WithInvalidLoopControlVariable_02()
        {
            string source = @"
class X
{
    public static void M(int[] x)
    /*<bind>*/{
        foreach (x[0] in x)
        {
        }
    }/*</bind>*/
}
";
            string expectedOperationTree = @"
IBlockStatement (1 statements) ([Root] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ ... }') (Parent: )
  IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForEachVariableStatement, 'foreach (x[ ... }')
    LoopControlVariable: 
      null
    Collection: 
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'x')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32[]) (Syntax: IdentifierName, 'x')
    Body: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
    NextVariables(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                Diagnostic(ErrorCode.ERR_BadForeachDecl, "in").WithLocation(6, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<BlockSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
