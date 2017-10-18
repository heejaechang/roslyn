' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_NoConversions()
            Dim source = <![CDATA[
Imports System

Class C
    Shared Sub Main()
        Dim t As (Integer, Integer) = (1, 2)'BIND:"(1, 2)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Int32)) (Syntax: TupleExpression, '(1, 2)') (Parent: VariableInitializer)
  Elements(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_NoConversions_ParentVariableDeclaration()
            Dim source = <![CDATA[
Imports System

Class C
    Shared Sub Main()
        Dim t As (Integer, Integer) = (1, 2)'BIND:"Dim t As (Integer, Integer) = (1, 2)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t As (I ... r) = (1, 2)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (System.Int32, System.Int32)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= (1, 2)')
        ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Int32)) (Syntax: TupleExpression, '(1, 2)')
          Elements(2):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_ImplicitConversions()
            Dim source = <![CDATA[
Imports System

Class C
    Shared Sub Main()
        Dim t As (UInteger, UInteger) = (1, 2)'BIND:"(1, 2)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32, System.UInt32)) (Syntax: TupleExpression, '(1, 2)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 2, IsImplicit) (Syntax: NumericLiteralExpression, '2')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_ImplicitConversions_ParentVariableDeclaration()
            Dim source = <![CDATA[
Imports System

Class C
    Shared Sub Main()
        Dim t As (UInteger, UInteger) = (1, 2)'BIND:"Dim t As (UInteger, UInteger) = (1, 2)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t As (U ... r) = (1, 2)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (System.UInt32, System.UInt32)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= (1, 2)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.UInt32, System.UInt32), IsImplicit) (Syntax: TupleExpression, '(1, 2)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32, System.UInt32)) (Syntax: TupleExpression, '(1, 2)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 2, IsImplicit) (Syntax: NumericLiteralExpression, '2')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_ImplicitConversionFromNull()
            Dim source = <![CDATA[
Imports System

Class C
    Shared Sub Main()
        Dim t As (UInteger, String) = (1, Nothing)'BIND:"(1, Nothing)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32, System.String)) (Syntax: TupleExpression, '(1, Nothing)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_ImplicitConversionFromNull_ParentVariableDeclaration()
            Dim source = <![CDATA[
Imports System

Class C
    Shared Sub Main()
        Dim t As (UInteger, String) = (1, Nothing)'BIND:"Dim t As (UInteger, String) = (1, Nothing)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t As (U ... 1, Nothing)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (System.UInt32, System.String)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= (1, Nothing)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.UInt32, System.String), IsImplicit) (Syntax: TupleExpression, '(1, Nothing)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32, System.String)) (Syntax: TupleExpression, '(1, Nothing)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_NamedArguments()
            Dim source = <![CDATA[
Option Strict Off
Imports System

Class C
    Shared Sub Main()
        Dim t = (A:=1, B:=2)'BIND:"(A:=1, B:=2)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (A As System.Int32, B As System.Int32)) (Syntax: TupleExpression, '(A:=1, B:=2)') (Parent: VariableInitializer)
  Elements(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_NamedArguments_ParentVariableDeclaration()
            Dim source = <![CDATA[
Option Strict Off
Imports System

Class C
    Shared Sub Main()
        Dim t = (A:=1, B:=2)'BIND:"Dim t = (A:=1, B:=2)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t = (A:=1, B:=2)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (A As System.Int32, B As System.Int32)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= (A:=1, B:=2)')
        ITupleExpression ([0] OperationKind.TupleExpression, Type: (A As System.Int32, B As System.Int32)) (Syntax: TupleExpression, '(A:=1, B:=2)')
          Elements(2):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_NamedElementsInTupleType()
            Dim source = <![CDATA[
Option Strict Off
Imports System

Class C
    Shared Sub Main()
        Dim t As (A As Integer, B As Integer) = (1, 2)'BIND:"(1, 2)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Int32)) (Syntax: TupleExpression, '(1, 2)') (Parent: ConversionExpression)
  Elements(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_NamedElementsInTupleType_ParentVariableDeclaration()
            Dim source = <![CDATA[
Option Strict Off
Imports System

Class C
    Shared Sub Main()
        Dim t As (A As Integer, B As Integer) = (1, 2)'BIND:"Dim t As (A As Integer, B As Integer) = (1, 2)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t As (A ... r) = (1, 2)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (A As System.Int32, B As System.Int32)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= (1, 2)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (A As System.Int32, B As System.Int32), IsImplicit) (Syntax: TupleExpression, '(1, 2)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Int32)) (Syntax: TupleExpression, '(1, 2)')
              Elements(2):
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_NamedElementsAndImplicitConversions()
            Dim source = <![CDATA[
Option Strict Off
Imports System

Class C
    Shared Sub Main()
        Dim t As (A As Int16, B As String) = (A:=1, B:=Nothing)'BIND:"(A:=1, B:=Nothing)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (A As System.Int16, B As System.String)) (Syntax: TupleExpression, '(A:=1, B:=Nothing)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int16, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_NamedElementsAndImplicitConversions_ParentVariableDeclaration()
            Dim source = <![CDATA[
Option Strict Off
Imports System

Class C
    Shared Sub Main()
        Dim t As (A As Int16, B As String) = (A:=1, B:=Nothing)'BIND:"Dim t As (A As Int16, B As String) = (A:=1, B:=Nothing)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t As (A ... B:=Nothing)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (A As System.Int16, B As System.String)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= (A:=1, B:=Nothing)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (A As System.Int16, B As System.String), IsImplicit) (Syntax: TupleExpression, '(A:=1, B:=Nothing)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (A As System.Int16, B As System.String)) (Syntax: TupleExpression, '(A:=1, B:=Nothing)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int16, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_UserDefinedConversionsForArguments()
            Dim source = <![CDATA[
Imports System

Class C
    Private ReadOnly _x As Integer
    Public Sub New(x As Integer)
        _x = x
    End Sub

    Public Shared Widening Operator CType(value As Integer) As C
        Return New C(value)
    End Operator

    Public Shared Widening Operator CType(c As C) As Short
        Return CShort(c._x)
    End Operator

    Public Shared Widening Operator CType(c As C) As String
        Return c._x.ToString()
    End Operator

    Public Sub M(c1 As C)
        Dim t As (A As Int16, B As String) = (New C(0), c1)'BIND:"(New C(0), c1)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16, c1 As System.String)) (Syntax: TupleExpression, '(New C(0), c1)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: Function C.op_Implicit(c As C) As System.Int16) ([0] OperationKind.ConversionExpression, Type: System.Int16, IsImplicit) (Syntax: ObjectCreationExpression, 'New C(0)')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: Function C.op_Implicit(c As C) As System.Int16)
      Operand: 
        IObjectCreationExpression (Constructor: Sub C..ctor(x As System.Int32)) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'New C(0)')
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Initializer: 
            null
    IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: Function C.op_Implicit(c As C) As System.String) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'c1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: Function C.op_Implicit(c As C) As System.String)
      Operand: 
        IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_UserDefinedConversionsForArguments_ParentVariableDeclaration()
            Dim source = <![CDATA[
Imports System

Class C
    Private ReadOnly _x As Integer
    Public Sub New(x As Integer)
        _x = x
    End Sub

    Public Shared Widening Operator CType(value As Integer) As C
        Return New C(value)
    End Operator

    Public Shared Widening Operator CType(c As C) As Short
        Return CShort(c._x)
    End Operator

    Public Shared Widening Operator CType(c As C) As String
        Return c._x.ToString()
    End Operator

    Public Sub M(c1 As C)
        Dim t As (A As Int16, B As String) = (New C(0), c1)'BIND:"Dim t As (A As Int16, B As String) = (New C(0), c1)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t As (A ... w C(0), c1)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (A As System.Int16, B As System.String)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= (New C(0), c1)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (A As System.Int16, B As System.String), IsImplicit) (Syntax: TupleExpression, '(New C(0), c1)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16, c1 As System.String)) (Syntax: TupleExpression, '(New C(0), c1)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: Function C.op_Implicit(c As C) As System.Int16) ([0] OperationKind.ConversionExpression, Type: System.Int16, IsImplicit) (Syntax: ObjectCreationExpression, 'New C(0)')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: Function C.op_Implicit(c As C) As System.Int16)
                  Operand: 
                    IObjectCreationExpression (Constructor: Sub C..ctor(x As System.Int32)) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'New C(0)')
                      Arguments(1):
                        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '0')
                          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Initializer: 
                        null
                IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: Function C.op_Implicit(c As C) As System.String) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'c1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: Function C.op_Implicit(c As C) As System.String)
                  Operand: 
                    IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_UserDefinedConversionFromTupleExpression()
            Dim source = <![CDATA[
Imports System

Class C
    Private ReadOnly _x As Integer
    Public Sub New(x As Integer)
        _x = x
    End Sub

    Public Shared Widening Operator CType(x As (Integer, String)) As C
        Return New C(x.Item1)
    End Operator

    Public Shared Widening Operator CType(c As C) As (Integer, String)
        Return (c._x, c._x.ToString)
    End Operator

    Public Sub M(c1 As C)
        Dim t As C = (0, Nothing)'BIND:"(0, Nothing)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Object)) (Syntax: TupleExpression, '(0, Nothing)') (Parent: ConversionExpression)
  Elements(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_UserDefinedConversionFromTupleExpression_ParentVariableDeclaration()
            Dim source = <![CDATA[
Imports System

Class C
    Private ReadOnly _x As Integer
    Public Sub New(x As Integer)
        _x = x
    End Sub

    Public Shared Widening Operator CType(x As (Integer, String)) As C
        Return New C(x.Item1)
    End Operator

    Public Shared Widening Operator CType(c As C) As (Integer, String)
        Return (c._x, c._x.ToString)
    End Operator

    Public Sub M(c1 As C)
        Dim t As C = (0, Nothing)'BIND:"Dim t As C = (0, Nothing)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t As C  ... 0, Nothing)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As C
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= (0, Nothing)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: Function C.op_Implicit(x As (System.Int32, System.String)) As C) ([0] OperationKind.ConversionExpression, Type: C, IsImplicit) (Syntax: TupleExpression, '(0, Nothing)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: Function C.op_Implicit(x As (System.Int32, System.String)) As C)
          Operand: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.Int32, System.Object), IsImplicit) (Syntax: TupleExpression, '(0, Nothing)')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Object)) (Syntax: TupleExpression, '(0, Nothing)')
                  Elements(2):
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_UserDefinedConversionToTupleType()
            Dim source = <![CDATA[
Imports System

Class C
    Private ReadOnly _x As Integer
    Public Sub New(x As Integer)
        _x = x
    End Sub

    Public Shared Widening Operator CType(x As (Integer, String)) As C
        Return New C(x.Item1)
    End Operator

    Public Shared Widening Operator CType(c As C) As (Integer, String)
        Return (c._x, c._x.ToString)
    End Operator

    Public Sub M(c1 As C)
        Dim t As (Integer, String) = c1'BIND:"c1"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1') (Parent: ConversionExpression)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of IdentifierNameSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_UserDefinedConversionToTupleType_ParentVariableDeclaration()
            Dim source = <![CDATA[
Imports System

Class C
    Private ReadOnly _x As Integer
    Public Sub New(x As Integer)
        _x = x
    End Sub

    Public Shared Widening Operator CType(x As (Integer, String)) As C
        Return New C(x.Item1)
    End Operator

    Public Shared Widening Operator CType(c As C) As (Integer, String)
        Return (c._x, c._x.ToString)
    End Operator

    Public Sub M(c1 As C)
        Dim t As (Integer, String) = c1'BIND:"Dim t As (Integer, String) = c1"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim t As (I ... tring) = c1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (System.Int32, System.String)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= c1')
        IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: Function C.op_Implicit(c As C) As (System.Int32, System.String)) ([0] OperationKind.ConversionExpression, Type: (System.Int32, System.String), IsImplicit) (Syntax: IdentifierName, 'c1')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: Function C.op_Implicit(c As C) As (System.Int32, System.String))
          Operand: 
            IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_InvalidConversion()
            Dim source = <![CDATA[
Class C
    Private ReadOnly _x As Integer
    Public Sub New(x As Integer)
        _x = x
    End Sub

    Public Shared Widening Operator CType(value As Integer) As C
        Return New C(value)
    End Operator

    Public Shared Widening Operator CType(c As C) As Integer
        Return CShort(c._x)
    End Operator

    Public Shared Widening Operator CType(c As C) As String
        Return c._x.ToString()
    End Operator

    Public Sub M(c1 As C)
        Dim t As (Short, String) = (New C(0), c1)'BIND:"(New C(0), c1)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16, c1 As System.String), IsInvalid) (Syntax: TupleExpression, '(New C(0), c1)') (Parent: VariableInitializer)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int16, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'New C(0)')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IObjectCreationExpression (Constructor: Sub C..ctor(x As System.Int32)) ([0] OperationKind.ObjectCreationExpression, Type: C, IsInvalid) (Syntax: ObjectCreationExpression, 'New C(0)')
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument, IsInvalid) (Syntax: SimpleArgument, '0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Initializer: 
            null
    IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: Function C.op_Implicit(c As C) As System.String) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'c1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: Function C.op_Implicit(c As C) As System.String)
      Operand: 
        IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'C' cannot be converted to 'Short'.
        Dim t As (Short, String) = (New C(0), c1)'BIND:"(New C(0), c1)"
                                    ~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")>
        Public Sub TupleExpression_InvalidConversion_ParentVariableDeclaration()
            Dim source = <![CDATA[
Class C
    Private ReadOnly _x As Integer
    Public Sub New(x As Integer)
        _x = x
    End Sub

    Public Shared Widening Operator CType(value As Integer) As C
        Return New C(value)
    End Operator

    Public Shared Widening Operator CType(c As C) As Integer
        Return CShort(c._x)
    End Operator

    Public Shared Widening Operator CType(c As C) As String
        Return c._x.ToString()
    End Operator

    Public Sub M(c1 As C)
        Dim t As (Short, String) = (New C(0), c1)'BIND:"Dim t As (Short, String) = (New C(0), c1)"
    End Sub
End Class

]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim t As (S ... w C(0), c1)') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 't')
    Variables: Local_1: t As (System.Int16, System.String)
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= (New C(0), c1)')
        ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16, c1 As System.String), IsInvalid) (Syntax: TupleExpression, '(New C(0), c1)')
          Elements(2):
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int16, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'New C(0)')
              Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                IObjectCreationExpression (Constructor: Sub C..ctor(x As System.Int32)) ([0] OperationKind.ObjectCreationExpression, Type: C, IsInvalid) (Syntax: ObjectCreationExpression, 'New C(0)')
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument, IsInvalid) (Syntax: SimpleArgument, '0')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Initializer: 
                    null
            IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: Function C.op_Implicit(c As C) As System.String) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'c1')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: Function C.op_Implicit(c As C) As System.String)
              Operand: 
                IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'C' cannot be converted to 'Short'.
        Dim t As (Short, String) = (New C(0), c1)'BIND:"Dim t As (Short, String) = (New C(0), c1)"
                                    ~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
