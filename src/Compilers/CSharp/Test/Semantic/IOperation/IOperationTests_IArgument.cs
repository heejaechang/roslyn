﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.CodeAnalysis.VisualBasic;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NoArgument()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }
    static void M2() { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2()) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void PositionalArgument()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(1, 2.0)/*</bind>*/;
    }

    static void M2(int x, double y) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2(System.Int32 x, System.Double y)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1, 2.0)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, '2.0')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 2) (Syntax: NumericLiteralExpression, '2.0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void PositionalArgumentWithDefaultValue()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(1)/*</bind>*/;
    }

    static void M2(int x, double y = 0.0) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2(System.Int32 x, [System.Double y = 0])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: y) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(1)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 0) (Syntax: InvocationExpression, 'M2(1)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedArgumentListedInParameterOrder()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(x: 1, y: 9.9)/*</bind>*/;
    }

    static void M2(int x, double y = 0.0) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2(System.Int32 x, [System.Double y = 0])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(x: 1, y: 9.9)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, 'x: 1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, 'y: 9.9')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 9.9) (Syntax: NumericLiteralExpression, '9.9')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedArgumentListedOutOfParameterOrder()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(y: 9.9, x: 1)/*</bind>*/;
    }

    static void M2(int x, double y = 0.0) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2(System.Int32 x, [System.Double y = 0])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(y: 9.9, x: 1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([0] OperationKind.Argument) (Syntax: Argument, 'y: 9.9')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 9.9) (Syntax: NumericLiteralExpression, '9.9')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, 'x: 1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedArgumentInParameterOrderWithDefaultValue()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(y: 0, z: 2)/*</bind>*/;
    }

    static void M2(int x = 1, int y = 2, int z = 3) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2([System.Int32 x = 1], [System.Int32 y = 2], [System.Int32 z = 3])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(y: 0, z: 2)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(3):
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([0] OperationKind.Argument) (Syntax: Argument, 'y: 0')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([1] OperationKind.Argument) (Syntax: Argument, 'z: 2')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(y: 0, z: 2)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: InvocationExpression, 'M2(y: 0, z: 2)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedArgumentOutOfParameterOrderWithDefaultValue()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(z: 2, x: 9)/*</bind>*/;
    }

    static void M2(int x = 1, int y = 2, int z = 3) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2([System.Int32 x = 1], [System.Int32 y = 2], [System.Int32 z = 3])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(z: 2, x: 9)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(3):
    IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([0] OperationKind.Argument) (Syntax: Argument, 'z: 2')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, 'x: 9')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 9) (Syntax: NumericLiteralExpression, '9')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: y) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(z: 2, x: 9)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: InvocationExpression, 'M2(z: 2, x: 9)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedAndPositionalArgumentsWithDefaultValue()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(9, z: 10);/*</bind>*/
    }

    static void M2(int x = 1, int y = 2, int z = 3) { }
}
";
            string expectedOperationTree = @"
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'M2(9, z: 10);') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (void P.M2([System.Int32 x = 1], [System.Int32 y = 2], [System.Int32 z = 3])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(9, z: 10)')
      Instance Receiver: 
        null
      Arguments(3):
        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '9')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 9) (Syntax: NumericLiteralExpression, '9')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([1] OperationKind.Argument) (Syntax: Argument, 'z: 10')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.DefaultValue, Matching Parameter: y) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(9, z: 10)')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: InvocationExpression, 'M2(9, z: 10)')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ExpressionStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void PositionalRefAndOutArguments()
        {
            string source = @"
class P
{
    void M1()
    {
        int a = 1;
        int b;
        /*<bind>*/M2(ref a, out b)/*</bind>*/;
    }

    void M2(ref int x, out int y) { y = 10; }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2(ref System.Int32 x, out System.Int32 y)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(ref a, out b)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, 'ref a')
      ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'a')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([2] OperationKind.Argument) (Syntax: Argument, 'out b')
      ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'b')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedRefAndOutArgumentsInParameterOrder()
        {
            string source = @"
class P
{
    void M1()
    {
        int a = 1;
        int b;
        /*<bind>*/M2(x: ref a, y: out b)/*</bind>*/;
    }

    void M2(ref int x, out int y) { y = 10; }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2(ref System.Int32 x, out System.Int32 y)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(x: ref a, y: out b)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, 'x: ref a')
      ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'a')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([2] OperationKind.Argument) (Syntax: Argument, 'y: out b')
      ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'b')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedRefAndOutArgumentsOutOfParameterOrder()
        {
            string source = @"
class P
{
    void M1()
    {
        int a = 1;
        int b;
        /*<bind>*/M2(y: out b, x: ref a)/*</bind>*/;
    }

    void M2(ref int x, out int y) { y = 10; }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2(ref System.Int32 x, out System.Int32 y)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(y: out b, x: ref a)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, 'y: out b')
      ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'b')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([2] OperationKind.Argument) (Syntax: Argument, 'x: ref a')
      ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'a')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueOfNewStruct()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }

    void M2(S sobj = new S()) { }
}

struct S { }
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([S sobj = default(S)])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sobj) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      IDefaultValueExpression ([0] OperationKind.DefaultValueExpression, Type: S) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueOfDefaultStruct()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }

    void M2(S sobj = default(S)) { }
}

struct S { }
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([S sobj = default(S)])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sobj) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      IDefaultValueExpression ([0] OperationKind.DefaultValueExpression, Type: S) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueOfConstant()
        {
            string source = @"
class P
{
    const double Pi = 3.14;
    void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }

    void M2(double s = Pi) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.Double s = 3.14])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: s) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 3.14) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void PositionalArgumentForExtensionMethod()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/this.E1(1, 2)/*</bind>*/;
    }
}

static class Extensions
{
    public static void E1(this P p, int x = 0, int y = 0)
    { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void Extensions.E1(this P p, [System.Int32 x = 0], [System.Int32 y = 0])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'this.E1(1, 2)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(3):
    IArgument (ArgumentKind.Explicit, Matching Parameter: p) ([0] OperationKind.Argument, IsImplicit) (Syntax: ThisExpression, 'this')
      IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([2] OperationKind.Argument) (Syntax: Argument, '2')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedArgumentOutOfParameterOrderForExtensionMethod()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/this.E1(y: 1, x: 2);/*</bind>*/
    }
}

static class Extensions
{
    public static void E1(this P p, int x = 0, int y = 0)
    { }
}
";
            string expectedOperationTree = @"
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'this.E1(y: 1, x: 2);') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (void Extensions.E1(this P p, [System.Int32 x = 0], [System.Int32 y = 0])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'this.E1(y: 1, x: 2)')
      Instance Receiver: 
        null
      Arguments(3):
        IArgument (ArgumentKind.Explicit, Matching Parameter: p) ([0] OperationKind.Argument, IsImplicit) (Syntax: ThisExpression, 'this')
          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, 'y: 1')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([2] OperationKind.Argument) (Syntax: Argument, 'x: 2')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ExpressionStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedArgumentWithDefaultValueForExtensionMethod()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/this.E1(y: 1)/*</bind>*/;
    }
}

