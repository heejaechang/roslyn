﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub SimpleRetuenFromRegularMethod()
            Dim source = <![CDATA[
Class C
    Sub M()
        Return'BIND:"Return"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return') (Parent: BlockStatement)
  ReturnedValue: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ReturnStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ReturnWithValueFromRegularMethod()
            Dim source = <![CDATA[
Class C
    Function M() As Boolean
        Return True'BIND:"Return True"
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return True') (Parent: BlockStatement)
  ReturnedValue: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ReturnStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub YieldFromIterator()
            Dim source = <![CDATA[
Class C
    Iterator Function M() As System.Collections.Generic.IEnumerable(Of Integer)
        Yield 0'BIND:"Yield 0"
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IReturnStatement ([0] OperationKind.YieldReturnStatement) (Syntax: YieldStatement, 'Yield 0') (Parent: BlockStatement)
  ReturnedValue: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of YieldStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub ReturnFromIterator()
            Dim source = <![CDATA[
Class C
    Iterator Function M() As System.Collections.Generic.IEnumerable(Of Integer)
        Yield 0
        Return'BIND:"Return"
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IReturnStatement ([1] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return') (Parent: BlockStatement)
  ReturnedValue: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ReturnStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(7299, "https://github.com/dotnet/roslyn/issues/7299")>
        Public Sub Return_ConstantConversions_01()
            Dim source = <![CDATA[
Option Strict On
Class C
    Function M() As Byte
        Return 0.0'BIND:"Return 0.0"
    End Function
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IReturnStatement ([0] OperationKind.ReturnStatement, IsInvalid) (Syntax: ReturnStatement, 'Return 0.0') (Parent: BlockStatement)
  ReturnedValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Byte, Constant: 0, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '0.0')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0.0')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30512: Option Strict On disallows implicit conversions from 'Double' to 'Byte'.
        Return 0.0'BIND:"Return 0.0"
               ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ReturnStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub


    End Class
End Namespace
