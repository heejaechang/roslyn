﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_SimpleUsingNewVariable()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        /*<bind>*/using (var c = new C())
        {
            Console.WriteLine(c.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([0] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (var  ... }') (Parent: BlockStatement)
  Resources: 
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'var c = new C()')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c = new C()')
        Variables: Local_1: C c
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
            IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
              Arguments(0)
              Initializer: 
                null
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c.ToString()')
                IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c.ToString()')
                  Instance Receiver: 
                    ILocalReferenceExpression: c ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
                  Arguments(0)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_MultipleNewVariable()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        
        /*<bind>*/using (C c1 = new C(), c2 = new C())
        {
            Console.WriteLine(c1.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([0] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (C c1 ... }') (Parent: BlockStatement)
  Resources: 
    IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'C c1 = new  ... 2 = new C()')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c1 = new C()')
        Variables: Local_1: C c1
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
            IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
              Arguments(0)
              Initializer: 
                null
      IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c2 = new C()')
        Variables: Local_1: C c2
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
            IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
              Arguments(0)
              Initializer: 
                null
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c1.ToString()')
                IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c1.ToString()')
                  Instance Receiver: 
                    ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                  Arguments(0)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_SimpleUsingStatementExistingResource()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        var c = new C();
        /*<bind>*/using (c)
        {
            Console.WriteLine(c.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([1] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (c) ... }') (Parent: BlockStatement)
  Resources: 
    ILocalReferenceExpression: c ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c.ToString()')
                IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c.ToString()')
                  Instance Receiver: 
                    ILocalReferenceExpression: c ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
                  Arguments(0)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_NestedUsingNewResources()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        /*<bind>*/using (var c1 = new C())
        using (var c2 = new C())
        {
            Console.WriteLine(c1.ToString() + c2.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([0] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (var  ... }') (Parent: BlockStatement)
  Resources: 
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'var c1 = new C()')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c1 = new C()')
        Variables: Local_1: C c1
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
            IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
              Arguments(0)
              Initializer: 
                null
  Body: 
    IUsingStatement ([1] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (var  ... }')
      Resources: 
        IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'var c2 = new C()')
          IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c2 = new C()')
            Variables: Local_1: C c2
            Initializer: 
              IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
                IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
                  Arguments(0)
                  Initializer: 
                    null
      Body: 
        IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
          IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
            Expression: 
              IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
                Instance Receiver: 
                  null
                Arguments(1):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c1.ToString ... .ToString()')
                    IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: AddExpression, 'c1.ToString ... .ToString()')
                      Left: 
                        IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c1.ToString()')
                          Instance Receiver: 
                            ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                          Arguments(0)
                      Right: 
                        IInvocationExpression (virtual System.String System.Object.ToString()) ([1] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c2.ToString()')
                          Instance Receiver: 
                            ILocalReferenceExpression: c2 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
                          Arguments(0)
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_NestedUsingExistingResources()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        var c1 = new C();
        var c2 = new C();
        /*<bind>*/using (c1)
        using (c2)
        {
            Console.WriteLine(c1.ToString() + c2.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([2] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (c1) ... }') (Parent: BlockStatement)
  Resources: 
    ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
  Body: 
    IUsingStatement ([1] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (c2) ... }')
      Resources: 
        ILocalReferenceExpression: c2 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
      Body: 
        IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
          IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
            Expression: 
              IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
                Instance Receiver: 
                  null
                Arguments(1):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c1.ToString ... .ToString()')
                    IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: AddExpression, 'c1.ToString ... .ToString()')
                      Left: 
                        IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c1.ToString()')
                          Instance Receiver: 
                            ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                          Arguments(0)
                      Right: 
                        IInvocationExpression (virtual System.String System.Object.ToString()) ([1] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c2.ToString()')
                          Instance Receiver: 
                            ILocalReferenceExpression: c2 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
                          Arguments(0)
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_InvalidMultipleVariableDeclaration()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        /*<bind>*/using (var c1 = new C(), c2 = new C())
        {
            Console.WriteLine(c1.ToString() + c2.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([0] OperationKind.UsingStatement, IsInvalid) (Syntax: UsingStatement, 'using (var  ... }') (Parent: BlockStatement)
  Resources: 
    IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'var c1 = ne ... 2 = new C()')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'c1 = new C()')
        Variables: Local_1: C c1
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= new C()')
            IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C, IsInvalid) (Syntax: ObjectCreationExpression, 'new C()')
              Arguments(0)
              Initializer: 
                null
      IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'c2 = new C()')
        Variables: Local_1: C c2
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= new C()')
            IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C, IsInvalid) (Syntax: ObjectCreationExpression, 'new C()')
              Arguments(0)
              Initializer: 
                null
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c1.ToString ... .ToString()')
                IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: AddExpression, 'c1.ToString ... .ToString()')
                  Left: 
                    IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c1.ToString()')
                      Instance Receiver: 
                        ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                      Arguments(0)
                  Right: 
                    IInvocationExpression (virtual System.String System.Object.ToString()) ([1] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c2.ToString()')
                      Instance Receiver: 
                        ILocalReferenceExpression: c2 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
                      Arguments(0)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0819: Implicitly-typed variables cannot have multiple declarators
                //         /*<bind>*/using (var c1 = new C(), c2 = new C())
                Diagnostic(ErrorCode.ERR_ImplicitlyTypedVariableMultipleDeclarator, "var c1 = new C(), c2 = new C()").WithLocation(12, 26)
            };

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IOperationTests_MultipleExistingResourcesPassed()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    /*<bind>*/{
        var c1 = new C();
        var c2 = new C();
        using (c1, c2)
        {
            Console.WriteLine(c1.ToString() + c2.ToString());
        }
    }/*</bind>*/
}
";
            // Capturing the whole block here, to show that the using statement is actually being bound as a using statement, followed by
            // an expression and a separate block, rather than being bound as a using statement with an invalid expression as the resources
            string expectedOperationTree = @"
IBlockStatement (5 statements, 2 locals) ([Root] OperationKind.BlockStatement, IsInvalid) (Syntax: Block, '{ ... }') (Parent: )
  Locals: Local_1: C c1
    Local_2: C c2
  IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var c1 = new C();')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c1 = new C()')
      Variables: Local_1: C c1
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
          IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
            Arguments(0)
            Initializer: 
              null
  IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var c2 = new C();')
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c2 = new C()')
      Variables: Local_1: C c2
      Initializer: 
        IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
          IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
            Arguments(0)
            Initializer: 
              null
  IUsingStatement ([2] OperationKind.UsingStatement, IsInvalid) (Syntax: UsingStatement, 'using (c1')
    Resources: 
      ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C, IsInvalid) (Syntax: IdentifierName, 'c1')
    Body: 
      IExpressionStatement ([1] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, '')
        Expression: 
          IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
            Children(0)
  IExpressionStatement ([3] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, 'c2')
    Expression: 
      ILocalReferenceExpression: c2 ([0] OperationKind.LocalReferenceExpression, Type: C, IsInvalid) (Syntax: IdentifierName, 'c2')
  IBlockStatement (1 statements) ([4] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
    IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
      Expression: 
        IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
          Instance Receiver: 
            null
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c1.ToString ... .ToString()')
              IBinaryOperatorExpression (BinaryOperatorKind.Add) ([0] OperationKind.BinaryOperatorExpression, Type: System.String) (Syntax: AddExpression, 'c1.ToString ... .ToString()')
                Left: 
                  IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c1.ToString()')
                    Instance Receiver: 
                      ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                    Arguments(0)
                Right: 
                  IInvocationExpression (virtual System.String System.Object.ToString()) ([1] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c2.ToString()')
                    Instance Receiver: 
                      ILocalReferenceExpression: c2 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c2')
                    Arguments(0)
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1026: ) expected
                //         using (c1, c2)
                Diagnostic(ErrorCode.ERR_CloseParenExpected, ",").WithLocation(14, 18),
                // CS1525: Invalid expression term ','
                //         using (c1, c2)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ",").WithArguments(",").WithLocation(14, 18),
                // CS1002: ; expected
                //         using (c1, c2)
                Diagnostic(ErrorCode.ERR_SemicolonExpected, ",").WithLocation(14, 18),
                // CS1513: } expected
                //         using (c1, c2)
                Diagnostic(ErrorCode.ERR_RbraceExpected, ",").WithLocation(14, 18),
                // CS1002: ; expected
                //         using (c1, c2)
                Diagnostic(ErrorCode.ERR_SemicolonExpected, ")").WithLocation(14, 22),
                // CS1513: } expected
                //         using (c1, c2)
                Diagnostic(ErrorCode.ERR_RbraceExpected, ")").WithLocation(14, 22)
            };

            VerifyOperationTreeAndDiagnosticsForTest<BlockSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_InvalidNonDisposableNewResource()
        {
            string source = @"
using System;

class C
{

    public static void M1()
    {
        /*<bind>*/using (var c1 = new C())
        {
            Console.WriteLine(c1.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([0] OperationKind.UsingStatement, IsInvalid) (Syntax: UsingStatement, 'using (var  ... }') (Parent: BlockStatement)
  Resources: 
    IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: VariableDeclaration, 'var c1 = new C()')
      IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 'c1 = new C()')
        Variables: Local_1: C c1
        Initializer: 
          IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= new C()')
            IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C, IsInvalid) (Syntax: ObjectCreationExpression, 'new C()')
              Arguments(0)
              Initializer: 
                null
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c1.ToString()')
                IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c1.ToString()')
                  Instance Receiver: 
                    ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                  Arguments(0)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1674: 'C': type used in a using statement must be implicitly convertible to 'System.IDisposable'
                //         /*<bind>*/using (var c1 = new C())
                Diagnostic(ErrorCode.ERR_NoConvToIDisp, "var c1 = new C()").WithArguments("C").WithLocation(9, 26)
            };

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_InvalidNonDisposableExistingResource()
        {
            string source = @"
using System;

class C
{

    public static void M1()
    {
        var c1 = new C();
        /*<bind>*/using (c1)
        {
            Console.WriteLine(c1.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([1] OperationKind.UsingStatement, IsInvalid) (Syntax: UsingStatement, 'using (c1) ... }') (Parent: BlockStatement)
  Resources: 
    ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C, IsInvalid) (Syntax: IdentifierName, 'c1')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c1.ToString()')
                IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c1.ToString()')
                  Instance Receiver: 
                    ILocalReferenceExpression: c1 ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
                  Arguments(0)
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1674: 'C': type used in a using statement must be implicitly convertible to 'System.IDisposable'
                //         /*<bind>*/using (c1)
                Diagnostic(ErrorCode.ERR_NoConvToIDisp, "c1").WithArguments("C").WithLocation(10, 26)
            };

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_InvalidEmptyUsingResources()
        {
            string source = @"
using System;

class C
{

    public static void M1()
    {
        /*<bind>*/using ()
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([0] OperationKind.UsingStatement, IsInvalid) (Syntax: UsingStatement, 'using () ... }') (Parent: BlockStatement)
  Resources: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ')'
                //         /*<bind>*/using ()
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(9, 26)
            };

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_UsingWithoutSavedReference()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        /*<bind>*/using (GetC())
        {
        }/*</bind>*/
    }

    public static C GetC() => new C();
}
";
            string expectedOperationTree = @"
IUsingStatement ([0] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (GetC ... }') (Parent: BlockStatement)
  Resources: 
    IInvocationExpression (C C.GetC()) ([0] OperationKind.InvocationExpression, Type: C) (Syntax: InvocationExpression, 'GetC()')
      Instance Receiver: 
        null
      Arguments(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_DynamicArgument()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        dynamic d = null;
        /*<bind>*/using (d)
        {
            Console.WriteLine(d);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([1] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (d) ... }') (Parent: BlockStatement)
  Resources: 
    ILocalReferenceExpression: d ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(d);')
        Expression: 
          IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'Console.WriteLine(d)')
            Expression: 
              IDynamicMemberReferenceExpression (Member Name: ""WriteLine"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: null) (Syntax: SimpleMemberAccessExpression, 'Console.WriteLine')
                Type Arguments(0)
                Instance Receiver: 
                  IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'Console')
            Arguments(1):
              ILocalReferenceExpression: d ([1] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
            ArgumentNames(0)
            ArgumentRefKinds(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_NullResource()
        {
            string source = @"
using System;

class C
{
    public static void M1()
    {
        /*<bind>*/using (null)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IUsingStatement ([0] OperationKind.UsingStatement) (Syntax: UsingStatement, 'using (null ... }') (Parent: BlockStatement)
  Resources: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<UsingStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_UsingStatementSyntax_Declaration()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        using (/*<bind>*/var c = new C()/*</bind>*/)
        {
            Console.WriteLine(c.ToString());
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: VariableDeclaration, 'var c = new C()') (Parent: UsingStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c = new C()')
    Variables: Local_1: C c
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
        IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
          Arguments(0)
          Initializer: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_UsingStatementSyntax_StatementSyntax()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        using (var c = new C())
        /*<bind>*/{
            Console.WriteLine(c.ToString());
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }') (Parent: UsingStatement)
  IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... oString());')
    Expression: 
      IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... ToString())')
        Instance Receiver: 
          null
        Arguments(1):
          IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'c.ToString()')
            IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'c.ToString()')
              Instance Receiver: 
                ILocalReferenceExpression: c ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
              Arguments(0)
            InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<BlockSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_UsingStatementSyntax_ExpressionSyntax()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        var c = new C();
        using (/*<bind>*/c/*</bind>*/)
        {
            Console.WriteLine(c.ToString());
        }
    }
}
";
            string expectedOperationTree = @"
ILocalReferenceExpression: c ([0] OperationKind.LocalReferenceExpression, Type: C) (Syntax: IdentifierName, 'c') (Parent: UsingStatement)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void IUsingStatement_UsingStatementSyntax_VariableDeclaratorSyntax()
        {
            string source = @"
using System;

class C : IDisposable
{
    public void Dispose()
    {
    }

    public static void M1()
    {
        
        using (C /*<bind>*/c1 = new C()/*</bind>*/, c2 = new C())
        {
            Console.WriteLine(c1.ToString());
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'c1 = new C()') (Parent: VariableDeclarationStatement)
  Variables: Local_1: C c1
  Initializer: 
    IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= new C()')
      IObjectCreationExpression (Constructor: C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C()')
        Arguments(0)
        Initializer: 
          null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<VariableDeclaratorSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
