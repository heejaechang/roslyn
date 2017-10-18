// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using System.Linq;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidVariableDeclarationStatement()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int x, ( 1 );/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'int x, ( 1 );') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x')
    Variables: Local_1: System.Int32 x
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, '( 1 ')
    Variables: Local_1: System.Int32 
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1001: Identifier expected
                //         /*<bind>*/int x, ( 1 );/*</bind>*/
                Diagnostic(ErrorCode.ERR_IdentifierExpected, "(").WithLocation(8, 26),
                // CS1528: Expected ; or = (cannot specify constructor arguments in declaration)
                //         /*<bind>*/int x, ( 1 );/*</bind>*/
                Diagnostic(ErrorCode.ERR_BadVarDecl, "( 1 ").WithLocation(8, 26),
                // CS1003: Syntax error, '[' expected
                //         /*<bind>*/int x, ( 1 );/*</bind>*/
                Diagnostic(ErrorCode.ERR_SyntaxError, "(").WithArguments("[", "(").WithLocation(8, 26),
                // CS1003: Syntax error, ']' expected
                //         /*<bind>*/int x, ( 1 );/*</bind>*/
                Diagnostic(ErrorCode.ERR_SyntaxError, ")").WithArguments("]", ")").WithLocation(8, 30),
                // CS0168: The variable 'x' is declared but never used
                //         /*<bind>*/int x, ( 1 );/*</bind>*/
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "x").WithArguments("x").WithLocation(8, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidSwitchStatementExpression()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/switch (Program)
        {
            case 1:
                break;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ISwitchStatement (1 cases) ([0] OperationKind.SwitchStatement, IsInvalid) (Syntax: SwitchStatement, 'switch (Pro ... }') (Parent: BlockStatement)
  Switch expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Program')
      Children(1):
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Program')
  Sections:
    ISwitchCase (1 case clauses, 1 statements) ([1] OperationKind.SwitchCase, IsInvalid) (Syntax: SwitchSection, 'case 1: ... break;')
        Clauses:
          IPatternCaseClause (Label Symbol: case 1:) (CaseKind.Pattern) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CaseSwitchLabel, 'case 1:')
            Pattern: 
              IConstantPattern ([0] OperationKind.ConstantPattern, IsInvalid) (Syntax: CaseSwitchLabel, 'case 1:')
                Value: 
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                    Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
            Guard Expression: 
              null
        Body:
          IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0119: 'Program' is a type, which is not valid in the given context
                //         /*<bind>*/switch (Program)
                Diagnostic(ErrorCode.ERR_BadSKunknown, "Program").WithArguments("Program", "type").WithLocation(8, 27),
                // CS0029: Cannot implicitly convert type 'int' to 'Program'
                //             case 1:
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "1").WithArguments("int", "Program").WithLocation(10, 18),
                // CS0162: Unreachable code detected
                //                 break;
                Diagnostic(ErrorCode.WRN_UnreachableCode, "break").WithLocation(11, 17)
            };

            VerifyOperationTreeAndDiagnosticsForTest<SwitchStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidSwitchStatementCaseLabel()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var x = new Program();
        /*<bind>*/switch (x.ToString())
        {
            case 1:
                break;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ISwitchStatement (1 cases) ([1] OperationKind.SwitchStatement, IsInvalid) (Syntax: SwitchStatement, 'switch (x.T ... }') (Parent: BlockStatement)
  Switch expression: 
    IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'x.ToString()')
      Instance Receiver: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'x')
      Arguments(0)
  Sections:
    ISwitchCase (1 case clauses, 1 statements) ([1] OperationKind.SwitchCase, IsInvalid) (Syntax: SwitchSection, 'case 1: ... break;')
        Clauses:
          ISingleValueCaseClause (CaseKind.SingleValue) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: CaseSwitchLabel, 'case 1:')
            Value: 
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
        Body:
          IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'int' to 'string'
                //             case 1:
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "1").WithArguments("int", "string").WithLocation(11, 18)
            };

            VerifyOperationTreeAndDiagnosticsForTest<SwitchStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidIfStatement()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var x = new Program();
        /*<bind>*/if (x = null)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IIfStatement ([1] OperationKind.IfStatement, IsInvalid) (Syntax: IfStatement, 'if (x = nul ... }') (Parent: BlockStatement)
  Condition: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: SimpleAssignmentExpression, 'x = null')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: Program, IsInvalid) (Syntax: SimpleAssignmentExpression, 'x = null')
          Left: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: Program, Constant: null, IsInvalid, IsImplicit) (Syntax: NullLiteralExpression, 'null')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null, IsInvalid) (Syntax: NullLiteralExpression, 'null')
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  IfFalse: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'Program' to 'bool'
                //         /*<bind>*/if (x = null)
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "x = null").WithArguments("Program", "bool").WithLocation(9, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IfStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidIfElseStatement()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var x = new Program();
        /*<bind>*/if ()
        {
        }
        else if (x) x;
        else
/*</bind>*/    }
}
";
            string expectedOperationTree = @"