static class Extensions
{
    public static void E1(this P p, int x = 0, int y = 0)
    { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void Extensions.E1(this P p, [System.Int32 x = 0], [System.Int32 y = 0])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'this.E1(y: 1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(3):
    IArgument (ArgumentKind.Explicit, Matching Parameter: p) ([0] OperationKind.Argument, IsImplicit) (Syntax: ThisExpression, 'this')
      IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, 'y: 1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'this.E1(y: 1)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: InvocationExpression, 'this.E1(y: 1)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ParamsArrayArgumentInNormalForm()
        {
            string source = @"
class P
{
    void M1()
    {
        var a = new[] { 0.0 };
        /*<bind>*/M2(1, a)/*</bind>*/;
    }

    void M2(int x, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2(System.Int32 x, params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1, a)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: array) ([2] OperationKind.Argument) (Syntax: Argument, 'a')
      ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Double[]) (Syntax: IdentifierName, 'a')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ParamsArrayArgumentInExpandedForm()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2(1, 0.1, 0.2)/*</bind>*/;
    }

    void M2(int x, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2(System.Int32 x, params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1, 0.1, 0.2)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(1, 0.1, 0.2)')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Double[]) (Syntax: InvocationExpression, 'M2(1, 0.1, 0.2)')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: InvocationExpression, 'M2(1, 0.1, 0.2)')
        Initializer: 
          IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: InvocationExpression, 'M2(1, 0.1, 0.2)')
            Element Values(2):
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 0.1) (Syntax: NumericLiteralExpression, '0.1')
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Double, Constant: 0.2) (Syntax: NumericLiteralExpression, '0.2')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ParamsArrayArgumentInExpandedFormWithNoArgument()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2(1)/*</bind>*/;
    }

    void M2(int x, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2(System.Int32 x, params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(1)')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Double[]) (Syntax: InvocationExpression, 'M2(1)')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: InvocationExpression, 'M2(1)')
        Initializer: 
          IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: InvocationExpression, 'M2(1)')
            Element Values(0)
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueAndParamsArrayArgumentInExpandedFormWithNoArgument()
        {
            string source = @"
class P
{
    void M1()
    {
        var a = new[] { 0.0 };
        /*<bind>*/M2()/*</bind>*/;
    }

    void M2(int x = 0, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.Int32 x = 0], params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Double[]) (Syntax: InvocationExpression, 'M2()')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: InvocationExpression, 'M2()')
        Initializer: 
          IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: InvocationExpression, 'M2()')
            Element Values(0)
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueAndNamedParamsArrayArgumentInNormalForm()
        {
            string source = @"
class P
{
    void M1()
    {
        var a = new[] { 0.0 };
        /*<bind>*/M2(array: a)/*</bind>*/;
    }

    void M2(int x = 0, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.Int32 x = 0], params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(array: a)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: array) ([1] OperationKind.Argument) (Syntax: Argument, 'array: a')
      ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Double[]) (Syntax: IdentifierName, 'a')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(array: a)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: InvocationExpression, 'M2(array: a)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueAndNamedParamsArrayArgumentInExpandedForm()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2(array: 1)/*</bind>*/;
    }

    void M2(int x = 0, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.Int32 x = 0], params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(array: 1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(array: 1)')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Double[]) (Syntax: InvocationExpression, 'M2(array: 1)')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: InvocationExpression, 'M2(array: 1)')
        Initializer: 
          IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: InvocationExpression, 'M2(array: 1)')
            Element Values(1):
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(array: 1)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: InvocationExpression, 'M2(array: 1)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void PositionalArgumentAndNamedParamsArrayArgumentInNormalForm()
        {
            string source = @"
class P
{
    void M1()
    {
        var a = new[] { 0.0 };
        /*<bind>*/M2(1, array: a)/*</bind>*/;
    }

    void M2(int x = 0, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.Int32 x = 0], params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1, array: a)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: array) ([2] OperationKind.Argument) (Syntax: Argument, 'array: a')
      ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Double[]) (Syntax: IdentifierName, 'a')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void PositionalArgumentAndNamedParamsArrayArgumentInExpandedForm()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2(1, array: 1)/*</bind>*/;
    }

    void M2(int x = 0, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.Int32 x = 0], params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1, array: 1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(1, array: 1)')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Double[]) (Syntax: InvocationExpression, 'M2(1, array: 1)')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: InvocationExpression, 'M2(1, array: 1)')
        Initializer: 
          IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: InvocationExpression, 'M2(1, array: 1)')
            Element Values(1):
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedArgumentAndNamedParamsArrayArgumentInNormalFormOutOfParameterOrder()
        {
            string source = @"
class P
{
    void M1()
    {
        var a = new[] { 0.0 };
        /*<bind>*/M2(array: a, x: 1);/*</bind>*/
    }

    void M2(int x = 0, params double[] array) { }
}
";
            string expectedOperationTree = @"
IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'M2(array: a, x: 1);') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression ( void P.M2([System.Int32 x = 0], params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(array: a, x: 1)')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
      Arguments(2):
        IArgument (ArgumentKind.Explicit, Matching Parameter: array) ([1] OperationKind.Argument) (Syntax: Argument, 'array: a')
          ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Double[]) (Syntax: IdentifierName, 'a')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([2] OperationKind.Argument) (Syntax: Argument, 'x: 1')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ExpressionStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NamedArgumentAndNamedParamsArrayArgumentInExpandedFormOutOfParameterOrder()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2(array: 1, x: 10)/*</bind>*/;
    }

    void M2(int x = 0, params double[] array) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.Int32 x = 0], params System.Double[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(array: 1, x: 10)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(2):
    IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(array: 1, x: 10)')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Double[]) (Syntax: InvocationExpression, 'M2(array: 1, x: 10)')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: InvocationExpression, 'M2(array: 1, x: 10)')
        Initializer: 
          IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: InvocationExpression, 'M2(array: 1, x: 10)')
            Element Values(1):
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([2] OperationKind.Argument) (Syntax: Argument, 'x: 10')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void CallerInfoAttributesInvokedInMethod()
        {
            string source = @"
using System.Runtime.CompilerServices;

class P
{
    void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }

    void M2(
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = 0)
    { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.String memberName = null], [System.String sourceFilePath = null], [System.Int32 sourceLineNumber = 0])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(3):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: memberName) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""M1"") (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sourceFilePath) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""file.cs"") (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sourceLineNumber) ([3] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 8) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalReferences: new[] { MscorlibRef_v46 });
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void CallerInfoAttributesInvokedInProperty()
        {
            string source = @"
using System.Runtime.CompilerServices;

class P
{
    bool M1 => /*<bind>*/M2()/*</bind>*/;

    bool M2(
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = 0)
    { 
        return true;
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( System.Boolean P.M2([System.String memberName = null], [System.String sourceFilePath = null], [System.Int32 sourceLineNumber = 0])) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'M2()') (Parent: ReturnStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(3):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: memberName) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""M1"") (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sourceFilePath) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""file.cs"") (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sourceLineNumber) ([3] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 6) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalReferences: new[] { MscorlibRef_v46 });
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void CallerInfoAttributesInvokedInFieldInitializer()
        {
            string source = @"
using System.Runtime.CompilerServices;

class P
{
    bool field = /*<bind>*/M2()/*</bind>*/;

    static bool M2(
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        return true;
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression (System.Boolean P.M2([System.String memberName = null], [System.String sourceFilePath = null], [System.Int32 sourceLineNumber = 0])) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'M2()') (Parent: FieldInitializer)
  Instance Receiver: 
    null
  Arguments(3):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: memberName) ([0] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""field"") (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sourceFilePath) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""file.cs"") (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sourceLineNumber) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 6) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalReferences: new[] { MscorlibRef_v46 });
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void CallerInfoAttributesInvokedInEventMethods()
        {
            string source = @"
using System;
using System.Runtime.CompilerServices;

class P
{
    public event EventHandler MyEvent
    {
        add
        {
            /*<bind>*/M2()/*</bind>*/;
        }

        remove
        {
            M2();
        }
    }

    static bool M2(
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        return true;
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression (System.Boolean P.M2([System.String memberName = null], [System.String sourceFilePath = null], [System.Int32 sourceLineNumber = 0])) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(3):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: memberName) ([0] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""MyEvent"") (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sourceFilePath) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""file.cs"") (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: sourceLineNumber) ([2] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 11) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalReferences: new[] { MscorlibRef_v46 });
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ExtraArgument()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2(1, 2)/*</bind>*/;
    }

    void M2(int x = 0)
    { }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'M2(1, 2)') (Parent: ExpressionStatement)
  Children(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1501: No overload for method 'M2' takes 2 arguments
                //         /*<bind>*/M2(1, 2)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadArgCount, "M2").WithArguments("M2", "2").WithLocation(6, 19)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TestOmittedArgument()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2(1,)/*</bind>*/;
    }

    void M2(int y, int x = 0)
    { }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'M2(1,)') (Parent: ExpressionStatement)
  Children(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    IInvalidExpression ([1] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(6,24): error CS1525: Invalid expression term ')'
                //         /*<bind>*/M2(1,)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(6, 24)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void WrongArgumentType()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2(1)/*</bind>*/;
    }

    void M2(string x )
    { }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'M2(1)') (Parent: ExpressionStatement)
  Children(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // file.cs(6,22): error CS1503: Argument 1: cannot convert from 'int' to 'string'
                //         /*<bind>*/M2(1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadArgType, "1").WithArguments("1", "int", "string").WithLocation(6, 22)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VarArgsCall()
        {
            string source = @"
using System;

public class P
{
    void M()
    {
        /*<bind>*/Console.Write(""{0} {1} {2} {3} {4}"", 1, 2, 3, 4, __arglist(5))/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void System.Console.Write(System.String format, System.Object arg0, System.Object arg1, System.Object arg2, System.Object arg3, __arglist)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... arglist(5))') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(6):
    IArgument (ArgumentKind.Explicit, Matching Parameter: format) ([0] OperationKind.Argument) (Syntax: Argument, '""{0} {1} {2} {3} {4}""')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""{0} {1} {2} {3} {4}"") (Syntax: StringLiteralExpression, '""{0} {1} {2} {3} {4}""')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: arg0) ([1] OperationKind.Argument) (Syntax: Argument, '1')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: NumericLiteralExpression, '1')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: arg1) ([2] OperationKind.Argument) (Syntax: Argument, '2')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: NumericLiteralExpression, '2')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: arg2) ([3] OperationKind.Argument) (Syntax: Argument, '3')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: NumericLiteralExpression, '3')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: arg3) ([4] OperationKind.Argument) (Syntax: Argument, '4')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: NumericLiteralExpression, '4')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.Explicit, Matching Parameter: null) ([5] OperationKind.Argument) (Syntax: Argument, '__arglist(5)')
      IOperation:  ([0] OperationKind.None) (Syntax: InvocationExpression, '__arglist(5)')
        Children(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void InvalidConversionForDefaultArgument_InSource()
        {
            string source = @"
class P
{
    void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }

    void M2(int x = ""string"")
    { }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M2([System.Int32 x = default(System.Int32)])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: IdentifierName, 'M2')
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1750: A value of type 'string' cannot be used as a default parameter because there are no standard conversions to type 'int'
                //     void M2(int x = "string")
                Diagnostic(ErrorCode.ERR_NoConversionForDefaultParam, "x").WithArguments("string", "int")
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AssigningToIndexer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int index]
    {
        get { return _number; }
        set { _number = value; }
    }

    void M1()
    {
        /*<bind>*/this[10]/*</bind>*/ = 9;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[System.Int32 index] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'this[10]') (Parent: SimpleAssignmentExpression)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(1):
    IArgument (ArgumentKind.Explicit, Matching Parameter: index) ([1] OperationKind.Argument) (Syntax: Argument, '10')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ReadingFromIndexer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int index]
    {
        get { return _number; }
        set { _number = value; }
    }

    void M1()
    {
        var x = /*<bind>*/this[10]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[System.Int32 index] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'this[10]') (Parent: VariableInitializer)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(1):
    IArgument (ArgumentKind.Explicit, Matching Parameter: index) ([1] OperationKind.Argument) (Syntax: Argument, '10')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultArgumentForIndexerGetter()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int i = 1, int j = 2]
    {
        get { return _number; }
        set { _number = i + j; }
    }

    void M1()
    {
        var x = /*<bind>*/this[j:10]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[[System.Int32 i = 1], [System.Int32 j = 2]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'this[j:10]') (Parent: VariableInitializer)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: j) ([1] OperationKind.Argument) (Syntax: Argument, 'j:10')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: i) ([2] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'this[j:10]')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: ElementAccessExpression, 'this[j:10]')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ReadingFromWriteOnlyIndexer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int index]
    {
        set { _number = value; }
    }

    void M1()
    {
        var x = /*<bind>*/this[10]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32, IsInvalid) (Syntax: ElementAccessExpression, 'this[10]') (Parent: InvalidExpression)
  Children(2):
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsInvalid) (Syntax: ThisExpression, 'this')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10, IsInvalid) (Syntax: NumericLiteralExpression, '10')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(12,27): error CS0154: The property or indexer 'P.this[int]' cannot be used in this context because it lacks the get accessor
                //         var x = /*<bind>*/this[10]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PropertyLacksGet, "this[10]").WithArguments("P.this[int]").WithLocation(12, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AssigningToReadOnlyIndexer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int index]
    {
        get { return _number; }
    }

    void M1()
    {
        /*<bind>*/this[10]/*</bind>*/ = 9;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32, IsInvalid) (Syntax: ElementAccessExpression, 'this[10]') (Parent: InvalidExpression)
  Children(2):
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsInvalid) (Syntax: ThisExpression, 'this')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10, IsInvalid) (Syntax: NumericLiteralExpression, '10')
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(12,19): error CS0200: Property or indexer 'P.this[int]' cannot be assigned to -- it is read only
                //         /*<bind>*/this[10]/*</bind>*/ = 9;
                Diagnostic(ErrorCode.ERR_AssgReadonlyProp, "this[10]").WithArguments("P.this[int]").WithLocation(12, 19)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void OverridingIndexerWithDefaultArgument()
        {
            string source = @"
class Base
{
    public virtual int this[int x = 0, int y = 1]
    {
        set { }
        get { System.Console.Write(y); return 0; }
    }
}

class Derived : Base
{
    public override int this[int x = 8, int y = 9]
    {
        set { }
    }
}

internal class P
{
    static void Main()
    {
        var d = new Derived();
        var x = /*<bind>*/d[0]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 Derived.this[[System.Int32 x = 8], [System.Int32 y = 9]] { set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'd[0]') (Parent: VariableInitializer)
  Instance Receiver: 
    ILocalReferenceExpression: d ([0] OperationKind.LocalReferenceExpression, Type: Derived) (Syntax: IdentifierName, 'd')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '0')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: y) ([2] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'd[0]')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: ElementAccessExpression, 'd[0]')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            string expectedOutput = @"1";

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);

            CompileAndVerify(new[] { source }, new[] { SystemRef }, expectedOutput: expectedOutput);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void OmittedParamArrayArgumentInIndexerAccess()
        {
            string source = @"
class P
{
    public int this[int x, params int[] y]
    {
        set { }
        get { return 0; }
    }

    public void M()
    {
        /*<bind>*/this[0]/*</bind>*/ = 0;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[System.Int32 x, params System.Int32[] y] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'this[0]') (Parent: SimpleAssignmentExpression)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '0')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.ParamArray, Matching Parameter: y) ([2] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'this[0]')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ElementAccessExpression, 'this[0]')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: ElementAccessExpression, 'this[0]')
        Initializer: 
          IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ElementAccessExpression, 'this[0]')
            Element Values(0)
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AssigningToReturnsByRefIndexer()
        {
            string source = @"
class P
{
    ref int this[int x]
    {
        get => throw null;
    }

    public void M()
    {
        /*<bind>*/this[0]/*</bind>*/ = 0;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: ref System.Int32 P.this[System.Int32 x] { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'this[0]') (Parent: SimpleAssignmentExpression)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(1):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '0')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [ClrOnlyFact(ClrOnlyReason.Ilasm)]
        public void AssigningToIndexer_UsingDefaultArgumentFromSetter()
        {
            var il = @"
.class public auto ansi beforefieldinit P
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) 
           = {string('Item')}

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method P::.ctor

  .method public hidebysig specialname instance int32 
          get_Item([opt] int32 i,
                   [opt] int32 j) cil managed
  {
    .param [1] = int32(0x00000001)
    .param [2] = int32(0x00000002)
    // Code size       35 (0x23)
    .maxstack  3
    .locals init ([0] int32 V_0)
    IL_0000:  nop
    IL_0001:  ldstr      ""{0} {1}""
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldarg.2
    IL_000d:  box        [mscorlib]System.Int32
    IL_0012:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001c:  nop
    IL_001d:  ldc.i4.0
    IL_001e:  stloc.0
    IL_001f:  br.s       IL_0021
    IL_0021:  ldloc.0
    IL_0022:  ret
  } // end of method P::get_Item

  .method public hidebysig specialname instance void
          set_Item([opt] int32 i,
                   [opt] int32 j,
                   int32 'value') cil managed
  {
    .param [1] = int32(0x00000003)
    .param [2] = int32(0x00000004)
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      ""{0} {1}""
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldarg.2
    IL_000d:  box        [mscorlib]System.Int32
    IL_0012:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method P::set_Item


  .property instance int32 Item(int32,
                                int32)
  {
    .get instance int32 P::get_Item(int32,
                                    int32)
    .set instance void P::set_Item(int32,
                                   int32,
                                   int32)
  } // end of property P::Item
} // end of class P
";

            var csharp = @"
class C
{
    public static void Main(string[] args)
    {
         P p = new P();
         /*<bind>*/p[10]/*</bind>*/ = 9;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[[System.Int32 i = 3], [System.Int32 j = 4]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'p[10]') (Parent: SimpleAssignmentExpression)
  Instance Receiver: 
    ILocalReferenceExpression: p ([0] OperationKind.LocalReferenceExpression, Type: P) (Syntax: IdentifierName, 'p')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([1] OperationKind.Argument) (Syntax: Argument, '10')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: j) ([2] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'p[10]')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: ElementAccessExpression, 'p[10]')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;
            var expectedOutput = @"10 4
";

            var ilReference = VerifyOperationTreeAndDiagnosticsForTestWithIL<ElementAccessExpressionSyntax>(csharp, il, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);

            CompileAndVerify(new[] { csharp }, new[] { SystemRef, ilReference }, expectedOutput: expectedOutput);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [ClrOnlyFact(ClrOnlyReason.Ilasm)]
        public void ReadFromIndexer_UsingDefaultArgumentFromGetter()
        {
            var il = @"
.class public auto ansi beforefieldinit P
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) 
           = {string('Item')}

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method P::.ctor

  .method public hidebysig specialname instance int32 
          get_Item([opt] int32 i,
                   [opt] int32 j) cil managed
  {
    .param [1] = int32(0x00000001)
    .param [2] = int32(0x00000002)
    // Code size       35 (0x23)
    .maxstack  3
    .locals init ([0] int32 V_0)
    IL_0000:  nop
    IL_0001:  ldstr      ""{0} {1}""
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldarg.2
    IL_000d:  box        [mscorlib]System.Int32
    IL_0012:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001c:  nop
    IL_001d:  ldc.i4.0
    IL_001e:  stloc.0
    IL_001f:  br.s       IL_0021
    IL_0021:  ldloc.0
    IL_0022:  ret
  } // end of method P::get_Item

  .method public hidebysig specialname instance void
          set_Item([opt] int32 i,
                   [opt] int32 j,
                   int32 'value') cil managed
  {
    .param [1] = int32(0x00000003)
    .param [2] = int32(0x00000004)
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      ""{0} {1}""
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldarg.2
    IL_000d:  box        [mscorlib]System.Int32
    IL_0012:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method P::set_Item


  .property instance int32 Item(int32,
                                int32)
  {
    .get instance int32 P::get_Item(int32,
                                    int32)
    .set instance void P::set_Item(int32,
                                   int32,
                                   int32)
  } // end of property P::Item
} // end of class P
";

            var csharp = @"
class C
{
    public static void Main(string[] args)
    {
         P p = new P();
         var x = /*<bind>*/p[10]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[[System.Int32 i = 3], [System.Int32 j = 4]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'p[10]') (Parent: VariableInitializer)
  Instance Receiver: 
    ILocalReferenceExpression: p ([0] OperationKind.LocalReferenceExpression, Type: P) (Syntax: IdentifierName, 'p')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([1] OperationKind.Argument) (Syntax: Argument, '10')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: j) ([2] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'p[10]')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: ElementAccessExpression, 'p[10]')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            var expectedOutput = @"10 2
";

            var ilReference = VerifyOperationTreeAndDiagnosticsForTestWithIL<ElementAccessExpressionSyntax>(csharp, il, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);

            CompileAndVerify(new[] { csharp }, new[] { SystemRef, ilReference }, expectedOutput: expectedOutput);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [ClrOnlyFact(ClrOnlyReason.Ilasm)]
        public void IndexerAccess_LHSOfCompoundAssignment()
        {
            var il = @"
.class public auto ansi beforefieldinit P
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) 
           = {string('Item')}

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method P::.ctor

  .method public hidebysig specialname instance int32 
          get_Item([opt] int32 i,
                   [opt] int32 j) cil managed
  {
    .param [1] = int32(0x00000001)
    .param [2] = int32(0x00000002)
    // Code size       35 (0x23)
    .maxstack  3
    .locals init ([0] int32 V_0)
    IL_0000:  nop
    IL_0001:  ldstr      ""{0} {1}""
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldarg.2
    IL_000d:  box        [mscorlib]System.Int32
    IL_0012:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001c:  nop
    IL_001d:  ldc.i4.0
    IL_001e:  stloc.0
    IL_001f:  br.s       IL_0021
    IL_0021:  ldloc.0
    IL_0022:  ret
  } // end of method P::get_Item

  .method public hidebysig specialname instance void
          set_Item([opt] int32 i,
                   [opt] int32 j,
                   int32 'value') cil managed
  {
    .param [1] = int32(0x00000003)
    .param [2] = int32(0x00000004)
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      ""{0} {1}""
    IL_0006:  ldarg.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldarg.2
    IL_000d:  box        [mscorlib]System.Int32
    IL_0012:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method P::set_Item


  .property instance int32 Item(int32,
                                int32)
  {
    .get instance int32 P::get_Item(int32,
                                    int32)
    .set instance void P::set_Item(int32,
                                   int32,
                                   int32)
  } // end of property P::Item
} // end of class P
";

            var csharp = @"
class C
{
    public static void Main(string[] args)
    {
         P p = new P();
         /*<bind>*/p[10]/*</bind>*/ += 99;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[[System.Int32 i = 3], [System.Int32 j = 4]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'p[10]') (Parent: CompoundAssignmentExpression)
  Instance Receiver: 
    ILocalReferenceExpression: p ([0] OperationKind.LocalReferenceExpression, Type: P) (Syntax: IdentifierName, 'p')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([1] OperationKind.Argument) (Syntax: Argument, '10')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: j) ([2] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'p[10]')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: ElementAccessExpression, 'p[10]')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            var expectedOutput = @"10 2
10 2
";

            var ilReference = VerifyOperationTreeAndDiagnosticsForTestWithIL<ElementAccessExpressionSyntax>(csharp, il, expectedOperationTree, expectedDiagnostics, additionalOperationTreeVerifier: IndexerAccessArgumentVerifier.Verify);

            CompileAndVerify(new[] { csharp }, new[] { SystemRef, ilReference }, expectedOutput: expectedOutput);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [ClrOnlyFact(ClrOnlyReason.Ilasm)]
        public void InvalidConversionForDefaultArgument_InIL()
        {
            var il = @"
.class public auto ansi beforefieldinit P
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method P::.ctor

  .method public hidebysig instance void  M1([opt] int32 s) cil managed
  {
    .param [1] = ""abc""
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000: nop
    IL_0001:  ret
  } // end of method P::M1
} // end of class P
";

            var csharp = @"
class C
{
    public void M2()
    {
         P p = new P();
         /*<bind>*/p.M1()/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression ( void P.M1([System.Int32 s = ""abc""])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'p.M1()') (Parent: ExpressionStatement)
  Instance Receiver: 
    ILocalReferenceExpression: p ([0] OperationKind.LocalReferenceExpression, Type: P) (Syntax: IdentifierName, 'p')
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: s) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'p.M1()')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: InvocationExpression, 'p.M1()')
        Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""abc"") (Syntax: InvocationExpression, 'p.M1()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTestWithIL<InvocationExpressionSyntax>(csharp, il, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(20330, "https://github.com/dotnet/roslyn/issues/20330")]
        public void DefaultValueNonNullForNullableParameterTypeWithMissingNullableReference_Call()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }

    static void M2(bool? x = true)
    {
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2([System.Boolean[missing]? x = true])) ([0] OperationKind.InvocationExpression, Type: System.Void[missing], IsInvalid) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([0] OperationKind.Argument, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Boolean[missing]?, IsInvalid) (Syntax: InvocationExpression, 'M2()')
        Children(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean[missing], Constant: True, IsInvalid) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // (2,7): error CS0518: Predefined type 'System.Object' is not defined or imported
                // class P
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(2, 7),
                // (9,20): error CS0518: Predefined type 'System.Nullable`1' is not defined or imported
                //     static void M2(bool? x = true)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "bool?").WithArguments("System.Nullable`1").WithLocation(9, 20),
                // (9,20): error CS0518: Predefined type 'System.Boolean' is not defined or imported
                //     static void M2(bool? x = true)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "bool").WithArguments("System.Boolean").WithLocation(9, 20),
                // (9,12): error CS0518: Predefined type 'System.Void' is not defined or imported
                //     static void M2(bool? x = true)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "void").WithArguments("System.Void").WithLocation(9, 12),
                // (4,12): error CS0518: Predefined type 'System.Void' is not defined or imported
                //     static void M1()
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "void").WithArguments("System.Void").WithLocation(4, 12),
                // (9,30): error CS0518: Predefined type 'System.Boolean' is not defined or imported
                //     static void M2(bool? x = true)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "true").WithArguments("System.Boolean").WithLocation(9, 30),
                // (6,19): error CS0518: Predefined type 'System.Object' is not defined or imported
                //         /*<bind>*/M2()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "M2").WithArguments("System.Object").WithLocation(6, 19),
                // (2,7): error CS1729: 'object' does not contain a constructor that takes 0 arguments
                // class P
                Diagnostic(ErrorCode.ERR_BadCtorArgCount, "P").WithArguments("object", "0").WithLocation(2, 7)
            };

            var compilation = CreateCompilation(source, options: Test.Utilities.TestOptions.ReleaseDll);
            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(compilation, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(20330, "https://github.com/dotnet/roslyn/issues/20330")]
        public void DefaultValueNonNullForNullableParameterTypeWithMissingNullableReference_ObjectCreation()
        {
            string source = @"
class P
{
    static P M1()
    {
        return /*<bind>*/new P()/*</bind>*/;
    }

    P(bool? x = true)
    {
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: P..ctor([System.Boolean[missing]? x = true])) ([0] OperationKind.ObjectCreationExpression, Type: P, IsInvalid) (Syntax: ObjectCreationExpression, 'new P()') (Parent: ReturnStatement)
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([0] OperationKind.Argument, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'new P()')
      IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Boolean[missing]?, IsInvalid) (Syntax: ObjectCreationExpression, 'new P()')
        Children(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean[missing], Constant: True, IsInvalid) (Syntax: ObjectCreationExpression, 'new P()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Initializer: 
    null
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // (2,7): error CS0518: Predefined type 'System.Object' is not defined or imported
                // class P
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(2, 7),
                // (4,12): error CS0518: Predefined type 'System.Object' is not defined or imported
                //     static P M1()
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(4, 12),
                // (9,7): error CS0518: Predefined type 'System.Nullable`1' is not defined or imported
                //     P(bool? x = true)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "bool?").WithArguments("System.Nullable`1").WithLocation(9, 7),
                // (9,7): error CS0518: Predefined type 'System.Boolean' is not defined or imported
                //     P(bool? x = true)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "bool").WithArguments("System.Boolean").WithLocation(9, 7),
                // (9,5): error CS0518: Predefined type 'System.Void' is not defined or imported
                //     P(bool? x = true)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, @"P(bool? x = true)
    {
    }").WithArguments("System.Void").WithLocation(9, 5),
                // (9,17): error CS0518: Predefined type 'System.Boolean' is not defined or imported
                //     P(bool? x = true)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "true").WithArguments("System.Boolean").WithLocation(9, 17),
                // (6,30): error CS0518: Predefined type 'System.Object' is not defined or imported
                //         return /*<bind>*/new P()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(6, 30),
                // (9,5): error CS1729: 'object' does not contain a constructor that takes 0 arguments
                //     P(bool? x = true)
                Diagnostic(ErrorCode.ERR_BadCtorArgCount, "P").WithArguments("object", "0").WithLocation(9, 5)
            };

            var compilation = CreateCompilation(source, options: Test.Utilities.TestOptions.ReleaseDll);
            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(compilation, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(20330, "https://github.com/dotnet/roslyn/issues/20330")]
        public void DefaultValueNonNullForNullableParameterTypeWithMissingNullableReference_Indexer()
        {
            string source = @"

class P
{
    private int _number = 0;
    public int this[int x, int? y = 5]
    {
        get { return _number; }
        set { _number = value; }
    }

    void M1()
    {
        /*<bind>*/this[0]/*</bind>*/ = 9;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32[missing] P.this[System.Int32[missing] x, [System.Int32[missing]? y = 5]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32[missing], IsInvalid) (Syntax: ElementAccessExpression, 'this[0]') (Parent: SimpleAssignmentExpression)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument, IsInvalid) (Syntax: Argument, '0')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32[missing], Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: y) ([2] OperationKind.Argument, IsInvalid, IsImplicit) (Syntax: ElementAccessExpression, 'this[0]')
      IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32[missing]?, IsInvalid) (Syntax: ElementAccessExpression, 'this[0]')
        Children(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32[missing], Constant: 5, IsInvalid) (Syntax: ElementAccessExpression, 'this[0]')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // (3,7): error CS0518: Predefined type 'System.Object' is not defined or imported
                // class P
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(3, 7),
                // (6,21): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = 5]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(6, 21),
                // (6,28): error CS0518: Predefined type 'System.Nullable`1' is not defined or imported
                //     public int this[int x, int? y = 5]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int?").WithArguments("System.Nullable`1").WithLocation(6, 28),
                // (6,28): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = 5]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(6, 28),
                // (6,12): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = 5]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(6, 12),
                // (9,9): error CS0518: Predefined type 'System.Void' is not defined or imported
                //         set { _number = value; }
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "set { _number = value; }").WithArguments("System.Void").WithLocation(9, 9),
                // (12,5): error CS0518: Predefined type 'System.Void' is not defined or imported
                //     void M1()
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "void").WithArguments("System.Void").WithLocation(12, 5),
                // (6,37): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = 5]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "5").WithArguments("System.Int32").WithLocation(6, 37),
                // (5,13): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     private int _number = 0;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 13),
                // (5,27): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     private int _number = 0;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(5, 27),
                // (14,24): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //         /*<bind>*/this[0]/*</bind>*/ = 9;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(14, 24),
                // (14,40): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //         /*<bind>*/this[0]/*</bind>*/ = 9;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "9").WithArguments("System.Int32").WithLocation(14, 40),
                // (3,7): error CS1729: 'object' does not contain a constructor that takes 0 arguments
                // class P
                Diagnostic(ErrorCode.ERR_BadCtorArgCount, "P").WithArguments("object", "0").WithLocation(3, 7)
            };

            var compilation = CreateCompilation(source, options: Test.Utilities.TestOptions.ReleaseDll);
            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(compilation, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(20330, "https://github.com/dotnet/roslyn/issues/20330")]
        public void DefaultValueNullForNullableParameterTypeWithMissingNullableReference_Call()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }

    static void M2(bool? x = null)
    {
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2([System.Boolean[missing]? x = null])) ([0] OperationKind.InvocationExpression, Type: System.Void[missing], IsInvalid) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([0] OperationKind.Argument, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      IDefaultValueExpression ([0] OperationKind.DefaultValueExpression, Type: System.Boolean[missing]?, IsInvalid) (Syntax: InvocationExpression, 'M2()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // (2,7): error CS0518: Predefined type 'System.Object' is not defined or imported
                // class P
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(2, 7),
                // (9,20): error CS0518: Predefined type 'System.Nullable`1' is not defined or imported
                //     static void M2(bool? x = null)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "bool?").WithArguments("System.Nullable`1").WithLocation(9, 20),
                // (9,20): error CS0518: Predefined type 'System.Boolean' is not defined or imported
                //     static void M2(bool? x = null)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "bool").WithArguments("System.Boolean").WithLocation(9, 20),
                // (9,12): error CS0518: Predefined type 'System.Void' is not defined or imported
                //     static void M2(bool? x = null)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "void").WithArguments("System.Void").WithLocation(9, 12),
                // (4,12): error CS0518: Predefined type 'System.Void' is not defined or imported
                //     static void M1()
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "void").WithArguments("System.Void").WithLocation(4, 12),
                // (6,19): error CS0518: Predefined type 'System.Object' is not defined or imported
                //         /*<bind>*/M2()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "M2").WithArguments("System.Object").WithLocation(6, 19),
                // (2,7): error CS1729: 'object' does not contain a constructor that takes 0 arguments
                // class P
                Diagnostic(ErrorCode.ERR_BadCtorArgCount, "P").WithArguments("object", "0").WithLocation(2, 7)
            };
            
            var compilation = CreateCompilation(source, options: Test.Utilities.TestOptions.ReleaseDll);
            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(compilation, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(20330, "https://github.com/dotnet/roslyn/issues/20330")]
        public void DefaultValueNullForNullableParameterTypeWithMissingNullableReference_ObjectCreation()
        {
            string source = @"
class P
{
    static P M1()
    {
        return /*<bind>*/new P()/*</bind>*/;
    }

    P(bool? x = null)
    {
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: P..ctor([System.Boolean[missing]? x = null])) ([0] OperationKind.ObjectCreationExpression, Type: P, IsInvalid) (Syntax: ObjectCreationExpression, 'new P()') (Parent: ReturnStatement)
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([0] OperationKind.Argument, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'new P()')
      IDefaultValueExpression ([0] OperationKind.DefaultValueExpression, Type: System.Boolean[missing]?, IsInvalid) (Syntax: ObjectCreationExpression, 'new P()')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Initializer: 
    null
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // (2,7): error CS0518: Predefined type 'System.Object' is not defined or imported
                // class P
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(2, 7),
                // (4,12): error CS0518: Predefined type 'System.Object' is not defined or imported
                //     static P M1()
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(4, 12),
                // (9,7): error CS0518: Predefined type 'System.Nullable`1' is not defined or imported
                //     P(bool? x = null)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "bool?").WithArguments("System.Nullable`1").WithLocation(9, 7),
                // (9,7): error CS0518: Predefined type 'System.Boolean' is not defined or imported
                //     P(bool? x = null)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "bool").WithArguments("System.Boolean").WithLocation(9, 7),
                // (9,5): error CS0518: Predefined type 'System.Void' is not defined or imported
                //     P(bool? x = null)
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, @"P(bool? x = null)
    {
    }").WithArguments("System.Void").WithLocation(9, 5),
                // (6,30): error CS0518: Predefined type 'System.Object' is not defined or imported
                //         return /*<bind>*/new P()/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(6, 30),
                // (9,5): error CS1729: 'object' does not contain a constructor that takes 0 arguments
                //     P(bool? x = null)
                Diagnostic(ErrorCode.ERR_BadCtorArgCount, "P").WithArguments("object", "0").WithLocation(9, 5)
            };

            var compilation = CreateCompilation(source, options: Test.Utilities.TestOptions.ReleaseDll);
            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(compilation, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(20330, "https://github.com/dotnet/roslyn/issues/20330")]
        public void DefaultValueNullForNullableParameterTypeWithMissingNullableReference_Indexer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int x, int? y = null]
    {
        get { return _number; }
        set { _number = value; }
    }

    void M1()
    {
        /*<bind>*/this[0]/*</bind>*/ = 9;
    }
}
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32[missing] P.this[System.Int32[missing] x, [System.Int32[missing]? y = null]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32[missing], IsInvalid) (Syntax: ElementAccessExpression, 'this[0]') (Parent: SimpleAssignmentExpression)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument, IsInvalid) (Syntax: Argument, '0')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32[missing], Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: y) ([2] OperationKind.Argument, IsInvalid, IsImplicit) (Syntax: ElementAccessExpression, 'this[0]')
      IDefaultValueExpression ([0] OperationKind.DefaultValueExpression, Type: System.Int32[missing]?, IsInvalid) (Syntax: ElementAccessExpression, 'this[0]')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // (16,1): error CS1022: Type or namespace definition, or end-of-file expected
                // }
                Diagnostic(ErrorCode.ERR_EOFExpected, "}").WithLocation(16, 1),
                // (2,7): error CS0518: Predefined type 'System.Object' is not defined or imported
                // class P
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(2, 7),
                // (5,21): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = null]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 21),
                // (5,28): error CS0518: Predefined type 'System.Nullable`1' is not defined or imported
                //     public int this[int x, int? y = null]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int?").WithArguments("System.Nullable`1").WithLocation(5, 28),
                // (5,28): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = null]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 28),
                // (5,12): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = null]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 12),
                // (8,9): error CS0518: Predefined type 'System.Void' is not defined or imported
                //         set { _number = value; }
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "set { _number = value; }").WithArguments("System.Void").WithLocation(8, 9),
                // (11,5): error CS0518: Predefined type 'System.Void' is not defined or imported
                //     void M1()
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "void").WithArguments("System.Void").WithLocation(11, 5),
                // (4,13): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     private int _number = 0;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(4, 13),
                // (4,27): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     private int _number = 0;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(4, 27),
                // (13,24): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //         /*<bind>*/this[0]/*</bind>*/ = 9;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(13, 24),
                // (13,40): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //         /*<bind>*/this[0]/*</bind>*/ = 9;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "9").WithArguments("System.Int32").WithLocation(13, 40),
                // (2,7): error CS1729: 'object' does not contain a constructor that takes 0 arguments
                // class P
                Diagnostic(ErrorCode.ERR_BadCtorArgCount, "P").WithArguments("object", "0").WithLocation(2, 7)
            };

            var compilation = CreateCompilation(source, options: Test.Utilities.TestOptions.ReleaseDll);
            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(compilation, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(20330, "https://github.com/dotnet/roslyn/issues/20330")]
        public void DefaultValueWithParameterErrorType_Call()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(1)/*</bind>*/;
    }

    static void M2(int x, S s = 0)
    {
    }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2(System.Int32 x, [S s = null])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: s) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(1)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: S) (Syntax: InvocationExpression, 'M2(1)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                      // file.cs(9,27): error CS0246: The type or namespace name 'S' could not be found (are you missing a using directive or an assembly reference?)
                      //     static void M2(int x, S s = 0)
                      Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "S").WithArguments("S").WithLocation(9, 27),
                      // file.cs(9,29): error CS1750: A value of type 'int' cannot be used as a default parameter because there are no standard conversions to type 'S'
                      //     static void M2(int x, S s = 0)
                      Diagnostic(ErrorCode.ERR_NoConversionForDefaultParam, "s").WithArguments("int", "S").WithLocation(9, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueWithParameterErrorType_ObjectCreation()
        {
            string source = @"
class P
{
    static P M1()
    {
        return /*<bind>*/new P(1)/*</bind>*/;
    }

    P(int x, S s = 0)
    {
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: P..ctor(System.Int32 x, [S s = null])) ([0] OperationKind.ObjectCreationExpression, Type: P) (Syntax: ObjectCreationExpression, 'new P(1)') (Parent: ReturnStatement)
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: s) ([1] OperationKind.Argument, IsImplicit) (Syntax: ObjectCreationExpression, 'new P(1)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: S) (Syntax: ObjectCreationExpression, 'new P(1)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Initializer: 
    null
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(9,14): error CS0246: The type or namespace name 'S' could not be found (are you missing a using directive or an assembly reference?)
                //     P(int x, S s = 0)
                Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "S").WithArguments("S").WithLocation(9, 14),
                // file.cs(9,16): error CS1750: A value of type 'int' cannot be used as a default parameter because there are no standard conversions to type 'S'
                //     P(int x, S s = 0)
                Diagnostic(ErrorCode.ERR_NoConversionForDefaultParam, "s").WithArguments("int", "S").WithLocation(9, 16)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueWithParameterErrorType_Indexer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int index, S s = 0]
    {
        get { return _number; }
        set { _number = value; }
    }

    void M1()
    {
        /*<bind>*/this[0]/*</bind>*/ = 9;
    }
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[System.Int32 index, [S s = null]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'this[0]') (Parent: SimpleAssignmentExpression)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: index) ([1] OperationKind.Argument) (Syntax: Argument, '0')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: s) ([2] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'this[0]')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: S) (Syntax: ElementAccessExpression, 'this[0]')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(5,32): error CS0246: The type or namespace name 'S' could not be found (are you missing a using directive or an assembly reference?)
                //     public int this[int index, S s = 0]
                Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "S").WithArguments("S").WithLocation(5, 32),
                // file.cs(5,34): error CS1750: A value of type 'int' cannot be used as a default parameter because there are no standard conversions to type 'S'
                //     public int this[int index, S s = 0]
                Diagnostic(ErrorCode.ERR_NoConversionForDefaultParam, "s").WithArguments("int", "S").WithLocation(5, 34)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(18722, "https://github.com/dotnet/roslyn/issues/18722")]
        public void DefaultValueForGenericWithUndefinedTypeArgument()
        {
            // TODO: https://github.com/dotnet/roslyn/issues/18722
            //       This should be treated as invalid invocation.
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(1)/*</bind>*/;
    }

    static void M2(int x, G<S> s = null)
    {
    }
}

