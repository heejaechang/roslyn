' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub IConditionalAccessExpression_SimpleMethodAccess()
            Dim source = <![CDATA[
Option Strict On

Public Class C1
    Public Sub M1()
        Dim o As New Object
        o?.ToString()'BIND:"o?.ToString()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IConditionalAccessExpression ([0] OperationKind.ConditionalAccessExpression, Type: System.Void) (Syntax: ConditionalAccessExpression, 'o?.ToString()') (Parent: ExpressionStatement)
  Expression: 
    ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  WhenNotNull: 
    IInvocationExpression (virtual Function System.Object.ToString() As System.String) ([1] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, '.ToString()')
      Instance Receiver: 
        IPlaceholderExpression ([0] OperationKind.None, IsImplicit) (Syntax: ConditionalAccessExpression, 'o?.ToString()')
      Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ConditionalAccessExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub IConditionalAccessExpression_SimplePropertyAccess()
            Dim source = <![CDATA[
Option Strict On

Public Class C1

    Public ReadOnly Property Prop1 As Integer

    Public Sub M1()
        Dim c1 As C1 = Nothing
        Dim propValue = c1?.Prop1'BIND:"c1?.Prop1"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IConditionalAccessExpression ([0] OperationKind.ConditionalAccessExpression, Type: System.Nullable(Of System.Int32)) (Syntax: ConditionalAccessExpression, 'c1?.Prop1') (Parent: VariableInitializer)
  Expression: 
    ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C1) (Syntax: IdentifierName, 'c1')
  WhenNotNull: 
    IPropertyReferenceExpression: ReadOnly Property C1.Prop1 As System.Int32 ([1] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, '.Prop1')
      Instance Receiver: 
        IPlaceholderExpression ([0] OperationKind.None, IsImplicit) (Syntax: ConditionalAccessExpression, 'c1?.Prop1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ConditionalAccessExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
