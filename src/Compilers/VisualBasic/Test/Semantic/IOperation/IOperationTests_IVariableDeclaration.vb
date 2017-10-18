' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Semantics
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics
    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase
#Region "Dim Declarations"

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub SingleVariableDeclaration()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1 As Integer'BIND:"Dim i1 As Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1 As Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Dim i1 As Integer'BIND:"Dim i1 As Integer"
            ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub MultipleVariableDeclarations()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1 As Integer, i2 As Integer, b1 As Boolean'BIND:"Dim i1 As Integer, i2 As Integer, b1 As Boolean"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (3 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1 As I ...  As Boolean') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([2] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'b1')
    Variables: Local_1: b1 As System.Boolean
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Dim i1 As Integer, i2 As Integer, b1 As Boolean'BIND:"Dim i1 As Integer, i2 As Integer, b1 As Boolean"
            ~~
BC42024: Unused local variable: 'i2'.
        Dim i1 As Integer, i2 As Integer, b1 As Boolean'BIND:"Dim i1 As Integer, i2 As Integer, b1 As Boolean"
                           ~~
BC42024: Unused local variable: 'b1'.
        Dim i1 As Integer, i2 As Integer, b1 As Boolean'BIND:"Dim i1 As Integer, i2 As Integer, b1 As Boolean"
                                          ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub SingleVariableDeclarationNoType()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Dim i1'BIND:"Dim i1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Dim i1'BIND:"Dim i1"
            ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub


        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub MultipleVariableDeclarationNoTypes()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Dim i1, i2'BIND:"Dim i1, i2"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1, i2') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Object
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Dim i1, i2'BIND:"Dim i1, i2"
            ~~
BC42024: Unused local variable: 'i2'.
        Dim i1, i2'BIND:"Dim i1, i2"
                ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub InvalidMultipleVariableDeclaration()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Dim i1 As Integer,'BIND:"Dim i1 As Integer,"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim i1 As Integer,') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: ModifiedIdentifier, '')
    Variables: Local_1:  As System.Object
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Dim i1 As Integer,'BIND:"Dim i1 As Integer,"
            ~~
BC30203: Identifier expected.
        Dim i1 As Integer,'BIND:"Dim i1 As Integer,"
                          ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub InvalidMultipleVariableDeclarationsNoType()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Dim i1,'BIND:"Dim i1,"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim i1,') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: ModifiedIdentifier, '')
    Variables: Local_1:  As System.Object
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Dim i1,'BIND:"Dim i1,"
            ~~
