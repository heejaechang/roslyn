' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementSingleLineIf()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim count As Integer = 0
        Dim returnValue As Integer = -1
        If count > 0 Then returnValue = count'BIND:"If count > 0 Then returnValue = count"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: SingleLineIfStatement, 'If count >  ... lue = count') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'count > 0')
      Left: 
        ILocalReferenceExpression: count ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineIfStatement, 'If count >  ... lue = count')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'returnValue = count')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'returnValue = count')
            Left: 
              ILocalReferenceExpression: returnValue ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'returnValue')
            Right: 
              ILocalReferenceExpression: count ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineIfStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementMultiLineIf()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim count As Integer = 0
        Dim returnValue As Integer = 1
        If count > 0 Then 'BIND:"If count > 0 Then"'BIND:"If count > 0 Then 'BIND:"If count > 0 Then""
            returnValue = count
        End If
    End Sub
End Module
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If count >  ... End If') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'count > 0')
      Left: 
        ILocalReferenceExpression: count ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If count >  ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'returnValue = count')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'returnValue = count')
            Left: 
              ILocalReferenceExpression: returnValue ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'returnValue')
            Right: 
              ILocalReferenceExpression: count ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementSingleLineIfAndElse()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim count As Integer
        Dim data As Integer
        If count > 10 Then data = data + count Else data = data - count'BIND:"If count > 10 Then data = data + count Else data = data - count"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: SingleLineIfStatement, 'If count >  ... ata - count') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'count > 10')
      Left: 
        ILocalReferenceExpression: count ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineIfStatement, 'If count >  ... ata - count')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'data = data + count')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'data = data + count')
            Left: 
              ILocalReferenceExpression: data ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'data')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'data + count')
                Left: 
                  ILocalReferenceExpression: data ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'data')
                Right: 
                  ILocalReferenceExpression: count ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
  IfFalse: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: SingleLineElseClause, 'Else data = data - count')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'data = data - count')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'data = data - count')
            Left: 
              ILocalReferenceExpression: data ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'data')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'data - count')
                Left: 
                  ILocalReferenceExpression: data ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'data')
                Right: 
                  ILocalReferenceExpression: count ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineIfStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementSingleLineIfAndElseNested()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim m As Integer = 12
        Dim n As Integer = 18
        Dim returnValue As Integer = -1
        If m > 10 Then If n > 20 Then returnValue = n'BIND:"If m > 10 Then If n > 20 Then returnValue = n"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([3] OperationKind.IfStatement) (Syntax: SingleLineIfStatement, 'If m > 10 T ... rnValue = n') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'm > 10')
      Left: 
        ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineIfStatement, 'If m > 10 T ... rnValue = n')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: SingleLineIfStatement, 'If n > 20 T ... rnValue = n')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'n > 20')
            Left: 
              ILocalReferenceExpression: n ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineIfStatement, 'If n > 20 T ... rnValue = n')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'returnValue = n')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'returnValue = n')
                  Left: 
                    ILocalReferenceExpression: returnValue ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'returnValue')
                  Right: 
                    ILocalReferenceExpression: n ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
        IfFalse: 
          null
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineIfStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementSimpleIfWithConditionEvaluationTrue()
            Dim source = <![CDATA[
Class P
    Private Sub M()
        Dim condition As Boolean = False
        If 1 = 1 Then'BIND:"If 1 = 1 Then"
            condition = True
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([1] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 = 1 Th ... End If') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: True) (Syntax: EqualsExpression, '1 = 1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 = 1 Th ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'condition = True')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'condition = True')
            Left: 
              ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementSimpleIfWithConditionConstantFalse()
            Dim source = <![CDATA[
Class P
    Private Sub M()
        Dim condition As Boolean = True
        If False Then'BIND:"If False Then"
            condition = False
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([1] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If False Th ... End If') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'False')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If False Th ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'condition = False')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'condition = False')
            Left: 
              ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'False')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementSingleLineWithOperator()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim m As Integer = 12
        Dim n As Integer = 18
        Dim returnValue As Integer = -1
        If (m > 10 And n > 20) Then returnValue = n'BIND:"If (m > 10 And n > 20) Then returnValue = n"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([3] OperationKind.IfStatement) (Syntax: SingleLineIfStatement, 'If (m > 10  ... rnValue = n') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(m > 10 And n > 20)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.And, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: AndExpression, 'm > 10 And n > 20')
          Left: 
            IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'm > 10')
              Left: 
                ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'n > 20')
              Left: 
                ILocalReferenceExpression: n ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineIfStatement, 'If (m > 10  ... rnValue = n')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'returnValue = n')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'returnValue = n')
            Left: 
              ILocalReferenceExpression: returnValue ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'returnValue')
            Right: 
              ILocalReferenceExpression: n ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineIfStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementMultiLineIfWithElse()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim count As Integer = 0
        Dim returnValue As Integer = -1
        If count > 0 Then'BIND:"If count > 0 Then"
            returnValue = count
        Else
            returnValue = -1
        End If
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If count >  ... End If') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'count > 0')
      Left: 
        ILocalReferenceExpression: count ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If count >  ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'returnValue = count')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'returnValue = count')
            Left: 
              ILocalReferenceExpression: returnValue ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'returnValue')
            Right: 
              ILocalReferenceExpression: count ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
  IfFalse: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else ... nValue = -1')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'returnValue = -1')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'returnValue = -1')
            Left: 
              ILocalReferenceExpression: returnValue ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'returnValue')
            Right: 
              IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([1] OperationKind.UnaryOperatorExpression, Type: System.Int32, Constant: -1) (Syntax: UnaryMinusExpression, '-1')
                Operand: 
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementSimpleIfNested1()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 12
        Dim n As Integer = 18
        Dim returnValue As Integer = -1
        If (m > 10) Then'BIND:"If (m > 10) Then"
            If (n > 20) Then
                Console.WriteLine("Result 1")
            End If
        Else
            Console.WriteLine("Result 2")
        End If
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([3] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (m > 10) ... End If') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(m > 10)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'm > 10')
          Left: 
            ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (m > 10) ... End If')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (n > 20) ... End If')
        Condition: 
          IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(n > 20)')
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'n > 20')
                Left: 
                  ILocalReferenceExpression: n ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (n > 20) ... End If')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... "Result 1")')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... "Result 1")')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result 1"')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result 1") (Syntax: StringLiteralExpression, '"Result 1"')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IfFalse: 
          null
  IfFalse: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else ... "Result 2")')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... "Result 2")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... "Result 2")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result 2"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result 2") (Syntax: StringLiteralExpression, '"Result 2"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementIfNested2()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 12
        Dim n As Integer = 18
        Dim returnValue As Integer = -1
        If (m > 10) Then'BIND:"If (m > 10) Then"
            If (n > 20) Then
                Console.WriteLine("Result 1")
            Else
                Console.WriteLine("Result 2")
            End If
        End If
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([3] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (m > 10) ... End If') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(m > 10)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'm > 10')
          Left: 
            ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (m > 10) ... End If')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (n > 20) ... End If')
        Condition: 
          IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(n > 20)')
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'n > 20')
                Left: 
                  ILocalReferenceExpression: n ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (n > 20) ... End If')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... "Result 1")')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... "Result 1")')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result 1"')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result 1") (Syntax: StringLiteralExpression, '"Result 1"')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IfFalse: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else ... "Result 2")')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... "Result 2")')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... "Result 2")')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result 2"')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result 2") (Syntax: StringLiteralExpression, '"Result 2"')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementWithMultipleCondition()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 9
        Dim n As Integer = 7
        Dim p As Integer = 5
        If (m >= n AndAlso m >= p) Then'BIND:"If (m >= n AndAlso m >= p) Then"
            Console.WriteLine("Nothing Is larger than m.")
        End If
    End Sub
