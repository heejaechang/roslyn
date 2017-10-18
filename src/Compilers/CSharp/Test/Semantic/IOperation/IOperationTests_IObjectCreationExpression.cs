// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")]
        public void ObjectCreationWithMemberInitializers()
        {
            string source = @"
struct B
{
    public bool Field;
}

class F
{
    public int Field;
    public string Property1 { set; get; }
    public B Property2 { set; get; }
}

class C
{
    public void M1()
    /*<bind>*/{
        var x1 = new F();
        var x2 = new F() { Field = 2 };
        var x3 = new F() { Property1 = """" };
        var x4 = new F() { Property1 = """", Field = 2 };
        var x5 = new F() { Property2 = new B { Field = true } };

        var e1 = new F() { Property2 = 1 };
        var e2 = new F() { """" };
    }/*</bind>*/
}
";
            string expectedOperationTree = @"
IBlockStatement (7 statements, 7 locals) ([Root] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ ... }') (Parent: )
  Locals: Local_1: F x1
    Local_2: F x2
    Local_3: F x3
    Local_4: F x4
    Local_5: F x5
    Local_6: F e1
    Local_7: F e2
  IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var x1 = new F();')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x1 = new F()')
      Variables: Local_1: F x1
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new F()')
          IObjectCreationExpression (Constructor: F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'new F()')
            Arguments(0)
            Initializer: 
              null
  IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var x2 = ne ... ield = 2 };')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x2 = new F( ... Field = 2 }')
      Variables: Local_1: F x2
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new F() { Field = 2 }')
          IObjectCreationExpression (Constructor: F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'new F() { Field = 2 }')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectInitializerExpression, '{ Field = 2 }')
                Initializers(1):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'Field = 2')
                    Left: 
                      IFieldReferenceExpression: System.Int32 F.Field ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'Field')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: IdentifierName, 'Field')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IVariableDeclarationStatement (1 declarations) ([2] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var x3 = ne ... ty1 = """" };')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x3 = new F( ... rty1 = """" }')
      Variables: Local_1: F x3
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new F() { ... rty1 = """" }')
          IObjectCreationExpression (Constructor: F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'new F() { P ... rty1 = """" }')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectInitializerExpression, '{ Property1 = """" }')
                Initializers(1):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.String) (Syntax: SimpleAssignmentExpression, 'Property1 = """"')
                    Left: 
                      IPropertyReferenceExpression: System.String F.Property1 { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'Property1')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: IdentifierName, 'Property1')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: """") (Syntax: StringLiteralExpression, '""""')
  IVariableDeclarationStatement (1 declarations) ([3] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var x4 = ne ... ield = 2 };')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x4 = new F( ... Field = 2 }')
      Variables: Local_1: F x4
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new F() { ... Field = 2 }')
          IObjectCreationExpression (Constructor: F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'new F() { P ... Field = 2 }')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectInitializerExpression, '{ Property1 ... Field = 2 }')
                Initializers(2):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.String) (Syntax: SimpleAssignmentExpression, 'Property1 = """"')
                    Left: 
                      IPropertyReferenceExpression: System.String F.Property1 { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'Property1')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: IdentifierName, 'Property1')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.String, Constant: """") (Syntax: StringLiteralExpression, '""""')
                  ISimpleAssignmentExpression ([1] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'Field = 2')
                    Left: 
                      IFieldReferenceExpression: System.Int32 F.Field ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'Field')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: IdentifierName, 'Field')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  IVariableDeclarationStatement (1 declarations) ([4] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var x5 = ne ... = true } };')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x5 = new F( ...  = true } }')
      Variables: Local_1: F x5
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new F() { ...  = true } }')
          IObjectCreationExpression (Constructor: F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F) (Syntax: ObjectCreationExpression, 'new F() { P ...  = true } }')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F) (Syntax: ObjectInitializerExpression, '{ Property2 ...  = true } }')
                Initializers(1):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B) (Syntax: SimpleAssignmentExpression, 'Property2 = ... ld = true }')
                    Left: 
                      IPropertyReferenceExpression: B F.Property2 { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: B) (Syntax: IdentifierName, 'Property2')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: IdentifierName, 'Property2')
                    Right: 
                      IObjectCreationExpression (Constructor: B..ctor()) ([1] OperationKind.ObjectCreationExpression, Type: B) (Syntax: ObjectCreationExpression, 'new B { Field = true }')
                        Arguments(0)
                        Initializer: 
                          IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: B) (Syntax: ObjectInitializerExpression, '{ Field = true }')
                            Initializers(1):
                              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentExpression, 'Field = true')
                                Left: 
                                  IFieldReferenceExpression: System.Boolean B.Field ([0] OperationKind.FieldReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'Field')
                                    Instance Receiver: 
                                      IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: B, IsImplicit) (Syntax: IdentifierName, 'Field')
                                Right: 
                                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'true')
  IVariableDeclarationStatement (1 declarations) ([5] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'var e1 = ne ... rty2 = 1 };')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'e1 = new F( ... erty2 = 1 }')
      Variables: Local_1: F e1
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= new F() { ... erty2 = 1 }')
          IObjectCreationExpression (Constructor: F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F, IsInvalid) (Syntax: ObjectCreationExpression, 'new F() { P ... erty2 = 1 }')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F, IsInvalid) (Syntax: ObjectInitializerExpression, '{ Property2 = 1 }')
                Initializers(1):
                  ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B, IsInvalid) (Syntax: SimpleAssignmentExpression, 'Property2 = 1')
                    Left: 
                      IPropertyReferenceExpression: B F.Property2 { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: B) (Syntax: IdentifierName, 'Property2')
                        Instance Receiver: 
                          IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: F, IsImplicit) (Syntax: IdentifierName, 'Property2')
                    Right: 
                      IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: B, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                        Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                        Operand: 
                          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
  IVariableDeclarationStatement (1 declarations) ([6] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'var e2 = new F() { """" };')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'e2 = new F() { """" }')
      Variables: Local_1: F e2
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= new F() { """" }')
          IObjectCreationExpression (Constructor: F..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: F, IsInvalid) (Syntax: ObjectCreationExpression, 'new F() { """" }')
            Arguments(0)
            Initializer: 
              IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: F, IsInvalid) (Syntax: CollectionInitializerExpression, '{ """" }')
                Initializers(1):
                  IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: StringLiteralExpression, '""""')
                    Children(1):
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: """", IsInvalid) (Syntax: StringLiteralExpression, '""""')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'int' to 'B'
                //         var e1 = new F() { Property2 = 1 };
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "1").WithArguments("int", "B").WithLocation(24, 40),
                // CS1922: Cannot initialize type 'F' with a collection initializer because it does not implement 'System.Collections.IEnumerable'
                //         var e2 = new F() { "" };
                Diagnostic(ErrorCode.ERR_CollectionInitRequiresIEnumerable, @"{ """" }").WithArguments("F").WithLocation(25, 26)
            };

            VerifyOperationTreeAndDiagnosticsForTest<BlockSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")]
        public void ObjectCreationWithCollectionInitializer()
        {
            string source = @"
using System.Collections.Generic;

class C
{
	private readonly int field;
	public void M1(int x)
	{
		int y = 0;
		var x1 = /*<bind>*/new List<int> { x, y, field }/*</bind>*/;
	}
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: System.Collections.Generic.List<System.Int32>..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: ObjectCreationExpression, 'new List<in ...  y, field }') (Parent: VariableInitializer)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: CollectionInitializerExpression, '{ x, y, field }')
      Initializers(3):
        ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'x')
          Arguments(1):
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([1] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'y')
          Arguments(1):
            ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
        ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([2] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'field')
          Arguments(1):
            IFieldReferenceExpression: System.Int32 C.field ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'field')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'field')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0649: Field 'C.field' is never assigned to, and will always have its default value 0
                // 	private readonly int field;
                Diagnostic(ErrorCode.WRN_UnassignedInternalField, "field").WithArguments("C.field", "0").WithLocation(6, 23)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")]
        public void ObjectCreationWithNestedCollectionInitializer()
        {
            string source = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    private readonly int field = 0;
    public void M1(int x)
    {
        int y = 0;
        var x1 = /*<bind>*/new List<List<int>> {
            new[] { x, y }.ToList(),
            new List<int> { field }
        }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: System.Collections.Generic.List<System.Collections.Generic.List<System.Int32>>..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Collections.Generic.List<System.Collections.Generic.List<System.Int32>>) (Syntax: ObjectCreationExpression, 'new List<Li ... }') (Parent: VariableInitializer)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.List<System.Collections.Generic.List<System.Int32>>) (Syntax: CollectionInitializerExpression, '{ ... }')
      Initializers(2):
        ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Collections.Generic.List<System.Int32>>.Add(System.Collections.Generic.List<System.Int32> item)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: InvocationExpression, 'new[] { x, y }.ToList()')
          Arguments(1):
            IInvocationExpression (System.Collections.Generic.List<System.Int32> System.Linq.Enumerable.ToList<System.Int32>(this System.Collections.Generic.IEnumerable<System.Int32> source)) ([0] OperationKind.InvocationExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: InvocationExpression, 'new[] { x, y }.ToList()')
              Instance Receiver: 
                null
              Arguments(1):
                IArgument (ArgumentKind.Explicit, Matching Parameter: source) ([0] OperationKind.Argument, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[] { x, y }')
                  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.Generic.IEnumerable<System.Int32>, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[] { x, y }')
                    Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                    Operand: 
                      IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ImplicitArrayCreationExpression, 'new[] { x, y }')
                        Dimension Sizes(1):
                          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[] { x, y }')
                        Initializer: 
                          IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ x, y }')
                            Element Values(2):
                              IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                              ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Collections.Generic.List<System.Int32>>.Add(System.Collections.Generic.List<System.Int32> item)) (IsDynamic: False) ([1] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: ObjectCreationExpression, 'new List<int> { field }')
          Arguments(1):
            IObjectCreationExpression (Constructor: System.Collections.Generic.List<System.Int32>..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: ObjectCreationExpression, 'new List<int> { field }')
              Arguments(0)
              Initializer: 
                IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: CollectionInitializerExpression, '{ field }')
                  Initializers(1):
                    ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'field')
                      Arguments(1):
                        IFieldReferenceExpression: System.Int32 C.field ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'field')
                          Instance Receiver: 
                            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'field')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")]
        public void ObjectCreationWithMemberAndCollectionInitializers()
        {
            string source = @"
using System.Collections.Generic;

internal class Class
{
    public int X { get; set; }
    public List<int> Y { get; set; }
    public Dictionary<int, int> Z { get; set; }
    public Class C { get; set; }

    private readonly int field = 0;

    public void M(int x)
    {
        int y = 0;
        var c = /*<bind>*/new Class() {
            X = x,
            Y = { x, y, 3 },
            Z = { { x, y } },
            C = { X = field }
        }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: Class..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: Class) (Syntax: ObjectCreationExpression, 'new Class() ... }') (Parent: VariableInitializer)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: Class) (Syntax: ObjectInitializerExpression, '{ ... }')
      Initializers(4):
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'X = x')
          Left: 
            IPropertyReferenceExpression: System.Int32 Class.X { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'X')
          Right: 
            IParameterReferenceExpression: x ([1] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
        IMemberInitializerExpression ([1] OperationKind.MemberInitializerExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: SimpleAssignmentExpression, 'Y = { x, y, 3 }')
          InitializedMember: 
            IPropertyReferenceExpression: System.Collections.Generic.List<System.Int32> Class.Y { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: IdentifierName, 'Y')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'Y')
          Initializer: 
            IObjectOrCollectionInitializerExpression ([1] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.List<System.Int32>) (Syntax: CollectionInitializerExpression, '{ x, y, 3 }')
              Initializers(3):
                ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'x')
                  Arguments(1):
                    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([1] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: IdentifierName, 'y')
                  Arguments(1):
                    ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
                ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.List<System.Int32>.Add(System.Int32 item)) (IsDynamic: False) ([2] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: NumericLiteralExpression, '3')
                  Arguments(1):
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IMemberInitializerExpression ([2] OperationKind.MemberInitializerExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>) (Syntax: SimpleAssignmentExpression, 'Z = { { x, y } }')
          InitializedMember: 
            IPropertyReferenceExpression: System.Collections.Generic.Dictionary<System.Int32, System.Int32> Class.Z { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>) (Syntax: IdentifierName, 'Z')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'Z')
          Initializer: 
            IObjectOrCollectionInitializerExpression ([1] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Collections.Generic.Dictionary<System.Int32, System.Int32>) (Syntax: CollectionInitializerExpression, '{ { x, y } }')
              Initializers(1):
                ICollectionElementInitializerExpression (AddMethod: void System.Collections.Generic.Dictionary<System.Int32, System.Int32>.Add(System.Int32 key, System.Int32 value)) (IsDynamic: False) ([0] OperationKind.CollectionElementInitializerExpression, Type: System.Void, IsImplicit) (Syntax: ComplexElementInitializerExpression, '{ x, y }')
                  Arguments(2):
                    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                    ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
        IMemberInitializerExpression ([3] OperationKind.MemberInitializerExpression, Type: Class) (Syntax: SimpleAssignmentExpression, 'C = { X = field }')
          InitializedMember: 
            IPropertyReferenceExpression: Class Class.C { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: Class) (Syntax: IdentifierName, 'C')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'C')
          Initializer: 
            IObjectOrCollectionInitializerExpression ([1] OperationKind.ObjectOrCollectionInitializerExpression, Type: Class) (Syntax: ObjectInitializerExpression, '{ X = field }')
              Initializers(1):
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'X = field')
                  Left: 
                    IPropertyReferenceExpression: System.Int32 Class.X { get; set; } ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
                      Instance Receiver: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'X')
                  Right: 
                    IFieldReferenceExpression: System.Int32 Class.field ([1] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'field')
                      Instance Receiver: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Class, IsImplicit) (Syntax: IdentifierName, 'field')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17588, "https://github.com/dotnet/roslyn/issues/17588")]
        public void ObjectCreationWithArrayInitializer()
        {
            string source = @"
class C
{
    int[] a;

    static void Main()
    {
        var a = /*<bind>*/new C { a = { [0] = 1, [1] = 2 } }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C { a = ... [1] = 2 } }') (Parent: VariableInitializer)
  Arguments(0)
  Initializer: 
    IObjectOrCollectionInitializerExpression ([0] OperationKind.ObjectOrCollectionInitializerExpression, Type: C) (Syntax: ObjectInitializerExpression, '{ a = { [0] ... [1] = 2 } }')
      Initializers(1):
        IMemberInitializerExpression ([0] OperationKind.MemberInitializerExpression, Type: System.Int32[]) (Syntax: SimpleAssignmentExpression, 'a = { [0] = 1, [1] = 2 }')
          InitializedMember: 
            IFieldReferenceExpression: System.Int32[] C.a ([0] OperationKind.FieldReferenceExpression, Type: System.Int32[]) (Syntax: IdentifierName, 'a')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'a')
          Initializer: 
            IObjectOrCollectionInitializerExpression ([1] OperationKind.ObjectOrCollectionInitializerExpression, Type: System.Int32[]) (Syntax: ObjectInitializerExpression, '{ [0] = 1, [1] = 2 }')
              Initializers(2):
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, '[0] = 1')
                  Left: 
                    IArrayElementReferenceExpression ([0] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: ImplicitElementAccess, '[0]')
                      Array reference: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: System.Int32[], IsImplicit) (Syntax: IdentifierName, 'a')
                      Indices(1):
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                ISimpleAssignmentExpression ([1] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, '[1] = 2')
                  Left: 
                    IArrayElementReferenceExpression ([0] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: ImplicitElementAccess, '[1]')
                      Array reference: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: System.Int32[], IsImplicit) (Syntax: IdentifierName, 'a')
                      Indices(1):
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = new DiagnosticDescription[]
            {
                // warning CS0414: The field 'C.a' is assigned but its value is never used
                //     int[] a;
                Diagnostic(ErrorCode.WRN_UnreferencedFieldAssg, "a").WithArguments("C.a").WithLocation(4, 11)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ObjectCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

    }
}
