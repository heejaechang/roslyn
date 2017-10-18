// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [Fact]
        [CompilerTrait(CompilerFeature.IOperation)]
        public void FixedStatement_FixedClassVariableAndPrint()
        {
            string source = @"
using System;

class C
{
    private int i;

    void M1()
    {
        unsafe
        {
            /*<bind>*/fixed(int *p = &i)
            {
                Console.WriteLine($""P is {*p}"");
            }/*</bind>*/
        }
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: FixedStatement, 'fixed(int * ... }') (Parent: BlockStatement)
  Children(2):
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int *p = &i')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p = &i')
        Variables: Local_1: System.Int32* p
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &i')
            IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&i')
              Children(1):
                IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&i')
                  Reference: 
                    IFieldReferenceExpression: System.Int32 C.i ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                      Instance Receiver: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'i')
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ...  is {*p}"");')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... P is {*p}"")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '$""P is {*p}""')
                IInterpolatedStringExpression ([0] OperationKind.InterpolatedStringExpression, Type: System.String) (Syntax: InterpolatedStringExpression, '$""P is {*p}""')
                  Parts(2):
                    IInterpolatedStringText ([0] OperationKind.InterpolatedStringText) (Syntax: InterpolatedStringText, 'P is ')
                      Text: 
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""P is "") (Syntax: InterpolatedStringText, 'P is ')
                    IInterpolation ([1] OperationKind.Interpolation) (Syntax: Interpolation, '{*p}')
                      Expression: 
                        IOperation:  ([0] OperationKind.None) (Syntax: PointerIndirectionExpression, '*p')
                          Children(1):
                            ILocalReferenceExpression: p ([0] OperationKind.LocalReferenceExpression, Type: System.Int32*) (Syntax: IdentifierName, 'p')
                      Alignment: 
                        null
                      FormatString: 
                        null
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<FixedStatementSyntax>(source, expectedOperationTree, expectedDiagnostics,
                compilationOptions: TestOptions.UnsafeDebugDll);
        }

        [Fact]
        [CompilerTrait(CompilerFeature.IOperation)]
        public void FixedStatement_MultipleDeclarators()
        {
            string source = @"
using System;

class C
{
    private int i1;
    private int i2;

    void M1()
    {
        int i3;
        unsafe
        {
            /*<bind>*/fixed (int* p1 = &i1, p2 = &i2)
            {
                i3 = *p1 + *p2;
            }/*</bind>*/
        }
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: FixedStatement, 'fixed (int* ... }') (Parent: BlockStatement)
  Children(2):
    IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int* p1 = &i1, p2 = &i2')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = &i1')
        Variables: Local_1: System.Int32* p1
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &i1')
            IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&i1')
              Children(1):
                IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&i1')
                  Reference: 
                    IFieldReferenceExpression: System.Int32 C.i1 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i1')
                      Instance Receiver: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'i1')
      IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p2 = &i2')
        Variables: Local_1: System.Int32* p2
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &i2')
            IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&i2')
              Children(1):
                IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&i2')
                  Reference: 
                    IFieldReferenceExpression: System.Int32 C.i2 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i2')
                      Instance Receiver: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'i2')
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i3 = *p1 + *p2;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i3 = *p1 + *p2')
            Left: 
              ILocalReferenceExpression: i3 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i3')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, '*p1 + *p2')
                Left: 
                  IOperation:  ([0] OperationKind.None) (Syntax: PointerIndirectionExpression, '*p1')
                    Children(1):
                      ILocalReferenceExpression: p1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32*) (Syntax: IdentifierName, 'p1')
                Right: 
                  IOperation:  ([1] OperationKind.None) (Syntax: PointerIndirectionExpression, '*p2')
                    Children(1):
                      ILocalReferenceExpression: p2 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32*) (Syntax: IdentifierName, 'p2')