IIfStatement ([1] OperationKind.IfStatement, IsInvalid) (Syntax: IfStatement, 'if () ... else') (Parent: BlockStatement)
  Condition: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  IfFalse: 
    IIfStatement ([2] OperationKind.IfStatement, IsInvalid) (Syntax: IfStatement, 'if (x) x; ... else')
      Condition: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'x')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
      IfTrue: 
        IExpressionStatement ([1] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, 'x;')
          Expression: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
      IfFalse: 
        IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, '')
          Expression: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null) (Syntax: IdentifierName, '')
              Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ')'
                //         /*<bind>*/if ()
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(9, 23),
                // CS1525: Invalid expression term '}'
                //         else
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, "").WithArguments("}").WithLocation(13, 13),
                // CS1002: ; expected
                //         else
                Diagnostic(ErrorCode.ERR_SemicolonExpected, "").WithLocation(13, 13),
                // CS0029: Cannot implicitly convert type 'Program' to 'bool'
                //         else if (x) x;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "x").WithArguments("Program", "bool").WithLocation(12, 18),
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         else if (x) x;
                Diagnostic(ErrorCode.ERR_IllegalStatement, "x").WithLocation(12, 21)
            };

            VerifyOperationTreeAndDiagnosticsForTest<IfStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidForStatement()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        var x = new Program();
        /*<bind>*/for (P; x;)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForLoopStatement (LoopKind.For) ([1] OperationKind.LoopStatement, IsInvalid) (Syntax: ForStatement, 'for (P; x;) ... }') (Parent: BlockStatement)
  Condition: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'x')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
  Before:
    IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'P')
      Expression: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'P')
          Children(0)
  AtLoopBottom(0)
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'P' does not exist in the current context
                //         /*<bind>*/for (P; x;)
                Diagnostic(ErrorCode.ERR_NameNotInContext, "P").WithArguments("P").WithLocation(9, 24),
                // CS0201: Only assignment, call, increment, decrement, and new object expressions can be used as a statement
                //         /*<bind>*/for (P; x;)
                Diagnostic(ErrorCode.ERR_IllegalStatement, "P").WithLocation(9, 24),
                // CS0029: Cannot implicitly convert type 'Program' to 'bool'
                //         /*<bind>*/for (P; x;)
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "x").WithArguments("Program", "bool").WithLocation(9, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ForStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidGotoCaseStatement_MissingLabel()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        switch (args.Length)
        {
            case 0:
                /*<bind>*/goto case 1;/*</bind>*/
                break;
        }
    }
}
";
            string expectedOperationTree = @"
IInvalidStatement ([1] OperationKind.InvalidStatement, IsInvalid) (Syntax: GotoCaseStatement, 'goto case 1;') (Parent: SwitchCase)
  Children(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0159: No such label 'case 1:' within the scope of the goto statement
                //                 /*<bind>*/goto case 1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_LabelNotFound, "goto case 1;").WithArguments("case 1:").WithLocation(11, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<GotoStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact(Skip = "https://github.com/dotnet/roslyn/issues/18225"), WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidGotoCaseStatement_OutsideSwitchStatement()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/goto case 1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IInvalidStatement (OperationKind.InvalidStatement, IsInvalid) (Syntax: 'goto case 1;')
  ILiteralExpression (OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: '1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0153: A goto case is only valid inside a switch statement
                //         /*<bind>*/goto case 1;/*</bind>*/
                Diagnostic(ErrorCode.ERR_InvalidGotoCase, "goto case 1;").WithLocation(8, 19)
            };

            VerifyOperationTreeAndDiagnosticsForTest<GotoStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidBreakStatement_OutsideLoopOrSwitch()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/break;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IInvalidStatement ([0] OperationKind.InvalidStatement, IsInvalid) (Syntax: BreakStatement, 'break;') (Parent: BlockStatement)
  Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0139: No enclosing loop out of which to break or continue
                //         /*<bind>*/break;/*</bind>*/
                Diagnostic(ErrorCode.ERR_NoBreakOrCont, "break;").WithLocation(8, 19)
            };

            VerifyOperationTreeAndDiagnosticsForTest<BreakStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")]
        public void InvalidContinueStatement_OutsideLoopOrSwitch()
        {
            string source = @"
using System;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/continue;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IInvalidStatement ([0] OperationKind.InvalidStatement, IsInvalid) (Syntax: ContinueStatement, 'continue;') (Parent: BlockStatement)
  Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0139: No enclosing loop out of which to break or continue
                //         /*<bind>*/continue;/*</bind>*/
                Diagnostic(ErrorCode.ERR_NoBreakOrCont, "continue;").WithLocation(8, 19)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ContinueStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