class G<T>
{
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2(System.Int32 x, [G<S> s = null])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: s) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(1)')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: G<S>, Constant: null) (Syntax: InvocationExpression, 'M2(1)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                      // file.cs(9,29): error CS0246: The type or namespace name 'S' could not be found (are you missing a using directive or an assembly reference?)
                      //     static void M2(int x, G<S> s = null)
                      Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "S").WithArguments("S").WithLocation(9, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(18722, "https://github.com/dotnet/roslyn/issues/18722")]
        public void DefaultValueForNullableGenericWithUndefinedTypeArgument()
        {
            // TODO: https://github.com/dotnet/roslyn/issues/18722
            //       This should be treated as invalid invocation.
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(1)/*</bind>*/;
    }

    static void M2(int x, G<S>? s = null)
    {
    }
}

struct G<T>
{
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2(System.Int32 x, [G<S>? s = null])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(2):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '1')
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: s) ([1] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(1)')
      IDefaultValueExpression ([0] OperationKind.DefaultValueExpression, Type: G<S>?) (Syntax: InvocationExpression, 'M2(1)')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                      // file.cs(9,29): error CS0246: The type or namespace name 'S' could not be found (are you missing a using directive or an assembly reference?)
                      //     static void M2(int x, G<S> s = null)
                      Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "S").WithArguments("S").WithLocation(9, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }


        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void GettingInOutConversionFromCSharpArgumentShouldThrowException()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(1)/*</bind>*/;
    }

    static void M2(int x)
    {
    }
}
";
            var compilation = CreateStandardCompilation(source);
            var (operation, syntaxNode) = GetOperationAndSyntaxForTest<InvocationExpressionSyntax>(compilation);

            var invocation = (IInvocationExpression)operation;
            var argument = invocation.Arguments[0];

            // We are calling VB extension methods on IArgument in C# code, therefore exception is expected here.
            Assert.Throws<ArgumentException>(() => argument.GetInConversion());
            Assert.Throws<ArgumentException>(() => argument.GetOutConversion());
        }

        [Fact]
        public void DirectlyBindArgument_InvocationExpression()
        {
            string source = @"
class P
{
    static void M1()
    {
        M2(/*<bind>*/1/*</bind>*/);
    }
    static void M2(int i) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([0] OperationKind.Argument) (Syntax: Argument, '1') (Parent: InvocationExpression)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindRefArgument_InvocationExpression()
        {
            string source = @"
class P
{
    static void M1()
    {
        int i = 0;
        M2(/*<bind>*/ref i/*</bind>*/);
    }
    static void M2(ref int i) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([0] OperationKind.Argument) (Syntax: Argument, 'ref i') (Parent: InvocationExpression)
  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindInArgument_InvocationExpression()
        {
            string source = @"
class P
{
    static void M1()
    {
        int i = 0;
        ref int refI = ref i;
        M2(/*<bind>*/refI/*</bind>*/);
    }
    static void M2(in int i) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([0] OperationKind.Argument) (Syntax: Argument, 'refI') (Parent: InvocationExpression)
  ILocalReferenceExpression: refI ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'refI')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindOutArgument_InvocationExpression()
        {
            string source = @"
class P
{
    static void M1()
    {
        int i = 0;
        M2(/*<bind>*/out i/*</bind>*/);
    }
    static void M2(out int i) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([0] OperationKind.Argument) (Syntax: Argument, 'out i') (Parent: InvocationExpression)
  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0177: The out parameter 'i' must be assigned to before control leaves the current method
                //     static void M2(out int i) { }
                Diagnostic(ErrorCode.ERR_ParamUnassigned, "M2").WithArguments("i").WithLocation(9, 17)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindParamsArgument1_InvocationExpression()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(1);/*</bind>*/
    }
    static void M2(params int[] array) { }
}
";
            string expectedOperationTree = @"
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'M2(1);') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (void P.M2(params System.Int32[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(1)')
      Instance Receiver: 
        null
      Arguments(1):
        IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([0] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(1)')
          IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: InvocationExpression, 'M2(1)')
            Dimension Sizes(1):
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: InvocationExpression, 'M2(1)')
            Initializer: 
              IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: InvocationExpression, 'M2(1)')
                Element Values(1):
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ExpressionStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindParamsArgument2_InvocationExpression()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(0, 1);/*</bind>*/
    }
    static void M2(params int[] array) { }
}
";
            string expectedOperationTree = @"
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'M2(0, 1);') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (void P.M2(params System.Int32[] array)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2(0, 1)')
      Instance Receiver: 
        null
      Arguments(1):
        IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([0] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2(0, 1)')
          IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: InvocationExpression, 'M2(0, 1)')
            Dimension Sizes(1):
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: InvocationExpression, 'M2(0, 1)')
            Initializer: 
              IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: InvocationExpression, 'M2(0, 1)')
                Element Values(2):
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ExpressionStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindNamedArgument1_InvocationExpression()
        {
            string source = @"
class P
{
    static void M1()
    {
        M2(/*<bind>*/j: 1/*</bind>*/, i: 1);
    }
    static void M2(int i, int j) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: j) ([0] OperationKind.Argument) (Syntax: Argument, 'j: 1') (Parent: InvocationExpression)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindNamedArgument2_InvocationExpression()
        {
            string source = @"
class P
{
    static void M1()
    {
        M2(j: 1, /*<bind>*/i: 1/*</bind>*/);
    }
    static void M2(int i, int j) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([1] OperationKind.Argument) (Syntax: Argument, 'i: 1') (Parent: InvocationExpression)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindArgument_ObjectCreation()
        {
            string source = @"
class P
{
    static void M1()
    {
        new P(/*<bind>*/1/*</bind>*/);
    }
    public P(int i) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([0] OperationKind.Argument) (Syntax: Argument, '1') (Parent: ObjectCreationExpression)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindRefArgument_ObjectCreation()
        {
            string source = @"
class P
{
    static void M1()
    {
        int i = 0;
        new P(/*<bind>*/ref i/*</bind>*/);
    }
    public P(ref int i) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([0] OperationKind.Argument) (Syntax: Argument, 'ref i') (Parent: ObjectCreationExpression)
  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindOutArgument_ObjectCreation()
        {
            string source = @"
class P
{
    static void M1()
    {
        int i = 0;
        new P(/*<bind>*/out i/*</bind>*/);
    }
    public P(out int i) { }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([0] OperationKind.Argument) (Syntax: Argument, 'out i') (Parent: ObjectCreationExpression)
  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0177: The out parameter 'i' must be assigned to before control leaves the current method
                //     public P(out int i) { }
                Diagnostic(ErrorCode.ERR_ParamUnassigned, "P").WithArguments("i").WithLocation(9, 12)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindParamsArgument1_ObjectCreation()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/new P(1);/*</bind>*/
    }
    public P(params int[] array) { }
}
";
            string expectedOperationTree = @"
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'new P(1);') (Parent: BlockStatement)
  Expression: 
    IObjectCreationExpression (Constructor: P..ctor(params System.Int32[] array)) ([0] OperationKind.ObjectCreationExpression, Type: P) (Syntax: ObjectCreationExpression, 'new P(1)')
      Arguments(1):
        IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([0] OperationKind.Argument, IsImplicit) (Syntax: ObjectCreationExpression, 'new P(1)')
          IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ObjectCreationExpression, 'new P(1)')
            Dimension Sizes(1):
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: ObjectCreationExpression, 'new P(1)')
            Initializer: 
              IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ObjectCreationExpression, 'new P(1)')
                Element Values(1):
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Initializer: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ExpressionStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindParamsArgument2_ObjectCreation()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/new P(0, 1);/*</bind>*/
    }
    public P(params int[] array) { }
}
";
            string expectedOperationTree = @"
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'new P(0, 1);') (Parent: BlockStatement)
  Expression: 
    IObjectCreationExpression (Constructor: P..ctor(params System.Int32[] array)) ([0] OperationKind.ObjectCreationExpression, Type: P) (Syntax: ObjectCreationExpression, 'new P(0, 1)')
      Arguments(1):
        IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([0] OperationKind.Argument, IsImplicit) (Syntax: ObjectCreationExpression, 'new P(0, 1)')
          IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ObjectCreationExpression, 'new P(0, 1)')
            Dimension Sizes(1):
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: ObjectCreationExpression, 'new P(0, 1)')
            Initializer: 
              IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ObjectCreationExpression, 'new P(0, 1)')
                Element Values(2):
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Initializer: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ExpressionStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindArgument_Indexer()
        {
            string source = @"
class P
{
    void M1()
    {
        var v = this[/*<bind>*/1/*</bind>*/];
    }
    public int this[int i] => 0;
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([1] OperationKind.Argument) (Syntax: Argument, '1') (Parent: PropertyReferenceExpression)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindParamsArgument1_Indexer()
        {
            string source = @"
class P
{
    void M1()
    {
        var v = /*<bind>*/this[1]/*</bind>*/;
    }
    public int this[params int[] array] => 0;
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[params System.Int32[] array] { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'this[1]') (Parent: VariableInitializer)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(1):
    IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([1] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'this[1]')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ElementAccessExpression, 'this[1]')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: ElementAccessExpression, 'this[1]')
        Initializer: 
          IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ElementAccessExpression, 'this[1]')
            Element Values(1):
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindParamsArgument2_Indexer()
        {
            string source = @"
class P
{
    void M1()
    {
        var v = /*<bind>*/this[0, 1]/*</bind>*/;
    }
    public int this[params int[] array] => 0;
}
";
            string expectedOperationTree = @"
IPropertyReferenceExpression: System.Int32 P.this[params System.Int32[] array] { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'this[0, 1]') (Parent: VariableInitializer)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P) (Syntax: ThisExpression, 'this')
  Arguments(1):
    IArgument (ArgumentKind.ParamArray, Matching Parameter: array) ([1] OperationKind.Argument, IsImplicit) (Syntax: ElementAccessExpression, 'this[0, 1]')
      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ElementAccessExpression, 'this[0, 1]')
        Dimension Sizes(1):
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: ElementAccessExpression, 'this[0, 1]')
        Initializer: 
          IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ElementAccessExpression, 'this[0, 1]')
            Element Values(2):
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindArgument_Attribute()
        {
            string source = @"
[assembly: /*<bind>*/System.CLSCompliant(isCompliant: true)/*</bind>*/]
";
            string expectedOperationTree = @"
IOperation:  ([Root] OperationKind.None) (Syntax: Attribute, 'System.CLSC ... iant: true)') (Parent: )
  Children(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'true')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<AttributeSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindArgument2_Attribute()
        {
            string source = @"
[assembly: MyA(/*<bind>*/Prop = ""test""/*</bind>*/)]

class MyA : System.Attribute
{
    public string Prop {get;set;}
}
";
            string expectedOperationTree = @"
ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.String) (Syntax: AttributeArgument, 'Prop = ""test""') (Parent: None)
  Left: 
    IPropertyReferenceExpression: System.String MyA.Prop { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'Prop')
      Instance Receiver: 
        null
  Right: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: ""test"") (Syntax: StringLiteralExpression, '""test""')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<AttributeArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact]
        public void DirectlyBindArgument_NonTrailingNamedArgument()
        {
            string source = @"
class P
{
    void M1(int i, int i2)
    {
        M1(i: 0, /*<bind>*/2/*</bind>*/);
    }
}
";
            string expectedOperationTree = @"
IArgument (ArgumentKind.Explicit, Matching Parameter: i2) ([2] OperationKind.Argument) (Syntax: Argument, '2') (Parent: InvocationExpression)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArgumentSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void NonNullDefaultValueForNullableParameterType()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2()/*</bind>*/;
    }
    static void M2(int? x = 10) { }
}
";
            string expectedOperationTree = @"
