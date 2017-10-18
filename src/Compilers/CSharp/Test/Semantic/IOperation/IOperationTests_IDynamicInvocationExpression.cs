﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_DynamicArgument()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        /*<bind>*/c.M2(d)/*</bind>*/;
    }

    public void M2(int i)
    {
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'c.M2(d)') (Parent: ExpressionStatement)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M2"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Type Arguments(0)
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(1):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_MultipleApplicableSymbols()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        var x = /*<bind>*/c.M2(d)/*</bind>*/;
    }

    public void M2(int i)
    {
    }

    public void M2(long i)
    {
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'c.M2(d)') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M2"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Type Arguments(0)
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(1):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_MultipleArgumentsAndApplicableSymbols()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        char ch = 'c';
        var x = /*<bind>*/c.M2(d, ch)/*</bind>*/;
    }

    public void M2(int i, char ch)
    {
    }

    public void M2(long i, char ch)
    {
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'c.M2(d, ch)') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M2"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Type Arguments(0)
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(2):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
    ILocalReferenceExpression: ch ([2] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'ch')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_ArgumentNames()
        {
            string source = @"
class C
{
    void M(C c, dynamic d, dynamic e)
    {
        var x = /*<bind>*/c.M2(i: d, ch: e)/*</bind>*/;
    }

    public void M2(int i, char ch)
    {
    }

    public void M2(long i, char ch)
    {
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'c.M2(i: d, ch: e)') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M2"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Type Arguments(0)
      Instance Receiver: 
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

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_ArgumentRefKinds()
        {
            string source = @"
class C
{
    void M(C c, object d, dynamic e)
    {
        int k;
        var x = /*<bind>*/c.M2(ref d, out k, e)/*</bind>*/;
    }

    public void M2(ref object i, out int j, char c)
    {
        j = 0;
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'c.M2(ref d, out k, e)') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M2"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Type Arguments(0)
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Arguments(3):
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'd')
    ILocalReferenceExpression: k ([2] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'k')
    IParameterReferenceExpression: e ([3] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'e')
  ArgumentNames(0)
  ArgumentRefKinds(3):
    Ref
    Out
    None
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_DelegateInvocation()
        {
            string source = @"
using System;

class C
{
    public Action<object> F;
    void M(dynamic i)
    {
        var x = /*<bind>*/F(i)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'F(i)') (Parent: VariableInitializer)
  Expression: 
    IFieldReferenceExpression: System.Action<System.Object> C.F ([0] OperationKind.FieldReferenceExpression, Type: System.Action<System.Object>) (Syntax: IdentifierName, 'F')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'F')
  Arguments(1):
    IParameterReferenceExpression: i ([1] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'i')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0649: Field 'C.F' is never assigned to, and will always have its default value null
                //     public Action<object> F;
                Diagnostic(ErrorCode.WRN_UnassignedInternalField, "F").WithArguments("C.F", "null").WithLocation(6, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_WithDynamicReceiver()
        {
            string source = @"
class C
{
    void M(dynamic d, int i)
    {
        var x = /*<bind>*/d(i)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'd(i)') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: d ([0] OperationKind.ParameterReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  Arguments(1):
    IParameterReferenceExpression: i ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  ArgumentNames(0)
  ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_WithDynamicMemberReceiver()
        {
            string source = @"
class C
{
    void M(dynamic c, int i)
    {
        var x = /*<bind>*/c.M2(i)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'c.M2(i)') (Parent: VariableInitializer)
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

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_WithDynamicTypedMemberReceiver()
        {
            string source = @"
class C
{
    dynamic M2 = null;
    void M(C c, int i)
    {
        var x = /*<bind>*/c.M2(i)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'c.M2(i)') (Parent: VariableInitializer)
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

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_AllFields()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        int i = 0;
        var x = /*<bind>*/c.M2(ref i, c: d)/*</bind>*/;
    }

    public void M2(ref int i, char c)
    {
    }

    public void M2(ref int i, long c)
    {
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'c.M2(ref i, c: d)') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M2"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Type Arguments(0)
      Instance Receiver: 
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
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_ErrorBadDynamicMethodArgLambda()
        {
            string source = @"
using System;

class C
{
    public void M(C c)
    {
        dynamic y = null;
        var x = /*<bind>*/c.M2(delegate { }, y)/*</bind>*/;
    }

    public void M2(Action a, Action y)
    {
    }
}
";
            string expectedOperationTree = @"
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic, IsInvalid) (Syntax: InvocationExpression, 'c.M2(delegate { }, y)') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: ""M2"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'c.M2')
      Type Arguments(0)
      Instance Receiver: 
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
                //         var x = /*<bind>*/c.M2(delegate { }, y)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_BadDynamicMethodArgLambda, "delegate { }").WithLocation(9, 32)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void DynamicInvocation_OverloadResolutionFailure()
        {
            string source = @"
class C
{
    void M(C c, dynamic d)
    {
        var x = /*<bind>*/c.M2(d)/*</bind>*/;
    }

    public void M2()
    {
    }

    public void M2(int i, int j)
    {
    }
}
";
            string expectedOperationTree = @"
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'c.M2(d)') (Parent: VariableInitializer)
  Children(2):
    IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C, IsInvalid) (Syntax: IdentifierName, 'c')
    IParameterReferenceExpression: d ([1] OperationKind.ParameterReferenceExpression, Type: dynamic, IsInvalid) (Syntax: IdentifierName, 'd')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS7036: There is no argument given that corresponds to the required formal parameter 'j' of 'C.M2(int, int)'
                //         var x = /*<bind>*/c.M2(d)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoCorrespondingArgument, "M2").WithArguments("j", "C.M2(int, int)").WithLocation(6, 29),
                // CS0815: Cannot assign void to an implicitly-typed variable
                //         var x = /*<bind>*/c.M2(d)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ImplicitlyTypedVariableAssignedBadValue, "x = /*<bind>*/c.M2(d)").WithArguments("void").WithLocation(6, 13)
            };

            VerifyOperationTreeAndDiagnosticsForTest<InvocationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