BC30203: Identifier expected.
        Dim i1,'BIND:"Dim i1,"
               ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub SingleVariableDeclarationLocalReferenceInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1 = 1
        Dim i2 = i1'BIND:"Dim i2 = i1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i2 = i1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub MultipleVariableDeclarationLocalReferenceInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1 = 1
        Dim i2 = i1, i3 = i1'BIND:"Dim i2 = i1, i3 = i1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i2 = i1, i3 = i1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i3')
    Variables: Local_1: i3 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub SingleVariableDeclarationExpressionInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1 = ReturnInt()'BIND:"Dim i1 = ReturnInt()"
    End Sub

    Function ReturnInt() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1 = ReturnInt()') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= ReturnInt()')
        IInvocationExpression (Function Program.ReturnInt() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'ReturnInt()')
          Instance Receiver: 
            null
          Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub MultipleVariableDeclarationExpressionInitializers()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1 = ReturnInt(), i2 = ReturnInt()'BIND:"Dim i1 = ReturnInt(), i2 = ReturnInt()"
    End Sub

    Function ReturnInt() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1 = Re ... ReturnInt()') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= ReturnInt()')
        IInvocationExpression (Function Program.ReturnInt() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'ReturnInt()')
          Instance Receiver: 
            null
          Arguments(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= ReturnInt()')
        IInvocationExpression (Function Program.ReturnInt() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'ReturnInt()')
          Instance Receiver: 
            null
          Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub DimAsNew()
            Dim source = <![CDATA[
Module Program
    Class C
    End Class
    Sub Main(args As String())
        Dim p1 As New C'BIND:"Dim p1 As New C"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim p1 As New C') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'p1')
    Variables: Local_1: p1 As Program.C
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New C')
        IObjectCreationExpression (Constructor: Sub Program.C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Program.C) (Syntax: ObjectCreationExpression, 'New C')
          Arguments(0)
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub MultipleDimAsNew()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1, i2 As New Integer'BIND:"Dim i1, i2 As New Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1, i2  ... New Integer') (Parent: BlockStatement)
  IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1, i2 As New Integer')
    Variables: Local_1: i1 As System.Int32
      Local_2: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
        IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
          Arguments(0)
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub


        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub DimAsNewNoObject()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1 As New'BIND:"Dim i1 As New"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim i1 As New') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As ?
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: AsNewClause, 'As New')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: ObjectCreationExpression, 'New')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30182: Type expected.
        Dim i1 As New'BIND:"Dim i1 As New"
                     ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub MultipleDimAsNewNoObject()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1, i2 As New'BIND:"Dim i1, i2 As New"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim i1, i2 As New') (Parent: BlockStatement)
  IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1, i2 As New')
    Variables: Local_1: i1 As ?
      Local_2: i2 As ?
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: AsNewClause, 'As New')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: ObjectCreationExpression, 'New')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30182: Type expected.
        Dim i1, i2 As New'BIND:"Dim i1, i2 As New"
                         ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub MixedDimAsNewAndEqualsDeclarations()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1, i2 As New Integer, b1 As Boolean = False'BIND:"Dim i1, i2 As New Integer, b1 As Boolean = False"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1, i2  ... ean = False') (Parent: BlockStatement)
  IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1, i2 As New Integer')
    Variables: Local_1: i1 As System.Int32
      Local_2: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
        IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
          Arguments(0)
          Initializer: 
            null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'b1')
    Variables: Local_1: b1 As System.Boolean
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= False')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'False')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub MixedDimAsNewAndEqualsDeclarationsReversedOrder()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim b1 As Boolean, i1, i2 As New Integer'BIND:"Dim b1 As Boolean, i1, i2 As New Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim b1 As B ... New Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'b1')
    Variables: Local_1: b1 As System.Boolean
    Initializer: 
      null
  IVariableDeclaration (2 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1, i2 As New Integer')
    Variables: Local_1: i1 As System.Int32
      Local_2: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
        IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
          Arguments(0)
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'b1'.
        Dim b1 As Boolean, i1, i2 As New Integer'BIND:"Dim b1 As Boolean, i1, i2 As New Integer"
            ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ArrayDeclarationWithLength()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1(2) As Integer'BIND:"Dim i1(2) As Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1(2) As Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1(2)')
    Variables: Local_1: i1 As System.Int32()
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsImplicit) (Syntax: ModifiedIdentifier, 'i1(2)')
        IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32()) (Syntax: ModifiedIdentifier, 'i1(2)')
          Dimension Sizes(1):
            IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: SimpleArgument, '2')
              Left: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '2')
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ArrayDeclarationMultipleVariables()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1(), i2 As Integer'BIND:"Dim i1(), i2 As Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim i1(), i2 As Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1()')
    Variables: Local_1: i1 As System.Int32()
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Dim i1(), i2 As Integer'BIND:"Dim i1(), i2 As Integer"
            ~~
BC42024: Unused local variable: 'i2'.
        Dim i1(), i2 As Integer'BIND:"Dim i1(), i2 As Integer"
                  ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ArrayDeclarationInvalidAsNew()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Dim i1(2) As New Integer'BIND:"Dim i1(2) As New Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim i1(2) As New Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1(2)')
    Variables: Local_1: i1 As System.Int32()
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: AsNewClause, 'As New Integer')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32(), IsInvalid) (Syntax: AsNewClause, 'As New Integer')
          Children(1):
            IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32, IsInvalid) (Syntax: ObjectCreationExpression, 'New Integer')
              Arguments(0)
              Initializer: 
                null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30053: Arrays cannot be declared with 'New'.
        Dim i1(2) As New Integer'BIND:"Dim i1(2) As New Integer"
                     ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

#End Region

