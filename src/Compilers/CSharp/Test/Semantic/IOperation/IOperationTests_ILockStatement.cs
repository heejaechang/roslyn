// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_ObjectLock_FieldReference()
        {
            string source = @"
public class C1
{
    object o = new object();

    public void M()
    {
        /*<bind>*/lock (o)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([0] OperationKind.LockStatement) (Syntax: LockStatement, 'lock (o) ... }') (Parent: BlockStatement)
  Expression: 
    IFieldReferenceExpression: System.Object C1.o ([0] OperationKind.FieldReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C1, IsImplicit) (Syntax: IdentifierName, 'o')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_ObjectLock_LocalReference()
        {
            string source = @"
public class C1
{
    public void M()
    {
        object o = new object();
        /*<bind>*/lock (o)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([1] OperationKind.LockStatement) (Syntax: LockStatement, 'lock (o) ... }') (Parent: BlockStatement)
  Expression: 
    ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_ObjectLock_Null()
        {
            string source = @"
public class C1
{
    public void M()
    {
        /*<bind>*/lock (null)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([0] OperationKind.LockStatement) (Syntax: LockStatement, 'lock (null) ... }') (Parent: BlockStatement)
  Expression: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_ObjectLock_NonReferenceType()
        {
            string source = @"
public class C1
{
    public void M()
    {
        int i = 1;
        /*<bind>*/lock (i)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([1] OperationKind.LockStatement, IsInvalid) (Syntax: LockStatement, 'lock (i) ... }') (Parent: BlockStatement)
  Expression: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'i')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0185: 'int' is not a reference type as required by the lock statement
                //         /*<bind>*/lock (i)
                Diagnostic(ErrorCode.ERR_LockNeedsReference, "i").WithArguments("int").WithLocation(7, 25)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_MissingLockExpression()
        {
            string source = @"
public class C1
{
    public void M()
    {
        /*<bind>*/lock ()
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([0] OperationKind.LockStatement, IsInvalid) (Syntax: LockStatement, 'lock () ... }') (Parent: BlockStatement)
  Expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term ')'
                //         /*<bind>*/lock ()
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, ")").WithArguments(")").WithLocation(6, 25)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_InvalidLockStatement()
        {
            string source = @"
using System;

public class C1
{
    public void M()
    {
        /*<bind>*/lock (invalidReference)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([0] OperationKind.LockStatement, IsInvalid) (Syntax: LockStatement, 'lock (inval ... }') (Parent: BlockStatement)
  Expression: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'invalidReference')
      Children(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0103: The name 'invalidReference' does not exist in the current context
                //         /*<bind>*/lock (invalidReference)
                Diagnostic(ErrorCode.ERR_NameNotInContext, "invalidReference").WithArguments("invalidReference").WithLocation(8, 25)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_MissingBody()
        {
            string source = @"
public class C1
{
    public void M()
    {
        object o = new object();
        /*<bind>*/lock (o)
/*</bind>*/    }
}
";
            string expectedOperationTree = @"
ILockStatement ([1] OperationKind.LockStatement, IsInvalid) (Syntax: LockStatement, 'lock (o)
') (Parent: BlockStatement)
  Expression: 
    ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
  Body: 
    IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, '')
      Expression: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null) (Syntax: IdentifierName, '')
          Children(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1525: Invalid expression term '}'
                //         /*<bind>*/lock (o)
                Diagnostic(ErrorCode.ERR_InvalidExprTerm, "").WithArguments("}").WithLocation(7, 27),
                // CS1002: ; expected
                //         /*<bind>*/lock (o)
                Diagnostic(ErrorCode.ERR_SemicolonExpected, "").WithLocation(7, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_ExpressionLock_ObjectMethodCall()
        {
            string source = @"
public class C1
{
    public void M()
    {
        object o = new object();
        /*<bind>*/lock (o.ToString())
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([1] OperationKind.LockStatement) (Syntax: LockStatement, 'lock (o.ToS ... }') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression (virtual System.String System.Object.ToString()) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'o.ToString()')
      Instance Receiver: 
        ILocalReferenceExpression: o ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
      Arguments(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_ExpressionLock_ClassMethodCall()
        {
            string source = @"
public class C1
{
    public void M()
    {
        /*<bind>*/lock (M2())
        {
        }/*</bind>*/
    }

    public object M2()
    {
        return new object();
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([0] OperationKind.LockStatement) (Syntax: LockStatement, 'lock (M2()) ... }') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression ( System.Object C1.M2()) ([0] OperationKind.InvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'M2()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C1, IsImplicit) (Syntax: IdentifierName, 'M2')
      Arguments(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_ExpressionCall_VoidMethodCall()
        {
            string source = @"
public class C1
{
    public void M()
    {
        /*<bind>*/lock (M2())
        {
        }/*</bind>*/
    }

    public void M2() { }
}
";
            string expectedOperationTree = @"
ILockStatement ([0] OperationKind.LockStatement, IsInvalid) (Syntax: LockStatement, 'lock (M2()) ... }') (Parent: BlockStatement)
  Expression: 
    IInvocationExpression ( void C1.M2()) ([0] OperationKind.InvocationExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'M2()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C1, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M2')
      Arguments(0)
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0185: 'void' is not a reference type as required by the lock statement
                //         /*<bind>*/lock (M2())
                Diagnostic(ErrorCode.ERR_LockNeedsReference, "M2()").WithArguments("void").WithLocation(6, 25)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILockStatement_NonEmptybody()
        {
            string source = @"
using System;

public class C1
{
    public void M()
    {
        /*<bind>*/lock (new object())
        {
            Console.WriteLine(""Hello World!"");
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ILockStatement ([0] OperationKind.LockStatement) (Syntax: LockStatement, 'lock (new o ... }') (Parent: BlockStatement)
  Expression: 
    IObjectCreationExpression (Constructor: System.Object..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Object) (Syntax: ObjectCreationExpression, 'new object()')
      Arguments(0)
      Initializer: 
        null
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... o World!"");')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... lo World!"")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '""Hello World!""')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""Hello World!"") (Syntax: StringLiteralExpression, '""Hello World!""')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LockStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