IInvocationExpression (void P.M2([System.Int32? x = 10])) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'M2()') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(1):
    IArgument (ArgumentKind.DefaultValue, Matching Parameter: x) ([0] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
      IObjectCreationExpression (Constructor: System.Int32?..ctor(System.Int32 value)) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32?) (Syntax: InvocationExpression, 'M2()')
        Arguments(1):
          IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument, IsImplicit) (Syntax: InvocationExpression, 'M2()')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: InvocationExpression, 'M2()')
            InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Initializer: 
          null
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;
            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, useLatestFrameworkReferences: true);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void AssigningToReadOnlyIndexerInObjectCreationInitializer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int index]
    {
        get { return _number; }
    }

    P M1()
    {
        return /*<bind>*/new P() { [0] = 1 }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: P..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: P, IsInvalid) (Syntax: ObjectCreationExpression, 'new P() { [0] = 1 }') (Parent: ReturnStatement)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: P, IsInvalid) (Syntax: ObjectInitializerExpression, '{ [0] = 1 }')
      Initializers(1):
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32, IsInvalid) (Syntax: SimpleAssignmentExpression, '[0] = 1')
          Left: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32, IsInvalid) (Syntax: ImplicitElementAccess, '[0]')
              Children(1):
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(12,36): error CS0200: Property or indexer 'P.this[int]' cannot be assigned to -- it is read only
                //         return /*<bind>*/new P() { [0] = 1 }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_AssgReadonlyProp, "[0]").WithArguments("P.this[int]").WithLocation(12, 36)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void WrongSignatureIndexerInObjectCreationInitializer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[string name]
    {
        get { return _number; }
        set { _number = value; }
    }

    P M1()
    {
        return /*<bind>*/new P() { [0] = 1 }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: P..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: P, IsInvalid) (Syntax: ObjectCreationExpression, 'new P() { [0] = 1 }') (Parent: ReturnStatement)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: P, IsInvalid) (Syntax: ObjectInitializerExpression, '{ [0] = 1 }')
      Initializers(1):
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32, IsInvalid) (Syntax: SimpleAssignmentExpression, '[0] = 1')
          Left: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32, IsInvalid) (Syntax: ImplicitElementAccess, '[0]')
              Children(1):
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(13,37): error CS1503: Argument 1: cannot convert from 'int' to 'string'
                //         return /*<bind>*/new P() { [0] = 1 }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadArgType, "0").WithArguments("1", "int", "string").WithLocation(13, 37)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueNonNullForNullableParameterTypeWithMissingNullableReference_IndexerInObjectCreationInitializer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int x, int? y = 0]
    {
        get { return _number; }
        set { _number = value; }
    }

    P M1()
    {
        return /*<bind>*/new P() { [0] = 1 }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: P, IsInvalid) (Syntax: ObjectCreationExpression, 'new P() { [0] = 1 }') (Parent: ReturnStatement)
  Children(1):
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: P, IsInvalid) (Syntax: ObjectInitializerExpression, '{ [0] = 1 }')
      Initializers(1):
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32[missing], IsInvalid) (Syntax: SimpleAssignmentExpression, '[0] = 1')
          Left: 
            IPropertyReferenceExpression: System.Int32[missing] P.this[System.Int32[missing] x, [System.Int32[missing]? y = 0]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32[missing], IsInvalid) (Syntax: ImplicitElementAccess, '[0]')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsInvalid, IsImplicit) (Syntax: ImplicitElementAccess, '[0]')
              Arguments(2):
                IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument, IsInvalid) (Syntax: Argument, '0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32[missing], Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                IArgument (ArgumentKind.DefaultValue, Matching Parameter: y) ([2] OperationKind.Argument, IsInvalid, IsImplicit) (Syntax: ImplicitElementAccess, '[0]')
                  IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32[missing]?, IsInvalid) (Syntax: ImplicitElementAccess, '[0]')
                    Children(1):
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32[missing], Constant: 0, IsInvalid) (Syntax: ImplicitElementAccess, '[0]')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32[missing], Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // (2,7): error CS0518: Predefined type 'System.Object' is not defined or imported
                // class P
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(2, 7),
                // (5,21): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = 0]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 21),
                // (5,28): error CS0518: Predefined type 'System.Nullable`1' is not defined or imported
                //     public int this[int x, int? y = 0]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int?").WithArguments("System.Nullable`1").WithLocation(5, 28),
                // (5,28): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = 0]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 28),
                // (5,12): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = 0]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 12),
                // (8,9): error CS0518: Predefined type 'System.Void' is not defined or imported
                //         set { _number = value; }
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "set { _number = value; }").WithArguments("System.Void").WithLocation(8, 9),
                // (11,5): error CS0518: Predefined type 'System.Object' is not defined or imported
                //     P M1()
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(11, 5),
                // (5,37): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = 0]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(5, 37),
                // (4,13): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     private int _number = 0;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(4, 13),
                // (4,27): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     private int _number = 0;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(4, 27),
                // (13,30): error CS0518: Predefined type 'System.Object' is not defined or imported
                //         return /*<bind>*/new P() { [0] = 1 };/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(13, 30),
                // (13,37): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //         return /*<bind>*/new P() { [0] = 1 };/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(13, 37),
                // (13,42): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //         return /*<bind>*/new P() { [0] = 1 };/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "1").WithArguments("System.Int32").WithLocation(13, 42),
                // (13,30): error CS0518: Predefined type 'System.Void' is not defined or imported
                //         return /*<bind>*/new P() { [0] = 1 };/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Void").WithLocation(13, 30),
                // (2,7): error CS1729: 'object' does not contain a constructor that takes 0 arguments
                // class P
                Diagnostic(ErrorCode.ERR_BadCtorArgCount, "P").WithArguments("object", "0").WithLocation(2, 7)
            };

            var compilation = CreateCompilation(source, options: Test.Utilities.TestOptions.ReleaseDll);
            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(compilation, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueNullForNullableParameterTypeWithMissingNullableReference_IndexerInObjectCreationInitializer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int x, int? y = null]
    {
        get { return _number; }
        set { _number = value; }
    }

    P M1()
    {
        return /*<bind>*/new P() { [0] = 1 }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: P, IsInvalid) (Syntax: ObjectCreationExpression, 'new P() { [0] = 1 }') (Parent: ReturnStatement)
  Children(1):
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: P, IsInvalid) (Syntax: ObjectInitializerExpression, '{ [0] = 1 }')
      Initializers(1):
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32[missing], IsInvalid) (Syntax: SimpleAssignmentExpression, '[0] = 1')
          Left: 
            IPropertyReferenceExpression: System.Int32[missing] P.this[System.Int32[missing] x, [System.Int32[missing]? y = null]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32[missing], IsInvalid) (Syntax: ImplicitElementAccess, '[0]')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsInvalid, IsImplicit) (Syntax: ImplicitElementAccess, '[0]')
              Arguments(2):
                IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument, IsInvalid) (Syntax: Argument, '0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32[missing], Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                IArgument (ArgumentKind.DefaultValue, Matching Parameter: y) ([2] OperationKind.Argument, IsInvalid, IsImplicit) (Syntax: ImplicitElementAccess, '[0]')
                  IDefaultValueExpression ([0] OperationKind.DefaultValueExpression, Type: System.Int32[missing]?, IsInvalid) (Syntax: ImplicitElementAccess, '[0]')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32[missing], Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // (2,7): error CS0518: Predefined type 'System.Object' is not defined or imported
                // class P
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(2, 7),
                // (5,21): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = null]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 21),
                // (5,28): error CS0518: Predefined type 'System.Nullable`1' is not defined or imported
                //     public int this[int x, int? y = null]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int?").WithArguments("System.Nullable`1").WithLocation(5, 28),
                // (5,28): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = null]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 28),
                // (5,12): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     public int this[int x, int? y = null]
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(5, 12),
                // (8,9): error CS0518: Predefined type 'System.Void' is not defined or imported
                //         set { _number = value; }
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "set { _number = value; }").WithArguments("System.Void").WithLocation(8, 9),
                // (11,5): error CS0518: Predefined type 'System.Object' is not defined or imported
                //     P M1()
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(11, 5),
                // (4,13): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     private int _number = 0;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "int").WithArguments("System.Int32").WithLocation(4, 13),
                // (4,27): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //     private int _number = 0;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(4, 27),
                // (13,30): error CS0518: Predefined type 'System.Object' is not defined or imported
                //         return /*<bind>*/new P() { [0] = 1 };/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Object").WithLocation(13, 30),
                // (13,37): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //         return /*<bind>*/new P() { [0] = 1 };/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "0").WithArguments("System.Int32").WithLocation(13, 37),
                // (13,42): error CS0518: Predefined type 'System.Int32' is not defined or imported
                //         return /*<bind>*/new P() { [0] = 1 };/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "1").WithArguments("System.Int32").WithLocation(13, 42),
                // (13,30): error CS0518: Predefined type 'System.Void' is not defined or imported
                //         return /*<bind>*/new P() { [0] = 1 };/*</bind>*/;
                Diagnostic(ErrorCode.ERR_PredefinedTypeNotFound, "P").WithArguments("System.Void").WithLocation(13, 30),
                // (2,7): error CS1729: 'object' does not contain a constructor that takes 0 arguments
                // class P
                Diagnostic(ErrorCode.ERR_BadCtorArgCount, "P").WithArguments("object", "0").WithLocation(2, 7)
            };

            var compilation = CreateCompilation(source, options: Test.Utilities.TestOptions.ReleaseDll);
            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(compilation, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DefaultValueWithParameterErrorType_IndexerInObjectCreationInitializer()
        {
            string source = @"
class P
{
    private int _number = 0;
    public int this[int x, S s = 0]
    {
        get { return _number; }
        set { _number = value; }
    }

    P M1()
    {
        return /*<bind>*/new P() { [0] = 1 }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: P..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: P) (Syntax: ObjectCreationExpression, 'new P() { [0] = 1 }') (Parent: ReturnStatement)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: P) (Syntax: ObjectInitializerExpression, '{ [0] = 1 }')
      Initializers(1):
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, '[0] = 1')
          Left: 
            IPropertyReferenceExpression: System.Int32 P.this[System.Int32 x, [S s = null]] { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: ImplicitElementAccess, '[0]')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: P, IsImplicit) (Syntax: ImplicitElementAccess, '[0]')
              Arguments(2):
                IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, '0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                IArgument (ArgumentKind.DefaultValue, Matching Parameter: s) ([2] OperationKind.Argument, IsImplicit) (Syntax: ImplicitElementAccess, '[0]')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: S) (Syntax: ImplicitElementAccess, '[0]')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";

            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(5,28): error CS0246: The type or namespace name 'S' could not be found (are you missing a using directive or an assembly reference?)
                //     public int this[int x, S s = 0]
                Diagnostic(ErrorCode.ERR_SingleTypeNameNotFound, "S").WithArguments("S").WithLocation(5, 28),
                // file.cs(5,30): error CS1750: A value of type 'int' cannot be used as a default parameter because there are no standard conversions to type 'S'
                //     public int this[int x, S s = 0]
                Diagnostic(ErrorCode.ERR_NoConversionForDefaultParam, "s").WithArguments("int", "S").WithLocation(5, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void UndefinedMethod()
        {
            string source = @"
class P
{
    static void M1()
    {
        /*<bind>*/M2(1, 2)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: InvocationExpression, 'M2(1, 2)') (Parent: ExpressionStatement)
  Children(3):
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'M2')
      Children(0)
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(6,19): error CS0103: The name 'M2' does not exist in the current context
                //         /*<bind>*/M2(1, 2)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NameNotInContext, "M2").WithArguments("M2").WithLocation(6, 19)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics, useLatestFrameworkReferences: true);
        }

        private class IndexerAccessArgumentVerifier : OperationWalker
        {
            private readonly Compilation _compilation;

            private IndexerAccessArgumentVerifier(Compilation compilation)
            {
                _compilation = compilation;
            }

            public static void Verify(IOperation operation, Compilation compilation, SyntaxNode syntaxNode)
            {
                new IndexerAccessArgumentVerifier(compilation).Visit(operation);
            }

            public override void VisitPropertyReferenceExpression(IPropertyReferenceExpression operation)
            {
                if (operation.HasErrors(_compilation) || operation.Arguments.Length == 0)
                {
                    return;
                }

                // Check if the parameter symbol for argument is corresponding to indexer instead of accessor.
                var indexerSymbol = operation.Property;
                foreach (var argument in operation.Arguments)
                {
                    if (!argument.HasErrors(_compilation))
                    {
                        Assert.Same(indexerSymbol, argument.Parameter.ContainingSymbol);
                    }
                }
            }
        }
    }
}