#Region "Using Statements"

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub UsingStatementDeclarationAsNew()
            Dim source = <![CDATA[
Imports System

Module Program
    Class C
        Implements IDisposable
        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub
    End Class
    Sub Main(args As String())
        Using c1 As New C'BIND:"Using c1 As New C"
            Console.WriteLine(c1)
        End Using
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUsingStatement ([0] OperationKind.UsingStatement) (Syntax: UsingBlock, 'Using c1 As ... End Using') (Parent: BlockStatement)
  Resources: 
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: UsingStatement, 'Using c1 As New C')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'c1')
        Variables: Local_1: c1 As Program.C
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New C')
            IObjectCreationExpression (Constructor: Sub Program.C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Program.C) (Syntax: ObjectCreationExpression, 'New C')
              Arguments(0)
              Initializer: 
                null
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: UsingBlock, 'Using c1 As ... End Using')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(c1)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Object)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(c1)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'c1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'c1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: Program.C) (Syntax: IdentifierName, 'c1')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of UsingBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub


        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(Skip:="https://github.com/dotnet/roslyn/issues/17917"), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub UsingStatementDeclaration()
            Dim source = <![CDATA[
Module Program
    Class C
        Implements IDisposable
        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub
    End Class
    Sub Main(args As String())
        Using c1 As C = New C'BIND:"c1 As C = New C"
            Console.WriteLine(c1)
        End Using
    End Sub
End Module
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) (OperationKind.VariableDeclarationStatement) (Syntax: 'c1 As C = New C')
  IVariableDeclaration (1 variables) (OperationKind.VariableDeclaration) (Syntax: 'c1')
    Variables: Local_1: c1 As Program.C
    Initializer: IObjectCreationExpression (Constructor: Sub Program.C..ctor()) (OperationKind.ObjectCreationExpression, Type: Program.C) (Syntax: 'New C')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

#End Region

#Region "Const Declarations"

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstDeclaration()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 As Integer = 1'BIND:"Const i1 As Integer = 1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Const i1 As Integer = 1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42099: Unused local constant: 'i1'.
        Const i1 As Integer = 1'BIND:"Const i1 As Integer = 1"
              ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstMultipleDeclaration()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Const i1 = 1, i2 = 2'BIND:"Const i1 = 1, i2 = 2"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Const i1 = 1, i2 = 2') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 2')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42099: Unused local constant: 'i1'.
        Const i1 = 1, i2 = 2'BIND:"Const i1 = 1, i2 = 2"
              ~~
BC42099: Unused local constant: 'i2'.
        Const i1 = 1, i2 = 2'BIND:"Const i1 = 1, i2 = 2"
                      ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstAsNew()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 As New Integer'BIND:"Const i1 As New Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Const i1 As New Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: AsNewClause, 'As New Integer')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: ModifiedIdentifier, 'i1')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30438: Constants must have a value.
        Const i1 As New Integer'BIND:"Const i1 As New Integer"
              ~~
BC30246: 'New' is not valid on a local constant declaration.
        Const i1 As New Integer'BIND:"Const i1 As New Integer"
                    ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstAsNewMultipleDeclarations()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1, i2 As New Integer'BIND:"Const i1, i2 As New Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Const i1, i ... New Integer') (Parent: BlockStatement)
  IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1, i2 As New Integer')
    Variables: Local_1: i1 As System.Int32
      Local_2: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: AsNewClause, 'As New Integer')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: ModifiedIdentifier, 'i1')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30438: Constants must have a value.
        Const i1, i2 As New Integer'BIND:"Const i1, i2 As New Integer"
              ~~
BC42099: Unused local constant: 'i1'.
        Const i1, i2 As New Integer'BIND:"Const i1, i2 As New Integer"
              ~~
BC30438: Constants must have a value.
        Const i1, i2 As New Integer'BIND:"Const i1, i2 As New Integer"
                  ~~
BC30246: 'New' is not valid on a local constant declaration.
        Const i1, i2 As New Integer'BIND:"Const i1, i2 As New Integer"
                        ~~~
BC30246: 'New' is not valid on a local constant declaration.
        Const i1, i2 As New Integer'BIND:"Const i1, i2 As New Integer"
                        ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstSingleDeclarationNoType()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 = 1'BIND:"Const i1 = 1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Const i1 = 1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42099: Unused local constant: 'i1'.
        Const i1 = 1'BIND:"Const i1 = 1"
              ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstMultipleDeclarationsNoTypes()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 = 1, i2 = 'BIND:"Const i1 = 1, i2 = "
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Const i1 = 1, i2 = ') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= ')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42099: Unused local constant: 'i1'.
        Const i1 = 1, i2 = 'BIND:"Const i1 = 1, i2 = "
              ~~
