﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        #region Variable Declarations

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void SingleVariableDeclaration()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0168: The variable 'i1' is declared but never used
                //         /*<bind>*/int i1;/*</bind>*/
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "i1").WithArguments("i1").WithLocation(6, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void SingleVariableDeclarationWithInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i1 = 1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i1 = 1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1 = 1')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i1' is assigned but its value is never used
                //         /*<bind>*/int i1 = 1;/*</bind>*/
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i1").WithArguments("i1").WithLocation(6, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void SingleVariableDeclarationWithInvalidInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i1 = ;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'int i1 = ;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1 = ')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= ')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ';'
                //         /*<bind>*/int i1 = ;/*</bind>*/
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ";").WithArguments(";").WithLocation(6, 28)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void MultipleDeclarations()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i1, i2;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i1, i2;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i2')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0168: The variable 'i1' is declared but never used
                //         /*<bind>*/int i1, i2;/*</bind>*/
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "i1").WithArguments("i1").WithLocation(6, 23),
                // CS0168: The variable 'i2' is declared but never used
                //         /*<bind>*/int i1, i2;/*</bind>*/
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "i2").WithArguments("i2").WithLocation(6, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void MultipleDeclarationsWithInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i1 = 2, i2 = 2;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i1 = 2, i2 = 2;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1 = 2')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 2')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i2 = 2')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 2')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i1' is assigned but its value is never used
                //         /*<bind>*/int i1 = 2, i2 = 2/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i1").WithArguments("i1").WithLocation(6, 23),
                // CS0219: The variable 'i2' is assigned but its value is never used
                //         /*<bind>*/int i1 = 2, i2 = 2/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i2").WithArguments("i2").WithLocation(6, 31)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void MultipleDeclarationsWithInvalidInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i1 = , i2 = 2;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'int i1 = , i2 = 2;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1 = ')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= ')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i2 = 2')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 2')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ','
                //         /*<bind>*/int i1 = , i2 = 2/*</bind>*/;
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(6, 28),
                // CS0219: The variable 'i2' is assigned but its value is never used
                //         /*<bind>*/int i1 = , i2 = 2/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i2").WithArguments("i2").WithLocation(6, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void InvalidMultipleVariableDeclaration()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i,;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'int i,;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i')
    Variables: Local_1: System.Int32 i
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, '')
    Variables: Local_1: System.Int32 
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1001: Identifier expected
                //         /*<bind>*/int i,/*</bind>*/;
                Diagnostic(ErrorCode.ERR_IdentifierExpected, ";").WithLocation(6, 25),
                // CS0168: The variable 'i' is declared but never used
                //         /*<bind>*/int i,/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "i").WithArguments("i").WithLocation(6, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void SingleVariableDeclarationExpressionInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i = GetInt();/*</bind>*/
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i = GetInt();') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = GetInt()')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void MutlipleVariableDeclarationsExpressionInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int i = GetInt(), j = GetInt();/*</bind>*/
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i = Get ... = GetInt();') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = GetInt()')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = GetInt()')
    Variables: Local_1: System.Int32 j
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void SingleVariableDeclarationLocalReferenceInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        int i = 1;
        /*<bind>*/int i1 = i;/*</bind>*/
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i1 = i;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1 = i')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= i')
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void MultipleDeclarationsLocalReferenceInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        int i = 1;
        /*<bind>*/int i1 = i, i2 = i1;/*</bind>*/
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i1 = i, i2 = i1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1 = i')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= i')
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i2 = i1')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void InvalidArrayDeclaration()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int[2, 3] a;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'int[2, 3] a;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'a')
    Variables: Local_1: System.Int32[,] a
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0270: Array size cannot be specified in a variable declaration (try initializing with a 'new' expression)
                //         /*<bind>*/int[2, 3] a/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ArraySizeInDeclaration, "2").WithLocation(6, 23),
                // CS0270: Array size cannot be specified in a variable declaration (try initializing with a 'new' expression)
                //         /*<bind>*/int[2, 3] a/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ArraySizeInDeclaration, "3").WithLocation(6, 26),
                // CS0168: The variable 'a' is declared but never used
                //         /*<bind>*/int[2, 3] a/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "a").WithArguments("a").WithLocation(6, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void InvalidArrayMultipleDeclaration()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/int[2, 3] a, b;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'int[2, 3] a, b;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'a')
    Variables: Local_1: System.Int32[,] a
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'b')
    Variables: Local_1: System.Int32[,] b
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0270: Array size cannot be specified in a variable declaration (try initializing with a 'new' expression)
                //         /*<bind>*/int[2, 3] a, b/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ArraySizeInDeclaration, "2").WithLocation(6, 23),
                // CS0270: Array size cannot be specified in a variable declaration (try initializing with a 'new' expression)
                //         /*<bind>*/int[2, 3] a, b/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ArraySizeInDeclaration, "3").WithLocation(6, 26),
                // CS0168: The variable 'a' is declared but never used
                //         /*<bind>*/int[2, 3] a, b/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "a").WithArguments("a").WithLocation(6, 29),
                // CS0168: The variable 'b' is declared but never used
                //         /*<bind>*/int[2, 3] a, b/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "b").WithArguments("b").WithLocation(6, 32)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TestGetOperationForVariableInitializer()
        {
            string source = @"
class Test
{
    void M()
    {
        var x /*<bind>*/= 1/*</bind>*/;
        System.Console.WriteLine(x);
    }
}
";
            string expectedOperationTree = @"
IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 1') (Parent: VariableDeclaration)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<EqualsValueClauseSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        #endregion

        #region Fixed Statements

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void FixedStatementDeclaration()
        {
            string source = @"
class Program
{
    int i1;
    static void Main(string[] args)
    {
        var reference = new Program();
        unsafe
        {
            fixed (/*<bind>*/int* p = &reference.i1/*</bind>*/)
            {

            }
        }
    }
}
";


            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int* p = &reference.i1') (Parent: None)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p = &reference.i1')
    Variables: Local_1: System.Int32* p
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &reference.i1')
        IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&reference.i1')
          Children(1):
            IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&reference.i1')
              Reference: 
                IFieldReferenceExpression: System.Int32 Program.i1 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'reference.i1')
                  Instance Receiver: 
                    ILocalReferenceExpression: reference ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'reference')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //         unsafe
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "unsafe").WithLocation(8, 9)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void FixedStatementMultipleDeclaration()
        {
            string source = @"
class Program
{
    int i1, i2;
    static void Main(string[] args)
    {
        var reference = new Program();
        unsafe
        {
            fixed (/*<bind>*/int* p1 = &reference.i1, p2 = &reference.i2/*</bind>*/)
            {

            }
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int* p1 = & ... eference.i2') (Parent: None)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = &reference.i1')
    Variables: Local_1: System.Int32* p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &reference.i1')
        IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&reference.i1')
          Children(1):
            IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&reference.i1')
              Reference: 
                IFieldReferenceExpression: System.Int32 Program.i1 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'reference.i1')
                  Instance Receiver: 
                    ILocalReferenceExpression: reference ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'reference')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p2 = &reference.i2')
    Variables: Local_1: System.Int32* p2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &reference.i2')
        IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&reference.i2')
          Children(1):
            IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&reference.i2')
              Reference: 
                IFieldReferenceExpression: System.Int32 Program.i2 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'reference.i2')
                  Instance Receiver: 
                    ILocalReferenceExpression: reference ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'reference')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //         unsafe
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "unsafe").WithLocation(8, 9)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void FixedStatementInvalidAssignment()
        {
            string source = @"
class Program
{
    int i1;
    static void Main(string[] args)
    {
        var reference = new Program();
        unsafe
        {
            fixed (/*<bind>*/int* p = /*</bind>*/)
            {

            }
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int* p = /*</bind>*/') (Parent: None)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p = /*</bind>*/')
    Variables: Local_1: System.Int32* p
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= /*</bind>*/')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ')'
                //             fixed (/*<bind>*/int* p = /*</bind>*/)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(10, 50),
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //         unsafe
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "unsafe").WithLocation(8, 9),
                // CS0169: The field 'Program.i1' is never used
                //     int i1;
                Diagnostic(ErrorCode.WRN_UnreferencedField, "i1").WithArguments("Program.i1").WithLocation(4, 9)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void FixedStatementMultipleDeclarationsInvalidInitializers()
        {
            string source = @"
class Program
{
    int i1, i2;
    static void Main(string[] args)
    {
        var reference = new Program();
        unsafe
        {
            fixed (/*<bind>*/int* p1 = , p2 = /*</bind>*/)
            {

            }
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int* p1 = , ... /*</bind>*/') (Parent: None)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p1 = ')
    Variables: Local_1: System.Int32* p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= ')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p2 = /*</bind>*/')
    Variables: Local_1: System.Int32* p2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= /*</bind>*/')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ','
                //             fixed (/*<bind>*/int* p1 = , p2 = /*</bind>*/)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(10, 40),
                // CS1525: Invalid expression term ')'
                //             fixed (/*<bind>*/int* p1 = , p2 = /*</bind>*/)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(10, 58),
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //         unsafe
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "unsafe").WithLocation(8, 9),
                // CS0169: The field 'Program.i2' is never used
                //     int i1, i2;
                Diagnostic(ErrorCode.WRN_UnreferencedField, "i2").WithArguments("Program.i2").WithLocation(4, 13),
                // CS0169: The field 'Program.i1' is never used
                //     int i1, i2;
                Diagnostic(ErrorCode.WRN_UnreferencedField, "i1").WithArguments("Program.i1").WithLocation(4, 9)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void FixedStatementNoInitializer()
        {
            string source = @"
class Program
{
    int i1;
    static void Main(string[] args)
    {
        var reference = new Program();
        unsafe
        {
            fixed (/*<bind>*/int* p/*</bind>*/)
            {

            }
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int* p') (Parent: None)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p')
    Variables: Local_1: System.Int32* p
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //         unsafe
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "unsafe").WithLocation(8, 9),
                // CS0210: You must provide an initializer in a fixed or using statement declaration
                //             fixed (/*<bind>*/int* p/*</bind>*/)
                Diagnostic(ErrorCode.ERR_FixedMustInit, "p").WithLocation(10, 35),
                // CS0169: The field 'Program.i1' is never used
                //     int i1;
                Diagnostic(ErrorCode.WRN_UnreferencedField, "i1").WithArguments("Program.i1").WithLocation(4, 9)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void FixedStatementMultipleDeclarationsNoInitializers()
        {
            string source = @"
class Program
{
    int i1, i2;
    static void Main(string[] args)
    {
        var reference = new Program();
        unsafe
        {
            fixed (/*<bind>*/int* p1, p2/*</bind>*/)
            {

            }
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int* p1, p2') (Parent: None)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p1')
    Variables: Local_1: System.Int32* p1
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p2')
    Variables: Local_1: System.Int32* p2
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //         unsafe
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "unsafe").WithLocation(8, 9),
                // CS0210: You must provide an initializer in a fixed or using statement declaration
                //             fixed (/*<bind>*/int* p1, p2/*</bind>*/)
                Diagnostic(ErrorCode.ERR_FixedMustInit, "p1").WithLocation(10, 35),
                // CS0210: You must provide an initializer in a fixed or using statement declaration
                //             fixed (/*<bind>*/int* p1, p2/*</bind>*/)
                Diagnostic(ErrorCode.ERR_FixedMustInit, "p2").WithLocation(10, 39),
                // CS0169: The field 'Program.i2' is never used
                //     int i1, i2;
                Diagnostic(ErrorCode.WRN_UnreferencedField, "i2").WithArguments("Program.i2").WithLocation(4, 13),
                // CS0169: The field 'Program.i1' is never used
                //     int i1, i2;
                Diagnostic(ErrorCode.WRN_UnreferencedField, "i1").WithArguments("Program.i1").WithLocation(4, 9)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void FixedStatementInvalidMulipleDeclarations()
        {
            string source = @"
class Program
{
    int i1, i2;
    static void Main(string[] args)
    {
        var reference = new Program();
        unsafe
        {
            fixed (/*<bind>*/int* p1 = &reference.i1,/*</bind>*/)
            {

            }
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int* p1 = & ... /*</bind>*/') (Parent: None)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = &reference.i1')
    Variables: Local_1: System.Int32* p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &reference.i1')
        IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&reference.i1')
          Children(1):
            IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&reference.i1')
              Reference: 
                IFieldReferenceExpression: System.Int32 Program.i1 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'reference.i1')
                  Instance Receiver: 
                    ILocalReferenceExpression: reference ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'reference')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, '')
    Variables: Local_1: System.Int32* 
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1001: Identifier expected
                //             fixed (/*<bind>*/int* p1 = &reference.i1,/*</bind>*/)
                Diagnostic(ErrorCode.ERR_IdentifierExpected, ")").WithLocation(10, 65),
                // CS0227: Unsafe code may only appear if compiling with /unsafe
                //         unsafe
                Diagnostic(ErrorCode.ERR_IllegalUnsafe, "unsafe").WithLocation(8, 9),
                // CS0210: You must provide an initializer in a fixed or using statement declaration
                //             fixed (/*<bind>*/int* p1 = &reference.i1,/*</bind>*/)
                Diagnostic(ErrorCode.ERR_FixedMustInit, "").WithLocation(10, 65),
                // CS0169: The field 'Program.i2' is never used
                //     int i1, i2;
                Diagnostic(ErrorCode.WRN_UnreferencedField, "i2").WithArguments("Program.i2").WithLocation(4, 13)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        #endregion

        #region Using Statements

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementDeclaration()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1 = new Program()/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'Program p1  ... w Program()') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = new Program()')
    Variables: Local_1: Program p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new Program()')
        IObjectCreationExpression (Constructor: Program..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Program) (Syntax: ObjectCreationExpression, 'new Program()')
          Arguments(0)
          Initializer: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementMultipleDeclarations()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1 = new Program(), p2 = new Program()/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'Program p1  ... w Program()') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = new Program()')
    Variables: Local_1: Program p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new Program()')
        IObjectCreationExpression (Constructor: Program..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Program) (Syntax: ObjectCreationExpression, 'new Program()')
          Arguments(0)
          Initializer: 
            null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p2 = new Program()')
    Variables: Local_1: Program p2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new Program()')
        IObjectCreationExpression (Constructor: Program..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Program) (Syntax: ObjectCreationExpression, 'new Program()')
          Arguments(0)
          Initializer: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementInvalidInitializer()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1 =/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'Program p1 =/*</bind>*/') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p1 =/*</bind>*/')
    Variables: Local_1: Program p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '=/*</bind>*/')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ')'
                //         using (/*<bind>*/Program p1 =/*</bind>*/)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(8, 49)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementMultipleDeclarationsInvalidInitializers()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1 =, p2 =/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'Program p1  ... /*</bind>*/') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p1 =')
    Variables: Local_1: Program p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '=')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p2 =/*</bind>*/')
    Variables: Local_1: Program p2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '=/*</bind>*/')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ','
                //         using (/*<bind>*/Program p1 =, p2 =/*</bind>*/)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(8, 38),
                // CS1525: Invalid expression term ')'
                //         using (/*<bind>*/Program p1 =, p2 =/*</bind>*/)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(8, 55)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementNoInitializer()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'Program p1') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p1')
    Variables: Local_1: Program p1
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0210: You must provide an initializer in a fixed or using statement declaration
                //         using (/*<bind>*/Program p1/*</bind>*/)
                Diagnostic(ErrorCode.ERR_FixedMustInit, "p1").WithLocation(8, 34)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementMultipleDeclarationsNoInitializers()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1, p2/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'Program p1, p2') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p1')
    Variables: Local_1: Program p1
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p2')
    Variables: Local_1: Program p2
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0210: You must provide an initializer in a fixed or using statement declaration
                //         using (/*<bind>*/Program p1, p2/*</bind>*/)
                Diagnostic(ErrorCode.ERR_FixedMustInit, "p1").WithLocation(8, 34),
                // CS0210: You must provide an initializer in a fixed or using statement declaration
                //         using (/*<bind>*/Program p1, p2/*</bind>*/)
                Diagnostic(ErrorCode.ERR_FixedMustInit, "p2").WithLocation(8, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementInvalidMultipleDeclarations()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1 = new Program(),/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'Program p1  ... /*</bind>*/') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = new Program()')
    Variables: Local_1: Program p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new Program()')
        IObjectCreationExpression (Constructor: Program..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Program) (Syntax: ObjectCreationExpression, 'new Program()')
          Arguments(0)
          Initializer: 
            null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, '')
    Variables: Local_1: Program 
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1001: Identifier expected
                //         using (/*<bind>*/Program p1 = new Program(),/*</bind>*/)
                Diagnostic(ErrorCode.ERR_IdentifierExpected, ")").WithLocation(8, 64),
                // CS0210: You must provide an initializer in a fixed or using statement declaration
                //         using (/*<bind>*/Program p1 = new Program(),/*</bind>*/)
                Diagnostic(ErrorCode.ERR_FixedMustInit, "").WithLocation(8, 64)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementExpressionInitializer()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1 = GetProgram()/*</bind>*/)
        {
        }
    }

    static Program GetProgram() => new Program();

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'Program p1  ... etProgram()') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = GetProgram()')
    Variables: Local_1: Program p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetProgram()')
        IInvocationExpression (Program Program.GetProgram()) ([0] OperationKind.InvocationExpression, Type: Program) (Syntax: InvocationExpression, 'GetProgram()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementMultipleDeclarationsExpressionInitializers()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        using (/*<bind>*/Program p1 = GetProgram(), p2 = GetProgram()/*</bind>*/)
        {
        }
    }

    static Program GetProgram() => new Program();

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'Program p1  ... etProgram()') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = GetProgram()')
    Variables: Local_1: Program p1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetProgram()')
        IInvocationExpression (Program Program.GetProgram()) ([0] OperationKind.InvocationExpression, Type: Program) (Syntax: InvocationExpression, 'GetProgram()')
          Instance Receiver: 
            null
          Arguments(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p2 = GetProgram()')
    Variables: Local_1: Program p2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetProgram()')
        IInvocationExpression (Program Program.GetProgram()) ([0] OperationKind.InvocationExpression, Type: Program) (Syntax: InvocationExpression, 'GetProgram()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementLocalReferenceInitializer()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        Program p1 = new Program();
        using (/*<bind>*/Program p2 = p1/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'Program p2 = p1') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p2 = p1')
    Variables: Local_1: Program p2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= p1')
        ILocalReferenceExpression: p1 ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'p1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void UsingStatementMultipleDeclarationsLocalReferenceInitializers()
        {
            string source = @"
using System;

class Program : IDisposable
{
    static void Main(string[] args)
    {
        Program p1 = new Program();
        using (/*<bind>*/Program p2 = p1, p3 = p1/*</bind>*/)
        {
        }
    }

    public void Dispose() { }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'Program p2 = p1, p3 = p1') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p2 = p1')
    Variables: Local_1: Program p2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= p1')
        ILocalReferenceExpression: p1 ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'p1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p3 = p1')
    Variables: Local_1: Program p3
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= p1')
        ILocalReferenceExpression: p1 ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'p1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        #endregion

        #region For Loops

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopDeclaration()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i = 0/*</bind>*/; i < 0; i++)
        {

        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopMultipleDeclarations()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i = 0, j = 0/*</bind>*/; i < 0; i++)
        {
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = 0, j = 0') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
    Variables: Local_1: System.Int32 j
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'j' is assigned but its value is never used
                //         for (/*<bind>*/int i = 0, j = 0/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "j").WithArguments("j").WithLocation(6, 35)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopInvalidInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i =/*</bind>*/; i < 0; i++)
        {

        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int i =/*</bind>*/') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i =/*</bind>*/')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '=/*</bind>*/')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ';'
                //         for (/*<bind>*/int i =/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ";").WithArguments(";").WithLocation(6, 42)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopMultipleDeclarationsInvalidInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i =, j =/*</bind>*/; i < 0; i++)
        {

        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int i =, j =/*</bind>*/') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i =')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '=')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'j =/*</bind>*/')
    Variables: Local_1: System.Int32 j
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '=/*</bind>*/')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ','
                //         for (/*<bind>*/int i =, j =/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(6, 31),
                // CS1525: Invalid expression term ';'
                //         for (/*<bind>*/int i =, j =/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ";").WithArguments(";").WithLocation(6, 47)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopNoInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i/*</bind>*/; i < 0; i++)
        {

        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i')
    Variables: Local_1: System.Int32 i
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0165: Use of unassigned local variable 'i'
                //         for (/*<bind>*/int i/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.ERR_UseDefViolation, "i").WithArguments("i").WithLocation(6, 42)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopMultipleDeclarationsNoInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i, j/*</bind>*/; i < 0; i++)
        {

        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i, j') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i')
    Variables: Local_1: System.Int32 i
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j')
    Variables: Local_1: System.Int32 j
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0165: Use of unassigned local variable 'i'
                //         for (/*<bind>*/int i, j/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.ERR_UseDefViolation, "i").WithArguments("i").WithLocation(6, 45),
                // CS0168: The variable 'j' is declared but never used
                //         for (/*<bind>*/int i, j/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "j").WithArguments("j").WithLocation(6, 31)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopInvalidMultipleDeclarations()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i =,/*</bind>*/; i < 0; i++)
        {

        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int i =,/*</bind>*/') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i =')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '=')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, '')
    Variables: Local_1: System.Int32 
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ','
                //         for (/*<bind>*/int i =,/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(6, 31),
                // CS1001: Identifier expected
                //         for (/*<bind>*/int i =,/*</bind>*/; i < 0; i++)
                Diagnostic(ErrorCode.ERR_IdentifierExpected, ";").WithLocation(6, 43)
            };

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopExpressionInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i = GetInt()/*</bind>*/; i < 0; i++)
        {

        }
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = GetInt()') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = GetInt()')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ForLoopMultipleDeclarationsExpressionInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        for (/*<bind>*/int i = GetInt(), j = GetInt()/*</bind>*/; i < 0; i++)
        {

        }
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int i = Get ...  = GetInt()') (Parent: LoopStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = GetInt()')
    Variables: Local_1: System.Int32 i
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = GetInt()')
    Variables: Local_1: System.Int32 j
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        #endregion

        #region Const Local Declarations

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalDeclaration()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1 = 1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'const int i1 = 1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1 = 1')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i1' is assigned but its value is never used
                //         /*<bind>*/const int i1 = 1;/*</bind>*/
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i1").WithArguments("i1").WithLocation(6, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalMultipleDeclarations()
        {
            string source = @"
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1 = 1, i2 = 2;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'const int i ...  1, i2 = 2;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1 = 1')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i2 = 2')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 2')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i1' is assigned but its value is never used
                //         /*<bind>*/const int i1 = 1, i2 = 2;/*</bind>*/
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i1").WithArguments("i1").WithLocation(9, 29),
                // CS0219: The variable 'i2' is assigned but its value is never used
                //         /*<bind>*/const int i1 = 1, i2 = 2;/*</bind>*/
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i2").WithArguments("i2").WithLocation(9, 37)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalDeclarationInvalidInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1 = ;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'const int i1 = ;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1 = ')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= ')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ';'
                //         /*<bind>*/const int i1 = ;/*</bind>*/
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ";").WithArguments(";").WithLocation(6, 34)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalMultipleDeclarationsInvalidInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1 = , i2 = ;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'const int i1 = , i2 = ;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1 = ')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= ')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i2 = ')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= ')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ','
                //         const /*<bind>*/int i1 = , i2 = /*</bind>*/;
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(6, 34),
                // CS1525: Invalid expression term ';'
                //         const /*<bind>*/int i1 = , i2 = /*</bind>*/;
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ";").WithArguments(";").WithLocation(6, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalDeclarationNoInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'const int i1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0145: A const field requires a value to be provided
                //         const /*<bind>*/int i1/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ConstValueRequired, "i1").WithLocation(6, 29),
                // CS0168: The variable 'i1' is declared but never used
                //         const /*<bind>*/int i1/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "i1").WithArguments("i1").WithLocation(6, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalMutlipleDeclarationsNoInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1, i2;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'const int i1, i2;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i2')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0145: A const field requires a value to be provided
                //         const /*<bind>*/int i1, i2/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ConstValueRequired, "i1").WithLocation(6, 29),
                // CS0145: A const field requires a value to be provided
                //         const /*<bind>*/int i1, i2/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ConstValueRequired, "i2").WithLocation(6, 33),
                // CS0168: The variable 'i1' is declared but never used
                //         const /*<bind>*/int i1, i2/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "i1").WithArguments("i1").WithLocation(6, 29),
                // CS0168: The variable 'i2' is declared but never used
                //         const /*<bind>*/int i1, i2/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "i2").WithArguments("i2").WithLocation(6, 33)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalInvalidMultipleDeclarations()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1,;/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'const int i1,;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, '')
    Variables: Local_1: System.Int32 
    Initializer: 
      null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0145: A const field requires a value to be provided
                //         const /*<bind>*/int i1,/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ConstValueRequired, "i1").WithLocation(6, 29),
                // CS1001: Identifier expected
                //         const /*<bind>*/int i1,/*</bind>*/;
                Diagnostic(ErrorCode.ERR_IdentifierExpected, ";").WithLocation(6, 32),
                // CS0145: A const field requires a value to be provided
                //         const /*<bind>*/int i1,/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ConstValueRequired, ";").WithLocation(6, 32),
                // CS0168: The variable 'i1' is declared but never used
                //         const /*<bind>*/int i1,/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "i1").WithArguments("i1").WithLocation(6, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalDeclarationExpressionInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1 = GetInt();/*</bind>*/
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'const int i1 = GetInt();') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1 = GetInt()')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0133: The expression being assigned to 'i1' must be constant
                //         const /*<bind>*/int i1 = GetInt()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NotConstantExpression, "GetInt()").WithArguments("i1").WithLocation(6, 34)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalMultipleDeclarationsExpressionInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        /*<bind>*/const int i1 = GetInt(), i2 = GetInt();/*</bind>*/
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'const int i ... = GetInt();') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1 = GetInt()')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i2 = GetInt()')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= GetInt()')
        IInvocationExpression (System.Int32 Program.GetInt()) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'GetInt()')
          Instance Receiver: 
            null
          Arguments(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0133: The expression being assigned to 'i1' must be constant
                //         const /*<bind>*/int i1 = GetInt(), i2 = GetInt()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NotConstantExpression, "GetInt()").WithArguments("i1").WithLocation(6, 34),
                // CS0133: The expression being assigned to 'i2' must be constant
                //         const /*<bind>*/int i1 = GetInt(), i2 = GetInt()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NotConstantExpression, "GetInt()").WithArguments("i2").WithLocation(6, 49)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalDeclarationLocalReferenceInitializer()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        const int i = 1;
        /*<bind>*/const int i1 = i;/*</bind>*/
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'const int i1 = i;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1 = i')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= i')
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: 1) (Syntax: IdentifierName, 'i')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i1' is assigned but its value is never used
                //         const /*<bind>*/int i1 = i/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i1").WithArguments("i1").WithLocation(7, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")]
        public void ConstLocalMultipleDeclarationsLocalReferenceInitializers()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        const int i = 1;
        /*<bind>*/const int i1 = i, i2 = i1;/*</bind>*/
    }

    static int GetInt() => 1;
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (2 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'const int i ... i, i2 = i1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1 = i')
    Variables: Local_1: System.Int32 i1
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= i')
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: 1) (Syntax: IdentifierName, 'i')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i2 = i1')
    Variables: Local_1: System.Int32 i2
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: 1) (Syntax: IdentifierName, 'i1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i2' is assigned but its value is never used
                //         const /*<bind>*/int i1 = i, i2 = i1/*</bind>*/;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i2").WithArguments("i2").WithLocation(7, 37)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        #endregion
    }
}
