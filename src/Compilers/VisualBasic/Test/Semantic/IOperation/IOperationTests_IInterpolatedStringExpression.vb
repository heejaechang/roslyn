' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_Empty()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M()
        Console.WriteLine($"")'BIND:"$"""
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$""') (Parent: Argument)
  Parts(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_OnlyTextPart()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M()
        Console.WriteLine($"Only text part")'BIND:"$"Only text part""
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$"Only text part"') (Parent: Argument)
  Parts(1):
    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'Only text part')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Only text part") (Syntax: InterpolatedStringText, 'Only text part')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_OnlyInterpolationPart()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M()
        Console.WriteLine($"{1}")'BIND:"$"{1}""
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$"{1}"') (Parent: Argument)
  Parts(1):
    IInterpolation ([0] OperationKind.Interpolation) (Syntax: Interpolation, '{1}')
      Expression: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Alignment: 
        null
      FormatString: 
        null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_EmptyInterpolationPart()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M()
        Console.WriteLine($"{}")'BIND:"$"{}""
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([1] OperationKind.InterpolatedStringExpression, Type: System.String, IsInvalid) (Syntax: InterpolatedStringExpression, '$"{}"') (Parent: InvalidExpression)
  Parts(1):
    IInterpolation ([0] OperationKind.Interpolation, IsInvalid) (Syntax: Interpolation, '{}')
      Expression: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
      Alignment: 
        null
      FormatString: 
        null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30201: Expression expected.
        Console.WriteLine($"{}")'BIND:"$"{}""
                             ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_TextAndInterpolationParts()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M(x As Integer)
        Console.WriteLine($"String {x} and constant {1}")'BIND:"$"String {x} and constant {1}""
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$"String {x ... nstant {1}"') (Parent: Argument)
  Parts(4):
    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'String ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "String ") (Syntax: InterpolatedStringText, 'String ')
    IInterpolation ([1] OperationKind.Interpolation) (Syntax: Interpolation, '{x}')
      Expression: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
      Alignment: 
        null
      FormatString: 
        null
    IInterpolatedStringText ([2] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and constant ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: " and constant ") (Syntax: InterpolatedStringText, ' and constant ')
    IInterpolation ([3] OperationKind.Interpolation) (Syntax: Interpolation, '{1}')
      Expression: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Alignment: 
        null
      FormatString: 
        null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_FormatAndAlignment()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Private x As String = String.Empty
    Private y As Integer = 0

    Public Sub M()
        Console.WriteLine($"String {x,20} and {y:D3} and constant {1}")'BIND:"$"String {x,20} and {y:D3} and constant {1}""
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$"String {x ... nstant {1}"') (Parent: Argument)
  Parts(6):
    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'String ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "String ") (Syntax: InterpolatedStringText, 'String ')
    IInterpolation ([1] OperationKind.Interpolation) (Syntax: Interpolation, '{x,20}')
      Expression: 
        IFieldReferenceExpression: [Class].x As System.String ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: IdentifierName, 'x')
      Alignment: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
      FormatString: 
        null
    IInterpolatedStringText ([2] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: " and ") (Syntax: InterpolatedStringText, ' and ')
    IInterpolation ([3] OperationKind.Interpolation) (Syntax: Interpolation, '{y:D3}')
      Expression: 
        IFieldReferenceExpression: [Class].y As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: IdentifierName, 'y')
      Alignment: 
        null
      FormatString: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: "D3") (Syntax: InterpolationFormatClause, ':D3')
    IInterpolatedStringText ([4] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and constant ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: " and constant ") (Syntax: InterpolatedStringText, ' and constant ')
    IInterpolation ([5] OperationKind.Interpolation) (Syntax: Interpolation, '{1}')
      Expression: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Alignment: 
        null
      FormatString: 
        null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_InterpolationAndFormatAndAlignment()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Private x As String = String.Empty

    Public Sub M()
        Console.WriteLine($"String {x,20:D3}")'BIND:"$"String {x,20:D3}""
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$"String {x,20:D3}"') (Parent: Argument)
  Parts(2):
    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'String ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "String ") (Syntax: InterpolatedStringText, 'String ')
    IInterpolation ([1] OperationKind.Interpolation) (Syntax: Interpolation, '{x,20:D3}')
      Expression: 
        IFieldReferenceExpression: [Class].x As System.String ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: IdentifierName, 'x')
      Alignment: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
      FormatString: 
        ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.String, Constant: "D3") (Syntax: InterpolationFormatClause, ':D3')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_InvocationInInterpolation()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M()
        Dim x As String = String.Empty
        Dim y As Integer = 0
        Console.WriteLine($"String {x} and {M2(y)} and constant {1}")'BIND:"$"String {x} and {M2(y)} and constant {1}""
    End Sub

    Private Function M2(z As Integer) As String
        Return z.ToString()
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$"String {x ... nstant {1}"') (Parent: Argument)
  Parts(6):
    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'String ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "String ") (Syntax: InterpolatedStringText, 'String ')
    IInterpolation ([1] OperationKind.Interpolation) (Syntax: Interpolation, '{x}')
      Expression: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
      Alignment: 
        null
      FormatString: 
        null
    IInterpolatedStringText ([2] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: " and ") (Syntax: InterpolatedStringText, ' and ')
    IInterpolation ([3] OperationKind.Interpolation) (Syntax: Interpolation, '{M2(y)}')
      Expression: 
        IInvocationExpression ( Function [Class].M2(z As System.Int32) As System.String) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'M2(y)')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: IdentifierName, 'M2')
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'y')
              ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Alignment: 
        null
      FormatString: 
        null
    IInterpolatedStringText ([4] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and constant ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: " and constant ") (Syntax: InterpolatedStringText, ' and constant ')
    IInterpolation ([5] OperationKind.Interpolation) (Syntax: Interpolation, '{1}')
      Expression: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Alignment: 
        null
      FormatString: 
        null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_NestedInterpolation()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M()
        Dim x As String = String.Empty
        Dim y As Integer = 0
        Console.WriteLine($"String {M2($"{y}")}")'BIND:"$"String {M2($"{y}")}""
    End Sub

    Private Function M2(z As String) As Integer
        Return Int32.Parse(z)
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$"String {M2($"{y}")}"') (Parent: Argument)
  Parts(2):
    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'String ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "String ") (Syntax: InterpolatedStringText, 'String ')
    IInterpolation ([1] OperationKind.Interpolation) (Syntax: Interpolation, '{M2($"{y}")}')
      Expression: 
        IInvocationExpression ( Function [Class].M2(z As System.String) As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'M2($"{y}")')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: IdentifierName, 'M2')
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([1] OperationKind.Argument) (Syntax: SimpleArgument, '$"{y}"')
              IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$"{y}"')
                Parts(1):
                  IInterpolation ([0] OperationKind.Interpolation) (Syntax: Interpolation, '{y}')
                    Expression: 
                      ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
                    Alignment: 
                      null
                    FormatString: 
                      null
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Alignment: 
        null
      FormatString: 
        null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(18300, "https://github.com/dotnet/roslyn/issues/18300")>
        Public Sub InterpolatedStringExpression_InvalidExpressionInInterpolation()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M(x As Integer)
        Console.WriteLine($"String {x1} and constant {[Class]}")'BIND:"$"String {x1} and constant {[Class]}""
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInterpolatedStringExpression ([1] OperationKind.InterpolatedStringExpression, Type: System.String, IsInvalid) (Syntax: InterpolatedStringExpression, '$"String {x ...  {[Class]}"') (Parent: InvalidExpression)
  Parts(4):
    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'String ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "String ") (Syntax: InterpolatedStringText, 'String ')
    IInterpolation ([1] OperationKind.Interpolation, IsInvalid) (Syntax: Interpolation, '{x1}')
      Expression: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'x1')
          Children(0)
      Alignment: 
        null
      FormatString: 
        null
    IInterpolatedStringText ([2] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and constant ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: " and constant ") (Syntax: InterpolatedStringText, ' and constant ')
    IInterpolation ([3] OperationKind.Interpolation, IsInvalid) (Syntax: Interpolation, '{[Class]}')
      Expression: 
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, '[Class]')
      Alignment: 
        null
      FormatString: 
        null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30451: 'x1' is not declared. It may be inaccessible due to its protection level.
        Console.WriteLine($"String {x1} and constant {[Class]}")'BIND:"$"String {x1} and constant {[Class]}""
                                    ~~
BC30109: '[Class]' is a class type and cannot be used as an expression.
        Console.WriteLine($"String {x1} and constant {[Class]}")'BIND:"$"String {x1} and constant {[Class]}""
                                                      ~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of InterpolatedStringExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