BC30201: Expression expected.
        Const i1 = 1, i2 = 'BIND:"Const i1 = 1, i2 = "
                           ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstSingleDeclarationLocalReferenceInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 = 1
        Const i2 = i1'BIND:"Const i2 = i1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Const i2 = i1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: 1) (Syntax: IdentifierName, 'i1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42099: Unused local constant: 'i2'.
        Const i2 = i1'BIND:"Const i2 = i1"
              ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstMultipleDeclarationsLocalReferenceInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 = 1
        Const i2 = i1, i3 = i1'BIND:"Const i2 = i1, i3 = i1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Const i2 = i1, i3 = i1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: 1) (Syntax: IdentifierName, 'i1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i3')
    Variables: Local_1: i3 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: 1) (Syntax: IdentifierName, 'i1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42099: Unused local constant: 'i2'.
        Const i2 = i1, i3 = i1'BIND:"Const i2 = i1, i3 = i1"
              ~~
BC42099: Unused local constant: 'i3'.
        Const i2 = i1, i3 = i1'BIND:"Const i2 = i1, i3 = i1"
                       ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstSingleDeclarationExpressionInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 = Int1()'BIND:"Const i1 = Int1()"
    End Sub

    Function Int1() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Const i1 = Int1()') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= Int1()')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Int1()')
          Children(1):
            IInvocationExpression (Function Program.Int1() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'Int1()')
              Instance Receiver: 
                null
              Arguments(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30059: Constant expression is required.
        Const i1 = Int1()'BIND:"Const i1 = Int1()"
                   ~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstMultipleDeclarationsExpressionInitializers()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 = Int1(), i2 = Int1()'BIND:"Const i1 = Int1(), i2 = Int1()"
    End Sub

    Function Int1() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Const i1 =  ... i2 = Int1()') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= Int1()')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Int1()')
          Children(1):
            IInvocationExpression (Function Program.Int1() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'Int1()')
              Instance Receiver: 
                null
              Arguments(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= Int1()')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Int1()')
          Children(1):
            IInvocationExpression (Function Program.Int1() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'Int1()')
              Instance Receiver: 
                null
              Arguments(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30059: Constant expression is required.
        Const i1 = Int1(), i2 = Int1()'BIND:"Const i1 = Int1(), i2 = Int1()"
                   ~~~~~~
BC30059: Constant expression is required.
        Const i1 = Int1(), i2 = Int1()'BIND:"Const i1 = Int1(), i2 = Int1()"
                                ~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstDimAsNewNoInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 As New'BIND:"Const i1 As New"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Const i1 As New') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As ?
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: AsNewClause, 'As New')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?) (Syntax: ModifiedIdentifier, 'i1')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30246: 'New' is not valid on a local constant declaration.
        Const i1 As New'BIND:"Const i1 As New"
                    ~~~
BC30182: Type expected.
        Const i1 As New'BIND:"Const i1 As New"
                       ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstDimAsNewMultipleDeclarationsNoInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1, i2 As New'BIND:"Const i1, i2 As New"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Const i1, i2 As New') (Parent: BlockStatement)
  IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'i1, i2 As New')
    Variables: Local_1: i1 As ?
      Local_2: i2 As ?
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: AsNewClause, 'As New')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?) (Syntax: ModifiedIdentifier, 'i1')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42099: Unused local constant: 'i1'.
        Const i1, i2 As New'BIND:"Const i1, i2 As New"
              ~~
BC30246: 'New' is not valid on a local constant declaration.
        Const i1, i2 As New'BIND:"Const i1, i2 As New"
                        ~~~
BC30246: 'New' is not valid on a local constant declaration.
        Const i1, i2 As New'BIND:"Const i1, i2 As New"
                        ~~~
BC30182: Type expected.
        Const i1, i2 As New'BIND:"Const i1, i2 As New"
                           ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub ConstInvalidMultipleDeclaration()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Const i1 = 1,'BIND:"Const i1 = 1,"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Const i1 = 1,') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: ModifiedIdentifier, '')
    Variables: Local_1:  As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid, IsImplicit) (Syntax: ModifiedIdentifier, '')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: ModifiedIdentifier, '')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42099: Unused local constant: 'i1'.
        Const i1 = 1,'BIND:"Const i1 = 1,"
              ~~
BC30203: Identifier expected.
        Const i1 = 1,'BIND:"Const i1 = 1,"
                     ~
BC30438: Constants must have a value.
        Const i1 = 1,'BIND:"Const i1 = 1,"
                     ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

#End Region

