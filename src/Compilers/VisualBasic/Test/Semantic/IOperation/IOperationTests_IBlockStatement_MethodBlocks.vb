﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_SubMethodBlock()
            Dim source = <![CDATA[
Class Program
    Sub Method()'BIND:"Sub Method()"
        If 1 > 2 Then
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements) ([Root] OperationKind.BlockStatement) (Syntax: SubBlock, 'Sub Method( ... End Sub') (Parent: )
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_SubNewBlock()
            Dim source = <![CDATA[
Class Program
    Sub New()'BIND:"Sub New()"
        If 1 > 2 Then
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements) ([Root] OperationKind.BlockStatement) (Syntax: ConstructorBlock, 'Sub New()'B ... End Sub') (Parent: )
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ConstructorBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_FunctionMethodBlock()
            Dim source = <![CDATA[
Class Program
    Function Method() As Boolean'BIND:"Function Method() As Boolean"
        If 1 > 2 Then
        End If

        Return True
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (4 statements, 1 locals) ([Root] OperationKind.BlockStatement) (Syntax: FunctionBlock, 'Function Me ... nd Function') (Parent: )
  Locals: Local_1: Method As System.Boolean
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  IReturnStatement ([1] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return True')
    ReturnedValue: 
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
  ILabeledStatement (Label: exit) ([2] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndFunctionStatement, 'End Function')
    Statement: 
      null
  IReturnStatement ([3] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndFunctionStatement, 'End Function')
    ReturnedValue: 
      ILocalReferenceExpression: Method ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean, IsImplicit) (Syntax: EndFunctionStatement, 'End Function')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_PropertyGetBlock()
            Dim source = <![CDATA[
Class Program
    ReadOnly Property Prop As Integer
        Get'BIND:"Get"
            If 1 > 2 Then
            End If
        End Get
    End Property
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements, 1 locals) ([Root] OperationKind.BlockStatement) (Syntax: GetAccessorBlock, 'Get'BIND:"G ... End Get') (Parent: )
  Locals: Local_1: Prop As System.Int32
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndGetStatement, 'End Get')
    Statement: 
      null
  IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndGetStatement, 'End Get')
    ReturnedValue: 
      ILocalReferenceExpression: Prop ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsImplicit) (Syntax: EndGetStatement, 'End Get')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42355: Property 'Prop' doesn't return a value on all code paths. Are you missing a 'Return' statement?
        End Get
        ~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of AccessorBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_PropertySetBlock()
            Dim source = <![CDATA[
Class Program
    WriteOnly Property Prop As Integer
        Set(Value As Integer)'BIND:"Set(Value As Integer)"
            If 1 > 2 Then
            End If
        End Set
    End Property
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements) ([Root] OperationKind.BlockStatement) (Syntax: SetAccessorBlock, 'Set(Value A ... End Set') (Parent: )
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSetStatement, 'End Set')
    Statement: 
      null
  IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSetStatement, 'End Set')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AccessorBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_CustomEventAddBlock()
            Dim source = <![CDATA[
Imports System

Class C
    Public Custom Event A As Action
        AddHandler(value As Action)'BIND:"AddHandler(value As Action)"
            If 1 > 2 Then
            End If
        End AddHandler

        RemoveHandler(value As Action)
        End RemoveHandler

        RaiseEvent()
        End RaiseEvent
    End Event
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements) ([Root] OperationKind.BlockStatement) (Syntax: AddHandlerAccessorBlock, 'AddHandler( ...  AddHandler') (Parent: )
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndAddHandlerStatement, 'End AddHandler')
    Statement: 
      null
  IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndAddHandlerStatement, 'End AddHandler')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AccessorBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_CustomEventRemoveBlock()
            Dim source = <![CDATA[
Imports System

Class C
    Public Custom Event A As Action
        AddHandler(value As Action)
        End AddHandler

        RemoveHandler(value As Action)'BIND:"RemoveHandler(value As Action)"
            If 1 > 2 Then
            End If
        End RemoveHandler

        RaiseEvent()
        End RaiseEvent
    End Event
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements) ([Root] OperationKind.BlockStatement) (Syntax: RemoveHandlerAccessorBlock, 'RemoveHandl ... moveHandler') (Parent: )
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndRemoveHandlerStatement, 'End RemoveHandler')
    Statement: 
      null
  IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndRemoveHandlerStatement, 'End RemoveHandler')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AccessorBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_CustomEventRaiseBlock()
            Dim source = <![CDATA[
Imports System

Class C
    Public Custom Event A As Action
        AddHandler(value As Action)
        End AddHandler

        RemoveHandler(value As Action)
        End RemoveHandler

        RaiseEvent()'BIND:"RaiseEvent()"
            If 1 > 2 Then
            End If
        End RaiseEvent
    End Event
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (3 statements) ([Root] OperationKind.BlockStatement) (Syntax: RaiseEventAccessorBlock, 'RaiseEvent( ...  RaiseEvent') (Parent: )
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndRaiseEventStatement, 'End RaiseEvent')
    Statement: 
      null
  IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndRaiseEventStatement, 'End RaiseEvent')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AccessorBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_OperatorBlock()
            Dim source = <![CDATA[
Class Program
    Public Shared Operator +(p As Program, i As Integer) As Integer'BIND:"Public Shared Operator +(p As Program, i As Integer) As Integer"
        If 1 > 2 Then
        End If

        Return 0
    End Operator
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (4 statements, 1 locals) ([Root] OperationKind.BlockStatement) (Syntax: OperatorBlock, 'Public Shar ... nd Operator') (Parent: )
  Locals: Local_1: <anonymous local> As System.Int32
  IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    Condition: 
      IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: False) (Syntax: GreaterThanExpression, '1 > 2')
        Left: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        Right: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    IfTrue: 
      IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If 1 > 2 Th ... End If')
    IfFalse: 
      null
  IReturnStatement ([1] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return 0')
    ReturnedValue: 
      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  ILabeledStatement (Label: exit) ([2] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndOperatorStatement, 'End Operator')
    Statement: 
      null
  IReturnStatement ([3] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndOperatorStatement, 'End Operator')
    ReturnedValue: 
      ILocalReferenceExpression:  ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsImplicit) (Syntax: EndOperatorStatement, 'End Operator')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of OperatorBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_MustOverrideSubMethodStatement()
            Dim source = "
MustInherit Class Program
    Public MustOverride Sub Method'BIND:""Public MustOverride Sub Method""
End Class"

            VerifyNoOperationTreeForTest(Of MethodStatementSyntax)(source)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_InterfaceSub()
            Dim source = "
Interface IProgram
    Sub Method'BIND:""Sub Method""
End Interface"

            VerifyNoOperationTreeForTest(Of MethodStatementSyntax)(source)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_InterfaceFunction()
            Dim source = "
Interface IProgram
    Function Method() As Boolean'BIND:""Function Method() As Boolean""
End Interface"

            VerifyNoOperationTreeForTest(Of MethodStatementSyntax)(source)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub IBlockStatement_NormalEvent()
            Dim source = "
Class Program
        Public Event A As System.Action'BIND:""Public Event A As System.Action""
End Class"

            VerifyNoOperationTreeForTest(Of EventStatementSyntax)(source)
        End Sub
    End Class
End Namespace