";

            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<FixedStatementSyntax>(source, expectedOperationTree, expectedDiagnostics,
                compilationOptions: TestOptions.UnsafeDebugDll);
        }


        [Fact]
        [CompilerTrait(CompilerFeature.IOperation)]
        public void FixedStatement_MultipleFixedStatements()
        {
            string source = @"
using System;

class C
{
    private int i1;
    private int i2;

    void M1()
    {
        int i3;
        unsafe
        {
            /*<bind>*/fixed (int* p1 = &i1)
            fixed (int* p2 = &i2)
            {
                i3 = *p1 + *p2;
            }/*</bind>*/
        }
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: FixedStatement, 'fixed (int* ... }') (Parent: BlockStatement)
  Children(2):
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int* p1 = &i1')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = &i1')
        Variables: Local_1: System.Int32* p1
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &i1')
            IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&i1')
              Children(1):
                IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&i1')
                  Reference: 
                    IFieldReferenceExpression: System.Int32 C.i1 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i1')
                      Instance Receiver: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'i1')
    IOperation:  ([1] OperationKind.None) (Syntax: FixedStatement, 'fixed (int* ... }')
      Children(2):
        IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int* p2 = &i2')
          IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p2 = &i2')
            Variables: Local_1: System.Int32* p2
            Initializer: 
              IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &i2')
                IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&i2')
                  Children(1):
                    IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&i2')
                      Reference: 
                        IFieldReferenceExpression: System.Int32 C.i2 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i2')
                          Instance Receiver: 
                            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'i2')
        IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
          IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i3 = *p1 + *p2;')
            Expression: 
              ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i3 = *p1 + *p2')
                Left: 
                  ILocalReferenceExpression: i3 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i3')
                Right: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, '*p1 + *p2')
                    Left: 
                      IOperation:  ([0] OperationKind.None) (Syntax: PointerIndirectionExpression, '*p1')
                        Children(1):
                          ILocalReferenceExpression: p1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32*) (Syntax: IdentifierName, 'p1')
                    Right: 
                      IOperation:  ([1] OperationKind.None) (Syntax: PointerIndirectionExpression, '*p2')
                        Children(1):
                          ILocalReferenceExpression: p2 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32*) (Syntax: IdentifierName, 'p2')
";

            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<FixedStatementSyntax>(source, expectedOperationTree, expectedDiagnostics,
                compilationOptions: TestOptions.UnsafeDebugDll);
        }

        [Fact]
        [CompilerTrait(CompilerFeature.IOperation)]
        public void FixedStatement_InvalidVariable()
        {
            string source = @"
using System;

class C
{
    void M1()
    {
        int i3;
        unsafe
        {
            /*<bind>*/fixed (int* p1 =)
            {
                i3 = *p1;
            }/*</bind>*/
        }
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: FixedStatement, 'fixed (int* ... }') (Parent: BlockStatement)
  Children(2):
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'int* p1 =')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'p1 =')
        Variables: Local_1: System.Int32* p1
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '=')
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
              Children(0)
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i3 = *p1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i3 = *p1')
            Left: 
              ILocalReferenceExpression: i3 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i3')
            Right: 
              IOperation:  ([1] OperationKind.None) (Syntax: PointerIndirectionExpression, '*p1')
                Children(1):
                  ILocalReferenceExpression: p1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32*) (Syntax: IdentifierName, 'p1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ')'
                //             /*<bind>*/fixed (int* p1 =)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(11, 39),
            };

            VerifyOperationTreeAndDiagnosticsForTest<FixedStatementSyntax>(source, expectedOperationTree, expectedDiagnostics,
                compilationOptions: TestOptions.UnsafeDebugDll);
        }

        [Fact]
        [CompilerTrait(CompilerFeature.IOperation)]
        public void FixedStatement_InvalidBody()
        {
            string source = @"
using System;

class C
{
    private int i1;

    void M1()
    {
        int i3;
        unsafe
        {
            /*<bind>*/fixed (int* p1 = &i1)
            {
                i3 = &p1;
            }/*</bind>*/
        }
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: FixedStatement, 'fixed (int* ... }') (Parent: BlockStatement)
  Children(2):
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'int* p1 = &i1')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'p1 = &i1')
        Variables: Local_1: System.Int32* p1
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= &i1')
            IOperation:  ([0] OperationKind.None) (Syntax: AddressOfExpression, '&i1')
              Children(1):
                IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32*) (Syntax: AddressOfExpression, '&i1')
                  Reference: 
                    IFieldReferenceExpression: System.Int32 C.i1 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i1')
                      Instance Receiver: 
                        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'i1')
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, 'i3 = &p1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32, IsInvalid) (Syntax: SimpleAssignmentExpression, 'i3 = &p1')
            Left: 
              ILocalReferenceExpression: i3 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i3')
            Right: 
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: AddressOfExpression, '&p1')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  IAddressOfExpression ([0] OperationKind.AddressOfExpression, Type: System.Int32**, IsInvalid) (Syntax: AddressOfExpression, '&p1')
                    Reference: 
                      ILocalReferenceExpression: p1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32*, IsInvalid) (Syntax: IdentifierName, 'p1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(15,22): error CS0266: Cannot implicitly convert type 'int**' to 'int'. An explicit conversion exists (are you missing a cast?)
                //                 i3 = &p1;
                Diagnostic(ErrorCode.ERR_NoImplicitConvCast, "&p1").WithArguments("int**", "int").WithLocation(15, 22)
            };

            VerifyOperationTreeAndDiagnosticsForTest<FixedStatementSyntax>(source, expectedOperationTree, expectedDiagnostics,
                compilationOptions: TestOptions.UnsafeDebugDll);
        }
    }
}