#Region "Static Declarations"

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticDeclaration()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Static i1 As Integer'BIND:"Static i1 As Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1 As Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Static i1 As Integer'BIND:"Static i1 As Integer"
               ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticMultipleDeclarations()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Static i1, i2 As Integer'BIND:"Static i1, i2 As Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1, i2 As Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Int32
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Static i1, i2 As Integer'BIND:"Static i1, i2 As Integer"
               ~~
BC42024: Unused local variable: 'i2'.
        Static i1, i2 As Integer'BIND:"Static i1, i2 As Integer"
                   ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticAsNewDeclaration()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Static i1 As New Integer'BIND:"Static i1 As New Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1 As New Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
        IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
          Arguments(0)
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticMulipleDeclarationAsNew()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Static i1, i2 As New Integer'BIND:"Static i1, i2 As New Integer"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1,  ... New Integer') (Parent: BlockStatement)
  IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1, i2 As New Integer')
    Variables: Local_1: i1 As System.Int32
      Local_2: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
        IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
          Arguments(0)
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticMixedAsNewAndEqualsDeclaration()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        Static i1, i2 As New Integer, b1 As Boolean = False'BIND:"Static i1, i2 As New Integer, b1 As Boolean = False"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1,  ... ean = False') (Parent: BlockStatement)
  IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i1, i2 As New Integer')
    Variables: Local_1: i1 As System.Int32
      Local_2: i2 As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Integer')
        IObjectCreationExpression (Constructor: Sub System.Int32..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Int32) (Syntax: ObjectCreationExpression, 'New Integer')
          Arguments(0)
          Initializer: 
            null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'b1')
    Variables: Local_1: b1 As System.Boolean
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= False')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'False')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticSingleDeclarationNoType()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 = 1'BIND:"Static i1 = 1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1 = 1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 1')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: NumericLiteralExpression, '1')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticMultipleDeclarationsNoTypes()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 = 1, i2 = 2'BIND:"Static i1 = 1, i2 = 2"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1 = 1, i2 = 2') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 1')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: NumericLiteralExpression, '1')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 2')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: NumericLiteralExpression, '2')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticSingleDeclarationLocalReferenceInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 = 1
        Static i2 = i1'BIND:"Static i2 = i1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i2 = i1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'i1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticMultipleDeclarationsLocalReferenceInitializers()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 = 1
        Static i2 = i1, i3 = i1'BIND:"Static i2 = i1, i3 = i1"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i2 = i1, i3 = i1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'i1')
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i3')
    Variables: Local_1: i3 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= i1')
        ILocalReferenceExpression: i1 ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'i1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticSingleDeclarationExpressionInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 = Int1()'BIND:"Static i1 = Int1()"
    End Sub

    Function Int1() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1 = Int1()') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= Int1()')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: InvocationExpression, 'Int1()')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvocationExpression (Function Program.Int1() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Int1()')
              Instance Receiver: 
                null
              Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticMultipleDeclarationsExpressionInitializers()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 = Int1(), i2 = Int1()'BIND:"Static i1 = Int1(), i2 = Int1()"
    End Sub

    Function Int1() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Static i1 = ... i2 = Int1()') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= Int1()')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: InvocationExpression, 'Int1()')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvocationExpression (Function Program.Int1() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Int1()')
              Instance Receiver: 
                null
              Arguments(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= Int1()')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: InvocationExpression, 'Int1()')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvocationExpression (Function Program.Int1() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Int1()')
              Instance Receiver: 
                null
              Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticAsNewSingleDeclarationInvalidInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 As'BIND:"Static i1 As"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Static i1 As') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As ?
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Static i1 As'BIND:"Static i1 As"
               ~~
BC30182: Type expected.
        Static i1 As'BIND:"Static i1 As"
                    ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticAsNewMultipleDeclarationInvalidInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1, i2 As'BIND:"Static i1, i2 As"
    End Sub

    Function Int1() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Static i1, i2 As') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As ?
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As ?
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Static i1, i2 As'BIND:"Static i1, i2 As"
               ~~
BC42024: Unused local variable: 'i2'.
        Static i1, i2 As'BIND:"Static i1, i2 As"
                   ~~
BC30182: Type expected.
        Static i1, i2 As'BIND:"Static i1, i2 As"
                        ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticSingleDeclarationInvalidInitializer()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 ='BIND:"Static i1 ="
    End Sub

    Function Int1() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Static i1 =') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '=')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30201: Expression expected.
        Static i1 ='BIND:"Static i1 ="
                   ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticMultipleDeclarationsInvalidInitializers()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1 =, i2 ='BIND:"Static i1 =, i2 ="
    End Sub

    Function Int1() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Static i1 =, i2 =') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '=')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i2')
    Variables: Local_1: i2 As System.Object
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '=')
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30201: Expression expected.
        Static i1 =, i2 ='BIND:"Static i1 =, i2 ="
                   ~
