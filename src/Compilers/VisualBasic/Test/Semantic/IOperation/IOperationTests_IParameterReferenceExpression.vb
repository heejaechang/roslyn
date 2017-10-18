' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Semantics
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_TupleExpression()
            Dim source = <![CDATA[
Class Class1
    Public Sub M(x As Integer, y As Integer)
        Dim tuple = (x, x + y)'BIND:"(x, x + y)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITupleExpression ([0] OperationKind.TupleExpression, Type: (x As System.Int32, System.Int32)) (Syntax: TupleExpression, '(x, x + y)') (Parent: VariableInitializer)
  Elements(2):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + y')
      Left: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
      Right: 
        IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of TupleExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_AnonymousObjectCreation()
            Dim source = <![CDATA[
Class Class1
    Public Sub M(x As Integer, y As String)
        Dim v = New With {'BIND:"New With {"'BIND:"New With {'BIND:"New With {""
            Key .Amount = x,
            Key .Message = "Hello" + y
        }
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAnonymousObjectCreationExpression ([0] OperationKind.AnonymousObjectCreationExpression, Type: <anonymous type: Key Amount As System.Int32, Key Message As System.String>) (Syntax: AnonymousObjectCreationExpression, 'New With {' ... }') (Parent: VariableInitializer)
  Initializers(2):
    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: NamedFieldInitializer, 'Key .Amount = x')
      Left: 
        IPropertyReferenceExpression: ReadOnly Property <anonymous type: Key Amount As System.Int32, Key Message As System.String>.Amount As System.Int32 ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'Amount')
          Instance Receiver: 
            null
      Right: 
        IParameterReferenceExpression: x ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
    ISimpleAssignmentExpression ([1] OperationKind.SimpleAssignmentExpression, Type: System.String) (Syntax: NamedFieldInitializer, 'Key .Messag ... "Hello" + y')
      Left: 
        IPropertyReferenceExpression: ReadOnly Property <anonymous type: Key Amount As System.Int32, Key Message As System.String>.Message As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'Message')
          Instance Receiver: 
            null
      Right: 
        IBinaryOperatorExpression (BinaryOperatorKind.Concatenate, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: AddExpression, '"Hello" + y')
          Left: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Hello") (Syntax: StringLiteralExpression, '"Hello"')
          Right: 
            IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'y')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AnonymousObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_QueryExpression()
            Dim source = <![CDATA[
Imports System.Linq
Imports System.Collections.Generic

Structure Customer
    Public Property Name As String
    Public Property Address As String
End Structure

Class Class1
    Public Sub M(customers As List(Of Customer))
        Dim result = From cust In customers'BIND:"From cust In customers"
                     Select cust.Name
    End Sub
End Class

]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITranslatedQueryExpression ([0] OperationKind.TranslatedQueryExpression, Type: System.Collections.Generic.IEnumerable(Of System.String)) (Syntax: QueryExpression, 'From cust I ... t cust.Name') (Parent: VariableInitializer)
  Expression: 
    IInvocationExpression ( Function System.Collections.Generic.IEnumerable(Of Customer).Select(Of System.String)(selector As System.Func(Of Customer, System.String)) As System.Collections.Generic.IEnumerable(Of System.String)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable(Of System.String), IsImplicit) (Syntax: SelectClause, 'Select cust.Name')
      Instance Receiver: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable(Of Customer), IsImplicit) (Syntax: CollectionRangeVariable, 'cust In customers')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IParameterReferenceExpression: customers ([0] OperationKind.ParameterReferenceExpression, Type: System.Collections.Generic.List(Of Customer)) (Syntax: IdentifierName, 'customers')
      Arguments(1):
        IArgument (ArgumentKind.DefaultValue, Matching Parameter: selector) ([1] OperationKind.Argument, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
          IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func(Of Customer, System.String), IsImplicit) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
            Target: 
              IAnonymousFunctionExpression (Symbol: Function (cust As Customer) As System.String) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
                IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
                  IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
                    ReturnedValue: 
                      IPropertyReferenceExpression: Property Customer.Name As System.String ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
                        Instance Receiver: 
                          IParameterReferenceExpression: cust ([0] OperationKind.ParameterReferenceExpression, Type: Customer) (Syntax: SimpleMemberAccessExpression, 'cust.Name')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of QueryExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_QueryExpressionAggregateClause()
            Dim source = <![CDATA[
Option Strict Off
Option Infer On

Imports System
Imports System.Collections
Imports System.Linq


Class C
    Public Sub Method(x As Integer)
        Console.WriteLine(Aggregate y In New Integer() {x} Into Count())'BIND:"Aggregate y In New Integer() {x} Into Count()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITranslatedQueryExpression ([0] OperationKind.TranslatedQueryExpression, Type: System.Int32) (Syntax: QueryExpression, 'Aggregate y ... nto Count()') (Parent: Argument)
  Expression: 
    IInvocationExpression ( Function System.Collections.Generic.IEnumerable(Of System.Int32).Count() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsImplicit) (Syntax: FunctionAggregation, 'Count()')
      Instance Receiver: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable(Of System.Int32), IsImplicit) (Syntax: CollectionRangeVariable, 'y In New Integer() {x}')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32()) (Syntax: ArrayCreationExpression, 'New Integer() {x}')
              Dimension Sizes(1):
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ArrayCreationExpression, 'New Integer() {x}')
              Initializer: 
                IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{x}')
                  Element Values(1):
                    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
      Arguments(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of QueryExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_QueryExpressionOrderByClause()
            Dim source = <![CDATA[
Option Strict Off
Option Infer On

Imports System
Imports System.Collections
Imports System.Linq


Class C
    Public Sub Method(x As String())
        Console.WriteLine(From y In x Order By y.Length)'BIND:"From y In x Order By y.Length"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITranslatedQueryExpression ([0] OperationKind.TranslatedQueryExpression, Type: System.Linq.IOrderedEnumerable(Of System.String)) (Syntax: QueryExpression, 'From y In x ... By y.Length') (Parent: ConversionExpression)
  Expression: 
    IInvocationExpression ( Function System.Collections.Generic.IEnumerable(Of System.String).OrderBy(Of System.Int32)(keySelector As System.Func(Of System.String, System.Int32)) As System.Linq.IOrderedEnumerable(Of System.String)) ([0] OperationKind.InvocationExpression, Type: System.Linq.IOrderedEnumerable(Of System.String), IsImplicit) (Syntax: AscendingOrdering, 'y.Length')
      Instance Receiver: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable(Of System.String), IsImplicit) (Syntax: CollectionRangeVariable, 'y In x')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String()) (Syntax: IdentifierName, 'x')
      Arguments(1):
        IArgument (ArgumentKind.DefaultValue, Matching Parameter: keySelector) ([1] OperationKind.Argument, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'y.Length')
          IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func(Of System.String, System.Int32), IsImplicit) (Syntax: SimpleMemberAccessExpression, 'y.Length')
            Target: 
              IAnonymousFunctionExpression (Symbol: Function (y As System.String) As System.Int32) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'y.Length')
                IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: SimpleMemberAccessExpression, 'y.Length')
                  IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: SimpleMemberAccessExpression, 'y.Length')
                    ReturnedValue: 
                      IPropertyReferenceExpression: ReadOnly Property System.String.Length As System.Int32 ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'y.Length')
                        Instance Receiver: 
                          IParameterReferenceExpression: y ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'y.Length')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of QueryExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_QueryExpressionGroupByClause()
            Dim source = <![CDATA[
Option Strict Off
Option Infer On

Imports System
Imports System.Collections
Imports System.Linq


Class C
    Public Sub Method(x As String())
        Dim c = From y In x Group By w = x, z = y Into Count()'BIND:"From y In x Group By w = x, z = y Into Count()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ITranslatedQueryExpression ([0] OperationKind.TranslatedQueryExpression, Type: System.Collections.Generic.IEnumerable(Of <anonymous type: Key w As System.String(), Key z As System.String, Key Count As System.Int32>)) (Syntax: QueryExpression, 'From y In x ... nto Count()') (Parent: VariableInitializer)
  Expression: 
    IInvocationExpression ( Function System.Collections.Generic.IEnumerable(Of System.String).GroupBy(Of <anonymous type: Key w As System.String(), Key z As System.String>, <anonymous type: Key w As System.String(), Key z As System.String, Key Count As System.Int32>)(keySelector As System.Func(Of System.String, <anonymous type: Key w As System.String(), Key z As System.String>), resultSelector As System.Func(Of <anonymous type: Key w As System.String(), Key z As System.String>, System.Collections.Generic.IEnumerable(Of System.String), <anonymous type: Key w As System.String(), Key z As System.String, Key Count As System.Int32>)) As System.Collections.Generic.IEnumerable(Of <anonymous type: Key w As System.String(), Key z As System.String, Key Count As System.Int32>)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.IEnumerable(Of <anonymous type: Key w As System.String(), Key z As System.String, Key Count As System.Int32>), IsImplicit) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
      Instance Receiver: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable(Of System.String), IsImplicit) (Syntax: CollectionRangeVariable, 'y In x')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String()) (Syntax: IdentifierName, 'x')
      Arguments(2):
        IArgument (ArgumentKind.DefaultValue, Matching Parameter: keySelector) ([1] OperationKind.Argument, IsImplicit) (Syntax: IdentifierName, 'x')
          IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func(Of System.String, <anonymous type: Key w As System.String(), Key z As System.String>), IsImplicit) (Syntax: IdentifierName, 'x')
            Target: 
              IAnonymousFunctionExpression (Symbol: Function (y As System.String) As <anonymous type: Key w As System.String(), Key z As System.String>) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: IdentifierName, 'x')
                IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: IdentifierName, 'x')
                  IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: IdentifierName, 'x')
                    ReturnedValue: 
                      IAnonymousObjectCreationExpression ([0] OperationKind.AnonymousObjectCreationExpression, Type: <anonymous type: Key w As System.String(), Key z As System.String>, IsImplicit) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                        Initializers(2):
                          IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String()) (Syntax: IdentifierName, 'x')
                          IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.DefaultValue, Matching Parameter: resultSelector) ([2] OperationKind.Argument, IsImplicit) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
          IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.Func(Of <anonymous type: Key w As System.String(), Key z As System.String>, System.Collections.Generic.IEnumerable(Of System.String), <anonymous type: Key w As System.String(), Key z As System.String, Key Count As System.Int32>), IsImplicit) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
            Target: 
              IAnonymousFunctionExpression (Symbol: Function ($VB$It As <anonymous type: Key w As System.String(), Key z As System.String>, $VB$ItAnonymous As System.Collections.Generic.IEnumerable(Of System.String)) As <anonymous type: Key w As System.String(), Key z As System.String, Key Count As System.Int32>) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsImplicit) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                  IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                    ReturnedValue: 
                      IAnonymousObjectCreationExpression ([0] OperationKind.AnonymousObjectCreationExpression, Type: <anonymous type: Key w As System.String(), Key z As System.String, Key Count As System.Int32>, IsImplicit) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                        Initializers(3):
                          IPropertyReferenceExpression: ReadOnly Property <anonymous type: Key w As System.String(), Key z As System.String>.w As System.String() ([0] OperationKind.PropertyReferenceExpression, Type: System.String()) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                            Instance Receiver: 
                              IParameterReferenceExpression: $VB$It ([0] OperationKind.ParameterReferenceExpression, Type: <anonymous type: Key w As System.String(), Key z As System.String>) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                          IPropertyReferenceExpression: ReadOnly Property <anonymous type: Key w As System.String(), Key z As System.String>.z As System.String ([1] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                            Instance Receiver: 
                              IParameterReferenceExpression: $VB$It ([0] OperationKind.ParameterReferenceExpression, Type: <anonymous type: Key w As System.String(), Key z As System.String>) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                          IInvocationExpression ( Function System.Collections.Generic.IEnumerable(Of System.String).Count() As System.Int32) ([2] OperationKind.InvocationExpression, Type: System.Int32, IsImplicit) (Syntax: FunctionAggregation, 'Count()')
                            Instance Receiver: 
                              IParameterReferenceExpression: $VB$ItAnonymous ([0] OperationKind.ParameterReferenceExpression, Type: System.Collections.Generic.IEnumerable(Of System.String), IsImplicit) (Syntax: GroupByClause, 'Group By w  ... nto Count()')
                            Arguments(0)
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of QueryExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_ObjectAndCollectionInitializer()
            Dim source = <![CDATA[
Imports System.Collections.Generic

Friend Class [Class]
    Public Property X As Integer
    Public Property Y As Integer()
    Public Property Z As Dictionary(Of Integer, Integer)
    Public Property C As [Class]

    Public Sub M(x As Integer, y As Integer, z As Integer)
        Dim c = New [Class]() With {'BIND:"New [Class]() With {"'BIND:"New [Class]() With {'BIND:"New [Class]() With {""
            .X = x,
            .Y = {x, y, 3},
            .Z = New Dictionary(Of Integer, Integer) From {{x, y}},
            .C = New [Class]() With {.X = z}
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
                    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
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
                        IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
        ISimpleAssignmentExpression ([3] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.C = New [C ... th {.X = z}')
          Left: 
            IPropertyReferenceExpression: Property [Class].C As [Class] ([0] OperationKind.PropertyReferenceExpression, Type: [Class]) (Syntax: IdentifierName, 'C')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: ObjectCreationExpression, 'New [Class] ... }')
          Right: 
            IObjectCreationExpression (Constructor: Sub [Class]..ctor()) ([1] OperationKind.ObjectCreationExpression, Type: [Class]) (Syntax: ObjectCreationExpression, 'New [Class] ... th {.X = z}')
              Arguments(0)
              Initializer: 
                IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: [Class]) (Syntax: ObjectMemberInitializer, 'With {.X = z}')
                  Initializers(1):
                    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: NamedFieldInitializer, '.X = z')
                      Left: 
                        IPropertyReferenceExpression: Property [Class].X As System.Int32 ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
                          Instance Receiver: 
                            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: [Class], IsImplicit) (Syntax: ObjectCreationExpression, 'New [Class] ... th {.X = z}')
                      Right: 
                        IParameterReferenceExpression: z ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'z')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_DelegateCreationExpressionWithLambdaArgument()
            Dim source = <![CDATA[
Option Strict Off
Imports System

Class Class1
    Delegate Sub DelegateType()
    Public Sub M(x As Object, y As EventArgs)
        Dim eventHandler As New EventHandler(Function() x)'BIND:"New EventHandler(Function() x)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.EventHandler) (Syntax: ObjectCreationExpression, 'New EventHa ... nction() x)') (Parent: VariableInitializer)
  Target: 
    IAnonymousFunctionExpression (Symbol: Function () As System.Object) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: SingleLineFunctionLambdaExpression, 'Function() x')
      IBlockStatement (3 statements, 1 locals) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() x')
        Locals: Local_1: <anonymous local> As System.Object
        IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: IdentifierName, 'x')
          ReturnedValue: 
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
        ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() x')
          Statement: 
            null
        IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() x')
          ReturnedValue: 
            ILocalReferenceExpression:  ([0] OperationKind.LocalReferenceExpression, Type: System.Object, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() x')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_DelegateCreationExpressionWithMethodArgument()
            Dim source = <![CDATA[
Imports System

Class Class1
    Public Sub M(x As Object, y As EventArgs)
        Dim eventHandler As New EventHandler(AddressOf Me.M)'BIND:"New EventHandler(AddressOf Me.M)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDelegateCreationExpression ([0] OperationKind.DelegateCreationExpression, Type: System.EventHandler) (Syntax: ObjectCreationExpression, 'New EventHa ... essOf Me.M)') (Parent: VariableInitializer)
  Target: 
    IMethodReferenceExpression: Sub Class1.M(x As System.Object, y As System.EventArgs) ([0] OperationKind.MethodReferenceExpression, Type: null) (Syntax: AddressOfExpression, 'AddressOf Me.M')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class1) (Syntax: MeExpression, 'Me')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_DelegateCreationExpressionWithInvalidArgument()
            Dim source = <![CDATA[
Option Strict Off
Imports System

Class Class1
    Delegate Sub DelegateType()
    Public Sub M(x As Object, y As EventArgs)
        Dim eventHandler As New EventHandler(x)'BIND:"New EventHandler(x)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.EventHandler, IsInvalid) (Syntax: ObjectCreationExpression, 'New EventHandler(x)') (Parent: VariableInitializer)
  Children(1):
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object, IsInvalid) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC32008: Delegate 'EventHandler' requires an 'AddressOf' expression or lambda expression as the only argument to its constructor.
        Dim eventHandler As New EventHandler(x)'BIND:"New EventHandler(x)"
                                            ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_NameOfExpression()
            Dim source = <![CDATA[
Class Class1
    Public Function M(x As Integer) As String
        Return NameOf(x)'BIND:"NameOf(x)"
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
INameOfExpression ([0] OperationKind.NameOfExpression, Type: System.String, Constant: "x") (Syntax: NameOfExpression, 'NameOf(x)') (Parent: ReturnStatement)
  IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of NameOfExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_NameOfExpression_ErrorCase()
            Dim source = <![CDATA[
Class Class1
    Public Function M(x As Integer, y As Integer) As String
        Return NameOf(x + y)'BIND:"NameOf(x + y)"
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
INameOfExpression ([0] OperationKind.NameOfExpression, Type: System.String, Constant: null, IsInvalid) (Syntax: NameOfExpression, 'NameOf(x + y)') (Parent: ReturnStatement)
  IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: AddExpression, 'x + y')
    Left: 
      IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'x')
    Right: 
      IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'y')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC37244: This expression does not have a name.
        Return NameOf(x + y)'BIND:"NameOf(x + y)"
                      ~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of NameOfExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_LateBoundIndexerAccess()
            Dim source = <![CDATA[
Option Strict Off

Class Class1
    Public Sub M(d As Object, x As Integer)
        Dim y = d(x)'BIND:"d(x)"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'd(x)') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: d ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'd')
  Arguments(1):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'x')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  ArgumentNames(0)
  ArgumentRefKinds: null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_LateBoundMemberAccess()
            Dim source = <![CDATA[
Option Strict Off

Class Class1
    Public Sub M(x As Object, y As Integer)
        Dim z = x.M(y)'BIND:"x.M(y)"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'x.M(y)') (Parent: VariableInitializer)
  Expression: 
    IDynamicMemberReferenceExpression (Member Name: "M", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: System.Object) (Syntax: SimpleMemberAccessExpression, 'x.M')
      Type Arguments(0)
      Instance Receiver: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
  Arguments(1):
    IParameterReferenceExpression: y ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  ArgumentNames(0)
  ArgumentRefKinds: null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_LateBoundInvocation()
            Dim source = <![CDATA[
Option Strict Off

Class Class1
    Public Sub M(x As Object, y As Integer)
        Dim z = x(y)'BIND:"x(y)"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'x(y)') (Parent: VariableInitializer)
  Expression: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
  Arguments(1):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'y')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: y ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  ArgumentNames(0)
  ArgumentRefKinds: null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_InterpolatedStringExpression()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M(x As String, y As Integer)
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
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'x')
      Alignment: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 20) (Syntax: NumericLiteralExpression, '20')
      FormatString: 
        null
    IInterpolatedStringText ([2] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, ' and ')
      Text: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: " and ") (Syntax: InterpolatedStringText, ' and ')
    IInterpolation ([3] OperationKind.Interpolation) (Syntax: Interpolation, '{y:D3}')
      Expression: 
        IParameterReferenceExpression: y ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
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
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_MidAssignmentStatement()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M(str As String, start As Integer, length As Integer)
        Mid(str, start, length) = str'BIND:"Mid(str, start, length) = str"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: MidAssignmentStatement, 'Mid(str, st ... ngth) = str') (Parent: BlockStatement)
  Expression: 
    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Void) (Syntax: MidAssignmentStatement, 'Mid(str, st ... ngth) = str')
      Left: 
        IParameterReferenceExpression: str ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'str')
      Right: 
        IOperation:  ([1] OperationKind.None, IsImplicit) (Syntax: MidAssignmentStatement, 'Mid(str, st ... ngth) = str')
          Children(4):
            IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.String) (Syntax: MidExpression, 'Mid(str, start, length)')
              Operand: 
                IOperation:  ([0] OperationKind.None, IsImplicit) (Syntax: IdentifierName, 'str')
            IParameterReferenceExpression: start ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'start')
            IParameterReferenceExpression: length ([2] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'length')
            IParameterReferenceExpression: str ([3] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'str')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of AssignmentStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_MisplacedCaseStatement()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M(x As Integer)
        Case x'BIND:"Case x"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: CaseStatement, 'Case x') (Parent: InvalidStatement)
  Children(1):
    ISingleValueCaseClause (CaseKind.SingleValue) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: SimpleCaseClause, 'x')
      Value: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30072: 'Case' can only appear inside a 'Select Case' statement.
        Case x'BIND:"Case x"
        ~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of CaseStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_RedimStatement()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M(x As Integer)
        Dim intArray(10, 10, 10) As Integer
        ReDim intArray(x, x, x)'BIND:"ReDim intArray(x, x, x)"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IOperation:  ([Root] OperationKind.None) (Syntax: ReDimStatement, 'ReDim intArray(x, x, x)') (Parent: )
  Children(1):
    IOperation:  ([0] OperationKind.None) (Syntax: RedimClause, 'intArray(x, x, x)')
      Children(4):
        ILocalReferenceExpression: intArray ([0] OperationKind.LocalReferenceExpression, Type: System.Int32(,,)) (Syntax: IdentifierName, 'intArray')
        IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'x')
          Left: 
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'x')
        IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([2] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'x')
          Left: 
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'x')
        IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([3] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'x')
          Left: 
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'x')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ReDimStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_EraseStatement()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M(x As Integer())
        Erase x'BIND:"Erase x"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IOperation:  ([Root] OperationKind.None) (Syntax: EraseStatement, 'Erase x') (Parent: )
  Children(1):
    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32(), IsImplicit) (Syntax: IdentifierName, 'x')
      Left: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'x')
      Right: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32(), Constant: null, IsImplicit) (Syntax: IdentifierName, 'x')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null, IsImplicit) (Syntax: EraseStatement, 'Erase x')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of EraseStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(Skip:="https://github.com/dotnet/roslyn/issues/19024"), WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_UnstructuredExceptionHandlingStatement()
            Dim source = <![CDATA[
Imports System

Friend Class [Class]
    Public Sub M(x As Integer)'BIND:"Public Sub M(x As Integer)"
        Resume Next
        Console.Write(x)
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBlockStatement (1 statements) (OperationKind.BlockStatement) (Syntax: 'Public Sub  ... End Sub')
  IReturnStatement (OperationKind.ReturnStatement, IsImplicit) (Syntax: 'End Sub')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MethodBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_LateAddressOfOperator()
            Dim source = <![CDATA[
Option Strict Off

Class Class1
    Public Sub M(x As Object)
        Dim y = AddressOf x.Method'BIND:"AddressOf x.Method"
    End Sub
    Public Sub M2(x As Boolean?)

    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, 'AddressOf x.Method') (Parent: VariableInitializer)
  Children(1):
    IDynamicMemberReferenceExpression (Member Name: "Method", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: System.Object) (Syntax: SimpleMemberAccessExpression, 'x.Method')
      Type Arguments(0)
      Instance Receiver: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_NullableIsTrueOperator()
            Dim source = <![CDATA[
Option Strict Off

Class Class1
    Public Sub M(x As Boolean?)
        If x Then'BIND:"If x Then"
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If x Then'B ... End If') (Parent: BlockStatement)
  Condition: 
    IOperation:  ([0] OperationKind.None, IsImplicit) (Syntax: IdentifierName, 'x')
      Children(1):
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of System.Boolean)) (Syntax: IdentifierName, 'x')
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If x Then'B ... End If')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(8884, "https://github.com/dotnet/roslyn/issues/8884")>
        Public Sub ParameterReference_NoPiaObjectCreation()
            Dim sources0 = <compilation>
                               <file name="a.vb"><![CDATA[
Imports System.Runtime.InteropServices
<Assembly: ImportedFromTypeLib("_.dll")>
<Assembly: Guid("f9c2d51d-4f44-45f0-9eda-c9d599b58257")>
<ComImport()>
<Guid("f9c2d51d-4f44-45f0-9eda-c9d599b58277")>
<CoClass(GetType(C))>
Public Interface I
    Property P As Integer
End Interface
<Guid("f9c2d51d-4f44-45f0-9eda-c9d599b58278")>
Public Class C
    Public Sub New(o As Object)
    End Sub
End Class
]]></file>
                           </compilation>
            Dim sources1 = <compilation>
                               <file name="a.vb"><![CDATA[
Structure S
    Function F(x as Object) As I
        Return New I(x)'BIND:"New I(x)"
    End Function
End Structure
]]></file>
                           </compilation>
            Dim compilation0 = CreateCompilationWithMscorlib(sources0)
            compilation0.AssertTheseDiagnostics()

            ' No errors for /r:_.dll
            Dim compilation1 = CreateCompilationWithReferences(
                sources1,
                references:={MscorlibRef, SystemRef, compilation0.EmitToImageReference(embedInteropTypes:=True)})

            Dim expectedOperationTree = <![CDATA[
IOperation:  ([1] OperationKind.None, IsInvalid) (Syntax: ObjectCreationExpression, 'New I(x)') (Parent: InvalidExpression)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30516: Overload resolution failed because no accessible 'New' accepts this number of arguments.
        Return New I(x)'BIND:"New I(x)"
               ~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ObjectCreationExpressionSyntax)(compilation1, "a.vb", expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
