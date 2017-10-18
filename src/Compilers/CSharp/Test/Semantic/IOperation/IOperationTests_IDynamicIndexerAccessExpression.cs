// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_DynamicArgument()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        var x = /*<bind>*/c[d]/*</bind>*/;
    }

    public int this[int i] => 0;
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'c[d]') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(1):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_MultipleApplicableSymbols()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        var x = /*<bind>*/c[d]/*</bind>*/;
    }

    public int this[int i] => 0;

    public int this[long i] => 0;
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'c[d]') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(1):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_MultipleArgumentsAndApplicableSymbols()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        char ch = 'c';
        var x = /*<bind>*/c[d, ch]/*</bind>*/;
    }

    public int this[int i, char ch] => 0;

    public int this[long i, char ch] => 0;
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'c[d, ch]') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(2):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
    ILocalReferenceExpression: ch ([2] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'ch')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_ArgumentNames()
        {
            string source = @"
class C
{
    void M(C c, dynamic d, dynamic e)
    {
        var x = /*<bind>*/c[i: d, ch: e]/*</bind>*/;
    }

    public int this[int i, char ch] => 0;

    public int this[long i, char ch] => 0;
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'c[i: d, ch: e]') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(2):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
    IParameterReferenceExpression: e ([2] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'e')
  ArgumentNames(2):
    ""i""
    ""ch""
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_ArgumentRefKinds()
        {
            string source = @"
class C
{
    void M(C c, dynamic d, dynamic e)
    {
        var x = /*<bind>*/c[i: d, ch: ref e]/*</bind>*/;
    }

    public int this[int i, ref dynamic ch] => 0;
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'c[i: d, ch: ref e]') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(2):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
    IParameterReferenceExpression: e ([2] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'e')
  ArgumentNames(2):
    ""i""
    ""ch""
  ArgumentRefKinds(2):
    None
    Ref
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0631: ref and out are not valid in this context
                //     public int this[int i, ref dynamic ch] => 0;
                Diagnostic(ErrorCode.ERR_IllegalRefParam, "ref").WithLocation(9, 28)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_WithDynamicReceiver()
        {
            string source = @"
class C
{
    void M(dynamic d, int i)
    {
        var x = /*<bind>*/d[i]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'd[i]') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: d ([0] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  Arguments(1):
    IParameterReferenceExpression: i ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_WithDynamicMemberReceiver()
        {
            string source = @"
class C
{
    void M(dynamic c, int i)
    {
        var x = /*<bind>*/c.M2[i]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'c.M2[i]') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M2"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: dynamic) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Type Arguments(0)
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'c')
  Arguments(1):
    IParameterReferenceExpression: i ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_WithDynamicTypedMemberReceiver()
        {
            string source = @"
class C
{
    dynamic M2 = null;

    void M(C c, int i)
    {
        var x = /*<bind>*/c.M2[i]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'c.M2[i]') (Parent: VariableInitializer)
  Expression: 
    IFieldReferenceExpression: dynamic C.M2 ([0] OperationKind.FieldReferenceExpression, Type: dynamic) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(1):
    IParameterReferenceExpression: i ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_AllFields()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        int i = 0;
        var x = /*<bind>*/c[ref i, c: d]/*</bind>*/;
    }

    public int this[ref int i, char c] => 0;
    public int this[ref int i, long c] => 0;
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic) (Syntax: ElementAccessExpression, 'c[ref i, c: d]') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(2):
    ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
    IParameterReferenceExpression: d ([2] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  ArgumentNames(2):
    ""null""
    ""c""
  ArgumentRefKinds(2):
    Ref
    None
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0631: ref and out are not valid in this context
                //     public int this[ref int i, char c] => 0;
                Diagnostic(ErrorCode.ERR_IllegalRefParam, "ref").WithLocation(10, 21),
                // CS0631: ref and out are not valid in this context
                //     public int this[ref int i, long c] => 0;
                Diagnostic(ErrorCode.ERR_IllegalRefParam, "ref").WithLocation(11, 21)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_ErrorBadDynamicMethodArgLambda()
        {
            string source = @"
using System;

class C
{
    public void M(C c)
    {
        dynamic y = null;
        var x = /*<bind>*/c[delegate { }, y]/*</bind>*/;
    }

    public int this[Action a, Action y] => 0;
}
";
            string expectedOperationTree = @"
IDynamicIndexerAccessExpression ([0] OperationKind.DynamicIndexerAccessExpression, Type: dynamic, IsInvalid) (Syntax: ElementAccessExpression, 'c[delegate { }, y]') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(2):
    IAnonymousFunctionExpression (Symbol: lambda expression) ([1] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: AnonymousMethodExpression, 'delegate { }')
      IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ }')
        IReturnStatement ([0] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: Block, '{ }')
          ReturnedValue: 
            null
    ILocalReferenceExpression: y ([2] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'y')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type.
                //         var x = /*<bind>*/c[delegate { }, y]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadDynamicMethodArgLambda, "delegate { }").WithLocation(9, 29)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicIndexerAccessExpression_OverloadResolutionFailure()
        {
            string source = @"
using System;

class C
{
    void M(C c, dynamic d)
    {
        var x = /*<bind>*/c[d]/*</bind>*/;
    }

    public int this[int i, int j] => 0;
    public int this[int i, int j, int k] => 0;
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32, IsInvalid) (Syntax: ElementAccessExpression, 'c[d]') (Parent: VariableInitializer)
  Children(2):
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C, IsInvalid) (Syntax: IdentifierName, 'c')
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic, IsInvalid) (Syntax: IdentifierName, 'd')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1501: No overload for method 'this' takes 1 arguments
                //         var x = /*<bind>*/c[d]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadArgCount, "c[d]").WithArguments("this", "1").WithLocation(8, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ElementAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