BC30201: Expression expected.
        Static i1 =, i2 ='BIND:"Static i1 =, i2 ="
                         ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17599, "https://github.com/dotnet/roslyn/issues/17599")>
        Public Sub StaticInvalidMultipleDeclaration()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Static i1,'BIND:"Static i1,"
    End Sub

    Function Int1() As Integer
        Return 1
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Static i1,') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'i1')
    Variables: Local_1: i1 As System.Object
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: ModifiedIdentifier, '')
    Variables: Local_1:  As System.Object
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'i1'.
        Static i1,'BIND:"Static i1,"
               ~~
BC30203: Identifier expected.
        Static i1,'BIND:"Static i1,"
                  ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

#End Region

#Region "Initializers"
        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestGetOperationForEqualsValueVariableInitializer()
            Dim source = <![CDATA[
Class Test
    Sub M()
        Dim x = 1'BIND:"= 1"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 1') (Parent: VariableDeclaration)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of EqualsValueSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestGetOperationForEqualsValueVariableInitializerWithMultipleLocals()
            Dim source = <![CDATA[
Class Test
    Sub M()
        Dim x, y = 1'BIND:"= 1"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= 1') (Parent: VariableDeclaration)
  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'x'.
        Dim x, y = 1'BIND:"= 1"
            ~
BC30671: Explicit initialization is not permitted with multiple variables declared with a single type specifier.
        Dim x, y = 1'BIND:"= 1"
            ~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of EqualsValueSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestGetOperationForEqualsValueVariableDeclarationWithMultipleLocals()
            Dim source = <![CDATA[
Class Test
    Sub M()
        Dim x, y = 1'BIND:"Dim x, y = 1"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim x, y = 1') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: ModifiedIdentifier, 'x')
    Variables: Local_1: x As System.Object
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: ModifiedIdentifier, 'y')
    Variables: Local_1: y As System.Int32
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= 1')
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'x'.
        Dim x, y = 1'BIND:"Dim x, y = 1"
            ~
BC30671: Explicit initialization is not permitted with multiple variables declared with a single type specifier.
        Dim x, y = 1'BIND:"Dim x, y = 1"
            ~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestGetOperationForAsNewVariableInitializer()
            Dim source = <![CDATA[
Class Test
    Sub M()
        Dim x As New Test'BIND:"As New Test"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Test') (Parent: VariableDeclaration)
  IObjectCreationExpression (Constructor: Sub Test..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Test) (Syntax: ObjectCreationExpression, 'New Test')
    Arguments(0)
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AsNewClauseSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestGetOperationForAsNewVariableDeclaration()
            Dim source = <![CDATA[
Class Test
    Sub M()
        Dim x As New Test'BIND:"Dim x As New Test"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x As New Test') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'x')
    Variables: Local_1: x As Test
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Test')
        IObjectCreationExpression (Constructor: Sub Test..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Test) (Syntax: ObjectCreationExpression, 'New Test')
          Arguments(0)
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestGetOperationForAsNewVariableInitializerWithMultipleLocals()
            Dim source = <![CDATA[
Class Test
    Sub M()
        Dim x, y As New Test'BIND:"As New Test"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Test') (Parent: VariableDeclaration)
  IObjectCreationExpression (Constructor: Sub Test..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Test) (Syntax: ObjectCreationExpression, 'New Test')
    Arguments(0)
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AsNewClauseSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestGetOperationForAsNewVariableDeclarationWithMultipleLocals()
            Dim source = <![CDATA[
Class Test
    Sub M()
        Dim x, y As New Test'BIND:"x, y As New Test"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclaration (2 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x, y As New Test') (Parent: VariableDeclarationStatement)
  Variables: Local_1: x As Test
    Local_2: y As Test
  Initializer: 
    IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: AsNewClause, 'As New Test')
      IObjectCreationExpression (Constructor: Sub Test..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Test) (Syntax: ObjectCreationExpression, 'New Test')
        Arguments(0)
        Initializer: 
          null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of VariableDeclaratorSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
#End Region
    End Class
End Namespace
