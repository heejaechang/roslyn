﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Semantics
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_SimpleForLoopsTest()
            Dim source = <![CDATA[
Imports System
Class C
    Shared Sub Main()
        Dim arr As String() = New String(1) {}
        arr(0) = "one"
        arr(1) = "two"
        For Each s As String In arr'BIND:"For Each s As String In arr"
            Console.WriteLine(s)
        Next
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([3] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each s  ... Next') (Parent: BlockStatement)
  Locals: Local_1: s As System.String
  LoopControlVariable: 
    ILocalReferenceExpression: s ([1] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: VariableDeclarator, 's As String')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'arr')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: arr ([0] OperationKind.LocalReferenceExpression, Type: System.String()) (Syntax: IdentifierName, 'arr')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each s  ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(s)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(s)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 's')
                ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_WithList()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main(args As String())
        Dim list As New System.Collections.Generic.List(Of String)()
        list.Add("a")
        list.Add("b")
        list.Add("c")
        For Each item As String In list'BIND:"For Each item As String In list"
            System.Console.WriteLine(item)
        Next

    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([4] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each it ... Next') (Parent: BlockStatement)
  Locals: Local_1: item As System.String
  LoopControlVariable: 
    ILocalReferenceExpression: item ([1] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: VariableDeclarator, 'item As String')
  Collection: 
    ILocalReferenceExpression: list ([0] OperationKind.LocalReferenceExpression, Type: System.Collections.Generic.List(Of System.String)) (Syntax: IdentifierName, 'list')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each it ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... eLine(item)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... eLine(item)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'item')
                ILocalReferenceExpression: item ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'item')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_WithBreak()
            Dim source = <![CDATA[
Class C
    Public Shared Sub Main()
        Dim S As String() = New String() {"ABC", "XYZ"}
        For Each x As String In S
            For Each y As Char In x'BIND:"For Each y As Char In x"
                If y = "B"c Then
                    Exit For
                Else
                    System.Console.WriteLine(y)
                End If
            Next
        Next
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each y  ... Next') (Parent: BlockStatement)
  Locals: Local_1: y As System.Char
  LoopControlVariable: 
    ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: VariableDeclarator, 'y As Char')
  Collection: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each y  ... Next')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If y = "B"c ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'y = "B"c')
            Left: 
              ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'y')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Char, Constant: B) (Syntax: CharacterLiteralExpression, '"B"c')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If y = "B"c ... End If')
            IBranchStatement (BranchKind.Break, Label: exit) ([0] OperationKind.BranchStatement) (Syntax: ExitForStatement, 'Exit For')
        IfFalse: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else ... riteLine(y)')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(y)')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.Char)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(y)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'y')
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'y')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_WithContinue()
            Dim source = <![CDATA[
Class C
    Public Shared Sub Main()
        Dim S As String() = New String() {"ABC", "XYZ"}
        For Each x As String In S'BIND:"For Each x As String In S"
            For Each y As Char In x
                If y = "B"c Then
                    Continue For
                End If
                System.Console.WriteLine(y)
        Next y, x
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each x  ... Next y, x') (Parent: BlockStatement)
  Locals: Local_1: x As System.String
  LoopControlVariable: 
    ILocalReferenceExpression: x ([1] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: VariableDeclarator, 'x As String')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'S')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: S ([0] OperationKind.LocalReferenceExpression, Type: System.String()) (Syntax: IdentifierName, 'S')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each x  ... Next y, x')
      IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each y  ... Next y, x')
        Locals: Local_1: y As System.Char
        LoopControlVariable: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: VariableDeclarator, 'y As Char')
        Collection: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
        Body: 
          IBlockStatement (2 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each y  ... Next y, x')
            IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If y = "B"c ... End If')
              Condition: 
                IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'y = "B"c')
                  Left: 
                    ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'y')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Char, Constant: B) (Syntax: CharacterLiteralExpression, '"B"c')
              IfTrue: 
                IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If y = "B"c ... End If')
                  IBranchStatement (BranchKind.Continue, Label: continue) ([0] OperationKind.BranchStatement) (Syntax: ContinueForStatement, 'Continue For')
              IfFalse: 
                null
            IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(y)')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.Char)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(y)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'y')
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'y')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        NextVariables(2):
          ILocalReferenceExpression: y ([3] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'y')
          ILocalReferenceExpression: x ([4] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_Nested()
            Dim source = <![CDATA[
Class C
    Shared Sub Main()
        Dim c(3)() As Integer
        For Each x As Integer() In c
            ReDim x(3)
            For i As Integer = 0 To 3
                x(i) = i
            Next
            For Each y As Integer In x'BIND:"For Each y As Integer In x"
                System.Console.WriteLine(y)
            Next
        Next
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each y  ... Next') (Parent: BlockStatement)
  Locals: Local_1: y As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'y As Integer')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'x')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'x')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each y  ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(y)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(y)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'y')
                ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_Nested1()
            Dim source = <![CDATA[
Class C
    Public Shared Sub Main()
        Dim S As String() = New String() {"ABC", "XYZ"}
        For Each x As String In S'BIND:"For Each x As String In S"
            For Each y As Char In x
                System.Console.WriteLine(y)
            Next
        Next
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each x  ... Next') (Parent: BlockStatement)
  Locals: Local_1: x As System.String
  LoopControlVariable: 
    ILocalReferenceExpression: x ([1] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: VariableDeclarator, 'x As String')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'S')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: S ([0] OperationKind.LocalReferenceExpression, Type: System.String()) (Syntax: IdentifierName, 'S')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each x  ... Next')
      IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each y  ... Next')
        Locals: Local_1: y As System.Char
        LoopControlVariable: 
          ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: VariableDeclarator, 'y As Char')
        Collection: 
          ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
        Body: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each y  ... Next')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(y)')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.Char)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(y)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'y')
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'y')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        NextVariables(0)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_Interface()
            Dim source = <![CDATA[
Option Infer On

Class C
    Public Shared Sub Main()
        For Each x In New Enumerable()'BIND:"For Each x In New Enumerable()"
            System.Console.WriteLine(x)
        Next
    End Sub
End Class

Class Enumerable
    Implements System.Collections.IEnumerable
    Private Function System_Collections_IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Dim list As New System.Collections.Generic.List(Of Integer)()
        list.Add(3)
        list.Add(2)
        list.Add(1)
        Return list.GetEnumerator()
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each x  ... Next') (Parent: BlockStatement)
  Locals: Local_1: x As System.Object
  LoopControlVariable: 
    ILocalReferenceExpression: x ([1] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: ObjectCreationExpression, 'New Enumerable()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IObjectCreationExpression (Constructor: Sub Enumerable..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Enumerable) (Syntax: ObjectCreationExpression, 'New Enumerable()')
          Arguments(0)
          Initializer: 
            null
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each x  ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(x)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Object)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(x)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'x')
                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_String()
            Dim source = <![CDATA[
Option Infer On
Class Program
    Public Shared Sub Main()
        Const s As String = Nothing
        For Each y In s'BIND:"For Each y In s"
            System.Console.WriteLine(y)
        Next
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each y  ... Next') (Parent: BlockStatement)
  Locals: Local_1: y As System.Char
  LoopControlVariable: 
    ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'y')
  Collection: 
    ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.String, Constant: null) (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each y  ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(y)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Char)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(y)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'y')
                ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'y')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_IterateStruct()
            Dim source = <![CDATA[
Option Infer On
Imports System.Collections
Class C
    Public Shared Sub Main()
        For Each x In New Enumerable()'BIND:"For Each x In New Enumerable()"
            System.Console.WriteLine(x)
        Next
    End Sub
End Class
Structure Enumerable
    Implements IEnumerable
    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return New Integer() {1, 2, 3}.GetEnumerator()
    End Function
End Structure
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each x  ... Next') (Parent: BlockStatement)
  Locals: Local_1: x As System.Object
  LoopControlVariable: 
    ILocalReferenceExpression: x ([1] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: ObjectCreationExpression, 'New Enumerable()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IObjectCreationExpression (Constructor: Sub Enumerable..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Enumerable) (Syntax: ObjectCreationExpression, 'New Enumerable()')
          Arguments(0)
          Initializer: 
            null
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each x  ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(x)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Object)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(x)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'x')
                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_QueryExpression()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq

Module Program
    Sub Main(args As String())
        ' Obtain a list of customers.
        Dim customers As List(Of Customer) = GetCustomers()

        ' Return customers that are grouped based on country.
        Dim countries = From cust In customers
                        Order By cust.Country, cust.City
                        Group By CountryName = cust.Country
                        Into CustomersInCountry = Group, Count()
                        Order By CountryName

        ' Output the results.
        For Each country In countries'BIND:"For Each country In countries"
            Debug.WriteLine(country.CountryName & " count=" & country.Count)

            For Each customer In country.CustomersInCountry
                Debug.WriteLine("   " & customer.CompanyName & "  " & customer.City)
            Next
        Next
    End Sub

    Private Function GetCustomers() As List(Of Customer)
        Return New List(Of Customer) From
            {
                New Customer With {.CustomerID = 1, .CompanyName = "C", .City = "H", .Country = "C"},
                New Customer With {.CustomerID = 2, .CompanyName = "M", .City = "R", .Country = "U"},
                New Customer With {.CustomerID = 3, .CompanyName = "F", .City = "V", .Country = "C"}
            }
    End Function
End Module

Class Customer
    Public Property CustomerID As Integer
    Public Property CompanyName As String
    Public Property City As String
    Public Property Country As String
End Class

]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([2] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each co ... Next') (Parent: BlockStatement)
  Locals: Local_1: country As <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>
  LoopControlVariable: 
    ILocalReferenceExpression: country ([1] OperationKind.LocalReferenceExpression, Type: <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>) (Syntax: IdentifierName, 'country')
  Collection: 
    ILocalReferenceExpression: countries ([0] OperationKind.LocalReferenceExpression, Type: System.Linq.IOrderedEnumerable(Of <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>)) (Syntax: IdentifierName, 'countries')
  Body: 
    IBlockStatement (2 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each co ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Debug.Write ... ntry.Count)')
        Expression: 
          IInvocationExpression (Sub System.Diagnostics.Debug.WriteLine(message As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Debug.Write ... ntry.Count)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: message) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'country.Cou ... untry.Count')
                IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: ConcatenateExpression, 'country.Cou ... untry.Count')
                  Left: 
                    IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: ConcatenateExpression, 'country.Cou ... & " count="')
                      Left: 
                        IPropertyReferenceExpression: ReadOnly Property <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>.CountryName As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'country.CountryName')
                          Instance Receiver: 
                            ILocalReferenceExpression: country ([0] OperationKind.LocalReferenceExpression, Type: <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>) (Syntax: IdentifierName, 'country')
                      Right: 
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: " count=") (Syntax: StringLiteralExpression, '" count="')
                  Right: 
                    IConversionExpression (Explicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'country.Count')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        IPropertyReferenceExpression: ReadOnly Property <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>.Count As System.Int32 ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'country.Count')
                          Instance Receiver: 
                            ILocalReferenceExpression: country ([0] OperationKind.LocalReferenceExpression, Type: <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>) (Syntax: IdentifierName, 'country')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each cu ... Next')
        Locals: Local_1: customer As Customer
        LoopControlVariable: 
          ILocalReferenceExpression: customer ([1] OperationKind.LocalReferenceExpression, Type: Customer) (Syntax: IdentifierName, 'customer')
        Collection: 
          IPropertyReferenceExpression: ReadOnly Property <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>.CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer) ([0] OperationKind.PropertyReferenceExpression, Type: System.Collections.Generic.IEnumerable(Of Customer)) (Syntax: SimpleMemberAccessExpression, 'country.Cus ... rsInCountry')
            Instance Receiver: 
              ILocalReferenceExpression: country ([0] OperationKind.LocalReferenceExpression, Type: <anonymous type: Key CountryName As System.String, Key CustomersInCountry As System.Collections.Generic.IEnumerable(Of Customer), Key Count As System.Int32>) (Syntax: IdentifierName, 'country')
        Body: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each cu ... Next')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Debug.Write ... tomer.City)')
              Expression: 
                IInvocationExpression (Sub System.Diagnostics.Debug.WriteLine(message As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Debug.Write ... tomer.City)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: message) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"   " & cus ... stomer.City')
                      IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: ConcatenateExpression, '"   " & cus ... stomer.City')
                        Left: 
                          IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: ConcatenateExpression, '"   " & cus ... Name & "  "')
                            Left: 
                              IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: ConcatenateExpression, '"   " & cus ... CompanyName')
                                Left: 
                                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "   ") (Syntax: StringLiteralExpression, '"   "')
                                Right: 
                                  IPropertyReferenceExpression: Property Customer.CompanyName As System.String ([1] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'customer.CompanyName')
                                    Instance Receiver: 
                                      ILocalReferenceExpression: customer ([0] OperationKind.LocalReferenceExpression, Type: Customer) (Syntax: IdentifierName, 'customer')
                            Right: 
                              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: "  ") (Syntax: StringLiteralExpression, '"  "')
                        Right: 
                          IPropertyReferenceExpression: Property Customer.City As System.String ([1] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'customer.City')
                            Instance Receiver: 
                              ILocalReferenceExpression: customer ([0] OperationKind.LocalReferenceExpression, Type: Customer) (Syntax: IdentifierName, 'customer')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        NextVariables(0)
  NextVariables(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ForEachBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub



        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_Multidimensional()
            Dim source = <![CDATA[
Option Strict On
Imports System

Module Program
    Sub Main()

        Dim k(,) = {{1}, {1}}
        For Each [Custom] In k'BIND:"For Each [Custom] In k"
            Console.Write(VerifyStaticType([Custom], GetType(Integer)))
            Console.Write(VerifyStaticType([Custom], GetType(Object)))
            Exit For
        Next
    End Sub

    Function VerifyStaticType(Of T)(ByVal x As T, ByVal y As System.Type) As Boolean
        Return GetType(T) Is y
    End Function
End Module

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each [C ... Next') (Parent: BlockStatement)
  Locals: Local_1: Custom As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: Custom ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, '[Custom]')
  Collection: 
    ILocalReferenceExpression: k ([0] OperationKind.LocalReferenceExpression, Type: System.Int32(,)) (Syntax: IdentifierName, 'k')
  Body: 
    IBlockStatement (3 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each [C ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... (Integer)))')
        Expression: 
          IInvocationExpression (Sub System.Console.Write(value As System.Boolean)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... (Integer)))')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'VerifyStati ... e(Integer))')
                IInvocationExpression (Function Program.VerifyStaticType(Of System.Int32)(x As System.Int32, y As System.Type) As System.Boolean) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'VerifyStati ... e(Integer))')
                  Instance Receiver: 
                    null
                  Arguments(2):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '[Custom]')
                      ILocalReferenceExpression: Custom ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, '[Custom]')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'GetType(Integer)')
                      ITypeOfExpression ([0] OperationKind.TypeOfExpression, Type: System.Type) (Syntax: GetTypeExpression, 'GetType(Integer)')
                        TypeOperand: System.Int32
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... e(Object)))')
        Expression: 
          IInvocationExpression (Sub System.Console.Write(value As System.Boolean)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... e(Object)))')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'VerifyStati ... pe(Object))')
                IInvocationExpression (Function Program.VerifyStaticType(Of System.Int32)(x As System.Int32, y As System.Type) As System.Boolean) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'VerifyStati ... pe(Object))')
                  Instance Receiver: 
                    null
                  Arguments(2):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '[Custom]')
                      ILocalReferenceExpression: Custom ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, '[Custom]')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'GetType(Object)')
                      ITypeOfExpression ([0] OperationKind.TypeOfExpression, Type: System.Type) (Syntax: GetTypeExpression, 'GetType(Object)')
                        TypeOperand: System.Object
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IBranchStatement (BranchKind.Break, Label: exit) ([2] OperationKind.BranchStatement) (Syntax: ExitForStatement, 'Exit For')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_LateBinding()
            Dim source = <![CDATA[
Option Strict Off
Imports System
Class C
    Shared Sub Main()
        Dim o As Object = {1, 2, 3}
        For Each x In o'BIND:"For Each x In o"
            Console.WriteLine(x)
        Next
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each x  ... Next') (Parent: BlockStatement)
  Locals: Local_1: x As System.Object
  LoopControlVariable: 
    ILocalReferenceExpression: x ([1] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'o')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each x  ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(x)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Object)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(x)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'x')
                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_Pattern()
            Dim source = <![CDATA[
Option Infer On
Class C
    Public Shared Sub Main()
        For Each x In New Enumerable()'BIND:"For Each x In New Enumerable()"
            System.Console.WriteLine(x)
        Next
    End Sub
End Class

Class Enumerable
    Public Function GetEnumerator() As Enumerator
        Return New Enumerator()
    End Function
End Class

Class Enumerator
    Private x As Integer = 0
    Public ReadOnly Property Current() As Integer
        Get
            Return x
        End Get
    End Property
    Public Function MoveNext() As Boolean
        Return System.Threading.Interlocked.Increment(x) & lt; 4
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each x  ... Next') (Parent: BlockStatement)
  Locals: Local_1: x As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: x ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  Collection: 
    IObjectCreationExpression (Constructor: Sub Enumerable..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Enumerable) (Syntax: ObjectCreationExpression, 'New Enumerable()')
      Arguments(0)
      Initializer: 
        null
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each x  ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(x)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(x)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'x')
                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_lamda()
            Dim source = <![CDATA[
Option Strict On
Option Infer On

Imports System

Class C1
    Private element_lambda_field As Integer

    Public Shared Sub Main()
        Dim c1 As New C1()
        c1.DoStuff()
    End Sub

    Public Sub DoStuff()
        Dim arr As Integer() = New Integer(1) {}
        arr(0) = 23
        arr(1) = 42

        Dim myDelegate As Action = Sub()
                                       Dim element_lambda_local As Integer
                                       For Each element_lambda_local In arr'BIND:"For Each element_lambda_local In arr"
                                           Console.WriteLine(element_lambda_local)
                                       Next element_lambda_local


                                   End Sub

        myDelegate.Invoke()
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([1] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each el ... ambda_local') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: element_lambda_local ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'element_lambda_local')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'arr')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: arr ([0] OperationKind.LocalReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'arr')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each el ... ambda_local')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... mbda_local)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... mbda_local)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'element_lambda_local')
                ILocalReferenceExpression: element_lambda_local ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'element_lambda_local')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(1):
    ILocalReferenceExpression: element_lambda_local ([3] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'element_lambda_local')
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_InvalidConversion()
            Dim source = <![CDATA[
Imports System

Class C1
    Public Shared Sub Main()
        For Each element As Integer In "Hello World."'BIND:"For Each element As Integer In "Hello World.""
            Console.WriteLine(element)
        Next
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForEachBlock, 'For Each el ... Next') (Parent: BlockStatement)
  Locals: Local_1: element As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: element ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'element As Integer')
  Collection: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Hello World.", IsInvalid) (Syntax: StringLiteralExpression, '"Hello World."')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: ForEachBlock, 'For Each el ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... ne(element)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ne(element)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'element')
                ILocalReferenceExpression: element ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'element')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_Throw()
            Dim source = <![CDATA[
Imports System
Class C
    Shared Sub Main()
        Dim arr As String() = New String(1) {}
        arr(0) = "one"
        arr(1) = "two"
        For Each s As String In arr'BIND:"For Each s As String In arr"
            If (s = "one") Then
                Throw New Exception
            End If
            Console.WriteLine(s)
        Next
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([3] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each s  ... Next') (Parent: BlockStatement)
  Locals: Local_1: s As System.String
  LoopControlVariable: 
    ILocalReferenceExpression: s ([1] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: VariableDeclarator, 's As String')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'arr')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: arr ([0] OperationKind.LocalReferenceExpression, Type: System.String()) (Syntax: IdentifierName, 'arr')
  Body: 
    IBlockStatement (2 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each s  ... Next')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (s = "on ... End If')
        Condition: 
          IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(s = "one")')
            Operand: 
              IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 's = "one"')
                Left: 
                  ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: "one") (Syntax: StringLiteralExpression, '"one"')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (s = "on ... End If')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'Throw New Exception')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'Throw New Exception')
                  IObjectCreationExpression (Constructor: Sub System.Exception..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Exception) (Syntax: ObjectCreationExpression, 'New Exception')
                    Arguments(0)
                    Initializer: 
                      null
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(s)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(s)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 's')
                ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_WithReturn()
            Dim source = <![CDATA[
Class C
    Private F As Object
    Shared Function M(c As Object()) As Boolean
        For Each o In c'BIND:"For Each o In c"
            If o IsNot Nothing Then Return True
        Next
        Return False
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each o  ... Next') (Parent: BlockStatement)
  Locals: Local_1: o As System.Object
  LoopControlVariable: 
    ILocalReferenceExpression: o ([1] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'c')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: System.Object()) (Syntax: IdentifierName, 'c')
  Body: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each o  ... Next')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: SingleLineIfStatement, 'If o IsNot  ... Return True')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsNotExpression, 'o IsNot Nothing')
            Left: 
              ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
            Right: 
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineIfStatement, 'If o IsNot  ... Return True')
            IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return True')
              ReturnedValue: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
        IfFalse: 
          null
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForEachBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_FieldAsIterationVariable()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Class C
    Private X As Integer = 0
    Sub M(args As Integer())
        For Each X In args'BIND:"For Each X In args"
        Next X
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each X  ... Next X') (Parent: BlockStatement)
  LoopControlVariable: 
    IFieldReferenceExpression: C.X As System.Int32 ([1] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'args')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: args ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'args')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each X  ... Next X')
  NextVariables(1):
    IFieldReferenceExpression: C.X As System.Int32 ([3] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ForEachBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForEachLoopStatement_FieldWithExplicitReceiverAsIterationVariable()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Class C
    Private X As Integer = 0
    Sub M(c As C, args As Integer())
        For Each c.X In args'BIND:"For Each c.X In args"
        Next c.X
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachBlock, 'For Each c. ... Next c.X') (Parent: BlockStatement)
  LoopControlVariable: 
    IFieldReferenceExpression: C.X As System.Int32 ([1] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'c.X')
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: IdentifierName, 'args')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: args ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'args')
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: ForEachBlock, 'For Each c. ... Next c.X')
  NextVariables(1):
    IFieldReferenceExpression: C.X As System.Int32 ([3] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'c.X')
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ForEachBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

    End Class
End Namespace
