' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestAwaitExpression()
            Dim source = <![CDATA[
Imports System
Imports System.Threading.Tasks

Class C
    Async Sub M()
        Await M2()'BIND:"Await M2()"
    End Sub

    Function M2() As Task
        Return Nothing
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAwaitExpression ([0] OperationKind.AwaitExpression, Type: System.Void) (Syntax: AwaitExpression, 'Await M2()') (Parent: ExpressionStatement)
  Expression: 
    IInvocationExpression ( Function C.M2() As System.Threading.Tasks.Task) ([0] OperationKind.InvocationExpression, Type: System.Threading.Tasks.Task) (Syntax: InvocationExpression, 'M2()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'M2')
      Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AwaitExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics, useLatestFramework:=True)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestAwaitExpression_ParameterReference()
            Dim source = <![CDATA[
Imports System
Imports System.Threading.Tasks

Class C
    Async Sub M(t As Task)
        Await t'BIND:"Await t"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAwaitExpression ([0] OperationKind.AwaitExpression, Type: System.Void) (Syntax: AwaitExpression, 'Await t') (Parent: ExpressionStatement)
  Expression: 
    IParameterReferenceExpression: t ([0] OperationKind.ParameterReferenceExpression, Type: System.Threading.Tasks.Task) (Syntax: IdentifierName, 't')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AwaitExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics, useLatestFramework:=True)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestAwaitExpression_InLambda()
            Dim source = <![CDATA[
Imports System
Imports System.Threading.Tasks

Class C
    Async Sub M(t As Task(Of Integer))
        Dim f As Func(Of Task) = Async Function() Await t'BIND:"Await t"
        Await f()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAwaitExpression ([0] OperationKind.AwaitExpression, Type: System.Int32) (Syntax: AwaitExpression, 'Await t') (Parent: ReturnStatement)
  Expression: 
    IParameterReferenceExpression: t ([0] OperationKind.ParameterReferenceExpression, Type: System.Threading.Tasks.Task(Of System.Int32)) (Syntax: IdentifierName, 't')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AwaitExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics, useLatestFramework:=True)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestAwaitExpression_ErrorArgument()
            Dim source = <![CDATA[
Imports System
Imports System.Threading.Tasks

Class C
    Async Sub M()
        Await UndefinedTask'BIND:"Await UndefinedTask"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAwaitExpression ([0] OperationKind.AwaitExpression, Type: System.Void, IsInvalid) (Syntax: AwaitExpression, 'Await UndefinedTask') (Parent: ExpressionStatement)
  Expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'UndefinedTask')
      Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30451: 'UndefinedTask' is not declared. It may be inaccessible due to its protection level.
        Await UndefinedTask'BIND:"Await UndefinedTask"
              ~~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of AwaitExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics, useLatestFramework:=True)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestAwaitExpression_ValueArgument()
            Dim source = <![CDATA[
Imports System
Imports System.Threading.Tasks

Class C
    Async Sub M(i As Integer)
        Await i'BIND:"Await i"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAwaitExpression ([0] OperationKind.AwaitExpression, Type: System.Void, IsInvalid) (Syntax: AwaitExpression, 'Await i') (Parent: ExpressionStatement)
  Expression: 
    IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'i')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC36930: 'Await' requires that the type 'Integer' have a suitable GetAwaiter method.
        Await i'BIND:"Await i"
        ~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of AwaitExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics, useLatestFramework:=True)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestAwaitExpression_MissingArgument()
            Dim source = <![CDATA[
Imports System
Imports System.Threading.Tasks

Class C
    Async Sub M()
        Await'BIND:"Await"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAwaitExpression ([0] OperationKind.AwaitExpression, Type: System.Void, IsInvalid) (Syntax: AwaitExpression, 'Await') (Parent: ExpressionStatement)
  Expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30201: Expression expected.
        Await'BIND:"Await"
             ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of AwaitExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics, useLatestFramework:=True)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestAwaitExpression_NonAsyncMethod()
            Dim source = <![CDATA[
Imports System
Imports System.Threading.Tasks

Class C
    Sub M(t As Task)
        Await t'BIND:"Await t"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, 'Await t') (Parent: BlockStatement)
  Expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: InvocationExpression, 'Await t')
      Children(2):
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'Await')
          Children(0)
        IParameterReferenceExpression: t ([1] OperationKind.ParameterReferenceExpression, Type: System.Threading.Tasks.Task, IsInvalid) (Syntax: IdentifierName, 't')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC37058: 'Await' can only be used within an Async method. Consider marking this method with the 'Async' modifier and changing its return type to 'Task'.
        Await t'BIND:"Await t"
        ~~~~~
BC30800: Method arguments must be enclosed in parentheses.
        Await t'BIND:"Await t"
              ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ExpressionStatementSyntax)(source, expectedOperationTree, expectedDiagnostics, useLatestFramework:=True)
        End Sub
    End Class
End Namespace