End Module
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([3] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (m >= n  ... End If') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(m >= n AndAlso m >= p)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.ConditionalAnd, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: AndAlsoExpression, 'm >= n AndAlso m >= p')
          Left: 
            IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, 'm >= n')
              Left: 
                ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
              Right: 
                ILocalReferenceExpression: n ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
          Right: 
            IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, 'm >= p')
              Left: 
                ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
              Right: 
                ILocalReferenceExpression: p ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'p')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (m >= n  ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... r than m.")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... r than m.")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Nothing Is ... er than m."')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Nothing Is larger than m.") (Syntax: StringLiteralExpression, '"Nothing Is ... er than m."')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementWithElseIfCondition()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 9
        Dim n As Integer = 7
        If (m > 20) Then'BIND:"If (m > 20) Then"
            Console.WriteLine("Result1")
        ElseIf (n > 10) Then
            Console.WriteLine("Result2")
        Else
            Console.WriteLine("Result3")
        End If
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (m > 20) ... End If') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(m > 20)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'm > 20')
          Left: 
            ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (m > 20) ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ("Result1")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ("Result1")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result1"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result1") (Syntax: StringLiteralExpression, '"Result1"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IfFalse: 
    IIfStatement ([2] OperationKind.IfStatement) (Syntax: ElseIfBlock, 'ElseIf (n > ... ("Result2")')
      Condition: 
        IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(n > 10)')
          Operand: 
            IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'n > 10')
              Left: 
                ILocalReferenceExpression: n ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
      IfTrue: 
        IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: ElseIfBlock, 'ElseIf (n > ... ("Result2")')
          IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ("Result2")')
            Expression: 
              IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ("Result2")')
                Instance Receiver: 
                  null
                Arguments(1):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result2"')
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result2") (Syntax: StringLiteralExpression, '"Result2"')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IfFalse: 
        IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else ... ("Result3")')
          IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ("Result3")')
            Expression: 
              IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ("Result3")')
                Instance Receiver: 
                  null
                Arguments(1):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result3"')
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result3") (Syntax: StringLiteralExpression, '"Result3"')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementWithElseIfSingleLine()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim m As Integer = 9
        Dim n As Integer = 7
        If (m > 20) Then System.Console.WriteLine("Result1") Else If (n > 10) Then System.Console.WriteLine("Result2") Else System.Console.WriteLine("Result3") End If'BIND:"If (m > 20) Then System.Console.WriteLine("Result1") Else If (n > 10) Then System.Console.WriteLine("Result2") Else System.Console.WriteLine("Result3")"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: SingleLineIfStatement, 'If (m > 20) ... ("Result3")') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(m > 20)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'm > 20')
          Left: 
            ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineIfStatement, 'If (m > 20) ... ("Result3")')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... ("Result1")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... ("Result1")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result1"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result1") (Syntax: StringLiteralExpression, '"Result1"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IfFalse: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: SingleLineElseClause, 'Else If (n  ... ("Result3")')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: SingleLineIfStatement, 'If (n > 10) ... ("Result3")')
        Condition: 
          IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(n > 10)')
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'n > 10')
                Left: 
                  ILocalReferenceExpression: n ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineIfStatement, 'If (n > 10) ... ("Result3")')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... ("Result2")')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... ("Result2")')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result2"')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result2") (Syntax: StringLiteralExpression, '"Result2"')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IfFalse: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: SingleLineElseClause, 'Else System ... ("Result3")')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... ("Result3")')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... ("Result3")')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result3"')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result3") (Syntax: StringLiteralExpression, '"Result3"')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30205: End of statement expected.
        If (m > 20) Then System.Console.WriteLine("Result1") Else If (n > 10) Then System.Console.WriteLine("Result2") Else System.Console.WriteLine("Result3") End If'BIND:"If (m > 20) Then System.Console.WriteLine("Result1") Else If (n > 10) Then System.Console.WriteLine("Result2") Else System.Console.WriteLine("Result3")"
                                                                                                                                                                ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineIfStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementWithElseMissing()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 9
        If (m > 20) Then'BIND:"If (m > 20) Then"
            Console.WriteLine("Result1")
        Else
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([1] OperationKind.IfStatement, IsInvalid) (Syntax: MultiLineIfBlock, 'If (m > 20) ... Else') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean, IsInvalid) (Syntax: ParenthesizedExpression, '(m > 20)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, IsInvalid) (Syntax: GreaterThanExpression, 'm > 20')
          Left: 
            ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'm')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20, IsInvalid) (Syntax: NumericLiteralExpression, '20')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: MultiLineIfBlock, 'If (m > 20) ... Else')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ("Result1")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ("Result1")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result1"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result1") (Syntax: StringLiteralExpression, '"Result1"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IfFalse: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30081: 'If' must end with a matching 'End If'.
        If (m > 20) Then'BIND:"If (m > 20) Then"
        ~~~~~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementWithConditionMissing()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 9
        If () Then'BIND:"If () Then"
            Console.WriteLine("Result1")
        End If
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([1] OperationKind.IfStatement, IsInvalid) (Syntax: MultiLineIfBlock, 'If () Then' ... End If') (Parent: BlockStatement)
  Condition: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: ParenthesizedExpression, '()')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: ?, IsInvalid) (Syntax: ParenthesizedExpression, '()')
          Operand: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
              Children(0)
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: MultiLineIfBlock, 'If () Then' ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ("Result1")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ("Result1")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result1"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result1") (Syntax: StringLiteralExpression, '"Result1"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30201: Expression expected.
        If () Then'BIND:"If () Then"
            ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementWithStatementMissing()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 9
        If (m = 9) Then'BIND:"If (m = 9) Then"
        Else
        End If

    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([1] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (m = 9)  ... End If') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(m = 9)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'm = 9')
          Left: 
            ILocalReferenceExpression: m ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'm')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 9) (Syntax: NumericLiteralExpression, '9')
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (m = 9)  ... End If')
  IfFalse: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17601, "https://github.com/dotnet/roslyn/issues/17601")>
        Public Sub IIfstatementWithFuncCall()
            Dim source = <![CDATA[
Module Module1
    Sub Main()
        If (True) Then'BIND:"If (True) Then"
            A()
        Else
            B()
        End If
    End Sub
    Function A() As String
        Return "A"
    End Function
    Function B() As String
        Return "B"
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (True) T ... End If') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean, Constant: True) (Syntax: ParenthesizedExpression, '(True)')
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (True) T ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'A()')
        Expression: 
          IInvocationExpression (Function Module1.A() As System.String) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'A()')
            Instance Receiver: 
              null
            Arguments(0)
  IfFalse: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else ... B()')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'B()')
        Expression: 
          IInvocationExpression (Function Module1.B() As System.String) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'B()')
            Instance Receiver: 
              null
            Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub IIfstatement_GetElseIf()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 9
        Dim n As Integer = 7
        If (m > 20) Then
            Console.WriteLine("Result1")
        ElseIf (n > 10) Then'BIND:"ElseIf (n > 10) Then"
            Console.WriteLine("Result2")
        Else
            Console.WriteLine("Result3")
        End If
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: ElseIfBlock, 'ElseIf (n > ... ("Result2")') (Parent: IfStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(n > 10)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'n > 10')
          Left: 
            ILocalReferenceExpression: n ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'n')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: ElseIfBlock, 'ElseIf (n > ... ("Result2")')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ("Result2")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ("Result2")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result2"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result2") (Syntax: StringLiteralExpression, '"Result2"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IfFalse: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else ... ("Result3")')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ("Result3")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ("Result3")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result3"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result3") (Syntax: StringLiteralExpression, '"Result3"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ElseIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub IIfstatement_GetElse()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim m As Integer = 9
        Dim n As Integer = 7
        If (m > 20) Then
            Console.WriteLine("Result1")
        ElseIf (n > 10) Then
            Console.WriteLine("Result2")
        Else'BIND:"Else"
            Console.WriteLine("Result3")
        End If
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else'BIND:" ... ("Result3")') (Parent: IfStatement)
  IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ("Result3")')
    Expression: 
      IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ("Result3")')
        Instance Receiver: 
          null
        Arguments(1):
          IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Result3"')
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Result3") (Syntax: StringLiteralExpression, '"Result3"')
            InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ElseBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub IIfstatementSingleLineIf_GetElse()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim count As Integer
        Dim data As Integer
        If count > 10 Then data = data + count Else data = data - count'BIND:"Else data = data - count"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: SingleLineElseClause, 'Else data = data - count') (Parent: IfStatement)
  IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'data = data - count')
    Expression: 
      ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'data = data - count')
        Left: 
          ILocalReferenceExpression: data ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'data')
        Right: 
          IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'data - count')
            Left: 
              ILocalReferenceExpression: data ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'data')
            Right: 
              ILocalReferenceExpression: count ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'count')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineElseClauseSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

    End Class
End Namespace
