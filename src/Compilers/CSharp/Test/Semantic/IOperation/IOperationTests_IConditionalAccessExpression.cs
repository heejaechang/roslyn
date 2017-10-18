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
        public void IConditionalAccessExpression_SimpleMethodAccess()

        {
            string source = @"
using System;

public class C1
{
    public void M()
    {
        var o = new object();
        /*<bind>*/o?.ToString()/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IConditionalAccessExpression ([0] OperationKind.ConditionalAccessExpression, Type: System.String) (Syntax: ConditionalAccessExpression, 'o?.ToString()') (Parent: ExpressionStatement)
  Expression: 
    ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  WhenNotNull: 
    IInvocationExpression (virtual System.String System.Object.ToString()) ([1] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, '.ToString()')
      Instance Receiver: 
        IConditionalAccessInstanceExpression ([0] OperationKind.ConditionalAccessInstanceExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'o')
      Arguments(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ConditionalAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IConditionalAccessExpression_SimplePropertyAccess()
        {
            string source = @"
using System;

public class C1
{
    int Prop1 { get; }
    public void M()
    {
        C1 c1 = null;
        var prop = /*<bind>*/c1?.Prop1/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IConditionalAccessExpression ([0] OperationKind.ConditionalAccessExpression, Type: System.Int32?) (Syntax: ConditionalAccessExpression, 'c1?.Prop1') (Parent: VariableInitializer)
  Expression: 
    ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C1) (Syntax: IdentifierName, 'c1')
  WhenNotNull: 
    IPropertyReferenceExpression: System.Int32 C1.Prop1 { get; } ([1] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: MemberBindingExpression, '.Prop1')
      Instance Receiver: 
        IConditionalAccessInstanceExpression ([0] OperationKind.ConditionalAccessInstanceExpression, Type: C1, IsImplicit) (Syntax: IdentifierName, 'c1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ConditionalAccessExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }


    }
}
