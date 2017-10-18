' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")>
        Public Sub ObjectCreationWithMemberInitializers()
            Dim source = <![CDATA[
Structure B
    Public Field As Boolean
End Structure

Class F
    Public Field As Integer
    Public Property Property1() As String
    Public Property Property2() As B
End Class

Class C
    Public Sub M1()'BIND:"Public Sub M1()"
        Dim x1 = New F()
        Dim x2 = New F() With {.Field = 2}
        Dim x3 = New F() With {.Property1 = ""}
        Dim x4 = New F() With {.Property1 = "", .Field = 2}
        Dim x5 = New F() With {.Property2 = New B() With {.Field = True}}

        Dim e1 = New F() With {.Property2 = 1}
        Dim e2 = New F() From {""}
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (9 statements, 7 locals) ([Root] OperationKind.BlockStatement, IsInvalid) (Syntax: SubBlock, 'Public Sub  ... End Sub') (Parent: )
  Locals: Local_1: x1 As F
    Local_2: x2 As F
    Local_3: x3 As F
    Local_4: x4 As F
    Local_5: x5 As F
    Local_6: e1 As F
    Local_7: e2 As F
  IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x1 = New F()')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'x1')
      Variables: Local_1: x1 As F
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= New F()')
          IObjectCreationExpression (Constructor: Sub F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'New F()')
            Arguments(0)
            Initializer: 
              null
  IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x2 = Ne ... .Field = 2}')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'x2')
      Variables: Local_1: x2 As F
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= New F() W ... .Field = 2}')
          IObjectCreationExpression (Constructor: Sub F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'New F() Wit ... .Field = 2}')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectMemberInitializer, 'With {.Field = 2}')
                Initializers(1):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: NamedFieldInitializer, '.Field = 2')
                    Left: 
                      IFieldReferenceExpression: F.Field As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'Field')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: ObjectCreationExpression, 'New F() Wit ... .Field = 2}')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IVariableDeclarationStatement (1 declarations) ([2] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x3 = Ne ... erty1 = ""}')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'x3')
      Variables: Local_1: x3 As F
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= New F() W ... erty1 = ""}')
          IObjectCreationExpression (Constructor: Sub F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'New F() Wit ... erty1 = ""}')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectMemberInitializer, 'With {.Property1 = ""}')
                Initializers(1):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.Property1 = ""')
                    Left: 
                      IPropertyReferenceExpression: Property F.Property1 As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'Property1')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: ObjectCreationExpression, 'New F() Wit ... erty1 = ""}')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: "") (Syntax: StringLiteralExpression, '""')
  IVariableDeclarationStatement (1 declarations) ([3] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x4 = Ne ... .Field = 2}')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'x4')
      Variables: Local_1: x4 As F
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= New F() W ... .Field = 2}')
          IObjectCreationExpression (Constructor: Sub F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'New F() Wit ... .Field = 2}')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectMemberInitializer, 'With {.Prop ... .Field = 2}')
                Initializers(2):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.Property1 = ""')
                    Left: 
                      IPropertyReferenceExpression: Property F.Property1 As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'Property1')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: ObjectCreationExpression, 'New F() Wit ... .Field = 2}')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: "") (Syntax: StringLiteralExpression, '""')
                  ISimpleAssignmentExpression ([1] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: NamedFieldInitializer, '.Field = 2')
                    Left: 
                      IFieldReferenceExpression: F.Field As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'Field')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: ObjectCreationExpression, 'New F() Wit ... .Field = 2}')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IVariableDeclarationStatement (1 declarations) ([4] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x5 = Ne ... ld = True}}')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'x5')
      Variables: Local_1: x5 As F
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= New F() W ... ld = True}}')
          IObjectCreationExpression (Constructor: Sub F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'New F() Wit ... ld = True}}')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectMemberInitializer, 'With {.Prop ... ld = True}}')
                Initializers(1):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.Property2  ... eld = True}')
                    Left: 
                      IPropertyReferenceExpression: Property F.Property2 As B ([0] OperationKind.PropertyReferenceExpression, Type: B) (Syntax: IdentifierName, 'Property2')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: ObjectCreationExpression, 'New F() Wit ... ld = True}}')
                    Right: 
                      IObjectCreationExpression (Constructor: Sub B..ctor()) ([1] OperationKind.ObjectCreationExpression, Type: B) (Syntax: ObjectCreationExpression, 'New B() Wit ... eld = True}')
                        Arguments(0)
                        Initializer: 
                          IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: B) (Syntax: ObjectMemberInitializer, 'With {.Field = True}')
                            Initializers(1):
                              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: NamedFieldInitializer, '.Field = True')
                                Left: 
                                  IFieldReferenceExpression: B.Field As System.Boolean ([0] OperationKind.FieldReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'Field')
                                    Instance Receiver: 
                                      IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: B, IsImplicit) (Syntax: ObjectCreationExpression, 'New B() Wit ... eld = True}')
                                Right: 
                                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
  IVariableDeclarationStatement (1 declarations) ([5] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim e1 = Ne ... perty2 = 1}')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'e1')
      Variables: Local_1: e1 As F
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= New F() W ... perty2 = 1}')
          IObjectCreationExpression (Constructor: Sub F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F, IsInvalid) (Syntax: ObjectCreationExpression, 'New F() Wit ... perty2 = 1}')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F, IsInvalid) (Syntax: ObjectMemberInitializer, 'With {.Property2 = 1}')
                Initializers(1):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Void, IsInvalid) (Syntax: NamedFieldInitializer, '.Property2 = 1')
                    Left: 
                      IPropertyReferenceExpression: Property F.Property2 As B ([0] OperationKind.PropertyReferenceExpression, Type: B) (Syntax: IdentifierName, 'Property2')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'New F() Wit ... perty2 = 1}')
                    Right: 
                      IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: B, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                        Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                        Operand: 
                          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
  IVariableDeclarationStatement (1 declarations) ([6] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim e2 = Ne ... ) From {""}')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'e2')
      Variables: Local_1: e2 As F
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValue, '= New F() From {""}')
          IObjectCreationExpression (Constructor: Sub F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F, IsInvalid) (Syntax: ObjectCreationExpression, 'New F() From {""}')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F, IsInvalid) (Syntax: ObjectCollectionInitializer, 'From {""}')
                Initializers(1):
                  IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: StringLiteralExpression, '""')
                    Children(1):
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "", IsInvalid) (Syntax: StringLiteralExpression, '""')
  ILabeledStatement (Label: exit) ([7] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    Statement: 
      null
  IReturnStatement ([8] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
    ReturnedValue: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'Integer' cannot be converted to 'B'.
        Dim e1 = New F() With {.Property2 = 1}
                                            ~
BC36718: Cannot initialize the type 'F' with a collection initializer because it is not a collection type.
        Dim e2 = New F() From {""}
                         ~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")>
        Public Sub ObjectCreationWithCollectionInitializer()
            Dim source = <![CDATA[
Imports System.Collections.Generic

Class C
    Private ReadOnly field As Integer

    Public Sub M1(x As Integer)
        Dim y As Integer = 0
        Dim x1 = New List(Of Integer) From {x, y, field}'BIND:"New List(Of Integer) From {x, y, field}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IObjectCreationExpression (Constructor: Sub System.Collections.Generic.List(Of System.Int32)..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Collections.Generic.List(Of System.Int32)) (Syntax: ObjectCreationExpression, 'New List(Of ... , y, field}') (Parent: VariableInitializer)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.List(Of System.Int32)) (Syntax: ObjectCollectionInitializer, 'From {x, y, field}')
      Initializers(3):
        ICollectionElementInitializerExpression (AddMethod: Sub System.Collections.Generic.List(Of System.Int32).Add(item As System.Int32)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'x')
          Arguments(1):
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        ICollectionElementInitializerExpression (AddMethod: Sub System.Collections.Generic.List(Of System.Int32).Add(item As System.Int32)) (IsDynamic: False) ([1] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'y')
          Arguments(1):
            ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
        ICollectionElementInitializerExpression (AddMethod: Sub System.Collections.Generic.List(Of System.Int32).Add(item As System.Int32)) (IsDynamic: False) ([2] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'field')
          Arguments(1):
            IFieldReferenceExpression: C.field As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'field')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'field')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")>
        Public Sub ObjectCreationWithNestedCollectionInitializer()
            Dim source = <![CDATA[
Imports System.Collections.Generic
Imports System.Linq

Class C
    Private ReadOnly field As Integer

    Public Sub M1(x As Integer)
        Dim y As Integer = 0
        Dim x1 = New List(Of List(Of Integer)) From {{x, y}.ToList, New List(Of Integer) From {field}}'BIND:"New List(Of List(Of Integer)) From {{x, y}.ToList, New List(Of Integer) From {field}}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IObjectCreationExpression (Constructor: Sub System.Collections.Generic.List(Of System.Collections.Generic.List(Of System.Int32))..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Collections.Generic.List(Of System.Collections.Generic.List(Of System.Int32))) (Syntax: ObjectCreationExpression, 'New List(Of ... om {field}}') (Parent: VariableInitializer)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.List(Of System.Collections.Generic.List(Of System.Int32))) (Syntax: ObjectCollectionInitializer, 'From {{x, y ... om {field}}')
      Initializers(2):
        ICollectionElementInitializerExpression (AddMethod: Sub System.Collections.Generic.List(Of System.Collections.Generic.List(Of System.Int32)).Add(item As System.Collections.Generic.List(Of System.Int32))) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: SimpleMemberAccessExpression, '{x, y}.ToList')
          Arguments(1):
            IInvocationExpression ( Function System.Collections.Generic.IEnumerable(Of System.Int32).ToList() As System.Collections.Generic.List(Of System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.List(Of System.Int32)) (Syntax: SimpleMemberAccessExpression, '{x, y}.ToList')
              Instance Receiver: 
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable(Of System.Int32), IsImplicit) (Syntax: CollectionInitializer, '{x, y}')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32()) (Syntax: CollectionInitializer, '{x, y}')
                      Dimension Sizes(1):
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: CollectionInitializer, '{x, y}')
                      Initializer: 
                        IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer, IsImplicit) (Syntax: CollectionInitializer, '{x, y}')
                          Element Values(2):
                            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                            ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
              Arguments(0)
        ICollectionElementInitializerExpression (AddMethod: Sub System.Collections.Generic.List(Of System.Collections.Generic.List(Of System.Int32)).Add(item As System.Collections.Generic.List(Of System.Int32))) (IsDynamic: False) ([1] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: ObjectCreationExpression, 'New List(Of ... rom {field}')
          Arguments(1):
            IObjectCreationExpression (Constructor: Sub System.Collections.Generic.List(Of System.Int32)..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Collections.Generic.List(Of System.Int32)) (Syntax: ObjectCreationExpression, 'New List(Of ... rom {field}')
              Arguments(0)
              Initializer: 
                IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.List(Of System.Int32)) (Syntax: ObjectCollectionInitializer, 'From {field}')
                  Initializers(1):
                    ICollectionElementInitializerExpression (AddMethod: Sub System.Collections.Generic.List(Of System.Int32).Add(item As System.Int32)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'field')
                      Arguments(1):
                        IFieldReferenceExpression: C.field As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'field')
                          Instance Receiver: 
                            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'field')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")>
        Public Sub ObjectCreationWithMemberAndCollectionInitializers()
            Dim source = <![CDATA[
Imports System.Collections.Generic

Friend Class [Class]
    Public Property X As Integer
    Public Property Y As Integer()
    Public Property Z As Dictionary(Of Integer, Integer)
    Public Property C As [Class]

    Private ReadOnly field As Integer

    Public Sub M(x As Integer)
        Dim y As Integer = 0
        Dim c = New [Class]() With {'BIND:"New [Class]() With {"
            .X = x,
            .Y = {x, y, 3},
            .Z = New Dictionary(Of Integer, Integer) From {{x, y}},
            .C = New [Class]() With {.X = field}
        }
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IObjectCreationExpression (Constructor: Sub [Class]..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: [Class]) (Syntax: ObjectCreationExpression, 'New [Class] ... }') (Parent: VariableInitializer)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: [Class]) (Syntax: ObjectMemberInitializer, 'With {'BIND ... }')
      Initializers(4):
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.X = x')
          Left: 
            IPropertyReferenceExpression: Property [Class].X As System.Int32 ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: ObjectCreationExpression, 'New [Class] ... }')
          Right: 
            IParameterReferenceExpression: x ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        ISimpleAssignmentExpression ([1] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.Y = {x, y, 3}')
          Left: 
            IPropertyReferenceExpression: Property [Class].Y As System.Int32() ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'Y')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: ObjectCreationExpression, 'New [Class] ... }')
          Right: 
            IArrayCreationExpression ([1] OperationKind.ArrayCreationExpression, Type: System.Int32()) (Syntax: CollectionInitializer, '{x, y, 3}')
              Dimension Sizes(1):
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: CollectionInitializer, '{x, y, 3}')
              Initializer: 
                IArrayInitializer (3 elements) ([1] OperationKind.ArrayInitializer, IsImplicit) (Syntax: CollectionInitializer, '{x, y, 3}')
                  Element Values(3):
                    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                    ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
                    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        ISimpleAssignmentExpression ([2] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.Z = New Di ... om {{x, y}}')
          Left: 
            IPropertyReferenceExpression: Property [Class].Z As System.Collections.Generic.Dictionary(Of System.Int32, System.Int32) ([0] OperationKind.PropertyReferenceExpression, Type: System.Collections.Generic.Dictionary(Of System.Int32, System.Int32)) (Syntax: IdentifierName, 'Z')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: ObjectCreationExpression, 'New [Class] ... }')
          Right: 
            IObjectCreationExpression (Constructor: Sub System.Collections.Generic.Dictionary(Of System.Int32, System.Int32)..ctor()) ([1] OperationKind.ObjectCreationExpression, Type: System.Collections.Generic.Dictionary(Of System.Int32, System.Int32)) (Syntax: ObjectCreationExpression, 'New Diction ... om {{x, y}}')
              Arguments(0)
              Initializer: 
                IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.Dictionary(Of System.Int32, System.Int32)) (Syntax: ObjectCollectionInitializer, 'From {{x, y}}')
                  Initializers(1):
                    ICollectionElementInitializerExpression (AddMethod: Sub System.Collections.Generic.Dictionary(Of System.Int32, System.Int32).Add(key As System.Int32, value As System.Int32)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: CollectionInitializer, '{x, y}')
                      Arguments(2):
                        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                        ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
        ISimpleAssignmentExpression ([3] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.C = New [C ... .X = field}')
          Left: 
            IPropertyReferenceExpression: Property [Class].C As [Class] ([0] OperationKind.PropertyReferenceExpression, Type: [Class]) (Syntax: IdentifierName, 'C')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: ObjectCreationExpression, 'New [Class] ... }')
          Right: 
            IObjectCreationExpression (Constructor: Sub [Class]..ctor()) ([1] OperationKind.ObjectCreationExpression, Type: [Class]) (Syntax: ObjectCreationExpression, 'New [Class] ... .X = field}')
              Arguments(0)
              Initializer: 
                IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: [Class]) (Syntax: ObjectMemberInitializer, 'With {.X = field}')
                  Initializers(1):
                    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.X = field')
                      Left: 
                        IPropertyReferenceExpression: Property [Class].X As System.Int32 ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
                          Instance Receiver: 
                            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: ObjectCreationExpression, 'New [Class] ... .X = field}')
                      Right: 
                        IFieldReferenceExpression: [Class].field As System.Int32 ([1] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'field')
                          Instance Receiver: 
                            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: IdentifierName, 'field')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
